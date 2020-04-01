using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10
{
    static class Program
    {
		/// <summary>
		/// アプリケーション起動パラメータ
		/// </summary>
		static public string[]	StartArguments;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            try {
                if (!ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop))
                {
                    MessageBox.Show(string.Format("ArcGIS Desktop もしくは ArcGIS Engine のライセンスを検出できない為、{0}を起動できません。", Properties.Resources.APP_PRODUCT_NAME));
                    return;
                }

                //Application.EnableVisualStyles(); // XPでカラーランプが表示できなくなるのでコメントアウト
                Application.SetCompatibleTextRenderingDefault(false);

				// ThreadExceptionイベントハンドラを追加
				Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
				// ThreadExceptionが発生しないようにする
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

				// UnhandledExceptionイベントハンドラを追加
				System.AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                // 起動ﾊﾟﾗﾒｰﾀを取得します
                StartArguments = args.Length <= 0 ? null : args;
                
                // 起動
                Application.Run(new Ui.MainForm());
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message,"予期せぬエラー",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                Common.Logger.Fatal(ex.Source);
                Common.Logger.Fatal(ex.Message);
                Common.Logger.Fatal(ex.StackTrace);
            }
        }

        /// <summary>
        /// ファイルの関連付けが設定されているかどうかを確認します
        /// </summary>
        /// <param name="Extension">ファイル拡張子</param>
        /// <returns></returns>
        static public bool IsRelateMXD(string Extension) {
			bool	blnRet = false;
			
			// 拡張子の補完
			if(!Extension.StartsWith(".")) {
				Extension = Extension.Insert(0, ".");
			}

			// 拡張子にｱｸｾｽ
			Microsoft.Win32.RegistryKey	regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Extension);
			if(regKey != null && regKey.ValueCount > 0) {
				// 値を取得
				string	strVal = (string)regKey.GetValue("");
				
				// ｱｾﾝﾌﾞﾘ名を取得
				Assembly	assm = Assembly.GetExecutingAssembly();
				string		strAsmName = assm.GetName().Name;
				
				blnRet = strVal.StartsWith(strAsmName);
			}
			
			return blnRet;
        }
        
        /// <summary>
        /// ArcGIS環境の登録有無を簡易的にチェックします
        /// </summary>
        /// <returns></returns>
        static public bool ExistArcGIS() {
			bool	blnRet = false;
			
			// 拡張子にｱｸｾｽ
			Microsoft.Win32.RegistryKey	regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".mxt");
			if(regKey != null && regKey.ValueCount > 0) {
				// 値をﾁｪｯｸ
				string	strVal = (string)regKey.GetValue("");
				if(!string.IsNullOrEmpty(strVal)) {
					blnRet = strVal.ToUpper() == "ARCMAPTEMPLATE";
				}
			}
			
			return blnRet;
        }
        
		/// <summary>
		/// ファイルの関連付けを実行します
		/// </summary>
		/// <param name="Extension">ファイル拡張子</param>
		/// <param name="IsRegist">登録／解除</param>
		/// <returns></returns>
		static public bool RegistDataFile(string FilePath, string AppName, bool IsRegist) {
			bool	blnRet = true;
			
			string[]	REGIST_EXTENSIONS = { ".mxd", ".pmf" };
			
			// ﾚｼﾞｽﾄﾘ
			Microsoft.Win32.RegistryKey	regKey;
			
			// ﾌｧｲﾙの関連付けを登録
			if(IsRegist) {
				// ｱﾌﾟﾘｹｰｼｮﾝの説明
				string	strAppDesc = "GIS Light";
				// ｱｸｼｮﾝ名
				string	strAct = "open";
				// ｱｸｼｮﾝの説明
				string	strActDesc = strAppDesc + "で開く (&O)";

				// 実行ｱﾌﾟﾘｹｰｼｮﾝ･ﾊﾟｽの補完 (ｺﾏﾝﾄﾞ･ﾗｲﾝ)
				string	strAppCmdLine = string.Format("\"{0}\" %1", FilePath);
				
				// ｱｲｺﾝの設定
				string	strIconPath = FilePath;
				int		intIconID = 0;
				
				foreach(string Extension in REGIST_EXTENSIONS) {
					// 拡張子を登録
					try {
						regKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(Extension);
						regKey.SetValue("", AppName);
						regKey.Close();
						
						// ｱﾌﾟﾘｹｰｼｮﾝ名を登録
						regKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(AppName);
						regKey.SetValue("", strAppDesc);
						
						// ｱｸｼｮﾝを登録
						regKey = regKey.CreateSubKey(@"shell\" + strAct);
						regKey.SetValue("", strActDesc);
						
						// ｺﾏﾝﾄﾞ･ﾗｲﾝを登録
						regKey = regKey.CreateSubKey("command");
						regKey.SetValue("", strAppCmdLine);
						regKey.Close();
						
						// ｱｲｺﾝの登録
						regKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(AppName + @"\DefaultIcon");
						regKey.SetValue("", string.Format("{0},{1}", strIconPath, intIconID));
						regKey.Close();
					}
					catch(Exception ex) {
						blnRet = false;
#if DEBUG
						System.Diagnostics.Debug.WriteLine("REG ERROR" + ex.Message);
#endif
					}
				}
			}
			// ﾌｧｲﾙの関連付けを解除
			else {
				foreach(string Extension in REGIST_EXTENSIONS) {
					try {
						// ArcGISｲﾝｽﾄｰﾙ状況をﾁｪｯｸ
						if(ExistArcGIS()) {
							// 拡張子の登録内容をもとに戻す
							regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Extension, true);
							if(Extension.Equals(".mxd")) {
								regKey.SetValue("", "ArcMapDocument");
							}
							else if(Extension.Equals(".pmf")) {
								regKey.SetValue("", "ArcReader");
							}
							regKey.Close();
						}
						else {
							// 登録を削除
							Microsoft.Win32.Registry.ClassesRoot.DeleteSubKey(Extension);
						}
						
						// V1.0での登録を解除 (ｱｾﾝﾌﾞﾘ名で登録するように変更)
						foreach(string strApName in new string[]{ AppName, "ESRIJapanGISLightVer1.0" }) {
							// ｱﾌﾟﾘｹｰｼｮﾝ登録の削除
							regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(AppName, true);
							if(regKey != null) {
								// ｻﾌﾞ･ｷｰを削除
								foreach(string strSubKey in regKey.GetSubKeyNames()) {
									regKey.DeleteSubKeyTree(strSubKey);
								}
								Microsoft.Win32.Registry.ClassesRoot.DeleteSubKey(AppName);
							}
						}						
					}
					catch(Exception ex) {
						blnRet = false;
#if DEBUG
						System.Diagnostics.Debug.WriteLine("UNREG ERROR" + ex.Message);
#endif
					}
				}
			}
			
			return blnRet;
		}

		static public string[] GetEsriSubKeys() {
			List<string>	arrSubKeys = new List<string>();

			// ﾚｼﾞｽﾄﾘ
			Microsoft.Win32.RegistryKey	regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ESRI", false);
			arrSubKeys.AddRange(regKey.GetSubKeyNames());

			regKey.Close();

			return arrSubKeys.ToArray();	
		}

		static public string GetEngineRegistryKey() {
			string	strRet = null;

			// Engineのﾚｼﾞｽﾄﾘ･ｷｰを取得します
			try {
				strRet = GetEsriSubKeys().Single(r=>r.StartsWith("Engine"));
			}
			catch {
				//
			}

			return strRet;
		}

		//UnhandledExceptionイベントハンドラ
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			try {
				//エラーメッセージを表示する
				MessageBox.Show(((Exception)e.ExceptionObject).Message, "エラー");
			}
			finally {
				//アプリケーションを終了する
				Environment.Exit(1);
			}
		}

		//ThreadExceptionイベントハンドラ
		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
			try {
				//エラーメッセージを表示する
				MessageBox.Show(e.Exception.Message, "エラー");
			}
			finally {
				//アプリケーションを終了する
				Application.Exit();
			}
		}
    }
}