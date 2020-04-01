namespace ESRIJapan.GISLight10.Ui
{
    partial class FormDefineFieldNameAlias
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
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonSelectFile = new System.Windows.Forms.Button();
			this.textFieldNameAliasDefFile = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.dataGridViewFieldNameAlias = new System.Windows.Forms.DataGridView();
			this.VisColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.CurColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.button_SaveDefFile = new System.Windows.Forms.Button();
			this.checkBox_AllSelect = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFieldNameAlias)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "フィールドの別名を定義します。";
			// 
			// buttonSelectFile
			// 
			this.buttonSelectFile.Image = global::ESRIJapan.GISLight10.Properties.Resources.CadastralJobLoad16;
			this.buttonSelectFile.Location = new System.Drawing.Point(404, 296);
			this.buttonSelectFile.Name = "buttonSelectFile";
			this.buttonSelectFile.Size = new System.Drawing.Size(28, 23);
			this.buttonSelectFile.TabIndex = 5;
			this.buttonSelectFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolTip1.SetToolTip(this.buttonSelectFile, "別名定義ファイルを開く...");
			this.buttonSelectFile.UseVisualStyleBackColor = true;
			this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
			// 
			// textFieldNameAliasDefFile
			// 
			this.textFieldNameAliasDefFile.Location = new System.Drawing.Point(12, 298);
			this.textFieldNameAliasDefFile.Name = "textFieldNameAliasDefFile";
			this.textFieldNameAliasDefFile.ReadOnly = true;
			this.textFieldNameAliasDefFile.Size = new System.Drawing.Size(386, 19);
			this.textFieldNameAliasDefFile.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 283);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "別名定義テーブル:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(388, 335);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(300, 335);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// dataGridViewFieldNameAlias
			// 
			this.dataGridViewFieldNameAlias.AllowUserToAddRows = false;
			this.dataGridViewFieldNameAlias.AllowUserToDeleteRows = false;
			this.dataGridViewFieldNameAlias.AllowUserToResizeRows = false;
			this.dataGridViewFieldNameAlias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dataGridViewFieldNameAlias.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VisColumn,
            this.CurColumn,
            this.DefColumn});
			this.dataGridViewFieldNameAlias.Location = new System.Drawing.Point(12, 57);
			this.dataGridViewFieldNameAlias.Name = "dataGridViewFieldNameAlias";
			this.dataGridViewFieldNameAlias.RowHeadersVisible = false;
			this.dataGridViewFieldNameAlias.RowHeadersWidth = 4;
			this.dataGridViewFieldNameAlias.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dataGridViewFieldNameAlias.RowTemplate.Height = 21;
			this.dataGridViewFieldNameAlias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewFieldNameAlias.Size = new System.Drawing.Size(451, 209);
			this.dataGridViewFieldNameAlias.TabIndex = 2;
			// 
			// VisColumn
			// 
			this.VisColumn.HeaderText = "表示";
			this.VisColumn.Name = "VisColumn";
			this.VisColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.VisColumn.Width = 40;
			// 
			// CurColumn
			// 
			this.CurColumn.HeaderText = "フィールド名";
			this.CurColumn.Name = "CurColumn";
			this.CurColumn.Width = 190;
			// 
			// DefColumn
			// 
			this.DefColumn.HeaderText = "別名";
			this.DefColumn.Name = "DefColumn";
			this.DefColumn.ReadOnly = true;
			this.DefColumn.Width = 190;
			// 
			// button_SaveDefFile
			// 
			this.button_SaveDefFile.Image = global::ESRIJapan.GISLight10.Properties.Resources.CadastralJobSaveToFile16;
			this.button_SaveDefFile.Location = new System.Drawing.Point(435, 296);
			this.button_SaveDefFile.Name = "button_SaveDefFile";
			this.button_SaveDefFile.Size = new System.Drawing.Size(28, 23);
			this.button_SaveDefFile.TabIndex = 6;
			this.button_SaveDefFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolTip1.SetToolTip(this.button_SaveDefFile, "別名定義ファイルに保存...");
			this.button_SaveDefFile.UseVisualStyleBackColor = true;
			this.button_SaveDefFile.Click += new System.EventHandler(this.button_SaveDefFile_Click);
			// 
			// checkBox_AllSelect
			// 
			this.checkBox_AllSelect.AutoSize = true;
			this.checkBox_AllSelect.Location = new System.Drawing.Point(25, 39);
			this.checkBox_AllSelect.Name = "checkBox_AllSelect";
			this.checkBox_AllSelect.Size = new System.Drawing.Size(96, 16);
			this.checkBox_AllSelect.TabIndex = 9;
			this.checkBox_AllSelect.Text = "全選択／クリア";
			this.checkBox_AllSelect.UseVisualStyleBackColor = true;
			this.checkBox_AllSelect.CheckedChanged += new System.EventHandler(this.checkBox_AllSelect_CheckedChanged);
			// 
			// FormDefineFieldNameAlias
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(474, 370);
			this.Controls.Add(this.checkBox_AllSelect);
			this.Controls.Add(this.button_SaveDefFile);
			this.Controls.Add(this.dataGridViewFieldNameAlias);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonSelectFile);
			this.Controls.Add(this.textFieldNameAliasDefFile);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FormDefineFieldNameAlias";
			this.Text = "フィールドの別名定義";
			this.Load += new System.EventHandler(this.FormDefineFieldNameAlias_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFieldNameAlias)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.TextBox textFieldNameAliasDefFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.DataGridView dataGridViewFieldNameAlias;
		private System.Windows.Forms.DataGridViewCheckBoxColumn VisColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn CurColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefColumn;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button button_SaveDefFile;
		private System.Windows.Forms.CheckBox checkBox_AllSelect;
    }
}