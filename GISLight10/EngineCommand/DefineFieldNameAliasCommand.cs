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
    /// フィールドの別名定義を行うフォームを呼び出す
    /// </summary>
    /// <history>
    ///  2011/05/31 新規作成 
    /// </history>
    public sealed class DefineFieldNameAliasCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// constructor
        /// </summary>
        public DefineFieldNameAliasCommand()
        {
            base.captionName = "フィールドの別名定義";

        }

        #region Overriden Class Methods

        /// <summary>
        /// コマンド生成時
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = 
                System.Windows.Forms.Control.FromHandle(ptr2);
            
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// コマンドクリック時
        /// </summary>
        public override void OnClick()
        {
            Ui.FormDefineFieldNameAlias frm = new Ui.FormDefineFieldNameAlias(mainFrm, true);
            frm.ShowDialog(mainFrm);
        }


        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();

                List<IFeatureLayer> featureLayerList =
                    pLayerManager.GetFeatureLayers(m_mapControl.Map);

                IFeatureLayer checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                if (featureLayerList.Contains(checkLayer))
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
