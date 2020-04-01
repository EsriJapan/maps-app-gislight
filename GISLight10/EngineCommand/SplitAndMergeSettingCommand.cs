using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �X�v���b�g�E�}�[�W�ݒ�̃R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
	///  2012-07-26 �N�������ύX(�ҏW�O��ŋN��OK �� �ҏW�O�̂�OK)
    /// </history>
    public sealed class SplitAndMergeSettingCommand : Common.EjBaseCommand
    {
        private IHookHelper m_HookHelper;
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public SplitAndMergeSettingCommand()
        {
            base.captionName = "�X�v���b�g�E�}�[�W�ݒ�";
        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�c�[���o�[�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_HookHelper = new HookHelperClass();
                m_HookHelper.Hook = hook;

                IToolbarControl2 tlb2 = (IToolbarControl2)m_HookHelper.Hook;
                m_mapControl = (IMapControl3)tlb2.Buddy;

                IntPtr ptr = (System.IntPtr)m_mapControl.hWnd;

                System.Windows.Forms.Control cntrl =
                    System.Windows.Forms.Control.FromHandle(ptr);
                mainForm = (Ui.MainForm)cntrl.FindForm();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// �N���b�N������
        /// �X�v���b�g�E�}�[�W�ݒ�t�H�[���̕\��
        /// </summary>
        public override void OnClick()
        {
			// �ݒ��ʂ�\��
            Ui.FormSplitAndMergeSetting frm = new Ui.FormSplitAndMergeSetting(m_mapControl);
            frm.Text = base.captionName;

            // �ݒ�̫�щ�ʂ�\��
            frm.ShowDialog(mainForm);
        }

        /// <summary>
        /// �ҏW�^�[�Q�b�g���C�����擾
        /// </summary>
        /// <returns>�ҏW�^�[�Q�b�g���C��</returns>
        private IFeatureLayer GetEditTargetLayer() {
            // ��ި����擾
            IEngineEditor engineEditor = new EngineEditorClass();

            // �ҏW�Ώ�ڲ԰���擾
            IEngineEditLayers editLayer = (IEngineEditLayers)engineEditor;
            IFeatureLayer targetFcLayer = editLayer.TargetLayer;

            return targetFcLayer;
        }

        #endregion

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {
                //�G�f�B�^ �c�[���o�[���擾
                ESRI.ArcGIS.Controls.IToolbarItem titm = mainForm.m_ToolbarControl2.GetItem(0);
                //�ҏW�J�n�R�}���h���擾
                ESRI.ArcGIS.Controls.IToolbarItem titmMem = titm.Menu.GetItem(0);

                IEngineEditor engineEditor = new EngineEditorClass();

                // �ҏWӰ�ނłȂ�����
                if(titmMem.Command.Enabled && engineEditor.EditState == esriEngineEditState.esriEngineStateNotEditing) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
    }
}
