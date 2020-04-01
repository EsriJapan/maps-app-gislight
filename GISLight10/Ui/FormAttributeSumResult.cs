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
    /// 属性値集計結果表示
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormAttributeSumResult : Form
	{
        /// <summary>
        /// コンストラクタ
        /// </summary>
		public FormAttributeSumResult()
		{
			InitializeComponent();
		}

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "名前を付けて保存";
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

        }
	}
}
