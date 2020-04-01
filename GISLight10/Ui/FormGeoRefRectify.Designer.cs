namespace ESRIJapan.GISLight10.Ui {
	partial class FormGeoRefRectify {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGeoRefRectify));
			this.label1 = new System.Windows.Forms.Label();
			this.textBox_WS = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox_RasterName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBox_ImageFormat = new System.Windows.Forms.ComboBox();
			this.button_OpenFolder = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.textBox_CellSize = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox_NoDataColor = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBox_ReSamplingType = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.numericUpDown_CompressionQuality = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.comboBox_CompressionType = new System.Windows.Forms.ComboBox();
			this.button_Save = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CompressionQuality)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 94);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(63, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "出力場所 : ";
			// 
			// textBox_WS
			// 
			this.textBox_WS.Location = new System.Drawing.Point(77, 91);
			this.textBox_WS.Name = "textBox_WS";
			this.textBox_WS.ReadOnly = true;
			this.textBox_WS.Size = new System.Drawing.Size(299, 19);
			this.textBox_WS.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 122);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 12);
			this.label2.TabIndex = 9;
			this.label2.Text = "名前 : ";
			// 
			// textBox_RasterName
			// 
			this.textBox_RasterName.Location = new System.Drawing.Point(77, 119);
			this.textBox_RasterName.Name = "textBox_RasterName";
			this.textBox_RasterName.Size = new System.Drawing.Size(152, 19);
			this.textBox_RasterName.TabIndex = 10;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 164);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 12);
			this.label3.TabIndex = 11;
			this.label3.Text = "形式 : ";
			// 
			// comboBox_ImageFormat
			// 
			this.comboBox_ImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ImageFormat.FormattingEnabled = true;
			this.comboBox_ImageFormat.Location = new System.Drawing.Point(77, 161);
			this.comboBox_ImageFormat.Name = "comboBox_ImageFormat";
			this.comboBox_ImageFormat.Size = new System.Drawing.Size(152, 20);
			this.comboBox_ImageFormat.TabIndex = 12;
			this.comboBox_ImageFormat.SelectedIndexChanged += new System.EventHandler(this.ImageFormat_SelectedIndexChanged);
			// 
			// button_OpenFolder
			// 
			this.button_OpenFolder.ImageIndex = 0;
			this.button_OpenFolder.ImageList = this.imageList1;
			this.button_OpenFolder.Location = new System.Drawing.Point(388, 88);
			this.button_OpenFolder.Name = "button_OpenFolder";
			this.button_OpenFolder.Size = new System.Drawing.Size(32, 25);
			this.button_OpenFolder.TabIndex = 8;
			this.button_OpenFolder.UseVisualStyleBackColor = true;
			this.button_OpenFolder.Click += new System.EventHandler(this.button_OpenFolder_Click);
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
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 15);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "セルサイズ : ";
			// 
			// textBox_CellSize
			// 
			this.textBox_CellSize.Location = new System.Drawing.Point(77, 12);
			this.textBox_CellSize.Name = "textBox_CellSize";
			this.textBox_CellSize.Size = new System.Drawing.Size(115, 19);
			this.textBox_CellSize.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(230, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(69, 12);
			this.label5.TabIndex = 2;
			this.label5.Text = "NoData 色 : ";
			// 
			// textBox_NoDataColor
			// 
			this.textBox_NoDataColor.Location = new System.Drawing.Point(302, 12);
			this.textBox_NoDataColor.Name = "textBox_NoDataColor";
			this.textBox_NoDataColor.Size = new System.Drawing.Size(115, 19);
			this.textBox_NoDataColor.TabIndex = 3;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 45);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(105, 12);
			this.label6.TabIndex = 4;
			this.label6.Text = "リサンプリング タイプ : ";
			// 
			// comboBox_ReSamplingType
			// 
			this.comboBox_ReSamplingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ReSamplingType.FormattingEnabled = true;
			this.comboBox_ReSamplingType.Location = new System.Drawing.Point(119, 42);
			this.comboBox_ReSamplingType.Name = "comboBox_ReSamplingType";
			this.comboBox_ReSamplingType.Size = new System.Drawing.Size(258, 20);
			this.comboBox_ReSamplingType.TabIndex = 5;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(252, 164);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(105, 12);
			this.label7.TabIndex = 13;
			this.label7.Text = "圧縮品質（1-100） : ";
			// 
			// numericUpDown_CompressionQuality
			// 
			this.numericUpDown_CompressionQuality.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericUpDown_CompressionQuality.Location = new System.Drawing.Point(364, 162);
			this.numericUpDown_CompressionQuality.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_CompressionQuality.Name = "numericUpDown_CompressionQuality";
			this.numericUpDown_CompressionQuality.Size = new System.Drawing.Size(55, 19);
			this.numericUpDown_CompressionQuality.TabIndex = 14;
			this.numericUpDown_CompressionQuality.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(12, 190);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(65, 12);
			this.label8.TabIndex = 15;
			this.label8.Text = "圧縮タイプ : ";
			// 
			// comboBox_CompressionType
			// 
			this.comboBox_CompressionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_CompressionType.FormattingEnabled = true;
			this.comboBox_CompressionType.Location = new System.Drawing.Point(77, 187);
			this.comboBox_CompressionType.Name = "comboBox_CompressionType";
			this.comboBox_CompressionType.Size = new System.Drawing.Size(152, 20);
			this.comboBox_CompressionType.TabIndex = 16;
			this.comboBox_CompressionType.SelectedIndexChanged += new System.EventHandler(this.CompressionType_SelectedIndexChanged);
			// 
			// button_Save
			// 
			this.button_Save.Location = new System.Drawing.Point(241, 222);
			this.button_Save.Name = "button_Save";
			this.button_Save.Size = new System.Drawing.Size(75, 23);
			this.button_Save.TabIndex = 17;
			this.button_Save.Text = "保存";
			this.button_Save.UseVisualStyleBackColor = true;
			this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(345, 222);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 18;
			this.button_Cancel.Text = "キャンセル";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// FormGeoRefRectify
			// 
			this.AcceptButton = this.button_Save;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(432, 255);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_Save);
			this.Controls.Add(this.numericUpDown_CompressionQuality);
			this.Controls.Add(this.button_OpenFolder);
			this.Controls.Add(this.comboBox_ReSamplingType);
			this.Controls.Add(this.comboBox_CompressionType);
			this.Controls.Add(this.comboBox_ImageFormat);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textBox_RasterName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox_NoDataColor);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBox_CellSize);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBox_WS);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormGeoRefRectify";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "名前を付けて保存";
			this.Load += new System.EventHandler(this.Form_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CompressionQuality)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox_WS;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox_RasterName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBox_ImageFormat;
		private System.Windows.Forms.Button button_OpenFolder;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox_CellSize;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox_NoDataColor;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboBox_ReSamplingType;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDown_CompressionQuality;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBox_CompressionType;
		private System.Windows.Forms.Button button_Save;
		private System.Windows.Forms.Button button_Cancel;
	}
}