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
    class ShowTOCCommand: BaseCommand
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
        public ShowTOCCommand() {
			// ﾌﾟﾛﾊﾟﾃｨ設定
            base.m_caption = "グループレイヤの構成を変更";
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

            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// OnClick (ﾒﾆｭｰ、ﾎﾞﾀﾝの両方からCall)
        /// </summary>
        public override void OnClick() {
			// ﾌｫｰﾑ起動
			Ui.FormCompositeInsert frm = new Ui.FormCompositeInsert(this.mainFrm.SelectedLayer, mainFrm);
			frm.ShowDialog(mainFrm);
        }

        /// <summary>
        /// Enabled
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
				if(this.mainFrm.SelectedLayer is ICompositeLayer) {
					blnRet = true;
				}
                
                return blnRet;
            }
        }
    }
}
