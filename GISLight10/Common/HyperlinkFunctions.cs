using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// ハイパーリンクの設定を行うクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public class HyperlinkFunctions
    {
        /// <summary>
        /// ハイパーリンクの設定
        /// </summary>
        /// <param name="featureLayer">ハイパーリンクを設定するレイヤ</param>
        /// <param name="hotlinkField">ハイパーリンクを設定するフィールド</param>
        /// <param name="hyperlinkType">設定するハイパーリンクタイプ</param>
        public static void SetHyperlinks(
            IFeatureLayer featureLayer, string hotlinkField, int hyperlinkType)
        {
            IHotlinkContainer hotlinkContainer = featureLayer as IHotlinkContainer;

            hotlinkContainer.HotlinkField = hotlinkField;

            switch (hyperlinkType)
            {
                case 0:
                    hotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeDocument;
                    break;

                case 1:
                    hotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeURL;
                    break;

                case 2:
                    hotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeMacro;
                    break;

                default:
                    throw new NotSupportedException(string.Format("無効なハイパーリンクタイプです。"));
            }
        }

        /// <summary>
        /// ハイパーリンクの解除
        /// </summary>
        /// <param name="featureLayer">ハイパーリンクを解除するレイヤ</param>
        public static void DeleteHyperlinks(IFeatureLayer featureLayer)
        {
            IHotlinkContainer hotlinkContainer = featureLayer as IHotlinkContainer;

            hotlinkContainer.HotlinkField = "";
            hotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeDocument;
        }

        /// <summary>
        /// ハイパーリンクの判別
        /// </summary>
        /// <param name="featureLayer">判別するレイヤ</param>
        /// <returns>
        /// <br>ハイパーリンクの判別結果</br>
        /// <br>ハイパーリンクあり:true,ハイパーリンクなし:false</br>
        /// </returns>
        public static bool HasHyperlinks(IFeatureLayer featureLayer)
        {
            IHotlinkContainer hotlinkContainer = featureLayer as IHotlinkContainer;

            if (hotlinkContainer.HotlinkField != "")
            {
                return true;
            }

            return false;
        }
    }
}
