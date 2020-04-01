using System;
using System.Collections.Generic;
using System.Text;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// カスタムカラー設定操作クラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-26 xmlコメント、引数の記述追加
    /// </history>
    class CustomColorsSettings : XMLAccessClass
    {
        private const string CUSTOM_COLORS = "CustomColors";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomColorsSettings()
        {
            LoadSettings();
        }

        /// <summary>
        /// カスタムカラー
        /// </summary>
        public int[] CustomColors
        {
            get
            {
                return base.GetIntNodeValue(CUSTOM_COLORS);
            }

            set
            {
                base.SetIntNodeValue(CUSTOM_COLORS, value);
            }
        }

        /// <summary>
        /// カスタムカラーエレメント追加
        /// </summary>
        /// <param name="intvalues">カスタムカラー格納整数型配列</param>
        public void CreateCustomColorElement(int[] intvalues)
        {
            base.CreateIntNodeValue(CUSTOM_COLORS, intvalues);
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
