using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// フィールド演算のコマンドクラス
    /// </summary>
    public class FieldCalculatorCommand: Common.EjBaseCommand
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
        public FieldCalculatorCommand()
        {
            base.captionName = "フィールド演算";

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
        /// OnClick
        /// </summary>
       public override void OnClick()
        {
            Ui.FormFieldCalculate frm = null;

            try
            {
                frm = new Ui.FormFieldCalculate(m_mapControl, mainFrm);
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
        /// Enabled
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

                IFeatureLayer pLayer;
                ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();
                List<IFeatureLayer> featureLayerList =
                    pLayerManager.GetFeatureLayers(m_mapControl.Map);

                if (featureLayerList.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

             }
        }

    }
}
