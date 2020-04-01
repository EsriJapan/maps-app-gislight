using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace ESRIJapan.GISLight10.Ui
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

    }

#region 呼び出し例
    //ProgressDialog pd = new ProgressDialog();
    //pd.Minimum = 0;
    //pd.Maximum = 10;
    //pd.Title = "プログレスダイアログ";
    //pd.Show(this);
    //for (int i = 0; i < 10; i++)
    //{
    //    if (pd.Canceled)
    //        break;
    //    pd.Message = string.Format("{0}件目の処理中・・・", i+1);
    //    pd.Value = i + 1;
    //    System.Threading.Thread.Sleep(500);
    //}
    //pd.Close();
#endregion
    public class ProgressDialog : IDisposable
    {

        //キャンセルボタンがクリックされたか
        private volatile bool _canceled = false;
        //ダイアログフォーム
        private volatile ProgressForm form;
        //フォームが表示されるまで待機するための待機ハンドル
        private System.Threading.ManualResetEvent startEvent;
        //フォームが一度表示されたか
        private bool showed = false;
        //フォームをコードで閉じているか
        private volatile bool closing = false;
        //オーナーフォーム
        private Form ownerForm;

        //別処理をするためのスレッド
        private System.Threading.Thread thread;

        //フォームのタイトル
        private volatile string _title = "進行状況";
        //ProgressBarの最小、最大、現在の値
        private volatile int _minimum = 0;
        private volatile int _maximum = 100;
        private volatile int _value = 0;
        //表示するメッセージ
        private volatile string _message = "";

        private volatile bool _cancelenable = true;

        //private volatile System.Windows.Forms.ProgressBarStyle _style = ProgressBarStyle.Blocks;

        //public System.Windows.Forms.ProgressBarStyle Style 
        //{
        //    set 
        //    {
        //        _style = value;
        //        if (form != null)
        //            form.Invoke(new MethodInvoker(SetStyleValue));
        //    }
        //    get 
        //    { 
        //        return _style;
        //    }
        //}

        public bool CancelEnable
        {
            set 
            {
                _cancelenable = value;
                if (form != null)
                    form.Invoke(new MethodInvoker(SetCancelButtonEnable));
            }
            get 
            {
                return _cancelenable;
            }
        }

        /// <summary>
        /// ダイアログのタイトルバーに表示する文字列
        /// </summary>
        public string Title
        {
            set
            {
                _title = value;
                if (form != null)
                    form.Invoke(new MethodInvoker(SetTitle));
            }
            get
            {
                return _title;
            }
        }

        /// <summary>
        /// プログレスバーの最小値
        /// </summary>
        public int Minimum
        {
            set
            {
                _minimum = value;
                if (form != null)
                    form.Invoke(new MethodInvoker(SetProgressMinimum));
            }
            get
            {
                return _minimum;
            }
        }

        /// <summary>
        /// プログレスバーの最大値
        /// </summary>
        public int Maximum
        {
            set
            {
                _maximum = value;
                if (form != null)
                    form.Invoke(new MethodInvoker(SetProgressMaximun));
            }
            get
            {
                return _maximum;
            }
        }

        /// <summary>
        /// プログレスバーの値
        /// </summary>
        public int Value
        {
            set
            {
                _value = value;
                if (form != null)
                    form.Invoke(new MethodInvoker(SetProgressValue));
            }
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// ダイアログに表示するメッセージ
        /// </summary>
        public string Message
        {
            set
            {
                _message = value;
                if (form != null)
                    form.Invoke(new MethodInvoker(SetMessage));
            }
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// キャンセルされたか
        /// </summary>
        public bool Canceled
        {
            get { return _canceled; }
        }

        /// <summary>
        /// ダイアログを表示する
        /// </summary>
        /// <param name="owner">
        /// ownerの中央にダイアログが表示される
        /// </param>
        /// <remarks>
        /// このメソッドは一回しか呼び出せません。
        /// </remarks>
        public void Show(Form owner)
        {
            if (showed)
                throw new Exception("ダイアログは一度表示されています。");
            showed = true;

            _canceled = false;
            
            startEvent = new System.Threading.ManualResetEvent(false);
            ownerForm = owner;

            //スレッドを作成
            thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(Run));
            thread.IsBackground = true;
            this.thread.ApartmentState =
                System.Threading.ApartmentState.STA;
            thread.Start();

            //フォームが表示されるまで待機する
            startEvent.WaitOne();
        }
        public void Show()
        {
            Show(null);
        }

        //別スレッドで処理するメソッド
        private void Run()
        {
            //フォームの設定
            form = new ProgressForm();
            form.Text = _title;
            form.buttonCancel.Click += new EventHandler(cancelButton_Click);
            form.Closing += new CancelEventHandler(form_Closing);
            form.Activated += new EventHandler(form_Activated);
            form.progressBar.Minimum = _minimum;
            form.progressBar.Maximum = _maximum;
            form.progressBar.Value = _value;
            form.buttonCancel.Enabled = _cancelenable;
            //フォームの表示位置をオーナーの中央へ
            if (ownerForm != null)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Left =
                    ownerForm.Left + (ownerForm.Width - form.Width) / 2;
                form.Top =
                    ownerForm.Top + (ownerForm.Height - form.Height) / 2;
            }
            //フォームの表示
            form.ShowDialog();

            form.Dispose();
        }

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        public void Close()
        {
            closing = true;
            form.Invoke(new MethodInvoker(form.Close));
        }

        public void Dispose()
        {
            form.Invoke(new MethodInvoker(form.Dispose));
        }

        //private void SetStyleValue()
        //{
        //    if (form != null && !form.IsDisposed)
        //        form.progressBar.Style = _style;
        //}

        private void SetProgressValue()
        {
            if (form != null && !form.IsDisposed)
                form.progressBar.Value = _value;
        }

        private void SetMessage()
        {
            if (form != null && !form.IsDisposed)
                form.labelMessage.Text = _message;
        }

        private void SetTitle()
        {
            if (form != null && !form.IsDisposed)
                form.Text = _title;
        }

        private void SetProgressMaximun()
        {
            if (form != null && !form.IsDisposed)
                form.progressBar.Maximum = _maximum;
        }

        private void SetProgressMinimum()
        {
            if (form != null && !form.IsDisposed)
                form.progressBar.Minimum = _minimum;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _canceled = true;
        }

        private void form_Closing(object sender, CancelEventArgs e)
        {
            if (!closing)
            {
                e.Cancel = true;
                _canceled = true;
            }
        }

        private void form_Activated(object sender, EventArgs e)
        {
            form.Activated -= new EventHandler(form_Activated);
            startEvent.Set();
        }

        private void SetCancelButtonEnable()
        {
            if (form != null && !form.IsDisposed)
                form.buttonCancel.Enabled = _cancelenable;

        }

    }

}
