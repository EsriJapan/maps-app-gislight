using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Collections.Generic;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// マップフレームのリセットコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class ResetPageLayoutCommand : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResetPageLayoutCommand()
        {

            base.m_caption = "リセット";
            base.m_category = "レイアウト";
            base.m_toolTip = "マップフレームのリセット";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">ツールバーコントロール</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            IToolbarControl2 tlb = (IToolbarControl2)m_hookHelper.Hook;
            IntPtr ptr = (System.IntPtr)tlb.hWnd;
            System.Windows.Forms.Control cntrl = System.Windows.Forms.Control.FromHandle(ptr);
            mainForm = (Ui.MainForm)cntrl.FindForm();
        }

        /// <summary>
        /// クリック時処理
        /// マップフレームのリセット実行
        /// </summary>
        public override void OnClick()
        {
            try
            {
                DialogResult resetLayout = MessageBoxManager.ShowMessageBoxWarining2(
                    mainForm, Properties.Resources.MainForm_WARNING_ResetLayout);

                if (resetLayout != DialogResult.OK)
                {
                    return;
                }

                IMaps tempMaps = new Maps();

                if (mainForm.originalMaps.Count > 0)
                {
                    for (int i = mainForm.originalMaps.Count - 1; i > 0 - 1; i--)
                    {
                        IElement mapFrameElement =
                            (IElement)mainForm.GetMapControlSyncronizer.PageLayoutControl.GraphicsContainer.FindFrame(
                            mainForm.originalMaps.get_Item(i));

                        if (mapFrameElement != null)
                        {
                            mainForm.GetMapControlSyncronizer.PageLayoutControl.GraphicsContainer.DeleteElement(
                                mapFrameElement);
                        }

                        tempMaps.Add(mainForm.originalMaps.get_Item(i));
                    }
                }
                else
                {
                    mainForm.originalMaps.Add(mainForm.axMapControl1.Map);
                    tempMaps.Add(mainForm.axMapControl1.Map);
                }

                mainForm.GetMapControlSyncronizer.ReplaceMap(mainForm.axMapControl1.Map);
                mainForm.GetMapControlSyncronizer.PageLayoutControl.PageLayout.ReplaceMaps(tempMaps);
                 


                IGraphicsContainerSelect graphicsContainerSelect =
                    (IGraphicsContainerSelect)mainForm.axPageLayoutControl1.PageLayout;

                graphicsContainerSelect.UnselectAllElements();

                for (int i = 0; i < mainForm.originalMaps.Count; i++)
                {
                    IFrameElement frameElement =
                        mainForm.axPageLayoutControl1.GraphicsContainer.FindFrame(mainForm.originalMaps.get_Item(i));

                    graphicsContainerSelect.SelectElement((IElement)frameElement);
                }

                if (graphicsContainerSelect.ElementSelectionCount > 0)
                {
                    mainForm.axPageLayoutControl1.GraphicsContainer.SendToBack(graphicsContainerSelect.SelectedElements);
                }

                IPageLayout pageLayout = mainForm.axPageLayoutControl1.PageLayout;
                IGraphicsContainer graphicsContainer = (IGraphicsContainer)pageLayout;

                graphicsContainer.Reset();
                IElement element = graphicsContainer.Next();



                while (element != null)
                {
                    if (element is IMapSurroundFrame)
                    {
                        IMapSurroundFrame mapSurroundFrame = (IMapSurroundFrame)element;
                        mapSurroundFrame.MapSurround.Map = mapSurroundFrame.MapFrame.Map;
                        IMapSurround mapSurround = mapSurroundFrame.MapSurround;

                    }

                    element = graphicsContainer.Next();
                }

 
                // 2011/05/20 コメント外し -->
                mainForm.GetMapControlSyncronizer.ReplaceMap(mainForm.axMapControl1.Map);
                mainForm.GetMapControlSyncronizer.ActivatePageLayout();
                // <--


                mainForm.axPageLayoutControl1.ActiveView.Refresh();
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
            }
        }

        #endregion
    }
}
