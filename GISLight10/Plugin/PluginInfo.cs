using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

namespace ESRIJapan.GISLight10.Plugin
{
    /// <summary>
    /// プラグイン・オブジェクト情報クラス
    /// </summary>
    public class PluginInfo
    {
        private string _location;
        private string _className;

        /// <summary>
        /// PluginInfoクラスのコンストラクタ
        /// </summary>
        /// <param name="path">アセンブリファイルのパス</param>
        /// <param name="cls">クラスの名前</param>
        private PluginInfo(string path, string cls) {
            this._location = path;
            this._className = cls;
        }

        /// <summary>
        /// アセンブリファイルのパス
        /// </summary>
        public string Location {
            get { return _location; }
        }

        /// <summary>
        /// クラスの名前
        /// </summary>
        public string ClassName {
            get { return _className; }
        }

        /// <summary>
        /// 指定のフォルダに存在するプラグインファイルを取得します。
        /// </summary>
        /// <param name="pluginPath">プラグインパス</param>
        /// <returns>有効なプラグインのPluginInfo配列</returns>
        static public PluginInfo[] FindPlugins(string pluginPath) {
            System.Collections.ArrayList plugins = new System.Collections.ArrayList();

            // IPlugin型の名前
            string ipluginName = typeof(PluginInterface.IPlugin).FullName;
            //string ipluginName = typeof(PluginLibrary.IAddOn).FullName;

            // プラグインフォルダ
            string folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            folder += pluginPath.StartsWith(@"\") ? pluginPath : @"\" + pluginPath; // @"\plugins";
            if(!System.IO.Directory.Exists(folder)) return null;
                //throw new ApplicationException(
                //    "プラグインフォルダ\"" + folder +
                //    "\"が見つかりませんでした。");

            //.dllファイルを探す
            string[] dlls = System.IO.Directory.GetFiles(folder, "*.dll");
            foreach(string dll in dlls) {
                try {
                    // アセンブリとして読み込む
                    System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom(dll);
                    foreach(Type t in asm.GetTypes()) {
                        // アセンブリ内のすべての型について、
                        // プラグインとして有効か調べる
                        if(t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(ipluginName) != null) {
                            // PluginInfoをコレクションに追加する
                            plugins.Add(new PluginInfo(dll, t.FullName));
                        }
                    }
                }
                catch(Exception ex) {
					// ﾛｸﾞに記録
					Common.UtilityClass.DoOnError(string.Format("●ERROR PluginInfo.FindPlugins(ｱｾﾝﾌﾞﾘ･ｴﾗｰ : {0})", dll), ex);
					// ﾃﾞﾊﾞｯｸﾞ出力
					Debug.WriteLine(string.Format("●ERROR PluginInfo.FindPlugins(ｱｾﾝﾌﾞﾘ･ｴﾗｰ : {0}) : {1}", dll, ex.Message));
                }
            }

            //コレクションを配列にして返す
            return (PluginInfo[])plugins.ToArray(typeof(PluginInfo));
        }

        /// <summary>
        /// プラグインクラスのインスタンスを作成する
        /// </summary>
        /// <returns>プラグインクラスのインスタンス</returns>
        //public PluginLibrary.IAddOn CreateInstance()
        public PluginInterface.IPlugin CreateInstance() {
            try {
                //アセンブリを読み込む
                System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom(this.Location);

                //クラス名からインスタンスを作成する
                //return (PluginLibrary.IAddOn)
                return (PluginInterface.IPlugin)asm.CreateInstance(this.ClassName);
            }
            catch(Exception ex) {
				Debug.WriteLine("●ERROR PluginInfo.CreateInstance : " + ex.Message);
                return null;
            }
        }
    }
}
