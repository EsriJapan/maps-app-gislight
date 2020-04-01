namespace ESRIJapan.GISLight10.Ui {
	partial class FormArcGISServerConnect {
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
			this.radioButtonI = new System.Windows.Forms.RadioButton();
			this.radioButtonL = new System.Windows.Forms.RadioButton();
			this.panelI = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox_InternetURL = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panelL = new System.Windows.Forms.Panel();
			this.textBox_LocalHost = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBox_Psw = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textBox_User = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Test = new System.Windows.Forms.Button();
			this.checkBox_SaveUser = new System.Windows.Forms.CheckBox();
			this.panelI.SuspendLayout();
			this.panelL.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(147, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "ArcGIS サーバ への接続方法";
			// 
			// radioButtonI
			// 
			this.radioButtonI.AutoSize = true;
			this.radioButtonI.Location = new System.Drawing.Point(19, 44);
			this.radioButtonI.Name = "radioButtonI";
			this.radioButtonI.Size = new System.Drawing.Size(84, 16);
			this.radioButtonI.TabIndex = 1;
			this.radioButtonI.TabStop = true;
			this.radioButtonI.Text = "インターネット";
			this.radioButtonI.UseVisualStyleBackColor = true;
			this.radioButtonI.CheckedChanged += new System.EventHandler(this.Server_CheckedChanged);
			// 
			// radioButtonL
			// 
			this.radioButtonL.AutoSize = true;
			this.radioButtonL.Location = new System.Drawing.Point(19, 113);
			this.radioButtonL.Name = "radioButtonL";
			this.radioButtonL.Size = new System.Drawing.Size(61, 16);
			this.radioButtonL.TabIndex = 1;
			this.radioButtonL.TabStop = true;
			this.radioButtonL.Text = "ローカル";
			this.radioButtonL.UseVisualStyleBackColor = true;
			this.radioButtonL.CheckedChanged += new System.EventHandler(this.Server_CheckedChanged);
			// 
			// panelI
			// 
			this.panelI.Controls.Add(this.label3);
			this.panelI.Controls.Add(this.textBox_InternetURL);
			this.panelI.Controls.Add(this.label2);
			this.panelI.Location = new System.Drawing.Point(36, 61);
			this.panelI.Name = "panelI";
			this.panelI.Size = new System.Drawing.Size(413, 47);
			this.panelI.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(90, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(221, 12);
			this.label3.TabIndex = 2;
			this.label3.Text = "http://www.myserver.com/arcgis/services";
			// 
			// textBox_InternetURL
			// 
			this.textBox_InternetURL.Location = new System.Drawing.Point(87, 4);
			this.textBox_InternetURL.Name = "textBox_InternetURL";
			this.textBox_InternetURL.Size = new System.Drawing.Size(323, 19);
			this.textBox_InternetURL.TabIndex = 1;
			this.textBox_InternetURL.Text = "http://";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "サーバの URL :";
			// 
			// panelL
			// 
			this.panelL.Controls.Add(this.textBox_LocalHost);
			this.panelL.Controls.Add(this.label5);
			this.panelL.Location = new System.Drawing.Point(36, 129);
			this.panelL.Name = "panelL";
			this.panelL.Size = new System.Drawing.Size(413, 31);
			this.panelL.TabIndex = 3;
			// 
			// textBox_LocalHost
			// 
			this.textBox_LocalHost.Location = new System.Drawing.Point(87, 4);
			this.textBox_LocalHost.Name = "textBox_LocalHost";
			this.textBox_LocalHost.Size = new System.Drawing.Size(323, 19);
			this.textBox_LocalHost.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(4, 7);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 12);
			this.label5.TabIndex = 0;
			this.label5.Text = "ホスト名 :";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBox_SaveUser);
			this.groupBox1.Controls.Add(this.textBox_Psw);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.textBox_User);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(19, 173);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(430, 97);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "認証（オプション）";
			// 
			// textBox_Psw
			// 
			this.textBox_Psw.Location = new System.Drawing.Point(104, 48);
			this.textBox_Psw.Name = "textBox_Psw";
			this.textBox_Psw.Size = new System.Drawing.Size(314, 19);
			this.textBox_Psw.TabIndex = 3;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(21, 51);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 12);
			this.label6.TabIndex = 2;
			this.label6.Text = "パスワード :";
			// 
			// textBox_User
			// 
			this.textBox_User.Location = new System.Drawing.Point(104, 21);
			this.textBox_User.Name = "textBox_User";
			this.textBox_User.Size = new System.Drawing.Size(314, 19);
			this.textBox_User.TabIndex = 3;
			this.textBox_User.TextChanged += new System.EventHandler(this.textBox_User_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(21, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "ユーザ名 :";
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label7.Location = new System.Drawing.Point(11, 284);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(440, 2);
			this.label7.TabIndex = 5;
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.Location = new System.Drawing.Point(369, 295);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(82, 23);
			this.button_Cancel.TabIndex = 11;
			this.button_Cancel.Text = "キャンセル";
			this.button_Cancel.UseVisualStyleBackColor = true;
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.Location = new System.Drawing.Point(260, 295);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(82, 23);
			this.button_OK.TabIndex = 10;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			// 
			// button_Test
			// 
			this.button_Test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_Test.Location = new System.Drawing.Point(19, 295);
			this.button_Test.Name = "button_Test";
			this.button_Test.Size = new System.Drawing.Size(84, 23);
			this.button_Test.TabIndex = 12;
			this.button_Test.Text = "接続テスト";
			this.button_Test.UseVisualStyleBackColor = true;
			this.button_Test.Click += new System.EventHandler(this.button_Test_Click);
			// 
			// checkBox_SaveUser
			// 
			this.checkBox_SaveUser.AutoSize = true;
			this.checkBox_SaveUser.Location = new System.Drawing.Point(105, 75);
			this.checkBox_SaveUser.Name = "checkBox_SaveUser";
			this.checkBox_SaveUser.Size = new System.Drawing.Size(154, 16);
			this.checkBox_SaveUser.TabIndex = 4;
			this.checkBox_SaveUser.Text = "ユーザ名とパスワードを保存";
			this.checkBox_SaveUser.UseVisualStyleBackColor = true;
			// 
			// FormArcGISServerConnect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(461, 326);
			this.Controls.Add(this.button_Test);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.panelL);
			this.Controls.Add(this.panelI);
			this.Controls.Add(this.radioButtonL);
			this.Controls.Add(this.radioButtonI);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormArcGISServerConnect";
			this.ShowIcon = false;
			this.Text = "ArcGIS サーバー接続の追加";
			this.Load += new System.EventHandler(this.Form_Load);
			this.panelI.ResumeLayout(false);
			this.panelI.PerformLayout();
			this.panelL.ResumeLayout(false);
			this.panelL.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioButtonI;
		private System.Windows.Forms.RadioButton radioButtonL;
		private System.Windows.Forms.Panel panelI;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox_InternetURL;
		private System.Windows.Forms.Panel panelL;
		private System.Windows.Forms.TextBox textBox_LocalHost;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox_User;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox_Psw;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Test;
		private System.Windows.Forms.CheckBox checkBox_SaveUser;
	}
}