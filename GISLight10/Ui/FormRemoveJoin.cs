using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// テーブル結合の解除
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成 
    ///  2011/06/07 レンダラの名称、フィールド別名保持対応
    /// </history>
    public partial class FormRemoveJoin : Form
    {
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormRemoveJoin(ESRI.ArcGIS.Controls.IMapControl3 mapControl)
        {            
            InitializeComponent();

            this.m_mapControl = mapControl;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();

            Common.Logger.Info("テーブル結合の解除フォームの起動");

            List<IFeatureLayer> fcLayerList = null;

            try
            {
                //テーブル結合を持っているフィーチャレイヤ一覧の取得
                ESRIJapan.GISLight10.Common.LayerManager layerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();
                fcLayerList = layerManager.GetHasJoinLayers(m_mapControl.Map);

                //フィーチャレイヤ一覧をコンボボックスに追加
                if (fcLayerList.Count > 0)
                {
                    AddComboItemLayer(comboSourceLayers, fcLayerList);
                }
                else
                {
                    Common.Logger.Info("テーブル結合を解除するレイヤが存在しない");

                    //キャンセルボタン以外は操作できないように設定
                    comboSourceLayers.Enabled = false;
                    comboDestinationTables.Enabled = false;
                    checkBoxAllRemove.Enabled = false;
                    buttonOK.Enabled = false;

                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                    (this, Properties.Resources.FormRemoveJoin_WARNING_NotHasJoin);
                    Common.Logger.Warn(Properties.Resources.FormRemoveJoin_WARNING_NotHasJoin);

                    this.Close();
                    //this.Dispose();
                }
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveJoin_ERROR_GetSrcLayers);
                Common.Logger.Error(Properties.Resources.FormRemoveJoin_ERROR_GetSrcLayers);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);

                this.Close();
				//this.Dispose();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveJoin_ERROR_GetSrcLayers);
                Common.Logger.Error(Properties.Resources.FormRemoveJoin_ERROR_GetSrcLayers);
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
        /// 結合元レイヤ コンボボックス 選択変更時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboSourceLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList relQueryTableComboItemList = null;

            try
            {
                //コンボボックスの選択アイテムの取得
                ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
                    (ESRIJapan.GISLight10.Common.LayerComboItem)comboSourceLayers.SelectedItem;

                //結合先テーブル一覧の取得                
                relQueryTableComboItemList =
                    ESRIJapan.GISLight10.Common.RemoveJoinFunctions.GetRelQueryTableComboItemList
                    ((IFeatureLayer)selectedLayerItem.Layer);

                //コンボボックスに追加
                AddComboItemLayer(comboDestinationTables, relQueryTableComboItemList);
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveJoin_ERROR_GetDestTables);
                Common.Logger.Error(Properties.Resources.FormRemoveJoin_ERROR_GetDestTables);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveJoin_ERROR_GetDestTables);
                Common.Logger.Error(Properties.Resources.FormRemoveJoin_ERROR_GetDestTables);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(relQueryTableComboItemList);
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
            IRelQueryTable relQueryTable = null;

            try
            {
                //入力チェック（パラメータが入力されているか確認）                
                if (comboSourceLayers.SelectedIndex == -1)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRemoveJoin_WARNING_NoSrcLayer);
                    Common.Logger.Warn(Properties.Resources.FormRemoveJoin_WARNING_NoSrcLayer);
                    return;
                }

                if (comboDestinationTables.SelectedIndex == -1)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormRemoveJoin_WARNING_NoDestTable);
                    Common.Logger.Warn(Properties.Resources.FormRemoveJoin_WARNING_NoDestTable);
                    return;
                }
                


                
                //結合元レイヤの取得
                ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
                    (ESRIJapan.GISLight10.Common.LayerComboItem)comboSourceLayers.SelectedItem;
                sourceFcLayer = (IFeatureLayer)selectedLayerItem.Layer;


                //結合先テーブル（結合情報）の取得
                ESRIJapan.GISLight10.Common.RelQueryTableComboItem relClassComboItem =
                    (ESRIJapan.GISLight10.Common.RelQueryTableComboItem)comboDestinationTables.SelectedItem;
                relQueryTable = relClassComboItem.RelQueryTable;
                
                // 2011/06/07 -->
                Hashtable fldAliases = null;
                fldAliases = new Hashtable();
                
                IFeatureLayer2 featLayer = (IFeatureLayer2)selectedLayerItem.Layer;
                IDisplayTable dispTbl = (IDisplayTable)featLayer;
                ITableFields tblFlds = (ITableFields)featLayer;

                for (int i = 0; i < dispTbl.DisplayTable.Fields.FieldCount; i++)
                {
                    IField field = dispTbl.DisplayTable.Fields.get_Field(i);
                    IFieldInfo fldinfo = tblFlds.get_FieldInfo(i);

                    //fldAliases.Add(field.Name, fldinfo.Alias);
                    fldAliases.Add(i, fldinfo.Alias);
                }
                //<--


                //テーブル結合解除の実行
                if (checkBoxAllRemove.Checked == true)
                {
                    Common.Logger.Info("すべてのテーブル結合の解除を実行");

                    //すべてのテーブル結合の解除
                    ESRIJapan.GISLight10.Common.RemoveJoinFunctions.RemoveAllJoin(sourceFcLayer);


                    // 2011/06/03 -->
                    // シンボル設定状態確認
                    EditRendererWhenRemoveJoin(false, sourceFcLayer);

                    Common.UtilityClass.RestoreEditableAlias(sourceFcLayer, fldAliases);
                    //<--

                }
                else
                {
                    Common.Logger.Info("指定したテーブル結合の解除を実行");

                    int index = comboDestinationTables.SelectedIndex;

                    if (index != 0)
                    {
                        DialogResult result =
                            ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxQuestion2
                            (this, Properties.Resources.FormRemoveJoin_QUESTION_RemoveOKorNO);

                        //解除の中止
                        if (result == DialogResult.Cancel)
                        {
                            Common.Logger.Info("解除の中止");

                            //ComReleaser.ReleaseCOMObject(sourceFcLayer);
                            //ComReleaser.ReleaseCOMObject(relQueryTable);

                            return;
                        }
                    }


                    //指定したテーブル結合の解除
                    ESRIJapan.GISLight10.Common.RemoveJoinFunctions.RemoveJoin(sourceFcLayer, relQueryTable);

                    // 2011/06/03 -->
                    ICollection keyColl2 = fldAliases.Keys;

                    for (int i = 0; i < dispTbl.DisplayTable.Fields.FieldCount - 1; i++)
                    {
                        IField field = dispTbl.DisplayTable.Fields.get_Field(i);
                        IFieldInfo fldinfo = tblFlds.get_FieldInfo(i);
                    }

                    IDisplayTable displayTable = (IDisplayTable)sourceFcLayer;
                    ITable table = displayTable.DisplayTable;
                    IRelQueryTable relQryTbl = table as IRelQueryTable;
                    bool isHashJoin = (relQryTbl != null);

                    //　複数結合時でまだ結合有りなら下記は行わない
                    EditRendererWhenRemoveJoin(isHashJoin, sourceFcLayer);

                    Common.UtilityClass.RestoreEditableAlias(sourceFcLayer, fldAliases);
                    //<--

                }

                m_mapControl.ActiveView.Refresh();
                mainFrm.axTOCControl1.Refresh();
                //m_mapControl.ActiveView.ContentsChanged();

                this.Close();
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveJoin_ERROR_DoRemoveJoin);
                Common.Logger.Error(Properties.Resources.FormRemoveJoin_ERROR_DoRemoveJoin);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormRemoveJoin_ERROR_DoRemoveJoin);
                Common.Logger.Error(Properties.Resources.FormRemoveJoin_ERROR_DoRemoveJoin);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally 
            {
                //ComReleaser.ReleaseCOMObject(sourceFcLayer);
                //ComReleaser.ReleaseCOMObject(relQueryTable);                
            }            
        }

        /// <summary>
        /// レンダラのフィールド名編集
        /// </summary>
        /// <param name="isHashJoin"></param>
        /// <param name="sourceFcLayer"></param>
        private void EditRendererWhenRemoveJoin(bool isHashJoin, IFeatureLayer sourceFcLayer)
        {
            if (!isHashJoin)
            {
                // シンボル設定状態確認
                IGeoFeatureLayer gflayer = sourceFcLayer as IGeoFeatureLayer;
                IFeatureRenderer frenderer = gflayer.Renderer;
                if (frenderer is IUniqueValueRenderer)
                {
                    IUniqueValueRenderer uniqueRenderer = (IUniqueValueRenderer)frenderer;
                    string fldname = uniqueRenderer.get_Field(0);
                    string[] spltname = fldname.Split('.');

                    if (spltname.Length >= 1) // 結合先がCSV時(Lenght==1),それ以外時にはLenght==2
                    {
                        if (spltname.Length >= 2 && sourceFcLayer.Name.Equals(spltname[0]))
                        {
                            uniqueRenderer.set_Field(0, spltname[spltname.Length - 1]);

                            ILegendInfo legendInfo = uniqueRenderer as ILegendInfo;
                            if (legendInfo != null && legendInfo.LegendGroupCount > 0)
                            {
                                ILegendGroup legendGroup =
                                    legendInfo.get_LegendGroup(legendInfo.LegendGroupCount - 1);

                                if (legendGroup.Editable && legendGroup.Heading != null)
                                {
                                    legendGroup.Heading = uniqueRenderer.get_Field(0);
                                }
                            }
                        }
                        else
                        {
                            ISimpleRenderer simpleRenderer = new SimpleRendererClass();
                            simpleRenderer.Symbol = uniqueRenderer.DefaultSymbol;
                            gflayer.Renderer = (IFeatureRenderer)simpleRenderer;

                        }
                        this.mainFrm.axMapControl1.ActiveView.ContentsChanged();
                    }

                }
                // 解除する場合には数値分類も行なう
                else if (frenderer is IClassBreaksRenderer)
                {
                    IClassBreaksRenderer clsbrksRenderer = (IClassBreaksRenderer)frenderer;
                    clsbrksRenderer.Field = sourceFcLayer.Name + "." + clsbrksRenderer.Field;

                    string[] spltname = clsbrksRenderer.Field.Split('.');
                    if (spltname.Length >= 2) // 結合先がCSV時(Lenght==2),それ以外時にはLenght==3
                    {
                        // 結合元にシンボル設定されている場合には0番目と1番目の内容のテーブル名は一致する
                        if (spltname.Length >= 3 &&
                            sourceFcLayer.Name.Equals(spltname[0]) &&
                            sourceFcLayer.Name.Equals(spltname[1]))
                        {
                            clsbrksRenderer.Field = spltname[spltname.Length - 1];

                            // 数値分類の場合、このタイミングで既にテーブル名部分は取り除かれている
                            //ILegendInfo legendInfo = clsbrksRenderer as ILegendInfo;
                            //ILegendGroup legendGroup =
                            //    legendInfo.get_LegendGroup(legendInfo.LegendGroupCount - 1);

                            //if (legendGroup.Editable && legendGroup.Heading != null)
                            //{
                            //    legendGroup.Heading = clsbrksRenderer.Field;
                            //    this.mainFrm.axMapControl1.ActiveView.ContentsChanged();
                            //}
                        }
                        else
                        {
                            ISimpleRenderer simpleRenderer = new SimpleRendererClass();
                            simpleRenderer.Symbol = clsbrksRenderer.get_Symbol(0);
                            gflayer.Renderer = (IFeatureRenderer)simpleRenderer;

                        }
                    }
                    this.mainFrm.axMapControl1.ActiveView.ContentsChanged();
                }

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
        private void FormRemoveJoin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Common.Logger.Info("テーブル結合の解除フォームの終了");
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
            comboDestinationTables.Items.Clear();

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
