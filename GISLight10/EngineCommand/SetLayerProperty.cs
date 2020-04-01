
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// レイヤープロパティ設定コマンド
    /// （未使用）
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class SetLayerProperty : Common.EjBaseCommand  
	{
		private IMapControl3 m_mapControl;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SetLayerProperty()
		{
			base.captionName = "プロパティ";
		}
	
        /// <summary>
        /// クリック時処理
        /// </summary>
		public override void OnClick()
		{
			ILayer layer =  (ILayer) m_mapControl.CustomProperty;
			//m_mapControl.Map.DeleteLayer(layer);
		}
	
        /// <summary>
        /// クリエイト時処理
        /// </summary>
        /// <param name="hook">マップコントロール</param>
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;
		}

	}
}


