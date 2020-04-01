using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRIJapan.GISLight10.Common;


namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// テーブル結合コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class JoinTableCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        //private List<IFeatureLayer> layerList = null;
        //private LayerManager layerManager = new ESRIJapan.GISLight10.Common.LayerManager();
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JoinTableCommand()
        {
            base.captionName = "テーブル結合";


            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new System.Drawing.Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }

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
        /// テーブル結合フォームの表示
        /// </summary>
        public override void OnClick()
        {
            Ui.FormJoinTable frm = new Ui.FormJoinTable(m_mapControl);

            //IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            //System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            //mainFrm = (Ui.MainForm)cntrl2.FindForm();
            frm.ShowDialog(mainFrm);
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
        #endregion
    }
}
