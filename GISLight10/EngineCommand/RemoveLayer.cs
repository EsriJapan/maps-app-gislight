using System;
using System.Drawing;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// レイヤの削除コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class RemoveLayer : Common.EjBaseCommand  
	{
		private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public RemoveLayer()
		{
			base.captionName = "レイヤの削除";
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }

		}
	
        /// <summary>
        /// クリック時処理
        /// レイヤ削除実行
        /// </summary>
		public override void OnClick()
		{
			ILayer layer =  (ILayer) m_mapControl.CustomProperty;
			m_mapControl.Map.DeleteLayer(layer);
		}
	
        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
		}

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                ILayer layer = (ILayer)m_mapControl.CustomProperty;
                //ILayer2 layer2 = this.mainFrm.SelectedLayer;

                if (layer is IMapServerLayer)
                {
                    return true;
                }

                //if (layer2 == null)
                //{
                //    return false;
                //}
                //else
                //{
                    if (mainFrm.HasFormAttributeTable())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                //}
            }
        }

	}
}


