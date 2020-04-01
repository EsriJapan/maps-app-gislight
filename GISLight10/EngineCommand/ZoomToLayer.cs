using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// レイヤの全体表示コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class ZoomToLayer : Common.EjBaseCommand
    {

        private IMapControl4 m_MapControl = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ZoomToLayer()
        {
            base.captionName = "レイヤの全体表示";


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

        /// <summary>
        /// クリック時処理
        /// レイヤの全体表示
        /// </summary>
        public override void OnClick()
        {
            if (m_MapControl == null)
                return;
            ILayer layer = (ILayer)m_MapControl.CustomProperty;
            if (layer == null)
                return;

            // ﾚｲﾔｰ範囲を取得
            IEnvelope	agEnv = layer.AreaOfInterest;
            // 更に範囲を少し拡張する
            agEnv = ESRIJapan.GISLight10.Common.UtilityClass.ExpandEnvelope(agEnv);
            
            m_MapControl.Extent = agEnv;
        }

        /// <summary>
        /// クリエイト時処理
        /// マップコントロールへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_MapControl = (IMapControl4)hook ;
        }
    }
}
