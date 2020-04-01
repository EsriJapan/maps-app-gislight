namespace ESRIJapan.GISLight10.Ui
{
    partial class FormSymbolSettings
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("1000");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("500");
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelPreview = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.checkBoxUseUniqueValueDefaultSymbol = new System.Windows.Forms.CheckBox();
			this.buttonCancelSelect_UniqueValue = new System.Windows.Forms.Button();
			this.buttonRemoveValues = new System.Windows.Forms.Button();
			this.buttonSelectAll_UniqueValue = new System.Windows.Forms.Button();
			this.buttonUpdatePreviewUniqueValue = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBoxFieldName = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listViewUniqueValue = new System.Windows.Forms.ListView();
			this.imageListSymbol = new System.Windows.Forms.ImageList(this.components);
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.buttonCancelSelect_ClassBreaks = new System.Windows.Forms.Button();
			this.buttonSelectAll_ClassBreaks = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxFieldNameClassBreaks = new System.Windows.Forms.ComboBox();
			this.buttonUpdatePreview = new System.Windows.Forms.Button();
			this.listViewClassBreaks = new System.Windows.Forms.ListView();
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.imageListSymbolClassBreaks = new System.Windows.Forms.ImageList(this.components);
			this.label7 = new System.Windows.Forms.Label();
			this.panelSimpleSymbolOutline = new System.Windows.Forms.Panel();
			this.numericUpDownSimpleSymbolOutLineWidth = new System.Windows.Forms.NumericUpDown();
			this.labelSetOutlineColor = new System.Windows.Forms.Label();
			this.buttonSetOutlineColor = new System.Windows.Forms.Button();
			this.labelUpDownSimpleSymbolOutLineWidth = new System.Windows.Forms.Label();
			this.numericUpDownSimpleSymbolSizeOrWidth = new System.Windows.Forms.NumericUpDown();
			this.buttonOpenImageFileOrColorRamp = new System.Windows.Forms.Button();
			this.buttonOpenSyleGallery = new System.Windows.Forms.Button();
			this.labelSetColor = new System.Windows.Forms.Label();
			this.buttonSetColor = new System.Windows.Forms.Button();
			this.labelSimpleSymbolSizeOrWidth = new System.Windows.Forms.Label();
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.panelParameter = new System.Windows.Forms.Panel();
			this.panelBunrui = new System.Windows.Forms.Panel();
			this.comboBoxBunruiSyuhou = new System.Windows.Forms.ComboBox();
			this.numericUpDownBunruiSu = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.groupBoxMain = new System.Windows.Forms.GroupBox();
			this.textBox_LVEdit = new System.Windows.Forms.TextBox();
			this.panel_LVEdit = new System.Windows.Forms.Panel();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.panelSimpleSymbolOutline.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSimpleSymbolOutLineWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSimpleSymbolSizeOrWidth)).BeginInit();
			this.panelParameter.SuspendLayout();
			this.panelBunrui.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownBunruiSu)).BeginInit();
			this.groupBoxMain.SuspendLayout();
			this.panel_LVEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.tabControl1.Location = new System.Drawing.Point(16, 12);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(440, 297);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.tabPage1.Size = new System.Drawing.Size(432, 268);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "単一シンボル";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(268, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "同じシンボルを使用して全てのフィーチャーを描画します。";
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.Add(this.labelPreview);
			this.groupBox1.Location = new System.Drawing.Point(7, 24);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(416, 233);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			// 
			// labelPreview
			// 
			this.labelPreview.BackColor = System.Drawing.SystemColors.Control;
			this.labelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelPreview.Location = new System.Drawing.Point(139, 62);
			this.labelPreview.Name = "labelPreview";
			this.labelPreview.Size = new System.Drawing.Size(147, 125);
			this.labelPreview.TabIndex = 1;
			// 
			// tabPage2
			// 
			this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabPage2.Controls.Add(this.checkBoxUseUniqueValueDefaultSymbol);
			this.tabPage2.Controls.Add(this.buttonCancelSelect_UniqueValue);
			this.tabPage2.Controls.Add(this.buttonRemoveValues);
			this.tabPage2.Controls.Add(this.buttonSelectAll_UniqueValue);
			this.tabPage2.Controls.Add(this.buttonUpdatePreviewUniqueValue);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this.comboBoxFieldName);
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.listViewUniqueValue);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.tabPage2.Size = new System.Drawing.Size(432, 268);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "個別値分類";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// checkBoxUseUniqueValueDefaultSymbol
			// 
			this.checkBoxUseUniqueValueDefaultSymbol.AutoSize = true;
			this.checkBoxUseUniqueValueDefaultSymbol.Checked = true;
			this.checkBoxUseUniqueValueDefaultSymbol.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxUseUniqueValueDefaultSymbol.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.checkBoxUseUniqueValueDefaultSymbol.Location = new System.Drawing.Point(271, 243);
			this.checkBoxUseUniqueValueDefaultSymbol.Name = "checkBoxUseUniqueValueDefaultSymbol";
			this.checkBoxUseUniqueValueDefaultSymbol.Size = new System.Drawing.Size(151, 16);
			this.checkBoxUseUniqueValueDefaultSymbol.TabIndex = 7;
			this.checkBoxUseUniqueValueDefaultSymbol.Text = "[その他の値すべて] を表示";
			this.checkBoxUseUniqueValueDefaultSymbol.UseVisualStyleBackColor = true;
			this.checkBoxUseUniqueValueDefaultSymbol.CheckedChanged += new System.EventHandler(this.checkBoxUseUniqueValueDefaultSymbol_CheckedChanged);
			this.checkBoxUseUniqueValueDefaultSymbol.Click += new System.EventHandler(this.checkBoxUseUniqueValueDefaultSymbol_Click);
			// 
			// buttonCancelSelect_UniqueValue
			// 
			this.buttonCancelSelect_UniqueValue.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.buttonCancelSelect_UniqueValue.Location = new System.Drawing.Point(95, 239);
			this.buttonCancelSelect_UniqueValue.Name = "buttonCancelSelect_UniqueValue";
			this.buttonCancelSelect_UniqueValue.Size = new System.Drawing.Size(75, 23);
			this.buttonCancelSelect_UniqueValue.TabIndex = 5;
			this.buttonCancelSelect_UniqueValue.Text = "選択解除";
			this.buttonCancelSelect_UniqueValue.UseVisualStyleBackColor = true;
			this.buttonCancelSelect_UniqueValue.Click += new System.EventHandler(this.buttonCancelSelect_ListViewValue_Click);
			// 
			// buttonRemoveValues
			// 
			this.buttonRemoveValues.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.buttonRemoveValues.Location = new System.Drawing.Point(183, 239);
			this.buttonRemoveValues.Name = "buttonRemoveValues";
			this.buttonRemoveValues.Size = new System.Drawing.Size(75, 23);
			this.buttonRemoveValues.TabIndex = 6;
			this.buttonRemoveValues.Text = "値の削除";
			this.buttonRemoveValues.UseVisualStyleBackColor = true;
			this.buttonRemoveValues.Click += new System.EventHandler(this.buttonRemoveValues_Click);
			// 
			// buttonSelectAll_UniqueValue
			// 
			this.buttonSelectAll_UniqueValue.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.buttonSelectAll_UniqueValue.Location = new System.Drawing.Point(7, 239);
			this.buttonSelectAll_UniqueValue.Name = "buttonSelectAll_UniqueValue";
			this.buttonSelectAll_UniqueValue.Size = new System.Drawing.Size(75, 23);
			this.buttonSelectAll_UniqueValue.TabIndex = 4;
			this.buttonSelectAll_UniqueValue.Text = "すべて選択";
			this.buttonSelectAll_UniqueValue.UseVisualStyleBackColor = true;
			this.buttonSelectAll_UniqueValue.Click += new System.EventHandler(this.buttonSelectAll_ListViewValue_Click);
			// 
			// buttonUpdatePreviewUniqueValue
			// 
			this.buttonUpdatePreviewUniqueValue.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonUpdatePreviewUniqueValue.Location = new System.Drawing.Point(336, 211);
			this.buttonUpdatePreviewUniqueValue.Name = "buttonUpdatePreviewUniqueValue";
			this.buttonUpdatePreviewUniqueValue.Size = new System.Drawing.Size(85, 23);
			this.buttonUpdatePreviewUniqueValue.TabIndex = 3;
			this.buttonUpdatePreviewUniqueValue.Text = "プレビュー更新";
			this.buttonUpdatePreviewUniqueValue.UseVisualStyleBackColor = true;
			this.buttonUpdatePreviewUniqueValue.Click += new System.EventHandler(this.buttonUpdatePreview_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 216);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(51, 12);
			this.label5.TabIndex = 4;
			this.label5.Text = "フィールド:";
			// 
			// comboBoxFieldName
			// 
			this.comboBoxFieldName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFieldName.FormattingEnabled = true;
			this.comboBoxFieldName.Location = new System.Drawing.Point(66, 213);
			this.comboBoxFieldName.Name = "comboBoxFieldName";
			this.comboBoxFieldName.Size = new System.Drawing.Size(265, 20);
			this.comboBoxFieldName.TabIndex = 2;
			this.comboBoxFieldName.SelectedIndexChanged += new System.EventHandler(this.comboBoxFieldName_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(257, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "1つのフィールドの個別値でカテゴリ分類し描画します。";
			// 
			// listViewUniqueValue
			// 
			this.listViewUniqueValue.FullRowSelect = true;
			this.listViewUniqueValue.GridLines = true;
			this.listViewUniqueValue.HideSelection = false;
			this.listViewUniqueValue.Location = new System.Drawing.Point(7, 30);
			this.listViewUniqueValue.Name = "listViewUniqueValue";
			this.listViewUniqueValue.Size = new System.Drawing.Size(415, 175);
			this.listViewUniqueValue.SmallImageList = this.imageListSymbol;
			this.listViewUniqueValue.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewUniqueValue.TabIndex = 1;
			this.listViewUniqueValue.UseCompatibleStateImageBehavior = false;
			this.listViewUniqueValue.View = System.Windows.Forms.View.Details;
			this.listViewUniqueValue.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewUniqueValue_ItemSelectionChanged);
			this.listViewUniqueValue.SelectedIndexChanged += new System.EventHandler(this.listViewUniqueValue_SelectedIndexChanged);
			// 
			// imageListSymbol
			// 
			this.imageListSymbol.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageListSymbol.ImageSize = new System.Drawing.Size(48, 24);
			this.imageListSymbol.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tabPage3
			// 
			this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabPage3.Controls.Add(this.buttonCancelSelect_ClassBreaks);
			this.tabPage3.Controls.Add(this.buttonSelectAll_ClassBreaks);
			this.tabPage3.Controls.Add(this.label1);
			this.tabPage3.Controls.Add(this.comboBoxFieldNameClassBreaks);
			this.tabPage3.Controls.Add(this.buttonUpdatePreview);
			this.tabPage3.Controls.Add(this.listViewClassBreaks);
			this.tabPage3.Controls.Add(this.label7);
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(432, 268);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "数値分類";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// buttonCancelSelect_ClassBreaks
			// 
			this.buttonCancelSelect_ClassBreaks.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.buttonCancelSelect_ClassBreaks.Location = new System.Drawing.Point(95, 239);
			this.buttonCancelSelect_ClassBreaks.Name = "buttonCancelSelect_ClassBreaks";
			this.buttonCancelSelect_ClassBreaks.Size = new System.Drawing.Size(75, 23);
			this.buttonCancelSelect_ClassBreaks.TabIndex = 5;
			this.buttonCancelSelect_ClassBreaks.Text = "選択解除";
			this.buttonCancelSelect_ClassBreaks.UseVisualStyleBackColor = true;
			this.buttonCancelSelect_ClassBreaks.Click += new System.EventHandler(this.buttonCancelSelect_ListViewValue_Click);
			// 
			// buttonSelectAll_ClassBreaks
			// 
			this.buttonSelectAll_ClassBreaks.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.buttonSelectAll_ClassBreaks.Location = new System.Drawing.Point(7, 239);
			this.buttonSelectAll_ClassBreaks.Name = "buttonSelectAll_ClassBreaks";
			this.buttonSelectAll_ClassBreaks.Size = new System.Drawing.Size(75, 23);
			this.buttonSelectAll_ClassBreaks.TabIndex = 4;
			this.buttonSelectAll_ClassBreaks.Text = "すべて選択";
			this.buttonSelectAll_ClassBreaks.UseVisualStyleBackColor = true;
			this.buttonSelectAll_ClassBreaks.Click += new System.EventHandler(this.buttonSelectAll_ListViewValue_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 216);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "フィールド:";
			// 
			// comboBoxFieldNameClassBreaks
			// 
			this.comboBoxFieldNameClassBreaks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFieldNameClassBreaks.FormattingEnabled = true;
			this.comboBoxFieldNameClassBreaks.Location = new System.Drawing.Point(66, 213);
			this.comboBoxFieldNameClassBreaks.Name = "comboBoxFieldNameClassBreaks";
			this.comboBoxFieldNameClassBreaks.Size = new System.Drawing.Size(265, 20);
			this.comboBoxFieldNameClassBreaks.TabIndex = 2;
			this.comboBoxFieldNameClassBreaks.SelectedIndexChanged += new System.EventHandler(this.comboBoxFieldNameClassBreaks_SelectedIndexChanged);
			// 
			// buttonUpdatePreview
			// 
			this.buttonUpdatePreview.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonUpdatePreview.Location = new System.Drawing.Point(336, 211);
			this.buttonUpdatePreview.Name = "buttonUpdatePreview";
			this.buttonUpdatePreview.Size = new System.Drawing.Size(85, 23);
			this.buttonUpdatePreview.TabIndex = 3;
			this.buttonUpdatePreview.Text = "プレビュー更新";
			this.buttonUpdatePreview.UseVisualStyleBackColor = true;
			this.buttonUpdatePreview.Click += new System.EventHandler(this.buttonUpdatePreview_Click);
			// 
			// listViewClassBreaks
			// 
			this.listViewClassBreaks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
			this.listViewClassBreaks.FullRowSelect = true;
			this.listViewClassBreaks.GridLines = true;
			this.listViewClassBreaks.HideSelection = false;
			listViewItem2.StateImageIndex = 0;
			this.listViewClassBreaks.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
			this.listViewClassBreaks.Location = new System.Drawing.Point(7, 30);
			this.listViewClassBreaks.Name = "listViewClassBreaks";
			this.listViewClassBreaks.Size = new System.Drawing.Size(415, 175);
			this.listViewClassBreaks.SmallImageList = this.imageListSymbolClassBreaks;
			this.listViewClassBreaks.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewClassBreaks.TabIndex = 1;
			this.listViewClassBreaks.UseCompatibleStateImageBehavior = false;
			this.listViewClassBreaks.View = System.Windows.Forms.View.Details;
			this.listViewClassBreaks.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewUniqueValue_ItemSelectionChanged);
			this.listViewClassBreaks.SelectedIndexChanged += new System.EventHandler(this.listViewClassBreaks_SelectedIndexChanged);
			this.listViewClassBreaks.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ListView_KeyUp);
			this.listViewClassBreaks.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListView_MouseUp);
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "シンボル";
			this.columnHeader4.Width = 85;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "値";
			this.columnHeader5.Width = 98;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "ラベル";
			this.columnHeader6.Width = 143;
			// 
			// imageListSymbolClassBreaks
			// 
			this.imageListSymbolClassBreaks.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageListSymbolClassBreaks.ImageSize = new System.Drawing.Size(48, 24);
			this.imageListSymbolClassBreaks.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 8);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(232, 12);
			this.label7.TabIndex = 1;
			this.label7.Text = "1つのフィールドの数値を色で分類し描画します。";
			// 
			// panelSimpleSymbolOutline
			// 
			this.panelSimpleSymbolOutline.Controls.Add(this.numericUpDownSimpleSymbolOutLineWidth);
			this.panelSimpleSymbolOutline.Controls.Add(this.labelSetOutlineColor);
			this.panelSimpleSymbolOutline.Controls.Add(this.buttonSetOutlineColor);
			this.panelSimpleSymbolOutline.Controls.Add(this.labelUpDownSimpleSymbolOutLineWidth);
			this.panelSimpleSymbolOutline.Location = new System.Drawing.Point(216, 8);
			this.panelSimpleSymbolOutline.Name = "panelSimpleSymbolOutline";
			this.panelSimpleSymbolOutline.Size = new System.Drawing.Size(159, 63);
			this.panelSimpleSymbolOutline.TabIndex = 10;
			// 
			// numericUpDownSimpleSymbolOutLineWidth
			// 
			this.numericUpDownSimpleSymbolOutLineWidth.DecimalPlaces = 2;
			this.numericUpDownSimpleSymbolOutLineWidth.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.numericUpDownSimpleSymbolOutLineWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.numericUpDownSimpleSymbolOutLineWidth.Location = new System.Drawing.Point(85, 38);
			this.numericUpDownSimpleSymbolOutLineWidth.Name = "numericUpDownSimpleSymbolOutLineWidth";
			this.numericUpDownSimpleSymbolOutLineWidth.Size = new System.Drawing.Size(56, 19);
			this.numericUpDownSimpleSymbolOutLineWidth.TabIndex = 12;
			this.numericUpDownSimpleSymbolOutLineWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownSimpleSymbolOutLineWidth.ValueChanged += new System.EventHandler(this.numericUpDownSimpleSymbolOutLineWidth_ValueChanged);
			// 
			// labelSetOutlineColor
			// 
			this.labelSetOutlineColor.AutoSize = true;
			this.labelSetOutlineColor.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.labelSetOutlineColor.Location = new System.Drawing.Point(8, 8);
			this.labelSetOutlineColor.Name = "labelSetOutlineColor";
			this.labelSetOutlineColor.Size = new System.Drawing.Size(71, 12);
			this.labelSetOutlineColor.TabIndex = 9;
			this.labelSetOutlineColor.Text = "アウトライン色:";
			// 
			// buttonSetOutlineColor
			// 
			this.buttonSetOutlineColor.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.buttonSetOutlineColor.Location = new System.Drawing.Point(85, 3);
			this.buttonSetOutlineColor.Name = "buttonSetOutlineColor";
			this.buttonSetOutlineColor.Size = new System.Drawing.Size(56, 23);
			this.buttonSetOutlineColor.TabIndex = 11;
			this.buttonSetOutlineColor.UseVisualStyleBackColor = true;
			this.buttonSetOutlineColor.Click += new System.EventHandler(this.buttonSetColor_Click);
			// 
			// labelUpDownSimpleSymbolOutLineWidth
			// 
			this.labelUpDownSimpleSymbolOutLineWidth.AutoSize = true;
			this.labelUpDownSimpleSymbolOutLineWidth.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.labelUpDownSimpleSymbolOutLineWidth.Location = new System.Drawing.Point(8, 40);
			this.labelUpDownSimpleSymbolOutLineWidth.Name = "labelUpDownSimpleSymbolOutLineWidth";
			this.labelUpDownSimpleSymbolOutLineWidth.Size = new System.Drawing.Size(71, 12);
			this.labelUpDownSimpleSymbolOutLineWidth.TabIndex = 4;
			this.labelUpDownSimpleSymbolOutLineWidth.Text = "アウトライン幅:";
			// 
			// numericUpDownSimpleSymbolSizeOrWidth
			// 
			this.numericUpDownSimpleSymbolSizeOrWidth.DecimalPlaces = 2;
			this.numericUpDownSimpleSymbolSizeOrWidth.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.numericUpDownSimpleSymbolSizeOrWidth.Location = new System.Drawing.Point(50, 46);
			this.numericUpDownSimpleSymbolSizeOrWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numericUpDownSimpleSymbolSizeOrWidth.MinimumSize = new System.Drawing.Size(5, 0);
			this.numericUpDownSimpleSymbolSizeOrWidth.Name = "numericUpDownSimpleSymbolSizeOrWidth";
			this.numericUpDownSimpleSymbolSizeOrWidth.Size = new System.Drawing.Size(56, 19);
			this.numericUpDownSimpleSymbolSizeOrWidth.TabIndex = 9;
			this.numericUpDownSimpleSymbolSizeOrWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownSimpleSymbolSizeOrWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownSimpleSymbolSizeOrWidth.ValueChanged += new System.EventHandler(this.numericUpDownSimpleSymbolSizeOrWidth_ValueChanged);
			// 
			// buttonOpenImageFileOrColorRamp
			// 
			this.buttonOpenImageFileOrColorRamp.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonOpenImageFileOrColorRamp.Location = new System.Drawing.Point(96, 120);
			this.buttonOpenImageFileOrColorRamp.Name = "buttonOpenImageFileOrColorRamp";
			this.buttonOpenImageFileOrColorRamp.Size = new System.Drawing.Size(75, 23);
			this.buttonOpenImageFileOrColorRamp.TabIndex = 15;
			this.buttonOpenImageFileOrColorRamp.Text = "画像";
			this.buttonOpenImageFileOrColorRamp.UseVisualStyleBackColor = true;
			this.buttonOpenImageFileOrColorRamp.Click += new System.EventHandler(this.buttonOpenImageFile_Click);
			// 
			// buttonOpenSyleGallery
			// 
			this.buttonOpenSyleGallery.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonOpenSyleGallery.Location = new System.Drawing.Point(8, 120);
			this.buttonOpenSyleGallery.Name = "buttonOpenSyleGallery";
			this.buttonOpenSyleGallery.Size = new System.Drawing.Size(75, 23);
			this.buttonOpenSyleGallery.TabIndex = 14;
			this.buttonOpenSyleGallery.Text = "スタイル";
			this.buttonOpenSyleGallery.UseVisualStyleBackColor = true;
			this.buttonOpenSyleGallery.Click += new System.EventHandler(this.buttonOpenSyleGallery_Click);
			// 
			// labelSetColor
			// 
			this.labelSetColor.AutoSize = true;
			this.labelSetColor.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.labelSetColor.Location = new System.Drawing.Point(8, 16);
			this.labelSetColor.Name = "labelSetColor";
			this.labelSetColor.Size = new System.Drawing.Size(19, 12);
			this.labelSetColor.TabIndex = 8;
			this.labelSetColor.Text = "色:";
			// 
			// buttonSetColor
			// 
			this.buttonSetColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.buttonSetColor.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.buttonSetColor.ForeColor = System.Drawing.SystemColors.ButtonFace;
			this.buttonSetColor.Location = new System.Drawing.Point(33, 12);
			this.buttonSetColor.Name = "buttonSetColor";
			this.buttonSetColor.Size = new System.Drawing.Size(50, 23);
			this.buttonSetColor.TabIndex = 8;
			this.buttonSetColor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.buttonSetColor.UseVisualStyleBackColor = false;
			this.buttonSetColor.Click += new System.EventHandler(this.buttonSetColor_Click);
			// 
			// labelSimpleSymbolSizeOrWidth
			// 
			this.labelSimpleSymbolSizeOrWidth.AutoSize = true;
			this.labelSimpleSymbolSizeOrWidth.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.labelSimpleSymbolSizeOrWidth.Location = new System.Drawing.Point(8, 48);
			this.labelSimpleSymbolSizeOrWidth.Name = "labelSimpleSymbolSizeOrWidth";
			this.labelSimpleSymbolSizeOrWidth.Size = new System.Drawing.Size(36, 12);
			this.labelSimpleSymbolSizeOrWidth.TabIndex = 3;
			this.labelSimpleSymbolSizeOrWidth.Text = "サイズ:";
			// 
			// buttonApply
			// 
			this.buttonApply.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonApply.Location = new System.Drawing.Point(296, 488);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(75, 23);
			this.buttonApply.TabIndex = 17;
			this.buttonApply.Text = "適用";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.buttonOKApply_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonCancel.Location = new System.Drawing.Point(384, 488);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 18;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonOK.Location = new System.Drawing.Point(208, 488);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 16;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOKApply_Click);
			// 
			// panelParameter
			// 
			this.panelParameter.Controls.Add(this.panelBunrui);
			this.panelParameter.Controls.Add(this.buttonSetColor);
			this.panelParameter.Controls.Add(this.panelSimpleSymbolOutline);
			this.panelParameter.Controls.Add(this.numericUpDownSimpleSymbolSizeOrWidth);
			this.panelParameter.Controls.Add(this.buttonOpenImageFileOrColorRamp);
			this.panelParameter.Controls.Add(this.buttonOpenSyleGallery);
			this.panelParameter.Controls.Add(this.labelSetColor);
			this.panelParameter.Controls.Add(this.labelSimpleSymbolSizeOrWidth);
			this.panelParameter.Location = new System.Drawing.Point(8, 312);
			this.panelParameter.Name = "panelParameter";
			this.panelParameter.Size = new System.Drawing.Size(440, 152);
			this.panelParameter.TabIndex = 10;
			// 
			// panelBunrui
			// 
			this.panelBunrui.Controls.Add(this.comboBoxBunruiSyuhou);
			this.panelBunrui.Controls.Add(this.numericUpDownBunruiSu);
			this.panelBunrui.Controls.Add(this.label9);
			this.panelBunrui.Controls.Add(this.label14);
			this.panelBunrui.Location = new System.Drawing.Point(0, 72);
			this.panelBunrui.Name = "panelBunrui";
			this.panelBunrui.Size = new System.Drawing.Size(429, 33);
			this.panelBunrui.TabIndex = 13;
			// 
			// comboBoxBunruiSyuhou
			// 
			this.comboBoxBunruiSyuhou.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBunruiSyuhou.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.comboBoxBunruiSyuhou.FormattingEnabled = true;
			this.comboBoxBunruiSyuhou.Location = new System.Drawing.Point(69, 5);
			this.comboBoxBunruiSyuhou.Name = "comboBoxBunruiSyuhou";
			this.comboBoxBunruiSyuhou.Size = new System.Drawing.Size(110, 20);
			this.comboBoxBunruiSyuhou.TabIndex = 10;
			this.comboBoxBunruiSyuhou.SelectedIndexChanged += new System.EventHandler(this.comboBoxBunruiSyuhou_SelectedIndexChanged);
			// 
			// numericUpDownBunruiSu
			// 
			this.numericUpDownBunruiSu.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.numericUpDownBunruiSu.Location = new System.Drawing.Point(273, 6);
			this.numericUpDownBunruiSu.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.numericUpDownBunruiSu.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownBunruiSu.Name = "numericUpDownBunruiSu";
			this.numericUpDownBunruiSu.Size = new System.Drawing.Size(50, 19);
			this.numericUpDownBunruiSu.TabIndex = 13;
			this.numericUpDownBunruiSu.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownBunruiSu.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownBunruiSu.ValueChanged += new System.EventHandler(this.numericUpDownBunruiSu_ValueChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.label9.Location = new System.Drawing.Point(8, 8);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(55, 12);
			this.label9.TabIndex = 16;
			this.label9.Text = "分類手法:";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.label14.Location = new System.Drawing.Point(224, 8);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(43, 12);
			this.label14.TabIndex = 18;
			this.label14.Text = "分類数:";
			// 
			// groupBoxMain
			// 
			this.groupBoxMain.Controls.Add(this.panelParameter);
			this.groupBoxMain.Location = new System.Drawing.Point(8, 0);
			this.groupBoxMain.Name = "groupBoxMain";
			this.groupBoxMain.Size = new System.Drawing.Size(456, 467);
			this.groupBoxMain.TabIndex = 0;
			this.groupBoxMain.TabStop = false;
			// 
			// textBox_LVEdit
			// 
			this.textBox_LVEdit.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox_LVEdit.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.textBox_LVEdit.Location = new System.Drawing.Point(0, 0);
			this.textBox_LVEdit.Name = "textBox_LVEdit";
			this.textBox_LVEdit.Size = new System.Drawing.Size(100, 14);
			this.textBox_LVEdit.TabIndex = 19;
			this.textBox_LVEdit.WordWrap = false;
			this.textBox_LVEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LVEditBox_KeyUp);
			this.textBox_LVEdit.Leave += new System.EventHandler(this.LVEditBox_Leave);
			// 
			// panel_LVEdit
			// 
			this.panel_LVEdit.BackColor = System.Drawing.Color.White;
			this.panel_LVEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_LVEdit.Controls.Add(this.textBox_LVEdit);
			this.panel_LVEdit.Location = new System.Drawing.Point(32, 470);
			this.panel_LVEdit.Name = "panel_LVEdit";
			this.panel_LVEdit.Size = new System.Drawing.Size(107, 26);
			this.panel_LVEdit.TabIndex = 20;
			this.panel_LVEdit.Visible = false;
			// 
			// FormSymbolSettings
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(474, 522);
			this.Controls.Add(this.panel_LVEdit);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.groupBoxMain);
			this.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSymbolSettings";
			this.ShowIcon = false;
			this.Text = "シンボル設定";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSymbolSettings_FormClosing);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.panelSimpleSymbolOutline.ResumeLayout(false);
			this.panelSimpleSymbolOutline.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSimpleSymbolOutLineWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSimpleSymbolSizeOrWidth)).EndInit();
			this.panelParameter.ResumeLayout(false);
			this.panelParameter.PerformLayout();
			this.panelBunrui.ResumeLayout(false);
			this.panelBunrui.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownBunruiSu)).EndInit();
			this.groupBoxMain.ResumeLayout(false);
			this.panel_LVEdit.ResumeLayout(false);
			this.panel_LVEdit.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelUpDownSimpleSymbolOutLineWidth;
        private System.Windows.Forms.Label labelSimpleSymbolSizeOrWidth;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonSetColor;
        private System.Windows.Forms.Button buttonSetOutlineColor;
        private System.Windows.Forms.ListView listViewUniqueValue;
        private System.Windows.Forms.ImageList imageListSymbol;
        private System.Windows.Forms.Label labelSetColor;
        private System.Windows.Forms.Button buttonOpenImageFileOrColorRamp;
        private System.Windows.Forms.Button buttonOpenSyleGallery;
        private System.Windows.Forms.Label labelSetOutlineColor;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView listViewClassBreaks;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDownSimpleSymbolOutLineWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownSimpleSymbolSizeOrWidth;
        private System.Windows.Forms.Panel panelSimpleSymbolOutline;
        private System.Windows.Forms.Panel panelParameter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxFieldName;
        private System.Windows.Forms.Button buttonUpdatePreview;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDownBunruiSu;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel panelBunrui;
        private System.Windows.Forms.ComboBox comboBoxBunruiSyuhou;
        private System.Windows.Forms.ImageList imageListSymbolClassBreaks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxFieldNameClassBreaks;
        private System.Windows.Forms.GroupBox groupBoxMain;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelPreview;
        private System.Windows.Forms.Button buttonUpdatePreviewUniqueValue;
        private System.Windows.Forms.Button buttonSelectAll_UniqueValue;
        private System.Windows.Forms.Button buttonCancelSelect_UniqueValue;
        private System.Windows.Forms.CheckBox checkBoxUseUniqueValueDefaultSymbol;
        private System.Windows.Forms.Button buttonRemoveValues;
        private System.Windows.Forms.Button buttonCancelSelect_ClassBreaks;
        private System.Windows.Forms.Button buttonSelectAll_ClassBreaks;
		private System.Windows.Forms.TextBox textBox_LVEdit;
		private System.Windows.Forms.Panel panel_LVEdit;
    }
}