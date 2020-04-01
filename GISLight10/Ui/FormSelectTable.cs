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
    public partial class FormSelectTable : Form
    {

        public string tableName;

        public FormSelectTable()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            tableName = this.comboBoxTable.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// PropcomboBoxTableプロパティ
        /// </summary>
        public ComboBox PropcomboBoxTable
        {
            get { return this.comboBoxTable; }
            set { this.PropcomboBoxTable = value; }
        }

    }
}
