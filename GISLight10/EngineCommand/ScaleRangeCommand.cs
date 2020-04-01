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
    /// 縮尺範囲コマンド
    /// </summary>
    /// <history>
    ///  2012-07-04 新規作成 
    /// </history>
    public sealed class ScaleRangeCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ScaleRangeCommand()
        {
            base.captionName = "縮尺範囲設定";

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
        /// 縮尺範囲フォームの表示
        /// </summary>
        public override void OnClick()
        {
            Ui.FormScaleRange frm = new Ui.FormScaleRange(m_mapControl, mainFrm);
            frm.ShowDialog(this.mainFrm);
        }
        #endregion

        /// <summary>
        /// Enabled
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IEngineEditor pEditor;

                pEditor = new EngineEditorClass();
                if (pEditor.EditState != esriEngineEditState.esriEngineStateNotEditing)
                {
                    return false;
                }

                ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();

                List<ILayer> LayerList =
                    pLayerManager.GetAllLayers(m_mapControl.Map);

                ILayer checkLayer = this.mainFrm.SelectedLayer;
                if (!LayerList.Contains(checkLayer))
                {
                    return false;
                }


                // ポリゴンフィーチャとラインフィーチャの数を数える
                if (LayerList.Count > 0)
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

