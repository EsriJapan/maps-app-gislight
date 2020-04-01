using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// オプション設定機能
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成 
    ///  2011/06/01 フィールド名タブ追加
    /// </history>
    public partial class FormOptionSettings : Form
    {
        // タブインデックス
        private const int TAB_TEMPORARY_FOLDER = 0;
        private const int TAB_MAX_RECORDS = 1;
        private const int TAB_RESET_SETTINGS = 3;//2;
        private const int TAB_FILED_NAME = 2;
        private const string FIELD_NAME_USE_ALIAS = "フィールド名の別名使用判定フラグ";
        private const string GEOPROCESSING_BACKGROUND= "ジオプロセシングのバックグラウンド処理使用フラグ";

        // ユーザのテンポラリフォルダ以下のサブディレクトリ名
        static private string APPLICATION_DIRECTORY_NAME =
            Properties.Settings.Default.TemporaryDirectoryPath;

        // 最大レコード数設定項目名
        private const string ATTRIBUTE_TABLE_DISPAY_OID_MAX = "1ページあたりの表示最大レコード数";
        private const string INDIVIDUAL_VALUE_DISPLAY_MAX = "個別値取得最大数";
        private const string UNIQUE_VALUE_MAX = "個別値分類の最大分類数";

        // 最大レコード数設定最大最小数
        private string MinAttributeTableDisplayOIDMax;
        private string MaxAttributeTableDisplayOIDMax;
        private string MinIndividualValueDisplayMax;
        private string MaxIndividualValueDisplayMax;
        private string MinUniqueValueMax;
        private string MaxUniqueValueMax;

        // 最小数
        private const int MinValue = 1;

        // パフォーマンス警告数
        private const int ATTRIBUTE_TABLE_DISPAY_OID_MAX_WARNING_NUM = 1000;
        private const int INDIVIDUAL_VALUE_DISPLAY_MAX_WARNING_NUM = 500;
        private const int UNIQUE_VALUE_MAX_WARNING_NUM = 500;

        //private Common.XMLAccessClass settingsFile = null;
        private Common.OptionSettings settingsFile = null;

		#region プロパティ
		/// <summary>
		/// テンポラリ・フォルダーのパスを取得します
		/// </summary>
		static public string UserTemporaryFolder {
			get {
				// テンポラリフォルダのパス生成
				StringBuilder fullPath = new StringBuilder();
				fullPath.Append(Path.GetTempPath());
				fullPath.Append(APPLICATION_DIRECTORY_NAME);
				
				string	temporaryFolderPath = fullPath.ToString();
				// テンポラリフォルダがない場合は作成
				if (!Directory.Exists(temporaryFolderPath))
				{
					Directory.CreateDirectory(temporaryFolderPath);
				}

				return temporaryFolderPath;
			}
		}
		
		/// <summary>
		/// 設定ファイルが保存されているフォルダーのパスを取得します
		/// </summary>
		static public string UserSettingFileFolder {
			get {
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

				return fullPath.ToString();
			}
		}
		#endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormOptionSettings()
        {
            Logger.Info("オプション設定開始");

            try
            {
                InitializeComponent();

                // 各UIを設定
                SetupTabTemporaryFolder();
                SetupTabMaxRecords();
                SetupTabResetSettings();
                SetupTabFieldName();
                SetupTabGeopro();
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                            this, Properties.Resources.FormOptionSettings_ERROR_Initialize);
                Common.Logger.Error(Properties.Resources.FormOptionSettings_ERROR_Initialize);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Close();
            }
            
            // MXD関連付けの状態をﾁｪｯｸ
            //tabControlOptionSettings.TabPages.Remove(this.tabPageFileRelate); //現在のﾊﾞｰｼﾞｮﾝでは無効
            this.checkBoxRelateMXD.Checked = Program.IsRelateMXD("mxd");
        }

        /// <summary>
        /// Formクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormOptionSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Dispose();

            Logger.Info("オプション設定終了");
        }

        /// <summary>
        /// テンポラリフォルダタブ初期化
        /// </summary>
        private void SetupTabTemporaryFolder()
        {
            // テンポラリフォルダのパスを設定
            textBoxTemporaryFolderPath.Text = UserTemporaryFolder;
        }

        /// <summary>
        /// 最大レコード数タブ初期化
        /// </summary>
        private void SetupTabMaxRecords()
        {
            //settingsFile = new Common.XMLAccessClass();

            //// 設定ファイルロード
            //settingsFile.LoadXMLDocument();

            //// 各設定項目に値を設定
            //textBoxAttributeTableDisplayOIDMax.Text = settingsFile.GetXMLValue("AttributeTableDisplayOIDMax");
            //textBoxIndividualValueDisplayMax.Text = settingsFile.GetXMLValue("IndividualValueDisplayMax");

            if (Common.ApplicationInitializer.IsUserSettingsExists() == false)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                return;
            }

            try
            {
                settingsFile = new Common.OptionSettings();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    //+ "[ " + ATTRIBUTE_TABLE_DISPAY_OID_MAX+ " ]\n"
                    //+ "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]\n"
                    //+ "[ " + UNIQUE_VALUE_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    //+ "[ " + ATTRIBUTE_TABLE_DISPAY_OID_MAX+ " ]\n"
                    //+ "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]\n"
                    //+ "[ " + UNIQUE_VALUE_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }

            // 各設定項目に値を設定
            try
            {
                textBoxAttributeTableDisplayOIDMax.Text = settingsFile.AttributeTableDisplayOIDMax;

                MaxAttributeTableDisplayOIDMax = settingsFile.AttributeTableDisplayOIDMaxMax;
                //MinAttributeTableDisplayOID = settingsFile.AttributeTableDisplayOIDMaxMin;
                MinAttributeTableDisplayOIDMax = MinValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + ATTRIBUTE_TABLE_DISPAY_OID_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + ATTRIBUTE_TABLE_DISPAY_OID_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }

            try
            {
                textBoxIndividualValueDisplayMax.Text = settingsFile.IndividualValueDisplayMax;

                MaxIndividualValueDisplayMax = settingsFile.IndividualValueDisplayMaxMax;
                //MinIndividualValueDisplay = settingsFile.IndividualValueDisplayMaxMin;
                MinIndividualValueDisplayMax = MinValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }

            try
            {
                textBoxUniqueValueMax.Text = settingsFile.UniqueValueMax;

                MaxUniqueValueMax = settingsFile.UniqueValueMaxMax;
                //MinUniqueValueMax = settingsFile.UniqueValueMaxMin;
                MinUniqueValueMax = MinValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + UNIQUE_VALUE_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + UNIQUE_VALUE_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }
        }

        /// <summary>
        /// フィールド名タブ初期化
        /// </summary>
        private void SetupTabFieldName()
        {
            try
            {
                this.checkBoxUseFieldNameAlias.Checked = false;

                settingsFile = new Common.OptionSettings();
                if (Common.UtilityClass.IsNumeric(settingsFile.FieldNmaeUseAlias))
                {
                    this.checkBoxUseFieldNameAlias.Checked = 
                        (Convert.ToInt32(settingsFile.FieldNmaeUseAlias) == 1);
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_USE_ALIAS + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_USE_ALIAS + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }
        }

        /// <summary>
        /// ジオプロセシングタブ初期化
        /// </summary>
        private void SetupTabGeopro()
        {
            try 
            {
                this.checkBoxGpBackground.Checked = false;

                settingsFile = new Common.OptionSettings();
                if (Common.UtilityClass.IsNumeric(settingsFile.GeoprocessingBackground))
                {
                    this.checkBoxGpBackground.Checked =
                        (Convert.ToInt32(settingsFile.GeoprocessingBackground) == 1);
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + GEOPROCESSING_BACKGROUND + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + GEOPROCESSING_BACKGROUND + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }
        }

        /// <summary>
        /// 設定リセットタブ初期化
        /// </summary>
        private void SetupTabResetSettings()
        {
            // 設定ファイルの保存フォルダを取得
            StringBuilder fullPath = new StringBuilder(UserSettingFileFolder);

            // 設定ファイルのパス生成
            fullPath.Append("\\");
            fullPath.Append(Properties.Settings.Default.UserSettingsXmlName);
            
            // 設定ファイルのパスを設定
            textBoxSettingFilePath.Text = fullPath.ToString();
        }

        /// <summary>
        /// テンポラリファイル削除(削除ボタン)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteTemporaryFile_Click(object sender, EventArgs e)
        {
            try
            {
                string targetDirectory = textBoxTemporaryFolderPath.Text;

                // テンポラリファイル削除確認
                if (Directory.Exists(targetDirectory))
                {
                    DialogResult resDeleteFile = Common.MessageBoxManager.ShowMessageBoxWarining2(
                        this, Properties.Resources.FormOptionSettings_WARNING_DeleteTemporaryFile);

                    if (resDeleteFile == DialogResult.OK)
                    {
                        // テンポラリフォルダ内のファイルを削除
                        List<string> filesList = null;
                        List<string> directorysList = null;

                        Common.FileManager.DeleteFilesInDirectory(targetDirectory, out filesList, out directorysList);

                        //if (filesList.Count > 0)
                        //{
                        //    foreach (string fileName in filesList)
                        //    {
                        //        Logger.Debug(fileName);
                        //    }
                        //}

                        //if (directorysList.Count > 0)
                        //{
                        //    foreach (string directoryName in directorysList)
                        //    {
                        //        Logger.Debug(directoryName);
                        //    }
                        //}
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                            this, Properties.Resources.FormOptionSettings_ERROR_DeleteTemporaryFile);
                Common.Logger.Error(Properties.Resources.FormOptionSettings_ERROR_DeleteTemporaryFile);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// パラメータチェック
        /// </summary>
        /// <param name="message">ダイアログ表示メッセージ</param>
        /// <returns>true or false</returns>
        private bool CheckParameters(ref string message)
        {
            //int attributeTableDisplayOIDMax = Convert.ToInt32(textBoxAttributeTableDisplayOIDMax.Text);
            //int individualValueDisplayMax = Convert.ToInt32(textBoxIndividualValueDisplayMax.Text);

            // 1. 入力された値が数値かチェック
            int attributeTableDisplayOIDMax = -1;
            int individualValueDisplayMax = -1;
            int uniqueValueMax = -1;

            if (!int.TryParse(textBoxAttributeTableDisplayOIDMax.Text, out attributeTableDisplayOIDMax))
            {
                //message = ATTRIBUTE_TABLE_DISPAY_OID_MAX
                //    + Properties.Resources.FormOptionSettings_WARNING_NotNumeric;

                message = ATTRIBUTE_TABLE_DISPAY_OID_MAX
                    + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
                    + "[ " + MinAttributeTableDisplayOIDMax
                    + " - " + MaxAttributeTableDisplayOIDMax + " ]";

                return false;
            }

            if (!int.TryParse(textBoxIndividualValueDisplayMax.Text, out individualValueDisplayMax))
            {
                //message = INDIVIDUAL_VALUE_DISPLAY_MAX
                //    + Properties.Resources.FormOptionSettings_WARNING_NotNumeric;

                message = INDIVIDUAL_VALUE_DISPLAY_MAX
                    + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
                    + "[ " + MinIndividualValueDisplayMax
                    + " - " + MaxIndividualValueDisplayMax + " ]";

                return false;
            }

            if (!int.TryParse(textBoxUniqueValueMax.Text, out uniqueValueMax))
            {
                message = UNIQUE_VALUE_MAX
                    + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
                    + "[ " + MinUniqueValueMax
                    + " - " + MaxUniqueValueMax + " ]";

                return false;
            }

            //if (!(attributeTableDisplayOIDMax >= MinAttributeTableDisplayOID 
            //    && attributeTableDisplayOIDMax <= MaxAttributeTableDisplayOID))
            //{
            //    message = ATTRIBUTE_TABLE_DISPAY_OID_MAX
            //        + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
            //        + "[ " + MinAttributeTableDisplayOID + " - " + MaxAttributeTableDisplayOID + " ]"; 

            //    return false;
            //}

            //if (!(individualValueDisplayMax >= MinIndividualValueDisplay &&
            //    individualValueDisplayMax <= MaxIndividualValueDisplay))
            //{
            //    message = INDIVIDUAL_VALUE_DISPLAY_MAX
            //        + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
            //        + "[ " + MinIndividualValueDisplay + " - " + MaxIndividualValueDisplay + " ]";

            //    return false;
            //}

            // 2. 入力された値が最大値最小値の範囲内かチェック
            int attributeTableDisplayOIDMaxMax = -1;
            int attributeTableDisplayOIDMaxMin = -1;
            int individualValueDisplayMaxMax = -1;
            int individualValueDisplayMaxMin = -1;
            int uniqueValueMaxMax = -1;
            int uniqueValueMaxMin = -1;

            //if (!int.TryParse(settingsFile.AttributeTableDisplayOIDMaxMax, out attributeTableDisplayOIDMaxMax)
            //    || !int.TryParse(settingsFile.AttributeTableDisplayOIDMaxMin, out attributeTableDisplayOIDMaxMin)
            //    || !int.TryParse(settingsFile.IndividualValueDisplayMaxMax, out individualValueDisplayMaxMax)
            //    || !int.TryParse(settingsFile.IndividualValueDisplayMaxMin, out individualValueDisplayMaxMin)
            //    || !int.TryParse(settingsFile.UniqueValueMaxMax, out uniqueValueMaxMax)
            //    || !int.TryParse(settingsFile.UniqueValueMaxMin, out uniqueValueMaxMin))
            if (!int.TryParse(MaxAttributeTableDisplayOIDMax, out attributeTableDisplayOIDMaxMax)
                || !int.TryParse(MinAttributeTableDisplayOIDMax, out attributeTableDisplayOIDMaxMin)
                || !int.TryParse(MaxIndividualValueDisplayMax, out individualValueDisplayMaxMax)
                || !int.TryParse(MinIndividualValueDisplayMax, out individualValueDisplayMaxMin)
                || !int.TryParse(MaxUniqueValueMax, out uniqueValueMaxMax)
                || !int.TryParse(MinUniqueValueMax, out uniqueValueMaxMin))
            {
                message = Properties.Resources.FormOptionSettings_ERROR_InvalidSettingFile;

                return false;
            }

            if (attributeTableDisplayOIDMaxMax < attributeTableDisplayOIDMaxMin
                || individualValueDisplayMaxMax < individualValueDisplayMaxMin
                || uniqueValueMaxMax < uniqueValueMaxMin)
            {
                message = Properties.Resources.FormOptionSettings_ERROR_InvalidSettingFile;

                return false;
            }

            if (!(attributeTableDisplayOIDMax >= attributeTableDisplayOIDMaxMin
                && attributeTableDisplayOIDMax <= attributeTableDisplayOIDMaxMax))
            {
                message = ATTRIBUTE_TABLE_DISPAY_OID_MAX
                    + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
                    + "[ " + attributeTableDisplayOIDMaxMin
                    + " - " + attributeTableDisplayOIDMaxMax + " ]";

                return false;
            }

            if (!(individualValueDisplayMax >= individualValueDisplayMaxMin
                && individualValueDisplayMax <= individualValueDisplayMaxMax))
            {
                message = INDIVIDUAL_VALUE_DISPLAY_MAX
                    + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
                    + "[ " + individualValueDisplayMaxMin
                    + " - " + individualValueDisplayMaxMax + " ]";

                return false;
            }

            if (!(uniqueValueMax >= uniqueValueMaxMin
                && uniqueValueMax <= uniqueValueMaxMax))
            {
                message = UNIQUE_VALUE_MAX
                    + Properties.Resources.FormOptionSettings_WARNING_OutOfRange
                    + "[ " + uniqueValueMaxMin
                    + " - " + uniqueValueMaxMax + " ]";

                return false;
            }

            // テキストボックスに再反映
            textBoxAttributeTableDisplayOIDMax.Text = attributeTableDisplayOIDMax.ToString();
            textBoxIndividualValueDisplayMax.Text = individualValueDisplayMax.ToString();
            textBoxUniqueValueMax.Text = uniqueValueMax.ToString();

            return true;
        }

        /// <summary>
        /// パフォーマンス警告チェック
        /// </summary>
        /// <returns>true or false</returns>
        private bool CheckPerfomance()
        {
            int attributeTableDisplayOIDMax = int.Parse(textBoxAttributeTableDisplayOIDMax.Text);
            int individualValueDisplayMax = int.Parse(textBoxIndividualValueDisplayMax.Text);
            int uniqueValueMax = int.Parse(textBoxUniqueValueMax.Text);

            if (attributeTableDisplayOIDMax > ATTRIBUTE_TABLE_DISPAY_OID_MAX_WARNING_NUM)
            {
                DialogResult resAttributeTableDisplayOIDMax = MessageBoxManager.ShowMessageBoxWarining2(
                    this, 
                    ATTRIBUTE_TABLE_DISPAY_OID_MAX
                    + " に "
                    + ATTRIBUTE_TABLE_DISPAY_OID_MAX_WARNING_NUM
                    + Properties.Resources.FormOptionSettings_WARNING_GoodPerformance);

                if (resAttributeTableDisplayOIDMax == DialogResult.Cancel)
                {
                    return false;
                }
            }

            if (individualValueDisplayMax > INDIVIDUAL_VALUE_DISPLAY_MAX_WARNING_NUM)
            {
                DialogResult resIndividualValueDisplayMax = MessageBoxManager.ShowMessageBoxWarining2(
                    this,
                    INDIVIDUAL_VALUE_DISPLAY_MAX
                    + " に "
                    + INDIVIDUAL_VALUE_DISPLAY_MAX_WARNING_NUM
                    + Properties.Resources.FormOptionSettings_WARNING_GoodPerformance);

                if (resIndividualValueDisplayMax == DialogResult.Cancel)
                {
                    return false;
                }
            }

            if (uniqueValueMax > UNIQUE_VALUE_MAX_WARNING_NUM)
            {
                DialogResult resUniqueValueMax = MessageBoxManager.ShowMessageBoxWarining2(
                    this,
                    UNIQUE_VALUE_MAX
                    + " に "
                    + UNIQUE_VALUE_MAX_WARNING_NUM
                    + Properties.Resources.FormOptionSettings_WARNING_GoodPerformance);

                if (resUniqueValueMax == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// テキストボックス入力キーチェック(数字のみ)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 設定ファイルへ設定を反映(OKボタン)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                // フィールド名タブ追加に伴い、OK時のカレントページ判断はコメントアウト 2011/06/01
                //if (tabControlOptionSettings.SelectedIndex == TAB_MAX_RECORDS)
                //{
                    string warning_message = null;

                    // 入力チェック
                    if (!CheckParameters(ref warning_message))
                    {
                        Common.MessageBoxManager.ShowMessageBoxWarining(this, warning_message);
                        return;
                    }

                    // パフォーマンス警告チェック
                    if (!CheckPerfomance())
                    {
                        return;
                    }
                //}

                // フィールド名タブ追加に伴い、OK時のカレントページ判断はコメントアウト 2011/06/01
                //if (tabControlOptionSettings.SelectedIndex == TAB_MAX_RECORDS)
                //{
                    //// 設定ファイルへ値を設定
                    //settingsFile.SetXMLValue("AttributeTableDisplayOIDMax", textBoxAttributeTableDisplayOIDMax.Text);
                    //settingsFile.SetXMLValue("IndividualValueDisplayMax", textBoxIndividualValueDisplayMax.Text);

                    //// 設定ファイルセーブ
                    //settingsFile.SaveXMLDocument();

                    // 設定ファイルへ値を設定
                    try
                    {
                        settingsFile.AttributeTableDisplayOIDMax = textBoxAttributeTableDisplayOIDMax.Text;
                    }
                    catch (Exception ex)
                    {
                        MessageBoxManager.ShowMessageBoxError(this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + ATTRIBUTE_TABLE_DISPAY_OID_MAX + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + ATTRIBUTE_TABLE_DISPAY_OID_MAX + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);

                        return;
                    }

                    try
                    {
                        settingsFile.IndividualValueDisplayMax = textBoxIndividualValueDisplayMax.Text;
                    }
                    catch (Exception ex)
                    {
                        MessageBoxManager.ShowMessageBoxError(this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);

                        return;
                    }

                    try
                    {
                        settingsFile.UniqueValueMax = textBoxUniqueValueMax.Text;
                    }
                    catch (Exception ex)
                    {
                        MessageBoxManager.ShowMessageBoxError(this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + UNIQUE_VALUE_MAX + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + UNIQUE_VALUE_MAX + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);

                        return;
                    }

                    // フィールド名別名使用スイッチ2011/06/01 add -->
                    try
                    {
                        if (this.checkBoxUseFieldNameAlias.Checked)
                        {
                            settingsFile.FieldNmaeUseAlias = "1";
                        }
                        else
                        {
                            settingsFile.FieldNmaeUseAlias = "0";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxManager.ShowMessageBoxError(this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + FIELD_NAME_USE_ALIAS + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                        Common.Logger.Error(
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + FIELD_NAME_USE_ALIAS + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        return;
                    } // <--


                    // ジオプロセシングのバックグラウンド処理2012/08/16 add
                    try 
                    {
                        if (this.checkBoxGpBackground.Checked)
                        {
                            settingsFile.GeoprocessingBackground = "1";
                        }
                        else 
                        {
                            settingsFile.GeoprocessingBackground = "0";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxManager.ShowMessageBoxError(this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                        + "[ " + GEOPROCESSING_BACKGROUND + " ]"
                        + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                        Common.Logger.Error(
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + "[ " + GEOPROCESSING_BACKGROUND + " ]"
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        return;

                    }
                    
                    // MXDﾌｧｲﾙ関連付け処理を定義
                    Action<bool>	execReg = (IsReg) => {
						// ｱｾﾝﾌﾞﾘ名を取得
						Assembly	assm = Assembly.GetExecutingAssembly();
						string		strAsmName = assm.GetName().Name;

						// MXD関連付け (ｱｾﾝﾌﾞﾘ名で登録)
						//Program.RegistDataFile(Application.ExecutablePath, strAsmName, true);
						ProcessStartInfo	proc = new ProcessStartInfo(Path.Combine(Application.StartupPath, "RegESRIJapanSetting.exe"), string.Join(" ", new string[] { "\"" + Application.ExecutablePath + "\"", strAsmName, IsReg ? "1" : "0" }));
						Process.Start(proc);
                    };

					// MXDﾌｧｲﾙの関連付け
					if(this.checkBoxRelateMXD.Checked && !Program.IsRelateMXD("mxd")) {
						// MXD関連付け (ｱｾﾝﾌﾞﾘ名で登録)
						//Program.RegistDataFile(Application.ExecutablePath, strAsmName, true);
						execReg(true);
					}
					else if(!this.checkBoxRelateMXD.Checked && Program.IsRelateMXD("mxd")) {
						// 関連付け解除
						//Program.RegistDataFile(Application.ExecutablePath, strAsmName, false);
						//Program.RegistDataFile(Application.ExecutablePath, Application.ProductName, false);	// V2.3.0で行なわれた登録を解除
						execReg(false);
					}

                    // 設定ファイルセーブ
                    try
                    {
                        settingsFile.SaveSettings();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxManager.ShowMessageBoxError(this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite
                            + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);

                        return;
                    }
                //}

            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                            this, Properties.Resources.FormOptionSettings_ERROR_SetSettingsFile);
                Common.Logger.Error(Properties.Resources.FormOptionSettings_ERROR_SetSettingsFile);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }

            if (settingsFile != null)
            {
                settingsFile = null;
            }

            this.Close();
       }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            settingsFile = null;

            this.Close();
        }

        /// <summary>
        /// 設定ファイルリセット(リセットボタン)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResetSettingFile_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult resResetSettingFile = Common.MessageBoxManager.ShowMessageBoxWarining2(
                        this, Properties.Resources.FormOptionSettings_WARNING_ResetSettingFile);

                if (resResetSettingFile == DialogResult.OK)
                {
                    if (ApplicationInitializer.IsUserSettingsExists())
                    {
                        ApplicationInitializer.DeleteSettings();
                    }

                    ApplicationInitializer.CreateUserSettings();
                }
                else
                {
                    return;
                }

                // 最大レコード数タブ初期化
                settingsFile = null;
                SetupTabMaxRecords();

                // 2012/08/31 ADD 
                // 別名表示設定初期化
                SetupTabFieldName();
                // ジオプロセシング設定初期化
                SetupTabGeopro();
                // 2012/08/31 ADD 
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                            this, Properties.Resources.FormOptionSettings_ERROR_ResetSettingFile);
                Common.Logger.Error(Properties.Resources.FormOptionSettings_ERROR_ResetSettingFile);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }
    }
}
