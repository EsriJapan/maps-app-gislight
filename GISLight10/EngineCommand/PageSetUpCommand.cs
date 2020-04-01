using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ����p�y�[�W�ݒ�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class PageSetUpCommand : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public PageSetUpCommand()
        {
            
            base.m_caption = "�ݒ�";
            base.m_category = "�y�[�W���C�A�E�g";
            base.m_toolTip = "����ݒ�";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
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
        }

        /// <summary>
        /// �N���b�N������
        /// ����p�y�[�W�ݒ�
        /// </summary>
        public override void OnClick() {
			// ����ݒ�����s
			PrinitPageLayoutCommand.Print(mainForm, false);
        }
        #endregion
    }
}
