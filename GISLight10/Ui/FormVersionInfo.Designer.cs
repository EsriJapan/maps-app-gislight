namespace ESRIJapan.GISLight10.Ui
{
    partial class FormVersionInfo
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
			this.labelAppName = new System.Windows.Forms.Label();
			this.labelAppVersion = new System.Windows.Forms.Label();
			this.labelAppCopyright = new System.Windows.Forms.Label();
			this.labelAppNote = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelAppName
			// 
			this.labelAppName.AutoSize = true;
			this.labelAppName.Location = new System.Drawing.Point(16, 16);
			this.labelAppName.Name = "labelAppName";
			this.labelAppName.Size = new System.Drawing.Size(88, 12);
			this.labelAppName.TabIndex = 0;
			this.labelAppName.Text = "GISLight";
			// 
			// labelAppVersion
			// 
			this.labelAppVersion.AutoSize = true;
			this.labelAppVersion.Location = new System.Drawing.Point(120, 16);
			this.labelAppVersion.Name = "labelAppVersion";
			this.labelAppVersion.Size = new System.Drawing.Size(76, 12);
			this.labelAppVersion.TabIndex = 1;
			this.labelAppVersion.Text = "(version 0.9.9)";
			// 
			// labelAppCopyright
			// 
			this.labelAppCopyright.AutoSize = true;
			this.labelAppCopyright.Location = new System.Drawing.Point(16, 48);
			this.labelAppCopyright.Name = "labelAppCopyright";
			this.labelAppCopyright.Size = new System.Drawing.Size(288, 12);
			this.labelAppCopyright.TabIndex = 2;
			this.labelAppCopyright.Text = "Copyright©ESRI Japan Corporation. All rights reserved.";
			// 
			// labelAppNote
			// 
			this.labelAppNote.AutoSize = true;
			this.labelAppNote.Location = new System.Drawing.Point(16, 80);
			this.labelAppNote.Name = "labelAppNote";
			this.labelAppNote.Size = new System.Drawing.Size(372, 24);
			this.labelAppNote.TabIndex = 3;
			this.labelAppNote.Text = "この製品の全部または一部を無断で複製したり、無断で複製物を頒布すると、\r\n著作権の侵害となりますので、ご注意ください。";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(328, 128);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.button1_Click);
			// 
			// FormVersionInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(424, 161);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.labelAppNote);
			this.Controls.Add(this.labelAppCopyright);
			this.Controls.Add(this.labelAppVersion);
			this.Controls.Add(this.labelAppName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormVersionInfo";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "バージョン情報";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormVersionInfo_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAppName;
        private System.Windows.Forms.Label labelAppVersion;
        private System.Windows.Forms.Label labelAppCopyright;
        private System.Windows.Forms.Label labelAppNote;
        private System.Windows.Forms.Button buttonOK;
    }
}