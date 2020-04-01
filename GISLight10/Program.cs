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
		/// �A�v���P�[�V�����N���p�����[�^
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
                    MessageBox.Show(string.Format("ArcGIS Desktop �������� ArcGIS Engine �̃��C�Z���X�����o�ł��Ȃ��ׁA{0}���N���ł��܂���B", Properties.Resources.APP_PRODUCT_NAME));
                    return;
                }

                //Application.EnableVisualStyles(); // XP�ŃJ���[�����v���\���ł��Ȃ��Ȃ�̂ŃR�����g�A�E�g
                Application.SetCompatibleTextRenderingDefault(false);

				// ThreadException�C�x���g�n���h����ǉ�
				Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
				// ThreadException���������Ȃ��悤�ɂ���
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

				// UnhandledException�C�x���g�n���h����ǉ�
				System.AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                // �N�����Ұ����擾���܂�
                StartArguments = args.Length <= 0 ? null : args;
                
                // �N��
                Application.Run(new Ui.MainForm());
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message,"�\�����ʃG���[",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                Common.Logger.Fatal(ex.Source);
                Common.Logger.Fatal(ex.Message);
                Common.Logger.Fatal(ex.StackTrace);
            }
        }

        /// <summary>
        /// �t�@�C���̊֘A�t�����ݒ肳��Ă��邩�ǂ������m�F���܂�
        /// </summary>
        /// <param name="Extension">�t�@�C���g���q</param>
        /// <returns></returns>
        static public bool IsRelateMXD(string Extension) {
			bool	blnRet = false;
			
			// �g���q�̕⊮
			if(!Extension.StartsWith(".")) {
				Extension = Extension.Insert(0, ".");
			}

			// �g���q�ɱ���
			Microsoft.Win32.RegistryKey	regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Extension);
			if(regKey != null && regKey.ValueCount > 0) {
				// �l���擾
				string	strVal = (string)regKey.GetValue("");
				
				// �����ؖ����擾
				Assembly	assm = Assembly.GetExecutingAssembly();
				string		strAsmName = assm.GetName().Name;
				
				blnRet = strVal.StartsWith(strAsmName);
			}
			
			return blnRet;
        }
        
        /// <summary>
        /// ArcGIS���̓o�^�L�����ȈՓI�Ƀ`�F�b�N���܂�
        /// </summary>
        /// <returns></returns>
        static public bool ExistArcGIS() {
			bool	blnRet = false;
			
			// �g���q�ɱ���
			Microsoft.Win32.RegistryKey	regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".mxt");
			if(regKey != null && regKey.ValueCount > 0) {
				// �l������
				string	strVal = (string)regKey.GetValue("");
				if(!string.IsNullOrEmpty(strVal)) {
					blnRet = strVal.ToUpper() == "ARCMAPTEMPLATE";
				}
			}
			
			return blnRet;
        }
        
		/// <summary>
		/// �t�@�C���̊֘A�t�������s���܂�
		/// </summary>
		/// <param name="Extension">�t�@�C���g���q</param>
		/// <param name="IsRegist">�o�^�^����</param>
		/// <returns></returns>
		static public bool RegistDataFile(string FilePath, string AppName, bool IsRegist) {
			bool	blnRet = true;
			
			string[]	REGIST_EXTENSIONS = { ".mxd", ".pmf" };
			
			// ڼ޽��
			Microsoft.Win32.RegistryKey	regKey;
			
			// ̧�ق̊֘A�t����o�^
			if(IsRegist) {
				// ���ع���݂̐���
				string	strAppDesc = "GIS Light";
				// ����ݖ�
				string	strAct = "open";
				// ����݂̐���
				string	strActDesc = strAppDesc + "�ŊJ�� (&O)";

				// ���s���ع���ݥ�߽�̕⊮ (����ޥײ�)
				string	strAppCmdLine = string.Format("\"{0}\" %1", FilePath);
				
				// ���݂̐ݒ�
				string	strIconPath = FilePath;
				int		intIconID = 0;
				
				foreach(string Extension in REGIST_EXTENSIONS) {
					// �g���q��o�^
					try {
						regKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(Extension);
						regKey.SetValue("", AppName);
						regKey.Close();
						
						// ���ع���ݖ���o�^
						regKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(AppName);
						regKey.SetValue("", strAppDesc);
						
						// ����݂�o�^
						regKey = regKey.CreateSubKey(@"shell\" + strAct);
						regKey.SetValue("", strActDesc);
						
						// ����ޥײ݂�o�^
						regKey = regKey.CreateSubKey("command");
						regKey.SetValue("", strAppCmdLine);
						regKey.Close();
						
						// ���݂̓o�^
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
			// ̧�ق̊֘A�t��������
			else {
				foreach(string Extension in REGIST_EXTENSIONS) {
					try {
						// ArcGIS�ݽİُ󋵂�����
						if(ExistArcGIS()) {
							// �g���q�̓o�^���e�����Ƃɖ߂�
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
							// �o�^���폜
							Microsoft.Win32.Registry.ClassesRoot.DeleteSubKey(Extension);
						}
						
						// V1.0�ł̓o�^������ (�����ؖ��œo�^����悤�ɕύX)
						foreach(string strApName in new string[]{ AppName, "ESRIJapanGISLightVer1.0" }) {
							// ���ع���ݓo�^�̍폜
							regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(AppName, true);
							if(regKey != null) {
								// ��ޥ�����폜
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

			// ڼ޽��
			Microsoft.Win32.RegistryKey	regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ESRI", false);
			arrSubKeys.AddRange(regKey.GetSubKeyNames());

			regKey.Close();

			return arrSubKeys.ToArray();	
		}

		static public string GetEngineRegistryKey() {
			string	strRet = null;

			// Engine��ڼ޽�إ�����擾���܂�
			try {
				strRet = GetEsriSubKeys().Single(r=>r.StartsWith("Engine"));
			}
			catch {
				//
			}

			return strRet;
		}

		//UnhandledException�C�x���g�n���h��
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			try {
				//�G���[���b�Z�[�W��\������
				MessageBox.Show(((Exception)e.ExceptionObject).Message, "�G���[");
			}
			finally {
				//�A�v���P�[�V�������I������
				Environment.Exit(1);
			}
		}

		//ThreadException�C�x���g�n���h��
		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
			try {
				//�G���[���b�Z�[�W��\������
				MessageBox.Show(e.Exception.Message, "�G���[");
			}
			finally {
				//�A�v���P�[�V�������I������
				Application.Exit();
			}
		}
    }
}