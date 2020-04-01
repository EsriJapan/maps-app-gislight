using stdole;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// シンボルのビットマップイメージを出力する
    /// WindowsAPI使用版
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 (ejxxxx)
    /// </history>
	/// <remarks>http://kiwigis.blogspot.jp/2009/05/accessing-esri-style-files-using-adonet_16.html</remarks>
    public static class WindowsAPI
    {
        private const int COLORONCOLOR = 3;
        private const int HORZSIZE = 4;
        private const int VERTSIZE = 6;
        private const int HORZRES = 8;
        private const int VERTRES = 10;
        private const int ASPECTX = 40;
        private const int ASPECTY = 42;
        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;

        private enum PictureTypeConstants
        {
            picTypeNone = 0,
            picTypeBitmap = 1,
            picTypeMetafile = 2,
            picTypeIcon = 3,
            picTypeEMetafile = 4
        }
        private struct PICTDESC
        {
            /// <summary>
            /// The size of the structure, in bytes
            /// </summary>
            public int cbSizeOfStruct;
            /// <summary>
            /// Type of picture described by this structure
            /// </summary>
            public int picType;
            /// <summary>
            /// The HBITMAP handle identifying the bitmap assigned to the picture object
            /// </summary>
            public IntPtr hPic;
            /// <summary>
            /// a handle to the palette of a picture in a Picture object
            /// </summary>
            public IntPtr hpal;
            /// <summary>
            ///  unused
            /// </summary>
            public int _pad;
        }
        private struct RECT
        {
            /// <summary>
            /// The x-coordinate of the upper-left corner of the rectangle
            /// </summary>
            public int Left;
            /// <summary>
            /// The y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Top;
            /// <summary>
            /// The x-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Right;
            /// <summary>
            /// The y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Bottom;
        }

		[DllImport("olepro32.dll", EntryPoint = "OleCreatePictureIndirect", PreserveSig = false)]
        private static extern int OleCreatePictureIndirect(
            ref PICTDESC pPictDesc, ref Guid riid, bool fOwn, out IPictureDisp ppvObj);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hObject, int width, int height);

        [DllImport("user32.dll", EntryPoint = "GetDC", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32", EntryPoint = "CreateSolidBrush", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("user32", EntryPoint = "FillRect", ExactSpelling = true, SetLastError = true)]
        private static extern int FillRect(IntPtr hdc, ref RECT lpRect, IntPtr hBrush);

        [DllImport("GDI32.dll", EntryPoint = "GetDeviceCaps", ExactSpelling = true, SetLastError = true)]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32", EntryPoint = "GetClientRect", ExactSpelling = true, SetLastError = true)]
        private static extern int GetClientRect(IntPtr hwnd, ref RECT lpRect);

 
        private static IPictureDisp CreatePictureFromSymbol(IntPtr hDCOld, ref IntPtr hBmpNew,
            ISymbol pSymbol, Size size, int lGap, int backColor)
        {
            IntPtr hDCNew = IntPtr.Zero;
            IntPtr hBmpOld = IntPtr.Zero;
            try
            {
                hDCNew = CreateCompatibleDC(hDCOld);
                hBmpNew = CreateCompatibleBitmap(hDCOld, size.Width, size.Height);
                hBmpOld = SelectObject(hDCNew, hBmpNew);

                // Draw the symbol to the new device context.
                bool lResult = DrawToDC(hDCNew, size, pSymbol, lGap, backColor);

                hBmpNew = SelectObject(hDCNew, hBmpOld);
                DeleteDC(hDCNew);

                // Return the Bitmap as an OLE Picture.
                return CreatePictureFromBitmap(hBmpNew);
            }
            catch (Exception error)
            {
                if (pSymbol != null)
                {
                    pSymbol.ResetDC();
                    if ((hBmpNew != IntPtr.Zero) && (hDCNew != IntPtr.Zero) && (hBmpOld != IntPtr.Zero))
                    {
                        hBmpNew = SelectObject(hDCNew, hBmpOld);
                        DeleteDC(hDCNew);
                    }
                }

                throw error;
            }
        }

        private static IPictureDisp CreatePictureFromBitmap(IntPtr hBmpNew)
        {
            try
            {
                Guid iidIPicture = new Guid("7BF80980-BF32-101A-8BBB-00AA00300CAB");

                PICTDESC picDesc = new PICTDESC();
                picDesc.cbSizeOfStruct = Marshal.SizeOf(picDesc);
                picDesc.picType = (int)PictureTypeConstants.picTypeBitmap;
                picDesc.hPic = (IntPtr)hBmpNew;
                picDesc.hpal = IntPtr.Zero;

                // Create Picture object.
                IPictureDisp newPic;
                OleCreatePictureIndirect(ref picDesc, ref iidIPicture, true, out newPic);

                // Return the new Picture object.
                return newPic;
            }
            catch (Exception error)
            {
                throw error;
            }
        }
        private static bool DrawToWnd(IntPtr hWnd, ISymbol pSymbol, int lGap, int backColor)
        {
            IntPtr hDC = IntPtr.Zero;
            try
            {
                if (hWnd != IntPtr.Zero)
                {
                    // Calculate size of window.
                    RECT udtRect = new RECT();
                    int lResult = GetClientRect(hWnd, ref udtRect);

                    if (lResult != 0)
                    {
                        int lWidth = (udtRect.Right - udtRect.Left);
                        int lHeight = (udtRect.Bottom - udtRect.Top);

                        hDC = GetDC(hWnd);
                        // Must release the DC afterwards.
                        if (hDC != IntPtr.Zero)
                        {
                            bool ok = DrawToDC(hDC, new Size(lWidth, lHeight), pSymbol, lGap, backColor);

                            // Release cached DC obtained with GetDC.
                            ReleaseDC(hWnd, hDC);

                            return ok;
                        }
                    }
                }
            }
            catch
            {
                if (pSymbol != null)
                {
                    // Try resetting DC, in case we have already called SetupDC for this symbol.
                    pSymbol.ResetDC();

                    if ((hWnd != IntPtr.Zero) && (hDC != IntPtr.Zero))
                    {
                        ReleaseDC(hWnd, hDC); // Try to release cached DC obtained with GetDC.
                    }
                }
                return false;
            }
            return true;
        }

        private static bool DrawToDC(IntPtr hDC, Size size, ISymbol pSymbol, int lGap, int backColor)
        {
            try
            {
                if (hDC != IntPtr.Zero)
                {
                    // First clear the existing device context.
                    if (!Clear(hDC, backColor, 0, 0, size.Width, size.Height))
                    {
                        throw new Exception("Could not clear the Device Context.");
                    }

                    // Create the Transformation and Geometry required by ISymbol::Draw.
                    ITransformation pTransformation = CreateTransFromDC(hDC, size.Width, size.Height);
                    IEnvelope pEnvelope = new EnvelopeClass();

                    //pEnvelope.PutCoords(lGap, lGap, size.Width - lGap, size.Height - lGap);
                    IGeometry pGeom = null; // CreateSymShape(pSymbol, pEnvelope);

                    //
                    if (pSymbol is ILineSymbol)
                    {
                        //Common.Logger.Debug("Check ILineSymbol....");
                        // ラインシンボルの場合に下記にしないと斜めになる
                        pEnvelope.PutCoords(0, size.Height / 2, size.Width, size.Height / 2);
                        IPolyline polyline = new PolylineClass();
                        polyline.FromPoint = pEnvelope.LowerLeft;
                        polyline.ToPoint = pEnvelope.UpperRight;
                        pGeom = (IGeometry)polyline;
                    }
                    else
                    {
                        pEnvelope.PutCoords(lGap, lGap, size.Width - lGap, size.Height - lGap);
                        pGeom = CreateSymShape(pSymbol, pEnvelope);
                    }
                    //

                    // Perform the Draw operation.
                    if ((pTransformation != null) && (pGeom != null))
                    {
                        pSymbol.SetupDC(hDC.ToInt32(), pTransformation);
                        pSymbol.Draw(pGeom);
                        pSymbol.ResetDC();
                    }
                    else
                    {
                        throw new Exception("Could not create required Transformation or Geometry.");
                    }
                }
            }
            catch
            {
                if (pSymbol != null)
                {
                    pSymbol.ResetDC();
                }
                return false;
            }

            return true;
        }

        private static bool Clear(IntPtr hDC, int backgroundColor, int xmin, int ymin, int xmax, int ymax)
        {
            // This function fill the passed in device context with a solid brush,
            // based on the OLE color passed in.
            IntPtr hBrushBackground = IntPtr.Zero;
            int lResult;
            bool ok;

            try
            {
                RECT udtBounds;
                udtBounds.Left = xmin;
                udtBounds.Top = ymin;
                udtBounds.Right = xmax;
                udtBounds.Bottom = ymax;

                hBrushBackground = CreateSolidBrush(backgroundColor);
                if (hBrushBackground == IntPtr.Zero)
                {
                    throw new Exception("Could not create GDI Brush.");
                }
                lResult = FillRect(hDC, ref udtBounds, hBrushBackground);
                if (hBrushBackground == IntPtr.Zero)
                {
                    throw new Exception("Could not fill Device Context.");
                }
                ok = DeleteObject(hBrushBackground);
                if (hBrushBackground == IntPtr.Zero)
                {
                    throw new Exception("Could not delete GDI Brush.");
                }
            }
            catch
            {
                if (hBrushBackground != IntPtr.Zero)
                {
                    ok = DeleteObject(hBrushBackground);
                }
                return false;
            }

            return true;
        }

        private static ITransformation CreateTransFromDC(IntPtr hDC, int lWidth, int lHeight)
        {
            // Calculate the parameters for the new transformation,
            // based on the dimensions passed to this function.
            try
            {
                IEnvelope pBoundsEnvelope = new EnvelopeClass();
                pBoundsEnvelope.PutCoords(0.0, 0.0, (double)lWidth, (double)lHeight);

                tagRECT deviceRect;
                deviceRect.left = 0;
                deviceRect.top = 0;
                deviceRect.right = lWidth;
                deviceRect.bottom = lHeight;

                int dpi = GetDeviceCaps(hDC, LOGPIXELSY);
                if (dpi == 0)
                {
                    throw new Exception("Could not retrieve Resolution from device context.");
                }

                // Create a new display transformation and set its properties.
                IDisplayTransformation newTrans = new DisplayTransformationClass();
                newTrans.VisibleBounds = pBoundsEnvelope;
                newTrans.Bounds = pBoundsEnvelope;
                newTrans.set_DeviceFrame(ref deviceRect);
                newTrans.Resolution = dpi;

                return newTrans;
            }
            catch
            {
                return null;
            }
        }

        private static IGeometry CreateSymShape(ISymbol pSymbol, IEnvelope pEnvelope)
        {
            // This function returns an appropriate Geometry type depending on the
            // Symbol type passed in.
            lock (pSymbol)
            {
                try
                {
                    if (pSymbol is IMarkerSymbol)
                    {
                        // For a MarkerSymbol return a Point.
                        IArea pArea = (IArea)pEnvelope;
                        return pArea.Centroid;
                    }
                    else if ((pSymbol is ILineSymbol) || (pSymbol is ITextSymbol))
                    {
                        // For a LineSymbol or TextSymbol return a Polyline.
                        IPolyline pPolyline = new PolylineClass();
                        pPolyline.FromPoint = pEnvelope.LowerLeft;
                        pPolyline.ToPoint = pEnvelope.UpperRight;
                        return pPolyline;
                    }
                    else
                    {
                        // For any FillSymbol return an Envelope.
                        return pEnvelope;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// シンボルのビットマップイメージ出力
        /// </summary>
        /// <param name="userSymbol">シンボル</param>
        /// <param name="size">サイズ</param>
        /// <param name="gr">表示対象フォームのGraphicsオブジェクト</param>
        /// <param name="backColor">背景色</param>
        /// <returns>ビットマップイメージ</returns>
        public static Bitmap SymbolToBitmap(
            ISymbol userSymbol, 
            Size size, 
            Graphics gr,
            int backColor)
        {
            lock (gr)
            {
                IntPtr graphicsHdc = gr.GetHdc();
                IntPtr hBitmap = IntPtr.Zero;
                IPictureDisp newPic = CreatePictureFromSymbol(
                    graphicsHdc, ref hBitmap, userSymbol, size, 1, backColor);
                Bitmap newBitmap = Bitmap.FromHbitmap(hBitmap);
                gr.ReleaseHdc(graphicsHdc);

                return newBitmap;
            }
        }
    }

}
