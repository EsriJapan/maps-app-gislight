using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;

using ESRIJapan.GISLight10.Common;


namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ジオメトリ演算のコマンドクラス
    /// </summary>
    public class LayerFieldCalculatorCommand : FieldCalculatorCommand
    {

        /// <summary>
        /// クリック時処理
        /// ジオメトリ演算フォームの表示
        /// </summary>
        public override void OnClick()
        {
            Ui.FormFieldCalculate frm = null;
            IFeatureLayer pLayer;

            try
            {
                pLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                if (pLayer == null)
                {
                    // ここには入らないはず
                    return;
                }

                frm = new Ui.FormFieldCalculate(m_mapControl, mainFrm, pLayer);
                frm.ShowDialog(mainFrm);
                frm.Dispose();
            }
            catch (Exception ex)
            {
                if (frm != null)
                {
                    frm.Close();
                    frm.Dispose();
                }
                MessageBoxManager.ShowMessageBoxError(mainFrm, ex.Message);
            }
        }


        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IFeatureLayer pLayer;
                ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();
                List<IFeatureLayer> featureLayerList =
                    pLayerManager.GetFeatureLayers(m_mapControl.Map);

                pLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                if (pLayer == null)
                {
                    // フィーチャレイヤではない場合
                    return false;
                }

                if (featureLayerList.Contains(pLayer) == false)
                {
                    // アプリが許さないレイヤの場合
                    return false;
                }
 
                return base.Enabled;
            }
        }
    }
}
