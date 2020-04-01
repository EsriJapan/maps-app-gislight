using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// スプリット・マージ設定
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
	///  2012-07-31 追加 
    /// </history>
    public partial class FormSplitAndMergeSetting : Form
    {        
        private IMapControl3	m_mapControl;
        private Ui.MainForm		mainFrm;
        SplitAndMargeSettings	m_splitAndMergeSettings;        
        private IFeatureLayer	_agFLayer = null;					// 編集対象ﾚｲﾔｰ

        //※ 編集ポリシーを追加するときの手順
        //① 列挙型にポリシー値を追加する。
	#region 編集ポリシーの列挙型（enum）群

        //スプリットのポリシー（数値型フィールド）
        private enum EnumPolicySplitNumField 
        { 
            POLICY_SPLIT_NUM_FIELD_0, 
            POLICY_SPLIT_NUM_FIELD_1
        };

        //スプリットのポリシー（日付型フィールド）
        private enum EnumPolicySplitDateField 
        { 
            POLICY_SPLIT_DATE_FIELD_0, 
            POLICY_SPLIT_DATE_FIELD_1 
        };

        //スプリットのポリシー（その他のフィールド）
        private enum EnumPolicySplitOtherField 
        { 
            POLICY_SPLIT_OTHER_FIELD_0
        };

        //マージのポリシー（数値型フィールド）
        private enum EnumPolicyMergeNumField 
        {
            POLICY_MERGE_NUM_FIELD_0,
            POLICY_MERGE_NUM_FIELD_1,
            POLICY_MERGE_NUM_FIELD_2
        };

        //マージのポリシー（日付型フィールド）
        private enum EnumPolicyMergeDateField
        {
            POLICY_MERGE_DATE_FIELD_0,
            POLICY_MERGE_DATE_FIELD_1,
            POLICY_MERGE_DATE_FIELD_2
        }

        //マージのポリシー（その他のフィールド）
        private enum EnumPolicyMergeOtherField
        {
            POLICY_MERGE_OTHER_FIELD_0,
            POLICY_MERGE_OTHER_FIELD_1
        }

    #endregion

        //② ①で追加したポリシーに対応する日本語文字列を追加する。
    #region 編集ポリシーの日本語文字列を返すメソッド群

        /// <summary>
        /// 編集ポリシーの日本語文字列を返す。
        /// スプリット：数値フィールド
        /// </summary>
        /// <param name="enumPolicy">編集ポリシーの列挙（enum）</param>
        /// <returns>文字列</returns>
        private string ConvertPolicySplitNum_JapaneseText(EnumPolicySplitNumField enumPolicy) {
            string policyJapaneseText = "";

            switch (enumPolicy) {
                case EnumPolicySplitNumField.POLICY_SPLIT_NUM_FIELD_0:
                    policyJapaneseText = "元のデータを使用する。";
                    break;
                case EnumPolicySplitNumField.POLICY_SPLIT_NUM_FIELD_1:
                    policyJapaneseText = "分割した面積比で割ったデータを使用する。";
                    break;
                default:
                    //エラーメッセージ
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);
                    Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);

                    this.Dispose();

                    break;
            }

            return policyJapaneseText;
        }

        /// <summary>
        /// 編集ポリシーの日本語文字列を返す。
        /// スプリット：日付フィールド
        /// </summary>
        /// <param name="enumPolicy">編集ポリシーの列挙（enum）</param>
        /// <returns>文字列</returns>
        private string ConvertPolicySplitDate_JapaneseText(EnumPolicySplitDateField enumPolicy) {
            string policyJapaneseText = "";

            switch (enumPolicy) {
                case EnumPolicySplitDateField.POLICY_SPLIT_DATE_FIELD_0:
                    policyJapaneseText = "元のデータを使用する。";
                    break;
                case EnumPolicySplitDateField.POLICY_SPLIT_DATE_FIELD_1:
                    policyJapaneseText = "現在の日付を使用する。";
                    break;
                default:
                    //エラーメッセージ
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);
                    Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);

                    this.Dispose();

                    break;
            }

            return policyJapaneseText;
        }

        /// <summary>
        /// 編集ポリシーの日本語文字列を返す。
        /// スプリット：その他のフィールド
        /// </summary>
        /// <param name="enumPolicy">編集ポリシーの列挙（enum）</param>
        /// <returns>文字列</returns>
        private string ConvertPolicySplitOther_JapaneseText(EnumPolicySplitOtherField enumPolicy) {
            string policyJapaneseText = "";
            
            switch (enumPolicy) {
                case EnumPolicySplitOtherField.POLICY_SPLIT_OTHER_FIELD_0:
                    policyJapaneseText = "元のデータを使用する。";
                    break;                    
                default:
                    //エラーメッセージ
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);
                    Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);

                    this.Dispose();

                    break;
            }

            return policyJapaneseText;
        }

        /// <summary>
        /// 編集ポリシーの日本語文字列を返す。
        /// マージ：数値フィールド
        /// </summary>
        /// <param name="enumPolicy">編集ポリシーの列挙（enum）</param>
        /// <returns>文字列</returns>
        private string ConvertPolicyMergeNum_JapaneseText(EnumPolicyMergeNumField enumPolicy)
        {
            string policyJapaneseText = "";
          
            switch (enumPolicy)
            {
                case EnumPolicyMergeNumField.POLICY_MERGE_NUM_FIELD_0:
                    policyJapaneseText = "1番大きい面積のデータを使用する。";
                    break;
                case EnumPolicyMergeNumField.POLICY_MERGE_NUM_FIELD_1:
                    policyJapaneseText = "1番小さい面積のデータを使用する。";
                    break;
                case EnumPolicyMergeNumField.POLICY_MERGE_NUM_FIELD_2:
                    policyJapaneseText = "合計したデータを使用する。";
                    break;
                default:
                    //エラーメッセージ
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);
                    Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);

                    this.Dispose();

                    break;
            }

            return policyJapaneseText;
        }

        /// <summary>
        /// 編集ポリシーの日本語文字列を返す。
        /// マージ：日付フィールド
        /// </summary>
        /// <param name="enumPolicy">編集ポリシーの列挙（enum）</param>
        /// <returns>文字列</returns>
        private string ConvertPolicyMergeDate_JapaneseText(EnumPolicyMergeDateField enumPolicy)
        {
            string policyJapaneseText = "";
                      
                switch (enumPolicy)
                {
                    case EnumPolicyMergeDateField.POLICY_MERGE_DATE_FIELD_0:
                        policyJapaneseText = "1番大きい面積のデータを使用する。";
                        break;
                    case EnumPolicyMergeDateField.POLICY_MERGE_DATE_FIELD_1:
                        policyJapaneseText = "1番小さい面積のデータを使用する。";
                        break;
                    case EnumPolicyMergeDateField.POLICY_MERGE_DATE_FIELD_2:
                        policyJapaneseText = "現在の日付を使用する。";
                        break;
                    default:
                        //エラーメッセージ
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);
                        Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);

                        this.Dispose();

                        break;
                }

            return policyJapaneseText;
        }

        /// <summary>
        /// 編集ポリシーの日本語文字列を返す。
        /// マージ：その他のフィールド
        /// </summary>
        /// <param name="enumPolicy">編集ポリシーの列挙（enum）</param>
        /// <returns>文字列</returns>
        private string ConvertPolicyMergeOther_JapaneseText(EnumPolicyMergeOtherField enumPolicy)
        {
            string policyJapaneseText = "";
            
                switch (enumPolicy)
                {
                    case EnumPolicyMergeOtherField.POLICY_MERGE_OTHER_FIELD_0:
                        policyJapaneseText = "1番大きい面積のデータを使用する。";
                        break;
                    case EnumPolicyMergeOtherField.POLICY_MERGE_OTHER_FIELD_1:
                        policyJapaneseText = "1番小さい面積のデータを使用する。";
                        break;
                    default:
                        //エラーメッセージ
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);
                        Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_UnexpectedPolicy);

                        this.Dispose();

                        break;
                }

            return policyJapaneseText;
        }

    #endregion

        //③ ①でポリシーを追加した列挙型のfor文終了条件を修正する。
    #region コンボボックスに編集ポリシーを追加するメソッド群

        /// <summary>
        /// コンボボックスに編集ポリシーを追加
        /// スプリット:数値型フィールド
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        private void AddComboBoxPolicySplitNum(ComboBox combobox)
        {
            for (EnumPolicySplitNumField enumPolicy = EnumPolicySplitNumField.POLICY_SPLIT_NUM_FIELD_0;
                 enumPolicy <= EnumPolicySplitNumField.POLICY_SPLIT_NUM_FIELD_1;
                 enumPolicy++)
            {
                string strPolicy = ConvertPolicySplitNum_JapaneseText(enumPolicy);

                //編集ポリシーの「パラメータ値」と「文字列」をペアでコンボボックスに追加する。
                ConstComboItem policyItem = new ConstComboItem((int)enumPolicy, strPolicy);

                combobox.Items.Add(policyItem);
            }
        }

        /// <summary>
        /// コンボボックスに編集ポリシーを追加
        /// スプリット:日付型フィールド
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        private void AddComboBoxPolicySplitDate(ComboBox combobox)
        {
            for (EnumPolicySplitDateField enumPolicy = EnumPolicySplitDateField.POLICY_SPLIT_DATE_FIELD_0;
                 enumPolicy <= EnumPolicySplitDateField.POLICY_SPLIT_DATE_FIELD_1;
                 enumPolicy++)
            {
                string strPolicy = ConvertPolicySplitDate_JapaneseText(enumPolicy);

                //編集ポリシーの「パラメータ値」と「文字列」をペアでコンボボックスに追加する。
                ConstComboItem policyItem = new ConstComboItem((int)enumPolicy, strPolicy);

                combobox.Items.Add(policyItem);
            }
        }

        /// <summary>
        /// コンボボックスに編集ポリシーを追加
        /// スプリット:その他のフィールド
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        private void AddComboBoxPolicySplitOther(ComboBox combobox)
        {
            for (EnumPolicySplitOtherField enumPolicy = EnumPolicySplitOtherField.POLICY_SPLIT_OTHER_FIELD_0;
                 enumPolicy <= EnumPolicySplitOtherField.POLICY_SPLIT_OTHER_FIELD_0;
                 enumPolicy++)
            {
                string strPolicy = ConvertPolicySplitOther_JapaneseText(enumPolicy);

                //編集ポリシーの「パラメータ値」と「文字列」をペアでコンボボックスに追加する。
                ConstComboItem policyItem = new ConstComboItem((int)enumPolicy, strPolicy);

                combobox.Items.Add(policyItem);
            }
        }

        /// <summary>
        /// コンボボックスに編集ポリシーを追加
        /// マージ:数値型フィールド
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        private void AddComboBoxPolicyMergeNum(ComboBox combobox)
        {
            for (EnumPolicyMergeNumField enumPolicy = EnumPolicyMergeNumField.POLICY_MERGE_NUM_FIELD_0;
                 enumPolicy <= EnumPolicyMergeNumField.POLICY_MERGE_NUM_FIELD_2;
                 enumPolicy++)
            {
                string strPolicy = ConvertPolicyMergeNum_JapaneseText(enumPolicy);

                //編集ポリシーの「パラメータ値」と「文字列」をペアでコンボボックスに追加する。
                ConstComboItem policyItem = new ConstComboItem((int)enumPolicy, strPolicy);

                combobox.Items.Add(policyItem);
            }
        }

        /// <summary>
        /// コンボボックスに編集ポリシーを追加
        /// マージ:日付型フィールド
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        private void AddComboBoxPolicyMergeDate(ComboBox combobox)
        {
            for (EnumPolicyMergeDateField enumPolicy = EnumPolicyMergeDateField.POLICY_MERGE_DATE_FIELD_0;
                 enumPolicy <= EnumPolicyMergeDateField.POLICY_MERGE_DATE_FIELD_2;
                 enumPolicy++)
            {
                string strPolicy = ConvertPolicyMergeDate_JapaneseText(enumPolicy);

                //編集ポリシーの「パラメータ値」と「文字列」をペアでコンボボックスに追加する。
                ConstComboItem policyItem = new ConstComboItem((int)enumPolicy, strPolicy);

                combobox.Items.Add(policyItem);
            }
        }

        /// <summary>
        /// コンボボックスに編集ポリシーを追加
        /// マージ:その他のフィールド
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        private void AddComboBoxPolicyMergeOther(ComboBox combobox)
        {
            for (EnumPolicyMergeOtherField enumPolicy = EnumPolicyMergeOtherField.POLICY_MERGE_OTHER_FIELD_0;
                 enumPolicy <= EnumPolicyMergeOtherField.POLICY_MERGE_OTHER_FIELD_1;
                 enumPolicy++)
            {
                string strPolicy = ConvertPolicyMergeOther_JapaneseText(enumPolicy);

                //編集ポリシーの「パラメータ値」と「文字列」をペアでコンボボックスに追加する。
                ConstComboItem policyItem = new ConstComboItem((int)enumPolicy, strPolicy);

                combobox.Items.Add(policyItem);
            }
        }

    #endregion

    #region 選択状態のポリシーを設定するメソッド

        /// <summary>
        /// 設定ファイル(GISLight10Settings.xml)で設定されている編集ポリシーを
        /// コンボボックスで選択されている状態にする。
        /// </summary>
        /// <param name="combobox">コンボボックス</param>        
        private void SetSelectedIndex(ComboBox combobox)
        {
            bool result = false;
            
            string policyValue = "";

            try
            {
                switch (combobox.Name)
                {
                    case "comboBoxSplitNumField":
                        policyValue = m_splitAndMergeSettings.EditorSplitNumField;
                        break;
                    case "comboBoxSplitDateField":
                        policyValue = m_splitAndMergeSettings.EditorSplitDateField;
                        break;
                    case "comboBoxSplitOtherField":
                        policyValue = m_splitAndMergeSettings.EditorSplitField;
                        break;
                    case "comboBoxMergeNumField":
                        policyValue = m_splitAndMergeSettings.EditorMargeNumField;
                        break;
                    case "comboBoxMergeDateField":
                        policyValue = m_splitAndMergeSettings.EditorMargeDateField;
                        break;
                    case "comboBoxMergeOtherField":
                        policyValue = m_splitAndMergeSettings.EditorMargeField;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                switch (combobox.Name)
                {
                    case "comboBoxSplitNumField":
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ スプリット：数値型フィールドの属性編集方法 ]" +
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error
                            (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ スプリット：数値型フィールドの属性編集方法 ]" +
                             Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        this.Dispose();
                        break;

                    case "comboBoxSplitDateField":
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ スプリット：日付型フィールドの属性編集方法 ]" +
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error
                            (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ スプリット：日付型フィールドの属性編集方法 ]" +
                             Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        this.Dispose();
                        break;

                    case "comboBoxSplitOtherField":
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ スプリット：その他フィールドの属性編集方法 ]" +
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error
                            (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ スプリット：その他フィールドの属性編集方法 ]" +
                             Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        this.Dispose();
                        break;

                    case "comboBoxMergeNumField":
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ マージ：数値型フィールドの属性編集方法 ]" +
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error
                            (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ マージ：数値型フィールドの属性編集方法 ]" +
                             Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        this.Dispose();
                        break;

                    case "comboBoxMergeDateField":
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ マージ：日付型フィールドの属性編集方法 ]" +
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error
                            (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ マージ：日付型フィールドの属性編集方法 ]" +
                             Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        this.Dispose();
                        break;

                    case "comboBoxMergeOtherField":
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this,
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ マージ：その他フィールドの属性編集方法 ]" +
                            Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error
                            (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                            "[ マージ：その他フィールドの属性編集方法 ]" +
                             Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        Common.Logger.Error(ex.Message);
                        Common.Logger.Error(ex.StackTrace);
                        this.Dispose();
                        break;

                    default:
                        break;
                }
            }
                        
            int intPolicyValue = int.Parse(policyValue);

            for (int i = 0; i < combobox.Items.Count; i++)
            {
                ConstComboItem policyItem = (ConstComboItem)combobox.Items[i];

                if (policyItem.Id == intPolicyValue)
                {
                    combobox.SelectedIndex = intPolicyValue;

                    result = true;
                }
            }

            //ポリシーの設定ファイルに不正なパラメータ値（数値）が設定されていた場合
            if (result == false)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_BadPolicySetting);
                Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_BadPolicySetting);

                this.Dispose();
            }
        }
    #endregion

    #region 設定ファイルを編集するメソッド

        /// <summary>
        /// 設定ファイル(GISLight10Settings.xml)の更新
        /// 指定した要素の「パラメータ値」を
        /// コンボボックスで選択している編集ポリシーのIDに変更する。
        /// </summary>
        /// <param name="combobox">コンボボックス</param>        
        private void UpdatePolicyXML(ComboBox combobox)
        {
            //選択アイテムを取得
            ConstComboItem policyItem = (ConstComboItem)combobox.SelectedItem;

            //設定ファイル(GISLight10Settings.xml)の更新
            switch (combobox.Name)
            {
                case "comboBoxSplitNumField":
                    m_splitAndMergeSettings.EditorSplitNumField = policyItem.Id.ToString();
                    break;
                case "comboBoxSplitDateField":
                    m_splitAndMergeSettings.EditorSplitDateField = policyItem.Id.ToString();
                    break;
                case "comboBoxSplitOtherField":
                    m_splitAndMergeSettings.EditorSplitField = policyItem.Id.ToString();
                    break;
                case "comboBoxMergeNumField":
                    m_splitAndMergeSettings.EditorMargeNumField = policyItem.Id.ToString();
                    break;
                case "comboBoxMergeDateField":
                    m_splitAndMergeSettings.EditorMargeDateField = policyItem.Id.ToString();
                    break;
                case "comboBoxMergeOtherField":
                    m_splitAndMergeSettings.EditorMargeField = policyItem.Id.ToString();
                    break;
                default:
                    break;
            }            

            //m_splitAndMergeSettings.SaveSettings();
        }

    #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormSplitAndMergeSetting(IMapControl3 mapControl) {
            try { 
                Common.Logger.Info("スプリット・マージ設定フォームの起動");

                InitializeComponent();

                //設定ファイルが存在するか確認する
                if(!ApplicationInitializer.IsUserSettingsExists())
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this, 
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                    this.Dispose();
                }       

                this.m_mapControl = mapControl;

				IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
				System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
				mainFrm = (Ui.MainForm)cntrl2.FindForm();

                try {
                    //設定ファイル
                    m_splitAndMergeSettings = new SplitAndMargeSettings();
                }
                catch(Exception ex) {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット・マージの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット・マージの属性編集方法 ]" +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    this.Dispose();
                }            
                           
                //コンボボックスにポリシーを追加（スプリット：数値型フィールド）
                AddComboBoxPolicySplitNum(comboBoxSplitNumField);
                //コンボボックスにポリシーを追加（スプリット：日付型フィールド）
                AddComboBoxPolicySplitDate(comboBoxSplitDateField);
                //コンボボックスにポリシーを追加（スプリット：その他のフィールド）
                AddComboBoxPolicySplitOther(comboBoxSplitOtherField);

                //コンボボックスにポリシーを追加（マージ：数値型フィールド）
                AddComboBoxPolicyMergeNum(comboBoxMergeNumField);
                //コンボボックスにポリシーを追加（マージ：日付型フィールド）
                AddComboBoxPolicyMergeDate(comboBoxMergeDateField);
                //コンボボックスにポリシーを追加（マージ：その他のフィールド）
                AddComboBoxPolicyMergeOther(comboBoxMergeOtherField);


                //選択状態のポリシーを設定（コンボボックスのselectedItem）
                SetSelectedIndex(comboBoxSplitNumField);
                SetSelectedIndex(comboBoxSplitDateField);
                SetSelectedIndex(comboBoxSplitOtherField);
                SetSelectedIndex(comboBoxMergeNumField);
                SetSelectedIndex(comboBoxMergeDateField);
                SetSelectedIndex(comboBoxMergeOtherField);
                
                // ｼﾞｵﾃﾞｰﾀﾍﾞｰｽ･ﾀﾌﾞの初期化
                this.InitGeoDatabaseForms(this.m_mapControl);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_FormLoad);
                Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_FormLoad);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Dispose();
            }
        }

		/// <summary>
		/// フォーム・ロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Load(object sender, EventArgs e) {
			// ﾄﾞﾒｲﾝ設定ﾋﾞｭｰの列幅を調整
			if(this.listView1.Columns.Count > 0) {
				this.listView1.Columns[0].Width = this.listView1.Columns.Count > 1 ? 160 : -2;
				for(int intCnt=1; intCnt < this.listView1.Columns.Count; intCnt++) {
					this.listView1.Columns[intCnt].Width = -2;
				}
			}
			
			// 注意ﾒｯｾｰｼﾞを非表示
			this.panelMsg.Visible = false;
		}
		        
        /// <summary>
        /// OKボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e) {
            try {
                Common.Logger.Info("スプリット・マージ設定ファイルの更新");

                //設定ファイルが存在するか確認する
                if (!ApplicationInitializer.IsUserSettingsExists()) {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                    return;
                }

                //設定ファイル(GISLight10Settings.xml)の更新
                UpdatePolicyXML(comboBoxSplitNumField);
                UpdatePolicyXML(comboBoxSplitDateField);
                UpdatePolicyXML(comboBoxSplitOtherField);
                UpdatePolicyXML(comboBoxMergeNumField);
                UpdatePolicyXML(comboBoxMergeDateField);
                UpdatePolicyXML(comboBoxMergeOtherField);                                

                //更新の保存
                m_splitAndMergeSettings.SaveSettings();
                
                // ｼﾞｵﾃﾞｰﾀﾍﾞｰｽ設定の実行 ※ｼﾞｵDBﾚｲﾔｰが存在する場合
                if(this.comboBoxLayers.Items.Count > 0) {
					this.SetGeoDatabaseDomain();
				}

                // 設定完了時に閉じる (基本設定時のみ)
                if(this.tabControl1.SelectedIndex == 0) {
					this.Close();
				}
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FromSplitAndMergeSetting_ERROR_UpdateXML);
                Common.Logger.Error(Properties.Resources.FromSplitAndMergeSetting_ERROR_UpdateXML);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }            
        }

        /// <summary>
        /// 「閉じる」ボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e) {
            // ﾌｫｰﾑを閉じる
            this.Close();
        }

        /// <summary>
        /// フォーム クローズ時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSplitAndMergeSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            Common.Logger.Info("スプリット・マージ設定フォームの終了");
        }


        private void CheckExistOptionSettingFile()
        {
            Common.ApplicationInitializer.IsUserSettingsExists();
        }

		/// <summary>
		/// ジオデータベース・レイヤーの有無を判定します
		/// </summary>
		/// <returns>あり / なし</returns>
		private bool HasGeoDBLayers() {
			int		intCnt = 0;
			ESRIJapan.GISLight10.Common.LayerManager	clsLMan = new LayerManager();
            foreach(IFeatureLayer agFLayer in clsLMan.GetFeatureLayers(this.m_mapControl.Map)) {
				// ｼﾞｵDBﾚｲﾔｰ数をｶｳﾝﾄ
				if(ESRIJapan.GISLight10.Common.LayerManager.IsGeoDB(ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(agFLayer))) {
					++intCnt;
				}
			}
						
			// 返却
			return intCnt > 0;
		}
		
#region ジオデータベース設定タブ内コントロール
        /// <summary>
        /// ジオデータベース設定用コントロールの初期処理を実行します
        /// </summary>
        private void InitGeoDatabaseForms(ESRI.ArcGIS.Controls.IMapControl3 mapControl) {
            // ｺﾝﾄﾛｰﾙ制御
			this.label_Nothing.Visible = false;
            this.listView1.CheckBoxes = true;
            this.listView1.MultiSelect = false;

			ESRIJapan.GISLight10.Common.LayerManager	clsLMan = new LayerManager();
            foreach(IFeatureLayer agFLayer in clsLMan.GetFeatureLayers(this.m_mapControl.Map)) {
				// ｼｪｲﾌﾟ型をﾁｪｯｸ ﾎﾟﾘｺﾞﾝのみ
				if(agFLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) {
					// ﾃﾞｰﾀｿｰｽをﾁｪｯｸ
					if(ESRIJapan.GISLight10.Common.LayerManager.IsGeoDB(ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(agFLayer))) {
						// 設定候補に追加
						this.comboBoxLayers.Items.Add(new LayerComboItem(agFLayer));
					}
				}
			}
			
			// 既定ﾚｲﾔｰを設定
			this.ChangeFeatureLayer(0);
        }

	#region コントロール・イベント

		/// <summary>
		/// 設定レイヤー変更 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Layer_SelectedIndexChanged(object sender, EventArgs e) {
			if(this.comboBoxLayers.SelectedIndex >= 0) {
				// 選択ﾚｲﾔｰを保存
				this._agFLayer = (IFeatureLayer)((LayerComboItem)this.comboBoxLayers.SelectedItem).Layer;

				// ﾌｨｰﾙﾄﾞ･ﾘｽﾄを更新
				if(this.SetFieldList(this._agFLayer, FieldManager.FieldCategory.Numeric)) {
					this.listView1.Visible = true;
					this.label_Nothing.Visible = false;
	            }
	            else {
					this.listView1.Visible = false;
					this.label_Nothing.Visible = true;
	            }
	            
	            // 注意ﾒｯｾｰｼﾞ制御
	            this.panelMsg.Visible = false;
			}
		}

		/// <summary>
		/// タブ・切り替え イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tab_SelectedIndexChanged(object sender, EventArgs e) {
			TabControl	ctlTab = sender as TabControl;
			
			// 注意書きの表示制御
			this.panelMsg.Visible = false;
		}

		/// <summary>
		/// リストビュー・チェック変更 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
			this.panelMsg.Visible = true;

            // 2012/08/28 add >>>>>
            // 設定不可のチェックボックスをOFFにする
            ListViewItem liFld  = e.Item as ListViewItem;
            if (liFld.ForeColor == Color.Gray)
            {
                liFld.Checked = false;
                this.panelMsg.Visible = false;
            }
            // 2012/08/28 add <<<<<
		}

	#endregion

	#region 内部メソッド
		/// <summary>
		/// ドメインの
		/// </summary>
		private void SetGeoDatabaseDomain() {
			// 対象ﾚｲﾔｰをﾁｪｯｸ
			if(this._agFLayer != null) {
				// ﾌｨｰﾁｬｰｸﾗｽを取得
				IFeatureClass	agFClass = this._agFLayer.FeatureClass;

				// ﾄﾞﾒｲﾝ生成用変数を準備
				IDomain	agRatioDomain;
				IDomain	agWorkDomain;
				string	strDName;
				
				ITableFields	agTblFld = (ITableFields)this._agFLayer;
				IField			agFld;
				bool			blnDomainRemoved = false;		// ﾄﾞﾒｲﾝ設定解除ﾌﾗｸﾞ
				
				// ﾌｨｰﾙﾄﾞを取得
				foreach(ListViewItem liFld in this.listView1.Items) {
					// ﾘｽﾄ･ﾀｸﾞにﾌｨｰﾙﾄﾞ･ｲﾝﾃﾞｯｸｽが埋め込まれている　※ﾕｰｻﾞｰ･ﾄﾞﾒｲﾝ設定済みﾌｨｰﾙﾄﾞは対象外
					if(liFld.Tag != null && liFld.Tag is int && liFld.ForeColor != Color.Gray) {
						// ﾌｨｰﾙﾄﾞを取得
						agFld = agTblFld.get_Field(Convert.ToInt32(liFld.Tag));
						
						// ﾄﾞﾒｲﾝを取得
						agWorkDomain = agFld.Domain;
						
						// スプリットﾄﾞﾒｲﾝを設定
						if(liFld.Checked && agWorkDomain == null) {
                            // スプリットﾄﾞﾒｲﾝ名を取得
                            strDName = Common.DomainManager.GetDomainName(agFld.Type);

                            // 既存のスプリットﾄﾞﾒｲﾝを取得
                            agRatioDomain = Common.DomainManager.FindDomain(agFClass, strDName);

                            // スプリットﾄﾞﾒｲﾝを作成
                            if (agRatioDomain == null) {
								agRatioDomain = Common.DomainManager.CreateSplitRatioDomain(strDName, "面積按分比率に応じた数値を自動入力します", agFld.Type);
								
								if(!Common.DomainManager.AddDomain(agFClass, agRatioDomain)) {
									Common.MessageBoxManager.ShowMessageBoxError("ドメインを追加できませんでした。");
								}
							}
							
							// ﾌｨｰﾙﾄﾞにﾄﾞﾒｲﾝを設定
							try {
								Common.DomainManager.SetDomain(agFClass, agFld.Name, agRatioDomain);
							}
							catch(Exception ex) {
								Common.MessageBoxManager.ShowMessageBoxError(ex.Message);
							}
						}
                        // スプリットﾄﾞﾒｲﾝを解除
                        else if (!liFld.Checked && agWorkDomain != null) {
                            // スプリットﾄﾞﾒｲﾝを対象
                            if (agWorkDomain.Name.StartsWith(Common.DomainManager.DOMAIN_NAME_SPLIT)) {
								try {
									blnDomainRemoved = Common.DomainManager.SetDomain(agFClass, agFld.Name, null);
								}
								catch(Exception ex) {
									Common.MessageBoxManager.ShowMessageBoxError(ex.Message);
								}
							}
						}

						// COMﾘﾘｰｽ
						if(agWorkDomain != null) {
							Marshal.ReleaseComObject(agWorkDomain);
						}
					}
				}

                // 未使用のスプリットﾄﾞﾒｲﾝを削除 (ﾄﾞﾒｲﾝ設定解除時)
                if (blnDomainRemoved) {
					Common.DomainManager.RemoveSplitDomains(agFClass);
				}
				
				// COM解放
				Marshal.ReleaseComObject(agFClass);
				agFClass = null;
				
				// 注意ﾒｯｾｰｼﾞを非表示
				this.panelMsg.Visible = false;
				
				// 表示を更新
				this.Layer_SelectedIndexChanged(this.comboBoxLayers, null);
			}
		}

		/// <summary>
		/// 設定対象レイヤーを変更します
		/// </summary>
		private void ChangeFeatureLayer(IFeatureLayer FLayer) {
			if(FLayer != null) {
				// ﾚｲﾔｰ選択ｺﾝﾎﾞの選択状態を変更
				foreach(LayerComboItem itemLayer in this.comboBoxLayers.Items) {
					if(itemLayer.Layer.Equals(this._agFLayer)) {
						this.comboBoxLayers.SelectedItem = itemLayer;
						break;
					}
				}
			}
		}
		
		/// <summary>
		/// 設定対象レイヤーを変更します
		/// </summary>
		private void ChangeFeatureLayer(int ItemIndex) {
			if(ItemIndex >= 0 && this.comboBoxLayers.Items.Count > ItemIndex) {
				// 選択状態を変更
				this.comboBoxLayers.SelectedIndex = ItemIndex;
			}
			else if(this.comboBoxLayers.Items.Count <= 0) {
				this.comboBoxLayers.Enabled = false;
				this.listView1.Visible = false;
				this.label_Nothing.Visible = true;
				this.label_Nothing.Location = this.listView1.Location;
				this.label_Nothing.Size = this.listView1.Size;
				
				this.label_Nothing.Text = "対象のジオデータベース・レイヤーはありません。";
			}
		}

		#region リストビュー関連
		/// <summary>
		/// フィールドリストを作成します
		/// </summary>
		/// <param name="FLayer">フィーチャーレイヤー</param>
		/// <param name="FldCategory">大分類（Unknown=全て対象）</param>
		/// <returns>対象ﾌｨｰﾙﾄﾞあり / なし</returns>
		private bool SetFieldList(IFeatureLayer FLayer, FieldManager.FieldCategory FldCategory) {
			// ﾌｨｰﾙﾄﾞ･ﾘｽﾄ 更新開始
			this.listView1.BeginUpdate();

			// 既存のﾘｽﾄを全削除
			for(int intCnt=this.listView1.Items.Count - 1; intCnt >= 0; intCnt--) {
				this.listView1.Items.RemoveAt(intCnt);
			}

			if(FLayer != null && FLayer.Valid) {
				// 純ﾌｨｰﾙﾄﾞ群を取得 (数値型ﾄﾞﾒｲﾝ設定可能ﾌｨｰﾙﾄﾞ)
				Common.FieldComboItem[]	cmbFlds = this.mainFrm.GetFieldItems(FLayer, false, true, true, true, 
														esriFieldType.esriFieldTypeSmallInteger,
														esriFieldType.esriFieldTypeInteger,
														esriFieldType.esriFieldTypeSingle,
														esriFieldType.esriFieldTypeDouble);

				IFeatureClass	agFC = FLayer.FeatureClass;
				IField			agFld;
				IDomain			agWorkDomain;
				string			strDomain;
				ListViewItem	lviFld;
				foreach(FieldComboItem fi in cmbFlds) {
					// ﾌｨｰﾙﾄﾞを取得
					agFld = fi.Field;

					// 延長,周長,面積ﾌｨｰﾙﾄﾞは除外
				    if(!(agFld.Equals(agFC.AreaField) || agFld.Equals(agFC.LengthField))) {
						// ﾘｽﾄを作成
						lviFld = new ListViewItem();
						
						// ﾌｨｰﾙﾄﾞ名を表示
						lviFld.Text = fi.FieldAlias;
						
						// ﾃﾞｰﾀ型を表示
						lviFld.SubItems.Add(FieldComboItem.getDataTypeString(agFld.Type));

						// ﾄﾞﾒｲﾝを表示
						agWorkDomain = agFld.Domain;
						if(agWorkDomain != null) {
							strDomain = agWorkDomain.Name + " : " + Convert.ToString(agWorkDomain.Description);
							
							if(!strDomain.StartsWith(Common.DomainManager.DOMAIN_NAME_SPLIT)) {
								// 異なるﾄﾞﾒｲﾝが設定されているﾌｨｰﾙﾄﾞは設定非対象とする
								lviFld.ForeColor = Color.Gray;
							}
							else {
								// 按分設定されていることを明示
								lviFld.Checked = true;
							}
							
							Marshal.ReleaseComObject(agWorkDomain);
						}
						else {
							strDomain = "";
						}
						lviFld.SubItems.Add(strDomain);
						
						// ﾀｸﾞにﾌｨｰﾙﾄﾞ名を埋め込んでおく
						lviFld.Tag = FLayer.FeatureClass.FindField(fi.FieldName);
						
						// ﾘｽﾄに追加
						this.listView1.Items.Add(lviFld);
				    }
				}
			}
			
			// ﾌｨｰﾙﾄﾞ･ﾘｽﾄ更新終了
			this.listView1.EndUpdate();
			
			// 返却
			return this.listView1.Items.Count > 0;
		}

		/// <summary>
		/// 選択リストのフィールドを取得します
		/// </summary>
		private IField[] SelectedFields {
			get {
				return this.GetFields(this.listView1.SelectedItems.Cast<ListViewItem>());
			}
		}

		/// <summary>
		/// チェックされたリストのフィールドを取得します
		/// </summary>
		private IField[] CheckedFields {
			get {
				return this.GetFields(this.listView1.CheckedItems.Cast<ListViewItem>());
			}
		}
		
		/// <summary>
		/// 全リストのフィールドを取得します
		/// </summary>
		private IField[] AllFields {
			get {
				return this.GetFields(this.listView1.Items.Cast<ListViewItem>());
			}
		}

		/// <summary>
		/// 指定のリスト群からフィールド群を取得します
		/// </summary>
		/// <param name="ItemLists">対象とするリストアイテム群</param>
		/// <returns>フィールド群</returns>
		private IField[] GetFields(IEnumerable<ListViewItem> ItemLists) {
			List<IField>	agFlds = new List<IField>();
			
			// 状況ﾁｪｯｸ
			if(this._agFLayer != null && ItemLists.Count() > 0) {
				ITableFields	itfFL = (ITableFields)this._agFLayer;
				
				// ﾌｨｰﾙﾄﾞを取得
				foreach(ListViewItem liFld in ItemLists) {
					// ﾘｽﾄ･ﾀｸﾞにﾌｨｰﾙﾄﾞ･ｲﾝﾃﾞｯｸｽが埋め込まれている　※ﾕｰｻﾞｰ･ﾄﾞﾒｲﾝ設定済みﾌｨｰﾙﾄﾞは対象外
					if(liFld.Tag != null && liFld.Tag is int && liFld.ForeColor != Color.Gray) {
						agFlds.Add(itfFL.get_Field(Convert.ToInt32(liFld.Tag)));
					}
				}
			}
			
			// 返却
			return agFlds.ToArray();
		}	
		#endregion
		
	#endregion

#endregion
	}
}
