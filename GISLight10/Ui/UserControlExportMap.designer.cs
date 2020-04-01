using ESRIJapan.FileDlgExtenders;

namespace ESRIJapan.GISLight10.Ui
{
    partial class UserControlExportMap
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.numericUpDownDpi = new System.Windows.Forms.NumericUpDown();
            this.labelWidth = new System.Windows.Forms.Label();
            this.labelHeight = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panelWidthHeight = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDpi)).BeginInit();
            this.panelWidthHeight.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericUpDownDpi
            // 
            this.numericUpDownDpi.Location = new System.Drawing.Point(82, 45);
            this.numericUpDownDpi.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownDpi.Name = "numericUpDownDpi";
            this.numericUpDownDpi.Size = new System.Drawing.Size(150, 19);
            this.numericUpDownDpi.TabIndex = 1;
            this.numericUpDownDpi.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDpi.ValueChanged += new System.EventHandler(this.numericUpDownDpi_ValueChanged);
            this.numericUpDownDpi.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.numericUpDownDpi_KeyPress);
            // 
            // labelWidth
            // 
            this.labelWidth.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelWidth.Location = new System.Drawing.Point(74, 20);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(150, 18);
            this.labelWidth.TabIndex = 2;
            this.labelWidth.Text = "Width";
            // 
            // labelHeight
            // 
            this.labelHeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelHeight.Location = new System.Drawing.Point(74, 44);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(150, 18);
            this.labelHeight.TabIndex = 3;
            this.labelHeight.Text = "Height";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(26, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "解像度";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(18, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "幅";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(18, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "高さ";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(230, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "ピクセル";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(230, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "ピクセル";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(238, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 17);
            this.label6.TabIndex = 7;
            this.label6.Text = "dpi";
            // 
            // panelWidthHeight
            // 
            this.panelWidthHeight.Controls.Add(this.label4);
            this.panelWidthHeight.Controls.Add(this.label5);
            this.panelWidthHeight.Controls.Add(this.label3);
            this.panelWidthHeight.Controls.Add(this.label2);
            this.panelWidthHeight.Controls.Add(this.labelHeight);
            this.panelWidthHeight.Controls.Add(this.labelWidth);
            this.panelWidthHeight.Location = new System.Drawing.Point(5, 79);
            this.panelWidthHeight.Name = "panelWidthHeight";
            this.panelWidthHeight.Size = new System.Drawing.Size(295, 91);
            this.panelWidthHeight.TabIndex = 10;
            // 
            // UserControlExportMap
            // 
            this.Controls.Add(this.panelWidthHeight);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownDpi);
            this.FileDlgStartLocation = ESRIJapan.FileDlgExtenders.AddonWindowLocation.Bottom;
            this.Name = "UserControlExportMap";
            this.Size = new System.Drawing.Size(304, 190);
            this.Load += new System.EventHandler(this.UserControlExportMap_Load);
            this.EventFilterChanged += new ESRIJapan.FileDlgExtenders.FileDialogControlBase.FilterChangedEventHandler(this.UserControlExporMap_FilterChanged);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDpi)).EndInit();
            this.panelWidthHeight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownDpi;
        private System.Windows.Forms.Panel panelWidthHeight;
    }
}
