namespace ESRIJapan.GISLight10.Ui {
	partial class FormAddQueryLayer {
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
			this.label1 = new System.Windows.Forms.Label();
			this.listView_Fields = new System.Windows.Forms.ListView();
			this.columnHeader0 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(115, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "一意識別子フィールド :";
			// 
			// listView_Fields
			// 
			this.listView_Fields.CheckBoxes = true;
			this.listView_Fields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.listView_Fields.Location = new System.Drawing.Point(15, 29);
			this.listView_Fields.Name = "listView_Fields";
			this.listView_Fields.Size = new System.Drawing.Size(468, 192);
			this.listView_Fields.TabIndex = 1;
			this.listView_Fields.UseCompatibleStateImageBehavior = false;
			this.listView_Fields.View = System.Windows.Forms.View.Details;
			this.listView_Fields.EnabledChanged += new System.EventHandler(this.listView_Fields_EnabledChanged);
			// 
			// columnHeader0
			// 
			this.columnHeader0.Text = "";
			this.columnHeader0.Width = 30;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "名前";
			this.columnHeader1.Width = 220;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "タイプ";
			this.columnHeader2.Width = 110;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Null 値を許可";
			this.columnHeader3.Width = 100;
			// 
			// button_OK
			// 
			this.button_OK.Location = new System.Drawing.Point(316, 231);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 2;
			this.button_OK.Text = "完了";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Location = new System.Drawing.Point(408, 231);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 3;
			this.button_Cancel.Text = "キャンセル";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// FormAddQueryLayer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(498, 262);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.listView_Fields);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAddQueryLayer";
			this.Text = "新規クエリ レイヤ";
			this.Load += new System.EventHandler(this.Form_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView_Fields;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.ColumnHeader columnHeader0;
	}
}