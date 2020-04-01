using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class LightCatalogView : UserControl
    {
		// ﾌｫﾙﾀﾞに接続 ﾄｯﾌﾟﾉｰﾄﾞ名
		private const string FOLDER_CONNECT_TOPTEXT = "フォルダ接続";
		private const string FOLDER_CONNECT_TOPNAME = "<ESRIJapanGIS_FolderConnectTopNode>";
		// ﾃﾞｰﾀﾍﾞｰｽ接続
		private const string DATABASE_CONNECT_TOPTEXT = "データベース接続";
		private const string DATABASE_CONNECT_TOPNAME = "<ESRIJapanGIS_DatabaseConnectTopNode>";
		// ArcGIS ｻｰﾊﾞ接続
		private const string ARCGISSERVER_CONNECT_TOPTEXT = "ArcGIS サーバ接続";
		private const string ARCGISSERVER_CONNECT_TOPNAME = "<ESRIJapanGIS_ArcGISServerConnectTopNode>";

        private string selectPath = "";
        private object selectObject = null;
        private bool returnObject = false;
        private string startFolder = "";
        //private SELECTTYPE selectType = SELECTTYPE.Unknown;
        //private int selectDatasetType = (int)esriDatasetType.esriDTAny;

        private bool blInit = false; // なぜかLightに追加したらLoadが3回走るための対応策
        // IWorkspaceFactoryを頻繁に作成するとたまにRCW例外が発生するため使いまわす
        // またsingletonであるIWorksapceFactoryはnewを使わないでSingletonUtilityを使う
        private IWorkspaceFactory shapeWsf = null;
        private IWorkspaceFactory rasterWsf = null;
        private IWorkspaceFactory cadWsf = null;
        private IWorkspaceFactory accessWsf = null;
        private IWorkspaceFactory fgdbWsf = null;
        private IWorkspaceFactory textWsf = null;

        private bool	_blnShowConnectFolder;
        private bool	_blnShowDBConnect;
        private bool	_blnShowGISServer;

        #region プロパティ
        public string StartFolder
        {
            set { startFolder = value; }
        }

        public bool ReturnObject
        {
            get { return returnObject; }
        }

        public object SelectObject
        {
            get { return selectObject; }
        }

        public string SelectPath
        {
            get { return selectPath; }
        }

        /// <summary>
        /// フォルダ接続を表示するかどうかを取得または設定します
        /// </summary>
        public bool ShowConnectFolder {
			get {
				return this._blnShowConnectFolder;
			}
			set {
				this._blnShowConnectFolder = value;
				this.toolStrip1.Visible = this._blnShowConnectFolder;
			}
        }

        /// <summary>
        /// データベース接続を表示するかどうかを取得または設定します
        /// </summary>
        public bool ShowDBConnect {
			get {
				return this._blnShowDBConnect;
			}
			set {
				this._blnShowDBConnect = value;

				this.SetVisibleTools(this._blnShowDBConnect, "D");
			}
        }

        /// <summary>
        /// ArcGISサーバー接続を表示するかどうかを取得または設定します
        /// </summary>
        public bool ShowGISServer {
			get {
				return this._blnShowGISServer;
			}
			set {
				this._blnShowGISServer = value;

				this.SetVisibleTools(this._blnShowGISServer, "S");
			}
        }
        #endregion

        /// <summary>
        /// ツールバーボタンの表示を制御します
        /// </summary>
        /// <param name="IsVisible">表示設定</param>
        /// <param name="Identifier">対象コントロール識別子</param>
        private void SetVisibleTools(bool IsVisible, string Identifier) {
			string	strTag;
			foreach(ToolStripItem tool in toolStrip1.Items) {
				if(tool.Tag != null) {
					strTag = (string)tool.Tag;
					if(strTag.StartsWith(Identifier)) {
						tool.Visible = IsVisible;
					}
				}
			}
        }

        //public SELECTTYPE SelectType
        //{
        //    get { return selectType; }
        //}

        //internal enum SELECTTYPE
        //{
        //    Unknown = 0,
        //    MapDocument = 1,
        //    ShapeFile = 2,
        //    LayerFile = 3,
        //    ProjectFile = 4,
        //    CadFile = 5,
        //    MdbFile = 6,
        //    Folder =7,
        //    GdbFolder = 8,
        //    RasterFile = 9
        //}

        #region ICON(=imgListIcon)
        public enum ICON
        {
            FolderWindowsStyle = 0,
            Geodatabase = 1,
            GeoprocessingToolbox = 2,
            ArcMap_MXD_File = 3,
            Folder = 4,
            FolderOpenState = 5,
            GeodatabaseFeatureClassAnnotation = 6,
            GeodatabaseFeatureClassLine = 7,
            GeodatabaseFeatureClassMultipoint = 8,
            GeodatabaseFeatureClassPoint = 9,
            GeodatabaseFeatureClassPolygon = 10,
            GeodatabaseFeatureDataset = 11,
            GeodatabaseMosaicDataset = 12,
            GeodatabaseNetworkDataset = 13,
            GeodatabaseRasterCatalog = 14,
            GeodatabaseTable = 15,
            LayerGroupNew = 16,
            /* Shape File */
            ShapefileLine = 17,
            ShapefilePoint = 18,
            ShapefilePolygon = 19,
            ShapefileUnknown = 59,	/* 追加 */
            TableStandalone = 20,
            TableExcel = 21,
            GeodatabaseRaster = 22,
            GeodatabaseRasterGrid = 23,
            GeodatabaseRelationship = 24,
            GeodatabaseRasterGridBand = 25,
            GeodatabaseNetworkGeometric = 26,
            GeodatabaseTerrain = 27,
            GeodatabaseTopology = 28,
            GeocodeAddressLocator = 29,
            CADDataset = 30,
            FileMosaicDataset = 31,
            FileNetworkDataset = 32,
            FileRasterGrid = 33,
            FileRasterGridBand = 34,
            GeodatabaseFeatureClassMultipatch = 35,
            Layer_LYR_File = 36,
            CoordinateSystem = 37,
            TableDBase = 38,
            TextFile = 39,
            /* ﾌｫﾙﾀﾞｰ接続 */
            ConnectFolder_Add = 40,
            ConnectFolder_Root = 41,
            ConnectFolder_Root_Open = 42,
            ConnectFolder_UserFolder = 43,
            ConnectFolder_UserFolder_Open = 44,
            ConnectFolder_Disconnect = 45,
            ConnectFolder_Rename = 46,
            ConnectFolder_Refresh = 47,
            /* ﾃﾞｰﾀﾍﾞｰｽ接続 */
            DatabaseConnect_Root = 48,
            DatabaseConnect_Add = 49,
            DatabaseConnect_Disconnect = 50,
            DatabaseConnect_DB = 51,
            StandaloneTable__Root = 52,
            /* ArcGIS ｻｰﾊﾞ接続 */
            ArcGISServerConnect_Root = 53,
            ArcGISServerConnect_Add = 54,
            ArcGISServerIConnect_Disconnect = 55,	/* www */
            ArcGISServerIConnect_Server = 56,
            ArcGISServerLConnect_Disconnect = 57,	/* Local */
            ArcGISServerLConnect_Server = 58,
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LightCatalogView() {
            InitializeComponent();

            // 表示設定を既定
            this.ShowConnectFolder = true;
            this.ShowDBConnect = false;
            this.ShowGISServer = false;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~LightCatalogView() {
            ReleaseWsfs();
        }

        private void ReleaseWsfs() {
            if (shapeWsf != null)
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(shapeWsf);
            if (rasterWsf != null)
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rasterWsf);
            if (cadWsf != null)
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(cadWsf);
            if (accessWsf != null)
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(accessWsf);
            if (fgdbWsf != null)
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(fgdbWsf);
            if (textWsf != null)
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(textWsf);

            //GC.Collect();
        }

        private void tvWs_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            System.Windows.Forms.Cursor preCursor = System.Windows.Forms.Cursor.Current;
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            try
            {
                TreeNode parentNode = e.Node;

                int optionfilter = 0; //オプションフィルターは未実施

                SetWorkspaceNode(parentNode, optionfilter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "エラー!! BeforeExpandイベント");
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = preCursor;
            }
        }

        /// <summary>
        /// フォーム・ロード イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightCatalogView_Load(object sender, EventArgs e) {
            if (blInit)
                return;

            // ﾂﾘｰの設定
            tvWs.Nodes.Clear();
            tvWs.ImageList = imgListIcon;
            tvWs.ItemHeight = 18;
            tvWs.LineColor = Color.FromArgb(140,140,140);
            tvWs.ShowRootLines = true;
            tvWs.ShowPlusMinus = true;
            tvWs.ShowLines = false;
            tvWs.Indent = 22;

			int			intFCID;
			TreeNode[]	nodeRegFolders;
            #region フォルダ接続の設定
            if(this.ShowConnectFolder) {
				// ﾌｫﾙﾀﾞ接続のﾄｯﾌﾟﾉｰﾄﾞを追加
				intFCID = tvWs.Nodes.Add(new TreeNode(FOLDER_CONNECT_TOPTEXT, (int)ICON.ConnectFolder_Root, (int)ICON.ConnectFolder_Root) {
					Name = FOLDER_CONNECT_TOPNAME,
				});
				// 登録ﾌｫﾙﾀﾞｰを取得, ﾉｰﾄﾞを追加
				nodeRegFolders = ConnectFoldersManager.GetConnectFolders();
				if(nodeRegFolders.Length > 0) {
					tvWs.Nodes[intFCID].Nodes.AddRange(nodeRegFolders);
					tvWs.Nodes[intFCID].Expand();
				}
            }
            #endregion

            #region データベース接続の設定
            if(this.ShowDBConnect) {
				// ﾃﾞｰﾀﾍﾞｰｽ接続のﾄｯﾌﾟﾉｰﾄﾞを追加
				intFCID = tvWs.Nodes.Add(new TreeNode(DATABASE_CONNECT_TOPTEXT, (int)ICON.DatabaseConnect_Root, (int)ICON.DatabaseConnect_Root) {
					Name = DATABASE_CONNECT_TOPNAME,
				});
				// 登録DBを取得, ﾉｰﾄﾞを追加
				nodeRegFolders = ConnectionFilesManager.GetDBConnectFiles();
				if(nodeRegFolders.Length > 0) {
					tvWs.Nodes[intFCID].Nodes.AddRange(nodeRegFolders);
					tvWs.Nodes[intFCID].Expand();
				}
			}
			#endregion

			#region ArcGIS サーバー接続の設定
			if(this.ShowGISServer) {
				// ArcGISｻｰﾊﾞ接続のﾄｯﾌﾟﾉｰﾄﾞを追加
				intFCID = tvWs.Nodes.Add(new TreeNode(ARCGISSERVER_CONNECT_TOPTEXT, (int)ICON.ArcGISServerConnect_Root, (int)ICON.ArcGISServerConnect_Root) {
					Name = ARCGISSERVER_CONNECT_TOPNAME,
				});

			}
			#endregion

            // 接続ドライブをノードに追加
            foreach(DriveInfo driveInfo in DriveInfo.GetDrives()) {
                if(driveInfo.IsReady) {
                    //TreeNode treeNode = new TreeNode(driveInfo.Name.Replace("\\", ""));
                    TreeNode treeNode = new TreeNode(driveInfo.Name);
                    treeNode.Nodes.Add("");
                    tvWs.Nodes.Add(treeNode);
                }
            }

            // 既定の指定ﾊﾟｽをﾁｪｯｸ
            string	strDefaultPath = "";
            if(!string.IsNullOrEmpty(this.startFolder)) {
				if(Directory.Exists(this.startFolder) || File.Exists(this.startFolder)) {
					// 一般ﾌｫﾙﾀﾞ or ﾌｧｲﾙ
					strDefaultPath = this.startFolder;
				}
				else {
					// ｼﾞｵﾃﾞｰﾀﾍﾞｰｽ
					strDefaultPath = Path.GetDirectoryName(this.startFolder);
					if((strDefaultPath.ToLower().EndsWith(".gdb") && Directory.Exists(strDefaultPath))
						|| (strDefaultPath.ToLower().EndsWith(".mdb") && File.Exists(strDefaultPath))) {
						strDefaultPath = this.startFolder;
					}
					else {
						strDefaultPath = "";
					}
				}
            }
            // 既定のﾊﾟｽを開く
            if(!strDefaultPath.Equals("")) {
                // ﾌｫﾙﾀﾞｰ接続ﾉｰﾄﾞを調べる
                bool	blnHit = false;
                if(this.ShowConnectFolder) {
					TreeNode	nodeFC = tvWs.Nodes.Find(FOLDER_CONNECT_TOPNAME, false)[0];
					TreeNode	nodeTemp;
					foreach(TreeNode nodeChild in nodeFC.Nodes) {
						if(strDefaultPath.StartsWith(nodeChild.Name)) {
							string		strChildPath = strDefaultPath.Replace(nodeChild.Name + "\\", "");
							string[]	strChildPaths = strChildPath.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

							nodeTemp = nodeChild;
							foreach(string strFolder in strChildPaths) {
								this.SetWorkspaceNode(nodeTemp, 0);
								nodeTemp.Expand();
								nodeTemp = this.FindChildNode(nodeTemp, strFolder);
							}
							if(nodeTemp.Nodes.Count > 0) {
								this.SetWorkspaceNode(nodeTemp, 0);
								nodeTemp.Expand();
							}
							//tvWs.TopNode = nodeTemp.Parent;
							tvWs.TopNode = nodeTemp;
							tvWs.SelectedNode = nodeTemp;
							this.tvWs_AfterSelect(tvWs, new TreeViewEventArgs(nodeTemp));
							tvWs.Select();

							blnHit = true;
							break;
						}
					}
                }

                // ﾌｫﾙﾀﾞｰを探索
                if(!blnHit) {
					string root = System.IO.Path.GetPathRoot(strDefaultPath);
					string[] snames = strDefaultPath.Split(new[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
					foreach(TreeNode treeNode in tvWs.Nodes) {
						if(root.Contains(treeNode.Text)) // Drive
						{
							treeNode.Nodes.Clear();
							// ドライブ直下

							TreeNode childNode = new TreeNode(snames[1]);
							TreeNode[] nodes = new TreeNode[snames.Length - 1];
							string fullpath = treeNode.Text;
							for(int i = 1; i < snames.Length; i++) {
								nodes[i-1] = new TreeNode(snames[i]);
								fullpath += "\\" + snames[i];
								if(i > 1) {
									if(System.IO.Directory.Exists(fullpath)) {
										nodes[i - 2].Nodes.Add(nodes[i - 1]);
									}
								}
							}
							treeNode.Nodes.Add(nodes[0]);
							//treeNode.Expand();
							// 指定のパスだけExpandしていくロジックが必要
							treeNode.ExpandAll();

							TreeNode child = null;
							for(int i = 1; i < snames.Length; i++) {
								if(i == 1) {
									child = FindChildNode(treeNode, snames[i]);
									child.Expand();
								}
								else {
									child = FindChildNode(child, snames[i]);
									SetWorkspaceNode(child, 0);
									child.Expand();
								}
							}
							if(child.Nodes.Count > 0) {
								this.SetWorkspaceNode(child, 0);
								child.Expand();
							}
							tvWs.TopNode = child.Parent;
							tvWs.SelectedNode = child;
							this.tvWs_AfterSelect(tvWs, new TreeViewEventArgs(child));
							tvWs.Select();
						}
					}
                }
            }

            // イベントハンドラを追加
            this.tvWs.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvWs_BeforeExpand);
            this.tvWs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvWs_AfterSelect);

            if(this.tvWs.AllowDrop) {
                this.tvWs.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvWs_ItemDrag);
            }

            // ﾎﾞﾀﾝ･ｲﾒｰｼﾞを設定
            this.toolStripButton_FC.Image = this.imgListIcon.Images[40];
            this.toolStripButton_FD.Image = this.imgListIcon.Images[45];
            this.toolStripButton_FD.Enabled = false;
            this.toolStripButton_RN.Image = this.imgListIcon.Images[46];
            this.toolStripButton_RN.Enabled = false;
            this.toolStripButton_FR.Image = this.imgListIcon.Images[47];
            this.toolStripButton_FR.Enabled = false;
            this.toolStripButton_DC.Image = this.imgListIcon.Images[49];
            this.toolStripButton_DD.Image = this.imgListIcon.Images[50];
            this.toolStripButton_DD.Enabled = false;
            this.toolStripButton_SC.Image = this.imgListIcon.Images[54];
            this.toolStripButton_SD.Image = this.imgListIcon.Images[55];
            this.toolStripButton_SD.Enabled = false;

            blInit = true;
        }

        private TreeNode FindChildNode(TreeNode parentNode, string chileName)
        {

            foreach (TreeNode childNode in parentNode.Nodes)
            {
                if (childNode.Text.Equals(chileName))
                {
                    return childNode;
                }
            }

            return null;
        }

        /// <summary>
        /// ツリーノードのフルパスを取得します
        /// </summary>
        /// <param name="TargetNode"></param>
        /// <returns></returns>
        private string GetTreeNodeFullPath(TreeNode TargetNode) {
			string		strRet = TargetNode.FullPath;

			// 親が接続ﾌｫﾙﾀﾞの場合、ﾌﾙﾊﾟｽを構成
			if(strRet.StartsWith(FOLDER_CONNECT_TOPTEXT)) {
				if(TargetNode.Level > 0) {
					if(!TargetNode.Parent.Name.Equals(FOLDER_CONNECT_TOPNAME)) {
						TreeNode	nodeCF = TargetNode;
						TreeNode	nodeTemp = TargetNode.Parent;
						while(!nodeTemp.Name.Equals(FOLDER_CONNECT_TOPNAME)) {
							nodeCF = nodeTemp;
							nodeTemp = nodeCF.Parent;
						}
						// ﾊﾟｽを調整
						strRet = strRet.Replace(nodeTemp.Text + "\\" + nodeCF.Text, nodeCF.Name);
					}
					else {
						// 接続ﾌｫﾙﾀﾞ
						strRet = TargetNode.Name;
					}
				}
				else {
					strRet = "";
				}
			}
			// 親がDB接続の場合、接続ﾌｧｲﾙのﾊﾟｽを構成
			else if(strRet.StartsWith(DATABASE_CONNECT_TOPTEXT)) {
				if(TargetNode.Level > 0) {
					if(!TargetNode.Parent.Name.Equals(DATABASE_CONNECT_TOPNAME)) {
						TreeNode	nodeCF = TargetNode;
						TreeNode	nodeTemp = TargetNode.Parent;
						while(!nodeTemp.Name.Equals(DATABASE_CONNECT_TOPNAME)) {
							nodeCF = nodeTemp;
							nodeTemp = nodeCF.Parent;
						}
						// ﾊﾟｽを調整
						strRet = strRet.Replace(nodeTemp.Text + "\\" + nodeCF.Text, nodeCF.Name);
					}
					else {
						// 接続ﾌｫﾙﾀﾞ
						strRet = TargetNode.Name;
					}
				}
				else {
					strRet = "";
				}
			}

			return strRet;
        }

        /// <summary>
        /// GDB内のデータタイプに応じてTreeNodeを作成
        /// </summary>
        /// <param name="dsName"></param>
        /// <returns></returns>
        private TreeNode CreateGdbDatasetNode(IDatasetName dsName)
        {
            TreeNode node = new TreeNode(dsName.Name);

            List<IDatasetName> list = new List<IDatasetName>();
            list.Add(dsName);
            node.Tag = list;

            #region データタイプによりICONを変更
            switch (dsName.Type)
            {
                case esriDatasetType.esriDTAny:
                    break;
                case esriDatasetType.esriDTCadDrawing:
                    break;
                case esriDatasetType.esriDTCadastralFabric:
                    break;
                case esriDatasetType.esriDTContainer:
                    break;
                case esriDatasetType.esriDTFeatureClass:
                    IFeatureClassName fcName = (IFeatureClassName)dsName;
                    switch (fcName.ShapeType)
                    {
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultiPatch:
                            node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassMultipatch;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                            node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassMultipoint;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                            node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassPoint;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                            if (fcName.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassAnnotation;
                            }
                            else
                            {
                                node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassPolygon;
                            }
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                            node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassLine;
                            break;
                    }
                    break;
                case esriDatasetType.esriDTFeatureDataset:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureDataset;
                    // FeatureDatasetの下のデータセットを追加
                    node.Nodes.Clear();
                    IEnumDatasetName subEnumDsName = dsName.SubsetNames;
                    subEnumDsName.Reset();
                    IDatasetName subDsName = subEnumDsName.Next();
                    while (subDsName != null)
                    {
                        TreeNode subnode = new TreeNode(subDsName.Name);
                        List<IDatasetName> sublist = new List<IDatasetName>();
                        sublist.Add(subDsName);
                        subnode.Tag = sublist;

                        switch (subDsName.Type)
                        {
                            case esriDatasetType.esriDTFeatureClass:
                                IFeatureClassName subFcName = (IFeatureClassName)subDsName;
                                switch (subFcName.ShapeType)
                                {
                                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultiPatch:
                                        node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassMultipatch;
                                        break;
                                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                                        subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassMultipoint;
                                        break;
                                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                                        subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassPoint;
                                        break;
                                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                                        if (subFcName.FeatureType == esriFeatureType.esriFTAnnotation)
                                        {
                                            subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassAnnotation;
                                        }
                                        else
                                        {
                                            subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassPolygon;
                                        }
                                        break;
                                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                                        subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseFeatureClassLine;
                                        break;
                                }
                                break;
                            case esriDatasetType.esriDTRelationshipClass:
                                subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseRelationship;
                                break;
                            case esriDatasetType.esriDTGeometricNetwork:
                                subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseNetworkGeometric;
                                break;
                            case esriDatasetType.esriDTNetworkDataset:
                                subnode.ImageIndex = subnode.SelectedImageIndex = (int)ICON.GeodatabaseNetworkDataset;
                                break;
                        }

                        node.Nodes.Add(subnode);

                        subDsName = subEnumDsName.Next();
                    }
                    break;
                case esriDatasetType.esriDTGeo:
                    break;
                case esriDatasetType.esriDTGeometricNetwork:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseNetworkGeometric;
                    break;
                case esriDatasetType.esriDTLayer:
                    break;
                case esriDatasetType.esriDTLocator: // LocatorがDatasetNameでとれない
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeocodeAddressLocator;
                    break;
                case esriDatasetType.esriDTMap:
                    break;
                case esriDatasetType.esriDTMosaicDataset:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseMosaicDataset;
                    break;
                case esriDatasetType.esriDTNetworkDataset:
                    node.ImageIndex = node.ImageIndex = (int)ICON.FileNetworkDataset;
                    break;
                case esriDatasetType.esriDTPlanarGraph:
                    break;
                case esriDatasetType.esriDTRasterBand:
                    break;
                case esriDatasetType.esriDTRasterCatalog:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseRasterCatalog;
                    break;
                case esriDatasetType.esriDTRasterDataset:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.FileRasterGrid;
                    break;
                case esriDatasetType.esriDTRelationshipClass:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseRelationship;
                    break;
                case esriDatasetType.esriDTRepresentationClass:
                    break;
                case esriDatasetType.esriDTSchematicDataset:
                    break;
                case esriDatasetType.esriDTStyle:
                    break;
                case esriDatasetType.esriDTTable:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.TableStandalone;
                    break;
                case esriDatasetType.esriDTTerrain:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseTerrain;
                    break;
                case esriDatasetType.esriDTText:
                    break;
                case esriDatasetType.esriDTTin:
                    break;
                case esriDatasetType.esriDTTool:
                    break;
                case esriDatasetType.esriDTToolbox:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeoprocessingToolbox;
                    break;
                case esriDatasetType.esriDTTopology:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.GeodatabaseTopology;
                    break;
                default:
                    break;
            }
            #endregion

            return node;
        }

        /// <summary>
        /// シェープファイルのタイプに応じてTreeNodeを作成
        /// </summary>
        /// <param name="dsName"></param>
        /// <returns></returns>
        private TreeNode CreateShapeDatasetNode(IDatasetName dsName)
        {
            TreeNode node = new TreeNode(dsName.Name + ".shp");
            List<IDatasetName> list = new List<IDatasetName>();
            list.Add(dsName);

            node.Tag = list;

            #region データタイプによりICONを変更
            switch (dsName.Type)
            {
                case esriDatasetType.esriDTAny:
                    break;
                case esriDatasetType.esriDTCadDrawing:
                    break;
                case esriDatasetType.esriDTCadastralFabric:
                    break;
                case esriDatasetType.esriDTContainer:
                    break;
                case esriDatasetType.esriDTFeatureClass:
                    IFeatureClassName fcName = (IFeatureClassName)dsName;
                    switch (fcName.ShapeType) {
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                        node.ImageIndex = node.SelectedImageIndex = (int)ICON.ShapefilePoint;
                        break;
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                        node.ImageIndex = node.SelectedImageIndex = (int)ICON.ShapefilePoint;
                        break;
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                        node.ImageIndex = node.SelectedImageIndex = (int)ICON.ShapefilePolygon;
                        break;
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                        node.ImageIndex = node.SelectedImageIndex = (int)ICON.ShapefileLine;
                        break;
                    default:	// ｼﾞｵﾒﾄﾘ不明
						node.ImageIndex = node.SelectedImageIndex = (int)ICON.ShapefileUnknown;
						break;
                    }
                    break;
                case esriDatasetType.esriDTFeatureDataset:
                    break;
                case esriDatasetType.esriDTGeo:
                    break;
                case esriDatasetType.esriDTGeometricNetwork:
                    break;
                case esriDatasetType.esriDTLayer:
                    break;
                case esriDatasetType.esriDTLocator:
                    break;
                case esriDatasetType.esriDTMap:
                    break;
                case esriDatasetType.esriDTMosaicDataset:
                    break;
                case esriDatasetType.esriDTNetworkDataset:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.FileNetworkDataset;
                    break;
                case esriDatasetType.esriDTPlanarGraph:
                    break;
                case esriDatasetType.esriDTRasterBand:
                    break;
                case esriDatasetType.esriDTRasterCatalog:
                    break;
                case esriDatasetType.esriDTRasterDataset:
                    break;
                case esriDatasetType.esriDTRelationshipClass:
                    break;
                case esriDatasetType.esriDTRepresentationClass:
                    break;
                case esriDatasetType.esriDTSchematicDataset:
                    break;
                case esriDatasetType.esriDTStyle:
                    break;
                case esriDatasetType.esriDTTable:
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.TableStandalone;
                    break;
                case esriDatasetType.esriDTTerrain:
                    break;
                case esriDatasetType.esriDTText:
                    break;
                case esriDatasetType.esriDTTin:
                    break;
                case esriDatasetType.esriDTTool:
                    break;
                case esriDatasetType.esriDTToolbox:
                    break;
                case esriDatasetType.esriDTTopology:
                    break;
                default:
                    break;
            }
            #endregion

            return node;
        }

        private TreeNode CreateTextDatasetNode(IDatasetName dsName)
        {
            TreeNode node = new TreeNode(dsName.Name);
            List<IDatasetName> list = new List<IDatasetName>();
            list.Add(dsName);

            node.Tag = list;
            node.ImageIndex = node.SelectedImageIndex = (int)ICON.TextFile;

            return node;
        }

        private TreeNode CreateDbfDatasetNode(IDatasetName dsName)
        {
            TreeNode node = new TreeNode(dsName.Name + ".dbf");
            List<IDatasetName> list = new List<IDatasetName>();
            list.Add(dsName);

            node.Tag = list;
            node.ImageIndex = node.SelectedImageIndex = (int)ICON.TableStandalone;

            return node;
        }


        /// <summary>
        /// TreeNode下のディレクトリとデータを変更するメインの機能
        /// </summary>
        /// <param name="parentNode"></param>
        private void SetWorkspaceNode(TreeNode parentNode, int option)
        {
            string fullpath = this.GetTreeNodeFullPath(parentNode);

            #region TreeNodeがGeoDBの場合
            IsGDBReturn	retGDB = this.IsGdbWorkspace(fullpath);
            if(retGDB == IsGDBReturn.GeoDB) {
                parentNode.Nodes.Clear(); // FGDB内のツリー削除

                //IWorkspace ws = GetFgdbWorkspace(fullpath);//GetGdbWorkspace(fullpath);
                IWorkspace ws = null;
                if(System.IO.Directory.Exists(fullpath)) {
                    ws = GetFgdbWorkspace(fullpath);
                }
                else {
                    ws = GetPgdbWorkspace(fullpath);
                }
                if(ws == null)
                    return;

                IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTAny);
                enumDsName.Reset();
                IDatasetName dsName = enumDsName.Next();
                while(dsName != null) {
                    TreeNode node = CreateGdbDatasetNode(dsName);
                    parentNode.Nodes.Add(node);
                    dsName = enumDsName.Next();
                }
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            }
            // 破損GDBの場合は、ｴﾗｰ表示
            else if(retGDB == IsGDBReturn.Corrupt) {
				parentNode.Nodes.Clear();
				Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.LightCatalogView_OPEN_MDB_ERROR);
            }
            #endregion

            #region TreeNodeがフォルダの場合
            else if (System.IO.Directory.Exists(fullpath))
            {
                parentNode.Nodes.Clear();

                // 未接続表示の場合
                if(parentNode.ImageIndex.Equals((int)ICON.ConnectFolder_Disconnect)) {
					parentNode.ImageIndex = ICON.ConnectFolder_UserFolder.GetHashCode();
					parentNode.SelectedImageIndex = ICON.ConnectFolder_UserFolder.GetHashCode();
                }

                DirectoryInfo dirInfo = new DirectoryInfo(fullpath);

                #region サブフォルダ
                foreach (DirectoryInfo dir in dirInfo.GetDirectories().OrderBy(d=>d.Name)) {
                    //System.IO.FileAttributes attr = dir.Attributes;
                    if(!dir.ToString().ToUpper().Equals("INFO")) {
                        if (!dir.Attributes.ToString().ToUpper().Contains("HIDDEN")) // 隠しフォルダは対象外とする
                        {
                            // アクセス許可がある場合だけ
                            try {
                                TreeNode node = new TreeNode(dir.Name);
                                IWorkspace ws = null;
                                if(IsGdbWorkspace(dir.FullName) == IsGDBReturn.GeoDB) // FGDB
                                {
                                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.Geodatabase;
                                    List<IWorkspaceName> wsList = new List<IWorkspaceName>();
                                    ws = GetFgdbWorkspace(dir.FullName);
                                    IWorkspaceName wsName = GetWorkspaceName(ws);
                                    wsList.Add(GetWorkspaceName(ws));
                                    node.Tag = wsList;
                                }
                                else // 通常のフォルダ
                                {
                                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.Folder;
                                    // フォルダ下のチェック-- CAD, RASTER, ShapeFile, TEXT, DBF
                                    List<IWorkspaceName> fwsList = new List<IWorkspaceName>();
                                    if (IsCadWorkspace(dir.FullName))
                                    {
                                        ws = GetCadWorkspace(dir.FullName);
                                        fwsList.Add(GetWorkspaceName(ws));
                                    }
                                    else if (IsShapeWorkspace(dir.FullName))
                                    {
                                        ws = GetShapeWorkspace(dir.FullName);
                                        fwsList.Add(GetWorkspaceName(ws));
                                    }
                                    else if (IsRasterWorkspace(dir.FullName))
                                    {
                                        ws = GetRasterWorkspace(dir.FullName);
                                        fwsList.Add(GetWorkspaceName(ws));
                                    }
                                    else if (IsTextWorkspace(dir.FullName))
                                    {
                                        ws = GetTextWorkspace(dir.FullName);
                                        fwsList.Add(GetWorkspaceName(ws));
                                    }
                                    else if (IsDbfWorkspace(dir.FullName))
                                    {
                                        ws = GetShapeWorkspace(dir.FullName);
                                        fwsList.Add(GetWorkspaceName(ws));
                                    }
                                    if (fwsList.Count > 0)
                                    {
                                        node.Tag = fwsList;
                                    }
                                }
                                if (ws != null)
                                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);

                                node.Nodes.Add("");
                                parentNode.Nodes.Add(node);
                            }
                            catch(UnauthorizedAccessException unauthex) {
								Debug.WriteLine(unauthex.Message);
                                continue;
                            }
                        }
                    }
                }
                #endregion

                #region CADデータの存在チェック
                if (IsCadWorkspace(fullpath))
                {
                    IWorkspace ws = GetCadWorkspace(fullpath);
                    IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTCadDrawing);
                    enumDsName.Reset();
                    IDatasetName dsName = enumDsName.Next();
                    while (dsName != null)
                    {
                        TreeNode node = new TreeNode(dsName.Name);
                        node.ImageIndex = node.SelectedImageIndex = (int)ICON.CADDataset;
                        List<IDatasetName> cadList = new List<IDatasetName>();
                        cadList.Add(dsName);
                        node.Tag = cadList;
                        parentNode.Nodes.Add(node);
                        dsName = enumDsName.Next();
                    }
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                }
                #endregion

                #region ラスタデータの存在チェック
                if (IsRasterWorkspace(fullpath))
                {
#if DEBUG
					// 検証
					this.ConfirmECWRasters(fullpath);

		            Common.Logger.Info("カタログビュー : ラスターフォルダー展開 : " + fullpath);
					//ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxInfo(this, "ラスターフォルダを展開");
#endif
                    try {
						IWorkspace ws = GetRasterWorkspace(fullpath);

#if DEBUG
						Common.Logger.Info("ラスターWS取得 : " + ws.PathName);
#endif

						IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTRasterDataset);
						enumDsName.Reset();
						IDatasetName dsName = enumDsName.Next();

#if DEBUG
						if(dsName == null) {
							Common.Logger.Info("IDatasetNameを取得 : " + "ラスターなし(DatasetName = Null)");
						}
						int intCnt = 0;
#endif

						while (dsName != null)
						{
#if DEBUG
							// ﾃﾞｰﾀｾｯﾄ名を記録
							Common.Logger.Info(string.Format("IDatasetName{0} : {1}", ++intCnt, dsName.Name));
#endif

							TreeNode node = new TreeNode(dsName.Name);
							node.ImageIndex = node.SelectedImageIndex = (int)ICON.FileRasterGrid;
							List<IDatasetName> rasList = new List<IDatasetName>();
							rasList.Add(dsName);
							node.Tag = rasList;
							parentNode.Nodes.Add(node);
							dsName = enumDsName.Next();
						}
						ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
						ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                    }
                    catch(Exception ex) {
			            Common.Logger.Error("ラスターフォルダー展開時にエラー発生");
		                Common.UtilityClass.DoOnError(ex);

#if DEBUG
						//ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(this, "ラスターノード展開中にエラー発生");
#endif
                    }
                }
                #endregion

                #region シェープファイルとDBFの存在チャック
                if (IsShapeWorkspace(fullpath))
                {
                    IWorkspace ws = GetShapeWorkspace(fullpath);
                    IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                    enumDsName.Reset();
                    IDatasetName dsName = enumDsName.Next();
                    while (dsName != null)
                    {
                        TreeNode node = CreateShapeDatasetNode(dsName);
                        parentNode.Nodes.Add(node);
                        dsName = enumDsName.Next();
                    }

                    enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTTable);
                    enumDsName.Reset();
                    dsName = enumDsName.Next();
                    while (dsName != null)
                    {
                        TreeNode node = CreateDbfDatasetNode(dsName);
                        parentNode.Nodes.Add(node);
                        dsName = enumDsName.Next();
                    }

                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                }
                #endregion

                #region CSVファイルとTXTファイルの存在チェック
                if (IsTextWorkspace(fullpath))
                {
                    IWorkspace ws = GetTextWorkspace(fullpath);
                    IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTTable);
                    enumDsName.Reset();
                    IDatasetName dsName = enumDsName.Next();
                    while (dsName != null)
                    {
                        TreeNode node = CreateTextDatasetNode(dsName);
                        parentNode.Nodes.Add(node);
                        dsName = enumDsName.Next();
                    }
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(dsName);
                }
                #endregion

                // ファイルのノードを追加
                SetFileNode(parentNode, dirInfo);
            }
            #endregion

            // 未接続ﾊﾟｽ
            else {
				// 接続ﾌｫﾙﾀﾞ時
				if(!parentNode.ImageIndex.Equals((int)ICON.ConnectFolder_Disconnect) &&
					parentNode.Level == 1 &&
					parentNode.FullPath.StartsWith(FOLDER_CONNECT_TOPTEXT + "\\")) {

					parentNode.Nodes.Clear();

					// 未接続を表示
					parentNode.ImageIndex = ICON.ConnectFolder_Disconnect.GetHashCode();
					parentNode.SelectedImageIndex = ICON.ConnectFolder_Disconnect.GetHashCode();
				}
				// ﾌｫﾙﾀﾞ接続 ﾄｯﾌﾟ
				else if(parentNode.Name.Equals(FOLDER_CONNECT_TOPNAME)) {
					// ｽﾙｰ
				}
				// DB接続時
				else if(parentNode.FullPath.StartsWith(DATABASE_CONNECT_TOPTEXT)) {
					// ｽﾙｰ
				}
				else if(parentNode.Nodes.Count > 0) {
					// 子ﾉｰﾄﾞ未取得の場合は、破棄
					if(parentNode.Nodes[0].Text.Equals("")) {
						// ｱﾌﾟﾘｹｰｼｮﾝ起動中に、ﾌｫﾙﾀﾞ接続が切断された場合、配下のﾉｰﾄﾞをｸﾘｱ
						parentNode.Nodes.Clear();
					}
				}
				else {
					// ｽﾙｰ : 主にﾌｨｰﾁｬｰ･ﾃﾞｰﾀｾｯﾄ向け (既にﾌｨｰﾁｬｰｸﾗｽ等のﾉｰﾄﾞがｾｯﾄ済み)
#if DEBUG
					Debug.WriteLine("●LightCatalogView : SetWorkspaceNode - FeatureDataset ?");
#endif
					//
				}
            }
        }

        // 検証用コード
        private bool ConfirmECWRasters(string FolderPath) {
			bool	blnRet = true;

			try {
#if DEBUG
				Common.Logger.Info("ラスターWS取得 : " + FolderPath);
#endif
				IWorkspaceFactory	agWSF = SingletonUtility.NewRasterWorkspaceFactory();
				IRasterWorkspace2	agRWS = agWSF.OpenFromFile(FolderPath, this.Handle.ToInt32()) as IRasterWorkspace2;
				IWorkspace			agWS = agRWS as IWorkspace;
				IEnumDatasetName	agEnumDSNames = agWS.get_DatasetNames(esriDatasetType.esriDTAny);
				IDatasetName		agDSName = agEnumDSNames.Next();

#if DEBUG
				// ﾃﾞｰﾀｾｯﾄ名の確認
				if(agDSName == null) {
					Common.Logger.Info("IDatasetNameを取得 : " + "ラスターなし(DatasetName = Null)");
				}
				int intCnt = 0;
#endif
				// ﾃﾞｰﾀｾｯﾄ名を取得
				while(agDSName != null) {
#if DEBUG
					// ﾃﾞｰﾀｾｯﾄ名を記録
					Common.Logger.Info(string.Format("IDatasetName{0} : {1}", ++intCnt, agDSName.Name));
#endif
					agDSName = agEnumDSNames.Next();
				}

				IEnumDataset		agEnumDSs = agWS.get_Datasets(esriDatasetType.esriDTAny);
				IDataset			agDS = agEnumDSs.Next();

#if DEBUG
				// ﾃﾞｰﾀｾｯﾄ名の確認
				if(agDS == null) {
					Common.Logger.Info("IDatasetを取得 : " + "ラスターなし(Dataset = Null)");
				}
				intCnt = 0;
#endif
				// ﾃﾞｰﾀｾｯﾄ名を取得
				while(agDS != null) {
#if DEBUG
					// ﾃﾞｰﾀｾｯﾄ名を記録
					Common.Logger.Info(string.Format("IDataset{0} : {1}", ++intCnt, agDS.Name));
#endif

					agDS = agEnumDSs.Next();
				}

			}
			catch(Exception ex) {
	            Common.Logger.Error("ラスターフォルダー展開時にエラー発生");
                Common.UtilityClass.DoOnError(ex);
			}

			return blnRet;
        }

        /// <summary>
        /// TreeNode下のファイルを変更する機能
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="dirInfo"></param>
        private void SetFileNode(TreeNode parentNode, DirectoryInfo dirInfo)
        {
            string fullpath = this.GetTreeNodeFullPath(parentNode);

            // PGDBを検索
            foreach(FileInfo file in dirInfo.GetFiles("*.mdb")) {
				// GDBをﾁｪｯｸ
				IsGDBReturn	ret = this.IsGdbWorkspace(file.FullName);
                if(ret == IsGDBReturn.GeoDB) {
                    TreeNode node = new TreeNode(file.Name);
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.Geodatabase;
                    List<IWorkspaceName> wsList2 = new List<IWorkspaceName>();
                    IWorkspace pgdb = GetPgdbWorkspace(file.FullName);
                    wsList2.Add(GetWorkspaceName(pgdb));
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pgdb);
                    node.Tag = wsList2;
                    node.Nodes.Add("");
                    parentNode.Nodes.Add(node);
                }
                // 破損MDB
                else if(ret == IsGDBReturn.Corrupt) {
					TreeNode node = new TreeNode(file.Name);
					node.ImageIndex = node.SelectedImageIndex = (int)ICON.Geodatabase;
					node.Nodes.Add("");
					parentNode.Nodes.Add(node);
                }
            }

            foreach (FileInfo file in dirInfo.GetFiles("*.mxd"))// マップドキュメント
            {
                TreeNode node = new TreeNode(file.Name);
                node.ImageIndex = node.SelectedImageIndex = (int)ICON.ArcMap_MXD_File;

                List<IFileName> lyrList = new List<IFileName>();
                FileNameClass fname = new FileNameClass();
                fname.Path = file.FullName;
                lyrList.Add(fname);
                node.Tag = lyrList;

                parentNode.Nodes.Add(node);
            }

            //foreach (FileInfo file in dirInfo.GetFiles("*.shp"))// シェープファイル
            //{
            //    // 一つでも存在する場合はすべてをツリーに追加
            //    IWorkspace ws = GetShapeWorkspace(fullpath);
            //    IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
            //    enumDsName.Reset();
            //    IDatasetName dsName = enumDsName.Next();
            //    while (dsName != null)
            //    {
            //        TreeNode node = CreateShapeDatasetNode(dsName);
            //        parentNode.Nodes.Add(node);
            //        dsName = enumDsName.Next();
            //    }

            //    enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTTable);
            //    enumDsName.Reset();
            //    dsName = enumDsName.Next();
            //    while (dsName != null)
            //    {
            //        TreeNode node = CreateDbfDatasetNode(dsName);
            //        parentNode.Nodes.Add(node);
            //        dsName = enumDsName.Next();
            //    }

            //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            //    // 追加は1回だけ
            //    break;
            //}

            //foreach (FileInfo file in dirInfo.GetFiles("*.csv"))
            //{
            //    IWorkspace ws = GetTextWorkspace(fullpath);
            //    IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTTable);
            //    enumDsName.Reset();
            //    IDatasetName dsName = enumDsName.Next();
            //    while (dsName != null)
            //    {
            //        TreeNode node = CreateTextDatasetNode(dsName);
            //        parentNode.Nodes.Add(node);
            //        dsName = enumDsName.Next();
            //    }
            //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(dsName);
            //    break;
            //}

            //foreach (FileInfo file in dirInfo.GetFiles("*.dbf"))
            //{
            //    // シェープファイルが存在する場合はdbfは追加しない
            //    string filepath = file.FullName;
            //    string shpfile = System.IO.Path.GetDirectoryName(filepath) + "\\"
            //        + System.IO.Path.GetFileNameWithoutExtension(filepath) + ".shp";
            //    if (!System.IO.File.Exists(shpfile))
            //    {
            //        TreeNode node = new TreeNode(file.Name);
            //        node.ImageIndex = node.SelectedImageIndex = (int)ICON.TableDBase;
            //        parentNode.Nodes.Add(node);
            //    }
            //}

            foreach (FileInfo file in dirInfo.GetFiles("*.lyr")) // Layerファイル
            {
                TreeNode node = new TreeNode(file.Name);
                node.ImageIndex = node.SelectedImageIndex = (int)ICON.Layer_LYR_File;
                List<IFileName> lyrList = new List<IFileName>();
                FileNameClass fname = new FileNameClass();
                fname.Path = file.FullName;
                lyrList.Add(fname);
                node.Tag = lyrList;
                parentNode.Nodes.Add(node);
            }

            foreach (FileInfo file in dirInfo.GetFiles("*.prj"))
            {
                // シェープファイルが存在する場合はprjは追加しない
                string filepath = file.FullName;
                string shpfile = System.IO.Path.GetDirectoryName(filepath) + "\\"
                    + System.IO.Path.GetFileNameWithoutExtension(filepath) + ".shp";
                if (!System.IO.File.Exists(shpfile))
                {
                    TreeNode node = new TreeNode(file.Name);
                    node.ImageIndex = node.SelectedImageIndex = (int)ICON.CoordinateSystem;
                    parentNode.Nodes.Add(node);
                }
            }
        }

        private IWorkspaceName GetWorkspaceName(IWorkspace ws)
        {
            //IWorkspaceName wsName = new WorkspaceNameClass();
            //wsName.PathName = ws.PathName;
            //wsName.WorkspaceFactoryProgID = GetWorkspaceFactProgID(wsName.WorkspaceFactory);
            IDataset dataset = (IDataset)ws;
            return (IWorkspaceName)dataset.FullName;
        }

        private IDatasetName OpenCadDatasetName(string filepath)
        {
            string file = System.IO.Path.GetFileName(filepath);
            string folder = System.IO.Path.GetDirectoryName(filepath);
            IWorkspace ws = GetCadWorkspace(folder);
            IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTCadDrawing);
            enumDsName.Reset();
            IDatasetName dsName = enumDsName.Next();
            while (dsName != null)
            {
                if (dsName.Name.Equals(file))
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                    return dsName;
                }
                dsName = enumDsName.Next();
            }
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            return null;
        }

        private IDatasetName OpenRasterDatasetName(string filepath)
        {
            string file = System.IO.Path.GetFileName(filepath);
            string folder = System.IO.Path.GetDirectoryName(filepath);
            IWorkspace ws = GetRasterWorkspace(folder);
            IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTRasterDataset);
            enumDsName.Reset();
            IDatasetName dsName = enumDsName.Next();
            while (dsName != null)
            {
                if (dsName.Name.Equals(file))
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                    return dsName;
                }

                dsName = enumDsName.Next();
            }
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            return null;
        }

        private IDatasetName OpenGdbDatasetName2(string filepath)
        {
            string fcname = System.IO.Path.GetFileName(filepath);
            string fcsetname = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(filepath));
            string gdbpath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(filepath));
            IWorkspace ws = GetFgdbWorkspace(gdbpath);//GetGdbWorkspace(gdbpath);
            IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            enumDsName.Reset();
            IDatasetName dsName = enumDsName.Next();
            while (dsName != null)
            {
                if (dsName.Name.Equals(fcsetname))
                {
                    enumDsName = dsName.SubsetNames;
                    enumDsName.Reset();
                    IDatasetName subDsName = enumDsName.Next();
                    while (subDsName != null)
                    {
                        if (subDsName.Name.Equals(fcname))
                        {
                            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                            return subDsName;
                        }
                        subDsName = enumDsName.Next();
                    }
                }
                dsName = enumDsName.Next();
            }
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            return null;
        }

        private IDatasetName OpenGdbDatasetName(string filepath)
        {
            string fcname = System.IO.Path.GetFileName(filepath);
            string gdbpath = System.IO.Path.GetDirectoryName(filepath);
            IWorkspace ws = GetFgdbWorkspace(gdbpath);//GetGdbWorkspace(gdbpath);
            IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTAny);
            enumDsName.Reset();
            IDatasetName dsName = enumDsName.Next();
            while (dsName != null)
            {
                if (dsName.Name.Equals(fcname))
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                    return dsName;
                }
                dsName = enumDsName.Next();
            }
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            return null;
        }


        private IDatasetName OpenTextDatasetName(string filepath)
        {
            string file = System.IO.Path.GetFileName(filepath);
            string folder = System.IO.Path.GetDirectoryName(filepath);

            IWorkspace ws = GetTextWorkspace(folder);
            IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTText);
            enumDsName.Reset();
            IDatasetName dsName = enumDsName.Next();
            while (dsName != null)
            {
                if (dsName.Name.Equals(file))
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                    return dsName;
                }
            }
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            return null;
        }

        private IDatasetName OpenDbfDatasetName(string filepath)
        {
            string file = System.IO.Path.GetFileName(filepath);
            string folder = System.IO.Path.GetDirectoryName(filepath);
            IWorkspace ws = GetShapeWorkspace(folder);
            IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTTable);
            enumDsName.Reset();
            IDatasetName dsName = enumDsName.Next();
            while (dsName != null)
            {
                if (dsName.Name.Equals(file))
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                    return dsName;
                }
                dsName = enumDsName.Next();
            }
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            return null;
        }

        private IDatasetName OpenShapeDatasetName(string filepath)
        {
            string file = System.IO.Path.GetFileNameWithoutExtension(filepath);
            string folder = System.IO.Path.GetDirectoryName(filepath);
            IWorkspace ws = GetShapeWorkspace(folder);
            IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTAny);
            enumDsName.Reset();
            IDatasetName dsName = enumDsName.Next();
            while (dsName != null)
            {
                if (dsName.Name.Equals(file))
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
                    return dsName;
                }
                dsName = enumDsName.Next();
            }
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            return null;
        }

        ///// <summary>
        ///// RCWのエラーになるためSingletonのオブジェクトをNewでなく作成
        /////  Interacting with singleton objects を参照
        /////  ms-help://ESRI.EDNv10.0/ArcObjects_NET/d91e445e-47c5-41ea-94ca-45f945b73c0f.htm
        ///// </summary>
        ///// <returns></returns>
        //private IWorkspaceFactory OpenSingletonWsf(string progID)
        //{
        //    Type t = Type.GetTypeFromProgID(progID);
        //    System.Object o = Activator.CreateInstance(t);
        //    return o as IWorkspaceFactory;
        //}

        private IWorkspace GetTextWorkspace(string path)
        {
            if (textWsf == null)
            {
                textWsf = SingletonUtility.NewTextFileWorkspaceFactory();
            }
            return textWsf.OpenFromFile(path, this.Handle.ToInt32());
        }

        /// <summary>
        /// MDBのワークスペースを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IWorkspace GetPgdbWorkspace(string path)
        {
            if (accessWsf == null)
            {
                accessWsf = SingletonUtility.NewAccessWorkspaceFactory();
            }
            return accessWsf.OpenFromFile(path, this.Handle.ToInt32());
        }

        /// <summary>
        /// FGDBのワークスペースを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IWorkspace GetFgdbWorkspace(string path)
        {
            if (fgdbWsf == null)
            {
                fgdbWsf = SingletonUtility.NewFileGDBWorkspaceFactory();
            }
            return fgdbWsf.OpenFromFile(path, this.Handle.ToInt32());
        }

        /// <summary>
        /// ShapeFileのワークスペースを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IWorkspace GetShapeWorkspace(string path)
        {
            if (shapeWsf == null)
            {
                shapeWsf = SingletonUtility.NewShapeFileWorkspaceFactory();
            }
            return shapeWsf.OpenFromFile(path, this.Handle.ToInt32());
        }

        /// <summary>
        /// Rasterのワークスペースを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IWorkspace GetRasterWorkspace(string path)
        {
            if (rasterWsf == null)
            {
                rasterWsf = SingletonUtility.NewRasterWorkspaceFactory();
            }
            return rasterWsf.OpenFromFile(path, this.Handle.ToInt32());
        }

        /// <summary>
        /// CADのワークスペースを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IWorkspace GetCadWorkspace(string path)
        {
            if (cadWsf == null)
            {
                cadWsf = SingletonUtility.NewCadWorkspaceFactory();
            }
            return cadWsf.OpenFromFile(path, this.Handle.ToInt32());
        }

        private enum IsGDBReturn {
			GeoDB,
			Other,
			Corrupt		// 破損
        }

        /// <summary>
        /// GDBワークスペースか
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IsGDBReturn IsGdbWorkspace(string path) {
			IsGDBReturn	ret = IsGDBReturn.Other;

            string ex = System.IO.Path.GetExtension(path);
            if(!string.IsNullOrEmpty(ex)) {
				// ﾌｧｲﾙ･ｼﾞｵ･ﾃﾞｰﾀﾍﾞｰｽ
				if(ex.ToLower().Equals(".gdb")) {
					// GDBが破損していないかﾁｪｯｸ
					try {
						IWorkspace	agWS =  this.GetFgdbWorkspace(path);
						if(agWS.Type == esriWorkspaceType.esriLocalDatabaseWorkspace) {
							ret = IsGDBReturn.GeoDB;
						}
					}
					catch(Exception err) {
						// ﾛｸﾞ出力
						Common.Logger.Error(string.Format("GDB OPEN ERROR : {0}.{1} [{2}]", "LightCatalogView", "IsGdbWorkspace", path));
						Common.UtilityClass.DoOnError(err);
						ret = IsGDBReturn.Corrupt;
					}
				}
				// ﾊﾟｰｿﾅﾙ･ｼﾞｵ･ﾃﾞｰﾀﾍﾞｰｽ
				else if(ex.ToLower().Equals(".mdb")) {
					// GDBが破損していないかﾁｪｯｸ
					try {
					    IWorkspace	agWS =  this.GetPgdbWorkspace(path);
					    if(agWS.Type == esriWorkspaceType.esriLocalDatabaseWorkspace) {
							ret = IsGDBReturn.GeoDB;
					    }
					}
					catch(Exception err) {
					    // ﾛｸﾞ出力
					    Common.Logger.Error(string.Format("GDB OPEN ERROR : {0}.{1} [{2}]", "LightCatalogView", "IsGdbWorkspace", path));
					    Common.UtilityClass.DoOnError(err);
						ret = IsGDBReturn.Corrupt;
					}
				}
			}

            return ret;
        }

        /// <summary>
        /// フォルダ内にCADデータの存在チェック
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsCadWorkspace(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.dxf").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.dwg").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.dgn").Length > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// フォルダ内にラスタデータの存在チェック
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsRasterWorkspace(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.tif").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.img").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.bmp").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.png").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.jpg").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.flt").Length > 0)
            {
                return true;
            }

            if (dirInfo.GetFiles("*.ecw").Length > 0)
            {
#if DEBUG
	            Common.Logger.Info("カタログビュー : .NETではECWを認識しています。");
#endif
                return true;
            }

            if (dirInfo.GetDirectories("info").Length > 0)
            {
                // ArcINFOグリッドフォーマット
                // http://support.esri.com/en/knowledgebase/techarticles/detail/30616
                // infoディレクトリ下に*.dat, *.nitファイルがあったらESRI GRIDとする
                DirectoryInfo subdirInfo = dirInfo.GetDirectories("info")[0];
                if ((subdirInfo.GetFiles("*.dat").Length > 0) && (subdirInfo.GetFiles("*.nit").Length > 0))
                {
                    return true;
                }
                return false;
            }

            return false;
        }


        /// <summary>
        /// フォルダ内にCSVファイルの存在チェック
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsTextWorkspace(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.csv").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.CSV").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.txt").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.TXT").Length > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// フォルダ内にDBFファイルの存在チェック
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsDbfWorkspace(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.dbf").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.DBF").Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// フォルダ内にマップドキュメントの存在チェック
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool ExistMapDocument(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.mxd").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.MXD").Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// フォルダ内にPRJファイルの存在チェック
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsPrjWorkspace(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.prj").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.PRJ").Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// フォルダ内にレイヤファイルの存在チェック
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsLayerWorkspace(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.lyr").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.LYR").Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// フォルダ内にシェープファイルの存在チェック
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsShapeWorkspace(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles("*.shp").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.SHP").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.dbf").Length > 0)
            {
                return true;
            }
            if (dirInfo.GetFiles("*.DBF").Length > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 選択イベント時
        /// 選択オブジェクト(IWorkspaceName, IDatasetName, それ以外)を設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvWs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            System.Windows.Forms.Cursor preCursor = System.Windows.Forms.Cursor.Current;
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            txtSelectPath.Text = "";
            txtSelType.Text = "";

            try {
                TreeNode currentNode = e.Node;

                selectPath = this.GetTreeNodeFullPath(currentNode);
                txtSelectPath.Text = selectPath;

                // ﾂｰﾙﾁｯﾌﾟを設定
                this.toolTip1.SetToolTip(this.txtSelectPath, selectPath);

                if (currentNode.Tag is System.Collections.Generic.List<IWorkspaceName>)
                {
                    returnObject = true;
                    string txt = "";
                    List<IWorkspaceName> wsName = (List<IWorkspaceName>)currentNode.Tag;
                    foreach (var ws in wsName)
                    {
                        txt += ws.Category + " ,";
                        //txt += ws.WorkspaceFactory.GetType().ToString() + " ,";
                    }
                    txtSelType.Text = txt.Remove(txt.Length - 1);
                    selectObject = currentNode.Tag;
                }
                else if (currentNode.Tag is System.Collections.Generic.List<IDatasetName>)
                {
                    returnObject = true;
                    string txt = "";
                    List<IDatasetName> dsName = (List<IDatasetName>)currentNode.Tag;
                    foreach (var ds in dsName)
                    {
                        if (ds.WorkspaceName.WorkspaceFactoryProgID == "esriDataSourcesFile.CadWorkspaceFactory.1")
                        {
                            txt += "CAD ドローイング ,";
                        }
                        else
                        {
                            txt += ds.Category + " ,";
                        }
                    }
                    txtSelType.Text = txt.Remove(txt.Length - 1);
                    selectObject = currentNode.Tag;
                }
                else if(currentNode.Tag is System.Collections.Generic.List<IFileName>) {
                    returnObject = true;
                    string	txt = "";
                    string	strExt;
                    //List<IFileName> fileName = (List<IFileName>)currentNode.Tag;
                    foreach(IFileName fname in (List<IFileName>)currentNode.Tag) {
						strExt = Path.GetExtension(fname.Path).ToLower();
                        if(strExt.Equals(".lyr")) {
							txt += "レイヤ 定義ファイル ,";
                        }
                        else if(strExt.Equals(".mxd")) {
							txt += "マップ ドキュメント ,";
                        }
                    }
                    txtSelType.Text = txt.Remove(txt.Length - 1);
                    selectObject = currentNode.Tag;
                }
                // DB接続ﾃｰﾌﾞﾙ (非ｼﾞｵﾃﾞｰﾀﾍﾞｰｽ)
                else if(currentNode.Tag is System.Collections.Generic.List<UserSelectQueryTableSet>) {
					returnObject = true;
					string txt = "";
                    List<UserSelectQueryTableSet> fileName = (List<UserSelectQueryTableSet>)currentNode.Tag;
                    foreach(var fname in fileName) {
                        txt += "クエリ テーブル,";
                    }
                    txtSelType.Text = txt.Remove(txt.Length - 1);
                    selectObject = currentNode.Tag;
                }
                else {
                    returnObject = false;
                    if(System.IO.Directory.Exists(selectPath)) {
                        txtSelType.Text = "フォルダ";
                    }
                    else if(System.IO.File.Exists(selectPath)) {
                        string ext = System.IO.Path.GetExtension(selectPath);
                        if (ext.ToUpper().Equals(".MXD"))
                        {
                            txtSelType.Text = "マップ ドキュメント";
                        }
                        else if (ext.ToUpper().Equals(".PRJ"))
                        {
                            txtSelType.Text = "プロジェクト ファイル";
                        }
                        //else if (ext.ToUpper().Equals(".DBF"))
                        //{
                        //    txtSelType.Text = "dBASE テーブル";
                        //}
                        //else if (ext.ToUpper().Equals(".CSV"))
                        //{
                        //    txtSelType.Text = "テキスト ファイル";
                        //}
                    }
                    selectObject = null;
                }

                // ﾂｰﾙﾁｯﾌﾟを設定
                this.toolTip1.SetToolTip(this.txtSelType, this.txtSelType.Text);
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message + ex.StackTrace, "エラー!! AfterSelect");
            }
            finally {
                System.Windows.Forms.Cursor.Current = preCursor;
            }

			// ﾌｫﾙﾀﾞ切断機能の制御
			this.toolStripButton_FD.Enabled = (e.Node.Level > 0 && e.Node.Parent.Name.Equals(FOLDER_CONNECT_TOPNAME));
			// ﾘﾌﾚｯｼｭ機能の制御
			this.toolStripButton_FR.Enabled = this.toolStripButton_FD.Enabled;

			// DB接続削除機能の制御
			this.toolStripButton_DD.Enabled = (e.Node.Level > 0 && e.Node.Parent.Name.Equals(DATABASE_CONNECT_TOPNAME));

			// 名前の変更機能の制御
			this.toolStripButton_RN.Enabled = this.toolStripButton_FD.Enabled || this.toolStripButton_DD.Enabled;
        }

        private void tvWs_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if(selectObject != null) {
				IDataObject dataObject;
				IObjectCopy objCopy;
				object		obj;

                // ドラッグ&ドロップ可能なものをIDatasetNameとIFileNameとする
                // DatasetNameでも限定
                if(selectObject is List<IDatasetName>) {
                    IDatasetName dsName = ((List<IDatasetName>)selectObject)[0];

                    switch (dsName.Type) {
                    case esriDatasetType.esriDTCadDrawing:
                    case esriDatasetType.esriDTFeatureClass:
                    case esriDatasetType.esriDTFeatureDataset:
                    case esriDatasetType.esriDTRasterCatalog:
                    case esriDatasetType.esriDTRasterDataset:
                        dataObject = new DataObject();
                        //dataObject.SetData("ESRI.ArcGIS.Geodatabase.IDatasetName", dsName);
                        objCopy = new ObjectCopyClass();
                        obj = objCopy.Copy((object)dsName); // DeepCopy
                        dataObject.SetData("ESRI.ArcGIS.Geodatabase.IDatasetName", (IDatasetName)obj);
                        tvWs.DoDragDrop(dataObject, DragDropEffects.Copy);
                        break;
                    // ﾃｰﾌﾞﾙに対応 
                    /*case esriDatasetType.esriDTTable:
                        dataObject = new DataObject();
                        objCopy = new ObjectCopyClass();
                        obj = objCopy.Copy((object)dsName); // DeepCopy
                        dataObject.SetData("ESRI.ArcGIS.Geodatabase.IDatasetName", (IDatasetName)obj);
                        tvWs.DoDragDrop(dataObject, DragDropEffects.Copy);
                        break;*/
                    }
                }
                else if(selectObject is List<IFileName>) {
                    IFileName fileName = ((List<IFileName>)selectObject)[0];
                    dataObject = new DataObject();
                    objCopy = new ObjectCopyClass();
                    obj = objCopy.Copy((object)fileName); // DeepCopy
                    dataObject.SetData("ESRI.ArcGIS.esriSystem.IFileName", (IFileName)obj);
                    tvWs.DoDragDrop(dataObject, DragDropEffects.Copy);
                }
                // DBﾃｰﾌﾞﾙに対応 
                /*else if(selectObject is List<UserSelectQueryTableSet>) {
					UserSelectQueryTableSet	objOrgDB = ((List<UserSelectQueryTableSet>)selectObject)[0];

					// ｵﾌﾞｼﾞｪｸﾄをそのまま引き渡す(中身は文字列情報の為)
					dataObject = new DataObject();
					//objCopy = new ObjectCopyClass();
					//UserSelectQueryTableSet	objDB = new UserSelectQueryTableSet() {
					//    TableName = objOrgDB.TableName,
					//    SqlWorkspace = (ISqlWorkspace)objCopy.Copy(objOrgDB.SqlWorkspace),
					//};

                    dataObject.SetData("ESRIJapan.GISLight10.Common.UserSelectQueryTableSet", objOrgDB);
                    tvWs.DoDragDrop(dataObject, DragDropEffects.Copy);
                }*/
            }
        }

		/// <summary>
		/// ツリーノードのラベル名変更完了 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TreeNode_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
			this.tvWs.LabelEdit = false;
			if(string.IsNullOrEmpty(e.Label) || e.Label.Trim().Equals("")) {
				e.CancelEdit = true;
			}
			else {
				// 新しい名前を保存
				if(e.Node.Parent.Name.Equals(FOLDER_CONNECT_TOPNAME)) {
					// ﾌｫﾙﾀﾞ接続
					ConnectFoldersManager.ReNameConnectFolder(e.Node.Name, e.Label);
				}
				else if(e.Node.Parent.Name.Equals(DATABASE_CONNECT_TOPNAME)) {
					// DB接続
					if(!ConnectionFilesManager.RenameDBConnectFile(e.Node.Name, e.Label)) {
						Common.MessageBoxManager.ShowMessageBoxWarining("データベース接続の名前を変更できませんでした。");
					}
				}
			}
		}

		/// <summary>
		/// ツリービュー マウスダウン イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TreeNode_MouseDown(object sender, MouseEventArgs e) {
			#if DEBUG
			Debug.WriteLine("[LightCatalogView] TreeNode_MouseDown EVENT.");
			#endif

			// ﾏｳｽ･ﾀﾞｳﾝでﾉｰﾄﾞを選択状態にする
			var		ctlTV = sender as TreeView;
			var		tvHitInfo = ctlTV.HitTest(e.X, e.Y);
			if(!(tvHitInfo.Node == null || tvHitInfo.Node.IsSelected)) {
				ctlTV.SelectedNode = tvHitInfo.Node;
			}
		}

		private void TreeNode_MouseDoubleClick(object sender, MouseEventArgs e) {
			TreeView	ctlTV = sender as TreeView;

			// 対象ﾉｰﾄﾞを取得
			TreeViewHitTestInfo ctlHit = ctlTV.HitTest(e.X, e.Y);
			if(ctlHit.Node != null) {
				// DB接続要素で未接続状態の場合
				TreeNode	nodeHit = ctlHit.Node;
				if(nodeHit.Level == 1 && nodeHit.Parent.Name == DATABASE_CONNECT_TOPNAME
					&& nodeHit.ImageIndex == ICON.DatabaseConnect_Disconnect.GetHashCode()) {
					// 待ち表示
					this.Cursor = Cursors.WaitCursor;

					// DB接続
					IWorkspace		agWS = ConnectionFilesManager.LoadWorkspace(nodeHit.Name);
					if(agWS != null) {
						// ｸｴﾘ ﾃｰﾌﾞﾙ
						if(agWS is ISqlWorkspace) {
							ISqlWorkspace	agSQLWS = agWS as ISqlWorkspace;

							// ﾃｰﾌﾞﾙ･ﾘｽﾄを作成
							IStringArray	agStrArr = agSQLWS.GetTables();
							string			strTblName;
							object			objTag = null;

							for(int intCnt=0; intCnt < agStrArr.Count; intCnt++) {
								// ﾃｰﾌﾞﾙ名を取得
								strTblName = agStrArr.get_Element(intCnt);

								// ｸｴﾘ･ﾃｰﾌﾞﾙ専用引き渡しｵﾌﾞｼﾞｪｸﾄを埋め込む
								List<UserSelectQueryTableSet>	lstQD = new List<UserSelectQueryTableSet>();
								lstQD.Add(new UserSelectQueryTableSet() {
									TableName = strTblName,
									ConnectProperty = agWS.ConnectionProperties,
									ConnectionFile = nodeHit.Name,
								});
								objTag = lstQD;

								// ﾂﾘｰ･ﾉｰﾄﾞを生成
								TreeNode nodeTbl = new TreeNode(strTblName, ICON.GeodatabaseTable.GetHashCode(), ICON.GeodatabaseTable.GetHashCode()) {
									Tag = objTag,
								};

								// ﾉｰﾄﾞを追加
								nodeHit.Nodes.Add(nodeTbl);
							}
							#if DEBUG
							// 接続ﾌﾟﾛﾊﾟﾃｨを確認
							object		objNames, objVals;
							agWS.ConnectionProperties.GetAllProperties(out objNames, out objVals);
							string[]	strNames = (string[])objNames;
							for(int intCnt=0; intCnt < strNames.Length; intCnt++) {
								if(!strNames[intCnt].ToLower().Equals("password")) {

								}
							}
							#endif
						}
						// SDE
						else {
							IFeatureWorkspace	agFWS = agWS as IFeatureWorkspace;
							IEnumDatasetName	agDSNames = agWS.get_DatasetNames(esriDatasetType.esriDTAny);
							IDatasetName		agDSName = null;

							while((agDSName = agDSNames.Next()) != null) {
								TreeNode node = CreateGdbDatasetNode(agDSName);
								nodeHit.Nodes.Add(node);
							}
							ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agDSNames);
						}

						// ﾘﾘｰｽしてしまうと、ﾄﾞﾗｯｸﾞ時に再度ﾊﾟｽﾜｰﾄﾞ入力が必要になってしまう
						//ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agWS);

						// 接続済みｱｲｺﾝに更新
						nodeHit.ImageIndex = ICON.DatabaseConnect_DB.GetHashCode();
						nodeHit.SelectedImageIndex = ICON.DatabaseConnect_DB.GetHashCode();

						// ﾉｰﾄﾞを展開
						if(nodeHit.Nodes.Count > 0) {
							nodeHit.Expand();
						}
					}
					else {
						// ｴﾗｰ表示
						Common.MessageBoxManager.ShowMessageBoxError(nodeHit.Text + "に接続できませんでした。");
					}

					// 待ち解除
					this.Cursor = Cursors.Default;
				}
			}
		}

		/// <summary>
		/// ツールボタン クリック イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tool_Click(object sender, EventArgs e) {
			var	ctlTSB = sender as ToolStripButton;

			string	strTag = ctlTSB.Tag.ToString();
			switch(strTag) {
			case "FC":		// ﾌｫﾙﾀﾞに接続
				this.ConnectFolder();
				break;
			case "FD":		// ﾌｫﾙﾀﾞの切断
				this.DisconnectFolder();
				break;
			case "RN":		// 名前の変更 (接続ﾌｫﾙﾀﾞ / DB接続)
				this.RenameFolder();
				break;
			case "FR":		// 最新の情報に更新
				this.RefreshFolders();
				break;
			case "DC":		// DB接続追加
				this.AddDBConnect();
				break;
			case "DD":		// DB接続削除
				this.DeleteDBConnect();
				break;
			case "SC":		// ArcGISｻｰﾊﾞ接続追加
				this.AddArcGISServerConnect();
				break;
			case "SD":		// ArcGISｻｰﾊﾞ接続削除
				this.DeleteArcGISServerConnect();
				break;
			}
		}

		/// <summary>
		/// フォルダに接続アクションを実行します
		/// </summary>
		private void ConnectFolder() {
			// ﾌｫﾙﾀﾞｰ･ﾌﾞﾗｳｻﾞを準備
			FolderBrowserDialog	dlgFolder = new FolderBrowserDialog() {
				RootFolder = Environment.SpecialFolder.Desktop,
				ShowNewFolderButton = true,
				Description = "接続するフォルダをご指定下さい。",
			};

			// ﾕｰｻﾞｰ指定
			if(dlgFolder.ShowDialog(this.ParentForm) == DialogResult.OK) {
				// ﾉｰﾄﾞに追加
				this.AddConnectFolder(dlgFolder.SelectedPath);
			}
		}

		/// <summary>
		/// フォルダの切断アクションを実行します
		/// </summary>
		private void DisconnectFolder() {
			// ﾕｰｻﾞｰ確認ﾒｯｾｰｼﾞを表示
			string	strQMsg = this.tvWs.SelectedNode.Text + " を切断しますか？";
			if(MessageBoxManager.ShowMessageBoxQuestion2(this.ParentForm, strQMsg) == DialogResult.OK) {
				// 設定ﾌｧｲﾙから削除
				if(ConnectFoldersManager.DeleteConnectFolder(this.tvWs.SelectedNode.Name)) {
					// ﾂﾘｰから削除
					this.tvWs.Nodes.Remove(this.tvWs.SelectedNode);
				}
				else {
					MessageBoxManager.ShowMessageBoxError(this.tvWs.SelectedNode.Text + " フォルダを切断できませんでした。");
				}
			}
		}

		/// <summary>
		/// 接続フォルダ名変更のアクションを実行します
		/// </summary>
		private void RenameFolder() {
			this.tvWs.LabelEdit = true;
			this.tvWs.SelectedNode.BeginEdit();
		}

		/// <summary>
		/// 接続フォルダのリフレッシュを実行します
		/// </summary>
		private void RefreshFolders() {
			this.tvWs.SuspendLayout();

			// 展開ｲﾍﾞﾝﾄを強制的に発生させる
			if(this.tvWs.SelectedNode.Nodes.Count > 0) {
				if(this.tvWs.SelectedNode.IsExpanded) {
					// 一旦閉じる
					this.tvWs.SelectedNode.Collapse();
				}
				this.tvWs.SelectedNode.Expand();
			}
			else {
				// 強制ｲﾍﾞﾝﾄ発生
				this.tvWs_BeforeExpand(this.tvWs, new TreeViewCancelEventArgs(this.tvWs.SelectedNode, false, TreeViewAction.Expand));
				if(this.tvWs.SelectedNode.Nodes.Count > 0) {
					this.tvWs.SelectedNode.Expand();
				}
			}

			this.tvWs.ResumeLayout();
		}

		/// <summary>
		/// データベース接続を追加します
		/// </summary>
		private void AddDBConnect() {
			// DB接続設定ﾌｫｰﾑを表示
			Ui.FormOLEDBConnect frm = new Ui.FormOLEDBConnect();
            if(frm.ShowDialog(this.ParentForm) == DialogResult.OK) {
				// 入力されたﾜｰｸｽﾍﾟｰｽを取得
				IWorkspace	agWS = frm.DBWorkspace as IWorkspace;

				// DB接続を登録してﾉｰﾄﾞを作成
				TreeNode	nodeNew = ConnectionFilesManager.AddDBConnectFile(agWS, frm.IsSavePassword);

				// ﾉｰﾄﾞを追加
				if(nodeNew != null) {
					// 接続Topﾌｫﾙﾀﾞｰを取得
					TreeNode	nodeFC = this.tvWs.Nodes.Find(DATABASE_CONNECT_TOPNAME, false)[0];
					nodeFC.Nodes.Add(nodeNew);
					if(!nodeFC.IsExpanded) {
						nodeFC.Expand();
					}

					// ﾃｰﾌﾞﾙを展開

				}
            }
		}

		/// <summary>
		/// データベース接続を削除します
		/// </summary>
		private void DeleteDBConnect() {
			// ﾕｰｻﾞｰ確認ﾒｯｾｰｼﾞを表示
			string	strQMsg = this.tvWs.SelectedNode.Text + " を削除してもよろしいですか？";
			if(MessageBoxManager.ShowMessageBoxQuestion2(this.ParentForm, strQMsg) == DialogResult.OK) {
				// 設定ﾌｧｲﾙから削除
				if(ConnectionFilesManager.DeleteDBConnectFile(this.tvWs.SelectedNode.Name)) {
					// ﾂﾘｰから削除
					this.tvWs.Nodes.Remove(this.tvWs.SelectedNode);
				}
				else {
					MessageBoxManager.ShowMessageBoxError(this.tvWs.SelectedNode.Text + " フォルダを切断できませんでした。");
				}
			}
		}

		/// <summary>
		/// ArcGISサーバー接続を追加します
		/// </summary>
		private void AddArcGISServerConnect() {
			// DB接続設定ﾌｫｰﾑを表示
			Ui.FormArcGISServerConnect frm = new Ui.FormArcGISServerConnect();
            if(frm.ShowDialog(this.ParentForm) == DialogResult.OK) {
				// 入力値を取得
				//IWorkspace	agWS = frm.DBWorkspace as IWorkspace;

				//// DB接続を登録してﾉｰﾄﾞを作成
				//TreeNode	nodeNew = ConnectionFilesManager.AddDBConnectFile(agWS, frm.IsSavePassword);

				//// ﾉｰﾄﾞを追加
				//if(nodeNew != null) {
				//    // 接続Topﾌｫﾙﾀﾞｰを取得
				//    TreeNode	nodeFC = this.tvWs.Nodes.Find(DATABASE_CONNECT_TOPNAME, false)[0];
				//    nodeFC.Nodes.Add(nodeNew);
				//    if(!nodeFC.IsExpanded) {
				//        nodeFC.Expand();
				//    }

				//    // ﾃｰﾌﾞﾙを展開

				//}
            }
		}

		/// <summary>
		/// ArcGISサーバー接続を削除します
		/// </summary>
		private void DeleteArcGISServerConnect() {
			// ﾕｰｻﾞｰ確認ﾒｯｾｰｼﾞを表示
			string	strQMsg = this.tvWs.SelectedNode.Text + " を削除してもよろしいですか？";
			if(MessageBoxManager.ShowMessageBoxQuestion2(this.ParentForm, strQMsg) == DialogResult.OK) {
				// 設定ﾌｧｲﾙから削除
				if(ConnectionFilesManager.DeleteDBConnectFile(this.tvWs.SelectedNode.Name)) {
					// ﾂﾘｰから削除
					this.tvWs.Nodes.Remove(this.tvWs.SelectedNode);
				}
				else {
					MessageBoxManager.ShowMessageBoxError(this.tvWs.SelectedNode.Text + " フォルダを切断できませんでした。");
				}
			}
		}

		/// <summary>
		/// 接続フォルダをツリーに登録します
		/// </summary>
		/// <param name="FolderPath"></param>
		/// <returns></returns>
		private void AddConnectFolder(string FolderPath) {
			// ﾊﾟｽを確認
			if(Directory.Exists(FolderPath)) {
				// FGDBはNG
				if(this.IsGdbWorkspace(FolderPath) != IsGDBReturn.GeoDB) {
					// 既存ﾁｪｯｸ
					if(!this.IsConnectedFolder(FolderPath)) {
						// 登録追加
						TreeNode	nodeNew = ConnectFoldersManager.AddConnectFolder(FolderPath);

						// ﾉｰﾄﾞを追加
						if(nodeNew != null) {
							// 接続Topﾌｫﾙﾀﾞｰを取得
							TreeNode	nodeFC = this.tvWs.Nodes.Find(FOLDER_CONNECT_TOPNAME, false)[0];
							nodeFC.Nodes.Add(nodeNew);
							if(!nodeFC.IsExpanded) {
								nodeFC.Expand();
							}
							//this.tvWs.Sort();
						}
					}
					else {
						MessageBoxManager.ShowMessageBoxError("ご指定のフォルダはすでに登録されています。");
					}
				}
				else {
					MessageBoxManager.ShowMessageBoxError("ファイルジオデータベースを直接接続することはできません。");
				}
			}
			else {
				MessageBoxManager.ShowMessageBoxError("フォルダへの接続に失敗しました。");
			}
		}

		/// <summary>
		/// 指定のフォルダ・パスが登録済みかどうかを確認します
		/// </summary>
		/// <param name="FolderPath">フォルダ・パス</param>
		/// <returns>済 / 未</returns>
		private bool IsConnectedFolder(string FolderPath) {
			bool	blnRet = false;

			// 実際のﾊﾟｽは、Nameに設定されている
			TreeNode	nodeFC = this.tvWs.Nodes.Find(FOLDER_CONNECT_TOPNAME, false)[0];
			foreach(TreeNode nodePath in nodeFC.Nodes) {
				if(nodePath.Name.ToLower().Equals(FolderPath.ToLower())) {
					blnRet = true;
					break;
				}
			}

			return blnRet;
		}

    }

    /// <summary>
    /// フォルダ接続 管理用クラス
    /// </summary>
    static public class ConnectFoldersManager {
		private const string	FIELD_NAME_CONNECT_FOLDER = "フォルダ接続";

		/// <summary>
		/// フォルダ接続ノードを取得します
		/// </summary>
		/// <returns></returns>
		static public TreeNode[] GetConnectFolders() {
			List<TreeNode>	nodeFolders = new List<TreeNode>();

			// 設定の読み込み
			List<ConnectFolder>	folders = ReadFolderSettings();

			// ﾂﾘｰ･ﾉｰﾄﾞを量産
			TreeNode	nodeChild;
			foreach(ConnectFolder clsFolder in folders) {
				nodeChild = new TreeNode(clsFolder.Name,
					(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.ConnectFolder_UserFolder,
					(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.ConnectFolder_UserFolder) { Name = clsFolder.Path };
				nodeChild.Nodes.Add("");
				nodeFolders.Add(nodeChild);
			}

			return nodeFolders.ToArray();
		}

		/// <summary>
		/// 新しいフォルダ接続を登録します
		/// </summary>
		/// <param name="FolderPath">フォルダ・パス</param>
		/// <returns>TreeNode / Null</returns>
		static public TreeNode AddConnectFolder(string FolderPath) {
			TreeNode	nodeNew = null;

			// 設定の読み込み・追加
			List<ConnectFolder>	regFolders = ReadFolderSettings();
			regFolders.Add(new ConnectFolder(FolderPath));

			// 設定ﾌｧｲﾙに保存
			if(SaveFolderSettings(regFolders)) {
				// ﾂﾘｰ･ﾉｰﾄﾞを生成
				nodeNew = new TreeNode(FolderPath,
					 (int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.ConnectFolder_UserFolder,
					 (int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.ConnectFolder_UserFolder) { Name = FolderPath };
				nodeNew.Nodes.Add("");
			}

			return nodeNew;
		}

		/// <summary>
		/// 指定のフォルダが登録済みかどうか判定します
		/// </summary>
		/// <param name="FolderPath">フォルダ・パス</param>
		/// <returns>済 / 未</returns>
		static public bool IsRegisterdFolder(string FolderPath) {
			bool	blnRet = false;

			// 設定の読み込み
			List<ConnectFolder>	folders = ReadFolderSettings();

			// 既存登録の確認
			foreach(ConnectFolder folder in folders) {
				if(folder.Path.ToLower().Equals(FolderPath.ToLower())) {
					blnRet = true;
					break;
				}
			}

			return blnRet;
		}

		/// <summary>
		/// 既存のフォルダ接続を削除します
		/// </summary>
		/// <param name="FolderPath">フォルダ・パス</param>
		/// <returns>OK / NG</returns>
		static public bool DeleteConnectFolder(string FolderPath) {
			bool	blnRet = false;

			// 設定の読み込み
			List<ConnectFolder>	regFolders = ReadFolderSettings();

			// 指定のﾌｫﾙﾀﾞｰを削除
			foreach(ConnectFolder folder in regFolders) {
				if(folder.Path.ToLower().Equals(FolderPath.ToLower())) {
					regFolders.Remove(folder);
					break;
				}
			}

			// 設定ﾌｧｲﾙに保存
			blnRet = SaveFolderSettings(regFolders);

			return blnRet;
		}

		/// <summary>
		/// フォルダ接続の表示名を変更します
		/// </summary>
		/// <param name="FolderPath">フォルダ・パス</param>
		/// <param name="NewName">名前</param>
		/// <returns></returns>
		static public bool ReNameConnectFolder(string FolderPath, string NewName) {
			bool	blnRet = false;

			// 設定の読み込み
			List<ConnectFolder>	regFolders = ReadFolderSettings();

			// 指定のﾌｫﾙﾀﾞｰ表示名を変更 (重複OK)
			foreach(ConnectFolder folder in regFolders) {
				if(folder.Path.ToLower().Equals(FolderPath.ToLower())) {
					folder.Name = NewName;
					break;
				}
			}

			// 設定ﾌｧｲﾙに保存
			blnRet = SaveFolderSettings(regFolders);

			return blnRet;
		}

		/// <summary>
		/// 既存の登録をすべて削除します
		/// </summary>
		/// <returns>OK / NG</returns>
		static public bool DeleteAll() {
			return SaveFolderSettings(new List<ConnectFolder>());
		}

		/// <summary>
		/// 設定ファイルからフォルダ接続情報を取得します
		/// </summary>
		/// <returns>フォルダ接続情報</returns>
		static private List<ConnectFolder> ReadFolderSettings() {
			List<ConnectFolder>	folders = new List<ConnectFolder>();

			try {
				// 設定ﾌｧｲﾙ管理ｲﾝｽﾀﾝｽを取得
				OptionSettings	confSetting = new OptionSettings();
				// 設定の読み込み
				folders = confSetting.ConnectFolders;
			}
			catch(Exception ex) {
				// 設定ﾌｧｲﾙ読み込みｴﾗｰ

			}

			return folders;
		}

		/// <summary>
		/// 設定ファイルにフォルダ接続情報を保存します
		/// </summary>
		/// <param name="FolderInfos"></param>
		/// <returns></returns>
		static private bool SaveFolderSettings(List<ConnectFolder> FolderInfos) {
			bool	blnRet = false;

			try {
				// 設定ﾌｧｲﾙ管理ｲﾝｽﾀﾝｽを取得
				OptionSettings	confSetting = new OptionSettings();

				// 設定の保存
				confSetting.ConnectFolders = FolderInfos;
				confSetting.SaveSettings();

				blnRet = true;
			}
			catch(Exception ex) {
				// 設定ﾌｧｲﾙ 読込 / 保存 ｴﾗｰ
                MessageBoxManager.ShowMessageBoxError(Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                    + "[ " + FIELD_NAME_CONNECT_FOLDER + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                    + "[ " + FIELD_NAME_CONNECT_FOLDER + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
			}

			return blnRet;
		}
    }

    /// <summary>
    /// データベース接続管理用クラス
    /// </summary>
    static public class ConnectionFilesManager {
		private const string FOLDER_NAME_CONNECTIONFILE = "Catalog";

		/// <summary>
		/// データベース接続ノードを取得します
		/// </summary>
		/// <returns></returns>
		static public TreeNode[] GetDBConnectFiles() {
			List<TreeNode>	nodeFolders = new List<TreeNode>();

			// 接続ﾌｧｲﾙの読み込み
			string[]	strSDEFiles = SearchConnectionFiles();
			TreeNode	nodeChild;
			foreach(string strSDE in strSDEFiles) {
				// ﾂﾘｰ･ﾉｰﾄﾞを生成
				nodeChild = new TreeNode(Path.GetFileNameWithoutExtension(strSDE),
					ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.DatabaseConnect_Disconnect.GetHashCode(),
					ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.DatabaseConnect_Disconnect.GetHashCode()) { Name = strSDE };
				//nodeChild.Nodes.Add("");
				nodeFolders.Add(nodeChild);
			}

			return nodeFolders.ToArray();
		}

		/// <summary>
		/// DB接続ファイルを追加登録します
		/// </summary>
		/// <param name="DBWS"></param>
		/// <param name="IsSavePassword"></param>
		/// <returns></returns>
		static public TreeNode AddDBConnectFile(IWorkspace DBWS, bool IsSavePassword) {
			TreeNode	nodeNew = null;

			// 接続ﾌｧｲﾙを作成
			IWorkspaceName agWSName = SaveConnectionFile(DBWS, IsSavePassword, "");
			if(agWSName != null) {
				// ﾂﾘｰ･ﾉｰﾄﾞを生成
				nodeNew = new TreeNode(Path.GetFileNameWithoutExtension(agWSName.PathName),
					ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.DatabaseConnect_DB.GetHashCode(),
					ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.DatabaseConnect_DB.GetHashCode()) { Name = agWSName.PathName };
				nodeNew.Nodes.Add("");
			}

			return nodeNew;
		}

		/// <summary>
		/// DB接続ファイル名を変更します
		/// </summary>
		/// <param name="DBConnectionFile"></param>
		/// <param name="AfterName"></param>
		/// <returns></returns>
		static public bool RenameDBConnectFile(string DBConnectionFile, string AfterName) {
			bool	blnRet = false;

			// 現在のﾌｧｲﾙ有無を確認
			if(File.Exists(DBConnectionFile)) {
				// 新しいﾊﾟｽ名を作成
				string	strNewFile = Path.GetDirectoryName(DBConnectionFile) + "\\" + AfterName + Path.GetExtension(DBConnectionFile);

				// 既存ﾁｪｯｸ
				if(!File.Exists(strNewFile)) {
					// ﾜｰｸｽﾍﾟｰｽを開く
					IWorkspace	agWS = LoadWorkspace(DBConnectionFile);

					// 名称を変更
					var	agRDBF = agWS.WorkspaceFactory as IRemoteDatabaseWorkspaceFactory;
					try {
						IWorkspaceName	agWSName = agRDBF.RenameConnectionFile(DBConnectionFile, AfterName);
						// 結果検証
						blnRet = agWSName.PathName == strNewFile;
					}
					catch(Exception ex) {
	#if DEBUG
						Debug.WriteLine("DB接続ファイル名変更 : NG : " + ex.Message);
	#endif
					}
				}
			}

			return blnRet;
		}

		/// <summary>
		/// DB接続ファイルを削除します
		/// </summary>
		/// <param name="DBWS"></param>
		/// <param name="DBConnectionFile"></param>
		static public bool DeleteDBConnectFile(string DBConnectionFile) {
			bool	blnRet = true;

			// 現在のﾌｧｲﾙ有無を確認
			if(File.Exists(DBConnectionFile)) {
				// ﾜｰｸｽﾍﾟｰｽを開く
				IWorkspace	agWS = LoadWorkspace(DBConnectionFile);

				var	agRDBF = agWS.WorkspaceFactory as IRemoteDatabaseWorkspaceFactory;
				try {
					agRDBF.DeleteConnectionFile(DBConnectionFile);
				}
				catch(Exception ex) {
#if DEBUG
					Debug.WriteLine("Delete Error : " + ex.Message);
#endif
					blnRet = false;
				}
			}

			return blnRet;
		}

		/// <summary>
		/// DB接続ファイルをワークスペースに展開します
		/// </summary>
		/// <param name="DBConnectionFile"></param>
		/// <returns></returns>
		static public IWorkspace LoadWorkspace(string DBConnectionFile) {
			IWorkspaceFactory	agWSF = null;
			IWorkspace			agWS = null;

			// SDE
			if(Path.GetExtension(DBConnectionFile).ToLower().Equals(".sde")) {
				agWSF = Common.SingletonUtility.NewSdeWorkspaceFactory();
			}
			// Remote DB
			else {
				agWSF = Common.SingletonUtility.NewSqlWorkspaceFactory();
			}

			// ﾜｰｸｽﾍﾟｰｽを開く
			try {
				agWS = agWSF.OpenFromFile(DBConnectionFile, 0);
			}
			catch(Exception ex) {
				#if DEBUG
				Debug.WriteLine("WS ERROR : " + ex.Message);
				#endif
			}

			return agWS;
		}
		static public IWorkspace LoadWorkspace(IPropertySet ConnectProperty) {
			IWorkspaceFactory	agWSF = null;
			IWorkspace			agWS = null;

			// SDE
			string strSqlProp = (string)ConnectProperty.GetProperty("dbclient");
			if(string.IsNullOrEmpty(strSqlProp)) {
				agWSF = Common.SingletonUtility.NewSdeWorkspaceFactory();
			}
			// Remote DB
			else {
				agWSF = Common.SingletonUtility.NewSqlWorkspaceFactory();
			}

			// ﾜｰｸｽﾍﾟｰｽを開く
			try {
				agWS = agWSF.Open(ConnectProperty, 0);
			}
			catch(Exception ex) {
				#if DEBUG
				Debug.WriteLine("WS ERROR : " + ex.Message);
				#endif
			}

			return agWS;
		}

		/// <summary>
		/// DB接続ファイルを保存します
		/// </summary>
		/// <param name="DBWS"></param>
		/// <param name="SavePassword"></param>
		/// <param name="FileName"></param>
		/// <returns></returns>
		static private IWorkspaceName SaveConnectionFile(IWorkspace DBWS, bool SavePassword, string FileName) {
			IWorkspaceName		agWSName = null;

			// 設定ﾌｧｲﾙ保存ﾌｫﾙﾀﾞｰを取得
			string	strUserFolder = Ui.FormOptionSettings.UserSettingFileFolder;
			strUserFolder = Path.Combine(strUserFolder, FOLDER_NAME_CONNECTIONFILE);

			bool	blnExistDir = true;

			// 保存ﾃﾞｨﾚｸﾄﾘを作成
			if(!Directory.Exists(strUserFolder)) {
				try {
					Directory.CreateDirectory(strUserFolder);
				}
				catch(Exception ex) {
					ESRIJapan.GISLight10.Common.Logger.Error("フォルダの作成に失敗");
					ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
					ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);
					blnExistDir = false;
				}
			}

			if(blnExistDir) {
				// ﾌｧｲﾙ名の調整
				string	strNodeName;
				if(string.IsNullOrEmpty(FileName)) {
					strNodeName = string.Format("Connection to {0}", DBWS.ConnectionProperties.GetProperty("serverinstance"));

					// ﾌｧｲﾙ名が重複する場合は、ﾅﾝﾊﾞﾘﾝｸﾞ
					string[]	strTempFile = Directory.GetFiles(strUserFolder, strNodeName + "*");
					if(strTempFile.Length > 0) {
						string		strTemp = "";
						int intCnt = 2;
						while(strTempFile.Length > 0) {
							strTemp = string.Format(strNodeName + " ({0})", intCnt++);
							strTempFile = Directory.GetFiles(strUserFolder, strTemp);
						}

						strNodeName = strTemp;
					}
				}
				else {
					strNodeName = FileName;
				}

				// 保存
				IPropertySet		agPropSet = new PropertySetClass();
				// ﾊﾟｽﾜｰﾄﾞの保存指定
				if(SavePassword) {
					agPropSet = DBWS.ConnectionProperties;
				}
				else {
					// 接続ﾌﾟﾛﾊﾟﾃｨを取得
					object	objNames, objVals;
					DBWS.ConnectionProperties.GetAllProperties(out objNames, out objVals);

					// ﾊﾟｽﾜｰﾄﾞ抜き
					string[]	strNames = (string[])objNames;
					for(int intCnt=0; intCnt < strNames.Length; intCnt++) {
						if(!strNames[intCnt].ToLower().Equals("password")) {
							agPropSet.SetProperty(strNames[intCnt], ((object[])objVals)[intCnt]);
						}
					}
				}

				IWorkspaceFactory	agWSF = DBWS.WorkspaceFactory;
				agWSName = agWSF.Create(strUserFolder, strNodeName, agPropSet, 0);
			}

			return agWSName;
		}

		/// <summary>
		/// DB接続ファイルを取得します
		/// </summary>
		/// <returns></returns>
		static private string[] SearchConnectionFiles() {
			IEnumerable<string>	confiles = new string[0];

			// 設定ﾌｧｲﾙ保存ﾌｫﾙﾀﾞｰを取得
			string	strUserFolder = Ui.FormOptionSettings.UserSettingFileFolder;
			strUserFolder = Path.Combine(strUserFolder, FOLDER_NAME_CONNECTIONFILE);

			// 接続ﾌｧｲﾙを探索
			if(Directory.Exists(strUserFolder)) {
				confiles = Directory.GetFiles(strUserFolder, "*.sde").Concat(Directory.GetFiles(strUserFolder, "*.qcf"));
			}

			return confiles.ToArray();
		}
    }
}
