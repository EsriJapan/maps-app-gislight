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
    /// �ҏW�I�v�V�����̃R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class EditOptionCommand : Common.EjBaseCommand
    {
        private IHookHelper m_HookHelper;
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public EditOptionCommand()
        {
            base.captionName = "�I�v�V����";
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
        /// �I�v�V�����ݒ�t�H�[����\��
        /// </summary>
        public override void OnClick()
        {
            
            Ui.FormEditOption frm = new Ui.FormEditOption(m_mapControl);
            frm.ShowDialog(mainForm);
        }
        #endregion

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled
        {
            get
            {
                //�G�f�B�^ �c�[���o�[���擾
                ESRI.ArcGIS.Controls.IToolbarItem titm = mainForm.m_ToolbarControl2.GetItem(0);
                //�ҏW�J�n�R�}���h���擾
                ESRI.ArcGIS.Controls.IToolbarItem titmMem = titm.Menu.GetItem(0);

                IEngineEditor engineEditor = new EngineEditorClass();
                
                //�ҏW�J�n�R�}���h���A�N�e�B�u�ȏꍇ
                if (titmMem.Command.Enabled ||
                    engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)                
                {                    
                    return true;                    
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
