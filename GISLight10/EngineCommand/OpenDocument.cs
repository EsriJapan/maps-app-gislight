#define usestndcmd__

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Output;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// サポート対象キュメントを開くコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public class OpenDocument : BaseCommand
    {

        private IHookHelper m_hookHelper = null;
        private IMapControl3 m_mapControl;
        private IPageLayoutControl3 m_pageLayoutControl = null;

        private Ui.MainForm mainForm;
        private ESRIJapan.GISLight10.Common.ControlsSynchronizer m_controlsSynchronizer = null;

        private ESRIJapan.GISLight10.Common.SubWindowNameClass subwinname = 
            ESRIJapan.GISLight10.Common.SubWindowNameClass.GetInstance();

        /// <summary>
        /// マップコントロールからメインフォームへの参照を取得
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        private void SetMainFormObj(IMapControl3 mapControl) {
            if (mapControl == null) return;

            m_mapControl = mapControl;
            IntPtr ptr2 = (System.IntPtr)mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();

            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;

        }
 
        /// <summary>
        /// 現在開いているMXDの状態を完結します
        /// </summary>
        /// <returns>OK / NG</returns>
        private bool CompleteOpenedMXDStates() {
			bool	blnRet = true;

            IEngineEditor	engineEditor = new EngineEditorClass();
            DialogResult	result;

            // 編集有無を審査
            if((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) &&
                (engineEditor.HasEdits() == true))
            {
                result = MessageBox.Show(
                    "編集中の状態を保存しますか？",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch(result) {
                case DialogResult.Cancel:
                    blnRet = false;
                    break;

                case DialogResult.No:
                    engineEditor.StopEditing(false);
                    break;

                case DialogResult.Yes:
                    engineEditor.StopEditing(true);
                    break;
                }
            }
            
            // ﾄﾞｷｭﾒﾝﾄ保存
            if(blnRet && mainForm.MainMapChanged) {
                DialogResult res = MessageBox.Show(
                    "現在のドキュメントを保存しますか?",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch(res) {
                case DialogResult.Cancel:
                    blnRet = false;
                    break;

                case DialogResult.No:
                    if(engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                        engineEditor.StopEditing(false);
                    }
                    break;

                case DialogResult.Yes:
                    if(engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                        engineEditor.StopEditing(true);
                    }

                    // MXDを保存
                    ICommand command = new ControlsSaveAsDocCommandClass();
                    command.OnCreate(m_hookHelper.Hook);
                    command.OnClick();
                    break;
                }
            }
            
            return blnRet;
		}

        /// <summary>
        /// マップ・ドキュメントを展開します（PageLayout.ReplaceMaps方式）
		/// ※10.0では、複数ﾏｯﾌﾟでﾚｲｱｳﾄされている場合、ﾏｯﾌﾟの配置が換わってしまう不具合があった。
        /// </summary>
        /// <param name="sFilePath">MXDﾌｧｲﾙ･ﾊﾟｽ</param>
		/// <returns>OK / NG</returns>
		private bool OpenSupportDocument(string sFilePath) {
            IMapDocument mapDoc = new MapDocumentClass();

            // マップドキュメントを開く場合に時間が掛かることがあるのでカーソルをWaitカーソルに変更
            System.Windows.Forms.Cursor preCursor = mainForm.Cursor;
            mainForm.Cursor = Cursors.WaitCursor;
            
            // ﾏｯﾌﾟｺﾝﾄﾛｰﾙ取得補助(ﾒﾆｭｰ/ﾂｰﾙ実行時によって、ﾏｯﾌﾟｺﾝﾄﾛｰﾙがｾｯﾄされない為)
            if(m_mapControl == null) {
				m_mapControl = mainForm.axMapControl1.Object as IMapControl3;
            }

            try {
                if(mapDoc.get_IsPresent(sFilePath) && !mapDoc.get_IsPasswordProtected(sFilePath)) {
                    mapDoc.Open(sFilePath, string.Empty);
                    
                    // 2011/06/16 固定縮尺の場合に解除
                    IPageLayout pageLayout = mapDoc.PageLayout;

                    // ﾄﾞｷｭﾒﾝﾄのﾌﾟﾘﾝﾀを設定
                    if(mapDoc.Printer != null) {
						// ﾌﾟﾘﾝﾀ設定の復元
						if(!mainForm.LoadPrintDocument(mapDoc.Printer)) {
							// 既定のﾌﾟﾘﾝﾀ･A4設定 (設定ﾌﾟﾘﾝﾀがなくなっている場合)
							//mainForm.SetDefaultPrinter();
							mainForm.InitPrintSettings();
						}
					}
					else {
						//mainForm.SetDefaultPrinter();
					}
					mainForm.LoadPrintSetting(pageLayout);
                    
                    mainForm.originalMaps.Reset();
                    if(mapDoc.MapCount > 0) {
                        for(int i = 0; i < mapDoc.MapCount; i++) {
#if DEBUG
							Debug.WriteLine(mapDoc.get_Map(i).Name);
#endif
                            mainForm.originalMaps.Add(mapDoc.get_Map(i));
                        }
                    }

                    IMapFrame			mapFrame;
                    IMapSurroundFrame	agMSF;

                    IGraphicsContainer graphicsContainer = (IGraphicsContainer)pageLayout;
                    graphicsContainer.Reset();

                    IElement element = graphicsContainer.Next();
                    while(element != null) {
                        if(element is IMapFrame) {
                            mapFrame = (IMapFrame)element;

                            if(mapFrame.ExtentType != esriExtentTypeEnum.esriExtentDefault) {
                                mapFrame.ExtentType = esriExtentTypeEnum.esriExtentDefault;
                            }
                        }
                        else if(element is IMapSurroundFrame) {
							agMSF = element as IMapSurroundFrame;
							if(agMSF.MapSurround is ILegend) {
								agMSF.MapSurround.Refresh();

								// 凡例の自動ﾚｲﾔ追加設定を強制的にOFF
								ILegend	agLeg = agMSF.MapSurround as ILegend;
								if(agLeg.AutoAdd) {
									agLeg.AutoAdd = false;
								}
							}
                        }

                        element = graphicsContainer.Next();
                    }
                    
					IActiveView	agActView = m_mapControl.Map as IActiveView;
					agActView.Clear();
					agActView.GraphicsContainer.DeleteAllElements();
					
					//m_controlsSynchronizer.PageLayoutControl.LoadMxFile(mapDoc.DocumentFilename, Type.Missing);

                    m_controlsSynchronizer.PageLayoutControl.DocumentFilename = mapDoc.DocumentFilename;
                    m_controlsSynchronizer.PageLayoutControl.PageLayout = pageLayout;
                    if(mapDoc.MapCount <= 1) {
						m_controlsSynchronizer.PageLayoutControl.PageLayout.ReplaceMaps(mainForm.originalMaps);
                    }

					m_mapControl.DocumentFilename = mapDoc.DocumentFilename;
					//m_mapControl.Map = mapDoc.ActiveView.FocusMap;
					m_controlsSynchronizer.ReplaceMap(mapDoc.ActiveView.FocusMap);

                    // ※ｶﾚﾝﾄ･ﾃﾞｨﾚｸﾄﾘを設定しても
                    Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(mapDoc.DocumentFilename);
#if DEBUG
                    // ﾊｲﾊﾟｰﾘﾝｸ･ﾁｪｯｸ ※相対ﾊﾟｽﾌｫﾙﾀﾞｰの場合、開かれない現象の確認 (ArcMapでは問題なし)
                    // 下記ﾌﾟﾛﾊﾟﾃｨを設定直後はﾌｫﾙﾀﾞｰが開かれるようになる?が、その後元に戻ってしまう。
                    IDocumentInfo2	agDocInfo2 = mapDoc as IDocumentInfo2;
                    if(string.IsNullOrEmpty(agDocInfo2.HyperlinkBase)) {
						agDocInfo2.HyperlinkBase = System.IO.Path.GetDirectoryName(mapDoc.DocumentFilename);
                    }
#endif

                    // 2010-12-27 add (透過レイヤのマップ表示されない件の対応)
                    m_mapControl.Map = mapDoc.ActiveView.FocusMap;
                    m_mapControl.ActiveView.Refresh();
                    
                    if(mapDoc.ActiveView is IMap) {
						mainForm.tabControl1.SelectedIndex = 0;
                    }
                    else {
						mainForm.tabControl1.SelectedIndex = 1;
                    }

                    if(mainForm.tabControl1.SelectedIndex == 1) {
                        m_controlsSynchronizer.ActivatePageLayout();
                        mainForm.axPageLayoutControl1.ActiveView.Refresh();
                    }
                    // end add

                    mapDoc.Close();
                    
                    return true;
                }
                return false;
            }
            catch(COMException comex) {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
                return false;
            }
            catch(Exception ex) {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
                return false;
            }
            finally {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mapDoc);
                mainForm.Cursor = preCursor; // 元のカーソルに戻す
            }
        }
        

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenDocument() {
            base.m_caption = "開く";
            base.m_toolTip = "開く";
            try {
                string bitmapResourceName = this.GetType().Name + ".bmp";
                base.m_bitmap = 
                    new Bitmap(this.GetType(), bitmapResourceName);

                //this.m_controlsSynchronizer = controlsSynchronizer;

            }
            catch(Exception ex) {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
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
			if (hook == null)
				return;

			if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// クリック時処理
        /// ファイルダイアログを表示
        /// </summary>
        public override void OnClick() {
			if(this.m_hookHelper == null) {
				return;
			}
			if(this.m_mapControl == null) {
				if(m_hookHelper.Hook is IMapControl3) {
					this.m_mapControl = (IMapControl3)m_hookHelper.Hook;
				}
				else if(m_hookHelper.Hook is IToolbarControl2) {
					IToolbarControl2	agToolCtl = m_hookHelper.Hook as IToolbarControl2;
					this.m_mapControl = agToolCtl.Buddy as IMapControl3;
				}
				else {
					return;
				}
				SetMainFormObj(this.m_mapControl);
			}

            try {
                IMapControl3 mapControl = m_mapControl;

                //get the MapControl from the hook in case the container is a ToolbarControl
                if(m_hookHelper.Hook is IToolbarControl) {
                    try {
                        mapControl = (IMapControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;
                    }
                    catch { 
                        mapControl = m_mapControl;
                        if(mapControl == null) {
                            m_pageLayoutControl = (IPageLayoutControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;

                            IntPtr ptr2 = (System.IntPtr)m_pageLayoutControl.hWnd;
                            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                            mainForm = (Ui.MainForm)cntrl2.FindForm();
                            mapControl = mainForm.MapControl;

                            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;
                        }
                    }
                }
                //In case the container is MapControl
                else if(m_hookHelper.Hook is IMapControl3) {
                    mapControl = (IMapControl3)m_hookHelper.Hook;
                }
                else {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(
                        mainForm, "アクティブコントロールはマップコントロールである必要があります");
                    return;
                }

                //allow the user to save the current document
                if (mainForm == null) SetMainFormObj(mapControl);

                // 現在の編集ｾｯｼｮﾝを終了させる, ﾏｯﾌﾟの変更を保存
                if(!this.CompleteOpenedMXDStates()) {
					return;
				}

                // 2011/06/09 
                // シンクロナイザ使用の影響でドキュメント開く際にはマップタブカレントが必須
                // そうしないと固定縮尺のMXD開く際にマップ側で固定縮尺ではなくなる
                // 本来ダイアログ閉じた後にしたいが、そうすると動作不正になるのでここで行なう
                //if (mainForm != null && mainForm.tabControl1 != null)
                //{
                //    if (mainForm.tabControl1.SelectedIndex != 0)
                //    {
                //        mainForm.tabControl1.SelectedIndex = 0;
                //    }
                //}
                // 

#if usestndcmd

                //2011/06/08-->
                string currentDocPath = string.Empty;
                if (mainForm.tabControl1.SelectedIndex == 0)
                {
                    if (mainForm.axMapControl1 != null && 
                        mainForm.axMapControl1.DocumentFilename != null &&
                        mainForm.axMapControl1.DocumentFilename.Length > 0 && 
                        System.IO.File.Exists(mainForm.axMapControl1.DocumentFilename))
                    {
                        currentDocPath = mainForm.axMapControl1.DocumentFilename;
                    }
                }
                else
                {
                    if (mainForm.axPageLayoutControl1 != null &&
                        mainForm.axPageLayoutControl1.DocumentFilename != null &&
                        mainForm.axPageLayoutControl1.DocumentFilename.Length > 0 &&
                        System.IO.File.Exists(mainForm.axPageLayoutControl1.DocumentFilename))
                    {
                        currentDocPath = mainForm.axPageLayoutControl1.DocumentFilename;
                    }
                }

                ICommand cmd = new ControlsOpenDocCommandClass();
                cmd.OnCreate(m_hookHelper.Hook);
                cmd.OnClick();
                
                string docpath = string.Empty;

                if (mainForm.tabControl1.SelectedIndex == 0)
                {
                    if (mainForm.axMapControl1.DocumentFilename == null ||
                        mainForm.axMapControl1.DocumentFilename.Length == 0 ||
                        !System.IO.File.Exists(mainForm.axMapControl1.DocumentFilename))
                    {
                        return;
                    }
                    else
                    {
                        docpath = mainForm.axMapControl1.DocumentFilename;
                    }
                }
                else
                {
                    if (mainForm.axPageLayoutControl1.DocumentFilename == null ||
                        mainForm.axPageLayoutControl1.DocumentFilename.Length == 0 ||
                        !System.IO.File.Exists(mainForm.axPageLayoutControl1.DocumentFilename))
                    {
                        return;
                    }
                    else
                    {
                        docpath = mainForm.axPageLayoutControl1.DocumentFilename;
                    }
                }

                if (currentDocPath.Equals(docpath))
                {
                    // 変更なし
                    return;
                }

                string filepath = System.IO.Path.GetFileName(docpath);
                if (filepath != null)
                {
                    // 開いている属性テーブルフォームあれば閉じる
                    ESRIJapan.GISLight10.Common.UtilityClass.DeleteAttributeTableObjects();

                    if (OpenSupportDocument(docpath))
                    {
                        this.mainForm.Text = filepath + " - " +
                            Properties.Resources.CommonMessage_ApplicationName;
                        this.mainForm.SetEvents();
                        this.mainForm.SetPageLayoutEvents();
                        this.mainForm.ClearMapChangeCount();
                    }
                }
                //return;
                //<--
#else
                // 2011/06/09 del -->
                OpenFileDialog ofd = new OpenFileDialog();
                try
                {

                    ofd.FileName = "";
                    ofd.InitialDirectory = "";

                    ofd.Filter =
                        "ArcMapドキュメント (*.mxd)|*.mxd|" +
                        "ArcMapテンプレート (*.mxt)|*.mxt|" +
                        "パブリッシュマップドキュメント (*.pmf)|*.pmf|" +
                        "サポートする全てのマップフォーマット (*.mxd, *.mxt, *.pmf)|*.mxd;*.mxt;*.pmf";

                    ofd.FilterIndex = 0;
                    ofd.Title = "開く";
                    ofd.RestoreDirectory = true;
                    ofd.CheckFileExists = true;
                    ofd.CheckPathExists = true;
                    if (ofd.ShowDialog(mainForm) == DialogResult.OK)
                    {
                        string path = System.IO.Path.GetFileName(ofd.FileName);
                        if (path != null)
                        {
                            //this.mainForm.ClearPageLayoutEvents();


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


                            // 開いている属性テーブルフォームあれば閉じる
                            ESRIJapan.GISLight10.Common.UtilityClass.DeleteAttributeTableObjects();

                            // 開く
                            this.OpenDocFile(ofd.FileName);
                        }
                    }
                }
                finally
                {
                    ofd.Dispose();
                    ofd = null;
                }
                //<--
#endif

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

        /// <summary>
        /// 指定のドキュメント・ファイルを展開します (内部用)
        /// </summary>
        /// <param name="DocFilePath"></param>
        /// <returns></returns>
        private bool OpenDocFile(string DocFilePath) {
            bool	blnRet = false;
            
            // MapControlﾁｪｯｸ
            try {
				// 指定のﾄﾞｷｭﾒﾝﾄ･ﾌｧｲﾙを開く
				blnRet = OpenSupportDocument(DocFilePath);
			
				if(blnRet) {
					this.mainForm.Text = System.IO.Path.GetFileName(DocFilePath) + " - " + Properties.Resources.CommonMessage_ApplicationName;
					this.mainForm.SetEvents();
					this.mainForm.SetPageLayoutEvents();
					this.mainForm.ClearMapChangeCount();
				}
            }
            catch(Exception ex) {
				ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
            }
			
			return blnRet;
        }

        /// <summary>
        /// 指定のドキュメント・ファイルを展開します
        /// </summary>
        /// <param name="DocFilePath">MXDﾌｧｲﾙ･ﾊﾟｽ</param>
        /// <param name="MapControl">MapControl</param>
        /// <returns>OK / NG ※MXDを実際に開いて失敗する場合を除き、OKとします。</returns>
        public bool OpenDocFile(string DocFilePath, IMapControl3 MapControl) {
			bool	blnRet = true;
			
			// ﾏｯﾌﾟ･ｺﾝﾄﾛｰﾙを設定
			if(MapControl != null) {
				this.SetMainFormObj(MapControl);
			}

            // 現在の編集ｾｯｼｮﾝを終了させる, ﾏｯﾌﾟの変更を保存
			blnRet = OpenDocFile(DocFilePath);
			
			return blnRet;
        }
    }
}
