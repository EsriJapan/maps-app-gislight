using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;

using ESRIJapan.GISLight10.Ui;
using ESRIJapan.GISLight10.Common.Calculator;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 面積、長さを表す列挙
    /// </summary>
    public enum Unit
    {
        /// <summary>
        /// 不明の単位
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// メートル
        /// </summary>
        METER,

        /// <summary>
        /// キロメートル
        /// </summary>
        KMETER,

        /// <summary>
        /// センチメートル
        /// </summary>
        CMETER,

        /// <summary>
        /// アール
        /// </summary>
        A,

        /// <summary>
        /// ヘクタール
        /// </summary>
        HA,

        /// <summary>
        /// 平方メートル
        /// </summary>
        METER2,

        /// <summary>
        /// 平方キロメートル
        /// </summary>
        KMETER2
    };
    
    /// <summary>
    /// ジオメトリ演算の基本的な共通のインタフェース
    /// </summary>
    public interface IGeometryCalculator
    {
        /// <summary>
        /// ジオメトリ演算をおこなうレイヤ
        /// </summary>
        IFeatureLayer TargetLayer { set; }

        /// <summary>
        /// ジオメトリ演算結果を格納するフィールドの名前
        /// </summary>
        string TargetFieldName { set; }

        /// <summary>
        /// ジオメトリ演算結果の単位
        /// </summary>
        Unit ToUnit { set; }

        /// <summary>
        /// 小数点以下の桁数
        /// </summary>
        int DecimalPrecision { set; }

        /// <summary>
        /// 単位文字列を結果に付けるかの有無
        /// </summary>
        bool AppendUnit { set; }

        /// <summary>
        /// ジオメトリ演算を実行する
        /// </summary>
        /// <param name="processUnit">ジオメトリ演算処理</param>
        /// <returns>エラー発生時のエラーメッセージ</returns>
        string Calculate(IGeometryCalculatorUnit processUnit);


    }
}
