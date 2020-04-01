namespace ESRIJapan.GISLight10.Ui
{
    partial class FormRemoveRelate
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkBoxAllRemove = new System.Windows.Forms.CheckBox();
            this.comboRealateNames = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboSourceLayers = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(384, 192);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(296, 192);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkBoxAllRemove
            // 
            this.checkBoxAllRemove.AutoSize = true;
            this.checkBoxAllRemove.Location = new System.Drawing.Point(8, 152);
            this.checkBoxAllRemove.Name = "checkBoxAllRemove";
            this.checkBoxAllRemove.Size = new System.Drawing.Size(231, 16);
            this.checkBoxAllRemove.TabIndex = 6;
            this.checkBoxAllRemove.Text = "リレート元レイヤのリレートをすべて解除する。";
            this.checkBoxAllRemove.UseVisualStyleBackColor = true;
            // 
            // comboRealateNames
            // 
            this.comboRealateNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRealateNames.FormattingEnabled = true;
            this.comboRealateNames.Location = new System.Drawing.Point(8, 112);
            this.comboRealateNames.Name = "comboRealateNames";
            this.comboRealateNames.Size = new System.Drawing.Size(456, 20);
            this.comboRealateNames.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "リレート名:";
            // 
            // comboSourceLayers
            // 
            this.comboSourceLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSourceLayers.FormattingEnabled = true;
            this.comboSourceLayers.Location = new System.Drawing.Point(8, 56);
            this.comboSourceLayers.Name = "comboSourceLayers";
            this.comboSourceLayers.Size = new System.Drawing.Size(456, 20);
            this.comboSourceLayers.TabIndex = 3;
            this.comboSourceLayers.SelectedIndexChanged += new System.EventHandler(this.comboSourceLayers_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "リレート元レイヤ:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "リレートを解除します。";
            // 
            // FormRemoveRelate
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(474, 225);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxAllRemove);
            this.Controls.Add(this.comboRealateNames);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboSourceLayers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRemoveRelate";
            this.Text = "リレートの解除";            
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRemoveRelate_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxAllRemove;
        private System.Windows.Forms.ComboBox comboRealateNames;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboSourceLayers;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}