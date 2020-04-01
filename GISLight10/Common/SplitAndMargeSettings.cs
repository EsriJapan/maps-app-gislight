using System;
using System.Collections.Generic;
using System.Text;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 図形・属性編集設定操作クラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    class SplitAndMargeSettings : XMLAccessClass
    {
        private const string EDITOR_SPLIT_FIELD = "EditorSplitField";
        private const string EDITOR_SPLIT_NUM_FIELD = "EditorSplitNumField";
        private const string EDITOR_SPLIT_DATE_FIELD = "EditorSplitDateField";
        private const string EDITOR_MARGE_FIELD = "EditorMargeField";
        private const string EDITOR_MARGE_NUM_FIELD = "EditorMargeNumField";
        private const string EDITOR_MARGE_DATE_FIELD = "EditorMargeDateField";
        private const string MAX = "max";
        private const string MIN = "min";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SplitAndMargeSettings()
        {
            LoadSettings();
        }

        /// <summary>
        /// スプリットポリシー
        /// </summary>
        public string EditorSplitField
        {
            get
            {
                return base.GetXMLValue(EDITOR_SPLIT_FIELD);
            }

            set
            {
                base.SetXMLValue(EDITOR_SPLIT_FIELD, value);
            }
        }

        /// <summary>
        /// スプリットポリシー(最大)
        /// </summary>
        public string EditorSplitFieldMax
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_SPLIT_FIELD, MAX);
            }
        }

        /// <summary>
        /// スプリットポリシー(最小)
        /// </summary>
        public string EditorSplitFieldMin
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_SPLIT_FIELD, MIN);
            }
        }

        /// <summary>
        /// スプリットポリシー(数値型フィールド)
        /// </summary>
        public string EditorSplitNumField
        {
            get
            {
                return base.GetXMLValue(EDITOR_SPLIT_NUM_FIELD);
            }

            set
            {
                base.SetXMLValue(EDITOR_SPLIT_NUM_FIELD, value);
            }
        }

        /// <summary>
        /// スプリットポリシー(数値型フィールド)(最大)
        /// </summary>
        public string EditorSplitNumFieldMax
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_SPLIT_NUM_FIELD, MAX);
            }
        }

        /// <summary>
        /// スプリットポリシー(数値型フィールド)(最小)
        /// </summary>
        public string EditorSplitNumFieldMin
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_SPLIT_NUM_FIELD, MIN);
            }
        }


        /// <summary>
        /// スプリットポリシー(日付型フィールド)
        /// </summary>
        public string EditorSplitDateField
        {
            get
            {
                return base.GetXMLValue(EDITOR_SPLIT_DATE_FIELD);
            }

            set
            {
                base.SetXMLValue(EDITOR_SPLIT_DATE_FIELD, value);
            }
        }

        /// <summary>
        /// スプリットポリシー(日付型フィールド)(最大)
        /// </summary>
        public string EditorSplitDateFieldMax
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_SPLIT_DATE_FIELD, MAX);
            }
        }

        /// <summary>
        /// スプリットポリシー(日付型フィールド)(最小)
        /// </summary>
        public string EditorSplitDateFieldMin
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_SPLIT_DATE_FIELD, MIN);
            }
        }

        /// <summary>
        /// マージポリシー
        /// </summary>
        public string EditorMargeField
        {
            get
            {
                return base.GetXMLValue(EDITOR_MARGE_FIELD);
            }

            set
            {
                base.SetXMLValue(EDITOR_MARGE_FIELD, value);
            }
        }

        /// <summary>
        /// マージポリシー(最大)
        /// </summary>
        public string EditorMargeFieldMax
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_MARGE_FIELD, MAX);
            }
        }

        /// <summary>
        /// マージポリシー(最小)
        /// </summary>
        public string EditorMargeFieldMin
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_MARGE_FIELD, MIN);
            }
        }

        /// <summary>
        /// マージポリシー(数値型フィールド)
        /// </summary>
        public string EditorMargeNumField
        {
            get
            {
                return base.GetXMLValue(EDITOR_MARGE_NUM_FIELD);
            }

            set
            {
                base.SetXMLValue(EDITOR_MARGE_NUM_FIELD, value);
            }
        }

        /// <summary>
        /// マージポリシー(数値型フィールド)(最大)
        /// </summary>
        public string EditorMargeNumFieldMax
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_MARGE_NUM_FIELD, MAX);
            }
        }

        /// <summary>
        /// マージポリシー(数値型フィールド)(最小)
        /// </summary>
        public string EditorMargeNumFieldMin
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_MARGE_NUM_FIELD, MIN);
            }
        }

        /// <summary>
        /// マージポリシー(日付型フィールド)
        /// </summary>
        public string EditorMargeDateField
        {
            get
            {
                return base.GetXMLValue(EDITOR_MARGE_DATE_FIELD);
            }

            set
            {
                base.SetXMLValue(EDITOR_MARGE_DATE_FIELD, value);
            }
        }

        /// <summary>
        /// マージポリシー(日付型フィールド)(最大)
        /// </summary>
        public string EditorMargeDateFieldMax
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_MARGE_DATE_FIELD, MAX);
            }
        }

        /// <summary>
        /// マージポリシー(日付型フィールド)(最小)
        /// </summary>
        public string EditorMargeDateFieldMin
        {
            get
            {
                return base.GetXMLAttributeValue(EDITOR_MARGE_DATE_FIELD, MIN);
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
