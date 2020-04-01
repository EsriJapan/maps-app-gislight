namespace ESRIJapan.GISLight10.Ui
{
    partial class FormFieldCalculate
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
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxParameter3 = new System.Windows.Forms.TextBox();
			this.labelField = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.checkBoxUseCodeblock = new System.Windows.Forms.CheckBox();
			this.checkBoxUseBackground = new System.Windows.Forms.CheckBox();
			this.comboBoxParameter1 = new System.Windows.Forms.ComboBox();
			this.textBoxParameter5 = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.comboBoxParameter2 = new System.Windows.Forms.ComboBox();
			this.buttonParameter3 = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.fieldList = new System.Windows.Forms.ListBox();
			this.checkBox_SelFeats = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 355);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(141, 12);
			this.label3.TabIndex = 10;
			this.label3.Text = "条件式                         ";
			// 
			// textBoxParameter3
			// 
			this.textBoxParameter3.AcceptsReturn = true;
			this.textBoxParameter3.Location = new System.Drawing.Point(14, 370);
			this.textBoxParameter3.Multiline = true;
			this.textBoxParameter3.Name = "textBoxParameter3";
			this.textBoxParameter3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxParameter3.Size = new System.Drawing.Size(458, 40);
			this.textBoxParameter3.TabIndex = 11;
			// 
			// labelField
			// 
			this.labelField.AutoSize = true;
			this.labelField.Location = new System.Drawing.Point(12, 139);
			this.labelField.Name = "labelField";
			this.labelField.Size = new System.Drawing.Size(211, 12);
			this.labelField.TabIndex = 5;
			this.labelField.Text = "フィールド（条件式、コード ブロック作成用）：";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButton1);
			this.groupBox1.Location = new System.Drawing.Point(14, 95);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(458, 41);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "形式（条件式の種類）";
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Checked = true;
			this.radioButton1.Enabled = false;
			this.radioButton1.Location = new System.Drawing.Point(6, 18);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(174, 16);
			this.radioButton1.TabIndex = 0;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Python（Pythonのみ実行可能）";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// checkBoxUseCodeblock
			// 
			this.checkBoxUseCodeblock.AutoSize = true;
			this.checkBoxUseCodeblock.Location = new System.Drawing.Point(14, 236);
			this.checkBoxUseCodeblock.Name = "checkBoxUseCodeblock";
			this.checkBoxUseCodeblock.Size = new System.Drawing.Size(121, 16);
			this.checkBoxUseCodeblock.TabIndex = 7;
			this.checkBoxUseCodeblock.Text = "コード ブロックを表示";
			this.checkBoxUseCodeblock.UseVisualStyleBackColor = true;
			this.checkBoxUseCodeblock.Visible = false;
			// 
			// checkBoxUseBackground
			// 
			this.checkBoxUseBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxUseBackground.AutoSize = true;
			this.checkBoxUseBackground.Location = new System.Drawing.Point(193, 449);
			this.checkBoxUseBackground.Name = "checkBoxUseBackground";
			this.checkBoxUseBackground.Size = new System.Drawing.Size(117, 16);
			this.checkBoxUseBackground.TabIndex = 13;
			this.checkBoxUseBackground.Text = "バックグラウンド実行";
			this.checkBoxUseBackground.UseVisualStyleBackColor = true;
			this.checkBoxUseBackground.Visible = false;
			// 
			// comboBoxParameter1
			// 
			this.comboBoxParameter1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxParameter1.FormattingEnabled = true;
			this.comboBoxParameter1.Location = new System.Drawing.Point(14, 29);
			this.comboBoxParameter1.Name = "comboBoxParameter1";
			this.comboBoxParameter1.Size = new System.Drawing.Size(458, 20);
			this.comboBoxParameter1.TabIndex = 1;
			this.comboBoxParameter1.SelectedIndexChanged += new System.EventHandler(this.comboBoxParameter1_SelectedIndexChanged);
			// 
			// textBoxParameter5
			// 
			this.textBoxParameter5.AcceptsReturn = true;
			this.textBoxParameter5.Location = new System.Drawing.Point(14, 272);
			this.textBoxParameter5.Multiline = true;
			this.textBoxParameter5.Name = "textBoxParameter5";
			this.textBoxParameter5.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxParameter5.Size = new System.Drawing.Size(458, 80);
			this.textBoxParameter5.TabIndex = 9;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(397, 445);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 21;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(316, 445);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 20;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// comboBoxParameter2
			// 
			this.comboBoxParameter2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxParameter2.FormattingEnabled = true;
			this.comboBoxParameter2.Location = new System.Drawing.Point(14, 69);
			this.comboBoxParameter2.Name = "comboBoxParameter2";
			this.comboBoxParameter2.Size = new System.Drawing.Size(458, 20);
			this.comboBoxParameter2.TabIndex = 3;
			this.comboBoxParameter2.SelectedIndexChanged += new System.EventHandler(this.comboBoxParameter2_SelectedIndexChanged);
			// 
			// buttonParameter3
			// 
			this.buttonParameter3.Location = new System.Drawing.Point(402, 416);
			this.buttonParameter3.Name = "buttonParameter3";
			this.buttonParameter3.Size = new System.Drawing.Size(70, 20);
			this.buttonParameter3.TabIndex = 12;
			this.buttonParameter3.Text = "読み込み...";
			this.buttonParameter3.UseVisualStyleBackColor = true;
			this.buttonParameter3.Click += new System.EventHandler(this.buttonParameter3_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 255);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(124, 12);
			this.label5.TabIndex = 8;
			this.label5.Text = "コード ブロック（オプション）";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "対象フィールド";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "入力フィーチャ レイヤ";
			// 
			// fieldList
			// 
			this.fieldList.FormattingEnabled = true;
			this.fieldList.ItemHeight = 12;
			this.fieldList.Location = new System.Drawing.Point(12, 154);
			this.fieldList.Name = "fieldList";
			this.fieldList.Size = new System.Drawing.Size(460, 76);
			this.fieldList.TabIndex = 6;
			this.fieldList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fieldList_MouseDoubleClick);
			// 
			// checkBox_SelFeats
			// 
			this.checkBox_SelFeats.AutoSize = true;
			this.checkBox_SelFeats.Location = new System.Drawing.Point(20, 416);
			this.checkBox_SelFeats.Name = "checkBox_SelFeats";
			this.checkBox_SelFeats.Size = new System.Drawing.Size(123, 16);
			this.checkBox_SelFeats.TabIndex = 22;
			this.checkBox_SelFeats.Text = "選択フィーチャを対象";
			this.checkBox_SelFeats.UseVisualStyleBackColor = true;
			// 
			// FormFieldCalculate
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(484, 483);
			this.Controls.Add(this.checkBox_SelFeats);
			this.Controls.Add(this.fieldList);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxParameter3);
			this.Controls.Add(this.labelField);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkBoxUseCodeblock);
			this.Controls.Add(this.checkBoxUseBackground);
			this.Controls.Add(this.comboBoxParameter1);
			this.Controls.Add(this.textBoxParameter5);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.comboBoxParameter2);
			this.Controls.Add(this.buttonParameter3);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormFieldCalculate";
			this.Text = "フィールド演算";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxParameter3;
        private System.Windows.Forms.Label labelField;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.CheckBox checkBoxUseCodeblock;
        private System.Windows.Forms.CheckBox checkBoxUseBackground;
        private System.Windows.Forms.ComboBox comboBoxParameter1;
        private System.Windows.Forms.TextBox textBoxParameter5;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox comboBoxParameter2;
        private System.Windows.Forms.Button buttonParameter3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox fieldList;
		private System.Windows.Forms.CheckBox checkBox_SelFeats;

    }
}