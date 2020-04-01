using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using System.Runtime.InteropServices;

using ESRIJapan.GISLight10.Ui;

namespace ESRIJapan.GISLight10.EngineCommand
{
    class SetDataFrameProjectionCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SetDataFrameProjectionCommand()
        {
            base.captionName = "座標系の設定";

        }

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_mapControl = (IMapControl3)hook;
                IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
                System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                mainFrm = (Ui.MainForm)cntrl2.FindForm();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// クリック時処理
        /// 
        /// </summary>
        public override void OnClick()
        {
            GISLight10.Common.Logger.Info("座標系の設定を実行");

            FormDataFrameProjection frm = new FormDataFrameProjection(this.m_mapControl);
            frm.ShowDialog(this.mainFrm);

        }


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

                if (featureLayerList.Count > 0)
                {
                    if (mainFrm.HasFormAttributeTable())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
