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
    /// ��������
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class AttributeSearchCommandForStd : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public AttributeSearchCommandForStd()
        {

            base.captionName = "��������";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new Bitmap(GetType(), bitmapResourceName);
				base.buttonImage.MakeTransparent(Color.White);
            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }

        #region Overriden Class Methods

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
        /// ���������t�H�[���̕\��
        /// </summary>
        public override void OnClick() {
            //new Ui.FormAttributeSearch(m_mapControl, mainForm, mainForm.SelectedTable).ShowDialog(mainForm);
        }

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
				// �I��ð��ق̗L�����m�F / ����ð��ٔ�\��
				if(this.mainForm.SelectedTable != null && !mainForm.HasFormAttributeTable()) {
					blnRet = true;
				}
				return blnRet;
            }
        }
        #endregion
    }
}
