#define doNotShowfrmProgress

using System;
using System.Windows.Forms;
using System.Timers;
using System.Runtime.InteropServices;
using System.ComponentModel;

using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRIJapan.GISLight10.Common;
using ESRIJapan.GISLight10.Ui;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace ESRIJapan.GISLight10.EngineCommand
{
    // A helper method used to delegate calls to the main thread.
    /// <summary>
    /// マップエクスポート処理の起動を行う
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成 
    ///  2011/04/06 ページレイアウトがアクティブな場合にはページレイアウトを出力
    /// </history>
    public sealed class InvokeExportMapHelper : Control
    {
        private string m_message = string.Empty;
        
        /// <summary>
        /// マップエクスポート実行終了時メッセージ
        /// </summary>
        public string ExportMapEndMessage
        {
            get { return this.m_message; }
        }

        private bool m_exportEnded = false;

        /// <summary>
        /// マップエクスポートの実行終了判定フラグ
        /// </summary>
        public bool ExportEnded
        {
            get { return this.m_exportEnded; }
        }

        /// <summary>
        /// マップエクスポート起動の為のデリゲート宣言
        /// </summary>
        /// <param name="navigationData">マップエクスポート時のパラメータ</param>
        /// <returns>bool</returns>
        private delegate bool MessageHandler(NavigationData navigationData);

        //Class members.
        private IActiveView m_activeView;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="activeView">マップコントロールのアクティブビュー</param>
        public InvokeExportMapHelper(IActiveView activeView)
        {
            //Make sure that the control was created and that it has a valid handle.
            //this.CreateHandle();
            this.CreateControl();

            //Get the active view.
            m_activeView = activeView;
        }

        /// <summary>
        /// マップエクスポート実行メソッド起動
        /// </summary>
        /// <param name="navigationData">マップエクスポート時のパラメータ</param>
        public void InvokeMethod(NavigationData navigationData)
        {
            // Invoke HandleMessage through its delegate.
            if (!this.IsDisposed && this.IsHandleCreated)
                Invoke(
                    new MessageHandler(ExportMap), 
                    new object[] { navigationData });
        }

        /// <summary>
        /// マップエクスポート実行本体
        /// </summary>
        /// <param name="navigationData">マップエクスポート時のパラメータ</param>
        /// <returns>マップエクスポート実行結果</returns>
        public bool ExportMap(NavigationData navigationData)
        {
            ESRI.ArcGIS.Carto.IActiveView activeView = m_activeView;
            int dpi = navigationData.Dpi;
            string filetype = navigationData.fileType;
            IExport export = SetExportClass(filetype);

            if (export == null ||
                activeView == null ||
                dpi == 0)
            {
                this.m_exportEnded = true;
                this.m_message = Properties.Resources.InvokeExportMapHelper_ParameterError; 
                // "エクスポートマップ：パラメータが不正です。";
                
                return false;
            }

            export.ExportFileName = navigationData.FileName;

            decimal screenResolution = 96;
            decimal outputResolution = Convert.ToDecimal(dpi);
            decimal calcpix = outputResolution / screenResolution;
            export.Resolution = Convert.ToDouble(dpi);

            //ESRI.ArcGIS.Display.tagRECT exportRECT;
            ESRI.ArcGIS.esriSystem.tagRECT exportRECT;
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

            Logger.Debug("Begin export.StartExporting");
            System.Int32 hDC = export.StartExporting();

            Logger.Debug("Begin activeView.Output");
            // Explicit Cast and 'ref' keyword needed 
            activeView.Output(
                hDC, (System.Int16)export.Resolution, ref exportRECT, null, null);

            Logger.Debug("Begin export.FinishExporting");
            export.FinishExporting();

            Logger.Debug("Begin export.Cleanup");
            export.Cleanup();

            this.m_message = Properties.Resources.InvokeExportMapHelper_EndedNormal;
            // "エクスポートマップ：正常終了しました。";
            
            this.m_exportEnded = true;
            return true;
        }

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
                SetOutputQuality(m_activeView, 1);

                return export as ESRI.ArcGIS.Output.IExport;
            }

            return null;
        }

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
    

        /// <summary>
        /// Control initialization.
        /// </summary>
        private void InitializeComponent() { }

    }

    /// <summary>
    /// マップエクスポート時のパラメータ
    /// </summary>
    public struct NavigationData
    {
        /// <summary>
        /// ファイルタイプ
        /// </summary>
        public string fileType;

        /// <summary>
        ///  DPI
        /// </summary>
        public int Dpi;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName;

        /// <summary>
        /// マップエクスポートパラメータ構造体の
        /// コンストラクタ
        /// </summary>
        /// <param name="filetype">ファイルタイプ</param>
        /// <param name="dpi">DPI</param>
        /// <param name="filename">ファイル名</param>
        public NavigationData(string filetype, int dpi, string filename)
        {
            fileType = filetype;
            Dpi = dpi;
            FileName = filename;
        }
    }

    // This command triggers the tracking functionality.
    /// <summary>
    /// マップエクスポート起動指定コマンド
    /// </summary>
    public sealed class TrackExportMapObject : BaseCommand
    {
        //Class members.
        private IActiveView targetView = null;
        //private ESRIJapan.GISLight10.Ui.MainForm mainFrm;

        private IHookHelper m_hookHelper = null;
        /// <summary>
        /// マップエクスポート起動ヘルパー
        /// </summary>
        public InvokeExportMapHelper m_invokeHelper = null;
        private System.Timers.Timer m_timer = null;

        private string m_fileType = null;
        private int m_dpi;
        private string m_fileName = null;

#if ShowfrmProgress
        private ESRIJapan.GISLight10.Ui.FormProgressManager frmProgress = null;
#endif

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetView">処理対象ビューインデクス</param>
        /// <param name="fileType">ファイルタイプ</param>
        /// <param name="dpi">DPI</param>
        /// <param name="filename">ファイル名</param>
        public TrackExportMapObject(
            IActiveView targetView, string fileType, int dpi, string filename)
        {
            this.targetView = targetView;
            this.m_fileType = fileType;
            this.m_dpi = dpi;
            this.m_fileName = filename;
        }

        /// <summary>
        /// クリエイト時処理
        /// マップエクスポート起動開始タイマー設定
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_hookHelper = new HookHelperClass();
            m_hookHelper.Hook = hook;
            
            m_invokeHelper = new InvokeExportMapHelper(this.targetView);

            // Instantiate the timer.
            m_timer = new System.Timers.Timer(3000);
            m_timer.Enabled = false;
            
            // Set the timer's elapsed event handler.
            m_timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);

        }

        /// <summary>
        /// クリック時処理
        /// </summary>
        public override void OnClick()
        {
            // Start the timer.
            m_timer.Enabled = true;

#if ShowfrmProgress
            frmProgress.Owner = this.mainFrm;
            frmProgress.SetTitle(this.mainFrm);
            frmProgress.SetMessage(Properties.Resources.InvokeExportMapHelper_RunningMessage);
            frmProgress.Location = mainFrm.Location;
            frmProgress.ShowDialog(this.mainFrm);
#endif

        }

        /// <summary>
        /// マップエクスポート起動開始タイマーハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // end timer
                m_timer.Stop();
                m_timer.Enabled = false;
                m_timer.Dispose();

                // set parameters
                NavigationData navigationData =
                    new NavigationData(m_fileType, m_dpi, m_fileName);

                // invoke export map
                m_invokeHelper.InvokeMethod(navigationData);

                // wait until export map ended
                while (!m_invokeHelper.ExportEnded)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
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
#if ShowfrmProgress
                if (frmProgress != null)
                {
                    frmProgress.CloseForm();
                }
#endif
                ESRIJapan.GISLight10.Common.Logger.Info(m_invokeHelper.ExportMapEndMessage);
            }
        }
    }
}
