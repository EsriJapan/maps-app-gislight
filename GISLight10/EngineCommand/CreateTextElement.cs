using System;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Runtime.InteropServices;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �e�L�X�g�G�������g�̍쐬
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class CreateTextElement : BaseCommand
	{
		private IHookHelper m_HookHelper; 
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public CreateTextElement()
		{
			m_HookHelper = new HookHelperClass();

            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);

            base.m_caption = "�e�L�X�g";
            base.m_category = "CustomCommands";
            base.m_toolTip = "�e�L�X�g";
        }

        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�y�[�W���C�A�E�g�R���g���[��</param>
        public override void OnCreate(object hook)
		{
			m_HookHelper.Hook = hook;

            IToolbarControl2 tlb = (IToolbarControl2)m_HookHelper.Hook;
            IntPtr ptr = (System.IntPtr)tlb.hWnd;
            System.Windows.Forms.Control cntrl = System.Windows.Forms.Control.FromHandle(ptr);
            mainForm = (Ui.MainForm)cntrl.FindForm();
        }

        /// <summary>
        /// �N���b�N������
        /// �e�L�X�g�G�������g�ݒ�t�H�[����\������
        /// </summary>
        public override void OnClick()
        {
            m_pageLayoutControl = 
                mainForm.axPageLayoutControl1.Object as IPageLayoutControl3;
            
            IMapFrame mapFrame = 
                (IMapFrame)m_HookHelper.ActiveView.GraphicsContainer.FindFrame(
                m_HookHelper.ActiveView.FocusMap);

            IEnvelope envelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            //envelope = m_pageLayoutControl.Extent.Envelope;
            //double mapfullXMin = m_pageLayoutControl.ActiveView.Extent.UpperLeft.X;
            //double mapfullYMin = 25.0;
            //double mapfullXMax = 20.0;
            //double mapfullYMax = m_pageLayoutControl.ActiveView.Extent.UpperLeft.Y;

            double pageWidth;
            double pageHeight;

            m_pageLayoutControl.Page.QuerySize(out pageWidth, out pageHeight);
            // �y�[�W�̒�����
            double mapfullXMin = (pageWidth / 2) - 2.4;
            double mapfullYMax = pageHeight - 2;
            double mapfullYMin = mapfullYMax - 0.6;
            double mapfullXMax = mapfullXMin + 4.8;
            

            envelope.PutCoords(mapfullXMin, mapfullYMin, mapfullXMax, mapfullYMax);

            Ui.FormText frm = new Ui.FormText(m_pageLayoutControl, envelope);
            frm.Text = base.m_caption + " ����";
            //frm.Text = base.captionName + " ����";
            frm.ShowDialog(mainForm);

            mainForm.MainMapChanged = true;
        }

	}
}
