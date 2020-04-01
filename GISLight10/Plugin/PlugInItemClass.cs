using System;
using System.Collections.Generic;
using System.Text;

namespace ESRIJapan.GISLight10.Plugin
{
    /// <summary>
    /// プラグイン設定（ファイル）読み込み用クラス
    /// </summary>
    class PlugInItemClass
    {
        // PlugInItemXMLFile.xml、PlugInItemのNameとも同じ内容である必要ある
        /// <summary>
        /// プラグイン名称（プラグインメニューに表示される名称）
        /// </summary>
        public string PlugInName = null;

        /// <summary>
        /// プラグイン実行時、取得パラメータ（プロパティ名）
        /// </summary>
        public List<string> ParaMeter = null;

    }
}
