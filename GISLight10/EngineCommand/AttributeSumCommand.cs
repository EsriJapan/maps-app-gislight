using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// 属性値集計
    /// </summary>
    /// <history>
    ///  2010-11-01新規作成 
    /// </history>
    public sealed class AttributeSumCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AttributeSumCommand()
        {
            base.captionName = "属性値集計";

        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// メインフォームのマップコントロールへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

        /// <summary>
        /// クリック時処理
        /// メインフォームへの参照取得
        /// </summary>
        public override void OnClick()
        {
            Ui.FormAttributeSum frm = new Ui.FormAttributeSum(m_mapControl);
            
            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            this.mainFrm = (Ui.MainForm)cntrl2.FindForm();
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
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
