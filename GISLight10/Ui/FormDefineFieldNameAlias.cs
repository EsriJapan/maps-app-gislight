using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.FileIO;
using System.IO;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// フィールド名別名の変更
    /// </summary>
    /// <history>
    ///  2011/05/31 新規作成 
    /// </history>
    public partial class FormDefineFieldNameAlias : Form
    {
        private Ui.MainForm mainFrm;
        
        private bool	_blnIsLayer = true;

        // 更新処理時エラーコード
        const int ERROR_EXCEPTION = -1;
        const int ERROR_IS_NULL = -2;
        const int ERROR_IS_SPACE = -3;
        
        // 入力チェック時ステータスコード
        const int IS_INVALID_NULL = -2;
        const int IS_INVALID_SPACE = -3;
        const int IS_ALL_INVISIBLE = -4;
        const int IS_VALID = 1;

        // Csv内容チェック時ステータスコード
        const int IS_INVALID_OTHER = -6;
        const int IS_INVALID_COLUMN_HEADER = -5;
        const int IS_INVALID_COLUMN_COUNT = -2;
        const int IS_INVALID_DUPLICATE_FIELD_NAME = -3;
        const int IS_INVALID_RECORD_NOT_EXIST = -4;
        const int IS_VALID_CSV_CONTENTS = 1;

        // IsValidDefCsvFileで使用する返却値格納用
        private struct ValidateCsvResult
        {
            public int valid_status;
            public string message;
            public int fieldNamePos;
            public int aliasNamePos;
            public int VisiblePos;
        }
        
        // 設定内容
        private struct CSVResult {
			public bool?	Visible;
			public string	Alias;
        }

        // IsValidFieldNameAlisOnGridで使用する返却値格納用
        private struct ValidateEditAliasResult
        {
            public int valid_status;
            public int row_no;
            public string message;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="frm">メインフォームへの参照</param>
		/// <param name="IsFeatureLayer">レイヤ／テーブルの識別</param>
        public FormDefineFieldNameAlias(Ui.MainForm frm, bool IsFeatureLayer)
        {   
            try
            {
                InitializeComponent();
                this.mainFrm = frm;
				
				// 識別設定を保存
				this._blnIsLayer = IsFeatureLayer;
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormDefineFieldNameAlias_ErrorWhenInit + " ComEx");

                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
                this.Close();
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormDefineFieldNameAlias_ErrorWhenInit);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
                this.Close();
            }
        }

        /// <summary>
        /// グリッド初期設定
        /// </summary>
        /// <param name="rowcount"></param>
        private void InitDataGridView(int rowcount)
        {
            this.dataGridViewFieldNameAlias.RowCount = rowcount;

            foreach (DataGridViewColumn c in this.dataGridViewFieldNameAlias.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// 現在の別名をグリッドに表示
        /// </summary>
        private void SetDataGridView()
        {
            
            ITableFields tblFlds;
            
            // 処理の対象を取得
            if(this._blnIsLayer) {
				IFeatureLayer2 featLayer = (IFeatureLayer2)this.mainFrm.SelectedLayer;
				tblFlds = (ITableFields)featLayer;
			}
			else {
				tblFlds = this.mainFrm.SelectedTable as ITableFields;
			}

            InitDataGridView(tblFlds.FieldCount);
            
            for (int i = 0; i <= (tblFlds.FieldCount - 1); i++)
            {
                IField2 field = tblFlds.get_Field(i) as IField2;
                //int tfldidx = tblFlds.FindField(field.Name);
                int tfldidx = i;
                IFieldInfo3 fldinfo = tblFlds.get_FieldInfo(tfldidx) as IFieldInfo3;

				// 表示設定
				dataGridViewFieldNameAlias[0, i].Value = fldinfo.Visible;
				dataGridViewFieldNameAlias[0, i].ReadOnly = false;

                // フィールド名（編集不可）
                dataGridViewFieldNameAlias[1, i].Value = field.Name; // AliasName;
                dataGridViewFieldNameAlias[1, i].ReadOnly = true;

                // 編集可能な別名：
                // この内容を編集、またはｃｓｖから定義を詠み込み変更する
                dataGridViewFieldNameAlias[2, i].Value = fldinfo.Alias;
                dataGridViewFieldNameAlias[2, i].ReadOnly = false;

				//if (field.Type == esriFieldType.esriFieldTypeGeometry/* ||
				//    field.Type == esriFieldType.esriFieldTypeOID*/)
				//{
				//    //dataGridViewFieldNameAlias.Rows[i].Visible = false;
				//    dataGridViewFieldNameAlias.Rows[i].Cells[0].ReadOnly = true;
				//}
            }
            
            // 列幅自動調整
            this.dataGridViewFieldNameAlias.Columns[2].Width = 200;
        }

        /// <summary>
        /// Csv内容チェック
        /// </summary>
        /// <param name="csvfilnm"></param>
        /// <returns></returns>
        private ValidateCsvResult IsValidDefCsvFile(string csvfilnm)
        {
            ValidateCsvResult result = new ValidateCsvResult();
            result.fieldNamePos = -1;
            result.aliasNamePos = -1;

            const int init_status_no = -1;
            int warning_sts_no = init_status_no;
            int cntrd = 0;

            using (TextFieldParser parser =
                new TextFieldParser(
                    csvfilnm, System.Text.Encoding.GetEncoding("Shift_JIS")))
            {
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(",");

                List<string> filedNameList = new List<string>();

                while (true)
                {
                    if (parser.EndOfData)
                    {
                        break;
                    }
                    else
                    {
                        //1行読み込み
                        string[] row = null;
                        try
                        {
                            row = parser.ReadFields();
                        }
                        catch (Exception ex)
                        {
                            Common.Logger.Error(ex.Message);
                            
                            warning_sts_no = IS_INVALID_OTHER;
                            result.message =
                                Properties.Resources.
                                FormDefineFieldNameAlias_ErrorWhenReadCsvFile;
                            return result;
                        }

                        // カラム数は２つ以上でなければＮＧ
                        if (row.Length >= 2)
                        {
                            // ヘッダ判定
                            if (cntrd == 0)
                            {
                                int headerCheckCnt = 0;
                                
                                // ヘッダ内容確認
                                //FieldName,AliasName
                                for (int i = 0; i < row.Length; i++)
                                {
                                    if (row[i].ToLower().Equals("FieldName".ToLower()))
                                    {
                                        if (result.fieldNamePos == -1) // 1つでもあれば可,最初の1つ目を使用
                                        {
                                            headerCheckCnt++;
                                            result.fieldNamePos = i;
                                        }
                                    }
                                    else if (row[i].ToLower().Equals("AliasName".ToLower()))
                                    {
                                        if (result.aliasNamePos == -1) // 1つでもあれば可,最初の1つ目を使用
                                        {
                                            headerCheckCnt++;
                                            result.aliasNamePos = i;
                                        }
                                    }
                                    else if (row[i].ToLower().Equals("Visible".ToLower()))
                                    {
                                        if (result.VisiblePos == -1) // 1つでもあれば可,最初の1つ目を使用
                                        {
                                            headerCheckCnt++;
                                            result.VisiblePos = i;
                                        }
                                    }

                                    if (headerCheckCnt == 3)
                                    {
                                        break;
                                    }
                                    
                                }

                                if (headerCheckCnt != 2) /* 初期版(表示ﾌｨｰﾙﾄﾞなし)をｻﾎﾟｰﾄ */
                                {
                                    warning_sts_no = IS_INVALID_COLUMN_HEADER;
                                    result.message =
                                        Properties.Resources.
                                        FormDefineFieldNameAlias_ErrorCsvFileFieHeaderInValid;
                                    return result;
                                }

                                cntrd++;
                                continue;
                            }

                            if (filedNameList.Count > 0)
                            {
                                if (filedNameList.Contains(row[result.fieldNamePos]))
                                {
                                    // フィールド名重複時は一応警告する
                                    warning_sts_no = IS_INVALID_DUPLICATE_FIELD_NAME;
                                }
                            }
                            filedNameList.Add(row[result.fieldNamePos]);
                        }
                        else
                        {
                            // カラム数が不足、必ず２つ以上のカラムである必要あり
                            // 1行でも不正であれば処理継続不可
                            result.valid_status = IS_INVALID_COLUMN_COUNT;
                            result.message =
                                Properties.Resources.
                                FormDefineFieldNameAlias_ErrorCsvFileColumnCountInValid;
                            return result;
                        }
                    }
                    cntrd++;
                }

                if (cntrd == 0)
                {
                    result.valid_status = IS_INVALID_RECORD_NOT_EXIST;
                    result.message =
                        Properties.Resources.
                        FormDefineFieldNameAlias_ErrorCsvFileRecordNotExist;
                    return result;
                }
            }

            if (warning_sts_no == init_status_no)
            {
                result.valid_status = IS_VALID_CSV_CONTENTS;
                result.message = "";
                return result;
            }
            else
            {
                result.valid_status = warning_sts_no;
                result.message =
                    Properties.Resources.
                    FormDefineFieldNameAlias_WarningCsvFileFieldNameDuplicate;
                return result;
            }
        }

        /// <summary>
        /// csv内に変更別名に対するキーとなるフィールド名が複数存在する場合には
        /// 必ず最初にヒットしたものしか使用されない
        /// </summary>
        /// <param name="csvfilnm"></param>
        /// <param name="itemKey"></param>
        /// <param name="csvinfo"></param>
        /// <returns>itemKeyに合致した定義名の行の変更後定義名</returns>
        private CSVResult GetDefCsvItem(
            string csvfilnm, string itemKey, ValidateCsvResult csvinfo)
        {
            CSVResult	csvRes = new CSVResult() {
				Visible = null,
				Alias = null,
            };
            int cntrd = 0;

            if (itemKey.Length == 0 || itemKey == null)
            {
                return csvRes;
            }

            using (TextFieldParser parser =
                new TextFieldParser(
                    csvfilnm, System.Text.Encoding.GetEncoding("Shift_JIS")))
            {
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(",");
                while (true)
                {
                    if (parser.EndOfData)
                    {
                        break;
                    }
                    else
                    {
                        //1行読み込み
                        string[] row = parser.ReadFields();
                        if (cntrd == 0)
                        {
                            cntrd++;
                            continue;
                        }

                        if (row.Length >= 2)
                        {
                            // カラム数は２つ以上時に対応：１つ目で比較、２つ目を取得
                            // テーブル定義ファイル上でのキーとなるフィールド名には結合時の考慮はないという仕様
                            // 現状テーブル結合されている場合にはフィールド名だけで一致確認をする
                            // 大文字、小文字区別しない
                            if (row[csvinfo.fieldNamePos].ToLower().Equals(itemKey.ToLower()))
                            {
                                // 一致
                                csvRes.Alias = row[csvinfo.aliasNamePos];
                                
                                // 表示設定を取得
                                bool	blnVis;
                                if(bool.TryParse(row[csvinfo.VisiblePos], out blnVis)) {
									csvRes.Visible = blnVis;
								}
								
								return csvRes;
                            }
                        }
                        else
                        {
                            // カラム数が不正、必ず２つのカラム以上は必要
                            break;
                        }
                    }
                    cntrd++;
                }
            }
            return csvRes;
        }

        /// <summary>
        /// テーブル結合されている場合には
        /// フィールド名が”テーブル名．フィールド名”
        /// という形式なので、フィールド名だけ取出す
        /// </summary>
        /// <param name="fldName"></param>
        /// <returns></returns>
        private string GetFieldName(string fldName)
        {
            if (fldName == null || fldName.Length == 0)
            {
                return string.Empty;
            }

            string[] editName = fldName.Split('.');
            if (editName.Length > 0)
            {
                return editName[editName.Length - 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// DataGridView上の定義済みエイリアスと一致する
        /// 行を指定されたｃｓｖファイルから検索
        /// 一致した行の変更別名定義を取得して、DataGridView上の該当定義名に設定する
        /// </summary>
        /// <returns></returns>
        private bool SetCsvDefItemToGridAliasName(string csvpath, ValidateCsvResult csvinfo)
        {
            if (!System.IO.File.Exists(csvpath))
            {
                // 指定ｃｓｖファイルが存在しない
                return false;
            }

            int setcntr = 0;
            for (int i = 0; i <= (dataGridViewFieldNameAlias.RowCount - 1); i++)
            {
                // フィールド名で検索
                string keyFldName = 
                    GetFieldName(dataGridViewFieldNameAlias[1, i].Value.ToString());
                
                if (keyFldName.Equals(string.Empty)) 
                {
                    keyFldName = dataGridViewFieldNameAlias[1, i].Value.ToString();
                }

                CSVResult retDefName = GetDefCsvItem(csvpath, keyFldName, csvinfo);

                //if (!retDefName.Equals(string.Empty) && 
                if (retDefName.Alias != null)
                {
					// 別名
                    dataGridViewFieldNameAlias[2, i].Value = retDefName.Alias;
                    dataGridViewFieldNameAlias[2, i].Style.ForeColor= Color.Blue;
                    dataGridViewFieldNameAlias[2, i].Style.BackColor = Color.White;
                    
                    // 表示
                    if(retDefName.Visible != null) {
						dataGridViewFieldNameAlias[0, i].Value = retDefName.Visible;
                    }
                    setcntr++;
                }
            }
            if (setcntr == 0)
            {
                Common.MessageBoxManager.ShowMessageBoxWarining(
                    this, 
                    Properties.Resources.
                        FormDefineFieldNameAlias_WarningCsvFileFieldNameNotMatch);

            }
            return true;
        }
        
        /// <summary>
        /// グリッド上の別名内容チェック
        /// </summary>
        /// <returns></returns>
        private ValidateEditAliasResult IsValidFieldNameAlisOnGrid()
        {
            ValidateEditAliasResult result = new ValidateEditAliasResult();
            int row_cnt = 0;
            int intVisNum = 0;

            for (row_cnt = 0; 
                row_cnt <= (dataGridViewFieldNameAlias.RowCount - 1); row_cnt++)
            {
				// 表示ﾌｨｰﾙﾄﾞをｶｳﾝﾄ
				object vvalue = dataGridViewFieldNameAlias[0, row_cnt].Value;
				if(vvalue.Equals(true)) {
					++intVisNum;
				}
				
                object tvalue = dataGridViewFieldNameAlias[2, row_cnt].Value;
                
                if (tvalue == null)
                {
                    result.valid_status = IS_INVALID_NULL;
                    result.row_no = row_cnt;
                    result.message = 
                        Properties.Resources.FormDefineFieldNameAlias_ErrorFileldAliasIsNull;
                    return result;
                }

                if (tvalue.ToString().Trim().Length == 0)
                {
                    result.valid_status = IS_INVALID_SPACE;
                    result.row_no = row_cnt;
                    result.message =
                        Properties.Resources.FormDefineFieldNameAlias_ErrorFileldAliasIsSpace;
                    return result;
                }

                dataGridViewFieldNameAlias
                    [1, row_cnt].Style.BackColor = Color.White;

            }

			if(intVisNum > 0) {
				result.valid_status = IS_VALID;
				result.row_no = row_cnt;
				result.message = string.Empty;
			}
			else {
				result.valid_status = IS_ALL_INVISIBLE;
				result.row_no = -1;
				result.message = "表示フィールドを1つ以上設定してください。";
			}
            return result;
        }

        /// <summary>
        /// グリッド上で変更ある場合に別名を更新 ※ﾃｰﾌﾞﾙ結合によって同名ﾌｨｰﾙﾄﾞが複数存在する場合、最後の設定が適用される(APIの問題な為回避できない)
        /// </summary>
        /// <returns></returns>
        private int UpdateFieldNameAlias()
        {
            int update_cntr = 0;
            ITableFields tblFlds;

            // 処理の対象を取得
            if(this._blnIsLayer) {
				IFeatureLayer2 featLayer = (IFeatureLayer2)this.mainFrm.SelectedLayer;
				tblFlds = (ITableFields)featLayer;
			}
			else {
				tblFlds = this.mainFrm.SelectedTable as ITableFields;
			}

            for (int i = 0; i <= (tblFlds.FieldCount - 1); i++)
            {
                IField2 field = tblFlds.get_Field(i) as IField2;
                //int tfldidx = tblFlds.FindField(field.Name);
                int tfldidx = i;
                IFieldInfo3 fldinfo = tblFlds.get_FieldInfo(tfldidx) as IFieldInfo3;

                // 表示設定
                object vvalue = dataGridViewFieldNameAlias[0, i].Value;
                if(!fldinfo.Visible.Equals(vvalue)) {
					fldinfo.Visible = (bool)vvalue;
                }
                
                object tvalue = dataGridViewFieldNameAlias[2, i].Value;

                // 元の別名から変更有るなし判定は止めて、
                // 全て更新する？：既に入力内容チェック済み
                // グリッド表示後に何度ｃｓｖファイル読込直しても、
                // 変更前の内容はレイヤーから取得するので、変更比較は可能
                if (!fldinfo.Alias.Equals(tvalue))
                {
                    if (tvalue != null)
                    {
                        if (tvalue.ToString().Trim().Length > 0)
                        {
                            fldinfo.Alias = tvalue.ToString();
                            //dataGridViewFieldNameAlias[1, i].Style.ForeColor = Color.Black;
                            update_cntr++;
                        }
                        //else 
                        //{
                        //    //fldinfo.Alias = field.AliasName;
                        //        return ERROR_IS_SPACE;
                        //}
                    }
                    //else
                    //{
                    //    //fldinfo.Alias = field.AliasName;
                    //    return ERROR_IS_NULL;
                    //}
                }
            }
            return update_cntr;
        }

        /// <summary>
        /// 別名定義ファイル読み込み、
        /// グリッド上に表示している、
        /// 現状別名にマッチする別名に対して編集可能な別名を設定する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = "";
                ofd.InitialDirectory = "";

                //ofd.Filter =
                //    "フィールド別名定義ファイル (*.csv)|*.csv|" +
                //    "CSV (*.csv)|*.csv";

                ofd.Filter = "CSV (*.csv)|*.csv";

                ofd.FilterIndex = 0;
                ofd.Title = "開く";
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string path = System.IO.Path.GetFileName(ofd.FileName);
                    if (path != null)
                    {
                        this.textFieldNameAliasDefFile.Text = ofd.FileName;
                        this.toolTip1.SetToolTip(this.textFieldNameAliasDefFile, ofd.FileName);	// ﾂｰﾙﾁｯﾌﾟにも表示

                        ValidateCsvResult result = IsValidDefCsvFile(ofd.FileName);

                        if (result.valid_status == IS_INVALID_DUPLICATE_FIELD_NAME)
                        {
                            Common.MessageBoxManager.ShowMessageBoxWarining(this, result.message);
                            // 処理は継続
                        }
                        else if (result.valid_status != IS_VALID_CSV_CONTENTS)
                        {
                            Common.MessageBoxManager.ShowMessageBoxError(this, result.message);
                            // 継続不可
                            return;
                        }

                        bool ret = SetCsvDefItemToGridAliasName(ofd.FileName, result);
                    }
                }
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this,
                    Properties.Resources.
                    FormDefineFieldNameAlias_ErrorWhenOpenCsvFile + " ComEx");

                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this,
                    Properties.Resources.
                    FormDefineFieldNameAlias_ErrorWhenOpenCsvFile);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// キャンセル時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ロード時に現状の別名を読込みグリッドに表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDefineFieldNameAlias_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataGridView();
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this,
                    Properties.Resources.
                    FormDefineFieldNameAlias_ErrorWhenFormLoad + " ComEx");

                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this,
                    Properties.Resources.
                    FormDefineFieldNameAlias_ErrorWhenFormLoad);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// OKボタン時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateEditAliasResult result = IsValidFieldNameAlisOnGrid();
                
                bool doContinue = (result.valid_status == IS_VALID);
                bool showErrMsg = true;

                if (result.valid_status == IS_INVALID_SPACE ||
                    result.valid_status == IS_INVALID_NULL)
                {
                    showErrMsg = false;

                    DialogResult drst = Common.MessageBoxManager.ShowMessageBoxWarining2(
                        this,
                        Properties.Resources.
                        FormDefineFieldNameAlias_WarningAliasNameSpaceOrNull);
                    doContinue = (drst == DialogResult.OK);
                }
                else if(result.valid_status == IS_ALL_INVISIBLE) {
					showErrMsg = false;
					Common.MessageBoxManager.ShowMessageBoxWarining(this, result.message);
                }

                if (doContinue) //result.valid_status == IS_VALID)
                {
                    int ret = UpdateFieldNameAlias();
                    if (ret > 0)
                    {
                        // 更新正常終了
                        this.mainFrm.MainMapChanged = true;
                        this.Close();
                    }
                    else if (ret == 0)
                    {
                        //　更新対象なし
                        // すべて更新する様にすれば
                        // ここはあり得ない
                        this.Close();
                    }
                    else
                    {
                        // 更新異常
                        // ここはあり得ないはず
                        Common.MessageBoxManager.ShowMessageBoxError(
                            this,
                            Properties.Resources.
                            FormDefineFieldNameAlias_ErrorWhenUpdateFileldAlias);
                    }
                }
                else
                {
                    if (showErrMsg)
                    {
                        Common.MessageBoxManager.ShowMessageBoxError(
                            this, result.message);
                    }

                    if (result.row_no >= 0 && result.row_no <= dataGridViewFieldNameAlias.RowCount)
                    {
                        dataGridViewFieldNameAlias
                            [1, result.row_no].Style.BackColor = Color.Red;
                    }
                }
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this,
                    Properties.Resources.
                    FormDefineFieldNameAlias_ErrorWhenUpdateFileldAlias + " ComEx");

                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, 
                    Properties.Resources.
                    FormDefineFieldNameAlias_ErrorWhenUpdateFileldAlias);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

		/// <summary>
		/// 別名定義ファイルを保存します
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_SaveDefFile_Click(object sender, EventArgs e) {
            SaveFileDialog	sfd = new SaveFileDialog();
            sfd.FileName = "";
            sfd.InitialDirectory = "";

            //sfd.Filter =
            //    "フィールド別名定義ファイル (*.csv)|*.csv|" +
            //    "CSV (*.csv)|*.csv";

            sfd.Filter = "CSV (*.csv)|*.csv";

            sfd.FilterIndex = 0;
            sfd.Title = "保存";
            sfd.RestoreDirectory = true;
            sfd.CheckPathExists = true;

            if(sfd.ShowDialog(this) == DialogResult.OK) {
                this.textFieldNameAliasDefFile.Text = sfd.FileName;
                this.toolTip1.SetToolTip(this.textFieldNameAliasDefFile, sfd.FileName);	// ﾂｰﾙﾁｯﾌﾟにも表示

                // CSVに保存
                StringBuilder	sbCSV = new StringBuilder();
                // ﾍｯﾀﾞｰを作成
                sbCSV.AppendFormat("{0},{1},{2}\r\n", "Visible", "FieldName", "AliasName");
                
                // 設定を取得
                string	strQ = "";
                foreach(DataGridViewRow row in this.dataGridViewFieldNameAlias.Rows) {
					for(int intCol=0; intCol < this.dataGridViewFieldNameAlias.ColumnCount; intCol++) {
						strQ = row.Cells[intCol].Value.ToString().Contains(",") ? "\"" : "";
						sbCSV.AppendFormat("{1}{0}{1},", row.Cells[intCol].Value, strQ);
					}
					sbCSV.Length -= 1;
					sbCSV.AppendLine("");
                }
                    
                // CSVﾌｧｲﾙに保存
                using(StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.GetEncoding("shift_jis"))) {
					sw.Write(sbCSV.ToString());

					try {
						sw.Close();
					}
					catch (Exception ex) {
						Common.MessageBoxManager.ShowMessageBoxError(
							this,
							Properties.Resources.
							FormDefineFieldNameAlias_ErrorWhenOpenCsvFile);

						Common.Logger.Error(ex.Message);
						Common.Logger.Error(ex.StackTrace);
					}
                }
            }
        }

		/// <summary>
		/// 「全選択」切り替え
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkBox_AllSelect_CheckedChanged(object sender, EventArgs e) {
			CheckBox	ctlCB = sender as CheckBox;
			
			foreach(DataGridViewRow row in this.dataGridViewFieldNameAlias.Rows) {
				row.Cells[0].Value = ctlCB.Checked;
			}
		}
    }
}
