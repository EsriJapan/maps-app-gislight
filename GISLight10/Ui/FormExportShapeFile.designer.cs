namespace ESRIJapan.GISLight10.Ui
{
    partial class FormExportShapeFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExportShapeFile));
            this.comboExportFeatureArea = new System.Windows.Forms.ComboBox();
            this.labelExportFeatureArea = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupSelectField = new System.Windows.Forms.GroupBox();
            this.buttonUnSelect = new System.Windows.Forms.Button();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.buttonSelectNone = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.dataSelectField = new System.Windows.Forms.DataGridView();
            this.ColumnFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSelectField = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.saveShapeFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.labelExplanation = new System.Windows.Forms.Label();
            this.tabControlOutPut = new System.Windows.Forms.TabControl();
            this.tabPageShape = new System.Windows.Forms.TabPage();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.textExportFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageGDB = new System.Windows.Forms.TabPage();
            this.textExportFeature = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.buttonSelectWorkspace = new System.Windows.Forms.Button();
            this.textExportWorkspace = new System.Windows.Forms.TextBox();
            this.labelExportFile = new System.Windows.Forms.Label();
            this.groupSelectField.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSelectField)).BeginInit();
            this.tabControlOutPut.SuspendLayout();
            this.tabPageShape.SuspendLayout();
            this.tabPageGDB.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboExportFeatureArea
            // 
            this.comboExportFeatureArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboExportFeatureArea.FormattingEnabled = true;
            this.comboExportFeatureArea.Location = new System.Drawing.Point(8, 210);
            this.comboExportFeatureArea.Name = "comboExportFeatureArea";
            this.comboExportFeatureArea.Size = new System.Drawing.Size(456, 20);
            this.comboExportFeatureArea.TabIndex = 3;
            // 
            // labelExportFeatureArea
            // 
            this.labelExportFeatureArea.Location = new System.Drawing.Point(4, 193);
            this.labelExportFeatureArea.Name = "labelExportFeatureArea";
            this.labelExportFeatureArea.Size = new System.Drawing.Size(160, 14);
            this.labelExportFeatureArea.TabIndex = 2;
            this.labelExportFeatureArea.Text = "エクスポートするフィーチャの範囲:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(296, 602);
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
            this.buttonCancel.Location = new System.Drawing.Point(384, 602);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupSelectField
            // 
            this.groupSelectField.Controls.Add(this.buttonUnSelect);
            this.groupSelectField.Controls.Add(this.buttonSelect);
            this.groupSelectField.Controls.Add(this.buttonSelectNone);
            this.groupSelectField.Controls.Add(this.buttonSelectAll);
            this.groupSelectField.Controls.Add(this.dataSelectField);
            this.groupSelectField.Location = new System.Drawing.Point(8, 247);
            this.groupSelectField.Name = "groupSelectField";
            this.groupSelectField.Size = new System.Drawing.Size(456, 336);
            this.groupSelectField.TabIndex = 7;
            this.groupSelectField.TabStop = false;
            this.groupSelectField.Text = "フィールド選択";
            // 
            // buttonUnSelect
            // 
            this.buttonUnSelect.Location = new System.Drawing.Point(200, 304);
            this.buttonUnSelect.Name = "buttonUnSelect";
            this.buttonUnSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonUnSelect.TabIndex = 10;
            this.buttonUnSelect.Text = "解除";
            this.buttonUnSelect.UseVisualStyleBackColor = true;
            this.buttonUnSelect.Click += new System.EventHandler(this.buttonUnSelect_Click);
            // 
            // buttonSelect
            // 
            this.buttonSelect.Location = new System.Drawing.Point(120, 304);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonSelect.TabIndex = 9;
            this.buttonSelect.Text = "選択";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // buttonSelectNone
            // 
            this.buttonSelectNone.Location = new System.Drawing.Point(368, 304);
            this.buttonSelectNone.Name = "buttonSelectNone";
            this.buttonSelectNone.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectNone.TabIndex = 12;
            this.buttonSelectNone.Text = "全解除";
            this.buttonSelectNone.UseVisualStyleBackColor = true;
            this.buttonSelectNone.Click += new System.EventHandler(this.buttonSelectNone_Click);
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Location = new System.Drawing.Point(288, 304);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectAll.TabIndex = 11;
            this.buttonSelectAll.Text = "全選択";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // dataSelectField
            // 
            this.dataSelectField.AllowUserToAddRows = false;
            this.dataSelectField.AllowUserToDeleteRows = false;
            this.dataSelectField.AllowUserToResizeColumns = false;
            this.dataSelectField.AllowUserToResizeRows = false;
            this.dataSelectField.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataSelectField.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnFieldName,
            this.ColumnSelectField});
            this.dataSelectField.Location = new System.Drawing.Point(11, 20);
            this.dataSelectField.Name = "dataSelectField";
            this.dataSelectField.RowHeadersWidth = 26;
            this.dataSelectField.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataSelectField.RowTemplate.Height = 21;
            this.dataSelectField.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataSelectField.Size = new System.Drawing.Size(429, 279);
            this.dataSelectField.TabIndex = 8;
            // 
            // ColumnFieldName
            // 
            this.ColumnFieldName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnFieldName.FillWeight = 213F;
            this.ColumnFieldName.HeaderText = "フィールド名";
            this.ColumnFieldName.MinimumWidth = 213;
            this.ColumnFieldName.Name = "ColumnFieldName";
            this.ColumnFieldName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnFieldName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnSelectField
            // 
            this.ColumnSelectField.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnSelectField.FillWeight = 48F;
            this.ColumnSelectField.HeaderText = "選択";
            this.ColumnSelectField.MinimumWidth = 48;
            this.ColumnSelectField.Name = "ColumnSelectField";
            this.ColumnSelectField.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // saveShapeFileDialog
            // 
            this.saveShapeFileDialog.Filter = "シェープファイル (*.shp)|*.shp";
            this.saveShapeFileDialog.FilterIndex = 0;
            this.saveShapeFileDialog.RestoreDirectory = true;
            this.saveShapeFileDialog.Title = "出力シェープファイルを指定してください";
            // 
            // labelExplanation
            // 
            this.labelExplanation.AutoSize = true;
            this.labelExplanation.Location = new System.Drawing.Point(8, 8);
            this.labelExplanation.Name = "labelExplanation";
            this.labelExplanation.Size = new System.Drawing.Size(231, 12);
            this.labelExplanation.TabIndex = 1;
            this.labelExplanation.Text = "レイヤをフィーチャクラスへエクスポートを行います。";
            // 
            // tabControlOutPut
            // 
            this.tabControlOutPut.Controls.Add(this.tabPageShape);
            this.tabControlOutPut.Controls.Add(this.tabPageGDB);
            this.tabControlOutPut.Location = new System.Drawing.Point(8, 43);
            this.tabControlOutPut.Name = "tabControlOutPut";
            this.tabControlOutPut.SelectedIndex = 0;
            this.tabControlOutPut.Size = new System.Drawing.Size(456, 147);
            this.tabControlOutPut.TabIndex = 59;
            // 
            // tabPageShape
            // 
            this.tabPageShape.Controls.Add(this.buttonSelectFile);
            this.tabPageShape.Controls.Add(this.textExportFile);
            this.tabPageShape.Controls.Add(this.label1);
            this.tabPageShape.Location = new System.Drawing.Point(4, 22);
            this.tabPageShape.Name = "tabPageShape";
            this.tabPageShape.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageShape.Size = new System.Drawing.Size(448, 121);
            this.tabPageShape.TabIndex = 0;
            this.tabPageShape.Text = "シェープファイル";
            this.tabPageShape.UseVisualStyleBackColor = true;
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(410, 43);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(28, 23);
            this.buttonSelectFile.TabIndex = 64;
            this.buttonSelectFile.Text = "...";
            this.buttonSelectFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
            // 
            // textExportFile
            // 
            this.textExportFile.Location = new System.Drawing.Point(7, 45);
            this.textExportFile.Name = "textExportFile";
            this.textExportFile.ReadOnly = true;
            this.textExportFile.Size = new System.Drawing.Size(394, 19);
            this.textExportFile.TabIndex = 63;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 14);
            this.label1.TabIndex = 62;
            this.label1.Text = "出力ファイル:";
            // 
            // tabPageGDB
            // 
            this.tabPageGDB.Controls.Add(this.textExportFeature);
            this.tabPageGDB.Controls.Add(this.label22);
            this.tabPageGDB.Controls.Add(this.buttonSelectWorkspace);
            this.tabPageGDB.Controls.Add(this.textExportWorkspace);
            this.tabPageGDB.Controls.Add(this.labelExportFile);
            this.tabPageGDB.Location = new System.Drawing.Point(4, 22);
            this.tabPageGDB.Name = "tabPageGDB";
            this.tabPageGDB.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGDB.Size = new System.Drawing.Size(448, 121);
            this.tabPageGDB.TabIndex = 1;
            this.tabPageGDB.Text = "ジオデータベース";
            this.tabPageGDB.UseVisualStyleBackColor = true;
            // 
            // textExportFeature
            // 
            this.textExportFeature.Location = new System.Drawing.Point(10, 74);
            this.textExportFeature.Name = "textExportFeature";
            this.textExportFeature.Size = new System.Drawing.Size(394, 19);
            this.textExportFeature.TabIndex = 62;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(8, 59);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(108, 12);
            this.label22.TabIndex = 63;
            this.label22.Text = "出力フィーチャクラス名";
            // 
            // buttonSelectWorkspace
            // 
            this.buttonSelectWorkspace.Location = new System.Drawing.Point(411, 24);
            this.buttonSelectWorkspace.Name = "buttonSelectWorkspace";
            this.buttonSelectWorkspace.Size = new System.Drawing.Size(28, 23);
            this.buttonSelectWorkspace.TabIndex = 61;
            this.buttonSelectWorkspace.Text = "...";
            this.buttonSelectWorkspace.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonSelectWorkspace.UseVisualStyleBackColor = true;
            this.buttonSelectWorkspace.Click += new System.EventHandler(this.buttonSelectWorkspace_Click);
            // 
            // textExportWorkspace
            // 
            this.textExportWorkspace.Location = new System.Drawing.Point(10, 27);
            this.textExportWorkspace.Name = "textExportWorkspace";
            this.textExportWorkspace.ReadOnly = true;
            this.textExportWorkspace.Size = new System.Drawing.Size(394, 19);
            this.textExportWorkspace.TabIndex = 60;
            // 
            // labelExportFile
            // 
            this.labelExportFile.Location = new System.Drawing.Point(8, 10);
            this.labelExportFile.Name = "labelExportFile";
            this.labelExportFile.Size = new System.Drawing.Size(102, 14);
            this.labelExportFile.TabIndex = 59;
            this.labelExportFile.Text = "出力ワークスペース:";
            // 
            // FormExportShapeFile
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(474, 633);
            this.Controls.Add(this.tabControlOutPut);
            this.Controls.Add(this.labelExplanation);
            this.Controls.Add(this.groupSelectField);
            this.Controls.Add(this.comboExportFeatureArea);
            this.Controls.Add(this.labelExportFeatureArea);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExportShapeFile";
            this.Text = "フィーチャクラスへエクスポート";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormExportShapeFile_FormClosed);
            this.Load += new System.EventHandler(this.FormExportShapeFile_Load);
            this.groupSelectField.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataSelectField)).EndInit();
            this.tabControlOutPut.ResumeLayout(false);
            this.tabPageShape.ResumeLayout(false);
            this.tabPageShape.PerformLayout();
            this.tabPageGDB.ResumeLayout(false);
            this.tabPageGDB.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboExportFeatureArea;
        private System.Windows.Forms.Label labelExportFeatureArea;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupSelectField;
		private System.Windows.Forms.DataGridView dataSelectField;
		private System.Windows.Forms.Button buttonSelectNone;
		private System.Windows.Forms.Button buttonSelectAll;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFieldName;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnSelectField;
		private System.Windows.Forms.SaveFileDialog saveShapeFileDialog;
		private System.Windows.Forms.Label labelExplanation;
		private System.Windows.Forms.Button buttonUnSelect;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.TabControl tabControlOutPut;
        private System.Windows.Forms.TabPage tabPageShape;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.TextBox textExportFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageGDB;
        private System.Windows.Forms.TextBox textExportFeature;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button buttonSelectWorkspace;
        private System.Windows.Forms.TextBox textExportWorkspace;
        private System.Windows.Forms.Label labelExportFile;
    }
}