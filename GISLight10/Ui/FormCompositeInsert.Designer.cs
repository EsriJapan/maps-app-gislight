namespace ESRIJapan.GISLight10.Ui {
	partial class FormCompositeInsert {
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
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Close = new System.Windows.Forms.Button();
			this.label_Desc1 = new System.Windows.Forms.Label();
			this.label_Desc2 = new System.Windows.Forms.Label();
			this.linkLabel_Option = new System.Windows.Forms.LinkLabel();
			this.panel_Option = new System.Windows.Forms.Panel();
			this.linkLabel_OK = new System.Windows.Forms.LinkLabel();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.layerTreeView1 = new ESRIJapan.GISLight10.Ui.LayerTreeView();
			this.panel_Option.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.Location = new System.Drawing.Point(176, 396);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 4;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.Location = new System.Drawing.Point(257, 396);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 5;
			this.button_Close.Text = "閉じる";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// label_Desc1
			// 
			this.label_Desc1.AutoSize = true;
			this.label_Desc1.Location = new System.Drawing.Point(12, 13);
			this.label_Desc1.Name = "label_Desc1";
			this.label_Desc1.Size = new System.Drawing.Size(219, 12);
			this.label_Desc1.TabIndex = 0;
			this.label_Desc1.Text = "［＠］に含めるレイヤにチェックを入れてください。";
			// 
			// label_Desc2
			// 
			this.label_Desc2.AutoSize = true;
			this.label_Desc2.Location = new System.Drawing.Point(14, 32);
			this.label_Desc2.Name = "label_Desc2";
			this.label_Desc2.Size = new System.Drawing.Size(261, 12);
			this.label_Desc2.TabIndex = 1;
			this.label_Desc2.Text = "（グループから除外する場合はチェックを外してください。）";
			// 
			// linkLabel_Option
			// 
			this.linkLabel_Option.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel_Option.AutoSize = true;
			this.linkLabel_Option.Location = new System.Drawing.Point(14, 403);
			this.linkLabel_Option.Name = "linkLabel_Option";
			this.linkLabel_Option.Size = new System.Drawing.Size(87, 12);
			this.linkLabel_Option.TabIndex = 3;
			this.linkLabel_Option.TabStop = true;
			this.linkLabel_Option.Text = "指定方式の設定";
			this.linkLabel_Option.Visible = false;
			this.linkLabel_Option.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Option_LinkClicked);
			// 
			// panel_Option
			// 
			this.panel_Option.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_Option.Controls.Add(this.linkLabel_OK);
			this.panel_Option.Controls.Add(this.radioButton2);
			this.panel_Option.Controls.Add(this.radioButton1);
			this.panel_Option.Location = new System.Drawing.Point(6, 336);
			this.panel_Option.Name = "panel_Option";
			this.panel_Option.Size = new System.Drawing.Size(225, 58);
			this.panel_Option.TabIndex = 6;
			// 
			// linkLabel_OK
			// 
			this.linkLabel_OK.AutoSize = true;
			this.linkLabel_OK.Location = new System.Drawing.Point(195, 35);
			this.linkLabel_OK.Name = "linkLabel_OK";
			this.linkLabel_OK.Size = new System.Drawing.Size(20, 12);
			this.linkLabel_OK.TabIndex = 2;
			this.linkLabel_OK.TabStop = true;
			this.linkLabel_OK.Text = "OK";
			this.linkLabel_OK.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_OK_LinkClicked);
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(12, 32);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(126, 16);
			this.radioButton2.TabIndex = 1;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "ドラッグ＆ドロップ方式";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(12, 7);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(78, 16);
			this.radioButton1.TabIndex = 0;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "チェック方式";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// layerTreeView1
			// 
			this.layerTreeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.layerTreeView1.CanDragDrop = false;
			this.layerTreeView1.Location = new System.Drawing.Point(12, 50);
			this.layerTreeView1.Name = "layerTreeView1";
			this.layerTreeView1.Size = new System.Drawing.Size(320, 340);
			this.layerTreeView1.TabIndex = 2;
			this.layerTreeView1.TargetLayer = null;
			this.layerTreeView1.TargetMap = null;
			// 
			// FormCompositeInsert
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 427);
			this.Controls.Add(this.panel_Option);
			this.Controls.Add(this.linkLabel_Option);
			this.Controls.Add(this.label_Desc2);
			this.Controls.Add(this.label_Desc1);
			this.Controls.Add(this.button_Close);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.layerTreeView1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(260, 310);
			this.Name = "FormCompositeInsert";
			this.ShowIcon = false;
			this.Text = "グループレイヤの構成";
			this.Load += new System.EventHandler(this.Form_Load);
			this.panel_Option.ResumeLayout(false);
			this.panel_Option.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private LayerTreeView layerTreeView1;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.Label label_Desc1;
		private System.Windows.Forms.Label label_Desc2;
		private System.Windows.Forms.LinkLabel linkLabel_Option;
		private System.Windows.Forms.Panel panel_Option;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.LinkLabel linkLabel_OK;
	}
}