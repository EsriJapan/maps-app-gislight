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
    /// �㏑���ۑ��R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬
    /// </history>
    class SaveDocument : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public SaveDocument()
        {
            base.m_caption = "�㏑���ۑ�";
            base.m_toolTip = "�㏑���ۑ�";

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
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�c�[���o�[�R���g���[��</param>
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
        /// �N���b�N������
        /// �h�L�������g�̏㏑�����s
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
                        ESRIJapan.GISLight10.Common.Logger.Info("���̃h�L�������g�͓Ǎ��ݐ�p�ł�");
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxInfo(
                            mainForm, "���̃h�L�������g�͓Ǎ��ݐ�p�ł�");
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
        /// �㏑���ۑ��R�}���h�L����
        /// </summary>
        public void EnableCommand()
        {
            this.m_enabled = true;
        }

        /// <summary>
        /// �㏑���ۑ��R�}���h������
        /// </summary>
        public void DisableCommand()
        {
            this.m_enabled = false;
        }

        #endregion
    }
}