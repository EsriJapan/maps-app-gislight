namespace ESRIJapan.GISLight10.Ui
{
    partial class FormLabelSetting
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
            this.checkBoxShowLabel = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxSameLabelMethod = new System.Windows.Forms.GroupBox();
            this.radioButtonOneLabelByOneFeature = new System.Windows.Forms.RadioButton();
            this.radioButtonDeleteSameLabel = new System.Windows.Forms.RadioButton();
            this.groupBoxLabelPosition = new System.Windows.Forms.GroupBox();
            this.checkBoxAllowOverlap = new System.Windows.Forms.CheckBox();
            this.comboBoxLabelPosition = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxTextSymbol = new System.Windows.Forms.GroupBox();
            this.comboBoxTextFont = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonTextColor = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxTextSize = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxFields = new System.Windows.Forms.GroupBox();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.listBoxLabelFields = new System.Windows.Forms.ListBox();
            this.listBoxAllFields = new System.Windows.Forms.ListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBoxSameLabelMethod.SuspendLayout();
            this.groupBoxLabelPosition.SuspendLayout();
            this.groupBoxTextSymbol.SuspendLayout();
            this.groupBoxFields.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(378, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "ラベル表示を設定します。\r\nラベル フィールドを複数追加した場合、ラベルは上から改行されて表示されます。";
            // 
            // checkBoxShowLabel
            // 
            this.checkBoxShowLabel.AutoSize = true;
            this.checkBoxShowLabel.Location = new System.Drawing.Point(8, 48);
            this.checkBoxShowLabel.Name = "checkBoxShowLabel";
            this.checkBoxShowLabel.Size = new System.Drawing.Size(168, 16);
            this.checkBoxShowLabel.TabIndex = 1;
            this.checkBoxShowLabel.Text = "このレイヤのラベルを表示する。";
            this.checkBoxShowLabel.UseVisualStyleBackColor = true;
            this.checkBoxShowLabel.CheckedChanged += new System.EventHandler(this.checkBoxShowLabel_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxSameLabelMethod);
            this.panel1.Controls.Add(this.groupBoxLabelPosition);
            this.panel1.Controls.Add(this.groupBoxTextSymbol);
            this.panel1.Controls.Add(this.groupBoxFields);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(472, 552);
            this.panel1.TabIndex = 2;
            // 
            // groupBoxSameLabelMethod
            // 
            this.groupBoxSameLabelMethod.Controls.Add(this.radioButtonOneLabelByOneFeature);
            this.groupBoxSameLabelMethod.Controls.Add(this.radioButtonDeleteSameLabel);
            this.groupBoxSameLabelMethod.Location = new System.Drawing.Point(8, 488);
            this.groupBoxSameLabelMethod.Name = "groupBoxSameLabelMethod";
            this.groupBoxSameLabelMethod.Size = new System.Drawing.Size(456, 56);
            this.groupBoxSameLabelMethod.TabIndex = 22;
            this.groupBoxSameLabelMethod.TabStop = false;
            this.groupBoxSameLabelMethod.Text = "同じラベルの配置方法";
            // 
            // radioButtonOneLabelByOneFeature
            // 
            this.radioButtonOneLabelByOneFeature.AutoSize = true;
            this.radioButtonOneLabelByOneFeature.Location = new System.Drawing.Point(176, 24);
            this.radioButtonOneLabelByOneFeature.Name = "radioButtonOneLabelByOneFeature";
            this.radioButtonOneLabelByOneFeature.Size = new System.Drawing.Size(172, 16);
            this.radioButtonOneLabelByOneFeature.TabIndex = 24;
            this.radioButtonOneLabelByOneFeature.Text = "フィーチャ毎に1つのラベルを表示";
            this.radioButtonOneLabelByOneFeature.UseVisualStyleBackColor = true;
            // 
            // radioButtonDeleteSameLabel
            // 
            this.radioButtonDeleteSameLabel.AutoSize = true;
            this.radioButtonDeleteSameLabel.Checked = true;
            this.radioButtonDeleteSameLabel.Location = new System.Drawing.Point(16, 24);
            this.radioButtonDeleteSameLabel.Name = "radioButtonDeleteSameLabel";
            this.radioButtonDeleteSameLabel.Size = new System.Drawing.Size(105, 16);
            this.radioButtonDeleteSameLabel.TabIndex = 23;
            this.radioButtonDeleteSameLabel.TabStop = true;
            this.radioButtonDeleteSameLabel.Text = "同じラベルを削除";
            this.radioButtonDeleteSameLabel.UseVisualStyleBackColor = true;
            // 
            // groupBoxLabelPosition
            // 
            this.groupBoxLabelPosition.Controls.Add(this.checkBoxAllowOverlap);
            this.groupBoxLabelPosition.Controls.Add(this.comboBoxLabelPosition);
            this.groupBoxLabelPosition.Controls.Add(this.label6);
            this.groupBoxLabelPosition.Location = new System.Drawing.Point(8, 416);
            this.groupBoxLabelPosition.Name = "groupBoxLabelPosition";
            this.groupBoxLabelPosition.Size = new System.Drawing.Size(456, 56);
            this.groupBoxLabelPosition.TabIndex = 18;
            this.groupBoxLabelPosition.TabStop = false;
            this.groupBoxLabelPosition.Text = "ラベル配置";
            // 
            // checkBoxAllowOverlap
            // 
            this.checkBoxAllowOverlap.AutoSize = true;
            this.checkBoxAllowOverlap.Location = new System.Drawing.Point(296, 23);
            this.checkBoxAllowOverlap.Name = "checkBoxAllowOverlap";
            this.checkBoxAllowOverlap.Size = new System.Drawing.Size(87, 16);
            this.checkBoxAllowOverlap.TabIndex = 21;
            this.checkBoxAllowOverlap.Text = "重なりを許可";
            this.checkBoxAllowOverlap.UseVisualStyleBackColor = true;
            // 
            // comboBoxLabelPosition
            // 
            this.comboBoxLabelPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLabelPosition.FormattingEnabled = true;
            this.comboBoxLabelPosition.Location = new System.Drawing.Point(53, 21);
            this.comboBoxLabelPosition.Name = "comboBoxLabelPosition";
            this.comboBoxLabelPosition.Size = new System.Drawing.Size(207, 20);
            this.comboBoxLabelPosition.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "配置:";
            // 
            // groupBoxTextSymbol
            // 
            this.groupBoxTextSymbol.Controls.Add(this.comboBoxTextFont);
            this.groupBoxTextSymbol.Controls.Add(this.label5);
            this.groupBoxTextSymbol.Controls.Add(this.buttonTextColor);
            this.groupBoxTextSymbol.Controls.Add(this.label4);
            this.groupBoxTextSymbol.Controls.Add(this.comboBoxTextSize);
            this.groupBoxTextSymbol.Controls.Add(this.label3);
            this.groupBoxTextSymbol.Location = new System.Drawing.Point(8, 312);
            this.groupBoxTextSymbol.Name = "groupBoxTextSymbol";
            this.groupBoxTextSymbol.Size = new System.Drawing.Size(456, 88);
            this.groupBoxTextSymbol.TabIndex = 11;
            this.groupBoxTextSymbol.TabStop = false;
            this.groupBoxTextSymbol.Text = "テキストシンボル";
            // 
            // comboBoxTextFont
            // 
            this.comboBoxTextFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTextFont.FormattingEnabled = true;
            this.comboBoxTextFont.Location = new System.Drawing.Point(62, 53);
            this.comboBoxTextFont.Name = "comboBoxTextFont";
            this.comboBoxTextFont.Size = new System.Drawing.Size(198, 20);
            this.comboBoxTextFont.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "フォント:";
            // 
            // buttonTextColor
            // 
            this.buttonTextColor.Location = new System.Drawing.Point(319, 19);
            this.buttonTextColor.Name = "buttonTextColor";
            this.buttonTextColor.Size = new System.Drawing.Size(50, 23);
            this.buttonTextColor.TabIndex = 15;
            this.buttonTextColor.UseVisualStyleBackColor = true;
            this.buttonTextColor.Click += new System.EventHandler(this.buttonTextColor_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(294, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "色:";
            // 
            // comboBoxTextSize
            // 
            this.comboBoxTextSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTextSize.FormattingEnabled = true;
            this.comboBoxTextSize.Items.AddRange(new object[] {
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
            this.comboBoxTextSize.Location = new System.Drawing.Point(58, 21);
            this.comboBoxTextSize.Name = "comboBoxTextSize";
            this.comboBoxTextSize.Size = new System.Drawing.Size(121, 20);
            this.comboBoxTextSize.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "サイズ:";
            // 
            // groupBoxFields
            // 
            this.groupBoxFields.Controls.Add(this.buttonDown);
            this.groupBoxFields.Controls.Add(this.buttonUp);
            this.groupBoxFields.Controls.Add(this.buttonRemove);
            this.groupBoxFields.Controls.Add(this.buttonAdd);
            this.groupBoxFields.Controls.Add(this.listBoxLabelFields);
            this.groupBoxFields.Controls.Add(this.listBoxAllFields);
            this.groupBoxFields.Location = new System.Drawing.Point(8, 16);
            this.groupBoxFields.Name = "groupBoxFields";
            this.groupBoxFields.Size = new System.Drawing.Size(456, 280);
            this.groupBoxFields.TabIndex = 3;
            this.groupBoxFields.TabStop = false;
            this.groupBoxFields.Text = "ラベル フィールドの選択";
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(424, 232);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(22, 21);
            this.buttonDown.TabIndex = 9;
            this.buttonDown.Text = "↓";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(424, 208);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(22, 21);
            this.buttonUp.TabIndex = 8;
            this.buttonUp.Text = "↑";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(112, 168);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 6;
            this.buttonRemove.Text = "削除";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(24, 168);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 5;
            this.buttonAdd.Text = "追加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // listBoxLabelFields
            // 
            this.listBoxLabelFields.FormattingEnabled = true;
            this.listBoxLabelFields.ItemHeight = 12;
            this.listBoxLabelFields.Location = new System.Drawing.Point(16, 200);
            this.listBoxLabelFields.Name = "listBoxLabelFields";
            this.listBoxLabelFields.Size = new System.Drawing.Size(400, 64);
            this.listBoxLabelFields.TabIndex = 7;
            this.listBoxLabelFields.DoubleClick += new System.EventHandler(this.buttonRemove_Click);
            // 
            // listBoxAllFields
            // 
            this.listBoxAllFields.FormattingEnabled = true;
            this.listBoxAllFields.ItemHeight = 12;
            this.listBoxAllFields.Location = new System.Drawing.Point(16, 24);
            this.listBoxAllFields.Name = "listBoxAllFields";
            this.listBoxAllFields.Size = new System.Drawing.Size(400, 136);
            this.listBoxAllFields.TabIndex = 0;
            this.listBoxAllFields.DoubleClick += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(208, 632);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 25;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(296, 632);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 23);
            this.buttonAccept.TabIndex = 26;
            this.buttonAccept.Text = "適用";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(384, 632);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 27;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormLabelSetting
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(474, 665);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.checkBoxShowLabel);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLabelSetting";
            this.Text = "ラベル表示";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLabelSetting_FormClosed);
            this.panel1.ResumeLayout(false);
            this.groupBoxSameLabelMethod.ResumeLayout(false);
            this.groupBoxSameLabelMethod.PerformLayout();
            this.groupBoxLabelPosition.ResumeLayout(false);
            this.groupBoxLabelPosition.PerformLayout();
            this.groupBoxTextSymbol.ResumeLayout(false);
            this.groupBoxTextSymbol.PerformLayout();
            this.groupBoxFields.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxShowLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxFields;
        private System.Windows.Forms.GroupBox groupBoxTextSymbol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxTextSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxTextFont;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonTextColor;
        private System.Windows.Forms.GroupBox groupBoxLabelPosition;
        private System.Windows.Forms.ComboBox comboBoxLabelPosition;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxAllowOverlap;
        private System.Windows.Forms.GroupBox groupBoxSameLabelMethod;
        private System.Windows.Forms.RadioButton radioButtonOneLabelByOneFeature;
        private System.Windows.Forms.RadioButton radioButtonDeleteSameLabel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListBox listBoxAllFields;
        private System.Windows.Forms.ListBox listBoxLabelFields;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
    }
}