using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ページレイアウトへのピクチャの取り込み
    /// </summary>
    /// <history>
    ///  2011-04-12 新規作成 
    /// </history>
    public sealed class InsertPictureCommand : BaseCommand
    {
        private Ui.MainForm mainForm;
        private IHookHelper m_hookHelper = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InsertPictureCommand()
        {
            base.m_caption = "ピクチャ";
            base.m_category = "CustomCommands";
            base.m_toolTip = "ピクチャ";
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// インスタンス生成時イベントハンドラ
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;

                if (m_hookHelper.ActiveView == null)
                {
                    m_hookHelper = null;
                }
                else
                {
                    IToolbarControl2 tlb = (IToolbarControl2)m_hookHelper.Hook;
                    IntPtr ptr = (System.IntPtr)tlb.hWnd;
                    System.Windows.Forms.Control cntrl = System.Windows.Forms.Control.FromHandle(ptr);
                    this.mainForm = (Ui.MainForm)cntrl.FindForm();
                }
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

        }

        private ESRI.ArcGIS.Display.IPictureMarkerSymbol CreatePictureMarkerSymbol(
            ESRI.ArcGIS.Display.esriIPictureType pictureType,
            System.String filename, System.Double markerSize)
        {

            // This example creates two PictureMarkerSymbols
            // One from an .EMF file and another from a .BMP file.

            if (pictureType != esriIPictureType.esriIPictureBitmap &&
                pictureType != esriIPictureType.esriIPictureEMF)
            {
                return null;
            }

            // Set the Transparent background color for the Picture Marker symbol to white.
            ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
            rgbColor.Red = 255;
            rgbColor.Green = 255;
            rgbColor.Blue = 255;

            // Create the Marker and assign properties.
            ESRI.ArcGIS.Display.IPictureMarkerSymbol pictureMarkerSymbol =
                new ESRI.ArcGIS.Display.PictureMarkerSymbolClass();
            pictureMarkerSymbol.CreateMarkerSymbolFromFile(pictureType, filename);
            pictureMarkerSymbol.Angle = 0;
            pictureMarkerSymbol.BitmapTransparencyColor = rgbColor;
            pictureMarkerSymbol.Size = markerSize;
            pictureMarkerSymbol.XOffset = 0;
            pictureMarkerSymbol.YOffset = 0;

            return pictureMarkerSymbol;
        }

        /// <summary>
        /// ピクチュアーエレメント数の取得
        /// </summary>
        /// <returns></returns>
        private int GetPictureElementCount()
        {
            int picElmCnt = 0;

            IPageLayout pl = this.mainForm.axPageLayoutControl1.PageLayout;
            IGraphicsContainer graphicsContainer = (IGraphicsContainer)pl;
            graphicsContainer.Reset();
            IElement element = graphicsContainer.Next();

            while (element != null)
            {
                if (element is IPictureElement)
                {
                    picElmCnt++;
                }
                element = graphicsContainer.Next();
            }

            return picElmCnt;
        }

        private const double CENTIMETER_FACTOR = 0.3937007874;
        private const double METER_FACTOR = 39.37007874;
        private const double KILOMETER_FACTOR = 3937.007874;
        private const double INCHE_FACTOR = 1.0;
        private const double DECIMAL_DEGREE_FACTOR = 40030174.00 / 360.00;
        private double targetFactor = 0.0;

        private int getMapScale(double unitsperpixel, double mapdpi)
        {
            ESRI.ArcGIS.esriSystem.esriUnits pageUnit = this.mainForm.axPageLayoutControl1.ActiveView.FocusMap.MapUnits;
            switch (pageUnit)
            {
                case ESRI.ArcGIS.esriSystem.esriUnits.esriCentimeters:
                    targetFactor = CENTIMETER_FACTOR;
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriMeters:
                    targetFactor = METER_FACTOR;
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers:
                    targetFactor = KILOMETER_FACTOR;
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriInches:
                    targetFactor = INCHE_FACTOR;
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriDecimalDegrees:
                default:
                    targetFactor = DECIMAL_DEGREE_FACTOR * METER_FACTOR;
                    break;
            }

            double unitsperinch = unitsperpixel * mapdpi * targetFactor;
            int calscale = (int)System.Math.Round(unitsperinch);

            return calscale; // (int)System.Math.Round(factor * unitsperinch);
        }

        /// <summary>
        /// クリック時イベントハンドラ
        /// 指定されたファイルタイプの画像をページレイアウトに取り込む
        /// </summary>
        public override void OnClick()
        {
            IPictureElement3 pictureElement = null;
            ESRI.ArcGIS.Carto.IElement element = null;
            try
            {
                if (mainForm == null) return;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = "";
                ofd.InitialDirectory = "";

                ofd.Filter =
                    "サポートするすべての画像フォーマット " +
                    "(*.bmp,*.gif,*.jpg,*.png|" +
                     "*.bmp;*.gif;*.jpg;*.png|" +
                    "BMP (*.bmp)|*.bmp|" +
                    "GIF (*.gif)|*.gif|" +
                    "JPEG (*.jpg)|*.jpg|" +
                    "PNG (*.png)|*.png";

                ofd.FilterIndex = 0;
                ofd.Title = "開く";
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                if (ofd.ShowDialog(this.mainForm) == DialogResult.OK)
                {
                    string path = System.IO.Path.GetFileName(ofd.FileName);
                    if (path != null)
                    {
                        pictureElement = GetPictureElement(path);
                        if (pictureElement == null)
                        {
                            GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                                Properties.Resources.InsertPictureCommand_ErrorWhenInsertPicture);

                            GISLight10.Common.Logger.Error("ピクチャエレメントの取得に失敗");
                            return;
                        }

                        GISLight10.Common.Logger.Debug("UsingMemorySize Before GC :" +
                           GC.GetTotalMemory(false).ToString() + " byte");

                        GC.Collect();

                        GISLight10.Common.Logger.Debug("UsingMemorySize After  GC :" +
                           GC.GetTotalMemory(false).ToString() + " byte");

                        pictureElement.ImportPictureFromFile(ofd.FileName);
                        //pictureElement.MaintainAspectRatio = false;

                        if (GC.GetTotalMemory(false) >= Properties.Settings.Default.LIMIT_GC_SIZE)
                        {
                            GISLight10.Common.Logger.Debug("Check UsingMemorySize GC :" +
                               GC.GetTotalMemory(false).ToString() + " byte");

                            GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            Properties.Resources.InsertPictureCommandWarningMessage);
                            
                            return;
                        }

                        IActiveView pAV = this.mainForm.axPageLayoutControl1.ActiveView;
                        ESRI.ArcGIS.Geometry.IEnvelope pEnv = pAV.Extent;
                        

                        double widthPoint = 0.0, heightPoint = 0.0;
                        pictureElement.QueryIntrinsicSize(ref widthPoint, ref heightPoint);

                        int limitWidth = Properties.Settings.Default.MAX_WIDTH_POINTS;
                        int limitHeight = Properties.Settings.Default.MAX_HEIGHT_POINTS;

                        System.Drawing.Bitmap img = null;
                        double imgH = 0.0;
                        double imgW = 0.0;
                        int limitH = 0;
                        int limitW = 0;

                        if (path.ToLower().LastIndexOf(".bmp") > 0 ||
                            path.ToLower().LastIndexOf(".png") > 0 ||
                            path.ToLower().LastIndexOf(".jpg") > 0 ||
                            path.ToLower().LastIndexOf(".gif") > 0)
                        {
                            try
                            {
                                img = new Bitmap(ofd.FileName);
                                limitH = img.Height;
                                limitW = img.Width;

                                imgH = img.Height / (int)img.HorizontalResolution;
                                imgW = img.Width / (int)img.VerticalResolution;
                            }
                            finally
                            {
                                img.Dispose();
                            }

                            if ((limitW >= limitWidth) || (limitH >= limitHeight))
                            {
                                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                                Properties.Resources.InsertPictureCommandWarningMessage);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }

                        if (GetPictureElementCount() >= Properties.Settings.Default.MAX_PIC_ELMENT)
                        {
                            GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            Properties.Resources.InsertPictureCommandWarningMessagePicureElementIsMax + 
                            "\n 制限数は[" + Properties.Settings.Default.MAX_PIC_ELMENT.ToString() + "]です。");

                            return;
                        }

                        double pageWidth;
                        double pageHeight;
                        this.mainForm.axPageLayoutControl1.Page.QuerySize(
                            out pageWidth, out pageHeight);

                        int centerWidth = (int)(pageWidth / 2);
                        int centerHeight = (int)(pageHeight / 2);

                        ESRI.ArcGIS.Geometry.IEnvelope pEnvTmp = new EnvelopeClass();
                        pEnvTmp.PutCoords(centerWidth / 2, centerHeight / 2, centerWidth, centerHeight);
                        pEnvTmp.Width = imgW;
                        pEnvTmp.Height = imgH;
                        
                        element = pictureElement as IElement;
                        element.Geometry = pEnvTmp;

                        IGraphicsContainer gcntnr = pAV as IGraphicsContainer;
                        gcntnr.AddElement(element, 0);

                        this.mainForm.axPageLayoutControl1.ActiveView.PartialRefresh(
                            esriViewDrawPhase.esriViewGraphics, null, null);


                        GISLight10.Common.Logger.Debug("UsingMemorySize After AddElement :" +
                            GC.GetTotalMemory(false).ToString() + " byte");

                    }
                }
            }
            catch (COMException comex)
            {
                GISLight10.Common.Logger.Debug("UsingMemorySize GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    Properties.Resources.InsertPictureCommand_ErrorWhenInsertPicture);

                GISLight10.Common.Logger.Error(
                    Properties.Resources.InsertPictureCommand_ErrorWhenInsertPicture);

                GISLight10.Common.Logger.Error(comex.Message + " " + comex.StackTrace);
            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Debug("UsingMemorySize GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                GISLight10.Common.Logger.Error(
                    Properties.Resources.InsertPictureCommand_ErrorWhenInsertPicture);

                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
            finally
            {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pictureElement);
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(element);
                
                GC.Collect();
            }
        }

        /// <summary>
        /// 引数指定されたファイルタイプに対応したグラフィックエレメントを返す
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private IPictureElement3 GetPictureElement(string fileType)
        {
            IPictureElement3 pictureElement = null;

            if (fileType.ToLower().LastIndexOf(".bmp") > 0)
            {
                pictureElement = new BmpPictureElementClass();
            }
            else if (fileType.ToLower().LastIndexOf(".emf") > 0)
            {
                pictureElement = new EmfPictureElementClass();
            }
            else if (fileType.ToLower().LastIndexOf(".gif") > 0)
            {
                pictureElement = new GifPictureElementClass();
            }
            else if (fileType.ToLower().LastIndexOf(".jpg") > 0)
            {
                pictureElement = new JpgPictureElementClass();
            }
            else if (fileType.ToLower().LastIndexOf(".png") > 0)
            {
                pictureElement = new PngPictureElementClass();
            }
            else if (fileType.ToLower().LastIndexOf(".tif") > 0)
            {
                pictureElement = new TifPictureElementClass();
            }
            else if (fileType.ToLower().LastIndexOf(".jp2") > 0)
            {
                pictureElement = new Jp2PictureElementClass();
            }

            return pictureElement;
        }

        #endregion
    }
}
