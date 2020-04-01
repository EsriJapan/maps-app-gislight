using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

//参考
//http://forums.esri.com/Thread.asp?c=93&f=1170&t=142911
//http://forums.esri.com/Thread.asp?c=159&f=1707&t=241873

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// シンボルのビットマップイメージ出力する関連処理
    /// </summary>
    /// <history>
    ///  
    /// </history>
    static class DrawSymbol
    {
        /// <summary>
        /// 引数指定されるシンボルをビットマップイメージ出力する
        /// </summary>
        /// <param name="symbol">シンボル</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <returns>ビットマップイメージ</returns>
        public static Bitmap SymbolToBitmap(ESRI.ArcGIS.Display.ISymbol symbol,int width,int height)
        {
            IEnvelope envelope = new EnvelopeClass();
            IPoint point = new PointClass();
            IGeometry geometry = null;
            double textSymbsize = 0.0;
            double[] textSymboffset ={ 0.0, 0.0};
            string textSymbtext = "";
            ESRI.ArcGIS.Display.esriTextHorizontalAlignment textHorizontalAlignment = 0 ;
            ESRI.ArcGIS.Display.esriTextVerticalAlignment textVerticalAlignment = 0 ;

            Bitmap mBitmap = null;
            
            Graphics graphics = null;

            IntPtr bmpDC = IntPtr.Zero;

            //ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy =
            //    new ESRI.ArcGIS.esriSystem.ObjectCopyClass();
            //ISymbol symbol = objectCopy.Copy(symbol_param) as ISymbol;

            try
            {
                if (symbol is IMarkerSymbol)
                {
                    envelope.PutCoords(width / 2, height / 2, width / 2, height / 2);
                    IArea area = (IArea)envelope;
                    geometry = (IGeometry)area.Centroid;
                }
                else if (symbol is ITextSymbol)
                {
                    envelope.PutCoords(0, height / 2, 0, height / 2);
                    IPolyline polyline = new PolylineClass();
                    polyline.FromPoint = envelope.LowerLeft;
                    polyline.ToPoint = envelope.UpperRight;
                    geometry = (IGeometry)polyline;
                    ISimpleTextSymbol txtsymbol = (ISimpleTextSymbol)symbol;
                    textSymbsize = txtsymbol.Size;
                    textHorizontalAlignment = txtsymbol.HorizontalAlignment;
                    textVerticalAlignment = txtsymbol.VerticalAlignment;
                    textSymboffset[0] = txtsymbol.XOffset;
                    textSymboffset[1] = txtsymbol.YOffset;
                    textSymbtext = txtsymbol.Text;

                    txtsymbol.Size = 8;
                    //txtsymbol.Text = "";
                    txtsymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHACenter;
                    txtsymbol.VerticalAlignment = esriTextVerticalAlignment.esriTVACenter;
                    txtsymbol.XOffset = 0;
                    txtsymbol.YOffset = 0;
                }
                else if (symbol is ILineSymbol)
                {
                    //if (symbol is MultiLayerLineSymbol)
                    //{
                    //    Common.Logger.Debug("MultiLayerLineSymbol....");
                    //}

                    envelope.PutCoords(0, height / 2, width, height / 2);
                    IPolyline polyline = new PolylineClass();
                    polyline.FromPoint = envelope.LowerLeft;
                    polyline.ToPoint = envelope.UpperRight;
                    geometry = (IGeometry)polyline;
                }
                else if (symbol is IFillSymbol)
                {
                    if (symbol is SimpleFillSymbol)
                    {
                        envelope.PutCoords(5, 5, width - 5, height - 5);
                        geometry = (IGeometry)envelope;
                    }
                    else if (symbol is MultiLayerFillSymbol)
                    {
                        IMultiLayerFillSymbol multifillsymbol = (IMultiLayerFillSymbol)symbol;
                        if (multifillsymbol.get_Layer(0) is PictureFillSymbol)
                        {
                            IPictureFillSymbol pictfillsymbol = (IPictureFillSymbol)multifillsymbol.get_Layer(0);
                            mBitmap = System.Drawing.Bitmap.FromHbitmap(new IntPtr(pictfillsymbol.Picture.Handle));
                            return mBitmap;
                        }
                        else if (multifillsymbol.get_Layer(0) is SimpleFillSymbol)
                        {
                            envelope.PutCoords(5, 5, width - 5, height - 5);
                            geometry = (IGeometry)envelope;
                        }
                    }
                    else if (symbol is PictureFillSymbol)
                    {
                        IPictureFillSymbol pictfillsymbol = (IPictureFillSymbol)symbol;
                        mBitmap = System.Drawing.Bitmap.FromHbitmap(new IntPtr(pictfillsymbol.Picture.Handle));
                        return mBitmap;
                    }
                }
                else if (symbol is PictureFillSymbol)
                {
                    IPictureFillSymbol pictfillsymbol = (IPictureFillSymbol)symbol;
                    mBitmap = System.Drawing.Bitmap.FromHbitmap(new IntPtr(pictfillsymbol.Picture.Handle));
                    return mBitmap;
                }

                mBitmap = new System.Drawing.Bitmap(width, height);
                graphics = Graphics.FromImage(mBitmap);
                graphics.Clear(Color.WhiteSmoke);

                try
                {
                    
                    bmpDC = graphics.GetHdc();
                    symbol.SetupDC((int)bmpDC, null);
                    symbol.ROP2 = esriRasterOpCode.esriROPCopyPen;
                    if (geometry != null)
                    {
                        symbol.Draw(geometry);
                    }
                }
                catch (COMException comex)
                {
                    Common.UtilityClass.DoOnError(comex);
                    return null;
                }
                catch (Exception ex)
                {
                    Common.UtilityClass.DoOnError(ex);
                    return null;
                }
                //catch 
                //{
                //    return null;
                //}
                finally
                {
                    symbol.ResetDC();
                    if (bmpDC != null)
                        graphics.ReleaseHdc(bmpDC);
                        
                    graphics.Dispose();
                }

                if (symbol is ITextSymbol)
                {
                    ISimpleTextSymbol txtSymbol = (ISimpleTextSymbol)symbol;
                    txtSymbol.Size = textSymbsize;
                    txtSymbol.HorizontalAlignment = textHorizontalAlignment;
                    txtSymbol.VerticalAlignment = textVerticalAlignment;
                    txtSymbol.Text = textSymbtext;
                    txtSymbol.XOffset = textSymboffset[0];
                    txtSymbol.YOffset = textSymboffset[1];
                }
                return mBitmap;
            }
            catch
            {
                return null;
            }
            finally
            {
                //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(symbol);
                //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(graphics);

                //mBitmap.Dispose();
            }
        }

        /// <summary>
        /// 引数指定されるシンボルをイメージ出力
        /// </summary>
        /// <param name="symbol">シンボル</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <returns>イメージ</returns>
        public static Image SymbolToImage(ESRI.ArcGIS.Display.ISymbol symbol, int width, int height)
        {
            Bitmap bitmap = SymbolToBitmap(symbol, width, height);

            if (bitmap != null)
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                try
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                    Image img = Image.FromStream(stream);
                    return img;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    stream.Dispose();
                }
            }
            else 
            {
                return null;
            }

        }

        /// <summary>
        /// 幅w、高さhのImageオブジェクト作成 
        /// </summary>
        /// <param name="image">イメージ</param>
        /// <param name="w">幅</param>
        /// <param name="h">高さ</param>
        /// <returns>イメージ</returns>
        public static Image GetBitmap(Image image, int w, int h)
        {
            Bitmap canvas = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(canvas);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, w, h);

            float fw = (float)w / (float)image.Width;
            float fh = (float)h / (float)image.Height;

            float scale = Math.Min(fw, fh);
            fw = image.Width * scale;
            fh = image.Height * scale;

            g.DrawImage(image, (w - fw) / 2, (h - fh) / 2, fw, fh);
            g.Dispose();

            return canvas;
        }

    }
}
