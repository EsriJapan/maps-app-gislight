using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// 単独テーブルの一覧を開くコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class OpenStandaloneTableList : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenStandaloneTableList()
        {

            base.m_caption = "テーブルの一覧を表示";
        }

        #region Overriden Class Methods

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
				// 単独ﾃｰﾌﾞﾙの有無を判定
				IStandaloneTableCollection	agStdTbls = this.mainForm.MapControl.Map as IStandaloneTableCollection;
				return agStdTbls.StandaloneTableCount > 0;
            }
        }
        
        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook) {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// クリック時処理
        /// テーブル一覧フォームを表示
        /// </summary>
        public override void OnClick() {
			// ﾃｰﾌﾞﾙの一覧を開く
            new Ui.FormTableView(this.mainForm.MapControl).ShowDialog();
        }
        #endregion
    }
}
