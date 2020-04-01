namespace ESRIJapan.GISLight10.Ui
{
    partial class FormAddAndDeleteField
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelExplanation = new System.Windows.Forms.Label();
            this.tabControlAddAndDeleteField = new System.Windows.Forms.TabControl();
            this.tabPageAddField = new System.Windows.Forms.TabPage();
            this.comboBoxAddFieldType = new System.Windows.Forms.ComboBox();
            this.groupBoxAddFieldProperties = new System.Windows.Forms.GroupBox();
            this.labelAddFieldScale = new System.Windows.Forms.Label();
            this.labelAddFieldPrecision = new System.Windows.Forms.Label();
            this.textBoxAddFieldScale = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
            this.textBoxAddFieldPrecision = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
            this.textBoxAddFieldLength = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
            this.textBoxAddFieldAlias = new System.Windows.Forms.TextBox();
            this.labelAddFieldLength = new System.Windows.Forms.Label();
            this.labelAddFieldAlias = new System.Windows.Forms.Label();
            this.textBoxAddFieldName = new System.Windows.Forms.TextBox();
            this.labelAddFieldType = new System.Windows.Forms.Label();
            this.labelAddFieldName = new System.Windows.Forms.Label();
            this.tabPageDeleteField = new System.Windows.Forms.TabPage();
            this.comboBoxDeleteField = new System.Windows.Forms.ComboBox();
            this.labelDeleteField = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControlAddAndDeleteField.SuspendLayout();
            this.tabPageAddField.SuspendLayout();
            this.groupBoxAddFieldProperties.SuspendLayout();
            this.tabPageDeleteField.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelExplanation
            // 
            this.labelExplanation.AutoSize = true;
            this.labelExplanation.Location = new System.Drawing.Point(8, 8);
            this.labelExplanation.Name = "labelExplanation";
            this.labelExplanation.Size = new System.Drawing.Size(173, 12);
            this.labelExplanation.TabIndex = 0;
            this.labelExplanation.Text = "フィールドの追加と削除を行います。";
            // 
            // tabControlAddAndDeleteField
            // 
            this.tabControlAddAndDeleteField.Controls.Add(this.tabPageAddField);
            this.tabControlAddAndDeleteField.Controls.Add(this.tabPageDeleteField);
            this.tabControlAddAndDeleteField.Location = new System.Drawing.Point(8, 40);
            this.tabControlAddAndDeleteField.Name = "tabControlAddAndDeleteField";
            this.tabControlAddAndDeleteField.SelectedIndex = 0;
            this.tabControlAddAndDeleteField.Size = new System.Drawing.Size(456, 272);
            this.tabControlAddAndDeleteField.TabIndex = 1;
            // 
            // tabPageAddField
            // 
            this.tabPageAddField.Controls.Add(this.comboBoxAddFieldType);
            this.tabPageAddField.Controls.Add(this.groupBoxAddFieldProperties);
            this.tabPageAddField.Controls.Add(this.textBoxAddFieldName);
            this.tabPageAddField.Controls.Add(this.labelAddFieldType);
            this.tabPageAddField.Controls.Add(this.labelAddFieldName);
            this.tabPageAddField.Location = new System.Drawing.Point(4, 22);
            this.tabPageAddField.Name = "tabPageAddField";
            this.tabPageAddField.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAddField.Size = new System.Drawing.Size(448, 246);
            this.tabPageAddField.TabIndex = 0;
            this.tabPageAddField.Text = "フィールドの追加";
            this.tabPageAddField.UseVisualStyleBackColor = true;
            // 
            // comboBoxAddFieldType
            // 
            this.comboBoxAddFieldType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAddFieldType.FormattingEnabled = true;
            this.comboBoxAddFieldType.Location = new System.Drawing.Point(80, 45);
            this.comboBoxAddFieldType.Name = "comboBoxAddFieldType";
            this.comboBoxAddFieldType.Size = new System.Drawing.Size(360, 20);
            this.comboBoxAddFieldType.TabIndex = 3;
            this.comboBoxAddFieldType.SelectedIndexChanged += new System.EventHandler(this.comboBoxAddFieldType_SelectedIndexChanged);
            // 
            // groupBoxAddFieldProperties
            // 
            this.groupBoxAddFieldProperties.Controls.Add(this.labelAddFieldScale);
            this.groupBoxAddFieldProperties.Controls.Add(this.labelAddFieldPrecision);
            this.groupBoxAddFieldProperties.Controls.Add(this.textBoxAddFieldScale);
            this.groupBoxAddFieldProperties.Controls.Add(this.textBoxAddFieldPrecision);
            this.groupBoxAddFieldProperties.Controls.Add(this.textBoxAddFieldLength);
            this.groupBoxAddFieldProperties.Controls.Add(this.textBoxAddFieldAlias);
            this.groupBoxAddFieldProperties.Controls.Add(this.labelAddFieldLength);
            this.groupBoxAddFieldProperties.Controls.Add(this.labelAddFieldAlias);
            this.groupBoxAddFieldProperties.Location = new System.Drawing.Point(8, 80);
            this.groupBoxAddFieldProperties.Name = "groupBoxAddFieldProperties";
            this.groupBoxAddFieldProperties.Size = new System.Drawing.Size(432, 152);
            this.groupBoxAddFieldProperties.TabIndex = 4;
            this.groupBoxAddFieldProperties.TabStop = false;
            this.groupBoxAddFieldProperties.Text = "フィールド プロパティ";
            // 
            // labelAddFieldScale
            // 
            this.labelAddFieldScale.AutoSize = true;
            this.labelAddFieldScale.Location = new System.Drawing.Point(8, 120);
            this.labelAddFieldScale.Name = "labelAddFieldScale";
            this.labelAddFieldScale.Size = new System.Drawing.Size(91, 12);
            this.labelAddFieldScale.TabIndex = 9;
            this.labelAddFieldScale.Text = "小数点以下桁数:";
            // 
            // labelAddFieldPrecision
            // 
            this.labelAddFieldPrecision.AutoSize = true;
            this.labelAddFieldPrecision.Location = new System.Drawing.Point(8, 88);
            this.labelAddFieldPrecision.Name = "labelAddFieldPrecision";
            this.labelAddFieldPrecision.Size = new System.Drawing.Size(43, 12);
            this.labelAddFieldPrecision.TabIndex = 8;
            this.labelAddFieldPrecision.Text = "全桁数:";
            // 
            // textBoxAddFieldScale
            // 
            this.textBoxAddFieldScale.Location = new System.Drawing.Point(112, 117);
            this.textBoxAddFieldScale.MaxLength = 2;
            this.textBoxAddFieldScale.Name = "textBoxAddFieldScale";
            this.textBoxAddFieldScale.Size = new System.Drawing.Size(312, 19);
            this.textBoxAddFieldScale.TabIndex = 7;
            this.textBoxAddFieldScale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // textBoxAddFieldPrecision
            // 
            this.textBoxAddFieldPrecision.Location = new System.Drawing.Point(112, 85);
            this.textBoxAddFieldPrecision.MaxLength = 2;
            this.textBoxAddFieldPrecision.Name = "textBoxAddFieldPrecision";
            this.textBoxAddFieldPrecision.Size = new System.Drawing.Size(312, 19);
            this.textBoxAddFieldPrecision.TabIndex = 6;
            this.textBoxAddFieldPrecision.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // textBoxAddFieldLength
            // 
            this.textBoxAddFieldLength.Location = new System.Drawing.Point(112, 53);
            this.textBoxAddFieldLength.MaxLength = 10;
            this.textBoxAddFieldLength.Name = "textBoxAddFieldLength";
            this.textBoxAddFieldLength.Size = new System.Drawing.Size(312, 19);
            this.textBoxAddFieldLength.TabIndex = 5;
            this.textBoxAddFieldLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // textBoxAddFieldAlias
            // 
            this.textBoxAddFieldAlias.Location = new System.Drawing.Point(112, 24);
            this.textBoxAddFieldAlias.Name = "textBoxAddFieldAlias";
            this.textBoxAddFieldAlias.Size = new System.Drawing.Size(312, 19);
            this.textBoxAddFieldAlias.TabIndex = 4;
            // 
            // labelAddFieldLength
            // 
            this.labelAddFieldLength.AutoSize = true;
            this.labelAddFieldLength.Location = new System.Drawing.Point(8, 56);
            this.labelAddFieldLength.Name = "labelAddFieldLength";
            this.labelAddFieldLength.Size = new System.Drawing.Size(27, 12);
            this.labelAddFieldLength.TabIndex = 1;
            this.labelAddFieldLength.Text = "長さ:";
            // 
            // labelAddFieldAlias
            // 
            this.labelAddFieldAlias.AutoSize = true;
            this.labelAddFieldAlias.Location = new System.Drawing.Point(8, 24);
            this.labelAddFieldAlias.Name = "labelAddFieldAlias";
            this.labelAddFieldAlias.Size = new System.Drawing.Size(50, 12);
            this.labelAddFieldAlias.TabIndex = 0;
            this.labelAddFieldAlias.Text = "エイリアス:";
            // 
            // textBoxAddFieldName
            // 
            this.textBoxAddFieldName.Location = new System.Drawing.Point(80, 13);
            this.textBoxAddFieldName.Name = "textBoxAddFieldName";
            this.textBoxAddFieldName.Size = new System.Drawing.Size(360, 19);
            this.textBoxAddFieldName.TabIndex = 2;
            // 
            // labelAddFieldType
            // 
            this.labelAddFieldType.AutoSize = true;
            this.labelAddFieldType.Location = new System.Drawing.Point(8, 48);
            this.labelAddFieldType.Name = "labelAddFieldType";
            this.labelAddFieldType.Size = new System.Drawing.Size(31, 12);
            this.labelAddFieldType.TabIndex = 1;
            this.labelAddFieldType.Text = "種類:";
            // 
            // labelAddFieldName
            // 
            this.labelAddFieldName.AutoSize = true;
            this.labelAddFieldName.Location = new System.Drawing.Point(8, 16);
            this.labelAddFieldName.Name = "labelAddFieldName";
            this.labelAddFieldName.Size = new System.Drawing.Size(31, 12);
            this.labelAddFieldName.TabIndex = 0;
            this.labelAddFieldName.Text = "名前:";
            // 
            // tabPageDeleteField
            // 
            this.tabPageDeleteField.Controls.Add(this.comboBoxDeleteField);
            this.tabPageDeleteField.Controls.Add(this.labelDeleteField);
            this.tabPageDeleteField.Location = new System.Drawing.Point(4, 22);
            this.tabPageDeleteField.Name = "tabPageDeleteField";
            this.tabPageDeleteField.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDeleteField.Size = new System.Drawing.Size(448, 246);
            this.tabPageDeleteField.TabIndex = 1;
            this.tabPageDeleteField.Text = "フィールドの削除";
            this.tabPageDeleteField.UseVisualStyleBackColor = true;
            // 
            // comboBoxDeleteField
            // 
            this.comboBoxDeleteField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDeleteField.FormattingEnabled = true;
            this.comboBoxDeleteField.Location = new System.Drawing.Point(8, 32);
            this.comboBoxDeleteField.Name = "comboBoxDeleteField";
            this.comboBoxDeleteField.Size = new System.Drawing.Size(432, 20);
            this.comboBoxDeleteField.TabIndex = 2;
            // 
            // labelDeleteField
            // 
            this.labelDeleteField.AutoSize = true;
            this.labelDeleteField.Location = new System.Drawing.Point(8, 16);
            this.labelDeleteField.Name = "labelDeleteField";
            this.labelDeleteField.Size = new System.Drawing.Size(94, 12);
            this.labelDeleteField.TabIndex = 0;
            this.labelDeleteField.Text = "削除するフィールド:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(296, 336);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(384, 336);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormAddAndDeleteField
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(474, 368);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControlAddAndDeleteField);
            this.Controls.Add(this.labelExplanation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddAndDeleteField";
            this.Text = "フィールドの追加と削除";
            this.tabControlAddAndDeleteField.ResumeLayout(false);
            this.tabPageAddField.ResumeLayout(false);
            this.tabPageAddField.PerformLayout();
            this.groupBoxAddFieldProperties.ResumeLayout(false);
            this.groupBoxAddFieldProperties.PerformLayout();
            this.tabPageDeleteField.ResumeLayout(false);
            this.tabPageDeleteField.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelExplanation;
        private System.Windows.Forms.TabControl tabControlAddAndDeleteField;
        private System.Windows.Forms.TabPage tabPageAddField;
        private System.Windows.Forms.TabPage tabPageDeleteField;
        private System.Windows.Forms.Label labelAddFieldName;
        private System.Windows.Forms.Label labelAddFieldType;
        private System.Windows.Forms.TextBox textBoxAddFieldName;
        private System.Windows.Forms.GroupBox groupBoxAddFieldProperties;
        private System.Windows.Forms.TextBox textBoxAddFieldAlias;
        private System.Windows.Forms.Label labelAddFieldLength;
        private System.Windows.Forms.Label labelAddFieldAlias;
        private System.Windows.Forms.Label labelDeleteField;
        private System.Windows.Forms.ComboBox comboBoxDeleteField;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxAddFieldType;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxAddFieldLength;
        private System.Windows.Forms.Label labelAddFieldScale;
        private System.Windows.Forms.Label labelAddFieldPrecision;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxAddFieldScale;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxAddFieldPrecision;
    }
}