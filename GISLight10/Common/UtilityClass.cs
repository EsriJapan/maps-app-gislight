using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// ユーティリティクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2010-01-26 SHGetFolderPathの返り値のXMLコメント記述修正
    /// </history>
    public static class UtilityClass
    {

        //[DllImport("shell32.dll")]
        //static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken,
        //   uint dwFlags, [Out] StringBuilder pszPath);

        /// <summary>
        /// CSIDL値に対応するフォルダのパス名を取得
        /// </summary>
        /// <param name="hwndOwner">オーナーウィンドウのハンドル</param>
        /// <param name="nFolder">パス名を取得するフォルダを識別するCSIDL 値</param>
        /// <param name="hToken">特定ユーザー指定するアクセストークン</param>
        /// <param name="dwFlags">フォルダの現在のパス名またはデフォルトのパス名のどちらのパスが返されるかを指定するフラグ</param>
        /// <param name="pszPath">パス名を表わすヌル終端文字列を格納するバッファのアドレス</param>
        /// <returns>
        /// <br>成功時:0x00000000 (S_OK)</br>
        /// <br>失敗時:0x00000001 (S_FALSE)</br>
        /// <br>失敗時:0x80000008 (E_FAIL)</br>
        /// <br>失敗時:0x80000003 (E_INVALIDARG)</br>
        /// </returns>
        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public static extern Int32 SHGetFolderPath(
            IntPtr hwndOwner,
            Int32 nFolder,
            IntPtr hToken,
            UInt32 dwFlags,
            System.Text.StringBuilder pszPath);

        private const Int32 CSIDL_FONTS = 0x0014;
        private const UInt32 SHGFP_TYPE_CURRENT = 0x0000;


        /// <summary>
        /// フォントディレクトリを取得
        /// </summary>
        /// <returns>フォントディレクトリパス</returns>
        public static string GetFontsFolderPath()
        {
            StringBuilder builder = new StringBuilder();
            SHGetFolderPath(IntPtr.Zero, CSIDL_FONTS, IntPtr.Zero, SHGFP_TYPE_CURRENT, builder);

            return builder.ToString();
        }

        /// <summary>
        /// 引数に指定された文字列に対応した実際のフォント名称を取得
        /// </summary>
        /// <param name="fontName">フォント名</param>
        /// <returns>実フォント名</returns>
        public static string GetFontFileName(string fontName)
        {
            Microsoft.Win32.RegistryKey fontsKey1 =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts");

            Microsoft.Win32.RegistryKey fontsKey2 =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts");

            string ret1 = fontsKey1.GetValue(fontName, string.Empty) as string;
            if (ret1 != null)
            {
                return ret1;
            }

            string ret2 = fontsKey2.GetValue(fontName, string.Empty) as string;
            if (ret2 != null)
            {
                return ret2;
            }
            return string.Empty;
        }

        /// <summary>
        /// フォームのリソースファイルから文字列を取得
        /// 未使用
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="messageKey"></param>
        /// <returns></returns>
        private static string GetFormResourceMessage(
            System.Windows.Forms.Form frm, string messageKey)
        {
            try
            {
                if (frm == null || messageKey == null || messageKey.Length == 0) 
                    return string.Empty;

                System.Reflection.Assembly assembly = frm.GetType().Assembly;

                string nsp = frm.GetType().Namespace;
                string name = frm.GetType().Name;

                // ResFile.Strings -> <Namespace i.e. ESRIJapan.GISLight10.Ui>.<ResourceFileName i.e. MainForm.resx> 
                System.Resources.ResourceManager resman =
                    new System.Resources.ResourceManager(nsp + "." + name, assembly);

                return resman.GetString(messageKey);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 開いている属性テーブルフォームあれば閉じる
        /// </summary>
        public static void DeleteAttributeTableObjects()
        {
            ESRIJapan.GISLight10.Common.SubWindowNameClass subwinname = 
                ESRIJapan.GISLight10.Common.SubWindowNameClass.GetInstance();

            if (subwinname.GetCount() > 0)
            {
                System.Collections.ICollection subwins = subwinname.Keys;
                System.Collections.IEnumerator frmAtt = subwins.GetEnumerator();
                frmAtt.Reset();
                while (frmAtt.MoveNext())
                {
                    string frmattkey = frmAtt.Current.ToString();
                    Type typ = subwinname[frmattkey].GetType();
                    if (typ != null)
                    {
                        Ui.FormAttributeTable attTbl =
                            subwinname[frmattkey] as Ui.FormAttributeTable;
                        attTbl.Dispose();
                    }
                }
                subwinname.RemoveAll();
            }
        }

        /// <summary>
        /// 引数指定された例外オブジェクトの
        /// エラーメッセージとスタックとレースのログ出力
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        public static void DoOnError(Exception ex)
        {
            if(ex != null) {
				Common.Logger.Error(ex.Message);
				Common.Logger.Error(ex.StackTrace);
            }
            else {
				Common.Logger.Error("NullのExceptionを受信しました。");
            }
        }
		/// <summary>
		/// 例外オブジェクトのエラーメッセージとスタックトレースをログに出力します。
		/// </summary>
		/// <param name="TargetSource">発生場所</param>
		/// <param name="ex">例外オブジェクト</param>
        public static void DoOnError(string TargetSource, Exception ex) {
			if(ex != null) {
				Common.Logger.Error(TargetSource + " : "+ ex.Message);
				Common.Logger.Error(ex.StackTrace);
            }
            else {
				Common.Logger.Error("NullのExceptionを受信しました。");
            }
        }

        /// <summary>
        /// 引数指定された、
        /// 例外時のエラーメッセージと、スタックトレース、エラーコードのログ出力
        /// </summary>
        /// <param name="msg">エラーメッセージ</param>
        /// <param name="strace">スタックトレース</param>
        /// <param name="errcd">エラーコード</param>
        public static void DoOnError(string msg, string strace, string errcd)
        {
            Common.Logger.Error(msg);
            Common.Logger.Error(strace);
            if (errcd != null) Common.Logger.Error(errcd);
        }

        /// <summary>
        /// 日本語マップユニット名の取得
        /// </summary>
        /// <param name="unit">マップユニット</param>
        /// <returns>日本語マップユニット名</returns>
        public static string GetMapUnitText(ESRI.ArcGIS.esriSystem.esriUnits unit)
        {
            string unitName = "";
            //switch (axMapControl1.MapUnits)
            switch (unit)
            {
                case ESRI.ArcGIS.esriSystem.esriUnits.esriCentimeters:
                    unitName = "センチメートル";
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriMeters:
                    unitName = "メートル";
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers:
                    unitName = "キロメートル";
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriInches:
                    unitName = "インチ";
                    break;

                case ESRI.ArcGIS.esriSystem.esriUnits.esriDecimalDegrees:
                    unitName = "度(10進)";
                    break;

                default:
                    break;
            }
            return unitName;
        }

        /// <summary>
        /// 数値チェック
        /// </summary>
        /// <param name="tValue">数値チェック対象文字列</param>
        /// <returns>数値の場合:true, 非数値の場合:false</returns>
        public static bool IsNumeric(string tValue)
        {
            Regex regex = new Regex("^[0-9]+$");
            if (!regex.IsMatch(tValue))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 引数指定される文字列の内容の数値チェック
        /// </summary>
        /// <param name="val">チェック対象の文字列</param>
        /// <param name="NumberStyle">数値基本型クラスの Parse メソッドに渡される数値文字列引数で使用できるスタイル</param>
        /// <returns>数値チェック結果</returns>
        public static bool IsNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            return Double.TryParse(val, NumberStyle,
                System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// レジストリから、キー指定されたパス下のInstallDirの内容を取得して返す
        /// </summary>
        /// <param name="sKey">レジストリから取得するキー</param>
        /// <returns>取得した内容</returns>
        public static string GetReadRegistryPath(string sKey)
        {
            //Open the subkey for reading
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sKey);//, false);
            if (rk == null) return "";
            
            // Get the data from a specified item in the key.
            return (string)rk.GetValue("InstallDir");
        }

        /// <summary>
        /// カラーパレットから色の取得
        /// </summary>
        /// <param name="initcolor">現状のカラー</param>
        /// <returns>カラーパレットから指定されたカラー</returns>
        public static System.Drawing.Color GetColor(System.Drawing.Color initcolor)
        {
            System.Windows.Forms.ColorDialog colorDialog1 = 
                new System.Windows.Forms.ColorDialog();

            // 初期選択する色を設定
            colorDialog1.Color = initcolor;

            // カスタム カラーを定義可能に (初期値 true)
            //colorDialog1.AllowFullOpen = true;

            // カスタム カラーを表示した状態に (初期値 false)
            colorDialog1.FullOpen = true;

            // 使用可能なすべての色を基本セットに表示 (初期値 false)
            colorDialog1.AnyColor = true;

            // 純色のみ表示 (初期値 false)
            colorDialog1.SolidColorOnly = true;

            // カスタム カラー設定
            colorDialog1.FullOpen = false;

            // カスタムカラー読み込み
            //int[] colors = GetCustomColors();
            CustomColorsSettings customcolset = new CustomColorsSettings();
            int[] colors = customcolset.CustomColors;

            if (colors != null)
            {
                colorDialog1.CustomColors = colors;
            }
            else
            {
                // カスタムカラーエレメント無い場合には追加
                customcolset.CreateCustomColorElement(colorDialog1.CustomColors);
                customcolset.SaveSettings();
            }

            colorDialog1.ShowHelp = false;

            System.Windows.Forms.DialogResult dr = colorDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Cancel) return initcolor;

            // カスタムカラーを保持
            //SaveCustomColors(colorDialog1.CustomColors);
            customcolset.CustomColors = colorDialog1.CustomColors;
            customcolset.SaveSettings();

            colorDialog1.Dispose();
            return colorDialog1.Color;
        }

        // 参考
        // http://forums.esri.com/Thread.asp?c=159&f=1707&t=225562&mc=3#msgid684835
        /// <summary>
        /// System.Drawing.ColorのESRIカラー（ESRI.ArcGIS.Display.IRgbColor）への変換
        /// </summary>
        /// <param name="color">System.Drawing.Color</param>
        /// <returns>ESRIカラー</returns>
        public static ESRI.ArcGIS.Display.IRgbColor ConvertToESRIColor(System.Drawing.Color color)
        {
            ESRI.ArcGIS.Display.IRgbColor esriColor = new ESRI.ArcGIS.Display.RgbColorClass();
            esriColor.Red = color.R;
            esriColor.Green = color.G;
            esriColor.Blue = color.B;
            return esriColor;
        }

        /// <summary>
        /// 色設定ボタンの背景色設定
        /// </summary>
        /// <param name="rgb">ESRIカラー</param>
        /// <param name="tbtn">背景色設定対象のボタンオブジェクト</param>
        public static void SetButtonBackColor(
            ESRI.ArcGIS.Display.IRgbColor rgb, 
            System.Windows.Forms.Button tbtn)
        {
            if (rgb == null) return;

            System.Drawing.Color color = 
                System.Drawing.Color.FromArgb(rgb.Red, rgb.Green, rgb.Blue);

            tbtn.BackColor = color;
        }

        /// <summary>
        /// カスタムカラーの保存
        /// </summary>
        /// <param name="customcolors">カスタムカラーコード格納配列</param>
        public static void SaveCustomColors(int[] customcolors)
        {
            string roamingDirectoryPath = 
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            
            string userSettingsSubDirectoryPath = 
                Properties.Settings.Default.UserSettingsDirectoryPath;

            string customColorFileName = Properties.Settings.Default.CustomColorFileName;
            // "CustomColor.dat";
            
            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(roamingDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(userSettingsSubDirectoryPath);

            string userSettingsDirectoryPath = fullPath.ToString();

            if (!System.IO.Directory.Exists(userSettingsDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(userSettingsDirectoryPath);
            }

            fullPath.Append("\\");
            fullPath.Append(customColorFileName);

            string customColorFullPath = fullPath.ToString();

            System.IO.FileStream fs = null;
            try
            {
                if (System.IO.File.Exists(customColorFullPath))
                {
                    fs = new System.IO.FileStream(customColorFullPath, System.IO.FileMode.Truncate);
                }
                else
                {
                    fs = new System.IO.FileStream(customColorFullPath, System.IO.FileMode.Create);
                }

                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(int[]));

                serializer.Serialize(fs, customcolors);
            }
            catch { }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// カスタムカラーの読み込み
        /// </summary>
        /// <returns>カスタムカラーコード格納配列</returns>
        public static int[] GetCustomColors()
        {
            string roamingDirectoryPath =
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string userSettingsSubDirectoryPath =
                Properties.Settings.Default.UserSettingsDirectoryPath;

            string customColorFileName = Properties.Settings.Default.CustomColorFileName;
            // "CustomColor.dat";

            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(roamingDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(userSettingsSubDirectoryPath);

            string userSettingsDirectoryPath = fullPath.ToString();

            if (!System.IO.Directory.Exists(userSettingsDirectoryPath))
            {
                return null;
            }

            fullPath.Append("\\");
            fullPath.Append(customColorFileName);

            string customColorFullPath = fullPath.ToString();
            if (!System.IO.File.Exists(customColorFullPath))
            {
                return null;
            }

            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream(
                    customColorFullPath, System.IO.FileMode.Open);

                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(int[]));

                int[] colors = (int[])(serializer.Deserialize(fs));

                return colors;
            }
            catch 
            {
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();   
                }
            }
        }

        /// <summary>
        /// テーブル結合後に編集可能な別名定義内容を戻す
        /// </summary>
        /// <param name="featLayer">別名定義内容を元に戻すレイヤ</param>
        /// <param name="fldAliases">別名定義内容</param>
        public static void RestoreEditableAlias(IFeatureLayer featLayer, Hashtable fldAliases)
        {

            IDisplayTable dispTbl = (IDisplayTable)featLayer;
            ITableFields tblFlds = (ITableFields)featLayer;

            for (int i = 0; i < dispTbl.DisplayTable.Fields.FieldCount; i++)
            {
                if (i > fldAliases.Count)
                {
                    return;
                }

                IField field = dispTbl.DisplayTable.Fields.get_Field(i);
                IFieldInfo fldinfo = tblFlds.get_FieldInfo(i);

                if (fldAliases.Contains(i))
                {
                    fldinfo.Alias = fldAliases[i].ToString();
                }
            }
        }

		/// <summary>
		/// 範囲を少し膨張します
		/// </summary>
		/// <param name="OrgEnv">元の範囲</param>
		/// <returns></returns>
		static public IEnvelope ExpandEnvelope(IEnvelope OrgEnv) {
			IEnvelope	agEnv = new EnvelopeClass();
			OrgEnv.QueryEnvelope(agEnv);

			IPoint	agPnt = (OrgEnv as IArea).Centroid;
			OrgEnv.Width *= 1.05;
			OrgEnv.Height *= 1.05;
			IPoint	agPnt2 = (OrgEnv as IArea).Centroid;
			OrgEnv.CenterAt(agPnt);

			return OrgEnv;
		}
    }
}
