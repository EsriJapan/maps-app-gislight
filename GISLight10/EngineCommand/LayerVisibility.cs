using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// レイヤー表示非表示設定コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class LayerVisibility : BaseCommand, ICommandSubType 
	{
		private IHookHelper m_hookHelper = new HookHelperClass();
		private long m_subType;

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public LayerVisibility()
		{
		}
	
        /// <summary>
        /// クリック時処理
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
        /// クリエイト処理
        /// </summary>
        /// <param name="hook">マップコントロール</param>
		public override void OnCreate(object hook)
		{
			m_hookHelper.Hook = hook;
		}
	
        /// <summary>
        /// レイヤーカウント取得
        /// </summary>
        /// <returns>レイヤーカウント</returns>
		public int GetCount()
		{
			return 2;
		}
	
        /// <summary>
        /// レイヤーサブタイプ設定
        /// </summary>
        /// <param name="SubType">レイヤーサブタイプ</param>
		public void SetSubType(int SubType)
		{
			m_subType = SubType;
		}
	
        /// <summary>
        /// キャプション取得
        /// </summary>
		public override string Caption
		{
            get
			{
				if (m_subType == 1) return "全てのレイヤを表示";//"Turn All Layers On";
                else return "全てのレイヤを非表示";// "Turn All Layers Off";
			}
		}
	
        /// <summary>
        /// 実行可能判定
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
