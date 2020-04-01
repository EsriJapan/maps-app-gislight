namespace ESRIJapan.GISLight10.Ui
{
    partial class LightTreeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LightTreeView));
            this.imgListIcon = new System.Windows.Forms.ImageList(this.components);
            this.tvWs = new System.Windows.Forms.TreeView();
            this.lblSelectPath = new System.Windows.Forms.Label();
            this.txtSelectPath = new System.Windows.Forms.TextBox();
            this.txtSelType = new System.Windows.Forms.TextBox();
            this.lblSelType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // imgListIcon
            // 
            this.imgListIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListIcon.ImageStream")));
            this.imgListIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListIcon.Images.SetKeyName(0, "FolderWindowsStyle16.png");
            this.imgListIcon.Images.SetKeyName(1, "Geodatabase16.png");
            this.imgListIcon.Images.SetKeyName(2, "GeoprocessingToolbox16.png");
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
            this.imgListIcon.Images.SetKeyName(16, "LayerGroupNew32.png");
            this.imgListIcon.Images.SetKeyName(17, "ShapefileLine16.png");
            this.imgListIcon.Images.SetKeyName(18, "ShapefilePoint16.png");
            this.imgListIcon.Images.SetKeyName(19, "ShapefilePolygon16.png");
            this.imgListIcon.Images.SetKeyName(20, "TableStandalone16.png");
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
            // 
            // tvWs
            // 
            this.tvWs.AllowDrop = true;
            this.tvWs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvWs.Location = new System.Drawing.Point(3, 24);
            this.tvWs.Name = "tvWs";
            this.tvWs.Size = new System.Drawing.Size(490, 352);
            this.tvWs.TabIndex = 1;
            // 
            // lblSelectPath
            // 
            this.lblSelectPath.AutoSize = true;
            this.lblSelectPath.Location = new System.Drawing.Point(3, 6);
            this.lblSelectPath.Name = "lblSelectPath";
            this.lblSelectPath.Size = new System.Drawing.Size(54, 12);
            this.lblSelectPath.TabIndex = 4;
            this.lblSelectPath.Text = "選択パス：";
            // 
            // txtSelectPath
            // 
            this.txtSelectPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelectPath.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtSelectPath.Enabled = false;
            this.txtSelectPath.Location = new System.Drawing.Point(63, 3);
            this.txtSelectPath.Name = "txtSelectPath";
            this.txtSelectPath.Size = new System.Drawing.Size(430, 19);
            this.txtSelectPath.TabIndex = 5;
            // 
            // txtSelType
            // 
            this.txtSelType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelType.BackColor = System.Drawing.SystemColors.Info;
            this.txtSelType.Enabled = false;
            this.txtSelType.Location = new System.Drawing.Point(119, 382);
            this.txtSelType.Name = "txtSelType";
            this.txtSelType.Size = new System.Drawing.Size(375, 19);
            this.txtSelType.TabIndex = 6;
            // 
            // lblSelType
            // 
            this.lblSelType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelType.AutoSize = true;
            this.lblSelType.Location = new System.Drawing.Point(26, 382);
            this.lblSelType.Name = "lblSelType";
            this.lblSelType.Size = new System.Drawing.Size(87, 12);
            this.lblSelType.TabIndex = 7;
            this.lblSelType.Text = "選択データ種類：";
            // 
            // LightTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblSelType);
            this.Controls.Add(this.txtSelType);
            this.Controls.Add(this.txtSelectPath);
            this.Controls.Add(this.lblSelectPath);
            this.Controls.Add(this.tvWs);
            this.Name = "LightTreeView";
            this.Size = new System.Drawing.Size(494, 404);
            this.Load += new System.EventHandler(this.LightTreeView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imgListIcon;
        private System.Windows.Forms.TreeView tvWs;
        private System.Windows.Forms.Label lblSelectPath;
        internal System.Windows.Forms.TextBox txtSelectPath;
        private System.Windows.Forms.TextBox txtSelType;
        private System.Windows.Forms.Label lblSelType;
    }
}
