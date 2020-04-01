using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// 上書き保存コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    class SaveDocument : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SaveDocument()
        {
            base.m_caption = "上書き保存";
            base.m_toolTip = "上書き保存";

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

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">ツールバーコントロール</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            IToolbarControl2 tlb = (IToolbarControl2)m_hookHelper.Hook;
            IntPtr ptr = (System.IntPtr)tlb.hWnd;
            System.Windows.Forms.Control cntrl = System.Windows.Forms.Control.FromHandle(ptr);
            mainForm = (Ui.MainForm)cntrl.FindForm();

            DisableCommand();
        }

        /// <summary>
        /// クリック時処理
        /// ドキュメントの上書き実行
        /// </summary>
        public override void OnClick()
        {
            IMapDocument mapDoc = new MapDocumentClass();
            try
            {
                if (mainForm.axMapControl1.CheckMxFile(mainForm.axMapControl1.DocumentFilename))
                {
                    mapDoc.Open(mainForm.axMapControl1.DocumentFilename, string.Empty);
                    if (mapDoc.get_IsReadOnly(mainForm.axMapControl1.DocumentFilename))
                    {
                        mapDoc.Close();
                        ESRIJapan.GISLight10.Common.Logger.Info("このドキュメントは読込み専用です");
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxInfo(
                            mainForm, "このドキュメントは読込み専用です");
                        return;
                    }

                    mainForm.axPageLayoutControl1.ActiveView.ShowScrollBars = true;
                    mainForm.axMapControl1.ActiveView.ShowScrollBars = true;

                    mapDoc.ReplaceContents((IMxdContents)mainForm.axPageLayoutControl1.PageLayout);

                    // 2010-12-27
                    mapDoc.SetActiveView(mainForm.axPageLayoutControl1.ActiveView);
                    mapDoc.SetActiveView(mainForm.axMapControl1.ActiveView);

                    mapDoc.Save(true, false);

                    mapDoc.Close();
                    mainForm.ClearMapChangeCount();
                }
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    mainForm, Properties.Resources.MainForm_ErrorWhenMenuSave);

                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    mainForm, Properties.Resources.MainForm_ErrorWhenMenuSave);

                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
            }
        }

        /// <summary>
        /// 上書き保存コマンド有効化
        /// </summary>
        public void EnableCommand()
        {
            this.m_enabled = true;
        }

        /// <summary>
        /// 上書き保存コマンド無効化
        /// </summary>
        public void DisableCommand()
        {
            this.m_enabled = false;
        }

        #endregion
    }
}