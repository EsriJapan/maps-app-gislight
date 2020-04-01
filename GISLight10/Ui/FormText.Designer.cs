namespace ESRIJapan.GISLight10.Ui
{
    partial class FormText
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.richTextBoxPrintTitle = new System.Windows.Forms.RichTextBox();
            this.comboBoxFontSiza = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSetColor = new System.Windows.Forms.Button();
            this.comboBoxFonts = new System.Windows.Forms.ComboBox();
            this.numericUpDownCharacterSpacing = new System.Windows.Forms.NumericUpDown();
            this.labelFontName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCharacterSpacing)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonOk);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(288, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(77, 147);
            this.panel1.TabIndex = 10;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(0, 93);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(71, 22);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(0, 119);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(71, 22);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // richTextBoxPrintTitle
            // 
            this.richTextBoxPrintTitle.Location = new System.Drawing.Point(8, 24);
            this.richTextBoxPrintTitle.Name = "richTextBoxPrintTitle";
            this.richTextBoxPrintTitle.Size = new System.Drawing.Size(280, 26);
            this.richTextBoxPrintTitle.TabIndex = 0;
            this.richTextBoxPrintTitle.Text = "";
            // 
            // comboBoxFontSiza
            // 
            this.comboBoxFontSiza.Location = new System.Drawing.Point(8, 80);
            this.comboBoxFontSiza.Name = "comboBoxFontSiza";
            this.comboBoxFontSiza.Size = new System.Drawing.Size(63, 20);
            this.comboBoxFontSiza.TabIndex = 1;
            this.comboBoxFontSiza.TextChanged += new System.EventHandler(this.comboBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "サイズ:";
            // 
            // buttonSetColor
            // 
            this.buttonSetColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.buttonSetColor.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonSetColor.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonSetColor.Location = new System.Drawing.Point(8, 120);
            this.buttonSetColor.Name = "buttonSetColor";
            this.buttonSetColor.Size = new System.Drawing.Size(54, 20);
            this.buttonSetColor.TabIndex = 3;
            this.buttonSetColor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSetColor.UseVisualStyleBackColor = false;
            this.buttonSetColor.Click += new System.EventHandler(this.buttonSetColor_Click);
            // 
            // comboBoxFonts
            // 
            this.comboBoxFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFonts.FormattingEnabled = true;
            this.comboBoxFonts.Location = new System.Drawing.Point(88, 120);
            this.comboBoxFonts.Name = "comboBoxFonts";
            this.comboBoxFonts.Size = new System.Drawing.Size(143, 20);
            this.comboBoxFonts.TabIndex = 4;
            // 
            // numericUpDownCharacterSpacing
            // 
            this.numericUpDownCharacterSpacing.DecimalPlaces = 2;
            this.numericUpDownCharacterSpacing.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.numericUpDownCharacterSpacing.Location = new System.Drawing.Point(88, 80);
            this.numericUpDownCharacterSpacing.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            131072});
            this.numericUpDownCharacterSpacing.Name = "numericUpDownCharacterSpacing";
            this.numericUpDownCharacterSpacing.Size = new System.Drawing.Size(56, 19);
            this.numericUpDownCharacterSpacing.TabIndex = 2;
            // 
            // labelFontName
            // 
            this.labelFontName.AutoSize = true;
            this.labelFontName.Location = new System.Drawing.Point(88, 104);
            this.labelFontName.Name = "labelFontName";
            this.labelFontName.Size = new System.Drawing.Size(40, 12);
            this.labelFontName.TabIndex = 16;
            this.labelFontName.Text = "フォント:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 12);
            this.label2.TabIndex = 17;
            this.label2.Text = "色:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(88, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 12);
            this.label3.TabIndex = 18;
            this.label3.Text = "文字間隔:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = "テキスト:";
            // 
            // FormText
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(365, 147);
            this.Controls.Add(this.richTextBoxPrintTitle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelFontName);
            this.Controls.Add(this.numericUpDownCharacterSpacing);
            this.Controls.Add(this.comboBoxFonts);
            this.Controls.Add(this.buttonSetColor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxFontSiza);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormText";
            this.Activated += new System.EventHandler(this.FormText_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormText_FormClosed);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCharacterSpacing)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RichTextBox richTextBoxPrintTitle;
        private System.Windows.Forms.ComboBox comboBoxFontSiza;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSetColor;
        private System.Windows.Forms.ComboBox comboBoxFonts;
        private System.Windows.Forms.NumericUpDown numericUpDownCharacterSpacing;
        private System.Windows.Forms.Label labelFontName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

    }
}