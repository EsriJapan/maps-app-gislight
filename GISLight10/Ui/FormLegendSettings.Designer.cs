namespace ESRIJapan.GISLight10.Ui
{
    partial class FormLegendSettings
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxUnderline = new System.Windows.Forms.CheckBox();
            this.checkBoxItalic = new System.Windows.Forms.CheckBox();
            this.checkBoxBold = new System.Windows.Forms.CheckBox();
            this.comboBoxFontName = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxFontSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnFontColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonRight = new System.Windows.Forms.RadioButton();
            this.radioButtonCenter = new System.Windows.Forms.RadioButton();
            this.radioButtonLeft = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnAllDel = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAllAdd = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.listBoxLegend = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.listBoxMapLayer = new System.Windows.Forms.ListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_FrameBorderWidth = new System.Windows.Forms.NumericUpDown();
            this.checkBox_FrameBorder = new System.Windows.Forms.CheckBox();
            this.radioButton_FrameBG_T = new System.Windows.Forms.RadioButton();
            this.radioButton_FrameBG_W = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FrameBorderWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(377, 548);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOk.Location = new System.Drawing.Point(284, 548);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 19;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(6, 18);
            this.textBoxTitle.MaxLength = 120;
            this.textBoxTitle.Multiline = true;
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(420, 71);
            this.textBoxTitle.TabIndex = 21;
            this.textBoxTitle.Text = "凡例";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxTitle);
            this.groupBox1.Location = new System.Drawing.Point(17, 235);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(435, 95);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "凡例タイトル";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxUnderline);
            this.groupBox2.Controls.Add(this.checkBoxItalic);
            this.groupBox2.Controls.Add(this.checkBoxBold);
            this.groupBox2.Controls.Add(this.comboBoxFontName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.comboBoxFontSize);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnFontColor);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(17, 350);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 188);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "凡例タイトルのフォントプロパティ";
            // 
            // checkBoxUnderline
            // 
            this.checkBoxUnderline.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxUnderline.AutoSize = true;
            this.checkBoxUnderline.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxUnderline.Location = new System.Drawing.Point(154, 148);
            this.checkBoxUnderline.Name = "checkBoxUnderline";
            this.checkBoxUnderline.Size = new System.Drawing.Size(27, 23);
            this.checkBoxUnderline.TabIndex = 8;
            this.checkBoxUnderline.Text = "U";
            this.checkBoxUnderline.UseVisualStyleBackColor = true;
            // 
            // checkBoxItalic
            // 
            this.checkBoxItalic.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxItalic.AutoSize = true;
            this.checkBoxItalic.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxItalic.Location = new System.Drawing.Point(110, 148);
            this.checkBoxItalic.Name = "checkBoxItalic";
            this.checkBoxItalic.Size = new System.Drawing.Size(22, 23);
            this.checkBoxItalic.TabIndex = 7;
            this.checkBoxItalic.Text = "I";
            this.checkBoxItalic.UseVisualStyleBackColor = true;
            // 
            // checkBoxBold
            // 
            this.checkBoxBold.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxBold.AutoSize = true;
            this.checkBoxBold.Checked = true;
            this.checkBoxBold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBold.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxBold.Location = new System.Drawing.Point(61, 148);
            this.checkBoxBold.Name = "checkBoxBold";
            this.checkBoxBold.Size = new System.Drawing.Size(26, 23);
            this.checkBoxBold.TabIndex = 6;
            this.checkBoxBold.Text = "B";
            this.checkBoxBold.UseVisualStyleBackColor = true;
            // 
            // comboBoxFontName
            // 
            this.comboBoxFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFontName.FormattingEnabled = true;
            this.comboBoxFontName.Location = new System.Drawing.Point(61, 106);
            this.comboBoxFontName.Name = "comboBoxFontName";
            this.comboBoxFontName.Size = new System.Drawing.Size(188, 20);
            this.comboBoxFontName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "フォント:";
            // 
            // comboBoxFontSize
            // 
            this.comboBoxFontSize.FormattingEnabled = true;
            this.comboBoxFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
            this.comboBoxFontSize.Location = new System.Drawing.Point(61, 66);
            this.comboBoxFontSize.MaxLength = 6;
            this.comboBoxFontSize.Name = "comboBoxFontSize";
            this.comboBoxFontSize.Size = new System.Drawing.Size(122, 20);
            this.comboBoxFontSize.TabIndex = 3;
            this.comboBoxFontSize.Leave += new System.EventHandler(this.FontSize_Leave);
            this.comboBoxFontSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FontSize_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "サイズ:";
            // 
            // btnFontColor
            // 
            this.btnFontColor.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnFontColor.Location = new System.Drawing.Point(61, 23);
            this.btnFontColor.Name = "btnFontColor";
            this.btnFontColor.Size = new System.Drawing.Size(49, 25);
            this.btnFontColor.TabIndex = 1;
            this.btnFontColor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFontColor.UseVisualStyleBackColor = false;
            this.btnFontColor.Click += new System.EventHandler(this.btnFontColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "色:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonRight);
            this.groupBox3.Controls.Add(this.radioButtonCenter);
            this.groupBox3.Controls.Add(this.radioButtonLeft);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(278, 350);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(174, 91);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "タイトル調整";
            // 
            // radioButtonRight
            // 
            this.radioButtonRight.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRight.AutoSize = true;
            this.radioButtonRight.Location = new System.Drawing.Point(106, 61);
            this.radioButtonRight.Name = "radioButtonRight";
            this.radioButtonRight.Size = new System.Drawing.Size(49, 22);
            this.radioButtonRight.TabIndex = 3;
            this.radioButtonRight.Text = "右詰め";
            this.radioButtonRight.UseVisualStyleBackColor = true;
            // 
            // radioButtonCenter
            // 
            this.radioButtonCenter.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonCenter.AutoSize = true;
            this.radioButtonCenter.Location = new System.Drawing.Point(61, 61);
            this.radioButtonCenter.Name = "radioButtonCenter";
            this.radioButtonCenter.Size = new System.Drawing.Size(39, 22);
            this.radioButtonCenter.TabIndex = 2;
            this.radioButtonCenter.Text = "中央";
            this.radioButtonCenter.UseVisualStyleBackColor = true;
            // 
            // radioButtonLeft
            // 
            this.radioButtonLeft.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonLeft.AutoSize = true;
            this.radioButtonLeft.Checked = true;
            this.radioButtonLeft.Location = new System.Drawing.Point(6, 61);
            this.radioButtonLeft.Name = "radioButtonLeft";
            this.radioButtonLeft.Size = new System.Drawing.Size(49, 22);
            this.radioButtonLeft.TabIndex = 1;
            this.radioButtonLeft.TabStop = true;
            this.radioButtonLeft.Text = "左詰め";
            this.radioButtonLeft.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(22, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 34);
            this.label4.TabIndex = 0;
            this.label4.Text = "凡例に対するタイトルの位置を調整します。";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.btnDown);
            this.groupBox4.Controls.Add(this.btnUp);
            this.groupBox4.Controls.Add(this.btnAllDel);
            this.groupBox4.Controls.Add(this.btnDel);
            this.groupBox4.Controls.Add(this.btnAllAdd);
            this.groupBox4.Controls.Add(this.btnAdd);
            this.groupBox4.Controls.Add(this.listBoxLegend);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.listBoxMapLayer);
            this.groupBox4.Location = new System.Drawing.Point(17, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(435, 207);
            this.groupBox4.TabIndex = 25;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "凡例に含めるレイヤの選択";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(239, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "凡例項目:";
            // 
            // btnDown
            // 
            this.btnDown.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDown.Location = new System.Drawing.Point(403, 125);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(23, 21);
            this.btnDown.TabIndex = 8;
            this.btnDown.Text = "↓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(403, 89);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(23, 21);
            this.btnUp.TabIndex = 7;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnAllDel
            // 
            this.btnAllDel.Location = new System.Drawing.Point(196, 163);
            this.btnAllDel.Name = "btnAllDel";
            this.btnAllDel.Size = new System.Drawing.Size(26, 21);
            this.btnAllDel.TabIndex = 6;
            this.btnAllDel.Text = "<<";
            this.btnAllDel.UseVisualStyleBackColor = true;
            this.btnAllDel.Click += new System.EventHandler(this.btnAllDel_Click);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(196, 136);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(26, 21);
            this.btnDel.TabIndex = 5;
            this.btnDel.Text = "<";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAllAdd
            // 
            this.btnAllAdd.Location = new System.Drawing.Point(196, 75);
            this.btnAllAdd.Name = "btnAllAdd";
            this.btnAllAdd.Size = new System.Drawing.Size(26, 21);
            this.btnAllAdd.TabIndex = 4;
            this.btnAllAdd.Text = ">>";
            this.btnAllAdd.UseVisualStyleBackColor = true;
            this.btnAllAdd.Click += new System.EventHandler(this.btnAllAdd_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(196, 48);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 21);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = ">";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // listBoxLegend
            // 
            this.listBoxLegend.FormattingEnabled = true;
            this.listBoxLegend.ItemHeight = 12;
            this.listBoxLegend.Location = new System.Drawing.Point(241, 48);
            this.listBoxLegend.Name = "listBoxLegend";
            this.listBoxLegend.Size = new System.Drawing.Size(156, 136);
            this.listBoxLegend.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "マップレイヤ:";
            // 
            // listBoxMapLayer
            // 
            this.listBoxMapLayer.FormattingEnabled = true;
            this.listBoxMapLayer.ItemHeight = 12;
            this.listBoxMapLayer.Location = new System.Drawing.Point(18, 48);
            this.listBoxMapLayer.Name = "listBoxMapLayer";
            this.listBoxMapLayer.Size = new System.Drawing.Size(156, 136);
            this.listBoxMapLayer.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.numericUpDown_FrameBorderWidth);
            this.groupBox5.Controls.Add(this.checkBox_FrameBorder);
            this.groupBox5.Controls.Add(this.radioButton_FrameBG_T);
            this.groupBox5.Controls.Add(this.radioButton_FrameBG_W);
            this.groupBox5.Location = new System.Drawing.Point(278, 448);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(174, 90);
            this.groupBox5.TabIndex = 26;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "凡例フレーム";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(137, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "pt.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "太さ :";
            // 
            // numericUpDown_FrameBorderWidth
            // 
            this.numericUpDown_FrameBorderWidth.DecimalPlaces = 1;
            this.numericUpDown_FrameBorderWidth.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numericUpDown_FrameBorderWidth.Location = new System.Drawing.Point(93, 67);
            this.numericUpDown_FrameBorderWidth.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            this.numericUpDown_FrameBorderWidth.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numericUpDown_FrameBorderWidth.Name = "numericUpDown_FrameBorderWidth";
            this.numericUpDown_FrameBorderWidth.Size = new System.Drawing.Size(40, 19);
            this.numericUpDown_FrameBorderWidth.TabIndex = 3;
            this.numericUpDown_FrameBorderWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown_FrameBorderWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // checkBox_FrameBorder
            // 
            this.checkBox_FrameBorder.AutoSize = true;
            this.checkBox_FrameBorder.Checked = true;
            this.checkBox_FrameBorder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_FrameBorder.Location = new System.Drawing.Point(7, 68);
            this.checkBox_FrameBorder.Name = "checkBox_FrameBorder";
            this.checkBox_FrameBorder.Size = new System.Drawing.Size(48, 16);
            this.checkBox_FrameBorder.TabIndex = 2;
            this.checkBox_FrameBorder.Text = "枠線";
            this.checkBox_FrameBorder.UseVisualStyleBackColor = true;
            // 
            // radioButton_FrameBG_T
            // 
            this.radioButton_FrameBG_T.AutoSize = true;
            this.radioButton_FrameBG_T.Location = new System.Drawing.Point(7, 43);
            this.radioButton_FrameBG_T.Name = "radioButton_FrameBG_T";
            this.radioButton_FrameBG_T.Size = new System.Drawing.Size(54, 16);
            this.radioButton_FrameBG_T.TabIndex = 1;
            this.radioButton_FrameBG_T.TabStop = true;
            this.radioButton_FrameBG_T.Text = "色なし";
            this.radioButton_FrameBG_T.UseVisualStyleBackColor = true;
            // 
            // radioButton_FrameBG_W
            // 
            this.radioButton_FrameBG_W.AutoSize = true;
            this.radioButton_FrameBG_W.Checked = true;
            this.radioButton_FrameBG_W.Location = new System.Drawing.Point(7, 20);
            this.radioButton_FrameBG_W.Name = "radioButton_FrameBG_W";
            this.radioButton_FrameBG_W.Size = new System.Drawing.Size(35, 16);
            this.radioButton_FrameBG_W.TabIndex = 0;
            this.radioButton_FrameBG_W.TabStop = true;
            this.radioButton_FrameBG_W.Text = "白";
            this.radioButton_FrameBG_W.UseVisualStyleBackColor = true;
            // 
            // FormLegendSettings
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(464, 583);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLegendSettings";
            this.Text = "凡例の設定";
            this.Load += new System.EventHandler(this.FormLegendSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FrameBorderWidth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnFontColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxFontSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxFontName;
        private System.Windows.Forms.CheckBox checkBoxItalic;
        private System.Windows.Forms.CheckBox checkBoxBold;
        private System.Windows.Forms.CheckBox checkBoxUnderline;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButtonRight;
        private System.Windows.Forms.RadioButton radioButtonCenter;
        private System.Windows.Forms.RadioButton radioButtonLeft;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox listBoxMapLayer;
        private System.Windows.Forms.Button btnAllDel;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnAllAdd;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox listBoxLegend;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.CheckBox checkBox_FrameBorder;
		private System.Windows.Forms.RadioButton radioButton_FrameBG_T;
		private System.Windows.Forms.RadioButton radioButton_FrameBG_W;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDown_FrameBorderWidth;
		private System.Windows.Forms.Label label8;
    }
}