using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// フィールド演算のコマンドクラス
    /// </summary>
    class GeoReferenceCommand: BaseCommand
    {

        /// <summary>
        /// マップコントロール
        /// </summary>
        protected IMapControl3 m_mapControl;

        /// <summary>
        /// メインフォーム
        /// </summary>
        protected Ui.MainForm mainFrm;

        private IHookHelper m_hookHelper = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GeoReferenceCommand() {
			// ﾌﾟﾛﾊﾟﾃｨ設定
            //string	bitmapResourceName = GetType().Name + ".bmp";
            //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            //m_bitmap.MakeTransparent(Color.Magenta);

            base.m_caption = "ジオリファレンス";
            base.m_toolTip = "ジオリファレンス";


            try
            {
                string bitmapResourceName = this.GetType().Name + ".bmp";
                base.m_bitmap =
                    new Bitmap(this.GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }

        }

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook) {
            if(m_hookHelper == null)
                m_hookHelper = new HookHelperClass();
            m_hookHelper.Hook = hook;

			// ※ 本ｲﾍﾞﾝﾄでMapControlへの参照が取得できなくなった (V10.2.0～)
        }

        /// <summary>
        /// OnClick (ﾒﾆｭｰ、ﾎﾞﾀﾝの両方からCall)
        /// </summary>
        public override void OnClick() {
			// ※ ｸﾘｯｸできるということは、既にﾒｲﾝﾌｫｰﾑ等は取得できている
			if(this.m_hookHelper == null) {
				return;
			}

			// ｼﾞｵﾘﾌｧﾚﾝｽ可能ﾗｽﾀｰを取得
            LayerManager		clsLM = new LayerManager();
            List<IRasterLayer>	agRasterLayers = clsLM.GetRasterLayers(this.mainFrm.MapControl.Map);
            IGeoReference		agGeoRef;
            int					intCnt = 0;
            
            foreach(IRasterLayer agRLayer in agRasterLayers) {
				// ｼﾞｵﾘﾌｧﾚﾝｽ可能なﾗｽﾀｰを判別
				agGeoRef = (IGeoReference)agRLayer;
				if(agGeoRef.CanGeoRef) {
					++intCnt;
				}
            }
			
            // ｼﾞｵﾘﾌｧﾚﾝｽ可能ﾗｽﾀｰあり
            if(intCnt > 0) {
				// ﾌｫｰﾑ起動
				Ui.FormGeoReference frm = new Ui.FormGeoReference(mainFrm);
				frm.Show(mainFrm);
            }
            else {
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxInfo(mainFrm, "ジオリファレンス可能なラスターデータがありません。");
            }
        }

        /// <summary>
        /// Enabled
        /// </summary>
        public override bool Enabled {
            get {
                bool returnVal = false;

                if(this.mainFrm == null && this.m_hookHelper != null) {
					if(m_hookHelper.Hook is IMapControl3) {
						this.m_mapControl = (IMapControl3)m_hookHelper.Hook;
					}
					else if(m_hookHelper.Hook is IToolbarControl2) {
						IToolbarControl2	agToolCtl = m_hookHelper.Hook as IToolbarControl2;
						this.m_mapControl = agToolCtl.Buddy as IMapControl3;
					}
#if DEBUG						
					else {
						if(m_hookHelper.Hook is IPageLayoutControl3) {
							IPageLayoutControl3 pageLayoutControl = m_hookHelper.Hook as IPageLayoutControl3;
						}
						else if(m_hookHelper.Hook is ISceneControl) {
							ISceneControl sceneControl = m_hookHelper.Hook as ISceneControl;
						}
						else if(m_hookHelper.Hook is IGlobeControl) {
							IGlobeControl globeControl = m_hookHelper.Hook as IGlobeControl;
						}
					}
#endif
                    
					// ﾒｲﾝ･ﾌｫｰﾑを取得
					if(this.m_mapControl != null) {
						System.Windows.Forms.Control ctlTemp = System.Windows.Forms.Control.FromHandle((IntPtr)this.m_mapControl.hWnd);
						this.mainFrm = (Ui.MainForm)ctlTemp.FindForm();
					}
                }

				if(this.mainFrm != null) {
					// 既に実行中の場合 / 属性テーブルを参照する場合 / 編集中の場合
					if(!(this.mainFrm.IsEditMode || this.mainFrm.HasFormAttributeTable() || this.mainFrm.HasGeoReference() || !this.mainFrm.IsMapVisible)) {
						// ﾚｲﾔｰ･ﾏﾈｰｼﾞｬｰを取得
						LayerManager clsLM = new LayerManager();

						// 有効なﾗｽﾀｰ･ﾚｲﾔｰがあればOK
						returnVal = clsLM.GetRasterLayers(m_mapControl.Map).Count > 0;
					}
				}
                    
                return returnVal;
            }
        }
    }
}
