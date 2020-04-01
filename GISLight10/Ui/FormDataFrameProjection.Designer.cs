namespace ESRIJapan.GISLight10.Ui
{
    partial class FormDataFrameProjection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDataFrameProjection));
            this.treeViewLayerList = new System.Windows.Forms.TreeView();
            this.imageListIcons = new System.Windows.Forms.ImageList(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtDataFrameProjction = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeViewLayerList
            // 
            this.treeViewLayerList.Location = new System.Drawing.Point(12, 89);
            this.treeViewLayerList.Name = "treeViewLayerList";
            this.treeViewLayerList.Size = new System.Drawing.Size(400, 176);
            this.treeViewLayerList.TabIndex = 0;
            // 
            // imageListIcons
            // 
            this.imageListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcons.ImageStream")));
            this.imageListIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIcons.Images.SetKeyName(0, "CoordinateSystem16.png");
            this.imageListIcons.Images.SetKeyName(1, "LayerPolygon_C_16.png");
            this.imageListIcons.Images.SetKeyName(2, "LayerRaster16.png");
            this.imageListIcons.Images.SetKeyName(3, "LayerGroup16.png");
            this.imageListIcons.Images.SetKeyName(4, "LayerBasemap16.png");
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(230, 299);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(83, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(329, 299);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtDataFrameProjction
            // 
            this.txtDataFrameProjction.BackColor = System.Drawing.SystemColors.Window;
            this.txtDataFrameProjction.Location = new System.Drawing.Point(12, 24);
            this.txtDataFrameProjction.Multiline = true;
            this.txtDataFrameProjction.Name = "txtDataFrameProjction";
            this.txtDataFrameProjction.ReadOnly = true;
            this.txtDataFrameProjction.Size = new System.Drawing.Size(400, 35);
            this.txtDataFrameProjction.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "現在の座標系:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "座標系選択:";
            // 
            // FormDataFrameProjection
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(424, 331);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDataFrameProjction);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.treeViewLayerList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDataFrameProjection";
            this.Text = "座標系の設定";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewLayerList;
        private System.Windows.Forms.ImageList imageListIcons;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtDataFrameProjction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}