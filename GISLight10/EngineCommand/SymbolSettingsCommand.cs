using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// シンボル設定起動コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class SymbolSettingsCommand : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SymbolSettingsCommand()
        {
			//Set the tool properties
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.Magenta);

            //base.captionName = "単一シンボル表示設定";
            base.m_caption = "シンボル設定";
        }

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
				// ﾚｲﾔｰ･ﾏﾈｰｼﾞｬｰを取得
                ESRIJapan.GISLight10.Common.LayerManager pLayerManager = new ESRIJapan.GISLight10.Common.LayerManager();

                // 有効なﾌｨｰﾁｬｰ･ﾚｲﾔｰ群を取得
                List<IFeatureLayer> featureLayerList = pLayerManager.GetFeatureLayers(m_mapControl.Map);

                // 選択ﾚｲﾔｰを取得
                IFeatureLayer checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                
                // 選択ﾚｲﾔｰがﾌｨｰﾁｬｰ･ﾚｲﾔｰであれば、OK
                if (featureLayerList.Contains(checkLayer))
                //if ((checkLayer != null) && !(checkLayer is GdbRasterCatalogLayer) &&
                //    !(checkLayer is CadFeatureLayer) &&
                //     (checkLayer.Valid) &&
                //     (ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(checkLayer).Type !=
                //      ESRI.ArcGIS.Geodatabase.esriWorkspaceType.esriRemoteDatabaseWorkspace) &&
                //      featureLayerList.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// クリック時処理
        /// シンボル設定フォームの表示
        /// </summary>
        public override void OnClick()
        {
            Ui.FormSymbolSettings frmRenderer = new Ui.FormSymbolSettings(m_mapControl);
            frmRenderer.ShowDialog(mainFrm);
        }
        #endregion
    }
}
