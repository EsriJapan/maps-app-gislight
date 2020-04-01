using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    /// 選択可能レイヤの設定機能
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormSelectableLayerSettings : Form
    {
        /// <summary>
        /// 選択可能レイヤの設定を行うレイヤを取得するマップコントロール
        /// </summary>
        private IMapControl3 m_mapControl;

        /// <summary>
        /// フィーチャレイヤ情報保存用
        /// </summary>
        private int sourceIndex;
        private IDictionary sourceSelectableLayer = null;

        /// <summary>
        /// チェック状態保存用
        /// </summary>
        private int targetIndex;
        private IDictionary targetSelectableLayer = null;

        /// <summary>
        /// コンストラクタ
        /// 各変数、UIの初期化を行う
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormSelectableLayerSettings(IMapControl3 mapControl)
        {
            try
            {
                InitializeComponent();
                this.m_mapControl = mapControl;

                this.sourceIndex = 0;
                this.sourceSelectableLayer = new Dictionary<int, IFeatureLayer>();
                sourceSelectableLayer.Clear();

                this.targetIndex = 0;
                this.targetSelectableLayer = new Dictionary<int, bool>();
                targetSelectableLayer.Clear();

                // レイヤ一覧TreeViewの初期化
                SetupTreeViewLayerList();
            }
            catch (COMException comex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_Initialize);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_Initialize);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Close();
            }
        }

        /// <summary>
        /// レイヤ一覧TreeViewの設定
        /// </summary>
        private void SetupTreeViewLayerList()
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

            // すべてのノードを展開
            treeViewLayerList.ExpandAll();
        }

        /// <summary>
        /// ノードの設定
        /// </summary>
        private void SetTreeViewFeatureLayer(ILayer sourceLayer, TreeNode parentNode)
        {
            // リンク切れは除外
            if (sourceLayer.Valid == true)
            {
                // フィーチャレイヤ
                if (sourceLayer is FeatureLayer)
                {
                    //if (!(sourceLayer is GdbRasterCatalogLayer) && !(sourceLayer is CadFeatureLayer))
                    //{
                    //    if (LayerManager.getWorkspace((IFeatureLayer)sourceLayer).Type !=
                    //        esriWorkspaceType.esriRemoteDatabaseWorkspace)
                    //    {
                            TreeNode treeNodeLayer = new TreeNode(sourceLayer.Name);

                            // 親ノードが存在する場合
                            if (parentNode != null)
                            {    
                                parentNode.Nodes.Add(treeNodeLayer);
                            }
                            else
                            {
                                treeViewLayerList.Nodes.Add(treeNodeLayer);
                            }

                            // 現在の選択可能状態を取得
                            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
                            treeNodeLayer.Checked = sourceFeatureLayer.Selectable;

                            // チェックボックスアイコンを設定
                            SetCheckBoxImage(treeNodeLayer);

                            // フィーチャレイヤ情報を保存
                            SetSourceFeatureLayer(sourceFeatureLayer);
                    //    }
                    //}
                }
                // グループレイヤ
                else if (sourceLayer is GroupLayer)
                {
                    ICompositeLayer sourceCompositeLayer = (ICompositeLayer)sourceLayer;

                    // グループレイヤ下にレイヤが存在
                    if (sourceCompositeLayer.Count > 0)
                    {
                        TreeNode treeNodeLayer = new TreeNode(sourceLayer.Name);

                        // グループレイヤのアイコンを設定
                        treeNodeLayer.ImageIndex = 2;
                        treeNodeLayer.SelectedImageIndex = 2;

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
                else
                {
                }
            }
        }

        /// <summary>
        /// チェック状態を検索
        /// </summary>
        /// <param name="nodes">検索対象のノードコレクション</param>
        private void SearchSelectableLayer(TreeNodeCollection nodes)
        {
            foreach (TreeNode treeNodeLayer in nodes)
            {
                // 子ノードが存在しない場合
                if (treeNodeLayer.GetNodeCount(true) == 0)
                {
                    // チェック状態を保存
                    SetSelectableResult(treeNodeLayer.Checked);
                }
                else
                {
                    // チェック状態を再帰的に検索
                    SearchSelectableLayer(treeNodeLayer.Nodes);
                }
            }
        }

        /// <summary>
        /// フィーチャレイヤに選択可能情報を設定
        /// </summary>
        private void SetSelectableTargetLayer()
        {
            // チェック状態を検索
            SearchSelectableLayer(treeViewLayerList.Nodes);

            //if (sourceSelectableLayer.Count == targetSelectableLayer.Count)
            //{
                // フィーチャレイヤにチェック状態を設定
                for (int i = 0; i < sourceSelectableLayer.Count; i++)
                {
                    IFeatureLayer targetFeatureLayer = (IFeatureLayer)sourceSelectableLayer[i];
                    targetFeatureLayer.Selectable = (bool)targetSelectableLayer[i];
                }
            //}
        }

        /// <summary>
        /// フィーチャレイヤ情報を保存
        /// </summary>
        /// <param name="featureLayer">フィーチャレイヤ</param>
        private void SetSourceFeatureLayer(IFeatureLayer featureLayer)
        {
            sourceSelectableLayer[sourceIndex] = featureLayer;
            sourceIndex++;
        }

        /// <summary>
        /// チェック状態を保存
        /// </summary>
        /// <param name="isSelectable">チェック状態</param>
        private void SetSelectableResult(bool isSelectable)
        {
            targetSelectableLayer[targetIndex] = isSelectable;
            targetIndex++;
        }

        /// <summary>
        /// 選択可能レイヤの設定(OKボタン)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeViewLayerList.Nodes.Count > 0)
                {
                    // フィーチャレイヤに選択可能情報を設定
                    SetSelectableTargetLayer();
                }
            }
            catch (COMException comex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_SetLayerSettings);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_SetLayerSettings);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                this.Close();
            }
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
        /// Formクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSelectableLayerSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            // フィーチャレイヤ情報をクリア
            if (sourceSelectableLayer != null)
            {
                sourceSelectableLayer = null;
            }

            // チェック状態をクリア
            if (targetSelectableLayer != null)
            {
                targetSelectableLayer = null;
            }
        }

        /// <summary>
        /// TreeViewクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewLayerList_MouseDown(object sender, MouseEventArgs e)
        {
            TreeViewHitTestInfo hitTestInfo = treeViewLayerList.HitTest(e.Location);

            // クリックした場所がイメージ画像の場合
            if (hitTestInfo.Location == TreeViewHitTestLocations.Image)
            {
                // 最下位ノードのみ
                if (hitTestInfo.Node.GetNodeCount(true) == 0)
                {
                    // チェック状態を反転
                    hitTestInfo.Node.Checked = !hitTestInfo.Node.Checked;
                    
                    // チェックボックスイメージを設定
                    SetCheckBoxImage(hitTestInfo.Node);
                }
            }
        }

        /// <summary>
        /// チェックボックスイメージを設定
        /// </summary>
        /// <param name="treeNode">設定対象のノード</param>
        private void SetCheckBoxImage(TreeNode treeNode)
        {
            // チェック
            if (treeNode.Checked == true)
            {
                treeNode.ImageIndex = 1;
                treeNode.SelectedImageIndex = 1;
            }
            // 未チェック
            else
            {
                treeNode.ImageIndex = 0;
                treeNode.SelectedImageIndex = 0;
            }
        }

        /// <summary>
        /// 全選択ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                // チェックボックス全選択
                CheckOrUncheckAllCheckBox(treeViewLayerList.Nodes, true);
            }
            catch (COMException comex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_ChangeSelectAll);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_ChangeSelectAll);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// 全解除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectNone_Click(object sender, EventArgs e)
        {
            try
            {
                // チェックボックス全解除
                CheckOrUncheckAllCheckBox(treeViewLayerList.Nodes, false);
            }
            catch (COMException comex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_ChangeSelectNone);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSelectableLayerSettings_ERROR_ChangeSelectNone);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// チェックボックス全選択or全解除
        /// </summary>
        /// <param name="nodes">設定対象のノードコレクション</param>
        /// <param name="check">選択解除スイッチ</param>
        private void CheckOrUncheckAllCheckBox(TreeNodeCollection nodes, bool check)
        {
            foreach (TreeNode treeNodeLayer in nodes)
            {
                // 子ノードが存在しない場合
                if (treeNodeLayer.GetNodeCount(true) == 0)
                {
                    // チェック状態を設定
                    treeNodeLayer.Checked = check;
                    
                    // チェックボックスイメージを設定
                    SetCheckBoxImage(treeNodeLayer);
                }
                else
                {
                    // チェック状態を再帰的に設定
                    CheckOrUncheckAllCheckBox(treeNodeLayer.Nodes, check);
                }
            }
        }
    }
}
