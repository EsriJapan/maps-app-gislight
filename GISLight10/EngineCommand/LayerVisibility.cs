using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ���C���[�\����\���ݒ�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class LayerVisibility : BaseCommand, ICommandSubType 
	{
		private IHookHelper m_hookHelper = new HookHelperClass();
		private long m_subType;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
		public LayerVisibility()
		{
		}
	
        /// <summary>
        /// �N���b�N������
        /// </summary>
		public override void OnClick()
		{
			for (int i=0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
			{
				if (m_subType == 1) m_hookHelper.FocusMap.get_Layer(i).Visible = true;
				if (m_subType == 2) m_hookHelper.FocusMap.get_Layer(i).Visible = false;
			}
			m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,null,null);
            m_hookHelper.ActiveView.Refresh();
		}
	
        /// <summary>
        /// �N���G�C�g����
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
		public override void OnCreate(object hook)
		{
			m_hookHelper.Hook = hook;
		}
	
        /// <summary>
        /// ���C���[�J�E���g�擾
        /// </summary>
        /// <returns>���C���[�J�E���g</returns>
		public int GetCount()
		{
			return 2;
		}
	
        /// <summary>
        /// ���C���[�T�u�^�C�v�ݒ�
        /// </summary>
        /// <param name="SubType">���C���[�T�u�^�C�v</param>
		public void SetSubType(int SubType)
		{
			m_subType = SubType;
		}
	
        /// <summary>
        /// �L���v�V�����擾
        /// </summary>
		public override string Caption
		{
            get
			{
				if (m_subType == 1) return "�S�Ẵ��C����\��";//"Turn All Layers On";
                else return "�S�Ẵ��C�����\��";// "Turn All Layers Off";
			}
		}
	
        /// <summary>
        /// ���s�\����
        /// </summary>
		public override bool Enabled
		{
			get
			{
				bool enabled = false; int i;
				if (m_subType == 1) 
				{
					for (i=0;i<=m_hookHelper.FocusMap.LayerCount - 1;i++)
					{
						if (m_hookHelper.ActiveView.FocusMap.get_Layer(i).Visible == false)
						{
							enabled = true;
							break;
						}
					}
				}
				else 
				{
					for (i=0;i<=m_hookHelper.FocusMap.LayerCount - 1;i++)
					{
						if (m_hookHelper.ActiveView.FocusMap.get_Layer(i).Visible == true)
						{
							enabled = true;
							break;
						}
					}
				}
				return enabled;
			}
		}
	}
}
