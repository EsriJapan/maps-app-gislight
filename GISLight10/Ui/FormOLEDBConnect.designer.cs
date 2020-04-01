namespace ESRIJapan.GISLight10.Ui {
	partial class FormOLEDBConnect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOLEDBConnect));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBox_DBSystem = new System.Windows.Forms.ComboBox();
			this.textBox_Instance = new System.Windows.Forms.TextBox();
			this.comboBox_AuthType = new System.Windows.Forms.ComboBox();
			this.panel_DBAuth = new System.Windows.Forms.Panel();
			this.checkBox_SaveAuth = new System.Windows.Forms.CheckBox();
			this.textBox_DBPsw = new System.Windows.Forms.TextBox();
			this.textBox_DBUser = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBox_DBName = new System.Windows.Forms.ComboBox();
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.panel_DBName = new System.Windows.Forms.Panel();
			this.button_TblList = new System.Windows.Forms.Button();
			this.panel_DBAuth.SuspendLayout();
			this.panel_DBName.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "データベースの種類 :";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "インスタンス :";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 12);
			this.label3.TabIndex = 4;
			this.label3.Text = "認証方法 :";
			// 
			// comboBox_DBSystem
			// 
			this.comboBox_DBSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_DBSystem.FormattingEnabled = true;
			this.comboBox_DBSystem.Location = new System.Drawing.Point(142, 16);
			this.comboBox_DBSystem.Name = "comboBox_DBSystem";
			this.comboBox_DBSystem.Size = new System.Drawing.Size(289, 20);
			this.comboBox_DBSystem.TabIndex = 1;
			this.comboBox_DBSystem.SelectedIndexChanged += new System.EventHandler(this.DBSystem_SelectedIndexChanged);
			// 
			// textBox_Instance
			// 
			this.textBox_Instance.Location = new System.Drawing.Point(142, 51);
			this.textBox_Instance.Name = "textBox_Instance";
			this.textBox_Instance.Size = new System.Drawing.Size(289, 19);
			this.textBox_Instance.TabIndex = 3;
			// 
			// comboBox_AuthType
			// 
			this.comboBox_AuthType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_AuthType.FormattingEnabled = true;
			this.comboBox_AuthType.Location = new System.Drawing.Point(142, 83);
			this.comboBox_AuthType.Name = "comboBox_AuthType";
			this.comboBox_AuthType.Size = new System.Drawing.Size(289, 20);
			this.comboBox_AuthType.TabIndex = 5;
			this.comboBox_AuthType.SelectedIndexChanged += new System.EventHandler(this.DBAuthType_SelectedIndexChanged);
			// 
			// panel_DBAuth
			// 
			this.panel_DBAuth.Controls.Add(this.checkBox_SaveAuth);
			this.panel_DBAuth.Controls.Add(this.textBox_DBPsw);
			this.panel_DBAuth.Controls.Add(this.textBox_DBUser);
			this.panel_DBAuth.Controls.Add(this.label5);
			this.panel_DBAuth.Controls.Add(this.label4);
			this.panel_DBAuth.Location = new System.Drawing.Point(142, 110);
			this.panel_DBAuth.Name = "panel_DBAuth";
			this.panel_DBAuth.Size = new System.Drawing.Size(289, 97);
			this.panel_DBAuth.TabIndex = 6;
			// 
			// checkBox_SaveAuth
			// 
			this.checkBox_SaveAuth.AutoSize = true;
			this.checkBox_SaveAuth.Location = new System.Drawing.Point(80, 69);
			this.checkBox_SaveAuth.Name = "checkBox_SaveAuth";
			this.checkBox_SaveAuth.Size = new System.Drawing.Size(173, 16);
			this.checkBox_SaveAuth.TabIndex = 4;
			this.checkBox_SaveAuth.Text = "ユーザ名とパスワードを保存する";
			this.checkBox_SaveAuth.UseVisualStyleBackColor = true;
			// 
			// textBox_DBPsw
			// 
			this.textBox_DBPsw.Location = new System.Drawing.Point(79, 38);
			this.textBox_DBPsw.Name = "textBox_DBPsw";
			this.textBox_DBPsw.Size = new System.Drawing.Size(207, 19);
			this.textBox_DBPsw.TabIndex = 3;
			// 
			// textBox_DBUser
			// 
			this.textBox_DBUser.Location = new System.Drawing.Point(79, 10);
			this.textBox_DBUser.Name = "textBox_DBUser";
			this.textBox_DBUser.Size = new System.Drawing.Size(207, 19);
			this.textBox_DBUser.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(5, 41);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 12);
			this.label5.TabIndex = 2;
			this.label5.Text = "パスワード :";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(10, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "ユーザ名 :";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(11, 6);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(68, 12);
			this.label6.TabIndex = 0;
			this.label6.Text = "データベース :";
			// 
			// comboBox_DBName
			// 
			this.comboBox_DBName.FormattingEnabled = true;
			this.comboBox_DBName.Location = new System.Drawing.Point(134, 3);
			this.comboBox_DBName.Name = "comboBox_DBName";
			this.comboBox_DBName.Size = new System.Drawing.Size(289, 20);
			this.comboBox_DBName.TabIndex = 1;
			this.comboBox_DBName.Enter += new System.EventHandler(this.DBNames_Enter);
			// 
			// button_OK
			// 
			this.button_OK.Location = new System.Drawing.Point(240, 253);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(82, 23);
			this.button_OK.TabIndex = 8;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Location = new System.Drawing.Point(349, 253);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(82, 23);
			this.button_Cancel.TabIndex = 9;
			this.button_Cancel.Text = "キャンセル";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// panel_DBName
			// 
			this.panel_DBName.Controls.Add(this.comboBox_DBName);
			this.panel_DBName.Controls.Add(this.label6);
			this.panel_DBName.Location = new System.Drawing.Point(5, 213);
			this.panel_DBName.Name = "panel_DBName";
			this.panel_DBName.Size = new System.Drawing.Size(426, 26);
			this.panel_DBName.TabIndex = 7;
			// 
			// button_TblList
			// 
			this.button_TblList.Location = new System.Drawing.Point(19, 253);
			this.button_TblList.Name = "button_TblList";
			this.button_TblList.Size = new System.Drawing.Size(96, 23);
			this.button_TblList.TabIndex = 10;
			this.button_TblList.Text = "テーブルリスト...";
			this.button_TblList.UseVisualStyleBackColor = true;
			this.button_TblList.Click += new System.EventHandler(this.button_TblList_Click);
			// 
			// FormOLEDBConnect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(453, 288);
			this.Controls.Add(this.button_TblList);
			this.Controls.Add(this.panel_DBName);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.panel_DBAuth);
			this.Controls.Add(this.comboBox_AuthType);
			this.Controls.Add(this.textBox_Instance);
			this.Controls.Add(this.comboBox_DBSystem);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormOLEDBConnect";
			this.ShowIcon = false;
			this.Text = "データベース接続";
			this.Load += new System.EventHandler(this.Form_Load);
			this.panel_DBAuth.ResumeLayout(false);
			this.panel_DBAuth.PerformLayout();
			this.panel_DBName.ResumeLayout(false);
			this.panel_DBName.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBox_DBSystem;
		private System.Windows.Forms.TextBox textBox_Instance;
		private System.Windows.Forms.ComboBox comboBox_AuthType;
		private System.Windows.Forms.Panel panel_DBAuth;
		private System.Windows.Forms.TextBox textBox_DBPsw;
		private System.Windows.Forms.TextBox textBox_DBUser;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboBox_DBName;
		private System.Windows.Forms.CheckBox checkBox_SaveAuth;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Panel panel_DBName;
		private System.Windows.Forms.Button button_TblList;
	}
}