namespace ESRIJapan.GISLight10.Ui
{
    partial class MainForm
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
            //Ensures that any ESRI libraries that have been used are unloaded in the correct order. 
            //Failure to do this may result in random crashes on exit due to the operating system unloading 
            //the libraries in the incorrect order. 
            ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolMenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDataAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReadCAD = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddXYData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExportMap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuExitApp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuSentaku = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSentakuZokukeiKensaku = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSentakuKukanKensaku = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSentakuZokuseichiSyukei = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSentakuSelectableLayerSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuTableJoinRelate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTableJoin = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveJoin = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuRelate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveRelate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuCalculate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGeometryCalculate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFieldCalculate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuIntersect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGeoReference = new System.Windows.Forms.ToolStripMenuItem();
            this.toolAddInMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuOptionSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuVersionInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlbtnTocClose = new System.Windows.Forms.ToolStripSplitButton();
            this.tlbtnTocExpand = new System.Windows.Forms.ToolStripSplitButton();
            this.statusBarXY = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.axLicenseControl1 = new ESRI.ArcGIS.Controls.AxLicenseControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.MapContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.axTOCControl1 = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lightCatalogView1 = new ESRIJapan.GISLight10.Ui.LightCatalogView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.axMapControl1 = new ESRI.ArcGIS.Controls.AxMapControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.axPageLayoutControl1 = new ESRI.ArcGIS.Controls.AxPageLayoutControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelPageOrientation = new System.Windows.Forms.Label();
            this.labelPageSize = new System.Windows.Forms.Label();
            this.axToolbarControl4 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.axToolbarControl2 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.axToolbarControl5 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.axToolbarControl1 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapContainer)).BeginInit();
            this.MapContainer.Panel1.SuspendLayout();
            this.MapContainer.Panel2.SuspendLayout();
            this.MapContainer.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMenuFile,
            this.toolMenuSentaku,
            this.toolMenuTableJoinRelate,
            this.toolMenuCalculate,
            this.toolAddInMenuItem,
            this.toolMenuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1445, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolMenuFile
            // 
            this.toolMenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNewDoc,
            this.menuOpenDoc,
            this.menuSaveDoc,
            this.menuSaveAs,
            this.toolStripSeparator2,
            this.menuDataAdd,
            this.menuExportMap,
            this.menuSeparator,
            this.menuExitApp});
            this.toolMenuFile.Name = "toolMenuFile";
            this.toolMenuFile.Size = new System.Drawing.Size(63, 24);
            this.toolMenuFile.Text = "ファイル";
            // 
            // menuNewDoc
            // 
            this.menuNewDoc.Image = ((System.Drawing.Image)(resources.GetObject("menuNewDoc.Image")));
            this.menuNewDoc.ImageTransparentColor = System.Drawing.Color.White;
            this.menuNewDoc.Name = "menuNewDoc";
            this.menuNewDoc.Size = new System.Drawing.Size(194, 26);
            this.menuNewDoc.Text = "新規作成";
            this.menuNewDoc.Click += new System.EventHandler(this.menuNewDoc_Click);
            // 
            // menuOpenDoc
            // 
            this.menuOpenDoc.Image = ((System.Drawing.Image)(resources.GetObject("menuOpenDoc.Image")));
            this.menuOpenDoc.ImageTransparentColor = System.Drawing.Color.White;
            this.menuOpenDoc.Name = "menuOpenDoc";
            this.menuOpenDoc.Size = new System.Drawing.Size(194, 26);
            this.menuOpenDoc.Text = "開く";
            this.menuOpenDoc.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuSaveDoc
            // 
            this.menuSaveDoc.Image = ((System.Drawing.Image)(resources.GetObject("menuSaveDoc.Image")));
            this.menuSaveDoc.ImageTransparentColor = System.Drawing.Color.White;
            this.menuSaveDoc.Name = "menuSaveDoc";
            this.menuSaveDoc.Size = new System.Drawing.Size(194, 26);
            this.menuSaveDoc.Text = "上書き保存";
            this.menuSaveDoc.Click += new System.EventHandler(this.menuSaveDoc_Click);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.Size = new System.Drawing.Size(194, 26);
            this.menuSaveAs.Text = "名前を付けて保存";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(191, 6);
            // 
            // menuDataAdd
            // 
            this.menuDataAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddData,
            this.menuReadCAD,
            this.menuAddXYData});
            this.menuDataAdd.Image = global::ESRIJapan.GISLight10.Properties.Resources.menuDataAdd;
            this.menuDataAdd.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.menuDataAdd.Name = "menuDataAdd";
            this.menuDataAdd.Size = new System.Drawing.Size(194, 26);
            this.menuDataAdd.Text = "データの追加 (&A)";
            // 
            // menuAddData
            // 
            this.menuAddData.Image = ((System.Drawing.Image)(resources.GetObject("menuAddData.Image")));
            this.menuAddData.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.menuAddData.Name = "menuAddData";
            this.menuAddData.Size = new System.Drawing.Size(203, 26);
            this.menuAddData.Text = "ArcGISデータの追加";
            this.menuAddData.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuReadCAD
            // 
            this.menuReadCAD.Image = global::ESRIJapan.GISLight10.Properties.Resources.AddCadData;
            this.menuReadCAD.Name = "menuReadCAD";
            this.menuReadCAD.Size = new System.Drawing.Size(203, 26);
            this.menuReadCAD.Text = "CADデータの追加";
            this.menuReadCAD.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuAddXYData
            // 
            this.menuAddXYData.Image = global::ESRIJapan.GISLight10.Properties.Resources.AddXYData;
            this.menuAddXYData.Name = "menuAddXYData";
            this.menuAddXYData.Size = new System.Drawing.Size(203, 26);
            this.menuAddXYData.Text = "XYデータの追加";
            this.menuAddXYData.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuExportMap
            // 
            this.menuExportMap.Image = global::ESRIJapan.GISLight10.Properties.Resources.MapExport;
            this.menuExportMap.Name = "menuExportMap";
            this.menuExportMap.Size = new System.Drawing.Size(194, 26);
            this.menuExportMap.Text = "マップのエクスポート";
            this.menuExportMap.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuSeparator
            // 
            this.menuSeparator.Name = "menuSeparator";
            this.menuSeparator.Size = new System.Drawing.Size(191, 6);
            // 
            // menuExitApp
            // 
            this.menuExitApp.Image = global::ESRIJapan.GISLight10.Properties.Resources.Close;
            this.menuExitApp.Name = "menuExitApp";
            this.menuExitApp.Size = new System.Drawing.Size(194, 26);
            this.menuExitApp.Text = "終了";
            this.menuExitApp.Click += new System.EventHandler(this.menuExitApp_Click);
            // 
            // toolMenuSentaku
            // 
            this.toolMenuSentaku.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSentakuZokukeiKensaku,
            this.menuSentakuKukanKensaku,
            this.toolStripSeparator4,
            this.menuSentakuZokuseichiSyukei,
            this.toolStripSeparator5,
            this.menuSentakuSelectableLayerSettings});
            this.toolMenuSentaku.Name = "toolMenuSentaku";
            this.toolMenuSentaku.Size = new System.Drawing.Size(51, 24);
            this.toolMenuSentaku.Text = "選択";
            this.toolMenuSentaku.DropDownOpening += new System.EventHandler(this.toolMenuSentaku_DropDownOpening);
            // 
            // menuSentakuZokukeiKensaku
            // 
            this.menuSentakuZokukeiKensaku.Image = global::ESRIJapan.GISLight10.Properties.Resources.AttrSearch;
            this.menuSentakuZokukeiKensaku.Name = "menuSentakuZokukeiKensaku";
            this.menuSentakuZokukeiKensaku.Size = new System.Drawing.Size(220, 26);
            this.menuSentakuZokukeiKensaku.Text = "属性検索";
            this.menuSentakuZokukeiKensaku.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuSentakuKukanKensaku
            // 
            this.menuSentakuKukanKensaku.Image = global::ESRIJapan.GISLight10.Properties.Resources.SpetialSearch;
            this.menuSentakuKukanKensaku.Name = "menuSentakuKukanKensaku";
            this.menuSentakuKukanKensaku.Size = new System.Drawing.Size(220, 26);
            this.menuSentakuKukanKensaku.Text = "空間検索";
            this.menuSentakuKukanKensaku.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(217, 6);
            // 
            // menuSentakuZokuseichiSyukei
            // 
            this.menuSentakuZokuseichiSyukei.Image = global::ESRIJapan.GISLight10.Properties.Resources.AtttrTotal;
            this.menuSentakuZokuseichiSyukei.Name = "menuSentakuZokuseichiSyukei";
            this.menuSentakuZokuseichiSyukei.Size = new System.Drawing.Size(220, 26);
            this.menuSentakuZokuseichiSyukei.Text = "属性値集計";
            this.menuSentakuZokuseichiSyukei.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(217, 6);
            // 
            // menuSentakuSelectableLayerSettings
            // 
            this.menuSentakuSelectableLayerSettings.Image = global::ESRIJapan.GISLight10.Properties.Resources.SelectLayer;
            this.menuSentakuSelectableLayerSettings.Name = "menuSentakuSelectableLayerSettings";
            this.menuSentakuSelectableLayerSettings.Size = new System.Drawing.Size(220, 26);
            this.menuSentakuSelectableLayerSettings.Text = "選択可能レイヤの設定";
            this.menuSentakuSelectableLayerSettings.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolMenuTableJoinRelate
            // 
            this.toolMenuTableJoinRelate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTableJoin,
            this.menuRemoveJoin,
            this.toolStripSeparator3,
            this.menuRelate,
            this.menuRemoveRelate});
            this.toolMenuTableJoinRelate.Name = "toolMenuTableJoinRelate";
            this.toolMenuTableJoinRelate.Size = new System.Drawing.Size(148, 24);
            this.toolMenuTableJoinRelate.Text = "テーブル結合とリレート";
            this.toolMenuTableJoinRelate.DropDownOpening += new System.EventHandler(this.toolMenuTableJoinRelate_DropDownOpening);
            // 
            // menuTableJoin
            // 
            this.menuTableJoin.Image = global::ESRIJapan.GISLight10.Properties.Resources.AttrJoin;
            this.menuTableJoin.Name = "menuTableJoin";
            this.menuTableJoin.Size = new System.Drawing.Size(200, 26);
            this.menuTableJoin.Text = "テーブル結合";
            this.menuTableJoin.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuRemoveJoin
            // 
            this.menuRemoveJoin.Image = global::ESRIJapan.GISLight10.Properties.Resources.AttrJoinCancel;
            this.menuRemoveJoin.Name = "menuRemoveJoin";
            this.menuRemoveJoin.Size = new System.Drawing.Size(200, 26);
            this.menuRemoveJoin.Text = "テーブル結合の解除";
            this.menuRemoveJoin.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(197, 6);
            // 
            // menuRelate
            // 
            this.menuRelate.Image = global::ESRIJapan.GISLight10.Properties.Resources.AttrRelate;
            this.menuRelate.Name = "menuRelate";
            this.menuRelate.Size = new System.Drawing.Size(200, 26);
            this.menuRelate.Text = "リレート";
            this.menuRelate.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuRemoveRelate
            // 
            this.menuRemoveRelate.Image = global::ESRIJapan.GISLight10.Properties.Resources.AttrRelateCancel;
            this.menuRemoveRelate.Name = "menuRemoveRelate";
            this.menuRemoveRelate.Size = new System.Drawing.Size(200, 26);
            this.menuRemoveRelate.Text = "リレートの解除";
            this.menuRemoveRelate.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolMenuCalculate
            // 
            this.toolMenuCalculate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGeometryCalculate,
            this.menuFieldCalculate,
            this.toolStripSeparator1,
            this.menuIntersect,
            this.menuGeoReference});
            this.toolMenuCalculate.Name = "toolMenuCalculate";
            this.toolMenuCalculate.Size = new System.Drawing.Size(92, 24);
            this.toolMenuCalculate.Text = "演算と解析";
            this.toolMenuCalculate.DropDownOpening += new System.EventHandler(this.toolMenuCalculate_DropDownOpening);
            // 
            // menuGeometryCalculate
            // 
            this.menuGeometryCalculate.Image = global::ESRIJapan.GISLight10.Properties.Resources.MeasureTool16;
            this.menuGeometryCalculate.Name = "menuGeometryCalculate";
            this.menuGeometryCalculate.Size = new System.Drawing.Size(177, 26);
            this.menuGeometryCalculate.Text = "面積・長さ計算";
            this.menuGeometryCalculate.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuFieldCalculate
            // 
            this.menuFieldCalculate.Image = global::ESRIJapan.GISLight10.Properties.Resources.FieldCalculate;
            this.menuFieldCalculate.Name = "menuFieldCalculate";
            this.menuFieldCalculate.Size = new System.Drawing.Size(177, 26);
            this.menuFieldCalculate.Text = "フィールド演算";
            this.menuFieldCalculate.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // menuIntersect
            // 
            this.menuIntersect.Image = global::ESRIJapan.GISLight10.Properties.Resources.Intersect;
            this.menuIntersect.Name = "menuIntersect";
            this.menuIntersect.Size = new System.Drawing.Size(177, 26);
            this.menuIntersect.Text = "インターセクト";
            this.menuIntersect.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // menuGeoReference
            // 
            this.menuGeoReference.Image = global::ESRIJapan.GISLight10.Properties.Resources.Georeference;
            this.menuGeoReference.Name = "menuGeoReference";
            this.menuGeoReference.Size = new System.Drawing.Size(177, 26);
            this.menuGeoReference.Text = "ジオリファレンス";
            this.menuGeoReference.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolAddInMenuItem
            // 
            this.toolAddInMenuItem.Name = "toolAddInMenuItem";
            this.toolAddInMenuItem.Size = new System.Drawing.Size(12, 24);
            // 
            // toolMenuHelp
            // 
            this.toolMenuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMenuOptionSettings,
            this.toolMenuVersionInfo});
            this.toolMenuHelp.Name = "toolMenuHelp";
            this.toolMenuHelp.Size = new System.Drawing.Size(75, 24);
            this.toolMenuHelp.Text = "オプション";
            // 
            // toolMenuOptionSettings
            // 
            this.toolMenuOptionSettings.Image = global::ESRIJapan.GISLight10.Properties.Resources.Option;
            this.toolMenuOptionSettings.Name = "toolMenuOptionSettings";
            this.toolMenuOptionSettings.Size = new System.Drawing.Size(168, 26);
            this.toolMenuOptionSettings.Text = "オプション設定";
            this.toolMenuOptionSettings.Click += new System.EventHandler(this.common_menuItemCommand_Click);
            // 
            // toolMenuVersionInfo
            // 
            this.toolMenuVersionInfo.Image = global::ESRIJapan.GISLight10.Properties.Resources.Version;
            this.toolMenuVersionInfo.Name = "toolMenuVersionInfo";
            this.toolMenuVersionInfo.Size = new System.Drawing.Size(168, 26);
            this.toolMenuVersionInfo.Text = "バージョン情報";
            this.toolMenuVersionInfo.Click += new System.EventHandler(this.toolMenuVersionInfo_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tlbtnTocClose,
            this.tlbtnTocExpand,
            this.statusBarXY});
            this.statusStrip1.Location = new System.Drawing.Point(0, 802);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(1445, 26);
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusBar1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 21);
            // 
            // tlbtnTocClose
            // 
            this.tlbtnTocClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tlbtnTocClose.Image = global::ESRIJapan.GISLight10.Properties.Resources.TOC_Collapsed;
            this.tlbtnTocClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlbtnTocClose.Name = "tlbtnTocClose";
            this.tlbtnTocClose.Size = new System.Drawing.Size(39, 24);
            this.tlbtnTocClose.ToolTipText = "コンテンツを非表示にします";
            this.tlbtnTocClose.ButtonClick += new System.EventHandler(this.tlbtnTocClose_ButtonClick);
            // 
            // tlbtnTocExpand
            // 
            this.tlbtnTocExpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tlbtnTocExpand.Image = global::ESRIJapan.GISLight10.Properties.Resources.TOC_Expand;
            this.tlbtnTocExpand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlbtnTocExpand.Name = "tlbtnTocExpand";
            this.tlbtnTocExpand.Size = new System.Drawing.Size(39, 24);
            this.tlbtnTocExpand.ToolTipText = "コンテンツを表示します";
            this.tlbtnTocExpand.Visible = false;
            this.tlbtnTocExpand.Click += new System.EventHandler(this.tlbtnTocExpand_Click);
            // 
            // statusBarXY
            // 
            this.statusBarXY.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.statusBarXY.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusBarXY.Name = "statusBarXY";
            this.statusBarXY.Size = new System.Drawing.Size(64, 21);
            this.statusBarXY.Text = "GISLight";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(668, 799);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(213, 16);
            this.progressBar1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(880, 682);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 16);
            this.label1.TabIndex = 12;
            // 
            // axLicenseControl1
            // 
            this.axLicenseControl1.Enabled = true;
            this.axLicenseControl1.Location = new System.Drawing.Point(830, 12);
            this.axLicenseControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axLicenseControl1.Name = "axLicenseControl1";
            this.axLicenseControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLicenseControl1.OcxState")));
            this.axLicenseControl1.Size = new System.Drawing.Size(32, 32);
            this.axLicenseControl1.TabIndex = 6;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Refresh.bmp");
            // 
            // MapContainer
            // 
            this.MapContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapContainer.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.MapContainer.Location = new System.Drawing.Point(0, 110);
            this.MapContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MapContainer.Name = "MapContainer";
            // 
            // MapContainer.Panel1
            // 
            this.MapContainer.Panel1.Controls.Add(this.tabControl2);
            // 
            // MapContainer.Panel2
            // 
            this.MapContainer.Panel2.Controls.Add(this.tabControl1);
            this.MapContainer.Size = new System.Drawing.Size(1445, 681);
            this.MapContainer.SplitterDistance = 429;
            this.MapContainer.SplitterWidth = 5;
            this.MapContainer.TabIndex = 13;
            // 
            // tabControl2
            // 
            this.tabControl2.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl2.Multiline = true;
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(431, 681);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Controls.Add(this.axTOCControl1);
            this.tabPage3.Location = new System.Drawing.Point(26, 4);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Size = new System.Drawing.Size(401, 673);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "コンテンツ";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // axTOCControl1
            // 
            this.axTOCControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axTOCControl1.Location = new System.Drawing.Point(0, 0);
            this.axTOCControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axTOCControl1.Name = "axTOCControl1";
            this.axTOCControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            this.axTOCControl1.Size = new System.Drawing.Size(461, 839);
            this.axTOCControl1.TabIndex = 3;
            this.axTOCControl1.OnMouseDown += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseDownEventHandler(this.axTOCControl1_OnMouseDown);
            this.axTOCControl1.OnMouseUp += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseUpEventHandler(this.axTOCControl1_OnMouseUp);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lightCatalogView1);
            this.tabPage4.Location = new System.Drawing.Point(26, 4);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Size = new System.Drawing.Size(401, 673);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "カタログ";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lightCatalogView1
            // 
            this.lightCatalogView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lightCatalogView1.AutoSize = true;
            this.lightCatalogView1.Location = new System.Drawing.Point(4, 4);
            this.lightCatalogView1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lightCatalogView1.Name = "lightCatalogView1";
            this.lightCatalogView1.ShowConnectFolder = true;
            this.lightCatalogView1.ShowDBConnect = false;
            this.lightCatalogView1.ShowGISServer = false;
            this.lightCatalogView1.Size = new System.Drawing.Size(385, 660);
            this.lightCatalogView1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1011, 681);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.axMapControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(1003, 652);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "マップ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // axMapControl1
            // 
            this.axMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axMapControl1.Location = new System.Drawing.Point(0, 0);
            this.axMapControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axMapControl1.Name = "axMapControl1";
            this.axMapControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControl1.OcxState")));
            this.axMapControl1.Size = new System.Drawing.Size(1171, 811);
            this.axMapControl1.TabIndex = 5;
            this.axMapControl1.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(this.axMapControl1_OnMouseDown);
            this.axMapControl1.OnMouseMove += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseMoveEventHandler(this.axMapControl1_OnMouseMove);
            this.axMapControl1.OnAfterDraw += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnAfterDrawEventHandler(this.axMapControl1_OnAfterDraw);
            this.axMapControl1.OnOleDrop += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnOleDropEventHandler(this.axMapControl1_OnOleDrop);
            this.axMapControl1.OnMapReplaced += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMapReplacedEventHandler(this.axMapControl1_OnMapReplaced);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.axPageLayoutControl1);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(1003, 652);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "レイアウト";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // axPageLayoutControl1
            // 
            this.axPageLayoutControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axPageLayoutControl1.Location = new System.Drawing.Point(0, 46);
            this.axPageLayoutControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axPageLayoutControl1.Name = "axPageLayoutControl1";
            this.axPageLayoutControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPageLayoutControl1.OcxState")));
            this.axPageLayoutControl1.Size = new System.Drawing.Size(1167, 739);
            this.axPageLayoutControl1.TabIndex = 6;
            this.axPageLayoutControl1.OnMouseDown += new ESRI.ArcGIS.Controls.IPageLayoutControlEvents_Ax_OnMouseDownEventHandler(this.axPageLayoutControl1_OnMouseDown);
            this.axPageLayoutControl1.OnMouseMove += new ESRI.ArcGIS.Controls.IPageLayoutControlEvents_Ax_OnMouseMoveEventHandler(this.axPageLayoutControl1_OnMouseMove);
            this.axPageLayoutControl1.OnDoubleClick += new ESRI.ArcGIS.Controls.IPageLayoutControlEvents_Ax_OnDoubleClickEventHandler(this.axPageLayoutControl1_OnDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelPageOrientation);
            this.panel1.Controls.Add(this.labelPageSize);
            this.panel1.Controls.Add(this.axToolbarControl4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(995, 46);
            this.panel1.TabIndex = 1;
            // 
            // labelPageOrientation
            // 
            this.labelPageOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPageOrientation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelPageOrientation.Location = new System.Drawing.Point(958, 14);
            this.labelPageOrientation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPageOrientation.Name = "labelPageOrientation";
            this.labelPageOrientation.Size = new System.Drawing.Size(33, 20);
            this.labelPageOrientation.TabIndex = 13;
            this.labelPageOrientation.Text = "向き";
            // 
            // labelPageSize
            // 
            this.labelPageSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPageSize.BackColor = System.Drawing.Color.Transparent;
            this.labelPageSize.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelPageSize.Location = new System.Drawing.Point(914, 14);
            this.labelPageSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPageSize.Name = "labelPageSize";
            this.labelPageSize.Size = new System.Drawing.Size(36, 20);
            this.labelPageSize.TabIndex = 12;
            this.labelPageSize.Text = "サイズ";
            // 
            // axToolbarControl4
            // 
            this.axToolbarControl4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axToolbarControl4.Location = new System.Drawing.Point(3, 3);
            this.axToolbarControl4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axToolbarControl4.Name = "axToolbarControl4";
            this.axToolbarControl4.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl4.OcxState")));
            this.axToolbarControl4.Size = new System.Drawing.Size(1165, 28);
            this.axToolbarControl4.TabIndex = 7;
            this.axToolbarControl4.OnItemClick += new ESRI.ArcGIS.Controls.IToolbarControlEvents_Ax_OnItemClickEventHandler(this.axToolbarControl4_OnItemClick);
            // 
            // axToolbarControl2
            // 
            this.axToolbarControl2.Location = new System.Drawing.Point(0, 53);
            this.axToolbarControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axToolbarControl2.Name = "axToolbarControl2";
            this.axToolbarControl2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl2.OcxState")));
            this.axToolbarControl2.Size = new System.Drawing.Size(1406, 28);
            this.axToolbarControl2.TabIndex = 16;
            // 
            // axToolbarControl5
            // 
            this.axToolbarControl5.Location = new System.Drawing.Point(906, 53);
            this.axToolbarControl5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axToolbarControl5.Name = "axToolbarControl5";
            this.axToolbarControl5.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl5.OcxState")));
            this.axToolbarControl5.Size = new System.Drawing.Size(279, 28);
            this.axToolbarControl5.TabIndex = 17;
            // 
            // axToolbarControl1
            // 
            this.axToolbarControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.axToolbarControl1.Location = new System.Drawing.Point(0, 28);
            this.axToolbarControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axToolbarControl1.Name = "axToolbarControl1";
            this.axToolbarControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl1.OcxState")));
            this.axToolbarControl1.Size = new System.Drawing.Size(1445, 28);
            this.axToolbarControl1.TabIndex = 18;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1445, 828);
            this.Controls.Add(this.axToolbarControl1);
            this.Controls.Add(this.axToolbarControl5);
            this.Controls.Add(this.axToolbarControl2);
            this.Controls.Add(this.MapContainer);
            this.Controls.Add(this.axLicenseControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "GISLight";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).EndInit();
            this.MapContainer.Panel1.ResumeLayout(false);
            this.MapContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MapContainer)).EndInit();
            this.MapContainer.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolMenuFile;
        private System.Windows.Forms.ToolStripMenuItem menuNewDoc;
        private System.Windows.Forms.ToolStripMenuItem menuOpenDoc;
        private System.Windows.Forms.ToolStripMenuItem menuSaveDoc;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem menuExitApp;
        private System.Windows.Forms.ToolStripSeparator menuSeparator;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusBarXY;
        private System.Windows.Forms.ToolStripMenuItem toolMenuSentaku;
        private System.Windows.Forms.ToolStripMenuItem menuSentakuZokukeiKensaku;
        private System.Windows.Forms.ToolStripMenuItem toolMenuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuSentakuKukanKensaku;
		private System.Windows.Forms.ToolStripMenuItem menuSentakuZokuseichiSyukei;
        private System.Windows.Forms.ToolStripMenuItem toolMenuTableJoinRelate;
        private System.Windows.Forms.ToolStripMenuItem menuTableJoin;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveJoin;
        private System.Windows.Forms.ToolStripMenuItem menuRelate;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveRelate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuExportMap;
        private System.Windows.Forms.ToolStripMenuItem toolAddInMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolMenuVersionInfo;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        /// <summary>
        /// マップコントロール
        /// </summary>
        internal ESRI.ArcGIS.Controls.AxMapControl axMapControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl4;
        /// <summary>
        /// ページレイアウトコントロール
        /// </summary>
        internal ESRI.ArcGIS.Controls.AxPageLayoutControl axPageLayoutControl1;
        private System.Windows.Forms.ToolStripMenuItem toolMenuOptionSettings;
        
        /// <summary>
        /// 印刷用紙サイズを示すキャプション
        /// </summary>
        internal System.Windows.Forms.Label labelPageSize;
        /// <summary>
        /// 印刷用紙向きを示すキャプション
        /// </summary>
        internal System.Windows.Forms.Label labelPageOrientation;
        /// <summary>
        /// TOCコントロール
        /// </summary>
        internal ESRI.ArcGIS.Controls.AxTOCControl axTOCControl1;
        private System.Windows.Forms.ToolStripMenuItem menuSentakuSelectableLayerSettings;
        /// <summary>
        /// タブコントロール
        /// </summary>
        internal System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem toolMenuCalculate;
        private System.Windows.Forms.ToolStripMenuItem menuGeometryCalculate;
        private ESRI.ArcGIS.Controls.AxLicenseControl axLicenseControl1;
        private System.Windows.Forms.SplitContainer MapContainer;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ToolStripSplitButton tlbtnTocClose;
        private System.Windows.Forms.ToolStripSplitButton tlbtnTocExpand;
        private System.Windows.Forms.ToolStripMenuItem menuFieldCalculate;
        private System.Windows.Forms.ToolStripMenuItem menuIntersect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private LightCatalogView lightCatalogView1;
		private System.Windows.Forms.ToolStripMenuItem menuGeoReference;
		private System.Windows.Forms.ToolStripMenuItem menuDataAdd;
		private System.Windows.Forms.ToolStripMenuItem menuReadCAD;
        private System.Windows.Forms.ToolStripMenuItem menuAddData;
		private System.Windows.Forms.ToolStripMenuItem menuAddXYData;
		private System.Windows.Forms.ToolTip toolTip1;
		private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl2;
		private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl5;
		private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl1;
    }
}

