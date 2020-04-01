using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// スレッド処理用クラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public class TaskInfo
    {
        /// <summary>
        /// シリアライズデータ
        /// </summary>
        public string SerializeData { get; set; }

        /// <summary>
        /// コールバック(戻り値なし)
        /// </summary>
        public MethodInvoker CallBack { get; set; }

        /// <summary>
        /// コールバック(戻り値あり)
        /// </summary>
        public ParameterizedThreadStart CallBackParam { get; set; }
    }
}
