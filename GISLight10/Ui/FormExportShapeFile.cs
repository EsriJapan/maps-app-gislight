using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using System.Collections;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// シェープファイルエクスポート
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormExportShapeFile : Form
    {


        private Ui.MainForm mainFrm;
        private IMapControl3 m_mapControl;

        // エクスポート用
        private IFeatureLayer2 m_sourceFeatureLayer;
        private string m_targetShapePath;
        private IDictionary m_targetFieldNames;
        private bool m_isSelectFeature;
        private IEnvelope m_selectEnv;
        private bool m_isReplaceFile;
        private bool m_isMapExtent;
        private WKSEnvelope m_wksEnvelope;
        //private int m_outPutFormat;
        //private OutputFormat m_outPutFormat;

        // スレッド用
        private Thread threadTask = null;
        private delegate void RethrowExceptionDelegate(Exception ex);
        private delegate void RethrowCOMExceptionDelegate(COMException comex);

        // プログレスバーフォーム
        private FormProgressManager frmProgress = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="frm">メインフォーム</param>
        /// <param name="mapControl">マップコントロール</param>
        public FormExportShapeFile(Ui.MainForm frm, IMapControl3 mapControl)
        {
            InitializeComponent();
            this.mainFrm = frm;
            this.m_mapControl = mapControl;
        }

		/// <summary>
		/// Formロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormExportShapeFile_Load(object sender, EventArgs e)
		{
			try
			{
				ESRI.ArcGIS.Carto.IFeatureLayer targetFeatureLayer =
					this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer;

				bool isSelectionSetExist = false;
				isSelectionSetExist = Common.ExportFunctions.IsExistSelectedFeatures(targetFeatureLayer as IFeatureLayer2);

				this.comboExportFeatureArea.Items.Clear();

				if (isSelectionSetExist)
				{
                    this.comboExportFeatureArea.Items.Add(Properties.Resources.FormExportShapeFile_EFA_All); //すべてのフィーチャ
                    this.comboExportFeatureArea.Items.Add(Properties.Resources.FormExportShapeFile_EFA_Selected); //選択フィーチャ
                    this.comboExportFeatureArea.Items.Add(Properties.Resources.FormExportShapeFile_EFA_ActiveView); //表示範囲のすべてのフィーチャ
					this.comboExportFeatureArea.SelectedIndex = 1;
				}
				else
				{
					this.comboExportFeatureArea.Items.Add(Properties.Resources.FormExportShapeFile_EFA_All);
					this.comboExportFeatureArea.Items.Add(Properties.Resources.FormExportShapeFile_EFA_ActiveView);
					this.comboExportFeatureArea.SelectedIndex = 0;
				}

				// データグリッドにフィールドを設定
				SetFieldNamesToGrid();
			}
			catch (COMException comex)
			{
				Common.MessageBoxManager.ShowMessageBoxError(
					this, Properties.Resources.FormExportShapeFile_ERROR_FormOpenFailed);
				Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_FormOpenFailed);
				Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
                this.Close();
				//this.Dispose();
			}
			catch (Exception ex)
			{
				Common.MessageBoxManager.ShowMessageBoxError(
					this, Properties.Resources.FormExportShapeFile_ERROR_FormOpenFailed);
				Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_FormOpenFailed);
				Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
                this.Close();
                //this.Dispose();
			}
		}

        /// <summary>
        /// Formクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormExportShapeFile_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Dispose();
        }

		/// <summary>
		/// グリッドにフィールドを設定
		/// </summary>
		private void SetFieldNamesToGrid() {
			// 対象レイヤのフィールドを表示
			var	agFL = this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer2;
			int intRow;

            // ﾌｨｰﾙﾄﾞ･ｱｲﾃﾑを取得 (ﾊﾞｲﾅﾘ型を除く)
            Common.FieldComboItem[]	cmbFlds = this.mainFrm.GetFieldItems(this.mainFrm.SelectedLayer as IFeatureLayer, true, false, false, true, 
													esriFieldType.esriFieldTypeSmallInteger,
													esriFieldType.esriFieldTypeInteger,
													esriFieldType.esriFieldTypeSingle,
													esriFieldType.esriFieldTypeDouble,
													esriFieldType.esriFieldTypeString,
													esriFieldType.esriFieldTypeDate);
			foreach(FieldComboItem fld in cmbFlds) {
				// ﾌｨｰﾙﾄﾞ行を追加
				intRow = this.dataSelectField.Rows.Add();
				this.dataSelectField[0, intRow].Value = fld.FieldAlias;
				this.dataSelectField[0, intRow].ToolTipText = fld.FieldAlias;
				this.dataSelectField[0, intRow].ReadOnly = true;
				this.dataSelectField[0, intRow].Tag = fld.FieldName;	// ｾﾙ･ﾀｸﾞにﾌｨｰﾙﾄﾞ名を潜入
				this.dataSelectField[1, intRow].Value = true;			// 選択
			}
		}

        private List<IField> GetSelectedField() 
        {
            List<IField> fields = new List<IField>();

            ESRI.ArcGIS.Carto.IFeatureLayer2 targetFeatureLayer =
                this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer2;

            IGeoFeatureLayer targetGeoFeatureLayer = targetFeatureLayer as IGeoFeatureLayer;
            IFeatureClass targetFeatureClass = targetGeoFeatureLayer.DisplayFeatureClass;
            IFields targetFields = targetFeatureClass.Fields;

            int i = -1;
            if (targetFeatureClass.OIDFieldName != null)
            {
                i = targetFields.FindField(targetFeatureClass.OIDFieldName);
                fields.Add(targetFields.get_Field(i));
            }
            if (targetFeatureClass.ShapeFieldName != null)
            {
                i = targetFields.FindField(targetFeatureClass.ShapeFieldName);
                fields.Add(targetFields.get_Field(i));
            }

            // グリッド
            for (int ii = 0; ii < this.dataSelectField.RowCount; ii++)
            {
                if (Convert.ToBoolean(this.dataSelectField[1, ii].Value))
                {
                    i = targetFields.FindField(this.dataSelectField[0, ii].Tag.ToString());
                    fields.Add(targetFields.get_Field(i));
                }
            }

            return fields;
        }

		/// <summary>
		/// グリッド上のチェック状態による処理対象フィールドの格納
		/// </summary>
		/// <returns>contextFieldNames</returns>
		private IDictionary SetFieldNamesToApContext()
		{
			ESRI.ArcGIS.Carto.IFeatureLayer2 targetFeatureLayer =
				this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer2;

			IGeoFeatureLayer targetGeoFeatureLayer = targetFeatureLayer as IGeoFeatureLayer;
			IFeatureClass targetFeatureClass = targetGeoFeatureLayer.DisplayFeatureClass;

			IDictionary contextFieldNames = new Hashtable();

			contextFieldNames.Clear();

			if (targetFeatureClass.OIDFieldName != null)
			{
				contextFieldNames[targetFeatureClass.OIDFieldName] = true;
			}

			if (targetFeatureClass.ShapeFieldName != null)
			{
				contextFieldNames[targetFeatureClass.ShapeFieldName] = true;
			}
            
			int selected_cnt = 0;

			for (int i = 0; i < this.dataSelectField.RowCount; i++)
			{
				if (Convert.ToBoolean(this.dataSelectField[1, i].Value))
				{
					contextFieldNames[this.dataSelectField[0, i].Value.ToString()] = true;

					selected_cnt++;
				}
				else
				{
					contextFieldNames[this.dataSelectField[0, i].Value.ToString()] = false;
				}
			}

			if (selected_cnt == 0)
			{
				contextFieldNames.Clear();
				return contextFieldNames;
			}

			return contextFieldNames;
		}

        /// <summary>
        /// 保存先指定ボタン(Shape)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            if (saveShapeFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                // 選択したものが同じフィーチャクラスでないか
                ESRI.ArcGIS.Carto.IFeatureLayer sourceLayer =
                    this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer;
 
                // 入力レイヤと出力フィーチャクラスが同じものを指定していないか
                if (CompareSameShapeFile(sourceLayer, saveShapeFileDialog.FileName))
                {
                    Common.MessageBoxManager.ShowMessageBoxWarining(
                        this, Properties.Resources.FormExportShapeFile_WARNING_SameShapeFile);
                    this.textExportFile.Text = "";
                    return;
                }
                else 
                {
                    if (System.IO.File.Exists(saveShapeFileDialog.FileName))
                    {
                        bool blDelresult = ExportFunctions.DeleteDataset(saveShapeFileDialog.FileName, ExportFunctions.OutputFormat.ShapeFile);
                        if (!blDelresult)
                            Common.MessageBoxManager.ShowMessageBoxError(
                                this, Properties.Resources.FormExportShapeFile_ErrorDeleteFeatureClass);
                    }
                    this.textExportFile.Text = saveShapeFileDialog.FileName;
                }

            }
        }

        /// <summary>
        /// 保存先指定ボタン(GeoDataBase)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectWorkspace_Click(object sender, EventArgs e)
        {
            using (FormGISDataSelectDialog frm = new FormGISDataSelectDialog())
            {
                frm.SelectType = FormGISDataSelectDialog.ReturnType.GdbWorkspaceName;
                //frm.StartFolder = "";
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    IWorkspaceName wsName = ((List<IWorkspaceName>)frm.SelectedObject)[0];
                    //MessageBox.Show(wsName.PathName);
                    textExportWorkspace.Text = wsName.PathName;
                }
            }

        }

		/// <summary>
		/// 選択フィールド選択or解除
		/// </summary>
		/// <param name="sel">選択解除スイッチ(trueまたはfalse)</param>
		private void SelectOrUnSelectSelectFields(bool sel)
		{
			for (int i = 0; i < this.dataSelectField.RowCount; i++)
			{
				if ((this.dataSelectField[0, i].Selected) || (this.dataSelectField[1, i].Selected))
				{
					if(!(this.dataSelectField[1, i].ReadOnly))
					{
						this.dataSelectField[1, i].Value = sel;
					}
				}
			}
		}

		/// <summary>
		/// 選択ボタンイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSelect_Click(object sender, EventArgs e)
		{
			SelectOrUnSelectSelectFields(true);
		}

		/// <summary>
		/// 解除ボタンイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonUnSelect_Click(object sender, EventArgs e)
		{
			SelectOrUnSelectSelectFields(false);

		}

		/// <summary>
		/// 全フィールド全て選択or全て解除
		/// </summary>
		/// <param name="sel">選択解除スイッチ(trueまたはfalse)</param>
		/// <returns></returns>
		private void SelectOrUnSelectAllFields(bool sel)
		{
			for (int i = 0; i < this.dataSelectField.RowCount; i++)
			{
				if (!this.dataSelectField[1, i].ReadOnly)
				{
					this.dataSelectField[1, i].Value = sel;
				}
			}
		}

		/// <summary>
		/// 全選択ボタンイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSelectAll_Click(object sender, EventArgs e)
		{
			SelectOrUnSelectAllFields(true);
		}

		/// <summary>
		/// 全て解除ボタンイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSelectNone_Click(object sender, EventArgs e)
		{
			SelectOrUnSelectAllFields(false);
		}

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// データソースと出力ファイルの比較
		/// </summary>
        /// <param name="sourceFeatureLayer"></param>
        /// <param name="targetShapePath"></param>
        /// <returns>sameResult</returns>
		private bool CompareSameShapeFile(IFeatureLayer sourceFeatureLayer, string targetShapePath)
		{
			string targetFileName = System.IO.Path.GetFileNameWithoutExtension(targetShapePath);
			string targetDirectoryName = System.IO.Path.GetDirectoryName(targetShapePath);

			bool sameResult = false;

			IDataset sourceDataset = sourceFeatureLayer.FeatureClass as IDataset;
			IWorkspace sourceWorkspace = sourceDataset.Workspace;
			if ((String.Compare(targetDirectoryName, sourceWorkspace.PathName) == 0) &&
					(String.Compare(targetFileName, sourceDataset.Name) == 0))
			{
				sameResult = true;
			}

			return sameResult;
		}



        /// <summary>
        /// シェープファイルへのエクスポート(OKボタン)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {

            ESRI.ArcGIS.Carto.IFeatureLayer sourceLayer =
                this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer;


            //IDictionary targetFeildNames;
            bool isReplaceFile;

            #region /* 各パラメータチェック */
            
            // 出力対象フィールドをリストに取得
            List<IField> sourceFieldList = GetSelectedField();
            ////targetFeildNames = SetFieldNamesToApContext();
            isReplaceFile = false;


            // エクスポート対象フィールド
            //if (targetFeildNames.Count == 0)
            if (sourceFieldList.Count == 0)
            {
                Common.MessageBoxManager.ShowMessageBoxWarining(
                    this, Properties.Resources.FormExportShapeFile_WARNING_NoFeild);
                return;
            }


            string outputfilepath = "";

            // 出力フォーマット
            ExportFunctions.OutputFormat outputFormat = ExportFunctions.OutputFormat.ShapeFile;
            switch (this.tabControlOutPut.SelectedIndex)
            {
                case 0:

                    // エクスポート先のシェープファイルのパス
                    if (this.textExportFile.Text == null || this.textExportFile.Text.Length == 0)
                    {
                        Common.MessageBoxManager.ShowMessageBoxWarining(
                            this, Properties.Resources.FormExportShapeFile_WARNING_NoFileName);
                        return;
                    }

                    // 入力レイヤと出力フィーチャクラスが同じものを指定していないか
                    if (CompareSameShapeFile(sourceLayer, this.textExportFile.Text))
                    {
                        Common.MessageBoxManager.ShowMessageBoxWarining(
                            this, Properties.Resources.FormExportShapeFile_WARNING_SameShapeFile);
                        return;
                    }

                    // ファイル置き換え
                    if (System.IO.File.Exists(this.textExportFile.Text))
                    {
                        DialogResult resReplaceFile = Common.MessageBoxManager.ShowMessageBoxWarining2(
                            this, this.textExportFile.Text + Properties.Resources.FormExportShapeFile_WARNING_ReplaceFile);

                        if (resReplaceFile == DialogResult.OK)
                        {
                            bool blSuccess = ExportFunctions.DeleteDataset(this.textExportFile.Text, ExportFunctions.OutputFormat.ShapeFile);
                            if (!blSuccess)
                            {
                                Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.FormExportShapeFile_ErrorDeleteFeatureClass);
                                return;
                            }
                            isReplaceFile = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                    //else
                    //{
                    //    isReplaceFile = true;
                    //}

                    string exportFileDirectoryPath = System.IO.Path.GetDirectoryName(this.textExportFile.Text);

                    // 出力フォルダが存在しない場合作成
                    if (!System.IO.Directory.Exists(exportFileDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(exportFileDirectoryPath);
                    }

                    // 出力タイプ
                    outputFormat = ExportFunctions.OutputFormat.ShapeFile;
                    
                    outputfilepath = textExportFile.Text;

                    break;

                case 1:


                    string errorStrings = "";
                    //　ジオデータベース指定のチェック
                    if (this.textExportWorkspace.Text == null || this.textExportWorkspace.TextLength == 0)
                    {
                        Common.MessageBoxManager.ShowMessageBoxWarining(
                            this, Properties.Resources.FormExportShapeFile_WARNING_Database);
                        return;
                    }
                    // フィーチャクラス名のチェック
                    if (this.textExportFeature.Text == null || this.textExportFeature.TextLength == 0)
                    {
                        Common.MessageBoxManager.ShowMessageBoxWarining(
                            this, Properties.Resources.FormExportShapeFile_WARNING_FeatureClassName);
                        return;
                    }

                    // 入力レイヤと出力フィーチャクラスが同じものを指定していないか
                    if (CompareSameShapeFile(sourceLayer, textExportWorkspace.Text + "\\" + textExportFeature.Text))
                    {
                        Common.MessageBoxManager.ShowMessageBoxWarining(
                            this, Properties.Resources.FormExportShapeFile_WARNING_SameShapeFile);
                        return;
                    }


                    // 出力フィーチャクラスのデリバード
                    errorStrings += Check.FeatureClassString(this.textExportWorkspace.Text, this.textExportFeature.Text, false);

                    // 2012/08/27 UPD 
                    if (errorStrings != "")
                    {
                        //DialogResult resReplaceFile = Common.MessageBoxManager.ShowMessageBoxWarining2(
                        //         this, this.textExportWorkspace.Text + "\\" +this.textExportFeature.Text + Properties.Resources.FormExportShapeFile_WARNING_ReplaceFile);
                        DialogResult resReplaceFile = Common.MessageBoxManager.ShowMessageBoxWarining2(this, errorStrings);
                        return;
                    }
                    else
                    {
                        //DialogResult resReplaceFile = Common.MessageBoxManager.ShowMessageBoxWarining2(
                        //         this, this.textExportWorkspace.Text + "\\" +this.textExportFeature.Text + Properties.Resources.FormExportShapeFile_WARNING_ReplaceFile);
                    // 2012/08/27 UPD 
                        //if (resReplaceFile == DialogResult.OK)
                        //{
                            //bool blSuccess = true;

                            //// 出力タイプ
                            //if (System.IO.Path.GetExtension(this.textExportWorkspace.Text).ToUpper().Equals(".MDB"))
                            //{
                            //    blSuccess = ExportFunctions.DeleteDataset(this.textExportWorkspace.Text + "\\" + textExportFeature.Text, ExportFunctions.OutputFormat.Pgdb);
                            //}
                            //else if (System.IO.Path.GetExtension(this.textExportWorkspace.Text).ToUpper().Equals(".GDB"))
                            //{
                            //    blSuccess = ExportFunctions.DeleteDataset(this.textExportWorkspace.Text + "\\" + textExportFeature.Text, ExportFunctions.OutputFormat.Fgdb);
                            //}

                            //if (!blSuccess)
                            //{
                            //    Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.FormExportShapeFile_ErrorDeleteFeatureClass);
                            //    return;
                            //}


                            //isReplaceFile = true;
                        //}
                        //else
                        //{
                        //    return;
                        //}
                        //Common.MessageBoxManager.ShowMessageBoxWarining(
                        //                       this, Properties.Resources.FormExportShapeFile_WARNING_FeatureClassExits);
                        //return;
                    }

                    // 出力タイプ
                    if (System.IO.Path.GetExtension(this.textExportWorkspace.Text).ToUpper().Equals(".MDB"))
                    {
                        outputFormat = ExportFunctions.OutputFormat.Pgdb;
                    }
                    else if (System.IO.Path.GetExtension(this.textExportWorkspace.Text).ToUpper().Equals(".GDB"))
                    {
                        outputFormat = ExportFunctions.OutputFormat.Fgdb;
                    }

                    outputfilepath = textExportWorkspace.Text + "\\" + textExportFeature.Text;

                    break;
                default:
                    return;
            }          

            // 選択条件
            int selectedExtentIndex = 1;
            if (this.comboExportFeatureArea.Items.Count == 3)
            {
                selectedExtentIndex++;
            }

            // パラメータセット
            m_sourceFeatureLayer = sourceLayer as IFeatureLayer2;
            //m_targetShapePath = targetShapePath;
            //m_targetFieldNames = targetFeildNames;
            m_isReplaceFile = isReplaceFile;

            m_isSelectFeature = false;
            m_isMapExtent = false;
            m_selectEnv = null;

            // 選択したフィーチャのみ
            if (this.comboExportFeatureArea.Items.Count == 3 &&
                this.comboExportFeatureArea.SelectedIndex == 1)
            {
                m_isSelectFeature = true;
            }
            else
            {
                // 表示範囲のみ
                if (this.comboExportFeatureArea.SelectedIndex == selectedExtentIndex)
                {
                    //m_selectEnv = m_mapControl.Extent;
                    m_selectEnv = m_mapControl.ActiveView.Extent;
                    m_isMapExtent = true;
                }

                m_isSelectFeature = false;
            }

            #endregion



            #region /* エクスポート処理 */

            this.Enabled = false;

            ProgressDialog pd = null;
            IFeatureCursor sourceFeautreCursor = null;
            IFeatureCursor targetFeatureCursor = null;
            bool blPdclose = false;

            try
            {
                Common.Logger.Info("シェープファイルへエクスポート開始");

                pd = new ProgressDialog();
                pd.Minimum = 0;
                pd.Maximum = 2;
                pd.Title = string.Format("{0}", this.Text);
                pd.Show(this);

                IGeoFeatureLayer sourceGeoFeatureLayer = (IGeoFeatureLayer)sourceLayer;
                IFeatureClass sourceDispFeatureClass = sourceGeoFeatureLayer.DisplayFeatureClass;

                // 選択しているフィールドで出力用フィーチャクラスの作成
                pd.Value = 1;
                pd.Message = "出力用フィーチャクラスの作成中・・・";
                IFeatureClass targetFeatureClass = ExportFunctions.CreateFeatureClass(outputfilepath, sourceFieldList, outputFormat);
                pd.Value = 2;

                // VALIDATE済みフィールドを取得
                // targetFieldsのフィールドインデックス番号
                IFields targetFields = targetFeatureClass.Fields;
                List<IField> validatedFieldList = new List<IField>();
                List<int> targetFieldIndexList = new List<int>();
                for (int i = 0; i < targetFields.FieldCount; i++)
                {
                    validatedFieldList.Add(targetFields.get_Field(i));
                    targetFieldIndexList.Add(i);
                }

                // sourceFieldsのフィールドインデックス番号
                List<int> sourceFieldIndexList = new List<int>();
                for (int i = 0; i < sourceFieldList.Count; i++)
                {
                    int index = sourceDispFeatureClass.Fields.FindField(sourceFieldList[i].Name);
                    sourceFieldIndexList.Add(index);
                }

                //if (sourceFieldList.Count != validatedFieldList.Count)
                //{
                //    MessageBox.Show(this, "フィールド数が一致しない");
                //    return;
                //}

                int max = 0;
                // 選択セットだけの場合はIDでクエリフィルタをセット
                // すべてのフィーチャ
                // 選択フィーチャ
                // 表示範囲のすべてのフィーチャ

                IQueryFilter queryFilter = new QueryFilterClass(); 
                if (m_isSelectFeature)
                {
                    queryFilter.WhereClause = createSelectedOidSet(sourceLayer);
                }

                // 現在の範囲の場合
                ISpatialFilter spatialFilter = new SpatialFilterClass();       
                if (m_isMapExtent)
                {
                    spatialFilter.Geometry = m_selectEnv;
                    spatialFilter.GeometryField = sourceLayer.FeatureClass.ShapeFieldName;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                }


                if (m_isSelectFeature)
                {
                    max = ((IFeatureSelection)sourceLayer).SelectionSet.Count;
                    sourceFeautreCursor = sourceDispFeatureClass.Search(queryFilter, true);
                }
                else
                {
                    if (m_isMapExtent)
                    {
                        max = sourceDispFeatureClass.Select(spatialFilter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, ((IDataset)sourceDispFeatureClass).Workspace).Count;
                        sourceFeautreCursor = sourceDispFeatureClass.Search(spatialFilter, true);
                    }
                    else
                    {
                        max = sourceDispFeatureClass.Select(null, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, ((IDataset)sourceDispFeatureClass).Workspace).Count;
                        sourceFeautreCursor = sourceDispFeatureClass.Search(null, true);
                    }
                }

                // ソースとターゲットのフィールドインデックス

                // 
                pd.Message = string.Format("{0} への出力中・・・", outputfilepath); 
                pd.Minimum = 0;
                pd.Maximum = max;

                targetFeatureCursor = targetFeatureClass.Insert(true);
                IFeatureBuffer targetBuffer = targetFeatureClass.CreateFeatureBuffer();

                int flushBufferCounter = 0;
                IFeature sourceFeature = sourceFeautreCursor.NextFeature();
                while (sourceFeature != null)
                {
                    pd.Value = flushBufferCounter + 1;
                    pd.Message = string.Format("{0} への出力中　\n\n {1} / {2}件を処理中・・・", outputfilepath, flushBufferCounter + 1, max);
                    if (pd.Canceled)
                    {
                        break;
                    }

                    targetBuffer.Shape = sourceFeature.ShapeCopy;
                    for (int i = 0; i < sourceFieldList.Count; i++)
                    {
                        int sourceIndex = sourceFieldIndexList[i];
                        int targetIndex = targetFieldIndexList[i];
                        if (!validatedFieldList[i].Required)
                        {
                            if (sourceFeature.get_Value(sourceIndex) != System.DBNull.Value)
                            {
                                    targetBuffer.set_Value(targetIndex, sourceFeature.get_Value(sourceIndex));
                            }
                            else // NULLの値も設定しないと前のテキストや日付レコードをコピーしてきてしまう
                            {
                                if (outputFormat == ExportFunctions.OutputFormat.ShapeFile) // outputがshapefileのときだけ
                                {
                                    switch (validatedFieldList[targetIndex].Type)
                                    {
                                        case esriFieldType.esriFieldTypeDouble:
                                        case esriFieldType.esriFieldTypeInteger:
                                        case esriFieldType.esriFieldTypeSingle:
                                        case esriFieldType.esriFieldTypeSmallInteger:
                                            targetBuffer.set_Value(targetIndex, 0);
                                            break;
                                        case esriFieldType.esriFieldTypeString:
                                        case esriFieldType.esriFieldTypeXML:
                                            targetBuffer.set_Value(targetIndex, "");       
                                            break;
                                        case esriFieldType.esriFieldTypeDate:
                                            targetBuffer.set_Value(targetIndex, System.DBNull.Value);
                                            break;
                                    }
                                }
                                else 
                                {
                                    targetBuffer.set_Value(targetIndex, System.DBNull.Value);
                                }
                            }

                        }


                    }

                    targetFeatureCursor.InsertFeature(targetBuffer);

                    flushBufferCounter += 1;

                    if (flushBufferCounter % 100 == 0)
                    {
                        targetFeatureCursor.Flush();
                    }

                    //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(sourceFeature);
                    sourceFeature = sourceFeautreCursor.NextFeature();
                }

                targetFeatureCursor.Flush();

                // データの追加
                pd.Close();
                blPdclose = true;

               // エクスポートしたデータをレイヤに追加
                DialogResult resAddExportData = Common.MessageBoxManager.ShowMessageBoxQuestion2(
                this, Properties.Resources.FormExportShapeFile_QUESTION_AddExpotData);

                if (resAddExportData == DialogResult.OK)
                {
                    IFeatureLayer addFaetureLayer = new FeatureLayerClass();
                    addFaetureLayer.FeatureClass = targetFeatureClass;
                    addFaetureLayer.Name = targetFeatureClass.AliasName;

                    this.mainFrm.MapControl.AddLayer(addFaetureLayer, 0);
                }


            }
            catch (ApplicationException)
            {
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
                Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
                Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
				Common.Logger.Info("シェープファイルへエクスポート終了");

                // スレッドメモリ解放
                if (threadTask != null)
                {
                    if (threadTask.IsAlive)
                    {
                        threadTask.Abort();
                    }

                    threadTask = null;
                }

                if (sourceFeautreCursor != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(sourceFeautreCursor);

                if (targetFeatureCursor != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(targetFeatureCursor);

                this.Enabled = true;

                if (!blPdclose)
                    pd.Close();
                
                this.Close();
            }

            #endregion

        }

        private string createSelectedOidSet(IFeatureLayer featureLayer)
        {
            IFeatureClass featureClass = featureLayer.FeatureClass;
            string oidfield = featureClass.OIDFieldName;

            // 入力データのタイプ
            ExportFunctions.OutputFormat inputFormat = ExportFunctions.OutputFormat.ShapeFile;
            IDataset dataset = (IDataset)featureClass;
            IDatasetName dsName = (IDatasetName)dataset.FullName;
            if (dsName.WorkspaceName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.AccessWorkspaceFactory"))
            {
                inputFormat = ExportFunctions.OutputFormat.Pgdb;
            }
            

            IFeatureSelection featureSel = (IFeatureSelection)featureLayer;
            IEnumIDs enumIds = featureSel.SelectionSet.IDs;
            enumIds.Reset();
            int id = -1;

            StringBuilder stb = new StringBuilder();
            if (inputFormat == ExportFunctions.OutputFormat.Pgdb) // [フィールド名]
            {
                stb.Append("[");
                stb.Append(oidfield);
                stb.Append("]");
            }
            else // "フィールド名"
            {
                stb.Append('"');
                stb.Append(oidfield);
                stb.Append('"');
            }

            stb.Append(" in (");
            id = enumIds.Next();
            while (id != -1)
            {
                stb.Append(id.ToString() + ",");
                id = enumIds.Next();
            }
            stb.Remove(stb.Length - 1, 1);

            stb.Append(")");

            return stb.ToString();

        }

        /// <summary>
        /// スレッドから呼び出されるコールバック
        /// </summary>
        private void CallBackResult()
        {
            frmProgress.IncrementProgressBar(10);
            System.Threading.Thread.Sleep(300);
            frmProgress.CloseForm();
            this.Refresh();

            // エクスポートしたデータをレイヤに追加
            DialogResult resAddExportData = Common.MessageBoxManager.ShowMessageBoxQuestion2(
                this, Properties.Resources.FormExportShapeFile_QUESTION_AddExpotData);

            if (resAddExportData == DialogResult.OK)
            {
                try
                {
                    //ILayer2 exportedLayer =
                    //    Common.ExportFunctions.GetExportedLayer(m_targetShapePath,m_outPutFormat);

                    //m_mapControl.AddLayer(exportedLayer as ILayer, 0);
                }
                catch (COMException comex)
                {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        this, Properties.Resources.FormExportShapeFile_ERROR_AddExportDataFailed);
                    Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_AddExportDataFailed);
                    Common.Logger.Error(comex.Message);
                    Common.Logger.Error(comex.StackTrace);

                    throw new ApplicationException();
                }
                catch (Exception ex)
                {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        this, Properties.Resources.FormExportShapeFile_ERROR_AddExportDataFailed);
                    Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_AddExportDataFailed);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    throw new ApplicationException();
                }
            }
        }

        /// <summary>
        /// 例外の再スロー(COMException)
        /// </summary>
        /// <param name="comex">例外(COMException)</param>
        private void RethrowCOMException(COMException comex)
        {
            Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
            Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
            Common.Logger.Error(comex.Message);
            Common.Logger.Error(comex.StackTrace);

            throw new ApplicationException();
        }

        /// <summary>
        /// 例外の再スロー(Exception)
        /// </summary>
        /// <param name="ex">例外(Exception)</param>
        private void RethrowException(Exception ex)
        {
            Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
            Common.Logger.Error(Properties.Resources.FormExportShapeFile_ERROR_ExportFailed);
            Common.Logger.Error(ex.Message);
            Common.Logger.Error(ex.StackTrace);

            throw new ApplicationException();
        }


        /// <summary>
        /// エクスポート処理
        /// </summary>
        /// <param name="obj">エクスポート処理を行うためのパラメータ</param>
        private void ExportFeatureThread(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet = 
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                IFeatureLayer featureLayer = propertySet.GetProperty("FeatureLayer") as IFeatureLayer;

                IEnvelope targetEnvelope = null;

                if (m_isMapExtent)
                {
                    targetEnvelope = new EnvelopeClass();
                    targetEnvelope.XMax = m_wksEnvelope.XMax;
                    targetEnvelope.XMin = m_wksEnvelope.XMin;
                    targetEnvelope.YMax = m_wksEnvelope.YMax;
                    targetEnvelope.YMin = m_wksEnvelope.YMin;
                }


                this.Invoke(ti.CallBack);
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(RethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(RethrowException);
                this.BeginInvoke(dlgt, new object[] { ex }); 
            }
        }

    }
}
