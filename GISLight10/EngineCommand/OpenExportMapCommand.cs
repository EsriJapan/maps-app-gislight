//#define doSubThread　//2012.08.18 進捗ダイアログ対応で無効化

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;
using System.Timers;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Output;

using ESRIJapan.GISLight10.Common;
using ESRIJapan.GISLight10.Ui;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// マップのエクスポート起動コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public sealed class OpenExportMapCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;
        private string lastExportPath = "";
        private Ui.UserControlExportMap expmap = null;
        private string[] filterTypes = null;
        private IActiveView tview = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenExportMapCommand()
        {
            base.captionName = "マップのエクスポート";
        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

        /// <summary>
        /// クリック時処理
        /// マップエクスポートコマンドをサブスレッドにて起動
        /// </summary>
        public override void OnClick()
        {
            bool dispMessage = false;
            try
            {
                IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
                System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                this.mainFrm = (Ui.MainForm)cntrl2.FindForm();

                expmap = new Ui.UserControlExportMap(m_mapControl.ActiveView);
    
                expmap.FilterFileTypes = "BMP,JPEG,PNG,GIF,PDF";
                filterTypes = expmap.FilterFileTypes.Split(',');

                expmap.FilterIndex = 0;
                expmap.Caption = base.captionName;
                expmap.FileDlgCheckFileExists = false;

                if (this.lastExportPath.Length > 0)
                {
                    expmap.LastExportPath = this.lastExportPath;
                }

                if (expmap.ShowDialog() == DialogResult.OK)
                {
                    if (!expmap.IsDpiValid())
                    {
                        expmap.ShowDpiError();
                        this.mainFrm.Enabled = true;
                        return;
                    }

                    if (System.IO.File.Exists(expmap.MSDialog.FileName))
                    {
                        DialogResult res = MessageBox.Show(
                            System.IO.Path.GetFileName(expmap.MSDialog.FileName) +
                            "は既に存在しています。置き換えても宜しいでしょうか？",
                            "存在確認", MessageBoxButtons.OKCancel);

                        if (res == DialogResult.Cancel) return;
                    }

                    if (expmap.MSDialog.FilterIndex > 0)
                    {
#if !doSubThread
                        ESRI.ArcGIS.Output.IExport export =
                            this.SetExportClass(filterTypes[expmap.MSDialog.FilterIndex - 1]);

                        if (export != null)
                        {
                            export.ExportFileName = expmap.MSDialog.FileName;

                            // 2010.12.8
                            // 以下を呼んでいるとメインフォームの再描画がおかしくなる
                            // 他の機能と同様別スレッドで実行できるようにするのが望ましい
                            //this.mainFrm.StartProgressBar = true;

                            //this.ExportMap(
                            //    export,
                            //    this.m_mapControl.ActiveView,
                            //    expmap.Dpi);

                            //this.mainFrm.StartProgressBar = false;

                            // 2012.08.18
                            if (this.mainFrm.tabControl1.SelectedIndex == 0)
                            {
                                this.ExportMap(
                                    export,
                                    this.mainFrm.MapControl.ActiveView,
                                    expmap.Dpi);
                            }
                            else 
                            {
                                this.ExportMap(
                                    export,
                                    this.mainFrm.axPageLayoutControl1.ActiveView,
                                    expmap.Dpi);
                            }

                            if (expmap.MSDialog.FileName.Length > 0)
                            {
                                this.lastExportPath =
                                    System.IO.Path.GetDirectoryName(expmap.MSDialog.FileName);
                            }
                        }

#else
                        if (this.mainFrm.tabControl1.SelectedIndex == 0)
                        {
                            tview = this.mainFrm.MapControl.ActiveView;
                        }
                        else
                        {
                            tview = this.mainFrm.axPageLayoutControl1.ActiveView;
                        }

                        this.mainFrm.Enabled = false;

                        backWrkr.DoWork += 
                            new System.ComponentModel.DoWorkEventHandler(backWrkr_DoWork);

                        backWrkr.RunWorkerCompleted += 
                            new RunWorkerCompletedEventHandler(backWrkr_RunWorkerCompleted);

                        backWrkr.WorkerSupportsCancellation = true;
                        backWrkr.WorkerReportsProgress = true;
                        backWrkr.RunWorkerAsync();

                        Application.DoEvents();
                        System.Threading.Thread.Sleep(100);
#endif
                    }

                }
            }
            catch (COMException comex)
            {
                dispMessage = true;
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMessage = true;
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (expmap != null) expmap.Dispose();

                if (dispMessage)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.OpenExportMapCommand_Error);
                }
            }
        }
        #endregion

        #region progressbar form
        private TrackExportMapObject trackObj = null;
        private System.Timers.Timer showProgress_timer = null;
        private System.Timers.Timer closeProgress_timer = null;
        private ESRIJapan.GISLight10.Ui.FormProgressManager frmProgress = null;

        // プログレスバーフォーム動作用BackgroundWorker
        System.ComponentModel.BackgroundWorker backWrkr =
            new System.ComponentModel.BackgroundWorker();

        /// <summary>
        /// プログレスバーフォームの非同期動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backWrkr_DoWork(object sender, DoWorkEventArgs e)
        {
            showProgress_timer = new System.Timers.Timer(500);
            showProgress_timer.Enabled = false;
            showProgress_timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);

            showProgress_timer.Enabled  = true;
            showProgress_timer.Start();
            
            trackObj =
                new TrackExportMapObject(
                    tview,
                    filterTypes[expmap.MSDialog.FilterIndex - 1],
                    expmap.Dpi,
                    expmap.MSDialog.FileName);

            trackObj.OnCreate(m_mapControl);
            trackObj.OnClick();

            while (trackObj != null &&
                  !trackObj.m_invokeHelper.ExportEnded)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
                if (frmProgress != null)
                {
                    int i = 1;
                    frmProgress.IncrementProgressBar(i++); 
                    frmProgress.TopLevel = true;
                }
            }
        }

        private void backWrkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.mainFrm.Enabled = true;
            this.mainFrm.Refresh();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // end timer
                showProgress_timer.Stop();
                showProgress_timer.Enabled = false;
                showProgress_timer.Dispose();

                closeProgress_timer = new System.Timers.Timer(1000);
                closeProgress_timer.Enabled = false;
                closeProgress_timer.Elapsed += new ElapsedEventHandler(OnCloseTimerElapsed);
                closeProgress_timer.Start();

                WindowWrapper wr =
                    new WindowWrapper
                        (System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);

                frmProgress = new ESRIJapan.GISLight10.Ui.FormProgressManager();
                frmProgress.Owner = wr;
                frmProgress.SetTitle(wr);
                frmProgress.SetMessage(Properties.Resources.InvokeExportMapHelper_RunningMessage);
                frmProgress.StartPosition = FormStartPosition.CenterScreen;
                frmProgress.SetProgressbarMaximum(50);    
                frmProgress.ShowDialog(wr);
                frmProgress.TopLevel = true;

            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// プログレスフォーム終了タイマー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseTimerElapsed(object sender, ElapsedEventArgs e)
        {
            closeProgress_timer.Stop();
            closeProgress_timer.Enabled = false;
            closeProgress_timer.Dispose();

            while (trackObj != null &&
                  !trackObj.m_invokeHelper.ExportEnded)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }

            if (frmProgress != null)
            {
                frmProgress.IncrementProgressBar(50);
                System.Threading.Thread.Sleep(300);
                frmProgress.CloseForm();
            }
        }

#endregion

#if !doSubThread
        /// <summary>
        /// Set Map Export Type
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private ESRI.ArcGIS.Output.IExport SetExportClass(string fileType)
        {
            if (fileType.Equals("JPEG"))
            {
                ESRI.ArcGIS.Output.ExportJPEGClass export =
                    new ESRI.ArcGIS.Output.ExportJPEGClass();

                ESRI.ArcGIS.Output.IExportJPEG exportFormat =
                    export as ESRI.ArcGIS.Output.IExportJPEG;

                exportFormat.Quality = 100;
                return export as ESRI.ArcGIS.Output.IExport;
            }

            if (fileType.Equals("BMP"))
            {
                ESRI.ArcGIS.Output.ExportBMPClass export =
                    new ESRI.ArcGIS.Output.ExportBMPClass();

                ESRI.ArcGIS.Output.IExportBMP exportFormat =
                    export as ESRI.ArcGIS.Output.IExportBMP;

                //exportFormat.Quality = 100;
                return export as ESRI.ArcGIS.Output.IExport;
            }

            if (fileType.Equals("PNG"))
            {
                ESRI.ArcGIS.Output.ExportPNGClass export =
                    new ESRI.ArcGIS.Output.ExportPNGClass();

                ESRI.ArcGIS.Output.IExportPNG exportFormat =
                    export as ESRI.ArcGIS.Output.IExportPNG;

                //exportFormat.Quality = 100;
                return export as ESRI.ArcGIS.Output.IExport;
            }

            if (fileType.Equals("GIF"))
            {
                ESRI.ArcGIS.Output.ExportGIFClass export =
                    new ESRI.ArcGIS.Output.ExportGIFClass();

                ESRI.ArcGIS.Output.IExportGIF exportFormat =
                    export as ESRI.ArcGIS.Output.IExportGIF;

                //exportFormat.Quality = 100;
                return export as ESRI.ArcGIS.Output.IExport;
            }

            if (fileType.Equals("PDF"))
            {
                ESRI.ArcGIS.Output.ExportPDFClass export =
                    new ESRI.ArcGIS.Output.ExportPDFClass();

                ESRI.ArcGIS.Output.IExportPDF2 exportFormat =
                    export as ESRI.ArcGIS.Output.IExportPDF2;

                // 2010-12-15 add
                SetOutputQuality(this.m_mapControl.ActiveView, 1);

                return export as ESRI.ArcGIS.Output.IExport;
            }

            return null;
        }

        // 2010-12-15 add
        /// <summary>
        /// Setting output image quality
        /// ms-help://ESRI.EDNv9.3/NET_Engine/07517e76-b1d6-41fb-90f4-bbb98a8134da.htm
        /// </summary>
        /// <param name="docActiveView"></param>
        /// <param name="iResampleRatio"></param>
        private void SetOutputQuality(IActiveView docActiveView, long iResampleRatio)
        {
            /* This function sets the OutputImageQuality for the active view. If the active view is a pagelayout, then
             * it must also set the output image quality for each of the maps in the pagelayout.
             */
            IGraphicsContainer docGraphicsContainer = null;
            IElement docElement = null;
            ESRI.ArcGIS.Display.IOutputRasterSettings docOutputRasterSettings = null;
            IMapFrame docMapFrame = null;
            IActiveView tmpActiveView = null;

            if (docActiveView is IMap)
            {
                docOutputRasterSettings = docActiveView.ScreenDisplay.DisplayTransformation
                    as ESRI.ArcGIS.Display.IOutputRasterSettings;

                docOutputRasterSettings.ResampleRatio = (int)iResampleRatio;
            }
            else if (docActiveView is IPageLayout)
            {
                //Assign ResampleRatio for PageLayout
                docOutputRasterSettings = docActiveView.ScreenDisplay.DisplayTransformation
                    as ESRI.ArcGIS.Display.IOutputRasterSettings;

                docOutputRasterSettings.ResampleRatio = (int)iResampleRatio;

                //and assign ResampleRatio to the maps in the PageLayout.
                docGraphicsContainer = docActiveView as IGraphicsContainer;
                docGraphicsContainer.Reset();

                docElement = docGraphicsContainer.Next();
                while (docElement != null)
                {
                    if (docElement is IMapFrame)
                    {
                        docMapFrame = docElement as IMapFrame;
                        tmpActiveView = docMapFrame.Map as IActiveView;
                        docOutputRasterSettings =
                            tmpActiveView.ScreenDisplay.DisplayTransformation as
                            ESRI.ArcGIS.Display.IOutputRasterSettings;
                        docOutputRasterSettings.ResampleRatio = (int)iResampleRatio;
                    }
                    docElement = docGraphicsContainer.Next();
                }
                docMapFrame = null;
                docGraphicsContainer = null;
                tmpActiveView = null;
            }
            docOutputRasterSettings = null;
        }
        // 2010-12-15 end add

        #region Create hi-resolution Map Image from ActiveView

        /// <summary>
        /// 
        /// </summary>
        /// <param name="export"></param>
        /// <param name="activeView"></param>
        /// <param name="dpi"></param>
        /// <returns></returns>
        public System.Boolean ExportMap(
            IExport export, 
            ESRI.ArcGIS.Carto.IActiveView activeView,
            int dpi)
        {
            if (export == null ||
                activeView == null ||
                dpi == 0)
            {
                return false;
            }

            ProgressDialog pd = null;
            try
            {
                Application.DoEvents();

                pd = new ProgressDialog();
                pd.Minimum = 0;
                pd.Maximum = 100;
                pd.CancelEnable = false;
                pd.Title = string.Format("{0} [{1}]", this.captionName, System.IO.Path.GetFileName(export.ExportFileName));
                pd.Show(mainFrm);

                pd.Value = 10;
                pd.Message = "エクスポート準備中・・・";

                decimal screenResolution = 96;
                decimal outputResolution = Convert.ToDecimal(dpi);
                decimal calcpix = outputResolution / screenResolution;
                export.Resolution = Convert.ToDouble(dpi);

                ESRI.ArcGIS.esriSystem.tagRECT exportRECT;
                //ESRI.ArcGIS.Display.tagRECT exportRECT;
                exportRECT.left = 0;
                exportRECT.top = 0;
                exportRECT.right = Convert.ToInt32(activeView.ExportFrame.right * calcpix);
                exportRECT.bottom = Convert.ToInt32(activeView.ExportFrame.bottom * calcpix);
                //ESRI.ArcGIS.Display.tagRECT exportRECT = activeView.ExportFrame;

                // Set up the PixelBounds envelope to match the exportRECT
                ESRI.ArcGIS.Geometry.IEnvelope envelope =
                    new ESRI.ArcGIS.Geometry.EnvelopeClass();

                envelope.PutCoords(
                    exportRECT.left, exportRECT.top, exportRECT.right, exportRECT.bottom);

                export.PixelBounds = envelope;

                if (!export.Name.Equals("PDF") && !export.Name.Equals("GIF"))
                {
                    IExportImage exportImage = export as IExportImage;
                    IWorldFileSettings exportWorld = export as IWorldFileSettings;
                    exportImage.Width = (int)exportRECT.right;
                    exportImage.Height = (int)exportRECT.bottom;
                    exportImage.ImageType = esriExportImageType.esriExportImageTypeTrueColor;
                    exportWorld.MapExtent = envelope;
                    exportWorld.OutputWorldFile = false;
                }

                pd.Value = 30;
                pd.Message = "エクスポート開始";
                Logger.Debug("Begin export.StartExporting");
                System.Int32 hDC = export.StartExporting();

                pd.Value = 60;
                pd.Message = "エクスポート中です・・・";
                Logger.Debug("Begin activeView.Output");
                // Explicit Cast and 'ref' keyword needed 
                activeView.Output(
                    hDC, (System.Int16)export.Resolution, ref exportRECT, null, null);
                
                pd.Value = 80;
                pd.Message = "エクスポート終了処理中です・・・";
                Logger.Debug("Begin export.FinishExporting");
                export.FinishExporting();

                pd.Value = 100;
                pd.Message = "エクスポート完了";
                Logger.Debug("Begin export.Cleanup");
                export.Cleanup();
                System.Threading.Thread.Sleep(500); // 完了メッセージがわかんないので

                return true;
            }
            catch 
            {
                throw;
            }
            finally
            {
                if (pd != null)
                    pd.Close();
            }

        }
        #endregion
#endif

        /// <summary>
        /// コンストラクタで指定されるウィンドウハンドルでウィンドウを表現する
        /// </summary>
        public class WindowWrapper : System.Windows.Forms.Form
        {
            /// <summary>
            /// 引数指定されたウィンドウハンドルを自クラス内に設定
            /// </summary>
            /// <param name="handle">ウィンドウハンドル</param>
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }
            private IntPtr _hwnd;
        }

    }

}
