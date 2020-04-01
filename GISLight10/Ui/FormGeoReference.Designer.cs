namespace ESRIJapan.GISLight10.Ui
{
    partial class FormGeoReference
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGeoReference));
			this.comboBoxLayers = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.colID = new System.Windows.Forms.ColumnHeader();
			this.colFromX = new System.Windows.Forms.ColumnHeader();
			this.colFromY = new System.Windows.Forms.ColumnHeader();
			this.colToX = new System.Windows.Forms.ColumnHeader();
			this.colToY = new System.Windows.Forms.ColumnHeader();
			this.colSub = new System.Windows.Forms.ColumnHeader();
			this.button_Del = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.imageList2 = new System.Windows.Forms.ImageList(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.axToolbarControl1 = new ESRI.ArcGIS.Controls.AxToolbarControl();
			this.comboBox_Trans = new System.Windows.Forms.ComboBox();
			this.button_SaveTextFile = new System.Windows.Forms.Button();
			this.button_OpenTextFile = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
			this.ToolStripMenuItem_GG = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_GY = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.ToolStripMenuItem_GF = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_GE = new System.Windows.Forms.ToolStripMenuItem();
			this.button_Close = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.button_Expand = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxLayers
			// 
			this.comboBoxLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLayers.Location = new System.Drawing.Point(179, 4);
			this.comboBoxLayers.Name = "comboBoxLayers";
			this.comboBoxLayers.Size = new System.Drawing.Size(170, 20);
			this.comboBoxLayers.TabIndex = 3;
			this.comboBoxLayers.SelectedIndexChanged += new System.EventHandler(this.RasterLayer_SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(140, 7);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(39, 12);
			this.label8.TabIndex = 2;
			this.label8.Text = "レイヤ：";
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colFromX,
            this.colFromY,
            this.colToX,
            this.colToY,
            this.colSub});
			this.listView1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.listView1.FullRowSelect = true;
			this.listView1.LabelWrap = false;
			this.listView1.Location = new System.Drawing.Point(8, 3);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(430, 121);
			this.listView1.TabIndex = 4;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
			this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListView_MouseUp);
			// 
			// colID
			// 
			this.colID.Text = "リンク";
			this.colID.Width = 40;
			// 
			// colFromX
			// 
			this.colFromX.Text = "元 X 座標";
			this.colFromX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.colFromX.Width = 90;
			// 
			// colFromY
			// 
			this.colFromY.Text = "元 Y 座標";
			this.colFromY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.colFromY.Width = 90;
			// 
			// colToX
			// 
			this.colToX.Text = "補正 X 座標";
			this.colToX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.colToX.Width = 90;
			// 
			// colToY
			// 
			this.colToY.Text = "補正 Y 座標";
			this.colToY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.colToY.Width = 90;
			// 
			// colSub
			// 
			this.colSub.Text = "残差";
			this.colSub.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.colSub.Width = 80;
			// 
			// button_Del
			// 
			this.button_Del.ImageIndex = 2;
			this.button_Del.ImageList = this.imageList1;
			this.button_Del.Location = new System.Drawing.Point(8, 126);
			this.button_Del.Name = "button_Del";
			this.button_Del.Size = new System.Drawing.Size(24, 21);
			this.button_Del.TabIndex = 15;
			this.toolTip1.SetToolTip(this.button_Del, "選択されたコントロールポイントを削除します");
			this.button_Del.UseVisualStyleBackColor = true;
			this.button_Del.Click += new System.EventHandler(this.Delete_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "GenericOpen_B_16.png");
			this.imageList1.Images.SetKeyName(1, "GenericSave_B_16.png");
			this.imageList1.Images.SetKeyName(2, "GenericDeleteBlack16.png");
			this.imageList1.Images.SetKeyName(3, "GeoReferencingAddControlPoints16.png");
			this.imageList1.Images.SetKeyName(4, "GeoReferencingScale16.png");
			this.imageList1.Images.SetKeyName(5, "GeoReferencingScaleIn16.png");
			this.imageList1.Images.SetKeyName(6, "GeoReferencingShift16.png");
			this.imageList1.Images.SetKeyName(7, "GeoReferencingToolbar16.png");
			this.imageList1.Images.SetKeyName(8, "GeoReferencingViewLinkTable16.png");
			// 
			// imageList2
			// 
			this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
			this.imageList2.TransparentColor = System.Drawing.Color.Fuchsia;
			this.imageList2.Images.SetKeyName(0, "BuilderDialog_delete.bmp");
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(137, 134);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 12);
			this.label1.TabIndex = 7;
			this.label1.Text = "変換：";
			// 
			// axToolbarControl1
			// 
			this.axToolbarControl1.Location = new System.Drawing.Point(358, 0);
			this.axToolbarControl1.Name = "axToolbarControl1";
			this.axToolbarControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl1.OcxState")));
			this.axToolbarControl1.Size = new System.Drawing.Size(65, 28);
			this.axToolbarControl1.TabIndex = 1;
			// 
			// comboBox_Trans
			// 
			this.comboBox_Trans.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Trans.FormattingEnabled = true;
			this.comboBox_Trans.Location = new System.Drawing.Point(178, 131);
			this.comboBox_Trans.Name = "comboBox_Trans";
			this.comboBox_Trans.Size = new System.Drawing.Size(156, 20);
			this.comboBox_Trans.TabIndex = 8;
			// 
			// button_SaveTextFile
			// 
			this.button_SaveTextFile.ImageIndex = 1;
			this.button_SaveTextFile.ImageList = this.imageList1;
			this.button_SaveTextFile.Location = new System.Drawing.Point(90, 127);
			this.button_SaveTextFile.Name = "button_SaveTextFile";
			this.button_SaveTextFile.Size = new System.Drawing.Size(34, 27);
			this.button_SaveTextFile.TabIndex = 6;
			this.toolTip1.SetToolTip(this.button_SaveTextFile, "設定ファイルの保存");
			this.button_SaveTextFile.UseVisualStyleBackColor = true;
			this.button_SaveTextFile.Click += new System.EventHandler(this.SaveTextFile_Click);
			// 
			// button_OpenTextFile
			// 
			this.button_OpenTextFile.ImageIndex = 0;
			this.button_OpenTextFile.ImageList = this.imageList1;
			this.button_OpenTextFile.Location = new System.Drawing.Point(50, 127);
			this.button_OpenTextFile.Name = "button_OpenTextFile";
			this.button_OpenTextFile.Size = new System.Drawing.Size(34, 27);
			this.button_OpenTextFile.TabIndex = 5;
			this.toolTip1.SetToolTip(this.button_OpenTextFile, "設定ファイルの読み込み");
			this.button_OpenTextFile.UseVisualStyleBackColor = true;
			this.button_OpenTextFile.Click += new System.EventHandler(this.OpenTextFile_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 0);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(137, 24);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripSplitButton1
			// 
			this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_GG,
            this.ToolStripMenuItem_GY,
            this.toolStripSeparator1,
            this.ToolStripMenuItem_GF,
            this.ToolStripMenuItem_GE});
			this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
			this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSplitButton1.Name = "toolStripSplitButton1";
			this.toolStripSplitButton1.Size = new System.Drawing.Size(120, 22);
			this.toolStripSplitButton1.Text = "ジオリファレンス";
			this.toolStripSplitButton1.Click += new System.EventHandler(this.SplitButton_Click);
			// 
			// ToolStripMenuItem_GG
			// 
			this.ToolStripMenuItem_GG.Name = "ToolStripMenuItem_GG";
			this.ToolStripMenuItem_GG.Size = new System.Drawing.Size(208, 22);
			this.ToolStripMenuItem_GG.Tag = "GG";
			this.ToolStripMenuItem_GG.Text = "ワールドファイルの更新";
			this.ToolStripMenuItem_GG.Click += new System.EventHandler(this.ToolStripMenu_Click);
			// 
			// ToolStripMenuItem_GY
			// 
			this.ToolStripMenuItem_GY.Name = "ToolStripMenuItem_GY";
			this.ToolStripMenuItem_GY.Size = new System.Drawing.Size(208, 22);
			this.ToolStripMenuItem_GY.Tag = "GY";
			this.ToolStripMenuItem_GY.Text = "レクティファイ...";
			this.ToolStripMenuItem_GY.Click += new System.EventHandler(this.ToolStripMenu_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
			// 
			// ToolStripMenuItem_GF
			// 
			this.ToolStripMenuItem_GF.Name = "ToolStripMenuItem_GF";
			this.ToolStripMenuItem_GF.Size = new System.Drawing.Size(208, 22);
			this.ToolStripMenuItem_GF.Tag = "GF";
			this.ToolStripMenuItem_GF.Text = "表示範囲にフィット";
			this.ToolStripMenuItem_GF.Click += new System.EventHandler(this.ToolStripMenu_Click);
			// 
			// ToolStripMenuItem_GE
			// 
			this.ToolStripMenuItem_GE.Name = "ToolStripMenuItem_GE";
			this.ToolStripMenuItem_GE.Size = new System.Drawing.Size(208, 22);
			this.ToolStripMenuItem_GE.Tag = "GE";
			this.ToolStripMenuItem_GE.Text = "変換のリセット";
			this.ToolStripMenuItem_GE.Click += new System.EventHandler(this.ToolStripMenu_Click);
			// 
			// button_Close
			// 
			this.button_Close.Location = new System.Drawing.Point(380, 129);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(58, 23);
			this.button_Close.TabIndex = 9;
			this.button_Close.Text = "閉じる";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// button_Expand
			// 
			this.button_Expand.Image = global::ESRIJapan.GISLight10.Properties.Resources.TOC_Expand;
			this.button_Expand.Location = new System.Drawing.Point(426, 2);
			this.button_Expand.Name = "button_Expand";
			this.button_Expand.Size = new System.Drawing.Size(21, 23);
			this.button_Expand.TabIndex = 16;
			this.button_Expand.UseVisualStyleBackColor = true;
			this.button_Expand.Click += new System.EventHandler(this.Expand_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listView1);
			this.panel1.Controls.Add(this.button_Del);
			this.panel1.Controls.Add(this.button_Close);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.comboBox_Trans);
			this.panel1.Controls.Add(this.button_OpenTextFile);
			this.panel1.Controls.Add(this.button_SaveTextFile);
			this.panel1.Location = new System.Drawing.Point(0, 34);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(447, 159);
			this.panel1.TabIndex = 17;
			// 
			// FormGeoReference
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(448, 197);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.button_Expand);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.axToolbarControl1);
			this.Controls.Add(this.comboBoxLayers);
			this.Controls.Add(this.label8);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormGeoReference";
			this.Text = "ジオリファレンス";
			this.Load += new System.EventHandler(this.Form_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Closed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
			((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.ComboBox comboBoxLayers;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader colID;
		private System.Windows.Forms.ColumnHeader colFromX;
		private System.Windows.Forms.ColumnHeader colFromY;
		private System.Windows.Forms.Button button_Del;
		private System.Windows.Forms.ColumnHeader colToX;
		private System.Windows.Forms.ColumnHeader colToY;
		private System.Windows.Forms.Label label1;
		private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl1;
		private System.Windows.Forms.ComboBox comboBox_Trans;
		private System.Windows.Forms.Button button_SaveTextFile;
		private System.Windows.Forms.Button button_OpenTextFile;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_GG;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_GF;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_GY;
		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ImageList imageList2;
		private System.Windows.Forms.ColumnHeader colSub;
		private System.Windows.Forms.Button button_Expand;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_GE;
		private System.Windows.Forms.Panel panel1;


	}
}