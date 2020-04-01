using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Common.Calculator
{
    /// <summary>
    /// 面積を求めるジオメトリ演算をおこなうクラス
    /// </summary>
    public class AreaCalculatorUnit : IGeometryCalculatorUnit
    {
        #region IGeometryCalculatorUnit メンバ

        /// <summary>
        /// IAreaインタフェースを持つ図形の面積を求める
        /// </summary>
        /// <param name="pFeature">面積を求めるフィーチャ</param>
        /// <returns>面積</returns>
        public double Calculate(ESRI.ArcGIS.Geodatabase.IFeature pFeature)
        {
            IArea pArea;

            pArea = (IArea)pFeature.Shape as IArea;
            if (pArea == null)
            {
                throw new NotSupportedException("サポートしていない図形タイプです");
            }

            return pArea.Area;
        }

        #endregion
    }
}
