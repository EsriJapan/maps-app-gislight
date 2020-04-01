namespace ESRIJapan.GISLight10.Ui
{
	partial class FormAttributeSum
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
            this.lbl_SummaryField = new System.Windows.Forms.Label();
            this.lbl_FeatureLayer = new System.Windows.Forms.Label();
            this.cmb_SummaryFieldName = new System.Windows.Forms.ComboBox();
            this.btn_Run = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cmb_FeatureLayerName = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Down = new System.Windows.Forms.Button();
            this.btn_Up = new System.Windows.Forms.Button();
            this.btn_RemoveField = new System.Windows.Forms.Button();
            this.btn_AddField = new System.Windows.Forms.Button();
            this.listBox_SelectedKeyFields = new System.Windows.Forms.ListBox();
            this.listBox_Fields = new System.Windows.Forms.ListBox();
            this.txt_SaveFilePath = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btn_SaveFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.chkSelectionSet = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSaveFileName = new System.Windows.Forms.TextBox();
            this.txtSaveFilePath = new System.Windows.Forms.TextBox();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_SummaryField
            // 
            this.lbl_SummaryField.AutoSize = true;
            this.lbl_SummaryField.Location = new System.Drawing.Point(10, 518);
            this.lbl_SummaryField.Name = "lbl_SummaryField";
            this.lbl_SummaryField.Size = new System.Drawing.Size(94, 12);
            this.lbl_SummaryField.TabIndex = 11;
            this.lbl_SummaryField.Text = "集計するフィールド:";
            // 
            // lbl_FeatureLayer
            // 
            this.lbl_FeatureLayer.AutoSize = true;
            this.lbl_FeatureLayer.Location = new System.Drawing.Point(8, 157);
            this.lbl_FeatureLayer.Name = "lbl_FeatureLayer";
            this.lbl_FeatureLayer.Size = new System.Drawing.Size(35, 12);
            this.lbl_FeatureLayer.TabIndex = 2;
            this.lbl_FeatureLayer.Text = "レイヤ:";
            // 
            // cmb_SummaryFieldName
            // 
            this.cmb_SummaryFieldName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_SummaryFieldName.FormattingEnabled = true;
            this.cmb_SummaryFieldName.Location = new System.Drawing.Point(10, 534);
            this.cmb_SummaryFieldName.Name = "cmb_SummaryFieldName";
            this.cmb_SummaryFieldName.Size = new System.Drawing.Size(456, 20);
            this.cmb_SummaryFieldName.TabIndex = 12;
            // 
            // btn_Run
            // 
            this.btn_Run.Location = new System.Drawing.Point(299, 569);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(75, 23);
            this.btn_Run.TabIndex = 16;
            this.btn_Run.Text = "OK";
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(387, 569);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 17;
            this.btn_Cancel.Text = "キャンセル";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(454, 36);
            this.label4.TabIndex = 1;
            this.label4.Text = "キー フィールドを属性値に基づいてグループ化し、グループごとに指定したフィールドを集計します。\r\nキー フィールドは最大3つまで設定できます。\r\nグループ化の優先" +
                "度はキー フィールドの追加後、上から順に高くなります。";
            // 
            // cmb_FeatureLayerName
            // 
            this.cmb_FeatureLayerName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_FeatureLayerName.FormattingEnabled = true;
            this.cmb_FeatureLayerName.Location = new System.Drawing.Point(10, 173);
            this.cmb_FeatureLayerName.Name = "cmb_FeatureLayerName";
            this.cmb_FeatureLayerName.Size = new System.Drawing.Size(454, 20);
            this.cmb_FeatureLayerName.TabIndex = 3;
            this.cmb_FeatureLayerName.SelectedIndexChanged += new System.EventHandler(this.cmb_FeatureLayerName_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Down);
            this.groupBox1.Controls.Add(this.btn_Up);
            this.groupBox1.Controls.Add(this.btn_RemoveField);
            this.groupBox1.Controls.Add(this.btn_AddField);
            this.groupBox1.Controls.Add(this.listBox_SelectedKeyFields);
            this.groupBox1.Controls.Add(this.listBox_Fields);
            this.groupBox1.Location = new System.Drawing.Point(12, 226);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 280);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "キー フィールドの選択";
            // 
            // btn_Down
            // 
            this.btn_Down.Location = new System.Drawing.Point(424, 232);
            this.btn_Down.Name = "btn_Down";
            this.btn_Down.Size = new System.Drawing.Size(22, 21);
            this.btn_Down.TabIndex = 10;
            this.btn_Down.Text = "↓";
            this.btn_Down.UseVisualStyleBackColor = true;
            this.btn_Down.Click += new System.EventHandler(this.btn_Down_Click);
            // 
            // btn_Up
            // 
            this.btn_Up.Location = new System.Drawing.Point(424, 208);
            this.btn_Up.Name = "btn_Up";
            this.btn_Up.Size = new System.Drawing.Size(22, 21);
            this.btn_Up.TabIndex = 9;
            this.btn_Up.Text = "↑";
            this.btn_Up.UseVisualStyleBackColor = true;
            this.btn_Up.Click += new System.EventHandler(this.btn_Up_Click);
            // 
            // btn_RemoveField
            // 
            this.btn_RemoveField.Location = new System.Drawing.Point(112, 168);
            this.btn_RemoveField.Name = "btn_RemoveField";
            this.btn_RemoveField.Size = new System.Drawing.Size(73, 22);
            this.btn_RemoveField.TabIndex = 7;
            this.btn_RemoveField.Text = "削除";
            this.btn_RemoveField.UseVisualStyleBackColor = true;
            this.btn_RemoveField.Click += new System.EventHandler(this.DeleteKeyField);
            // 
            // btn_AddField
            // 
            this.btn_AddField.Location = new System.Drawing.Point(24, 168);
            this.btn_AddField.Name = "btn_AddField";
            this.btn_AddField.Size = new System.Drawing.Size(73, 22);
            this.btn_AddField.TabIndex = 6;
            this.btn_AddField.Text = "追加";
            this.btn_AddField.UseVisualStyleBackColor = true;
            this.btn_AddField.Click += new System.EventHandler(this.AddKeyField);
            // 
            // listBox_SelectedKeyFields
            // 
            this.listBox_SelectedKeyFields.FormattingEnabled = true;
            this.listBox_SelectedKeyFields.ItemHeight = 12;
            this.listBox_SelectedKeyFields.Location = new System.Drawing.Point(16, 200);
            this.listBox_SelectedKeyFields.Name = "listBox_SelectedKeyFields";
            this.listBox_SelectedKeyFields.Size = new System.Drawing.Size(400, 64);
            this.listBox_SelectedKeyFields.TabIndex = 8;
            this.listBox_SelectedKeyFields.DoubleClick += new System.EventHandler(this.DeleteKeyField);
            // 
            // listBox_Fields
            // 
            this.listBox_Fields.FormattingEnabled = true;
            this.listBox_Fields.ItemHeight = 12;
            this.listBox_Fields.Location = new System.Drawing.Point(16, 24);
            this.listBox_Fields.Name = "listBox_Fields";
            this.listBox_Fields.Size = new System.Drawing.Size(400, 136);
            this.listBox_Fields.TabIndex = 5;
            this.listBox_Fields.DoubleClick += new System.EventHandler(this.AddKeyField);
            // 
            // txt_SaveFilePath
            // 
            this.txt_SaveFilePath.Location = new System.Drawing.Point(6, 30);
            this.txt_SaveFilePath.Name = "txt_SaveFilePath";
            this.txt_SaveFilePath.ReadOnly = true;
            this.txt_SaveFilePath.Size = new System.Drawing.Size(406, 19);
            this.txt_SaveFilePath.TabIndex = 14;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "CSV (*.csv)|*.csv|DBF (*.dbf)|*.dbf";
            // 
            // btn_SaveFile
            // 
            this.btn_SaveFile.Location = new System.Drawing.Point(417, 29);
            this.btn_SaveFile.Name = "btn_SaveFile";
            this.btn_SaveFile.Size = new System.Drawing.Size(31, 22);
            this.btn_SaveFile.TabIndex = 15;
            this.btn_SaveFile.Text = "...";
            this.btn_SaveFile.UseVisualStyleBackColor = true;
            this.btn_SaveFile.Click += new System.EventHandler(this.btn_SaveFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "出力ファイル:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(513, 96);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(478, 312);
            this.dataGridView1.TabIndex = 18;
            this.dataGridView1.TabStop = false;
            // 
            // chkSelectionSet
            // 
            this.chkSelectionSet.AutoSize = true;
            this.chkSelectionSet.Location = new System.Drawing.Point(14, 199);
            this.chkSelectionSet.Name = "chkSelectionSet";
            this.chkSelectionSet.Size = new System.Drawing.Size(201, 16);
            this.chkSelectionSet.TabIndex = 19;
            this.chkSelectionSet.Text = "選択されているフィーチャのみ集計する";
            this.chkSelectionSet.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(10, 47);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(458, 98);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txt_SaveFilePath);
            this.tabPage1.Controls.Add(this.btn_SaveFile);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(450, 72);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ファイル";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtSaveFileName);
            this.tabPage2.Controls.Add(this.txtSaveFilePath);
            this.tabPage2.Controls.Add(this.btnSaveFile);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(450, 72);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ジオデータベース";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 12);
            this.label3.TabIndex = 20;
            this.label3.Text = "出力テーブル名:";
            // 
            // txtSaveFileName
            // 
            this.txtSaveFileName.Location = new System.Drawing.Point(104, 37);
            this.txtSaveFileName.Name = "txtSaveFileName";
            this.txtSaveFileName.Size = new System.Drawing.Size(307, 19);
            this.txtSaveFileName.TabIndex = 19;
            // 
            // txtSaveFilePath
            // 
            this.txtSaveFilePath.Location = new System.Drawing.Point(104, 14);
            this.txtSaveFilePath.Name = "txtSaveFilePath";
            this.txtSaveFilePath.ReadOnly = true;
            this.txtSaveFilePath.Size = new System.Drawing.Size(307, 19);
            this.txtSaveFilePath.TabIndex = 17;
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Location = new System.Drawing.Point(416, 14);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(31, 22);
            this.btnSaveFile.TabIndex = 18;
            this.btnSaveFile.Text = "...";
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "出力ワークスペース:";
            // 
            // FormAttributeSum
            // 
            this.AcceptButton = this.btn_Run;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(474, 604);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkSelectionSet);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Run);
            this.Controls.Add(this.cmb_SummaryFieldName);
            this.Controls.Add(this.lbl_FeatureLayer);
            this.Controls.Add(this.cmb_FeatureLayerName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_SummaryField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAttributeSum";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "属性値集計";
            this.Load += new System.EventHandler(this.FormAttributeSum_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Label lbl_SummaryField;
        private System.Windows.Forms.Label lbl_FeatureLayer;
		private System.Windows.Forms.ComboBox cmb_SummaryFieldName;
		private System.Windows.Forms.Button btn_Run;
		private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmb_FeatureLayerName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Down;
        private System.Windows.Forms.Button btn_Up;
        private System.Windows.Forms.Button btn_RemoveField;
        private System.Windows.Forms.Button btn_AddField;
        private System.Windows.Forms.ListBox listBox_SelectedKeyFields;
        private System.Windows.Forms.ListBox listBox_Fields;

        private System.Windows.Forms.TextBox txt_SaveFilePath;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btn_SaveFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox chkSelectionSet;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtSaveFilePath;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSaveFileName;

	}
}