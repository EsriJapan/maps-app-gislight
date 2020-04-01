using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;


namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 図形チェッククラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public class GeometryChecker
    {
        /// <summary>
        /// 不正な図形を修正する
        /// </summary>
        /// <param name="pGeometry">修正する図形</param>
        public static void Simplify4(IGeometry pGeometry)
        {
            if (pGeometry == null)
            {
                return;
            }

            pGeometry.SnapToSpatialReference();
            if (pGeometry is IPolyline4)
            {
                IPolyline4 pPolyline;
                pPolyline = (IPolyline4)pGeometry;
                pPolyline.SimplifyEx(false);
                if (Math.Abs(pPolyline.Length) < Double.Epsilon)
                {
                    // ポリラインの長さが限りなく０の場合
                    pGeometry.SetEmpty();
                }
            }
            else if (pGeometry is ITopologicalOperator2)
            {
                ITopologicalOperator2 pTopologicalOperator;
                pTopologicalOperator = (ITopologicalOperator2)pGeometry;
                pTopologicalOperator.IsKnownSimple_2 = false;
                pTopologicalOperator.Simplify();
            }
        }
    }
}
