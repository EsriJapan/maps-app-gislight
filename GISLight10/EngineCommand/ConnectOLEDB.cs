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
    public sealed class ConnectOLEDB : Common.EjBaseCommand
    {
        private IMapControl3	m_mapControl;
        private Ui.MainForm		mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConnectOLEDB() {
            base.captionName = "OLEDB接続";

            try {
                //string bitmapResourceName = GetType().Name + ".bmp";
                //base.buttonImage = new System.Drawing.Bitmap(GetType(), bitmapResourceName);
            }
            catch(Exception ex) {
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
        public override void OnClick() {
            Ui.FormOLEDBConnect frm = new Ui.FormOLEDBConnect() {
				MapControl = m_mapControl,
            };
            frm.ShowDialog(mainFrm);
        }

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
                return true;
            }
        }
        #endregion
    }
}
