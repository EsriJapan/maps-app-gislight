namespace ESRIJapan.GISLight10.Ui
{
    partial class FormAttributeSearch
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAttributeSearch));
			this.comboBoxLayerNames = new System.Windows.Forms.ComboBox();
			this.listBoxFieldNames = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxSelectionMethods = new System.Windows.Forms.ComboBox();
			this.buttonEqual = new System.Windows.Forms.Button();
			this.buttonNotEqual = new System.Windows.Forms.Button();
			this.buttonLike = new System.Windows.Forms.Button();
			this.buttonAnd = new System.Windows.Forms.Button();
			this.buttonGreaterOrEqual = new System.Windows.Forms.Button();
			this.buttonGreaterThan = new System.Windows.Forms.Button();
			this.buttonOr = new System.Windows.Forms.Button();
			this.buttonLessOrEqual = new System.Windows.Forms.Button();
			this.buttonLessThan = new System.Windows.Forms.Button();
			this.labelSqlString = new System.Windows.Forms.Label();
			this.buttonGetIndividualValue = new System.Windows.Forms.Button();
			this.listBoxIndividualValues = new System.Windows.Forms.ListBox();
			this.richTextBoxCriteria = new System.Windows.Forms.RichTextBox();
			this.buttonClearCriteria = new System.Windows.Forms.Button();
			this.buttonPaddingChar = new System.Windows.Forms.Button();
			this.buttonLikeChar = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.labelMessage = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonParentheses = new System.Windows.Forms.Button();
			this.buttonNot = new System.Windows.Forms.Button();
			this.buttonIs = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxLayerNames
			// 
			this.comboBoxLayerNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLayerNames.FormattingEnabled = true;
			this.comboBoxLayerNames.Location = new System.Drawing.Point(70, 30);
			this.comboBoxLayerNames.Name = "comboBoxLayerNames";
			this.comboBoxLayerNames.Size = new System.Drawing.Size(394, 20);
			this.comboBoxLayerNames.TabIndex = 0;
			this.comboBoxLayerNames.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerNames_SelectedIndexChanged);
			// 
			// listBoxFieldNames
			// 
			this.listBoxFieldNames.FormattingEnabled = true;
			this.listBoxFieldNames.ItemHeight = 12;
			this.listBoxFieldNames.Location = new System.Drawing.Point(8, 112);
			this.listBoxFieldNames.Name = "listBoxFieldNames";
			this.listBoxFieldNames.Size = new System.Drawing.Size(456, 100);
			this.listBoxFieldNames.TabIndex = 2;
			this.listBoxFieldNames.DoubleClick += new System.EventHandler(this.listBoxFieldNames_DoubleClick);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "レイヤ:";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(208, 498);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(78, 23);
			this.buttonOK.TabIndex = 18;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOKorApply_Click);
			// 
			// buttonApply
			// 
			this.buttonApply.Location = new System.Drawing.Point(296, 498);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(78, 23);
			this.buttonApply.TabIndex = 19;
			this.buttonApply.Text = "適用";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.buttonOKorApply_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(384, 498);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(78, 23);
			this.buttonCancel.TabIndex = 20;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 69);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 13);
			this.label4.TabIndex = 19;
			this.label4.Text = "選択方法:";
			// 
			// comboBoxSelectionMethods
			// 
			this.comboBoxSelectionMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSelectionMethods.FormattingEnabled = true;
			this.comboBoxSelectionMethods.Location = new System.Drawing.Point(70, 66);
			this.comboBoxSelectionMethods.Name = "comboBoxSelectionMethods";
			this.comboBoxSelectionMethods.Size = new System.Drawing.Size(395, 20);
			this.comboBoxSelectionMethods.TabIndex = 1;
			this.comboBoxSelectionMethods.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectionMethods_SelectedIndexChanged);
			// 
			// buttonEqual
			// 
			this.buttonEqual.Location = new System.Drawing.Point(8, 220);
			this.buttonEqual.Name = "buttonEqual";
			this.buttonEqual.Size = new System.Drawing.Size(41, 22);
			this.buttonEqual.TabIndex = 3;
			this.buttonEqual.Text = "=";
			this.buttonEqual.UseVisualStyleBackColor = true;
			this.buttonEqual.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonNotEqual
			// 
			this.buttonNotEqual.Location = new System.Drawing.Point(57, 220);
			this.buttonNotEqual.Name = "buttonNotEqual";
			this.buttonNotEqual.Size = new System.Drawing.Size(41, 22);
			this.buttonNotEqual.TabIndex = 4;
			this.buttonNotEqual.Text = "<>";
			this.buttonNotEqual.UseVisualStyleBackColor = true;
			this.buttonNotEqual.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonLike
			// 
			this.buttonLike.Location = new System.Drawing.Point(106, 220);
			this.buttonLike.Name = "buttonLike";
			this.buttonLike.Size = new System.Drawing.Size(41, 22);
			this.buttonLike.TabIndex = 5;
			this.buttonLike.Text = "Like";
			this.buttonLike.UseVisualStyleBackColor = true;
			this.buttonLike.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonAnd
			// 
			this.buttonAnd.Location = new System.Drawing.Point(106, 247);
			this.buttonAnd.Name = "buttonAnd";
			this.buttonAnd.Size = new System.Drawing.Size(41, 22);
			this.buttonAnd.TabIndex = 8;
			this.buttonAnd.Text = "And";
			this.buttonAnd.UseVisualStyleBackColor = true;
			this.buttonAnd.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonGreaterOrEqual
			// 
			this.buttonGreaterOrEqual.Location = new System.Drawing.Point(57, 247);
			this.buttonGreaterOrEqual.Name = "buttonGreaterOrEqual";
			this.buttonGreaterOrEqual.Size = new System.Drawing.Size(41, 22);
			this.buttonGreaterOrEqual.TabIndex = 7;
			this.buttonGreaterOrEqual.Text = ">=";
			this.buttonGreaterOrEqual.UseVisualStyleBackColor = true;
			this.buttonGreaterOrEqual.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonGreaterThan
			// 
			this.buttonGreaterThan.Location = new System.Drawing.Point(8, 247);
			this.buttonGreaterThan.Name = "buttonGreaterThan";
			this.buttonGreaterThan.Size = new System.Drawing.Size(41, 22);
			this.buttonGreaterThan.TabIndex = 6;
			this.buttonGreaterThan.Text = ">";
			this.buttonGreaterThan.UseVisualStyleBackColor = true;
			this.buttonGreaterThan.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonOr
			// 
			this.buttonOr.Location = new System.Drawing.Point(106, 274);
			this.buttonOr.Name = "buttonOr";
			this.buttonOr.Size = new System.Drawing.Size(41, 22);
			this.buttonOr.TabIndex = 11;
			this.buttonOr.Text = "Or";
			this.buttonOr.UseVisualStyleBackColor = true;
			this.buttonOr.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonLessOrEqual
			// 
			this.buttonLessOrEqual.Location = new System.Drawing.Point(57, 274);
			this.buttonLessOrEqual.Name = "buttonLessOrEqual";
			this.buttonLessOrEqual.Size = new System.Drawing.Size(41, 22);
			this.buttonLessOrEqual.TabIndex = 10;
			this.buttonLessOrEqual.Text = "<=";
			this.buttonLessOrEqual.UseVisualStyleBackColor = true;
			this.buttonLessOrEqual.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonLessThan
			// 
			this.buttonLessThan.Location = new System.Drawing.Point(8, 274);
			this.buttonLessThan.Name = "buttonLessThan";
			this.buttonLessThan.Size = new System.Drawing.Size(41, 22);
			this.buttonLessThan.TabIndex = 9;
			this.buttonLessThan.Text = "<";
			this.buttonLessThan.UseVisualStyleBackColor = true;
			this.buttonLessThan.Click += new System.EventHandler(this.buttonOperation);
			// 
			// labelSqlString
			// 
			this.labelSqlString.AutoSize = true;
			this.labelSqlString.Location = new System.Drawing.Point(4, 4);
			this.labelSqlString.Name = "labelSqlString";
			this.labelSqlString.Size = new System.Drawing.Size(97, 12);
			this.labelSqlString.TabIndex = 30;
			this.labelSqlString.Text = "SELECT * FROM ";
			// 
			// buttonGetIndividualValue
			// 
			this.buttonGetIndividualValue.Location = new System.Drawing.Point(368, 330);
			this.buttonGetIndividualValue.Name = "buttonGetIndividualValue";
			this.buttonGetIndividualValue.Size = new System.Drawing.Size(96, 22);
			this.buttonGetIndividualValue.TabIndex = 15;
			this.buttonGetIndividualValue.Text = "個別値を取得";
			this.buttonGetIndividualValue.UseVisualStyleBackColor = true;
			this.buttonGetIndividualValue.Click += new System.EventHandler(this.buttonGetIndividualValue_Click);
			// 
			// listBoxIndividualValues
			// 
			this.listBoxIndividualValues.FormattingEnabled = true;
			this.listBoxIndividualValues.ItemHeight = 12;
			this.listBoxIndividualValues.Location = new System.Drawing.Point(160, 224);
			this.listBoxIndividualValues.Name = "listBoxIndividualValues";
			this.listBoxIndividualValues.Size = new System.Drawing.Size(304, 100);
			this.listBoxIndividualValues.TabIndex = 14;
			this.listBoxIndividualValues.DoubleClick += new System.EventHandler(this.listBoxIndividualValues_DoubleClick);
			// 
			// richTextBoxCriteria
			// 
			this.richTextBoxCriteria.DetectUrls = false;
			this.richTextBoxCriteria.Location = new System.Drawing.Point(0, 18);
			this.richTextBoxCriteria.Name = "richTextBoxCriteria";
			this.richTextBoxCriteria.Size = new System.Drawing.Size(457, 87);
			this.richTextBoxCriteria.TabIndex = 16;
			this.richTextBoxCriteria.Text = "\"\"";
			this.richTextBoxCriteria.TextChanged += new System.EventHandler(this.richTextBoxCriteria_TextChanged);
			// 
			// buttonClearCriteria
			// 
			this.buttonClearCriteria.Location = new System.Drawing.Point(3, 106);
			this.buttonClearCriteria.Name = "buttonClearCriteria";
			this.buttonClearCriteria.Size = new System.Drawing.Size(78, 23);
			this.buttonClearCriteria.TabIndex = 17;
			this.buttonClearCriteria.Text = "消去";
			this.buttonClearCriteria.UseVisualStyleBackColor = true;
			this.buttonClearCriteria.Click += new System.EventHandler(this.buttonClearCriteria_Click);
			// 
			// buttonPaddingChar
			// 
			this.buttonPaddingChar.Location = new System.Drawing.Point(8, 302);
			this.buttonPaddingChar.Name = "buttonPaddingChar";
			this.buttonPaddingChar.Size = new System.Drawing.Size(20, 22);
			this.buttonPaddingChar.TabIndex = 12;
			this.buttonPaddingChar.Text = "_";
			this.buttonPaddingChar.UseVisualStyleBackColor = true;
			this.buttonPaddingChar.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonLikeChar
			// 
			this.buttonLikeChar.Location = new System.Drawing.Point(30, 302);
			this.buttonLikeChar.Name = "buttonLikeChar";
			this.buttonLikeChar.Size = new System.Drawing.Size(20, 22);
			this.buttonLikeChar.TabIndex = 13;
			this.buttonLikeChar.Text = "%";
			this.buttonLikeChar.UseVisualStyleBackColor = true;
			this.buttonLikeChar.Click += new System.EventHandler(this.buttonOperation);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.richTextBoxCriteria);
			this.panel1.Controls.Add(this.labelSqlString);
			this.panel1.Controls.Add(this.buttonClearCriteria);
			this.panel1.Location = new System.Drawing.Point(8, 355);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(456, 135);
			this.panel1.TabIndex = 16;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(364, 12);
			this.label1.TabIndex = 21;
			this.label1.Text = "入力レイヤから選択方法と指定検索条件に基づいてフィーチャを選択します。";
			// 
			// labelMessage
			// 
			this.labelMessage.AutoSize = true;
			this.labelMessage.Location = new System.Drawing.Point(14, 568);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(0, 12);
			this.labelMessage.TabIndex = 22;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 97);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(51, 12);
			this.label3.TabIndex = 23;
			this.label3.Text = "フィールド:";
			// 
			// buttonParentheses
			// 
			this.buttonParentheses.Location = new System.Drawing.Point(57, 302);
			this.buttonParentheses.Name = "buttonParentheses";
			this.buttonParentheses.Size = new System.Drawing.Size(41, 22);
			this.buttonParentheses.TabIndex = 10;
			this.buttonParentheses.Text = "( )";
			this.buttonParentheses.UseVisualStyleBackColor = true;
			this.buttonParentheses.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonNot
			// 
			this.buttonNot.Location = new System.Drawing.Point(106, 302);
			this.buttonNot.Name = "buttonNot";
			this.buttonNot.Size = new System.Drawing.Size(41, 22);
			this.buttonNot.TabIndex = 11;
			this.buttonNot.Text = "Not";
			this.buttonNot.UseVisualStyleBackColor = true;
			this.buttonNot.Click += new System.EventHandler(this.buttonOperation);
			// 
			// buttonIs
			// 
			this.buttonIs.Location = new System.Drawing.Point(8, 330);
			this.buttonIs.Name = "buttonIs";
			this.buttonIs.Size = new System.Drawing.Size(41, 22);
			this.buttonIs.TabIndex = 11;
			this.buttonIs.Text = "Is";
			this.buttonIs.UseVisualStyleBackColor = true;
			this.buttonIs.Click += new System.EventHandler(this.buttonOperation);
			// 
			// FormAttributeSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(474, 527);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonGetIndividualValue);
			this.Controls.Add(this.buttonLikeChar);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonPaddingChar);
			this.Controls.Add(this.listBoxIndividualValues);
			this.Controls.Add(this.buttonIs);
			this.Controls.Add(this.buttonNot);
			this.Controls.Add(this.buttonParentheses);
			this.Controls.Add(this.buttonOr);
			this.Controls.Add(this.buttonLessOrEqual);
			this.Controls.Add(this.buttonLessThan);
			this.Controls.Add(this.buttonAnd);
			this.Controls.Add(this.buttonGreaterOrEqual);
			this.Controls.Add(this.buttonGreaterThan);
			this.Controls.Add(this.buttonLike);
			this.Controls.Add(this.buttonNotEqual);
			this.Controls.Add(this.buttonEqual);
			this.Controls.Add(this.comboBoxSelectionMethods);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxLayerNames);
			this.Controls.Add(this.listBoxFieldNames);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAttributeSearch";
			this.ShowIcon = false;
			this.Text = "属性検索";
			this.Load += new System.EventHandler(this.FormAttributeSearch_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxLayerNames;
        private System.Windows.Forms.ListBox listBoxFieldNames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxSelectionMethods;
        private System.Windows.Forms.Button buttonEqual;
        private System.Windows.Forms.Button buttonNotEqual;
        private System.Windows.Forms.Button buttonLike;
        private System.Windows.Forms.Button buttonAnd;
        private System.Windows.Forms.Button buttonGreaterOrEqual;
        private System.Windows.Forms.Button buttonGreaterThan;
        private System.Windows.Forms.Button buttonOr;
        private System.Windows.Forms.Button buttonLessOrEqual;
        private System.Windows.Forms.Button buttonLessThan;
        private System.Windows.Forms.Label labelSqlString;
        private System.Windows.Forms.Button buttonGetIndividualValue;
        private System.Windows.Forms.ListBox listBoxIndividualValues;
        private System.Windows.Forms.RichTextBox richTextBoxCriteria;
        private System.Windows.Forms.Button buttonClearCriteria;
        private System.Windows.Forms.Button buttonPaddingChar;
        private System.Windows.Forms.Button buttonLikeChar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonParentheses;
		private System.Windows.Forms.Button buttonNot;
		private System.Windows.Forms.Button buttonIs;
    }
}