using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// 編集オプション
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormEditOption : Form
    {
        private IMapControl3 m_mapControl;
        
        private IEngineEditor m_engineEditor;
        IEngineEditProperties m_engineEditProp;
        IEngineEditProperties2 m_engineEditProp2;
        IEngineSnapEnvironment m_engineSnapEnv;

        EditOptionSettings m_editOptionSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormEditOption(IMapControl3 mapControl)
        {
            try
            {
                Common.Logger.Info("編集オプションフォームの起動");

                InitializeComponent();

                //設定ファイルが存在するか確認する
                if (!ApplicationInitializer.IsUserSettingsExists())
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

                try
                {
                    //設定ファイル
                    m_editOptionSettings = new EditOptionSettings();
                }
                catch (Exception ex)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ 編集のオプション ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ 編集のオプション ]" +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    this.Dispose();
                }    

                this.m_mapControl = mapControl;

                m_engineEditor = new EngineEditorClass();

                m_engineEditProp = (IEngineEditProperties)m_engineEditor;
                m_engineEditProp2 = (IEngineEditProperties2)m_engineEditor;
                m_engineSnapEnv = (IEngineSnapEnvironment)m_engineEditor;

                //スナップ許容値
                try
                {
                    textBoxSnapTolerance.Text = m_editOptionSettings.SnapTolerance;
                }
                catch (Exception ex)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スナップ許容値 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スナップ許容値 ]" +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    this.Dispose();
                }
                

                //移動抑制許容値
                try
                {
                    textBoxStickyMoveTolerance.Text = m_editOptionSettings.StickyMoveTolerance;
                }
                catch (Exception ex)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ 移動抑制許容値 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ 移動抑制許容値 ]" +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    this.Dispose();
                }
                

                //ストリーム許容値
                try
                {
                    textBoxStreamTolerance.Text = m_editOptionSettings.StreamTolerance;
                }
                catch (Exception ex)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                       (this,
                       Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                       "[ ストリーム モード：ストリーム許容値 ]" +
                       Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ ストリーム モード：ストリーム許容値 ]" +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    this.Dispose();
                }
                

                //グループ化する頂点数
                try
                {
                    textBoxStreamGroupingCount.Text = m_editOptionSettings.StreamGroupingCount;
                }
                catch (Exception ex)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                       (this,
                       Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                       "[ ストリーム モード：グループ化する頂点数 ]" +
                       Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ ストリーム モード：グループ化する頂点数 ]" +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    this.Dispose();
                }                
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormEditOption_ERROR_Load);
                Common.Logger.Error(Properties.Resources.FormEditOption_ERROR_Load);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormEditOption_ERROR_Load);
                Common.Logger.Error(Properties.Resources.FormEditOption_ERROR_Load);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }                        
        }

        /// <summary>
        /// OKボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                //設定ファイルが存在するか確認する
                if (!ApplicationInitializer.IsUserSettingsExists())
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                    return;
                }

                //入力チェック
                if (CheckTextBoxSnapTolerance() == false)
                {
                    return;
                }
                else if (ChecktextBoxStickyMoveTolerance() == false)
                {
                    return;
                }
                else if (CheckTextBoxStreamTolerance() == false)
                {
                    return;
                }
                else if (CheckTextBoxStreamGroupingCount() == false)
                {
                    return;
                }
                
                //編集オプションの設定値の変更
                SetEditOptions();

                //設定ファイルの更新
                UpdateEditOptionXML();
                
                this.Close();
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormEditOption_ERROR_UpdateXML);
                Common.Logger.Error(Properties.Resources.FormEditOption_ERROR_UpdateXML);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormEditOption_ERROR_UpdateXML);
                Common.Logger.Error(Properties.Resources.FormEditOption_ERROR_UpdateXML);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }                        
        }

        /// <summary>
        /// キャンセルボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        #region 入力チェック

        /// <summary>
        /// スナップ許容値の入力チェック
        /// </summary>
        /// <returns>チェック結果</returns>
        private bool CheckTextBoxSnapTolerance()
        {
            //設定ファイルのmin値以上の整数 かつ 設定ファイルのmax値以下の整数の場合
            //^(0|[1-9]\d{0,6})$
            //0から始まる数字列もOKに変更
            if (//Regex.IsMatch(textBoxSnapTolerance.Text, @"^(0|[1-9]\d*)$") &&
                //!textBoxSnapTolerance.Text.Equals("") &&
                Regex.IsMatch(textBoxSnapTolerance.Text, @"^([0-9]\d*)$") &&
                int.Parse(textBoxSnapTolerance.Text) >= 0 &&
                int.Parse(textBoxSnapTolerance.Text) <= int.Parse(m_editOptionSettings.SnapToleranceMax))
            {
                return true;
            }
            else
            {

                string message = "スナップ許容値" + 
                    Properties.Resources.FormEditOption_WARNING_IntRange + 
                    "[ " + 0 + " - " + m_editOptionSettings.SnapToleranceMax + " ]";

                Common.MessageBoxManager.ShowMessageBoxWarining(this, message);
                Common.Logger.Warn(message);                

                return false;
            }
        }


        /// <summary>
        /// 移動抑制許容値の入力チェック
        /// </summary>
        /// <returns>チェック結果</returns>
        private bool ChecktextBoxStickyMoveTolerance()
        {
            //1桁～8桁までの整数
            //設定ファイルのmin値以上の整数 かつ 設定ファイルのmax値以下の整数の場合
            //0から始まる数字列もOKに変更
            if (//Regex.IsMatch(textBoxStickyMoveTolerance.Text, @"^(0|[1-9]\d*)$") &&
                //!textBoxStickyMoveTolerance.Text.Equals("") &&
                Regex.IsMatch(textBoxStickyMoveTolerance.Text, @"^([0-9]\d*)$") &&
                int.Parse(textBoxStickyMoveTolerance.Text) >= 0 &&
                int.Parse(textBoxStickyMoveTolerance.Text) <= int.Parse(m_editOptionSettings.StickyMoveToleranceMax))
            {
                return true;
            }
            else
            {

                string message = "移動抑制許容値" +
                    Properties.Resources.FormEditOption_WARNING_IntRange +
                    "[ " + 0 + " - " + m_editOptionSettings.StickyMoveToleranceMax + " ]";

                Common.MessageBoxManager.ShowMessageBoxWarining(this, message);
                Common.Logger.Warn(message);   

                return false;
            }
        }


        /// <summary>
        /// ストリーム許容値の入力チェック
        /// </summary>
        /// <returns>チェック結果</returns>
        private bool CheckTextBoxStreamTolerance()
        {
            //1桁～8桁までの実数
            //小数点3桁まで（ArcMapに合わせた）
            //設定ファイルのmin値以上の整数 かつ 設定ファイルのmax値以下の整数の場合
            //0から始まる数字列もOKに変更
            if (//Regex.IsMatch(textBoxStreamTolerance.Text, @"^(0|[1-9]\d*)$") &&
                //!textBoxStreamTolerance.Text.Equals("") &&
                Regex.IsMatch(textBoxStreamTolerance.Text, @"^[0-9]+(\.[0-9]{1,3})?$") &&
                double.Parse(textBoxStreamTolerance.Text) >= 0.0 &&
                double.Parse(textBoxStreamTolerance.Text) <= double.Parse(m_editOptionSettings.StreamToleranceMax))
            {
                return true;
            }
            else
            {

                string message = "ストリーム許容値" +
                    Properties.Resources.FormEditOption_WARNING_IntRange +
                    "[ " + 0 + " - " + m_editOptionSettings.StreamToleranceMax + " ]" +
                    "[ 小数点3桁以内 ]";

                Common.MessageBoxManager.ShowMessageBoxWarining(this, message);
                Common.Logger.Warn(message); 

                return false;
            }
        }


        /// <summary>
        /// グループ化する頂点数の入力チェック
        /// </summary>
        /// <returns>チェック結果</returns>
        private bool CheckTextBoxStreamGroupingCount()
        {
            //1桁～8桁までの整数
            //設定ファイルのmin値以上の整数 かつ 設定ファイルのmax値以下の整数の場合
            //0から始まる数字列もOKに変更
            if (//Regex.IsMatch(textBoxStreamGroupingCount.Text, @"^([1-9]\d*)$") &&
                //!textBoxStreamGroupingCount.Text.Equals("") &&
                Regex.IsMatch(textBoxStreamGroupingCount.Text, @"^([0-9]\d*)$") &&
                int.Parse(textBoxStreamGroupingCount.Text) >= 1 &&
                int.Parse(textBoxStreamGroupingCount.Text) <= int.Parse(m_editOptionSettings.StreamGroupingCountMax))
            {
                return true;
            }
            else
            {

                string message = "グループ化する頂点数" +
                    Properties.Resources.FormEditOption_WARNING_IntRange +
                    "[ " + 1 + " - " + m_editOptionSettings.StreamGroupingCountMax + " ]";

                Common.MessageBoxManager.ShowMessageBoxWarining(this, message);
                Common.Logger.Warn(message); 

                return false;
            }
        }

        #endregion


        /// <summary>
        /// 編集オプションの設定値の更新
        /// </summary>
        private void SetEditOptions()
        {
            //スナップ許容値
            m_engineSnapEnv.SnapTolerance = double.Parse(textBoxSnapTolerance.Text);

            //移動抑制許容値
            m_engineEditProp2.StickyMoveTolerance = int.Parse(textBoxStickyMoveTolerance.Text);

            //ストリーム許容値
            m_engineEditProp.StreamTolerance = double.Parse(textBoxStreamTolerance.Text);

            //グループ化する頂点数
            m_engineEditProp.StreamGroupingCount = int.Parse(textBoxStreamGroupingCount.Text);
        }


        /// <summary>
        /// 設定ファイル(GISLight10Settings.xml)を更新する。
        /// 0から始まる文字列（01,009,010など）を一度int型に変換して、0を除外する。
        /// </summary>
        private void UpdateEditOptionXML()
        {
            //スナップ許容値
            int intTextBoxSnapTolerance = int.Parse(textBoxSnapTolerance.Text);
            m_editOptionSettings.SnapTolerance = intTextBoxSnapTolerance.ToString();


            //移動抑制許容値
            int intTextBoxStickyMoveTolerance = int.Parse(textBoxStickyMoveTolerance.Text);
            m_editOptionSettings.StickyMoveTolerance = intTextBoxStickyMoveTolerance.ToString();


            //ストリーム許容値
            double doubleTextBoxStreamTolerance = double.Parse(textBoxStreamTolerance.Text);
            m_editOptionSettings.StreamTolerance = doubleTextBoxStreamTolerance.ToString();


            //グループ化する頂点数
            int intTextBoxStreamGroupingCount = int.Parse(textBoxStreamGroupingCount.Text);
            m_editOptionSettings.StreamGroupingCount = intTextBoxStreamGroupingCount.ToString();
          

            //上書き保存
            m_editOptionSettings.SaveSettings();
        }

        /// <summary>
        /// フォーム クローズ時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEditOption_FormClosed(object sender, FormClosedEventArgs e)
        {
            Common.Logger.Info("編集オプションフォームの終了");
        }



        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBoxStreamTolerance_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && !(e.KeyChar.Equals('.')))
            {
                e.Handled = true;
            }
        }
    }
}