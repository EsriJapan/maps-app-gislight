using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// バージョン情報
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormVersionInfo : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="verMajor">メジャーバージョン</param>
        /// <param name="verMinor">マイナーバージョン</param>
        /// <param name="verBuild">ビルド番号</param>
        public FormVersionInfo(string verMajor, string verMinor, string verBuild)
        {
            Common.Logger.Info("バージョン情報フォームの起動");

            InitializeComponent();

            labelAppName.Text = Properties.Resources.CommonMessage_ApplicationName;

            labelAppVersion.Text = string.Format("(version {0}.{1}.{2})", verMajor, verMinor, verBuild);
        }

        /// <summary>
        /// OKボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// フォーム クローズ時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormVersionInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            Common.Logger.Info("バージョン情報フォームの終了");
        }        
    }
}
