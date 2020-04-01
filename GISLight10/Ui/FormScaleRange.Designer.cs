namespace ESRIJapan.GISLight10.Ui
{
    partial class FormScaleRange
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxMax = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxMin = new System.Windows.Forms.ComboBox();
            this.radioButtonScaleRange = new System.Windows.Forms.RadioButton();
            this.radioButtonNoScaleRange = new System.Windows.Forms.RadioButton();
            this.tabControlRange = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLayerMax = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxLayerMin = new System.Windows.Forms.ComboBox();
            this.radioButtonLayerScaleRange = new System.Windows.Forms.RadioButton();
            this.radioButtonNoLayerScaleRange = new System.Windows.Forms.RadioButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnAccept = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tabControlRange.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(185, 233);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(366, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBoxMax);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxMin);
            this.groupBox1.Controls.Add(this.radioButtonScaleRange);
            this.groupBox1.Controls.Add(this.radioButtonNoScaleRange);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 177);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "より縮小時に表示　（最大表示縮尺）";
            // 
            // comboBoxMax
            // 
            this.comboBoxMax.FormattingEnabled = true;
            this.comboBoxMax.Items.AddRange(new object[] {
            "<なし>",
            "1:1,000",
            "1:10,000",
            "1:24,000",
            "1:100,000",
            "1:250,000",
            "1:500,000",
            "1:750,000",
            "1:1,000,000",
            "1:3,000,000",
            "1:10,000,000"});
            this.comboBoxMax.Location = new System.Drawing.Point(32, 137);
            this.comboBoxMax.MaxDropDownItems = 11;
            this.comboBoxMax.Name = "comboBoxMax";
            this.comboBoxMax.Size = new System.Drawing.Size(152, 20);
            this.comboBoxMax.TabIndex = 4;
            this.comboBoxMax.Leave += new System.EventHandler(this.comboBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "より拡大時に表示　（最小表示縮尺）";
            // 
            // comboBoxMin
            // 
            this.comboBoxMin.FormattingEnabled = true;
            this.comboBoxMin.Items.AddRange(new object[] {
            "<なし>",
            "1:1,000",
            "1:10,000",
            "1:24,000",
            "1:100,000",
            "1:250,000",
            "1:500,000",
            "1:750,000",
            "1:1,000,000",
            "1:3,000,000",
            "1:10,000,000"});
            this.comboBoxMin.Location = new System.Drawing.Point(32, 95);
            this.comboBoxMin.MaxDropDownItems = 11;
            this.comboBoxMin.Name = "comboBoxMin";
            this.comboBoxMin.Size = new System.Drawing.Size(152, 20);
            this.comboBoxMin.TabIndex = 2;
            this.comboBoxMin.Leave += new System.EventHandler(this.comboBox_Leave);
            // 
            // radioButtonScaleRange
            // 
            this.radioButtonScaleRange.AutoSize = true;
            this.radioButtonScaleRange.Location = new System.Drawing.Point(16, 63);
            this.radioButtonScaleRange.Name = "radioButtonScaleRange";
            this.radioButtonScaleRange.Size = new System.Drawing.Size(143, 16);
            this.radioButtonScaleRange.TabIndex = 1;
            this.radioButtonScaleRange.TabStop = true;
            this.radioButtonScaleRange.Text = "縮尺に応じて表示を制御";
            this.radioButtonScaleRange.UseVisualStyleBackColor = true;
            this.radioButtonScaleRange.CheckedChanged += new System.EventHandler(this.radioButtonScaleRange_CheckedChanged);
            // 
            // radioButtonNoScaleRange
            // 
            this.radioButtonNoScaleRange.AutoSize = true;
            this.radioButtonNoScaleRange.Checked = true;
            this.radioButtonNoScaleRange.Location = new System.Drawing.Point(16, 29);
            this.radioButtonNoScaleRange.Name = "radioButtonNoScaleRange";
            this.radioButtonNoScaleRange.Size = new System.Drawing.Size(157, 16);
            this.radioButtonNoScaleRange.TabIndex = 0;
            this.radioButtonNoScaleRange.TabStop = true;
            this.radioButtonNoScaleRange.Text = "縮尺に関わらずラベルを表示";
            this.radioButtonNoScaleRange.UseVisualStyleBackColor = true;
            this.radioButtonNoScaleRange.CheckedChanged += new System.EventHandler(this.radioButtonNoScaleRange_CheckedChanged);
            // 
            // tabControlRange
            // 
            this.tabControlRange.Controls.Add(this.tabPage2);
            this.tabControlRange.Controls.Add(this.tabPage1);
            this.tabControlRange.Location = new System.Drawing.Point(12, 12);
            this.tabControlRange.Name = "tabControlRange";
            this.tabControlRange.SelectedIndex = 0;
            this.tabControlRange.Size = new System.Drawing.Size(429, 215);
            this.tabControlRange.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(421, 189);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "レイヤ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.comboBoxLayerMax);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.comboBoxLayerMin);
            this.groupBox2.Controls.Add(this.radioButtonLayerScaleRange);
            this.groupBox2.Controls.Add(this.radioButtonNoLayerScaleRange);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(409, 177);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(190, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(183, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "より縮小時に表示　（最大表示縮尺）";
            // 
            // comboBoxLayerMax
            // 
            this.comboBoxLayerMax.FormattingEnabled = true;
            this.comboBoxLayerMax.Items.AddRange(new object[] {
            "<なし>",
            "1:1,000",
            "1:10,000",
            "1:24,000",
            "1:100,000",
            "1:250,000",
            "1:500,000",
            "1:750,000",
            "1:1,000,000",
            "1:3,000,000",
            "1:10,000,000"});
            this.comboBoxLayerMax.Location = new System.Drawing.Point(32, 137);
            this.comboBoxLayerMax.MaxDropDownItems = 11;
            this.comboBoxLayerMax.Name = "comboBoxLayerMax";
            this.comboBoxLayerMax.Size = new System.Drawing.Size(152, 20);
            this.comboBoxLayerMax.TabIndex = 4;
            this.comboBoxLayerMax.Leave += new System.EventHandler(this.comboBox_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(190, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(183, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "より拡大時に表示　（最小表示縮尺）";
            // 
            // comboBoxLayerMin
            // 
            this.comboBoxLayerMin.FormattingEnabled = true;
            this.comboBoxLayerMin.Items.AddRange(new object[] {
            "<なし>",
            "1:1,000",
            "1:10,000",
            "1:24,000",
            "1:100,000",
            "1:250,000",
            "1:500,000",
            "1:750,000",
            "1:1,000,000",
            "1:3,000,000",
            "1:10,000,000"});
            this.comboBoxLayerMin.Location = new System.Drawing.Point(32, 95);
            this.comboBoxLayerMin.MaxDropDownItems = 11;
            this.comboBoxLayerMin.Name = "comboBoxLayerMin";
            this.comboBoxLayerMin.Size = new System.Drawing.Size(152, 20);
            this.comboBoxLayerMin.TabIndex = 2;
            this.comboBoxLayerMin.Leave += new System.EventHandler(this.comboBox_Leave);
            // 
            // radioButtonLayerScaleRange
            // 
            this.radioButtonLayerScaleRange.AutoSize = true;
            this.radioButtonLayerScaleRange.Location = new System.Drawing.Point(16, 63);
            this.radioButtonLayerScaleRange.Name = "radioButtonLayerScaleRange";
            this.radioButtonLayerScaleRange.Size = new System.Drawing.Size(143, 16);
            this.radioButtonLayerScaleRange.TabIndex = 1;
            this.radioButtonLayerScaleRange.TabStop = true;
            this.radioButtonLayerScaleRange.Text = "縮尺に応じて表示を制御";
            this.radioButtonLayerScaleRange.UseVisualStyleBackColor = true;
            this.radioButtonLayerScaleRange.CheckedChanged += new System.EventHandler(this.radioButtonLayerScaleRange_CheckedChanged);
            // 
            // radioButtonNoLayerScaleRange
            // 
            this.radioButtonNoLayerScaleRange.AutoSize = true;
            this.radioButtonNoLayerScaleRange.Checked = true;
            this.radioButtonNoLayerScaleRange.Location = new System.Drawing.Point(16, 29);
            this.radioButtonNoLayerScaleRange.Name = "radioButtonNoLayerScaleRange";
            this.radioButtonNoLayerScaleRange.Size = new System.Drawing.Size(157, 16);
            this.radioButtonNoLayerScaleRange.TabIndex = 0;
            this.radioButtonNoLayerScaleRange.TabStop = true;
            this.radioButtonNoLayerScaleRange.Text = "縮尺に関わらずレイヤを表示";
            this.radioButtonNoLayerScaleRange.UseVisualStyleBackColor = true;
            this.radioButtonNoLayerScaleRange.CheckedChanged += new System.EventHandler(this.radioButtonNoLayerScaleRange_CheckedChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(421, 189);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ラベル";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(276, 233);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 7;
            this.btnAccept.Text = "適用";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // FormScaleRange
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(452, 265);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.tabControlRange);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormScaleRange";
            this.Text = "縮尺範囲";
            this.Load += new System.EventHandler(this.FormScaleRange_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControlRange.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonNoScaleRange;
        private System.Windows.Forms.ComboBox comboBoxMin;
        private System.Windows.Forms.RadioButton radioButtonScaleRange;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControlRange;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxLayerMax;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxLayerMin;
        private System.Windows.Forms.RadioButton radioButtonLayerScaleRange;
        private System.Windows.Forms.RadioButton radioButtonNoLayerScaleRange;
        private System.Windows.Forms.Button btnAccept;
    }
}