namespace ESRIJapan.GISLight10.Ui
{
    partial class FormSplitAndMergeSetting
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxSplitOtherField = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxSplitDateField = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSplitNumField = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxMergeOtherField = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxMergeDateField = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxMergeNumField = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colFldName = new System.Windows.Forms.ColumnHeader();
            this.colDType = new System.Windows.Forms.ColumnHeader();
            this.colDomain = new System.Windows.Forms.ColumnHeader();
            this.comboBoxLayers = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label_Nothing = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panelMsg = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelMsg.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "分筆／合筆編集後の、属性データの既定値を設定します。";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxSplitOtherField);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBoxSplitDateField);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxSplitNumField);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox1.Location = new System.Drawing.Point(8, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 125);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "分筆 編集後";
            // 
            // comboBoxSplitOtherField
            // 
            this.comboBoxSplitOtherField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSplitOtherField.FormattingEnabled = true;
            this.comboBoxSplitOtherField.Location = new System.Drawing.Point(155, 85);
            this.comboBoxSplitOtherField.Name = "comboBoxSplitOtherField";
            this.comboBoxSplitOtherField.Size = new System.Drawing.Size(239, 20);
            this.comboBoxSplitOtherField.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "その他のフィールド:";
            // 
            // comboBoxSplitDateField
            // 
            this.comboBoxSplitDateField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSplitDateField.FormattingEnabled = true;
            this.comboBoxSplitDateField.Location = new System.Drawing.Point(155, 53);
            this.comboBoxSplitDateField.Name = "comboBoxSplitDateField";
            this.comboBoxSplitDateField.Size = new System.Drawing.Size(239, 20);
            this.comboBoxSplitDateField.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "日付型フィールド:";
            // 
            // comboBoxSplitNumField
            // 
            this.comboBoxSplitNumField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSplitNumField.FormattingEnabled = true;
            this.comboBoxSplitNumField.Location = new System.Drawing.Point(155, 21);
            this.comboBoxSplitNumField.Name = "comboBoxSplitNumField";
            this.comboBoxSplitNumField.Size = new System.Drawing.Size(239, 20);
            this.comboBoxSplitNumField.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "数値型フィールド:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxMergeOtherField);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.comboBoxMergeDateField);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comboBoxMergeNumField);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(8, 162);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(429, 125);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "合筆 編集後";
            // 
            // comboBoxMergeOtherField
            // 
            this.comboBoxMergeOtherField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMergeOtherField.FormattingEnabled = true;
            this.comboBoxMergeOtherField.Location = new System.Drawing.Point(155, 85);
            this.comboBoxMergeOtherField.Name = "comboBoxMergeOtherField";
            this.comboBoxMergeOtherField.Size = new System.Drawing.Size(239, 20);
            this.comboBoxMergeOtherField.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(47, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "その他のフィールド:";
            // 
            // comboBoxMergeDateField
            // 
            this.comboBoxMergeDateField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMergeDateField.FormattingEnabled = true;
            this.comboBoxMergeDateField.Location = new System.Drawing.Point(155, 53);
            this.comboBoxMergeDateField.Name = "comboBoxMergeDateField";
            this.comboBoxMergeDateField.Size = new System.Drawing.Size(239, 20);
            this.comboBoxMergeDateField.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(47, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "日付型フィールド:";
            // 
            // comboBoxMergeNumField
            // 
            this.comboBoxMergeNumField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMergeNumField.FormattingEnabled = true;
            this.comboBoxMergeNumField.Location = new System.Drawing.Point(155, 21);
            this.comboBoxMergeNumField.Name = "comboBoxMergeNumField";
            this.comboBoxMergeNumField.Size = new System.Drawing.Size(239, 20);
            this.comboBoxMergeNumField.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "数値型フィールド:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(300, 378);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "設定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(390, 378);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "閉じる";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 37);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(454, 331);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.Tab_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(446, 305);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "基本設定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Controls.Add(this.comboBoxLayers);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label_Nothing);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(446, 305);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "個別設定（ジオデータベースのみ）";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFldName,
            this.colDType,
            this.colDomain});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(12, 55);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(423, 244);
            this.listView1.TabIndex = 12;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListView_ItemChecked);
            // 
            // colFldName
            // 
            this.colFldName.Text = "フィールド";
            this.colFldName.Width = 117;
            // 
            // colDType
            // 
            this.colDType.Text = "データ型";
            // 
            // colDomain
            // 
            this.colDomain.Text = "設定済みドメイン";
            this.colDomain.Width = 152;
            // 
            // comboBoxLayers
            // 
            this.comboBoxLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLayers.FormattingEnabled = true;
            this.comboBoxLayers.Location = new System.Drawing.Point(70, 16);
            this.comboBoxLayers.Name = "comboBoxLayers";
            this.comboBoxLayers.Size = new System.Drawing.Size(323, 20);
            this.comboBoxLayers.TabIndex = 11;
            this.comboBoxLayers.SelectedIndexChanged += new System.EventHandler(this.Layer_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "レイヤ：";
            // 
            // label_Nothing
            // 
            this.label_Nothing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_Nothing.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_Nothing.Location = new System.Drawing.Point(12, 55);
            this.label_Nothing.Name = "label_Nothing";
            this.label_Nothing.Size = new System.Drawing.Size(423, 223);
            this.label_Nothing.TabIndex = 9;
            this.label_Nothing.Text = "数値フィールドがありません";
            this.label_Nothing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ESRIJapan.GISLight10.Properties.Resources.GenericWarning16;
            this.pictureBox1.Location = new System.Drawing.Point(1, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(17, 5);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(257, 12);
            this.label9.TabIndex = 14;
            this.label9.Text = "レイヤ毎に「設定」ボタンをクリックして保存してください。";
            // 
            // panelMsg
            // 
            this.panelMsg.Controls.Add(this.label9);
            this.panelMsg.Controls.Add(this.pictureBox1);
            this.panelMsg.Location = new System.Drawing.Point(10, 379);
            this.panelMsg.Name = "panelMsg";
            this.panelMsg.Size = new System.Drawing.Size(286, 22);
            this.panelMsg.TabIndex = 6;
            // 
            // FormSplitAndMergeSetting
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(477, 409);
            this.Controls.Add(this.panelMsg);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSplitAndMergeSetting";
            this.Text = "スプリット・マージ設定";
            this.Load += new System.EventHandler(this.Form_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSplitAndMergeSetting_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelMsg.ResumeLayout(false);
            this.panelMsg.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxSplitNumField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxSplitDateField;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxSplitOtherField;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxMergeNumField;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxMergeOtherField;
        private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboBoxMergeDateField;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader colFldName;
		private System.Windows.Forms.ColumnHeader colDType;
		private System.Windows.Forms.ColumnHeader colDomain;
		private System.Windows.Forms.ComboBox comboBoxLayers;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label_Nothing;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel panelMsg;
    }
}