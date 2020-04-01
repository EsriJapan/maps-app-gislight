using System;
using System.Collections.Generic;
using System.Text;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// ジオメトリ演算の設定を管理するクラス
    /// </summary>
    class GeometryCalculatorSettings : XMLAccessClass
    {
        private const string DECIMAL_PLACES = "DecimalPlaces";


        /// <summary>
        /// クラスコンストラクタ。設定ファイルを読み込む。
        /// </summary>
        public GeometryCalculatorSettings()
        {
            LoadSettings();
        }


        /// <summary>
        /// 小数点以下の桁数の設定
        /// </summary>
        public int DecimalPlaces
        {
            get
            {
                string work;

                work = base.GetXMLValue(DECIMAL_PLACES);

                return int.Parse(work);
            }

            set
            {
                base.SetXMLValue(DECIMAL_PLACES, value.ToString());
            }
        }


        /// <summary>
        /// 設定ファイル読み込み
        /// </summary>
        private void LoadSettings()
        {
            base.LoadXMLDocument();
        }


        /// <summary>
        /// 設定ファイル保存
        /// </summary>
        public void SaveSettings()
        {
            base.SaveXMLDocument();
        }
    }
}
