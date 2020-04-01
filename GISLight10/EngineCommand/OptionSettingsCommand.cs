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
    /// �I�v�V�����ݒ�t�H�[���N���R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class OptionSettingsCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public OptionSettingsCommand()
        {
            base.captionName = "FormOptionSettings�̋N���R�}���h";
        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

        /// <summary>
        /// �N���b�N������
        /// �I�v�V�����ݒ�t�H�[����\��
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
