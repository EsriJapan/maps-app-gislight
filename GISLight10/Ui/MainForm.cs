#region USING
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Xml;
using System.Drawing.Printing;
using System.Timers;
using System.Linq;
using System.Text;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;


using ESRIJapan.GISLight10.Properties;
using ESRIJapan.GISLight10.Common;
using ESRIJapan.GISLight10.EngineEditTask;
#endregion

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ESRIJapanGISLightのメインフォーム
    /// </summary>
    ///
    /// <history>
    ///  2010/11/01 新規作成 
    ///  2011/01/07 マップ上マウスホイール動作不安定対応(サブウィンドウ表示時未対応) 
    ///  2011/01/21 xmlコメント見直し:記述対象スコープはpublic,protected 
    ///  2011/01/25 xmlコメント記述対象スコープにinternalも追加 
    ///  2011/01/25 xmlコメントreturnsタグ記述漏れの対応 
    ///  2011/04/12 ページレイアウトに画像取り込みコマンドを追加
    ///  2011/08/05 ArcGIS10環境で下記を実行すると画面崩れるのでGetWindowLongなどコメントアウト
    /// </history>
    public sealed partial class MainForm : Form
    {
        #region WinAPI

        #region 2019/02/21 テスト用
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="childAfter"></param>
        /// <param name="className"></param>
        /// <param name="windowTitle"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);
        #endregion

        /// <summary>
        /// 印刷デバイス情報取得
        /// </summary>
        /// <param name="hdc">デバイスコンテキストのハンドル</param>
        /// <param name="nIndex">取得情報の項目種類</param>
        /// <returns>項目種類に対応したデバイス情報</returns>
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);

        /// <summary>
        /// 指定デバイスのデバイスコンテキスト作成
        /// </summary>
        /// <param name="strDriver">ドライバ名</param>
        /// <param name="strDevice">デバイス名</param>
        /// <param name="strOutput">未使用、NULL</param>
        /// <param name="pData">オプションのプリンタデータ</param>
        /// <returns>
        /// <br>成功時：指定デバイスに関連するデバイスコンテキストのハンドル</br>
        /// <br>失敗時：NULL</br>
        /// </returns>
        [DllImport("GDI32.dll")]
        public static extern int CreateDC(
            string strDriver, string strDevice, string strOutput, IntPtr pData);

        /// <summary>
        /// デバイスコンテキストの解放
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="hDC">デバイスコンテキストのハンドル</param>
        /// <returns>
        /// <br>デバイスコンテキストが解放されたとき：1</br>
        /// <br>デバイスコンテキストが解放されないとき：0</br>
        /// </returns>
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);

        /// <summary>
        /// ウインドウカスタマイズ
        /// </summary>
        /// <param name="h">ウィンドウハンドル</param>
        /// <param name="s">タイトルテキストまたはコントロールテキスト</param>
        [DllImport("User32.Dll")]
        public static extern void SetWindowText(int h, String s);

        /// <summary>
        /// ダイアログボックス内のコントロールのタイトルまたはテキストを設定
        /// </summary>
        /// <param name="hWnd">ダイアログボックスのハンドル</param>
        /// <param name="nIDDlgItem">コントロールの識別子</param>
        /// <param name="lpString">設定したいテキスト</param>
        /// <returns>
        /// <br>成功時：0 以外の値</br>
        /// <br>失敗時：0</br>
        /// </returns>
        [DllImport("user32.dll")]
        private static extern bool SetDlgItemText(IntPtr hWnd, int nIDDlgItem, string lpString);

        /// <summary>
        /// ウィンドウがアクティブ時または非アクティブ時に
        /// ウィンドウへ送信されるメッセージ
        /// </summary>
        public const int WM_ACTIVATE = 0x0006;

        /// <summary>
        /// ツールバーの透過設定用
        /// </summary>
        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hWnd, int Index);

        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int SetWindowLong(IntPtr hWnd, int Index, int Value);

        const int GWL_STYLE = -16;
        const int WS_CLIPCHILDREN = 0x02000000;

        // マウスホイール動作不安定対応 -->
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const int WM_MOUSEWHEEL = 0x20A;
        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;

        #endregion

        /// <summary>
        /// ウィンドウへ送信されるメッセージ変更チェックフラグ
        /// </summary>
        internal bool setPrintDialogTitle = false;

        /// <summary>
        /// 印刷設定、印刷ダイアログタイトル
        /// </summary>
        internal string printDialogTitle = "";

        /// <summary>
        /// SetDlgItemTextで使用する
        /// コントロールの識別子
        /// </summary>
        public const int ID_BUT_OK = 1;

        /// <summary>
        /// 印刷設定、印刷ダイアログタイトル変更フラグ
        /// </summary>
        internal bool setPrintDialogButton = false;

        /// <summary>
        /// 印刷設定、印刷ダイアログOKボタンキャプション
        /// </summary>
        internal string printDialogButton = "";


        /// <summary>
        /// Windowsメッセージを処理
        /// </summary>
        /// <param name="m">処理対象のWindows Message</param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // メッセージチェック
            if (setPrintDialogTitle && (m.Msg == WM_ACTIVATE))
            {
                // 変更フラグ
                setPrintDialogTitle = false;

                // タイトル変更
                SetWindowText((int)m.LParam, printDialogTitle);
            }

            // メッセージチェック
            if (setPrintDialogButton && (m.Msg == WM_ACTIVATE))
            {
                // 変更フラグ
                setPrintDialogButton = false;

                // タイトル変更
                SetDlgItemText(m.LParam, ID_BUT_OK, printDialogButton);
            }

            // マウスホイール動作不安定対応案 -->
            if (m.Msg == WM_MOUSEWHEEL)
            {
                this.axMapControl1.Select();
                return;
            } //<--

            base.WndProc(ref m);
        }

        #region Declare Event Handlers

        // Notes:
        // Variables are prefixed with an 'm_' denoting that they are member variables.
        // This means they are global in scope for this class.
        //private ESRI.ArcGIS.Carto.IActiveViewEvents_AfterDrawEventHandler m_ActiveViewEventsAfterDraw;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_AfterItemDrawEventHandler m_ActiveViewEventsAfterItemDraw;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ContentsChangedEventHandler m_ActiveViewEventsContentsChanged;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ContentsClearedEventHandler m_ActiveViewEventsContentsCleared;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_FocusMapChangedEventHandler m_ActiveViewEventsFocusMapChanged;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ItemAddedEventHandler m_ActiveViewEventsItemAdded;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ItemDeletedEventHandler m_ActiveViewEventsItemDeleted;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ItemReorderedEventHandler m_ActiveViewEventsItemReordered;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_SelectionChangedEventHandler m_ActiveViewEventsSelectionChanged;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_SpatialReferenceChangedEventHandler m_ActiveViewEventsSpatialReferenceChanged;
        //private ESRI.ArcGIS.Carto.IActiveViewEvents_ViewRefreshedEventHandler m_ActiveViewEventsViewRefreshed;
        // ページレイアウトイベント
        private IPageLayoutControlEvents_OnViewRefreshedEventHandler m_PageLayoutControlEventsOnViewRefreshed;
        private IPageLayoutControlEvents_OnFocusMapChangedEventHandler m_PageLayoutControlEventsOnFocusMapChanged;
        private IPageLayoutControlEvents_OnExtentUpdatedEventHandler m_PageLayoutControlEventsOnExtentUpdated;

        #endregion Declare Event Handlers


        #region print concern
        //declare the dialogs for print preview, print dialog, page setup
        /// <summary>
        /// プリントプレビューダイアログ
        /// </summary>
        internal PrintPreviewDialog printPreviewDialog1;

        /// <summary>
        /// プリントダイアログ
        /// </summary>
        internal PrintDialog printDialog1;

        /// <summary>
        /// プリンタ設定ダイアログ
        /// </summary>
        internal PageSetupDialog pageSetupDialog1;

        /// <summary>
        /// 用紙サイズキャプション
        /// </summary>
        internal string[] supportPaperSizeName = { "A3", "A4" };
        /// <summary>
        /// 用紙サイズ変更可能フラグ
        /// </summary>
        internal bool[] supportPaperSizeEnable = { false, false };
        /// <summary>
        /// A4用紙サイズキャプションインデクス
        /// </summary>
        internal int A3_Index = 0;
        /// <summary>
        /// A3用紙サイズキャプションインデクス
        /// </summary>
        internal int A4_Index = 1;
        /// <summary>
        /// 用紙横方向キャプション
        /// </summary>
        internal string pageLandscape = "横";
        /// <summary>
        /// 用紙縦方向キャプション
        /// </summary>
        internal string pagePortrait = "縦";

        private bool canContinuePageLayout = false;

		/// <summary>
		/// マップのページサイズ表記を取得します
		/// </summary>
		private	Dictionary<int, string>	_dicPageSizes = new Dictionary<int,string>();
		// ﾏｯﾌﾟのﾍﾟｰｼﾞ･ｻｲｽﾞ表記を初期化
		private void InitPageSizes() {
			if(_dicPageSizes.Count <= 0) {
				_dicPageSizes.Add(esriPageFormID.esriPageFormLetter.GetHashCode(), "レター");
				_dicPageSizes.Add(esriPageFormID.esriPageFormLegal.GetHashCode(), "リーガル");
				_dicPageSizes.Add(esriPageFormID.esriPageFormTabloid.GetHashCode(), "タブロイド");
				_dicPageSizes.Add(esriPageFormID.esriPageFormC.GetHashCode(), "C");
				_dicPageSizes.Add(esriPageFormID.esriPageFormD.GetHashCode(), "D");
				_dicPageSizes.Add(esriPageFormID.esriPageFormE.GetHashCode(), "E");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA5.GetHashCode(), "A5");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA4.GetHashCode(), "A4");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA3.GetHashCode(), "A3");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA2.GetHashCode(), "A2");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA1.GetHashCode(), "A1");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA0.GetHashCode(), "A0");
				_dicPageSizes.Add(esriPageFormID.esriPageFormCUSTOM.GetHashCode(), "カスタム");
				_dicPageSizes.Add(esriPageFormID.esriPageFormSameAsPrinter.GetHashCode(), "用紙");
			}
		}

        /// <summary>
        /// 用紙サイズ制限判定
        /// </summary>
        public bool ContinuPageLayout
        {
            get { return this.canContinuePageLayout; }
            set { this.canContinuePageLayout = value; }
        }

        //internal IPageLayout savePageLayout = null;

        /// <summary>
        /// 初期状態のページレイアウト
        /// </summary>
        internal IPageLayout initPageLayout = null;

        /// <summary>
        /// MXDオープン時のMaps
        /// </summary>
        internal IMaps originalMaps = null;

        //declare a PrintDocument object named document, to be displayed in the print preview

        /// <summary>
        /// ページレイアウトで使用する印刷可能なドキュメントオブジェクト
        /// </summary>
        internal System.Drawing.Printing.PrintDocument document =
            new System.Drawing.Printing.PrintDocument();

        //cancel tracker which is passed to the output function when printing to the print preview
        private ITrackCancel m_TrackCancel = new CancelTrackerClass();

        //the page that is currently printed to the print preview
        private short m_CurrentPrintPage;

        /// <summary>
        /// プレビューコマンド実行フラグ
        /// </summary>
        internal bool isPreviewPage = false;

        /// <summary>
        /// プリントコマンド実行フラグ
        /// </summary>
        internal bool isPrintPage = false;

        #endregion

        #region class private members

        private ESRI.ArcGIS.Controls.IPageLayoutControl3 m_pageLayoutControl = null;
        private Common.ControlsSynchronizer m_controlsSynchronizer = null;

        private System.Windows.Forms.Cursor preCursor = Cursors.Default;

        /// <summary>
        /// マップコントロールとページレイアウトコントロールの
        /// 同期を行うControlsSynchronizerへの参照
        /// </summary>
        public Common.ControlsSynchronizer GetMapControlSyncronizer
        {
            get { return this.m_controlsSynchronizer; }
        }

        private string ScaleBarElementName = "ScaleBar";

        /// <summary>
        /// ページレイアウトで使用する
        /// スケールバー（縮尺記号）オブジェクトの名称
        /// </summary>
        public string SCALE_BAR_ELEMENT_NAME
        {
            get { return this.ScaleBarElementName; }
        }

        // マップ状態変更操作判断時の対象のツールバーコマンドのインデクス保持
        private int CurrentToolIndex = -1;
        private int PageLayoutCurrentToolIndex = -1;

        // マップ状態変更操作判断時の除外対象のツールバーコマンドのインデクス
        private const int OpenDox_Index = 0;
        private const int SaveDoc_Index = 1;
        private const int Addlayer_Index = 2;
        private const int KobetsuZokusei_Index = 17;
        private const int Kensaku_Index = 18;
        private const int XY_Idou_Index = 19;
        private const int Keisoku_Index = 20;
        private const int HyperLink_Index = 21;
        private const int Swipe_Index = 22;

        // m_map is set to be global in scope because it will be used for both the AddHandler and RemoveHandler
        private ESRI.ArcGIS.Carto.IMap m_map;
        private ILayer _selectedLayer = null;

        private ITOCControl2 m_tocControl = null;

        private int map_sts_changed_cmt = 0;
        private bool map_sts_changed = false;

        /// <summary>
        /// メインマップ変更状態
        /// </summary>
        public bool MainMapChanged
        {
            get { return this.map_sts_changed; }
            set { this.map_sts_changed = value;}
        }

        // レイヤー透過設定に必要
        private CommandsEnvironmentClass m_CommandsEnvironment = null;

        /// <summary>
        /// メインフォームのタイトル
        /// </summary>
        public string Title_Name = null; // Formタイトル

        // プラグイン実行時用 (MapDocumentFile)
        private string m_mapDocumentName = string.Empty;

        // プラグインメニュークリック時,名称からプラグインインスタンスのインデクス取得
        private System.Collections.Hashtable _pluginNameList = null;

#region プラグイン・インターフェース継承
#if DEBUG
		// IActiveView ｲﾍﾞﾝﾄ･ﾄﾚｰｽ用ｽｲｯﾁ
		static public bool Debug_Confirm = true;
#endif

	#region プロパティ
		/// <summary>
		/// 開いているマップドキュメントを取得します
		/// </summary>
		public string MapDocumentFile {
			get {
#if DEBUG
				Debug.WriteLineIf(Debug_Confirm, "◆MapDocument Property Call : " + this.
                    m_mapDocumentName);
#endif
				return this.m_mapDocumentName;
			}
		}

		/// <summary>
		/// マップの表示縮尺を取得または設定します
		/// </summary>
		public double MapScale {
			get {
				double dblScale;

				try {
					// 投影によっては取得不可
					dblScale = this.m_map.MapScale;
				}
				catch {
					dblScale = double.NaN;
				}

				return dblScale;
			}
			set {
				// 設定可能な場合のみ対応
				if(!this.MapScale.Equals(double.NaN)) {
					this.m_MapControl.MapScale = value;
				}
			}
		}

		/// <summary>
		/// マップの表示範囲を取得または設定します
		/// </summary>
		public ESRI.ArcGIS.Geometry.IEnvelope MapExtent {
			get {
				return this.m_MapControl.Extent;
			}
			set {
#if DEBUG
				Debug.WriteLine("●MapExtent Set.");
#endif
				this.m_MapControl.Extent = value;
			}
		}

        /// <summary>
        /// 選択レイヤーを取得または設定します (TOC)
        /// </summary>
        public ILayer SelectedLayer {
            get { return this._selectedLayer; }
            set {
				if(value != null) {
					this._selectedLayer = value;
					this.m_TocControl.SelectItem(value, null);
				}
			}
        }

        /// <summary>
        /// すべてのフィーチャーレイヤーを取得します
        /// </summary>
        public IFeatureLayer[] FeatureLayers {
			get {
				LayerManager	clsLM = new LayerManager();
				return clsLM.GetFeatureLayers(this.m_map).ToArray();
			}
        }

        delegate bool GetMapVisibleCallBack();
        delegate void SetMapVisibleCallBack(int TabID);
        private bool GetMapVisible() {
			return this.tabControl1.SelectedIndex.Equals(0);
        }
        private void SetMapVisible(int TabID) {
			if(TabID < this.tabControl1.TabCount) {
				this.tabControl1.SelectedIndex = TabID;
			}
        }

        /// <summary>
        /// マップ画面を表示しているかどうかを取得または設定します
        /// </summary>
        public bool IsMapVisible {
			get {
				if(this.InvokeRequired) {
					GetMapVisibleCallBack	dlgMV = new GetMapVisibleCallBack(this.GetMapVisible);
					return (bool)this.Invoke(dlgMV);
				}
				else {
					return this.GetMapVisible();
				}
			}
			set {
				int	intVal = value ? 0 : 1;
				if(this.InvokeRequired) {
					SetMapVisibleCallBack	dlgMV = new SetMapVisibleCallBack(this.SetMapVisible);
					this.Invoke(dlgMV, new object[] { intVal });
				}
				else {
					this.tabControl1.SelectedIndex = intVal;
				}
			}
        }

		/// <summary>
		/// 編集状態かどうかを取得します
		/// </summary>
		public bool IsEditMode {
			get {
				return m_EngineEditor.EditState != esriEngineEditState.esriEngineStateNotEditing;
			}
		}
		/// <summary>
		/// 編集対象レイヤを取得します
		/// </summary>
		public IFeatureLayer EditTargetLayer {
			get {
				IFeatureLayer	agFL = null;
				if(this.IsEditMode) {
					agFL = (this.m_EngineEditor as IEngineEditLayers).TargetLayer;
				}

				return agFL;
			}
			set {
				if(this.IsEditMode) {
					// ｾｯﾄ可能か確認
					IEngineEditLayers	agEdLayers = this.m_EngineEditor as IEngineEditLayers;
					if(agEdLayers.IsEditable(value)) {
						agEdLayers.SetTargetLayer(value, 0);
					}
				}
			}
		}

		/// <summary>
		/// 編集対象ワークスペースを取得します
		/// </summary>
		public IWorkspace EditWorkspace {
			get {
				IWorkspace	agWS = null;
				if(this.IsEditMode) {
					agWS = this.m_EngineEditor.EditWorkspace;
				}

				return agWS;
			}
		}

		/// <summary>
		/// 編集選択フィーチャー数を取得します
		/// </summary>
		public int EditSelectionNum {
			get {
				int	intRet = 0;

				if(this.IsEditMode) {
					intRet = this.m_EngineEditor.SelectionCount;
				}

				return intRet;
			}
		}
	#endregion

	#region メソッド
		/// <summary>
		/// 指定のレイヤが存在するかどうかを取得します
		/// </summary>
		/// <param name="LayerName">レイヤ名</param>
		/// <returns>有 / 無</returns>
		public bool HasLayer(string LayerName) {
			return this.GetLayer(LayerName) != null;
		}

		/// <summary>
		/// 指定のレイヤを取得します
		/// </summary>
		/// <param name="LayerName">レイヤ名</param>
		/// <returns>対象レイヤ</returns>
		public ILayer GetLayer(string LayerName) {
			// ﾚｲﾔを取得
			LayerManager	clsLM = new LayerManager();
			ILayer agLayer = clsLM.GetAllLayers(this.m_map).Find(L=>L.Name.Equals(LayerName));

			return agLayer;
		}

		/// <summary>
		/// マップ範囲を全域に設定します
		/// </summary>
		public void ShowMapFullExtent() {
			IActiveView	agActView = this.m_MapControl.ActiveView;
			agActView.Extent = agActView.FullExtent;

			// 描画更新
			agActView.Refresh();
		}

		/// <summary>
		/// 指定のオブジェクトを指定の範囲で再描画します
		/// </summary>
		/// <param name="Extent"></param>
		/// <param name="DataObject"></param>
		public void RefreshViewExtent(ESRI.ArcGIS.Geometry.IEnvelope Extent, object DataObject) {
			IActiveView	agActView = (this.IsMapVisible ? this.m_MapControl.ActiveView : this.m_pageLayoutControl.ActiveView);
#if DEBUG
			Debug.WriteLine(string.Format("●RefreshViewExtent / IsActive : {0}, InvokeRequired : {1}", agActView.IsActive() ? "YES" : "NO", this.InvokeRequired ? "YES" : "NO"));
#endif
			if(agActView != null) {
				// 地図範囲を取得
				if(Extent != null && !Extent.IsEmpty) {
					agActView.Extent = Extent;
				}

				agActView.PartialRefresh(esriViewDrawPhase.esriViewGeography | esriViewDrawPhase.esriViewGeoSelection | esriViewDrawPhase.esriViewBackground, DataObject, agActView.Extent);
				//agActView.PartialRefresh(esriViewDrawPhase.esriViewAll, DataObject, Extent);
				//this.m_MapControl.ActiveView.ScreenDisplay.UpdateWindow();
			}
		}
		/// <summary>
		/// 現在のビュー範囲を再描画します
		/// </summary>
		public void RefreshViewExtent() {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			this.RefreshViewExtent(null, null);
		}
		/// <summary>
		/// 指定の範囲を再描画します
		/// </summary>
		/// <param name="Extent"></param>
		public void RefreshViewExtent(ESRI.ArcGIS.Geometry.IEnvelope Extent) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			this.RefreshViewExtent(Extent, null);
		}

		/// <summary>
		/// ビューの全体を再描画します
		/// </summary>
		public void RefreshView() {
			//IActiveView	agActView = (this.IsMapVisible ? this.m_MapControl.ActiveView : this.m_pageLayoutControl.ActiveView);
			IActiveView	agActView = (this.IsMapVisible ? this.m_map as IActiveView : this.m_pageLayoutControl.PageLayout as IActiveView);
#if DEBUG
			Debug.WriteLine("●RefreshView");
#endif
			if(agActView != null) {
				agActView.Refresh();
			}
		}

		/// <summary>
		/// フォームを表示します
		/// </summary>
		/// <param name="FormWindow"></param>
		public void ShowForm(Form FormWindow) {
			FormWindow.Show();
		}
		public void ShowFormOnMainForm(Form FormWindow) {
			FormWindow.Show(this);
		}
		public DialogResult ShowFormOnModalWindow(Form FormWindow) {
			return FormWindow.ShowDialog(this);
		}

		// ﾃﾞﾘｹﾞｰﾄ
		delegate void			ShowMessageCallBack(string Message);
		delegate DialogResult	ShowConfirmCallBack(string Message);

		/// <summary>
		/// メッセージボックスを表示します
		/// </summary>
		/// <param name="Message"></param>
		public void ShowMessage_I(string Message) {
			if(this.InvokeRequired) {
				ShowMessageCallBack	dlgSM = new ShowMessageCallBack(ShowMessage_I);
				this.Invoke(dlgSM, new object[] { Message });
			}
			else {
				Common.MessageBoxManager.ShowMessageBoxInfo(this, Message);
			}
		}
		public void ShowMessage_W(string Message) {
			if(this.InvokeRequired) {
				ShowMessageCallBack	dlgSM = new ShowMessageCallBack(ShowMessage_W);
				this.Invoke(dlgSM, new object[] { Message });
			}
			else {
				Common.MessageBoxManager.ShowMessageBoxWarining(this, Message);
			}
		}
		public void ShowMessage_E(string Message) {
			if(this.InvokeRequired) {
				ShowMessageCallBack	dlgSM = new ShowMessageCallBack(ShowMessage_E);
				this.Invoke(dlgSM, new object[] { Message });
			}
			else {
				Common.MessageBoxManager.ShowMessageBoxError(this, Message);
			}
		}
		public DialogResult ShowMessage_C(string Message) {
			if(this.InvokeRequired) {
				ShowConfirmCallBack	dlgSC = new ShowConfirmCallBack(ShowMessage_C);
				return (DialogResult)this.Invoke(dlgSC, new object[] { Message });
			}
			else {
				return Common.MessageBoxManager.ShowMessageBoxQuestion2(this, Message);
			}
		}
	#endregion
#endregion

        private ITOCControl2 m_TocControl = null;
        private IToolbarMenu2 m_TocMenu = null;
        private IToolbarMenu2 m_TocLayerMenu = null;
        //private IToolbarMenu2 m_ToolBarMenuCustom = null;

        /// <summary>
        /// 選択されたテーブルへの参照を取得または設定します
        /// </summary>
        private IStandaloneTable _agSelTable = null;
        public IStandaloneTable SelectedTable {
			get {
				return _agSelTable;
			}
			set {
				this._agSelTable = value;
			}
        }

        private IMapControl4 m_MapControl = null;

        /// <summary>
        /// メインマップへの参照
        /// </summary>
        public IMapControl4 MapControl
        {
            get { return this.m_MapControl; }
        }

        private	bool	_blnUseFieldAlias = true;
        /// <summary>
        /// フィールド名を表示する際にエイリアスを使用するかどうかを取得します
        /// </summary>
        public bool IsUseFieldAlias {
			get {
				try {
					// 現在の設定を取得
					Common.OptionSettings	clsConf = new Common.OptionSettings();
					this._blnUseFieldAlias = clsConf.FieldNmaeUseAlias.Equals("1");
				}
				catch(Exception ex) {
					// 取得時ｴﾗｰ (ﾒｯｾｰｼﾞを表示して処理を継続 戻り値は前回の内容)
					MessageBoxManager.ShowMessageBoxError(this,
						Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead + "[USE FIELD NAME ALIAS SETTING]"
						+ Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
					Common.Logger.Error(Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
						+ "[USE FIELD NAME ALIAS SETTING]" + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

					Common.Logger.Error("MainForm.IsUseFieldAlias : " + ex.Message);
					Common.Logger.Error(ex.StackTrace);
				}

				return this._blnUseFieldAlias;
			}
        }

        /// <summary>
        /// フィールド・リスト・アイテムを取得します
        /// </summary>
        /// <param name="TableFields">レイヤー / スタンドアロンテーブル</param>
        /// <param name="VisibleOnly">可視フィールド?</param>
        /// <param name="EditableOnly">編集可能フィールド?</param>
        /// <param name="OwnFieldOnly">直属フィールド?</param>
        /// <param name="ShowFieldType">アイテムにフィールド型を表示する?</param>
        /// <param name="FieldTypes">抽出するフィールド型</param>
        /// <returns></returns>
        public FieldComboItem[] GetFieldItems(ITableFields TableFields, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// FieldManagerをｺｰﾙ
			return FieldManager.GetFieldItems(TableFields, VisibleOnly, EditableOnly, OwnFieldOnly, this.IsUseFieldAlias, ShowFieldType, FieldTypes);
        }
        public FieldComboItem[] GetFieldItems(IStandaloneTable SATable, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return this.GetFieldItems(SATable as ITableFields, VisibleOnly, EditableOnly, OwnFieldOnly, ShowFieldType, FieldTypes);
        }
        public FieldComboItem[] GetFieldItems(IFeatureLayer FLayer, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return this.GetFieldItems(FLayer as ITableFields, VisibleOnly, EditableOnly, OwnFieldOnly, ShowFieldType, FieldTypes);
        }

        // ToolBar ｺﾝﾄﾛｰﾙ
        private IToolbarControl2 m_ToolbarControl = null;	// axToolbarControl1 (Map)
        internal IToolbarControl2 m_ToolbarControl2 = null; // axToolbarControl2 (Edit)
        //private IToolbarControl2 m_ToolbarControl3 = null;
        private IToolbarControl2 m_ToolbarControl4 = null;	// axToolbarControl4 (PageLayout)
		
		private IToolbarMenu m_ToolbarMenuEditStarted = null;
        private IToolbarMenu m_ToolbarMenu = null;
        //private IToolbarMenu m_ToolbarMenu2 = null;
        private IToolbarMenu m_ToolbarMenu3 = null;			// ﾍﾟｰｼﾞ･ﾚｲｱｳﾄ 右ｸﾘｯｸ･ﾒﾆｭｰ
        //private IToolbarMenu m_ToolbarMenu4 = null;			// 

        private IEngineEditor m_EngineEditor = null; //new EngineEditorClass();
        private IEngineEditSketch m_EngineEditSketch;

        private IEngineEditEvents_Event m_EngineEditEvents = null;

#if DEBUG
		// ツールバー イベント・キャプチャ
        private IToolbarControlEvents_Event	_agToolbarCtlEvents = null;
        private void ToolOnItemClickEvent(int Index) {
			if(Index >= 0) {
				Debug.WriteLine("●TOOL CLICK EVENT : ToolID = " + Index.ToString());
                // 2019.2.21 テスト用
                if (m_ToolbarControl != null) {
                    
                    IToolbarItem clickItem =  m_ToolbarControl.GetItem(Index);
                    if (clickItem != null) {
                        //IntPtr child = FindWindowEx(m_ToolbarControl.hWnd, IntPtr.Zero, "", "");
                    }
                }
			}
        }
#endif

        //
        //internal void SavePageLayout(IPageLayout pagelayout)
        //{
        //    //Get IObjectCopy interface
        //    IObjectCopy objectCopy = new ObjectCopyClass();

        //    //Get IUnknown interface (map to copy)
        //    object toCopyLayout = pagelayout; //axPageLayoutControl1.PageLayout;
        //    savePageLayout = toCopyLayout as PageLayout;

        //}
        private ESRIJapan.GISLight10.dispatcher.Dispatcher dispatcher = null;

        #endregion


        #region Constructor
        /// <summary>
        /// メインフォームのコンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // ﾛｶﾞｰの設定 (ﾍﾞｰｽ･ﾃﾞｨﾚｸﾄﾘの設定) ※Processから直接起動した場合、作業ﾃﾞｨﾚｸﾄﾘ不明の為、相対ﾊﾟｽが無効になってしまう
            Common.Logger.BaseDirectory = Application.StartupPath;
        }
        #endregion

        /// <summary>
        /// ステータスラベル設定
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="status"></param>
        private void SetStatus(bool visible, string status)
        {
            this.label1.Visible = visible;
            this.progressBar1.Visible = visible;

            if (visible)
            {
                this.label1.Text = status;
                this.Enabled = false;
            }
            else
            {
                this.label1.Text = "";
                this.Enabled = true;
            }
        }

        /// <summary>
        /// TOCレイヤ状態で使用可不可設定
        /// </summary>
        /// <param name="enble"></param>
        private void SetToolStripMenuEnable(bool enble)
        {
            //menuHyperLinkKensaku.Enabled = enble;
            menuSentakuZokukeiKensaku.Enabled = enble;
            menuSentakuKukanKensaku.Enabled = enble;
            menuSentakuZokuseichiSyukei.Enabled = enble;
            menuSentakuSelectableLayerSettings.Enabled = enble; // 2010-11-10(wed) add

            menuExportMap.Enabled = enble;
            menuTableJoin.Enabled = enble;
            menuRemoveJoin.Enabled = enble;
            menuRelate.Enabled = enble;
            menuRemoveRelate.Enabled = enble;

            // ジオメトリ演算の使用可不可
            if(enble == true) {
                int count = 0;
                if(!this.IsEditMode) {
                    Common.LayerManager pLayerManager = new GISLight10.Common.LayerManager();
                    List<IFeatureLayer> featureLayerList = pLayerManager.GetFeatureLayers(m_MapControl.Map);

                    foreach(IFeatureLayer fLay in featureLayerList) {
                        if (fLay.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon ||
                            fLay.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                        {
                            count++;
                        }
                    }
                }

                // 編集開始ではなく、ポリゴン、ポリラインが１つ以上ある場合true
                menuGeometryCalculate.Enabled = count > 0 ? true : false;
                menuFieldCalculate.Enabled = count > 0 ? true : false;
                menuIntersect.Enabled = count > 0 ? true : false;
            }
            else {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
            }

        }

        /// <summary>
        /// マップコントロールイベントハンドラ
        /// </summary>
        internal void SetEvents()
        {
            // It is important to perfom the Explict Cast or else the wiring up of the Events with the
            // AddHandler will not function properly. Also the if the Explict Cast is not performed
            // when the RemoveHandler's are called a diconnected RCW error will result.
            m_map = (IMap)axMapControl1.ActiveView.FocusMap; // Explict Cast

            //Create an instance of the delegate, add it to AfterDraw event
            //m_ActiveViewEventsAfterDraw =
            //    new IActiveViewEvents_AfterDrawEventHandler(OnActiveViewEventsAfterDraw);
            //((IActiveViewEvents_Event)(m_map)).AfterDraw += m_ActiveViewEventsAfterDraw;

            //Create an instance of the delegate, add it to AfterItemDraw event
            m_ActiveViewEventsAfterItemDraw =
                new IActiveViewEvents_AfterItemDrawEventHandler(OnActiveViewEventsItemDraw);
            ((IActiveViewEvents_Event)(m_map)).AfterItemDraw += m_ActiveViewEventsAfterItemDraw;

            //Create an instance of the delegate, add it to ContentsChanged event
            m_ActiveViewEventsContentsChanged =
                new IActiveViewEvents_ContentsChangedEventHandler(OnActiveViewEventsContentsChanged);
            ((IActiveViewEvents_Event)(m_map)).ContentsChanged += m_ActiveViewEventsContentsChanged;

            //Create an instance of the delegate, add it to ContentsCleared event
            m_ActiveViewEventsContentsCleared =
                new IActiveViewEvents_ContentsClearedEventHandler(OnActiveViewEventsContentsCleared);
            ((IActiveViewEvents_Event)(m_map)).ContentsCleared += m_ActiveViewEventsContentsCleared;

            //Create an instance of the delegate, add it to FocusMapChanged event
            m_ActiveViewEventsFocusMapChanged =
                new IActiveViewEvents_FocusMapChangedEventHandler(OnActiveViewEventsFocusMapChanged);
            ((IActiveViewEvents_Event)(m_map)).FocusMapChanged += m_ActiveViewEventsFocusMapChanged;

            //Create an instance of the delegate, add it to ItemAdded event
            m_ActiveViewEventsItemAdded =
                new IActiveViewEvents_ItemAddedEventHandler(OnActiveViewEventsItemAdded);
            ((IActiveViewEvents_Event)(m_map)).ItemAdded += m_ActiveViewEventsItemAdded;

            //Create an instance of the delegate, add it to ItemDeleted event
            m_ActiveViewEventsItemDeleted =
                new IActiveViewEvents_ItemDeletedEventHandler(OnActiveViewEventsItemDeleted);
            ((IActiveViewEvents_Event)(m_map)).ItemDeleted += m_ActiveViewEventsItemDeleted;

            //Create an instance of the delegate, add it to ItemReordered event
            m_ActiveViewEventsItemReordered =
                new IActiveViewEvents_ItemReorderedEventHandler(OnActiveViewEventsItemReordered);
            ((IActiveViewEvents_Event)(m_map)).ItemReordered += m_ActiveViewEventsItemReordered;

            //Create an instance of the delegate, add it to SelectionChanged event
            m_ActiveViewEventsSelectionChanged =
                new IActiveViewEvents_SelectionChangedEventHandler(OnActiveViewEventsSelectionChanged);
            ((IActiveViewEvents_Event)(m_map)).SelectionChanged += m_ActiveViewEventsSelectionChanged;

            //Create an instance of the delegate, add it to SpatialReferenceChanged event
            m_ActiveViewEventsSpatialReferenceChanged =
                new IActiveViewEvents_SpatialReferenceChangedEventHandler(
                    OnActiveViewEventsSpatialReferenceChanged);
            ((IActiveViewEvents_Event)(m_map)).SpatialReferenceChanged +=
                m_ActiveViewEventsSpatialReferenceChanged;

            //Create an instance of the delegate, add it to ViewRefreshed event
            //m_ActiveViewEventsViewRefreshed =
            //    new IActiveViewEvents_ViewRefreshedEventHandler(OnActiveViewEventsViewRefreshed);
            //((IActiveViewEvents_Event)(m_map)).ViewRefreshed += m_ActiveViewEventsViewRefreshed;

        }

        /// <summary>
        /// ページレイアウトイベントハンドラ追加
        /// </summary>
        internal void SetPageLayoutEvents()
        {
            m_PageLayoutControlEventsOnViewRefreshed =
                new IPageLayoutControlEvents_OnViewRefreshedEventHandler(
                    OnPageLayoutControlEventsOnViewRefreshed);
            ((IPageLayoutControlEvents_Event)(m_pageLayoutControl)).OnViewRefreshed +=
                m_PageLayoutControlEventsOnViewRefreshed;

            m_PageLayoutControlEventsOnFocusMapChanged =
                new IPageLayoutControlEvents_OnFocusMapChangedEventHandler(
                    OnPageLayoutControlEventsOnFocusMapChanged);
            ((IPageLayoutControlEvents_Event)(m_pageLayoutControl)).OnFocusMapChanged +=
                m_PageLayoutControlEventsOnFocusMapChanged;

            m_PageLayoutControlEventsOnExtentUpdated =
                new IPageLayoutControlEvents_OnExtentUpdatedEventHandler(
                    ONPageLayoutControlEventsOnExtentUpdated);
            ((IPageLayoutControlEvents_Event)(m_pageLayoutControl)).OnExtentUpdated +=
                m_PageLayoutControlEventsOnExtentUpdated;
        }

        /// <summary>
        /// 初期設定
        /// </summary>
        private void SetThisInit()
        {
            //this.Text = Properties.Resources.CommonMessage_ApplicationName;
            this.Title_Name = Properties.Resources.CommonMessage_ApplicationName;

			// ﾀｲﾄﾙ設定
            this.Text = "無題 - " + Properties.Resources.CommonMessage_ApplicationName;

            SetStatus(false, "");

            // Get the CommandsEnvironment singleton
            m_CommandsEnvironment = new CommandsEnvironmentClass();

            // get the MapControl
            //m_mapControl
            m_MapControl = (IMapControl4)axMapControl1.Object;
            //create a new Map
            IMap map = new MapClass();
            map.Name = "Map";
            m_MapControl.DocumentFilename = string.Empty;
            m_MapControl.Map = map;

            //Get IObjectCopy interface
            IObjectCopy objectCopy = new ObjectCopyClass();

            //Get IUnknown interface (map to copy)
            object toCopyLayout = axPageLayoutControl1.PageLayout;
            initPageLayout = toCopyLayout as PageLayout;
            //savePageLayout = toCopyLayout as PageLayout;
            //

            m_tocControl = (ITOCControl2)axTOCControl1.Object;

            originalMaps = new Maps();

            ////////
            // map pagelayout syncro init
            ////////
            m_pageLayoutControl = (IPageLayoutControl3)axPageLayoutControl1.Object;
            m_EngineEditor = new EngineEditorClass();

            // initialize the controls synchronization calss
            m_controlsSynchronizer = new Common.ControlsSynchronizer(m_MapControl, m_pageLayoutControl);

            // add the framework controls (TOC and Toolbars) in order to synchronize then when the
            // active control changes (call SetBuddyControl)
            m_controlsSynchronizer.AddFrameworkControl(axTOCControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl2.Object);
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl5.Object);
            //m_controlsSynchronizer.AddFrameworkControl(axMapControl1.Object);
            /////////

            // disable the Save menu (since there is no document yet)
            menuSaveDoc.Enabled = false;

            // 新規作成
            axToolbarControl1.AddItem(new EngineCommand.CreateNewDocument(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            // 開く
            axToolbarControl1.AddItem(new EngineCommand.OpenDocument(), 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            // 上書き保存
            axToolbarControl1.AddItem(new EngineCommand.SaveDocument(), 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);

            axToolbarControl1.AddItem("esriControls.ControlsAddDataCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomToolControl", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomInTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomOutTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomInFixedCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomOutFixedCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapPanTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapFullExtentCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomToLastExtentBackCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomToLastExtentForwardCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapRotateTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);            
            axToolbarControl1.AddItem("esriControls.ControlsMapClearMapRotationCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);            

            axToolbarControl1.AddItem("esriControls.ControlsSelectFeaturesTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsZoomToSelectedCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsClearSelectionCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsSelectTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapIdentifyTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);

            axToolbarControl1.AddItem("esriControls.ControlsMapFindCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapMeasureTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapHyperlinkTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapGoToCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapSwipeTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsLayerTransparencyCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsLayerListToolControl", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
			// ↓確認用に追加
			//axToolbarControl1.AddItem("esriControls.ControlsUndoCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            //axToolbarControl1.AddItem("esriControls.ControlsRedoCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
 
            //axToolbarControl1.AddItem("esriControls.ControlsOpenDocCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            //axToolbarControl1.AddItem("esriControls.ControlsSaveAsDocCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);

            #region エディット・右クリックメニュー
            // 編集開始後の頂点挿入、頂点削除、頂点移動
            m_ToolbarMenuEditStarted = new ToolbarMenu();
            m_ToolbarMenuEditStarted.AddItem("esriControls.ControlsEditingSketchContextMenu", 0, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_ToolbarMenuEditStarted.AddItem("esriControls.ControlsEditingVertexContextMenu", 0, 0, true, esriCommandStyles.esriCommandStyleTextOnly);
            #endregion

            // map control context menu
            // ビュー更新 2010-12-27 add
            //m_ToolbarMenu.AddItem("esriControls.ControlsMapRefreshViewCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            #region ToolbarMenu
            m_ToolbarMenu = new ToolbarMenu();
            // 選択解除
            m_ToolbarMenu.AddItem("esriControls.ControlsClearSelectionCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 選択フィーチャにズーム
            m_ToolbarMenu.AddItem("esriControls.ControlsZoomToSelectedCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 個別属性表示
            m_ToolbarMenu.AddItem("esriControls.ControlsMapIdentifyTool", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // フィーチャ選択
            m_ToolbarMenu.AddItem("esriControls.ControlsSelectFeaturesTool", 0, 0, true, esriCommandStyles.esriCommandStyleIconAndText);
            // 定率縮小
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomOutFixedCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 定率拡大
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomInFixedCommand", 0, 0, true, esriCommandStyles.esriCommandStyleIconAndText);
            // 次の表示範囲に進む
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomToLastExtentForwardCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 前の表示範囲に戻す
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomToLastExtentBackCommand", 0, 0, true, esriCommandStyles.esriCommandStyleIconAndText);
            // ページ全体を表示
            //m_ToolbarMenu.AddItem("esriControls.ControlsPageZoomWholePageCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 全体表示
            m_ToolbarMenu.AddItem("esriControls.ControlsMapFullExtentCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            #endregion

            #region Share the ToolbarControl command pool with the ToolbarMenu command pool
            m_ToolbarControl = (IToolbarControl2)axToolbarControl1.Object;
            m_ToolbarMenu.CommandPool = m_ToolbarControl.CommandPool;

            m_ToolbarMenuEditStarted.CommandPool = m_ToolbarControl.CommandPool;
#if DEBUG
			// EDIT CONTEXT MENU EVENT CAPTURE (できない)
            this._agToolbarCtlEvents = (IToolbarControlEvents_Event)m_ToolbarMenuEditStarted.Hook;
            this._agToolbarCtlEvents.OnItemClick += new IToolbarControlEvents_OnItemClickEventHandler(ToolOnItemClickEvent);
#endif

            axToolbarControl2.AddItem("esriControls.ControlsEditingEditorMenu", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingEditTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
			axToolbarControl2.AddItem("esriControls.ControlsEditingSketchTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingAttributeCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
			axToolbarControl2.AddItem("esriControls.ControlsEditingSketchPropertiesCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsUndoCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsRedoCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingCutCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingCopyCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingPasteCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingClearCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingTaskToolControl", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingTargetToolControl", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
			//axToolbarControl2.AddItem(new EngineCommand.GeoReferenceCommand(), 0, -1, true, 11, esriCommandStyles.esriCommandStyleIconOnly);
			//IOperationStack	agOpStack = new ControlsOperationStackClass();
			//this.axToolbarControl2.OperationStack = agOpStack;

            //
            m_ToolbarControl2 = (IToolbarControl2)axToolbarControl2.Object;
            //m_ToolbarMenu2 = new ToolbarMenu();
            //m_ToolbarMenu2.CommandPool = m_ToolbarControl2.CommandPool;

			// 編集ﾂｰﾙ･ﾊﾞｰの幅を調整
			this.FitToolBarWidth(m_ToolbarControl2);

            // Undo / Redoが効かなくなった為、ﾊﾞｲﾝﾄﾞのﾀｲﾐﾝｸﾞを変更(10.2.1-)
			// bind the controls together (both point at the same map) and set the MapControl as the active control
            //m_controlsSynchronizer.BindControls(true);
			m_controlsSynchronizer.BindControls(m_MapControl, m_pageLayoutControl, true);

            m_ToolbarControl4 = (IToolbarControl2)axToolbarControl4.Object;
            //m_ToolbarMenu4 = new ToolbarMenu();
            //m_ToolbarMenu4.CommandPool = m_ToolbarControl4.CommandPool;

            //
            #region ページレイアウト・右クリックメニュー
            m_ToolbarMenu3 = new ToolbarMenu();
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapViewMenu", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 画面移動 page
            m_ToolbarMenu3.AddItem("esriControls.ControlsPagePanTool", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 画面移動 map
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapPanTool", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // データフレームの回転
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapRotateTool", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 回転の解除
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapClearMapRotationCommand", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // フィーチャ選択
            m_ToolbarMenu3.AddItem("esriControls.ControlsSelectFeaturesTool", 0, m_ToolbarMenu3.Count, true, esriCommandStyles.esriCommandStyleIconAndText);
            // 選択フィーチャにズーム
            m_ToolbarMenu3.AddItem("esriControls.ControlsZoomToSelectedCommand", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 選択解除
            m_ToolbarMenu3.AddItem("esriControls.ControlsClearSelectionCommand", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu3.AddItem(new EngineCommand.EditTextElementCommand(), 0, m_ToolbarMenu3.Count, true, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu3.AddItem(new EngineCommand.EditScaleBarElementPropCommand(), 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 凡例の編集
            m_ToolbarMenu3.AddItem(new EngineCommand.EditLegendCommand(), 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);

            //m_ToolbarControl3 = (IToolbarControl2)axToolbarControl1.Object;
            m_ToolbarMenu3.CommandPool = m_ToolbarControl.CommandPool;
            #endregion

			// 既定のﾂｰﾙを選択 (ｴﾚﾒﾝﾄ選択ﾂｰﾙ)
			//this.SetElementSelectTool();

            #endregion

            #region TOCメニュー
            m_TocControl = (ITOCControl2)axTOCControl1.Object;

            m_TocMenu = new ToolbarMenuClass();

            //すべてのレイヤを表示
            m_TocMenu.AddItem(
                new EngineCommand.LayerVisibility(),
                1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //すべてのレイヤを非表示
            m_TocMenu.AddItem(
                new EngineCommand.LayerVisibility(),
                2, 1, false, esriCommandStyles.esriCommandStyleTextOnly);

            //グループレイヤの追加
            m_TocMenu.AddItem(
                new EngineCommand.AddGroupLayerCommand(),
                3, 2, false, esriCommandStyles.esriCommandStyleIconAndText);

            //ベースマップレイヤの追加
            m_TocMenu.AddItem(
                new EngineCommand.AddBaseMapLayerCommand(),
                4, 3, false, esriCommandStyles.esriCommandStyleIconAndText);

            //属性検索
            m_TocMenu.AddItem(
                new EngineCommand.AttributeSearchCommand(),
                5, 4, true, esriCommandStyles.esriCommandStyleIconAndText);

            //空間検索
            m_TocMenu.AddItem(
                new EngineCommand.SpatialSearchCommand(),
                6, 5, false, esriCommandStyles.esriCommandStyleIconAndText);

            //属性値集計
            m_TocMenu.AddItem(
                new EngineCommand.AttributeSumCommand(),
                7, 6, false, esriCommandStyles.esriCommandStyleTextOnly);

            //テーブル結合
            m_TocMenu.AddItem(
                new EngineCommand.JoinTableCommand(),
                8, 7, true, esriCommandStyles.esriCommandStyleIconAndText);

            //テーブル結合の解除
            m_TocMenu.AddItem(
                new EngineCommand.RemoveJoinCommand(),
                9, 8, false, esriCommandStyles.esriCommandStyleTextOnly);

            //リレート
            m_TocMenu.AddItem(
                new EngineCommand.RelateCommand(),
                10, 9, false, esriCommandStyles.esriCommandStyleIconAndText);

            //リレートの解除
            m_TocMenu.AddItem(
                new EngineCommand.RemoveRelateCommand(),
                11, 10, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// XYデータの追加
            //m_TocMenu.AddItem(
            //    new EngineCommand.AddXYDataCommand(),
            //    13, 12, true, esriCommandStyles.esriCommandStyleIconAndText);

            // インターセクト
            m_TocMenu.AddItem(
                new EngineCommand.IntersectCommand(),
                12, 11, true, esriCommandStyles.esriCommandStyleIconAndText);

            //選択可能レイヤの設定
            m_TocMenu.AddItem(
                new EngineCommand.SelectableLayerSettingsCommand(),
                13, 12, true, esriCommandStyles.esriCommandStyleTextOnly);

            // 2012/08/06 ADD
            // 座標系の設定
            m_TocMenu.AddItem(
                new EngineCommand.SetDataFrameProjectionCommand(),
                14, 13, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 ADD

            // 2012/08/06 DEL
            // ジオリファレンス
            //m_TocMenu.AddItem(
            //    new EngineCommand.GeoReferenceCommand(),
            //    15, 14, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 DEL
            #endregion

            #region TOCレイヤメニュー
            m_TocLayerMenu = new ToolbarMenuClass();

            // 2012/08/06 ADD 
            //// レイヤ保存
            m_TocLayerMenu.AddItem(new EngineCommand.SaveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // フィーチャクラスへエクスポート
            m_TocLayerMenu.AddItem(new EngineCommand.ExportShapeFile(), -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            // 2012/08/14 chg >>>>>
            //// フィールド演算
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.FieldCalculatorCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // フィールドの追加と削除
            m_TocLayerMenu.AddItem(new EngineCommand.AddAndDeleteFieldCommand(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // フィールド演算
            m_TocLayerMenu.AddItem(new EngineCommand.LayerFieldCalculatorCommand(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 2012/08/14  chg <<<<<

            // 長さ・面積計算
            m_TocLayerMenu.AddItem(new EngineCommand.LayerGeometryCalculatorCommand(), -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            // 2012/08/06 ADD 

            // 2012/08/06 DEL 
            //// 縮尺範囲
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.ScaleRangeCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// レイヤ保存
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SaveLayer(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 DEL 

            // データソース設定
            m_TocLayerMenu.AddItem(
                new EngineCommand.OpenDataSource(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // 2012/08/06 ADD 
            // ハイパーリンクの設定
            m_TocLayerMenu.AddItem(
                new EngineCommand.HyperLinkSettingCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // ラベルシンボル
            m_TocLayerMenu.AddItem(
                new EngineCommand.LabelSettingCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // シンボル設定
            m_TocLayerMenu.AddItem(
                new EngineCommand.SymbolSettingsCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            // 縮尺範囲
            m_TocLayerMenu.AddItem(
                new EngineCommand.ScaleRangeCommand(),
                -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            // 属性テーブル表示
            m_TocLayerMenu.AddItem(
                new EngineCommand.OpenAttributeTable(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            // フィールドの別名定義
            m_TocLayerMenu.AddItem(
                new EngineCommand.DefineFieldNameAliasCommand(),
                -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            // レイヤの削除
            m_TocLayerMenu.AddItem(
                new EngineCommand.RemoveLayer(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            // グループレイヤ操作
            m_TocLayerMenu.AddItem(
                new EngineCommand.ShowTOCCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // メニュー一番上
            // レイヤの全体表示
            m_TocLayerMenu.AddItem(
                new EngineCommand.ZoomToLayer(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 2012/08/06 ADD 

            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SetLayerProperty(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // 2012/08/06 DEL 
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.ZoomToLayer(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            ////m_TocLayerMenu.AddItem(
            ////    "esriControls.ControlsZoomToSelectedCommand",
            ////    -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            ////
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SelectionFeatureZoomCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.RemoveLayer(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            //// export shape file
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.ExportShapeFile(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            //// ハイパーリンクの設定
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.HyperLinkSettingCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// ラベルシンボル
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.LabelSettingCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// シンボル設定
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SymbolSettingsCommand(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            //// フィールド演算
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.FieldCalculatorCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// 長さ・面積計算
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.LayerGeometryCalculatorCommand(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            //// 属性テーブル表示
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.OpenAttributeTable(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            //// フィールドの別名定義
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.DefineFieldNameAliasCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // メニュー一番上
            //// フィールドの追加と削除
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.AddAndDeleteFieldCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 DEL 
            #endregion

            // メニューバーに追加
            //m_ToolbarControl.AddItem(
            //    m_TocLayerMenu, 0, m_ToolbarControl.Count,
            //    false, 0, esriCommandStyles.esriCommandStyleIconAndText);

            //m_ToolBarMenuCustom = new ToolbarMenuClass();
            //m_ToolBarMenuCustom.Caption = "プラグイン";
            //m_ToolbarControl.AddItem(
            //    m_ToolBarMenuCustom, 0, m_ToolbarControl.Count,
            //    false, 0, esriCommandStyles.esriCommandStyleIconAndText);

            // 2012/08/07  
            // ジオリファレンス
            this.axToolbarControl5.AddItem(new EngineCommand.GeoReferenceCommand(), 0, 0, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
			// 2012/08/07  

            // ﾌｨｰﾁｬｰ編集メニューに追加
            IToolbarItem titm = m_ToolbarControl2.GetItem(0);
            titm.Menu.AddItem(new EngineCommand.MergeCommand(), 0, titm.Menu.Count, true, esriCommandStyles.esriCommandStyleTextOnly);
            titm.Menu.AddItem(new EngineCommand.SplitAndMergeSettingCommand(), 0, titm.Menu.Count, false, esriCommandStyles.esriCommandStyleTextOnly);
            // "esriControls.ControlsEditingSnappingCommand"
            titm.Menu.AddItem(new EngineCommand.EditingSnappingCommand(), 0, titm.Menu.Count, true, esriCommandStyles.esriCommandStyleTextOnly);
            titm.Menu.AddItem(new EngineCommand.EditOptionCommand(), 0, titm.Menu.Count, false, esriCommandStyles.esriCommandStyleTextOnly);

            //m_ToolbarControl3.AddItem(
            //    new EngineCommand.PrintActiveViewCommand(),
            //    -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);

            // PageLayoutTab エレメント挿入
            IToolbarItem titm4 = axToolbarControl4.GetItem(0);
            titm4.Menu.AddItem(new EngineCommand.CreateLegend(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.InsertPictureCommand(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.CreateScaleBar(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.CreateNorthArrow(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.CreateTextElement(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            #region PageLayoutのﾂｰﾙ･ﾊﾞｰに印刷用ｺﾏﾝﾄﾞを追加
            m_ToolbarControl4.AddItem(
                new EngineCommand.PageSetUpCommand(),
                -1, m_ToolbarControl4.Count, true, -1, esriCommandStyles.esriCommandStyleIconAndText);

            m_ToolbarControl4.AddItem(
                new EngineCommand.PrinitPreviewCommand(),
                -1, m_ToolbarControl4.Count, false, -1, esriCommandStyles.esriCommandStyleIconAndText);

            m_ToolbarControl4.AddItem(
                new EngineCommand.PrinitPageLayoutCommand(),
                -1, m_ToolbarControl4.Count, false, -1, esriCommandStyles.esriCommandStyleIconAndText);

            m_ToolbarControl4.AddItem(
                new EngineCommand.ResetPageLayoutCommand(),
                -1, m_ToolbarControl4.Count, false, -1, esriCommandStyles.esriCommandStyleIconAndText);
            #endregion



            // hook
            //m_ToolbarMenu.SetHook(axToolbarControl1); //m_MapControl);
            //m_ToolbarMenuEditStarted.SetHook(axToolbarControl1);
            m_ToolbarMenu.SetHook(m_ToolbarControl);
            //m_ToolbarMenu2.SetHook(m_ToolbarControl2);
            //m_ToolbarMenu4.SetHook(m_ToolbarControl4);
            m_ToolbarMenu3.SetHook(m_ToolbarControl);

            m_TocMenu.SetHook(m_MapControl);
            m_TocLayerMenu.SetHook(m_MapControl);

            // 実行コマンドクラスの割り振り時に使用
            this.dispatcher = new GISLight10.dispatcher.Dispatcher();

            // Set the Engine Edit Sketch
            m_EngineEditor = new EngineEditorClass();
            m_EngineEditSketch = (IEngineEditSketch)m_EngineEditor;

            // TOCレイヤ未設定時
            SetToolStripMenuEnable(false);

            // add edittask
            SplitPolylineEditTask splitPolylineEditTask = new SplitPolylineEditTask();
            m_EngineEditor.AddTask((IEngineEditTask)splitPolylineEditTask);
            SplitPolygonEditTask splitPolygonEditTask = new SplitPolygonEditTask();
            m_EngineEditor.AddTask((IEngineEditTask)splitPolygonEditTask);
        }

        /// <summary>
        /// マップ変更状態監視
        /// </summary>
        public void SetMapStateChanged()
        {
            //if (map_sts_changed_cmt > 0)
            map_sts_changed = true;
            map_sts_changed_cmt++;
        }

        /// <summary>
        /// マップ変更状態クリア
        /// </summary>
        public void ClearMapChangeCount()
        {
            this.map_sts_changed_cmt = 0;
            this.map_sts_changed = false;
        }

        /// <summary>
        /// ツールメニュー使用可能不可切り替え
        /// </summary>
        private void SetMenuItemEnabled()
        {
#if DEBUG
			Debug.WriteLine(string.Format("●SetMenuItemEnabled 実行 {0:HH:mm:ss:ffff}", DateTime.Now));
#endif



            if(m_MapControl.LayerCount == 0) {
                // レイヤが存在しなければ
                // レイヤを参照するメニューのアイテムを使用不可に
                SetToolStripMenuEnable(false);
                return;
            }

            Common.LayerManager pLayerManager = new GISLight10.Common.LayerManager();

            List<IFeatureLayer> featureLayerList = pLayerManager.GetFeatureLayers(m_MapControl.Map);

            if(featureLayerList.Count == 0) {
                // 有効なレイヤが存在しなければ
                // レイヤを参照するメニューのアイテムを使用不可に
                SetToolStripMenuEnable(false);
            }
            else {
                // 有効なレイヤが存在すれば
                // レイヤを参照するメニューのアイテムを使用可能に
                SetToolStripMenuEnable(true);
            }

            // 属性テーブル表示している時は編集メニュー不可
            if(HasFormAttributeTable()) {
                m_ToolbarControl2.Enabled = false;
            }
			// ｼﾞｵ･ﾘﾌｧﾚﾝｽ中は不可
			else if(this.HasGeoReference()) {
                m_ToolbarControl2.Enabled = false;
			}
            else {
				m_ToolbarControl2.Enabled = this.IsMapVisible;
            }

        }

        /// <summary>
        /// テキスト修正
        /// </summary>
        private void EditTextElement()
        {
            IPageLayout pl = axPageLayoutControl1.PageLayout;
            IGraphicsContainerSelect graphicsSelect = pl as IGraphicsContainerSelect;

            if (graphicsSelect.ElementSelectionCount > 0)
            {
                IEnumElement ienum = graphicsSelect.SelectedElements;
                ienum.Reset();
                IElement selectedElement = ienum.Next();
                if (selectedElement != null)
                {
                    if (selectedElement is ITextElement)
                    {
                        ITextElement tel = selectedElement as ITextElement;
                        IGraphicsContainer graphicsContainer =
                            this.axPageLayoutControl1.PageLayout as IGraphicsContainer;

                        Ui.FormText frm = new Ui.FormText(
                            this.axPageLayoutControl1.Object as IPageLayoutControl3,
                            tel, graphicsContainer);

                        frm.ShowDialog(this);
                    }
                }
            }
        }

        /// <summary>
        /// スケールバー修正
        /// </summary>
        private void SetEditEnableScaleBar()
        {
            IPageLayout pl = axPageLayoutControl1.PageLayout;
            IGraphicsContainerSelect graphicsSelect = pl as IGraphicsContainerSelect;
            if (graphicsSelect.ElementSelectionCount > 0)
            {
                IEnumElement ienum = graphicsSelect.SelectedElements;
                ienum.Reset();
                IElement selectedElement = ienum.Next();
                if (selectedElement != null)
                {
                    IElement targetelm =
                        m_pageLayoutControl.FindElementByName(SCALE_BAR_ELEMENT_NAME, 1);

                    if (selectedElement == targetelm)
                    {
                        IMapFrame mapFrame =
                            (IMapFrame)m_pageLayoutControl.ActiveView.GraphicsContainer.FindFrame(
                            m_pageLayoutControl.ActiveView.FocusMap);

                        FormScaleBarSettings frm =
                            new FormScaleBarSettings(mapFrame, m_pageLayoutControl, targetelm);

                        frm.ShowDialog(this);
                    }

                }
            }
        }

        /// <summary>
        /// マップ状態変更確認：
        /// マップでのマウス操作前にツールバーでクリックされたコマンド確認
        /// </summary>
        private void SetMapStatusChanged()
        {
            if (CurrentToolIndex != OpenDox_Index &&
                CurrentToolIndex != SaveDoc_Index &&
                CurrentToolIndex != Addlayer_Index &&
                CurrentToolIndex != KobetsuZokusei_Index &&
                CurrentToolIndex != Kensaku_Index &&
                CurrentToolIndex != XY_Idou_Index &&
                CurrentToolIndex != Keisoku_Index &&
                CurrentToolIndex != HyperLink_Index &&
                CurrentToolIndex != Swipe_Index)
            {
                map_sts_changed = true;
            }
            //else
            //{
            //    map_sts_changed = false;
            //}
        }

        /// <summary>
        /// マップコントロールリセット
        /// </summary>
        private void ClearMap()
        {
            IMap map = new MapClass();
            map.Name = "Map";
            m_controlsSynchronizer.MapControl.DocumentFilename = string.Empty;

            //replace the shared map with the new Map instance
            this.originalMaps.Reset();
            originalMaps.Add(map);
            m_controlsSynchronizer.PageLayoutControl.PageLayout.ReplaceMaps(originalMaps);
            m_controlsSynchronizer.ReplaceMap(map);

            //this.Text = Properties.Resources.CommonMessage_ApplicationName;
            this.Text = "無題 - " +
                Properties.Resources.CommonMessage_ApplicationName;

        }
        /// <summary>
        /// ページレイアウトリセット
        /// </summary>
        internal void ClearPageLayout()
        {
            IMapFrame mapFrame =
                (IMapFrame)this.axPageLayoutControl1.ActiveView.GraphicsContainer.FindFrame(
                this.axPageLayoutControl1.ActiveView.FocusMap);

            if (mapFrame != null)
            {
                mapFrame.Container.DeleteAllElements();
            }

            this.axPageLayoutControl1.PageLayout = initPageLayout; // new PageLayoutClass();
            this.axPageLayoutControl1.ActiveView.Extent = this.axPageLayoutControl1.ActiveView.FullExtent;
        }

        #region FormLoad
        private void MainForm_Load(object sender, EventArgs e) {
            try {
                Common.Logger.Info(Properties.Resources.CommonMessage_ApplicationName + "開始");

                // ログインユーザ別設定ファイル存在チェック
                if(!Common.ApplicationInitializer.IsUserSettingsExists()) {
                    // 存在しなければ作成
                    // 2020.03.17 コメントアウト
                    Common.ApplicationInitializer.CreateUserSettings();
                }

                // 2020/03/17 : Workaround for controls display issues in 96 versus 120 dpi
                // https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#Controls92DPIWorkaround.htm
                AdjustBounds(this.axToolbarControl1);
                AdjustBounds(this.axToolbarControl2);
                AdjustBounds(this.axToolbarControl4);
                AdjustBounds(this.axToolbarControl5);
                AdjustBounds(this.axLicenseControl1);
                AdjustBounds(this.axMapControl1);
                AdjustBounds(this.axTOCControl1);
                AdjustBounds(this.axPageLayoutControl1);

                // ｲﾆｼｬﾙ処理を開始
                SetThisInit();

                //2011/08/05 ArcGIS10環境で下記を実行すると画面崩れるのでコメントアウト -->
                //ツールバーの透過設定
                //Set label to not clip controls so ToolbarControl background is re-painted
                //int style = GetWindowLong(this.Handle, GWL_STYLE);
                //SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_CLIPCHILDREN);

                //Set the ToolbarControl to be transparent
                //axToolbarControl1.Transparent = true;
                //axToolbarControl2.Transparent = true;
                // <--

                //Set the ToolbarControl to be a child control of the label
                //axToolbarControl1.Parent = this;
                //axToolbarControl2.Parent = this;

				// 背景ｶﾗｰを合わせる
				{
					Color	colCtl = Color.FromName("Control");
					this.axToolbarControl1.BackColor = colCtl;
					this.axToolbarControl2.BackColor = colCtl;
					this.axToolbarControl4.BackColor = colCtl;
					this.axToolbarControl5.BackColor = colCtl;
				}

                // リフレッシュボタン追加
                ToolStripStatusLabel dummyLabel = new ToolStripStatusLabel();
                dummyLabel.Text = "";
                dummyLabel.Spring = true;
                statusStrip1.Items.Add(dummyLabel);


                Bitmap refreshButtonBitmap = (Bitmap)imageList1.Images[0];
                refreshButtonBitmap.MakeTransparent();

                ToolStripButton refreshButton
                    = new ToolStripButton("マップのリフレッシュ", refreshButtonBitmap,
                        new EventHandler(this.refreshButton_Click));

                refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                statusStrip1.Items.Add(refreshButton);

                statusStrip1.ShowItemToolTips = true;

                //編集開始イベントハンドラの追加
                m_EngineEditEvents = (IEngineEditEvents_Event)m_EngineEditor;
                m_EngineEditEvents.OnStartEditing +=
                    new IEngineEditEvents_OnStartEditingEventHandler(OnStartEditingMethod);

                //m_EngineEditEvents.OnStopEditing += new IEngineEditEvents_OnStopEditingEventHandler(OnStopEditingMethod);

#if DEBUG
                // ｴﾃﾞｨﾀ･ｲﾍﾞﾝﾄをｷｬﾌﾟﾁｬﾘﾝｸﾞ
				m_EngineEditEvents.OnSelectionChanged += new IEngineEditEvents_OnSelectionChangedEventHandler(OnEditorSelectionChanged);
#endif

                InitPrintSettings();
                SetEvents();
                SetPageLayoutEvents();

                // 起動ﾊﾟﾗﾒｰﾀをﾁｪｯｸ
				if(Program.StartArguments != null && !string.IsNullOrEmpty(Program.StartArguments[0])) {
					// 第1引数 - MXDﾌｧｲﾙ･ﾊﾟｽ
					string	strMXD = Program.StartArguments[0];
					// 拡張子を取得
					string	strExt = Path.GetExtension(strMXD).ToLower();
					// ﾊﾟｽの有効性を確認
					if(File.Exists(strMXD) && (strExt.Equals(".mxd") | strExt.Equals(".mxt") | strExt.Equals(".pmf"))) {
						// ﾏｯﾌﾟﾄﾞｷｭﾒﾝﾄを開く
						this.OpenMXDFile(strMXD);
					}
				}
				Common.Logger.Info(Properties.Resources.CommonMessage_ApplicationName + "起動");
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_WhenLoad);

                this.Close();
            }
        }
        #endregion

        #region MenuEventHandlers

        /// <summary>
        /// 共通イベントハンドラ
        /// コマンドクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void common_menuItemCommand_Click(object sender, EventArgs e)
        {
            try
            {
                this.dispatcher.DoCommand(sender, m_MapControl.Object);
            }
            catch (Exception ex)
            {
                Common.Logger.Error(sender.ToString() + " " + ex.Message);
            }
        }

        /// <summary>
        /// Save Document command
        /// 上書き保存時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveDoc_Click(object sender, EventArgs e)
        {
            IMapDocument mapDoc = new MapDocumentClass();
            try
            {
                if (m_MapControl.CheckMxFile(axMapControl1.DocumentFilename))
                {
                    mapDoc.Open(axMapControl1.DocumentFilename, string.Empty);
                    if (mapDoc.get_IsReadOnly(axMapControl1.DocumentFilename))
                    {
                        mapDoc.Close();
                        Common.Logger.Info("このドキュメントは読込み専用です");
                        Common.MessageBoxManager.ShowMessageBoxInfo(
                            this, "このドキュメントは読込み専用です");
                        return;
                    }
                    m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                    m_MapControl.ActiveView.ShowScrollBars = true;

                    mapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                    //mapDoc.ReplaceContents((IMxdContents)m_MapControl.Map);

                    // 2010-12-27
                    mapDoc.SetActiveView(m_pageLayoutControl.ActiveView);
                    mapDoc.SetActiveView(m_MapControl.ActiveView);
                    //

                    mapDoc.Save(true, false);

                    mapDoc.Close();
                    ClearMapChangeCount();
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMenuSave);

            }
            finally
            {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mapDoc);
            }
        }

        /// <summary>
        /// exit the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExitApp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// listen to MapReplaced evant in order to update the statusbar and the Save menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMapReplaced(
            object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            try
            {
                //get the current document name from the MapControl
                m_mapDocumentName = m_MapControl.DocumentFilename;

                //if there is no MapDocument, diable the Save menu and clear the statusbar
                if (m_mapDocumentName == string.Empty)
                {
                    menuSaveDoc.Enabled = false;

                    // Toolbarの上書き保存無効化
                    if (m_ToolbarControl != null)
                    {
                        for (int i = 0; i < m_ToolbarControl.Count; i++)
                        {
                            IToolbarItem ti = m_ToolbarControl.GetItem(i);

                            if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                            {
                                EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                saveDocumet.DisableCommand();
                            }
                        }
                    }

                    statusBarXY.Text = string.Empty;
                }
                else
                {
                    //enable the Save manu and write the doc name to the statusbar
                    menuSaveDoc.Enabled = true;

                    // Toolbarの上書き保存有効化
                    if (m_ToolbarControl != null)
                    {
                        for (int i = 0; i < m_ToolbarControl.Count; i++)
                        {
                            IToolbarItem ti = m_ToolbarControl.GetItem(i);

                            if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                            {
                                EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                saveDocumet.EnableCommand();
                            }
                        }
                    }

                    statusBarXY.Text = Path.GetFileName(m_mapDocumentName);
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenReplaceMap);

            }
        }

        /// <summary>
        /// ステータス バーの座標表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMouseMove(
            object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            try
            {
                string mapUnitName = Common.UtilityClass.GetMapUnitText(axMapControl1.MapUnits);
                if (mapUnitName.Length == 0)
                {
                    mapUnitName = axMapControl1.MapUnits.ToString().Substring(4);
                }

                //投影座標系
                if (axMapControl1.SpatialReference is
                        ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)
                {
                    //Pointオブジェクトの作成
                    ESRI.ArcGIS.Geometry.IPoint point = new ESRI.ArcGIS.Geometry.PointClass();
                    point.SpatialReference = axMapControl1.SpatialReference;
                    point.X = e.mapX;
                    point.Y = e.mapY;

                    //空間参照（地理座標系:JGD 2000）の作成
                    ESRI.ArcGIS.Geometry.ISpatialReferenceFactory sprefFactry
                        = new ESRI.ArcGIS.Geometry.SpatialReferenceEnvironmentClass();
                    ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem geoCoordSystem =
                        sprefFactry.CreateGeographicCoordinateSystem
                            ((int)ESRI.ArcGIS.Geometry.esriSRGeoCS3Type.esriSRGeoCS_JapanGeodeticDatum2000);

                    //地理座標系:JGD 2000で投影
                    point.Project(geoCoordSystem);

                    if (point.IsEmpty == true)
                    {
                        statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}",
                            e.mapX.ToString("#######.###"),
                            e.mapY.ToString("#######.###"),
                            mapUnitName
                        );

                        return;
                    }

                    statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}    {3}, {4}  {5}",
                            e.mapX.ToString("#######.###"),         //0
                            e.mapY.ToString("#######.###"),         //1
                            mapUnitName,                            //2
                            point.X.ToString("####.#####"),         //3
                            point.Y.ToString("####.#####"),         //4
                            "度(10進)"                              //5
                        );
                }
                //地理座標系
                else if (axMapControl1.SpatialReference is
                            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem)
                {
                    statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}",
                            e.mapX.ToString("####.#####"),
                            e.mapY.ToString("####.#####"),
                            mapUnitName
                        );
                }
                //UnknownCoordinateSystem
                else
                {
                    statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}",
                            e.mapX.ToString("#######.#####"),
                            e.mapY.ToString("#######.#####"),
                            mapUnitName
                        );
                }
            }
            catch (COMException comex)
            {
                Common.UtilityClass.DoOnError(comex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseMove);
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseMove);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e) {
            try {
                if(e.button != 2) {
                    SetMapStatusChanged();

                    // 右クリックではない
                    return;
                }

                // 頂点挿入、削除メニューのEnable Switching
                // Call the SetEditLocation method using the OnMouseUp location
                m_EngineEditSketch.SetEditLocation(e.x, e.y);

                if(this.IsEditMode) {
                    m_ToolbarMenuEditStarted.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
                else {
                  //m_MapMenu.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                    m_ToolbarMenu.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
            }
            catch(Exception ex) {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseDown);
            }
        }

        /// <summary>
        /// TOCControlで右クリック時にメニューを表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e) {
            try {
                if(e.button != 2) // 右クリックのみで動作
                    return;

                // splitcontainerに入れた場合popupが表示される瞬間に
                // 一番上のメニューアイテムが実行されるのを回避
                // 2012/07/25 UPD 
                //int offsetx = 1;
                int offsetx = 5;
                // 2012/07/25 UPD 

                // レイアウト表示の場合は表示しない
                if(!this.IsMapVisible) {
                    return;
                }

                esriTOCControlItem tocItem = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                System.Object obj = null;
                System.Object index = null;

                // アイテム選択判定
                m_TocControl.HitTest(e.x, e.y, ref tocItem, ref map, ref layer, ref obj, ref index);

                if(tocItem == esriTOCControlItem.esriTOCControlItemNone)
                    return;

                if(tocItem == esriTOCControlItem.esriTOCControlItemMap) {
                    m_TocControl.SelectItem(m_MapControl.ActiveView, null);
                }
                else if(tocItem == esriTOCControlItem.esriTOCControlItemLayer) {
					this.SelectedLayer = layer;
                }

                // レイヤをカスタムプロパティ設定
                m_MapControl.CustomProperty = layer;

                // ポップアップ表示
                if (tocItem == esriTOCControlItem.esriTOCControlItemMap)
                    m_TocMenu.PopupMenu(e.x + offsetx, e.y, m_TocControl.hWnd);

                if (tocItem == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    // 処理可能なレイヤである場合に
                    // 属性テーブル表示やシェープファイルエクスポートを起動可能にする
                    // そうではない場合には使用不可にする。
                    // 該当するコマンドクラス側のEnableプロパティで設定している
                    m_TocLayerMenu.PopupMenu(e.x + offsetx, e.y, m_TocControl.hWnd);
                }

            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenTocControlMouseDown);
            }
        }

        /// <summary>
        /// TOCControlマウスアップ時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            try
            {
                //現行、左クリックのみでの操作が対象であるためフィルタをかける
                if (e.button != 1)
                    return;


                esriTOCControlItem tocItem = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                System.Object obj = null;
                System.Object index = null;

                // アイテム選択判定
                m_TocControl.HitTest(
                    e.x, e.y, ref tocItem, ref map, ref layer, ref obj, ref index);

                // レイヤをカスタムプロパティ設定
                m_MapControl.CustomProperty = layer;

                //レイヤ左クリック時
                if (tocItem == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    _selectedLayer = layer;

                    if (e.button == 1)
                    {
                        //グループレイヤ以外でリンク切れの場合
                        if (layer != null
                            && !(layer is IGroupLayer)
                            && !layer.Valid)
                        {
                            var command = new EngineCommand.OpenDataSource();
                            command.OnCreate(m_MapControl.Object);
                            command.OnClick(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenTocControlMouseDown);

            }
        }

        /// <summary>
        /// 終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            this.Dispose();
            Common.Logger.Info(Properties.Resources.CommonMessage_ApplicationName + "終了");
        }


        /// <summary>
        /// バージョン情報表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolMenuVersionInfo_Click(object sender, EventArgs e)
        {
            try
            {
                // 自分自身のバージョン情報を取得
                // Assembly
                System.Reflection.Assembly asm =
                    System.Reflection.Assembly.GetExecutingAssembly();

                // バージョン取得
                System.Version ver = asm.GetName().Version;


                //バージョン情報の表示
                FormVersionInfo formVersionInfo =
                    new FormVersionInfo(ver.Major.ToString(),ver.Minor.ToString(),ver.Build.ToString());
                formVersionInfo.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_WhenToolMenuVersionInfo_Click);
            }
        }


        #region Custom Functions that you write to add additionaly functionality for the events
        // <summary>
        // MapControl Event handler
        // </summary>
        // <param name="sender"></param>
        // <param name="e"></param>
        //private void OnActiveViewEventsAfterDraw(
        //    ESRI.ArcGIS.Display.IDisplay Display, ESRI.ArcGIS.Carto.esriViewDrawPhase phase)
        //{
        //    SetMenuItemEnabled();
        //    //Common.Logger.Debug("AfterDraw");
        //}
        /// <summary>
        ///
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Display"></param>
        /// <param name="phase"></param>
        private void OnActiveViewEventsItemDraw(
            short Index, ESRI.ArcGIS.Display.IDisplay Display, ESRI.ArcGIS.esriSystem.esriDrawPhase phase)
        {
            if(!this.IsMapVisible) return;

#if DEBUG
            Common.Logger.Debug("ItemDraw");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : ItemDraw");
#endif
        }

        private void OnActiveViewEventsContentsChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("ContentsChanged");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : ContentsChanged");
#endif
        }

        private void OnActiveViewEventsContentsCleared()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("ContentsCleared");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : ContentsCleared");
#endif
        }

        private void OnActiveViewEventsFocusMapChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("FocusMapChanged");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : FocusMapChanged");
#endif
        }

        private void OnActiveViewEventsItemAdded(object Item)
        {
            SetMapStateChanged();
            SetMenuItemEnabled();
#if DEBUG
            Common.Logger.Debug("ItemAdded");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : ItemAdded");
#endif
        }

        private void OnActiveViewEventsItemDeleted(object Item)
        {
            SetMapStateChanged();
            SetMenuItemEnabled();
#if DEBUG
            Common.Logger.Debug("ItemDeleted");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : ItemDeleted");
#endif
        }

        private void OnActiveViewEventsItemReordered(object Item, int toIndex)
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("ItemReordered");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : ItemReordered");
#endif
        }

        private void OnActiveViewEventsSelectionChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("SelectionChanged");
            Debug.WriteLineIf(Debug_Confirm, "◆◆ACTVIEW EVENT : SelectionChanged, " + DateTime.Now.ToString("HH:mm:ss"));

            StringBuilder	sbRepo = new StringBuilder();
            // 編集状況ﾚﾎﾟｰﾄ
            if(this.IsEditMode) {
				IFeatureLayer		agFLayer = this.EditTargetLayer;
				if(agFLayer == null) {
					sbRepo.AppendLine("編集対象ﾚｲﾔ : なし");
				}
				else {
					IFeatureSelection	agFSel = agFLayer as IFeatureSelection;
					sbRepo.AppendFormat("編集対象ﾚｲﾔ : {0}, 選択ﾌｨｰﾁｬｰ数 : {1}\r\n", agFLayer.Name, agFSel.SelectionSet.Count);
				}

				// 編集選択ｾｯﾄを確認
				int	intFeats = 0;
				IEnumFeature	agSelFeat = this.m_EngineEditor.EditSelection;
				IFeature		agFeat;
				while((agFeat = agSelFeat.Next()) != null) {
					++intFeats;
				}
            }
            // ﾏｯﾌﾟ選択ﾌｨｰﾁｬｰ･ﾚﾎﾟｰﾄ
            if(this.m_map.SelectionCount > 0) {
				sbRepo.AppendLine("選択ﾌｨｰﾁｬｰ :");

				IEnumFeature	agSelFEnum = this.m_map.FeatureSelection as IEnumFeature;
				IFeature		agFeat;
				while((agFeat = agSelFEnum.Next()) != null) {
					sbRepo.AppendFormat("FC = {0}, OID = {1}\r\n", (agFeat.Table as IFeatureClass).AliasName, agFeat.OID);
				}
            }
            else {
				sbRepo.AppendLine("選択ﾌｨｰﾁｬｰなし");
            }

            Debug.WriteLine(sbRepo.ToString());
#endif
        }

        private void OnActiveViewEventsSpatialReferenceChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("SpatialReferenceChanged");
            Debug.WriteLineIf(Debug_Confirm, "◆ACTVIEW EVENT : SpatialReferenceChanged");
#endif
        }

        //private void OnActiveViewEventsViewRefreshed(
        //    ESRI.ArcGIS.Carto.IActiveView view,
        //    ESRI.ArcGIS.Carto.esriViewDrawPhase phase,
        //    object data, ESRI.ArcGIS.Geometry.IEnvelope envelope)
        //{
        //    Common.Logger.Debug("ViewRefreshed");
        //}

        private void OnPageLayoutControlEventsOnViewRefreshed(
            object ActiveView, int viewDrawPhase, object layerOrElement, object envelope)
        {
            if (viewDrawPhase == (int)esriViewDrawPhase.esriViewGraphics)
            {
                SetMapStateChanged();
            }
        }

        private void OnPageLayoutControlEventsOnFocusMapChanged()
        {
            SetMapStateChanged();
        }

        private void ONPageLayoutControlEventsOnExtentUpdated(
            object displayTransformation, bool sizeChanged, object newEnvelope)
        {
            SetMapStateChanged();
        }


        /// <summary>
        /// ドキュメント保存確認と保存
        /// </summary>
        private bool CheckDocumentSaveOrNot()
        {
            IEngineEditor engineEditor = new EngineEditorClass();
            if((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) &&
                (engineEditor.HasEdits() == true))
            {
                DialogResult result = MessageBox.Show(
                    "編集中の状態を保存しますか？",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.No:
                        engineEditor.StopEditing(false);
                        break;

                    case DialogResult.Yes:
                        engineEditor.StopEditing(true);
                        break;
                }
            }

            bool doSave = false;
            //if (m_MapControl.LayerCount > 0 && this.MainMapChanged)
            if (this.MainMapChanged)
            {
                DialogResult res = MessageBox.Show(
                    "現在のドキュメントを保存しますか?",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (res)
                {
                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.No:
                        break;

                    case DialogResult.Yes:
                        doSave = true;
                        break;
                }
            }

            // 編集開始状態の確認
            if(engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                engineEditor.StopEditing(true);
            }

            if(doSave) {
                IMapDocument mapDoc = new MapDocumentClass();
                try {
                    if(m_MapControl.CheckMxFile(axMapControl1.DocumentFilename)) {
                        mapDoc.Open(axMapControl1.DocumentFilename, string.Empty);
                        if(mapDoc.get_IsReadOnly(axMapControl1.DocumentFilename)) {
                            mapDoc.Close();
                            Common.Logger.Info("このドキュメントは読込み専用です");
                            Common.MessageBoxManager.ShowMessageBoxInfo(
                                this, "このドキュメントは読込み専用です");
                            return false;
                        }

                        m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                        m_MapControl.ActiveView.ShowScrollBars = true;

                        mapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                        //mapDoc.ReplaceContents((IMxdContents)m_MapControl.Map);

                        mapDoc.SetActiveView(m_MapControl.ActiveView);

                        mapDoc.Save(true, false);
                        mapDoc.Close();
                        ClearMapChangeCount();
                    }
                    else {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "ArcMapドキュメント (*.mxd)|*.mxd|" +
                                                "ArcMapテンプレート (*.mxt)|*.mxt";

                        DialogResult result = saveFileDialog.ShowDialog(this);

                        if(result == DialogResult.OK) {
                            mapDoc.New(saveFileDialog.FileName);

                            m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                            m_MapControl.ActiveView.ShowScrollBars = true;

                            mapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                            mapDoc.SetActiveView(m_pageLayoutControl.ActiveView);

                            if (saveFileDialog.FilterIndex == 1) // mxd
                            {
                                // mxd保存時
                                mapDoc.SetActiveView(m_MapControl.ActiveView);
                            }

                            mapDoc.Save(true, false);
                            axMapControl1.DocumentFilename = mapDoc.DocumentFilename;
                            mapDoc.Close();

                            axMapControl1.DocumentFilename = saveFileDialog.FileName;
                            string path = System.IO.Path.GetFileName(saveFileDialog.FileName);
                            this.Text = path + " - " +
                                Properties.Resources.CommonMessage_ApplicationName;

                            // 上書き保存有効化
                            menuSaveDoc.Enabled = true;

                            // Toolbarの上書き保存有効化
                            if (m_ToolbarControl != null)
                            {
                                for (int i = 0; i < m_ToolbarControl.Count; i++)
                                {
                                    IToolbarItem ti = m_ToolbarControl.GetItem(i);

                                    if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                                    {
                                        EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                        saveDocumet.EnableCommand();
                                    }
                                }
                            }

                            this.SetEvents();
                            this.ClearMapChangeCount();
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Common.UtilityClass.DoOnError(ex);

                    DialogResult dResult = GISLight10.Common.MessageBoxManager.ShowMessageBoxError2(
                        this, Properties.Resources.MainForm_ErrorWhenMenuSave + "\n" +
                        ex.Message + "\n" +
                        "終了してもよいですか？");

                    if (dResult == DialogResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mapDoc);
                }
            }
            return true;
        }

        #endregion write to add additionaly functionality for the events


        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axToolbarControl1_OnItemClick(object sender, IToolbarControlEvents_OnItemClickEvent e)
        {
            // マップ変更状態でなければ、カレントツールインデクス保持
            if (!map_sts_changed)
            {
                CurrentToolIndex = e.index;

            }
            //Common.Logger.Debug(e.index.ToString());

        }

        /// <summary>
        /// 終了時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 2012/08/28 add >>>>>
            // 開いているジオリファレンスフォームあれば閉じる
            if (this.OwnedForms.Length > 0)
            {
                for (int i = 0; i < this.OwnedForms.Length; i++)
                {
                    if (this.OwnedForms[i].Text.Contains("ジオリファレンス"))
                    {
                        this.OwnedForms[i].Dispose();
                        break;
                    }
                }
            }
            // 2012/08/28 add <<<<<

            if (!CheckDocumentSaveOrNot())
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// マップコントロール描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            if(!this.IsMapVisible) return;
#if DEBUG
			// ﾌｪｰｽﾞを判定
			string	strPhase = "";
			switch(e.viewDrawPhase) {
			case 0:
				strPhase = "None";				break;
			case 1:
				strPhase = "Background";		break;
			case 2:
				strPhase = "Geography";			break;
			case 4:
				strPhase = "GeoSelection";		break;
			case 8:
				strPhase = "Graphics";			break;
			case 16:
				strPhase = "GraphicSelection";	break;
			case 32:
				strPhase = "Foreground";		break;
			case 64:
				strPhase = "Initialized";		break;
			}

			// 表示範囲を取得
			ESRI.ArcGIS.Geometry.IEnvelope	agEnv = this.m_MapControl.ActiveView.Extent;
			Debug.WriteLineIf(Debug_Confirm, string.Format("◆MAPCONTROL EVENT : AfterDraw / EXTENT : L={0}, R={1}, B={2}, T={3} / DrawPhase : {4}", agEnv.XMin, agEnv.XMax, agEnv.YMin, agEnv.YMax, strPhase));
#endif
            if(e.viewDrawPhase == Convert.ToInt32(esriViewDrawPhase.esriViewForeground)) {
                SetMenuItemEnabled();
            }
            //Common.Logger.Debug("axMapControl1_OnAfterDraw " +  e.viewDrawPhase.ToString());
        }

        /// <summary>
        /// MapControlとPageLayoutの切替
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 2011/06/06 編集中はレイアウト切り替え抑止する
            if(this.IsEditMode) return;

            if(this.IsMapVisible) //map view
            {
                //activate the MapControl and deactivate the PageLayoutControl
                //m_controlsSynchronizer.PageLayoutControl.PageLayout = axPageLayoutControl1.PageLayout;

                //2010-12-27 del m_controlsSynchronizer.ActivatePageLayout();
                m_controlsSynchronizer.ActivateMap();

                // メニュー有効
                toolMenuSentaku.Enabled = true;
                toolMenuTableJoinRelate.Enabled = true;

                // 編集有効
                m_ToolbarControl2.Enabled = true;

                // TOCドラッグ有効
                m_TocControl.EnableLayerDragDrop = true;

                // ツールバーの選択状態をクリア
                m_ToolbarControl.CurrentTool = null;
            }
            else //layout view
            {
                //// 編集終了確認
                //DialogResult stopEditingResult;
                //DialogResult saveEditingResult;
                //IEngineEditor engineEditor = new EngineEditorClass();

                //if (engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)
                //{
                //    stopEditingResult = MessageBox.Show(
                //            "編集を終了しますか？",
                //            Properties.Resources.CommonMessage_ApplicationName,
                //            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //    if (stopEditingResult == DialogResult.No)
                //    {
                //        this.IsMapVisible = true;
                //        return;
                //    }
                //    else
                //    {
                //        if (engineEditor.HasEdits() == true)
                //        {
                //            saveEditingResult = MessageBox.Show(
                //                "編集中の状態を保存しますか？",
                //                Properties.Resources.CommonMessage_ApplicationName,
                //                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                //            if (saveEditingResult == DialogResult.Cancel)
                //            {
                //                this.IsMapVisible = true;
                //                return;
                //            }
                //            else if (saveEditingResult == DialogResult.Yes)
                //            {
                //                engineEditor.StopEditing(true);
                //            }
                //            else
                //            {
                //                engineEditor.StopEditing(false);
                //            }
                //        }
                //    }
                //}

                //activate the PageLayoutControl and deactivate the MapControl
                m_controlsSynchronizer.ActivatePageLayout();

                // メニュー無効
                //toolMenuSentaku.Enabled = false;
                //toolMenuTableJoinRelate.Enabled = false;

                // 編集無効
                m_ToolbarControl2.Enabled = false;

                // TOCドラッグ無効
                m_TocControl.EnableLayerDragDrop = false;

                // ツールバーの選択状態をクリア
                m_ToolbarControl.CurrentTool = null;

	            // ｽｸﾛｰﾙ･ﾊﾞｰを有効化
	            m_pageLayoutControl.ActiveView.ShowScrollBars = true;
            }
        }

		// ﾂｰﾙﾊﾞｰの表示調整
		private void FitToolBarWidth(IToolbarControl2 TargetToolbar) {
			// 最後のﾂｰﾙ･ｱｲﾃﾑ描画範囲を取得
			int	intL = 0, intR = 0, intT = 0, intB = 0;
			TargetToolbar.GetItemRect(TargetToolbar.Count - 1, ref intT, ref intL, ref intB, ref intR);

			// ﾂｰﾙﾊﾞｰの幅を調整 ※WidthをintRぴったりにすると、何故か右端のﾂｰﾙが使用できなくなってしまう...
			this.axToolbarControl2.Width = intR + 10;
			// ｼﾞｵ･ﾘﾌｧﾚﾝｽﾂｰﾙ･ﾊﾞｰの位置を調整
			this.axToolbarControl5.Left = this.axToolbarControl2.Width;

			// ﾂｰﾙﾊﾞｰにｸﾞﾗﾃﾞｰｼｮﾝを...
			//TargetToolbar.FillDirection = esriToolbarFillDirection.esriToolbarFillVertical;
			//TargetToolbar.Transparent = true;
			//TargetToolbar.ThemedDrawing = true;
			//TargetToolbar.Appearance = esriControlsAppearance.esri3D;
			//TargetToolbar.BackColor = ColorTranslator.FromHtml("#CCCCCC").ToArgb();
			//TargetToolbar.FadeColor = ColorTranslator.FromHtml("#FCFCFC").ToArgb();
		}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axPageLayoutControl1_OnMouseDown(
            object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {
            try
            {
                if (e.button != 2)
                {
                    SetMapStatusChanged();

                    // 右クリックではない
                    return;
                }

                // 頂点挿入、削除メニューのEnable Switching
                // Call the SetEditLocation method using the OnMouseUp location
                m_EngineEditSketch.SetEditLocation(e.x, e.y);

                if(this.IsEditMode) {
                    m_ToolbarMenuEditStarted.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
                else {
                    m_ToolbarMenu3.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseDown);
            }
        }

        private void axToolbarControl4_OnItemClick(object sender, IToolbarControlEvents_OnItemClickEvent e)
        {
            PageLayoutCurrentToolIndex = e.index;
            Common.Logger.Debug("PageLayout CurrentTool Inde x= " + PageLayoutCurrentToolIndex.ToString());
        }

        /// <summary>
        /// ページレイアウト設定
        /// コマンドクラスで実装し直し 10/18(mon)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPageSetup_Click(object sender, EventArgs e)
        {
            //bool dispMessage = false;
            //try
            //{
            //    //Show the page setup dialog storing the result.
            //    DialogResult result = pageSetupDialog1.ShowDialog();

            //    //set the printer settings of the preview document to the selected printer settings
            //    document.PrinterSettings = pageSetupDialog1.PrinterSettings;

            //    //set the page settings of the preview document to the selected page settings
            //    document.DefaultPageSettings = pageSetupDialog1.PageSettings;

            //    //due to a bug in PageSetupDialog the PaperSize has to be set explicitly by iterating through the
            //    //available PaperSizes in the PageSetupDialog finding the selected PaperSize
            //    //int i;
            //    IEnumerator paperSizes = pageSetupDialog1.PrinterSettings.PaperSizes.GetEnumerator();
            //    paperSizes.Reset();

            //    for (int i = 0; i < pageSetupDialog1.PrinterSettings.PaperSizes.Count; ++i)
            //    {
            //        paperSizes.MoveNext();
            //        if (((PaperSize)paperSizes.Current).Kind == document.DefaultPageSettings.PaperSize.Kind)
            //        {
            //            document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
            //        }
            //    }

            //    /////////////////////////////////////////////////////////////
            //    ///initialize the current printer from the printer settings selected
            //    ///in the page setup dialog
            //    /////////////////////////////////////////////////////////////
            //    IPaper paper = new PaperClass(); //create a paper object

            //    IPrinter printer = new EmfPrinterClass(); //create a printer object
            //    //in this case an EMF printer, alternatively a PS printer could be used

            //    //initialize the paper with the DEVMODE and DEVNAMES structures from the windows GDI
            //    //these structures specify information about the initialization and environment of a printer as well as
            //    //driver, device, and output port names for a printer
            //    paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(
            //        pageSetupDialog1.PageSettings).ToInt32(),
            //        pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

            //    //pass the paper to the emf printer
            //    printer.Paper = paper;

            //    //set the page layout control's printer to the currently selected printer
            //    axPageLayoutControl1.Printer = printer;


            //    if (pageSetupDialog1.PageSettings.PaperSize.PaperName.Contains(
            //        supportPaperSizeName[A4_Index]))
            //    {
            //        this.labelPageSize.Text = supportPaperSizeName[A4_Index];
            //    }
            //    else if (pageSetupDialog1.PageSettings.PaperSize.PaperName.Contains(
            //        supportPaperSizeName[A3_Index]))
            //    {
            //        this.labelPageSize.Text = supportPaperSizeName[A3_Index];
            //    }
            //    else
            //    {
            //        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
            //            this, Properties.Resources.MainForm_PrintLayoutPageSizeInvalid);

            //        this.labelPageSize.Text = pageSetupDialog1.PageSettings.PaperSize.PaperName;
            //        this.buttonPrint.Enabled = false;
            //        this.buttonPrintPreview.Enabled = false;
            //        return;
            //    }

            //    this.buttonPrint.Enabled = true;
            //    this.buttonPrintPreview.Enabled = true;
            //    if (pageSetupDialog1.PageSettings.Landscape)
            //        this.labelPageOrientation.Text = pageLandscape;
            //    else
            //        this.labelPageOrientation.Text = pagePortrait;

            //}
            //catch (COMException comex)
            //{
            //    dispMessage = true;
            //    Common.UtilityClass.DoOnError(comex);
            //}
            //catch (Exception ex)
            //{
            //    dispMessage = true;
            //    Common.UtilityClass.DoOnError(ex);
            //}
            //finally
            //{
            //    if (dispMessage)
            //    {

            //    }
            //}
        }


        /// <summary>
        /// ページレイアウト初期設定
        /// </summary>
        internal void InitPrintSettings()
        {
            //associate the event-handling method with the document's PrintPage event
            this.document.PrintPage +=
                new System.Drawing.Printing.PrintPageEventHandler(document_PrintPage);

            //create a new PageSetupDialog using constructor
            pageSetupDialog1 = new PageSetupDialog();

            // 余白減少バグ対応
            pageSetupDialog1.EnableMetric = true;

            // プリンタ設定非アクティヴ
            //pageSetupDialog1.AllowPrinter = false;

            //initialize the dialog's PrinterSettings property to hold user defined printer settings
            pageSetupDialog1.PageSettings = new System.Drawing.Printing.PageSettings();
            //initialize dialog's PrinterSettings property to hold user set printer settings
            pageSetupDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            //do not show the network in the printer dialog
            pageSetupDialog1.ShowNetwork = false;

            pageSetupDialog1.PageSettings.Margins.Top = 0;
            pageSetupDialog1.PageSettings.Margins.Left = 0;
            pageSetupDialog1.PageSettings.Margins.Bottom = 0;
            pageSetupDialog1.PageSettings.Margins.Right = 0;

            //pageSetupDialog1.AllowMargins = false;

            for(int i = 0; i < supportPaperSizeName.Length; i++) {
	            foreach(PaperSize ps in pageSetupDialog1.PrinterSettings.PaperSizes) {
                    if(ps.PaperName.Contains(supportPaperSizeName[i])) {
                        supportPaperSizeEnable[i] = true;
                        break;
                    }
                }
            }

			//// デフォルト A4 縦
			//if (supportPaperSizeEnable[A4_Index])
			//{
			//    this.labelPageSize.Text = supportPaperSizeName[A4_Index];
			//}
			//else if (supportPaperSizeEnable[A3_Index])
			//{
			//    this.labelPageSize.Text = supportPaperSizeName[A3_Index];
			//}

            pageSetupDialog1.PageSettings.Landscape = false;
			//this.labelPageOrientation.Text = pagePortrait;

            this.axPageLayoutControl1.PageLayout.Page.FormID =
                ESRI.ArcGIS.Carto.esriPageFormID.esriPageFormSameAsPrinter;

            // インストールされているプリンタがある場合
            if(System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count > 0) {
                // ページ設定を開かなくても印刷できるように対応
                // プリンタの設定
                this.document.PrinterSettings = this.pageSetupDialog1.PrinterSettings;
                this.document.DefaultPageSettings = this.pageSetupDialog1.PageSettings;

                //IPaper paper = new PaperClass();
                //IPrinter printer = new EmfPrinterClass();

                //paper.Attach(this.pageSetupDialog1.PrinterSettings.GetHdevmode(
                //    this.pageSetupDialog1.PageSettings).ToInt32(),
                //    this.pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

                //printer.Paper = paper;

                //this.axPageLayoutControl1.Printer = printer;

                this.document.OriginAtMargins = false;

                //mainForm.axPageLayoutControl1.Refresh();
			}

			// 既定のﾌﾟﾘﾝﾀ設定
			this.SetDefaultPrinter();

            // 印刷設定の概要表示を更新
            this.LoadPrintSetting(null);

            // ﾍﾟｰｼﾞにﾌｨｯﾄ
            this.axPageLayoutControl1.PageLayout.ZoomToWhole();
        }

        /// <summary>
        /// APP既定のプリンタ設定を採用します
        /// </summary>
        private void SetDefaultPrinter() {
			IPrinter	agPrinter = new EmfPrinterClass();	//create a printer object
			IPaper		agPaper = new PaperClass();			//create a paper object
			// in this case an EMF printer, alternatively a PS printer could be used

			// initialize the paper with the DEVMODE and DEVNAMES structures from the windows GDI
			// these structures specify information about the initialization
			// and environment of a printer as well as
			// driver, device, and output port names for a printer
			agPaper.Attach(this.pageSetupDialog1.PrinterSettings.GetHdevmode(
				this.pageSetupDialog1.PageSettings).ToInt32(),
				this.pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

			//pass the paper to the emf printer
			agPrinter.Paper = agPaper;


			// ﾍﾟｰｼﾞ設定を取得
			IPage	agPage = this.axPageLayoutControl1.PageLayout.Page;

			// ﾍﾟｰｼﾞに合わせてﾏｯﾌﾟｴﾚﾒﾝﾄのｻｲｽﾞをｽｹｰﾘﾝｸﾞ
			if(!agPage.StretchGraphicsWithPage) {
				agPage.StretchGraphicsWithPage = true;
			}

			// 用紙の向きを設定
			if(agPage.Orientation != agPaper.Orientation) {
				agPage.Orientation = agPaper.Orientation;
			}

			// ﾌﾟﾘﾝﾀの用紙設定を使用
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				agPage.FormID = esriPageFormID.esriPageFormSameAsPrinter;
			}

			// ｻｲｽﾞ調整 (ﾌﾟﾘﾝﾀの用紙設定を使用しない場合、自力で合わせる)
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				double	dblW, dblH;
				agPage.FormID = EngineCommand.PrinitPageLayoutCommand.GetStretchPageSize(agPage, agPaper, out dblW, out dblH);
				if(agPage.FormID == esriPageFormID.esriPageFormCUSTOM) {
					agPage.PutCustomSize(dblW, dblH);
				}
			}

			// ｻｲｽﾞ単位を設定
			if(agPage.Units != esriUnits.esriCentimeters) {
				agPage.Units = esriUnits.esriCentimeters;
			}


            //set the page layout control's printer to the currently selected printer
            this.axPageLayoutControl1.Printer = agPrinter;

            // ﾌﾟﾘﾝﾀの設定を表示

            // 用紙ｻｲｽﾞ
            string	strPaperSizeName = this.pageSetupDialog1.PageSettings.PaperSize.PaperName;
            if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A4_Index])) {
                this.labelPageSize.Text = this.supportPaperSizeName[this.A4_Index];
            }
            else if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A3_Index])) {
                this.labelPageSize.Text = this.supportPaperSizeName[this.A3_Index];
            }
            else {
                // 用紙サイズの制限はしない様に変更
                //ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                //    this.mainForm, Properties.Resources.MainForm_PrintLayoutPageSizeInvalid);

                this.labelPageSize.Text =
                    this.pageSetupDialog1.PageSettings.PaperSize.PaperName;

                //this.ContinuPageLayout = true; // false;
                //return;
            }

            this.ContinuPageLayout = true;

            // 用紙の向き
            if (this.pageSetupDialog1.PageSettings.Landscape)
                this.labelPageOrientation.Text = this.pageLandscape;
            else
                this.labelPageOrientation.Text = this.pagePortrait;

            this.axPageLayoutControl1.Refresh();
        }

        /// <summary>
        /// OS既定のプリンタ設定を採用します (未使用)
        /// </summary>
        private void SetDefaultPrinterSetting() {
			// 既定のﾌﾟﾘﾝﾀ設定を取得
			PrintDocument	prDoc = new System.Drawing.Printing.PrintDocument();

			// 用紙設定を取得
			IPaper			agPaper = new PaperClass();
			agPaper.Attach(prDoc.PrinterSettings.GetHdevmode().ToInt32(), prDoc.PrinterSettings.GetHdevnames().ToInt32());

			// 用紙を設定
			IPrinter	agPrinter = new EmfPrinterClass();
			agPrinter.Paper = agPaper;

			// ﾍﾟｰｼﾞ設定を取得
			IPage	agPage = this.axPageLayoutControl1.PageLayout.Page;

			// ﾍﾟｰｼﾞに合わせてﾏｯﾌﾟｴﾚﾒﾝﾄのｻｲｽﾞをｽｹｰﾘﾝｸﾞ
			if(!agPage.StretchGraphicsWithPage) {
				agPage.StretchGraphicsWithPage = true;
			}

			// 用紙の向きを設定
			if(agPage.Orientation != agPaper.Orientation) {
				agPage.Orientation = agPaper.Orientation;
			}

			// ﾌﾟﾘﾝﾀの用紙設定を使用
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				agPage.FormID = esriPageFormID.esriPageFormSameAsPrinter;
			}

			// ｻｲｽﾞ調整 (ﾌﾟﾘﾝﾀの用紙設定を使用しない場合、自力で合わせる)
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				double	dblW, dblH;
				agPage.FormID = EngineCommand.PrinitPageLayoutCommand.GetStretchPageSize(agPage, agPaper, out dblW, out dblH);
				if(agPage.FormID == esriPageFormID.esriPageFormCUSTOM) {
					agPage.PutCustomSize(dblW, dblH);
				}
			}

			// ｻｲｽﾞ単位を設定
			if(agPage.Units != esriUnits.esriCentimeters) {
				agPage.Units = esriUnits.esriCentimeters;
			}

            // ﾌﾟﾘﾝﾀｰを設定
            this.axPageLayoutControl1.Printer = agPrinter;

            // ﾌﾟﾘﾝﾀ設定を表示

            // 用紙ｻｲｽﾞ
            string	strPaperSizeName = this.axPageLayoutControl1.Printer.Paper.FormName;
            if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A4_Index])) {
				this.labelPageSize.Text = this.supportPaperSizeName[this.A4_Index];
            }
            else if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A3_Index])) {
				this.labelPageSize.Text = this.supportPaperSizeName[this.A3_Index];
            }
            else {
				this.labelPageSize.Text = strPaperSizeName;
            }

            this.ContinuPageLayout = true;

            // 用紙の向き
            if(agPaper.Orientation == 2) {
				this.labelPageOrientation.Text = this.pageLandscape;
            }
            else {
				this.labelPageOrientation.Text = this.pagePortrait;
            }

            // 設定を反映
            this.axPageLayoutControl1.Refresh();
        }

        /// <summary>
        /// 印刷プレビュー
        /// コマンドクラスで実装　１０／１８（MON)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrintPreview_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                //// create a new PrintPreviewDialog using constructor
                //printPreviewDialog1 = new PrintPreviewDialog();
                //printPreviewDialog1.ShowIcon = false;


                ////initialize the currently printed page number
                //m_CurrentPrintPage = 0;

                ////check if a document is loaded into PageLayout	control
                ////if (axPageLayoutControl1.DocumentFilename == null) return;
                ////if (m_pageLayoutControl.DocumentFilename == null) return;
                //if (m_controlsSynchronizer.MapControl.DocumentFilename == null)
                //{
                //    dispMessage = true;
                //    return;
                //}

                ////set the name of the print preview document to the name of the mxd doc
                //document.DocumentName = m_controlsSynchronizer.MapControl.DocumentFilename;

                ////set the PrintPreviewDialog.Document property to the PrintDocument object selected by the user
                //printPreviewDialog1.Document = document;

                ////show the dialog - this triggers the document's PrintPage event
                //printPreviewDialog1.ShowDialog(this);
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

                }
            }

        }



        /// <summary>
        ///  this code will be called when the PrintPreviewDialog.Show method is called
        ///  set the PageToPrinterMapping property of the Page. This specifies how the page
        ///  is mapped onto the printer page. By default the page will be tiled
        ///  get the selected mapping option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            short printPageCount = (short)0.0;
            int hInfoDC = 0;
            IntPtr hdc = new IntPtr();

            try
            {
                //string sPageToPrinterMapping = (string)this.comboBox1.SelectedItem;
                //if (sPageToPrinterMapping == null)
                //if no selection has been made the default is tiling

                //axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;

                //else if (sPageToPrinterMapping.Equals("esriPageMappingTile"))1
                //    axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;
                //else if (sPageToPrinterMapping.Equals("esriPageMappingCrop"))
                //    axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingCrop;
                //else if (sPageToPrinterMapping.Equals("esriPageMappingScale"))

                axPageLayoutControl1.Page.PageToPrinterMapping =
                        esriPageToPrinterMapping.esriPageMappingScale;

                //else
                //    axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;

                //get the resolution of the graphics device used by the print preview (including the graphics device)
                short dpi = (short)e.Graphics.DpiX;

                //envelope for the device boundaries
                ESRI.ArcGIS.Geometry.IEnvelope devBounds = new ESRI.ArcGIS.Geometry.EnvelopeClass();

                //get page
                IPage page = axPageLayoutControl1.Page;

                //the number of printer pages the page will be printed on
                printPageCount = axPageLayoutControl1.get_PrinterPageCount(0);
                m_CurrentPrintPage++;

                //the currently selected printer
                IPrinter printer = axPageLayoutControl1.Printer;

                //get the device bounds of the currently selected printer
                page.GetDeviceBounds(printer, m_CurrentPrintPage, 0, dpi, devBounds);

                //Returns the coordinates of lower, left and upper, right corners
                double xmin, ymin, xmax, ymax;
                devBounds.QueryCoords(out xmin, out ymin, out xmax, out ymax);

                //structure for the device boundaries
                tagRECT deviceRect;

                deviceRect.bottom = 0;
                deviceRect.left = 0;
                deviceRect.top = 0;
                deviceRect.right = 0;

                //initialize the structure for the device boundaries
                if (isPreviewPage == true)
                {
                    deviceRect.bottom = (int)ymax;
                    deviceRect.left = (int)xmin;
                    deviceRect.top = (int)ymin;
                    deviceRect.right = (int)xmax;
                }

                // Get the printer's hDC, so we can use the Win32 GetDeviceCaps function to
                //  get Printer's Physical Printable Area x and y margins
                hInfoDC = CreateDC(printer.DriverName, printer.Paper.PrinterName, "", IntPtr.Zero);

                if (isPrintPage == true)
                {
                    deviceRect.bottom = (int)(ymax - GetDeviceCaps(hInfoDC, 113));
                    deviceRect.left = (int)(xmin - GetDeviceCaps(hInfoDC, 112));
                    deviceRect.top = (int)(ymin - GetDeviceCaps(hInfoDC, 113));
                    deviceRect.right = (int)(xmax - GetDeviceCaps(hInfoDC, 112));
                }

                devBounds.PutCoords(0, 0, deviceRect.right - deviceRect.left, deviceRect.bottom - deviceRect.top);

                //determine the visible bounds of the currently printed page
                ESRI.ArcGIS.Geometry.IEnvelope visBounds = new ESRI.ArcGIS.Geometry.EnvelopeClass();
                page.GetPageBounds(printer, m_CurrentPrintPage, 0, visBounds);

                //get a handle to the graphics device that the print preview will be drawn to
                hdc = e.Graphics.GetHdc();

                //print the page to the graphics device using the specified boundaries
                axPageLayoutControl1.ActiveView.Output(
                    hdc.ToInt32(), dpi, ref deviceRect, visBounds, m_TrackCancel);

            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.Logger.Debug("COM Exception On document_PrintPage UsingMemorySize GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Debug("UsingMemorySize GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                MessageBox.Show(ex.Message, "印刷時の予期せぬエラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Common.Logger.Fatal(ex.Source);
                Common.Logger.Fatal(ex.Message);
                Common.Logger.Fatal(ex.StackTrace);
            }
            finally
            {
                //release the graphics device handle
                e.Graphics.ReleaseHdc(hdc);

                //release the DC...
                ReleaseDC(0, hInfoDC);

                //check if further pages have to be printed
                if (m_CurrentPrintPage < printPageCount)
                    e.HasMorePages = true; //document_PrintPage event will be called again
                else
                    e.HasMorePages = false;
            }
        }

        /// <summary>
        /// 印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                printDialog1 = new PrintDialog(); //create a print dialog object

                //allow the user to choose the page range to be printed
                printDialog1.AllowSomePages = true;
                //show the help button.
                printDialog1.ShowHelp = true;

                //set the Document property to the PrintDocument for which the PrintPage Event
                //has been handled. To display the dialog, either this property or the
                //PrinterSettings property must be set
                printDialog1.Document = document;

                //show the print dialog and wait for user input
                DialogResult result = printDialog1.ShowDialog();

                // If the result is OK then print the document.
                if (result == DialogResult.OK) document.Print();
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

                }
            }
        }

        /// <summary>
        /// ページレイアウトのリセット
        /// コマンドクラスで実装し直し１０．１８（MON)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResetLayout_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                //ClearPageLayout();

                //// restore when map document open saved pagelayout
                //this.axPageLayoutControl1.PageLayout = savePageLayout;

                //m_controlsSynchronizer.ReplaceMap(this.axMapControl1.Map);
                //m_controlsSynchronizer.ActivatePageLayout();

                //buttonPrint.Enabled = true;
                //buttonPrintPreview.Enabled = true;
                //InitPrintSettings();

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

                }
            }

        }

        /// <summary>
        /// 名前を付けて保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            IMapDocument pMapDoc = new MapDocumentClass();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "ArcMapドキュメント (*.mxd)|*.mxd|" +
                                        "ArcMapテンプレート (*.mxt)|*.mxt";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    pMapDoc.New(saveFileDialog.FileName);

                    m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                    m_MapControl.ActiveView.ShowScrollBars = true;

                    pMapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                    pMapDoc.SetActiveView(m_pageLayoutControl.ActiveView);

                    if (saveFileDialog.FilterIndex == 1) // mxd
                    {
                        // mxd保存時
                        pMapDoc.SetActiveView(m_MapControl.ActiveView);
                    }

                    pMapDoc.Save(true, false);
                    axMapControl1.DocumentFilename = pMapDoc.DocumentFilename;
                    pMapDoc.Close();

                    axMapControl1.DocumentFilename = saveFileDialog.FileName;
                    string path = System.IO.Path.GetFileName(saveFileDialog.FileName);
                    this.Text = path + " - " +
                        Properties.Resources.CommonMessage_ApplicationName;

                    // 上書き保存有効化
                    menuSaveDoc.Enabled = true;

                    // Toolbarの上書き保存有効化
                    if (m_ToolbarControl != null)
                    {
                        for (int i = 0; i < m_ToolbarControl.Count; i++)
                        {
                            IToolbarItem ti = m_ToolbarControl.GetItem(i);

                            if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                            {
                                EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                saveDocumet.EnableCommand();
                            }
                        }
                    }

                    this.SetEvents();
                    this.ClearMapChangeCount();
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMenuSave);

            }
            finally
            {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pMapDoc);
            }
        }

        /// <summary>
        /// ページレイアウト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axPageLayoutControl1_OnDoubleClick(object sender, IPageLayoutControlEvents_OnDoubleClickEvent e)
        {
            bool dispMessage = false;
            try
            {
                // テキスト修正
                //EditTextElement();

                //SetEditEnableScaleBar();
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

                }
            }
        }

        /// <summary>
        /// ページレイアウト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            try
            {
                string unitName = Common.UtilityClass.GetMapUnitText(axPageLayoutControl1.Page.Units);
                if (unitName.Length == 0)
                {
                    unitName = axPageLayoutControl1.Page.Units.ToString().Substring(4);
                }

                statusBarXY.Text =
                    string.Format(
                    "{0} {1} {2}",
                    e.pageX.ToString("###.##"),
                    e.pageY.ToString("###.##"),
                    unitName);

            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseMove);
            }
        }

        /// <summary>
        /// 新規
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuNewDoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckDocumentSaveOrNot())
                {
                    // 2012/08/24 add >>>>>
                    // 開いているジオリファレンスフォームあれば閉じる
                    if (this.OwnedForms.Length > 0)
                    {
                        for (int i = 0; i < this.OwnedForms.Length; i++)
                        {
                            if (this.OwnedForms[i].Text.Contains("ジオリファレンス"))
                            {
                                this.OwnedForms[i].Dispose();
                                break;
                            }
                        }
                    }
                    // 2012/08/24 add <<<<<

                    ClearPageLayout();

	                // 直前に開いていたﾄﾞｷｭﾒﾝﾄのﾌﾟﾘﾝﾀ設定状態によっては、既定のﾌﾟﾘﾝﾀ設定に戻らない不具合に対応
                    this.InitPrintSettings();

                    ClearMap();

                    SetEvents();
                    SetPageLayoutEvents();

                    ClearMapChangeCount();
                }
            }
            catch (Exception ex)
            {
                Common.Logger.Error(sender.ToString() + " " + ex.Message);
            }
        }

        /// <summary>
        /// 属性テーブル表示状態を取得
        /// </summary>
        /// <returns>属性テーブル表示状態である場合にはtrue</returns>
        internal bool HasFormAttributeTable()
        {
            if (this.OwnedForms.Length > 0)
            {
                for (int i = 0; i < this.OwnedForms.Length; i++)
                {
                    if (this.OwnedForms[i].Text.Contains("属性:"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 2012/08/07 ADD
        /// <summary>
        /// ジオリファレンス機能実行状態を取得
        /// </summary>
        /// <returns>ジオリファレンス機能実行状態である場合にはtrue</returns>
        internal bool HasGeoReference()
        {
            bool retrunVal;
            retrunVal = false;
            if (this.OwnedForms.Length > 0)
            {
                for (int i = 0; i < this.OwnedForms.Length; i++)
                {
                    if (this.OwnedForms[i].Text.Contains("ジオリファレンス"))
                    {
                        retrunVal = true;
                        break;
                    }
                }
            }
            return retrunVal;
        }
        // 2012/08/07 ADD

        /// <summary>
        /// 選択メニュー切り替え
        /// </summary>
        private void toolMenuSentaku_DropDownOpening(object sender, EventArgs e)
        {
            if (HasFormAttributeTable())
            {
                menuSentakuZokukeiKensaku.Enabled = false;
                menuSentakuKukanKensaku.Enabled = false;
                menuSentakuSelectableLayerSettings.Enabled = false;
            }
            else if(!this.IsMapVisible) {
                menuSentakuZokukeiKensaku.Enabled = false;
                menuSentakuKukanKensaku.Enabled = false;
                menuSentakuZokuseichiSyukei.Enabled = false;
                menuSentakuSelectableLayerSettings.Enabled = false;
            }
        }

        /// <summary>
        /// テーブル結合とリレートメニュー切り替え
        /// </summary>
        private void toolMenuTableJoinRelate_DropDownOpening(object sender, EventArgs e)
        {
            if (HasFormAttributeTable())
            {
                menuTableJoin.Enabled = false;
                menuRemoveJoin.Enabled = false;
                menuRelate.Enabled = false;
                menuRemoveRelate.Enabled = false;
            }
            else if(!this.IsMapVisible) {
                menuTableJoin.Enabled = false;
                menuRemoveJoin.Enabled = false;
                menuRelate.Enabled = false;
                menuRemoveRelate.Enabled = false;
            }
        }

        /// <summary>
        /// 演算メニューの切り替え
        /// </summary>
        private void toolMenuCalculate_DropDownOpening(object sender, EventArgs e)
        {
            if (HasFormAttributeTable()) {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
                menuGeoReference.Enabled = false;
            }
            else if(!this.IsMapVisible) {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
                menuGeoReference.Enabled = false;
            }
            else if(this.IsEditMode) {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
                menuGeoReference.Enabled = false;
            }
			else if(this.HasGeoReference()) {
                menuGeoReference.Enabled = false;
			}
			else {
			    int					count_R = 0;	// ﾗｽﾀｰ･ﾚｲﾔｰ数
			    Common.LayerManager	pLayerManager = new GISLight10.Common.LayerManager();

			    // ﾗｽﾀｰのｶｳﾝﾄ
			    count_R = pLayerManager.GetRasterLayers(m_MapControl.Map).Count;
			    menuGeoReference.Enabled = count_R > 0;
			}
        }

        /// <summary>
        /// 編集オプションの設定
        /// </summary>
        private void OnStartEditingMethod() {

            EditOptionSettings m_editOptionSettings = null;
            IEngineEditProperties m_engineEditProp = (IEngineEditProperties)m_EngineEditor;
            IEngineEditProperties2 m_engineEditProp2 = (IEngineEditProperties2)m_EngineEditor;
            IEngineSnapEnvironment m_engineSnapEnv = (IEngineSnapEnvironment)m_EngineEditor;

            //設定ファイルが存在するか確認する
            if (!ApplicationInitializer.IsUserSettingsExists()) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                return;
            }

            try {
                //設定ファイル
                m_editOptionSettings = new EditOptionSettings();
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ 編集のオプション ]" +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ 編集のオプション ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }

            //スナップ許容値を設定
            try {
                m_engineSnapEnv.SnapTolerance = double.Parse(m_editOptionSettings.SnapTolerance);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ スナップ許容値 ]" +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ スナップ許容値 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }


            //移動抑制許容値を設定
            try {
                m_engineEditProp2.StickyMoveTolerance = int.Parse(m_editOptionSettings.StickyMoveTolerance);
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ 移動抑制許容値 ]" +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ 移動抑制許容値 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }


            //ストリーム許容値を設定
            try {
                m_engineEditProp.StreamTolerance = double.Parse(m_editOptionSettings.StreamTolerance);
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                   (this,
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                   "[ ストリーム モード：ストリーム許容値 ]" +
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ ストリーム モード：ストリーム許容値 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }


            //グループ化する頂点数を設定
            try {
                m_engineEditProp.StreamGroupingCount = int.Parse(m_editOptionSettings.StreamGroupingCount);
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                   (this,
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                   "[ ストリーム モード：グループ化する頂点数 ]" +
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ ストリーム モード：グループ化する頂点数 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }



        }

#if DEBUG
        private void OnEditorSelectionChanged() {
			Debug.WriteLine("●●INFO - EditorEvent : SelectionChanged.");
        }
#endif

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // 2011/06/06 編集中はレイアウト切り替え抑止する
            if(this.IsMapVisible) return;

            if(this.IsEditMode) {
                // 切り替え不可
                this.IsMapVisible = true;
                MessageBoxManager.ShowMessageBoxWarining(this, Properties.Resources.MainFrom_WARNING_TabChange);
            }

            // 2012/08/24 ジオリファレンス起動時は切り替え抑止する
            if(HasGeoReference()) {
                // 切り替え不可
                this.IsMapVisible = true;
                MessageBoxManager.ShowMessageBoxWarining(this, Properties.Resources.FormGeoReference_WARNING_ChangeLayoutView);
            }
        }

        /// <summary>
        /// リフレッシュボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshButton_Click(object sender, EventArgs e)
        {
            try {
                ICommand mapRefresh = new ControlsMapRefreshViewCommandClass();

                mapRefresh.OnCreate(this.axMapControl1.Object);
                mapRefresh.OnClick();

                //this.axMapControl1.Refresh();
                //this.axPageLayoutControl1.Refresh();
            }
            catch (Exception ex)
            {
                Common.Logger.Error(sender.ToString() + " " + ex.Message);
            }
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)　{
            // ﾏｳｽ･ｶｰｿﾙを変更
            if(this.Cursor != Cursors.SizeAll) {
				preCursor = this.Cursor;
				this.Cursor = Cursors.SizeAll;
            }

            // ﾘｻｲｽﾞ時のｺﾝﾃﾝﾂ描画更新を軽減する
            if(this.IsMapVisible) {
				axMapControl1.SuppressResizeDrawing(true, this.Handle.ToInt32());
            }
            else {
				this.axPageLayoutControl1.SuppressResizeDrawing(true, this.Handle.ToInt32());
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)　{
            //Stop bitmap draw and draw data
            if(this.IsMapVisible) {
				axMapControl1.SuppressResizeDrawing(false, this.Handle.ToInt32());
            }
            else {
				this.axPageLayoutControl1.SuppressResizeDrawing(false, this.Handle.ToInt32());
            }

            this.Cursor = preCursor;
        }

        private void tlbtnTocClose_ButtonClick(object sender, EventArgs e)
        {
            this.MapContainer.Panel1Collapsed = true;
            this.tlbtnTocClose.Visible = false;
            this.tlbtnTocExpand.Visible = true;
        }

        private void tlbtnTocExpand_Click(object sender, EventArgs e)
        {
            this.MapContainer.Panel1Collapsed = false;
            this.tlbtnTocClose.Visible = true;
            this.tlbtnTocExpand.Visible = false;
        }

        private esriControlsDragDropEffect m_Effect;

        /// <summary>
        /// MapControlへのドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnOleDrop(object sender, IMapControlEvents2_OnOleDropEvent e) {
            IDataObjectHelper dataObject = (IDataObjectHelper)e.dataObjectHelper;
            esriControlsDropAction action = e.dropAction;

            e.effect = (int)esriControlsDragDropEffect.esriDragDropNone;

            if (action == esriControlsDropAction.esriDropEnter)
            {
                //if (dataObject.CanGetFiles() | dataObject.CanGetNames())
                //{
                m_Effect = esriControlsDragDropEffect.esriDragDropCopy;
                //}
                //e.effect = (int)esriControlsDragDropEffect.esriDragDropCopy;
                return;
            }

            if (action == esriControlsDropAction.esriDropOver)
            {
                e.effect = (int)m_Effect;
                //e.effect = (int)esriControlsDragDropEffect.esriDragDropCopy;
                return;
            }

            if(action == esriControlsDropAction.esriDropped) {
#if DEBUG
				Debug.WriteLine("●ドロップ / CanGetFiles : " + (dataObject.CanGetFiles() ? "YES" : "NO"));
#endif
				// ﾌｧｲﾙ･ｼｽﾃﾑからﾄﾞﾛｯﾌﾟされた、MXDを開けるように拡張
				if(dataObject.CanGetFiles()) {
					bool	blnOpened = false;
					System.Array	arrFiles = System.Array.CreateInstance(typeof(string), 0);
					arrFiles = (System.Array)dataObject.GetFiles();
					foreach(string strFile in arrFiles) {
						if(m_MapControl.CheckMxFile(strFile)) {
							if(this.CheckDocumentSaveOrNot()) {
								// ﾏｯﾌﾟﾄﾞｷｭﾒﾝﾄを開く
								this.OpenMXDFile(strFile);
							}
							blnOpened = true;	// 有効なMXDに対応した、という意味で。
							break;
						}
					}
					if(!blnOpened) {
						// ﾒｯｾｰｼﾞを表示
						ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.MainForm_ErrorWhenDataAdd);
					}
				}
                // ﾃﾞｰﾀをﾏｯﾌﾟに追加
				else {
					IDataObject dataObj = null;
					try {
						// ｶﾀﾛｸﾞ･ﾂﾘｰからのみﾃﾞｰﾀを受け付ける
						dataObj = (IDataObject)dataObject.InternalObject;
					}
					catch {
						ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
						(this, Properties.Resources.MainForm_ErrorWhenDataAdd);
						return;
					}

#if DEBUG
	                System.Diagnostics.Debug.WriteLine("●ドロップ / GetDataPresent : " + dataObj.GetType().ToString());
#endif
					if(dataObj.GetDataPresent("ESRI.ArcGIS.Geodatabase.IDatasetName")) {
						try {
							ESRI.ArcGIS.Geodatabase.IDatasetName dsName = (ESRI.ArcGIS.Geodatabase.IDatasetName)dataObj.GetData("ESRI.ArcGIS.Geodatabase.IDatasetName");
							//MessageBox.Show(dsName.Name);
							ESRI.ArcGIS.esriSystem.IName name = (IName)dsName;
							//ESRI.ArcGIS.Geodatabase.IWorkspaceName wsName;
							//ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wsFact;

							switch (dsName.Type) {
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureDataset: // CAD or FeatureDataset
								if(dsName.WorkspaceName.WorkspaceFactoryProgID == "esriDataSourcesFile.CadWorkspaceFactory.1") {
									//addCadLayer(dsName);
									addCadGroupLayer(dsName);
								}
								else {
									addFDatasetLayer(dsName);
								}
								break;
							//case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTCadDrawing:
							//    ESRI.ArcGIS.DataSourcesFile.ICadDrawingDataset cadDataset
							//        = (ESRI.ArcGIS.DataSourcesFile.ICadDrawingDataset)name.Open();
							//    if (cadDataset == null)
							//        return;
							//    ICadLayer cadLayer = new CadLayerClass();
							//    cadLayer.CadDrawingDataset = cadDataset;
							//    cadLayer.Name = dsName.Name;
							//    this.MapControl.AddLayer(cadLayer, 0);
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass:
								addFeatureLayer(dsName);
								break;
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterCatalog:
								addRasterCatalog(dsName);
								break;
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset:
								addRasterLayer(dsName);
								break;
							case esriDatasetType.esriDTTable:
								// ﾃｰﾌﾞﾙを追加して、ｺﾝﾃﾝﾂ追加 ｲﾍﾞﾝﾄ を叩く
								this.OnActiveViewEventsItemAdded( this.addTableLayer(dsName) );
								break;
							default:
								break;
							}
						}
						catch(Exception ex) {
							ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
								(this, Properties.Resources.MainForm_ErrorWhenLayerAdd);
							return;
						}
					}
					else if(dataObj.GetDataPresent("ESRI.ArcGIS.esriSystem.IFileName")) {
						try {
							ESRI.ArcGIS.esriSystem.IFileName fileName = (ESRI.ArcGIS.esriSystem.IFileName)dataObj.GetData("ESRI.ArcGIS.esriSystem.IFileName");
							string	strExt = Path.GetExtension(fileName.Path).ToLower();
							if(strExt.Equals(".lyr")) {
								addLayerFile(fileName);
							}
							else if(strExt.Equals(".mxd")) {
								if(m_MapControl.CheckMxFile(fileName.Path)) {
									if(this.CheckDocumentSaveOrNot()) {
										// ﾏｯﾌﾟﾄﾞｷｭﾒﾝﾄを開く
										this.OpenMXDFile(fileName.Path);
									}
								}
								else {
									// ﾒｯｾｰｼﾞを表示
									ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.MainForm_ErrorWhenDataAdd);
								}
							}
						}
						catch(Exception ex) {
							ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
								(this, Properties.Resources.MainForm_ErrorWhenLayerAdd);
						}
					}
					// ﾃﾞｰﾀﾍﾞｰｽ･ﾃｰﾌﾞﾙ
					else if(dataObj.GetDataPresent("ESRIJapan.GISLight10.Common.UserSelectQueryTableSet")) {
						Common.UserSelectQueryTableSet	objDB = dataObj.GetData("ESRIJapan.GISLight10.Common.UserSelectQueryTableSet") as Common.UserSelectQueryTableSet;
						// ﾃｰﾌﾞﾙを追加して、ｺﾝﾃﾝﾂ追加 ｲﾍﾞﾝﾄ を叩く
						this.OnActiveViewEventsItemAdded( this.addDBTableLayer(objDB) );
					}
				}
            }
        }


        /// <summary>
        /// FeatureLayerの追加
        /// </summary>
        /// <param name="dsName"></param>
        private void addFeatureLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IFeatureWorkspace featWs = null;
            IFeatureClass featClass = null;

            try
            {

                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                featWs = (IFeatureWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                featClass = featWs.OpenFeatureClass(dsName.Name);

                string name = featClass.AliasName;
                if (name.Length == 0)
                {
                    name = dsName.Name;
                }

                IFeatureLayer featLayer = null;
                if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    IFeatureDataset featDataset = featClass.FeatureDataset;
                    IAnnotationLayerFactory annoLayerFact = new FDOGraphicsLayerFactoryClass();
                    IAnnotationLayer annoLayer = annoLayerFact.OpenAnnotationLayer(featWs, featDataset, dsName.Name);
                    featLayer = (IFeatureLayer)annoLayer;
                }
                else
                {
                    featLayer = new FeatureLayerClass();
                }

                featLayer.FeatureClass = featClass;
                featLayer.Name = name;

                this.m_MapControl.AddLayer(featLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, featLayer, m_MapControl.ActiveView.Extent.Envelope);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (featWs != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(featWs);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// CadLayerの追加
        /// </summary>
        /// <param name="dsName"></param>
        private void addCadLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            ICadDrawingWorkspace cadWs = null;
            ICadDrawingDataset cadDs = null;
            ICadLayer cadLayer = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                cadWs = (ICadDrawingWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                cadDs = cadWs.OpenCadDrawingDataset(dsName.Name);
                cadLayer = new CadLayerClass();
                cadLayer.CadDrawingDataset = cadDs;
                cadLayer.Name = dsName.Name;
                this.m_MapControl.AddLayer(cadLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, cadLayer, m_MapControl.ActiveView.Extent.Envelope);

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (cadWs != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(cadWs);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }

        }

        /// <summary>
        /// CadLayerをグループレイヤとして追加
        /// </summary>
        /// <param name="dsName"></param>
        private void addCadGroupLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IFeatureWorkspace fws = null;
            IFeatureDataset featDs = null;
            IGroupLayer gLayer = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                fws = (IFeatureWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                featDs = fws.OpenFeatureDataset(dsName.Name);
                gLayer = new GroupLayerClass();
                gLayer.Name = dsName.Name;

                IEnumDataset enumDs = featDs.Subsets;
                enumDs.Reset();
                IDataset ds = enumDs.Next();
                while (ds != null)
                {
                    if (ds is IFeatureClass)
                    {
                        IFeatureLayer cadFeatLayer = new FeatureLayerClass();
                        cadFeatLayer.FeatureClass = (IFeatureClass)ds;
                        string name = ((IFeatureClass)ds).AliasName;
                        if (name.Length == 0)
                        {
                            name = dsName.Name;
                        }
                        cadFeatLayer.Name = name;
                        gLayer.Add(cadFeatLayer);
                    }

                    ds = enumDs.Next();
                }

                this.m_MapControl.AddLayer(gLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, gLayer, m_MapControl.ActiveView.Extent.Envelope);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (fws != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(fws);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// FeatureDataset内のフィーチャクラスをそれぞれレイヤとして追加
        /// </summary>
        /// <param name="dsName"></param>
        private void addFDatasetLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IFeatureWorkspace fws = null;
            IFeatureDataset featDs = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                fws = (IFeatureWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                featDs = fws.OpenFeatureDataset(dsName.Name);

                IEnumDataset enumDs = featDs.Subsets;
                enumDs.Reset();
                IDataset ds = enumDs.Next();
                while (ds != null)
                {
                    if (ds is IFeatureClass)
                    {
                        IFeatureClass featClass = (IFeatureClass)ds;
                        IFeatureLayer featLayer = null;
                        if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            IAnnotationLayerFactory annoLayerFact = new FDOGraphicsLayerFactoryClass();
                            IAnnotationLayer annoLayer = annoLayerFact.OpenAnnotationLayer(fws, featDs, ds.Name);
                            featLayer = (IFeatureLayer)annoLayer;
                        }
                        else
                        {
                            featLayer = new FeatureLayerClass();
                        }

                        featLayer.FeatureClass = featClass;//(IFeatureClass)ds;

                        string name = ((IFeatureClass)ds).AliasName;
                        if (name.Length == 0)
                        {
                            name = dsName.Name;
                        }
                        featLayer.Name = name;
                        this.m_MapControl.AddLayer(featLayer, 0);
                        this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, featLayer, m_MapControl.ActiveView.Extent.Envelope);
                    }

                    ds = enumDs.Next();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (fws != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(fws);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }

        }

        /// <summary>
        /// 旧 RasterLayerの追加
        /// </summary>
        /// <param name="dsName"></param>
        private void addRasterLayer(IDatasetName dsName) {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IRasterLayer rasLayer = null;
            IRasterWorkspace rasWs = null;
            IRasterWorkspaceEx rasWsEx = null;

            try {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;

                if(wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesRaster.RasterWorkspaceFactory")) {
                    rasWs = (IRasterWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                    rasLayer = new RasterLayerClass();
                    IRasterDataset rasDs = rasWs.OpenRasterDataset(dsName.Name);
                    //IRaster raster = rasDs.CreateDefaultRaster();
                    //rasLayer.CreateFromDataset(rasWs.OpenRasterDataset(dsName.Name));
                    //rasLayer.CreateFromRaster(raster);
                    rasLayer.CreateFromDataset(rasDs);
                }
                else if (wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.AccessWorkspaceFactory") ||
                    wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.FileGDBWorkspaceFactory"))
                {
                    rasWsEx = (IRasterWorkspaceEx)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                    rasLayer = new RasterLayerClass();
                    //rasLayer.CreateFromDataset(rasWsEx.OpenRasterDataset(dsName.Name));
                    IRasterDataset rasDsEx = rasWsEx.OpenRasterDataset(dsName.Name);
                    //IRaster rasterEx = rasDsEx.CreateDefaultRaster();
                    //rasLayer.CreateFromRaster(rasterEx);
                    rasLayer.CreateFromDataset(rasDsEx);
                }
                else {
                    throw new Exception();
                }

                if(rasLayer != null) {
                    this.m_MapControl.AddLayer(rasLayer, 0);
                    this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, rasLayer, m_MapControl.ActiveView.Extent.Envelope);
                }
            }
            catch (Exception ex) {
                throw;
            }
            finally {
                if (rasWsEx != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rasWsEx);
                if (rasWs != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rasWs);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// GDB内のRasterCatalogの追加
        /// </summary>
        /// <param name="dsName"></param>
        private void addRasterCatalog(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IRasterWorkspaceEx rasWsEx = null;
            IRasterCatalog rasCatalog = null;
            IGdbRasterCatalogLayer rasCatalogLayer = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                rasWsEx = (IRasterWorkspaceEx)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                rasCatalog = rasWsEx.OpenRasterCatalog(dsName.Name);
                rasCatalogLayer = new GdbRasterCatalogLayerClass();
                rasCatalogLayer.Setup((ITable)rasCatalog);
                this.m_MapControl.AddLayer((ILayer)rasCatalogLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, rasCatalogLayer, m_MapControl.ActiveView.Extent.Envelope);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (rasWsEx != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rasWsEx);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// LayerFileの追加
        /// </summary>
        /// <param name="fileName"></param>
        private void addLayerFile(IFileName fileName) {
            //if(System.IO.File.Exists(fileName.Path))
                this.m_MapControl.AddLayerFromFile(fileName.Path, 0);
        }


        /// <summary>
        /// テーブル・レイヤーを追加します
        /// </summary>
        /// <param name="agDSName"></param>
        /// <returns></returns>
        private IStandaloneTable addTableLayer(IDatasetName agDSName) {
			IStandaloneTable	agStdTbl = null;
			IWorkspaceName		agWSName = null;
			IWorkspace			agWS = null;
			ITable				agTbl;

            try {
				// ﾜｰｸｽﾍﾟｰｽを取得
                agWSName = agDSName.WorkspaceName;
                IWorkspaceFactory	agWSF = agWSName.WorkspaceFactory;
                agWS = agWSF.OpenFromFile(agWSName.PathName, this.Handle.ToInt32());

                if(agWS is ISqlWorkspace) {
					ISqlWorkspace				agSqlWS = agWS as ISqlWorkspace;
					Common.QueryTableOperator	clsQTO = new QueryTableOperator(agSqlWS);
					agTbl = clsQTO.GetTable(null);
				}
				else {
					IFeatureWorkspace	agFWS = agWS as IFeatureWorkspace;
					agTbl = agFWS.OpenTable(agDSName.Name);
                }

				// ﾃｰﾌﾞﾙを追加
				agStdTbl = this.AddStandaloneTable(agTbl);
                if(agStdTbl == null) {
					Common.MessageBoxManager.ShowMessageBoxError("テーブルを追加できませんでした。");
                }
            }
            catch(Exception ex) {
                throw ex;
            }
            finally {
                if(agWS != null) {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agWS);
                }
                if(agWSName != null) {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agWSName);
                }
            }

            return agStdTbl;
        }

        // 同名の単独テーブルの有無を確認します
        private bool IsExistStandaloneTable(string TableName) {
			bool	blnRet = false;

			IStandaloneTableCollection	agStdTblColl = m_MapControl.Map as IStandaloneTableCollection;
			if(agStdTblColl.StandaloneTableCount > 0) {
				IStandaloneTable agStdTbl = null;
				for(int intCnt=0; intCnt < agStdTblColl.StandaloneTableCount; intCnt++) {
					agStdTbl = agStdTblColl.get_StandaloneTable(intCnt);
					if(agStdTbl.Name.Equals(TableName)) {
						blnRet = true;
						break;
					}
				}
			}

			return blnRet;
        }

        // 単独テーブルを追加します
        private IStandaloneTable AddStandaloneTable(ITable TargetTable) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return Common.StandAloneTableOpener.AddStandaloneTable(TargetTable, m_MapControl.Map);
        }

        /// <summary>
        /// クエリ テーブルを追加します
        /// </summary>
        /// <param name="DBTableSet"></param>
        /// <returns></returns>
        private IStandaloneTable addDBTableLayer(Common.UserSelectQueryTableSet DBTableSet) {
			IStandaloneTable	agStdTbl = null;

			// DB接続
			//ISqlWorkspace	agSQLWS = ConnectionFilesManager.LoadWorkspace(DBTableSet.ConnectProperty) as ISqlWorkspace;
			ISqlWorkspace	agSQLWS = ConnectionFilesManager.LoadWorkspace(DBTableSet.ConnectionFile) as ISqlWorkspace;
			if(agSQLWS != null) {
				// ﾕｰｻﾞｰにｷｰ･ﾌｨｰﾙﾄﾞを指定してもらう
				Ui.FormAddQueryLayer	formAQLayer = new FormAddQueryLayer(agSQLWS) {
					TableName = DBTableSet.TableName,
				};
				if(formAQLayer.ShowDialog() == DialogResult.OK) {
					//// ﾃｰﾌﾞﾙの概要を取得
					//IQueryDescription	agQDesc = agSQLWS.GetQueryDescription(formAQLayer.QueryString);
					//// ｷｰ･ﾌｨｰﾙﾄﾞを設定
					//agQDesc.OIDFields = string.Join(",", formAQLayer.OIDFields);

					//// ﾃｰﾌﾞﾙ名を調整
					//string	strTblName = DBTableSet.TableName;
					////agSQLWS.CheckDatasetName(strTblName, agQDesc, out strTblName);

					//// ﾃｰﾌﾞﾙを追加
					//agStdTbl = this.AddStandaloneTable(agSQLWS.OpenQueryClass(strTblName, agQDesc));
					agStdTbl = this.AddStandaloneTable(formAQLayer.GetQueryTable(null));
					if(agStdTbl == null) {
						Common.MessageBoxManager.ShowMessageBoxError("テーブルを追加できませんでした。");
					}
				}

				ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agSQLWS);
			}

			return agStdTbl;
        }

		/// <summary>
		/// マップのページサイズ表記を取得します
		/// </summary>
		/// <param name="PageFormID"></param>
		/// <returns></returns>
		public string GetPageSizeDescript(esriPageFormID PageFormID) {
			string	strRet = "";

			// ﾌﾟﾘﾝﾀの用紙設定を使用 時
			if(PageFormID == esriPageFormID.esriPageFormSameAsPrinter) {
				// 既定のﾌﾟﾘﾝﾀの用紙ｻｲｽﾞを取得
				strRet = this.axPageLayoutControl1.Printer.Paper.FormName;
			}
			else {
				// ｻｲｽﾞ表記を初期化
				this.InitPageSizes();

				// ｻｲｽﾞ表記を取得
				int	intFormID = PageFormID.GetHashCode();
				if(this._dicPageSizes.ContainsKey(intFormID)) {
					strRet = this._dicPageSizes[intFormID];
				}
				else {
					strRet = " ? ";
				}
			}

			return strRet;
		}

		/// <summary>
		/// プリント設定（マップ・ページ）を識別します
		/// </summary>
		public void LoadPrintSetting(IPageLayout agPageLayout) {
			string	strToolTipPre = "";

			// ﾍﾟｰｼﾞ設定を取得
			IPage	agPage;
			if(agPageLayout != null) {
				agPage = agPageLayout.Page;
			}
			else {
				agPage = this.axPageLayoutControl1.PageLayout.Page;
			}

			// ﾌﾟﾘﾝﾀの用紙設定を使用
			if(agPage.FormID == esriPageFormID.esriPageFormSameAsPrinter) {
				strToolTipPre = "用紙";

				// 用紙ｻｲｽﾞを識別
				IPaper	agPaper = this.axPageLayoutControl1.Printer.Paper;
				//this.labelPageSize.Text = this.GetPageSizeDescript(agPage.FormID);
				this.labelPageSize.Text = agPaper.FormName;

				// 用紙方向を識別
				if(agPaper.Orientation == 2) {
					this.labelPageOrientation.Text = this.pageLandscape;
				}
				else {
					this.labelPageOrientation.Text = this.pagePortrait;
				}
			}
			// ﾕｰｻﾞｰ･ｾｯﾃｨﾝｸﾞ
			else {
				strToolTipPre = "ページ";

				// ﾏｯﾌﾟのﾍﾟｰｼﾞｻｲｽﾞを表示
				this.labelPageSize.Text = this.GetPageSizeDescript(agPage.FormID);

				// ﾍﾟｰｼﾞの向き
				if(agPage.Orientation == 2) {
					this.labelPageOrientation.Text = this.pageLandscape;
				}
				else {
					this.labelPageOrientation.Text = this.pagePortrait;
				}
			}

			// ﾂｰﾙﾁｯﾌﾟを表示
			this.toolTip1.SetToolTip(this.labelPageSize, strToolTipPre + "サイズ : " + this.labelPageSize.Text);
			this.toolTip1.SetToolTip(this.labelPageOrientation, strToolTipPre + "の向き : " + this.labelPageOrientation.Text);
		}

		/// <summary>
		/// プリンタ設定を復元します
		/// </summary>
		/// <param name="DocPrinter"></param>
		/// <returns></returns>
		public bool LoadPrintDocument(IPrinter DocPrinter) {
			bool	blnRet = true;

			try {
				// ﾚｲｱｳﾄのﾌﾟﾘﾝﾀを設定
				this.axPageLayoutControl1.Printer = DocPrinter;

				// ﾌﾟﾘﾝﾀ設定をﾛｰﾄﾞ
				IPaper	agPaper = DocPrinter.Paper;

				// ﾌﾟﾘﾝﾄ･ﾄﾞｷｭﾒﾝﾄの設定
				this.document.PrinterSettings.PrinterName = agPaper.PrinterName;
				this.document.DefaultPageSettings.Landscape = agPaper.Orientation == 2;

				// 用紙ｻｲｽﾞを探索
				PaperSize	psDef = null;
				foreach(PaperSize ps in this.document.PrinterSettings.PaperSizes) {
					if(ps.PaperName.Equals(agPaper.FormName)) {
						psDef = ps;
						break;
					}
				}
				if(psDef != null) {
					this.document.DefaultPageSettings.PaperSize = psDef;
				}
			}
			catch(Exception ex) {
				blnRet = false;
				//GISLight10.Common.MessageBoxManager.ShowMessageBoxError("保存されているプリンタの設定が失敗しました。");
			}

			return blnRet;
		}

		/// <summary>
		/// 起動時に指定されたドキュメント・ファイルを展開します
		/// </summary>
		/// <param name="DocFilePath"></param>
		/// <returns></returns>
		private bool OpenMXDFile(string DocFilePath) {
			bool	blnRet = false;

            for(int intCnt=0; intCnt < m_ToolbarControl.Count; intCnt++) {
                IToolbarItem ti = m_ToolbarControl.GetItem(intCnt);

                // MXDｵｰﾌﾟﾝ･ｺﾏﾝﾄﾞを探索
                if(ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.OpenDocument") {
                    var cmdOpemDoc = ti.Command as EngineCommand.OpenDocument;
                    if(cmdOpemDoc.OpenDocFile(DocFilePath, this.MapControl as IMapControl3)) {
						blnRet = true;
					}
					else {
                        // ｴﾗｰ表示
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this, "ドキュメントを開けません。");
                    }
                    break;
                }
            }

            return blnRet;
		}

		/// <summary>
		/// 指定のツールを有効にします (ToolBar1 ONLY)
		/// </summary>
		public void SetElementSelectTool(string ToolName) {
			// 対象ﾂｰﾙを取得 (ﾂｰﾙﾊﾞｰ1)
			if(m_ToolbarControl != null) {
				// ｸﾞﾗﾌｨｯｸ選択ﾂｰﾙを起動
				string	strCurToolName = ToolName;
				for(int intCnt=0; intCnt < m_ToolbarControl.Count; intCnt++) {
					var agTool = m_ToolbarControl.GetItem(intCnt);
					if(agTool.Command != null && agTool.Command.Name.Equals(strCurToolName)) {
						// Map
						if(this.IsMapVisible) {
							this.m_MapControl.CurrentTool = agTool.Command as ITool;
						}
						// PageLayout
						else {
							this.m_pageLayoutControl.CurrentTool = agTool.Command as ITool;
						}
					}
				}
			}
		}
		/// <summary>
		/// エレメント選択ツールを設定します
		/// </summary>
		public void SetElementSelectTool() {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			this.SetElementSelectTool("ControlToolsGraphicElement_SelectTool");
		}


        // 2020/03/17
        /// <summary>
        /// ActiveX Control のリサイズ
        /// Workaround for controls display issues in 96 versus 120 dpi
        /// https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#Controls92DPIWorkaround.htm
        /// </summary>
        /// <param name="controlToAdjust"></param>
        private void AdjustBounds(AxHost controlToAdjust)
        {
            if (this.CurrentAutoScaleDimensions.Width != 6F)
            {
                //Adjust location: ActiveX control doesn't do this by itself 
                controlToAdjust.Left = Convert.ToInt32(controlToAdjust.Left * this.CurrentAutoScaleDimensions.Width / 6F);
                //Undo the automatic resize... 
                controlToAdjust.Width = controlToAdjust.Width / DPIX() * 96;
                //...and apply the appropriate resize
                controlToAdjust.Width = Convert.ToInt32(controlToAdjust.Width * this.CurrentAutoScaleDimensions.Width / 6F);
            }

            if (this.CurrentAutoScaleDimensions.Height != 13F)
            {
                //Adjust location: ActiveX control doesn't do this by itself 
                controlToAdjust.Top = Convert.ToInt32(controlToAdjust.Top * this.CurrentAutoScaleDimensions.Height / 13F);
                //Undo the automatic resize... 
                controlToAdjust.Height = controlToAdjust.Height / DPIY() * 96;
                //...and apply the appropriate resize 
                controlToAdjust.Height = Convert.ToInt32(controlToAdjust.Height * this.CurrentAutoScaleDimensions.Height / 13F);
            }
        }

        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;

        int DPIX()
        {
            return DPI(LOGPIXELSX);
        }

        int DPIY()
        {
            return DPI(LOGPIXELSY);
        }

        int DPI(int logPixelOrientation)
        {
            int displayPointer = CreateDC("DISPLAY", null, null, IntPtr.Zero);
            return Convert.ToInt32(GetDeviceCaps(displayPointer, logPixelOrientation));
        }
    }
}