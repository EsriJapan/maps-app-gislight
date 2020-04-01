namespace ESRIJapan.GISLight10.Ui
{
    partial class FormIntersect
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
            this.checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            this.checkBoxAddMap = new System.Windows.Forms.CheckBox();
            this.checkBoxUseBackground = new System.Windows.Forms.CheckBox();
            this.showOpenWorkspace = new System.Windows.Forms.Button();
            this.textBoxParameter21 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxParameter12 = new System.Windows.Forms.ComboBox();
            this.comboBoxParameter11 = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.textBoxParameter22 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBoxOverwrite
            // 
            this.checkBoxOverwrite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxOverwrite.AutoSize = true;
            this.checkBoxOverwrite.Location = new System.Drawing.Point(11, 172);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(90, 16);
            this.checkBoxOverwrite.TabIndex = 10;
            this.checkBoxOverwrite.Text = "上書きを許可";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            this.checkBoxOverwrite.Visible = false;
            // 
            // checkBoxAddMap
            // 
            this.checkBoxAddMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAddMap.AutoSize = true;
            this.checkBoxAddMap.Checked = true;
            this.checkBoxAddMap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAddMap.Location = new System.Drawing.Point(230, 172);
            this.checkBoxAddMap.Name = "checkBoxAddMap";
            this.checkBoxAddMap.Size = new System.Drawing.Size(82, 16);
            this.checkBoxAddMap.TabIndex = 12;
            this.checkBoxAddMap.Text = "マップに追加";
            this.checkBoxAddMap.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseBackground
            // 
            this.checkBoxUseBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseBackground.AutoSize = true;
            this.checkBoxUseBackground.Location = new System.Drawing.Point(107, 172);
            this.checkBoxUseBackground.Name = "checkBoxUseBackground";
            this.checkBoxUseBackground.Size = new System.Drawing.Size(117, 16);
            this.checkBoxUseBackground.TabIndex = 11;
            this.checkBoxUseBackground.Text = "バックグラウンド実行";
            this.checkBoxUseBackground.UseVisualStyleBackColor = true;
            this.checkBoxUseBackground.Visible = false;
            // 
            // showOpenWorkspace
            // 
            this.showOpenWorkspace.Location = new System.Drawing.Point(441, 96);
            this.showOpenWorkspace.Name = "showOpenWorkspace";
            this.showOpenWorkspace.Size = new System.Drawing.Size(33, 23);
            this.showOpenWorkspace.TabIndex = 6;
            this.showOpenWorkspace.Text = "...";
            this.showOpenWorkspace.UseVisualStyleBackColor = true;
            this.showOpenWorkspace.Click += new System.EventHandler(this.showOpenWorkspace_Click);
            // 
            // textBoxParameter21
            // 
            this.textBoxParameter21.Location = new System.Drawing.Point(16, 98);
            this.textBoxParameter21.Name = "textBoxParameter21";
            this.textBoxParameter21.ReadOnly = true;
            this.textBoxParameter21.Size = new System.Drawing.Size(419, 19);
            this.textBoxParameter21.TabIndex = 5;
            this.textBoxParameter21.TabStop = false;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(14, 82);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(94, 12);
            this.label21.TabIndex = 4;
            this.label21.Text = "出力ワークスペース";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(103, 12);
            this.label12.TabIndex = 2;
            this.label12.Text = "交差フィーチャ レイヤ";
            // 
            // comboBoxParameter12
            // 
            this.comboBoxParameter12.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParameter12.FormattingEnabled = true;
            this.comboBoxParameter12.Location = new System.Drawing.Point(16, 59);
            this.comboBoxParameter12.Name = "comboBoxParameter12";
            this.comboBoxParameter12.Size = new System.Drawing.Size(419, 20);
            this.comboBoxParameter12.TabIndex = 3;
            // 
            // comboBoxParameter11
            // 
            this.comboBoxParameter11.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParameter11.FormattingEnabled = true;
            this.comboBoxParameter11.Location = new System.Drawing.Point(14, 21);
            this.comboBoxParameter11.Name = "comboBoxParameter11";
            this.comboBoxParameter11.Size = new System.Drawing.Size(421, 20);
            this.comboBoxParameter11.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(399, 168);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 21;
            this.cancelButton.Text = "キャンセル";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(318, 168);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 20;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // textBoxParameter22
            // 
            this.textBoxParameter22.Location = new System.Drawing.Point(16, 135);
            this.textBoxParameter22.Name = "textBoxParameter22";
            this.textBoxParameter22.Size = new System.Drawing.Size(419, 19);
            this.textBoxParameter22.TabIndex = 8;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(14, 120);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(108, 12);
            this.label22.TabIndex = 7;
            this.label22.Text = "出力フィーチャクラス名";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "入力フィーチャ レイヤ";
            // 
            // FormIntersect
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(484, 203);
            this.Controls.Add(this.checkBoxOverwrite);
            this.Controls.Add(this.checkBoxAddMap);
            this.Controls.Add(this.checkBoxUseBackground);
            this.Controls.Add(this.showOpenWorkspace);
            this.Controls.Add(this.textBoxParameter21);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.comboBoxParameter12);
            this.Controls.Add(this.comboBoxParameter11);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.textBoxParameter22);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label11);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormIntersect";
            this.Text = "インターセクト";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxOverwrite;
        private System.Windows.Forms.CheckBox checkBoxAddMap;
        private System.Windows.Forms.CheckBox checkBoxUseBackground;
        private System.Windows.Forms.Button showOpenWorkspace;
        private System.Windows.Forms.TextBox textBoxParameter21;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBoxParameter12;
        private System.Windows.Forms.ComboBox comboBoxParameter11;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox textBoxParameter22;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label11;

    }
}