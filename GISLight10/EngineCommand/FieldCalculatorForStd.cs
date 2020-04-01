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
    /// �X�^���h�A�����e�[�u���̃t�B�[���h���Z���s�Ȃ��܂�
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class FieldCalculatorForStd : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public FieldCalculatorForStd()
        {
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.White);

            base.m_caption = "�t�B�[���h���Z";
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
        /// �t�B�[���h���Z�t�H�[����\��
        /// </summary>
        public override void OnClick() {
			// ̨���މ��Z���J��
            Ui.FormFieldCalculate frmCalc = new Ui.FormFieldCalculate(this.m_mapControl, this.mainForm, this.mainForm.SelectedTable);
            frmCalc.ShowDialog(this.mainForm);
            frmCalc.Dispose();
        }
        #endregion
    }
}
