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
    public sealed class CreateLegend : BaseCommand
	{
		private IHookHelper m_HookHelper; 
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private Ui.MainForm mainForm;
 
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
		public CreateLegend()
		{
			m_HookHelper = new HookHelperClass();

			//Set the tool properties
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            base.m_bitmap.MakeTransparent(Color.Magenta);
            
            base.m_caption = "�}��";
			base.m_category= "CustomCommands";
            base.m_toolTip = "�}��";
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

            // Add-->
            ESRIJapan.GISLight10.Common.Logger.Debug("On CreateLegend UsingMemorySize:" +
                GC.GetTotalMemory(false).ToString() + " byte");

            GC.Collect();

            ESRIJapan.GISLight10.Common.Logger.Debug("After Full GC.Collect UsingMemorySize:" +
                GC.GetTotalMemory(false).ToString() + " byte");
            //<--

            // �}��쐬̫�т�\��
			Ui.FormLegendSettings2 frm = new Ui.FormLegendSettings2(m_pageLayoutControl, m_HookHelper.ActiveView);


            frm.ShowDialog(mainForm);

            mainForm.MainMapChanged = true;
        }

	}
}
