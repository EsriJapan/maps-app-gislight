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
    /// ラベル表示コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public sealed class LabelSettingCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

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

                IFeatureLayer checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                if (featureLayerList.Contains(checkLayer))
                //if ((checkLayer != null) && !(checkLayer is GdbRasterCatalogLayer) &&
                //    !(checkLayer is CadFeatureLayer) &&
                //     (checkLayer.Valid) &&
                //     (ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(checkLayer).Type !=
                //      ESRI.ArcGIS.Geodatabase.esriWorkspaceType.esriRemoteDatabaseWorkspace) &&
                //      featureLayerList.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LabelSettingCommand()
        {
            base.captionName = "ラベル設定";

        }

        #region Overriden Class Methods

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
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// クリック時処理
        /// ラベル表示フォームの表示
        /// </summary>
        public override void OnClick()
        {
            Ui.FormLabelSetting frm = new Ui.FormLabelSetting(mainFrm, m_mapControl);
            frm.ShowDialog(this.mainFrm);
        }
        #endregion
    }
}
