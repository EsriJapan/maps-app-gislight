namespace ESRIJapan.GISLight10.Ui
{
    partial class FormManageHyperlinks
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
            this.groupSelectField = new System.Windows.Forms.GroupBox();
            this.panelSelectField = new System.Windows.Forms.Panel();
            this.radioUseURL = new System.Windows.Forms.RadioButton();
            this.labelUseField = new System.Windows.Forms.Label();
            this.radioUseDoc = new System.Windows.Forms.RadioButton();
            this.comboSelectField = new System.Windows.Forms.ComboBox();
            this.labelHyperlinkType = new System.Windows.Forms.Label();
            this.checkUseHyperlinks = new System.Windows.Forms.CheckBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupSelectField.SuspendLayout();
            this.panelSelectField.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupSelectField
            // 
            this.groupSelectField.Controls.Add(this.panelSelectField);
            this.groupSelectField.Controls.Add(this.checkUseHyperlinks);
            this.groupSelectField.Location = new System.Drawing.Point(8, 40);
            this.groupSelectField.Name = "groupSelectField";
            this.groupSelectField.Size = new System.Drawing.Size(448, 136);
            this.groupSelectField.TabIndex = 3;
            this.groupSelectField.TabStop = false;
            // 
            // panelSelectField
            // 
            this.panelSelectField.Controls.Add(this.radioUseURL);
            this.panelSelectField.Controls.Add(this.labelUseField);
            this.panelSelectField.Controls.Add(this.radioUseDoc);
            this.panelSelectField.Controls.Add(this.comboSelectField);
            this.panelSelectField.Controls.Add(this.labelHyperlinkType);
            this.panelSelectField.Location = new System.Drawing.Point(8, 32);
            this.panelSelectField.Name = "panelSelectField";
            this.panelSelectField.Size = new System.Drawing.Size(432, 88);
            this.panelSelectField.TabIndex = 4;
            // 
            // radioUseURL
            // 
            this.radioUseURL.AutoSize = true;
            this.radioUseURL.Location = new System.Drawing.Point(96, 72);
            this.radioUseURL.Name = "radioUseURL";
            this.radioUseURL.Size = new System.Drawing.Size(45, 16);
            this.radioUseURL.TabIndex = 9;
            this.radioUseURL.TabStop = true;
            this.radioUseURL.Text = "URL";
            this.radioUseURL.UseVisualStyleBackColor = true;
            // 
            // labelUseField
            // 
            this.labelUseField.AutoSize = true;
            this.labelUseField.Location = new System.Drawing.Point(0, 0);
            this.labelUseField.Name = "labelUseField";
            this.labelUseField.Size = new System.Drawing.Size(75, 12);
            this.labelUseField.TabIndex = 5;
            this.labelUseField.Text = "使用フィールド:";
            // 
            // radioUseDoc
            // 
            this.radioUseDoc.AutoSize = true;
            this.radioUseDoc.Checked = true;
            this.radioUseDoc.Location = new System.Drawing.Point(0, 72);
            this.radioUseDoc.Name = "radioUseDoc";
            this.radioUseDoc.Size = new System.Drawing.Size(75, 16);
            this.radioUseDoc.TabIndex = 8;
            this.radioUseDoc.TabStop = true;
            this.radioUseDoc.Text = "ドキュメント";
            this.radioUseDoc.UseVisualStyleBackColor = true;
            // 
            // comboSelectField
            // 
            this.comboSelectField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSelectField.FormattingEnabled = true;
            this.comboSelectField.Location = new System.Drawing.Point(0, 16);
            this.comboSelectField.Name = "comboSelectField";
            this.comboSelectField.Size = new System.Drawing.Size(432, 20);
            this.comboSelectField.TabIndex = 6;
            // 
            // labelHyperlinkType
            // 
            this.labelHyperlinkType.AutoSize = true;
            this.labelHyperlinkType.Location = new System.Drawing.Point(0, 56);
            this.labelHyperlinkType.Name = "labelHyperlinkType";
            this.labelHyperlinkType.Size = new System.Drawing.Size(104, 12);
            this.labelHyperlinkType.TabIndex = 7;
            this.labelHyperlinkType.Text = "ハイパーリンクの種類:";
            // 
            // checkUseHyperlinks
            // 
            this.checkUseHyperlinks.AutoSize = true;
            this.checkUseHyperlinks.Location = new System.Drawing.Point(8, 0);
            this.checkUseHyperlinks.Name = "checkUseHyperlinks";
            this.checkUseHyperlinks.Size = new System.Drawing.Size(215, 16);
            this.checkUseHyperlinks.TabIndex = 1;
            this.checkUseHyperlinks.Text = "フィールドを使用してハイパーリンクを設定";
            this.checkUseHyperlinks.UseVisualStyleBackColor = true;
            this.checkUseHyperlinks.CheckedChanged += new System.EventHandler(this.checkUseHyperlinks_CheckedChanged);
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(8, 8);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(160, 12);
            this.labelDescription.TabIndex = 0;
            this.labelDescription.Text = "ハイパーリンクの設定を行います。";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(288, 200);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(376, 200);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormManageHyperlinks
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(464, 233);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.groupSelectField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormManageHyperlinks";
            this.Text = "ハイパーリンクの設定";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormManageHyperlinks_FormClosed);
            this.groupSelectField.ResumeLayout(false);
            this.groupSelectField.PerformLayout();
            this.panelSelectField.ResumeLayout(false);
            this.panelSelectField.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupSelectField;
        private System.Windows.Forms.CheckBox checkUseHyperlinks;
        private System.Windows.Forms.ComboBox comboSelectField;
        private System.Windows.Forms.Label labelUseField;
        private System.Windows.Forms.Label labelHyperlinkType;
        private System.Windows.Forms.RadioButton radioUseDoc;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.RadioButton radioUseURL;
        private System.Windows.Forms.Panel panelSelectField;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}