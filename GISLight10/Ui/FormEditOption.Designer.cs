namespace ESRIJapan.GISLight10.Ui
{
    partial class FormEditOption
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxStreamTolerance = new ESRIJapan.GISLight10.Common.NumbersAndPointTextBox();
            this.textBoxStreamGroupingCount = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxStickyMoveTolerance = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
            this.textBoxSnapTolerance = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "各編集設定を行います。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "スナップ許容値:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(352, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "ピクセル";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "移動抑制許容値:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(352, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "ピクセル";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxStreamTolerance);
            this.groupBox1.Controls.Add(this.textBoxStreamGroupingCount);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(8, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(408, 88);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ストリーム モード";
            // 
            // textBoxStreamTolerance
            // 
            this.textBoxStreamTolerance.Location = new System.Drawing.Point(168, 21);
            this.textBoxStreamTolerance.Name = "textBoxStreamTolerance";
            this.textBoxStreamTolerance.Size = new System.Drawing.Size(168, 19);
            this.textBoxStreamTolerance.TabIndex = 13;
            this.textBoxStreamTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxStreamTolerance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxStreamTolerance_KeyPress);
            // 
            // textBoxStreamGroupingCount
            // 
            this.textBoxStreamGroupingCount.Location = new System.Drawing.Point(168, 53);
            this.textBoxStreamGroupingCount.Name = "textBoxStreamGroupingCount";
            this.textBoxStreamGroupingCount.Size = new System.Drawing.Size(168, 19);
            this.textBoxStreamGroupingCount.TabIndex = 11;
            this.textBoxStreamGroupingCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxStreamGroupingCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(344, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "ポイント";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "グループ化する頂点数:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(344, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "マップ単位";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "ストリーム許容値:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(248, 224);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 13;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(336, 224);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxStickyMoveTolerance
            // 
            this.textBoxStickyMoveTolerance.Location = new System.Drawing.Point(176, 72);
            this.textBoxStickyMoveTolerance.Name = "textBoxStickyMoveTolerance";
            this.textBoxStickyMoveTolerance.Size = new System.Drawing.Size(168, 19);
            this.textBoxStickyMoveTolerance.TabIndex = 5;
            this.textBoxStickyMoveTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxStickyMoveTolerance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxSnapTolerance
            // 
            this.textBoxSnapTolerance.Location = new System.Drawing.Point(176, 40);
            this.textBoxSnapTolerance.Name = "textBoxSnapTolerance";
            this.textBoxSnapTolerance.Size = new System.Drawing.Size(168, 19);
            this.textBoxSnapTolerance.TabIndex = 2;
            this.textBoxSnapTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxSnapTolerance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // FormEditOption
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(424, 257);
            this.Controls.Add(this.textBoxStickyMoveTolerance);
            this.Controls.Add(this.textBoxSnapTolerance);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditOption";
            this.Text = "オプション";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormEditOption_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxSnapTolerance;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxStickyMoveTolerance;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxStreamGroupingCount;
        private ESRIJapan.GISLight10.Common.NumbersAndPointTextBox textBoxStreamTolerance;
    }
}