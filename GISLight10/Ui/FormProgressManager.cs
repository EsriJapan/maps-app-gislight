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
    /// プログレスバー表示
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormProgressManager : Form
    {
        private delegate void IncrementProgressBarCallback(int value);
        private delegate void SetProgressBarMaximumCallback(int maxValue);
        private delegate void CloseFormCallback();
        private delegate void SetMessageCallback(string message);
        private delegate void SetTitleCallback(Form owner);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormProgressManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// プログレスバーを指定した値だけ進める
        /// </summary>
        /// <param name="value">進める値</param>
        public void IncrementProgressBar(int value)
        {
            if (this.InvokeRequired)
            {
                IncrementProgressBarCallback dlgt = new IncrementProgressBarCallback(IncrementProgressBar);
                this.Invoke(dlgt, new object[] { value });
            }
            else
            {
                if (this.progressBar.Value == this.progressBar.Maximum)
                {
                    this.progressBar.Value = this.progressBar.Minimum;
                }
                else
                {
                    this.progressBar.Increment(value);
                }
            }
        }

        /// <summary>
        /// プログレスバーの最大値を設定
        /// </summary>
        /// <param name="maxVal">最大値</param>
        public void SetProgressbarMaximum(int maxVal)
        {
            if (this.InvokeRequired)
            {
                SetProgressBarMaximumCallback dlgt = new SetProgressBarMaximumCallback(SetProgressbarMaximum);
                this.Invoke(dlgt, new object[] { maxVal });
            }
            else
            {
                this.progressBar.Maximum = maxVal;
            }
        }

        /// <summary>
        /// フォームを閉じる。
        /// </summary>
        public void CloseForm()
        {
            if (this.InvokeRequired)
            {
                CloseFormCallback dlgt = new CloseFormCallback(CloseForm);
                this.Invoke(dlgt);
            }
            else
            {
                this.Close();
                //this.Dispose();
            }
        }

        /// <summary>
        /// メッセージを設定
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void SetMessage(string message)
        {
            if (this.InvokeRequired)
            {
                SetMessageCallback d = new SetMessageCallback(SetMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                this.labelMessage.Text = message;
                labelMessage.Update();
            }
        }

        /// <summary>
        /// タイトルを設定
        /// </summary>
        /// <param name="owner">呼び出し元フォーム</param>
        public void SetTitle(Form owner)
        {
            if (this.InvokeRequired)
            {
                SetTitleCallback d = new SetTitleCallback(SetTitle);
                this.Invoke(d, new object[] { owner });
            }
            else
            {
                try
                {
                    this.Text = GetMessageBoxTitle(owner);
                }
                catch
                {
                    this.Text = Properties.Resources.CommonMessage_ApplicationName;
                }
            }
        }

        /// <summary>
        /// タイトルを取得
        /// </summary>
        /// <param name="parentForm">親ウインドウ</param>
        /// <returns>Properties.Resources.CommonMessage_ApplicationName + " - " + parentForm.Text</returns>
        /// <returns>Properties.Resources.CommonMessage_ApplicationName</returns>
        private static string GetMessageBoxTitle(Form parentForm)
        {
            if (parentForm.Text != Properties.Resources.CommonMessage_ApplicationName)
            {
                return Properties.Resources.CommonMessage_ApplicationName + " - " + parentForm.Text;
            }
            else
            {
                return Properties.Resources.CommonMessage_ApplicationName;
            }
        }
    }
}
