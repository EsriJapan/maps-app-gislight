using System;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// アプリケーションの初期化を行うクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-26 xmlコメント、publicメソッドの返却値の記述追加
    /// </history>
    public class ApplicationInitializer
    {
        /// <summary>
        /// 設定ファイルを作成
        /// </summary>
        public static void CreateUserSettings()
        {
            // 設定ファイルはApplication Dataフォルダに保存
            string roamingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // 設定ファイル保存先サブフォルダ
            string userSettingsSubDirectoryPath = Properties.Settings.Default.UserSettingsDirectoryPath;
            
            // 設定ファイル名
            string userSettingsFileName = Properties.Settings.Default.UserSettingsXmlName;

            // 設定ファイルのパス生成
            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(roamingDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(userSettingsSubDirectoryPath);

            string userSettingsDirectoryPath = fullPath.ToString();

            // 設定ファイル格納フォルダの作成
            if (!Directory.Exists(userSettingsDirectoryPath))
            {
                Directory.CreateDirectory(userSettingsDirectoryPath);
            }

            fullPath.Append("\\");
            fullPath.Append(userSettingsFileName);

            string userSettingsPath = fullPath.ToString();

            // デフォルト設定ファイルをコピー
            if (!File.Exists(userSettingsPath))
            {
                File.Copy(System.IO.Path.Combine(Application.StartupPath, userSettingsFileName), userSettingsPath);
            }
        }

        /// <summary>
        /// プラグイン設定ファイルを作成します
        /// </summary>
        public static void CreatePluginSettings() {
			// 設定ファイルはApplication Dataフォルダに保存
			string roamingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			// 設定ファイル保存先サブフォルダ
			string userSettingsSubDirectoryPath = Properties.Settings.Default.UserSettingsDirectoryPath;
			// 設定ファイル名
			string pluginSettingsFileName = Properties.Settings.Default.PlugInInfoXmlPath;

			// 設定ファイルのパス生成
			StringBuilder fullPath = new StringBuilder();
			fullPath.Append(roamingDirectoryPath);
			fullPath.Append("\\");
			fullPath.Append(userSettingsSubDirectoryPath);

			string userSettingsDirectoryPath = fullPath.ToString();

			// 設定ファイル格納フォルダの作成
			if(!Directory.Exists(userSettingsDirectoryPath)) {
				Directory.CreateDirectory(userSettingsDirectoryPath);
			}

			fullPath.Append("\\");
			fullPath.Append(pluginSettingsFileName);

			string userSettingsPath = fullPath.ToString();

			// ﾌﾟﾗｸﾞｲﾝ設定ﾌｧｲﾙをｺﾋﾟｰ (上書き)
			File.Copy(System.IO.Path.Combine(Application.StartupPath, pluginSettingsFileName), userSettingsPath, true);
        }

        /// <summary>
        /// テンポラリフォルダを作成
        /// </summary>
        public static void CreateTemporaryFolder()
        {
            // テンポラリフォルダはTemporary Folderフォルダに保存
            string temporaryDirectoryPath = Path.GetTempPath();
            // テンポラリフォルダ名
            string temporaryFolderName = Properties.Settings.Default.TemporaryDirectoryPath;

            // テンポラリフォルダのパス生成
            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(Path.GetTempPath());
            fullPath.Append(temporaryFolderName);

            string temporaryFolderPath = fullPath.ToString();

            // テンポラリフォルダ作成
            if (!Directory.Exists(temporaryFolderPath))
            {
                Directory.CreateDirectory(temporaryFolderPath);
            }
        }

        /// <summary>
        /// 設定ファイルの存在確認
        /// </summary>
        /// <returns><br>設定ファイル存在確認結果</br>
        /// <br>存在時:true,非存在時:false</br>
        /// </returns>
        public static bool IsUserSettingsExists()
        {
            // 設定ファイルはApplication Dataフォルダに保存
            string roamingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // 設定ファイル保存先サブフォルダ(ESRI\GISLight10)
            string userSettingsSubDirectoryPath = Properties.Settings.Default.UserSettingsDirectoryPath;
            // 設定ファイル名
            string userSettingsFileName = Properties.Settings.Default.UserSettingsXmlName;

            // 設定ファイルのパス生成
            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(roamingDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(userSettingsSubDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(userSettingsFileName);

            string userSettingsPath = fullPath.ToString();

            if (!File.Exists(userSettingsPath))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// プラグイン設定ファイルの存在確認
        /// </summary>
        /// <returns></returns>
        public static bool IsPluginSettingsExists() {
            // 設定ファイルはApplication Dataフォルダに保存
            string roamingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // 設定ファイル保存先サブフォルダ(ESRI\GISLight10)
            string userSettingsSubDirectoryPath = Properties.Settings.Default.UserSettingsDirectoryPath;
            // 設定ファイル名
            string pluginSettingsFileName = Properties.Settings.Default.PlugInInfoXmlPath;

            // 設定ファイルのパス生成
            StringBuilder fullPath = new StringBuilder();
            fullPath.Append(roamingDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(userSettingsSubDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(pluginSettingsFileName);

            return File.Exists(fullPath.ToString());
        }

        /// <summary>
        /// テンポラリフォルダの存在確認
        /// </summary>
        /// <returns><br>テンポラリフォルダ存在確認結果</br>
        /// <br>存在時:true,非存在時:false</br>
        /// </returns>
        public static bool IsTemporaryFolderExists()
        {
            // テンポラリフォルダはTemporary Folderフォルダに保存
            string temporaryDirectoryPath = Path.GetTempPath();
            // テンポラリフォルダ名
            string temporaryFolderName = Properties.Settings.Default.TemporaryDirectoryPath;

            // テンポラリフォルダのパス生成
            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(Path.GetTempPath());
            fullPath.Append(temporaryFolderName);

            string temporaryFolderPath = fullPath.ToString();

            if (!Directory.Exists(temporaryFolderPath))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 設定ファイル削除
        /// </summary>
        public static void DeleteSettings()
        {
            // 設定ファイルはApplication Dataフォルダに保存
            string roamingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // 設定ファイル保存先サブフォルダ
            string userSettingsSubDirectoryPath = Properties.Settings.Default.UserSettingsDirectoryPath;
            // 設定ファイル名
            string userSettingsFileName = Properties.Settings.Default.UserSettingsXmlName;

            // 設定ファイルのパス生成
            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(roamingDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(userSettingsSubDirectoryPath);

            string userSettingsDirectoryPath = fullPath.ToString();

            fullPath.Append("\\");
            fullPath.Append(userSettingsFileName);

            string userSettingsPath = fullPath.ToString();

            // 設定ファイルを削除
            if (File.Exists(userSettingsPath))
            {
                File.Delete(userSettingsPath);
            }

            //// 設定ファイル格納フォルダを削除
            //if (Directory.Exists(userSettingsDirectoryPath))
            //{
            //    Directory.Delete(userSettingsDirectoryPath);
            //}
        }
    }
}
