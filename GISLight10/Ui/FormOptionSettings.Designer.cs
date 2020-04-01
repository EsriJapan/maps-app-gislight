namespace ESRIJapan.GISLight10.Ui
{
    partial class FormOptionSettings
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
			this.labelExplanation = new System.Windows.Forms.Label();
			this.tabControlOptionSettings = new System.Windows.Forms.TabControl();
			this.tabPageTemporaryFolder = new System.Windows.Forms.TabPage();
			this.buttonDeleteTemporaryFile = new System.Windows.Forms.Button();
			this.labelDeleteTemporaryFile = new System.Windows.Forms.Label();
			this.textBoxTemporaryFolderPath = new System.Windows.Forms.TextBox();
			this.labelTemporaryFolderPath = new System.Windows.Forms.Label();
			this.tabPageMaxRecords = new System.Windows.Forms.TabPage();
			this.groupBoxSymbolSettings = new System.Windows.Forms.GroupBox();
			this.textBoxUniqueValueMax = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
			this.labelUniqueValueMax = new System.Windows.Forms.Label();
			this.groupBoxAttributeSearch = new System.Windows.Forms.GroupBox();
			this.labelIndividualValueDisplayMax = new System.Windows.Forms.Label();
			this.textBoxIndividualValueDisplayMax = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
			this.groupBoxAttributeTable = new System.Windows.Forms.GroupBox();
			this.textBoxAttributeTableDisplayOIDMax = new ESRIJapan.GISLight10.Common.NumbersOnlyTextBox();
			this.labelAttributeTableDisplayOIDMax = new System.Windows.Forms.Label();
			this.tabPageFieldName = new System.Windows.Forms.TabPage();
			this.checkBoxUseFieldNameAlias = new System.Windows.Forms.CheckBox();
			this.tabPageGeopro = new System.Windows.Forms.TabPage();
			this.checkBoxGpBackground = new System.Windows.Forms.CheckBox();
			this.tabPageResetSettings = new System.Windows.Forms.TabPage();
			this.labelResetSettingFile = new System.Windows.Forms.Label();
			this.buttonResetSettingFile = new System.Windows.Forms.Button();
			this.textBoxSettingFilePath = new System.Windows.Forms.TextBox();
			this.labelSettingsFilePath = new System.Windows.Forms.Label();
			this.tabPageFileRelate = new System.Windows.Forms.TabPage();
			this.checkBoxRelateMXD = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.tabControlOptionSettings.SuspendLayout();
			this.tabPageTemporaryFolder.SuspendLayout();
			this.tabPageMaxRecords.SuspendLayout();
			this.groupBoxSymbolSettings.SuspendLayout();
			this.groupBoxAttributeSearch.SuspendLayout();
			this.groupBoxAttributeTable.SuspendLayout();
			this.tabPageFieldName.SuspendLayout();
			this.tabPageGeopro.SuspendLayout();
			this.tabPageResetSettings.SuspendLayout();
			this.tabPageFileRelate.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelExplanation
			// 
			this.labelExplanation.AutoSize = true;
			this.labelExplanation.Location = new System.Drawing.Point(8, 8);
			this.labelExplanation.Name = "labelExplanation";
			this.labelExplanation.Size = new System.Drawing.Size(142, 12);
			this.labelExplanation.TabIndex = 0;
			this.labelExplanation.Text = "各オプション設定を行います。";
			// 
			// tabControlOptionSettings
			// 
			this.tabControlOptionSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlOptionSettings.Controls.Add(this.tabPageTemporaryFolder);
			this.tabControlOptionSettings.Controls.Add(this.tabPageMaxRecords);
			this.tabControlOptionSettings.Controls.Add(this.tabPageFieldName);
			this.tabControlOptionSettings.Controls.Add(this.tabPageGeopro);
			this.tabControlOptionSettings.Controls.Add(this.tabPageResetSettings);
			this.tabControlOptionSettings.Controls.Add(this.tabPageFileRelate);
			this.tabControlOptionSettings.Location = new System.Drawing.Point(10, 40);
			this.tabControlOptionSettings.Name = "tabControlOptionSettings";
			this.tabControlOptionSettings.SelectedIndex = 0;
			this.tabControlOptionSettings.Size = new System.Drawing.Size(510, 179);
			this.tabControlOptionSettings.TabIndex = 1;
			// 
			// tabPageTemporaryFolder
			// 
			this.tabPageTemporaryFolder.Controls.Add(this.buttonDeleteTemporaryFile);
			this.tabPageTemporaryFolder.Controls.Add(this.labelDeleteTemporaryFile);
			this.tabPageTemporaryFolder.Controls.Add(this.textBoxTemporaryFolderPath);
			this.tabPageTemporaryFolder.Controls.Add(this.labelTemporaryFolderPath);
			this.tabPageTemporaryFolder.Location = new System.Drawing.Point(4, 22);
			this.tabPageTemporaryFolder.Name = "tabPageTemporaryFolder";
			this.tabPageTemporaryFolder.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageTemporaryFolder.Size = new System.Drawing.Size(502, 153);
			this.tabPageTemporaryFolder.TabIndex = 0;
			this.tabPageTemporaryFolder.Text = "テンポラリフォルダ";
			this.tabPageTemporaryFolder.UseVisualStyleBackColor = true;
			// 
			// buttonDeleteTemporaryFile
			// 
			this.buttonDeleteTemporaryFile.Location = new System.Drawing.Point(8, 88);
			this.buttonDeleteTemporaryFile.Name = "buttonDeleteTemporaryFile";
			this.buttonDeleteTemporaryFile.Size = new System.Drawing.Size(75, 23);
			this.buttonDeleteTemporaryFile.TabIndex = 3;
			this.buttonDeleteTemporaryFile.Text = "削除";
			this.buttonDeleteTemporaryFile.UseVisualStyleBackColor = true;
			this.buttonDeleteTemporaryFile.Click += new System.EventHandler(this.buttonDeleteTemporaryFile_Click);
			// 
			// labelDeleteTemporaryFile
			// 
			this.labelDeleteTemporaryFile.AutoSize = true;
			this.labelDeleteTemporaryFile.Location = new System.Drawing.Point(8, 72);
			this.labelDeleteTemporaryFile.Name = "labelDeleteTemporaryFile";
			this.labelDeleteTemporaryFile.Size = new System.Drawing.Size(174, 12);
			this.labelDeleteTemporaryFile.TabIndex = 0;
			this.labelDeleteTemporaryFile.Text = "テンポラリファイルの削除を行います。";
			// 
			// textBoxTemporaryFolderPath
			// 
			this.textBoxTemporaryFolderPath.Location = new System.Drawing.Point(8, 32);
			this.textBoxTemporaryFolderPath.Name = "textBoxTemporaryFolderPath";
			this.textBoxTemporaryFolderPath.ReadOnly = true;
			this.textBoxTemporaryFolderPath.Size = new System.Drawing.Size(432, 19);
			this.textBoxTemporaryFolderPath.TabIndex = 2;
			// 
			// labelTemporaryFolderPath
			// 
			this.labelTemporaryFolderPath.AutoSize = true;
			this.labelTemporaryFolderPath.Location = new System.Drawing.Point(8, 16);
			this.labelTemporaryFolderPath.Name = "labelTemporaryFolderPath";
			this.labelTemporaryFolderPath.Size = new System.Drawing.Size(85, 12);
			this.labelTemporaryFolderPath.TabIndex = 0;
			this.labelTemporaryFolderPath.Text = "テンポラリフォルダ:";
			// 
			// tabPageMaxRecords
			// 
			this.tabPageMaxRecords.Controls.Add(this.groupBoxSymbolSettings);
			this.tabPageMaxRecords.Controls.Add(this.groupBoxAttributeSearch);
			this.tabPageMaxRecords.Controls.Add(this.groupBoxAttributeTable);
			this.tabPageMaxRecords.Location = new System.Drawing.Point(4, 22);
			this.tabPageMaxRecords.Name = "tabPageMaxRecords";
			this.tabPageMaxRecords.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMaxRecords.Size = new System.Drawing.Size(448, 153);
			this.tabPageMaxRecords.TabIndex = 1;
			this.tabPageMaxRecords.Text = "最大レコード数";
			this.tabPageMaxRecords.UseVisualStyleBackColor = true;
			// 
			// groupBoxSymbolSettings
			// 
			this.groupBoxSymbolSettings.Controls.Add(this.textBoxUniqueValueMax);
			this.groupBoxSymbolSettings.Controls.Add(this.labelUniqueValueMax);
			this.groupBoxSymbolSettings.Location = new System.Drawing.Point(8, 87);
			this.groupBoxSymbolSettings.Name = "groupBoxSymbolSettings";
			this.groupBoxSymbolSettings.Size = new System.Drawing.Size(432, 56);
			this.groupBoxSymbolSettings.TabIndex = 3;
			this.groupBoxSymbolSettings.TabStop = false;
			this.groupBoxSymbolSettings.Text = "シンボル設定";
			// 
			// textBoxUniqueValueMax
			// 
			this.textBoxUniqueValueMax.Location = new System.Drawing.Point(224, 21);
			this.textBoxUniqueValueMax.MaxLength = 8;
			this.textBoxUniqueValueMax.Name = "textBoxUniqueValueMax";
			this.textBoxUniqueValueMax.Size = new System.Drawing.Size(200, 19);
			this.textBoxUniqueValueMax.TabIndex = 6;
			this.textBoxUniqueValueMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBoxUniqueValueMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
			// 
			// labelUniqueValueMax
			// 
			this.labelUniqueValueMax.AutoSize = true;
			this.labelUniqueValueMax.Location = new System.Drawing.Point(8, 24);
			this.labelUniqueValueMax.Name = "labelUniqueValueMax";
			this.labelUniqueValueMax.Size = new System.Drawing.Size(135, 12);
			this.labelUniqueValueMax.TabIndex = 0;
			this.labelUniqueValueMax.Text = "個別値分類の最大分類数";
			// 
			// groupBoxAttributeSearch
			// 
			this.groupBoxAttributeSearch.Controls.Add(this.labelIndividualValueDisplayMax);
			this.groupBoxAttributeSearch.Controls.Add(this.textBoxIndividualValueDisplayMax);
			this.groupBoxAttributeSearch.Location = new System.Drawing.Point(8, 159);
			this.groupBoxAttributeSearch.Name = "groupBoxAttributeSearch";
			this.groupBoxAttributeSearch.Size = new System.Drawing.Size(432, 56);
			this.groupBoxAttributeSearch.TabIndex = 2;
			this.groupBoxAttributeSearch.TabStop = false;
			this.groupBoxAttributeSearch.Text = "属性検索";
			this.groupBoxAttributeSearch.Visible = false;
			// 
			// labelIndividualValueDisplayMax
			// 
			this.labelIndividualValueDisplayMax.AutoSize = true;
			this.labelIndividualValueDisplayMax.Location = new System.Drawing.Point(8, 24);
			this.labelIndividualValueDisplayMax.Name = "labelIndividualValueDisplayMax";
			this.labelIndividualValueDisplayMax.Size = new System.Drawing.Size(103, 12);
			this.labelIndividualValueDisplayMax.TabIndex = 0;
			this.labelIndividualValueDisplayMax.Text = "個別値取得最大数:";
			// 
			// textBoxIndividualValueDisplayMax
			// 
			this.textBoxIndividualValueDisplayMax.Location = new System.Drawing.Point(224, 21);
			this.textBoxIndividualValueDisplayMax.MaxLength = 8;
			this.textBoxIndividualValueDisplayMax.Name = "textBoxIndividualValueDisplayMax";
			this.textBoxIndividualValueDisplayMax.Size = new System.Drawing.Size(200, 19);
			this.textBoxIndividualValueDisplayMax.TabIndex = 5;
			this.textBoxIndividualValueDisplayMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBoxIndividualValueDisplayMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
			// 
			// groupBoxAttributeTable
			// 
			this.groupBoxAttributeTable.Controls.Add(this.textBoxAttributeTableDisplayOIDMax);
			this.groupBoxAttributeTable.Controls.Add(this.labelAttributeTableDisplayOIDMax);
			this.groupBoxAttributeTable.Location = new System.Drawing.Point(8, 16);
			this.groupBoxAttributeTable.Name = "groupBoxAttributeTable";
			this.groupBoxAttributeTable.Size = new System.Drawing.Size(432, 56);
			this.groupBoxAttributeTable.TabIndex = 1;
			this.groupBoxAttributeTable.TabStop = false;
			this.groupBoxAttributeTable.Text = "属性テーブル";
			// 
			// textBoxAttributeTableDisplayOIDMax
			// 
			this.textBoxAttributeTableDisplayOIDMax.Location = new System.Drawing.Point(224, 21);
			this.textBoxAttributeTableDisplayOIDMax.MaxLength = 8;
			this.textBoxAttributeTableDisplayOIDMax.Name = "textBoxAttributeTableDisplayOIDMax";
			this.textBoxAttributeTableDisplayOIDMax.Size = new System.Drawing.Size(200, 19);
			this.textBoxAttributeTableDisplayOIDMax.TabIndex = 4;
			this.textBoxAttributeTableDisplayOIDMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBoxAttributeTableDisplayOIDMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
			// 
			// labelAttributeTableDisplayOIDMax
			// 
			this.labelAttributeTableDisplayOIDMax.AutoSize = true;
			this.labelAttributeTableDisplayOIDMax.Location = new System.Drawing.Point(8, 24);
			this.labelAttributeTableDisplayOIDMax.Name = "labelAttributeTableDisplayOIDMax";
			this.labelAttributeTableDisplayOIDMax.Size = new System.Drawing.Size(176, 12);
			this.labelAttributeTableDisplayOIDMax.TabIndex = 0;
			this.labelAttributeTableDisplayOIDMax.Text = "1ページあたりの表示最大レコード数:";
			// 
			// tabPageFieldName
			// 
			this.tabPageFieldName.Controls.Add(this.checkBoxUseFieldNameAlias);
			this.tabPageFieldName.Location = new System.Drawing.Point(4, 22);
			this.tabPageFieldName.Name = "tabPageFieldName";
			this.tabPageFieldName.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFieldName.Size = new System.Drawing.Size(448, 153);
			this.tabPageFieldName.TabIndex = 3;
			this.tabPageFieldName.Text = "フィールド名";
			this.tabPageFieldName.UseVisualStyleBackColor = true;
			// 
			// checkBoxUseFieldNameAlias
			// 
			this.checkBoxUseFieldNameAlias.AutoSize = true;
			this.checkBoxUseFieldNameAlias.Location = new System.Drawing.Point(8, 16);
			this.checkBoxUseFieldNameAlias.Name = "checkBoxUseFieldNameAlias";
			this.checkBoxUseFieldNameAlias.Size = new System.Drawing.Size(108, 16);
			this.checkBoxUseFieldNameAlias.TabIndex = 0;
			this.checkBoxUseFieldNameAlias.Text = "別名を使用する。";
			this.checkBoxUseFieldNameAlias.UseVisualStyleBackColor = true;
			// 
			// tabPageGeopro
			// 
			this.tabPageGeopro.Controls.Add(this.checkBoxGpBackground);
			this.tabPageGeopro.Location = new System.Drawing.Point(4, 22);
			this.tabPageGeopro.Name = "tabPageGeopro";
			this.tabPageGeopro.Size = new System.Drawing.Size(448, 153);
			this.tabPageGeopro.TabIndex = 4;
			this.tabPageGeopro.Text = "ジオプロセシング";
			this.tabPageGeopro.UseVisualStyleBackColor = true;
			// 
			// checkBoxGpBackground
			// 
			this.checkBoxGpBackground.AutoSize = true;
			this.checkBoxGpBackground.Location = new System.Drawing.Point(9, 14);
			this.checkBoxGpBackground.Name = "checkBoxGpBackground";
			this.checkBoxGpBackground.Size = new System.Drawing.Size(127, 16);
			this.checkBoxGpBackground.TabIndex = 1;
			this.checkBoxGpBackground.Text = "バックグラウンドで処理";
			this.checkBoxGpBackground.UseVisualStyleBackColor = true;
			// 
			// tabPageResetSettings
			// 
			this.tabPageResetSettings.Controls.Add(this.labelResetSettingFile);
			this.tabPageResetSettings.Controls.Add(this.buttonResetSettingFile);
			this.tabPageResetSettings.Controls.Add(this.textBoxSettingFilePath);
			this.tabPageResetSettings.Controls.Add(this.labelSettingsFilePath);
			this.tabPageResetSettings.Location = new System.Drawing.Point(4, 22);
			this.tabPageResetSettings.Name = "tabPageResetSettings";
			this.tabPageResetSettings.Size = new System.Drawing.Size(448, 153);
			this.tabPageResetSettings.TabIndex = 2;
			this.tabPageResetSettings.Text = "設定リセット";
			this.tabPageResetSettings.UseVisualStyleBackColor = true;
			// 
			// labelResetSettingFile
			// 
			this.labelResetSettingFile.AutoSize = true;
			this.labelResetSettingFile.Location = new System.Drawing.Point(8, 72);
			this.labelResetSettingFile.Name = "labelResetSettingFile";
			this.labelResetSettingFile.Size = new System.Drawing.Size(163, 12);
			this.labelResetSettingFile.TabIndex = 0;
			this.labelResetSettingFile.Text = "設定ファイルのリセットを行います。";
			// 
			// buttonResetSettingFile
			// 
			this.buttonResetSettingFile.Location = new System.Drawing.Point(8, 88);
			this.buttonResetSettingFile.Name = "buttonResetSettingFile";
			this.buttonResetSettingFile.Size = new System.Drawing.Size(75, 23);
			this.buttonResetSettingFile.TabIndex = 3;
			this.buttonResetSettingFile.Text = "リセット";
			this.buttonResetSettingFile.UseVisualStyleBackColor = true;
			this.buttonResetSettingFile.Click += new System.EventHandler(this.buttonResetSettingFile_Click);
			// 
			// textBoxSettingFilePath
			// 
			this.textBoxSettingFilePath.Location = new System.Drawing.Point(8, 32);
			this.textBoxSettingFilePath.Name = "textBoxSettingFilePath";
			this.textBoxSettingFilePath.ReadOnly = true;
			this.textBoxSettingFilePath.Size = new System.Drawing.Size(432, 19);
			this.textBoxSettingFilePath.TabIndex = 2;
			// 
			// labelSettingsFilePath
			// 
			this.labelSettingsFilePath.AutoSize = true;
			this.labelSettingsFilePath.Location = new System.Drawing.Point(8, 16);
			this.labelSettingsFilePath.Name = "labelSettingsFilePath";
			this.labelSettingsFilePath.Size = new System.Drawing.Size(63, 12);
			this.labelSettingsFilePath.TabIndex = 0;
			this.labelSettingsFilePath.Text = "設定ファイル";
			// 
			// tabPageFileRelate
			// 
			this.tabPageFileRelate.Controls.Add(this.checkBoxRelateMXD);
			this.tabPageFileRelate.Location = new System.Drawing.Point(4, 22);
			this.tabPageFileRelate.Name = "tabPageFileRelate";
			this.tabPageFileRelate.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFileRelate.Size = new System.Drawing.Size(448, 153);
			this.tabPageFileRelate.TabIndex = 5;
			this.tabPageFileRelate.Text = "ファイルの関連付け";
			this.tabPageFileRelate.UseVisualStyleBackColor = true;
			// 
			// checkBoxRelateMXD
			// 
			this.checkBoxRelateMXD.AutoSize = true;
			this.checkBoxRelateMXD.Location = new System.Drawing.Point(25, 24);
			this.checkBoxRelateMXD.Name = "checkBoxRelateMXD";
			this.checkBoxRelateMXD.Size = new System.Drawing.Size(137, 16);
			this.checkBoxRelateMXD.TabIndex = 0;
			this.checkBoxRelateMXD.Text = "MXDファイルを関連付け";
			this.checkBoxRelateMXD.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(354, 225);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(442, 225);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// FormOptionSettings
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(528, 257);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControlOptionSettings);
			this.Controls.Add(this.labelExplanation);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormOptionSettings";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "オプション設定";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOptionSettings_FormClosed);
			this.tabControlOptionSettings.ResumeLayout(false);
			this.tabPageTemporaryFolder.ResumeLayout(false);
			this.tabPageTemporaryFolder.PerformLayout();
			this.tabPageMaxRecords.ResumeLayout(false);
			this.groupBoxSymbolSettings.ResumeLayout(false);
			this.groupBoxSymbolSettings.PerformLayout();
			this.groupBoxAttributeSearch.ResumeLayout(false);
			this.groupBoxAttributeSearch.PerformLayout();
			this.groupBoxAttributeTable.ResumeLayout(false);
			this.groupBoxAttributeTable.PerformLayout();
			this.tabPageFieldName.ResumeLayout(false);
			this.tabPageFieldName.PerformLayout();
			this.tabPageGeopro.ResumeLayout(false);
			this.tabPageGeopro.PerformLayout();
			this.tabPageResetSettings.ResumeLayout(false);
			this.tabPageResetSettings.PerformLayout();
			this.tabPageFileRelate.ResumeLayout(false);
			this.tabPageFileRelate.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelExplanation;
        private System.Windows.Forms.TabControl tabControlOptionSettings;
        private System.Windows.Forms.TabPage tabPageTemporaryFolder;
        private System.Windows.Forms.TabPage tabPageMaxRecords;
        private System.Windows.Forms.Label labelTemporaryFolderPath;
        private System.Windows.Forms.TextBox textBoxTemporaryFolderPath;
        private System.Windows.Forms.Label labelDeleteTemporaryFile;
        private System.Windows.Forms.Button buttonDeleteTemporaryFile;
        private System.Windows.Forms.GroupBox groupBoxAttributeSearch;
        private System.Windows.Forms.GroupBox groupBoxAttributeTable;
        private System.Windows.Forms.Label labelAttributeTableDisplayOIDMax;
        private System.Windows.Forms.Label labelIndividualValueDisplayMax;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxIndividualValueDisplayMax;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxAttributeTableDisplayOIDMax;
        private System.Windows.Forms.GroupBox groupBoxSymbolSettings;
        private ESRIJapan.GISLight10.Common.NumbersOnlyTextBox textBoxUniqueValueMax;
        private System.Windows.Forms.Label labelUniqueValueMax;
        private System.Windows.Forms.TabPage tabPageResetSettings;
        private System.Windows.Forms.Label labelResetSettingFile;
        private System.Windows.Forms.Button buttonResetSettingFile;
        private System.Windows.Forms.TextBox textBoxSettingFilePath;
        private System.Windows.Forms.Label labelSettingsFilePath;
        private System.Windows.Forms.TabPage tabPageFieldName;
        private System.Windows.Forms.CheckBox checkBoxUseFieldNameAlias;
        private System.Windows.Forms.TabPage tabPageGeopro;
        private System.Windows.Forms.CheckBox checkBoxGpBackground;
		private System.Windows.Forms.TabPage tabPageFileRelate;
		private System.Windows.Forms.CheckBox checkBoxRelateMXD;
    }
}