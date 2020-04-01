using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.AccessControl;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class LightTreeView : UserControl
    {
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
        enum ICON
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
            ShapefileLine = 17,
            ShapefilePoint = 18,
            ShapefilePolygon = 19,
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
            TextFile = 39
        }
        #endregion



        public LightTreeView()
        {
            InitializeComponent();

        }

        ~LightTreeView()
        {
            ReleaseWsfs();
        }

        private void ReleaseWsfs()
        {

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
            //catch (System.Runtime.InteropServices.COMException comex)
            //{ 
                
            //}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "エラー!! BeforeExpandイベント");
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = preCursor;
            }

        }

        private void LightTreeView_Load(object sender, EventArgs e)
        {
            if (blInit)
                return;


            tvWs.Nodes.Clear();
            tvWs.ImageList = imgListIcon;

            // ドライブをノードに追加
            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                if (driveInfo.IsReady)
                {
                    TreeNode treeNode = new TreeNode(driveInfo.Name);
                    treeNode.Nodes.Add("");
                    tvWs.Nodes.Add(treeNode);
                }

            }

            if (System.IO.Directory.Exists(startFolder))
            {
                string root = System.IO.Path.GetPathRoot(startFolder);
                string[] snames = startFolder.Split(new[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                foreach (TreeNode treeNode in tvWs.Nodes)
                { 
                    if (treeNode.Text.Equals(root)) // Drive
                    {
                        treeNode.Nodes.Clear();
                        // ドライブ直下

                        TreeNode childNode = new TreeNode(snames[1]);
                        TreeNode[] nodes = new TreeNode[snames.Length - 1];
                        string fullpath = treeNode.Text;
                        for (int i = 1; i < snames.Length; i++)
                        {
                            nodes[i-1] = new TreeNode(snames[i]);
                            fullpath += "\\" + snames[i];
                            if (i > 1)
                            {
                                if (System.IO.Directory.Exists(fullpath))
                                {
                                    nodes[i - 2].Nodes.Add(nodes[i - 1]);
                                }
                            }
                        }
                        treeNode.Nodes.Add(nodes[0]);
                        //treeNode.Expand();
                        // 指定のパスだけExpandしていくロジックが必要
                        treeNode.ExpandAll();
                        
                        TreeNode child = null;
                        for (int i = 1; i < snames.Length; i++)
                        {
                            if (i == 1)
                            {
                                child = FindChildNode(treeNode, snames[i]);
                                // 2013/01/29 ADD  >>>>>
                                SetWorkspaceNode(child, 0);
                                // 2013/01/29 ADD  <<<<<
                                child.Expand();
                            }
                            else
                            {
                                child = FindChildNode(child, snames[i]);
                                SetWorkspaceNode(child, 0);
                                child.Expand();
                            }
                        }

                    }
                }

            }

            // イベントハンドラを追加
            this.tvWs.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvWs_BeforeExpand);
            this.tvWs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvWs_AfterSelect);

            if (this.tvWs.AllowDrop)
            {
                this.tvWs.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvWs_ItemDrag);
            }

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
        /// tvWsにフォルダダイアログで指定したフォルダを追加
        /// </summary>
        /// <param name="dirpath"></param>
        private void tvWsAddNode(string dirpath)
        {
            if (System.IO.Directory.Exists(dirpath))
            {
                TreeNode treeNode = new TreeNode(dirpath);
                treeNode.Nodes.Add("");
                // 重複チェック
                bool exist = false;
                foreach (TreeNode node in tvWs.Nodes)
                {
                    if (node.Text.Equals(dirpath))
                        exist = true;
                }
                if (!exist)
                {
                    tvWs.Nodes.Add(treeNode);
                }
            }
            else
            {
                MessageBox.Show(dirpath + "が存在しません。", "エラー!! AddNodeイベント");
            }
        }


        /// <summary>
        /// GDB内のデータタイプに応じてListViewItemを作成
        /// </summary>
        /// <param name="dsName"></param>
        /// <returns></returns>
        private ListViewItem CreateGdbDatasetItem(IDatasetName dsName)
        {
            string[] subinfo = new string[2];
            subinfo[0] = dsName.Name;
            subinfo[1] = dsName.Category;
            ListViewItem item = new ListViewItem(subinfo);

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
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                            item.ImageIndex = (int)ICON.GeodatabaseFeatureClassMultipoint;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                            item.ImageIndex = (int)ICON.GeodatabaseFeatureClassPoint;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                            if (fcName.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                item.ImageIndex = (int)ICON.GeodatabaseFeatureClassAnnotation;
                            }
                            else 
                            {
                                item.ImageIndex = (int)ICON.GeodatabaseFeatureClassPolygon;
                            }
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                            item.ImageIndex = (int)ICON.GeodatabaseFeatureClassLine;
                            break;
                    }
                    break;
                case esriDatasetType.esriDTFeatureDataset:
                    item.ImageIndex = (int)ICON.GeodatabaseFeatureDataset;
                    break;
                case esriDatasetType.esriDTGeo:
                    break;
                case esriDatasetType.esriDTGeometricNetwork:
                    item.ImageIndex = (int)ICON.GeodatabaseNetworkGeometric;
                    break;
                case esriDatasetType.esriDTLayer:
                    break;
                case esriDatasetType.esriDTLocator: // LocatorがDatasetNameでとれない
                    item.ImageIndex = (int)ICON.GeocodeAddressLocator;
                    break;
                case esriDatasetType.esriDTMap:
                    break;
                case esriDatasetType.esriDTMosaicDataset:
                    item.ImageIndex = (int)ICON.GeodatabaseMosaicDataset;
                    break;
                case esriDatasetType.esriDTNetworkDataset:
                    item.ImageIndex = (int)ICON.FileNetworkDataset;
                    break;
                case esriDatasetType.esriDTPlanarGraph:
                    break;
                case esriDatasetType.esriDTRasterBand:
                    break;
                case esriDatasetType.esriDTRasterCatalog:
                    item.ImageIndex = (int)ICON.GeodatabaseRasterCatalog;
                    break;
                case esriDatasetType.esriDTRasterDataset:
                    item.ImageIndex = (int)ICON.FileRasterGrid;
                    break;
                case esriDatasetType.esriDTRelationshipClass:
                    item.ImageIndex = (int)ICON.GeodatabaseRelationship;
                    break;
                case esriDatasetType.esriDTRepresentationClass:
                    break;
                case esriDatasetType.esriDTSchematicDataset:
                    break;
                case esriDatasetType.esriDTStyle:
                    break;
                case esriDatasetType.esriDTTable:
                    item.ImageIndex = (int)ICON.TableStandalone;
                    break;
                case esriDatasetType.esriDTTerrain:
                    item.ImageIndex = (int)ICON.GeodatabaseTerrain;
                    break;
                case esriDatasetType.esriDTText:
                    break;
                case esriDatasetType.esriDTTin:
                    break;
                case esriDatasetType.esriDTTool:
                    break;
                case esriDatasetType.esriDTToolbox:
                    item.ImageIndex = (int)ICON.GeoprocessingToolbox;
                    break;
                case esriDatasetType.esriDTTopology:
                    item.ImageIndex = (int)ICON.GeodatabaseTopology;
                    break;
                default:
                    break;
            }
            #endregion

            return item;
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
        /// シェープファイルのタイプに応じてListViewItemを作成
        /// </summary>
        /// <param name="dsName"></param>
        /// <returns></returns>
        private ListViewItem CreateShapeDatasetItem(IDatasetName dsName)
        {
            string[] subinfo = new string[2];
            subinfo[0] = dsName.Name + ".shp";
            subinfo[1] = dsName.Category;
            ListViewItem item = new ListViewItem(subinfo);

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
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                            item.ImageIndex = (int)ICON.ShapefilePoint;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                            item.ImageIndex = (int)ICON.ShapefilePoint;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                            item.ImageIndex = (int)ICON.ShapefilePolygon;
                            break;
                        case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                            item.ImageIndex = (int)ICON.ShapefileLine;
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
                    item.ImageIndex = (int)ICON.FileNetworkDataset;
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
                    item.ImageIndex = (int)ICON.TableStandalone;
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

            return item;
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
                    switch (fcName.ShapeType)
                    {
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


            string fullpath = parentNode.FullPath;

            // TreeNodeがFGDBの場合
            if (IsGdbWorkspace(fullpath)) // FGDB
            {
                parentNode.Nodes.Clear(); // FGDB内のツリー削除

                //IWorkspace ws = GetFgdbWorkspace(fullpath);//GetGdbWorkspace(fullpath);
                IWorkspace ws = null;
                if (System.IO.Directory.Exists(fullpath))
                {
                    ws = GetFgdbWorkspace(fullpath);
                }
                else 
                {
                    ws = GetPgdbWorkspace(fullpath);
                }
                if (ws == null)
                    return;

                IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTAny);
                enumDsName.Reset();
                IDatasetName dsName = enumDsName.Next();
                while (dsName != null)
                {
                    TreeNode node = CreateGdbDatasetNode(dsName);
                    parentNode.Nodes.Add(node);                    
                    dsName = enumDsName.Next();
                }
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDsName);
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            }
            else if (System.IO.Directory.Exists(fullpath)) // TreeNodeがフォルダの場合
            {
                parentNode.Nodes.Clear();

                DirectoryInfo dirInfo = new DirectoryInfo(fullpath);

                // サブフォルダ
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    //System.IO.FileAttributes attr = dir.Attributes;
                    if (!dir.ToString().ToUpper().Equals("INFO"))
                    { 
                        if (!dir.Attributes.ToString().ToUpper().Contains("HIDDEN")) // 隠しフォルダは対象外とする
                        {

                            // アクセス許可がある場合だけ
                            try 
                            {
                                TreeNode node = new TreeNode(dir.Name);
                                IWorkspace ws = null;
                                if (IsGdbWorkspace(dir.FullName)) // FGDB
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
                            catch (UnauthorizedAccessException unauthex)
                            {
                                continue;
                            }

                        }                    

                    }


                }

                if (IsCadWorkspace(fullpath)) // CADデータの存在チェック
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

                if (IsRasterWorkspace(fullpath)) // ラスタデータの存在チェック
                {
                    IWorkspace ws = GetRasterWorkspace(fullpath);
                    IEnumDatasetName enumDsName = ws.get_DatasetNames(esriDatasetType.esriDTRasterDataset);
                    enumDsName.Reset();
                    IDatasetName dsName = enumDsName.Next();
                    while (dsName != null)
                    {
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

                if (IsShapeWorkspace(fullpath)) // シェープファイルとDBFの存在チャック
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

                if (IsTextWorkspace(fullpath)) // CSVファイルとTXTファイルの存在チェック
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

                // ファイルのノードを追加
                SetFileNode(parentNode, dirInfo);


            }

        }

        /// <summary>
        /// TreeNode下のファイルを変更する機能
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="dirInfo"></param>
        private void SetFileNode(TreeNode parentNode, DirectoryInfo dirInfo)
        {

            string fullpath = parentNode.FullPath;

            // ファイル
            foreach (FileInfo file in dirInfo.GetFiles("*.mdb")) //MDB
            {
                //string ext = System.IO.Path.GetExtension(file.FullName);

                if (IsGdbWorkspace(file.FullName))
                {
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

            }

            foreach (FileInfo file in dirInfo.GetFiles("*.mxd"))// マップドキュメント
            {
                TreeNode node = new TreeNode(file.Name);
                node.ImageIndex = node.SelectedImageIndex = (int)ICON.ArcMap_MXD_File;
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

        /// <summary>
        /// GDBワークスペースか
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsGdbWorkspace(string path)
        {
            string ex = System.IO.Path.GetExtension(path);
            if (ex == null)
                return false;

            if (ex.ToUpper().Equals(".GDB") || ex.ToUpper().Equals(".MDB"))
            {
                return true;
            }

            return false;

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

            try
            {
                TreeNode currentNode = e.Node;
                                
                selectPath = currentNode.FullPath;
                txtSelectPath.Text = selectPath;

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
                else if (currentNode.Tag is System.Collections.Generic.List<IFileName>)
                {
                    returnObject = true;
                    string txt = "";
                    List<IFileName> fileName = (List<IFileName>)currentNode.Tag;
                    foreach (var fname in fileName)
                    {
                        txt += "LAYER ファイル ,";
                    }
                    txtSelType.Text = txt.Remove(txt.Length - 1);
                    selectObject = currentNode.Tag;
                }
                else
                {
                    returnObject = false;
                    if (System.IO.Directory.Exists(currentNode.FullPath))
                    {
                        txtSelType.Text = "フォルダ";
                    }
                    else if (System.IO.File.Exists(currentNode.FullPath))
                    {
                        string ext = System.IO.Path.GetExtension(currentNode.FullPath);
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
                
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "エラー!! AfterSelect");
            }
            finally 
            {
                System.Windows.Forms.Cursor.Current = preCursor;
            }


        }

        private void tvWs_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (selectObject != null)
            {
                // ドラッグ&ドロップ可能なものをIDatasetNameとIFileNameとする
                // DatasetNameでも限定
                if (selectObject is List<IDatasetName>)
                {
                    IDatasetName dsName = ((List<IDatasetName>)selectObject)[0];
                    switch (dsName.Type)
                    {
                        case esriDatasetType.esriDTCadDrawing:
                        case esriDatasetType.esriDTFeatureClass:
                        case esriDatasetType.esriDTFeatureDataset:
                        case esriDatasetType.esriDTRasterCatalog:
                        case esriDatasetType.esriDTRasterDataset:
                            IDataObject dataObject = new DataObject();
                            //dataObject.SetData("ESRI.ArcGIS.Geodatabase.IDatasetName", dsName);
                            IObjectCopy objCopy = new ObjectCopyClass();
                            object obj = objCopy.Copy((object)dsName); // DeepCopy
                            dataObject.SetData("ESRI.ArcGIS.Geodatabase.IDatasetName", (IDatasetName)obj);
                            tvWs.DoDragDrop(dataObject, DragDropEffects.Copy);
                            break;
                    }
                    return;
                }

                if (selectObject is List<IFileName>)
                {
                    IFileName fileName = ((List<IFileName>)selectObject)[0];
                    IDataObject dataObject = new DataObject();
                    IObjectCopy objCopy = new ObjectCopyClass();
                    object obj = objCopy.Copy((object)fileName); // DeepCopy
                    dataObject.SetData("ESRI.ArcGIS.esriSystem.IFileName", (IFileName)obj);
                    tvWs.DoDragDrop(dataObject, DragDropEffects.Copy);
                    return;
                }

            }

        }



        //private void textBox1_DragDrop(object sender, DragEventArgs e)
        //{
        //    textBox1.Text = "";

        //    IDataObject dataObj = e.Data;
        //    if (dataObj.GetDataPresent("ESRI.ArcGIS.Geodatabase.IDatasetName"))
        //    {
        //        ESRI.ArcGIS.Geodatabase.IDatasetName dsName = (IDatasetName)dataObj.GetData("ESRI.ArcGIS.Geodatabase.IDatasetName");
        //        textBox1.Text = dsName.Name;
        //    }

        //    if (dataObj.GetDataPresent("ESRI.ArcGIS.esriSystem.IFileName"))
        //    {
        //        ESRI.ArcGIS.esriSystem.IFileName fileName = (IFileName)dataObj.GetData("ESRI.ArcGIS.esriSystem.IFileName");
        //        textBox1.Text = fileName.Path;
        //    }

        //}

        //private void textBox1_DragEnter(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Copy;
        //}

        //private void textBox1_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Copy;
        //}

 

    }
}
