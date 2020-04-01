using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;


namespace ESRIJapan.GISLight10.Common.Calculator
{
    /// <summary>
    /// 周長、長さを求めるジオメトリ演算をおこなうクラス
    /// </summary>
    public class LengthCalculatorUnit : IGeometryCalculatorUnit
    {
        #region IGeometryCalculatorUnit メンバ

        /// <summary>
        /// ポリゴンの場合は周長、ラインの場合には長さを求める
        /// </summary>
        /// <param name="pFeature">周長または長さを求めるフィーチャ</param>
        /// <returns>周長または長さ</returns>
        public double Calculate(ESRI.ArcGIS.Geodatabase.IFeature pFeature)
        {
            double val = 0d;

            if (pFeature.Shape is IPolygon)
            {
                val = getPolygonLength(pFeature.Shape);
            }
            else if (pFeature.Shape is IPolyline)
            {
                val = getPolylineLength(pFeature.Shape);
            }
            else
            {
                throw new NotSupportedException("サポートしていない図形タイプです");
            }

            return val;
        }

        #endregion


        /// <summary>
        /// ポリゴンの周長を求める
        /// </summary>
        /// <param name="pGeometry">周長を求める図形（ポリゴン）</param>
        /// <returns>周長</returns>
        protected double getPolygonLength(IGeometry pGeometry)
        {
            IPolygon pPolygon;

            pPolygon = (IPolygon)pGeometry;

            return pPolygon.Length;
        }


        /// <summary>
        /// ラインの長さを求める
        /// </summary>
        /// <param name="pGeometry">長さを求める図形（ライン）</param>
        /// <returns>長さ</returns>
        protected double getPolylineLength(IGeometry pGeometry)
        {
            IPolyline pPolyline;

            pPolyline = (IPolyline)pGeometry;

            return pPolyline.Length;
        }
    }
}
