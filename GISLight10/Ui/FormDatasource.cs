using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using System.Collections.Specialized;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class FormDatasource : Form
    {
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl;
        private Ui.MainForm m_mainFrm;
        private ILayer _layer;
        private IDatasetName _datasetName; // 元のIDatasetNameオブジェクト
        private IDatasetName _seldatasetName; // 選択ダイアログで選択したIDatasetNameオブジェクト

        private bool blApplyClick = false;


        public FormDatasource(
            ESRI.ArcGIS.Controls.IMapControl3 mapControl, Ui.MainForm mainFrmm, ILayer layer)
        {
            InitializeComponent();

            this.m_mapControl = mapControl;
            this.m_mainFrm = mainFrmm;
            this._layer = layer;
            
        }

        /// <summary>
        /// Layerのデータ情報をListViewに表示
        /// </summary>
        /// <param name="dataLayer"></param>
        private void SetLayerInfo(IDatasetName datasetName)//IDataLayer dataLayer)
        {
            
            //IDatasetName datasetName = (IDatasetName)dataLayer.DataSourceName;
            IWorkspaceName workspaceName = datasetName.WorkspaceName;

            string sname = datasetName.Name;
            IDatasetName reopenDatasetName = OpenDatasetName(workspaceName, sname);
            if (reopenDatasetName != null)
            {
                datasetName = reopenDatasetName;
            }

            _datasetName = datasetName;

            string[] item1 ={"データ種類：", datasetName.Category};
            this.listView1.Items.Add(new ListViewItem(item1));

            string[] item2 = {"名前：", datasetName.Name};
            this.listView1.Items.Add(new ListViewItem(item2));


            string[] item3 = { "場所：", workspaceName.PathName };

            // 2012/08/28 add >>>>>
            // 文字列のピクセル数を取得してリストビューの列幅を設定する。
            Font textFont = this.listView1.Font;
            // 2013/01/29  upd >>>>>
            //Size textSize = TextRenderer.MeasureText(item3[1], textFont);
            Size textSizetmp = TextRenderer.MeasureText(item1[1], textFont);
            Size textSize = TextRenderer.MeasureText(item2[1], textFont);
            // 文字列の長いものを基準とする
            if (textSizetmp.Width > textSize.Width)
            {
                textSize = textSizetmp;
            }
            textSizetmp = TextRenderer.MeasureText(item3[1], textFont);
            // 文字列の長いものを基準とする
            if (textSizetmp.Width > textSize.Width)
            {
                textSize = textSizetmp;
            }
            this.listView1.Columns[1].Width = textSize.Width + 10;

            this.listView1.Items.Add(new ListViewItem(item3));


            string[] item4 = { "", "" };
            this.listView1.Items.Add(new ListViewItem(item4));
        }

        /// <summary>
        /// IDatasetNameのCategoryが取得できないものがあるため、IDatasetNameを再度オープン
        /// </summary>
        /// <param name="wsName"></param>
        /// <param name="sname"></param>
        /// <returns></returns>
        private IDatasetName OpenDatasetName(IWorkspaceName wsName, string sname)
        { 

            IWorkspace workspace = null;
            IDatasetName datasetName = null;
            try
            {
                workspace = wsName.WorkspaceFactory.OpenFromFile(wsName.PathName, 0);
                IEnumDatasetName enumDatasetName = workspace.get_DatasetNames(esriDatasetType.esriDTAny);

                enumDatasetName.Reset();

                datasetName = enumDatasetName.Next();
                while (datasetName != null)
                {
                    if (datasetName.Name.Equals(sname))
                    {
                        return datasetName;
                    }
                    datasetName = enumDatasetName.Next();
                }
                return null;
            }
            catch 
            {
                return null;
            }
            finally 
            {
                if (workspace != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(workspace);
            }
        }

        /// <summary>
        /// IDatasetNameから同名のIDatasetをオープン
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        private IDataset OpenDataset(IDatasetName datasetName)
        {

            IWorkspace ws = null;
            try
            {
                IWorkspaceFactory wsf = datasetName.WorkspaceName.WorkspaceFactory;
                ws = wsf.OpenFromFile(datasetName.WorkspaceName.PathName, 0);

                IEnumDataset enumDataset = null;
                IDataset dataset = null;
                enumDataset = ws.get_Datasets(esriDatasetType.esriDTAny);
                dataset = enumDataset.Next();
                while (dataset != null)
                {
                    if (dataset.Name.Equals(datasetName.Name))
                    {
                        return dataset;
                    }
                    dataset = enumDataset.Next();
                }

                return null;

            }
            catch
            {
                return null;
            }
            finally
            {
                if (ws != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(ws);
            }

        
        }


        /// <summary>
        /// 座標情報をGeodatasetから取得しListViewに表示
        /// </summary>
        /// <param name="geoDataset"></param>
        private void SetSpatialInfo(IGeoDataset geoDataset)
        {

            ISpatialReference spatialRef = geoDataset.SpatialReference;
            if (spatialRef is IProjectedCoordinateSystem)
            {
                string[] item1 = { "投影座標系：",spatialRef.Name};
                this.listView1.Items.Add(new ListViewItem(item1));
            }
            else if (spatialRef is IGeographicCoordinateSystem)
            {
                string[] item1 = { "地理座標系：", spatialRef.Name };
                this.listView1.Items.Add(new ListViewItem(item1));
            }
            else if (spatialRef is IUnknownCoordinateSystem)
            {
                string[] item1 = { "座標系：", spatialRef.Name };
                this.listView1.Items.Add(new ListViewItem(item1));
            }
        }
 

        /// <summary>
        /// データソースの設定をクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetDatasource_Click(object sender, EventArgs e)
        {

            _seldatasetName = null;

            //DialogResult:OK
            using (FormGISDataSelectDialog frm = new FormGISDataSelectDialog())
            {
                frm.SelectType = FormGISDataSelectDialog.ReturnType.DatasetName;
                if (_datasetName != null)
                {
                    frm.StartFolder = _datasetName.WorkspaceName.PathName;
                }

                DialogResult result = frm.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    IDatasetName selectDatasetName = ((List<IDatasetName>)frm.SelectedObject)[0];
                    if (selectDatasetName.Type == _datasetName.Type)
                    {

                        try
                        {
                            _seldatasetName = selectDatasetName;
                            ResetDatainfo();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("選択したデータの情報を取得できません");//FormDatasource_ErrorGetDataInfo
                            Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.FormDatasource_ErrorGetDataInfo);
                        }
                    }
                    else 
                    {
                        //MessageBox.Show("異なるデータの種類を設定できません");//FormDatasource_ErrorDifDatatype
                        Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.FormDatasource_ErrorDifDatatype);
                    }

                }
            }



        }

        /// <summary>
        /// 選択したデータ情報をListViewに表示
        /// </summary>
        private void ResetDatainfo()
        {

            this.listView1.Items.Clear();
            this.listView1.HeaderStyle = ColumnHeaderStyle.None;
            this.listView1.Scrollable = true;

            SetLayerInfo(_seldatasetName);

            IDataset dataset = OpenDataset(_seldatasetName);
            if (dataset is IGeoDataset) 
            {
                IGeoDataset geodataset = (IGeoDataset)dataset;

                SetSpatialInfo(geodataset);
            }

            btnApply.Enabled = true;
        }

        /// <summary>
        /// 初期のレイヤデータ情報をListViewに表示
        /// </summary>
        private void SetDatainfo()
        {

            _layer = m_mainFrm.SelectedLayer; 


            this.listView1.Items.Clear();
            //this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listView1.HeaderStyle = ColumnHeaderStyle.None;
            this.listView1.Scrollable = true;

            if (_layer is IDataLayer)
            {
                IDataLayer dataLayer = (IDataLayer)_layer;

                //SetLayerInfo(dataLayer);
                SetLayerInfo((IDatasetName)dataLayer.DataSourceName);
            }

            if (_layer.Valid)
            {
                if (_layer is IGeoDataset)
                {
                    IGeoDataset geodataset = (IGeoDataset)_layer;
                    SetSpatialInfo(geodataset);
                }
            }

            btnApply.Enabled = false;
        }

        private void FormDatasource_Load(object sender, EventArgs e)
        {
            SetDatainfo();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if ((btnApply.Enabled) && (_seldatasetName != null))
            { 
                this.RefreshLayerDisplay();
                btnApply.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ((btnApply.Enabled) && (_seldatasetName != null))
            {
				this.RefreshLayerDisplay();
            }
            this.Close();
        }
        
        private void RefreshLayerDisplay() {
            IDataLayer dataLayer = (IDataLayer)_layer;
            dataLayer.DataSourceName = (IName)_seldatasetName;
            
            // 描画更新
			m_mapControl.ActiveView.ContentsChanged();
            m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, _layer, m_mapControl.Extent);
        }


    }





}

