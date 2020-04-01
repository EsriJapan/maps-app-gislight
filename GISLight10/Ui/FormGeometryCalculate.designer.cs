namespace ESRIJapan.GISLight10.Ui
{
    partial class FormGeometryCalculate
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
            this.lblLayer = new System.Windows.Forms.Label();
            this.comboLayer = new System.Windows.Forms.ComboBox();
            this.lblErrorLayer = new System.Windows.Forms.Label();
            this.gboxSr = new System.Windows.Forms.GroupBox();
            this.lblErrorSr = new System.Windows.Forms.Label();
            this.txtSr = new System.Windows.Forms.TextBox();
            this.lblProp = new System.Windows.Forms.Label();
            this.lblField = new System.Windows.Forms.Label();
            this.lblUnit = new System.Windows.Forms.Label();
            this.comboProperty = new System.Windows.Forms.ComboBox();
            this.comboField = new System.Windows.Forms.ComboBox();
            this.comboUnit = new System.Windows.Forms.ComboBox();
            this.chkPrecision = new System.Windows.Forms.CheckBox();
            this.chkAppendUnit = new System.Windows.Forms.CheckBox();
            this.chkUseSelectionSet = new System.Windows.Forms.CheckBox();
            this.nudDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gboxSr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimalPlaces)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "指定したフィールドに対して、面積・長さ計算を行います。";
            // 
            // lblLayer
            // 
            this.lblLayer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(8, 39);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(35, 12);
            this.lblLayer.TabIndex = 1;
            this.lblLayer.Text = "レイヤ:";
            // 
            // comboLayer
            // 
            this.comboLayer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLayer.FormattingEnabled = true;
            this.comboLayer.Location = new System.Drawing.Point(72, 36);
            this.comboLayer.Name = "comboLayer";
            this.comboLayer.Size = new System.Drawing.Size(390, 20);
            this.comboLayer.TabIndex = 2;
            this.comboLayer.SelectedIndexChanged += new System.EventHandler(this.comboLayer_SelectedIndexChanged);
            // 
            // lblErrorLayer
            // 
            this.lblErrorLayer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblErrorLayer.AutoSize = true;
            this.lblErrorLayer.ForeColor = System.Drawing.Color.DarkRed;
            this.lblErrorLayer.Location = new System.Drawing.Point(71, 69);
            this.lblErrorLayer.Name = "lblErrorLayer";
            this.lblErrorLayer.Size = new System.Drawing.Size(0, 12);
            this.lblErrorLayer.TabIndex = 3;
            // 
            // gboxSr
            // 
            this.gboxSr.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gboxSr.Controls.Add(this.lblErrorSr);
            this.gboxSr.Controls.Add(this.txtSr);
            this.gboxSr.Location = new System.Drawing.Point(8, 95);
            this.gboxSr.Name = "gboxSr";
            this.gboxSr.Size = new System.Drawing.Size(456, 76);
            this.gboxSr.TabIndex = 4;
            this.gboxSr.TabStop = false;
            this.gboxSr.Text = "座標系";
            // 
            // lblErrorSr
            // 
            this.lblErrorSr.AutoSize = true;
            this.lblErrorSr.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblErrorSr.ForeColor = System.Drawing.Color.DarkRed;
            this.lblErrorSr.Location = new System.Drawing.Point(19, 50);
            this.lblErrorSr.Name = "lblErrorSr";
            this.lblErrorSr.Size = new System.Drawing.Size(0, 12);
            this.lblErrorSr.TabIndex = 4;
            // 
            // txtSr
            // 
            this.txtSr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSr.Location = new System.Drawing.Point(8, 18);
            this.txtSr.Name = "txtSr";
            this.txtSr.ReadOnly = true;
            this.txtSr.Size = new System.Drawing.Size(442, 19);
            this.txtSr.TabIndex = 0;
            this.txtSr.TabStop = false;
            // 
            // lblProp
            // 
            this.lblProp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblProp.AutoSize = true;
            this.lblProp.Location = new System.Drawing.Point(8, 195);
            this.lblProp.Name = "lblProp";
            this.lblProp.Size = new System.Drawing.Size(51, 12);
            this.lblProp.TabIndex = 5;
            this.lblProp.Text = "プロパティ:";
            // 
            // lblField
            // 
            this.lblField.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(8, 235);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(51, 12);
            this.lblField.TabIndex = 6;
            this.lblField.Text = "フィールド:";
            // 
            // lblUnit
            // 
            this.lblUnit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(8, 275);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(31, 12);
            this.lblUnit.TabIndex = 7;
            this.lblUnit.Text = "単位:";
            // 
            // comboProperty
            // 
            this.comboProperty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboProperty.FormattingEnabled = true;
            this.comboProperty.Location = new System.Drawing.Point(72, 192);
            this.comboProperty.Name = "comboProperty";
            this.comboProperty.Size = new System.Drawing.Size(392, 20);
            this.comboProperty.TabIndex = 8;
            this.comboProperty.SelectedIndexChanged += new System.EventHandler(this.comboProperty_SelectedIndexChanged);
            // 
            // comboField
            // 
            this.comboField.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboField.FormattingEnabled = true;
            this.comboField.Location = new System.Drawing.Point(72, 232);
            this.comboField.Name = "comboField";
            this.comboField.Size = new System.Drawing.Size(392, 20);
            this.comboField.TabIndex = 9;
            this.comboField.SelectedIndexChanged += new System.EventHandler(this.comboField_SelectedIndexChanged);
            // 
            // comboUnit
            // 
            this.comboUnit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUnit.FormattingEnabled = true;
            this.comboUnit.Location = new System.Drawing.Point(72, 272);
            this.comboUnit.Name = "comboUnit";
            this.comboUnit.Size = new System.Drawing.Size(392, 20);
            this.comboUnit.TabIndex = 10;
            // 
            // chkPrecision
            // 
            this.chkPrecision.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkPrecision.AutoSize = true;
            this.chkPrecision.Location = new System.Drawing.Point(8, 312);
            this.chkPrecision.Name = "chkPrecision";
            this.chkPrecision.Size = new System.Drawing.Size(170, 16);
            this.chkPrecision.TabIndex = 11;
            this.chkPrecision.Text = "小数点以下の桁数を指定する";
            this.chkPrecision.UseVisualStyleBackColor = true;
            // 
            // chkAppendUnit
            // 
            this.chkAppendUnit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkAppendUnit.AutoSize = true;
            this.chkAppendUnit.Location = new System.Drawing.Point(8, 344);
            this.chkAppendUnit.Name = "chkAppendUnit";
            this.chkAppendUnit.Size = new System.Drawing.Size(204, 16);
            this.chkAppendUnit.TabIndex = 13;
            this.chkAppendUnit.Text = "単位の略名をテキストフィールドに追加";
            this.chkAppendUnit.UseVisualStyleBackColor = true;
            // 
            // chkUseSelectionSet
            // 
            this.chkUseSelectionSet.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkUseSelectionSet.AutoSize = true;
            this.chkUseSelectionSet.Location = new System.Drawing.Point(8, 376);
            this.chkUseSelectionSet.Name = "chkUseSelectionSet";
            this.chkUseSelectionSet.Size = new System.Drawing.Size(148, 16);
            this.chkUseSelectionSet.TabIndex = 14;
            this.chkUseSelectionSet.Text = "選択レコードのみ演算する";
            this.chkUseSelectionSet.UseVisualStyleBackColor = true;
            // 
            // nudDecimalPlaces
            // 
            this.nudDecimalPlaces.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudDecimalPlaces.Location = new System.Drawing.Point(423, 309);
            this.nudDecimalPlaces.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudDecimalPlaces.Name = "nudDecimalPlaces";
            this.nudDecimalPlaces.Size = new System.Drawing.Size(39, 19);
            this.nudDecimalPlaces.TabIndex = 12;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOk.Location = new System.Drawing.Point(296, 424);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(384, 424);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormGeometryCalculate
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(474, 463);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.nudDecimalPlaces);
            this.Controls.Add(this.chkUseSelectionSet);
            this.Controls.Add(this.chkAppendUnit);
            this.Controls.Add(this.chkPrecision);
            this.Controls.Add(this.comboUnit);
            this.Controls.Add(this.comboField);
            this.Controls.Add(this.comboProperty);
            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.lblField);
            this.Controls.Add(this.lblProp);
            this.Controls.Add(this.gboxSr);
            this.Controls.Add(this.lblErrorLayer);
            this.Controls.Add(this.comboLayer);
            this.Controls.Add(this.lblLayer);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGeometryCalculate";
            this.Text = "面積・長さ計算";
            this.gboxSr.ResumeLayout(false);
            this.gboxSr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimalPlaces)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox comboLayer;
        private System.Windows.Forms.Label lblErrorLayer;
        private System.Windows.Forms.GroupBox gboxSr;
        private System.Windows.Forms.Label lblErrorSr;
        private System.Windows.Forms.TextBox txtSr;
        private System.Windows.Forms.Label lblProp;
        private System.Windows.Forms.Label lblField;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.ComboBox comboProperty;
        private System.Windows.Forms.ComboBox comboField;
        private System.Windows.Forms.ComboBox comboUnit;
        private System.Windows.Forms.CheckBox chkPrecision;
        private System.Windows.Forms.CheckBox chkAppendUnit;
        private System.Windows.Forms.CheckBox chkUseSelectionSet;
        private System.Windows.Forms.NumericUpDown nudDecimalPlaces;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}