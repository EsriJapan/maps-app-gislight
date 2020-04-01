using System;
using System.Collections.Generic;
using System.Text;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 編集オプション操作クラス
    /// </summary>
    /// <history>
    ///  2010-11-01  新規作成 
    /// </history>
    class EditOptionSettings : XMLAccessClass
    {
        private const string SNAP_TOLERANCE = "SnapTolerance";
        private const string STICKY_MOVE_TOLERANCE = "StickyMoveTolerance";
        private const string STREAM_TOLERANCE = "StreamTolerance";
        private const string STREAM_GROUPING_COUNT = "StreamGroupingCount";
        private const string MAX = "max";
        private const string MIN = "min";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditOptionSettings()
        {
            LoadSettings();
        }

        /// <summary>
        /// スナップ許容値
        /// </summary>
        public string SnapTolerance
        {
            get
            {
                return base.GetXMLValue(SNAP_TOLERANCE);
            }

            set
            {
                base.SetXMLValue(SNAP_TOLERANCE, value);
            }
        }

        /// <summary>
        /// スナップ許容値(最大)
        /// </summary>
        public string SnapToleranceMax
        {
            get
            {
                return base.GetXMLAttributeValue(SNAP_TOLERANCE, MAX);
            }
        }

        ///// <summary>
        ///// スナップ許容値(最小)
        ///// </summary>
        //public string SnapToleranceMin
        //{
        //    get
        //    {
        //        return base.GetXMLAttributeValue(SNAP_TOLERANCE, MIN);
        //    }
        //}

        /// <summary>
        /// 移動抑制許容値
        /// </summary>
        public string StickyMoveTolerance
        {
            get
            {
                return base.GetXMLValue(STICKY_MOVE_TOLERANCE);
            }

            set
            {
                base.SetXMLValue(STICKY_MOVE_TOLERANCE, value);
            }
        }

        /// <summary>
        /// 移動抑制許容値(最大)
        /// </summary>
        public string StickyMoveToleranceMax
        {
            get
            {
                return base.GetXMLAttributeValue(STICKY_MOVE_TOLERANCE, MAX);
            }
        }

        ///// <summary>
        ///// 移動抑制許容値(最小)
        ///// </summary>
        //public string StickyMoveToleranceMin
        //{
        //    get
        //    {
        //        return base.GetXMLAttributeValue(STICKY_MOVE_TOLERANCE, MIN);
        //    }
        //}

        /// <summary>
        /// ストリーム許容値
        /// </summary>
        public string StreamTolerance
        {
            get
            {
                return base.GetXMLValue(STREAM_TOLERANCE);
            }

            set
            {
                base.SetXMLValue(STREAM_TOLERANCE, value);
            }
        }

        /// <summary>
        /// ストリーム許容値(最大)
        /// </summary>
        public string StreamToleranceMax
        {
            get
            {
                return base.GetXMLAttributeValue(STREAM_TOLERANCE, MAX);
            }
        }

        ///// <summary>
        ///// ストリーム許容値(最小)
        ///// </summary>
        //public string StreamToleranceMin
        //{
        //    get
        //    {
        //        return base.GetXMLAttributeValue(STREAM_TOLERANCE, MIN);
        //    }
        //}

        /// <summary>
        /// グループ化する頂点数
        /// </summary>
        public string StreamGroupingCount
        {
            get
            {
                return base.GetXMLValue(STREAM_GROUPING_COUNT);
            }

            set
            {
                base.SetXMLValue(STREAM_GROUPING_COUNT, value);
            }
        }

        /// <summary>
        /// グループ化する頂点数(最大)
        /// </summary>
        public string StreamGroupingCountMax
        {
            get
            {
                return base.GetXMLAttributeValue(STREAM_GROUPING_COUNT, MAX);
            }
        }

        ///// <summary>
        ///// グループ化する頂点数(最小)
        ///// </summary>
        //public string StreamGroupingCountMin
        //{
        //    get
        //    {
        //        return base.GetXMLAttributeValue(STREAM_GROUPING_COUNT, MIN);
        //    }
        //}

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
