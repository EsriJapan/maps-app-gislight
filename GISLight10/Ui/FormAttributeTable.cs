using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.ADF;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// 属性テーブルの表示
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成
    ///  2011/11/21 TableWrapper,FieldPropertyDescriptorをプロジェクトから削除の対応 
    ///  2011/04/05 フィールド非表示プロパティ対応 
    ///  2011/04/06 少数点表示桁数指定をGISLight10Settings.xmlから取得
    ///  2011/05/31 エイリアス名表示対応
    /// </history>
    public partial class FormAttributeTable : Form
    {
        private const int PAGE_MAX_ROW_LIMIT = 100000; // １頁最大件数制限(絶対）
        private int PageNo = 1;
        private int CurrentPageNo_Index = -1;

        // 選択セットページ番号
        private int CurrentSelectPageNo_index = 0;

        private List<int> StartOid = new List<int>();
        private const int GET_INIT = 0;
        private const int GET_NEXT = 1;
        private const int GET_PREV = 2;

        private const int GET_SELECTED_DATA = 3;

        private const int SET_GRIDBACKCOLOR = 10;

        private bool NextPrevEnable = false;
        private bool IsExistNextData = false;

        private Common.SubWindowNameClass subwinname = Common.SubWindowNameClass.GetInstance();
        private string thisWinName = null;
        private bool DoRemoveThis = false;

        private ILayer targetLayer = null;
        private Ui.MainForm mainFrm;
        private IStandaloneTable	_agStdTbl = null;

        private bool isGridStateAll = true;

        private Common.OptionSettings settingFile = null;
        private int attributeTableDisplayOIDMax = -1;
        private int attribuetTableDisplayAftDecPntLen = 5;

        private const string ATTRIBUTE_TABLE_DISPLAY_OID_MAX = "1ページあたりの表示最大レコード数";
        private const string FIELD_NAME_USE_ALIAS = "フィールド名の別名使用判定フラグ";
        private bool use_fieldNameAlias = false;

        #region Private Memebers
        //private ESRIJapan.GISLight10.Common.TableWrapper tableWrapper;

        DataTable SelectedFeatureDataTable = null; // 選択フィーチャデータ
        DataTable AllFeatureDataTable = null; // 全てのデータ
        private string OIDFieldName = string.Empty;
        private int OIDFieldColumIndex = 0;　  // 選択フィーチャデータの反転判定に使用する
        private string _strOIDColumnName = "";	// ﾃﾞｰﾀｸﾞﾘｯﾄﾞのOID列名
        private List<int> VisibleFalseColumnIndexs = new List<int>();
        private List<int> DateTimeColumnIndexs = new List<int>();
        private List<int> NumericColumnIndexs = new List<int>();
        private List<int> FloatColumnIndexs = new List<int>();

        /// <summary>
        /// データソースのタイプ
        /// </summary>
        private const string TYPE_SHAPE = "SHAPE";
        private const string TYPE_FGDB = "FGDB";
        private const string TYPE_MDB = "MDB";

        #endregion Private Memebers

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetLayer">処理対象レイヤへの参照</param>
        /// <param name="mainFrm">メインフォームへの参照</param>
        public FormAttributeTable(ILayer targetLayer, Ui.MainForm mainFrm)
        {
            Common.Logger.Debug("属性テーブル表示開始");

            InitializeComponent();
            
            this.targetLayer = targetLayer;
            this.mainFrm = mainFrm;

            if (!InitializeForm())
            {
                this.Close();
            }
        }
        
        public FormAttributeTable(IStandaloneTable targetTable, Ui.MainForm mainFrm)
        {
            Common.Logger.Debug("属性テーブル表示開始");

            InitializeComponent();
            
            this._agStdTbl = targetTable;
            this.mainFrm = mainFrm;

            if (!InitializeForm())
            {
                this.Close();
            }
        }
        
        #region Properties
        /// <summary>
        /// テーブルを取得します
        /// </summary>
        public ITable TargetTable {
			get {
				return TargetDisplayTable.DisplayTable;
			}
        }
        /// <summary>
        /// 表示テーブルを取得します
        /// </summary>
        public IDisplayTable TargetDisplayTable {
			get {
				IDisplayTable	agDispTbl;
				
				if(_agStdTbl == null) {
					agDispTbl = this.targetLayer as IDisplayTable;
				}
				else {
					agDispTbl = _agStdTbl as IDisplayTable;
				}

				return agDispTbl;
			}
        }
        
        /// <summary>
        /// 並び替えられたフィールド情報を取得します
        /// </summary>
        public IFieldInfoSet OrderFields {
			get {
				IOrderedLayerFields	agOLF;
				
				if(_agStdTbl == null) {
					agOLF = this.targetLayer as IOrderedLayerFields;
				}
				else {
					agOLF = _agStdTbl as IOrderedLayerFields;
				}
				
				return agOLF.FieldInfos;
			}
        }
        
        /// <summary>
        /// レコード数を取得します
        /// </summary>
        public int RecordCount {
			get {
				// ﾌｨﾙﾀ設定を取得
				ITableDefinition	agTblDef;
				if(_agStdTbl == null) {
					agTblDef = this.targetLayer as ITableDefinition;
				}
				else {
					agTblDef = _agStdTbl as ITableDefinition;
				}
				// ﾚｺｰﾄﾞ･ｶｳﾝﾄの条件設定
				IQueryFilter		agQFil = new QueryFilterClass() {
					WhereClause = agTblDef.DefinitionExpression
				};
				
				return TargetTable.RowCount(agQFil);
			}
        }
        /// <summary>
        /// 選択レコード群を取得します
        /// </summary>
        public ITableSelection TargetTableSelection {
			get {
				ITableSelection	agTblSel;
				
				if(_agStdTbl == null) {
					agTblSel = this.targetLayer as ITableSelection;
				}
				else {
					agTblSel = _agStdTbl as ITableSelection;
				}
				
				return agTblSel;
			}
        }
        /// <summary>
        /// テーブル名称を取得します
        /// </summary>
        public string TargetTableName {
			get {
				string	strRet = "";

				if(_agStdTbl == null) {
					strRet = this.targetLayer.Name;
				}
				else {
					strRet = _agStdTbl.Name;
				}
				
				return strRet;
			}
        }
        /// <summary>
        /// フィールド情報を取得します
        /// </summary>
        public ITableFields TargetTableFields {
			get {
				ITableFields	agTblFlds;
				
				if(_agStdTbl == null) {
					agTblFlds = this.targetLayer as ITableFields;
				}
				else {
					agTblFlds = _agStdTbl as ITableFields;
				}
				
				return agTblFlds;
			}
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// 初期化
        /// </summary>
        private bool InitializeForm()
        {
            if (ApplicationInitializer.IsUserSettingsExists() == false)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                return false;
            }

            try
            {
                settingFile = new Common.OptionSettings();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return false;
            }

            try
            {
                attributeTableDisplayOIDMax = int.Parse(settingFile.AttributeTableDisplayOIDMax);
                attribuetTableDisplayAftDecPntLen = int.Parse(settingFile.AttributeTableDisplayAftDecPntLength);

                // 2010-12-20 add １頁最大件数制限チェック
                if (attributeTableDisplayOIDMax > PAGE_MAX_ROW_LIMIT)
                {
                    Common.MessageBoxManager.ShowMessageBoxWarining(
                        this,
                        Properties.Resources.FormAttributeTable_Warinig_PageRowMaxLimitOver +
                        "１頁あたりの上限、" + PAGE_MAX_ROW_LIMIT + "件までの処理を行います。");

                    Common.Logger.Warn(
                        Properties.Resources.FormAttributeTable_Warinig_PageRowMaxLimitOver + 
                        ":" + attributeTableDisplayOIDMax);

                    Common.Logger.Warn("１頁あたりの上限、" + PAGE_MAX_ROW_LIMIT + "件までの処理を行います。");

                    attributeTableDisplayOIDMax = PAGE_MAX_ROW_LIMIT;
                }
                // 2010-10-20 end add
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + ATTRIBUTE_TABLE_DISPLAY_OID_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + ATTRIBUTE_TABLE_DISPLAY_OID_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return false;
            }

            // フィールド名別名表示切り替え使用チェック
            try
            {
                use_fieldNameAlias = (settingFile.FieldNmaeUseAlias == "1");
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_USE_ALIAS + " ]"

                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_USE_ALIAS + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                // フィールド名別名表示切り替えスイッチの場合、読み込めない場合でも、
                // 処理の継続をする（別名表示をしないだけ）
            }

            return true;
        }

        /// <summary>
        /// 属性テーブルへの表示データ取得（選択状態）
        /// </summary>
        /// <param name="comReleaser"></param>
        /// <param name="selOID"></param>
        /// <returns></returns>
        private IRow GetRow(
            ESRI.ArcGIS.ADF.ComReleaser comReleaser, string selOID)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            if (selOID.Length > 0)
                queryFilter.WhereClause = selOID;

            IDisplayTable	featClass = this.TargetDisplayTable;
            ICursor	geoFeatCursor = featClass.SearchDisplayTable(queryFilter, false);
            comReleaser.ManageLifetime(geoFeatCursor);

            IRow geoFeat = geoFeatCursor.NextRow();
            return geoFeat;
        }

        /// <summary>
        /// 重複しない名称を返す
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private string GetUniqueFieldName(DataTable dataTable, string fieldName)
        {
            if (dataTable.Columns.IndexOf(fieldName) == -1)
            {
                return fieldName;
            }
            else
            {
                int seq = 0;
                while (true)
                {
                    seq++;
                    if (dataTable.Columns.IndexOf(
                        fieldName + "~" + seq.ToString()) == -1)
                    {
                        return fieldName + "~" + seq.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 選択状態のデータ表示時のフィールド取得
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="dc"></param>
        /// <param name="dataTable"></param>
        private void AddFieldDataGrid(int[] OrderIDs, IFields fields, DataColumn dc, DataTable dataTable) {
            // 2011/04/05 -->
            ITableFields tblFlds = this.TargetTableFields;
            // <--
            
			//for(int i = 0; i <= (fields.FieldCount - 1); i++) {
			//    IField field = fields.get_Field(i);
            for(int i = 0; i < OrderIDs.Length; i++) {
                IField field = fields.get_Field(OrderIDs[i]);

                // 2011/04/05 -->
                //int tfldidx = tblFlds.FindField(gridFieldName);

                IFieldInfo fldinfo = tblFlds.get_FieldInfo(OrderIDs[i]);
				// ﾌｨｰﾙﾄﾞ名を取得
                string gridFieldName = GetUniqueFieldName(dataTable, 
					use_fieldNameAlias && !string.IsNullOrWhiteSpace(fldinfo.Alias) ? fldinfo.Alias : field.Name);

                if(!fldinfo.Visible) {
                    if (field.Type != esriFieldType.esriFieldTypeRaster &&
                        field.Type != esriFieldType.esriFieldTypeBlob &&
                        field.Type != esriFieldType.esriFieldTypeGeometry)
                    {
                        VisibleFalseColumnIndexs.Add(i);
                    }
                }

                switch(field.Type) {
                    case esriFieldType.esriFieldTypeRaster:
                    case esriFieldType.esriFieldTypeBlob:
                        VisibleFalseColumnIndexs.Add(i);
                        break;

                    case esriFieldType.esriFieldTypeGeometry:
                        dc = new DataColumn(gridFieldName);
                        dataTable.Columns.Add(dc);
                        VisibleFalseColumnIndexs.Add(i);
                        break;

                    case esriFieldType.esriFieldTypeDate:
                        dc = new DataColumn(gridFieldName);
                        dc.DataType = System.Type.GetType("System.DateTime");
                        dataTable.Columns.Add(dc);
                        DateTimeColumnIndexs.Add(i);
                        break;

                    case esriFieldType.esriFieldTypeOID:
                        dc = new DataColumn(gridFieldName);
                        dc.DataType = System.Type.GetType("System.Int32");
                        dataTable.Columns.Add(dc);
                        NumericColumnIndexs.Add(i);

                        // 列ﾍｯﾀﾞｰ名を保存
						this._strOIDColumnName = gridFieldName;
                        break;

                    case esriFieldType.esriFieldTypeString:
                        dc = new DataColumn(gridFieldName);
                        dc.DataType = System.Type.GetType("System.String");
                        
                        dataTable.Columns.Add(dc);
                        break;

                    case esriFieldType.esriFieldTypeSingle:
                        dc = new DataColumn(gridFieldName);
                        dc.DataType = System.Type.GetType("System.Single");
                        dataTable.Columns.Add(dc);
                        FloatColumnIndexs.Add(i);
                        break;

                    case esriFieldType.esriFieldTypeDouble:
                        dc = new DataColumn(gridFieldName);
                        dc.DataType = System.Type.GetType("System.Double");
                        dataTable.Columns.Add(dc);
                        FloatColumnIndexs.Add(i);
                        break;

                    case esriFieldType.esriFieldTypeInteger:
                        dc = new DataColumn(gridFieldName);
                        dc.DataType = System.Type.GetType("System.Int32");
                        dataTable.Columns.Add(dc);
                        NumericColumnIndexs.Add(i);
                        break;

                    case esriFieldType.esriFieldTypeSmallInteger:
                        dc = new DataColumn(gridFieldName);
                        dc.DataType = System.Type.GetType("System.Int16");
                        dataTable.Columns.Add(dc);
                        NumericColumnIndexs.Add(i);
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 選択フィーチャのOIDを取得する
        /// </summary>
        /// <returns></returns>
        private int[] GetIDs() {
			List<int>	intOIDs = new List<int>();
			
            ITableSelection featSelection = this.TargetTableSelection;
            if(featSelection.SelectionSet.Count > 0) {
                ISelectionSet	selectionset = featSelection.SelectionSet;
                IEnumIDs		enumIDs = selectionset.IDs;
                int				intOID;
				while((intOID = enumIDs.Next()) >= 0) {
				    intOIDs.Add(intOID);
				}
                
                //enumIDs.Reset();
            }
            
            return intOIDs.ToArray();
        }

        /// <summary>
        /// 非表示対象属性の非表示指定
        /// </summary>
        private void SetVisibleFalseColumns()
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                if (VisibleFalseColumnIndexs != null)
                {
                    for (int i = 0; i < VisibleFalseColumnIndexs.Count; i++)
                    {
                        this.dataGridView1.Columns[VisibleFalseColumnIndexs[i]].Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// is col index data time field
        /// </summary>
        /// <param name="colIdx"></param>
        /// <returns></returns>
        private bool IsDataTimeField(int colIdx)
        {
            for (int i = 0; i < DateTimeColumnIndexs.Count; i++)
            {
                if (DateTimeColumnIndexs[i] == colIdx) return true;
            }
            return false;
        }

        /// <summary>
        /// is col index field numeic type
        /// </summary>
        /// <param name="colIdx"></param>
        /// <returns></returns>
        private bool IsNumericField(int colIdx)
        {
            for (int i = 0; i < NumericColumnIndexs.Count; i++)
            {
                if (NumericColumnIndexs[i] == colIdx) return true;
            }
            return false;
        }

        /// <summary>
        /// is col index field float (single or double) 
        /// </summary>
        /// <param name="colIdx"></param>
        /// <returns></returns>
        private bool IsFloatField(int colIdx)
        {
            for (int i = 0; i < FloatColumnIndexs.Count; i++)
            {
                if (FloatColumnIndexs[i] == colIdx) return true;
            }
            return false;
        }

        /// <summary>
        /// set datetime field format 
        /// </summary>
        private void SetFieldFormat()
        {
            int colidx = 0;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (IsDataTimeField(col.Index))
                {
                    col.DefaultCellStyle.Format = "G";
                }
                else if (IsNumericField(col.Index))
                {
                    col.DefaultCellStyle.Format = "###################0"; // "n"; "n0";
                    
                    col.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }
                else if (IsFloatField(col.Index))
                {
                    StringBuilder strtmp = new StringBuilder();
                    strtmp.Append("###################0.0");

                    for (int i = 0; i < attribuetTableDisplayAftDecPntLen-1; i++)
                    {
                        strtmp.Append("#");
                    }
                    col.DefaultCellStyle.Format = strtmp.ToString();
                    //col.DefaultCellStyle.Format = "###################0.0#####";

                    col.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }

                colidx++;
            }
            
            dataGridView1.Visible = true;
            tableLayoutPanel1.Enabled = true;
            dataGridView1.Focus();

            //Application.DoEvents();
            //System.Threading.Thread.Sleep(1000);

        }

        /// <summary>
        /// 選択フィーチャ分のレコードを反転
        /// </summary>
        private void SetGridBackColor()
        {            
            this.bindingNavigator1.BindingSource = this.bindingSource1;
            this.dataGridView1.DataSource = this.bindingSource1;

            DataGridViewCellStyle cellStyleWhite = new DataGridViewCellStyle();
            DataGridViewCellStyle cellStyleAquamarine = new DataGridViewCellStyle();
            //cellStyleWhite.BackColor = Color.White;
            cellStyleAquamarine.BackColor = Color.Aquamarine;

			// 選択ﾌｨｰﾁｬｰを取得
                int[] intOIDs = GetIDs();
                if(intOIDs.Length > 0) {
                    foreach(DataGridViewRow row in dataGridView1.Rows) {
                        //int ID = enumIDs.Next();
                        
                        //DataRowView viewRow = (DataRowView)row.DataBoundItem;
                        //if (viewRow == null) break;
                        
                        // 現在行のOIDを取得
                        int oid = Convert.ToInt32(row.Cells[this._strOIDColumnName].Value);
                        if(intOIDs.Contains(oid)) {
							row.DefaultCellStyle = cellStyleAquamarine;
                        }
                        else if(row.DefaultCellStyle.BackColor != cellStyleWhite.BackColor) {
							row.DefaultCellStyle = cellStyleWhite;
                        }
                        
                    }
                }
                else
                {
                    //dataGridView1.DefaultCellStyle.BackColor = Color.White;
                    ////dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;

                    //★
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        //row.DefaultCellStyle.BackColor = Color.White;
                        if(row.DefaultCellStyle.BackColor != cellStyleWhite.BackColor) {
							row.DefaultCellStyle = cellStyleWhite;
                        }
                    }
                    
					// ﾃﾞｰﾀ･ｸﾞﾘｯﾄﾞの選択を解除
					this.dataGridView1.ClearSelection();
                }
        }

        /// <summary>
        /// 全てのデータ
        /// 一回の検索件数は
        /// AttributeTableDisplayOIDMaxに指定されるOIDの内容以下の該当データ)
        /// </summary>
        /// <returns></returns>
		private DataTable CreateAllFeatureDataTable(int mode) {
#if DEBUG
			Debug.WriteLine("●グリッド・データ作成");
#endif
			
            using (ESRI.ArcGIS.ADF.ComReleaser comReleaser = new ESRI.ArcGIS.ADF.ComReleaser()) {
                IsExistNextData = false;

                ITable	agTbl = TargetTable;
                OIDFieldName = agTbl.OIDFieldName;
                if(!string.IsNullOrEmpty(OIDFieldName)) {
					OIDFieldColumIndex = agTbl.FindField(OIDFieldName);
                }
                else if(!this.dataGridView1.ReadOnly) {
					// ｲﾝﾃﾞｯｸｽを確認
					IIndexes	agIndexes = agTbl.Indexes;
					if(agIndexes.IndexCount > 0) {
						for(int intCnt=0; intCnt < agIndexes.IndexCount; intCnt++) {
							IIndex	agIndex = agIndexes.get_Index(intCnt);
							if(agIndex.IsUnique) {
								StringBuilder	sbFlds = new StringBuilder();
								for(int intCol=0; intCol < agIndex.Fields.FieldCount; intCol++) {
									sbFlds.AppendFormat("{0},", agIndex.Fields.get_Field(intCol).Name);
								}
								sbFlds.Length -= 1;
								OIDFieldName = sbFlds.ToString();
							}
						}
					}
					if(OIDFieldName == "") {
						this.dataGridView1.ReadOnly = true;
						this.Text += "（読み取り専用）";
					}
                }
                
                // ﾚｺｰﾄﾞの表示範囲を設定
                int intStartRec;
                int	intFinishRec;
                switch (mode) {
                case GET_NEXT:
					intStartRec = (CurrentPageNo_Index + 1) * attributeTableDisplayOIDMax;
                    break;
                case GET_PREV:
					intStartRec = (CurrentPageNo_Index - 1) * attributeTableDisplayOIDMax;
                    break;
                default:
					intStartRec = 0;
					break;
                }
				intFinishRec = intStartRec + attributeTableDisplayOIDMax - 1;

                DataColumn dc = null;
                DataTable dataTable = new DataTable();

                DataRow workRow;
                object[] rowValues = new object[agTbl.Fields.FieldCount];

                IFields fields = agTbl.Fields;
                comReleaser.ManageLifetime(fields);
                
				// ﾌｨｰﾙﾄﾞの並べ替えに対応 (並び順を取得)
				int[]	intFldIDs = FieldManager.GetOrderFieldIndexes(this.TargetTableFields);
                
                // ｸﾞﾘｯﾄﾞの列を作成
                AddFieldDataGrid(intFldIDs, fields, dc, dataTable);

                // 表示ﾚｺｰﾄﾞ･ｶｰｿﾙを取得
                ICursor	agCur = TargetDisplayTable.SearchDisplayTable(null, true);
                comReleaser.ManageLifetime(agCur);

                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();
                
                //★★
                //double limit = Properties.Settings.Default.AttributeTableDisplayMax / 
                //    (Properties.Settings.Default.AttributeTableDisplayMax / 2);
                //double limit = attributeTableDisplayOIDMax /
                //    (attributeTableDisplayOIDMax / 2);

                //Common.Logger.Debug("Begin agRow.Fields.FieldCount Loop");
                // 2012/07/24 バグ修正
                
                // ﾚｺｰﾄﾞ･ｽｷｯﾌﾟ
                int intRec = 0;
                for(; intRec < intStartRec; intRec++) {
					agCur.NextRow();
                }
#if DEBUG
				Debug.WriteLine("●ｽｷｯﾌﾟ行数 : " + intRec.ToString());
#endif
                IRow	 agRow;
                while((agRow = agCur.NextRow()) != null) {
					for(int j = 0; j < intFldIDs.Length; j++) {
					    // 対象ﾌｨｰﾙﾄﾞを取得
					    IField fld = agRow.Fields.get_Field(intFldIDs[j]);
                        if(fld.Type == esriFieldType.esriFieldTypeGeometry) {
                            IGeometry	editShape = (agRow as IFeature).Shape;
                            if(editShape != null) {
                                rowValues[j] = editShape.GeometryType.ToString();
                            }
                            else {
                                rowValues[j] = "<Null>";
                            }
                        }
                        else {
							// 値を取得
							rowValues[j] = agRow.get_Value(intFldIDs[j]);
                        }
                    }

                    // ｸﾞﾘｯﾄﾞ行を作成
                    workRow = dataTable.NewRow();
                    workRow.ItemArray = rowValues;
                    dataTable.Rows.Add(workRow);
                    
                    if(intRec++ >= intFinishRec) {
                        break;
                    }
                }
#if DEBUG
				Debug.WriteLine("●読み込み行数 : " + intRec.ToString());
#endif
                //sw.Stop();
                //Common.Logger.Debug("Ended agRow.Fields.FieldCount Loop");

                IsExistNextData = agRow != null;
                
                this.AllFeatureDataTable = dataTable;

                //Common.Logger.Debug("Ended CreateAllFeatureDataTable");

                return dataTable;
            }
        }
        
        /// <summary>
        /// 属性テーブルへの表示データ取得 (選択状態のデータ)
        /// </summary>
        /// <returns></returns>
        private DataTable CreateSelectedFeatureDataTable()
        {
            using (
                ESRI.ArcGIS.ADF.ComReleaser comReleaser =
                    new ESRI.ArcGIS.ADF.ComReleaser())
            {
                IsExistNextData = false;

                ITable featClass = this.TargetTable;
                OIDFieldName = featClass.OIDFieldName;
                if(!string.IsNullOrEmpty(OIDFieldName)) {
					OIDFieldColumIndex = featClass.FindField(featClass.OIDFieldName);
                }

                IQueryFilter queryFilter = new QueryFilterClass();

                ITableSelection targetFeatureSelection = this.TargetTableSelection;

                DataTable dataTable = new DataTable();

                DataRow workRow;
                object[] rowValues = new object[featClass.Fields.FieldCount];

                IFields fields = featClass.Fields;
                comReleaser.ManageLifetime(fields);

                DataColumn dc = null;

				// ﾌｨｰﾙﾄﾞの並べ替えに対応 (並び順を取得)
				int[]	intFldIDs = FieldManager.GetOrderFieldIndexes(this.TargetTableFields);

                AddFieldDataGrid(intFldIDs, fields, dc, dataTable);

                if (targetFeatureSelection.SelectionSet.Count <= 0)
                {
                    this.buttonNext.Enabled = false;
                    this.buttonPrevious.Enabled = false;

                    return dataTable;
                }

                int startNum = CurrentSelectPageNo_index * attributeTableDisplayOIDMax;
                int endNum = startNum + attributeTableDisplayOIDMax - 1;

                if (targetFeatureSelection.SelectionSet.Count > endNum + 1)
                {
                    this.buttonNext.Enabled = true;
                }
                else
                {
                    this.buttonNext.Enabled = false;
                }

                if (startNum == 0)
                {
                    this.buttonPrevious.Enabled = false;
                }
                else
                {
                    this.buttonPrevious.Enabled = true;
                }


                IEnumIDs enumIDs = targetFeatureSelection.SelectionSet.IDs;
                
                if (enumIDs != null)
                {
              
                    int cnt = startNum;
                    int ID;

                    if (startNum > 0)
                    {
                        for (int i = 0; i < startNum; i++)
                        {
                            ID = enumIDs.Next();
                        }
                    }

                    ID = enumIDs.Next();
                    while (ID != -1)
                    {
                        string where = featClass.OIDFieldName + "=" + ID.ToString();
                        IRow geofeature = this.GetRow(comReleaser, where);
                        if (geofeature != null)
                        {
                            for (int j = 0; j <= geofeature.Fields.FieldCount - 1; j++)
                            {
                                IField fld = geofeature.Fields.get_Field(intFldIDs[j]);
                                if (fld.Type == esriFieldType.esriFieldTypeGeometry)
                                {
                                    IGeometry editShape = (geofeature as IFeature).ShapeCopy;
                                    rowValues[j] = editShape.GeometryType.ToString();
                                    continue;
                                }
                                rowValues[j] = geofeature.get_Value(intFldIDs[j]);
                            }
                            workRow = dataTable.NewRow();
                            workRow.ItemArray = rowValues;
                            dataTable.Rows.Add(workRow);
                        } 

                        cnt++;

                        if (cnt > endNum)
                        {
                            break;
                        }

                        ID = enumIDs.Next();
                    }
                }

                this.SelectedFeatureDataTable = dataTable;
                return dataTable;
            }
        }

        /// <summary>
        /// 全て表示
        /// </summary>
        private void GetAllData(int mode)
        {
            this.bindingSource1.DataSource = null;
            //if (AllFeatureDataTable == null)
            //{
            //this.SetBindingSource();

            // 2010-12-14(tue) del 元処理
            //this.bindingSource1.DataSource = CreateAllFeatureDataTable(mode);

            //　2010-12-14(tue) add
            // 半非同期対応１ 初期化処理以降の場合 
            InitSubThreadProc(mode);
            DoSubThreadSynco(mode);
            DoSubThreadSyncoSetGridBackColor();
            EndSubThreadProc();
            // <--- end add

            //}
            //else
            //{
            //    this.bindingSource1.DataSource = AllFeatureDataTable;
            //}

            SetFieldFormat();
            SetVisibleFalseColumns();

        }

        /// <summary>
        /// 
        /// </summary>
        private void InitSubThreadProc(int mode)
        {
            labelStatus.Visible = true;
            labelStatus.Text = "読込み処理中です。しばらくお待ち下さい。";
            Application.DoEvents();
            // 2012/07/20 表示時間短縮
            //System.Threading.Thread.Sleep(1000);

            tableLayoutPanel1.Enabled = false;
            dataGridView1.Visible = false;

            if (mode == GET_SELECTED_DATA)
            {
                dataGridView1.RowsDefaultCellStyle.BackColor = Color.Aquamarine;
            }
            else
            {
                //dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 半非同期で処理実行
        /// </summary>
        /// <param name="mode"></param>
        private void DoSubThreadSynco(int mode)
        {
            RunAnyProcess dlg_para =
                new RunAnyProcess(execAnyProcess);

            IAsyncResult ar_para =
                this.BeginInvoke(dlg_para, mode);

            progressBar1.Maximum = 100;
            progressBar1.Minimum = 1;
            progressBar1.Step = 3;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Visible = true;
            progressBar1.Enabled = true;

            while (!ar_para.IsCompleted)
            {
                progressBar1.PerformStep();
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    progressBar1.Value = progressBar1.Minimum;
                }
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }

            //SetVisibleFalseColumns();
            //Application.DoEvents();
            //System.Threading.Thread.Sleep(1000);
            //dataGridView1.Visible = true;

        }

        /// <summary>
        /// 半非同期的set grid back color (All Data)
        /// </summary>
        private void DoSubThreadSyncoSetGridBackColor()
        {
            //SetGridBackColor();
            RunAnyProcess dlg_para =
                new RunAnyProcess(execAnyProcess);

            IAsyncResult ar_para =
                this.BeginInvoke(dlg_para, SET_GRIDBACKCOLOR);

            while (!ar_para.IsCompleted)
            {
                progressBar1.PerformStep();
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    progressBar1.Value = progressBar1.Minimum;
                }
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndSubThreadProc()
        {
            progressBar1.Value = progressBar1.Maximum;
            Application.DoEvents();
            System.Threading.Thread.Sleep(100);
            progressBar1.Visible = false;
            labelStatus.Text = "読込み処理終了しました。";

            Application.DoEvents();
            // 2012/07/20 表示時間短縮
            //System.Threading.Thread.Sleep(1000);
            labelStatus.Visible = false;
            labelStatus.Text = "";

            tableLayoutPanel1.Enabled = true;
        }

        /// <summary>
        /// 選択セットだけ表示
        /// </summary>
        private void GetSelectedData()
        {
            this.bindingSource1.DataSource = null; // 2010-12-14(tue) コメント外し
            //★
            //if (this.SelectedFeatureDataTable == null)
            //{
            //    this.bindingSource1.DataSource = CreateSelectedFeatureDataTable();  // del 2010-12-14(tue)コメント
            //}
            //else
            //{
            //    this.bindingSource1.DataSource = this.SelectedFeatureDataTable;
            //}

            //this.bindingNavigator1.BindingSource = this.bindingSource1;
            //this.dataGridView1.DataSource = this.bindingSource1;

            // add 2010-12-14(tue)
            InitSubThreadProc(GET_SELECTED_DATA);
            DoSubThreadSynco(GET_SELECTED_DATA);
            EndSubThreadProc();
            // end add

            // del 2010-12-14
            //dataGridView1.RowsDefaultCellStyle.BackColor = Color.Aquamarine;
            SetFieldFormat();
            SetVisibleFalseColumns();
        }        

        #endregion Private Methods

        private delegate void RunAnyProcess(int mode);
        private void CallbackMethodRunAnyProcess(IAsyncResult ar)
        {
            try
            {
                RunAnyProcess dlgt = (RunAnyProcess)ar.AsyncState;
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }
        private void execAnyProcess(int mode)
        {
            try
            {
                if (mode != SET_GRIDBACKCOLOR)
                {
                    if (mode == GET_SELECTED_DATA)
                    {
                        this.bindingSource1.DataSource = CreateSelectedFeatureDataTable();
                    }
                    else
                    {
                        this.bindingSource1.DataSource = CreateAllFeatureDataTable(mode);
                    }
                }
                else
                {
                    SetGridBackColor();
                }
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        private bool initDoing = false;

        /// <summary>
        /// ロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAttributeTable_Load(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                //SelectionChangedイベントハンドラを追加
                IMap map = mainFrm.axMapControl1.Map;
                ((IActiveViewEvents_Event)map).SelectionChanged
                    += new IActiveViewEvents_SelectionChangedEventHandler(OnActiveViewEventsSelectionChanged);

                StringBuilder str = new StringBuilder(500);
                str.Append("属性: ");
                str.Append(this.TargetTableName);
                this.Text = str.ToString();
                str.Append(" - ");
                str.Append(Properties.Resources.CommonMessage_ApplicationName);
                this.thisWinName = str.ToString();

                if (subwinname.IsContain(this.thisWinName))
                {
                    List<string> winlist = new List<string>();
                    foreach (Form frm in Application.OpenForms)
                    {
                        winlist.Add(frm.Name);
                        if (this.thisWinName.Contains(frm.Text))
                        {
                            DoRemoveThis = false;
                            winlist.Clear();
                            break;
                        }
                    }

                    if (winlist.Count > 0)
                    {
                        DoRemoveThis = true;
                    }

                    this.Close();
                    return;
                }
                else
                {
                    DoRemoveThis = true;
                    subwinname[this.thisWinName] = this;
                }

                initDoing = true;
                // 2012/07/19 表示時間短縮
                //System.Threading.Thread.Sleep(3000);
                Application.DoEvents();
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(
                    comex.Message, comex.StackTrace, comex.ErrorCode.ToString());

                DoRemoveThis = true;
                subwinname[this.thisWinName] = this;

            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex.Message, ex.StackTrace, null);

                DoRemoveThis = true;
                subwinname[this.thisWinName] = this;
            }
            finally
            {
                if (dispMessage)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this, Properties.Resources.FormAttributeTable_ErrorWhenFormLoad);
                }
            }
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAttributeTable_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Common.Logger.Debug("属性テーブル表示終了");
                //if (this.tableWrapper != null) this.tableWrapper.Clear();

                IMap map = mainFrm.axMapControl1.Map;
                IActiveView activeView = (IActiveView)map;

                //黄色ハイライトのクリア用
                activeView.PartialRefresh
                    (esriViewDrawPhase.esriViewForeground, null, activeView.Extent);

                //SelectionChangedイベントハンドラを削除
                //★2010/12/7 コメントアウト
                //((IActiveViewEvents_Event)map).SelectionChanged
                //    -= new IActiveViewEvents_SelectionChangedEventHandler(OnActiveViewEventsSelectionChanged);
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex.Message, ex.StackTrace, null);

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormAttributeTable_ErrorWhenFormClosed);
            }
            finally
            {
                if (DoRemoveThis) subwinname.Remove(this.thisWinName);
                //this.Close();
                this.Dispose();
            }
        }

        /// <summary>
        /// 全てOr 選択セットだけ表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSwitchAllOrSelect_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            Ui.MainForm main = this.mainFrm;
            try
            {
                this.Enabled = false;

                buttonPrevious.Enabled = false;
                buttonNext.Enabled = false;

                this.Enabled = false;
                //if (main != null) main.StartMainFormProgress();

                if (sender.ToString().Contains("全て"))
                {
                    // 初期化
                    CurrentPageNo_Index = -1;
                    //StartOid.Clear();

                    //フラグ：属性テーブルが「全て」画面
                    isGridStateAll = true;

                    IMap map = mainFrm.axMapControl1.Map;
                    IActiveView activeView = (IActiveView)map;
                    activeView.PartialRefresh
                        (esriViewDrawPhase.esriViewForeground, null, activeView.Extent);

                    //AfterDrawイベントハンドラを削除                    
                    ((IActiveViewEvents_Event)map).AfterDraw
                        -= new IActiveViewEvents_AfterDrawEventHandler(OnActiveViewEventsAfterDraw);

                    // 選択ｶﾗｰを元に戻す
                    this.dataGridView1.RowsDefaultCellStyle = new DataGridViewCellStyle();

                    GetAllData(GET_INIT);
                }
                else
                {
                    // 初期化
                    CurrentSelectPageNo_index = 0;

                    //フラグ：属性テーブルが「選択」画面
                    isGridStateAll = false;

                    //AfterDrawイベントハンドラを追加
                    IMap map = mainFrm.axMapControl1.Map;
                    ((IActiveViewEvents_Event)map).AfterDraw 
                        += new IActiveViewEvents_AfterDrawEventHandler(OnActiveViewEventsAfterDraw);
                    
                    // 選択ｶﾗｰをﾊｲﾗｲﾄ
                    this.dataGridView1.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(255, Color.Black);
                    this.dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, Color.Yellow);

                    GetSelectedData();
                    //SetVisibleFalseColumns();

                }
                //SetVisibleFalseColumns();

                // 2010-12-14 add
                SetPageNo(0);
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(
                    comex.Message, comex.StackTrace, comex.ErrorCode.ToString());
            }
            catch (Exception ex)
            {
                dispMessage = true;
                this.Enabled = true;
                Common.UtilityClass.DoOnError(ex.Message, ex.StackTrace, null);
            }
            finally
            {
                this.Enabled = true;
                this.buttonSwitchSelect.Enabled = !sender.ToString().Contains("選択");
                this.buttonSwitchAll.Enabled = !sender.ToString().Contains("全て");

                if (sender.ToString().Contains("全て"))
                {
                    if (NextPrevEnable)
                    {
                        if (this.CurrentPageNo_Index >= 0) buttonPrevious.Enabled =
                            this.buttonSwitchSelect.Enabled;

                        //if (this.bindingSource1.Count >=
                        //    Properties.Settings.Default.AttributeTableDisplayOIDMax ||
                        //    IsExistNextData)
                        if (IsExistNextData)
                            buttonNext.Enabled = this.buttonSwitchSelect.Enabled;
                    }
                }

                //if (main != null) main.EndMainFormProgress();
                if (dispMessage)
                {
                    if (sender.ToString().Contains("全て"))
                    {
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            this, Properties.Resources.
                                FormAttributeTable_ErrorWhenAllData);
                    }
                    else
                    {
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            this, Properties.Resources.
                                FormAttributeTable_ErrorWhenSelectData);
                    }
                }
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// データグリッド上の行選択時に選択セットを更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try {                
                StringBuilder criteria = new StringBuilder();
                if(!string.IsNullOrEmpty(OIDFieldName)) {
					// 選択条件句を構築
					foreach (DataGridViewRow r in dataGridView1.SelectedRows) {
						if(criteria.Length > 0) {
							criteria.Append(" OR ");
						}
						criteria.Append(OIDFieldName);
						criteria.Append(" = ");
						criteria.Append(r.Cells[this._strOIDColumnName].Value.ToString());
					}
                }

                if(criteria.Length > 0) {
                    //グリッドテーブルが「選択」画面の場合                    
                    if (!isGridStateAll)
                    {
                        //選択レコードを黄色にハイライト ※実際にはﾊｲﾗｲﾄされていない()
                        HighLightSelectedRecord_OnGridStateSelect();

                        ////選択レコードのジオメトリを黄色にハイライト (されない )
                        IActiveView activeView = (IActiveView)mainFrm.axMapControl1.Map;
                        activeView.PartialRefresh
                            (esriViewDrawPhase.esriViewForeground, null, activeView.Extent);

                        return;
                    }

                    // 対象のﾚｺｰﾄﾞを選択
                    ITableSelection	agTblSel = TargetTableSelection;
                    agTblSel.Clear();

                    IQueryFilter queryFilter = new QueryFilterClass();
                    if (criteria.Length > 0) queryFilter.WhereClause = criteria.ToString();
                    queryFilter.SubFields = OIDFieldName;
                    
                    if(agTblSel is IFeatureSelection) {
						IFeatureSelection	agFSel = TargetTableSelection as IFeatureSelection;
						agFSel.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
						agFSel.SelectionChanged();
					}
					else {
						agTblSel.SelectRows(queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
						agTblSel.SelectionChanged();
                    }
                    
                    //this.mainFrm.MapControl.ActiveView.Refresh();
                    this.mainFrm.MapControl.ActiveView.PartialRefresh
                        (esriViewDrawPhase.esriViewGeoSelection, null, this.mainFrm.MapControl.ActiveView.Extent);
                    this.mainFrm.SetMapStateChanged();

                    SetGridBackColor();
                    this.SelectedFeatureDataTable = null;
                }
                //選択レコード数のラベル表示を設定
                SetToolStripLabelSelectedCount();
            }
            catch (COMException comex)
            {
                dispMessage= true;
                Common.UtilityClass.DoOnError(comex.Message, comex.StackTrace, comex.ErrorCode.ToString());
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex.Message, ex.StackTrace, null);
            }
            finally {
                this.Enabled = true;
                if(dispMessage) {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this, 
                        Properties.Resources.
                            FormAttributeTable_ErrorWhenDataGridViewSelectionChanged);
                }
            }
        }

        /// <summary>
        /// データグリッドデータエラー時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView1_DataError(object sender,
            DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null)
            {
                Common.UtilityClass.DoOnError(
                    e.Exception.Message, e.Exception.StackTrace, null);
            }
        }

        /// <summary>
        /// 次へ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                if (!buttonNext.Enabled) return;

                buttonPrevious.Enabled = false;
                buttonNext.Enabled = false;
                //buttonSwitchSelect.Enabled = false;

                if (!buttonSwitchAll.Enabled)
                {
                    this.Enabled = false;

                    this.CurrentPageNo_Index++;
                    
                    // 2010-12-14 del
                    //this.bindingSource1.DataSource = CreateAllFeatureDataTable(GET_NEXT);
                    //SetGridBackColor();

                    this.bindingSource1.DataSource = null;
                    this.bindingNavigator1.BindingSource = this.bindingSource1;
                    this.dataGridView1.DataSource = this.bindingSource1;

                    // 2010-12-14 add
                    InitSubThreadProc(GET_NEXT);
                    DoSubThreadSynco(GET_NEXT);
                    DoSubThreadSyncoSetGridBackColor();
                    EndSubThreadProc();
                    // end add
                    
                    SetFieldFormat();
                    SetVisibleFalseColumns();
                    buttonPrevious.Enabled = true;

                }
                else if (!buttonSwitchSelect.Enabled)
                {
                    this.Enabled = false;

                    this.CurrentSelectPageNo_index++;

                    //GetSelectedData();
                    
                    // 2010-12-14 add
                    this.bindingSource1.DataSource = null;

                    this.bindingSource1.DataSource = CreateSelectedFeatureDataTable();
                    dataGridView1.RowsDefaultCellStyle.BackColor = Color.Aquamarine;
                    SetFieldFormat();
                    SetVisibleFalseColumns();

                    //buttonPrevious.Enabled = true;
                }

                // 2010-12-14 add
                SetPageNo(1);
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(
                    comex.Message, comex.StackTrace, comex.ErrorCode.ToString());
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex.Message, ex.StackTrace, null);
            }
            finally
            {
                this.Enabled = true;
                //buttonPrevious.Enabled = true;

                if (!buttonSwitchAll.Enabled)
                {
                    //if (this.bindingSource1.Count >= 
                    //    Properties.Settings.Default.AttributeTableDisplayOIDMax ||
                    //    IsExistNextData)
                    if (IsExistNextData)
                        buttonNext.Enabled = true;

                    //buttonSwitchSelect.Enabled = true;
                }

                if (dispMessage)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this, 
                        Properties.Resources.FormAttributeTable_ErrorWhenButtonNext_Click);
                }
            }
        }

        /// <summary>
        /// 前へ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                if (!buttonPrevious.Enabled) return;

                buttonPrevious.Enabled = false;
                buttonNext.Enabled = false;
                //buttonSwitchSelect.Enabled = false;

                if (!buttonSwitchAll.Enabled)
                {
                    this.Enabled = false;

                    // 2010-12-14 del
                    //this.bindingSource1.DataSource = CreateAllFeatureDataTable(GET_PREV);
                    //SetGridBackColor();

                    this.bindingSource1.DataSource = null;

                    // 2010-12-14 add
                    InitSubThreadProc(GET_PREV);
                    DoSubThreadSynco(GET_PREV);
                    DoSubThreadSyncoSetGridBackColor();
                    EndSubThreadProc();
                    // end add

                    SetFieldFormat();
                    SetVisibleFalseColumns();

                    if (this.CurrentPageNo_Index >= 0)
                    {
                        this.CurrentPageNo_Index--;
                    }
                }
                else if (!buttonSwitchSelect.Enabled)
                {
                    this.CurrentSelectPageNo_index--;

                    //GetSelectedData();

                    this.bindingSource1.DataSource = CreateSelectedFeatureDataTable();
                    dataGridView1.RowsDefaultCellStyle.BackColor = Color.Aquamarine;
                    SetFieldFormat();
                    SetVisibleFalseColumns();

                    //buttonNext.Enabled = true;
                }

                // 2010-12-14 add
                SetPageNo(-1);
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(
                    comex.Message, comex.StackTrace, comex.ErrorCode.ToString());
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex.Message, ex.StackTrace, null);
            }
            finally
            {
                this.Enabled = true;
                //buttonSwitchSelect.Enabled = true;

                if (!buttonSwitchAll.Enabled)
                {
                    if (this.CurrentPageNo_Index >= 0) buttonPrevious.Enabled = true;
                    buttonNext.Enabled = true;
                }

                if (dispMessage)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this, 
                        Properties.Resources.FormAttributeTable_ErrorWhenButtonPrevious_Click);
                }
            }
        }


        /// <summary>
        /// 属性テーブルをダブルクリックした時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int selectedOid = 0;
            IGeoFeatureLayer	geoFeatureLayer = null;
            IFeatureClass		displayFC = null;
            DataGridViewRow		selectedRow = null;
            DataGridViewCell	cell = null;
            IFeature			selectedFeature = null;

            try {
                //レコードが選択されていない場合
                if(dataGridView1.SelectedRows.Count == 0) {
                    return;
                }
                // 単独ﾃｰﾌﾞﾙの場合
				else if(this.targetLayer == null) {
					return;
				}
				
                geoFeatureLayer = (IGeoFeatureLayer)targetLayer;
                displayFC = geoFeatureLayer.DisplayFeatureClass;

                // 対象のﾌｨｰﾁｬｰを取得
                selectedRow = dataGridView1.SelectedRows[0];
                
                // OIDを取得
                cell = selectedRow.Cells[this._strOIDColumnName];
                selectedOid = (int)cell.Value;
                selectedFeature = displayFC.GetFeature(selectedOid);

                //選択レコードのフィーチャにズーム
                ZoomFeature(selectedFeature);
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormAttributeTable_ErrorWhenZoomFeature);
                Common.Logger.Error(Properties.Resources.FormAttributeTable_ErrorWhenZoomFeature);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormAttributeTable_ErrorWhenZoomFeature);
                Common.Logger.Error(Properties.Resources.FormAttributeTable_ErrorWhenZoomFeature);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                //ComReleaser.ReleaseCOMObject(geoFeatureLayer);
                ComReleaser.ReleaseCOMObject(displayFC);
                ComReleaser.ReleaseCOMObject(selectedRow);
                ComReleaser.ReleaseCOMObject(cell);
                ComReleaser.ReleaseCOMObject(selectedFeature);                
            }
        }


        /// <summary>
        /// フィーチャにズームする。
        /// （ポイントの場合はズームしない。）
        /// </summary>
        /// <param name="feature">フィーチャ</param>
        protected void ZoomFeature(IFeature feature)
        {
            IGeometry geometry = feature.Shape;                        
            
            //ポイントの場合
            if (geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                mainFrm.axMapControl1.CenterAt((IPoint)geometry);
            }
            //ポリゴン、ラインの場合
            else
            {
				// ﾌｨｰﾁｬｰの範囲を少し広げて取得
				IEnvelope	agEnv = Common.UtilityClass.ExpandEnvelope(feature.Extent);
                mainFrm.axMapControl1.Extent = agEnv;
                //mainFrm.axMapControl1.MapScale = mainFrm.axMapControl1.MapScale * 1.1;                
            }                                   
        }        

        
        /// <summary>
        /// AfterDrawイベント時に実行される
        /// </summary>
        /// <param name="display">IDisplay</param>
        /// <param name="phase">esriViewDrawPhase</param>
        private void OnActiveViewEventsAfterDraw(IDisplay display, esriViewDrawPhase phase)
        {            
            try
            {
                if (phase == esriViewDrawPhase.esriViewForeground)
                {                    
                    //選択レコードのハイライト
                    HighLightSelectedGeometry_OnGridStateSelect(display);
                }
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormAttributeTable_ErrorWhenHighLightGeometry);
                Common.Logger.Error(Properties.Resources.FormAttributeTable_ErrorWhenHighLightGeometry);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormAttributeTable_ErrorWhenHighLightGeometry);
                Common.Logger.Error(Properties.Resources.FormAttributeTable_ErrorWhenHighLightGeometry);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }            
        }

        /// <summary>
        /// 属性テーブルが「選択」画面の時
        /// さらに選択したレコードをハイライトする。 
        /// </summary>
        private void HighLightSelectedRecord_OnGridStateSelect()
        {            
            DataGridViewCellStyle cellStyleYelllow = new DataGridViewCellStyle();
            DataGridViewCellStyle cellStyleAquamarine = new DataGridViewCellStyle();
            cellStyleYelllow.BackColor = Color.Yellow;            
            cellStyleAquamarine.BackColor = Color.Aquamarine;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            //foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Selected)
                {
                    //row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle = cellStyleYelllow;                    
                }
                else
                {
                    //row.DefaultCellStyle.BackColor = Color.Aquamarine;
                    row.DefaultCellStyle = cellStyleAquamarine;
                }
            }            
        }

        /// <summary>
        /// 属性テーブルが「選択」画面の時
        /// さらに選択したレコードのジオメトリをハイライトする。
        /// </summary>
        /// <param name="display"></param>
        private void HighLightSelectedGeometry_OnGridStateSelect(IDisplay display)
        {            
            DataGridViewCell cell;
            int		selectedOid;
            ITable	displayFC;
            IRow	selectedFeature;                      

            displayFC = TargetTable;
                        
            //foreach (DataGridViewRow row in dataGridView1.Rows)
            foreach (DataGridViewRow row in dataGridView1.SelectedRows) {
                cell = row.Cells[this._strOIDColumnName];
                selectedOid = (int)cell.Value;

                selectedFeature = displayFC.GetRow(selectedOid);

				// ﾌｨｰﾁｬｰもﾊｲﾗｲﾄ
                if(row.Selected && this.targetLayer != null) {
                    HighLightGeometry((selectedFeature as IFeature).Shape, display);
                }                
            }
        }

        /// <summary>
        /// ジオメトリをハイライトする。
        /// </summary>
        /// <param name="geo">ジオメトリ</param>
        /// <param name="display">IDisplay</param>
        private void HighLightGeometry(IGeometry geo, IDisplay display) {
            display.StartDrawing(display.hDC, (short)esriScreenCache.esriNoScreenCache);

            IRgbColor color = new RgbColorClass();
            color.Red = 255;
            color.Green = 255;

            ISymbol symbol = null;
            switch (geo.GeometryType) {
                case esriGeometryType.esriGeometryPoint:
                    ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
                    simpleMarkerSymbol.Color = color;
                    simpleMarkerSymbol.OutlineColor = color;
                    simpleMarkerSymbol.Size = 12;
                    symbol = (ISymbol)simpleMarkerSymbol;
                    symbol.ROP2 = esriRasterOpCode.esriROPCopyPen;

                    display.SetSymbol(symbol);
                    display.DrawPoint(geo);

                    break;

                case esriGeometryType.esriGeometryPolyline:
                    ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                    simpleLineSymbol.Color = color;
                    simpleLineSymbol.Width = 4;

                    symbol = (ISymbol)simpleLineSymbol;
                    symbol.ROP2 = esriRasterOpCode.esriROPCopyPen;

                    display.SetSymbol(symbol);
                    display.DrawPolyline(geo);

                    break;

                case esriGeometryType.esriGeometryPolygon:
                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = color;

                    symbol = (ISymbol)simpleFillSymbol;
                    symbol.ROP2 = esriRasterOpCode.esriROPCopyPen;

                    display.SetSymbol(symbol);
                    display.DrawPolygon(geo);

                    break;

                default:
                    break;
            }

            display.FinishDrawing();
        }


        /// <summary>
        /// SelectionChangedイベント時に実行される。
        /// </summary>
        private void OnActiveViewEventsSelectionChanged()
        {
            //属性テーブルが「全て」画面の時
            if (isGridStateAll)
            {
                //選択レコード数をラベル表示
                SetToolStripLabelSelectedCount();

                SetGridBackColor();
            }
            //属性テーブルが「選択」画面の時
            else
            {
                //選択レコード数をラベル表示
                SetToolStripLabelSelectedCount();

                GetSelectedData();
            }            
        }


        /// <summary>
        /// 選択レコード数のラベル表示を設定する。
        /// </summary>
        private void SetToolStripLabelSelectedCount()
        {            
            ITableSelection	featureSelection = TargetTableSelection;
            ISelectionSet	selectionSet = featureSelection.SelectionSet;

            toolStripLabelSelectedCount.Text = selectionSet.Count.ToString();            
        }

        /// <summary>
        /// データソースのタイプを判別
        /// </summary>
        /// <param name="sourceLayer">判別対象のレイヤ</param>
        /// <returns>データソースのタイプ</returns>
        private string GetTypeOfDataSource(ILayer2 sourceLayer)
        {
            //IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
            //IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;

            //IWorkspace sourceWorkspace = LayerManager.getWorkspace(sourceFeatureClass);
            IWorkspace	sourceWorkspace = (TargetTable as IDataset).Workspace;

            // シェープファイル
            if (sourceWorkspace.Type == esriWorkspaceType.esriFileSystemWorkspace)
            {
                return TYPE_SHAPE;
            }
            else if (sourceWorkspace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                // ファイルジオデータベース
                if (sourceWorkspace.WorkspaceFactory.GetClassID().Value.ToString() ==
                    "{71FE75F0-EA0C-4406-873E-B7D53748AE7E}")
                {
                    return TYPE_FGDB;
                }
                //パーソナルジオデータベース
                else
                {
                    return TYPE_MDB;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        private void SetPageNo(int count)
        {
            if (count == 0)
            {
                this.PageNo = 1;
            }
            else
            {
                this.PageNo = this.PageNo + count;
            }
            this.labelPage.Text = this.PageNo.ToString() + "頁";
        }
        /// <summary>
        /// 初期読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAttributeTable_Activated(object sender, EventArgs e)
        {
            if (!initDoing) return;

            bool dispMessage = false;
            //Ui.MainForm main = this.mainFrm; // this.Parent as Ui.MainForm;
            try
            {
                // 元の時間のかかる処理を含むメソッド 2010-12-14 del
                //this.bindingSource1.DataSource = CreateAllFeatureDataTable(GET_INIT);

                // 2010-12-14 add
                InitSubThreadProc(GET_INIT);
                DoSubThreadSynco(GET_INIT);
                // <--- end add

                this.bindingNavigator1.BindingSource = this.bindingSource1;
                this.dataGridView1.DataSource = this.bindingSource1;

                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.AllowUserToResizeRows = false;
                this.dataGridView1.AllowUserToAddRows = false;
                this.dataGridView1.AutoSizeRowsMode =
                    DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;

                // 2010-12-14 add
                DoSubThreadSyncoSetGridBackColor();
                EndSubThreadProc();
                SetPageNo(0);

                // <--- end add

                SetFieldFormat();
                SetVisibleFalseColumns();

                this.buttonSwitchAll.Enabled = false;
                buttonPrevious.Enabled = false;

                
                int cnt = RecordCount;

                //総レコード数のラベル表示を設定
                toolStripLabelAllCount.Text = cnt.ToString();

                //選択レコード数のラベル表示を設定
                SetToolStripLabelSelectedCount();

                //if (cnt < Properties.Settings.Default.AttributeTableDisplayOIDMax)
                if(cnt < attributeTableDisplayOIDMax) {
                    buttonNext.Visible = false;
                    buttonPrevious.Visible = false;
                    NextPrevEnable = false;
                }
                else {
                    NextPrevEnable = true;
                }

            }
            catch (COMException comex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(
                    comex.Message, comex.StackTrace, comex.ErrorCode.ToString());
                
                DoRemoveThis = true;
                subwinname[this.thisWinName] = this;
            }
            catch (Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex.Message, ex.StackTrace, null);

                DoRemoveThis = true;
                subwinname[this.thisWinName] = this;
            }
            finally {
                //if (main != null) main.EndMainFormProgress();

                if (dispMessage)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this, Properties.Resources.FormAttributeTable_ErrorWhenFormLoad);
                }

                initDoing = false;

                this.Enabled = true;
            }
        }
    }
}