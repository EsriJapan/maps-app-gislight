namespace ESRIJapan.GISLight10.Ui
{
    partial class FormSpatialSearch
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboSetOperation = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkedListTargetLayer = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboSpatialRel = new System.Windows.Forms.ComboBox();
            this.checkSelectedFeature = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboSourceLayer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(466, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "入力レイヤと選択レイヤのフィーチャの位置関係に基づいて、選択レイヤからフィーチャを選択します。";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(208, 424);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 12;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(384, 424);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 14;
            this.buttonClose.Text = "キャンセル";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboSetOperation);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.checkedListTargetLayer);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboSpatialRel);
            this.groupBox1.Controls.Add(this.checkSelectedFeature);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboSourceLayer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(8, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 352);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "選択条件文";
            // 
            // comboSetOperation
            // 
            this.comboSetOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSetOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSetOperation.FormattingEnabled = true;
            this.comboSetOperation.Location = new System.Drawing.Point(16, 312);
            this.comboSetOperation.Name = "comboSetOperation";
            this.comboSetOperation.Size = new System.Drawing.Size(424, 20);
            this.comboSetOperation.TabIndex = 11;
            this.comboSetOperation.SelectedIndexChanged += new System.EventHandler(this.comboSetOperation_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 296);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "のフィーチャを検索し、";
            // 
            // checkedListTargetLayer
            // 
            this.checkedListTargetLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListTargetLayer.CheckOnClick = true;
            this.checkedListTargetLayer.FormattingEnabled = true;
            this.checkedListTargetLayer.Location = new System.Drawing.Point(16, 152);
            this.checkedListTargetLayer.Name = "checkedListTargetLayer";
            this.checkedListTargetLayer.Size = new System.Drawing.Size(424, 130);
            this.checkedListTargetLayer.TabIndex = 9;
            this.checkedListTargetLayer.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListTargetLayer_ItemCheck);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "選択レイヤ:";
            // 
            // comboSpatialRel
            // 
            this.comboSpatialRel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSpatialRel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSpatialRel.FormattingEnabled = true;
            this.comboSpatialRel.Location = new System.Drawing.Point(16, 96);
            this.comboSpatialRel.Name = "comboSpatialRel";
            this.comboSpatialRel.Size = new System.Drawing.Size(424, 20);
            this.comboSpatialRel.TabIndex = 7;
            this.comboSpatialRel.SelectedIndexChanged += new System.EventHandler(this.comboSpatialRel_SelectedIndexChanged);
            // 
            // checkSelectedFeature
            // 
            this.checkSelectedFeature.AutoSize = true;
            this.checkSelectedFeature.Checked = true;
            this.checkSelectedFeature.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSelectedFeature.Location = new System.Drawing.Point(82, 78);
            this.checkSelectedFeature.Name = "checkSelectedFeature";
            this.checkSelectedFeature.Size = new System.Drawing.Size(177, 16);
            this.checkSelectedFeature.TabIndex = 6;
            this.checkSelectedFeature.Text = "のうち、選択されているフィーチャ）";
            this.checkSelectedFeature.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "のフィーチャ（";
            // 
            // comboSourceLayer
            // 
            this.comboSourceLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSourceLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSourceLayer.FormattingEnabled = true;
            this.comboSourceLayer.Location = new System.Drawing.Point(16, 40);
            this.comboSourceLayer.Name = "comboSourceLayer";
            this.comboSourceLayer.Size = new System.Drawing.Size(424, 20);
            this.comboSourceLayer.TabIndex = 4;
            this.comboSourceLayer.SelectedIndexChanged += new System.EventHandler(this.comboSourceLayer_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "入力レイヤ:";
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(296, 424);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 23);
            this.buttonAccept.TabIndex = 13;
            this.buttonAccept.Text = "適用";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // FormSpatialSearch
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(474, 457);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSpatialSearch";
            this.Text = "空間検索";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboSourceLayer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkSelectedFeature;
        private System.Windows.Forms.ComboBox comboSetOperation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckedListBox checkedListTargetLayer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboSpatialRel;
        private System.Windows.Forms.Button buttonAccept;
    }
}