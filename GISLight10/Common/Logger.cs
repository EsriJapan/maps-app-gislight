using System;
using System.Collections.Generic;
using System.Text;
using ESRIJapan.GISLight10.Properties;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// クラスインスタンスを作成せずにログを出力するためのクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public class Logger
    {
        private static bool loadConfig = false;
        private static string	_strBDir = "";

        private static readonly log4net.ILog logger = 
            log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// ロガー実行フォルダを取得または設定します
        /// </summary>
        public static string BaseDirectory {
			get {
				return _strBDir;
			}
			set {
				_strBDir = value ?? "";
			}
        }

        /// <summary>
        /// クラスインスタンス。クラスの初期設定をおこなう。
        /// </summary>
        public Logger()
        {
            SetConfig();
        }
        
        /// <summary>
        /// log4netを設定する。
        /// 設定ファイル名はapp.configから取得する。
        /// </summary>
        protected static void SetConfig()
        {
            if (loadConfig == false)
            {
                log4net.Config.XmlConfigurator.Configure(
                    log4net.LogManager.GetRepository(),
                    new System.IO.FileInfo(_strBDir + (_strBDir.EndsWith(@"\") ? "" : @"\") + Settings.Default.Log4NetConfPath));

                loadConfig = true;
            }
        }


        /// <summary>
        /// デバッグレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        static public void Debug(string message)
        {
            SetConfig();
            logger.Debug(message);
        }

        /// <summary>
        /// デバッグレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        /// <param name="ex">ログ出力する例外</param>
        static public void Debug(string message, Exception ex)
        {
            SetConfig();
            logger.Debug(message, ex);
        }

        /// <summary>
        /// インフォレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        static public void Info(string message)
        {
            SetConfig();
            logger.Info(message);
        }

        /// <summary>
        /// インフォレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        /// <param name="ex">ログ出力する例外</param>
        static public void Info(string message, Exception ex)
        {
            SetConfig();
            logger.Info(message, ex);
        }

        /// <summary>
        /// ワーニングレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        static public void Warn(string message)
        {
            SetConfig();
            logger.Warn(message);
        }

        /// <summary>
        /// ワーニングレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        /// <param name="ex">ログ出力する例外</param>
        static public void Warn(string message, Exception ex)
        {
            SetConfig();
            logger.Warn(message, ex);
        }

        /// <summary>
        /// エラーレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        static public void Error(string message)
        {
            SetConfig();
            logger.Error(message);
        }

        /// <summary>
        /// エラーレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        /// <param name="ex">ログ出力する例外</param>
        static public void Error(string message, Exception ex)
        {
            SetConfig();
            logger.Error(message, ex);
        }

        /// <summary>
        /// フェイタルレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        static public void Fatal(string message)
        {
            SetConfig();
            logger.Fatal(message);
        }

        /// <summary>
        /// フェイタルレベルでログを出力する
        /// </summary>
        /// <param name="message">ログ出力する文字列</param>
        /// <param name="ex">ログ出力する例外</param>
        static public void Fatal(string message, Exception ex)
        {
            SetConfig();
            logger.Fatal(message, ex);
        }
    }
}
