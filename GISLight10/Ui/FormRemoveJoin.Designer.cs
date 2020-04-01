namespace ESRIJapan.GISLight10.Ui
{
    partial class FormRemoveJoin
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
            this.label2 = new System.Windows.Forms.Label();
            this.comboSourceLayers = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboDestinationTables = new System.Windows.Forms.ComboBox();
            this.checkBoxAllRemove = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "テーブル結合を解除します。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "結合元レイヤ:";
            // 
            // comboSourceLayers
            // 
            this.comboSourceLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSourceLayers.FormattingEnabled = true;
            this.comboSourceLayers.Location = new System.Drawing.Point(8, 56);
            this.comboSourceLayers.Name = "comboSourceLayers";
            this.comboSourceLayers.Size = new System.Drawing.Size(456, 20);
            this.comboSourceLayers.TabIndex = 3;
            this.comboSourceLayers.SelectedIndexChanged += new System.EventHandler(this.comboSourceLayers_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "結合先テーブル:";
            // 
            // comboDestinationTables
            // 
            this.comboDestinationTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDestinationTables.FormattingEnabled = true;
            this.comboDestinationTables.Location = new System.Drawing.Point(8, 112);
            this.comboDestinationTables.Name = "comboDestinationTables";
            this.comboDestinationTables.Size = new System.Drawing.Size(456, 20);
            this.comboDestinationTables.TabIndex = 5;
            // 
            // checkBoxAllRemove
            // 
            this.checkBoxAllRemove.AutoSize = true;
            this.checkBoxAllRemove.Location = new System.Drawing.Point(8, 152);
            this.checkBoxAllRemove.Name = "checkBoxAllRemove";
            this.checkBoxAllRemove.Size = new System.Drawing.Size(249, 16);
            this.checkBoxAllRemove.TabIndex = 6;
            this.checkBoxAllRemove.Text = "結合元レイヤのテーブル結合をすべて解除する。";
            this.checkBoxAllRemove.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(296, 192);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(384, 192);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormRemoveJoin
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(474, 225);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxAllRemove);
            this.Controls.Add(this.comboDestinationTables);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboSourceLayers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRemoveJoin";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "テーブル結合の解除";            
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRemoveJoin_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboSourceLayers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboDestinationTables;
        private System.Windows.Forms.CheckBox checkBoxAllRemove;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}