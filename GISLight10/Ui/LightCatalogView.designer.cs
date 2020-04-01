namespace ESRIJapan.GISLight10.Ui
{
    partial class LightCatalogView
    {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LightCatalogView));
			this.imgListIcon = new System.Windows.Forms.ImageList(this.components);
			this.tvWs = new System.Windows.Forms.TreeView();
			this.lblSelectPath = new System.Windows.Forms.Label();
			this.txtSelectPath = new System.Windows.Forms.TextBox();
			this.txtSelType = new System.Windows.Forms.TextBox();
			this.lblSelType = new System.Windows.Forms.Label();
			this.panel_Place = new System.Windows.Forms.Panel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton_FC = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_FD = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_FR = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_DC = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_DD = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_SC = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_SD = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_RN = new System.Windows.Forms.ToolStripButton();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.panel_Place.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// imgListIcon
			// 
			this.imgListIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListIcon.ImageStream")));
			this.imgListIcon.TransparentColor = System.Drawing.Color.White;
			this.imgListIcon.Images.SetKeyName(0, "FolderWindowsStyle16.png");
			this.imgListIcon.Images.SetKeyName(1, "Geodatabase16.png");
			this.imgListIcon.Images.SetKeyName(2, "GeoprocessingArcToolboxWindowShow16.png");
			this.imgListIcon.Images.SetKeyName(3, "ArcMap_MXD_File16.png");
			this.imgListIcon.Images.SetKeyName(4, "Folder16.png");
			this.imgListIcon.Images.SetKeyName(5, "FolderOpenState16.png");
			this.imgListIcon.Images.SetKeyName(6, "GeodatabaseFeatureClassAnnotation16.png");
			this.imgListIcon.Images.SetKeyName(7, "GeodatabaseFeatureClassLine16.png");
			this.imgListIcon.Images.SetKeyName(8, "GeodatabaseFeatureClassMultipoint16.png");
			this.imgListIcon.Images.SetKeyName(9, "GeodatabaseFeatureClassPoint16.png");
			this.imgListIcon.Images.SetKeyName(10, "GeodatabaseFeatureClassPolygon16.png");
			this.imgListIcon.Images.SetKeyName(11, "GeodatabaseFeatureDataset16.png");
			this.imgListIcon.Images.SetKeyName(12, "GeodatabaseMosaicDataset16.png");
			this.imgListIcon.Images.SetKeyName(13, "GeodatabaseNetworkDataset16.png");
			this.imgListIcon.Images.SetKeyName(14, "GeodatabaseRasterCatalog16.png");
			this.imgListIcon.Images.SetKeyName(15, "GeodatabaseTable16.png");
			this.imgListIcon.Images.SetKeyName(16, "LayerGroupNew16.png");
			this.imgListIcon.Images.SetKeyName(17, "ShapefileLine16.png");
			this.imgListIcon.Images.SetKeyName(18, "ShapefilePoint16.png");
			this.imgListIcon.Images.SetKeyName(19, "ShapefilePolygon16.png");
			this.imgListIcon.Images.SetKeyName(20, "TableStandaloneSmall16.png");
			this.imgListIcon.Images.SetKeyName(21, "TableExcel16.png");
			this.imgListIcon.Images.SetKeyName(22, "GeodatabaseRaster16.png");
			this.imgListIcon.Images.SetKeyName(23, "GeodatabaseRasterGrid16.png");
			this.imgListIcon.Images.SetKeyName(24, "GeodatabaseRelationship16.png");
			this.imgListIcon.Images.SetKeyName(25, "GeodatabaseRasterGridBand16.png");
			this.imgListIcon.Images.SetKeyName(26, "GeodatabaseNetworkGeometric16.png");
			this.imgListIcon.Images.SetKeyName(27, "GeodatabaseTerrain16.png");
			this.imgListIcon.Images.SetKeyName(28, "GeodatabaseTopology16.png");
			this.imgListIcon.Images.SetKeyName(29, "GeocodeAddressLocator16.png");
			this.imgListIcon.Images.SetKeyName(30, "CADDataset16.png");
			this.imgListIcon.Images.SetKeyName(31, "FileMosaicDataset16.png");
			this.imgListIcon.Images.SetKeyName(32, "FileNetworkDataset16.png");
			this.imgListIcon.Images.SetKeyName(33, "FileRasterGrid16.png");
			this.imgListIcon.Images.SetKeyName(34, "FileRasterGridBand16.png");
			this.imgListIcon.Images.SetKeyName(35, "GeodatabaseFeatureClassMultipatch16.png");
			this.imgListIcon.Images.SetKeyName(36, "Layer_LYR_File16.png");
			this.imgListIcon.Images.SetKeyName(37, "CoordinateSystem16.png");
			this.imgListIcon.Images.SetKeyName(38, "TableDBase16.png");
			this.imgListIcon.Images.SetKeyName(39, "Text_File16.png");
			this.imgListIcon.Images.SetKeyName(40, "FolderConnectionAdd16.png");
			this.imgListIcon.Images.SetKeyName(41, "FolderConnectionsFolder16.png");
			this.imgListIcon.Images.SetKeyName(42, "FolderConnectionsFolderOpenState16.png");
			this.imgListIcon.Images.SetKeyName(43, "FolderConnection16.png");
			this.imgListIcon.Images.SetKeyName(44, "FolderConnectionOpenState16.png");
			this.imgListIcon.Images.SetKeyName(45, "FolderConnectionDisconnectFrom16.png");
			this.imgListIcon.Images.SetKeyName(46, "CoverageFeatureClassAnnotation16.png");
			this.imgListIcon.Images.SetKeyName(47, "GenericRefresh_B_16.png");
			this.imgListIcon.Images.SetKeyName(48, "GeodatabaseConnectionsFolder16.png");
			this.imgListIcon.Images.SetKeyName(49, "GeodatabaseConnectionAdd16.png");
			this.imgListIcon.Images.SetKeyName(50, "GeodatabaseConnectionBroken16.png");
			this.imgListIcon.Images.SetKeyName(51, "GeodatabaseConnection16.png");
			this.imgListIcon.Images.SetKeyName(52, "ContentsWindowListBySource16.png");
			this.imgListIcon.Images.SetKeyName(53, "ServersFolder16.png");
			this.imgListIcon.Images.SetKeyName(54, "ServerArcGISAdd16.png");
			this.imgListIcon.Images.SetKeyName(55, "ServerArcGISBroken16.png");
			this.imgListIcon.Images.SetKeyName(56, "ServerArcGIS16.png");
			this.imgListIcon.Images.SetKeyName(57, "ServerArcGISLocalBroken16.png");
			this.imgListIcon.Images.SetKeyName(58, "ServerArcGISLocal16.png");
			this.imgListIcon.Images.SetKeyName(59, "ShapefileEmpty16.png");
			// 
			// tvWs
			// 
			this.tvWs.AllowDrop = true;
			this.tvWs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvWs.Location = new System.Drawing.Point(4, 56);
			this.tvWs.Name = "tvWs";
			this.tvWs.Size = new System.Drawing.Size(236, 200);
			this.tvWs.TabIndex = 0;
			this.tvWs.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeNode_AfterLabelEdit);
			this.tvWs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TreeNode_MouseDoubleClick);
			this.tvWs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeNode_MouseDown);
			// 
			// lblSelectPath
			// 
			this.lblSelectPath.AutoSize = true;
			this.lblSelectPath.Location = new System.Drawing.Point(1, 6);
			this.lblSelectPath.Name = "lblSelectPath";
			this.lblSelectPath.Size = new System.Drawing.Size(35, 12);
			this.lblSelectPath.TabIndex = 0;
			this.lblSelectPath.Text = "場所：";
			// 
			// txtSelectPath
			// 
			this.txtSelectPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSelectPath.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.txtSelectPath.Location = new System.Drawing.Point(42, 3);
			this.txtSelectPath.Name = "txtSelectPath";
			this.txtSelectPath.ReadOnly = true;
			this.txtSelectPath.Size = new System.Drawing.Size(198, 19);
			this.txtSelectPath.TabIndex = 1;
			// 
			// txtSelType
			// 
			this.txtSelType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSelType.BackColor = System.Drawing.SystemColors.Info;
			this.txtSelType.Enabled = false;
			this.txtSelType.Location = new System.Drawing.Point(97, 261);
			this.txtSelType.Name = "txtSelType";
			this.txtSelType.Size = new System.Drawing.Size(143, 19);
			this.txtSelType.TabIndex = 3;
			// 
			// lblSelType
			// 
			this.lblSelType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSelType.AutoSize = true;
			this.lblSelType.Location = new System.Drawing.Point(4, 264);
			this.lblSelType.Name = "lblSelType";
			this.lblSelType.Size = new System.Drawing.Size(87, 12);
			this.lblSelType.TabIndex = 2;
			this.lblSelType.Text = "選択データ種類：";
			// 
			// panel_Place
			// 
			this.panel_Place.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel_Place.Controls.Add(this.txtSelectPath);
			this.panel_Place.Controls.Add(this.lblSelectPath);
			this.panel_Place.Location = new System.Drawing.Point(0, 28);
			this.panel_Place.Name = "panel_Place";
			this.panel_Place.Size = new System.Drawing.Size(242, 25);
			this.panel_Place.TabIndex = 1;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_FC,
            this.toolStripButton_FD,
            this.toolStripButton_FR,
            this.toolStripSeparator1,
            this.toolStripButton_DC,
            this.toolStripButton_DD,
            this.toolStripSeparator2,
            this.toolStripButton_SC,
            this.toolStripButton_SD,
            this.toolStripSeparator3,
            this.toolStripButton_RN});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(240, 25);
			this.toolStrip1.Stretch = true;
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton_FC
			// 
			this.toolStripButton_FC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_FC.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_FC.Image")));
			this.toolStripButton_FC.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_FC.Name = "toolStripButton_FC";
			this.toolStripButton_FC.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_FC.Tag = "FC";
			this.toolStripButton_FC.Text = "フォルダに接続";
			this.toolStripButton_FC.Click += new System.EventHandler(this.Tool_Click);
			// 
			// toolStripButton_FD
			// 
			this.toolStripButton_FD.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_FD.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_FD.Image")));
			this.toolStripButton_FD.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_FD.Name = "toolStripButton_FD";
			this.toolStripButton_FD.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_FD.Tag = "FD";
			this.toolStripButton_FD.Text = "フォルダの切断";
			this.toolStripButton_FD.Click += new System.EventHandler(this.Tool_Click);
			// 
			// toolStripButton_FR
			// 
			this.toolStripButton_FR.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_FR.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_FR.Image")));
			this.toolStripButton_FR.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_FR.Name = "toolStripButton_FR";
			this.toolStripButton_FR.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_FR.Tag = "FR";
			this.toolStripButton_FR.Text = "フォルダ接続の状態を最新の情報に更新";
			this.toolStripButton_FR.Click += new System.EventHandler(this.Tool_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			this.toolStripSeparator1.Tag = "D";
			// 
			// toolStripButton_DC
			// 
			this.toolStripButton_DC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_DC.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_DC.Image")));
			this.toolStripButton_DC.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_DC.Name = "toolStripButton_DC";
			this.toolStripButton_DC.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_DC.Tag = "DC";
			this.toolStripButton_DC.Text = "データベース接続を追加します";
			this.toolStripButton_DC.Click += new System.EventHandler(this.Tool_Click);
			// 
			// toolStripButton_DD
			// 
			this.toolStripButton_DD.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_DD.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_DD.Image")));
			this.toolStripButton_DD.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_DD.Name = "toolStripButton_DD";
			this.toolStripButton_DD.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_DD.Tag = "DD";
			this.toolStripButton_DD.Text = "データベース接続を削除します";
			this.toolStripButton_DD.ToolTipText = "データベース接続を削除します";
			this.toolStripButton_DD.Click += new System.EventHandler(this.Tool_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			this.toolStripSeparator2.Tag = "S";
			// 
			// toolStripButton_SC
			// 
			this.toolStripButton_SC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_SC.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SC.Image")));
			this.toolStripButton_SC.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_SC.Name = "toolStripButton_SC";
			this.toolStripButton_SC.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_SC.Tag = "SC";
			this.toolStripButton_SC.Text = "ArcGIS サーバ接続を追加します";
			this.toolStripButton_SC.Click += new System.EventHandler(this.Tool_Click);
			// 
			// toolStripButton_SD
			// 
			this.toolStripButton_SD.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_SD.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SD.Image")));
			this.toolStripButton_SD.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_SD.Name = "toolStripButton_SD";
			this.toolStripButton_SD.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_SD.Tag = "SD";
			this.toolStripButton_SD.Text = "ArcGIS サーバ接続を削除します";
			this.toolStripButton_SD.Click += new System.EventHandler(this.Tool_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_RN
			// 
			this.toolStripButton_RN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_RN.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_RN.Image")));
			this.toolStripButton_RN.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_RN.Name = "toolStripButton_RN";
			this.toolStripButton_RN.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_RN.Tag = "RN";
			this.toolStripButton_RN.Text = "表示名の変更";
			this.toolStripButton_RN.Click += new System.EventHandler(this.Tool_Click);
			// 
			// LightCatalogView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.panel_Place);
			this.Controls.Add(this.lblSelType);
			this.Controls.Add(this.txtSelType);
			this.Controls.Add(this.tvWs);
			this.Name = "LightCatalogView";
			this.Size = new System.Drawing.Size(242, 283);
			this.Load += new System.EventHandler(this.LightCatalogView_Load);
			this.panel_Place.ResumeLayout(false);
			this.panel_Place.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TreeView tvWs;
        private System.Windows.Forms.Label lblSelectPath;
        internal System.Windows.Forms.TextBox txtSelectPath;
        private System.Windows.Forms.TextBox txtSelType;
        private System.Windows.Forms.Label lblSelType;
		private System.Windows.Forms.Panel panel_Place;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton_FC;
		private System.Windows.Forms.ToolStripButton toolStripButton_FD;
		private System.Windows.Forms.ToolStripButton toolStripButton_RN;
		private System.Windows.Forms.ToolStripButton toolStripButton_FR;
		protected internal System.Windows.Forms.ImageList imgListIcon;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButton_DC;
		private System.Windows.Forms.ToolStripButton toolStripButton_DD;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton toolStripButton_SC;
		private System.Windows.Forms.ToolStripButton toolStripButton_SD;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolTip toolTip1;
    }
}
