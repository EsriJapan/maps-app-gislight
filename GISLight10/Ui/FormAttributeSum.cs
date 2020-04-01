using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    ///　属性値集計
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormAttributeSum : Form
    {
        //private Ui.MainForm mainFrm;
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;
        private IMap pMap;
        private List<IFeatureLayer> featureLayerList;

        //private Hashtable fieldnam2alias = null;
        private String outTemporaryWorkspace;
        private String outTemporaryMDBName;
        private String outTemporaryMDBFullPath;
        private int _filterIndex = 1;
        private int _MaxCount = 3;

        private String sqlCommand;
        int[] keyFieldNum;
        int sumFieldsNum;
        private String saveFilePath;

        // スレッド用
        private Thread threadTask = null;
        private delegate void RethrowExceptionDelegate(Exception ex);
        private delegate void RethrowCOMExceptionDelegate(COMException comex);

        // プログレスバーフォーム
        private FormProgressManager frmProgress = null;

        // 別名表示設定
        private bool	_blnUseAlias;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormAttributeSum(ESRI.ArcGIS.Controls.IMapControl3 mapControl)
        {
            this.m_mapControl = mapControl;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();

            InitializeComponent();
        }

        /// <summary>
        /// FormAttributeSumフォームのLoadイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAttributeSum_Load(object sender, EventArgs e)
        {
            try
            {
                pMap = (IMap)m_mapControl.Map;
                if (pMap.LayerCount <= 0)
                    this.Close();

				// 別名表示設定を取得
				this._blnUseAlias = (Control.FromHandle(((IntPtr)this.m_mapControl.hWnd)).FindForm() as MainForm).IsUseFieldAlias;

                Common.LayerManager pLayerManager = new ESRIJapan.GISLight10.Common.LayerManager();
                featureLayerList = pLayerManager.GetFeatureLayers(pMap);

				for (int i = 0; i < featureLayerList.Count; i++)
				{
					IFeatureLayer pFLayer = featureLayerList[i];
					// 数値型フィールドを持つレイヤのみcmb_FeatureLayerNameに追加
					if (this.hasNumericalField(pFLayer))
					{
						cmb_FeatureLayerName.Items.Add(new Common.LayerComboItem(pFLayer));
					}
				}

				if (cmb_FeatureLayerName.Items.Count > 0)
				{
					cmb_FeatureLayerName.Text = cmb_FeatureLayerName.Items[0].ToString();

					// テンポラリMDB名を作成
                    //outTemporaryWorkspace = System.IO.Path.GetTempPath();
                    StringBuilder temporaryWorkspacePath = new StringBuilder();
                    temporaryWorkspacePath.Append(System.IO.Path.GetTempPath());
                    //temporaryWorkspacePath.Append("\\");
                    temporaryWorkspacePath.Append("GISLight10");
                    outTemporaryWorkspace = temporaryWorkspacePath.ToString();

                    outTemporaryMDBName = GenerateTempMDBFileName();

                    outTemporaryMDBFullPath = CreateFullPath(outTemporaryWorkspace, outTemporaryMDBName);

                    if (!System.IO.Directory.Exists(outTemporaryWorkspace))
                    {
                        System.IO.Directory.CreateDirectory(outTemporaryWorkspace);
                    }

					if (System.IO.File.Exists(outTemporaryMDBFullPath))
					{
						System.IO.File.Delete(outTemporaryMDBFullPath);
					}
				}
				else
				{
					Common.MessageBoxManager.ShowMessageBoxWarining(
						this, Properties.Resources.FormAttributeSum_NoLayer);
					Common.Logger.Warn(Properties.Resources.FormAttributeSum_NoLayer);

                    this.Close();
					//this.Dispose();
				}

            }
			catch (COMException comex)
			{
				Common.MessageBoxManager.ShowMessageBoxError(
					this, Properties.Resources.FormAttributeSum_WhenLoad);
				Common.Logger.Error(Properties.Resources.FormAttributeSum_WhenLoad);
				Common.Logger.Error(comex.StackTrace);
				Common.Logger.Error(comex.Message);
                this.Close();
				//this.Dispose();
			}
			catch (Exception ex)
			{
				Common.MessageBoxManager.ShowMessageBoxError(
					this, Properties.Resources.FormAttributeSum_WhenLoad);
				Common.Logger.Error(Properties.Resources.FormAttributeSum_WhenLoad);
				Common.Logger.Error(ex.StackTrace);
				Common.Logger.Error(ex.Message);
                this.Close();
				//this.Dispose();
			}
        }

        /// <summary>
        /// キャンセルボタンをClick時の処理
        /// FormAttributeSumを閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 進捗がでるように対応
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="itemLayer"></param>
        private void attributeSum(IFeatureLayer featureLayer, ILayer itemLayer)
        {
            ProgressDialog pd = null;
            try 
            {
                pd = new ProgressDialog();
                pd.Minimum = 0;
                pd.Maximum = 3;
                pd.Title = string.Format("{0}", this.Text);
                pd.CancelEnable = false;
                pd.Show(this);

                //pd.Message = string.Format("{0} / {1}を取得中・・・。 個別値: {2}件", ii, max, fldNamesTbl.Count);
                pd.Message = string.Format("集計 {0}/{1}のプロセスを実行中・・・",1,3);
                // テンポラリ用MDBに出力
                Common.ExportFunctions2.ExportMDB(outTemporaryWorkspace,
                    outTemporaryMDBName,
                    featureLayer,
                    featureLayer.Name,
                    sumFieldsNum,
                    chkSelectionSet.Checked,
                    keyFieldNum);
                pd.Value = 1;


                // 属性値を集計するためのSQLコマンドを生成
                pd.Message = string.Format("集計 {0}/{1}のプロセスを実行中・・・", 2, 3);
                sqlCommand = CreateSQLCommand2(
                    outTemporaryMDBFullPath,
                    itemLayer,
                    sumFieldsNum,
                    keyFieldNum);
                pd.Value = 2;
                
                // ｴｲﾘｱｽに対応
                if(this._blnUseAlias) {
					// SQLを調整
					int			intPos = sqlCommand.IndexOf("SUM(");
					string		strSelect = sqlCommand.Substring(7, intPos - 1 - 7);
					string[]	strFlds = strSelect.Split(',');
					
					bool			blnChange = false;
					List<string>	strFldAs = new List<string>();
					foreach(string strFld in strFlds) {
						// 表示設定を組み込む
						foreach(Common.FieldComboItem fldItem in listBox_SelectedKeyFields.Items) {
							if(strFld.Equals(fldItem.FieldName) || strFld.Equals(fldItem.Field.AliasName)) {
								if(strFld.Equals(fldItem.FieldAlias)) {
									strFldAs.Add(strFld);
								}
								else {
									strFldAs.Add(strFld + " AS " + fldItem.FieldAlias);
									blnChange = true;
								}
								break;
							}
						}
					}
					
					// 変更がある場合のみ適用
					if(blnChange) {
						sqlCommand = "SELECT " + string.Join(",", strFldAs.ToArray()) + sqlCommand.Substring(intPos - 1);
					}
                }

                // 2012/07/19 mdbに対応
                // 集計とCSV/DBF出力
                pd.Message = string.Format("集計 {0}/{1}のプロセスを実行中・・・", 3, 3);
                if (_filterIndex == 1)
                {
                    Common.ExportFunctions2.ExportToCSV(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }
                else if (_filterIndex == 2)
                {
                    Common.ExportFunctions2.ExportToDBF(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }
                else if (_filterIndex == 3)
                {
                    Common.ExportFunctions2.ExportToMDB(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }
                else if (_filterIndex == 4)
                {
                    Common.ExportFunctions2.ExportToGDB(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }
                pd.Value = 3;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally 
            {
                pd.Close();
            }



        }

        /// <summary>
        /// 実行ボタンをClick時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Run_Click(object sender, EventArgs e)
        {
            try
            {
                // 選択している出力フォーマットを決定
                switch (tabControl1.SelectedIndex)
                {
                    case 0: // DBF or CSV
                        string file = txt_SaveFilePath.Text;
                        if (file != null)
                        {
                            if (System.IO.Path.GetExtension(file).ToUpper().Equals(".CSV"))
                            {
                                this._filterIndex = 1;
                            }
                            else if (System.IO.Path.GetExtension(file).ToUpper().Equals(".DBF"))
                            {
                                this._filterIndex = 2;
                            }
                        }
                        break;
                    case 1: // FGDB or PGDBのテーブル
                        string path = txtSaveFilePath.Text;
                        if (path != null)
                        {
                            if (System.IO.Path.GetExtension(path).ToUpper().Equals(".MDB"))
                            {
                                this._filterIndex = 3;
                            }
                            else if (System.IO.Path.GetExtension(path).ToUpper().Equals(".GDB"))
                            {
                                this._filterIndex = 4;
                            }
                        }
                        break;
                }


                // 入力チェック
                String warning_message = "";
                if (!InParameterCheck(ref warning_message))
                {
                    Common.MessageBoxManager.ShowMessageBoxWarining(this, warning_message);
                    Common.Logger.Info(warning_message);
                    return;
                }

                Common.Logger.Info("属性値集計開始");

                this.Enabled = false;

                #region キーフィールド名、集計フィールド名を取得
                Common.LayerComboItem cmbItem = (Common.LayerComboItem)cmb_FeatureLayerName.SelectedItem;
                IFeatureLayer pFLayer = (IFeatureLayer)cmbItem.Layer;

                // 集計対象のフィールド番号を取得
                Common.FieldComboItem fItem = (Common.FieldComboItem)cmb_SummaryFieldName.SelectedItem;
                keyFieldNum = new int[listBox_SelectedKeyFields.Items.Count];
                sumFieldsNum = fItem.FieldIndex;
                for (int i = 0; i < listBox_SelectedKeyFields.Items.Count; i++)
                {
                    Common.FieldComboItem fieldCmbItem =
                        (Common.FieldComboItem)listBox_SelectedKeyFields.Items[i];
                    keyFieldNum[i] = fieldCmbItem.FieldIndex;
                }

                // キーフィールド文字列を配列に格納
                String[] strKeyFieldsName = new String[listBox_SelectedKeyFields.Items.Count];
                String strSubFields = "";
                for (int i = 0; i < listBox_SelectedKeyFields.Items.Count; i++)
                {
                    Common.FieldComboItem fieldCmbItem =
                        (Common.FieldComboItem)listBox_SelectedKeyFields.Items[i];

                    strKeyFieldsName[i] = Common.ExportFunctions2.GetFieldName(fieldCmbItem.Field.Name);
                    strSubFields += Common.ExportFunctions2.GetFieldName(fieldCmbItem.Field.Name);
                    strSubFields += ",";
                }
                Common.FieldComboItem sumField =
                    (Common.FieldComboItem)cmb_SummaryFieldName.SelectedItem;
                strSubFields += Common.ExportFunctions2.GetFieldName(sumField.Field.Name);
                #endregion

                Common.LayerComboItem layerItem =
                    (Common.LayerComboItem)cmb_FeatureLayerName.SelectedItem;

                /* 2010/10/19 Multi Thread */
                // 2012/07/25 PGDB,FGDB対応
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        saveFilePath = txt_SaveFilePath.Text;
                        break;
                    case 1:
                        saveFilePath = txtSaveFilePath.Text + @"\" + txtSaveFileName.Text;

                        break;
                }


                attributeSum(pFLayer, layerItem.Layer);

                Common.MessageBoxManager.ShowMessageBoxInfo(this,
                    Properties.Resources.FormAttributeSum_FinishedMessage);
            }
            catch (ApplicationException)
            {
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormAttributeSum_SumError);
                Common.Logger.Error(Properties.Resources.FormAttributeSum_SumError);
                Common.Logger.Error(comex.StackTrace);
                Common.Logger.Error(comex.Message);
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormAttributeSum_SumError);
                Common.Logger.Error(Properties.Resources.FormAttributeSum_SumError);
                Common.Logger.Error(ex.StackTrace);
                Common.Logger.Error(ex.Message);
            }
            finally
            {
                try
                {
                    if (System.IO.File.Exists(outTemporaryMDBFullPath))
                        System.IO.File.Delete(outTemporaryMDBFullPath);
                }
				catch (Exception ex)
				{
					Common.Logger.Error(ex.StackTrace);
					Common.Logger.Error(ex.Message);
				}
            }

            Common.Logger.Info("属性値集計終了");

            // スレッドメモリ解放
            if (threadTask != null)
            {
                if (threadTask.IsAlive)
                {
                    threadTask.Abort();
                }

                threadTask = null;
            }

            this.Enabled = true;

            this.Close();
        }
        
        /// <summary>
        /// フィーチャレイヤ選択コンボボックスの選択インデックスチェンジイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_FeatureLayerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelectionSet;
            IFeatureSelection pFeatureSelection;
            ISelectionSet pSelectionSet;

            try
            {
                listBox_SelectedKeyFields.Items.Clear();
                listBox_Fields.Items.Clear();
                cmb_SummaryFieldName.Items.Clear();

                Common.LayerComboItem cmbItem = (Common.LayerComboItem)cmb_FeatureLayerName.SelectedItem;
                IGeoFeatureLayer pGeoFLayer = (IGeoFeatureLayer)cmbItem.Layer;
                
				// ｷｰ･ﾌｨｰﾙﾄﾞ ﾘｽﾄを作成
				Common.FieldComboItem[]	cmbFlds = this.mainFrm.GetFieldItems(pGeoFLayer as ITableFields, true, false, false, true, 
														esriFieldType.esriFieldTypeSmallInteger,
														esriFieldType.esriFieldTypeInteger,
														esriFieldType.esriFieldTypeSingle,
														esriFieldType.esriFieldTypeDouble,
														esriFieldType.esriFieldTypeString);
				this.listBox_Fields.Items.AddRange(cmbFlds);
				
				// 集計ﾌｨｰﾙﾄﾞ ﾘｽﾄを作成
				cmbFlds = this.mainFrm.GetFieldItems(pGeoFLayer as ITableFields, true, false, false, true, 
														esriFieldType.esriFieldTypeSmallInteger,
														esriFieldType.esriFieldTypeInteger,
														esriFieldType.esriFieldTypeSingle,
														esriFieldType.esriFieldTypeDouble);
				this.cmb_SummaryFieldName.Items.AddRange(cmbFlds);
                
                if(cmb_SummaryFieldName.Items.Count > 0) {
					cmb_SummaryFieldName.Text = cmb_SummaryFieldName.Items[0].ToString();
                }
                this.btn_Run.Enabled = cmb_SummaryFieldName.Items.Count > 0;
                btn_AddField.Enabled = true;

                // 選択されているフィーチャのみ集計するチェックボックスをの処理
                pFeatureSelection = (IFeatureSelection)pGeoFLayer;
                pSelectionSet = pFeatureSelection.SelectionSet;
                hasSelectionSet = pSelectionSet != null && pSelectionSet.Count > 0;
                chkSelectionSet.Enabled = hasSelectionSet;
                chkSelectionSet.Checked = hasSelectionSet;
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.StackTrace);
                Common.Logger.Error(ex.Message);
            }
        }      

        /// <summary>
        /// [↑]をクリックした時に選択フィールド名を一つ上に移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Up_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBox_SelectedKeyFields.SelectedIndex;
                if (selectedIndex < 1) return; // 未選択あるいは最上位を選択している場合は処理しない

                Common.FieldComboItem fItem = (Common.FieldComboItem)listBox_SelectedKeyFields.Items[selectedIndex-1];
                listBox_SelectedKeyFields.Items[selectedIndex - 1] = listBox_SelectedKeyFields.Items[selectedIndex];
                listBox_SelectedKeyFields.Items[selectedIndex] = fItem;
                listBox_SelectedKeyFields.SelectedIndex = selectedIndex - 1;
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.StackTrace);
                Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// [↓]ボタンをクリックした時に選択フィールド名を一つ下に移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Down_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBox_SelectedKeyFields.SelectedIndex;
                if (selectedIndex == -1 || selectedIndex >= listBox_SelectedKeyFields.Items.Count - 1) return; // 未選択あるいは最下位を選択している場合は処理しない

                Common.FieldComboItem fItem = (Common.FieldComboItem)listBox_SelectedKeyFields.Items[selectedIndex + 1];
                listBox_SelectedKeyFields.Items[selectedIndex + 1] = listBox_SelectedKeyFields.Items[selectedIndex];
                listBox_SelectedKeyFields.Items[selectedIndex] = fItem;
                listBox_SelectedKeyFields.SelectedIndex = selectedIndex + 1;
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.StackTrace);
                Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 保存先ファイル名を取得する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SaveFile_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.FileName = cmb_FeatureLayerName.SelectedText;
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    this._filterIndex = saveFileDialog1.FilterIndex;
                    txt_SaveFilePath.Text = saveFileDialog1.FileName;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.StackTrace);
                Common.Logger.Error(ex.Message);
            }
        }
        

        /// <summary>
        /// 選択中のフィールドをキーフィールドとして追加する
        /// </summary>
        private void AddKeyField(object sender, EventArgs e)
        {
            try
            {
                Common.FieldComboItem fieldCmbItem = (Common.FieldComboItem)listBox_Fields.SelectedItem;

                if (listBox_Fields.Items.Count <= 0 || listBox_SelectedKeyFields.Items.Count >= this._MaxCount) return;
                for (int i = 0; i < listBox_SelectedKeyFields.Items.Count; i++)
                {
                    Common.FieldComboItem fItem = (Common.FieldComboItem)listBox_SelectedKeyFields.Items[i];
                    if (fieldCmbItem == fItem) return;
                }

                listBox_SelectedKeyFields.Items.Add(fieldCmbItem);

                if (listBox_SelectedKeyFields.Items.Count >= _MaxCount) btn_AddField.Enabled = false;
                else btn_AddField.Enabled = true;

            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.StackTrace);
                Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 選択中のキーフィールドを候補リストから削除
        /// </summary>
        private void DeleteKeyField(object sender, EventArgs e)
        {
            try
            {
                if (listBox_SelectedKeyFields.Items.Count <= 0 ||
                    listBox_SelectedKeyFields.SelectedIndex == -1)
                {
                    return;
                }
                listBox_SelectedKeyFields.Items.RemoveAt(listBox_SelectedKeyFields.SelectedIndex);

                // キーフィールド候補数で[追加]の状態を切り替え
                if (listBox_SelectedKeyFields.Items.Count >= _MaxCount)
                {
                    btn_AddField.Enabled = false;
                }
                else
                {
                    btn_AddField.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.StackTrace);
                Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 数値型フィールドを持つレイヤかどうかチェックする
        /// </summary>
        /// <param name="pLayer"></param>
        /// <returns></returns>
        private bool hasNumericalField(ESRI.ArcGIS.Carto.ILayer pLayer)
        {
            bool hasNumField = false;
            IFeatureLayer pFLayer = (ESRI.ArcGIS.Carto.IFeatureLayer)pLayer;
            ESRI.ArcGIS.Geodatabase.IFields pFields = pFLayer.FeatureClass.Fields;
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                ESRI.ArcGIS.Geodatabase.esriFieldType pType = pFields.get_Field(i).Type;
                // いずれかの数値型フィールドである場合はhasNumFieldをTrue
                if (pType == ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSmallInteger ||
                    pType == ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeInteger ||
                    pType == ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSingle ||
                    pType == ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeDouble)
                {
                    hasNumField = true;
                    break;
                }
            }
            return hasNumField;
        }

        /// <summary>
        /// 属性値集計に使用するSQL文を作成する
        /// </summary>
        /// <param name="mdbPath"></param>
        /// <param name="Layer"></param>
        /// <param name="sumField"></param>
        /// <param name="keyFields"></param>
        /// <returns></returns>
        private static String CreateSQLCommand2(String mdbPath, ILayer Layer, int sumField, params int[] keyFields)
        {
            IGeoFeatureLayer geoFLayer = (IGeoFeatureLayer)Layer;
            String LayerName = Layer.Name;
            StringBuilder tmpSQL = new StringBuilder();
            DataTable dtTbl = new DataTable();

            tmpSQL.Append("select * from ");
            tmpSQL.Append(LayerName);

            Common.ExportFunctions2.GetDataTabl_FromMDB(
                mdbPath, tmpSQL.ToString(), ref dtTbl);

            // MDBの2列目が集計フィールド
            String sumFieldName = dtTbl.Columns[1].ColumnName;
            StringBuilder sqlCommand = new StringBuilder("");

            #region SQLコマンドを生成
            sqlCommand.Append("SELECT ");
            for (int i = 0; i < keyFields.Length; i++)
            {
                // キー フィールド列
                String tmpName = dtTbl.Columns[i + 2].ColumnName; // MDBの3列目以降がキー フィールド
                String tmpAlName = Common.ExportFunctions2.GetFieldName(
                    geoFLayer.DisplayFeatureClass.Fields.get_Field(keyFields[i]).Name);
                sqlCommand.Append(tmpName);
//                sqlCommand.Append(" as ");
//                sqlCommand.Append(tmpAlName); // MDB内のフィールド名と元のキーフィールド名が異なる場合を考慮
                sqlCommand.Append(",");
            }
            sqlCommand.Append("SUM(");
            sqlCommand.Append(sumFieldName);
            sqlCommand.Append(") AS 合計,");
            sqlCommand.Append("AVG(");
            sqlCommand.Append(sumFieldName);
            sqlCommand.Append(") AS 平均,");
            sqlCommand.Append("COUNT(");
            sqlCommand.Append(sumFieldName);
            sqlCommand.Append(") AS 件数,");
            sqlCommand.Append("MAX(");
            sqlCommand.Append(sumFieldName);
            sqlCommand.Append(") AS 最大値,");
            sqlCommand.Append("MIN(");
            sqlCommand.Append(sumFieldName);
            sqlCommand.Append(") AS 最小値 ");
            sqlCommand.Append("FROM ");
            sqlCommand.Append(Layer.Name);
            sqlCommand.Append(" ");
            sqlCommand.Append("GROUP BY ");

            // GROUP BY以降の文字列を作成
            for (int i = 0; i < keyFields.Length; i++)
            {
                // MDBの3列目以降がキー フィールド
                sqlCommand.Append(dtTbl.Columns[i + 2].ColumnName);
                if (i < keyFields.Length - 1) sqlCommand.Append(",");
            }
            #endregion
            
            return sqlCommand.ToString();
        }

        /// <summary>
        /// 属性値集計に使用するSQL文を作成する
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="summaryField"></param>
        /// <param name="keyFields"></param>
        /// <returns></returns>
        private static String CreateSQLCommand(String tableName, String summaryField, params String[] keyFields)
        {
            StringBuilder sqlCommand = new StringBuilder("");

            #region SQLコマンドを生成
            sqlCommand.Append("SELECT ");
            for (int i = 0; i < keyFields.Length; i++)
            {
                sqlCommand.Append(keyFields[i]);
                sqlCommand.Append(",");
            }
            sqlCommand.Append("SUM(");
            sqlCommand.Append(summaryField);
            sqlCommand.Append(") AS 合計,");
            sqlCommand.Append("AVG(");
            sqlCommand.Append(summaryField);
            sqlCommand.Append(") AS 平均,");
            sqlCommand.Append("COUNT(");
            sqlCommand.Append(summaryField);
            sqlCommand.Append(") AS 件数,");
            sqlCommand.Append("MAX(");
            sqlCommand.Append(summaryField);
            sqlCommand.Append(") AS 最大値,");
            sqlCommand.Append("MIN(");
            sqlCommand.Append(summaryField);
            sqlCommand.Append(") AS 最小値 ");
            sqlCommand.Append("FROM ");
            sqlCommand.Append(tableName);
            sqlCommand.Append(" ");
            sqlCommand.Append("GROUP BY ");
            #endregion

            for (int i = 0; i < keyFields.Length; i++)
            {
                sqlCommand.Append(keyFields[i]);
                if (i < keyFields.Length - 1) sqlCommand.Append(",");
            }

            return sqlCommand.ToString();
        }

        /// <summary>
        /// 入力パラメータのチェック
        /// </summary>
        /// <returns></returns>
        private bool InParameterCheck(ref String message)
        {
            //bool ret = true;
            try
            {
                #region 入力チェック
                // Check1 全てのパラメータが入力されているか
                if (listBox_SelectedKeyFields.Items.Count <= 0)
                {
                    message = Properties.Resources.FormAttributeSum_NoSelectKeyField;
                    //ret = false;

                    return false;
                }
                
                // Check3 キーフィールドと集計フィールドが同じでないか
                for (int i = 0; i < listBox_SelectedKeyFields.Items.Count; i++)
                {
                    Common.FieldComboItem fItem1 = (Common.FieldComboItem)listBox_SelectedKeyFields.Items[i];
                    Common.FieldComboItem fItemSelected = (Common.FieldComboItem)cmb_SummaryFieldName.SelectedItem;
                    
					//if(fItem1==fItemSelected)
					//if (listBox_SelectedKeyFields.Items[i].ToString() == cmb_SummaryFieldName.SelectedItem.ToString())
					if (fItem1.Field.Equals(fItemSelected.Field))
					{
                        message = Properties.Resources.FormAttributeSum_KeySumFieldEqualed;
                        //ret = false;

                        return false;
                    }
                }

                // 2012/07/25 PGDB,FGDB対応
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        // Check2 出力ファイルが指定されているか
                        if (txt_SaveFilePath.Text == "")
                        {
                            message = Properties.Resources.FormAttributeSum_NoSelectOutput;
                            //ret = false;

                            return false;
                        }
                        break;
                    case 1:
                        // Check2 出力ファイルが指定されているか
                        //if (txtSaveFilePath.Text == "" && txtSaveFileName.Text == "")
                        if ((txtSaveFilePath.Text == "") || (txtSaveFileName.Text == ""))
                        {
                            message = Properties.Resources.FormAttributeSum_NoSelectOutput;
                            //ret = false;

                            return false;
                        }
                        break;
                }

                // Check4 出力ファイルのパスは正しいか
                String outPathName = System.IO.Path.GetDirectoryName(this.outTemporaryMDBFullPath);
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(this.outTemporaryMDBFullPath)))
                {
                    message = Properties.Resources.FormAttributeSum_InValidateOutputPath;
                    //ret = false;

                    return false;
                }

                // 2012/07/25 PGDB,FGDB対応
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        // Check5 出力フォルダが存在しない場合作成
                        string saveFileDirectoryPath = System.IO.Path.GetDirectoryName(txt_SaveFilePath.Text);
                        if (!System.IO.Directory.Exists(saveFileDirectoryPath))
                        {
                            System.IO.Directory.CreateDirectory(saveFileDirectoryPath);
                        }
                        break;
                    case 1:
                        // Check5 出力フォルダが存在しない場合作成
                        saveFileDirectoryPath = System.IO.Path.GetDirectoryName(txtSaveFilePath.Text);
                        if (!System.IO.Directory.Exists(saveFileDirectoryPath))
                        {
                            System.IO.Directory.CreateDirectory(saveFileDirectoryPath);
                        }
                        break;
                }

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //return ret;
        }

        /// <summary>
        /// テンポラリ用MDBファイル名を作成する
        /// </summary>
        /// <returns></returns>
        private static String GenerateTempMDBFileName()
        {
            Guid guid = Guid.NewGuid();
            StringBuilder sb = new StringBuilder("");
            //sb.Append("EJ_GISLight_AttSum_");
            sb.Append("GISLight10_AttSum_");
            sb.Append(guid.ToString());
            sb.Append(".mdb");
            return sb.ToString();
        }

        private static String CreateFullPath(String dirPath, String fileName)
        {
            StringBuilder fullPath = new StringBuilder();
            fullPath.Append(System.IO.Path.GetFullPath(dirPath));
            fullPath.Append("\\");
            fullPath.Append(fileName);
            return fullPath.ToString();
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
        }

        /// <summary>
        /// 例外の再スロー(COMException)
        /// </summary>
        /// <param name="comex">例外(COMException)</param>
        private void RethrowCOMException(COMException comex)
        {
            Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormAttributeSum_SumError);
            Common.Logger.Error(Properties.Resources.FormAttributeSum_SumError);
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
                    this, Properties.Resources.FormAttributeSum_SumError);
            Common.Logger.Error(Properties.Resources.FormAttributeSum_SumError);
            Common.Logger.Error(ex.Message);
            Common.Logger.Error(ex.StackTrace);

            throw new ApplicationException();
        }

        /// <summary>
        /// 属性値集計処理
        /// </summary>
        /// <param name="obj">属性値集計処理を行うためのパラメータ</param>
        private void AttributeSumThread(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                IFeatureLayer featureLayer = propertySet.GetProperty("FeatureLayer") as IFeatureLayer;
                ILayer itemLayer = propertySet.GetProperty("ItemLayer") as IFeatureLayer;

                frmProgress.IncrementProgressBar(2);

                // テンポラリ用MDBに出力
                Common.ExportFunctions2.ExportMDB(outTemporaryWorkspace,
                    outTemporaryMDBName,
                    featureLayer,
                    featureLayer.Name,
                    sumFieldsNum,
                    chkSelectionSet.Checked,
                    keyFieldNum);

                frmProgress.IncrementProgressBar(4);

                // 属性値を集計するためのSQLコマンドを生成
                sqlCommand = CreateSQLCommand2(
                    outTemporaryMDBFullPath,
                    itemLayer,
                    sumFieldsNum,
                    keyFieldNum);

                frmProgress.IncrementProgressBar(6);

                // 2012/07/19 mdbに対応
                // 集計とCSV/DBF出力
                if (_filterIndex == 1)
                {
                    Common.ExportFunctions2.ExportToCSV(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }
                else if (_filterIndex == 2)
                {
                    Common.ExportFunctions2.ExportToDBF(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }
                else if (_filterIndex == 3)
                {
                    Common.ExportFunctions2.ExportToMDB(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }
                else if (_filterIndex == 4)
                {
                    Common.ExportFunctions2.ExportToGDB(
                        outTemporaryMDBFullPath, sqlCommand, saveFilePath);
                }

                frmProgress.IncrementProgressBar(8);

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

        /// <summary>
        /// 保存先DB名を取得する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            using (FormGISDataSelectDialog frm = new FormGISDataSelectDialog())
            {
                frm.SelectType = FormGISDataSelectDialog.ReturnType.GdbWorkspaceName;
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    IWorkspaceName wsName = ((List<IWorkspaceName>)frm.SelectedObject)[0];
                    //MessageBox.Show(wsName.PathName);
                    txtSaveFilePath.Text = wsName.PathName;
                    if (wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.AccessWorkspaceFactory"))
                    {
                        this._filterIndex = 3;
                    }
                    else if (wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.FileGDBWorkspaceFactory"))
                    {
                        this._filterIndex = 4;
                    }
                }
            }


        }
    }

}
