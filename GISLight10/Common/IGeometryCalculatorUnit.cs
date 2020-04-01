using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;

namespace ESRIJapan.GISLight10.Common.Calculator
{
    /// <summary>
    /// ジオメトリ演算の演算処理のインタフェース
    /// </summary>
    public interface IGeometryCalculatorUnit
    {
        /// <summary>
        /// 1フィーチャに対してジオメトリ演算をおこなう
        /// </summary>
        /// <param name="pFeature">ジオメトリ演算をおこなうフィーチャ</param>
        /// <returns>ジオメトリ演算結果</returns>
        double Calculate(IFeature pFeature);
    }
}
