using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// マップドキュメントの新規作成
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public class CreateNewDocument : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private IMapControl3 m_mapControl;

        // 2012/08/22 ADD
        private IPageLayoutControl3 m_pageLayoutControl = null;

        private Ui.MainForm mainForm;
        private ESRIJapan.GISLight10.Common.ControlsSynchronizer m_controlsSynchronizer = null;

        /// <summary>
        /// マップコントロールからメインフォームへの参照を取得
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        private void SetMainFormObj(IMapControl3 mapControl)
        {
            if (mapControl == null) return;

            m_mapControl = mapControl;
            IntPtr ptr2 = (System.IntPtr)mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();

            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;

        }
        // 2012/08/22 ADD

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateNewDocument()
        {
            //update the base properties
            // 2012/08/01 UPD
            base.m_caption = "新規作成";
            base.m_toolTip = "新規作成";

            try
            {
                string bitmapResourceName = this.GetType().Name + ".bmp";
                base.m_bitmap =
                    new Bitmap(this.GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
            // 2012/08/01 UPD
        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// クリック時処理
        /// マップドキュメントの新規作成
        /// </summary>
        public override void OnClick()
        {
			if(this.m_hookHelper == null) {
				return;
			}
			if(this.m_mapControl == null) {
				if(m_hookHelper.Hook is IMapControl3) {
					m_mapControl = (IMapControl3)m_hookHelper.Hook;
				}
				else if(m_hookHelper.Hook is IToolbarControl2) {
					IToolbarControl2	agToolCtl = m_hookHelper.Hook as IToolbarControl2;
					this.m_mapControl = agToolCtl.Buddy as IMapControl3;
				}
				else {
					return;
				}
				SetMainFormObj(m_mapControl);
			}

            try {
                IMapControl3 mapControl = null;

                //get the MapControl from the hook in case the container is a ToolbarControl
                if (m_hookHelper.Hook is IToolbarControl)
                {
                    // 2012/08/01 UPD
                    try
                    {
                        mapControl = (IMapControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;
                        // 2012/08/01 ADD
                        IntPtr ptr2 = (System.IntPtr)mapControl.hWnd;
                        System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                        mainForm = (Ui.MainForm)cntrl2.FindForm();
                        // 2012/08/01 ADD
                    }
                    catch
                    {
                        mapControl = m_mapControl;
                        if (mapControl == null)
                        {
                            m_pageLayoutControl =
                                (IPageLayoutControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;

                            IntPtr ptr2 = (System.IntPtr)m_pageLayoutControl.hWnd;
                            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                            mainForm = (Ui.MainForm)cntrl2.FindForm();
                            mapControl = mainForm.MapControl;

                            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;
                        }
                    }
                                   }
                //In case the container is MapControl
                else if (m_hookHelper.Hook is IMapControl3)
                {
                    mapControl = (IMapControl3)m_hookHelper.Hook;
                }
                else
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(
                        mainForm, "アクティブコントロールはマップコントロールである必要があります");
                    return;
                }

                DialogResult result;
                IEngineEditor engineEditor = new EngineEditorClass();

                if ((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) &&
                    (engineEditor.HasEdits() == true))
                {
                    result = MessageBox.Show(
                        "編集中の状態を保存しますか？",
                        Properties.Resources.CommonMessage_ApplicationName,
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    switch (result)
                    {
                        case DialogResult.Cancel:
                            return;

                        case DialogResult.No:
                            engineEditor.StopEditing(false);
                            break;

                        case DialogResult.Yes:
                            engineEditor.StopEditing(true);
                            break;
                    }
                }

				// 単独ﾃｰﾌﾞﾙ追加状態を取得
				bool	blnAddTable = mapControl.Map != null ? (mapControl.Map as IStandaloneTableCollection).StandaloneTableCount > 0 : false;
                
                // 変更を判定
                if ((mapControl.LayerCount > 0 || blnAddTable) && mainForm.MainMapChanged)
                {
                    DialogResult res = MessageBox.Show(
                        "現在のドキュメントを保存しますか?",
                        Properties.Resources.CommonMessage_ApplicationName,
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    switch (res)
                    {
                        case DialogResult.Cancel:
                            return;

                        case DialogResult.No:
                            break;

                        case DialogResult.Yes:
                            //launch the save command
                            ICommand command = new ControlsSaveAsDocCommandClass();
                            command.OnCreate(m_hookHelper.Hook);
                            command.OnClick();
                            break;
                    }
                }

                // 開いている属性テーブルフォームあれば閉じる
                ESRIJapan.GISLight10.Common.UtilityClass.DeleteAttributeTableObjects();

                // 2012/08/24 add >>>>>
                // 開いているジオリファレンスフォームあれば閉じる
                if (this.mainForm.OwnedForms.Length > 0)
                {
                    for (int i = 0; i < this.mainForm.OwnedForms.Length; i++)
                    {
                        if (this.mainForm.OwnedForms[i].Text.Contains("ジオリファレンス"))
                        {
                            this.mainForm.OwnedForms[i].Dispose();
                            break;
                        }
                    }
                }

                this.mainForm.ClearPageLayout();

                // 直前に開いていたﾄﾞｷｭﾒﾝﾄのﾌﾟﾘﾝﾀ設定状態によっては、既定のﾌﾟﾘﾝﾀ設定に戻らない不具合に対応
                this.mainForm.InitPrintSettings();

                //create a new Map
                IMap map = new MapClass();
                map.Name = "Map";

                //assign the new map to the MapControl
                mapControl.DocumentFilename = string.Empty;
                mapControl.Map = map;

                this.mainForm.GetMapControlSyncronizer.MapControl.DocumentFilename = string.Empty;

                //replace the shared map with the new Map instance
                this.mainForm.originalMaps.Reset();
                this.mainForm.originalMaps.Add(map);

                this.mainForm.GetMapControlSyncronizer.PageLayoutControl.PageLayout.ReplaceMaps(this.mainForm.originalMaps);
                this.mainForm.GetMapControlSyncronizer.ReplaceMap(map);
                
                this.mainForm.SetEvents();
                this.mainForm.SetPageLayoutEvents();
                this.mainForm.ClearMapChangeCount();
                this.mainForm.Text = Properties.Resources.CommonMessage_ApplicationName;
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
