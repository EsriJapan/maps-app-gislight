using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// オプション設定フォーム起動コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class OptionSettingsCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OptionSettingsCommand()
        {
            base.captionName = "FormOptionSettingsの起動コマンド";
        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

        /// <summary>
        /// クリック時処理
        /// オプション設定フォームを表示
        /// </summary>
        public override void OnClick()
        {
            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();

            Ui.FormOptionSettings frm = new Ui.FormOptionSettings();
            frm.ShowDialog(mainFrm);
        }
        #endregion
    }
}
