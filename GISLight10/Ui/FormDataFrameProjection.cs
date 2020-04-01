using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// 座標系の設定機能
    /// </summary>
    /// <hitory>
    /// 2012-08-03 新規作成 
    /// </hitory>
    public partial class FormDataFrameProjection : Form
    {

        private IMapControl3 m_mapControl;

        private int sourceIndex = 0;
        private int sourceRasterIndex = 0;
        private IDictionary sourceLayerList = null;
        private IDictionary sourceRasterLayerList = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormDataFrameProjection(IMapControl3 mapControl)
        {
            try
            {
                InitializeComponent();
                this.m_mapControl = mapControl;

                this.sourceIndex = 0;
                this.sourceLayerList = new Dictionary<int, IFeatureLayer>();
                sourceLayerList.Clear();

                this.sourceRasterIndex = 0;
                this.sourceRasterLayerList = new Dictionary<int, IRasterLayer>();
                sourceRasterLayerList.Clear();

                // データフレームの座標系の取得
                SetTxtDataFrameProjection();

                // レイヤツリービューの設定
                SetTreeViewLayerList();

            }
            catch (COMException comex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormDataFrameProjection_ERROR_Initialize);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormDataFrameProjection_ERROR_Initialize);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Close();
            }
        }

        /// <summary>
        /// データフレームの座標系の取得
        /// </summary>
        private void SetTxtDataFrameProjection()
        {
            this.txtDataFrameProjction.Text = this.m_mapControl.Map.SpatialReference.Name;
        }

        /// <summary>
        /// ツリービューの設定
        /// </summary>
        private void SetTreeViewLayerList()
        {

            // TreeViewのノード設定
            treeViewLayerList.Nodes.Clear();

            // TreeViewのイメージリスト設定
            treeViewLayerList.ImageList = this.imageListIcons;

            // 上位から順にレイヤを取得
            if (m_mapControl.Map.LayerCount > 0)
            {
                for (int i = 0; i < m_mapControl.Map.LayerCount; i++)
                {
                    ILayer sourceLayer = m_mapControl.Map.get_Layer(i);

                    // ノードの設定
                    SetTreeViewFeatureLayer(sourceLayer, null);
                }
            }
        }

        /// <summary>
        /// ノードの設定
        /// </summary>
        private void SetTreeViewFeatureLayer(ILayer sourceLayer, TreeNode parentNode)
        {

            try
            {
                IGeoDataset geoDataset = null;
                ISpatialReference spRef = null;
                TreeNode treeNodeSpatialReference = null;

                geoDataset = sourceLayer as IGeoDataset;
                if (geoDataset != null)
                {
                    spRef = geoDataset.SpatialReference;
                }

                // リンク切れは除外
                if (sourceLayer.Valid == true)
                {
                    // フィーチャレイヤ
                    if (sourceLayer is FeatureLayer)
                    {
                        TreeNode treeNodeLayer = new TreeNode(sourceLayer.Name);
                        
                        // アイコンの設定
                        treeNodeLayer.ImageIndex = 1;
                        treeNodeLayer.SelectedImageIndex = 1;

                        // 座標系ノード
                        treeNodeSpatialReference = new TreeNode(spRef.Name);
                        treeNodeSpatialReference.ImageIndex = 0;
                        treeNodeSpatialReference.SelectedImageIndex = 0;

                        // 親ノードが存在する場合
                        if (parentNode != null)
                        {
                            parentNode.Nodes.Add(treeNodeLayer);
                            treeNodeLayer.Nodes.Add(treeNodeSpatialReference);
                        }
                        else
                        {
                            treeViewLayerList.Nodes.Add(treeNodeLayer);
                            treeNodeLayer.Nodes.Add(treeNodeSpatialReference);
                        }

                        IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
                        // フィーチャレイヤ情報を保存
                        SetSourceFeatureLayer(sourceFeatureLayer);
                    }
                    // ラスタレイヤ
                    else if (sourceLayer is RasterLayer)
                    {
                        TreeNode treeNodeLayer = new TreeNode(sourceLayer.Name);

                        // アイコンの設定
                        treeNodeLayer.ImageIndex = 2;
                        treeNodeLayer.SelectedImageIndex = 2;

                        // 座標系ノード
                        treeNodeSpatialReference = new TreeNode(spRef.Name);
                        treeNodeSpatialReference.ImageIndex = 0;
                        treeNodeSpatialReference.SelectedImageIndex = 0;

                        // 親ノードが存在する場合
                        if (parentNode != null)
                        {
                            parentNode.Nodes.Add(treeNodeLayer);
                            treeNodeLayer.Nodes.Add(treeNodeSpatialReference);
                        }
                        else
                        {
                            treeViewLayerList.Nodes.Add(treeNodeLayer);
                            treeNodeLayer.Nodes.Add(treeNodeSpatialReference);
                        }

                        IRasterLayer sourceFeatureLayer = (IRasterLayer)sourceLayer;
                        // ラスタレイヤ情報を保存
                        SetSourceRasterLayer(sourceFeatureLayer);
                    }
                    // グループレイヤ
                    else if (sourceLayer is GroupLayer && !(sourceLayer is BasemapLayer))
                    {
                        ICompositeLayer sourceCompositeLayer = (ICompositeLayer)sourceLayer;

                        // グループレイヤ下にレイヤが存在
                        if (sourceCompositeLayer.Count > 0)
                        {
                            TreeNode treeNodeLayer = new TreeNode(sourceLayer.Name);

                            // グループレイヤのアイコンを設定
                            treeNodeLayer.ImageIndex = 3;
                            treeNodeLayer.SelectedImageIndex = 3;

                            // 親ノードが存在する場合
                            if (parentNode != null)
                            {
                                parentNode.Nodes.Add(treeNodeLayer);
                            }
                            else
                            {
                                treeViewLayerList.Nodes.Add(treeNodeLayer);
                            }

                            // レイヤを再帰的に設定
                            for (int i = 0; i < sourceCompositeLayer.Count; i++)
                            {
                                SetTreeViewFeatureLayer(sourceCompositeLayer.get_Layer(i), treeNodeLayer);
                            }

                            // 子ノードがない場合ノードを削除
                            if (treeNodeLayer.GetNodeCount(true) == 0)
                            {
                                treeViewLayerList.Nodes.Remove(treeNodeLayer);
                            }
                        }
                    }
                    // ベースマップレイヤ
                    else if (sourceLayer is GroupLayer && sourceLayer is BasemapLayer)
                    {
                        ICompositeLayer sourceCompositeLayer = (ICompositeLayer)sourceLayer;

                        // ベースマップレイヤ下にレイヤが存在
                        if (sourceCompositeLayer.Count > 0)
                        {
                            TreeNode treeNodeLayer = new TreeNode(sourceLayer.Name);

                            // ベースマップレイヤのアイコンを設定
                            treeNodeLayer.ImageIndex = 4;
                            treeNodeLayer.SelectedImageIndex = 4;

                            // 親ノードが存在する場合
                            if (parentNode != null)
                            {
                                parentNode.Nodes.Add(treeNodeLayer);
                            }
                            else
                            {
                                treeViewLayerList.Nodes.Add(treeNodeLayer);
                            }

                            // レイヤを再帰的に設定
                            for (int i = 0; i < sourceCompositeLayer.Count; i++)
                            {
                                IBasemapSubLayer basemapSubLayer = sourceCompositeLayer.get_Layer(i) as IBasemapSubLayer;
                                ILayer innerLayer = basemapSubLayer.Layer;
                                SetTreeViewFeatureLayer(innerLayer, treeNodeLayer);
                            }

                            // 子ノードがない場合ノードを削除
                            if (treeNodeLayer.GetNodeCount(true) == 0)
                            {
                                treeViewLayerList.Nodes.Remove(treeNodeLayer);
                            }
                        }
                    }
                }
            }
            catch (COMException comex)
            {
                throw comex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// フィーチャレイヤ情報を保存
        /// </summary>
        /// <param name="featureLayer">フィーチャレイヤ</param>
        private void SetSourceFeatureLayer(IFeatureLayer featureLayer)
        {
            sourceLayerList[sourceIndex] = featureLayer;
            sourceIndex++;
        }

        /// <summary>
        /// ラスタレイヤ情報を保存
        /// </summary>
        /// <param name="featureLayer">ラスタレイヤ</param>
        private void SetSourceRasterLayer(IRasterLayer featureLayer)
        {
            sourceRasterLayerList[sourceRasterIndex] = featureLayer;
            sourceRasterIndex++;
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            bool selectFlg = false;

            try
            {
                // Unknownは設定させない
                if (treeViewLayerList.SelectedNode.Text.ToLower() == "unknown")
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormDataFrameProjection_WARNING_Unknown);
                    return;
                }

                // フィーチャレイヤを検索
                for (int i = 0; i < sourceLayerList.Count; i++)
                {
                    IFeatureLayer targetFeatureLayer = (IFeatureLayer)sourceLayerList[i];
                    IGeoDataset geoDataset = targetFeatureLayer as IGeoDataset;
                    ISpatialReference spRef = geoDataset.SpatialReference;

                    if (treeViewLayerList.SelectedNode.Text == spRef.Name)
                    {
                        // データフレームに座標系を設定
                        this.m_mapControl.Map.SpatialReference = spRef;
                        this.m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                        selectFlg = true;
                        break;
                    }
                }

                if (selectFlg == false)
                {
                    // ラスタレイヤを検索
                    for (int i = 0; i < sourceRasterLayerList.Count; i++)
                    {
                        IRasterLayer targetFeatureLayer = (IRasterLayer)sourceRasterLayerList[i];
                        IGeoDataset geoDataset = targetFeatureLayer as IGeoDataset;
                        ISpatialReference spRef = geoDataset.SpatialReference;

                        if (treeViewLayerList.SelectedNode.Text == spRef.Name)
                        {
                            // データフレームに座標系を設定
                            this.m_mapControl.Map.SpatialReference = spRef;
                            this.m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                            selectFlg = true;
                            break;
                        }
                    }
                }

                // 選択チェック
                if (selectFlg == false)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormDataFrameProjection_WARNING_SelectProjection);
                    return;
                }

                this.Close();
            }
            catch (COMException comex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormDataFrameProjection_ERROR_SetDataFrameProjection);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormDataFrameProjection_ERROR_SetDataFrameProjection);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
