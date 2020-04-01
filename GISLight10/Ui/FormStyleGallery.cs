using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Collections.Generic;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// �X�^�C���t�@�C����Ǎ��݁A
    /// �w��X�^�C���N���X�̃V���{�����ꗗ�\�����A
    /// �w��̃V���{�����擾����B
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    ///  2011-01-25 internal�X�R�[�v��XML�R�����g�s�����̋L�q
    /// </history>
    public class FormStyleGallery : System.Windows.Forms.Form
    {
        private bool InitStatus = true;

        private string m_sInstall = string.Empty;

        private IContainer components;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private ESRI.ArcGIS.Controls.AxSymbologyControl axSymbologyControl1;

        private IMapFrame mapFrame;
		private IStyleGalleryItem m_styleGalleryItem;
        private System.Drawing.Image m_image; 

        private IPageLayoutControl3 m_pageLayoutControl = null;
        private Panel panel1;
        private Label labelPreview;
        private IEnvelope envelope;
        private Ui.MainForm mainFrm;

        // �V���{���ݒ�p�������[�h�R�[�h
        /// <summary>�}�[�J�[�V���{���i�|�C���g�j�������[�h�R�[�h </summary>
        internal const int ModeMarkerSymbol = 0;

        /// <summary>���C���V���{���������[�h�R�[�h </summary>
        internal const int ModeLineSymbol = 1;

        /// <summary>�t�B���V���{���i�|���S���j�������[�h�R�[�h</summary>
        internal const int ModeFillSymbol = 2;

        /// <summary>�G�������g�ݒ菈���i���ʋL���A�k�ڋL���j���[�h�R�[�h</summary>
        internal const int ModeSetElem = 3;

        /// <summary>�L�����N�^�}�[�J�[�V���{���������[�h�ւ̕ύX</summary>
        internal const int ModeChangedToCharacterFont = 4;

        /// <summary>�J���[�����v�������[�h�R�[�h</summary>
        internal const int ModeColorRamp = 5;

        /// <summary>�������[�h�ێ�</summary>
        internal int SaveMode;

        private ComboBox comboBoxFonts;
        static private string[] TargetFontNameHeaders = 
            { "ESRI" }; //, "plm", "plo","plu","psp","pxx","DM_point" };

		static private string[]	StyleFileNames = {
			"ESRI.ServerStyle",
			"��ʕW��.ServerStyle",
			"�������ʁiDM�j�W���}��.ServerStyle",
			"�n�`�}�}��.ServerStyle"
		};

        /// <summary>
        /// �}�[�J�[�V���{���V���{���^�C�v�I��p�R���{�{�b�N�X
        /// �V���v���}�[�J�[�A�܂��͂��̑��}�[�J�[
        /// </summary>
        internal ComboBox comboBoxStyleType;

        private Label labelFontName;
        private Label labelStyleType;

        /// <summary>
        /// �V���v���}�[�J�[�V���{���̃^�C�v�I��p�R���{�{�b�N�X
        /// </summary>
        internal ComboBox comboBoxSimpelMarkerStyle;
        private Label labelSimpleMarkerStyle;

        /// <summary>
        /// �V���{���ݒ�p�������[�h�R�[�h
        /// </summary>
        internal int RunMode = ModeMarkerSymbol;
        private ESRI.ArcGIS.Display.IRgbColor m_rgbColor;

        /// <summary>
        /// �I���\�V���v���}�[�J�[���i�[����z��
        /// </summary>
        internal ESRI.ArcGIS.Display.ISimpleMarkerSymbol[] simpleMarkers = null;
        
        /// <summary>
        /// �I�����ꂽ�t�H���g
        /// </summary>
        internal System.Drawing.Font selectedCharacterFont = null;
        
        /// <summary>
        /// �I�����ꂽ�t�H���g����
        /// </summary>
        internal string selefontName = null;

        private ImageList imageListGlyph;
        private ListView listViewGlyph;

        /// <summary>
        /// �I�����ꂽ�摜
        /// </summary>
        internal stdole.IPictureDisp selectedPpicture = null;
        private int selectedCharacterIndex = -1;
        private System.Collections.ArrayList arrglyphIdx;
        private CheckBox checkBoxSelectFromFonts;
        /// <summary>
        /// �I�����ꂽ�摜�̃L�����N�^�}�[�J�[�V���{��
        /// </summary>
        internal ICharacterMarkerSymbol SelecteCharecterSymbol = null;
        private CheckBox checkBoxKoukyouSokuryou;
        private CheckBox checkBoxKoutsuHyoushiki;
        private CheckBox checkBoxChikeizu;

        private Label labelStyleName;

		/// <summary>
		/// �Ǎ��ΏۃX�^�C���t�@�C���̍ő吔���擾���܂�
		/// </summary>
		public int StyleFileNum {
			get {
				return StyleFileNames.Length;
			}
		}

        /// <summary>
        /// �V���{���ݒ�@�\����̌Ăяo���p�R���X�g���N�^
        /// </summary>
        /// <param name="rgbColor">IRgbColor</param>
        /// <param name="mode">�������[�h</param>
        public FormStyleGallery(ESRI.ArcGIS.Display.IRgbColor rgbColor, int mode)
        {
            InitializeComponent();

            this.RunMode = mode;
            this.SaveMode = this.RunMode;

            this.m_rgbColor = rgbColor;
            
			// ���٥̧�ق̓ǂݍ���
            this.LoadStyle();
        }

        /// <summary>
        /// �G�������g�ݒ菈���p�R���X�g���N�^(PageLayout�\����Ԏ�)
        /// </summary>
        /// <param name="mapFrame">�y�[�W���C�A�E�g�R���g���[���̃}�b�v�t���[��</param>
        /// <param name="pageLayoutControl">�y�[�W���C�A�E�g�R���g���[��</param>
        /// <param name="envelope">�y�[�W���C�A�E�g�R���g���[����őI����Ԃ̃G�������g</param>
        public FormStyleGallery(
            IMapFrame mapFrame, 
            IPageLayoutControl3 pageLayoutControl, 
            IEnvelope envelope)
		{
			InitializeComponent();

            // �y�[�W���C�A�E�g��ʕ\����Ԃ̃G�������g�ݒ菈����
            RunMode = ModeSetElem;

            this.mapFrame = mapFrame;
            this.m_pageLayoutControl = pageLayoutControl;
            this.envelope = envelope;

            IntPtr ptr2 = (System.IntPtr)this.m_pageLayoutControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();

            LoadStyle();
		}


        /// <summary>
        /// �X�^�C���t�@�C���Ǎ�
        /// </summary>
        private void LoadStyle() {

			// Engine���߽���擾����
			m_sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;

			// ���٥̧�ق�ǂݍ���
			this.LoadStyleFile(0, true);

 
        }

        /// <summary>
        /// �g�p�����I�u�W�F�N�g�̔p��
        /// </summary>
        /// <param name="disposing">
        /// �}�l�[�W�h�I�u�W�F�N�g�ێ����\�[�X���ׂĉ������t���O</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)  
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStyleGallery));
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.axSymbologyControl1 = new ESRI.ArcGIS.Controls.AxSymbologyControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkBoxChikeizu = new System.Windows.Forms.CheckBox();
			this.checkBoxKoukyouSokuryou = new System.Windows.Forms.CheckBox();
			this.checkBoxKoutsuHyoushiki = new System.Windows.Forms.CheckBox();
			this.labelStyleName = new System.Windows.Forms.Label();
			this.checkBoxSelectFromFonts = new System.Windows.Forms.CheckBox();
			this.labelFontName = new System.Windows.Forms.Label();
			this.labelStyleType = new System.Windows.Forms.Label();
			this.comboBoxStyleType = new System.Windows.Forms.ComboBox();
			this.comboBoxFonts = new System.Windows.Forms.ComboBox();
			this.labelPreview = new System.Windows.Forms.Label();
			this.comboBoxSimpelMarkerStyle = new System.Windows.Forms.ComboBox();
			this.labelSimpleMarkerStyle = new System.Windows.Forms.Label();
			this.imageListGlyph = new System.Windows.Forms.ImageList(this.components);
			this.listViewGlyph = new System.Windows.Forms.ListView();
			((System.ComponentModel.ISupportInitialize)(this.axSymbologyControl1)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.Location = new System.Drawing.Point(9, 296);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(71, 22);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(86, 296);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(71, 22);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "�L�����Z��";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// axSymbologyControl1
			// 
			this.axSymbologyControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.axSymbologyControl1.Location = new System.Drawing.Point(0, 0);
			this.axSymbologyControl1.Name = "axSymbologyControl1";
			this.axSymbologyControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSymbologyControl1.OcxState")));
			this.axSymbologyControl1.Size = new System.Drawing.Size(361, 323);
			this.axSymbologyControl1.TabIndex = 0;
			this.axSymbologyControl1.OnDoubleClick += new ESRI.ArcGIS.Controls.ISymbologyControlEvents_Ax_OnDoubleClickEventHandler(this.axSymbologyControl1_OnDoubleClick);
			this.axSymbologyControl1.OnItemSelected += new ESRI.ArcGIS.Controls.ISymbologyControlEvents_Ax_OnItemSelectedEventHandler(this.axSymbologyControl1_OnItemSelected);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.checkBoxChikeizu);
			this.panel1.Controls.Add(this.checkBoxKoukyouSokuryou);
			this.panel1.Controls.Add(this.checkBoxKoutsuHyoushiki);
			this.panel1.Controls.Add(this.labelStyleName);
			this.panel1.Controls.Add(this.checkBoxSelectFromFonts);
			this.panel1.Controls.Add(this.labelFontName);
			this.panel1.Controls.Add(this.labelStyleType);
			this.panel1.Controls.Add(this.comboBoxStyleType);
			this.panel1.Controls.Add(this.comboBoxFonts);
			this.panel1.Controls.Add(this.labelPreview);
			this.panel1.Controls.Add(this.buttonOk);
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Location = new System.Drawing.Point(362, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(162, 323);
			this.panel1.TabIndex = 9;
			// 
			// checkBoxChikeizu
			// 
			this.checkBoxChikeizu.AutoSize = true;
			this.checkBoxChikeizu.Location = new System.Drawing.Point(9, 173);
			this.checkBoxChikeizu.Name = "checkBoxChikeizu";
			this.checkBoxChikeizu.Size = new System.Drawing.Size(84, 16);
			this.checkBoxChikeizu.TabIndex = 13;
			this.checkBoxChikeizu.Tag = "3";
			this.checkBoxChikeizu.Text = "�n�`�}�}��";
			this.checkBoxChikeizu.UseVisualStyleBackColor = true;
			this.checkBoxChikeizu.CheckedChanged += new System.EventHandler(this.StyleFileLoad_CheckedChanged);
			// 
			// checkBoxKoukyouSokuryou
			// 
			this.checkBoxKoukyouSokuryou.AutoSize = true;
			this.checkBoxKoukyouSokuryou.Location = new System.Drawing.Point(9, 157);
			this.checkBoxKoukyouSokuryou.Name = "checkBoxKoukyouSokuryou";
			this.checkBoxKoukyouSokuryou.Size = new System.Drawing.Size(101, 16);
			this.checkBoxKoukyouSokuryou.TabIndex = 12;
			this.checkBoxKoukyouSokuryou.Tag = "2";
			this.checkBoxKoukyouSokuryou.Text = "�������ʁiDM�j";
			this.checkBoxKoukyouSokuryou.UseVisualStyleBackColor = true;
			this.checkBoxKoukyouSokuryou.CheckedChanged += new System.EventHandler(this.StyleFileLoad_CheckedChanged);
			// 
			// checkBoxKoutsuHyoushiki
			// 
			this.checkBoxKoutsuHyoushiki.AutoSize = true;
			this.checkBoxKoutsuHyoushiki.Location = new System.Drawing.Point(9, 140);
			this.checkBoxKoutsuHyoushiki.Name = "checkBoxKoutsuHyoushiki";
			this.checkBoxKoutsuHyoushiki.Size = new System.Drawing.Size(72, 16);
			this.checkBoxKoutsuHyoushiki.TabIndex = 11;
			this.checkBoxKoutsuHyoushiki.Tag = "1";
			this.checkBoxKoutsuHyoushiki.Text = "��ʕW��";
			this.checkBoxKoutsuHyoushiki.UseVisualStyleBackColor = true;
			this.checkBoxKoutsuHyoushiki.CheckedChanged += new System.EventHandler(this.StyleFileLoad_CheckedChanged);
			// 
			// labelStyleName
			// 
			this.labelStyleName.AutoSize = true;
			this.labelStyleName.Location = new System.Drawing.Point(12, 195);
			this.labelStyleName.Name = "labelStyleName";
			this.labelStyleName.Size = new System.Drawing.Size(11, 12);
			this.labelStyleName.TabIndex = 10;
			this.labelStyleName.Text = "...";
			// 
			// checkBoxSelectFromFonts
			// 
			this.checkBoxSelectFromFonts.AutoSize = true;
			this.checkBoxSelectFromFonts.Location = new System.Drawing.Point(8, 214);
			this.checkBoxSelectFromFonts.Name = "checkBoxSelectFromFonts";
			this.checkBoxSelectFromFonts.Size = new System.Drawing.Size(123, 16);
			this.checkBoxSelectFromFonts.TabIndex = 9;
			this.checkBoxSelectFromFonts.Text = "�w��t�H���g����I��";
			this.checkBoxSelectFromFonts.UseVisualStyleBackColor = true;
			this.checkBoxSelectFromFonts.Click += new System.EventHandler(this.checkBoxSelectFromFonts_Click);
			// 
			// labelFontName
			// 
			this.labelFontName.AutoSize = true;
			this.labelFontName.Location = new System.Drawing.Point(7, 235);
			this.labelFontName.Name = "labelFontName";
			this.labelFontName.Size = new System.Drawing.Size(40, 12);
			this.labelFontName.TabIndex = 7;
			this.labelFontName.Text = "�t�H���g:";
			// 
			// labelStyleType
			// 
			this.labelStyleType.AutoSize = true;
			this.labelStyleType.Location = new System.Drawing.Point(5, 99);
			this.labelStyleType.Name = "labelStyleType";
			this.labelStyleType.Size = new System.Drawing.Size(33, 12);
			this.labelStyleType.TabIndex = 6;
			this.labelStyleType.Text = "�^�C�v:";
			// 
			// comboBoxStyleType
			// 
			this.comboBoxStyleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxStyleType.FormattingEnabled = true;
			this.comboBoxStyleType.Location = new System.Drawing.Point(7, 114);
			this.comboBoxStyleType.Name = "comboBoxStyleType";
			this.comboBoxStyleType.Size = new System.Drawing.Size(143, 20);
			this.comboBoxStyleType.TabIndex = 2;
			this.comboBoxStyleType.SelectedIndexChanged += new System.EventHandler(this.comboBoxStyleType_SelectedIndexChanged);
			// 
			// comboBoxFonts
			// 
			this.comboBoxFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFonts.FormattingEnabled = true;
			this.comboBoxFonts.Location = new System.Drawing.Point(7, 250);
			this.comboBoxFonts.Name = "comboBoxFonts";
			this.comboBoxFonts.Size = new System.Drawing.Size(143, 20);
			this.comboBoxFonts.TabIndex = 3;
			this.comboBoxFonts.DropDown += new System.EventHandler(this.AdjustWidthComboBox_DropDown);
			this.comboBoxFonts.SelectedIndexChanged += new System.EventHandler(this.comboBoxFonts_SelectedIndexChanged);
			// 
			// labelPreview
			// 
			this.labelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelPreview.Location = new System.Drawing.Point(38, 5);
			this.labelPreview.Name = "labelPreview";
			this.labelPreview.Size = new System.Drawing.Size(88, 86);
			this.labelPreview.TabIndex = 3;
			// 
			// comboBoxSimpelMarkerStyle
			// 
			this.comboBoxSimpelMarkerStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSimpelMarkerStyle.FormattingEnabled = true;
			this.comboBoxSimpelMarkerStyle.Location = new System.Drawing.Point(59, 41);
			this.comboBoxSimpelMarkerStyle.Name = "comboBoxSimpelMarkerStyle";
			this.comboBoxSimpelMarkerStyle.Size = new System.Drawing.Size(141, 20);
			this.comboBoxSimpelMarkerStyle.TabIndex = 1;
			this.comboBoxSimpelMarkerStyle.SelectedIndexChanged += new System.EventHandler(this.comboBoxSimpelMarkerStyle_SelectedIndexChanged);
			// 
			// labelSimpleMarkerStyle
			// 
			this.labelSimpleMarkerStyle.AutoSize = true;
			this.labelSimpleMarkerStyle.Location = new System.Drawing.Point(18, 41);
			this.labelSimpleMarkerStyle.Name = "labelSimpleMarkerStyle";
			this.labelSimpleMarkerStyle.Size = new System.Drawing.Size(41, 12);
			this.labelSimpleMarkerStyle.TabIndex = 11;
			this.labelSimpleMarkerStyle.Text = "�X�^�C��";
			// 
			// imageListGlyph
			// 
			this.imageListGlyph.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageListGlyph.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListGlyph.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// listViewGlyph
			// 
			this.listViewGlyph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewGlyph.LargeImageList = this.imageListGlyph;
			this.listViewGlyph.Location = new System.Drawing.Point(0, 0);
			this.listViewGlyph.Name = "listViewGlyph";
			this.listViewGlyph.Size = new System.Drawing.Size(361, 323);
			this.listViewGlyph.SmallImageList = this.imageListGlyph;
			this.listViewGlyph.StateImageList = this.imageListGlyph;
			this.listViewGlyph.TabIndex = 8;
			this.listViewGlyph.UseCompatibleStateImageBehavior = false;
			this.listViewGlyph.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewGlyph_ItemSelectionChanged);
			this.listViewGlyph.DoubleClick += new System.EventHandler(this.buttonOK_Click);
			// 
			// FormStyleGallery
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(524, 323);
			this.Controls.Add(this.listViewGlyph);
			this.Controls.Add(this.labelSimpleMarkerStyle);
			this.Controls.Add(this.comboBoxSimpelMarkerStyle);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.axSymbologyControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 360);
			this.Name = "FormStyleGallery";
			this.ShowIcon = false;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormStyleGallery_FormClosed);
			this.Load += new System.EventHandler(this.FormStyleGallery_Load);
			this.Shown += new System.EventHandler(this.FormStyleGallery_Shown);
			((System.ComponentModel.ISupportInitialize)(this.axSymbologyControl1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
        /// FormStyleGallery���[�h���C�x���g�n���h��
		/// �������[�h���肵�����\����Ԃ̐ݒ�
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void FormStyleGallery_Load(object sender, System.EventArgs e)
		{
            if (!InitStatus) return;

            if (this.RunMode == ModeMarkerSymbol)
            {
                SetSimpleMarkerSimbol();
                SetSimpleMarkerStyle();
                SetVisibleSimpleMarker(true);

                SetStyleType();
                SetFontsList();
            }
            else
            {
                SetVisibleSimpleMarker(false);
                
                this.checkBoxSelectFromFonts.Visible = false;
                this.comboBoxFonts.Visible = false;
                this.listViewGlyph.Visible = false;

                this.checkBoxChikeizu.Visible = (this.RunMode == ModeFillSymbol);

                this.checkBoxKoukyouSokuryou.Visible =
                    (this.RunMode == ModeLineSymbol || this.RunMode == ModeFillSymbol);

                this.checkBoxKoutsuHyoushiki.Visible = false;
            }

            //Get the selected item
            ISymbologyStyleClass symbologyStyleClass =
                axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass);

            if (symbologyStyleClass.get_ItemCount(axSymbologyControl1.StyleClass) > 0)
            {
                // �f�t�H���g�̑I��
                m_styleGalleryItem = (IStyleGalleryItem)symbologyStyleClass.GetItem(0);
                PreviewImage();
            }

            this.comboBoxFonts.Enabled = false;
            this.labelStyleName.Visible = false;

            InitStatus = false;
        }



        /// <summary>
        /// �V���v���[�}�[�J�[�V���{�������̃R���g���[���\����\���؂�ւ�
        /// </summary>
        /// <param name="enable"></param>
        private void SetVisibleSimpleMarker(bool enable)
        {
            this.comboBoxFonts.Visible = enable;
            this.labelFontName.Visible = enable;
            this.labelStyleType.Visible = enable;
            this.comboBoxStyleType.Visible = enable;
            this.comboBoxSimpelMarkerStyle.Visible = enable;
            this.labelSimpleMarkerStyle.Visible = enable;
        }

        /// <summary>
        /// �V���v���[�}�[�J�[�V���{�������̑I���\�V���{���ݒ�
        /// </summary>
        private void SetSimpleMarkerStyle()
        {
            this.comboBoxSimpelMarkerStyle.Items.Add("�~");
            this.comboBoxSimpelMarkerStyle.Items.Add("�l�p");
            this.comboBoxSimpelMarkerStyle.Items.Add("�\��");
            this.comboBoxSimpelMarkerStyle.Items.Add("�~");
            this.comboBoxSimpelMarkerStyle.Items.Add("�H�`");
            this.comboBoxSimpelMarkerStyle.SelectedIndex = 0;
            this.comboBoxSimpelMarkerStyle.SelectedText = 
                this.comboBoxSimpelMarkerStyle.SelectedItem.ToString();
        }

        /// <summary>
        /// �}�[�J�[�V���{���i�|�C���g�j�������[�h���̑I���\�I�u�W�F�N�g�ݒ�
        /// </summary>
        private void SetStyleType()
        {
            this.comboBoxStyleType.Items.Add("�V���v���}�[�J�[");
            this.comboBoxStyleType.Items.Add("���̑��}�[�J�[�V���{��"); //�G�����}�[�J�[");
            this.comboBoxStyleType.SelectedIndex = 0;
            this.axSymbologyControl1.Visible = false;
            this.labelStyleName.Visible = false;
        }

        /// <summary>
        /// �I���\�Ώۃt�H���g����
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        private bool IsTargetFont(string fontName)
        {
            for (int i = 0; i < TargetFontNameHeaders.Length; i++)
            {
                //Common.Logger.Debug(fontName + " Compare ->" + TargetFontNameHeaders[i]);
                
                if (fontName.ToLower().IndexOf(
                        TargetFontNameHeaders[i].ToLower()) == 0) 
                    return true;
            }
            return false;
        }

        /// <summary>
        /// �\���Ώۂ̃t�H���g�擾
        /// </summary>
        private void SetFontsList()
        {

            Graphics g = CreateGraphics();
            //System.Drawing.FontFamily[] ffs = System.Drawing.FontFamily.GetFamilies(g);
            System.Drawing.FontFamily[] ffs = System.Drawing.FontFamily.Families;

            int defalutIndex = 0;
            int cnt = 0;
            foreach (System.Drawing.FontFamily ff in ffs)
            {
                if (IsTargetFont(ff.Name)) 
                {
                    comboBoxFonts.Items.Add(ff.Name);
                    if (ff.Name.Contains("ESRI Default Marker")) defalutIndex = cnt;

                    cnt++;
                }
            }
            g.Dispose();

            comboBoxFonts.SelectedIndex = defalutIndex;
           
        }

        /// <summary>
        /// �\���Ώۂ̃X�^�C���N���X���擾
        /// </summary>
        /// <param name="styleClass">
        /// �\���ΏۃX�^�C���N���X</param>
        public void SetItem(ESRI.ArcGIS.Controls.esriSymbologyStyleClass styleClass)
        {
            m_styleGalleryItem = null;

            //Set the style class of SymbologyControl1
            axSymbologyControl1.StyleClass = styleClass;

            //Change cursor
            //this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// �\���Ώۂ̃X�^�C���N���X���擾
        /// </summary>
        /// <param name="styleClass">�\���ΏۃX�^�C���N���X</param>
        /// <param name="symbol">�Ăяo�����Ŏg�p���̃V���{��</param>
		public void SetItem(ESRI.ArcGIS.Controls.esriSymbologyStyleClass styleClass, ISymbol symbol)
		{
			m_styleGalleryItem = null;
       
			//Set the style class of SymbologyControl1
			axSymbologyControl1.StyleClass = styleClass; 
      
		    ISymbologyStyleClass symbologyStyleClass = axSymbologyControl1.GetStyleClass(styleClass);



            this.Text = "�V���{���v���p�e�B�ݒ�";
		}

        /// <summary>
        /// �g�p�����X�^�C���M���������̉��
        /// </summary>
        public void CleraStyleItem()
        {
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(m_styleGalleryItem);

            //System.Runtime.InteropServices.Marshal.ReleaseComObject(m_styleGalleryItem);
            axSymbologyControl1.Clear();
            axSymbologyControl1.Dispose();
            axSymbologyControl1 = null;

            Common.Logger.Info("StyleForm UsingMemorySize Before GC :" +
               GC.GetTotalMemory(false).ToString() + " byte");

            GC.Collect();

            Common.Logger.Info("StyleForm UsingMemorySize After GC :" +
               GC.GetTotalMemory(false).ToString() + " byte");
        }


        /// <summary>
        /// �I�����ꂽ�X�^�C����Ԃ�
        /// </summary>
        /// <returns>�I�����ꂽ�X�^�C��</returns>
        public IStyleGalleryItem GetSelectedStyleItem()
        {
            ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy =
                new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

            IStyleGalleryItem temp_styleGalleryItem =
                objectCopy.Copy(m_styleGalleryItem) as IStyleGalleryItem;

            return temp_styleGalleryItem; // m_styleGalleryItem;
        }

        /// <summary>
        /// �w��X�^�C���N���X�̃V���{���ꗗ�\��
        /// �V���{���ݒ��ʂŎg�p
        /// </summary>
        /// <returns>�I�����ꂽ�X�^�C��</returns>
        public IStyleGalleryItem GetItem()
        {
            m_styleGalleryItem = null;

            //Show the modal form
            this.ShowDialog();

            return m_styleGalleryItem;
        }

        /// <summary>
        /// �w��X�^�C���N���X�̃V���{���ꗗ�\����
        /// �I������Ă���V���{���̃C���[�W��Ԃ�
        /// </summary>
        /// <returns>�V���{���̃C���[�W</returns>
        public System.Drawing.Image GetImage()
        {
            return m_image;
        }

        /// <summary>
        /// OK�{�^���N���b�N���܂��̓V���{���R���g���[���_�u���N���b�N������
        /// </summary>
        private void OKProc()
        {
            bool dispMessage = false;
            try
            {
                //
                if (RunMode == ModeChangedToCharacterFont)
                {
                    this.SelecteCharecterSymbol =
                        CreateCharacterMarkerSymbol(
                            m_rgbColor,
                            this.selectedCharacterFont.Size,
                            this.selectedCharacterIndex);
                }

                // �V���{���ݒ胂�[�h�ł͒P�Ƀ��^�[��
                // 
                if (RunMode != ModeSetElem)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                    return;
                }

                if (mapFrame == null) return;

                //Get the map frame of the focus map
                //Create a map surround frame
                IMapSurroundFrame mapSurroundFrame = new MapSurroundFrameClass();

                //Set its map frame and map surround
                mapSurroundFrame.MapFrame = mapFrame;
                mapSurroundFrame.MapSurround = (IMapSurround)m_styleGalleryItem.Item;

                if (axSymbologyControl1.StyleClass ==
                    esriSymbologyStyleClass.esriStyleClassScaleBars)
                {
                    IElementProperties pElemProp = mapSurroundFrame as IElementProperties;
                    pElemProp.Name = mainFrm.SCALE_BAR_ELEMENT_NAME;

                    IScaleBar scaleBar = mapSurroundFrame.MapSurround as IScaleBar;
                    scaleBar.Units = this.mainFrm.axMapControl1.MapUnits;
                    scaleBar.UnitLabel = Common.UtilityClass.GetMapUnitText(scaleBar.Units);
                }

                //QI to IElement and set its geometry
                IElement element = (IElement)mapSurroundFrame;
                element.Geometry = this.envelope;

                //Add the element to the graphics container
                m_pageLayoutControl.ActiveView.GraphicsContainer.AddElement(
                    (IElement)mapSurroundFrame, 0);

                
                //Refresh
                m_pageLayoutControl.ActiveView.PartialRefresh(
                    esriViewDrawPhase.esriViewGraphics, mapSurroundFrame, null);

               

                this.Close();
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (dispMessage)
                {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.FormStyleGallery_ErrorWhenSetStyle);
                }
            }
        }

        /// <summary>
        /// �I���V���{���̃C���[�W�v���r���[�ݒ�
        /// </summary>
        private void PreviewImage()
        {
            //Get and set the style class 
            ISymbologyStyleClass symbologyStyleClass = 
                axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass);

            //Preview an image of the symbol
            stdole.IPictureDisp picture = 
                symbologyStyleClass.PreviewItem(
                m_styleGalleryItem, labelPreview.Width, labelPreview.Height);

            this.selectedPpicture = picture;

            //System.Drawing.Image image = 
            m_image= 
                System.Drawing.Image.FromHbitmap(new System.IntPtr(picture.Handle));

            labelPreview.Image = m_image;
        }

        /// <summary>
        /// �V���{���I�����C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axSymbologyControl1_OnItemSelected(
            object sender, 
            ESRI.ArcGIS.Controls.ISymbologyControlEvents_OnItemSelectedEvent e)
        {

            //Get the selected item
            m_styleGalleryItem = (IStyleGalleryItem)e.styleGalleryItem;


            // �V���{���ݒ��ʁA���̑��}�[�J�[�V���{����(�|�C���g�t�B�[�`��������)
            this.labelStyleName.Visible = false;
            this.labelStyleName.Text = "";

            if (this.comboBoxFonts.Visible && this.comboBoxStyleType.SelectedIndex == 1)
            {
                this.labelStyleName.Visible = true;
 
                ISymbol selectedItem = (ISymbol)m_styleGalleryItem.Item;

                if (selectedItem is SimpleMarkerSymbol || 
                    selectedItem is ISimpleMarkerSymbol || 
                    selectedItem is SimpleMarkerSymbolClass)
                {
                    this.labelStyleName.Text = "�V���v���}�[�J�[�V���{��";
                }
                else if (selectedItem is ICharacterMarkerSymbol || 
                         selectedItem is CharacterMarkerSymbol || 
                         selectedItem is CharacterMarkerSymbolClass)
                {
                    this.labelStyleName.Text = "�G�����}�[�J�[�V���{��";
                }
                else if (selectedItem is IPictureMarkerSymbol || 
                         selectedItem is PictureMarkerSymbol || 
                         selectedItem is PictureMarkerSymbolClass)
                {
                    this.labelStyleName.Text = "�s�N�`���[�}�[�J�[�V���{��";
                }
                else if (selectedItem is IMultiLayerMarkerSymbol || 
                         selectedItem is MultiLayerMarkerSymbol ||
                         selectedItem is MultiLayerMarkerSymbolClass)
                {
                    IMultiLayerMarkerSymbol multiLayerMarker =
                        selectedItem as IMultiLayerMarkerSymbol;

                    if (multiLayerMarker.LayerCount >= 1)
                    {
                        IMarkerSymbol markersym = multiLayerMarker.get_Layer(0);
                        if (markersym != null)
                        {
                            if (markersym is IPictureMarkerSymbol ||
                                markersym is PictureMarkerSymbol ||
                                markersym is PictureMarkerSymbolClass)
                            {
                                this.labelStyleName.Text = "�s�N�`���[�}�[�J�[�V���{��";
                            }
                            else if (markersym is ICharacterMarkerSymbol ||
                                     markersym is CharacterMarkerSymbol ||
                                     markersym is CharacterMarkerSymbolClass)
                            {
                                this.labelStyleName.Text = "�G�����}�[�J�[�V���{��";
                            }
                            else if (markersym is ISimpleMarkerSymbol ||
                                     markersym is SimpleMarkerSymbol ||
                                     markersym is SimpleMarkerSymbolClass)
                            {
                                this.labelStyleName.Text = "�V���v���}�[�J�[�V���{��";
                            }
                            else
                            {
                                this.labelStyleName.Text = "�}���`���C���[�}�[�J�[�V���{��";
                            }
                        }
                    }
                    else
                    {
                        this.labelStyleName.Text = "���̑��}���`���C���[�}�[�J�[�V���{��";
                    }
                }
                else if (selectedItem is IMarkerSymbol) 
                {
                    this.labelStyleName.Text = "���̑��}�[�J�[�V���{��";
                }
            }

            PreviewImage();
        }

        /// <summary>
        /// OK�{�^�����C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            OKProc();
		}

        /// <summary>
        /// �L�����Z���{�^�����C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_styleGalleryItem = null;
			this.Close(); 
		}

        /// <summary>
        /// Hide���������ł����L�͎��s�����
        /// �V���{���ݒ�̏ꍇ�ɂ�Hide��ɂ��̃t�H�[���̑������Q�Ƃ��Ă���̂�
        /// ���L�����s�ł��Ȃ��̂ł��̂��̃t�H�[�����Ăԑ��ł́A
        /// �Ō��CleraStyleItem���R�[������B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormStyleGallery_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /// <summary>
        /// �}�[�J�[�V���{���i�|�C���g�j�������[�h��
        /// �����Ώۃ}�[�J�[�V���{����ʑI�����C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxStyleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStyleType.SelectedIndex == 0)
            {
                // simple marker
                this.axSymbologyControl1.Visible = false;
                
                this.comboBoxFonts.Visible = false;
                this.labelFontName.Visible = false;
                
                this.comboBoxSimpelMarkerStyle.Visible = true;
                this.labelSimpleMarkerStyle.Visible = true;
                
                this.checkBoxSelectFromFonts.Visible = false;
                this.listViewGlyph.Visible = false;

                this.checkBoxChikeizu.Visible = false;
                this.checkBoxKoukyouSokuryou.Visible = false;
                this.checkBoxKoutsuHyoushiki.Visible = false;

            }
            else
            {
                // sonota marker symbol
                this.axSymbologyControl1.Visible = true;
                
                this.comboBoxFonts.Visible = true;
                this.labelFontName.Visible = true;
                
                this.comboBoxSimpelMarkerStyle.Visible = false;
                this.labelSimpleMarkerStyle.Visible = false;

                this.checkBoxSelectFromFonts.Visible = true;
                this.listViewGlyph.Visible = false;

                this.checkBoxChikeizu.Visible = true;
                this.checkBoxKoukyouSokuryou.Visible = true;
                this.checkBoxKoutsuHyoushiki.Visible = true;


                string selefontName = 
                    comboBoxFonts.GetItemText(comboBoxFonts.SelectedItem);
            }

            if (this.SaveMode != this.RunMode)
            {
                this.RunMode = this.SaveMode;
            }
        }

        /// <summary>
        /// �I���\�V���v���}�[�J�[��ݒ�
        /// </summary>
        private void SetSimpleMarkerSimbol()
        {
            simpleMarkers = new ISimpleMarkerSymbol[5];
            
            simpleMarkers[0] = 
                CreateSimpleMarkerSymbol(this.m_rgbColor, esriSimpleMarkerStyle.esriSMSCircle);

            simpleMarkers[1] =
                CreateSimpleMarkerSymbol(this.m_rgbColor, esriSimpleMarkerStyle.esriSMSSquare);
            
            simpleMarkers[2] =
                CreateSimpleMarkerSymbol(this.m_rgbColor, esriSimpleMarkerStyle.esriSMSCross);

            simpleMarkers[3] =
                CreateSimpleMarkerSymbol(this.m_rgbColor, esriSimpleMarkerStyle.esriSMSX);

            simpleMarkers[4] =
                CreateSimpleMarkerSymbol(this.m_rgbColor, esriSimpleMarkerStyle.esriSMSDiamond);
        }

        /// <summary>
        /// �V���v���}�[�J�[�V���{���̍쐬
        /// </summary>
        /// <param name="rgbColor"></param>
        /// <param name="inputStyle"></param>
        /// <returns></returns>
        private ESRI.ArcGIS.Display.ISimpleMarkerSymbol CreateSimpleMarkerSymbol(
            ESRI.ArcGIS.Display.IRgbColor rgbColor,
            ESRI.ArcGIS.Display.esriSimpleMarkerStyle inputStyle)
        {

            ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol =
              new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Color = rgbColor;
            simpleMarkerSymbol.Style = inputStyle;

            return simpleMarkerSymbol;
        }

        /// <summary>
        /// �I���ςݏ����ΏۃV���v���}�[�J�[�V���{���̕ύX���C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxSimpelMarkerStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelPreview.Image = Common.DrawSymbol.SymbolToImage(
                simpleMarkers[comboBoxSimpelMarkerStyle.SelectedIndex] as ISymbol, 
                this.labelPreview.Width, 
                this.labelPreview.Height);

            m_image = this.labelPreview.Image;
        }

        // �t�H���g�C���[�W�擾�������s������t���O
        private bool DoingFontImageGetting = false;
        
        /// <summary>
        /// �I���\�t�H���g�̑I���ςݏ�Ԃ̕ύX�C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DoingFontImageGetting)
                {
                    return;
                }

                this.selefontName = comboBoxFonts.GetItemText(comboBoxFonts.SelectedItem);
                if (selefontName == null || selefontName.Length == 0) return;

                //this.selectedCharacterFont = new Font(selefontName, 25.0f, FontStyle.Bold);
                this.selectedCharacterFont = new Font(selefontName, 25.0f, FontStyle.Regular);

                //if (checkBoxSelectFromFonts.Visible) SetGlyphs(selefontName);


                // �w��t�H���g����ݒ�`�F�b�N����Ă���΃t�H���g�C���[�W�擾
                if (this.checkBoxSelectFromFonts.Checked)
                {
                    DoingFontImageGetting = true;

                    this.listViewGlyph.Visible = true;
                    this.axSymbologyControl1.Visible = false;

                    this.RunMode = ModeChangedToCharacterFont;
                    SetGlyphs(this.selefontName);
                }
            }
            catch { }
            finally 
            {
                DoingFontImageGetting = false;
            }
        }

 
        /// <summary>
        /// �w��t�H���g�̃O���t���擾���ă��X�g�r���[�ɐݒ�
        /// </summary>
        /// <param name="selefontName"></param>
        private void SetGlyphs(string selefontName)
        {
            imageListGlyph.Dispose();
            listViewGlyph.Clear();
            listViewGlyph.Invalidate();

            if (selefontName == null || selefontName.Length == 0) return;

            try
            {
                System.Windows.Media.FontFamily ff = new System.Windows.Media.FontFamily(selefontName);

                IDictionary<int, ushort> characterMap = null;

                // Character Map�擾
                foreach (Typeface typeface in ff.GetTypefaces())
                {
                    //if (typeface.Weight == System.Windows.FontWeights.Normal
                    //    && typeface.Stretch == System.Windows.FontStretches.Normal)
                    //    && typeface.Style == System.Windows.FontStyles.Normal)
                    //{
                    GlyphTypeface glyph;
                    if (typeface.TryGetGlyphTypeface(out glyph))
                    {
                        characterMap = glyph.CharacterToGlyphMap;
                        break;
                    }
                    //}
                }

                ICollection<int> index = null;

                // Character Map�̃R���N�V������ݒ�
                if (characterMap.Count > 0)
                {
                    index = characterMap.Keys;
                }

                arrglyphIdx = new System.Collections.ArrayList();

                // �R���N�V������ArryList�ɐݒ�
                foreach (int i in index)
                {
                    // 65535���傫�����̂͏��O
                    if (!(i > 65535))
                    {
                        arrglyphIdx.Add(i);
                        //Common.Logger.Debug(i.ToString());
                    }
                }
                
                // AddRange�p
                ListViewItem[] fontItem = new ListViewItem[arrglyphIdx.Count];

                // �V���{���̐ݒ�
                ICharacterMarkerSymbol markerSymbol = new CharacterMarkerSymbolClass();
                markerSymbol.Font = ESRI.ArcGIS.ADF.Connection.Local.Converter.ToStdFont(selectedCharacterFont);
                markerSymbol.Size = 25.0;
                markerSymbol.Color = this.m_rgbColor;

                // listview��imagelist��ݒ�
                listViewGlyph.LargeImageList = imageListGlyph;

                // imagelist��index
                int listImageIndex = 0;

                // �`���~
                listViewGlyph.BeginUpdate();

                // image��1���쐬
                for (int idx = 0; idx < arrglyphIdx.Count; idx++)
                {
                    // ���݂���CharacterIndex�̕�,Image�擾,ImageList�ɐݒ�
                    markerSymbol.CharacterIndex = (int)arrglyphIdx[idx];

                    Image original = Common.DrawSymbol.SymbolToImage(
                        markerSymbol as ISymbol,
                        this.labelPreview.Width,
                        this.labelPreview.Height);

                    int width = 80;
                    int height = 60;

                    Image fontImage = Common.DrawSymbol.GetBitmap(original, width, height);
                    imageListGlyph.ImageSize = new Size(width, height);
                    imageListGlyph.Images.Add(fontImage);

                    fontItem[idx] = new ListViewItem("", listImageIndex);

                    listImageIndex++;

                    original.Dispose();
                    fontImage.Dispose();
                }

                // listview�ɃA�C�e����ݒ�
                listViewGlyph.Items.AddRange(fontItem);

                // �ĕ`��
                listViewGlyph.EndUpdate();

                // ���X�g�̐擪��I��
                listViewGlyph.Items[0].Selected = true;
            }
            finally
            {

            }
        }


        //   To find what character index you need for a particular font:
        //   1. Open ArcMap
        //   2. Add in any point dataset
        //   3. Double click on a point symbol in the TOC
        //   4. In the Symbol Selector dialog, click the properties button
        //   5. In the Symbol Property Editor dialog, change the Type to 'Character Marker Symbol'
        //   6. Choose the desired Font 
        //   7. Click on the different character symbols to see what Unicode index numer you need</remarks>
        /// <summary>
        /// �L�����N�^�[�}�[�J�[�V���{���̍쐬
        /// </summary>
        /// <param name="rgbColor">ESRI�J���[</param>
        /// <param name="fontSize">�t�H���g�T�C�Y</param>
        /// <param name="characterIndex">�L�����N�^�C���f�N�X</param>
        /// <returns>�L�����N�^�[�}�[�J�[�V���{��</returns>
        public ESRI.ArcGIS.Display.ICharacterMarkerSymbol CreateCharacterMarkerSymbol(
           ESRI.ArcGIS.Display.IRgbColor rgbColor,
           System.Double fontSize,
           System.Int32 characterIndex)
        {

            if (rgbColor == null)
            {
                return null;
            }

            // Define the Font we want to use
            stdole.IFontDisp stdFont = new stdole.StdFontClass() as stdole.IFontDisp; // Dynamic Cast
            stdFont.Name = selefontName;
            stdFont.Size = (System.Decimal)fontSize; // Explicit Cast

            // Set the CharacterMarkerSymbols Properties
            ESRI.ArcGIS.Display.ICharacterMarkerSymbol characterMarkerSymbol = 
                new ESRI.ArcGIS.Display.CharacterMarkerSymbolClass();
            
            characterMarkerSymbol.Angle = 0;  // 0 to 360
            characterMarkerSymbol.CharacterIndex = characterIndex;
            characterMarkerSymbol.Color =  rgbColor;
            characterMarkerSymbol.Font = stdFont;
            characterMarkerSymbol.Size = fontSize;
            characterMarkerSymbol.XOffset = 0;
            characterMarkerSymbol.YOffset = 0;

            return characterMarkerSymbol;
        }



        /// <summary>
        /// �I���\�ȃt�H���g�̃t�H���g�I����ԕύX���C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewGlyph_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                //Common.Logger.Debug("e.ItemIndex= " + e.ItemIndex.ToString());
                //Common.Logger.Debug("listViewGlyph.Items[e.ItemIndex].ImageIndex= " + listViewGlyph.Items[e.ItemIndex].ImageIndex.ToString());
                //Common.Logger.Debug("this.arrglyphIdx[e.ItemIndex]= " + this.arrglyphIdx[e.ItemIndex].ToString());

                //listViewGlyph.Items[e.ItemIndex].ImageIndex = e.ItemIndex;

                this.selectedCharacterIndex = Convert.ToInt32(this.arrglyphIdx[e.ItemIndex]);
                this.labelPreview.Image = imageListGlyph.Images[e.ItemIndex];
                
                m_image = this.labelPreview.Image;

                this.labelStyleName.Visible = true;
                this.labelStyleName.Text = "�G�����}�[�J�[�V���{��";

            }
            catch { }
        }

        /// <summary>
        /// �I���\�ȃt�H���g�g�p����`�F�b�N�{�b�N�X�̃N���b�N���C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxSelectFromFonts_Click(object sender, EventArgs e)
        {
            this.labelStyleName.Text = "";

            if (this.checkBoxSelectFromFonts.Checked)
            {
                this.listViewGlyph.Visible = true;
                this.axSymbologyControl1.Visible = false;

                this.RunMode = ModeChangedToCharacterFont;
                SetGlyphs(this.selefontName);

                this.comboBoxFonts.Enabled = true;
            }
            else
            {
                this.listViewGlyph.Visible = false;
                this.axSymbologyControl1.Visible = true;

                this.comboBoxFonts.Enabled = false;

                this.RunMode = this.SaveMode;
            }
        }

        /// <summary>
        /// �I���\�ȃV���{���ꗗ�R���g���[���̃_�u���N���b�N���C�x���g�n���h��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axSymbologyControl1_OnDoubleClick(
            object sender, ISymbologyControlEvents_OnDoubleClickEvent e)
        {
            OKProc();
        }

        /// <summary>
        /// �h���b�v�_�E�����X�g�̕�����
        /// �g�p�\�t�H���g�ꗗ�\���̃h���b�v�_�E�����Ɏg�p
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdjustWidthComboBox_DropDown(object sender, System.EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth;
            foreach (string s in ((ComboBox)sender).Items)
            {
                newWidth = (int)g.MeasureString(s, font).Width
                    + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            senderComboBox.DropDownWidth = width;
        }

        /// <summary>
        /// ���g�p
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormStyleGallery_Shown(object sender, EventArgs e)
        {
            //SW.Stop();
            //System.Diagnostics.Debug.Print(SW.ElapsedMilliseconds.ToString());
        }

		private void StyleFileLoad_CheckedChanged(object sender, EventArgs e) {
			CheckBox	ctlCBX = sender as CheckBox;

			// ���٥̧�ق̓ǂݍ��݂𐧌�
			this.LoadStyleFile(Convert.ToInt32(ctlCBX.Tag), ctlCBX.Checked);
		}
		
		/// <summary>
		/// �X�^�C���t�@�C���̃p�X���擾���܂�
		/// </summary>
		/// <param name="Index">�t�@�C���E�C���f�b�N�X</param>
		/// <returns></returns>
		public string GetStyleFilePath(int Index) {
			string	strRet = null;

			// ��������
			if(!string.IsNullOrWhiteSpace(this.m_sInstall) && (Index >= 0 && Index < this.StyleFileNum)) {
				string strFName = StyleFileNames[Index];
				if(System.IO.File.Exists(m_sInstall + @"Styles\ja\" + strFName)) {
					strRet = m_sInstall + @"Styles\ja\" + strFName;
				}
				else if(System.IO.File.Exists(m_sInstall + @"Styles\" + strFName)) {
					strRet = m_sInstall + @"Styles\" + strFName;
				}
			}

			return strRet;
		}

		private void LoadStyleFile(int StyleFileIndex, bool IsLoad) {
			// ���٥̧�ق����
			string	strFName = this.GetStyleFilePath(StyleFileIndex);
			if(!string.IsNullOrEmpty(strFName)) {
				try {
					if(IsLoad) {
						this.axSymbologyControl1.LoadStyleFile(strFName);
					}
					else {
						this.axSymbologyControl1.RemoveFile(strFName);
					}
				}
				catch(Exception ex) {
					Common.UtilityClass.DoOnError("FormStyleGallery �X�^�C���t�@�C����" + (IsLoad ? "���[�h" : "�A�����[�h") + "�Ɏ��s���܂��� [" + strFName + "]", ex);
				}
			}
		}
	}
}
