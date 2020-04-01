using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using System.Collections;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// リレートの解除
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormRemoveRelate : Form
    {
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormRemoveRelate(ESRI.ArcGIS.Controls.IMapControl3 mapControl)
        {
            InitializeComponent();

            this.m_mapControl = mapControl;

            Common.Logger.Info("リレートの解除フォームの起動");

            List<IFeatureLayer> fcLayerList = null;

            try
            {
                //リレートを持っているフィーチャレイヤ一覧の取得
                ESRIJapan.GISLight10.Common.LayerManager layerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();
                fcLayerList = layerManager.GetHasRelateLayers(m_mapControl.Map);

                //フィーチャレイヤ一覧をコンボボックスに追加
                if (fcLayerList.Count > 0)
                {
                    AddComboItemLayer(comboSourceLayers, fcLayerList);
                }
                else
                {
                    Common.Logger.Info("リレートを解除するレイヤが存在しない");

                    //キャンセルボタン以外は操作できないように設定
                    comboSourceLayers.Enabled = false;
                    comboRealateNames.Enabled = false;
                    checkBoxAllRemove.Enabled = false;
                    buttonOK.Enabled = false;

                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                    (this, Properties.Resources.FormRemoveRelate_WARNING_NotHasRelate);
                    Common.Logger.Warn(Properties.Resources.FormRemoveRelate_WARNING_NotHasRelate);

                    this.Close();
                    //this.Dispose();
                }
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveRelate_ERROR_GetSrcLayers);
                Common.Logger.Error(Properties.Resources.FormRemoveRelate_ERROR_GetSrcLayers);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);

                this.Close();
				//this.Dispose();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveRelate_ERROR_GetSrcLayers);
                Common.Logger.Error(Properties.Resources.FormRemoveRelate_ERROR_GetSrcLayers);
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
       

        /// <summary>
        /// リレート元レイヤ コンボボックス 選択変更時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboSourceLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList relClassComboItemList = null;

            try
            {
                //コンボボックスの選択アイテムの取得
                ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
                    (ESRIJapan.GISLight10.Common.LayerComboItem)comboSourceLayers.SelectedItem;

                //リレート情報一覧の取得                
                relClassComboItemList =
                    ESRIJapan.GISLight10.Common.RemoveRelateFunctions.GetRelClassItemList
                    ((IFeatureLayer)selectedLayerItem.Layer);

                //コンボボックスに追加
                AddComboItemLayer(comboRealateNames, relClassComboItemList);
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveRelate_ERROR_GetRelateNames);
                Common.Logger.Error(Properties.Resources.FormRemoveRelate_ERROR_GetRelateNames);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveRelate_ERROR_GetRelateNames);
                Common.Logger.Error(Properties.Resources.FormRemoveRelate_ERROR_GetRelateNames);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(relClassComboItemList);
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
            IRelationshipClass relationshipClass = null;

            try
            {
                //入力チェック（パラメータが入力されているか確認）                
                if (comboSourceLayers.SelectedIndex == -1)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRemoveRelate_WARNING_NoSrcLayer);
                    Common.Logger.Warn(Properties.Resources.FormRemoveRelate_WARNING_NoSrcLayer);
                    return;
                }

                if (comboRealateNames.SelectedIndex == -1)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRemoveRelate_WARNING_NoRelateName);
                    Common.Logger.Warn(Properties.Resources.FormRemoveRelate_WARNING_NoRelateName);
                    return;
                }                


                //パラメータの取得
                //リレート元レイヤの取得
                ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
                    (ESRIJapan.GISLight10.Common.LayerComboItem)comboSourceLayers.SelectedItem;
                sourceFcLayer = (IFeatureLayer)selectedLayerItem.Layer;


                //リレート名（リレート情報）の取得
                ESRIJapan.GISLight10.Common.RelClassComboItem relClassComboItem =
                    (ESRIJapan.GISLight10.Common.RelClassComboItem)comboRealateNames.SelectedItem;
                relationshipClass = relClassComboItem.RelationshipClass;
                


                //リレート解除の実行
                if (checkBoxAllRemove.Checked == true)
                {
                    Common.Logger.Info("すべてのリレートの解除を実行");

                    //すべてのリレートの解除
                    ESRIJapan.GISLight10.Common.RemoveRelateFunctions.RemoveAllRelate(sourceFcLayer);
                }
                else
                {
                    Common.Logger.Info("指定したリレートの解除を実行");

                    //指定したリレートの解除
                    ESRIJapan.GISLight10.Common.RemoveRelateFunctions.RemoveRelate
                        (sourceFcLayer, relationshipClass);
                }

                this.Close();
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveRelate_ERROR_DoRemoveRelate);
                Common.Logger.Error(Properties.Resources.FormRemoveRelate_ERROR_DoRemoveRelate);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveRelate_ERROR_DoRemoveRelate);
                Common.Logger.Error(Properties.Resources.FormRemoveRelate_ERROR_DoRemoveRelate);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                //ComReleaser.ReleaseCOMObject(sourceFcLayer);
                //ComReleaser.ReleaseCOMObject(relationshipClass);                
            }            
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
        private void FormRemoveRelate_FormClosed(object sender, FormClosedEventArgs e)
        {
            Common.Logger.Info("リレートの解除フォームの終了");
        }


 
        /// <summary>
        /// コンボボックスにレイヤ名を追加
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        /// <param name="fcLayerList">レイヤ一覧（List）</param>
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
        /// コンボボックスにレイヤ名を追加
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        /// <param name="fcLayerList">レイヤ一覧（ArrayList）</param>
        private void AddComboItemLayer(ComboBox combobox, ArrayList fcLayerList)
        {
            comboRealateNames.Items.Clear();

            for (int i = 0; i < fcLayerList.Count; i++)
            {
                combobox.Items.Add(fcLayerList[i]);
            }

            //先頭のレイヤを選択
            if (combobox.Items.Count > 0)
            {
                combobox.SelectedIndex = 0;
            }
        }


        

    }
}
