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
    /// �����e�[�u�����J���R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class OpenStandaloneTable : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public OpenStandaloneTable()
        {
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.Magenta);

            base.m_caption = "�����e�[�u�����J��";
        }

        #region Overriden Class Methods

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
				
                // ��ި�� �܂��� �޵�̧�ݽ�����s���Ă��Ȃ�����
                IEngineEditor engineEditor = new EngineEditorClass();
                if(engineEditor.EditState == esriEngineEditState.esriEngineStateNotEditing && !this.mainForm.HasGeoReference()) {
					// �I��ð��ق̗L�����m�F
					if(this.mainForm.SelectedTable != null) {
						blnRet = true;
					}
                }

				return blnRet;
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
        /// �����e�[�u���\���t�H�[���̕\��
        /// </summary>
        public override void OnClick() {
			// �P��ð��ق��J��
            new Ui.FormAttributeTable(this.mainForm.SelectedTable, this.mainForm).Show(mainForm);

            mainForm.m_ToolbarControl2.Enabled = false;
        }
        #endregion
    }
}