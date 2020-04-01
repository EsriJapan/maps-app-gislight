using System;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// 方位記号エレメントの作成
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public sealed class CreateLegend : BaseCommand
	{
		private IHookHelper m_HookHelper; 
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private Ui.MainForm mainForm;
 
        /// <summary>
        /// コンストラクタ
        /// </summary>
		public CreateLegend()
		{
			m_HookHelper = new HookHelperClass();

			//Set the tool properties
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            base.m_bitmap.MakeTransparent(Color.Magenta);
            
            base.m_caption = "凡例";
			base.m_category= "CustomCommands";
            base.m_toolTip = "凡例";
		}

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">ページレイアウトコントロール</param>
		public override void OnCreate(object hook)
		{
			m_HookHelper.Hook = hook;

            IToolbarControl2 tlb = (IToolbarControl2)m_HookHelper.Hook;
            IntPtr ptr = (System.IntPtr)tlb.hWnd;
            System.Windows.Forms.Control cntrl = System.Windows.Forms.Control.FromHandle(ptr);
            mainForm = (Ui.MainForm)cntrl.FindForm();
        }

        /// <summary>
        /// クリック時処理
        /// スタイルクラスに方位記号を指定して
        /// スタイルギャラリーフォームを表示する
        /// </summary>
        public override void OnClick()
        {
            m_pageLayoutControl = 
                mainForm.axPageLayoutControl1.Object as IPageLayoutControl3;
            
            IMapFrame mapFrame = 
                (IMapFrame)m_HookHelper.ActiveView.GraphicsContainer.FindFrame(
                m_HookHelper.ActiveView.FocusMap);

            // Add-->
            ESRIJapan.GISLight10.Common.Logger.Debug("On CreateLegend UsingMemorySize:" +
                GC.GetTotalMemory(false).ToString() + " byte");

            GC.Collect();

            ESRIJapan.GISLight10.Common.Logger.Debug("After Full GC.Collect UsingMemorySize:" +
                GC.GetTotalMemory(false).ToString() + " byte");
            //<--

            // 凡例作成ﾌｫｰﾑを表示
			Ui.FormLegendSettings2 frm = new Ui.FormLegendSettings2(m_pageLayoutControl, m_HookHelper.ActiveView);


            frm.ShowDialog(mainForm);

            mainForm.MainMapChanged = true;
        }

	}
}
