using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF.BaseClasses;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// XYテキストデータからポイントを作成するコマンドクラス
    /// </summary>
    class AddXYDataCommand: BaseCommand //Common.EjBaseCommand//
    {

        /// <summary>
        /// マップコントロール
        /// </summary>
        protected IMapControl3 m_mapControl;

        /// <summary>
        /// メインフォーム
        /// </summary>
        protected Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AddXYDataCommand()
        {

            base.m_caption = "XYデータの追加";
            base.m_toolTip = "XYデータの追加";
        }

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook"></param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// OnClick
        /// </summary>
        public override void OnClick()
        {
            Ui.FormAddXYData frm = new Ui.FormAddXYData(m_mapControl, mainFrm);
            frm.ShowDialog(mainFrm);

        }

        /// <summary>
        /// Enabled
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
        }

    }
}
