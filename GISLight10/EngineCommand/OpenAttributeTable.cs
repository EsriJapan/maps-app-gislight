using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// 属性テーブルを開くコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public sealed class OpenAttributeTable : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenAttributeTable()
        {
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.Magenta);

            base.m_caption = "属性テーブルを開く";
        }

        #region Overriden Class Methods

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();

                List<IFeatureLayer> featureLayerList =
                    pLayerManager.GetFeatureLayers(m_mapControl.Map);

                IFeatureLayer checkLayer = this.mainForm.SelectedLayer as IFeatureLayer;
                if (featureLayerList.Contains(checkLayer))
                // １１・１０時点：RasterCatalogの属性テーブル読み込みは対応できていない。
                //if ((checkLayer != null) && !(checkLayer is GdbRasterCatalogLayer) &&
                //    !(checkLayer is CadFeatureLayer) &&
                //     (checkLayer.Valid) &&
                //     (ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(checkLayer).Type !=
                //      ESRI.ArcGIS.Geodatabase.esriWorkspaceType.esriRemoteDatabaseWorkspace) &&
                //      featureLayerList.Count > 0)
                {
                    IEngineEditor engineEditor = new EngineEditorClass();

                    //編集中の場合
                    if (engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)
                    {
                        return false;
                    }

                    // 2012/08/07 ADD 
                    // ジオリファレンス実行中の場合
                    if (this.mainForm.HasGeoReference())
                    {
                        return false;
                    }
                    // 2012/08/07 ADD 

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// クリック時処理
        /// 属性テーブル表示フォームの表示
        /// </summary>
        public override void OnClick()
        {
            Ui.FormAttributeTable frmAttr =
                new Ui.FormAttributeTable(
                    this.mainForm.SelectedLayer, this.mainForm);

            frmAttr.Show(mainForm);

            mainForm.m_ToolbarControl2.Enabled = false;
        }
        #endregion
    }
}
