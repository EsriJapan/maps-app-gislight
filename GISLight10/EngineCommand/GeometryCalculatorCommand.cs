using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;

using ESRIJapan.GISLight10.Common;


namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ジオメトリ演算のコマンドクラス
    /// </summary>
    public class GeometryCalculatorCommand : Common.EjBaseCommand
    {
        /// <summary>
        /// マップコントロール
        /// </summary>
        protected IMapControl3 m_mapControl;

        /// <summary>
        /// メインフォーム
        /// </summary>
        protected Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GeometryCalculatorCommand()
        {
            base.captionName = "面積・長さ計算";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new System.Drawing.Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
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
        /// ジオメトリ演算フォームの表示
        /// </summary>
        public override void OnClick()
        {
            Ui.FormGeometryCalculate frm = null;

            try
            {
                frm = new Ui.FormGeometryCalculate(m_mapControl);
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
                int count = 0;
                IEngineEditor pEditor;

                pEditor = new EngineEditorClass();
                if (pEditor.EditState != esriEngineEditState.esriEngineStateNotEditing)
                {
                    return false;
                }

                ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();

                List<IFeatureLayer> featureLayerList =
                    pLayerManager.GetFeatureLayers(m_mapControl.Map);

                // ポリゴンフィーチャとラインフィーチャの数を数える
                foreach (IFeatureLayer flay in featureLayerList)
                {
                    if (flay.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon ||
                        flay.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    if (mainFrm.HasFormAttributeTable())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
