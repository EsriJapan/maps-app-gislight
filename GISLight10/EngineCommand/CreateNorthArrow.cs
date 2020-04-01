using System;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ���ʋL���G�������g�̍쐬
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class CreateNorthArrow : BaseCommand
	{
		private IHookHelper m_HookHelper; 
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private Ui.MainForm mainForm;
        private Ui.FormStyleGallery frmSymbol;
 
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
		public CreateNorthArrow()
		{
			m_HookHelper = new HookHelperClass();

			//Set the tool properties
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            
            base.m_caption = "���ʋL��";
			base.m_category= "CustomCommands";
            base.m_toolTip = "���ʋL��";
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
        /// �X�^�C���N���X�ɕ��ʋL�����w�肵��
        /// �X�^�C���M�������[�t�H�[����\������
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
            //double mapfullXMax = 1.5;
            //double mapfullYMax = m_pageLayoutControl.ActiveView.Extent.UpperLeft.Y;

            double pageWidth;
            double pageHeight;

            m_pageLayoutControl.Page.QuerySize(out pageWidth, out pageHeight);
            // �y�[�W�̍���ɂȂ�悤��
            double mapfullXMin = 2;// m_pageLayoutControl.Extent.XMin; //pageWidth / 2;
            double mapfullYMax = pageHeight - 2;// m_pageLayoutControl.Extent.YMax; //mapfullYMin + 2.4;
            double mapfullYMin = mapfullYMax - 2.4; //pageHeight / 2;
            double mapfullXMax = mapfullXMin + 2.4;

            envelope.PutCoords(mapfullXMin, mapfullYMin, mapfullXMax, mapfullYMax);

            // Add-->
            ESRIJapan.GISLight10.Common.Logger.Debug("On CreateNorthArrow UsingMemorySize:" +
                GC.GetTotalMemory(false).ToString() + " byte");

            GC.Collect();

            ESRIJapan.GISLight10.Common.Logger.Debug("After Full GC.Collect UsingMemorySize:" +
                GC.GetTotalMemory(false).ToString() + " byte");
            //<--

            frmSymbol = new Ui.FormStyleGallery(mapFrame, m_pageLayoutControl, envelope);
            frmSymbol.SetItem(esriSymbologyStyleClass.esriStyleClassNorthArrows);
            frmSymbol.Text = base.m_caption + " �I��";
            frmSymbol.ShowDialog(mainForm);
            frmSymbol.Dispose();

            mainForm.MainMapChanged = true;
        }

	}
}
