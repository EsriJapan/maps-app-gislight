#define doNotSubThread
#define doSetColorSentouLayer

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// シンボルの設定
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成 
    ///  2011/01/11 graphicsの解放処理を追加 
    ///  2011/04/13 個別値,数値分類リストビュー上で選択シンボルの塗潰し色の変更機能追加
    ///  2011/05/11 個別値リストビュー上で選択シンボルの削除機能追加
    /// </history>
    public partial class FormSymbolSettings : Form
    {
        // 2011/04/13 個別値,数値分類色変更 -->
        private const int COL_MENU = 0;
        private const int DEL_MENU = 1;
        //<--

        // エンターキーの場合のプレビュー更新処理の抑止判定フラグ
        private bool Pass_Preview_Process = false;
        private bool Pass_Preview_Process_Escape = false;

        private bool TabChanged = false;

        // スタイルフォーム
        //private FormStyleGallery symbolFormStyle = null;

        // カラーランプ用スタイルフォーム
        //private FormStyleGallery symbolFormColorRamp = null;

        // サブスレッド用 : シンボル設定機能では、Iteration2において、この機能は使用しない
        // 選択スタイルによっては、サブスレッドからの戻り時に回避策の見つける事がまだ出来ていないエラーになる為。
        private bool DoRetryUniqueValue = false; // 1度でもリトライモードになればサブスレッドでは実行しない
        private int RetryCount = 0;

#if !doNotSubThread
        private System.Threading.Thread threadTask = null;
#endif
        private delegate void RethrowExceptionDelegate(Exception ex);//, string msg);
        private delegate void RethrowCOMExceptionDelegate(COMException comex);//, string msg);

        // サブスレッド実行時プログレスバーフォーム
        //private FormProgressManager frmProgress = null;

        private const int	UNIQUEVALUE_MAX_LIMIT = 5000;		// 個別値最大件数許容制限(絶対）
        private int			UNIQUEVALUE_MAX = 500;              // 個別値最大件数制限(オプション設定から）
        
        private bool ShowUniquValueLimitOverWarning = false;			// 最大許容分割数適用ﾌﾗｸﾞ
        private bool UniquValueLimitOverWarningMessageShowDone = false;	// 許容ｵｰﾊﾞｰ･警告ﾒｯｾｰｼﾞ表示済みﾌﾗｸﾞ

        private const string DEFALULT_COLOR_RAMP_NAME = "Pastels";

        private const int SIMPLE_RENDERER = 0;
        private const int UNIQUEVALUE_RENDERER = 1;
        private const int CLASSBREAK_RENDERER = 2;

        private bool UniqueValueInit = false;
        private bool ClassBreaksInit = false;

        // 指定可能な数値分類数
        private const int MAX_CLASS_BREAK_COUNT = 256;

        private const double DEFALUT_POINT_SIZE = 8.0;
        private const double DEFALUT_BREAK_COUNT = 5;

        // ｸﾗｽ･ﾌﾞﾚｲｸ数の設定
        private const int CLASSBREAKS_MIN = 1;
        private const int CLASSBREAKS_MAX = 32;

		// 分類手法
        private const string CLASSBREAKS_NATURAL        = "自然分類";
        private const string CLASSBREAKS_EQUAL_INTERVAL = "等間隔分類";
        private const string CLASSBREAKS_QUANTILE       = "等量分類";

        private const string CLASSBREAKS_NATURAL_UID        = "{62144BEA-E05E-11D1-AAAE-00C04FA334B3}";
        private const string CLASSBREAKS_EQUAL_INTERVAL_UID = "{62144BE1-E05E-11D1-AAAE-00C04FA334B3}";
        private const string CLASSBREAKS_QUANTILE_UID       = "{62144BE9-E05E-11D1-AAAE-00C04FA334B3}";

        private enum CLASSBREAK_INDEX 
            { CLASSBREAKS_NATURAL, CLASSBREAKS_EQUAL_INTERVAL, CLASSBREAKS_QUANTILE };

        
        // index1:等間隔分類の場合にはCLASSBREAKS_MAX固定
        private int[]			CLASSBREAKS_BREAKCOUNTS = new int[3] { CLASSBREAKS_MAX, CLASSBREAKS_MAX, CLASSBREAKS_MAX };

        // 数値分類各種の閾値保存用
        private double[][]		CLASSBREAKS_VALUES = new double[3][];

        private int				SCALE_COUNT = 6;			// 小数点以下の有効桁数
        private int				CB_EDITABLE_COLUMN_ID = -1;	// 数値分類・編集可能列ID

        // 個別値の場合のリストビュー表示対象イメージ:サブスレッド内で設定する
        List<Bitmap>			bitmapList = null;

        // 個別値分類設定済みフィールド名称
        private string			UniqueValueSelectedFieldName = null;
        
        // 数値分類値設定済みフィールド名称
        private string			ClassBreaksSelectedFieldName = null;

        // 地図ｺﾝﾄﾛｰﾙ
        private IMapControl3	m_mapControl;
        // ﾒｲﾝ･ﾌｫｰﾑ
        private Ui.MainForm		mainFrm;
        
        // ｼﾝﾎﾞﾙ
        private ISymbol			m_symbol = null;
        private ISymbol			m_symbol_uniquevalue = null;
        private ISymbol			m_symbol_classbreak = null;
        private ISymbol[]		m_symbol_of_renderer = new ISymbol[3];

#if doSetColorSentouLayer
        //// マルチレイヤライン時に他の場合と同様に対処できないスタイル名
        //private string[] m_symbolname_of_renderer = new string[3];
        //private const string multiLineSymbolNijyuDash   = "二重 ダッシュ";
        //private List<string> multiLineSymbolToGetTopLayer = new List<string>();
#endif

		// ﾚﾝﾀﾞﾗｰ設定対象ﾌｨｰﾁｬｰ･ﾚｲﾔｰ
        private IFeatureLayer		m_featureLayer = null;
        private esriGeometryType	m_geometryType;


        // 作業用ﾚﾝﾀﾞﾗｰ
        private IClassBreaksRenderer	m_work_ClassBreaksRenderer = null;
        private IUniqueValueRenderer	m_work_UniqueValueRenderer = null;

        // 2011/05/02 個別色変更対応 -->
		private enum SymbolUpdateOption {
			FROM_STYLE_GALLERY = 0,
			FROM_COLOR_RAMP = 1,
			FROM_UPDATE_PREVIEW_DEFAULT = 2
		}

        private List<IColor> m_work_UniqueValueRendererEachColor = null;
        private List<IColor> m_work_ClassBreaksRendererEachColor = null;

        private List<SelectedSymbolInfo> m_work_UniqueValueRendererEachInfo = null;
        private List<SelectedSymbolInfo> m_work_ClassBreaksRendererEachInfo = null;

        // 個別削除対応: 個別値
        private List<bool> m_work_UniqueValueRendererSetOthersSymbol = null;
        // <--
        
        private System.Drawing.Font m_characterFont = null;
        //private stdole.IPictureDisp selectedPpicture = null;
        private stdole.IPictureDisp selectedSylePpicture = null;
        
        private const string SIZE_CAPTION = "サイズ";
        private const string WIDTH_CAPTION = "幅";

        private bool SetColorButtonEnable = false;

        // 保存済み判定フラグ
        private bool[] m_do_save = new bool[3] {false, false, false};
        
        // UniqueValue or ClassBreaks の場合のプレビュー更新実行済み判定
        private bool[] m_do_renderer_update = new bool[3] { true, true, true};

        // 分類対象ﾌｨｰﾙﾄﾞ
        private string m_selected_field_Name_UV = "";
        private string m_selected_field_Name_CB = "";

        // 既存データ開いた際のタブインデクス
        private int m_defaut_tabIndex = 0;

		// ｶﾗｰ･ﾗﾝﾌﾟ情報
        private IColorRamp	style_colorRamp = null;
        private string		colorSchemeName = "";
        
        // ｼﾝﾎﾞﾙ画像取得時に使用
        Graphics graphics = null;
        
        // ﾌｨｰﾙﾄﾞ別名設定識別ﾌﾗｸﾞ
		private bool		_blnUseFieldAlias = true;
		
		// 個別分類・既定ﾗﾍﾞﾙ
		private string		_strUVRDefaultLabel;
        
        // initial status check
        //private bool InitStatus = true;
        // ｺﾝｽﾄﾗｸﾀ処理中ﾌﾗｸﾞ
        private bool InitLoad = true;

        // リストビュー全体選択状態
        bool	selectingall = false;		// ﾘｽﾄ選択ｲﾍﾞﾝﾄの実行制御用ﾌﾗｸﾞ
        int		selectingall_cnt = 0;		// ﾘｽﾄ選択中、処理回数ｶｳﾝﾄ

		// 選択ｼﾝﾎﾞﾙ管理用ｸﾗｽ
		//private class SelectSymbol {
		//	internal int	Index = 0;
		//	internal bool	IsSelect = false;
		//}
		//private SelectSymbol	_selSymbol;

		private Random	_random;

        /// <summary>
        /// シンボル設定フォーム
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormSymbolSettings(IMapControl3 mapControl) {
            try {
                Common.Logger.Debug("シンボル設定表示開始");

                InitializeComponent();

                this.m_mapControl = mapControl;

                // ﾏｯﾌﾟを取得
                IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
                // ﾏｯﾌﾟのﾌｫｰﾑを取得
                System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                mainFrm = (Ui.MainForm)cntrl2.FindForm();

                // ｸﾞﾗﾌｨｯｸｽを作成
                this.graphics = this.CreateGraphics();
                
				// ﾗﾝﾀﾞﾑ･ｵﾌﾞｼﾞｪｸﾄを生成
				this._random = new Random(Environment.TickCount);

				// 選択ｼﾝﾎﾞﾙ管理ｸﾗｽを生成
				//_selSymbol = new SelectSymbol();

                // 個別分類既定ﾗﾍﾞﾙを取得
                IUniqueValueRenderer uvrTemp = new UniqueValueRendererClass();
                this._strUVRDefaultLabel = uvrTemp.DefaultLabel;

                // もしオプション設定から取得した個別値最大件数が
                // UNIQUEVALUE_MAX_LIMIT(5000件)越すときは,UNIQUEVALUE_MAX_LIMITを上限にする
                ShowUniquValueLimitOverWarning = false;
                int uniquValueMax = 0;
                Common.OptionSettings optionSettings = null;
                try {
					// ｵﾌﾟｼｮﾝ設定情報の読み込み
                    optionSettings = new Common.OptionSettings();
                    
                    // ﾌｨｰﾙﾄﾞ別名設定を取得
                    this._blnUseFieldAlias = optionSettings.FieldNmaeUseAlias.Equals("1");
                }
                catch(Exception ex) {
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    Common.MessageBoxManager.ShowMessageBoxError(this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ 個別値最大件数 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                    this.Close();
                    return;
                }
                try {
					// 最大分割数を取得
                    uniquValueMax = Convert.ToInt32(optionSettings.UniqueValueMax);
                }
                catch (Exception ex) {
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    Common.MessageBoxManager.ShowMessageBoxError(this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ 個別値最大件数 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                    this.Close();
                    return;
                }

				// 最大分割数をﾁｪｯｸ (下限ﾁｪｯｸ)
                if (uniquValueMax != 0)
                {
                    UNIQUEVALUE_MAX = uniquValueMax;
                }
                else
                {
                    Common.MessageBoxManager.ShowMessageBoxError(this,
                        "個別値最大件数がゼロ件指定されています。\n" +
                        "オプション設定を見直して下さい。");

                    this.Close();
                    return;
                }

				// 最大分割数をﾁｪｯｸ (上限ﾁｪｯｸ)
                if (UNIQUEVALUE_MAX > UNIQUEVALUE_MAX_LIMIT)
                {
                    // 許容する最大値を適用
                    UNIQUEVALUE_MAX = UNIQUEVALUE_MAX_LIMIT;

                    // 分割数最大越えﾌﾗｸﾞの設定
                    UniquValueLimitOverWarningMessageShowDone = false;	// 許容ｵｰﾊﾞｰ･警告ﾒｯｾｰｼﾞ表示済みﾌﾗｸﾞ
                    ShowUniquValueLimitOverWarning = true;				// 最大許容数を適用
                }

				// 初回表示設定を実行
                SetInit();
                SetTabControls();
            }
            catch (COMException comex)
            {
                Common.Logger.Error(comex.Message.ToString());
                Common.Logger.Error(comex.StackTrace.ToString());
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.Message.ToString());
                Common.Logger.Error(ex.StackTrace.ToString());
            }
            finally
            {
                InitLoad = false;
            }
        }

        ///<summary>
        /// EMFファイルまたはBMPファイルから
        /// ピクチャーマーカーシンボル作成
        ///</summary>
        ///<param name="pictureType">
        /// An ESRI.ArcGIS.Display.esriIPictureType enumeration. 
        /// Example: ESRI.ArcGIS.Display.esriIPictureType.esriIPictureEMF or 
        /// ESRI.ArcGIS.Display.esriIPictureType.esriIPictureBitmap</param>
        ///<param name="filename">
        /// A System.String that is the path and file name of the picture marker. 
        /// Example: "c:\temp\mypict.bmp" or "c:\temp\mypict.emf"</param>
        ///<param name="markerSize">
        /// A System.Double that is the marker size of the picture. Example: 24.0</param>
        ///<returns>An IPictureMarkerSymbol interface.</returns>
        ///<remarks></remarks>
        private ESRI.ArcGIS.Display.IPictureMarkerSymbol CreatePictureMarkerSymbol(
            ESRI.ArcGIS.Display.esriIPictureType pictureType,
            System.String filename, System.Double markerSize)
        {

            // This example creates two PictureMarkerSymbols
            // One from an .EMF file and another from a .BMP file.

            if (pictureType != esriIPictureType.esriIPictureBitmap &&
                pictureType != esriIPictureType.esriIPictureEMF)
            {
                return null;
            }

            // Set the Transparent background color for the Picture Marker symbol to white.
            ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
            rgbColor.Red = 255;
            rgbColor.Green = 255;
            rgbColor.Blue = 255;

            // Create the Marker and assign properties.
            ESRI.ArcGIS.Display.IPictureMarkerSymbol pictureMarkerSymbol =
                new ESRI.ArcGIS.Display.PictureMarkerSymbolClass();
            pictureMarkerSymbol.CreateMarkerSymbolFromFile(pictureType, filename);
            pictureMarkerSymbol.Angle = 0;
            pictureMarkerSymbol.BitmapTransparencyColor = rgbColor;
            pictureMarkerSymbol.Size = markerSize;
            pictureMarkerSymbol.XOffset = 0;
            pictureMarkerSymbol.YOffset = 0;

            return pictureMarkerSymbol;
        }


        /// <summary>
        /// 処理対象レイヤーのレンダラ種類に応じて初期表示タブを振り分け (ｺﾝｽﾄﾗｸﾀからのみｺｰﾙされる)
        /// </summary>
        public void SetInit() {
            Common.Logger.Debug("シンボル設定初期表示処理開始");

            m_featureLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
            m_geometryType = m_featureLayer.FeatureClass.ShapeType; 

            IGeoFeatureLayer gflayer = m_featureLayer as IGeoFeatureLayer;
            IFeatureRenderer source_frenderer = gflayer.Renderer;
            IFeatureRenderer frenderer = null;

            ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();
            frenderer = objectCopy.Copy(source_frenderer) as IFeatureRenderer;

#if doSetColorSentouLayer
            //m_symbolname_of_renderer[SIMPLE_RENDERER] = string.Empty;
            //m_symbolname_of_renderer[UNIQUEVALUE_RENDERER] = string.Empty;
            //m_symbolname_of_renderer[CLASSBREAK_RENDERER] = string.Empty;
            //multiLineSymbolToGetTopLayer.Add(multiLineSymbolNijyuDash);
#endif

            // 単一シンボル
            if (frenderer is ISimpleRenderer) {
                this.tabControl1.SelectedIndex = SIMPLE_RENDERER;

                ISimpleRenderer simpleRenderer = (ISimpleRenderer)frenderer;
                
                // 各レンダラでの処理用のシンボルを作成しておく
                m_symbol = objectCopy.Copy(simpleRenderer.Symbol) as ISymbol;
                m_symbol_uniquevalue = objectCopy.Copy(simpleRenderer.Symbol) as ISymbol;
                m_symbol_classbreak = objectCopy.Copy(simpleRenderer.Symbol) as ISymbol;
                
                m_symbol_of_renderer[SIMPLE_RENDERER] = m_symbol;
                m_symbol_of_renderer[UNIQUEVALUE_RENDERER] = m_symbol_uniquevalue;
                m_symbol_of_renderer[CLASSBREAK_RENDERER] = m_symbol_classbreak;
                //m_symbol_of_renderer[UNIQUEVALUE_RENDERER] = GetDefaultSimpleSymbol(true);
                //m_symbol_of_renderer[CLASSBREAK_RENDERER] = GetDefaultSimpleSymbol(true);

                //SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], true);
				// ｼﾝﾎﾞﾙ･ｲﾒｰｼﾞを表示
				UpdatePreviewImage();
            }
            else {
				// ﾊﾞｯﾌｧ･ﾚﾝﾀﾞﾗｰの初期化
                this.m_symbol_of_renderer[SIMPLE_RENDERER] = GetDefaultSimpleSymbol(false);
                this.m_symbol_of_renderer[UNIQUEVALUE_RENDERER] = GetDefaultSimpleSymbol(true);
                this.m_symbol_of_renderer[CLASSBREAK_RENDERER] = GetDefaultSimpleSymbol(true);

				// 個別分類
				if (frenderer is IUniqueValueRenderer) {
					// 最大指定に問題がある場合
					if (ShowUniquValueLimitOverWarning) {
						if (!UniquValueLimitOverWarningMessageShowDone) {
							// 警告ﾒｯｾｰｼﾞを表示
							Common.MessageBoxManager.ShowMessageBoxWarining(
								this,
								Properties.Resources.FormSymbolSettings_WARNING_UniqueValueCountLimitOver + 
								"上限の" + UNIQUEVALUE_MAX_LIMIT + "件までの処理を行います。");

							UniquValueLimitOverWarningMessageShowDone = true;	// ﾒｯｾｰｼﾞ表示済みﾌﾗｸﾞ
						}
					}

					this.tabControl1.SelectedIndex = UNIQUEVALUE_RENDERER;

					// 個別値ﾚﾝﾀﾞﾗを作成
					// 2011/04/26 下から移動
					//m_work_UniqueValueRenderer = (IUniqueValueRenderer)frenderer;
					m_work_UniqueValueRenderer = objectCopy.Copy(frenderer) as IUniqueValueRenderer;
					//<--


					SetDetailPreviewInit();
					SetUniqueRendererTab(m_work_UniqueValueRenderer); //(IUniqueValueRenderer)frenderer);

					SetFieldNamesToComboboxes();
					SetListViewSelectedValueCheck();

					UniqueValueInit = true;
				}
				// 数値分類
				else if (frenderer is IClassBreaksRenderer) {
					// 表示ﾀﾌﾞを設定
					this.tabControl1.SelectedIndex = CLASSBREAK_RENDERER;

					// 数値分類ﾚﾝﾀﾞﾗを作成
					//m_work_ClassBreaksRenderer = (IClassBreaksRenderer)frenderer;
					m_work_ClassBreaksRenderer = objectCopy.Copy(frenderer) as IClassBreaksRenderer;
					
					SetDetailPreviewInitClassBreaks();
					SetClassificationMethodTypeToCombobox();

					SetClassBreakRendererTab((IClassBreaksRenderer)frenderer);

					SetFieldNamesToComboboxes();

					ClassBreaksInit = true;
				}
            }

            // デフォルトタブ
            m_defaut_tabIndex = this.tabControl1.SelectedIndex;

            Common.Logger.Debug("シンボル設定初期表示処理終了");
        }

        /// <summary>
        /// 初期状態が個別値または数値分類である場合に
        /// 単一シンボルにタブ切り替えされる場合の対応の為に
        /// ジオメトリ別に単一シンボルのデフォルトを作成する
        /// </summary>
        private ISymbol GetDefaultSimpleSymbol(bool IsInitial = false)
        {
            m_featureLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
            IGeoFeatureLayer gflayer = m_featureLayer as IGeoFeatureLayer;
            IFeatureRenderer frenderer = gflayer.Renderer;

            ISymbol tsymbol = null;
            switch (m_geometryType)
            {
                // ﾎﾟｲﾝﾄ
                case esriGeometryType.esriGeometryPoint:
                    ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();

                    simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                    
                    simpleMarkerSymbol.Color =
                        //Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
						this.CreateRandomColor();
					if(IsInitial) {
						simpleMarkerSymbol.Color = Common.UtilityClass.ConvertToESRIColor(Color.FromName("Control"));
					}

                    simpleMarkerSymbol.Size = DEFALUT_POINT_SIZE;

                    tsymbol = simpleMarkerSymbol as ISymbol;
                    break;

                // ﾗｲﾝ
                case esriGeometryType.esriGeometryPolyline:
                    ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();

                    simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    
                    simpleLineSymbol.Color =
                        //Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
						this.CreateRandomColor();
					if(IsInitial) {
						simpleLineSymbol.Color = Common.UtilityClass.ConvertToESRIColor(Color.FromName("Control"));
					}
                    
                    simpleLineSymbol.Width =
                        Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);
                    
                    tsymbol = simpleLineSymbol as ISymbol;
                    break;

                // ﾎﾟﾘｺﾞﾝ
                case esriGeometryType.esriGeometryPolygon:
                    ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                    pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                    pSimpleFillSymbol.Color = //Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
											this.CreateRandomColor();
					if(IsInitial) {
						pSimpleFillSymbol.Color = Common.UtilityClass.ConvertToESRIColor(Color.FromName("Control"));
					}

                    pSimpleFillSymbol.Outline.Color =
                        Common.UtilityClass.ConvertToESRIColor(/*buttonSetOutlineColor.BackColor*/Color.Gray);

                    pSimpleFillSymbol.Outline.Width =
                        Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);

                    //IFillSymbol fillsymbol = pSimpleFillSymbol as IFillSymbol;

                    tsymbol = pSimpleFillSymbol as ISymbol;
                    break;

                default:
                    return null;
            }
            return tsymbol;
        }

        /// <summary>
        /// 色設定ボタンの背景色設定
        /// </summary>
        /// <param name="rgb"></param>
        /// <param name="tbtn"></param>
        private void SetButtonBackColor(IRgbColor rgb, Button tbtn) {
            Color color;
			if(rgb == null || rgb.NullColor) {
				color = Color.Transparent;
			}
			else {
				color = Color.FromArgb(rgb.Red, rgb.Green, rgb.Blue);
			}
            tbtn.BackColor = color;
        }

        /// <summary>
        /// 単一シンボル設定・マーカー時のアウトラインGUIの設定
        /// </summary>
        /// <param name="symbol">表示対象シンボル</param>
        /// <returns>編集可／不可</returns>
        private bool SetSimpleMarkerStatus(IMarkerSymbol simplemark) {
			bool	blnRet = false;

            // 有効ｼﾝﾎﾞﾙを含むかどうかﾁｪｯｸ
			if(this.HasSimpleMarkerSymbol(simplemark)) {
				// 最上層のｼﾝﾌﾟﾙ･ｼﾝﾎﾞﾙを取得
				var agSym = this.GetUnderSimpleLayer(simplemark).First();

                // ｱｳﾄﾗｲﾝ幅を取得
				if(Convert.ToDecimal(agSym.OutlineSize) >= numericUpDownSimpleSymbolOutLineWidth.Minimum)
                    numericUpDownSimpleSymbolOutLineWidth.Value = Convert.ToDecimal(agSym.OutlineSize);
				// ｱｳﾄﾗｲﾝの色を取得
                if(!(agSym.OutlineColor == null || agSym.OutlineColor.NullColor)) {
                    SetButtonBackColor(agSym.OutlineColor as IRgbColor, buttonSetOutlineColor);
				}
				else {
					this.buttonSetOutlineColor.BackColor = Color.Transparent;
				}

				blnRet = true;
            }
            return blnRet;
        }

        /// <summary>
        /// マーカーシンボル時のコントロール表示状態の設定
        /// </summary>
        /// <param name="symbol">表示対象シンボル</param>
        private void SetMarkerSymbolStatus(IMarkerSymbol MarkerSym) {
            if(MarkerSym != null) {
                if(!(MarkerSym.Color == null || MarkerSym.Color.NullColor)) {
                    SetButtonBackColor(MarkerSym.Color as IRgbColor, buttonSetColor);
                }
				else {
					this.buttonSetColor.BackColor = Color.Transparent;

					if(MarkerSym.Color == null) {	// ﾋﾟｸﾁｬｰ･ﾏｰｶｰなど
						SetColorButtonEnable = false;
					}
					else if(!CanChangeColor(MarkerSym as ISymbol, false)) {
						SetColorButtonEnable = false;
					}
				}

                if(Convert.ToDecimal(MarkerSym.Size) >= numericUpDownSimpleSymbolSizeOrWidth.Minimum)
                    numericUpDownSimpleSymbolSizeOrWidth.Value = Convert.ToDecimal(MarkerSym.Size);
            }
        }

        /// <summary>
        /// カラーランプ作成
        /// </summary>
        /// <param name="EdColor">終了色</param>
        /// <param name="StColor">開始色</param>
        /// <param name="ColorSize">色数</param>
        /// <returns></returns>
        private IColorRamp CreateColorRamp(
            ESRI.ArcGIS.Display.IRgbColor EdColor,
            ESRI.ArcGIS.Display.IRgbColor StColor,
            int ColorSize)
        {
            //Create a new color algorithmic ramp
            ESRI.ArcGIS.Display.IAlgorithmicColorRamp algColorRamp = 
                new AlgorithmicColorRampClass();
            
            //Create the start and end colors
            IRgbColor startColor = null;IRgbColor endColor = null;
            startColor = new RgbColor();
            endColor = new RgbColor();

            startColor.Red = StColor.Red;
            startColor.Green = StColor.Green;
            startColor.Blue = StColor.Blue;

            endColor.Red = EdColor.Red;
            endColor.Green = EdColor.Green;
            endColor.Blue = EdColor.Blue;
            
            //Set the Start and End Colors
            algColorRamp.ToColor = startColor;algColorRamp.FromColor = endColor;
            
            //Set the ramping Alglorithm 
            algColorRamp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm;
            
            //Set the size of the ramp (the number of colors to be derived)
            algColorRamp.Size = ColorSize;// 5;
            
            //Create the ramp
            bool bOK;
            algColorRamp.CreateRamp(out bOK);

            IColorRamp colorRamp = algColorRamp;

            return colorRamp;
        }

        /// <summary>
        /// 既存の個別値分類、数値分類のカラーランプを取得　※通常、使用されない見込み。(GetColorRampFromStyleGalleryを改造した為。)
        /// </summary>
        /// <param name="StSymbol">開始シンボル</param>
        /// <param name="EndSymbol">終了シンボル</param>
        /// <param name="dataCount">データ数</param>
        private void GetCurrentColor(ISymbol StSymbol, ISymbol EndSymbol, int dataCount) {
			// ｶﾗｰﾗﾝﾌﾟが未設定の場合のみ作成
            if(this.style_colorRamp == null) {

				IColor	agCol1 = null;
				IColor	agCol2 = null;
				// 現在の設定ｶﾗｰを取得
				if(StSymbol is IMarkerSymbol && EndSymbol is IMarkerSymbol) {
					agCol1 = (StSymbol as IMarkerSymbol).Color;
					agCol2 = (EndSymbol as IMarkerSymbol).Color;
				}
				else if(StSymbol is ILineSymbol && EndSymbol is ILineSymbol) {
					agCol1 = (StSymbol as ILineSymbol).Color;
					agCol2 = (EndSymbol as ILineSymbol).Color;
				}
				else if(StSymbol is IFillSymbol && EndSymbol is IFillSymbol) {
					agCol1 = (StSymbol as IFillSymbol).Color;
					agCol2 = (EndSymbol as IFillSymbol).Color;
				}

				// ｶﾗｰﾗﾝﾌﾟを自作する
				if(agCol1 != null && agCol2 != null) {
					IRgbColor esriColor = new RgbColorClass();
					esriColor.RGB = agCol1.RGB;
					esriColor.CMYK = agCol1.CMYK;
					esriColor.Transparency = agCol1.Transparency;
					esriColor.UseWindowsDithering = agCol1.UseWindowsDithering;

					IRgbColor esriColorEnd = new RgbColorClass();
					esriColorEnd.RGB = agCol2.RGB;
					esriColorEnd.CMYK = agCol2.CMYK;
					esriColorEnd.Transparency = agCol2.Transparency;
					esriColorEnd.UseWindowsDithering = agCol2.UseWindowsDithering;

					this.style_colorRamp = this.CreateColorRamp(esriColor, esriColorEnd, dataCount);
				}
			}
        }

        /// <summary>
        /// 処理対象のシンボルによってコントロール表示状態設定
        /// </summary>
        /// <param name="symbol">処理対象シンボル</param>
        /// <param name="init">処理設定時判定フラグ</param>
        private void SetCommonControlsEnable(ISymbol symbol, bool init) {
            bool useOutline = false;

			// 塗りの有効性を取得
            SetColorButtonEnable = CanUseColorButtons(); //true;

            if(symbol is IMarkerSymbol) {
                // 塗り、サイズをGUIに反映
                SetMarkerSymbolStatus(symbol as IMarkerSymbol);

                // ｱｳﾄﾗｲﾝ･ｺﾝﾄﾛｰﾙの設定と有効性を取得
                useOutline = SetSimpleMarkerStatus(symbol as IMarkerSymbol);
            }
            else if(symbol is ILineSymbol) {
				ILineSymbol simplelin = symbol as ILineSymbol;
                if(simplelin.Color != null) {
                    SetButtonBackColor(simplelin.Color as IRgbColor, buttonSetColor);
                }

                if(Convert.ToDecimal(simplelin.Width) >= numericUpDownSimpleSymbolSizeOrWidth.Minimum)
                    numericUpDownSimpleSymbolSizeOrWidth.Value = Convert.ToDecimal(simplelin.Width);

				labelSimpleSymbolSizeOrWidth.Text = WIDTH_CAPTION;

				if(tabControl1.SelectedIndex != UNIQUEVALUE_RENDERER && tabControl1.SelectedIndex != CLASSBREAK_RENDERER)
					buttonOpenImageFileOrColorRamp.Visible = false;
			}
			else if(symbol is IFillSymbol) {
                IFillSymbol simplefil = symbol as IFillSymbol;

                useOutline = true;

                if(simplefil.Color != null) {
                    // 2011/05/26 simplefil.Color != nullの部分追加 
                    if(simplefil.Color == null || simplefil.Color.NullColor) {
                        IRgbColor rgbcolor = new RgbColorClass();
                        rgbcolor.Red = 190;
                        rgbcolor.Green = 190;
                        rgbcolor.Blue = 190;
                        SetButtonBackColor(rgbcolor, buttonSetColor);
                        SetColorButtonEnable = false;
                    }
                    else {
                        SetButtonBackColor(simplefil.Color as IRgbColor, buttonSetColor);
                    }
                }
                    
                if(simplefil.Outline.Color != null)
                    SetButtonBackColor(simplefil.Outline.Color as IRgbColor, buttonSetOutlineColor);

                SetFillSymbolOutline(init);

                labelSimpleSymbolSizeOrWidth.Enabled = false;
                numericUpDownSimpleSymbolSizeOrWidth.Enabled = false;

                // 2010-12-28 add
                m_symbol_of_renderer[this.tabControl1.SelectedIndex] = simplefil as ISymbol;

                IFillSymbol simplefil_check = m_symbol_of_renderer[this.tabControl1.SelectedIndex] as IFillSymbol;
                // end add

                if(tabControl1.SelectedIndex != UNIQUEVALUE_RENDERER && tabControl1.SelectedIndex != CLASSBREAK_RENDERER)
                    buttonOpenImageFileOrColorRamp.Visible = false;
			}

            // 単一ｼﾝﾎﾞﾙ設定時、塗りｺﾝﾄﾛｰﾙの有効性を設定
			if(tabControl1.SelectedIndex != UNIQUEVALUE_RENDERER
				&& tabControl1.SelectedIndex != CLASSBREAK_RENDERER) {
                this.labelSetColor.Enabled = SetColorButtonEnable;
                this.buttonSetColor.Enabled = SetColorButtonEnable;
            }

			// ｱｳﾄﾗｲﾝ･ｺﾝﾄﾛｰﾙの有効性を設定
			{
				this.panelSimpleSymbolOutline.Enabled = useOutline;
				// 色
				this.buttonSetOutlineColor.Enabled = useOutline;
				this.labelSetOutlineColor.Enabled = useOutline;
				// 幅
				this.numericUpDownSimpleSymbolOutLineWidth.Enabled = useOutline;
				this.labelUpDownSimpleSymbolOutLineWidth.Enabled = useOutline;
			}

            // 単一シンボルの初期時
            if(init) UpdatePreviewImage();
        }

        /// <summary>
        /// シンボルプレビューの色を指定色に変更
        /// </summary>
        /// <param name="tsymbol">処理対象シンボル</param>
        /// <param name="color">指定色</param>
        private void SetPreviewImage(ISymbol tsymbol, Color color) {
            //Bitmap img =
            //    Common.DrawSymbol.SymbolToBitmap(
            //    tsymbol, labelPreview.Width, labelPreview.Height);

            Bitmap img = this.CreateSymbolImage(tsymbol, 16, 16, this.labelPreview.BackColor);
            if (img == null) return; 

            // インデックス付きピクセル形式のイメージを判断
            Common.Logger.Debug("PixelFormat=" + img.PixelFormat.ToString());

            if (img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed ||
                img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed ||
                img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed ||
                img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppArgb1555 ||
                img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale ||
                img.PixelFormat == System.Drawing.Imaging.PixelFormat.Undefined ||
                img.PixelFormat == System.Drawing.Imaging.PixelFormat.DontCare)
            {

                Bitmap bmpOld = new Bitmap(img, img.Width, img.Height);
                Bitmap bmpNew = new Bitmap(bmpOld.Width, bmpOld.Height);
                Graphics g = Graphics.FromImage(bmpNew);
                Rectangle rect = new Rectangle(0, 0, bmpNew.Width, bmpNew.Height);

                System.Drawing.Imaging.ImageAttributes attr
                 = new System.Drawing.Imaging.ImageAttributes();
                
                attr.SetThreshold((200f / 255f), System.Drawing.Imaging.ColorAdjustType.Bitmap);
                
                g.DrawImage(
                 bmpOld,
                 rect,
                 0,
                 0,
                 bmpOld.Width,
                 bmpOld.Height,
                 GraphicsUnit.Pixel,
                 attr);

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                bmpNew.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                Image image = Image.FromStream(stream);
                labelPreview.Image = image;
                stream.Dispose();
                return;
            }
            else {
                for(int x = 0; x < img.Width; x++) {
                    for(int y = 0; y < img.Height; y++) {
                        Color cor = img.GetPixel(x, y);
                        //if ((cor.ToArgb()&0xffffff) != 0)  
                        //{
                        if((128 < cor.R) && (128 < cor.G) && (128 < cor.B)) {
                            img.SetPixel(x, y, Color.FromArgb(0));
                        }
                        else {
                            img.SetPixel(x, y, color);
                        }
                        //}
                    }
                }

                labelPreview.Image = img;
            }
        }

        /// <summary>
        /// 単一シンボルのプレビュー更新
        /// </summary>
        private void UpdatePreviewImage() {
			// ｼﾝﾎﾞﾙ･ｲﾒｰｼﾞを生成
			Bitmap img = Common.WindowsAPI45.SymbolToBitmap(
				m_symbol_of_renderer[this.tabControl1.SelectedIndex],
				new Size(90, 90),
				this.graphics,
				ColorTranslator.ToWin32(this.labelPreview.BackColor));

			// 表示
			if(img != null) {
				labelPreview.Image = img;
			}
			else {
				Image image = System.Drawing.Image.FromHbitmap(
					new System.IntPtr(this.selectedSylePpicture.Handle));

				//Bitmap image = DrowLineSymbol(m_symbol);
				if(image != null) labelPreview.Image = image;
			}
        }

		/// <summary>
		/// アウトラインの変更が可能なシンプルマーカーシンボルをすべて取得します
		/// </summary>
		/// <param name="MultiMarkerSymbol"></param>
		/// <returns></returns>
		private ISimpleMarkerSymbol[] GetUnderSimpleLayer(IMarkerSymbol TargetSymbol) {
			List<ISimpleMarkerSymbol>	agSimples = new List<ISimpleMarkerSymbol>();

			// ﾏﾙﾁ
			if(TargetSymbol is IMultiLayerMarkerSymbol) {
				var	agMMSym = TargetSymbol as IMultiLayerMarkerSymbol;

				ISimpleMarkerSymbol	agSMSym;
				foreach(int intCnt in Enumerable.Range(0, agMMSym.LayerCount)) {
					agSMSym = agMMSym.get_Layer(intCnt) as ISimpleMarkerSymbol;
					if(agSMSym != null && (agSMSym.Outline || agMMSym.LayerCount == 1)) {
						agSimples.Add(agSMSym);
					}
				}
			}
			// ｼﾝｸﾞﾙ
			else if(TargetSymbol is ISimpleMarkerSymbol) {
				agSimples.Add(TargetSymbol as ISimpleMarkerSymbol);
			}

			return agSimples.ToArray();
		}

        /// <summary>
        /// 処理対象シンボルのプロパティ設定
		/// 塗り・アウトライン色・アウトライン幅・サイズを m_symbol_of_renderer[] に保存
        /// </summary>
        private bool SetUpdateProperties() {
			bool	blnRet = false;

            if(tabControl1.SelectedIndex == 0 && labelPreview.Image == null) return blnRet;

            // 塗り色設定を取得
            IRgbColor rgb;
			if(!this.buttonSetColor.BackColor.Equals(Color.Transparent)) {
				rgb = Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
			}
			else {
				rgb = new RgbColorClass();
				rgb.NullColor = true;
			}
            
			// 淵色設定を取得
            IRgbColor rgbOutline;
			if(!this.buttonSetOutlineColor.BackColor.Equals(Color.Transparent)) {
				rgbOutline = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
			}
			else {
				rgbOutline = new RgbColorClass();
				rgbOutline.NullColor = true;
			}

            // ｼﾞｵﾒﾄﾘ別ｼﾝﾎﾞﾙ設定
            switch(m_geometryType) {
			// ﾎﾟｲﾝﾄ
            case esriGeometryType.esriGeometryPoint:
				IMarkerSymbol	agMSym = m_symbol_of_renderer[this.tabControl1.SelectedIndex] as IMarkerSymbol;

                // 個別色変更対応の影響、色ボタン使用不可時には色設定しない
				if(buttonSetColor.Enabled && agMSym.Color.RGB != rgb.RGB) {
                    agMSym.Color = rgb;
					blnRet = true;
                }
				if(agMSym.Size != (double)numericUpDownSimpleSymbolSizeOrWidth.Value) {
					agMSym.Size = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);
					blnRet = true;
				}

				// ｼﾝﾌﾟﾙ･ｼﾝﾎﾞﾙ･ﾁｪｯｸ
                if(this.HasSimpleMarkerSymbol(agMSym)) {
					// ｼﾝﾌﾟﾙｼﾝﾎﾞﾙを取得
					ISimpleMarkerSymbol[]	agSimpleMSyms = this.GetUnderSimpleLayer(agMSym);
					
					foreach(var agSMSym in agSimpleMSyms) {
						// ｼﾝﾎﾞﾙを取得
						if(agSMSym.OutlineColor.RGB != rgbOutline.RGB) {
							agSMSym.OutlineColor = rgbOutline;
							blnRet = true;
						}

						if(agSMSym.OutlineSize != (double)numericUpDownSimpleSymbolOutLineWidth.Value) {
							agSMSym.OutlineSize = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);
							blnRet = true;
						}

						bool	blnVisOL = agSMSym.OutlineSize > 0d;
						if(agSMSym.Outline != blnVisOL) {
							agSMSym.Outline = blnVisOL;
							blnRet = true;
						}
					}
				}
                break;

			// ﾗｲﾝ
            case esriGeometryType.esriGeometryPolyline:
				ILineSymbol	agLSym = m_symbol_of_renderer[this.tabControl1.SelectedIndex] as ILineSymbol;
				//if(agLSym is IMultiLayerLineSymbol) {
				//	IMultiLayerLineSymbol	agMultiLSym = agLSym as IMultiLayerLineSymbol;
				//	if(agMultiLSym.LayerCount > 0) {
				//		agLSym = agMultiLSym.get_Layer(0);
				//	}
				//}

				if(buttonSetColor.Enabled && agLSym.Color.RGB != rgb.RGB) {
                    agLSym.Color = rgb;
					blnRet = true;
                }

                if(agLSym.Width != (double)numericUpDownSimpleSymbolSizeOrWidth.Value) {
					agLSym.Width = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);
					blnRet = true;
				}
                break;

            // ﾎﾟﾘｺﾞﾝ
			case esriGeometryType.esriGeometryPolygon:
                blnRet = SetFillSymbolOutline(false);
                break;
            }

            return blnRet;
        }

        /// <summary>
        /// 初期処理
        /// （処理対象が個別値である場合）
        /// </summary>
        /// <param name="uniqueRenderer"></param>
        private void SetUniqueRendererTab(IUniqueValueRenderer uniqueRenderer)
        {
			// ｶﾗｰ･ﾗﾝﾌﾟの設定を取得
            colorSchemeName = uniqueRenderer.ColorScheme;
            if (colorSchemeName == null || colorSchemeName.Length == 0)
                colorSchemeName = DEFALULT_COLOR_RAMP_NAME;

            // サイズ
            numericUpDownSimpleSymbolSizeOrWidth.Value = Convert.ToDecimal(DEFALUT_POINT_SIZE);

            imageListSymbol.Images.Clear();
            listViewUniqueValue.Items.Clear();
            listViewUniqueValue.SmallImageList = imageListSymbol;
            listViewUniqueValue.LargeImageList = imageListSymbol;

            // フィールド名称サンプル
            this.UniqueValueSelectedFieldName = uniqueRenderer.get_Field(0);
            
            ListViewItem item1 = null;
            int listIndex = 0;

            // その他の値すべてのｼﾝﾎﾞﾙ･ｲﾒｰｼﾞを取得
            ISymbol defsymbol = uniqueRenderer.DefaultSymbol;
            Image defimg = this.CreateSymbolImage(
				defsymbol, 
				this.imageListSymbol.ImageSize.Width, 
				this.imageListSymbol.ImageSize.Height, 
				this.listViewUniqueValue.BackColor);

            imageListSymbol.Images.Add(defimg);

            item1 = new ListViewItem("", listIndex);
            item1.SubItems.Add(this._strUVRDefaultLabel);
            item1.SubItems.Add(uniqueRenderer.DefaultLabel);
            listViewUniqueValue.Items.AddRange(new ListViewItem[] { item1 });
            listIndex++;

            //-->
            int eachColorIdx = 0;
            IColor defColor = GetSymbolColor(defsymbol);
            SaveUniqueValueEachColorToList(eachColorIdx, defColor);
            SaveUniqueValueDelSymbolToList(eachColorIdx, true, true); // 削除対象保持初期化

            SaveUniqueValueEachInfoToList(eachColorIdx, 
                SetSelectedItemSymbolAttributes(defsymbol), true);

            checkBoxUseUniqueValueDefaultSymbol.Checked = uniqueRenderer.UseDefaultSymbol;
            //<--

            ISymbol sampleSymbol = null;
            ISymbol sampleEndSymbol = null;

            listViewUniqueValue.BeginUpdate();

            for(int i = 0; i < uniqueRenderer.ValueCount; i++) {
                ISymbol symbol = uniqueRenderer.get_Symbol(uniqueRenderer.get_Value(i));

                //-->
                IColor eachColor = GetSymbolColor(symbol);
                eachColorIdx++;
                SaveUniqueValueEachColorToList(eachColorIdx, eachColor);

                SaveUniqueValueEachInfoToList(eachColorIdx, SetSelectedItemSymbolAttributes(symbol), true);

                //<--

                // pop sample symbol
                if (i == 0) sampleSymbol = symbol;
                if (i == uniqueRenderer.ValueCount - 1) sampleEndSymbol = symbol;

				Image img = this.CreateSymbolImage(
					symbol, 
					this.imageListSymbol.ImageSize.Width, 
					this.imageListSymbol.ImageSize.Height, 
					this.listViewUniqueValue.BackColor);

                imageListSymbol.Images.Add(img);
                //listViewUniqueValue.Items.Add(uniqueRenderer.get_Value(i), i);

                //ListViewItem 
                item1 = new ListViewItem("", i + 1);
                item1.SubItems.Add(uniqueRenderer.get_Value(i));
                item1.SubItems.Add(uniqueRenderer.get_Label(uniqueRenderer.get_Value(i)));
                listViewUniqueValue.Items.AddRange(new ListViewItem[] { item1 });
            }

            listViewUniqueValue.EndUpdate();

            // 2011/05/13 デフォルトシンボルをObjectCopy -->
            //m_symbol_of_renderer[this.tabControl1.SelectedIndex] = defsymbol;
            ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy =
                new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

            m_symbol_of_renderer[this.tabControl1.SelectedIndex] =
                objectCopy.Copy(defsymbol) as ISymbol;
            //<--
            
            SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], true);

            if (colorSchemeName != null)
                this.style_colorRamp = GetColorRampFromStyleGallery(colorSchemeName);

            if (this.style_colorRamp == null)
                GetCurrentColor(sampleSymbol, sampleEndSymbol, uniqueRenderer.ValueCount);
        }

        /// <summary>
        /// 個別値分類用プレビュー・リストを形成します
        /// </summary>
        private void SetDetailPreviewInit() {
			// ｼﾝﾎﾞﾙ･ﾘｽﾄを初期化 (個別値分類)
            imageListSymbol.Images.Clear();
            listViewUniqueValue.Items.Clear();

			//listViewUniqueValue.Columns.Clear();
			if(listViewUniqueValue.Columns.Count <= 0) {
				// 個別値分類ﾘｽﾄの設定
				listViewUniqueValue.FullRowSelect = true;
				listViewUniqueValue.GridLines = true;
				listViewUniqueValue.Sorting = SortOrder.Ascending;
				listViewUniqueValue.View = View.Details;

				ColumnHeader columnName = new ColumnHeader();
				ColumnHeader columnType = new ColumnHeader();
				ColumnHeader columnLabel = new ColumnHeader();
				columnName.Text = "シンボル";
				columnName.Width = 60;
				columnType.Text = "値";
				columnType.Width = (this.listViewUniqueValue.Width - columnName.Width) / 2;
				columnLabel.Text = "ラベル";
				columnLabel.Width = -2;

				ColumnHeader[] colHeaderRegValue = { columnName, columnType, columnLabel };
				listViewUniqueValue.Columns.AddRange(colHeaderRegValue);
            
				listViewUniqueValue.SmallImageList = imageListSymbol;
				listViewUniqueValue.LargeImageList = imageListSymbol;

				//listViewUniqueValue.CheckBoxes = true;

				// すべての列幅を自動調整
				//foreach (ColumnHeader ch in listViewUniqueValue.Columns) {
				//    ch.Width = -2;
				//}
			}
        }

        /// <summary>
        /// 数値分類用プレビュー・リストを形成します
        /// </summary>
        private void SetDetailPreviewInitClassBreaks() {
			// ｼﾝﾎﾞﾙ･ﾘｽﾄを初期化 (数値分類)
            imageListSymbolClassBreaks.Images.Clear();
            listViewClassBreaks.Items.Clear();
            listViewClassBreaks.Columns.Clear();

            listViewClassBreaks.Sorting = SortOrder.Ascending;
            listViewClassBreaks.View = View.Details;

            ColumnHeader columnName = new ColumnHeader();
            ColumnHeader columnType = new ColumnHeader();
            ColumnHeader columnLabel = new ColumnHeader();
            columnName.Text = "シンボル";
            columnName.Width = 60;
            columnType.Text = "範囲";
            columnType.Width = (this.listViewClassBreaks.Width - columnName.Width) / 2;
            columnLabel.Text = "ラベル";
            columnLabel.Width = -2;
            ColumnHeader[] colHeaderRegValue = { columnName, columnType, columnLabel };
            
            // 編集可能列IDを設定 ("範囲")
            this.CB_EDITABLE_COLUMN_ID = 1;
            
            listViewClassBreaks.Columns.AddRange(colHeaderRegValue);

            listViewClassBreaks.SmallImageList = imageListSymbolClassBreaks;
            
            // すべての列幅を自動調整
            //foreach (ColumnHeader ch in listViewClassBreaks.Columns) {
                //ch.Width = -2;
            //}
        }

        /// <summary>
        /// 個別値, 数値の分類処理対象のフィールド名称設定
        /// （処理対象が個別値、または数値分類である場合）
        /// </summary>
        private void SetFieldNamesToComboboxes()
        {
            if (UniqueValueInit || ClassBreaksInit) return;

            // 対象ﾌｨｰﾁｬｰ･ﾚｲﾔｰのﾌｨｰﾙﾄﾞを取得
            IGeoFeatureLayer	gflayer = m_featureLayer as IGeoFeatureLayer;
            IFields				fields = gflayer.DisplayFeatureClass.Fields;  // del
            string				OidFieldName = gflayer.FeatureClass.OIDFieldName;
            
            // ﾌｨｰﾙﾄﾞ情報･ｲﾝﾀｰﾌｪｲｽを取得
            //IDisplayTable dispTbl = (IDisplayTable)m_featureLayer; // add
            ITableFields tblFlds = (ITableFields)m_featureLayer;   // add

            int uniqueValueSelectIndex = -1;
            int classBreaksSelectIndex = -1;

            // 対象ﾌｨｰﾙﾄﾞを抽出
            for (int i = 0; i < fields.FieldCount; i++) // replace below
            //for (int i = 0; i < dispTbl.DisplayTable.Fields.FieldCount; i++) // add
            {
                // ﾌｨｰﾙﾄﾞを取得
                IField		tfield = fields.get_Field(i); // dispTbl.DisplayTable.Fields.get_Field(i); // 
                //IFieldInfo	fldinfo = tblFlds.get_FieldInfo(i); // add

                esriFieldType	fldType = tfield.Type;
                
                // ｼｽﾃﾑ･ﾌｨｰﾙﾄﾞは対象外
                if(!tfield.Name.Equals(OidFieldName)) {
					// ﾃﾞｰﾀ型をﾁｪｯｸ (文字または数値のみ)
                    if (fldType == esriFieldType.esriFieldTypeString ||
                        fldType == esriFieldType.esriFieldTypeSmallInteger ||
                        fldType == esriFieldType.esriFieldTypeInteger ||
                        fldType == esriFieldType.esriFieldTypeSingle ||
                        fldType == esriFieldType.esriFieldTypeDouble)
                    {
                        // どちらかで初期化すれば両方ともに初期化する
                        if (!UniqueValueInit && !ClassBreaksInit)
                        {
                            // 選択項目に追加 (個別値分類)
                            this.comboBoxFieldName.Items.Add(new Common.FieldComboItem(tblFlds, i, this._blnUseFieldAlias, false));

                            // 既存フィールド名判定（個別値分類）
                            if (UniqueValueSelectedFieldName != null &&
                                UniqueValueSelectedFieldName.Equals(tfield.Name))
                            {
                                uniqueValueSelectIndex = this.comboBoxFieldName.Items.Count - 1;
                            }

                            // 数値型ﾌｨｰﾙﾄﾞ時は、数値分類の選択項目にも追加
                            if (fldType != esriFieldType.esriFieldTypeString)
								// 選択項目に追加 (数値分類)
                                this.comboBoxFieldNameClassBreaks.Items.Add(new Common.FieldComboItem(tblFlds, i, this._blnUseFieldAlias, false));

                            // 既存フィールド名判定（数値分類）
                            if (ClassBreaksSelectedFieldName != null &&
                                ClassBreaksSelectedFieldName.Equals(tfield.Name))
                            {
                                classBreaksSelectIndex = this.comboBoxFieldNameClassBreaks.Items.Count - 1;
                            }
                        }
                    }
                }
            }

            if (!UniqueValueInit && !ClassBreaksInit)
            {
                // 個別分類の既定ﾌｨｰﾙﾄﾞを設定
                if (this.comboBoxFieldName.Items.Count > 0)
                    if (uniqueValueSelectIndex == -1)
                        this.comboBoxFieldName.SelectedIndex = 0;
                    else
                        this.comboBoxFieldName.SelectedIndex = uniqueValueSelectIndex;

				// 数値分類の既定ﾌｨｰﾙﾄﾞを設定
                if (this.comboBoxFieldNameClassBreaks.Items.Count > 0)
                    if (classBreaksSelectIndex == -1)
                        this.comboBoxFieldNameClassBreaks.SelectedIndex = 0;
                    else
                        this.comboBoxFieldNameClassBreaks.SelectedIndex = classBreaksSelectIndex;
            }
        }

        /// <summary>
        /// 分類種類の設定
        /// （処理対象が数値分類である場合）
        /// </summary>
        private void SetClassificationMethodTypeToCombobox()
        {
			// 数値分類・分類手法ﾘｽﾄを生成
            this.comboBoxBunruiSyuhou.Items.Add(CLASSBREAKS_NATURAL);
            this.comboBoxBunruiSyuhou.Items.Add(CLASSBREAKS_EQUAL_INTERVAL);
            this.comboBoxBunruiSyuhou.Items.Add(CLASSBREAKS_QUANTILE);
            
            // 既定の設定 → SelectedIndexChanged ｲﾍﾞﾝﾄ
            this.comboBoxBunruiSyuhou.SelectedIndex = 0; // defalut NATURAL
        }

        /// <summary>
        /// 数値分類アクセスオブジェクト取得
        /// （処理対象が数値分類である場合）
        /// </summary>
        /// <param name="pClassBreaksRenderer">処理対象レンダラ</param>
        /// <param name="pUIProperties">処理対象レンダラのプロパティ（参照渡し）</param>
        /// <returns>IClassifyGEN</returns>
        private ESRI.ArcGIS.esriSystem.IClassifyGEN GetClassify(
            IClassBreaksRenderer pClassBreaksRenderer, 
            ref IClassBreaksUIProperties pUIProperties)
        {
            ESRI.ArcGIS.esriSystem.IClassifyGEN classsify = null;

            if(pClassBreaksRenderer == null) {
                pClassBreaksRenderer = new ClassBreaksRendererClass();
            }

            //IClassBreaksUIProperties 
            pUIProperties = pClassBreaksRenderer as IClassBreaksUIProperties;

            //' Define options in the INumericFormat interface.
            //' NumericFormat is the coclass -- use New to set this.
            ESRI.ArcGIS.esriSystem.INumericFormat pNumericFormat = new ESRI.ArcGIS.esriSystem.NumericFormatClass();

            pNumericFormat.AlignmentOption = ESRI.ArcGIS.esriSystem.esriNumericAlignmentEnum.esriAlignRight;

            pNumericFormat.RoundingOption = ESRI.ArcGIS.esriSystem.esriRoundingOptionEnum.esriRoundNumberOfDecimals;

            pNumericFormat.AlignmentWidth = 12;
            pNumericFormat.RoundingValue = 6;
            pNumericFormat.ShowPlusSign = true;
            pNumericFormat.UseSeparator = true;
            pNumericFormat.ZeroPad = false;

            //' ValueToString & StringToValue methods are in the 
            //' INumberFormat interface.  QI for INumberFormat to 
            //' use them.
            ESRI.ArcGIS.esriSystem.INumberFormat pNumberFormat = pNumericFormat as ESRI.ArcGIS.esriSystem.INumberFormat;
            pUIProperties.NumberFormat = pNumberFormat;

			// 分類手法を設定
            if(this.comboBoxBunruiSyuhou.SelectedIndex == Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_NATURAL)) {
                // 自然分類
                classsify = new ESRI.ArcGIS.esriSystem.NaturalBreaks() as ESRI.ArcGIS.esriSystem.IClassifyGEN;

                Common.Logger.Debug("CLASSBREAKS_NATURAL");
            }
            else if(this.comboBoxBunruiSyuhou.SelectedIndex == Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_EQUAL_INTERVAL)) {
                // 等間隔分類
                classsify = new ESRI.ArcGIS.esriSystem.EqualInterval() as ESRI.ArcGIS.esriSystem.IClassifyGEN;

                Common.Logger.Debug("CLASSBREAKS_EQUAL_INTERVAL");
            }
            else if(this.comboBoxBunruiSyuhou.SelectedIndex == Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_QUANTILE)) {
                // 等量分類
                classsify = new ESRI.ArcGIS.esriSystem.Quantile() as ESRI.ArcGIS.esriSystem.IClassifyGEN;

                Common.Logger.Debug("CLASSBREAKS_QUANTILE");
            }
            else {
                classsify = null;
            }
            
            return classsify;
        }
        
        /// <summary>
        /// 数値分類の分類値を取得します
        /// </summary>
        /// <param name="CBRenderer">数値分類レンダラー</param>
        /// <returns>分類値群</returns>
        private double[] GetExistBreaks(IClassBreaksRenderer CBRenderer) {
			double[]	dblBreaks;
			
			if(CBRenderer != null) {
				IClassBreaksUIProperties	pCBProp = (IClassBreaksUIProperties)CBRenderer;
				
				// 分類方法の識別値を取得
				int	intMethod = this.GetCBMethodID(pCBProp);

				// 配列要素数を割り当て
				dblBreaks = new double[CBRenderer.BreakCount + 1];
							
				// 分類値を取得
				dblBreaks[0] = pCBProp.get_LowBreak(0);
				for(int intCnt = 0; intCnt < CBRenderer.BreakCount; intCnt++) {
					dblBreaks[intCnt + 1] = CBRenderer.get_Break(intCnt);
				}
				
				// 分類値ﾊﾞｯﾌｧに保存
				CLASSBREAKS_VALUES[intMethod] = dblBreaks;
			}
			else {
				dblBreaks = new double[0];
			}
			
			// 返却
			return dblBreaks;
        }
        
        /// <summary>
        /// 数値分類の分類方法を判定します
        /// </summary>
        /// <param name="pCBProp">数値分類プロパティ</param>
        /// <returns>判定値 (判定負荷時は負数)</returns>
        private int GetCBMethodID(IClassBreaksUIProperties pCBProp) {
            int		intRet = -1;
            string	strLog = "";
            
			if(pCBProp != null) {
                strLog = "IClassBreaksUIProperties.Method.IUID.Value... = " + pCBProp.Method.Value.ToString() + Environment.NewLine;

				// 分類手法の判定
				switch (pCBProp.Method.Value.ToString()) {
				// 自然分類
				case CLASSBREAKS_NATURAL_UID:
					intRet = Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_NATURAL);
					strLog += "Natural Breaks (Jenks)";
					break;

				// 等間隔
				case CLASSBREAKS_EQUAL_INTERVAL_UID:
					intRet = Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_EQUAL_INTERVAL);
					strLog += "Equal Interval";
					break;

				// 等量
				case CLASSBREAKS_QUANTILE_UID:
					intRet = Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_QUANTILE);
					strLog += "Quantile";
					break;

				//case "{DC6D8015-49C2-11D2-AAFF-00C04FA334B3}":
				//    Common.Logger.Debug("Standard Deviation");
				//    break;

				//case "{62144BE8-E05E-11D1-AAAE-00C04FA334B3}":
				//    Common.Logger.Debug("Defined Interval");
				//    break;

				default:
					strLog += "Unknown Classification Method UID - ";
					break;
				}
			}
			else {
				strLog = "NULL";
			}
			
			// ﾛｸﾞ出力
			Common.Logger.Debug("ClassBreaks Method = " + strLog);
			
			// 返却
			return intRet;
        }

        /// <summary>
        /// 分類手法に選択されている方法（自然分類、等間隔または等量分類）により、
        /// 対象フィールドのユニークな内容の値の合計数をもとめる。
        /// その内容を選択分類手法に対応する配列要素に保持する。
        /// プレビュー更新指定時に実行する。
        /// （処理対象が数値分類である場合）
        /// </summary>
        private IClassBreaksRenderer GetClassBreaks(
			string	FieldName,
            int		breaksCount, 
            ref IClassBreaksUIProperties pUIProperties)
        {
            // 数値フィールドが存在しない場合
            if (this.comboBoxFieldNameClassBreaks.SelectedIndex < 0) return null;

            // 分類ﾃｰﾌﾞﾙを取得
            IGeoFeatureLayer	gflayer = m_featureLayer as IGeoFeatureLayer;
            IDisplayTable		displayTable = gflayer as IDisplayTable;
            ITable				pTable = displayTable as ITable;

            // 分類数を調整
            if(breaksCount >= MAX_CLASS_BREAK_COUNT) {
                breaksCount = MAX_CLASS_BREAK_COUNT - 1;
            }

			// ﾋｽﾄｸﾞﾗﾑを使用
            IBasicHistogram			basicHistogram = new BasicTableHistogramClass();
            IClassBreaksRenderer	pClassBreaksRenderer = new ClassBreaksRendererClass();
            object					histValues = null;
            object					frequencyValues = null;

            lock(basicHistogram) {
                try {
					// ﾃﾞｰﾀ区間を算出
                    ITableHistogram tableHistogram = basicHistogram as ITableHistogram;

                    tableHistogram.Table = pTable;
                    tableHistogram.Field = FieldName;
                    basicHistogram.GetHistogram(out histValues, out frequencyValues);

                    int			numClasses = breaksCount + 1;
                    double[]	classBreakRtn = null;

                    // 分類手法ｵﾌﾞｼﾞｪｸﾄを生成
                    ESRI.ArcGIS.esriSystem.IClassifyGEN classsify = GetClassify(pClassBreaksRenderer, ref pUIProperties);

                    if(classsify != null) {
						// 閾値を取得
                        classsify.Classify(histValues, frequencyValues, ref numClasses);
                        classBreakRtn = (double[])classsify.ClassBreaks;

                        pUIProperties.Method = classsify.ClassID;
                    }
                    else {
                        CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex] = new double[0];
                    }

                    if(classBreakRtn != null && classBreakRtn.Length > 0) {
                        // 閾値をﾘｽﾄにｺﾋﾟｰ
                        List<double> tmpClassValues = new List<double>(classBreakRtn);
						//for(int i = 0; i < classBreakRtn.Length; i++) {
						//	tmpClassValues.Add(classBreakRtn[i]);
						//}

                        // 分類保存域を生成
                        CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex] = new double[tmpClassValues.Count];

                        // 等間隔分類以外の場合
                        if(this.comboBoxBunruiSyuhou.SelectedIndex != Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_EQUAL_INTERVAL)) {
                            for (int i = 0; i < tmpClassValues.Count; i++) {
								// 小数点第6位まで
                                CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][i] = Math.Round(tmpClassValues[i], 6);

                                Common.Logger.Debug(CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][i].ToString());
                            }
                        }
                        // 等間隔分類時
                        else {
                            // 等間隔分類の場合は整数型でも閾値が少数点に成る場合があり、
                            // その場合整数だけで表現する様に調整
                            // 処理対象のフィールドタイプ（数値以外の型は処理対象にはならない）
                            bool	blnInt = this.IsIntegerField(FieldName);
                            double	dblVal;

                            List<double> tmpEqualIntvlClassValues = new List<double>();
                            for(int i = 0; i < tmpClassValues.Count; i++) {
                                // 整数時
                                if(blnInt) {
									// 小数点以下を除去 ※ArcGISでは四捨五入の為、変更
									//dblVal = System.Math.Truncate(tmpClassValues[i]);
									dblVal = System.Math.Round(tmpClassValues[i], 0);
                                    if(!tmpEqualIntvlClassValues.Contains(dblVal)) {
                                        tmpEqualIntvlClassValues.Add(dblVal);

                                        Common.Logger.Debug("bef conv: " + tmpClassValues[i].ToString());
                                        Common.Logger.Debug("aft conv: " + System.Math.Truncate(tmpClassValues[i]).ToString());
                                    }
                                }
                                // 実数時
                                else {
									// 小数点第6位まで
                                    if(!tmpEqualIntvlClassValues.Contains(tmpClassValues[i])) {
                                        tmpEqualIntvlClassValues.Add(Math.Round(tmpClassValues[i], 6));
                                    }
                                }
                            }

                            // 分類値を保存
                            CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex] = new double[tmpEqualIntvlClassValues.Count];
                            for(int i = 0; i < tmpEqualIntvlClassValues.Count; i++) {
                                CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][i] = tmpEqualIntvlClassValues[i];
                            }
                        }
                    }
                    
                    return pClassBreaksRenderer;
                }
                catch(COMException comex) {
                    Common.Logger.Debug(comex.Message.ToString());
                    Common.Logger.Debug(comex.StackTrace.ToString());
                    return null;
                }
                catch(Exception ex) {
                    Common.Logger.Debug(ex.Message.ToString());
                    Common.Logger.Debug(ex.StackTrace.ToString());
                    return null;
                }
                finally {
                    histValues = null;
                    frequencyValues = null;
                }
            }
        }

        /// <summary>
        /// 個別値リストビュー設定
        /// </summary>
        /// <param name="pUniqueValueRenderer">個別分類レンダラー</param>
        /// <param name="currentIndex">選択状態にする行番号</param>
        private void SetEachUniqueValueToListView(IUniqueValueRenderer pUniqueValueRenderer, int currentIndex) {
            Common.Logger.Debug("UniqueValueRenderer Preview Set STARTED:" + pUniqueValueRenderer.ValueCount.ToString());

            // 最大分類数を取得
            int endedCont = pUniqueValueRenderer.ValueCount;
            if(endedCont > UNIQUEVALUE_MAX) {
                endedCont = UNIQUEVALUE_MAX;
            }

			// ｼﾝﾎﾞﾙ･ﾋﾞｯﾄﾏｯﾌﾟ･ﾘｽﾄをｸﾘｱ
			if(this.bitmapList != null) {
				this.bitmapList.Clear();
			}
			else {
				this.bitmapList = new List<Bitmap>();
            }

            // 個別カラー保持
            //-->
            //bool saveEachColor = false;
            ISymbol defsymbol = pUniqueValueRenderer.DefaultSymbol;
            int eachColorIdx = 0;

            // 0513 このタイミングで個別色は常に保存し直し
            //if (m_work_UniqueValueRendererEachColor == null ||
            //    m_work_UniqueValueRendererEachColor.Count == 0)
            //{
            //    saveEachColor = true;
            //}
            //if (saveEachColor)
            //{
            
            // ﾃﾞﾌｫﾙﾄ･ｼﾝﾎﾞﾙ情報を保存
            IColor defColor = GetSymbolColor(defsymbol);
            // 色設定ﾘｽﾄに塗り色を保存
            SaveUniqueValueEachColorToList(eachColorIdx, defColor);
			// ｼﾝﾎﾞﾙ設定情報を保存
            SaveUniqueValueEachInfoToList(eachColorIdx, SetSelectedItemSymbolAttributes(defsymbol), true);
            //}
            //<--
            
            // ｼﾝﾎﾞﾙの画像を作成 (その他の値)
            Bitmap defimage = this.CreateSymbolImage(
                pUniqueValueRenderer.DefaultSymbol,
                this.imageListSymbol.ImageSize.Width,
                this.imageListSymbol.ImageSize.Height,
                this.listViewUniqueValue.BackColor);

            this.bitmapList.Add(defimage);

			// 個別ｼﾝﾎﾞﾙ情報を保存 (個別値)
            for(int i = 0; i < endedCont; i++) {
				// ｼﾝﾎﾞﾙを取得
                ISymbol symbol = pUniqueValueRenderer.get_Symbol(pUniqueValueRenderer.get_Value(i));

                // カラー保持内容内容が空なら保持して置く
                //-->
                //if (saveEachColor)
                //{
                    IColor eachColor = GetSymbolColor(symbol);
                    eachColorIdx++;
                    SaveUniqueValueEachColorToList(eachColorIdx, eachColor);
                    SaveUniqueValueEachInfoToList(eachColorIdx, SetSelectedItemSymbolAttributes(symbol), true);
                //}
                //<--

				// ｼﾝﾎﾞﾙの画像を作成
                Bitmap image = this.CreateSymbolImage(
                    symbol,
                    this.imageListSymbol.ImageSize.Width,
                    this.imageListSymbol.ImageSize.Height,
                    this.listViewUniqueValue.BackColor);

                this.bitmapList.Add(image);

                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(symbol);

                Common.Logger.Debug("UniqueValueRenderer Preview Set :" + i.ToString() + "件目");
            }

            Common.Logger.Debug("UniqueValueRenderer Preview Set ENDED:" + endedCont.ToString());

            // 個別値分類用ﾘｽﾄﾋﾞｭｰをｸﾘｱ
            SetDetailPreviewInit();

            // ﾘｽﾄ･ｱｲﾃﾑを生成
            ListViewItem[] item1 = new ListViewItem[endedCont + 1];
            listViewUniqueValue.BeginUpdate();

            imageListSymbol.Images.Add(this.bitmapList[0]);
            item1[0] = new ListViewItem(new string [] { "", this._strUVRDefaultLabel, pUniqueValueRenderer.DefaultLabel }, 0);

            // 個別値ﾘｽﾄを作成
			string	strVal;
            for(int i = 0; i < endedCont; i++) {
                imageListSymbol.Images.Add(this.bitmapList[i + 1]);
				item1[i + 1] = new ListViewItem(new string[] { "", strVal = pUniqueValueRenderer.get_Value(i), pUniqueValueRenderer.get_Label(strVal) }, i + 1);
            }

            listViewUniqueValue.Items.AddRange(item1);
            listViewUniqueValue.EndUpdate();

            listViewUniqueValue.Items[currentIndex].Focused = true;
            //listViewUniqueValue.Items[currentIndex].Selected = true;
            listViewUniqueValue.Items[currentIndex].EnsureVisible();
        }

        /// <summary>
        /// 個別値リストビューの更新
        /// </summary>
        /// <param name="agUVRend">個別分類レンダラー</param>
		/// <param name="IsDelete">削除による更新かどうか</param>
		/// <param name="IsAll">すべて更新するかどうか</param>
        private void UpdateUVListViewItems(IUniqueValueRenderer agUVRend, bool IsDelete, bool IsAll) {
			ListView	ctlLV = this.listViewUniqueValue;

			ISymbol agSym;
			IColor	agClr;
			Bitmap	bmpSym;

			// ﾌﾗｸﾞに加え、選択状況も確認(ﾎﾟｲﾝﾄ,ﾗｲﾝのｻｲｽﾞ変更に対応する)
			int[]	intIDs = IsAll || ctlLV.SelectedIndices.Count == 0 ? Enumerable.Range(0, ctlLV.Items.Count).ToArray() : ctlLV.SelectedIndices.Cast<int>().ToArray();
			if(IsDelete) {
				intIDs = intIDs.Reverse().ToArray();
			}

			ctlLV.BeginUpdate();
			foreach(int intCnt in intIDs) {
				if(IsDelete) {
					// 削除
					ctlLV.Items.RemoveAt(intCnt);
					this.imageListSymbol.Images.RemoveAt(intCnt);		// ｼﾝﾎﾞﾙ･ｲﾒｰｼﾞを削除

					// 保存情報を削除
					m_work_UniqueValueRendererEachColor.RemoveAt(intCnt);
					m_work_UniqueValueRendererEachInfo.RemoveAt(intCnt);
				}
				else {
					// ｼﾝﾎﾞﾙを取得
					agSym = intCnt != 0 ? agUVRend.get_Symbol(agUVRend.get_Value(intCnt - 1)) : agUVRend.DefaultSymbol;

					// ｼﾝﾎﾞﾙ塗り色を取得して保存
					agClr = GetSymbolColor(agSym);
					SaveUniqueValueEachColorToList(intCnt, agClr);
					SaveUniqueValueEachInfoToList(intCnt, SetSelectedItemSymbolAttributes(agSym), true);

					// ｼﾝﾎﾞﾙの画像を作成して保存
					bmpSym = this.CreateSymbolImage(agSym, this.imageListSymbol.ImageSize.Width, this.imageListSymbol.ImageSize.Height, ctlLV.BackColor);
					this.imageListSymbol.Images[intCnt] = bmpSym;

					ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agSym);
				}
			}
			if(IsDelete) {
				// ｼﾝﾎﾞﾙ･ｲﾒｰｼﾞ･ｲﾝﾃﾞｯｸｽを更新
				foreach(int intCnt in Enumerable.Range(0, ctlLV.Items.Count)) {
					ctlLV.Items[intCnt].ImageIndex = intCnt;
				}
			}

			ctlLV.EndUpdate();
			//ctlLV.Select();
        }

        /// <summary>
        /// 確定UVレンダラ
        /// </summary>
        /// <param name="inUVrederer"></param>
        /// <returns></returns>
        private IUniqueValueRenderer GetFixedUniqueValueRenderer(
            IUniqueValueRenderer inUVrederer)
        {
            ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy =
                new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

            // デフォルトシンボル(その他全て)表示非表示切り替え判定
            inUVrederer.UseDefaultSymbol = 
                this.checkBoxUseUniqueValueDefaultSymbol.Checked;
            
            IUniqueValueRenderer edit_UniqueValueRenderer =
                objectCopy.Copy(inUVrederer) as IUniqueValueRenderer;

            return edit_UniqueValueRenderer;
        }

        /// <summary>
        /// プレビュー設定
        /// （数値分類の場合）
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="pclassBreakRenderer"></param>
        private void SetEachClassBreaksToListView(IClassBreaksRenderer pclassBreakRenderer, int currentIndex) {
			// ﾌﾟﾚﾋﾞｭｰ･ﾘｽﾄを初期化
            SetDetailPreviewInitClassBreaks();
            
            Common.Logger.Debug("ClassBreaksRenderer Preview Set STARTED");

            //-->
            int eachColorIdx = 0;
            //<--

            listViewClassBreaks.BeginUpdate();
            for(int i = 0; i < pclassBreakRenderer.BreakCount; i++) {
                try {
                    ISymbol symbol = pclassBreakRenderer.get_Symbol(i);

                    //-->
                    IColor eachColor = GetSymbolColor(symbol);
                    // ｼﾝﾎﾞﾙ･ｶﾗｰを保存
                    SaveClassBreakEaceColorToList(eachColorIdx, eachColor);

                    // ｼﾝﾎﾞﾙ情報を保存
                    SaveClassBreaksEachInfoToList(
                        eachColorIdx, SetSelectedItemSymbolAttributes(symbol), true);

                    eachColorIdx++;
                    //<--

                    //Image image = Common.DrawSymbol.SymbolToImage(symbol, 24, 24);
                    //Bitmap image = Common.DrawSymbol.SymbolToBitmap(symbol, 16, 16);
                    Bitmap image = this.CreateSymbolImage(
                        symbol,
                        this.imageListSymbolClassBreaks.ImageSize.Width,
                        this.imageListSymbolClassBreaks.ImageSize.Height,
                        this.listViewClassBreaks.BackColor);

                    imageListSymbolClassBreaks.Images.Add(image);

                    // ﾌﾟﾚﾋﾞｭｰ･ﾘｽﾄに追加
                    ListViewItem item1 = new ListViewItem("", i);
					item1.SubItems.Add(this.CreateCBLabel(pclassBreakRenderer, i));		// 範囲表示
                    item1.SubItems.Add(pclassBreakRenderer.get_Label(i));				// ﾗﾍﾞﾙ表示
                    
                    listViewClassBreaks.Items.AddRange(new ListViewItem[] { item1 });
                }
                catch(Exception ex) {
                    Common.Logger.Debug(ex.Message);
                }
            }
            listViewClassBreaks.EndUpdate();

            Common.Logger.Debug("ClassBreaksRenderer Preview Set ENDED");

            listViewClassBreaks.Sorting = SortOrder.Ascending;
            listViewClassBreaks.Sorting = SortOrder.Descending;
            listViewClassBreaks.Sort();

            listViewClassBreaks.Items[currentIndex].Focused = true;
            //listViewClassBreaks.Items[currentIndex].Selected = true;
            listViewClassBreaks.Items[currentIndex].EnsureVisible();
        }

        /// <summary>
        /// 数値分類リストビューの更新
        /// </summary>
        /// <param name="agCBRend"></param>
        /// <param name="IsAll"></param>
		private void UpdateCBListViewItems(IClassBreaksRenderer agCBRend) {
			ListView	ctlLV = this.listViewClassBreaks;

			ISymbol agSym;
			IColor	agClr;
			
			// ﾌﾗｸﾞに加え、選択状況も確認(ﾎﾟｲﾝﾄ,ﾗｲﾝのｻｲｽﾞ変更に対応する)
			int[]	intIDs = ctlLV.SelectedIndices.Count <= 0 || ctlLV.SelectedIndices.Count >= ctlLV.Items.Count ? 
				Enumerable.Range(0, ctlLV.Items.Count).ToArray() : ctlLV.SelectedIndices.Cast<int>().ToArray();

			ctlLV.BeginUpdate();
			foreach(int intCnt in intIDs) {
				// ｼﾝﾎﾞﾙを取得
				agSym = agCBRend.get_Symbol(intCnt);

				// ｼﾝﾎﾞﾙ塗り色を取得して保存
				agClr = GetSymbolColor(agSym);
				SaveClassBreakEaceColorToList(intCnt, agClr);
				SaveClassBreaksEachInfoToList(intCnt, SetSelectedItemSymbolAttributes(agSym), true);

				// ｼﾝﾎﾞﾙの画像を作成して保存
				this.imageListSymbolClassBreaks.Images[intCnt] = 
					this.CreateSymbolImage(agSym, 
						this.imageListSymbolClassBreaks.ImageSize.Width, this.imageListSymbolClassBreaks.ImageSize.Height, ctlLV.BackColor);

				ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agSym);
			}
			ctlLV.EndUpdate();
			//ctlLV.Select();
        }

        /// <summary>
        /// 指定カラーランプから指定データ数の色を取得
        /// 指定カラーランプがNullの場合には
        /// デフォルトカラーランプで指定データ数の色を作成
        /// </summary>
        /// <param name="colorRamp">カラーランプ</param>
        /// <param name="count">データ数</param>
        /// <returns>IEnumColors</returns>
        private IEnumColors GetEnumColors(IColorRamp colorRamp, int count) {
            IEnumColors pEnumColors = null;
            //if (colorRamp != null && count >= 1)
            if(colorRamp != null && count > 1) {
                bool bOK;
                colorRamp.Size = count;
                colorRamp.CreateRamp(out bOK);
                pEnumColors = colorRamp.Colors;
                pEnumColors.Reset();
            }
            else {
                // スタイルギャラリからカラーランプ取得していない場合
                
                if(count >= 1) {
					// 既定のｶﾗｰﾗﾝﾌﾟを取得
                    colorRamp = GetColorRampFromStyleGallery(DEFALULT_COLOR_RAMP_NAME);
                    
                    bool bOK;
                    colorRamp.Size = count;
                    colorRamp.CreateRamp(out bOK);
                    pEnumColors = colorRamp.Colors;
                    pEnumColors.Reset();
                }

            }
            return pEnumColors;
        }

        /// <summary>
        /// カラーランプから取得したカラーからカレント色を取得
        /// </summary>
        /// <param name="eumColors">カラーランプから取得したカラー</param>
        /// <returns>IColor</returns>
        private IColor GetIColor(IEnumColors eumColors) {
            IColor color = null;
            
            if(eumColors != null) {
				color = eumColors.Next();
				if(color == null) {
					eumColors.Reset();
					color = eumColors.Next();
				}
			}
            
            // 返却
            return color;
        }

        /// <summary>
        /// 小数点以下の桁数を取得
        /// </summary>
        private int GetScale(double val) {
            int		intRet = 0;
            string	valString = val.ToString().TrimEnd('0');

            int index = valString.IndexOf('.');
			intRet = index < 0 ? 0 : valString.Substring(index + 1).Length;
			
			// 返却
            return intRet;
        }

        /// <summary>
        /// 少数点の有る開始閾値インクリメントの為の増分値を取得
        /// </summary>
        /// <param name="scaleCount">小数点以下桁数</param>
        /// <returns>インクリメント値</returns>
        private decimal GetScaleOffset(int scaleCount) {
			decimal	decRet = 0;

            if(scaleCount > 0) {
				string	strPrecision = "0.".PadRight(scaleCount + 1, '0') + "1";
				decRet = decimal.Parse(strPrecision);
            }
            
            // 返却
            return decRet;
        }



        /// <summary>
        /// 未使用
        /// </summary>
        /// <param name="dValue"></param>
        /// <param name="iDigits"></param>
        /// <returns></returns>
        private double ToRoundUp(double dValue, int iDigits) {
            // 指定数値を,指定した値で累乗
            double dCoef = System.Math.Pow(10, iDigits);

            // System.Math.Ceiling : 指定数以上の数のうち、最小の整数
            // System.Math.Floor   : 指定数以下の数のうち、最大の整数
            return dValue >= 0 ? System.Math.Ceiling(dValue * dCoef) / dCoef :
                                 System.Math.Floor(dValue * dCoef) / dCoef;
        }
        
		/// <summary>
		/// 数値分類用ラベルを作成します
		/// </summary>
		/// <param name="CBRenderer">数値分類レンダラー</param>
		/// <param name="RenderIndex">レンダーID</param>
		/// <returns>ラベル</returns>
		private string CreateCBLabel(IClassBreaksRenderer CBRenderer, int RenderIndex) {
			string	strLabel = "";
			
			// 入力ﾁｪｯｸ
			if(CBRenderer != null && (RenderIndex >= 0 && RenderIndex < CBRenderer.BreakCount)) {
				// 分類ﾌﾟﾛﾊﾟﾃｨを取得
				IClassBreaksUIProperties	agCBUIProps = (IClassBreaksUIProperties)CBRenderer;
				
				// 分類値を取得
				double	dblMinVal = Math.Round(agCBUIProps.get_LowBreak(RenderIndex), 6);
				double	dblMaxVal = Math.Round(CBRenderer.get_Break(RenderIndex), 6);

				// ﾃﾞｼﾏﾙに変換 (倍精度による誤差発生の抑制)
				decimal	decMin = Convert.ToDecimal(dblMinVal);
				decimal	decMax = Convert.ToDecimal(dblMaxVal);
				
				double	dblPreMax;
				string	strFormat;

                // 実数
                if(!this.IsIntegerField(CBRenderer.Field)) {
                    if(RenderIndex > 0) {
						// 下限値のｲﾝｸﾘﾒﾝﾄ (ﾃﾞｼﾏﾙにしないと計算誤差が生じてしまう)
						dblPreMax = Math.Round(CBRenderer.get_Break(RenderIndex - 1), 6);
						if(dblPreMax == dblMinVal) {
							decMin = this.CalcCBMinValue(dblMinVal);
						}
					}
					
					// ﾗﾍﾞﾙ･ﾌｫｰﾏｯﾄ
					strFormat = decMin < decMax ? "{0:f6} - {1:f6}" : "{0:f6}";
					strLabel = string.Format(strFormat, decMin, decMax);
                }
                // 整数
                else {
					if(RenderIndex > 0) {
						// 下限値のｲﾝｸﾘﾒﾝﾄ
						dblPreMax = CBRenderer.get_Break(RenderIndex - 1);
						if(dblPreMax == dblMinVal) {
							decMin += 1;
						}
					}
					// ﾗﾍﾞﾙ･ﾌｫｰﾏｯﾄ
					strFormat = decMin < decMax ? "{0:f0} - {1:f0}" : "{0:f0}";
					strLabel = string.Format(strFormat, decMin, decMax);
				}
			}
                
			// 返却
			return strLabel;
		}
		
		/// <summary>
		/// 数値分類・各分類の下限値を算出します
		/// </summary>
		/// <param name="CBValue">前分類の上限値</param>
		/// <returns>下限値</returns>
		private decimal CalcCBMinValue(double CBValue) {
			decimal	decRet;
			
			if(decimal.TryParse(CBValue.ToString(), out decRet)) {
				int	intScale = this.GetScale(CBValue);
				
				// 数値判定
				if(intScale > 0) {		// 実数
					// ｲﾝｸﾘﾒﾝﾄ値を取得
					decimal	decAdd = this.GetScaleOffset(SCALE_COUNT);
					decRet += decAdd;
				}
				else {					// 整数
					// ｲﾝｸﾘﾒﾝﾄ
					decRet += 1;
				}
			}
			
			// 返却
			return decRet;
		}

	    /// <summary>
        /// 数値分類ラベルから下限値と上限値を取得します
        /// </summary>
        /// <param name="BreakLabel"></param>
        /// <param name="LowerValue"></param>
        /// <param name="UpperValue"></param>
        private void GetBreakValuePair(string BreakLabel, out double LowerValue, out double UpperValue) {
			LowerValue = double.NaN;
			UpperValue = double.NaN;
			
			string[]	strVals = BreakLabel.Split(' ');
			double		dblVal;
			for(int	intCnt= strVals.Length - 1; intCnt >= 0; intCnt--) {
				if(double.TryParse(strVals[intCnt], out dblVal)) {
					if(UpperValue.Equals(double.NaN)) {
						UpperValue = dblVal;
					}
					else if(LowerValue.Equals(double.NaN)) {
						LowerValue = dblVal;
					}
					else {
						break;
					}
				}
			}
        }

        /// <summary>
        /// 指定パラメータで数値分類レンダラを作成
        /// </summary>
        /// <param name="fieldName">作成対象フィールド名</param>
        /// <param name="breakCount">分類数</param>
        /// <param name="colorRamp">カラーランプ</param>
        /// <param name="tsymbol">処理対象シンボル</param>
        /// <param name="useCurrentColor">現状カラー使用判定フラグ</param>
        /// <param name="whereFrom"></param>
        /// <returns>IClassBreaksRenderer</returns>
        private IClassBreaksRenderer DefineClassBreakRenderer(
            string fieldName, 
            string LegendTitle,
            int breakCount,
            IColorRamp colorRamp, 
            ISymbol tsymbol, 
            bool useCurrentColor,
            SymbolUpdateOption UpdateOption)
        {
            if (tsymbol == null) return null;

            // 分類数の調整
            if (breakCount >= 1) breakCount--;
            
            // 分類内容を算出 (実体と諸元)
            IClassBreaksUIProperties	pUIProperties = null;
            IClassBreaksRenderer		classBreaksRenderer = this.GetClassBreaks(fieldName, breakCount, ref pUIProperties);

            //pUIProperties.DeviationInterval
            pUIProperties.ColorRamp = colorSchemeName;

            // 対象ﾌｨｰﾙﾄﾞを設定
            classBreaksRenderer.Field = fieldName;

            // note: minimum break is NOT the first break, it is actually the lowest value    
            // in the data set, the minimum value.
            classBreaksRenderer.MinimumBreak = CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][0];
            
            // 分類数の設定
            int	breakCountEdit = CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex].Length - 1;
            classBreaksRenderer.BreakCount = breakCountEdit;

            // 分類数に応じた設定色を取得
            IEnumColors pEnumColors = this.GetEnumColors(colorRamp, classBreaksRenderer.BreakCount);

			// ﾗﾍﾞﾙ編集用
            StringBuilder labelStr = new StringBuilder();
            for(int i = 0; i < breakCountEdit; i++)  {
                // 閾値設定
                classBreaksRenderer.set_Break(i, CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][i + 1]);

                // 分類範囲の下限値を設定 (諸元)
                pUIProperties.set_LowBreak(i, CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][i]);
                Common.Logger.Debug("label check ... " + classBreaksRenderer.get_Label(i));

                // ﾗﾍﾞﾙを作成
                labelStr = new StringBuilder(this.CreateCBLabel(classBreaksRenderer, i));
                classBreaksRenderer.set_Label(i, labelStr.ToString());

                //-->
                IColor tcolor = null;
                if (m_work_ClassBreaksRendererEachColor != null &&
                    m_work_ClassBreaksRendererEachColor.Count - 1 == classBreaksRenderer.BreakCount)
                {
                    tcolor = m_work_ClassBreaksRendererEachColor[i];
                }
                if(tcolor == null) {
                    tcolor = GetIColor(pEnumColors);
                }
                //<--

                // ｼﾝﾎﾞﾙを設定
                SetClassBreaksSymbol(classBreaksRenderer, tsymbol, i, tcolor, false, useCurrentColor, UpdateOption);
            }
            
            // 入力分類数を調整
            this.numericUpDownBunruiSu.Value = classBreaksRenderer.BreakCount;
            
            // 凡例ﾀｲﾄﾙの調整
            this.SetTOCLegendTitle((ILegendInfo)classBreaksRenderer, LegendTitle);

            // ﾚﾝﾀﾞﾗｰの更新状態を設定
            m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
            m_do_save[this.tabControl1.SelectedIndex] = false;

            return classBreaksRenderer;
        }
        
        /// <summary>
        /// 指定フィールドが整数型かどうかを判定します
        /// </summary>
        /// <param name="FieldName">フィールド名</param>
        /// <returns>整数 / その他</returns>
        private bool IsIntegerField(string FieldName) {
			bool	blnRet = false;
			
			// 対象ﾌｨｰﾁｬｰ･ﾚｲﾔｰの状態を確認
			if(this.m_featureLayer != null) {
				// ﾃﾞｨｽﾌﾟﾚｲ･ﾃｰﾌﾞﾙからﾌｨｰﾙﾄﾞを取得
				IDisplayTable	agDTbl = this.m_featureLayer as IDisplayTable;
				int				intFldId = agDTbl.DisplayTable.FindField(FieldName);
				IField			agFld = agDTbl.DisplayTable.Fields.get_Field(intFldId);
				
				// 整数型かﾁｪｯｸ
				blnRet = agFld.Type == esriFieldType.esriFieldTypeInteger || agFld.Type == esriFieldType.esriFieldTypeSmallInteger;
			}
			
			// 返却
			return blnRet;
        }
        
        /// <summary>
        /// TOCの表示フィールド名を別名に設定します
        /// </summary>
        /// <param name="Renderer">レンダラー</param>
        /// <param name="FieldAlias">フィールド別名</param>
        /// <returns>OK / NG</returns>
        private bool SetTOCLegendTitle(ILegendInfo Renderer, string FieldAlias) {
			bool	blnRet = false;
			
			if(Renderer.LegendGroupCount > 0) {
				// ※ 個別分類時は、その他の値すべてが1つ目となる。（有効時）
				ILegendGroup	liLG = Renderer.get_LegendGroup(Renderer.LegendGroupCount - 1);
				liLG.Heading = FieldAlias;
				
				blnRet = true;
			}
			
			return blnRet;
        }
        
        /// <summary>
        /// 数値分類のシンボルを更新します
        /// </summary>
        /// <param name="CBRenderer"></param>
        /// <param name="pEnumColors"></param>
        /// <param name="tsymbol"></param>
        /// <param name="useCurrentColor"></param>
        /// <param name="whereFrom"></param>
        private IClassBreaksRenderer UpdateCBSymbols(IClassBreaksRenderer CBRenderer, IEnumColors pEnumColors, ISymbol tsymbol, bool useCurrentColor, SymbolUpdateOption UpdateOption) {
			ListView	ctlLV = this.listViewClassBreaks;
			IColor		pColor;

			IObjectCopy agObjCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();
			var agCBR = agObjCopy.Copy(CBRenderer) as IClassBreaksRenderer;

			// 設定対象のｲﾝﾃﾞｯｸｽを取得
			int[]	intIDs = ctlLV.SelectedIndices.Count == 0 ? Enumerable.Range(0, ctlLV.Items.Count).ToArray() : ctlLV.SelectedIndices.Cast<int>().ToArray();
			
            foreach(int intCnt in intIDs) {
				// 色を取得
				if(UpdateOption == SymbolUpdateOption.FROM_COLOR_RAMP) {
					pColor = GetIColor(pEnumColors);
				}
				else if(UpdateOption == SymbolUpdateOption.FROM_STYLE_GALLERY || !useCurrentColor) {
					//pColor = GetSymbolColor(m_symbol_of_renderer[CLASSBREAK_RENDERER]);
					pColor = GetSymbolColor(tsymbol);
				}
				else {
					pColor = GetSymbolColor(CBRenderer.get_Symbol(intCnt));
				}

				// ｼﾝﾎﾞﾙを設定
				SetClassBreaksSymbol(agCBR, tsymbol, intCnt, pColor, false, useCurrentColor, UpdateOption);
			}

            // ﾚﾝﾀﾞﾗｰの更新状態を設定
            m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
            m_do_save[this.tabControl1.SelectedIndex] = false;

			return agCBR;
        }

        /// <summary>
        /// 個別値データ追加
        /// </summary>
        /// <param name="pUniqueValueRenderer">個別分類レンダラー</param>
        /// <param name="tsymbol">シンボル</param>
        /// <param name="fieldName">フィールド別名</param>
        /// <param name="classValue">分類値</param>
        private void AddUniqueValue(
            IUniqueValueRenderer pUniqueValueRenderer, 
            ISymbol tsymbol,
            string fieldName, 
            string classValue)
        {
            ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

            switch (m_geometryType) {
            // ﾎﾟｲﾝﾄ
            case esriGeometryType.esriGeometryPoint:
                if(tsymbol is IMarkerSymbol) {
                    if(tsymbol is ISimpleMarkerSymbol) {
                        // 一度キャストしてスタイルを取得
                        ISimpleMarkerSymbol tmpSymbol = tsymbol as ISimpleMarkerSymbol;

                        // 新規にSimpleMarkerSymbol作成して取得したスタイルを適用
                        // こうしないと指定されたシンボルにカラーランプが適用されない
                        // 個別値の数分だけ同じシンボルを作成するのは無意味に思えるが
                        // こうしないと実際には個別のオブジェクトとして使用できない。
                        ISimpleMarkerSymbol pClassMarkerSymbol = new SimpleMarkerSymbolClass();
                        pClassMarkerSymbol.Style = tmpSymbol.Style;
                        pClassMarkerSymbol.Size = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);

                        pClassMarkerSymbol.OutlineSize = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);

                        pClassMarkerSymbol.OutlineColor = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);

                        pClassMarkerSymbol.Outline = (pClassMarkerSymbol.OutlineSize > 0.0);

                        pUniqueValueRenderer.AddValue(classValue, fieldName, pClassMarkerSymbol as ISymbol);
                        pUniqueValueRenderer.set_Label(classValue, classValue);
                        pUniqueValueRenderer.set_Symbol(classValue, pClassMarkerSymbol as ISymbol);
                    }
                    else {
                        if(tsymbol is ICharacterMarkerSymbol) {
                            ICharacterMarkerSymbol tmpCharacterSymbol = tsymbol as CharacterMarkerSymbol;
                            ICharacterMarkerSymbol characterSymbol = new CharacterMarkerSymbolClass();
                            characterSymbol.CharacterIndex = tmpCharacterSymbol.CharacterIndex;
                            characterSymbol.Size = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);

                            pUniqueValueRenderer.AddValue(classValue, fieldName, characterSymbol as ISymbol);
                            pUniqueValueRenderer.set_Label(classValue, classValue);
                            pUniqueValueRenderer.set_Symbol(classValue, characterSymbol as ISymbol);
                        }
                        else {
                            // 下記の様にコピーしないとシンボルの形状が反映されない
                            IMarkerSymbol markerSymbol = objectCopy.Copy(tsymbol) as IMarkerSymbol;
                            markerSymbol.Size = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);

                            pUniqueValueRenderer.AddValue(classValue, fieldName, markerSymbol as ISymbol);
                            pUniqueValueRenderer.set_Label(classValue, classValue);
                            pUniqueValueRenderer.set_Symbol(classValue, markerSymbol as ISymbol);
                        }
                    }
                }
                break;

            // ﾗｲﾝ
            case esriGeometryType.esriGeometryPolyline:
                if(tsymbol is ISimpleLineSymbol) {
                    ISimpleLineSymbol tmpLineSymbol = tsymbol as ISimpleLineSymbol;
                    ISimpleLineSymbol pClassLineSymbol = new SimpleLineSymbolClass();
                    if(tmpLineSymbol != null) {
                        pClassLineSymbol.Style = tmpLineSymbol.Style; // esriSimpleLineStyle.esriSLSSolid;
                        pClassLineSymbol.Width = tmpLineSymbol.Width;
                        //Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);
                    }

                    pUniqueValueRenderer.AddValue(classValue, fieldName, pClassLineSymbol as ISymbol);
                    pUniqueValueRenderer.set_Label(classValue, classValue);
                    pUniqueValueRenderer.set_Symbol(classValue, pClassLineSymbol as ISymbol);
                }
                else if(tsymbol is IMarkerLineSymbol) {
                    IMarkerLineSymbol markerLineSymbol = objectCopy.Copy(tsymbol) as IMarkerLineSymbol;

                    pUniqueValueRenderer.AddValue(classValue, fieldName, markerLineSymbol as ISymbol);
                    pUniqueValueRenderer.set_Label(classValue, classValue);
                    pUniqueValueRenderer.set_Symbol(classValue, markerLineSymbol as ISymbol);
                }

                else if(tsymbol is IMultiLayerLineSymbol) {
                    IMultiLayerLineSymbol pClassMultiLayerLine = null;
                    try {
                        pClassMultiLayerLine = objectCopy.Copy(tsymbol) as IMultiLayerLineSymbol;
                    }
                    catch(System.OutOfMemoryException outofmem) {
                        Common.Logger.Error("UsingMemorySize:" + GC.GetTotalMemory(false).ToString() + " byte");

                        Common.Logger.Error(outofmem.Message);
                        Common.Logger.Error(outofmem.StackTrace);

                        GC.Collect();
                        Common.Logger.Error("After Full GC.Collect UsingMemorySize:" +
                            GC.GetTotalMemory(false).ToString() + " byte");

                        // retry
                        pClassMultiLayerLine = objectCopy.Copy(tsymbol) as IMultiLayerLineSymbol;
                    }

                    pUniqueValueRenderer.AddValue(classValue, fieldName, pClassMultiLayerLine as ISymbol);
                    pUniqueValueRenderer.set_Label(classValue, classValue);
                    pUniqueValueRenderer.set_Symbol(classValue, pClassMultiLayerLine as ISymbol);
                }
                break;

            // ﾎﾟﾘｺﾞﾝ
            case esriGeometryType.esriGeometryPolygon:
                if(tsymbol is ISimpleFillSymbol) {
                    ISimpleFillSymbol tmpSimpleFillSymbol = tsymbol as ISimpleFillSymbol;
                    ISimpleFillSymbol pClassSimpleFillSymbol = new SimpleFillSymbolClass();

                    pClassSimpleFillSymbol.Style = tmpSimpleFillSymbol.Style;

                    ICartographicLineSymbol cartoLine = new CartographicLineSymbol();
                    cartoLine.Width = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);
                    cartoLine.Color = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
                    pClassSimpleFillSymbol.Outline = cartoLine;

                    //pClassSimpleFillSymbol.Outline.Width =
                    //    Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);

                    //pClassSimpleFillSymbol.Outline.Color =
                    //    ConvertToESRIColor(buttonSetOutlineColor.BackColor);

                    pUniqueValueRenderer.AddValue(classValue, fieldName, pClassSimpleFillSymbol as ISymbol);
                    pUniqueValueRenderer.set_Label(classValue, classValue);
                    pUniqueValueRenderer.set_Symbol(classValue, pClassSimpleFillSymbol as ISymbol);
                }
                else {
                    if(tsymbol is IFillSymbol) {
                        IFillSymbol pClassFillSymbol = objectCopy.Copy(tsymbol) as IFillSymbol;

                        pClassFillSymbol.Outline.Width = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);

                        pClassFillSymbol.Outline.Color = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);

                        pUniqueValueRenderer.AddValue(classValue, fieldName, pClassFillSymbol as ISymbol);
                        pUniqueValueRenderer.set_Label(classValue, classValue);
                        pUniqueValueRenderer.set_Symbol(classValue, pClassFillSymbol as ISymbol);
                    }
                    else if(tsymbol is IMultiLayerFillSymbol) {
                        IMultiLayerFillSymbol pClassMultiLayerFillSymbol = objectCopy.Copy(tsymbol) as IMultiLayerFillSymbol;

                        pClassMultiLayerFillSymbol.Outline.Width = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);

                        pClassMultiLayerFillSymbol.Outline.Color = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);

                        pUniqueValueRenderer.AddValue(classValue, fieldName, pClassMultiLayerFillSymbol as ISymbol);
                        pUniqueValueRenderer.set_Label(classValue, classValue);
                        pUniqueValueRenderer.set_Symbol(classValue, pClassMultiLayerFillSymbol as ISymbol);
                    }
                }
                break;
            }
        }

        /// <summary>
        /// 指定パラメータで個別値レンダラを作成
        /// </summary>
        /// <param name="fieldName">処理対象フィールド名</param>
		/// <param name="LegendTitle">凡例タイトル(フィールド別名)</param>
        /// <param name="colorRamp">使用カラーランプ</param>
        /// <param name="tsymbol">使用シンボル</param>
        /// <param name="useCurrentColor">現状カラー使用判定フラグ</param>
        /// <param name="useColorRamp"></param>
        /// <returns>IUniqueValueRenderer</returns>
        private IUniqueValueRenderer DefineUniqueValueRenderer(
            string fieldName,
            string LegendTitle,
            IColorRamp colorRamp,
            ISymbol tsymbol,
            bool useCurrentColor,
            bool useColorRamp)
        {
			Common.Logger.Debug("DefineUniqueValueRenderer STARTED");

			if(tsymbol == null) return null;

			// 個別分類ﾚﾝﾀﾞﾗｰを生成
			IUniqueValueRenderer	pUniqueValueRenderer = null;
            IFeatureCursor			pFeatureCursor = null;
            
            try {
                Common.Logger.Debug("UsingMemorySize Before GC :" + GC.GetTotalMemory(false).ToString() + " byte");
                GC.Collect();
                Common.Logger.Debug("UsingMemorySize After  GC :" + GC.GetTotalMemory(false).ToString() + " byte");

                // These properties should be set prior to adding values.
				pUniqueValueRenderer = new UniqueValueRendererClass();

				// 分類ﾌｨｰﾙﾄﾞを設定
				pUniqueValueRenderer.FieldCount = 1;
				pUniqueValueRenderer.set_Field(0, fieldName);

                //--> 2011/04/27 DefaultSymbol 必ずオブジェクトコピー
                ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();
                pUniqueValueRenderer.DefaultSymbol = objectCopy.Copy(tsymbol) as ISymbol;

                // その他の値の取扱い設定
                pUniqueValueRenderer.UseDefaultSymbol = this.checkBoxUseUniqueValueDefaultSymbol.Checked;

                IGeoFeatureLayer	pGeoFeatureLayer = (IGeoFeatureLayer)m_featureLayer;
                IDisplayTable		pDisplayTable = pGeoFeatureLayer as IDisplayTable;

                Common.Logger.Debug("feature class field count=" + m_featureLayer.FeatureClass.Fields.FieldCount);
                Common.Logger.Debug("getfeature layer field count=" + pDisplayTable.DisplayTable.Fields.FieldCount);

                // 2010-12-28 add
                int row_cnt = pDisplayTable.DisplayTable.RowCount(null);
                if(row_cnt > UNIQUEVALUE_MAX) {
                    Common.Logger.Debug("個別値全件数 = " + row_cnt.ToString());
                }
                // end add

                object[] maxcount_args = new object[2];
                maxcount_args[0] = UNIQUEVALUE_MAX;
                maxcount_args[1] = row_cnt;

       			pFeatureCursor = pDisplayTable.SearchDisplayTable(null, false) as IFeatureCursor;
                IFeature		pFeature = pFeatureCursor.NextFeature();

                IFields			pFields = pFeatureCursor.Fields;
                int				fieldIndex = pFields.FindField(fieldName);
				IField			tfield = null;

                Common.Logger.Debug("UniqueValueRenderer Value Set Loop STARTED");

				// 個別ｼﾝﾎﾞﾙをﾛｰﾄﾞして、ﾘｽﾄに表示
                int				limitUniqueValueCount = 0;
                List<string>	listClassValues = new List<string>();

                while(pFeature != null) {
                    // ﾌｨｰﾙﾄﾞ･ﾃﾞｰﾀを取得
                    string eachClassValue = string.Empty;
					if(tfield == null) {
						tfield = pFeature.Fields.get_Field(fieldIndex);
					}

                    if(tfield.Type == esriFieldType.esriFieldTypeString) {
                        eachClassValue = pFeature.get_Value(fieldIndex) as string;
                    }
                    else if(tfield.Type == esriFieldType.esriFieldTypeSmallInteger ||
                            tfield.Type == esriFieldType.esriFieldTypeInteger ||
                            tfield.Type == esriFieldType.esriFieldTypeSingle ||
                            tfield.Type == esriFieldType.esriFieldTypeDouble)
                    {
                        eachClassValue = Convert.ToString(pFeature.get_Value(fieldIndex));
                    }

                    if(eachClassValue != string.Empty) {
                        if(!listClassValues.Contains(eachClassValue)) {
                            listClassValues.Add(eachClassValue);

                            // 個別値データ作成
                            AddUniqueValue(pUniqueValueRenderer, tsymbol, fieldName, eachClassValue);

                            // 個別値最大件数の制限判定
                            limitUniqueValueCount++;
                            if (limitUniqueValueCount > UNIQUEVALUE_MAX) // 2011-01-05del  break;
                            {
                                Common.MessageBoxManager.ShowMessageBoxWarining(
                                    this,
                                    string.Format(
                                    Properties.Resources.FormSymbolSettings_Warning_UniuqueValue_MaxCount,
                                    maxcount_args));

                                break;
                            }
                        }
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
                Common.Logger.Debug("UniqueValueRenderer Value Set Loop ENDED");

                // 取得した個別値データ件数分のカラーランプ取得
                IEnumColors pEnumColors = GetEnumColors(colorRamp, pUniqueValueRenderer.ValueCount);

                Common.Logger.Debug("UniqueValueRenderer Symbol Set Color Loop STARTED");
                for(int j = 0; j <= pUniqueValueRenderer.ValueCount; j++) {
                    IColor tcolor = null;
                    
					// 配色を取得
					if(!useColorRamp) {
                        // シンボル変更時、カラーランプでの表示更新時の場合は取得カラーランプで表示にする
                        if(useCurrentColor
							&& (m_work_UniqueValueRendererEachColor != null && m_work_UniqueValueRendererEachColor.Count - 1 == pUniqueValueRenderer.ValueCount)
							&& m_work_UniqueValueRendererEachColor[j] != null) {
							tcolor = m_work_UniqueValueRendererEachColor[j];
                        }
                    }
                    if(tcolor == null) {
						tcolor = (j == 0 ? this.CreateRandomColor() : GetIColor(pEnumColors));
						if(useCurrentColor) {
							useCurrentColor = false;
						}
                    }

                    // 個別にｼﾝﾎﾞﾙを設定
					SetUniqueValueSymbol(pUniqueValueRenderer, j - 1, tcolor, useCurrentColor, false, useColorRamp);
                }

                Common.Logger.Debug("UniqueValueRenderer Symbol Set Color Loop ENDED");

                // '** If you didn't use a predefined color ramp
                // '** in a style, use "Custom" here. Otherwise,
                // '** use the name of the color ramp you selected.
                pUniqueValueRenderer.ColorScheme = colorSchemeName;
                ITable pTable = pDisplayTable as ITable;

                // 対象ﾌｨｰﾙﾄﾞのﾃﾞｰﾀが文字列型か判定
                bool isString = (pTable.Fields.get_Field(fieldIndex).Type == esriFieldType.esriFieldTypeString);
                pUniqueValueRenderer.set_FieldType(0, isString);
                
                Common.Logger.Debug("DefineUniqueValueRenderer ENDED Normal");

	            // 凡例ﾀｲﾄﾙの調整
				this.SetTOCLegendTitle((ILegendInfo)pUniqueValueRenderer, LegendTitle);

                m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
                m_do_save[this.tabControl1.SelectedIndex] = false;
            }
            catch(Exception ex) {
                Common.UtilityClass.DoOnError(ex);
				if(pUniqueValueRenderer != null) {
					pUniqueValueRenderer = null;
				}
            }
            finally {
                //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pFeatureCursor);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                Common.Logger.Debug("DefineUniqueValueRenderer Exit");
            }

            return pUniqueValueRenderer;
        }

        /// <summary>
        /// 既存の個別値読込み後のプレビュー更新時
        /// </summary>
        /// <param name="inputUV">個別分類レンダラー</param>useColorRamp
        /// <param name="fieldName">フィールド名</param>
		/// <param name="LegendTitle">フィールド別名</param>
        /// <param name="colorRamp"></param>
        /// <param name="tsymbol"></param>
        /// <param name="useCurrentColor"></param>
        /// <param name="deleteSymbol">選択シンボルを削除フラグ</param>
        /// <param name="UpdateOption">スタイル変更 / カラーランプ設定</param>
        /// <returns></returns>
        private IUniqueValueRenderer DefineUniqueValueRendererUpdate(
            IUniqueValueRenderer inputUV,
            string	fieldName,
            string	LegendTitle,
            IColorRamp colorRamp,
            ISymbol tsymbol,
            bool useCurrentColor,
            bool deleteSymbol,
            SymbolUpdateOption UpdateOption)
        {
            Common.Logger.Debug("DefineUniqueValueRendererUpdate STARTED");

            if(tsymbol == null) return null;

            IUniqueValueRenderer pUniqueValueRenderer = null;

            try {
                Common.Logger.Debug("UsingMemorySize Before GC :" + GC.GetTotalMemory(false).ToString() + " byte");
                GC.Collect();
                Common.Logger.Debug("UsingMemorySize After  GC :" + GC.GetTotalMemory(false).ToString() + " byte");

                //--> 2011/04/27 DefaultSymbol 必ずオブジェクトコピー
                ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

                // 個別値削除処理
				if(deleteSymbol) {
                    if(m_work_UniqueValueRendererSetOthersSymbol != null &&
                        m_work_UniqueValueRendererSetOthersSymbol.Count > 0) {

						// 削除時は現状のUVRをコピー
						pUniqueValueRenderer = objectCopy.Copy(inputUV) as IUniqueValueRenderer;

						string	strVal;
						foreach(int intCnt in Enumerable.Range(0, pUniqueValueRenderer.ValueCount)) {
							if(m_work_UniqueValueRendererSetOthersSymbol.Count <= intCnt + 1) {
								break;
							}
							// ｼﾝﾎﾞﾙ(分類値)を削除
							else if(m_work_UniqueValueRendererSetOthersSymbol[intCnt + 1]) {
								strVal = inputUV.get_Value(intCnt);

								// 削除時は現状のUVRからコピーしない内容から取り除く
								pUniqueValueRenderer.RemoveValue(strVal);
							}
						}
					}
                }
				//
				else if(UpdateOption == SymbolUpdateOption.FROM_STYLE_GALLERY) {
					// ※ m_work_UniqueValueRendererSetOthersSymbolに選択ｼﾝﾎﾞﾙをﾏｰｸしてある
                    if(m_work_UniqueValueRendererSetOthersSymbol != null &&
                        m_work_UniqueValueRendererSetOthersSymbol.Count > 0) {

						// 現在設定されているUVRをコピー
						//pUniqueValueRenderer = objectCopy.Copy(inputUV) as IUniqueValueRenderer;
						pUniqueValueRenderer = inputUV;

						foreach(int intCnt in Enumerable.Range(0, m_work_UniqueValueRendererSetOthersSymbol.Count)) {
							// ｼﾝﾎﾞﾙ(分類値)を変更
							if(m_work_UniqueValueRendererSetOthersSymbol[intCnt]) {
								this.ChangeUniqueValueSymbol(pUniqueValueRenderer, tsymbol, intCnt - 1);
							}
						}
					}
				}
                else {
					// 個別値ﾚﾝﾀﾞﾗｰを再作成
					pUniqueValueRenderer = objectCopy.Copy(inputUV) as IUniqueValueRenderer;
                
					Common.Logger.Debug("UniqueValueRenderer Value Set Loop STARTED");

					// 取得した個別値データ件数分のカラーランプ取得
					IEnumColors pEnumColors = GetEnumColors(colorRamp, pUniqueValueRenderer.ValueCount);
					IColor		tcolor;
					bool		useColorRamp = UpdateOption == SymbolUpdateOption.FROM_COLOR_RAMP;

					Common.Logger.Debug("UniqueValueRendererUpdate Symbol Set Color Loop STARTED");
					for(int j = 0; j <= pUniqueValueRenderer.ValueCount - 1; j++) {

						// ﾃﾞﾌｫﾙﾄ･ｼﾝﾎﾞﾙ設定
						if(j <= 0  && pUniqueValueRenderer.UseDefaultSymbol) {
							tcolor = useColorRamp ? CreateRandomColor() : 
								useCurrentColor ?  m_work_UniqueValueRendererEachColor[j] : GetSymbolColor(tsymbol);

							// デフォルトシンボル時も引数updateColorをfalse
							SetUniqueValueSymbol(pUniqueValueRenderer, -1, tcolor, false, false, useColorRamp);
						}

						// 個別値のｼﾝﾎﾞﾙをｾｯﾄ
						tcolor = useColorRamp ? GetIColor(pEnumColors) : 
							useCurrentColor ? m_work_UniqueValueRendererEachColor[j + 1] : GetSymbolColor(tsymbol);
						SetUniqueValueSymbol(pUniqueValueRenderer, j, tcolor, useCurrentColor, false, useColorRamp);
					}

					Common.Logger.Debug("UniqueValueRendererUpdate Symbol Set Color Loop ENDED");

					// '** If you didn't use a predefined color ramp
					// '** in a style, use "Custom" here. Otherwise,
					// '** use the name of the color ramp you selected.
					pUniqueValueRenderer.ColorScheme = colorSchemeName;
					

					// 凡例ﾀｲﾄﾙの調整
					this.SetTOCLegendTitle((ILegendInfo)pUniqueValueRenderer, LegendTitle);
				}

                Common.Logger.Debug("DefineUniqueValueRendererUpdate ENDED Normal");

                // 変更状況をｾｯﾄ
				m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
                m_do_save[this.tabControl1.SelectedIndex] = false;
            }
            catch(Exception ex) {
                Common.UtilityClass.DoOnError(ex);
				if(pUniqueValueRenderer != null) {
					pUniqueValueRenderer = null;
				}
            }
            finally {
                Common.Logger.Debug("DefineUniqueValueRendererUpdate Exit");
            }

            return pUniqueValueRenderer;
        }

        /// <summary>
        /// 現在設定されている数値分類レンダラーの読み込み
        /// </summary>
        /// <param name="classBreakRenderer"></param>
        private void SetClassBreakRendererTab(IClassBreaksRenderer classBreakRenderer)
        {
            // サイズ
            numericUpDownSimpleSymbolSizeOrWidth.Value = Convert.ToDecimal(DEFALUT_POINT_SIZE);

            imageListSymbolClassBreaks.Images.Clear();
            listViewClassBreaks.Items.Clear();
            listViewClassBreaks.LargeImageList = imageListSymbolClassBreaks;

			// ﾚｲﾔｰから直接現在の数値分類ﾚﾝﾀﾞﾗｰを取得
            IGeoFeatureLayer			pGeoFeatureLayer = (IGeoFeatureLayer)m_featureLayer;
            IClassBreaksRenderer		pClassBreaksRenderer = pGeoFeatureLayer.Renderer as IClassBreaksRenderer;
            IClassBreaksUIProperties	pUIProperties = pClassBreaksRenderer as IClassBreaksUIProperties;

            if (pUIProperties != null && pUIProperties.Method != null) 
            {
                // ｶﾗｰﾗﾝﾌﾟの名称を取得
                colorSchemeName = pUIProperties.ColorRamp;
                if (colorSchemeName == null || colorSchemeName.Length == 0)
                    colorSchemeName = DEFALULT_COLOR_RAMP_NAME;

                // 分類手法の読み出し
                int	intItemID = this.GetCBMethodID(pUIProperties);
                if(intItemID >= 0) {
					this.comboBoxBunruiSyuhou.SelectedIndex = intItemID;
				}
            }

            // フィールド名称サンプル
            this.ClassBreaksSelectedFieldName = classBreakRenderer.Field;

            ISymbol sampleSymbol = null;
            ISymbol sampleEndSymbol = null;

            //-->
            int eachColorIdx = 0;
            //<--

            // ﾘｽﾄを更新
            listViewClassBreaks.BeginUpdate();

            for (int i = 0; i < classBreakRenderer.BreakCount; i++)
            {
                ISymbol symbol = classBreakRenderer.get_Symbol(i);

                //-->
                IColor eachColor = GetSymbolColor(symbol);
                SaveClassBreakEaceColorToList(eachColorIdx, eachColor);

                SaveClassBreaksEachInfoToList(
                    eachColorIdx, SetSelectedItemSymbolAttributes(symbol), true);

                eachColorIdx++;
                //<--

                // ｶﾗｰﾗﾝﾌﾟ取得用に最初と最後のｼﾝﾎﾞﾙを保存
                if (i == 0) sampleSymbol = symbol;
                if (i == classBreakRenderer.BreakCount-1) sampleEndSymbol = symbol;

                //Image img = Common.DrawSymbol.SymbolToImage(symbol, 24, 24);
                Image img = this.CreateSymbolImage(
                    symbol,
                    this.imageListSymbolClassBreaks.ImageSize.Width,
                    this.imageListSymbolClassBreaks.ImageSize.Height,
                    this.listViewClassBreaks.BackColor);

                imageListSymbolClassBreaks.Images.Add(img);

				// 分類ﾘｽﾄを構成
                ListViewItem	item1 = new ListViewItem("", i);
                item1.SubItems.Add(this.CreateCBLabel(classBreakRenderer, i));		// 範囲表示
                item1.SubItems.Add(classBreakRenderer.get_Label(i));				// ﾗﾍﾞﾙ表示
                listViewClassBreaks.Items.AddRange(new ListViewItem[] { item1 });
            }

            listViewClassBreaks.EndUpdate();

            // 分類数を読み出し
            numericUpDownBunruiSu.Value = classBreakRenderer.BreakCount;

            // 2011/05/13 デフォルトシンボルをObjectCopy -->
            //m_symbol_of_renderer[this.tabControl1.SelectedIndex] = sampleSymbol;
            ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

            m_symbol_of_renderer[this.tabControl1.SelectedIndex] = objectCopy.Copy(sampleSymbol) as ISymbol;
            //<--

            SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], true);

            // ｶﾗｰﾗﾝﾌﾟを取得
            if (colorSchemeName != null)
                this.style_colorRamp = GetColorRampFromStyleGallery(colorSchemeName);
            
            // 該当ｶﾗｰﾗﾝﾌﾟが取得不可の場合、作成
            if (this.style_colorRamp == null)
                GetCurrentColor(sampleSymbol, sampleEndSymbol, classBreakRenderer.BreakCount);
                
            // 分類値ﾊﾞｯﾌｧを取得
            this.GetExistBreaks(classBreakRenderer);
        }

        /// <summary>
        /// 色設定関連コントロール使用可能判定
        /// </summary>
        /// <returns></returns>
        private bool CanUseColorButtons() {
            switch(this.tabControl1.SelectedIndex) {
            case SIMPLE_RENDERER:
                return true;
                //break;

            case UNIQUEVALUE_RENDERER:
                return (this.listViewUniqueValue.SelectedItems.Count > 0);
                //break;

            case CLASSBREAK_RENDERER:
                return (this.listViewClassBreaks.SelectedItems.Count > 0);
                //break;
            }
            return false;
        }

        /// <summary>
        /// 処理対象タブのコントロール設定
        /// </summary>
        private void SetTabControls() {
            panelParameter.Enabled = true;
            buttonApply.Enabled = true;
            buttonOK.Enabled = true;

            //SetColorButtonEnable = CanUseColorButtons();// true;

            IGeoFeatureLayer pGeoFeatureLayer = (IGeoFeatureLayer)m_featureLayer;
            switch(this.tabControl1.SelectedIndex) {
            case SIMPLE_RENDERER:
                    
                //InitLoad = true;
                SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], false);
                //InitLoad = false;

				//this.labelSetColor.Enabled = SetColorButtonEnable;
				//this.buttonSetColor.Enabled = SetColorButtonEnable;

                // ｶﾗｰﾗﾝﾌﾟ/画像選択 ﾎﾞﾀﾝの設定
                switch(m_geometryType) {
                case esriGeometryType.esriGeometryPoint:
					this.buttonOpenImageFileOrColorRamp.Text = "画像";
		            this.buttonOpenImageFileOrColorRamp.Visible = true;
                    break;

                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryPolygon:
                    this.buttonOpenImageFileOrColorRamp.Visible = false;
                    break;
                }

                this.buttonOpenSyleGallery.Visible = true;		// ｽﾀｲﾙ選択ﾎﾞﾀﾝ
                this.panelSimpleSymbolOutline.Visible = true;	// ｱｳﾄﾗｲﾝ編集
                this.buttonUpdatePreview.Visible = false;		// ﾌﾟﾚﾋﾞｭｰ更新ﾎﾞﾀﾝ
                this.panelBunrui.Visible = false;				// 分類設定

				this.panelSimpleSymbolOutline.Enabled = true;

                break;

            case UNIQUEVALUE_RENDERER:

				// 塗りｺﾝﾄﾛｰﾙ
                SetColorButtonEnable = false;
                this.labelSetColor.Enabled = SetColorButtonEnable;
                this.buttonSetColor.Enabled = SetColorButtonEnable;

                // ｶﾗｰﾗﾝﾌﾟ選択ﾎﾞﾀﾝ
				this.buttonOpenImageFileOrColorRamp.Text = "カラーランプ";
                this.buttonOpenImageFileOrColorRamp.Visible = true;
                this.buttonUpdatePreviewUniqueValue.Visible = true;			// ﾌﾟﾚﾋﾞｭｰ更新ﾎﾞﾀﾝ
                this.panelBunrui.Visible = false;							// 分類設定

                if(InitLoad) return;

                if(this.comboBoxFieldName.SelectedIndex < 0) {
                    Common.MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormSymbolSettings_UniqueValueDataNotFound);

                    listViewUniqueValue.Enabled = false;
                    comboBoxFieldName.Enabled = false;
                    panelParameter.Enabled = false;
                    buttonApply.Enabled = false;
                    buttonOK.Enabled = false;
                    buttonUpdatePreviewUniqueValue.Enabled = false;
                    return;
                }
                //InitLoad = true;
                SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], false);
                //InitLoad = false;

				this.panelSimpleSymbolOutline.Enabled = this.listViewUniqueValue.Items.Count > 0;

                break;

            case CLASSBREAK_RENDERER:
				// 塗りｺﾝﾄﾛｰﾙ
                SetColorButtonEnable = false;
                this.labelSetColor.Enabled = SetColorButtonEnable;
                this.buttonSetColor.Enabled = SetColorButtonEnable;

				// ｶﾗｰﾗﾝﾌﾟ選択ﾎﾞﾀﾝ
                this.buttonOpenImageFileOrColorRamp.Text = "カラーランプ";
                this.buttonOpenImageFileOrColorRamp.Visible = true;
                this.buttonUpdatePreview.Visible = true;					// ﾌﾟﾚﾋﾞｭｰ更新ﾎﾞﾀﾝ
                this.panelBunrui.Visible = true;							// 分類設定

                if(InitLoad) return;

                if(this.comboBoxFieldNameClassBreaks.SelectedIndex < 0) {
                    Common.MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormSymbolSettings_ClassBreaksDataNotFound);
                        
                    listViewClassBreaks.Enabled = false;
                    comboBoxFieldNameClassBreaks.Enabled = false;
                    panelParameter.Enabled = false;
                    buttonApply.Enabled = false;
                    buttonOK.Enabled = false;
                    buttonUpdatePreview.Enabled = false;
                    return;
                }

                listViewClassBreaks.Enabled = true;
                comboBoxFieldNameClassBreaks.Enabled = true;
                panelParameter.Enabled = true;

                //InitLoad = true;
                SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], false);
                //InitLoad = false;

				this.panelSimpleSymbolOutline.Enabled = this.listViewClassBreaks.Items.Count > 0;

                break;
            }
        }

        /// <summary>
        /// 個別値シンボルプレビュー更新
        /// 個別値の場合、件数多い場合に時間かかるので、最初はサブスレッドで実行
        /// 選択されたシンボルによっては編集用レンダラのXMLシリアライズ時にエラーになる。
        /// その場合にはリトライモードに切り替え、メインスレッド側で処理をする。
        /// </summary>
        private void UpdatePreview(
            IColorRamp colorRamp, 
            ISymbol symbol, 
            bool useCurrentColor,			// ﾎﾟﾘｺﾞﾝ時(ｽﾀｲﾙ変更)既存の塗りを使用
            bool deleteSymbol,				// ｼﾝﾎﾞﾙ削除
            SymbolUpdateOption UpdateOption	// ｽﾀｲﾙ OR ｶﾗｰﾗﾝﾌﾟ
		) {
			// 選択ﾌｨｰﾙﾄﾞ･ｵﾌﾞｼﾞｪｸﾄを取得
            Common.FieldComboItem	itmSelFld = (Common.FieldComboItem)this.comboBoxFieldName.SelectedItem;
			if(itmSelFld == null) return;

            //string tname = comboBoxFieldName.SelectedItem.ToString();
            //if (tname == null || tname.Length == 0) return;

            // カラーランプ時でも個別削除は適用状態で表示
            // スタイルの場合でも同様で色は特に個別色変更あればそれを反映
            if (m_work_UniqueValueRenderer != null) //  && !fromColorRampOrSytle)
            {
                m_work_UniqueValueRenderer =
                    DefineUniqueValueRendererUpdate(
                        m_work_UniqueValueRenderer,
                        itmSelFld.FieldName, itmSelFld.FieldAlias,
                        colorRamp, 
                        symbol, 
                        useCurrentColor, deleteSymbol, UpdateOption);

				// ﾘｽﾄﾋﾞｭｰを更新
				this.UpdateUVListViewItems(m_work_UniqueValueRenderer, deleteSymbol, UpdateOption == SymbolUpdateOption.FROM_COLOR_RAMP);
            }
            else {
				// 個別分類を新規に作成する
                m_work_UniqueValueRenderer =
                    DefineUniqueValueRenderer(itmSelFld.FieldName, itmSelFld.FieldAlias, colorRamp, symbol, useCurrentColor, UpdateOption == SymbolUpdateOption.FROM_COLOR_RAMP);

				// ﾘｽﾄﾋﾞｭｰを設定
				SetEachUniqueValueToListView(m_work_UniqueValueRenderer, 0);
            }
        }

        /// <summary>
        /// 数値分類のプレビューを更新します
        /// </summary>
        /// <param name="colorRamp"></param>
        /// <param name="symbol"></param>
        /// <param name="useCurrentColor"></param>
        /// <param name="whereFrom">0:StyleGallery, 1:ColorRamp, 2:UpdatePreview (default)</param>
        private void UpdatePreviewClassBreaks(IColorRamp colorRamp, ISymbol symbol, bool useCurrentColor, SymbolUpdateOption UpdateOption) {
			// 分類ﾌｨｰﾙﾄﾞ･ｵﾌﾞｼﾞｪｸﾄを取得
            Common.FieldComboItem	itmSelFld = (Common.FieldComboItem)this.comboBoxFieldNameClassBreaks.SelectedItem;
            if(itmSelFld != null) {
				// 分類数を取得
				int		breakCount = Convert.ToInt32(numericUpDownBunruiSu.Value);

				// 既存分類ﾁｪｯｸ　※ﾌｨｰﾙﾄﾞ及び、分類種別と分類数に変更がない場合は、ｼﾝﾎﾞﾙのみ更新
				if(m_work_ClassBreaksRenderer != null && 
					this.comboBoxBunruiSyuhou.SelectedIndex.Equals(this.GetCBMethodID((IClassBreaksUIProperties)m_work_ClassBreaksRenderer)) &&
					m_work_ClassBreaksRenderer.BreakCount.Equals(breakCount) && this.m_work_ClassBreaksRenderer.Field.Equals(itmSelFld.FieldName)) {

					// ｼﾝﾎﾞﾙのみ更新
					m_work_ClassBreaksRenderer = 
						this.UpdateCBSymbols(m_work_ClassBreaksRenderer, this.GetEnumColors(colorRamp, breakCount), symbol, useCurrentColor, UpdateOption);

					// ﾘｽﾄ･ﾋﾞｭｰを更新
					this.UpdateCBListViewItems(m_work_ClassBreaksRenderer);
				}
                // ﾚﾝﾀﾞﾗｰを作成
				else {
					m_work_ClassBreaksRenderer =  
						DefineClassBreakRenderer(itmSelFld.FieldName, itmSelFld.FieldAlias, breakCount, colorRamp, symbol, useCurrentColor, UpdateOption);

					// ﾘｽﾄ･ﾋﾞｭｰを更新
					if(m_work_ClassBreaksRenderer != null) {
						SetEachClassBreaksToListView(m_work_ClassBreaksRenderer, 0);
					}
				}
            }
        }

        /// <summary>
        /// ポリゴン塗りシンボルの淵、塗りを変更します
        /// </summary>
        /// <returns></returns>
        private bool SetFillSymbolOutline(bool init) {
            IFillSymbol simplefil = m_symbol_of_renderer[this.tabControl1.SelectedIndex] as IFillSymbol;
            if(simplefil == null) return false;

            if(init) {
				if(Convert.ToDecimal(simplefil.Outline.Width) >= numericUpDownSimpleSymbolOutLineWidth.Minimum) {
                    numericUpDownSimpleSymbolOutLineWidth.Value = Convert.ToDecimal(simplefil.Outline.Width);
				}
            }
            else {
                if(TabChanged) {
                    numericUpDownSimpleSymbolOutLineWidth.Value = Convert.ToDecimal(simplefil.Outline.Width);
                }
                else {
					IRgbColor	agColor;

					// ｱｳﾄﾗｲﾝ
                    if(simplefil.Outline is ILineSymbol) {
                        ILineSymbol lineSymbol = (ILineSymbol)simplefil.Outline;

						// 色変更(非ﾛｯｸの場合)
						if(CanChangeColor(simplefil as ISymbol, true)) {
							// 設定色を作成
							if(!this.buttonSetOutlineColor.BackColor.Equals(Color.Transparent)) {
								agColor = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
							}
							else {
								agColor = new RgbColorClass();
								agColor.NullColor = true;
							}
                            lineSymbol.Color = agColor;
						}

						// 幅変更
                        lineSymbol.Width = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);
                        
						// ｱｳﾄﾗｲﾝを更新
						simplefil.Outline = lineSymbol;
                    }

                    // 2011/05/26 simplefil.Color != nullの部分追加 
                    if(simplefil.Color != null && CanChangeColor(simplefil as ISymbol, false)) {
						// 設定色を作成
						if(!this.buttonSetColor.BackColor.Equals(Color.Transparent)) {
							agColor = Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
						}
						else {
							agColor = new RgbColorClass();
							agColor.NullColor = true;
						}
						
                        simplefil.Color = agColor;
                    }
                }
            }
            return true;
        }

        #region event handler
        /// <summary>
        /// キャンセル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Escape Key押下で実行される場合には,この処理が抜けてから、
            // numericUpDownSimpleSymbolSizeOrWidthサイズ変更イベントが処理される。
            // その為、そのイベント処理をパスするフラグをOnする。
            Pass_Preview_Process_Escape = true;
            
            this.Close();
        }

        /// <summary>
        /// 色設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetColor_Click(object sender, EventArgs e) {
			Button	ctlBtn = sender as Button;

            bool dispMessage = false;
            try {
                //設定ファイルが存在するか確認する
                if(!ESRIJapan.GISLight10.Common.ApplicationInitializer.IsUserSettingsExists()) {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                    //this.Dispose();
                    return;
                }

                try {
                    //選択された色をボタンの背景色に設定する
                    Color newColor = Common.UtilityClass.GetColor(ctlBtn.BackColor);
                    if(newColor == ctlBtn.BackColor) return; // 変更なし
                    
                    ctlBtn.BackColor = newColor;
                }
                catch(Exception ex) {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                        (this,
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ カラーダイアログ ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error
                        (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ カラーダイアログ ]" +
                         Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    //this.Dispose();
                    return;
                }                    

                //bool refresh = true;
				//if(m_geometryType == esriGeometryType.esriGeometryPolygon) {
				//	refresh = SetFillSymbolOutline(false);
				//}
                //if(refresh) {
                    //SetPreviewImage(m_symbol_of_renderer[this.tabControl1.SelectedIndex], buttonSetColor.BackColor);

					if(SetUpdateProperties()) {
						if(this.tabControl1.SelectedIndex == SIMPLE_RENDERER) {
							// 単一ｼﾝﾎﾞﾙのﾌﾟﾚﾋﾞｭｰを更新
							UpdatePreviewImage();
						}
						else {
							// ﾌﾟﾚﾋﾞｭｰを更新
							DoButtonUpdatePreview(!(ctlBtn.Name == "buttonSetColor"));
						}
					}

                    // OK適用時更新
                    if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER ||
                        this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER)
                    {
                        // 個別、数値の場合本来は個別色選択は指定不可であったのを
                        // 指定可能にしたが、この場合の色変更はプレビュー更新実行しないと
                        // 反映しないという前提あるので、ここで
                        // m_do_renderer_update[this.tabControl1.SelectedIndex]のｽｲｯﾁ切り替えは不要
                    }
                    else {
                        m_do_renderer_update[this.tabControl1.SelectedIndex] = false;
                    }
                //}
            }
            catch(Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally {
                if(dispMessage) {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.FormSymbolSettings_ErrorWhen_buttonSetColor_Click);
                }
            }
        }


        /// <summary>
        /// スタイルファイル表示と選択されたスタイルの反映
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenSyleGallery_Click(object sender, EventArgs e) {
            bool dispMessage = false;
            FormStyleGallery symbolFormStyle = null;

            IRgbColor rgb = 
                Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
            
            try {
                Common.Logger.Debug("buttonOpenSyleGallery_Click UsingMemorySize Before GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");
                GC.Collect();
                Common.Logger.Debug("buttonOpenSyleGallery_Click UsingMemorySize After  GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                // 表示中のタブ別、
                // 処理対象レイヤのジオメトリタイプに処理の振り分けをする
                switch(m_geometryType) {
                    case esriGeometryType.esriGeometryPoint:
                        if(symbolFormStyle == null) {
                            symbolFormStyle = new FormStyleGallery(rgb, FormStyleGallery.ModeMarkerSymbol);

                            symbolFormStyle.SetItem(
                                esriSymbologyStyleClass.esriStyleClassMarkerSymbols,
                                m_symbol_of_renderer[this.tabControl1.SelectedIndex]);
                        }
                        break;

                    case esriGeometryType.esriGeometryPolyline:
                        if(symbolFormStyle == null) {
                            symbolFormStyle = new FormStyleGallery(rgb, FormStyleGallery.ModeLineSymbol);

                            symbolFormStyle.SetItem(
                                esriSymbologyStyleClass.esriStyleClassLineSymbols,
                                m_symbol_of_renderer[this.tabControl1.SelectedIndex]);
                        }
                        break;

                    case esriGeometryType.esriGeometryPolygon:
                        if(symbolFormStyle == null) {
                            symbolFormStyle = new FormStyleGallery(rgb, FormStyleGallery.ModeFillSymbol);

                            symbolFormStyle.SetItem(
                                esriSymbologyStyleClass.esriStyleClassFillSymbols,
                                m_symbol_of_renderer[this.tabControl1.SelectedIndex]);
                        }
                        break;

                    default:
                        return;
                }

                // ｽﾀｲﾙ･選択画面を表示
                IStyleGalleryItem styleGalleryItem = symbolFormStyle.GetItem();

                // ここでのShowDialog時でOKボタン後にはthis.symbolFormStyleは非表示にしてある
                // IStyleGalleryItem を取得する為
                // this.symbolFormStyleはエレメント設定時の動作に向けになっている為(2010-10-20)。
                //DialogResult diaresult = this.symbolFormStyle.ShowDialog(this);
                //if (diaresult == DialogResult.Cancel)
                //{
                //    return;
                //}

                if(styleGalleryItem == null) {
                    return;
                }

#if doSetColorSentouLayer
                //if (styleGalleryItem.Name != null)
                //{
                //    m_symbolname_of_renderer[this.tabControl1.SelectedIndex] = styleGalleryItem.Name;
                //}
#endif

                bool refresh = false;
				// ﾏｰｶｰ
                if (symbolFormStyle.RunMode == FormStyleGallery.ModeMarkerSymbol &&
                    symbolFormStyle.comboBoxStyleType.SelectedIndex == 0) {
                    m_symbol_of_renderer[this.tabControl1.SelectedIndex] =
                        (ISymbol)symbolFormStyle.simpleMarkers[symbolFormStyle.comboBoxSimpelMarkerStyle.SelectedIndex];

                    refresh = true;
                }
				// ﾌｫﾝﾄ
                else if(symbolFormStyle.RunMode == FormStyleGallery.ModeChangedToCharacterFont &&
                        symbolFormStyle.comboBoxStyleType.SelectedIndex == 1) {
                    m_symbol_of_renderer[this.tabControl1.SelectedIndex] = (ISymbol)symbolFormStyle.SelecteCharecterSymbol;

                    if(m_symbol_of_renderer[this.tabControl1.SelectedIndex] is ICharacterMarkerSymbol) {
                        this.m_characterFont = symbolFormStyle.selectedCharacterFont;
                    }

                    refresh = true;
                }
                else {
                    // Get the IStyleGalleryItem
                    //IStyleGalleryItem styleGalleryItem = this.symbolFormStyle.GetSelectedStyleItem();
                    
                    refresh = true;

                    ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();
                    m_symbol_of_renderer[this.tabControl1.SelectedIndex] = objectCopy.Copy(styleGalleryItem.Item) as ISymbol;
                    //m_symbol_of_renderer[this.tabControl1.SelectedIndex] = (ISymbol)styleGalleryItem.Item;

                    this.selectedSylePpicture = symbolFormStyle.selectedPpicture;

                    switch(m_geometryType) {
					case esriGeometryType.esriGeometryPoint:
						if(m_symbol_of_renderer[this.tabControl1.SelectedIndex] is ICharacterMarkerSymbol) {
							this.m_characterFont = symbolFormStyle.selectedCharacterFont;
						}
						break;
                    }
                }

                IGeoFeatureLayer gflayer = m_featureLayer as IGeoFeatureLayer;
                IFeatureRenderer frenderer = gflayer.Renderer;

                if(tabControl1.SelectedIndex == SIMPLE_RENDERER) {
                    // 単一シンボル
                    if(refresh) {
                        // OK適用時更新
                        m_do_renderer_update[this.tabControl1.SelectedIndex] = false;

                        SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], true);
                    }
                }
                else if(tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER || 
                         tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                    SetTabControls();

                    // スタイル変更時にはパラメータはリセットなので、
                    // アウトライン以外は選択スタイルによって設定される
                    //numericUpDownSimpleSymbolOutLineWidth.Value = Convert.ToDecimal(1.0);

                    // 個別値レンダラ
                    // 数値分類レンダラ
                    bool	useCurrentColor = (m_geometryType == esriGeometryType.esriGeometryPolygon);
					bool	blnIsAllChange;
                    if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
						// 更新方法
						blnIsAllChange = this.listViewUniqueValue.SelectedIndices.Count <= 0
									|| this.listViewUniqueValue.SelectedIndices.Count == this.listViewUniqueValue.Items.Count;

						// 選択ﾌﾗｸﾞを初期化
						this.SaveUniqueValueDelSymbolToList(-1, true, false);

                        // カラー保持内容をクリア(スタイルとカラーランプの場合）-->しないに変更 -->やはりクリア
                        if(blnIsAllChange) {
							// 全更新
							if(m_work_UniqueValueRendererEachColor != null)
								m_work_UniqueValueRendererEachColor.Clear();
                        
							if(m_work_UniqueValueRendererEachInfo != null)
								m_work_UniqueValueRendererEachInfo.Clear();

							// 削除保持内容をクリア-->しないに変更
							//if (m_work_UniqueValueRendererSetOthersSymbol != null)
							//    m_work_UniqueValueRendererSetOthersSymbol.Clear();

							// 選択ﾌﾗｸﾞを設定(All)
							foreach(int intElm in Enumerable.Range(0, this.listViewUniqueValue.Items.Count)) {
								this.SaveUniqueValueDelSymbolToList(intElm, false, false); 
							}
						}
						else {
							// 選択ﾌﾗｸﾞを設定
							foreach(int intElm in this.listViewUniqueValue.SelectedIndices) {
								this.SaveUniqueValueDelSymbolToList(intElm, false, false); 
							}
						}

						// ｼﾝﾎﾞﾙ変更処理
						UpdatePreview(
							this.style_colorRamp, 
							m_symbol_of_renderer[this.tabControl1.SelectedIndex], 
							useCurrentColor, false, SymbolUpdateOption.FROM_STYLE_GALLERY);

						SetListViewSelectedValueCheck();
                    }
                    else if(this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                        if(numericUpDownBunruiSu.Value > 0) {
							// 更新方法
							blnIsAllChange = this.listViewClassBreaks.SelectedIndices.Count <= 0
										|| this.listViewClassBreaks.SelectedIndices.Count == this.listViewClassBreaks.Items.Count;

							if(blnIsAllChange) {
								// カラー保持内容をクリア(スタイルとカラーランプの場合）-->しないに変更 -->やはりクリア
								if(m_work_ClassBreaksRendererEachColor != null)
									m_work_ClassBreaksRendererEachColor.Clear();

								if(m_work_ClassBreaksRendererEachInfo != null)
									m_work_ClassBreaksRendererEachInfo.Clear();
							}

							// ｼﾝﾎﾞﾙ変更処理
                            UpdatePreviewClassBreaks(this.style_colorRamp,
                                m_symbol_of_renderer[this.tabControl1.SelectedIndex], 
                                useCurrentColor, SymbolUpdateOption.FROM_STYLE_GALLERY);

                            SetListViewSelectedValueCheck();
                        }
                    }
                }
            }
            catch(Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally {
                if(symbolFormStyle != null) {
                    ////////////////////////////////
                    //Release the form
                    symbolFormStyle.CleraStyleItem();
                    symbolFormStyle.Close();
                    symbolFormStyle.Dispose();
                    symbolFormStyle = null;
                    this.Activate();
                    ////////////////////////////////
                }

                if(dispMessage) {
                    StringBuilder msginfo = new StringBuilder();
                    msginfo.Append(Properties.Resources.FormStyleGallery_ErrorWhenSetStyle);

                    if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                        if(DoRetryUniqueValue) {
                            RetryCount++;
                            if(RetryCount == 1) {
                                msginfo.Append(" ");
                                msginfo.Append("\n");
                                msginfo.Append(Properties.Resources.FormSymbolSettings_RETRY_MESSAGE);
                            }
                        }
                    }
                    Common.MessageBoxManager.ShowMessageBoxError(msginfo.ToString());
                }

                Common.Logger.Debug("End Of buttonOpenSyleGallery_Click UsingMemorySize Before GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");
                GC.Collect();
                Common.Logger.Debug("End Of buttonOpenSyleGallery_Click UsingMemorySize After  GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");
            }
        }

        /// <summary>
        /// 適用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOKApply_Click(object sender, EventArgs e) {
            bool dispMessage = false;
            bool messageWarning = false;
            bool doNext = false;
            StringBuilder messageStr = new StringBuilder();

            try {
                // Enter Key押下で実行される場合にはnumericUpDownSimpleSymbolSizeOrWidthサイズ変更イベント実行前に処理される。
                // この処理に遷移されて来た時点で不正な内容、例えばマイナスなどを入力されても最小値に訂正される。
                // Enter Key押下で実行される場合で問題になるのはnumericUpDownSimpleSymbolSizeOrWidthのだけ。
                SetUpdateProperties();

                // スタイル、カラーランプ、またはプレビュー更新の何れかのボタンを
                // 押すことで、選択されているタブに対応するレンダラのシンボルが更新される。
                if(!m_do_renderer_update[this.tabControl1.SelectedIndex] || !m_do_save[this.tabControl1.SelectedIndex]) {
                    // 初期表示以外でプレビュー更新していない場合
                    //doNext = (this.tabControl1.SelectedIndex == SIMPLE_RENDERER);

                    // 初期表示以外でプレビュー更新していない場合にはガイダンス表示
                    if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                        if(this.listViewUniqueValue.Items.Count == 0) {
                            messageStr.Append(
                                "個別値分類のシンボル更新処理が実行されていない様です。\n");

                            messageStr.Append(
                                "スタイル、カラーランプ、またはプレビュー更新の何れかのボタンをクリックして、\n");

                            messageStr.Append(
                                "個別値分類のシンボル更新処理を実行して下さい。");

                            dispMessage = true;
                            messageWarning = true;
                            return;
                        }
                        // 2011/04/13 個別値色変更 -->
                        doNext = m_do_renderer_update[this.tabControl1.SelectedIndex];
                        //<--
                    }
                    else if (this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                        if(this.listViewClassBreaks.Items.Count == 0) {
                            messageStr.Append(
                                "数値分類のシンボル更新処理が実行されていない様です。\n");

                            messageStr.Append(
                                "スタイル、カラーランプ、またはプレビュー更新の何れかのボタンをクリックして、\n");

                            messageStr.Append(
                                "数値分類のシンボル更新処理を実行して下さい。");

                            dispMessage = true;
                            messageWarning = true;
                            return;
                        }
                        // 2011/04/13 個別値色変更 -->
                        doNext = m_do_renderer_update[this.tabControl1.SelectedIndex];
                        //<--
                    }
                    else {
                        // 単一の場合はOK
                        doNext = true;
                        m_do_save[this.tabControl1.SelectedIndex] = false;
                    }
                }
                else {
                    doNext = m_do_renderer_update[this.tabControl1.SelectedIndex];

                    // 個別色設定可能状態でプレビュー未更新時の判定
                    if (doNext)
                        doNext = !(this.buttonSetColor.Enabled);
                }

                if(!doNext) {
                    // プレビュー更新されていない場合
                    if((numericUpDownSimpleSymbolSizeOrWidth.Enabled) &&
                        (m_geometryType == esriGeometryType.esriGeometryPoint &&
                         numericUpDownSimpleSymbolSizeOrWidth.Value == 0))
                    {
                        Common.MessageBoxManager.ShowMessageBoxError(Properties.Resources.FormSymbolSettings_Error_PointSizeZero);

                        return;
                    }

                    // 2011/05/13 
                    if (this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER ||
                        this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                       // DoButtonUpdatePreview();
                    }
                }

                // 保存済み確認
                if(m_do_save[this.tabControl1.SelectedIndex]) {
                    Common.Logger.Debug("Is Not Changed");
                    if(sender.ToString().Contains("OK")) {
                        // Enter Key押下で実行される場合には,この処理が抜けてから、
                        // numericUpDownSimpleSymbolSizeOrWidthサイズ変更イベントが処理される。
                        // その為、そのイベント処理をパスするフラグをOnする。
                        Pass_Preview_Process = true;

                        this.Close();
                        return;
                    }
                    else {
                        return;
                    }
                }

                Common.Logger.Debug("buttonOKApply_Click Main Proc STARTED");

                IRgbColor rgb = Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
                IRgbColor rgbOutline = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
                IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)m_featureLayer;

                ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

                if(tabControl1.SelectedIndex == SIMPLE_RENDERER) {
                    // 単一シンボル
                    ISimpleRenderer simpleRenderer = new SimpleRendererClass();
                    simpleRenderer.Symbol = m_symbol_of_renderer[this.tabControl1.SelectedIndex];
                    geoFeatureLayer.Renderer = (IFeatureRenderer)simpleRenderer;
                }
                else if(tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                    // 個別値レンダラ
                    //geoFeatureLayer.Renderer = (IFeatureRenderer)m_work_UniqueValueRenderer; 
                    // ObjectCopyしないと、OK、適用しなくてもTOC、Mapリフレッシュ時、つまり画面を閉じる際に
                    // 画面上の未確定内容が反映される。
                    //geoFeatureLayer.Renderer =
                    //        objectCopy.Copy(m_work_UniqueValueRenderer) as IFeatureRenderer;
                    geoFeatureLayer.Renderer = 
                        GetFixedUniqueValueRenderer(m_work_UniqueValueRenderer) as IFeatureRenderer;
                }
                else if(tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                    // 数値分類レンダラ
                    //geoFeatureLayer.Renderer = (IFeatureRenderer)m_work_ClassBreaksRenderer;
                    geoFeatureLayer.Renderer =
                            objectCopy.Copy(m_work_ClassBreaksRenderer) as IFeatureRenderer;

                    IClassBreaksUIProperties pUIProperties = m_work_ClassBreaksRenderer as IClassBreaksUIProperties;

                    if(pUIProperties != null && pUIProperties.Method != null) {
                        Common.Logger.Debug("Save Method.IUID.Value... =" + pUIProperties.Method.Value.ToString());
                    }
                }

                Common.Logger.Debug("axMapControl1, TOC update STARTED");

                // Fire contents changed event that the TOCControl listens to
                mainFrm.axMapControl1.ActiveView.ContentsChanged();

                //mainFrm.axMapControl1.ActiveView.Deactivate

                // Refresh the display
                mainFrm.axPageLayoutControl1.ActiveView.PartialRefresh(
                    esriViewDrawPhase.esriViewGeography, null, null);
                
                Common.Logger.Debug("axPageLayoutControl1.Refresh ENDED");

                Common.Logger.Debug("UsingMemorySize Before GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                GC.Collect();

                Common.Logger.Debug("UsingMemorySize After GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                Common.Logger.Debug("OK or Apply ENDED");

                //m_do_save[this.tabControl1.SelectedIndex] = true;
                for(int i = SIMPLE_RENDERER; i < CLASSBREAK_RENDERER + 1; i++) {
                    if(this.tabControl1.SelectedIndex == i) {
                        m_do_save[this.tabControl1.SelectedIndex] = true;
                        m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
                    }
                    else {
                        m_do_save[i] = false;
                        m_do_renderer_update[i] = false;
                    }
                }

                if(sender.ToString().Contains("OK")) {
                    this.Close();
                }
                else {
                    // 個別色変更対応の影響
                    if(tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER ||
                        tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                        if(buttonSetColor.Enabled) {
                            labelSetColor.Enabled = false;
                            buttonSetColor.Enabled = false;
                        }
                    }
                }
            }
            catch(Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally {
                if(dispMessage) {
                    if(messageStr.Length == 0) {
                        messageStr.Append(
                            Properties.Resources.FormSymbolSettings_ErrorWhen_buttonOKApply_Click);
                    }
                    if(messageWarning) {
                        Common.MessageBoxManager.ShowMessageBoxWarining(
                            messageStr.ToString());
                    }
                    else {
                        Common.MessageBoxManager.ShowMessageBoxError(
                            messageStr.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// シンプルシンボルアウトラインサイズ変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownSimpleSymbolOutLineWidth_ValueChanged(object sender, EventArgs e) {
            if(InitLoad) {
                m_do_renderer_update[this.tabControl1.SelectedIndex] = InitLoad; // 変更無し
                m_do_save[this.tabControl1.SelectedIndex] = InitLoad;
                return;
            }

            if(TabChanged) return;

            if(Pass_Preview_Process) {
                Common.Logger.Debug("numericUpDownSimpleSymbolOutLineWidth_ValueChanged On Enter Key, Exit");
                return;
            }
            if(Pass_Preview_Process_Escape) {
                Common.Logger.Debug("numericUpDownSimpleSymbolOutLineWidth_ValueChanged On Escape Key, Exit");
                return;
            }

			(sender as Control).Enabled = false;
            this.Cursor = Cursors.WaitCursor;
			
			bool dispMessage = false;
            try {
                if(SetUpdateProperties()) {
					if(this.tabControl1.SelectedIndex == SIMPLE_RENDERER) {
						// 単一ｼﾝﾎﾞﾙのﾌﾟﾚﾋﾞｭｰを更新
						UpdatePreviewImage();
						// OK適用時更新
						m_do_renderer_update[this.tabControl1.SelectedIndex] = false;
					}
					else {
						// ﾌﾟﾚﾋﾞｭｰを更新
						DoButtonUpdatePreview(true);
					}
                }
            }
            catch(Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally {
                if(dispMessage) {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.
                        FormSymbolSettings_ErrorWhen_numericUpDownSimpleSymbolOutLineWidth_ValueChanged);
                }
            }

            this.Cursor = Cursors.Default;
			(sender as Control).Enabled = true;
        }

        /// <summary>
        /// フォームを閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSymbolSettings_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(m_symbol_of_renderer);
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(m_symbol);
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(m_symbol_uniquevalue);
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(m_symbol_classbreak);

                if(this.graphics != null)
                    this.graphics.Dispose();

                Common.Logger.Debug("シンボル設定画面終了");

            }
            catch(Exception ex) {
                Common.UtilityClass.DoOnError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownSimpleSymbolSizeOrWidth_ValueChanged(object sender, EventArgs e) {
            if(InitLoad) {
                m_do_renderer_update[this.tabControl1.SelectedIndex] = InitLoad; // 変更無し
                m_do_save[this.tabControl1.SelectedIndex] = InitLoad;
                return;
            }

            if(TabChanged) return;

            // Enterキー, Escapeキー時の場合にはOKボタン時処理実行済みの為、下記実行しない
            if(Pass_Preview_Process) {
                Common.Logger.Debug("numericUpDowonSymbolSizeOrWidth On Enter Key, Exit");
                return;
            }
            if(Pass_Preview_Process_Escape) {
                Common.Logger.Debug("numericUpDowonSymbolSizeOrWidth On Escape Key, Exit");
                return;
            }

			(sender as Control).Enabled = false;
            this.Cursor = Cursors.WaitCursor;

			bool dispMessage = false;
			try {
				if(numericUpDownSimpleSymbolSizeOrWidth.Value <= 0) {
					if(labelSimpleSymbolSizeOrWidth.Text.Contains("サイズ")) {
						Common.MessageBoxManager.ShowMessageBoxError(
							Properties.Resources.FormSymbolSettings_Error_PointSizeZero);
					}
					return;
				}

				if(SetUpdateProperties()) {
					if(this.tabControl1.SelectedIndex == SIMPLE_RENDERER) {
						// 単一ｼﾝﾎﾞﾙのﾌﾟﾚﾋﾞｭｰを更新
						UpdatePreviewImage();
						// OK適用時更新
						m_do_renderer_update[this.tabControl1.SelectedIndex] = false;
					}
					else {
						// ﾌﾟﾚﾋﾞｭｰを更新
						DoButtonUpdatePreview(true);
					}
				}
			}
			catch(Exception ex) {
				dispMessage = true;
				Common.UtilityClass.DoOnError(ex);
			}
			finally {
				if(dispMessage) {
					Common.MessageBoxManager.ShowMessageBoxError(
						Properties.Resources.
						FormSymbolSettings_ErrorWhen_numericUpDownSimpleSymbolSizeOrWidth_ValueChanged);
				}
			}

            this.Cursor = Cursors.Default;
			(sender as Control).Enabled = true;
        }

        /// <summary>
        /// 画像(単一シンボル時)またはカラーランプ(個別値 or 数値分類時)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenImageFile_Click(object sender, EventArgs e) {
            bool dispMessage = false;

            FormStyleGallery symbolFormColorRamp = null;
            try {
                Common.Logger.Debug("buttonOpenImageFile_Click UsingMemorySize Before GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");
                GC.Collect();
                Common.Logger.Debug("buttonOpenImageFile_Click UsingMemorySize After  GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                if(this.buttonOpenImageFileOrColorRamp.Text.Contains("画像")) {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.FileName = "";
                    ofd.InitialDirectory = "";

                    ofd.Filter =
                        //"PictureFiles (*.bmp,*.emf)|*.bmp;*.emf|" +
                        //"Bitmaps (*.bmp)|*.bmp|" +
                        //"Enhanced Metafiles (*.emf)|*.emf";
                        "サポートするすべての画像フォーマット (*.bmp,*.emf)|*.bmp;*.emf|" +
                        "BMP (*.bmp)|*.bmp|" +
                        "EMF (*.emf)|*.emf";

                    ofd.FilterIndex = 0;
                    ofd.Title = "開く";
                    ofd.RestoreDirectory = true;
                    ofd.CheckFileExists = true;
                    ofd.CheckPathExists = true;
                    if(ofd.ShowDialog(this) == DialogResult.OK) {
                        string path = System.IO.Path.GetFileName(ofd.FileName);
                        if(path != null) {
                            IPictureMarkerSymbol picsym = null;
                            if(path.ToLower().LastIndexOf(".bmp") > 0) {
                                picsym = CreatePictureMarkerSymbol(
                                    esriIPictureType.esriIPictureBitmap, ofd.FileName, 18.0);
                            }
                            else {
                                picsym = CreatePictureMarkerSymbol(
                                    esriIPictureType.esriIPictureEMF, ofd.FileName, 18.0);
                            }

                            m_symbol_of_renderer[this.tabControl1.SelectedIndex] = (picsym as ISymbol);

                            SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], true);

							// OK適用時更新
							m_do_renderer_update[this.tabControl1.SelectedIndex] = false;
                        }
                    }
                }
                else {
                    // カラーランプ 
                    //FormStyleGallery 
                    this.style_colorRamp = null;

                    if(symbolFormColorRamp == null) {
                        symbolFormColorRamp = new FormStyleGallery(null, FormStyleGallery.ModeColorRamp);

                        symbolFormColorRamp.SetItem(esriSymbologyStyleClass.esriStyleClassColorRamps,
                            m_symbol_of_renderer[this.tabControl1.SelectedIndex]);
                    }

                    IStyleGalleryItem styleGalleryItem = symbolFormColorRamp.GetItem();
                    if(styleGalleryItem == null) {
                        return;
                    }

                    //IColorRamp colorRamp 
                    this.style_colorRamp = styleGalleryItem.Item as IColorRamp;
                    colorSchemeName = styleGalleryItem.Name;

                    if(this.style_colorRamp != null) {
                        if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                            // カラー保持内容をクリア(スタイルとカラーランプの場合）
                            //if (m_work_UniqueValueRendererEachColor != null)
                            //    m_work_UniqueValueRendererEachColor.Clear();

                            // 削除内容はクリアしない様に変更
                            //if (m_work_UniqueValueRendererSetOthersSymbol != null) 
                            //    m_work_UniqueValueRendererSetOthersSymbol.Clear();

                            UpdatePreview(this.style_colorRamp,
                                m_symbol_of_renderer[this.tabControl1.SelectedIndex], 
                                false, false, SymbolUpdateOption.FROM_COLOR_RAMP);

                            SetListViewSelectedValueCheck();
                        }
                        else if(this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                            if(numericUpDownBunruiSu.Value > 0) {
                                // カラー保持内容をクリア(スタイルとカラーランプの場合）
                                if(m_work_ClassBreaksRendererEachColor != null)
                                    m_work_ClassBreaksRendererEachColor.Clear();

                                UpdatePreviewClassBreaks(this.style_colorRamp,
                                    m_symbol_of_renderer[this.tabControl1.SelectedIndex], false, SymbolUpdateOption.FROM_COLOR_RAMP);

                                SetListViewSelectedValueCheck();
                            }
                        }
                    }
                }
            }
            catch(Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally {
                if(this.buttonOpenImageFileOrColorRamp.Text.Contains("カラーランプ")) {
                    if(symbolFormColorRamp != null) {
                        Common.Logger.Debug("OpenColorRamp: IStyleGalleryItem Release After This ... ");
                        ////////////////////////////////
                        //Release the form
                        symbolFormColorRamp.CleraStyleItem();
                        symbolFormColorRamp.Dispose();
                        symbolFormColorRamp = null;
                        this.Activate();
                        ////////////////////////////////
                        Common.Logger.Debug("OpenColorRamp: IStyleGalleryItem Release Ended");
                    }
                }

                if(dispMessage) {
                    if(this.buttonOpenImageFileOrColorRamp.Text.Contains("画像")) {
                        Common.MessageBoxManager.ShowMessageBoxError(
                            Properties.Resources.FormSymbolSettings_ErrorWhen_buttonOpenImageFile_Click_Pict);
                    }
                    else {
                        StringBuilder msginfo = new StringBuilder();
                        msginfo.Append(
                            Properties.Resources.FormSymbolSettings_ErrorWhen_buttonOpenImageFile_Click_ColorRamp);

                        if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                            if(DoRetryUniqueValue) {
                                RetryCount++;
                                if(RetryCount == 1) {
                                    msginfo.Append(" ");
                                    msginfo.Append("\n");
                                    msginfo.Append(Properties.Resources.FormSymbolSettings_RETRY_MESSAGE);
                                }
                            }
                        }
                        Common.MessageBoxManager.ShowMessageBoxError(msginfo.ToString());
                    }
                }

                Common.Logger.Debug("End OF buttonOpenImageFile_Click UsingMemorySize Before GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");
                GC.Collect();
                Common.Logger.Debug("End Of buttonOpenImageFile_Click UsingMemorySize After  GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");
            }
        }

        /// <summary>
        /// 各コントロールの状態を設定を有効に設定します
        /// </summary>
        private void SetControlsEnableTrue()
        {
            //System.Threading.Thread.Sleep(1000);

            // 個別値分類ｺﾝﾄﾛｰﾙの設定
            listViewUniqueValue.Enabled = true;
            comboBoxFieldName.Enabled = true;

            // 数値分類ｺﾝﾄﾛｰﾙの設定
            listViewClassBreaks.Enabled = true;
            comboBoxFieldNameClassBreaks.Enabled = true;
            
            // 共通ｺﾝﾄﾛｰﾙの設定
            panelParameter.Enabled = true;
            buttonApply.Enabled = true;
            buttonOK.Enabled = true;
        }

		/// <summary>
		/// 各コントロールの状態を無効に設定します
		/// </summary>
        private void SetControlsEnableFalse()
        {
            //System.Threading.Thread.Sleep(1000);

            // 個別値分類ｺﾝﾄﾛｰﾙの設定
            listViewUniqueValue.Enabled = false;
            comboBoxFieldName.Enabled = false;

            // 数値分類ｺﾝﾄﾛｰﾙの設定
            listViewClassBreaks.Enabled = false;
            comboBoxFieldNameClassBreaks.Enabled = false;

            // 共通ｺﾝﾄﾛｰﾙの設定
            panelParameter.Enabled = false;
            buttonApply.Enabled = false;
            buttonOK.Enabled = false;
        }

        /// <summary>
        /// 指定カラー名称のカラーランプを取得
        /// </summary>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        private IColorRamp GetColorRampFromStyleGallery(string symbolName) {
			IColorRamp				agClrRamp = null;
            ISymbologyControl		agSymCtl = new SymbologyControlClass();
			ISymbologyStyleClass	agSymSCls;
			IStyleGalleryItem		agSGItem;

			IColorRamp				agClrRampTemp = null;	// 対象がなかった場合の保険(最初に検出したｶﾗｰﾗﾝﾌﾟで代替え)

			FormStyleGallery	frmSG = new FormStyleGallery(new RgbColorClass(), FormStyleGallery.ModeColorRamp);
			string	strSFile;
			int		intItems = 0;
			foreach(int intCnt in Enumerable.Range(0, frmSG.StyleFileNum)) {
				strSFile = frmSG.GetStyleFilePath(intCnt);
				if(!string.IsNullOrWhiteSpace(strSFile)) {
					try {
						// ｽﾀｲﾙ･ﾌｧｲﾙの読み込み
						agSymCtl.LoadStyleFile(strSFile);

						// ｶﾗｰﾗﾝﾌﾟを取得
						agSymSCls = agSymCtl.GetStyleClass(esriSymbologyStyleClass.esriStyleClassColorRamps);
						intItems = agSymSCls.get_ItemCount(esriSymbologyStyleClass.esriStyleClassColorRamps);
						
						// 対象ｱｲﾃﾑを探索
						foreach(int intItem in Enumerable.Range(0, intItems)) {
							agSGItem = agSymSCls.GetItem(intItem);
							if(agSGItem.Name.Equals(symbolName)) {
								agClrRamp = agSGItem.Item as IColorRamp;
								break;
							}
							else if(agClrRampTemp == null) {
								agClrRampTemp = agSGItem.Item as IColorRamp;
							}
						}
						agSymCtl.RemoveFile(strSFile);
						if(agClrRamp != null) {
							break;
						}
					}
					catch(Exception ex) {
						if(agSymCtl == null) {
							Common.UtilityClass.DoOnError("FormSymbolSettings カラーランプの読み込みに失敗しました [" + strSFile + "]", ex);
						}
						else {
							Common.UtilityClass.DoOnError(ex);
						}
					}
				}
			}

			if(agSymCtl != null) {
				agSymCtl.Clear();
				agSymCtl = null;
			}
			frmSG.Dispose();
			frmSG = null;

			// 代用ｶﾗｰﾗﾝﾌﾟを返却
			if(agClrRamp == null) {
				agClrRamp = agClrRampTemp;
			}

			return agClrRamp;
        }

        /// <summary>
        /// タブ選択変更時イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InitLoad) return;

            bool dispMessage = false;
            try {
				// 単一ｼﾝﾎﾞﾙ
                if (this.tabControl1.SelectedIndex == SIMPLE_RENDERER) {
                    // 単一シンボルの場合には切替時表示内容更新する
                    SetCommonControlsEnable(this.m_symbol_of_renderer[SIMPLE_RENDERER], true);
                }
                else {
					// 個別分類
                    if (this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                        // 切替時は特に表示内容の更新は不要
						// 但し最大分割数に問題がある場合は警告を表示する
                        if (ShowUniquValueLimitOverWarning) {
                            if(!UniquValueLimitOverWarningMessageShowDone) {
                                Common.MessageBoxManager.ShowMessageBoxWarining(
                                    this, 
                                    Properties.Resources.FormSymbolSettings_WARNING_UniqueValueCountLimitOver +
                                    "上限の" + UNIQUEVALUE_MAX_LIMIT + "件までの処理を行います。");

                                UniquValueLimitOverWarningMessageShowDone = true;
                            }
                        }

                        if(!UniqueValueInit) {
                            SetControlsEnableFalse();

                            // ﾌﾟﾚﾋﾞｭｰ･ﾘｽﾄの初期化
                            SetDetailPreviewInit();

                            // ﾌｨｰﾙﾄﾞ選択ｺﾝﾎﾞの設定
                            SetFieldNamesToComboboxes();
                            UniqueValueInit = true;			// 初期化完了

                            SetControlsEnableTrue();
                        }

                        // ﾕｰｻﾞｰによるｼﾝﾎﾞﾙの選択は解除
                        listViewUniqueValue.SelectedIndices.Clear();
                        listViewUniqueValue.SelectedItems.Clear();
                        SetListViewSelectedValueCheck();
                    }
					// 数値分類
                    else if (this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                        // 切替時は特に表示内容の更新は不要
                        if(!ClassBreaksInit) {
                            // ｺﾝﾄﾛｰﾙを無効化
                            SetControlsEnableFalse();

                            // ﾌﾟﾚﾋﾞｭｰ･ﾘｽﾄの初期化
                            SetDetailPreviewInitClassBreaks();
                            // 分類手法ｺﾝﾎﾞの設定
                            SetClassificationMethodTypeToCombobox();

                            // ﾌｨｰﾙﾄﾞ選択ｺﾝﾎﾞの設定
                            SetFieldNamesToComboboxes();
                            ClassBreaksInit = true;			// 初期化完了

                            // ｺﾝﾄﾛｰﾙを有効化
                            SetControlsEnableTrue();

                            // 塗り色設定ﾎﾞﾀﾝの制御
                            SetColorButtonEnable = CanUseColorButtons();
                            buttonSetColor.Enabled = SetColorButtonEnable;
                            labelSetColor.Enabled = SetColorButtonEnable;

                            // ﾘｽﾄ選択ﾎﾞﾀﾝの制御
                            buttonSelectAll_ClassBreaks.Enabled = SetColorButtonEnable;
                            buttonCancelSelect_ClassBreaks.Enabled = SetColorButtonEnable;

                            // 分類数の設定
                            numericUpDownBunruiSu.Value = Convert.ToDecimal(DEFALUT_BREAK_COUNT);
                        }

                        // ﾕｰｻﾞｰによるｼﾝﾎﾞﾙの選択を解除
                        listViewClassBreaks.SelectedIndices.Clear();
                        listViewClassBreaks.SelectedItems.Clear();
                        SetListViewSelectedValueCheck();
                    }
                }

                TabChanged = true;
                SetTabControls();
                TabChanged = false;
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
            finally {
                if(dispMessage) {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.FormSymbolSettings_ErrorWhen_tabControl1_SelectedIndexChanged);
                }
            }
        }

		private void DoButtonUpdatePreviewNew() {
			if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
				// 再描画
				this.UpdateUVListViewItems(this.m_work_UniqueValueRenderer, false, true);
			}
			else if(this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
				// 再描画
				this.UpdateCBListViewItems(this.m_work_ClassBreaksRenderer);
			}
		}

        /// <summary>
        /// プレビュー更新
        /// </summary>
        private void DoButtonUpdatePreview(bool UseCurrentColor = false) {
			// ﾌﾟﾚﾋﾞｭｰ･ﾎﾞﾀﾝの状態を確認
            if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                // 個別値
                if(!this.buttonUpdatePreviewUniqueValue.Enabled) return;
            }
            else {
                // 数値分類
                if(!this.buttonUpdatePreview.Enabled) return;
            }

            // ﾎﾟｲﾝﾄのｼﾝﾎﾞﾙ･ｻｲｽﾞをﾁｪｯｸ
			if((numericUpDownSimpleSymbolSizeOrWidth.Enabled) &&
                (m_geometryType == esriGeometryType.esriGeometryPoint &&
                    numericUpDownSimpleSymbolSizeOrWidth.Value == 0))
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    Properties.Resources.FormSymbolSettings_Error_PointSizeZero);
                return;
            }

            bool dispMessage = false;
            try {
                this.buttonUpdatePreview.Enabled = false;

                // 個別値分類
                if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                    Common.Logger.Debug("個別値表示更新処理開始");

                    // 2011/05/02 個別色変更対応 -->
                    //if (listViewUniqueValue.SelectedIndices.Count > 0)
                    if(m_work_UniqueValueRendererEachColor != null && m_work_UniqueValueRendererEachColor.Count > 0 &&
                        (this.listViewUniqueValue.SelectedIndices.Count > 0 && this.listViewUniqueValue.SelectedIndices.Count < this.listViewUniqueValue.Items.Count))
                    {
                        // リストビューで1つでも要素が選択されていればそれらに対してだけ処理
                        IColor	icolor = Common.UtilityClass.ConvertToESRIColor(this.buttonSetColor.BackColor);
                        listViewUniqueValueContextMenu_OnClick(icolor, UseCurrentColor);
                    }
                    else {
						//** 分類を再作成 ** ※ﾌｨｰﾙﾄﾞ変更時は、m_work_UniqueValueRendererEachColorをｸﾘｱしておく

                        // 個別色変更対応の影響でここで実行
                        SetUpdateProperties();

                        // 全ての要素に対して処理
                        UpdatePreview(this.style_colorRamp,
                            m_symbol_of_renderer[this.tabControl1.SelectedIndex],
                            UseCurrentColor, false, 
								UseCurrentColor ? SymbolUpdateOption.FROM_UPDATE_PREVIEW_DEFAULT : SymbolUpdateOption.FROM_COLOR_RAMP);
                    }

                    SetListViewSelectedValueCheck();

                    Common.Logger.Debug("個別値表示更新処理終了");
                }
                // 数値分類
                else if(this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                    if(numericUpDownBunruiSu.Value > 0) {
                        // 2011/05/02 個別色変更対応 -->
                        if(m_work_ClassBreaksRendererEachColor != null && m_work_ClassBreaksRendererEachColor.Count > 0
							//&& buttonSetColor.Enabled)
							&& (this.listViewClassBreaks.SelectedIndices.Count > 0 && this.listViewClassBreaks.SelectedIndices.Count < this.listViewClassBreaks.Items.Count))
                        {
                            // リストビューで1つでも要素が選択されていればそれらに対してだけ処理
                            IColor icolor = Common.UtilityClass.ConvertToESRIColor(this.buttonSetColor.BackColor);
                            listViewClassBreaksContextMenu_OnClick(icolor, UseCurrentColor);
                        }
                        else {
							// 全体更新表示
                            UpdatePreviewClassBreaks(this.style_colorRamp,
                                m_symbol_of_renderer[this.tabControl1.SelectedIndex], 
								UseCurrentColor, 
								UseCurrentColor ? SymbolUpdateOption.FROM_UPDATE_PREVIEW_DEFAULT : SymbolUpdateOption.FROM_COLOR_RAMP);
                        }

                        // ｺﾝﾄﾛｰﾙ制御
                        SetListViewSelectedValueCheck();
                    }
                }

                //SetCommonControlsEnable(m_symbol_of_renderer[this.tabControl1.SelectedIndex], false);
                //SetColorButtonEnable = false;
                //this.labelSetColor.Enabled = SetColorButtonEnable;
                //this.buttonSetColor.Enabled = SetColorButtonEnable;
            }
            catch(Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally {
                if(dispMessage) {
                    StringBuilder msginfo = new StringBuilder();
                    msginfo.Append(
                        Properties.Resources.FormSymbolSettings_ErrorWhen_ButtonUpdatePreview_Click);

                    Common.MessageBoxManager.ShowMessageBoxError(this,
                        msginfo.ToString());
                }

                //System.Threading.Thread.Sleep(1000);
                this.buttonUpdatePreview.Enabled = true;
            }
        }

        /// <summary>
        /// プレビュー更新ボタンクリック時イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUpdatePreview_Click(object sender, EventArgs e)
        {
			// ﾌﾟﾚﾋﾞｭｰ･ﾎﾞﾀﾝの状態を確認
            if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                // 個別値
                if(!this.buttonUpdatePreviewUniqueValue.Enabled) return;
            }
            else {
                // 数値分類
                if(!this.buttonUpdatePreview.Enabled) return;
            }

            // 待ち表示
            this.Cursor = Cursors.WaitCursor;
            this.buttonUpdatePreview.Enabled = false;

			// ﾘｽﾄ未作成の場合、
            if(this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
				if(this.m_work_ClassBreaksRenderer == null 
					|| this.m_selected_field_Name_CB != (this.comboBoxFieldNameClassBreaks.SelectedItem as Common.FieldComboItem).FieldName
					|| this.listViewClassBreaks.Items.Count <= 0) {
					this.DoButtonUpdatePreview();
				}
				else {
					this.DoButtonUpdatePreviewNew();
				}
			}
			else if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
				if(this.m_work_UniqueValueRenderer == null
					|| this.m_selected_field_Name_UV != (this.comboBoxFieldName.SelectedItem as Common.FieldComboItem).FieldName
					|| this.listViewUniqueValue.Items.Count <= 0) {
					this.DoButtonUpdatePreview();
				}
				else {
					this.DoButtonUpdatePreviewNew();
				}
			}

            // 待ち解除
            this.buttonUpdatePreview.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 数値分類時：フィールド名選択変更時イベントハンドラ
        /// 数値フィールドの場合のみ対応
        /// 選択フィールドの最大値、最少値を取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFieldNameClassBreaks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!buttonUpdatePreview.Enabled) return;
            if (!comboBoxFieldNameClassBreaks.Enabled) return;

            if (InitLoad)
            {
                m_do_renderer_update[this.tabControl1.SelectedIndex] = InitLoad; // 変更無し
                m_do_save[this.tabControl1.SelectedIndex] = InitLoad;
                return;
            }

            if (TabChanged) return;

            if (comboBoxFieldNameClassBreaks.Items.Count == 1 || comboBoxFieldNameClassBreaks.Items.Count == 0)
            {
                return;
            }

            if (m_selected_field_Name_CB.Equals(
                ((Common.FieldComboItem)comboBoxFieldNameClassBreaks.SelectedItem).FieldName))
            {
                return;
            }

            // 待ち表示
			(sender as Control).Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            
            // カラー保持内容をクリア
            if (m_work_ClassBreaksRendererEachColor != null)
                m_work_ClassBreaksRendererEachColor.Clear();

            if (m_work_ClassBreaksRendererEachInfo  != null)
                m_work_ClassBreaksRendererEachInfo.Clear();

            // OK適用時更新
            m_do_renderer_update[this.tabControl1.SelectedIndex] = false;

            // 選択ﾌｨｰﾙﾄﾞ名を取得
            m_selected_field_Name_CB = ((Common.FieldComboItem)comboBoxFieldNameClassBreaks.SelectedItem).FieldName;

            // 初期状態以降の場合でフィールド変更された場合にはプレビュー更新する
            DoButtonUpdatePreview();

            // 待ち解除
            this.Cursor = Cursors.Default;
			(sender as Control).Enabled = true;
        }
        #endregion event handler

        /// <summary>
        /// 数値分類時：分類種別選択変更時イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxBunruiSyuhou_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InitLoad)
            {
                m_do_renderer_update[this.tabControl1.SelectedIndex] = true;// InitLoad; // 変更無し
                m_do_save[this.tabControl1.SelectedIndex] = true;
                return;
            }

            if (TabChanged) return;

			(sender as Control).Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            // カラー保持内容をクリア
            if (m_work_ClassBreaksRendererEachColor != null)
                m_work_ClassBreaksRendererEachColor.Clear();

            if (m_work_ClassBreaksRendererEachInfo  != null)
                m_work_ClassBreaksRendererEachInfo.Clear();

            this.DoButtonUpdatePreview();

            // OK適用時更新
            m_do_renderer_update[this.tabControl1.SelectedIndex] = false;

            this.Cursor = Cursors.Default;
			(sender as Control).Enabled = true;
        }

        /// <summary>
        /// 数値分類時：分類数変更時イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownBunruiSu_ValueChanged(object sender, EventArgs e)
        {
            if (InitLoad)
            {
                m_do_renderer_update[this.tabControl1.SelectedIndex] = InitLoad; // 変更無し
                m_do_save[this.tabControl1.SelectedIndex] = InitLoad;
                return;
            }

            if (TabChanged) return;

			(sender as Control).Enabled = false;
            this.Cursor = Cursors.WaitCursor;

			// 分類数変更時は保持している個別色をクリア
			if (m_work_ClassBreaksRendererEachColor != null)
				m_work_ClassBreaksRendererEachColor.Clear();

			if (m_work_ClassBreaksRendererEachInfo != null)
				m_work_ClassBreaksRendererEachInfo.Clear();

			this.DoButtonUpdatePreview();
			
			// OK適用時更新
			m_do_renderer_update[this.tabControl1.SelectedIndex] = false;

            this.Cursor = Cursors.Default;
			(sender as Control).Enabled = true;
        }

        /// <summary>
        /// 個別値分類時：フィールド名選択変更時イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFieldName_SelectedIndexChanged(object sender, EventArgs e) {
            if(!buttonUpdatePreviewUniqueValue.Enabled) return;
            if(!comboBoxFieldName.Enabled) return;

            if(InitLoad) {
                m_do_renderer_update[this.tabControl1.SelectedIndex] = InitLoad; // 変更無し
                m_do_save[this.tabControl1.SelectedIndex] = InitLoad;
                return;
            }

            if(TabChanged) return;

            //if (comboBoxFieldName.Items.Count == 1 || listViewUniqueValue.Items.Count == 0)
            //{
            //    return;
            //}

            if(m_selected_field_Name_UV.Equals(((Common.FieldComboItem)comboBoxFieldName.SelectedItem).FieldName)) {
                return;
            }

            // 待ち表示
            (sender as Control).Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            
            // カラー保持内容をクリア(スタイルとカラーランプ,選択フィールド変更の場合）
            if(m_work_UniqueValueRendererEachColor != null)
                m_work_UniqueValueRendererEachColor.Clear();

            if(m_work_UniqueValueRendererEachInfo != null)
                m_work_UniqueValueRendererEachInfo.Clear();
            
            if(m_work_UniqueValueRendererSetOthersSymbol != null) 
                m_work_UniqueValueRendererSetOthersSymbol.Clear();

            if(m_work_UniqueValueRenderer != null) {
                m_work_UniqueValueRenderer.RemoveAllValues();
                m_work_UniqueValueRenderer = null;
            }
			
			m_work_UniqueValueRendererEachColor = null;
			m_work_UniqueValueRendererEachInfo = null;
			m_work_UniqueValueRendererSetOthersSymbol = null;

            // OK適用時更新
            m_do_renderer_update[this.tabControl1.SelectedIndex] = false;

            m_selected_field_Name_UV = ((Common.FieldComboItem)comboBoxFieldName.SelectedItem).FieldName;

            // 初期状態以降の場合でフィールド変更された場合にはプレビュー更新する
            DoButtonUpdatePreview();
			//UpdatePreview(this.style_colorRamp,
			//	m_symbol_of_renderer[this.tabControl1.SelectedIndex], 
			//	true, false, SymbolUpdateOption.FROM_UPDATE_PREVIEW_DEFAULT);

			//SetListViewSelectedValueCheck();

            // 待ち解除
            this.Cursor = Cursors.Default;
			(sender as Control).Enabled = true;
        }

        /// <summary>
        /// 2011/04/13 個別値色変更
        /// 個別値リストビュー
        /// アイテムコンテキストメニュー色設定クリック時
        /// </summary>
        private void listViewUniqueValueContextMenu_OnClick(IColor icolor, bool UseCurrentColor = false) {
			IObjectCopy agObjCopy = new ObjectCopyClass();
            ISymbol tsymbol;
            foreach(int tidx in this.listViewUniqueValue.SelectedIndices) {
                // ｼﾝﾎﾞﾙ(塗り、ｱｳﾄﾗｲﾝ、ｻｲｽﾞ)を更新
                SetUniqueValueSymbol(m_work_UniqueValueRenderer, tidx - 1, icolor, UseCurrentColor, !UseCurrentColor, false);

				// ｼﾝﾎﾞﾙ設定を取得
                if(tidx == 0) {
                    tsymbol = m_work_UniqueValueRenderer.DefaultSymbol;
                }
                else {
					string	strVal = m_work_UniqueValueRenderer.get_Value(tidx - 1);
                    tsymbol = m_work_UniqueValueRenderer.get_Symbol(strVal);
                }

				// 変更内容を保存
                SaveUniqueValueEachColorToList(tidx, icolor);
                SaveUniqueValueEachInfoToList(tidx, SetSelectedItemSymbolAttributes(tsymbol), false);

				// ｼﾝﾎﾞﾙ･ｲﾒｰｼﾞを更新
				this.imageListSymbol.Images[tidx] = this.CreateSymbolImage(tsymbol,
                    this.imageListSymbol.ImageSize.Width, this.imageListSymbol.ImageSize.Height,
                    this.listViewUniqueValue.BackColor);
            }

            m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
            m_do_save[this.tabControl1.SelectedIndex] = false;

			// ﾘｽﾄ・ﾋﾞｭｰのｼﾝﾎﾞﾙ描画を更新
			this.listViewUniqueValue.Invalidate();
        }
        private void listViewUniqueValueContextMenu_OnClick0(IColor icolor) {
            bool changeDefSymbol = false;
            int currentIndex = -1;		// 最上位の選択位置を保存

            foreach(int tidx in this.listViewUniqueValue.SelectedIndices) {
                ISymbol tsymbol = null;
                if(tidx == 0) {
                    tsymbol = m_work_UniqueValueRenderer.DefaultSymbol;
                    changeDefSymbol = true;
                }
                else {
					string	strVal = m_work_UniqueValueRenderer.get_Value(tidx - 1);
                    tsymbol = m_work_UniqueValueRenderer.get_Symbol(strVal);
                }
                if(tsymbol == null) continue;

                // ｼﾝﾎﾞﾙ設定
                SetUniqueValueSymbol(m_work_UniqueValueRenderer, tidx - 1, icolor, false, false, false);
				// 変更内容を保存
                SaveUniqueValueEachColorToList(tidx, icolor);
                SaveUniqueValueEachInfoToList(tidx, SetSelectedItemSymbolAttributes(tsymbol), false);

				// ｼﾝﾎﾞﾙ･ｲﾒｰｼﾞを更新
				this.imageListSymbol.Images[tidx] = this.CreateSymbolImage(tsymbol,
                    this.imageListSymbol.ImageSize.Width, this.imageListSymbol.ImageSize.Height,
                    this.listViewUniqueValue.BackColor);

                if(!changeDefSymbol) { // ※個別値だけ変更する場合に、デフォルトシンボルの現状を再設定して置く
                    IColor tcolor = m_work_UniqueValueRendererEachColor[0];
                    if(tcolor != null) {
                        SetUniqueValueSymbol(m_work_UniqueValueRenderer, -1, tcolor, false, true, false);
                    }
                    changeDefSymbol = true;
                }

                if(tidx == 0) {
                    ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

                    // プレビュー更新してない場合に備えてデフォルトシンボルを更新
                    m_symbol_of_renderer[this.tabControl1.SelectedIndex] = objectCopy.Copy(tsymbol) as ISymbol;
                }

                if(currentIndex == -1) {
                    currentIndex = tidx;
                }
                else if(currentIndex > tidx) {
                    currentIndex = tidx;
                }
            }

            m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
            m_do_save[this.tabControl1.SelectedIndex] = false;

            if(currentIndex == -1) {
				// ﾘｽﾄ･ﾋﾞｭｰを再構成
				currentIndex = 0;
                SetEachUniqueValueToListView(m_work_UniqueValueRenderer, currentIndex);
			}
			else {
				// 変更箇所の描画を更新
				this.listViewUniqueValue.Invalidate();
			}

            // SetEachUniqueValueToListView2の代わりに部分更新対応として以下に変更予定
            // ５／１３以降
            //UpdateEachUniqueValueToListView(m_work_UniqueValueRenderer, currentIndex);
        }

        /// <summary>
        /// 2011/04/15 数値分類色変更
        /// 個別値リストビュー
        /// アイテムコンテキストメニュー色設定クリック時
        /// </summary>
        private void listViewClassBreaksContextMenu_OnClick(IColor icolor, bool UseCurrentColor = false) {
            int currentIndex = -1;		// 最上位の選択位置を保存
			ISymbol tsymbol;
            // 選択ｼﾝﾎﾞﾙのｲﾝﾃﾞｯｸｽを取得
            foreach(int tidx in this.listViewClassBreaks.SelectedIndices) {
                // ｼﾝﾎﾞﾙ情報を取得
				tsymbol = m_work_ClassBreaksRenderer.get_Symbol(tidx);
				if(tsymbol == null) continue;

                // ｼﾝﾎﾞﾙを設定
                SetClassBreaksSymbol(m_work_ClassBreaksRenderer, tsymbol, tidx, icolor, !UseCurrentColor, UseCurrentColor, SymbolUpdateOption.FROM_UPDATE_PREVIEW_DEFAULT);
                // ｼﾝﾎﾞﾙ･ｶﾗｰを保存
                SaveClassBreakEaceColorToList(tidx, icolor);
                // ｼﾝﾎﾞﾙ情報を保存
				tsymbol = m_work_ClassBreaksRenderer.get_Symbol(tidx);	// 新しいのを取得
                SaveClassBreaksEachInfoToList(tidx, SetSelectedItemSymbolAttributes(tsymbol), true);

				// ｼﾝﾎﾞﾙ･ｲﾒｰｼﾞを更新
				this.imageListSymbolClassBreaks.Images[tidx] = this.CreateSymbolImage(tsymbol,
                    this.imageListSymbolClassBreaks.ImageSize.Width, this.imageListSymbolClassBreaks.ImageSize.Height,
                    this.listViewClassBreaks.BackColor);

                if(currentIndex == -1) {
                    currentIndex = tidx;
                }
                else if(currentIndex > tidx) {
                    currentIndex = tidx;
                }
            }

            // 更新制御設定
            m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
            m_do_save[this.tabControl1.SelectedIndex] = false;

            if(currentIndex == -1) {
				// ﾘｽﾄ･ﾋﾞｭｰを再構成
				currentIndex = 0;
				SetEachClassBreaksToListView(m_work_ClassBreaksRenderer, currentIndex);
			}
			else {
				// 変更箇所の描画を更新
				this.listViewClassBreaks.Invalidate();
			}
        }

        /// <summary>
        /// 個別値分類シンボル設定
        /// </summary>
        /// <param name="pUniqueValueRenderer">個別値分類レンダラ</param>
        /// <param name="tsymbol">処理対象個別値シンボル</param>
        /// <param name="index">処理対象個別値シンボルインデクス</param>
        /// <param name="color">塗りつぶし色</param>
        /// <param name="useCurrentColor">設定済み塗りつぶし色使用判断フラグ</param>
        /// <param name="updateColor">塗り色だけ変更フラグ(アウトラインは無視)</param>
        /// <param name="useColorRamp">カラーランプ使用判定フラグ</param>
        private void SetUniqueValueSymbol(
            IUniqueValueRenderer pUniqueValueRenderer,
            int index, IColor color,
            bool useCurrentColor, bool updateColor, bool useColorRamp)
        {
            int definfoIdx; // 個別設定済み内容適用時に参照するインデクス

            // 個別値を取得
            string	xv = string.Empty;
			ISymbol tsymbol = null;
				
            if(index < 0) {
                xv = pUniqueValueRenderer.DefaultLabel;
				tsymbol = pUniqueValueRenderer.DefaultSymbol;
				definfoIdx = 0;
            }
            else {
                xv = pUniqueValueRenderer.get_Value(index);
				tsymbol = pUniqueValueRenderer.get_Symbol(xv);
				definfoIdx = index + 1;
            }

			IRgbColor	agColor;

			// 個別ｼﾝﾎﾞﾙを設定
            if(tsymbol is IMarkerSymbol) {
                IMarkerSymbol markersymbol = tsymbol as IMarkerSymbol;
				// 塗りを変更
                if(!useCurrentColor
					&& (m_work_UniqueValueRendererEachInfo == null || m_work_UniqueValueRendererEachInfo[definfoIdx].CanColorChenge)
					&& color != null) {
					if(markersymbol.Color != null && markersymbol.Color.RGB != color.RGB) {
						markersymbol.Color = color;
					}
				}

				// ｼﾝﾌﾟﾙの場合は、ｱｳﾄﾗｲﾝも更新
				ISimpleMarkerSymbol[]	agSSyms = null;
				if(this.HasSimpleMarkerSymbol(markersymbol)) {
					agSSyms = this.GetUnderSimpleLayer(markersymbol);
				}

                // ↓ｱｳﾄﾗｲﾝやｻｲｽﾞを再設定する必要はないのではないか ?
				if(!useColorRamp && !updateColor) {
                    markersymbol.Size = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);

					// ｱｳﾄﾗｲﾝの設定を変更
					if(agSSyms != null && agSSyms.Length > 0) {
						// 設定色を調整
						if(!this.buttonSetOutlineColor.BackColor.Equals(Color.Transparent)) {
							agColor = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
						}
						else {
							agColor = new RgbColorClass();
							agColor.NullColor = true;
						}

						foreach(var agSSym in agSSyms) {
							agSSym.OutlineSize = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);
							//agSSym.Outline = (agSSym.OutlineSize > 0.0);
							if(m_work_UniqueValueRendererEachInfo == null || m_work_UniqueValueRendererEachInfo[definfoIdx].CanOutlineColorChange) {
								agSSym.OutlineColor = agColor;
								agSSym.Outline = agSSym.OutlineSize > 0d;
							}
						}
					}
                }
			}
            else if(tsymbol is ILineSymbol) {
                ILineSymbol agLSym = tsymbol as ILineSymbol;
                if(!useCurrentColor
					&& (m_work_UniqueValueRendererEachInfo == null || m_work_UniqueValueRendererEachInfo[definfoIdx].CanColorChenge)
					&& color != null) {
					if(agLSym.Color != null && agLSym.Color.RGB != color.RGB) {
						agLSym.Color = color;
					}
				}

				// ↓ｱｳﾄﾗｲﾝやｻｲｽﾞを再設定する必要はないのではないか ?
                if(!useColorRamp && !updateColor) {
                    agLSym.Width = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);
                }
            }
			else if(tsymbol is IFillSymbol) {
				IRgbColor back_rgb = Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);
				IRgbColor outline_rgb = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);

                IFillSymbol fillSymbol = tsymbol as IFillSymbol;

                if(!useCurrentColor
					&& (m_work_UniqueValueRendererEachInfo == null || m_work_UniqueValueRendererEachInfo[definfoIdx].CanColorChenge)
					&& color != null) {
					if(fillSymbol.Color != null && fillSymbol.Color.RGB != color.RGB) {
						fillSymbol.Color = color;
					}
                }

                // ↓ｱｳﾄﾗｲﾝやｻｲｽﾞを再設定する必要はないのではないか ?
				if(!useColorRamp && !updateColor) {
					// ｱｳﾄﾗｲﾝの設定を変更
					if(fillSymbol.Outline is ILineSymbol) {
						// 設定色を調整
						if(!this.buttonSetOutlineColor.BackColor.Equals(Color.Transparent)) {
							agColor = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
						}
						else {
							agColor = new RgbColorClass();
							agColor.NullColor = true;
						}

						ILineSymbol lineSymbol = (ILineSymbol)fillSymbol.Outline;
						if(m_work_UniqueValueRendererEachInfo[definfoIdx].CanOutlineColorChange) {
							lineSymbol.Color =
								Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
						}

						lineSymbol.Width =
							Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);

						fillSymbol.Outline = lineSymbol;
					}
				}
            }

			// ｼﾝﾎﾞﾙを更新
            if(index < 0) {
                pUniqueValueRenderer.DefaultSymbol = tsymbol;
            }
            else {
                pUniqueValueRenderer.set_Symbol(xv, tsymbol);
            }
        }

        // 個別値分類・指定ｲﾝﾃﾞｯｸｽのｼﾝﾎﾞﾙを変更 (ｽﾀｲﾙ･ｷﾞｬﾗﾘｰからの変更)
		private void ChangeUniqueValueSymbol(IUniqueValueRenderer pUniqueValueRenderer, ISymbol NewSymbol, int ValIndex) {
            // 個別値を取得
            string	strVal = string.Empty;
            if(ValIndex >= 0) {
                strVal = pUniqueValueRenderer.get_Value(ValIndex);
            }

			// ｵﾌﾞｼﾞｪｸﾄのｺﾋﾟｰを作成
			IObjectCopy	agObjCopy = new ObjectCopyClass();
			ISymbol		agNewSym = agObjCopy.Copy(NewSymbol) as ISymbol;

			// ※塗り、ｻｲｽﾞ等、既存の設定を継承する場合は、下記のｺﾒﾝﾄを外す
/*
			// 個別ｼﾝﾎﾞﾙを設定
            switch(m_geometryType) {
			// Marker
            case esriGeometryType.esriGeometryPoint:
				IMarkerSymbol	agNewMSym = agNewSym as IMarkerSymbol;
				IMarkerSymbol	agOldMSym;
                if(ValIndex < 0) {
                    agOldMSym = pUniqueValueRenderer.DefaultSymbol as IMarkerSymbol;
                }
                else {
                    agOldMSym = pUniqueValueRenderer.get_Symbol(strVal) as IMarkerSymbol;
                }

				agNewMSym.Color = agOldMSym.Color;
				agNewMSym.Size = agOldMSym.Size;

                // ｼﾝﾌﾟﾙの場合はｱｳﾄﾗｲﾝの設定を継承
				if(agNewMSym is ISimpleMarkerSymbol && agOldMSym is ISimpleMarkerSymbol) {
					ISimpleMarkerSymbol	agNewSimpleM = agNewMSym as ISimpleMarkerSymbol;
					ISimpleMarkerSymbol	agOldSimpleM = agOldMSym as ISimpleMarkerSymbol;

                    agNewSimpleM.Outline = agOldSimpleM.Outline;
                    agNewSimpleM.OutlineSize = agOldSimpleM.OutlineSize;
					agNewSimpleM.OutlineColor = agOldSimpleM.OutlineColor;
				}
                break;
			// Line
            case esriGeometryType.esriGeometryPolyline:
				ILineSymbol	agNewLSym = agNewSym as ILineSymbol;
				ILineSymbol	agOldLSym;
                if(ValIndex < 0) {
                    agOldLSym = pUniqueValueRenderer.DefaultSymbol as ILineSymbol;
                }
                else {
                    agOldLSym = pUniqueValueRenderer.get_Symbol(strVal) as ILineSymbol;
                }

				agNewLSym.Color = agOldLSym.Color;
				agNewLSym.Width = agOldLSym.Width;
                break;
			// Fill
            case esriGeometryType.esriGeometryPolygon:
				IFillSymbol	agNewFSym = agNewSym as IFillSymbol;
				IFillSymbol	agOldFSym;
                if(ValIndex < 0) {
                    agOldFSym = pUniqueValueRenderer.DefaultSymbol as IFillSymbol;
                }
                else {
                    agOldFSym = pUniqueValueRenderer.get_Symbol(strVal) as IFillSymbol;
                }

				agNewFSym.Color = agOldFSym.Color;
				agNewFSym.Outline = agOldFSym.Outline;
                break;
            }
*/
            // 変更をｾｯﾄ
			if(ValIndex < 0) {
				bool	blnUseDef = pUniqueValueRenderer.UseDefaultSymbol;
				pUniqueValueRenderer.DefaultSymbol = null;
                pUniqueValueRenderer.DefaultSymbol = agNewSym;
				pUniqueValueRenderer.UseDefaultSymbol = blnUseDef;
            }
            else {
                pUniqueValueRenderer.set_Symbol(strVal, agNewSym);
            }
        }

        /// <summary>
        /// 数値分類シンボル設定
        /// </summary>
        /// <param name="classBreaksRenderer">数値分類レンダラ</param>
        /// <param name="tsymbol">数値分類シンボル</param>
        /// <param name="i">処理対象数値分類シンボルのインデクス</param>
        /// <param name="color">設定色</param>
        /// <param name="updateColor"></param>
        /// <param name="useCurrentColor"></param>
        /// <param name="whereFrom"></param>
        private void SetClassBreaksSymbol(
            IClassBreaksRenderer classBreaksRenderer,
            ISymbol tsymbol, int i, IColor color,
            bool updateColor, bool useCurrentColor, SymbolUpdateOption UpdateOption)
        {
			ISymbol agSym;
			ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = new ESRI.ArcGIS.esriSystem.ObjectCopyClass();

			if(UpdateOption != SymbolUpdateOption.FROM_STYLE_GALLERY) {
				// ｼﾝﾎﾞﾙを取得
				try {
					agSym = classBreaksRenderer.get_Symbol(i);
				}
				catch {
					// CB作成時はｼﾝﾎﾞﾙがｾｯﾄされていない
					agSym = objectCopy.Copy(tsymbol) as ISymbol;
					this.m_work_ClassBreaksRendererEachInfo = null;
				}

				// ｼﾝﾎﾞﾙ設定
				if(tsymbol is IMarkerSymbol) {
					IMarkerSymbol markerSymbol = agSym as IMarkerSymbol;

					// 塗りを変更
					if(!useCurrentColor
						&& (this.m_work_ClassBreaksRendererEachInfo == null || this.m_work_ClassBreaksRendererEachInfo[i].CanColorChenge)
						&& color != null) {
						if(markerSymbol.Color != null && markerSymbol.Color.RGB != color.RGB) {
							markerSymbol.Color = color;
						}
					}

					// ｼﾝﾌﾟﾙの場合は、ｱｳﾄﾗｲﾝも更新
					ISimpleMarkerSymbol[]	agSSyms = null;
					if(this.HasSimpleMarkerSymbol(markerSymbol)) {
						agSSyms = this.GetUnderSimpleLayer(markerSymbol);
					}

					if(UpdateOption != SymbolUpdateOption.FROM_COLOR_RAMP) {
						if(!updateColor) {
							markerSymbol.Size = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);

							// ｱｳﾄﾗｲﾝを設定
							if(agSSyms != null && agSSyms.Length > 0) {
								foreach(var agSSym in agSSyms) {
									agSSym.OutlineSize = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);
									if(m_work_ClassBreaksRendererEachInfo == null || m_work_ClassBreaksRendererEachInfo[i].CanOutlineColorChange) {
										agSSym.OutlineColor = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
										agSSym.Outline = agSSym.OutlineSize > 0d;
									}
								}
							}
						}

						//if(agSSyms != null && agSSyms.Length > 0) {
						//	foreach(var agSSym in agSSyms) {
						//		if(this.m_work_ClassBreaksRendererEachInfo[i].CanOutlineColorChange) {
						//			agSSym.OutlineColor = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
						//		}
						//		//agSSym.Outline = (agSSym.OutlineSize > 0.0);
						//	}
						//}
					}
				}
				else if(tsymbol is ILineSymbol) {
					ILineSymbol agLSym = agSym as ILineSymbol;
					if(!useCurrentColor
						&& (this.m_work_ClassBreaksRendererEachInfo == null || this.m_work_ClassBreaksRendererEachInfo[i].CanColorChenge)
						&& color != null) {
						if(agLSym.Color != null && agLSym.Color.RGB != color.RGB) {
							agLSym.Color = color;
						}
					}

					if(UpdateOption != SymbolUpdateOption.FROM_COLOR_RAMP) {
						if(!updateColor) {
							agLSym.Width = Convert.ToDouble(numericUpDownSimpleSymbolSizeOrWidth.Value);
						}
					}
				}
				else if(tsymbol is IFillSymbol) {
					IFillSymbol pClassFillSymbol = agSym as IFillSymbol;

					if(!useCurrentColor
						&& (this.m_work_ClassBreaksRendererEachInfo == null || this.m_work_ClassBreaksRendererEachInfo[i].CanColorChenge)
						&& color != null) {
						if(pClassFillSymbol.Color != null && pClassFillSymbol.Color.RGB != color.RGB) {
							pClassFillSymbol.Color = color;
						}
					}
					
					if(UpdateOption != SymbolUpdateOption.FROM_COLOR_RAMP) {
						if(!updateColor) {
							if(pClassFillSymbol.Outline is ILineSymbol) {
								ILineSymbol lineSymbol = (ILineSymbol)pClassFillSymbol.Outline;
								lineSymbol.Width = Convert.ToDouble(numericUpDownSimpleSymbolOutLineWidth.Value);

								if(this.m_work_ClassBreaksRendererEachInfo == null || this.m_work_ClassBreaksRendererEachInfo[i].CanOutlineColorChange) {
									lineSymbol.Color = Common.UtilityClass.ConvertToESRIColor(buttonSetOutlineColor.BackColor);
								}
								pClassFillSymbol.Outline = lineSymbol;
							}
						}
					}
				}
			}
			else {
				agSym = objectCopy.Copy(tsymbol) as ISymbol;
			}

            classBreaksRenderer.set_Symbol(i, agSym);
        }

        /// <summary>
        /// 数値分類時
        /// 個別に塗潰し色指定時にその色を保持するListに設定する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="eachcolor"></param>
        private void SaveClassBreakEaceColorToList(int index, IColor eachcolor) {
            // 初期化
            if(m_work_ClassBreaksRendererEachColor == null || m_work_ClassBreaksRendererEachColor.Count == 0) {
                m_work_ClassBreaksRendererEachColor = new List<IColor>();

                for(int idx = 0; idx < m_work_ClassBreaksRenderer.BreakCount + 1; idx++) {
                    m_work_ClassBreaksRendererEachColor.Add(null);
                }
            }

            // 保存
            m_work_ClassBreaksRendererEachColor[index] = eachcolor;
        }

        /// <summary>
        /// 数値値時に
        /// 個別に塗潰し個別選択時にそのシンボルを保持するListに設定する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="eachinfo"></param>
        /// <param name="seteach"></param>
        private void SaveClassBreaksEachInfoToList(int index, SelectedSymbolInfo eachinfo, bool seteach) {
			// 初期化
            if(m_work_ClassBreaksRendererEachInfo == null || m_work_ClassBreaksRendererEachInfo.Count == 0) {
                m_work_ClassBreaksRendererEachInfo = new List<SelectedSymbolInfo>();

                for(int idx = 0; idx < m_work_ClassBreaksRenderer.BreakCount; idx++) {
                    m_work_ClassBreaksRendererEachInfo.Add(new SelectedSymbolInfo());
                }
            }

            if(!seteach) {
                eachinfo.selectedItem = true;
            }
            else {
                eachinfo.selectedItem = m_work_ClassBreaksRendererEachInfo[index].selectedItem;
            }

            // 保存
            m_work_ClassBreaksRendererEachInfo[index] = eachinfo;
        }

        /// <summary>
        /// 個別値時に
        /// 個別に塗潰し色指定時にその色を保持するListに設定する
        /// </summary>
        private void SaveUniqueValueEachColorToList(int index, IColor eachcolor) {
			// 初期化
            if(m_work_UniqueValueRendererEachColor == null || m_work_UniqueValueRendererEachColor.Count == 0 || 
                (m_work_UniqueValueRendererEachColor.Count - 1 != m_work_UniqueValueRenderer.ValueCount)) {
                /* m_work_UniqueValueRendererEachColorには デフォルトシンボル分も格納されているので＋１ */

                m_work_UniqueValueRendererEachColor = new List<IColor>();

                // default symbol の分まで格納
                for(int idx = 0; idx < m_work_UniqueValueRenderer.ValueCount + 1; idx++) {
                    m_work_UniqueValueRendererEachColor.Add(null);
                }
            }

			// 色ﾘｽﾄに保存
			m_work_UniqueValueRendererEachColor[index] = eachcolor;
        }

        /// <summary>
        /// 個別値時に
        /// 個別に塗潰し個別選択時にそのシンボルを保持するListに設定する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="eachinfo"></param>
        /// <param name="seteach"></param>
        private void SaveUniqueValueEachInfoToList(int index, SelectedSymbolInfo eachinfo, bool seteach) {
			// 初期化
            if(m_work_UniqueValueRendererEachInfo == null || m_work_UniqueValueRendererEachInfo.Count == 0 ||
                (m_work_UniqueValueRendererEachInfo.Count - 1 != m_work_UniqueValueRenderer.ValueCount)) {
				// m_work_UniqueValueRendererEachSymbolには デフォルトシンボル分も格納されているので＋１

                m_work_UniqueValueRendererEachInfo = new List<SelectedSymbolInfo>();

                // default symbol の分まで格納
                for(int idx = 0; idx < m_work_UniqueValueRenderer.ValueCount + 1; idx++) {
                    m_work_UniqueValueRendererEachInfo.Add(new SelectedSymbolInfo());
                }
            }

            if(!seteach) {
                eachinfo.selectedItem = true;
            }
            else {
                eachinfo.selectedItem = m_work_UniqueValueRendererEachInfo[index].selectedItem;
            }

            // 保存
            m_work_UniqueValueRendererEachInfo[index] = eachinfo;
        }

        /// <summary>
        /// 選択シンボルの属性情報
        /// </summary>
        private class SelectedSymbolInfo {
            public bool IsInfoSet;

            /// <summary>
            /// 選択シンボルの塗潰し色
            /// </summary>
            public IRgbColor backColor;

			/// <summary>
			/// 色変更可能かどうか
			/// </summary>
			public bool	CanColorChenge;
            
            /// <summary>
            /// 選択シンボルのサイズ（ポイント時）、
            /// 幅（ライン時）
            /// -0.1　（ポリゴン時は指定不可）
            /// </summary>
            public double symbolSizeOrWidth;

            /// <summary>
            /// 選択シンボルのアウトラインカラー
            /// (ポイント、ポリゴン時だけ）
            /// （ライン時は指定不可: null）
            /// </summary>
            public IColor outLineColor;

			/// <summary>
			/// アウトラインの色変更可能かどうか
			/// </summary>
			public bool CanOutlineColorChange;

            /// <summary>
            /// 選択シンボルのアウトライン幅
            /// (ポイント、ポリゴン時だけ）
            /// （ライン時は指定不可：-0.1）
            /// </summary>
            public double outLineWidth;

            /// <summary>
            /// リストビュー上で選択されたか否かを判定
            /// </summary>
            public bool selectedItem;

			/// <summary>
			/// Constructor
			/// </summary>
			public SelectedSymbolInfo() {
				this.CanColorChenge = true;
				this.CanOutlineColorChange = true;
			}
        }

        /// <summary>
        /// 個別値か数値
        /// 指定シンボルの属性（色、サイズ、アウトライン（色、幅）を
        /// 画面上のコントロールに設定
        /// </summary>
		/// <remarks>※塗りボタンが有効の時に実行される前提</remarks>
        private void SetSelectedSymbolInfoToGUIContorl() {
			int	intCurrentIndex = -1;
            ListView.SelectedIndexCollection indexes = null;

			if(selectingall && selectingall_cnt > 0) return;

            if(tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                if(m_work_UniqueValueRenderer.ValueCount == 0) return;
                indexes = this.listViewUniqueValue.SelectedIndices;
            }
            else {
                if(m_work_ClassBreaksRenderer.BreakCount == 0) return;
                indexes = this.listViewClassBreaks.SelectedIndices;
            }
			if(indexes.Count > 0) {
				intCurrentIndex = indexes[0];
			}
			else {
				return;
			}

#if DEBUG
			// ↓検証中 きちんとやるとしたら、それなりの工数が必要。なので元のままとする。
			//if(this._selSymbol.IsSelect) {
			//	intCurrentIndex = this._selSymbol.Index;
			//}
			//else {
			//	return;
			//}
#endif

            if(intCurrentIndex >= 0) {
				SelectedSymbolInfo	symbolinfo = new SelectedSymbolInfo();
				try {
					if(tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
						symbolinfo = this.m_work_UniqueValueRendererEachInfo[intCurrentIndex];
					}
					else {
						symbolinfo = this.m_work_ClassBreaksRendererEachInfo[intCurrentIndex];
					}

					// 選択ｼﾝﾎﾞﾙをｺﾋﾟｰ
					ISymbol tsymbol = null;
					if(tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
						if(intCurrentIndex == 0) {
							tsymbol = m_work_UniqueValueRenderer.DefaultSymbol;
						}
						else {
							tsymbol = m_work_UniqueValueRenderer.get_Symbol(m_work_UniqueValueRenderer.get_Value(intCurrentIndex - 1));
						}
					}
					else {
						tsymbol = m_work_ClassBreaksRenderer.get_Symbol(intCurrentIndex);
					}

					if(tsymbol != null) {
						//symbolinfo = SetSelectedItemSymbolAttributes(tsymbol);
						IObjectCopy agObjCopy = new ObjectCopyClass();
						m_symbol_of_renderer[this.tabControl1.SelectedIndex] = agObjCopy.Copy(tsymbol) as ISymbol;
					}

					if(symbolinfo.IsInfoSet) {
						// 塗りﾎﾞﾀﾝの設定
						if(symbolinfo.backColor == null) {
							this.buttonSetColor.Enabled = false;
						}
						else {
							this.buttonSetColor.Enabled = symbolinfo.CanColorChenge;
						}
						SetButtonBackColor(symbolinfo.backColor, this.buttonSetColor);
						this.labelSetColor.Enabled = this.buttonSetColor.Enabled;

						// ｼﾝﾎﾞﾙ･ｻｲｽﾞ
						if(numericUpDownSimpleSymbolSizeOrWidth.Enabled) {
							decimal	decW = Convert.ToDecimal(symbolinfo.symbolSizeOrWidth);
							if(decW != -1.0m && decW > 0.0m && numericUpDownSimpleSymbolSizeOrWidth.Value != decW) {
								this.InitLoad = true;
								numericUpDownSimpleSymbolSizeOrWidth.Value = decW;
								this.InitLoad = false;
							}
						}

						// ｱｳﾄﾗｲﾝ塗り
						this.panelSimpleSymbolOutline.Enabled = symbolinfo.CanOutlineColorChange;
						this.buttonSetOutlineColor.Enabled = symbolinfo.CanOutlineColorChange;
						this.labelSetOutlineColor.Enabled = symbolinfo.CanOutlineColorChange;
						SetButtonBackColor(symbolinfo.outLineColor as IRgbColor, this.buttonSetOutlineColor);

						// ｱｳﾄﾗｲﾝ幅
						this.numericUpDownSimpleSymbolOutLineWidth.Enabled = symbolinfo.CanOutlineColorChange;
						this.labelUpDownSimpleSymbolOutLineWidth.Enabled = symbolinfo.CanOutlineColorChange;
						decimal	decLW = Convert.ToDecimal(symbolinfo.outLineWidth);
						if(decLW != -1.0m && numericUpDownSimpleSymbolOutLineWidth.Value != decLW) {
							this.InitLoad = true;
							numericUpDownSimpleSymbolOutLineWidth.Value = decLW;
							this.InitLoad = false;
						}

						if(selectingall) selectingall_cnt++;
					}
				}
				catch(Exception ex) {
					Common.UtilityClass.DoOnError(ex);
				}
			}
		}

        /// <summary>
        /// 指定シンボルの属性（色、サイズ、アウトライン（色、幅）を取得
        /// </summary>
        /// <param name="tsymbol"></param>
        private SelectedSymbolInfo SetSelectedItemSymbolAttributes(ISymbol tsymbol) {
            SelectedSymbolInfo selSymbolInfo = new SelectedSymbolInfo();
            selSymbolInfo.IsInfoSet = false;

            if(tsymbol is IMarkerSymbol) {
                IMarkerSymbol markersymbol = tsymbol as IMarkerSymbol;
				//if(!this.IsPictureSymbol(tsymbol)) {
					if(!(markersymbol.Color == null || markersymbol.Color.NullColor)) {
						IRgbColor esriColor = new RgbColorClass();
						esriColor.RGB = markersymbol.Color.RGB;
						esriColor.CMYK = markersymbol.Color.CMYK;
						esriColor.Transparency = markersymbol.Color.Transparency;
						esriColor.UseWindowsDithering = markersymbol.Color.UseWindowsDithering;

						selSymbolInfo.backColor = esriColor;
					}

					selSymbolInfo.CanColorChenge = CanChangeColor(tsymbol, false);
				//}
                selSymbolInfo.symbolSizeOrWidth = markersymbol.Size;
                selSymbolInfo.IsInfoSet = true;

                // ｱｳﾄﾗｲﾝ情報
				if(this.HasSimpleMarkerSymbol(markersymbol)) {
                    ISimpleMarkerSymbol pSimpleMarker = this.GetUnderSimpleLayer(markersymbol).First();
                    selSymbolInfo.outLineColor = pSimpleMarker.OutlineColor;
                    selSymbolInfo.outLineWidth = pSimpleMarker.OutlineSize;

					selSymbolInfo.CanOutlineColorChange = true;
                }
				else {
					selSymbolInfo.CanOutlineColorChange = false;
				}
            }
            else if(tsymbol is ILineSymbol) {
                ILineSymbol markeLineSym = tsymbol as ILineSymbol;
				//if(!this.IsPictureSymbol(tsymbol)) {
					if(!(markeLineSym.Color == null || markeLineSym.Color.NullColor)) {
						IRgbColor esriColor = new RgbColorClass();
						esriColor.RGB = markeLineSym.Color.RGB;
						esriColor.CMYK = markeLineSym.Color.CMYK;
						esriColor.Transparency = markeLineSym.Color.Transparency;
						esriColor.UseWindowsDithering = markeLineSym.Color.UseWindowsDithering;

						selSymbolInfo.backColor = esriColor;
					}
					selSymbolInfo.CanColorChenge = CanChangeColor(tsymbol, false);
				//}

				selSymbolInfo.CanOutlineColorChange = false;
                selSymbolInfo.symbolSizeOrWidth = markeLineSym.Width;
                selSymbolInfo.IsInfoSet = true;
            }
            else if(tsymbol is IFillSymbol) {
                IFillSymbol pFillsymbol = tsymbol as IFillSymbol;
				//if(!this.IsPictureSymbol(tsymbol)) {
					if(!(pFillsymbol.Color == null || pFillsymbol.Color.NullColor)) {
						IRgbColor esriColor = new RgbColorClass();
						esriColor.RGB = pFillsymbol.Color.RGB;
						esriColor.CMYK = pFillsymbol.Color.CMYK;
						esriColor.Transparency = pFillsymbol.Color.Transparency;
						esriColor.UseWindowsDithering = pFillsymbol.Color.UseWindowsDithering;

						selSymbolInfo.backColor = esriColor;
					}
					selSymbolInfo.CanColorChenge = CanChangeColor(tsymbol, false);
				//}
				selSymbolInfo.CanOutlineColorChange = CanChangeColor(tsymbol, true);
                selSymbolInfo.outLineColor = pFillsymbol.Outline.Color;
                selSymbolInfo.outLineWidth = pFillsymbol.Outline.Width;
                selSymbolInfo.IsInfoSet = true;
            }

            return selSymbolInfo;
        }

		/// <summary>
		/// 指定のシンボルが画像シンボルかどうか
		/// </summary>
		/// <param name="TargetSymbol"></param>
		/// <returns>画像 / その他</returns>
		private bool IsPictureSymbol(ISymbol TargetSymbol) {
			bool	blnRet = false;

			int		intPicLayerNum = 0;
			if(TargetSymbol is IMarkerSymbol) {
				if(TargetSymbol is IPictureMarkerSymbol) {
					blnRet = true;
				}
				else if(TargetSymbol is IMultiLayerMarkerSymbol) {
					var agMMSym = TargetSymbol as IMultiLayerMarkerSymbol;
					foreach(int intCnt in Enumerable.Range(0, agMMSym.LayerCount)) {
						if(agMMSym.get_Layer(intCnt) is IPictureMarkerSymbol) {
							++intPicLayerNum;
						}
						else {
							break;
						}
					}
					blnRet = agMMSym.LayerCount > 0 && intPicLayerNum == agMMSym.LayerCount;
				}
			}
			else if(TargetSymbol is ILineSymbol) {
				if(TargetSymbol is IPictureLineSymbol) {
					blnRet = true;
				}
				else if(TargetSymbol is IMultiLayerLineSymbol) {
					var agMLSym = TargetSymbol as IMultiLayerLineSymbol;
					foreach(int intCnt in Enumerable.Range(0, agMLSym.LayerCount)) {
						if(agMLSym.get_Layer(intCnt) is IPictureLineSymbol) {
							++intPicLayerNum;
						}
						else {
							break;
						}
					}
					blnRet = agMLSym.LayerCount > 0 && intPicLayerNum == agMLSym.LayerCount;
				}
			}
			else if(TargetSymbol is IFillSymbol) {
				if(TargetSymbol is IPictureFillSymbol) {
					blnRet = true;
				}
				else if(TargetSymbol is IMultiLayerFillSymbol) {
					var agMFSym = TargetSymbol as IMultiLayerFillSymbol;
					foreach(int intCnt in Enumerable.Range(0, agMFSym.LayerCount)) {
						if(agMFSym.get_Layer(intCnt) is IPictureFillSymbol) {
							++intPicLayerNum;
						}
						else {
							break;
						}
					}
					blnRet = agMFSym.LayerCount > 0 && intPicLayerNum == agMFSym.LayerCount;
				}
			}

			return blnRet;
		}

		/// <summary>
		/// 指定のマーカーがアウトラインの変更が可能なシンプルを含んでいるかどうか判定します
		/// </summary>
		/// <param name="TargetMarker"></param>
		/// <returns></returns>
		private bool HasSimpleMarkerSymbol(IMarkerSymbol TargetMarker) {
			bool	blnRet = false;

			// ｼﾝﾌﾟﾙｼﾝﾎﾞﾙ1つの場合は有効
			if(TargetMarker is ISimpleMarkerSymbol) {
				blnRet = true;
			}
			else if(TargetMarker is IMultiLayerMarkerSymbol) {
				// ｼﾝﾌﾟﾙ･ﾚｲﾔｰ
				var agMMSym = TargetMarker as IMultiLayerMarkerSymbol;

				ISimpleMarkerSymbol	agSMSym;
				foreach(int intCnt in Enumerable.Range(0, agMMSym.LayerCount)) {
					agSMSym = agMMSym.get_Layer(intCnt) as ISimpleMarkerSymbol;

					// ｼﾝﾌﾟﾙｼﾝﾎﾞﾙ1つの場合は有効、複数の場合は1つでもｱｳﾄﾗｲﾝがある場合に有効
					if(agSMSym != null && (agSMSym.Outline || agMMSym.LayerCount == 1)) {
						blnRet = true;
						break;
					}
					// 最上層がﾋﾟｸﾁｬｰの場合は、無効
					else if(intCnt == 0 && agMMSym.get_Layer(intCnt) is IPictureMarkerSymbol) {
						break;
					}
				}
			}

			return blnRet;
		}

        /// <summary>
        /// シンボルの塗り色を取得します
        /// </summary>
        /// <param name="TargetSymbol">対象シンボル</param>
        /// <returns>色</returns>
		static public IColor GetSymbolColor(ISymbol TargetSymbol) {
			IColor	agColor = new RgbColorClass();

			// Point
            if(TargetSymbol is IMarkerSymbol) {
				agColor = (TargetSymbol as IMarkerSymbol).Color;
            }
            // Line
			else if(TargetSymbol is ILineSymbol) {
				agColor = (TargetSymbol as ILineSymbol).Color;
			}
			// Polygon
			else if(TargetSymbol is IFillSymbol) {
				agColor = (TargetSymbol as IFillSymbol).Color;
			}

            return agColor;
        }

        /// <summary>
        /// 個別値分類・数値分類
        /// リストビュー要素選択判定によって関連するボタンの制御
        /// 使用可能／不可能切り替え
        /// </summary>
        private void SetListViewSelectedValueCheck() {
            // 共通設定
            {
				// 塗り設定
				SetColorButtonEnable = CanUseColorButtons();
				//buttonSetColor.Enabled = SetColorButtonEnable;
				//labelSetColor.Enabled = SetColorButtonEnable;
				if(SetColorButtonEnable) {
					// 全て選択ではない場合
					SetSelectedSymbolInfoToGUIContorl();
				}
			}

            // 個別値分類 ｺﾝﾄﾛｰﾙ制御
            if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                if(SetColorButtonEnable &&
                    this.listViewUniqueValue.SelectedItems.Count == this.listViewUniqueValue.Items.Count) {
                    buttonSelectAll_UniqueValue.Enabled = !SetColorButtonEnable;
                }
                else {
                    buttonSelectAll_UniqueValue.Enabled = 
                        (this.listViewUniqueValue.Items.Count > 0);
                }

                buttonCancelSelect_UniqueValue.Enabled = SetColorButtonEnable;
                buttonRemoveValues.Enabled = SetColorButtonEnable;
            }
            // 数値分類 ｺﾝﾄﾛｰﾙ制御
            else if(this.tabControl1.SelectedIndex == CLASSBREAK_RENDERER) {
                buttonSelectAll_ClassBreaks.Enabled = SetColorButtonEnable;
                
                if (SetColorButtonEnable &&
                    this.listViewClassBreaks.SelectedItems.Count == this.listViewClassBreaks.Items.Count) {
                    buttonSelectAll_ClassBreaks.Enabled = !SetColorButtonEnable;
                }
                else {
                    buttonSelectAll_ClassBreaks.Enabled =
                        (this.listViewClassBreaks.Items.Count > 0);
                }
                
                buttonCancelSelect_ClassBreaks.Enabled = SetColorButtonEnable;
            }
        }

        /// <summary>
        /// 個別値個別色変更対応
        /// 一つ以上、個別値が選択されていれば色指定ボタン利用可能
        /// この状態でプレビュー更新すると選択されている個別値に対してだけ
        /// 色、サイズ、アウトライン（色、幅）がプレビュー上反映される
        /// OK、適用で確定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewUniqueValue_SelectedIndexChanged(object sender, EventArgs e) {
            if(!selectingall) {
				// 選択ｱｲﾃﾑ切り替えｲﾍﾞﾝﾄの判定
				SetListViewSelectedValueCheck();
			}
        }

        /// <summary>
        /// 個別値、数値
        /// 選択解除設定
        /// </summary>
        /// <param name="selectswt"></param>
        private void SelectOrCancelAllValues(bool selectswt) {
            ListView lstview = null;
            int startidx = 0;
            if(this.tabControl1.SelectedIndex == UNIQUEVALUE_RENDERER) {
                lstview = this.listViewUniqueValue;
                startidx = 1;
            }
            else {
                lstview = this.listViewClassBreaks;
            }

            selectingall = true;	// ﾘｽﾄ選択ｲﾍﾞﾝﾄ制御 ON
            lstview.HideSelection = false;
            
			lstview.BeginUpdate();

            if(selectswt) {
                // "その他全て"は全て選択には含まない
                //for(int idx = startidx; idx < lstview.Items.Count; idx++) {
				foreach(int idx in Enumerable.Range(startidx, lstview.Items.Count - startidx)) {
                    lstview.Items[idx].Selected = true;
                }
                lstview.Focus();
            }
            else {
				//for(int idx = 0; idx < lstview.SelectedIndices.Count; idx++) {
				//	System.Collections.IEnumerator listviewEnum =
				//		lstview.SelectedItems.GetEnumerator();

				//	while(listviewEnum.MoveNext()) {
				//		ListViewItem viewItem = (ListViewItem)listviewEnum.Current;
				//		viewItem.Selected = false;
				//	}
				//}
				foreach(int intCnt in lstview.SelectedIndices) {
					lstview.Items[intCnt].Selected = false;
				}
            }
			lstview.EndUpdate();

            selectingall = false;	// ﾘｽﾄ選択ｲﾍﾞﾝﾄ制御 OFF
            selectingall_cnt = 0;

            SetListViewSelectedValueCheck();
        }

        /// <summary>
        /// 個別値
        /// 削除対象要素へ削除フラグ設定
        /// </summary>
        /// <param name="Index">削除インデックス</param>
        /// <param name="IsInitOnly">初期化だけ実行</param>
        /// <param name="Repair">場合によっては初期化してから設定</param>
        private void SaveUniqueValueDelSymbolToList(int Index, bool IsInitOnly, bool Repair) {
            // m_work_UniqueValueRendererSetOthersSymbolにはデフォルトシンボルは含まない。
            // デフォルトシンボルの場合は削除ではなく、UseDefalutSymbolプロパティをfalse設定する
            // プレビュー上からも削除しないで表示はしたままとする。
            //　連続実行時には最初の一度だけ行なう
            bool doInit = (m_work_UniqueValueRendererSetOthersSymbol == null || m_work_UniqueValueRendererSetOthersSymbol.Count == 0);
            if(!doInit && (IsInitOnly || Repair 
				&& (m_work_UniqueValueRendererSetOthersSymbol.Count != m_work_UniqueValueRenderer.ValueCount ||
                    m_work_UniqueValueRendererSetOthersSymbol.Count < m_work_UniqueValueRenderer.ValueCount))) {
                doInit = true;
            }

            if(doInit) {
				// 個別値削除ﾌﾗｸﾞの初期化
                m_work_UniqueValueRendererSetOthersSymbol = new List<bool>();

                // default symbolも格納
                for(int idx = 0; idx < m_work_UniqueValueRenderer.ValueCount + 1; idx++) {
                    m_work_UniqueValueRendererSetOthersSymbol.Add(false);
                }
            }

			if(!IsInitOnly) {
				try {
					// その他シンボルへ変更するシンボル ※指定値を[その他]としてﾏｰｸする
					if(!m_work_UniqueValueRendererSetOthersSymbol[Index]) {
						m_work_UniqueValueRendererSetOthersSymbol[Index] = true;
					}
				}
				catch { }
			}
        }

        /// <summary>
        /// 個別値、数値
        /// 全選択時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectAll_ListViewValue_Click(object sender, EventArgs e) {
            SelectOrCancelAllValues(true);

            selectingall = true;
            SetSelectedSymbolInfoToGUIContorl();
            selectingall = false;
        }

        /// <summary>
        /// 個別値、数値
        /// 全解除時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancelSelect_ListViewValue_Click(object sender, EventArgs e) {
            SelectOrCancelAllValues(false);
        }

        /// <summary>
        /// 個別値
        /// 選択要素のレンダラから取り除く
        /// </summary>
        private void RemoveSelectedUniqueValues() {
            bool dispMessage = false;
            try {
                // 分類ﾌｨｰﾙﾄﾞ名を取得
				//listViewUniqueValueContextMenuDeleteSymbol_OnClick();
                string tname = ((Common.FieldComboItem)comboBoxFieldName.SelectedItem).FieldName;
                if(tname.Length == 0) return;

                ListView.SelectedIndexCollection indexes = this.listViewUniqueValue.SelectedIndices;

				// 指定値を[その他]としてﾏｰｸする
                int		execcnt = 0;
                foreach(int tidx in indexes) {
					// 削除ﾌﾗｸﾞを設定 (初回は初期化)
                    // (ListViewにはデフォルトシンボルも含まれているのでレンダラ上のインデクスとは-1差がある)
                    SaveUniqueValueDelSymbolToList(tidx, false, execcnt++ <= 0);
                }

                if((indexes.Count == 1 && indexes[0] == 0 && execcnt == 1) || 
                    m_work_UniqueValueRendererSetOthersSymbol == null ||
                    m_work_UniqueValueRendererSetOthersSymbol.Count == 0) {
                    return;
                }

                // 個別値ｼﾝﾎﾞﾙを更新
				UpdatePreview(
                    this.style_colorRamp,
                    m_symbol_of_renderer[this.tabControl1.SelectedIndex], 
                    false, true, SymbolUpdateOption.FROM_UPDATE_PREVIEW_DEFAULT);

                //System.Threading.Thread.Sleep(500);
                //this.listViewUniqueValue.Items[0].Selected = true;

                m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
                m_do_save[this.tabControl1.SelectedIndex] = false;
            }
            catch(Exception ex) {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally {
                listViewUniqueValue.SelectedIndices.Clear();
                listViewUniqueValue.SelectedItems.Clear();

                if(dispMessage) {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.FormSymbolSettings_ErrorWhenSymbolColorSetting);
                }
            }
        }

        /// <summary>
        /// 個別値分類個別削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemoveValues_Click(object sender, EventArgs e) {
            if(this.listViewUniqueValue.SelectedIndices.Count > 0) {
                ListView.SelectedIndexCollection indexes = this.listViewUniqueValue.SelectedIndices;
                if(indexes[0] == 0) {
                    // [その他の値すべて]は削除出来ません
                    Common.MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormSymbolSettings_WARNING_DefaultSymbol_CannotDelete);
                }
				else {
					// 削除処理を実行
					RemoveSelectedUniqueValues();
					// 設定画面を調整する
					SetListViewSelectedValueCheck();
				}
				// ﾘｽﾄにﾌｫｰｶｽ
				this.listViewUniqueValue.Select();
            }
        }

        /// <summary>
        /// 数値分類
        /// 要素選択内容変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewClassBreaks_SelectedIndexChanged(object sender, EventArgs e) {
            if(!selectingall) {
				// ｺﾝﾄﾛｰﾙ制御
				SetListViewSelectedValueCheck();
			}
        }

        /// <summary>
        /// 個別値
        /// デフォルトシンボル使用未使用切り替え設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUseUniqueValueDefaultSymbol_CheckedChanged(object sender, EventArgs e) {
            if(InitLoad) return;

			if(m_work_UniqueValueRenderer != null) {
				m_work_UniqueValueRenderer.UseDefaultSymbol = (sender as CheckBox).Checked;
			}

            m_do_renderer_update[this.tabControl1.SelectedIndex] = false;
            m_do_save[this.tabControl1.SelectedIndex] = false;
        }

        /// <summary>
        /// デフォルトシンボル使用未使用切り替え
        /// 手動にて切り替えた場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUseUniqueValueDefaultSymbol_Click(object sender, EventArgs e)
        {
            m_do_renderer_update[this.tabControl1.SelectedIndex] = false;
            m_do_save[this.tabControl1.SelectedIndex] = false;
        }

		/// <summary>
		/// 分類リストビュー MOUSEUP EVENT
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_MouseUp(object sender, MouseEventArgs e) {
			// 左ﾎﾞﾀﾝ時のみ
			if(e.Button == MouseButtons.Left) {
				ListView			ctlLV = sender as ListView;
				ListViewHitTestInfo	hitInfo = this.listViewClassBreaks.HitTest(e.X, e.Y);
				
				// 分類の値範囲をｸﾘｯｸした場合
				if(hitInfo.Item != null && (hitInfo.SubItem != null && this.HitValueCol(ctlLV, e.X))) {
					// 閾値編集ﾎﾞｯｸｽを表示
					this.ShowCBEditTextBox(ctlLV);
				}
				else if(this.panel_LVEdit.Visible) {
					this.panel_LVEdit.Visible = false;
				}
			}
		}
		
		/// <summary>
		/// クリック地点が「範囲」列かどうかをチェックします
		/// </summary>
		/// <param name="ParentListView">対象リストビュー</param>
		/// <param name="XPosition">X位置</param>
		/// <returns>OK / NG</returns>
		private bool HitValueCol(ListView ParentListView, int XPosition) {
			bool	blnRet = false;
			
			int		intX = 0;
			if(ParentListView != null) {
				foreach(ColumnHeader colTemp in ParentListView.Columns) {
					// 微妙な境界では反応させない
					if(XPosition > intX && (intX + colTemp.Width) > XPosition) {
						blnRet = colTemp.Index.Equals(this.CB_EDITABLE_COLUMN_ID);
						break;
					}
					// 微妙な境界
					else if(XPosition <= intX) {
						break;
					}
					
					// 開始位置を更新
					intX += colTemp.Width;
				}
			}
			
			// 返却
			return blnRet;
		}

		/// <summary>
		/// 分類表示リストビュー KEYUP EVENT
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_KeyUp(object sender, KeyEventArgs e) {
			ListView	ctlLV = sender as ListView;
			
			// ﾘｽﾄの状態を確認 (ｼﾝﾎﾞﾙが選択されている時)
			if(ctlLV.SelectedIndices.Count > 0 && ctlLV.FocusedItem != null) {
				// 押下ｷｰを確認
				if(e.KeyCode == Keys.F2) {
					// 閾値編集ﾎﾞｯｸｽを表示
					this.ShowCBEditTextBox(ctlLV);
				}
			}
		}
		
		/// <summary>
		/// 閾値編集ボックスを表示します
		/// </summary>
		private void ShowCBEditTextBox(ListView EditListView) {
			// 選択状態をﾁｪｯｸ
			if(EditListView.SelectedIndices.Count > 0) {
				ListViewItem	liSel = EditListView.SelectedItems[0];
				
				// 横位置を算出
				int	intX = 0;
				int intCol = 0;
				for(; intCol < this.CB_EDITABLE_COLUMN_ID; intCol++) {
					intX += EditListView.Columns[intCol].Width;
				}
				int intW = liSel.SubItems[this.CB_EDITABLE_COLUMN_ID].Bounds.Width;
				int intH = liSel.SubItems[this.CB_EDITABLE_COLUMN_ID].Bounds.Height - 1;

				// 表示位置を設定
				this.panel_LVEdit.Parent = EditListView;
				this.panel_LVEdit.Size = new Size(intW, intH);
				this.panel_LVEdit.Left = intX;
				this.panel_LVEdit.Top = liSel.Position.Y;
				
				this.panel_LVEdit.BorderStyle = BorderStyle.None;
				this.textBox_LVEdit.Multiline = false;
				this.panel_LVEdit.Tag = liSel.Index;		// ﾘｽﾄ行を記録
				
				System.Drawing.Point	pntLoc = new System.Drawing.Point();
				pntLoc.X = 5;
				pntLoc.Y = (this.panel_LVEdit.Height - this.textBox_LVEdit.Height) / 2;
				this.textBox_LVEdit.Location = pntLoc;
				this.textBox_LVEdit.Width = this.panel_LVEdit.ClientSize.Width - pntLoc.X * 2;
				
				// 分類値の表示設定
				double	dblBreakValue = m_work_ClassBreaksRenderer.get_Break(liSel.Index);
				string	strBreakValue;
				if(this.IsIntegerField(m_work_ClassBreaksRenderer.Field)) {
					strBreakValue = dblBreakValue.ToString();
				}
				else {
					strBreakValue = dblBreakValue.ToString("0.000000");
				}
				
				// 表示設定
				this.textBox_LVEdit.Text = strBreakValue;
				this.textBox_LVEdit.SelectAll();
				
				// 表示
				if(!this.panel_LVEdit.Visible) {
					this.panel_LVEdit.Visible = true;
					this.textBox_LVEdit.Focus();
				}
			}
		}

		/// <summary>
		/// 閾値編集ボックス LEAVE EVENT
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LVEditBox_Leave(object sender, EventArgs e) {
			this.panel_LVEdit.Visible = false;

			// 数値分類の閾値を調整
			this.AdjustCBValue();
		}

		/// <summary>
		/// 閾値編集ボックス KEYUP EVENT
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LVEditBox_KeyUp(object sender, KeyEventArgs e) {
			int	intUD = 0;		// ﾌｫｰｶｽ移動ｽｲｯﾁ
			
			// 入力文字判定
			if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab) {
				intUD = e.Shift ? 1 : -1;
			}
			else if(e.KeyCode == Keys.Up) {
				intUD = 1;
			}
			else if(e.KeyCode == Keys.Down) {
				intUD = -1;
			}
			
			// ﾌｫｰｶｽ移動
			if(intUD != 0) {
				// 数値分類の閾値を調整
				this.AdjustCBValue();
				
				// 選択位置を取得
				int	intSelID = this.listViewClassBreaks.SelectedIndices[0];

				// 上に移動
				if(intUD > 0 && intSelID > 0) {
					// 現在の選択を解除
					this.listViewClassBreaks.Items[intSelID].Selected = false;
					// 選択を前に移動
					this.listViewClassBreaks.Items[intSelID - 1].Selected = true;
					// 編集ﾎﾞｯｸｽを移動
					this.ShowCBEditTextBox(this.listViewClassBreaks);
				}
				// 下に移動
				else if(intUD < 0 && intSelID < this.listViewClassBreaks.Items.Count - 1) {
					// 現在の選択を解除
					this.listViewClassBreaks.Items[intSelID].Selected = false;
					// 選択を次に移動
					this.listViewClassBreaks.Items[intSelID + 1].Selected = true;
					// 編集ﾎﾞｯｸｽを移動
					this.ShowCBEditTextBox(this.listViewClassBreaks);
				}
				else {
					this.panel_LVEdit.Visible = false;
					this.listViewClassBreaks.Focus();
				}
			}
		}
		
		/// <summary>
		/// クラスブレイクを調整します
		/// </summary>
		/// <returns></returns>
		private bool AdjustCBValue() {
			bool	blnRet = false;
			
			if(this.listViewClassBreaks.SelectedIndices.Count > 0) {
				// 設定対象ｲﾝﾃﾞｯｸｽを取得
				int		intID = Convert.ToInt32(this.panel_LVEdit.Tag);
				double	dblBreak;
				
				// 設定値を取得
				if(double.TryParse(this.textBox_LVEdit.Text, out dblBreak)) {
                    // 等間隔分類で、選択ﾌｨｰﾙﾄﾞが整数型の時は、値を調整
                    if(this.comboBoxBunruiSyuhou.SelectedIndex == Convert.ToInt32(CLASSBREAK_INDEX.CLASSBREAKS_EQUAL_INTERVAL)
						&& this.IsIntegerField(this.m_work_ClassBreaksRenderer.Field)) {
						
						dblBreak = Math.Round(dblBreak, 0);
                    }
                    else {
						// 小数点第6位に統一
						dblBreak = Math.Round(dblBreak, 6);
                    }
                    
                    // 値(小数6桁まで)の変更ﾁｪｯｸ
                    if(Math.Round(CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][intID + 1], 6) != dblBreak) {
						
						// 分類上限値を更新
						CLASSBREAKS_VALUES[this.comboBoxBunruiSyuhou.SelectedIndex][intID + 1] = dblBreak;
						this.m_work_ClassBreaksRenderer.set_Break(intID, dblBreak);
						
						// ﾗﾍﾞﾙを更新
						string	strLabel = this.CreateCBLabel(this.m_work_ClassBreaksRenderer, intID);
						this.m_work_ClassBreaksRenderer.set_Label(intID, strLabel);
						
						// ﾘｽﾄ更新
						this.listViewClassBreaks.SelectedItems[0].SubItems[1].Text = strLabel;
						this.listViewClassBreaks.SelectedItems[0].SubItems[2].Text = strLabel;
						
						// 次分類の更新
						if(intID < this.m_work_ClassBreaksRenderer.BreakCount - 1) {
							// 分類下限値を更新
							IClassBreaksUIProperties	agCBProps = (IClassBreaksUIProperties)this.m_work_ClassBreaksRenderer;
							double	dblNextMin = Convert.ToDouble(this.CalcCBMinValue(dblBreak));
							agCBProps.set_LowBreak(intID + 1, dblNextMin);

							// ﾗﾍﾞﾙを再作成
							strLabel = this.CreateCBLabel(this.m_work_ClassBreaksRenderer, intID + 1);
							this.m_work_ClassBreaksRenderer.set_Label(intID + 1, strLabel);
							
							// ﾘｽﾄ更新
							this.listViewClassBreaks.Items[intID + 1].SubItems[1].Text = strLabel;
							this.listViewClassBreaks.Items[intID + 1].SubItems[2].Text = strLabel;

							// ｼﾝﾎﾞﾙ･ｱｯﾌﾟﾃﾞｰﾄ制御
							m_do_renderer_update[this.tabControl1.SelectedIndex] = true;
							m_do_save[this.tabControl1.SelectedIndex] = false;
						}
						
						// 編集ｸﾘｱ
						this.textBox_LVEdit.Text = "";
					}
				}
			}
			
			// 返却
			return blnRet;
		}

		/// <summary>
		/// FORM PROCESSCMDKEY EVENT
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData) {
			// 閾値編集時は、ﾘﾀｰﾝ･ｷｰとﾀﾌﾞ･ｷｰの押下を横取りする
			if(this.panel_LVEdit.Visible) {
				// ※ｷｰﾎﾞｰﾄﾞによって+や-が、OemKeyの場合がある様子の為KeyValueで判定する
				Keys	keyMod = keyData & Keys.Modifiers;		// 修飾ｷｰを取得
				Keys	keyCode = keyData & Keys.KeyCode;		// 押下ｷｰを取得
				//Keys	keyEnt = keyCode & Keys.Enter;
				//Keys	keyTab = keyCode & Keys.Tab;
				int		intKey = keyCode.GetHashCode();
				if(intKey == Keys.Enter.GetHashCode() || intKey == Keys.Tab.GetHashCode()) {
					return true;
				}
			}
			// NumericUpDownにﾌｫｰｶｽがある場合は、Enter入力を却下する ※ OKﾎﾞﾀﾝ･ｸﾘｯｸ ｲﾍﾞﾝﾄが先に走ってしまうのを防ぐため。
			else if(this.ActiveControl is NumericUpDown) {
				if((keyData & Keys.KeyCode) == Keys.Enter
					&& (keyData & (Keys.Alt | Keys.Control)) == Keys.None) {
					this.ProcessTabKey((keyData & Keys.Shift) != Keys.Shift);
					return true;
				}
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// シンボル画像を作成します
		/// </summary>
		/// <param name="Symbol">シンボル</param>
		/// <param name="Width">画像の幅</param>
		/// <param name="Height">画像の高さ</param>
		/// <param name="TransparentColor">透過設定色</param>
		/// <returns>シンボル画像</returns>
		private Bitmap CreateSymbolImage(ISymbol Symbol, int Width, int Height, Color TransparentColor) {
			return Common.WindowsAPI45.SymbolToBitmap(
										Symbol, new Size(Width, Height), this.graphics, 
										ColorTranslator.ToWin32(TransparentColor));
		}
		private Bitmap CreateSymbolImage2(ISymbol Symbol, int Width, int Height, Color TransparentColor) {
			return new Bitmap(0,0);
		}

		/// <summary>
		/// シンボルの塗りを変更できるかどうかチェックします
		/// </summary>
		/// <param name="TargetSymbol"></param>
		/// <param name="IsOutline"></param>
		/// <returns></returns>
		static private bool CanChangeColor(ISymbol TargetSymbol, bool IsOutline) {
			bool			IsLock = true;
			int				intLayerNum = 0;
			ILayerColorLock	agLCLock = null;

			// 共通ｲﾝﾀｰﾌｪｰｽの取得 = ﾛｯｸ
			if(TargetSymbol is IMarkerSymbol) {
				var agMMSym = TargetSymbol as IMultiLayerMarkerSymbol;
				if(agMMSym != null) {
					agLCLock = agMMSym as ILayerColorLock;
					intLayerNum = agMMSym.LayerCount;
				}
				else {
					IsLock = false;
				}
			}
			else if(TargetSymbol is ILineSymbol) {
				var agMLSym = TargetSymbol as IMultiLayerLineSymbol;
				if(agMLSym != null) {
					agLCLock = agMLSym as ILayerColorLock;
					intLayerNum = agMLSym.LayerCount;
				}
				else {
					IsLock = false;
				}
			}
			else if(TargetSymbol is IFillSymbol) {
				var	agMFSym = TargetSymbol as IMultiLayerFillSymbol;
				if(agMFSym != null) {
					// ｱｳﾄﾗｲﾝをﾁｪｯｸ
					if(IsOutline) {
						IFillSymbol				agFSym;
						IMultiLayerLineSymbol	agMLSym;
						foreach(int intCnt in Enumerable.Range(0, agMFSym.LayerCount)) {
							agFSym = agMFSym.get_Layer(intCnt);
							agMLSym = agFSym.Outline as IMultiLayerLineSymbol;
							agLCLock = agMLSym as ILayerColorLock;
							if(agLCLock != null) {
								foreach(int intElm in Enumerable.Range(0, agMLSym.LayerCount)) {
									IsLock &= agLCLock.get_LayerColorLock(intElm);
									if(!IsLock) {
										break;
									}
								}
								if(!IsLock) {
									break;
								}
							}
							else {
								IsLock = false;
								break;
							}
						}
						agLCLock = null;
					}
					// 塗りをﾁｪｯｸ
					else {
						agLCLock = agMFSym as ILayerColorLock;
						intLayerNum = agMFSym.LayerCount;
					}
				}
				else {
					IsLock = false;
				}
			}

			// ﾛｯｸ状態を調査
			if(agLCLock != null && intLayerNum > 0) {
				foreach(int intCnt in Enumerable.Range(0, intLayerNum)) {
					IsLock &= agLCLock.get_LayerColorLock(intCnt);
					if(!IsLock) {
						break;
					}
				}
			}

			return !IsLock;
		}

		/// <summary>
		/// ランダム・カラーを作成します
		/// </summary>
		/// <returns></returns>
		private IRgbColor CreateRandomColor() {
			IRgbColor	agCol = new RgbColorClass();

			agCol.Red = this._random.Next(256);
			agCol.Green = this._random.Next(256);
			agCol.Blue = this._random.Next(256);

			return agCol;
		}

		private void listViewUniqueValue_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			// ↓検証中
			//ListView	ctlLV = sender as ListView;
			//if(ctlLV.Name != "") {
			//	this._selSymbol.Index = e.ItemIndex;
			//	this._selSymbol.IsSelect = e.IsSelected;
			//}
		}



    }
}
