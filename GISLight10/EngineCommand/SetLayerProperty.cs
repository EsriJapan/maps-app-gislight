
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ���C���[�v���p�e�B�ݒ�R�}���h
    /// �i���g�p�j
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class SetLayerProperty : Common.EjBaseCommand  
	{
		private IMapControl3 m_mapControl;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public SetLayerProperty()
		{
			base.captionName = "�v���p�e�B";
		}
	
        /// <summary>
        /// �N���b�N������
        /// </summary>
		public override void OnClick()
		{
			ILayer layer =  (ILayer) m_mapControl.CustomProperty;
			//m_mapControl.Map.DeleteLayer(layer);
		}
	
        /// <summary>
        /// �N���G�C�g������
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;
		}

	}
}


