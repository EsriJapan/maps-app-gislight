using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �X�i�b�v�ݒ�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    ///  2011-01-27 �p������EJBaseCommand�ɕύX 
    /// </history>
    public sealed class EditingSnappingCommand : Common.EjBaseCommand //BaseCommand
    {

        private IHookHelper m_hookHelper;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public EditingSnappingCommand()
        {

            base.captionName = "�X�i�b�v�ݒ�";

        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// </summary>
        /// <param name="hook">�c�[���o�[�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

        }

        /// <summary>
        /// �N���b�N������
        /// </summary>
        public override void OnClick()
        {
            Type commandType =
                Type.GetTypeFromProgID("esriControls.ControlsEditingSnappingCommand");

            if (commandType != null)
            {
                object cmd = Activator.CreateInstance(commandType);
                ICommand command = cmd as ICommand;
                command.OnCreate(m_hookHelper.Hook);
                command.OnClick();
            }
        }

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IEngineEditor engineEditor = new EngineEditorClass();

                if ((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)) // &&
                //(engineEditor.HasEdits() == true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    }
}
