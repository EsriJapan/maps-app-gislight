using System;
using System.Drawing;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ���C���̍폜�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class RemoveLayer : Common.EjBaseCommand  
	{
		private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
		public RemoveLayer()
		{
			base.captionName = "���C���̍폜";
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }

		}
	
        /// <summary>
        /// �N���b�N������
        /// ���C���폜���s
        /// </summary>
		public override void OnClick()
		{
			ILayer layer =  (ILayer) m_mapControl.CustomProperty;
			m_mapControl.Map.DeleteLayer(layer);
		}
	
        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
		}

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled
        {
            get
            {
                ILayer layer = (ILayer)m_mapControl.CustomProperty;
                //ILayer2 layer2 = this.mainFrm.SelectedLayer;

                if (layer is IMapServerLayer)
                {
                    return true;
                }

                //if (layer2 == null)
                //{
                //    return false;
                //}
                //else
                //{
                    if (mainFrm.HasFormAttributeTable())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                //}
            }
        }

	}
}


