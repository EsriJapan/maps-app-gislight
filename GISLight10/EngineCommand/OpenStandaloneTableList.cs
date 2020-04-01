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
    /// �P�ƃe�[�u���̈ꗗ���J���R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class OpenStandaloneTableList : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public OpenStandaloneTableList()
        {

            base.m_caption = "�e�[�u���̈ꗗ��\��";
        }

        #region Overriden Class Methods

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {
				// �P��ð��ق̗L���𔻒�
				IStandaloneTableCollection	agStdTbls = this.mainForm.MapControl.Map as IStandaloneTableCollection;
				return agStdTbls.StandaloneTableCount > 0;
            }
        }
        
        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
        public override void OnCreate(object hook) {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// �N���b�N������
        /// �e�[�u���ꗗ�t�H�[����\��
        /// </summary>
        public override void OnClick() {
			// ð��ق̈ꗗ���J��
            new Ui.FormTableView(this.mainForm.MapControl).ShowDialog();
        }
        #endregion
    }
}
