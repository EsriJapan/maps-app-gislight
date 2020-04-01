namespace ESRIJapan.GISLight10.Ui {
	partial class LayerTreeView {
		/// <summary> 
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region コンポーネント デザイナで生成されたコード

		/// <summary> 
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayerTreeView));
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.imageList_LayerTypes = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(150, 150);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
			this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
			this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
			this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
			this.treeView1.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCheck);
			this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
			this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
			// 
			// imageList_LayerTypes
			// 
			this.imageList_LayerTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_LayerTypes.ImageStream")));
			this.imageList_LayerTypes.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList_LayerTypes.Images.SetKeyName(0, "LayerGroup16.png");
			this.imageList_LayerTypes.Images.SetKeyName(1, "LayerBasemap16.png");
			this.imageList_LayerTypes.Images.SetKeyName(2, "LayerPoint16.png");
			this.imageList_LayerTypes.Images.SetKeyName(3, "LayerLine16.png");
			this.imageList_LayerTypes.Images.SetKeyName(4, "LayerPolygon16.png");
			this.imageList_LayerTypes.Images.SetKeyName(5, "LayerAnnotation16.png");
			this.imageList_LayerTypes.Images.SetKeyName(6, "LayerRaster16.png");
			this.imageList_LayerTypes.Images.SetKeyName(7, "ElementLabel16.png");
			// 
			// LayerTreeView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeView1);
			this.Name = "LayerTreeView";
			this.Load += new System.EventHandler(this.UC_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ImageList imageList_LayerTypes;
	}
}
