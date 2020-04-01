namespace ESRIJapan.GISLight10.Ui
{
    partial class FormScaleBarSettings
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TextBoxDivision = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxResizeHint = new System.Windows.Forms.ComboBox();
            this.checkBoxDivisionsBeforeZero = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownSubDiv = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownDiv = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxUnitLabelPosition = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxUnits = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSubDiv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiv)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TextBoxDivision);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxResizeHint);
            this.groupBox1.Controls.Add(this.checkBoxDivisionsBeforeZero);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownSubDiv);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownDiv);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 208);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "縮尺";
            // 
            // TextBoxDivision
            // 
            this.TextBoxDivision.Location = new System.Drawing.Point(104, 21);
            this.TextBoxDivision.MaxLength = 8;
            this.TextBoxDivision.Name = "TextBoxDivision";
            this.TextBoxDivision.Size = new System.Drawing.Size(144, 19);
            this.TextBoxDivision.TabIndex = 1;
            this.TextBoxDivision.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TextBoxDivision.TextChanged += new System.EventHandler(this.TextBoxDivision_TextChanged);
            this.TextBoxDivision.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "目盛幅:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "サイズ変更時:";
            // 
            // comboBoxResizeHint
            // 
            this.comboBoxResizeHint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResizeHint.FormattingEnabled = true;
            this.comboBoxResizeHint.Location = new System.Drawing.Point(16, 168);
            this.comboBoxResizeHint.Name = "comboBoxResizeHint";
            this.comboBoxResizeHint.Size = new System.Drawing.Size(232, 20);
            this.comboBoxResizeHint.TabIndex = 5;
            this.comboBoxResizeHint.SelectedIndexChanged += new System.EventHandler(this.comboBoxResizeHint_SelectedIndexChanged);
            // 
            // checkBoxDivisionsBeforeZero
            // 
            this.checkBoxDivisionsBeforeZero.AutoSize = true;
            this.checkBoxDivisionsBeforeZero.Location = new System.Drawing.Point(16, 120);
            this.checkBoxDivisionsBeforeZero.Name = "checkBoxDivisionsBeforeZero";
            this.checkBoxDivisionsBeforeZero.Size = new System.Drawing.Size(148, 16);
            this.checkBoxDivisionsBeforeZero.TabIndex = 4;
            this.checkBoxDivisionsBeforeZero.Text = "ゼロの前に目盛を１つ表示";
            this.checkBoxDivisionsBeforeZero.UseVisualStyleBackColor = true;
            this.checkBoxDivisionsBeforeZero.CheckedChanged += new System.EventHandler(this.checkBoxDivisionsBeforeZero_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "補助目盛数:";
            // 
            // numericUpDownSubDiv
            // 
            this.numericUpDownSubDiv.Location = new System.Drawing.Point(180, 86);
            this.numericUpDownSubDiv.Name = "numericUpDownSubDiv";
            this.numericUpDownSubDiv.Size = new System.Drawing.Size(68, 19);
            this.numericUpDownSubDiv.TabIndex = 3;
            this.numericUpDownSubDiv.ValueChanged += new System.EventHandler(this.numericUpDownSubDiv_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "目盛数:";
            // 
            // numericUpDownDiv
            // 
            this.numericUpDownDiv.Location = new System.Drawing.Point(180, 54);
            this.numericUpDownDiv.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDiv.Name = "numericUpDownDiv";
            this.numericUpDownDiv.Size = new System.Drawing.Size(68, 19);
            this.numericUpDownDiv.TabIndex = 2;
            this.numericUpDownDiv.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDiv.ValueChanged += new System.EventHandler(this.numericUpDownDiv_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.comboBoxUnitLabelPosition);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.comboBoxUnits);
            this.groupBox2.Location = new System.Drawing.Point(8, 232);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 136);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "単位";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "ラベル位置:";
            // 
            // comboBoxUnitLabelPosition
            // 
            this.comboBoxUnitLabelPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUnitLabelPosition.FormattingEnabled = true;
            this.comboBoxUnitLabelPosition.Location = new System.Drawing.Point(16, 96);
            this.comboBoxUnitLabelPosition.Name = "comboBoxUnitLabelPosition";
            this.comboBoxUnitLabelPosition.Size = new System.Drawing.Size(232, 20);
            this.comboBoxUnitLabelPosition.TabIndex = 7;
            this.comboBoxUnitLabelPosition.SelectedIndexChanged += new System.EventHandler(this.comboBoxUnitLabelPosition_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "目盛単位:";
            // 
            // comboBoxUnits
            // 
            this.comboBoxUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUnits.FormattingEnabled = true;
            this.comboBoxUnits.Location = new System.Drawing.Point(16, 40);
            this.comboBoxUnits.Name = "comboBoxUnits";
            this.comboBoxUnits.Size = new System.Drawing.Size(232, 20);
            this.comboBoxUnits.TabIndex = 6;
            this.comboBoxUnits.SelectedIndexChanged += new System.EventHandler(this.comboBoxUnits_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.buttonOk);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Location = new System.Drawing.Point(8, 384);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(264, 37);
            this.panel1.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(96, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 22);
            this.button1.TabIndex = 9;
            this.button1.Text = "適用";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOk.Location = new System.Drawing.Point(8, 8);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(71, 22);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(184, 8);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(71, 22);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormScaleBarSettings
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(282, 427);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormScaleBarSettings";
            this.ShowIcon = false;
            this.Text = "FormScaleBarSettings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSubDiv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiv)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownDiv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownSubDiv;
        private System.Windows.Forms.CheckBox checkBoxDivisionsBeforeZero;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxResizeHint;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxUnits;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxUnitLabelPosition;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox TextBoxDivision;
        private System.Windows.Forms.Label label6;
    }
}