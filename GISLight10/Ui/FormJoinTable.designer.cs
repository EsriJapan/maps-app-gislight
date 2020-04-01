﻿namespace ESRIJapan.GISLight10.Ui
{
    partial class FormJoinTable
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboSourceLayers = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxDestinationTable = new System.Windows.Forms.TextBox();
			this.comboSourceKeyField = new System.Windows.Forms.ComboBox();
			this.buttonOpenFile = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboDestinationKeyField = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboBox_DestinationTable = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(429, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "共通のフィールドに基づいて、結合元レイヤの属性テーブルに結合先テーブルを結合します。\r\n結合元と結合先のキー フィールドは、データ型が一致している必要があります。" +
				"";
			// 
			// comboSourceLayers
			// 
			this.comboSourceLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSourceLayers.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.comboSourceLayers.FormattingEnabled = true;
			this.comboSourceLayers.Location = new System.Drawing.Point(8, 40);
			this.comboSourceLayers.Name = "comboSourceLayers";
			this.comboSourceLayers.Size = new System.Drawing.Size(440, 20);
			this.comboSourceLayers.TabIndex = 3;
			this.comboSourceLayers.SelectedIndexChanged += new System.EventHandler(this.comboSourceLayers_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label3.Location = new System.Drawing.Point(8, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 12);
			this.label3.TabIndex = 2;
			this.label3.Text = "レイヤ:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label4.Location = new System.Drawing.Point(8, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(75, 12);
			this.label4.TabIndex = 4;
			this.label4.Text = "キー フィールド:";
			// 
			// textBoxDestinationTable
			// 
			this.textBoxDestinationTable.BackColor = System.Drawing.SystemColors.Control;
			this.textBoxDestinationTable.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.textBoxDestinationTable.Location = new System.Drawing.Point(8, 40);
			this.textBoxDestinationTable.Name = "textBoxDestinationTable";
			this.textBoxDestinationTable.ReadOnly = true;
			this.textBoxDestinationTable.Size = new System.Drawing.Size(400, 19);
			this.textBoxDestinationTable.TabIndex = 8;
			// 
			// comboSourceKeyField
			// 
			this.comboSourceKeyField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSourceKeyField.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.comboSourceKeyField.FormattingEnabled = true;
			this.comboSourceKeyField.Location = new System.Drawing.Point(8, 96);
			this.comboSourceKeyField.Name = "comboSourceKeyField";
			this.comboSourceKeyField.Size = new System.Drawing.Size(440, 20);
			this.comboSourceKeyField.TabIndex = 5;
			// 
			// buttonOpenFile
			// 
			this.buttonOpenFile.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonOpenFile.Location = new System.Drawing.Point(417, 38);
			this.buttonOpenFile.Name = "buttonOpenFile";
			this.buttonOpenFile.Size = new System.Drawing.Size(31, 23);
			this.buttonOpenFile.TabIndex = 9;
			this.buttonOpenFile.Text = "...";
			this.buttonOpenFile.UseVisualStyleBackColor = true;
			this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonOK.Location = new System.Drawing.Point(296, 360);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 12;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonCancel.Location = new System.Drawing.Point(384, 360);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "サポートするすべてのテーブル (*.csv,*.dbf,*.mdb)|*.csv;*.dbf;*.mdb|CSV (*.csv)|*.csv|DBF (*.dbf" +
				")|*.dbf|MDB (*.mdb)|*.mdb";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label5.Location = new System.Drawing.Point(8, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(45, 12);
			this.label5.TabIndex = 7;
			this.label5.Text = "テーブル:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label6.Location = new System.Drawing.Point(8, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(75, 12);
			this.label6.TabIndex = 10;
			this.label6.Text = "キー フィールド:";
			// 
			// comboDestinationKeyField
			// 
			this.comboDestinationKeyField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDestinationKeyField.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.comboDestinationKeyField.FormattingEnabled = true;
			this.comboDestinationKeyField.Location = new System.Drawing.Point(8, 96);
			this.comboDestinationKeyField.Name = "comboDestinationKeyField";
			this.comboDestinationKeyField.Size = new System.Drawing.Size(440, 20);
			this.comboDestinationKeyField.TabIndex = 11;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.comboSourceLayers);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.comboSourceKeyField);
			this.groupBox1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.groupBox1.Location = new System.Drawing.Point(8, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(456, 136);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "結合元";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboBox_DestinationTable);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.comboDestinationKeyField);
			this.groupBox2.Controls.Add(this.textBoxDestinationTable);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.buttonOpenFile);
			this.groupBox2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.groupBox2.Location = new System.Drawing.Point(8, 200);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(456, 136);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "結合先";
			// 
			// comboBox_DestinationTable
			// 
			this.comboBox_DestinationTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_DestinationTable.FormattingEnabled = true;
			this.comboBox_DestinationTable.Location = new System.Drawing.Point(59, 18);
			this.comboBox_DestinationTable.Name = "comboBox_DestinationTable";
			this.comboBox_DestinationTable.Size = new System.Drawing.Size(141, 20);
			this.comboBox_DestinationTable.TabIndex = 13;
			this.comboBox_DestinationTable.SelectedIndexChanged += new System.EventHandler(this.comboBox_DestinationTable_SelectedIndexChanged);
			// 
			// FormJoinTable
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(474, 393);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormJoinTable";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "テーブル結合";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormJoinTable_FormClosed);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboSourceLayers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDestinationTable;
        private System.Windows.Forms.ComboBox comboSourceKeyField;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboDestinationKeyField;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboBox_DestinationTable;
    }
}