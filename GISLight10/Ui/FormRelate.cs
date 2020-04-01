using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// リレート
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormRelate : Form
    {
		private const string RELATE_NAME_PREFIX = "リレート";
		
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;
        // 2012/07/19 mdbに対応
        //private string selectTable = null;

        //private IDatasetName selectDatasetName = null;
        // 選択結合先ﾃｰﾌﾞﾙ (但しｸｴﾘ ﾃｰﾌﾞﾙを除く)
        private ITable selectTable = null;

        // 別名表示設定
        private bool	_blnUseAlias;

        // 結合ﾃｰﾌﾞﾙ･ﾘｽﾄ
        private List<Common.StandaloneTableUnitClass>	_agSelTables = new List<ESRIJapan.GISLight10.Common.StandaloneTableUnitClass>();
		// ﾌｨｰﾙﾄﾞ･ﾘｽﾄ用
        private List<Common.FieldComboItem[]>			_agSelFields = new List<Common.FieldComboItem[]>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormRelate(ESRI.ArcGIS.Controls.IMapControl3 mapControl)
        {
            InitializeComponent();

            this.m_mapControl = mapControl;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();

            Common.Logger.Info("リレートフォームの起動");

            // 別名表示設定を取得
            this._blnUseAlias = (Control.FromHandle(((IntPtr)this.m_mapControl.hWnd)).FindForm() as MainForm).IsUseFieldAlias;

            List<IFeatureLayer> fcLayerList = null;

            try
            {
                // 既存単独ﾃｰﾌﾞﾙの読み込み
                this.comboBox_DestinationTable.Visible = false;
                if((this.m_mapControl.Map as IStandaloneTableCollection).StandaloneTableCount > 0) {
					// ﾃｰﾌﾞﾙ選択ｺﾝﾎﾞを使用する
					IStandaloneTableCollection	agStdColl = this.m_mapControl.Map as IStandaloneTableCollection;
					IStandaloneTable			agStdTbl;
					for(int intCnt=0; intCnt < agStdColl.StandaloneTableCount; intCnt++) {
						agStdTbl = agStdColl.get_StandaloneTable(intCnt);
						if(agStdTbl.Valid) {
							// 選択用ﾊﾟｯｹｰｼﾞを作成
							Common.StandaloneTableUnitClass	clsStd = new ESRIJapan.GISLight10.Common.StandaloneTableUnitClass() {
								TableName = agStdTbl.Name,
								StdTable = agStdTbl,
							};
							// ﾃｰﾌﾞﾙ選択に追加
							this._agSelTables.Add(clsStd);

							// ﾌｨｰﾙﾄﾞ･ﾘｽﾄを作成
							this.AddComboItemField(-1, agStdTbl);
						}
					}
					
					// ｺﾝﾄﾛｰﾙ制御
					if(this._agSelTables.Count > 0) {
						// ｺﾝﾎﾞを配置
						this.comboBox_DestinationTable.Size = new Size(this.textBoxDestinationTable.Width, this.comboBox_DestinationTable.Height);
						this.comboBox_DestinationTable.Location = new Point(this.textBoxDestinationTable.Left, this.textBoxDestinationTable.Top);
						this.comboBox_DestinationTable.Visible = true;
						this.textBoxDestinationTable.Visible = false;

						// 候補ﾘｽﾄを作成
						this.comboBox_DestinationTable.Items.AddRange(this._agSelTables.ToArray());
					}
                }

                //フィーチャレイヤ一覧の取得
                ESRIJapan.GISLight10.Common.LayerManager layerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();
                fcLayerList = layerManager.GetFeatureLayers(m_mapControl.Map);

                //フィーチャレイヤ一覧をコンボボックスに追加
                if (fcLayerList.Count > 0)
                {
                    AddComboItemLayer(comboSourceLayers, fcLayerList);
                }
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_GetSrcLayers);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_GetSrcLayers);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);

                this.Close();
                //this.Dispose();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_GetSrcLayers);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_GetSrcLayers);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Close();
                //this.Dispose();
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(fcLayerList);
            }
        }

		#region プロパティ
		/// <summary>
		/// リレート先テーブルの完全パスを取得または設定します
		/// </summary>
		private string DestinationTableName {
			get {
				string	strRet = "";
				// 既存単独ﾃｰﾌﾞﾙがない場合
				if(this.textBoxDestinationTable.Visible) {
					strRet = this.textBoxDestinationTable.Text;
				}
				else if(this.comboBox_DestinationTable.SelectedIndex >= 0) {
					var	clsST = this.comboBox_DestinationTable.SelectedItem as Common.StandaloneTableUnitClass;
					
					if(clsST.StdTable != null && clsST.StdTable.Table != null) {
						strRet = Common.StandAloneTableOpener.GetFullPath(clsST.StdTable);
					}
					else {
						strRet = this.comboBox_DestinationTable.Text;
					}
				}
				else {
					//
				}
				
				return strRet;
			}
			set {
				// 既存単独ﾃｰﾌﾞﾙがない場合
				if(this.textBoxDestinationTable.Visible) {
					this.textBoxDestinationTable.Text = value ?? "";

					// ﾌｨｰﾙﾄﾞ･ﾘｽﾄを作成
					if(selectTable != null) {
						// 一般ﾃｰﾌﾞﾙ
						this.AddComboItemField(this.comboDestinationKeyField, this.selectTable.Fields);
					}
					else {
						// ｸｴﾘｰﾃｰﾌﾞﾙ
						this.AddQueryTableFields(ConnectionFilesManager.LoadWorkspace(value) as ISqlWorkspace, Common.StandAloneTableOpener.GetTableName(value));
					}
				}
				else if(!string.IsNullOrEmpty(value)) {
					// 既に読み込まれているﾃﾞｰﾀかどうかﾁｪｯｸ
					bool	blnExist = false;
					foreach(Common.StandaloneTableUnitClass clsTU in this.comboBox_DestinationTable.Items) {
						// ShapeWS時のDBF補完の為、ﾃｰﾌﾞﾙも照合
						if(clsTU.Equals(value) || (this.selectTable != null ? clsTU.Equals(this.selectTable) : false)) {
							// 選択項目を合わせる
							this.comboBox_DestinationTable.SelectedItem = clsTU;
							blnExist = true;
							break;
						}
					}
					
					if(!blnExist) {
						// 選択ﾊﾟｯｹｰｼﾞを作成
						Common.StandaloneTableUnitClass	clsTU = new ESRIJapan.GISLight10.Common.StandaloneTableUnitClass() {
							TableName = Common.StandAloneTableOpener.GetTableName(value),	// ﾃｰﾌﾞﾙ名を抽出
							WorkSpacePath = Path.GetDirectoryName(value),
						};
						
						// 選択項目に追加
						this.comboBox_DestinationTable.Items.Insert(0, clsTU);
						this.comboBox_DestinationTable.SelectedIndex = 0;
						
						// ﾌｨｰﾙﾄﾞ･ﾘｽﾄを作成
						if(selectTable != null) {
							// 一般ﾃｰﾌﾞﾙ
							clsTU.Tag = this.selectTable;	// ﾃｰﾌﾞﾙを潜ませる
							this.AddComboItemField(0, this.selectTable);
							this.selectTable = null;
						}
						else {
							// ｸｴﾘ ﾃｰﾌﾞﾙ
							this.AddQueryTableFields(ConnectionFilesManager.LoadWorkspace(value) as ISqlWorkspace, Common.StandAloneTableOpener.GetTableName(value));
						}
					}
				}
				else {
					// ﾃｽﾄ確認
					string	strErr = "Value is Null.";
				}
			}
		}
		#endregion

		/// <summary>
		/// クエリテーブルのフィールドリストを作成します
		/// </summary>
		/// <param name="agSqlWS">SQLワークスペース</param>
		/// <param name="TableName">テーブル名</param>
		private void AddQueryTableFields(ISqlWorkspace agSqlWS, string TableName) {
			// ｸｴﾘ ﾃｰﾌﾞﾙの読み込み
			Common.QueryTableOperator	clsQTO = new ESRIJapan.GISLight10.Common.QueryTableOperator(agSqlWS);
			clsQTO.TableName = TableName;
										
			// 手動でﾌｨｰﾙﾄﾞﾘｽﾄを作成
			IStringArray	agSA_ColNames = new StrArrayClass();
			IStringArray	agSA_ColTypes = new StrArrayClass();
			IVariantArray	agVA_IsNulls = new VarArrayClass();
			ILongArray		agLA_Sizes = new LongArrayClass();
			ILongArray		agLA_Precs = new LongArrayClass();
			ILongArray		agLA_Scales = new LongArrayClass();
			
			clsQTO.GetColumnInfos(TableName, 
				out agSA_ColNames, out agSA_ColTypes, out agVA_IsNulls, out agLA_Sizes, out agLA_Precs, out agLA_Scales);
			
			IFieldsEdit	agFlds = new FieldsClass();
			IFieldEdit	agFld;
			string		strColName;
			string		strColType;
			
			// 連結可能ﾌｨｰﾙﾄﾞ名を取得
			string[]	strRelFields = clsQTO.RelateableFields;
			
			for(int intCnt=0; intCnt < agSA_ColNames.Count; intCnt++) {
				// ﾌｨｰﾙﾄﾞ名を取得
				strColName = agSA_ColNames.get_Element(intCnt);
				if(strRelFields.Contains(strColName)) {
					// ﾌｨｰﾙﾄﾞを作成
					agFld = new FieldClass() {
						IFieldEdit_Name_2 = strColName,
						IFieldEdit_Required_2 = (bool)agVA_IsNulls.get_Element(intCnt),
						IFieldEdit_Length_2 = agLA_Sizes.get_Element(intCnt),
						IFieldEdit_Scale_2 = agLA_Scales.get_Element(intCnt),
						IFieldEdit_Precision_2 = agLA_Precs.get_Element(intCnt),
					};
				
					// ﾃﾞｰﾀ型を取得
					strColType = agSA_ColTypes.get_Element(intCnt);
					switch(strColType.ToLower()) {
					case "long integer":
						agFld.Type_2 = esriFieldType.esriFieldTypeInteger;
						break;
					case "short integer":
						agFld.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
						break;
					case "double":
						agFld.Type_2 = esriFieldType.esriFieldTypeDouble;
						break;
					case "float":
						agFld.Type_2 = esriFieldType.esriFieldTypeSingle;
						break;
					case "text":
						agFld.Type_2 = esriFieldType.esriFieldTypeString;
						break;
					}
					
					// 追加
					agFlds.AddField(agFld);
				}
			}
			
			// ｺﾝﾎﾞにﾌｨｰﾙﾄﾞを追加
			if(!this.comboBox_DestinationTable.Visible) {
				this.AddComboItemField(this.comboDestinationKeyField, agFlds);
			}
			else {
				this.AddComboItemField(0, agFlds);
			}
		}

        /// <summary>
        /// リレート元レイヤ コンボボックス 選択変更時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboSourceLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ESRIJapan.GISLight10.Common.FieldManager fieldManager = null;

            try {
                //コンボボックスの選択アイテムの取得
                ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
                    (ESRIJapan.GISLight10.Common.LayerComboItem)comboSourceLayers.SelectedItem;

                //コンボボックスにフィールドを追加
				AddComboItemField(comboSourceKeyField, selectedLayerItem.Layer as ITableFields);

                // ﾘﾚｰﾄ名を調整
                int	intRelID = 1;
                while(!this.CheckSameRelateName(selectedLayerItem.Layer as IFeatureLayer, string.Format(RELATE_NAME_PREFIX + "{0}", intRelID++)));
                this.textBoxRelateName.Text = string.Format(RELATE_NAME_PREFIX + "{0}", --intRelID);
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_GetSrcKeyField);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_GetSrcKeyField);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_GetSrcKeyField);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_GetSrcKeyField);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(fieldManager);
            }
        }


        /// <summary>
        /// リレート先テーブルを開くボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            //ITable destinationTable = null;
            //IFields fields = null;
            IWorkspace ws = null;
            IFeatureWorkspace fws = null;

			IDatasetName	selectDatasetName = null;
			
            try
            {
                using (FormGISDataSelectDialog frm = new FormGISDataSelectDialog())
                {
                    frm.SelectType = FormGISDataSelectDialog.ReturnType.TableOrText;
                    if(!this.textBoxDestinationTable.Text.Equals("")) {
						frm.StartFolder = this.textBoxDestinationTable.Text;
                    }
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    { 
                        //selectDatasetName = null;
                        selectTable = null;

                        // 通常のﾃｰﾌﾞﾙ等
                        if(frm.SelectedObject is List<IDatasetName>) {
							selectDatasetName = ((List<IDatasetName>)frm.SelectedObject)[0];
							IWorkspaceName wsName = selectDatasetName.WorkspaceName;
							IWorkspaceFactory wsFact = wsName.WorkspaceFactory;
							ws = wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
							fws = (IFeatureWorkspace)ws;

							selectTable = fws.OpenTable(selectDatasetName.Name);
						}
                        // DB ｸｴﾘ ﾃｰﾌﾞﾙ　※ｸｴﾘ ﾃｰﾌﾞﾙ時は選択ﾃｰﾌﾞﾙのｲﾝｽﾀﾝｽを取得しない
                        else if(frm.SelectedObject is List<Common.UserSelectQueryTableSet>) {
							// ｵﾌﾞｼﾞｪｸﾄを取得
							Common.UserSelectQueryTableSet	objDT = (frm.SelectedObject as List<Common.UserSelectQueryTableSet>)[0];
							
							this.DestinationTableName = objDT.ConnectionFile + "\\" + objDT.TableName;
                        }
                    }
                }

                // ﾃｰﾌﾞﾙ名を表示
                if (selectDatasetName != null)
                {
                    // Flat Table
                    if (selectDatasetName.WorkspaceName.WorkspaceFactoryProgID.Contains("esriDataSourcesFile.ShapefileWorkspaceFactory"))
                    {
                        this.DestinationTableName = string.Format("{0}\\{1}", selectDatasetName.WorkspaceName.PathName, selectDatasetName.Name + ".dbf");
                    }
                    // (F/P)GDB
                    else
                    {
                        this.DestinationTableName = string.Format("{0}\\{1}", selectDatasetName.WorkspaceName.PathName, selectDatasetName.Name);
                    }
                }



            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_GetDesrKeyField);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_GetDesrKeyField);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_GetDesrKeyField);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_GetDesrKeyField);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                //ComReleaser.ReleaseCOMObject(destinationTable);
                //ComReleaser.ReleaseCOMObject(fields);
                if (fws != null)
                    ComReleaser.ReleaseCOMObject(fws);
                if (ws != null)
                    ComReleaser.ReleaseCOMObject(ws);
            }
        }


        /// <summary>
        /// OKボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            IFeatureLayer sourceFcLayer = null;
            ITable destinationTable = null;
            IField sourceKeyField = null;
            IField destinationKeyField = null;

            ESRIJapan.GISLight10.Common.RelateFunctions relateFunction = null;
            try
            {
                /***** 入力チェック（パラメータが入力されているか確認）*****/
                if (comboSourceLayers.SelectedIndex == -1)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_NoSrcLayer);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_NoSrcLayer);
                    return;
                }

                if (comboSourceKeyField.SelectedIndex == -1)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_NoSrcKeyField);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_NoSrcKeyField);
                    return;
                }

                // 2012/07/26 mdb,gdbに対応
                String output_path = this.DestinationTableName;
                if (output_path.ToLower().IndexOf(".mdb") > 0 ||
                    output_path.ToLower().IndexOf(".gdb") > 0)
                {
                    output_path = System.IO.Path.GetDirectoryName(output_path);
                }

                if (output_path.Equals(""))
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_NoDestTable);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_NoDestTable);
                    return;
                }
                else if (!File.Exists(output_path))
                {
                    // 2012/07/23 gdbに対応
                    string fileExtension = Path.GetExtension(output_path);
                    if (String.Compare(fileExtension, ".gdb", true) == 0)
                    {
                        if (!Directory.Exists(output_path))
                        {
                            ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                                (this, Properties.Resources.FormJoinTable_WARNING_NotExistDestTable);
                            Common.Logger.Warn(Properties.Resources.FormJoinTable_WARNING_NotExistDestTable);
                            return;
                        }
                    }
                    // ｸｴﾘ･ﾃｰﾌﾞﾙに対応
                    else if(!Common.QueryTableOperator.IsValidConnectionFile(Common.QueryTableOperator.GetConnectionFilePath(output_path))) {
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                            (this, Properties.Resources.FormRelate_WARNING_NotExistDestTable);
                        Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_NotExistDestTable);
                        return;
                    }
                }

                if (comboDestinationKeyField.SelectedIndex == -1)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_NoDestKeyField);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_NoDestKeyField);
                    return;
                }

                //リレート名のチェック
                if (textBoxRelateName.Text.Equals(""))
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_NoRelateName);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_NoRelateName);
                    return;
                }
                /**************************************/


                /***** パラメータ値の取得 ******/                
                //リレート元レイヤの取得
                ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
                    (ESRIJapan.GISLight10.Common.LayerComboItem)comboSourceLayers.SelectedItem;
                sourceFcLayer = (IFeatureLayer)selectedLayerItem.Layer;

                //リレート先テーブルの取得
                //ESRIJapan.GISLight10.Common.StandAloneTableOpener openTable =
                //    new ESRIJapan.GISLight10.Common.StandAloneTableOpener();
                //// 2012/07/19 mdbに対応
                //selectTable = System.IO.Path.GetFileNameWithoutExtension(textBoxDestinationTable.Text);

                //destinationTable = openTable.OpenCsvAndDbfTable(output_path, ref selectTable);
                //destinationTable = selectTable;


                //リレート元レイヤのキーフィールドを取得
                ESRIJapan.GISLight10.Common.FieldComboItem sourceFieldItem =
                    (ESRIJapan.GISLight10.Common.FieldComboItem)comboSourceKeyField.SelectedItem;
                sourceKeyField = sourceFieldItem.Field;

                //リレート先レイヤのキーフィールドを取得
                ESRIJapan.GISLight10.Common.FieldComboItem destinationFieldItem =
                    (ESRIJapan.GISLight10.Common.FieldComboItem)comboDestinationKeyField.SelectedItem;
                destinationKeyField = destinationFieldItem.Field;

                //リレート名の取得
                string relateName = textBoxRelateName.Text;
                /****************************/
                


                /***** 入力チェック（リレート名の禁則文字,同名確認）*****/                
                //キーフィールドの型の一致確認（数値 or テキスト）
                bool checkKeyFieldType = CheckKeyFieldType(sourceKeyField, destinationKeyField);
                if (checkKeyFieldType == false)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_JoinTextAndInt);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_JoinTextAndInt);
                    return;
                }


                //リレート名の禁則文字確認
                bool chekBadRelateName = CheckBadRelateName(relateName);
                if (chekBadRelateName == false)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_BadRelateName);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_BadRelateName);
                    return;
                }

                //リレート名がすでに使用されているか確認
                bool checkSameRelateName = CheckSameRelateName(sourceFcLayer, relateName);
                if (checkSameRelateName == false)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_SameRelateName);
                    Common.Logger.Warn(Properties.Resources.FormRelate_WARNING_SameRelateName);
                    return;
                }
                /***************************/

				// ﾃｰﾌﾞﾙﾚｲﾔｰが存在している場合、結合対象をｾｯﾄ。 ※存在しない場合は、既にｾｯﾄ済み。
				if(this.comboBox_DestinationTable.Visible) {
					Common.StandaloneTableUnitClass	clsTU = this.comboBox_DestinationTable.SelectedItem as Common.StandaloneTableUnitClass;
					if(clsTU.StdTable != null) {
						this.selectTable = clsTU.StdTable.Table;
					}
					else if(clsTU.Tag != null) {
						this.selectTable = clsTU.Tag as ITable;
					}
				}
				
				// ｸｴﾘ･ﾃｰﾌﾞﾙは、ここで初めて読み込む(ｷｰ･ﾌｨｰﾙﾄﾞ確定)
				if(this.selectTable == null) {
					destinationTable = this.LoadSqlTable(this.comboBox_DestinationTable.SelectedItem as Common.StandaloneTableUnitClass, destinationKeyField.Name);
				}
				else {
					destinationTable = this.selectTable;
				}

                //テーブルを開けなかった場合（CSVがExcelでロックされているときなど）
                if (destinationTable == null)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRelate_WARNING_CantOpenDestTable);
                    Common.Logger.Info(Properties.Resources.FormRelate_WARNING_CantOpenDestTable);

                    return;
                }

                //Common.RelateFunctions relateFunction =
                relateFunction =
                    new ESRIJapan.GISLight10.Common.RelateFunctions
                        (sourceFcLayer, destinationTable,
                        sourceKeyField.Name, destinationKeyField.Name, relateName);


                Common.Logger.Info("リレート実行");

                //リレートの実行
                relateFunction.Relate();

                //フォームを閉じる
                this.Close();
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_DoRelate);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_DoRelate);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRelate_ERROR_DoRelate);
                Common.Logger.Error(Properties.Resources.FormRelate_ERROR_DoRelate);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                if (relateFunction != null)
                    relateFunction = null;
                //ComReleaser.ReleaseCOMObject(sourceFcLayer);
                //ComReleaser.ReleaseCOMObject(destinationTable);
                //ComReleaser.ReleaseCOMObject(sourceKeyField);
                //ComReleaser.ReleaseCOMObject(destinationKeyField);                
            }
        }

        /// <summary>
        /// クエリテーブルを読み込みます
        /// </summary>
        /// <param name="StandaloneTblUnit"></param>
        /// <param name="PKFieldName"></param>
        /// <returns></returns>
        private ITable LoadSqlTable(Common.StandaloneTableUnitClass StandaloneTblUnit, string PKFieldName) {
			ITable	agTbl = null;
			
			// ｸｴﾘ ﾃｰﾌﾞﾙの読み込み
			ISqlWorkspace				agSqlWS = ConnectionFilesManager.LoadWorkspace(StandaloneTblUnit.WorkSpacePath) as ISqlWorkspace;
			Common.QueryTableOperator	clsQT = new ESRIJapan.GISLight10.Common.QueryTableOperator(agSqlWS);
			clsQT.TableName = StandaloneTblUnit.TableName;
			clsQT.OIDFields = PKFieldName;
			
			// ﾃｰﾌﾞﾙを取得
			agTbl = clsQT.GetTable(null);
			
			return agTbl;
        }

        /// <summary>
        /// キャンセルボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// フォーム クローズ時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormRelate_FormClosing(object sender, FormClosingEventArgs e)
        {
            Common.Logger.Info("リレートフォームの終了");
        }  

		/// <summary>
		/// 結合先テーブルの選択変更 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBox_DestinationTable_SelectedIndexChanged(object sender, EventArgs e) {
			ComboBox	ctlCB = sender as ComboBox;

			if(ctlCB.SelectedIndex >= 0) {
				// ﾌｨｰﾙﾄﾞ選択をｸﾘｱ
				if(this.comboDestinationKeyField.Items.Count > 0) {
					this.comboDestinationKeyField.Items.Clear();
				}
				// 対象ﾃｰﾌﾞﾙの選択ﾌｨｰﾙﾄﾞをｾｯﾄ
				if(this._agSelFields[ctlCB.SelectedIndex] != null) {
					this.comboDestinationKeyField.Items.AddRange(this._agSelFields[ctlCB.SelectedIndex]);
					this.comboDestinationKeyField.SelectedIndex = 0;
				}
			}
		}

        //privateメソッド
        #region

        /// <summary>
        /// コンボボックスにレイヤを追加
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        /// <param name="fcLayerList">レイヤ一覧</param>
        private void AddComboItemLayer(ComboBox combobox, List<IFeatureLayer> fcLayerList)
        {
            for (int i = 0; i < fcLayerList.Count; i++)
            {
                ESRIJapan.GISLight10.Common.LayerComboItem layerItem =
                    new ESRIJapan.GISLight10.Common.LayerComboItem(fcLayerList[i]);

                combobox.Items.Add(layerItem);
            }

            //先頭のレイヤを選択
            if (combobox.Items.Count > 0)
            {
                combobox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// コンボボックスにフィールドを追加 (外部読み込みテーブルが対象)
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        /// <param name="fields">フィールド一覧</param>
        private void AddComboItemField(ComboBox combobox ,IFields fields) {
            combobox.Items.Clear();            

            //コンボボックスにフィールド名を追加
            IField						agFld;
            List<Common.FieldComboItem>	cmbItems = new List<ESRIJapan.GISLight10.Common.FieldComboItem>();
            for(int i = 0; i < fields.FieldCount; i++) {
                agFld = fields.get_Field(i);

                // ﾃﾞｰﾀ型で抽出 (日付型は対象外)
                if (agFld.Type == esriFieldType.esriFieldTypeSmallInteger ||
                    agFld.Type == esriFieldType.esriFieldTypeInteger ||
                    agFld.Type == esriFieldType.esriFieldTypeSingle ||
                    agFld.Type == esriFieldType.esriFieldTypeDouble ||
                    agFld.Type == esriFieldType.esriFieldTypeString ||
                    agFld.Type == esriFieldType.esriFieldTypeOID)
                {
                    // ﾘｽﾄに追加
                    cmbItems.Add(new ESRIJapan.GISLight10.Common.FieldComboItem(agFld, i));
                }
            }

            // ｺﾝﾄﾛｰﾙに設定
            if(cmbItems.Count > 0) {
				combobox.Items.AddRange(cmbItems.ToArray());
                combobox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// コンボボックスにフィールドを追加 (レイヤーが対象)
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        /// <param name="FieldInfos">フィールド情報</param>
        private void AddComboItemField(ComboBox combobox, ITableFields FieldInfos) {
            combobox.Items.Clear();
            
            // 対象ﾌｨｰﾙﾄﾞ･ｱｲﾃﾑを取得
            Common.FieldComboItem[]	cmbFlds = this.mainFrm.GetFieldItems(FieldInfos, true, false, false, true, 
													esriFieldType.esriFieldTypeSmallInteger,
													esriFieldType.esriFieldTypeInteger,
													esriFieldType.esriFieldTypeSingle,
													esriFieldType.esriFieldTypeDouble,
													esriFieldType.esriFieldTypeString,
													esriFieldType.esriFieldTypeOID);

            // ﾘｽﾄ･ｺﾝﾄﾛｰﾙに設定
            if(cmbFlds.Length > 0) {
				combobox.Items.AddRange(cmbFlds);
                combobox.SelectedIndex = 0;    
            }            
        }

		/// <summary>
		/// 選択ﾌｨｰﾙﾄﾞ･ｱｲﾃﾑを作成します 
		/// </summary>
		/// <param name="ListIndex"></param>
		/// <param name="Fields"></param>
        private void AddComboItemField(int ListIndex, IFields Fields) {
			bool						blnOK = false;
			List<Common.FieldComboItem>	clsFldItems = new List<Common.FieldComboItem>();

			// 入力ﾁｪｯｸ
			if(Fields != null) {
				// ﾌｨｰﾙﾄﾞ･ﾘｽﾄを作成
				IField	agFld;
				IClone	agClone;
				
				for(int intCnt=0; intCnt < Fields.FieldCount; intCnt++) {
					// ﾌｨｰﾙﾄﾞを取得
					agFld = Fields.get_Field(intCnt);
					
					// 選択対象ﾌｨｰﾙﾄﾞを抽出
					if(agFld.Type == esriFieldType.esriFieldTypeSmallInteger ||
						agFld.Type == esriFieldType.esriFieldTypeInteger ||
						agFld.Type == esriFieldType.esriFieldTypeSingle ||
						agFld.Type == esriFieldType.esriFieldTypeDouble ||
						agFld.Type == esriFieldType.esriFieldTypeString ||
						agFld.Type == esriFieldType.esriFieldTypeOID) {
						// ﾌｨｰﾙﾄﾞのｺﾋﾟｰを作成
						agClone = agFld as IClone;
						// 選択ｱｲﾃﾑを作成
						Common.FieldComboItem cbiFld = new Common.FieldComboItem(agClone.Clone() as IField);
						clsFldItems.Add(cbiFld);
						
						if(!blnOK) {
							blnOK = true;
						}
					}
				}
			}
			
			// ﾁｪｯｸ
			if(blnOK) {
				// 保存
				if(ListIndex < 0 || ListIndex >= this._agSelFields.Count) {
					// 追加
					this._agSelFields.Add(clsFldItems.ToArray());
				}
				else {
					// 挿入
					this._agSelFields.Insert(ListIndex, clsFldItems.ToArray());
				}
			}
			else {
				this._agSelFields.Add(null);
			}
        }
		private void AddComboItemField(int ListIndex, ITable TargetTable) {
			// ｵｰﾊﾞｰ･ﾛｰﾄﾞ
			this.AddComboItemField(ListIndex, TargetTable.Fields);
		}
		private void AddComboItemField(int ListIndex, IStandaloneTable TargetTable) {
			IDisplayTable	agDTbl = TargetTable as IDisplayTable;
			
			// ｵｰﾊﾞｰ･ﾛｰﾄﾞ
			this.AddComboItemField(ListIndex, agDTbl.DisplayTable);
		}

        /// <summary>
        /// キーフィールドの型の一致確認（数値 or 文字列）
        /// </summary>
        /// <param name="srcKeyField">リレート元レイヤのキーフィールド</param>
        /// <param name="destKeyField">リレート先テーブルのキーフィールド</param>
        /// <returns>一致:true、不一致:false</returns>
        private bool CheckKeyFieldType(IField srcKeyField, IField destKeyField)
        {
            bool checkResult = true;

            if (srcKeyField.Type == esriFieldType.esriFieldTypeString)
            {
                if (destKeyField.Type != esriFieldType.esriFieldTypeString)
                {
                    checkResult = false;
                }
            }
            else
            {
                if (destKeyField.Type == esriFieldType.esriFieldTypeString)
                {
                    checkResult = false;
                }
            }

            return checkResult;
        }


        /// <summary>
        /// リレート名がすでに使用されているか確認
        /// </summary>
        /// <param name="srcFcLayer">リレート元レイヤ</param>
        /// <param name="relateName">リレート名</param>
        /// <returns>未使用:true、使用中:false</returns>
        private bool CheckSameRelateName(IFeatureLayer srcFcLayer, string relateName)
        {
            bool checkResult = true;

            //リレート元レイヤのRelationshipClass一覧の取得
            IRelationshipClassCollection relClassCol = (IRelationshipClassCollection)srcFcLayer;
            IEnumRelationshipClass enumRelClass = relClassCol.RelationshipClasses;
            
            enumRelClass.Reset();
            IRelationshipClass relClass = enumRelClass.Next();
            
            IDataset dataSet = null;            

            while (relClass != null)
            {
                dataSet = (IDataset)relClass;

                if (relateName.Equals(dataSet.Name))
                {
                    checkResult = false;
                    break;
                }

                relClass = enumRelClass.Next();                
            }

            return checkResult;
        }


        /// <summary>
        /// リレート名の禁則文字確認
        /// </summary>
        /// <param name="name"></param>
        /// <returns>含まない:true、含む:false</returns>
        private bool CheckBadRelateName(string name)
        {
            bool checkResult = true;

            if (name.Contains("\\") == true ||
                name.Contains("/") == true ||
                name.Contains(":") == true ||
                name.Contains("*") == true ||
                name.Contains("?") == true ||
                name.Contains("\"") == true ||
                name.Contains("<") == true ||
                name.Contains(">") == true ||
                name.Contains("|") == true)
            {
                checkResult = false;
            }

            return checkResult;
        }

        #endregion
    }
        
}
