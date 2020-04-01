using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// 凡例の編集
    /// </summary>
    public sealed class EditLegendCommand : Common.EjBaseCommand
    {
		private IHookHelper m_HookHelper; 
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private Ui.MainForm mainForm;
        private IMapSurroundFrame	_agLeg;		// 編集対象の凡例
 
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditLegendCommand()
		{
			m_HookHelper = new HookHelperClass();

            base.captionName = "凡例の編集";
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
        /// 凡例の設定フォームを表示する
        /// </summary>
        public override void OnClick() {
            m_pageLayoutControl = mainForm.axPageLayoutControl1.Object as IPageLayoutControl3;
            
            // 凡例編集ﾌｫｰﾑを起動
            Ui.FormLegendSettings frm = new Ui.FormLegendSettings(m_pageLayoutControl, m_HookHelper.ActiveView, _agLeg);
            frm.ShowDialog(mainForm);

            mainForm.MainMapChanged = true;
        }

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
                try {
					// 選択済みのｸﾞﾗﾌｨｯｸを取得
                    IPageLayout pl = mainForm.axPageLayoutControl1.PageLayout;
                    IGraphicsContainerSelect graphicsSelect = pl as IGraphicsContainerSelect;

                    // 選択されているエレメントが1つである場合を対象とします。
                    // 編集対象の凡例が複数選択されていても、
                    // 一度には1つのものしか処理対象に出来ない為。
                    if(graphicsSelect.ElementSelectionCount == 1) {
                        IEnumElement ienum = graphicsSelect.SelectedElements;
                        ienum.Reset();
                        IElement selectedElement = ienum.Next();

                        if(selectedElement is IMapSurroundFrame) {
							IMapSurroundFrame selectMapSurroundFrame = selectedElement as IMapSurroundFrame;
							IMapSurround mapSurround = selectMapSurroundFrame.MapSurround;

							// 選択ｱｲﾃﾑが凡例かどうか判定
							if(mapSurround is ILegend) {
								// 対象の凡例を保存
								this._agLeg = selectMapSurroundFrame;
								blnRet = true;
							}
                        }
                    }
                }
				catch(Exception ex) {
#if DEBUG
					Debug.WriteLine("●判例編集(Enabledﾌﾟﾛﾊﾟﾃｨ) ERROR : " + ex.Message);
#endif			
				}
                
                // 前回の対象凡例を解放
                if(!blnRet && this._agLeg != null) {
					this._agLeg = null;
                }
                
                return blnRet;
            }
        }
    }
}
