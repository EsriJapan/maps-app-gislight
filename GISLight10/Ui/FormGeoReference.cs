using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class FormGeoReference : Form
    {
		// 表示規定 (小数点以下6桁)
		private const string		DISPLAY_FORMAT_COORD = "0.000000";
		private const string		DISPLAY_FORMAT_SUBST = "0.00000";
		
        private Ui.MainForm			mainFrm;
        private IMapControl3		m_mapControl;
		
		//private IToolbarMenu		_agToolManu;
		private IToolbarControl2	_agToolCtl;
		
		// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ追加ﾂｰﾙ
		private EngineCommand.GeoRef_AddControlPointTool	_agGRTool;
		
		// ﾌｫｰﾑの高さ調整
		private int[]				_intHeights = new int[] {0, 57};

		// ｶﾚﾝﾄ･ﾚｲﾔｰ
		private IRasterLayer		_agRLayer;
		
		// ｽｸﾘｰﾝ･ｷｬｯｼｭID
		private short				_shoScrID;
		
		// 指定位置ｺﾝﾄﾛｰﾙﾎﾟｲﾝﾄ群
		private IPointCollection4	_agCtlPointsO = null;

		// 元位置ｺﾝﾄﾛｰﾙﾎﾟｲﾝﾄ群
		private IPointCollection4	_agCtlPointsS = null;
		
		// ﾌｨｯﾄ情報
		private IPointCollection4	_agFitBefore = null;
		private IPointCollection4	_agFitAfter = null;
		
		// 編集ﾎﾞｯｸｽ制御
		private int						_intSelPosID = -1;
		private bool					_blnPosEditable = false;
		private Common.ListViewInputBox	_ctlEditBox = null;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mainFrm">メイン・フォーム</param>
        public FormGeoReference(Ui.MainForm MainForm) {
            InitializeComponent();

            this.mainFrm = MainForm;
            this.m_mapControl = MainForm.MapControl;
            
            // ﾗｽﾀｰ･ﾚｲﾔｰの一覧を作成
            Common.LayerManager	clsLM = new Common.LayerManager();
            List<IRasterLayer>	agRasterLayers = clsLM.GetRasterLayers(this.m_mapControl.Map);
            IGeoReference		agGeoRef;
            
            foreach(IRasterLayer agRLayer in agRasterLayers) {
				// ｼﾞｵﾘﾌｧﾚﾝｽ可能なﾗｽﾀｰを選択肢に追加
				agGeoRef = (IGeoReference)agRLayer;
				if(agGeoRef.CanGeoRef) {
					this.comboBoxLayers.Items.Add(new LayerComboItem(agRLayer));
				}
            }
            if(this.comboBoxLayers.Items.Count > 0) {
				this.comboBoxLayers.SelectedIndex = 0;
            }

			// 変換ﾘｽﾄを作成
			this.comboBox_Trans.Items.Add("1次多項式（アフィン）");
			this.comboBox_Trans.SelectedIndex = 0;
            
            // ﾂｰﾙ･ｺﾝﾄﾛｰﾙの設定
            this._agToolCtl = (IToolbarControl2)this.axToolbarControl1.Object;
            
			// 機能実行可否設定
			this.SetGeoRefFuncControl(false, false);
        }

		/// <summary>
		/// フォーム・ロード イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Load(object sender, EventArgs e) {
			// 通常ﾌｫｰﾑ時の高さを記録
			this._intHeights[0] = this.Height;
			
			// ﾂｰﾙ･ｺﾝﾄﾛｰﾙの初期化
			this._agToolCtl.SetBuddyControl(m_mapControl.Object);
			this._agToolCtl.ItemAppearance = esriControlsAppearance.esri3D;
			this._agToolCtl.AlignLeft = false;
			
			// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ追加ﾂｰﾙの準備
			this._agGRTool = new ESRIJapan.GISLight10.EngineCommand.GeoRef_AddControlPointTool(this);
			
			// ﾂｰﾙﾎﾞﾀﾝを設定
			this._agToolCtl.AddItem("esriControls.ControlsSelectTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
			this._agToolCtl.AddItem(this._agGRTool, 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
			
			//this._agToolManu.SetHook(this._agToolCtl);
			
			// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ削除ﾎﾞﾀﾝの制御
			this.ListView_SelectedIndexChanged(this.listView1, e);
			
			// ﾘｽﾄﾋﾞｭｰの設定
			this.listView1.CheckBoxes = false;

            // ｽｸﾘｰﾝ･ｷｬｯｼｭIDの追加
			IActiveView		agActView = (IActiveView)this.m_mapControl.Map;
			IScreenDisplay	agScrDisplay = agActView.ScreenDisplay;
            this._shoScrID = agScrDisplay.AddCache();
            
            // ｺﾝﾄﾛｰﾙﾎﾟｲﾝﾄの初期化
            this._agCtlPointsO = new MultipointClass();
            this._agCtlPointsS = new MultipointClass();
            
            // ﾋﾞｭｰ･ｲﾍﾞﾝﾄを設定
            IActiveViewEvents_Event	agActViewEvents = (IActiveViewEvents_Event)agActView;
            agActViewEvents.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler(ActiveViewEvents_AfterDraw);
            
            if(this.comboBoxLayers.Items.Count <= 0) {
				// 実行不可 (通常はGeoReferenceCommandにて事前判定、対象がない場合はﾌｫｰﾑを起動しない)
				this.statusStrip1.Enabled = false;
				this._agToolCtl.Enabled = false;
				this.button_OpenTextFile.Enabled = false;
				
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(Properties.Resources.FormGeoReference_WARNIG_InvalidRaster);
            }
		}

		/// <summary>
		/// フォームを閉じる イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Closing(object sender, FormClosingEventArgs e) {
			// 途中のﾘﾌｧﾚﾝｽ処理を解除
			if(this._agRLayer != null) {
				this.ResetRaster(this._agRLayer.Raster);

				// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを破棄
				this.listView1.Items.Clear();

				// ｽｸﾘｰﾝ･ｷｬｯｼｭを削除
				IActiveView		agActView = (IActiveView)this.m_mapControl.Map;
				IScreenDisplay	agScrDisplay = agActView.ScreenDisplay;
				agScrDisplay.RemoveCache(this._shoScrID);

				// 描画更新
				agActView.PartialRefresh(esriViewDrawPhase.esriViewAll, this._agRLayer.Raster, agActView.Extent);

				// COM解放
				Marshal.ReleaseComObject(this._agRLayer);
				this._agRLayer = null;

				// ﾋﾞｭｰ･ｲﾍﾞﾝﾄを解除
				IActiveViewEvents_Event	agActViewEvents = (IActiveViewEvents_Event)agActView;
				agActViewEvents.AfterDraw -= new IActiveViewEvents_AfterDrawEventHandler(ActiveViewEvents_AfterDraw);
			}
		}

		/// <summary>
		/// フォームを閉じた イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Closed(object sender, FormClosedEventArgs e) {
			// 描画の残骸をｸﾘｱ
			this._agGRTool.EndTool();
		}

#region 公開メソッド
		
		/// <summary>
		/// メインフォームをアクティブにします
		/// </summary>
		public void SetMapActive() {
			this.mainFrm.Select();
		}
		
		/// <summary>
		/// コントロール・ポイントをリストに追加します
		/// </summary>
		/// <param name="FromPointElement">元位置</param>
		/// <param name="ToPointElement">補正位置</param>
		/// <param name="IsExec">追加して補正を実行するかどうか</param>
		public void AddControlPointList(IPoint FromPoint, IPoint ToPoint, bool IsExec) {
			// 入力ﾁｪｯｸ
			if(ToPoint != null) {
				object	objM = Type.Missing;

				// 位置情報を保存
				this._agCtlPointsO.AddPoint(FromPoint, ref objM, ref objM);
				
				// 元位置を調整
				IPoint	agPntF;
				IPoint	agPntB;
				IPoint	agNewPnt;
				this.GetSourcePoint(FromPoint, out agPntF, out agPntB);
				
				agNewPnt = agPntB;
				
				// ﾘｽﾄ･ｱｲﾃﾑを生成
				ListViewItem	lviInfo = new ListViewItem(new string[] {
															(this.listView1.Items.Count + 1).ToString(), 
															agNewPnt.X.ToString(DISPLAY_FORMAT_COORD),
															agNewPnt.Y.ToString(DISPLAY_FORMAT_COORD),
															ToPoint.X.ToString(DISPLAY_FORMAT_COORD),
															ToPoint.Y.ToString(DISPLAY_FORMAT_COORD),
															""
															});
				
				this.listView1.Items.Add(lviInfo);
				
				// 採用ｺﾝﾄﾛｰﾙﾎﾟｲﾝﾄの登録
				this._agCtlPointsS.AddPoint(agNewPnt, ref objM, ref objM);
			}
			
			// ﾗｽﾀｰ補正を実行
			if(IsExec) {
				this.ExecTrans(true);
			}
		}
		
		/// <summary>
		/// 位置指定ツールとの架け橋
		/// </summary>
		/// <param name="FromPointElement">元位置要素</param>
		/// <param name="ToPointElement">補正位置要素</param>
		public void AddControlPointList(IElement FromPointElement, IElement ToPointElement) {
			// ﾎﾟｲﾝﾄの抜き出し
			IPoint	agFrom = (IPoint)FromPointElement.Geometry;
			IPoint	agTo = (IPoint)ToPointElement.Geometry;
			
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			this.AddControlPointList(agFrom, agTo, true);
		}
		
		/// <summary>
		/// アフィン変換を実行します
		/// </summary>
		public void ExecTrans(bool IsFw) {
			// 補正ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを取得
			//IPointCollection4	agPnts_S = this.GetControlPoints(false);	// 元のｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを取得
			IPointCollection4	agPnts_S;
			
			if(IsFw) {	// 追加時
				agPnts_S = this._agCtlPointsS;	// 元のｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを取得
			}
			else {		// 削除時
				agPnts_S = this.GetControlPoints(false);
			}
			IPointCollection4	agPnts_T = this.GetControlPoints(true);		// 補正ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを取得
			IPoint				agPnt_T;

			IActiveView			agActView = (IActiveView)this.m_mapControl.Map;

			// ﾗｽﾀのﾌﾟﾛﾊﾟﾃｨを取得
			IRaster2			agR2 = (IRaster2)this._agRLayer.Raster;
			IRasterProps		agRProps = (IRasterProps)agR2;
			IEnvelope			agREnv = agRProps.Extent;
			IRelationalOperator	agRO = (IRelationalOperator)agREnv;
			
			// 残差表示をｸﾘｱ
			this.ClearSubstructValue();
			
			// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ･ｾｯﾄの1つ目は、ｼﾌﾄ。
			if(agPnts_T.PointCount < 2) {
				if(!IsFw) {
					// 既存ﾗｽﾀ処理をﾘｾｯﾄ
					this.ResetRaster(this._agRLayer.Raster);

					// 表示範囲ﾌｨｯﾄ
					if(this._agFitAfter != null) {
						this.GeoReferenceRaster(this._agRLayer.Raster, this._agFitBefore, this._agFitAfter, 1);
						//this.ReFitExtentRaster(this._agRLayer.Raster);
					}
				}
				
				// 元位置を取得
				IPoint	agPnt_S = this._agCtlPointsO.get_Point(0);
				// 補正位置を取得
				agPnt_T = agPnts_T.get_Point(0);

				// 移動量を算出
				double	dblMX = agPnt_T.X - agPnt_S.X;
				double	dblMY = agPnt_T.Y - agPnt_S.Y;

				// ｼﾌﾄ補正
				this.ShiftRaster(this._agRLayer.Raster, dblMX, dblMY);
			}
			
			// ｱｼﾞｬｽﾄ補正
			else if(agPnts_T.PointCount < 3) {
				this.AdjustRaster(this._agRLayer.Raster, agPnts_S, agPnts_T);
			}

			// 多項式変換
			if(agPnts_T.PointCount > 2) {
				// 残差算出
				this.LeastSquareFitRaster(this._agRLayer.Raster, agPnts_S, agPnts_T);
		
				// 多項式変換によるｼﾞｵﾘﾌｧﾚﾝｽの実行
				this.GeoReferenceRaster(this._agRLayer.Raster, agPnts_S, agPnts_T, 1);
			}

			// 描画更新
			agActView.PartialRefresh(esriViewDrawPhase.esriViewAll, this._agRLayer.Raster, agActView.Extent);
			
			// 状態変更
			this.SetGeoRefFuncControl(true, true);
		}
#endregion

        /// <summary>
        /// アクティブ・ビュー イベント
        /// </summary>
        /// <param name="Display">対象ディスプレイ</param>
        /// <param name="phase">フェーズ</param>
        void ActiveViewEvents_AfterDraw(ESRI.ArcGIS.Display.IDisplay Display, esriViewDrawPhase phase) {
			// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを表示
            if(this.listView1.Items.Count > 0) {
				this.DrawControlPoint(Display);
			}
        }

		/// <summary>
		/// コントロール・ポイントを描画します
		/// </summary>
		/// <param name="ScrDisplay"></param>
		private void DrawControlPoint(IDisplay ScrDisplay) {
			IScreenDisplay	agScrDisplay = (IScreenDisplay)ScrDisplay;

//			bool	blnDirty = agScrDisplay.IsCacheDirty(this._shoScrID);
//			if(blnDirty) {
				//agScrDisplay.RemoveCache(this._shoScrID);
				//this._shoScrID = agScrDisplay.AddCache();

				// 描画開始
				//agScrDisplay.StartDrawing(agScrDisplay.hDC, this._shoScrID);
				agScrDisplay.StartDrawing(agScrDisplay.hDC, (Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);

				// 色設定
				IRgbColor				agRGBColor = new RgbColorClass() {
					Red = 255,
					Green = 0,
					Blue = 0
				};

				// ｼﾝﾎﾞﾙ設定
				ISimpleMarkerSymbol		agPntSym = new SimpleMarkerSymbolClass() {
					Color = agRGBColor,
					Size = 14,
					Style = esriSimpleMarkerStyle.esriSMSCross
				};

				agScrDisplay.SetSymbol((ISymbol)agPntSym);

				// 表示
				IPointCollection4	agFixedControlPoints = this.GetControlPoints(true);
				for(int intCnt=0; intCnt < agFixedControlPoints.PointCount; intCnt++) {
					// 全てのｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを描画
					agScrDisplay.DrawPoint(agFixedControlPoints.get_Point(intCnt));
				}
				agScrDisplay.FinishDrawing();

//			}
//			else {
//				tagRECT	agRect = new tagRECT();
				//agScrDisplay.DrawCache(agScrDisplay.hDC, this._shoScrID,  ref agRect, ref agRect);
//			}
		}

		/// <summary>
		/// ラスタの元位置に対する指定位置座標を取得します
		/// </summary>
		/// <param name="UserPoint">追加される元座標</param>
		/// <returns>ラスタへの指定を示す座標</returns>
		private void GetSourcePoint(IPoint UserPoint, out IPoint FwPoint, out IPoint BwPoint) {
			// 指定元座標群を取得
			IPointCollection4	agPnts_S = this.GetControlPoints(false);
			
			// 作業ﾎﾟｲﾝﾄに追加
			this.AddPointCollection(ref agPnts_S, this.CreatePointCollection(new IPoint[] {UserPoint}, new MultipointClass()));
			#if DEBUG
			Debug.WriteLine(string.Format("IN: {0}, {1}", UserPoint.X, UserPoint.Y));
			#endif

			// 戻り値初期化
			FwPoint = null;
			BwPoint = null;
			IPoint	agPntW;
			
			// 補正前ﾗｽﾀｰに対する指定位置を取得
			IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
			IPointCollection		agPnts_F = agRProc.PointsTransform(agPnts_S, true, this._agRLayer.Raster);
			IPointCollection		agPnts_B = agRProc.PointsTransform(agPnts_S, false, this._agRLayer.Raster);
				
			#if DEBUG
			// 確認ｺｰﾄﾞ (F)
			for(int intCnt=0; intCnt < agPnts_F.PointCount; intCnt++) {
			    agPntW = agPnts_F.get_Point(intCnt);
			    Debug.WriteLine(string.Format(" F: {0}, {1}", agPntW.X, agPntW.Y));
			}
			#endif
			FwPoint = agPnts_F.get_Point(agPnts_F.PointCount - 1);

			#if DEBUG
			// 確認ｺｰﾄﾞ (B)
			for(int intCnt=0; intCnt < agPnts_B.PointCount; intCnt++) {
			    agPntW = agPnts_B.get_Point(intCnt);
			    Debug.WriteLine(string.Format(" B: {0}, {1}", agPntW.X, agPntW.Y));
			}
			#endif
			
			BwPoint = agPnts_B.get_Point(agPnts_B.PointCount - 1);
		}

		private IPointCollection GetBwPoints() {
			// 指定元座標群を取得
			IPointCollection4	agPnts_S = this.GetControlPoints(false);
			//IPointCollection4	agPnts_S = this._agCtlPointsO;
			
			// 補正前ﾗｽﾀｰに対する指定位置を取得
			IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
			IPointCollection		agPnts_W = agRProc.PointsTransform(agPnts_S, false, this._agRLayer.Raster);
				
			// 確認ｺｰﾄﾞ
			IPoint	agPntRet;
			string	strTempT = "";
			for(int intCnt=0; intCnt < agPnts_W.PointCount; intCnt++) {
			    agPntRet = agPnts_W.get_Point(intCnt);
			    strTempT += agPntRet.X.ToString() + ", " + agPntRet.Y.ToString() + Environment.NewLine;
			}
			
			agPntRet = agPnts_W.get_Point(agPnts_W.PointCount - 1);
		
			// 返却
			return agPnts_W;
		}
		
#region コントロール・イベント
		/// <summary>
		/// 「閉じる」ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Close_Click(object sender, EventArgs e) {
			//if(this.comboBoxLayers.Items.Count <= 0) {
			//    // ﾒﾆｭｰﾎﾞﾀﾝが非活性のまま状態を回避
			//    IActiveView		agActView = (IActiveView)this.m_mapControl.Map;
			//    agActView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, agActView.Extent);
			//}
			
			this.Close();
		}

		/// <summary>
		/// ツールストリップメニュー クリック イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolStripMenu_Click(object sender, EventArgs e) {
			ToolStripMenuItem	ctlTemp = sender as ToolStripMenuItem;
			
			// ｱｲﾃﾑ識別
			string				strTag = ctlTemp.Tag.ToString();
			bool				blnUpdate = true;
			bool				blnCanSave = true;

			switch(strTag) {
			case "GG":		// ﾜｰﾙﾄﾞﾌｧｲﾙ更新
				this.UpdateRaster();
				blnUpdate = false;
				blnCanSave = true;
				break;
			case "GY":		// ﾚｸﾃｨﾌｧｲ
				if(this.RectifyRaster(this._agRLayer.Raster)) {
					// 全削除
					this.ResetRasterRef(this._agRLayer.Raster);
					blnUpdate = false;
					blnCanSave = false;
					
					this.Close();
				}
				break;
			case "GF":		// ﾌｨｯﾄ
				this.FitExtentForRaster(this._agRLayer.Raster);
				blnUpdate = true;
				blnCanSave = true;
				break;
			case "GE":		// ﾘｾｯﾄ
				// 全削除
				this.ResetRasterRef(this._agRLayer.Raster);
				blnUpdate = false;
				blnCanSave = false;
				break;
			}

            // 機能実行可否設定
            this.SetGeoRefFuncControl(blnUpdate, blnCanSave);
		}

		/// <summary>
		/// コントロールポイント選択変更 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_SelectedIndexChanged(object sender, EventArgs e) {
			ListView	ctlLV = sender as ListView;
			
			// ｺﾝﾄﾛｰﾙﾎﾟｲﾝﾄ 削除ﾎﾞﾀﾝの制御
			this.button_Del.Enabled = ctlLV.SelectedIndices.Count > 0;
			
			if(ctlLV.SelectedIndices.Count > 0) {
				if(!this._intSelPosID.Equals(ctlLV.SelectedIndices[0])) {
					this._intSelPosID = ctlLV.SelectedIndices[0];
					this._blnPosEditable = false;
				}
				else {
					this._blnPosEditable = true;
				}
			}
		}

		/// <summary>
		/// コントロールポイント マウスUp イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_MouseUp(object sender, MouseEventArgs e) {
			// 左ﾎﾞﾀﾝ時のみ
			if(e.Button == MouseButtons.Left) {
				// ｸﾘｯｸ位置を特定
				ListView			ctlLV = sender as ListView;
				ListViewHitTestInfo	hitInfo = ctlLV.HitTest(e.X, e.Y);
				
				if(hitInfo.Item != null && hitInfo.SubItem != null) {
					// 既に選択されている場合に、編集に移行 (ArcMapと同じ動き)
					if(this._blnPosEditable) {
						// 編集可能列か判定
						int	intColID = this.GetHitColID(ctlLV, e.X);
						if(intColID >= 1 && intColID <= 4) {
							// 編集ﾎﾞｯｸｽを表示
							_ctlEditBox = new Common.ListViewInputBox(ctlLV, hitInfo.Item, intColID);
							_ctlEditBox.FinishInput += new Common.ListViewInputBox.InputEventHandler(ListViewInputBox_FinishInput);
							_ctlEditBox.Show();
						}
					}
					else {
						this._blnPosEditable = true;
					}
				}
			}
		}

		/// <summary>
		/// クリック地点の列IDを取得します
		/// </summary>
		/// <param name="ParentListView">対象リストビュー</param>
		/// <param name="XPosition">X位置</param>
		/// <param name="ColumnID">編集可能列ID</param>
		/// <returns>OK / NG</returns>
		private int GetHitColID(ListView ParentListView, int XPosition) {
			int	intRet = -1;
			
			int	intX = 0;
			if(ParentListView != null) {
				foreach(ColumnHeader colTemp in ParentListView.Columns) {
					// ｸﾘｯｸ位置の列を特定
					if(XPosition > intX && (intX + colTemp.Width) > XPosition) {
						// 該当する列IDを返却
						intRet = colTemp.Index;
						break;
					}
					// 微妙な境界では反応させない
					else if(XPosition == intX) {
						break;
					}
					
					// 判定開始位置を更新
					intX += colTemp.Width;
				}
			}
			
			// 返却
			return intRet;
		}
		
		/// <summary>
		/// 座標値の入力完了 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListViewInputBox_FinishInput(object sender, Common.ListViewInputBox.InputEventArgs e) {
			double	dblNewCoord;
			
			// 入力ﾁｪｯｸ
			if(double.TryParse(e.NewValue, out dblNewCoord) && !e.OldValue.Equals(e.NewValue)) {
				// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを更新
				this.listView1.Items[e.ListItemIndex].SubItems[e.SubItemIndex].Text = dblNewCoord.ToString(DISPLAY_FORMAT_COORD);
				
				// 元位置を更新した場合は、保存ﾎﾟｲﾝﾄを更新する
				if(e.SubItemIndex < 3) {
					IPoint	agPntS = this._agCtlPointsS.get_Point(e.ListItemIndex);

					// 座標更新
					if(e.SubItemIndex % 2 == 0) {
						agPntS.Y = dblNewCoord;
					}
					else {
						agPntS.X = dblNewCoord;
					}
					this._agCtlPointsS.UpdatePoint(e.ListItemIndex, agPntS);

					if(e.ListItemIndex == 0) {
						// ﾘﾌｧﾚﾝｽ･ﾘｾｯﾄ
						this.ResetRaster(this._agRLayer.Raster);
						// 初回配置を更新
						this._agCtlPointsO.UpdatePoint(0, agPntS);
					}
				}

				#if DEBUG
				// 確認ｺｰﾄﾞ (B)
				IPoint	agPntW;
				for(int intCnt=0; intCnt < this._agCtlPointsO.PointCount; intCnt++) {
					agPntW = this._agCtlPointsO.get_Point(intCnt);
					Debug.WriteLine(string.Format("O[{0}] : {1}, {2}", intCnt, agPntW.X, agPntW.Y));
				}
				for(int intCnt=0; intCnt < this._agCtlPointsS.PointCount; intCnt++) {
					agPntW = this._agCtlPointsS.get_Point(intCnt);
					Debug.WriteLine(string.Format("S[{0}] : {1}, {2}", intCnt, agPntW.X, agPntW.Y));
				}
				#endif
				
				// 補正処理を更新
				this.ExecTrans(true);
			}
			// 編集可能制御(一息置く)
			this._blnPosEditable = false;
		}

		/// <summary>
		/// ラスタレイヤー変更 イベント　※同じものが選択されても発生する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RasterLayer_SelectedIndexChanged(object sender, EventArgs e) {
			// 選択ﾚｲﾔｰを取得
			IRasterLayer	agRLayer = (IRasterLayer)((LayerComboItem)this.comboBoxLayers.SelectedItem).Layer;

			// 変更ﾁｪｯｸ
			if(!agRLayer.Equals(this._agRLayer)) {
				// 途中のﾘﾌｧﾚﾝｽ処理を解除
				if(this._agRLayer != null) {
					// 全削除
					this.ResetRasterRef(this._agRLayer.Raster);

					// 各機能の制御
					this.SetGeoRefFuncControl(false, false);
				}
				
				// ｶﾚﾝﾄ･ﾗｽﾀを更新
				this._agRLayer = agRLayer;
				
				// ﾚｲﾔｰ名をﾂｰﾙﾁｯﾌﾟでも表示する
				this.toolTip1.SetToolTip(this.comboBoxLayers, this.comboBoxLayers.Text);
			}
		}

		/// <summary>
		/// 「展開／折り畳み」ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Expand_Click(object sender, EventArgs e) {
			Button	ctlBtn = sender as Button;
			
			// ﾌｫｰﾑの高さを制御
			if(this.Height == this._intHeights[0]) {
				ctlBtn.Image = global::ESRIJapan.GISLight10.Properties.Resources.TOC_Collapsed;
				this.toolTip1.SetToolTip(ctlBtn, "フォームを展開します");
				this.Height = this._intHeights[1];
				
				this.panel1.Enabled = false;
			}
			else {
				ctlBtn.Image = global::ESRIJapan.GISLight10.Properties.Resources.TOC_Expand;
				this.toolTip1.SetToolTip(ctlBtn, "フォームをたたみます");
				this.Height = this._intHeights[0];

				this.panel1.Enabled = true;
			}
		}

		/// <summary>
		/// ステータス・スプリットボタン クリック イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SplitButton_Click(object sender, EventArgs e) {
			ToolStripSplitButton	btnSplit = sender as ToolStripSplitButton;
			
			// 操作性向上
			if(!btnSplit.DropDownButtonPressed) {
				btnSplit.ShowDropDown();
			}
		}

		/// <summary>
		/// コントロールポイント削除ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Delete_Click(object sender, EventArgs e) {
			int	intID;
			for(int intCnt = this.listView1.SelectedIndices.Count - 1; intCnt >= 0; intCnt--) {
				intID = this.listView1.SelectedIndices[intCnt];
				// 保存位置を削除
				this._agCtlPointsO.RemovePoints(intID, 1);
				this._agCtlPointsS.RemovePoints(intID, 1);
				
				// ﾘｽﾄから削除
				this.listView1.Items.RemoveAt(this.listView1.SelectedIndices[intCnt]);
			}
			
			// ﾘﾝｸID再取得
			this.ReNumberCtlPointList();
			
			// 状態制御
			if(this.listView1.Items.Count <= 0) {
				// ﾘﾌｧﾚﾝｽ･ﾘｾｯﾄ
				this.ResetRaster(this._agRLayer.Raster);

				this.SetGeoRefFuncControl(false, false);

				// 描画更新
				IActiveView		agActView = (IActiveView)this.m_mapControl.Map;
				agActView.PartialRefresh(esriViewDrawPhase.esriViewAll, this._agRLayer.Raster, agActView.Extent);
			}
			else {
				// ﾘﾌｧﾚﾝｽ更新
				this.ExecTrans(false);
			}
		}

		/// <summary>
		/// 「リンクファイルを開く」ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenTextFile_Click(object sender, EventArgs e) {
			// 表示ﾀﾞｲｱﾛｸﾞの設定
			OpenFileDialog	oFileDialog = new OpenFileDialog();
			
			oFileDialog.CheckFileExists = true;
			//oFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			oFileDialog.Multiselect = false;
			oFileDialog.RestoreDirectory = true;
			oFileDialog.Filter = "リンクファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";
			
			oFileDialog.Title = "リンクファイルを開く";
			
			// ﾕｰｻﾞｰ選択
			if(oFileDialog.ShowDialog() == DialogResult.OK) {
				// 設定をﾘﾝｸﾃｰﾌﾞﾙに展開
				if(this.OpenCtlPoints(oFileDialog.FileName)) {
					// 状態変更
					this.SetGeoRefFuncControl(true, true);
				}
			}
		}

		/// <summary>
		/// 「リンクファイルの保存」ボタン クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveTextFile_Click(object sender, EventArgs e) {
			// 表示ﾀﾞｲｱﾛｸﾞの設定
			SaveFileDialog	sFileDialog = new SaveFileDialog();
			
			sFileDialog.AutoUpgradeEnabled = true;
			sFileDialog.CheckPathExists = true;
			sFileDialog.Filter = "リンクファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";
			sFileDialog.RestoreDirectory = true;
			sFileDialog.Title = "リンクファイルの保存";
			
			// ﾕｰﾞｻｰ指定
			if(sFileDialog.ShowDialog() == DialogResult.OK) {
				// ﾘﾝｸﾌｧｲﾙの保存
				this.SaveLinkFile(sFileDialog.FileName);
			}
		}
#endregion

#region 内部メソッド

		/// <summary>
		/// コントロールポイント・リストのリンクIDを振りなおします
		/// </summary>
		private void ReNumberCtlPointList() {
			int intID = 0;
			foreach(ListViewItem lviTemp in this.listView1.Items) {
				lviTemp.Text = (++intID).ToString();
			}
		}
		
		/// <summary>
		/// ラスタの表示サイズを地図の表示範囲にフィットさせます
		/// </summary>
		/// <param name="TargetRaster">ラスタ</param>
		private void FitExtentForRaster(IRaster TargetRaster) {
			if(TargetRaster != null) {
				// 既存の調整を破棄
				this.ResetRaster(this._agRLayer.Raster);
				
				// ﾗｽﾀｰの範囲を取得
				IRasterProps		agRProp = (IRasterProps)TargetRaster;
				
				// 画面の範囲を取得
				IActiveView			agActView = (IActiveView)this.m_mapControl.Map;
				
				IPoint				agCenter_S, agCenter_T;
				double				dblW, dblH;
				double				dblXMin, dblXMax, dblYMin, dblYMax;
				
				IPointCollection4	agPointsF, agPointsT;
				
				// 重心を取得
				agCenter_T = this.GetCentroid(agActView.Extent);

				// 辺長比を維持する
				double	dblWRatio = agActView.Extent.Width / agRProp.Extent.Width;
				double	dblHRatio = agActView.Extent.Height / agRProp.Extent.Height;
				
				// 適用範囲を作成
				if(dblHRatio >= dblWRatio) {
					// 幅で調整
					dblW = agActView.Extent.Width / 2;
					dblXMin = agCenter_T.X - dblW;
					dblXMax = agCenter_T.X + dblW;

					dblH = agRProp.Extent.Height * dblWRatio / 2;
					
					dblYMin = agCenter_T.Y + dblH;
					dblYMax = agCenter_T.Y - dblH;
				}
				else {
					// 高さで調整
					dblH = agActView.Extent.Height / 2;
					dblYMin = agCenter_T.Y + dblH;
					dblYMax = agCenter_T.Y - dblH;

					dblW = agRProp.Extent.Width * dblHRatio / 2;

					dblXMin = agCenter_T.X - dblW;
					dblXMax = agCenter_T.X + dblW;
				}
				
				agPointsT = this.CreatePointCollection(new IPoint[] {
                                            this.CreatePoint(dblXMin, dblYMax),
                                            this.CreatePoint(dblXMax, dblYMax),
                                            this.CreatePoint(dblXMax, dblYMin),
                                            this.CreatePoint(dblXMin, dblYMin)
                                        }, new MultipointClass());

				// 元ﾗｽﾀｰの調整点を作成 (各辺の中点)
				agCenter_S = this.GetCentroid(agRProp.Extent);
				dblW = agRProp.Extent.Width / 2;
				dblH = agRProp.Extent.Height / 2;
				
				dblXMin = agCenter_S.X - dblW;
				dblXMax = agCenter_S.X + dblW;
				dblYMin = agCenter_S.Y + dblH;
				dblYMax = agCenter_S.Y - dblH;
				
				agPointsF = this.CreatePointCollection(new IPoint[] {
											this.CreatePoint(dblXMin, dblYMax),
											this.CreatePoint(dblXMax, dblYMax),
											this.CreatePoint(dblXMax, dblYMin),
											this.CreatePoint(dblXMin, dblYMin)
										}, new MultipointClass());
				
				
				// ﾌｨｯﾄ実行
				this.GeoReferenceRaster(TargetRaster, agPointsF, agPointsT, 1);
				
				// ﾌｨｯﾄ情報を記録
				if(this._agFitBefore == null) {
					this._agFitBefore = agPointsF;
				}
				this._agFitAfter = agPointsT;

				// 描画更新
				agActView.PartialRefresh(esriViewDrawPhase.esriViewAll, TargetRaster, agActView.Extent);
			}
		}
		
		/// <summary>
		/// 「変換のリセット」を実行します
		/// </summary>
		/// <param name="TargetRaster">対象ラスター</param>
		private void ResetRasterRef(IRaster TargetRaster) {
			// 保存位置を削除
			if(this.listView1.Items.Count > 0) {
				// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄを全削除
				this.listView1.Items.Clear();
				// 位置ｸﾘｱ
				this._agCtlPointsO.RemovePoints(0, this._agCtlPointsO.PointCount);
				this._agCtlPointsS.RemovePoints(0, this._agCtlPointsS.PointCount);
			}

			// ﾌｨｯﾄ情報をｸﾘｱ
			if(this._agFitBefore != null) {
				this._agFitBefore.RemovePoints(0, this._agFitBefore.PointCount);
				this._agFitAfter.RemovePoints(0, this._agFitAfter.PointCount);
				
				this._agFitBefore = null;
				this._agFitAfter = null;
			}

			// ﾘｾｯﾄ
			this.ResetRaster(this._agRLayer.Raster);

			// 描画更新
			IActiveView		agActView = (IActiveView)this.m_mapControl.Map;
			agActView.PartialRefresh(esriViewDrawPhase.esriViewAll, TargetRaster, agActView.Extent);
		}
		
		/// <summary>
		/// 多項式変換によるジオリファレンスを実行します
		/// </summary>
		/// <param name="TargetRaster">ラスタ</param>
		/// <param name="SourcePoints">元座標群</param>
		/// <param name="TargetPoints">補正先座標群</param>
		/// <param name="OrderNum">多項式計算次元 (1～3)</param>
		private void GeoReferenceRaster(IRaster TargetRaster, IPointCollection SourcePoints, IPointCollection TargetPoints, int OrderNum) {
			// 入力ﾁｪｯｸ
			if(!(TargetRaster == null || SourcePoints == null || TargetPoints == null) && 
				SourcePoints.PointCount == TargetPoints.PointCount && SourcePoints.PointCount > 0) {
				
				// 計算次元の設定
				esriGeoTransTypeEnum	agTransType = esriGeoTransTypeEnum.esriGeoTransPolyOrder1;
				if(OrderNum == 2 && SourcePoints.PointCount >= 6) {
					// 2次計算 (ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ 6点以上)
					agTransType = esriGeoTransTypeEnum.esriGeoTransPolyOrder2;
				}
				else if(OrderNum >= 3 && SourcePoints.PointCount >= 10) {
					// 3次計算 (ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ 10点以上)
					agTransType = esriGeoTransTypeEnum.esriGeoTransPolyOrder3;
				}
				
				// 多項式変換
				IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
				agRProc.Warp(SourcePoints, TargetPoints, agTransType, TargetRaster);
			}
		}
		
		// ｱｼﾞｬｽﾄ
		private void AdjustRaster(IRaster TargetRaster, IPointCollection SourcePoints, IPointCollection TargetPoints) {
			// 入力ﾁｪｯｸ
			if(!(TargetRaster == null || SourcePoints == null || TargetPoints == null) && 
				SourcePoints.PointCount == TargetPoints.PointCount && SourcePoints.PointCount > 0) {
				
				// 多項式変換
				IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
				agRProc.TwoPointsAdjust(SourcePoints, TargetPoints, TargetRaster);
			}
		}
		
		/// <summary>
		/// 残差を算出します　※指定座標3点以上
		/// </summary>
		/// <param name="TargetRaster">対象ラスター</param>
		/// <param name="SourcePoints">元座標</param>
		/// <param name="TargetPoints">補正座標</param>
		private void LeastSquareFitRaster(IRaster TargetRaster, IPointCollection SourcePoints, IPointCollection TargetPoints) {
			// 入力ﾁｪｯｸ
			if(!(TargetRaster == null || SourcePoints == null || TargetPoints == null) && 
				SourcePoints.PointCount == TargetPoints.PointCount && SourcePoints.PointCount > 0) {
				
				// 残差を算出
				// dblRes[x, 0] = 　元座標X値, dblRes[x, 1] = 　元座標Y値
				// dblRes[x, 2] = 補正座標X値, dblRes[x, 3] = 補正座標Y値
				// dblRes[x, 4] = 　　残差X値, dblRes[x, 5] = 　　残差Y値, dblRes[x, 6] = 残差値
				IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
				object		objRes = agRProc.LeastSquareFit(SourcePoints, TargetPoints, esriGeoTransTypeEnum.esriGeoTransPolyOrder1, true, false);
				double[,]	dblRes = (double[,])objRes;
				
				int			intLen = dblRes.GetLength(0);
				IPoint		agPoint_S;
				for(int intCnt=0; intCnt < intLen; intCnt++) {
					// 残差位置を作成
					agPoint_S = this.CreatePoint(dblRes[intCnt, 2] + dblRes[intCnt, 4], dblRes[intCnt, 3] + dblRes[intCnt, 5]);
					
					// ﾘｽﾄ表示
					this.ShowSubstructValue(intCnt, dblRes[intCnt, 6], agPoint_S);
				}
			}
		}
	
		// 回転
		private void RotateRaster(IRaster TargetRaster, IPoint CenterAt, double Rotate) {
			IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
			agRProc.Rotate(null, Rotate, TargetRaster);
		}
		
		// ｼﾌﾄ
		private void ShiftRaster(IRaster TargetRaster, double DeltaX, double DeltaY) {
			IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
			agRProc.Shift(DeltaX, DeltaY, TargetRaster);
		}
		
		// 縮尺更新
		private void ReScaleRaster(IRaster TargetRaster, double DeltaX, double DeltaY) {
			IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
			agRProc.ReScale(DeltaX, DeltaY, TargetRaster);
		}
		
		// ﾘｾｯﾄ
		private void ResetRaster(IRaster TargetRaster) {
			IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
			agRProc.Reset(TargetRaster);
		}
		
		/// <summary>
		/// ワールドファイルを更新します
		/// </summary>
		private void UpdateRaster() {
			// GRIDﾃﾞｰﾀは更新対象外 (事前に判定)
			IRaster2	agRas2 = (IRaster2)this._agRLayer.Raster;
			IDataset	agDS = (IDataset)agRas2.RasterDataset;
			IWorkspace	agWS = agDS.Workspace;		

			if(!agDS.Name.Contains('.') && Directory.Exists(System.IO.Path.Combine(agWS.PathName, "info"))) {
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(
							"グリッド形式のラスターはワールドファイルを更新できません。" + Environment.NewLine
							+ "更新するには、レクティファイを使用してください。");
			}
			else {
				// 既存のﾜｰﾙﾄﾞﾌｧｲﾙの有無を確認
				string	strWldFile = this.GetWorldFilePath(agRas2);
				bool	blnExistWld = false;
				
				if(!string.IsNullOrEmpty(strWldFile)) {
					blnExistWld = File.Exists(strWldFile);
				}
				
				// ﾗｽﾀｰ設定ﾌｧｲﾙ (*.aux.xml)を保存します　※ﾜｰﾙﾄﾞ･ﾌｧｲﾙがない場合は、ﾜｰﾙﾄﾞﾌｧｲﾙも作成します
				IRasterGeometryProc3	agRProc = new RasterGeometryProcClass();
				try {
					agRProc.Register((IRaster)agRas2);
				}
				catch(COMException comex) {
                    Common.Logger.Error(comex.Message);
                    Common.Logger.Error(comex.StackTrace);
					ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(Properties.Resources.FormGeoReference_WARNING_UpdateWorldFile);
				}
				catch(Exception ex) {
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);
					ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(Properties.Resources.FormGeoReference_WARNING_UpdateWorldFile);
				}

				// ﾜｰﾙﾄﾞﾌｧｲﾙが既に存在する場合は、変更されない為、強制出力
				if(blnExistWld && agWS.IsDirectory()) {
					// 既存のﾜｰﾙﾄﾞﾌｧｲﾙを更新
					if(!CreateWorldFile(this._agRLayer.Raster)) {
						ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(Properties.Resources.FormGeoReference_WARNING_UpdateWorldFile);
					}
				}
			}
		}
		
		/// <summary>
		/// ワールドファイルのパスを取得します
		/// </summary>
		/// <param name="TargetRaster"></param>
		/// <returns></returns>
		private string GetWorldFilePath(IRaster2 TargetRaster) {
			string	strRet = null;
			
			// ﾜｰｸｽﾍﾟｰｽを取得 (ﾌｧｲﾙ･ﾗｽﾀｰを対象)
			IDataset	agDSet = (IDataset)TargetRaster.RasterDataset;
			IWorkspace	agWS = (IWorkspace)agDSet.Workspace;

			// ﾜｰﾙﾄﾞﾌｧｲﾙ拡張子を設定
			if(agWS.IsDirectory()) {
				string	strRPath = TargetRaster.RasterDataset.CompleteName;
				string	strFolder = System.IO.Path.GetDirectoryName(strRPath);
				string	strFileName = System.IO.Path.GetFileNameWithoutExtension(strRPath);
				string	strExt = System.IO.Path.GetExtension(strRPath);

				// 拡張子の長さをﾁｪｯｸ (3文字拡張子を対象)
				if(strExt.Length == 4) {
					strRet = (strExt.Substring(0, 2) + strExt.Substring(3, 1) + "w").ToLower();
				}
			}
			
			// COM解放
			Marshal.ReleaseComObject(agDSet);
			Marshal.ReleaseComObject(agWS);
			
			// 返却
			return strRet;
		}
		
		/// <summary>
		/// ワールドファイルを作成します
		/// </summary>
		/// <param name="TargetRaster"></param>
		/// <returns></returns>
		private bool CreateWorldFile(IRaster TargetRaster) {
			bool		blnRet = false;
			
			IRaster2	agRas2 = (IRaster2)TargetRaster;

			// ﾜｰﾙﾄﾞﾌｧｲﾙのﾊﾟｽを取得
			string		strWldFile = this.GetWorldFilePath(agRas2);

			// ﾜｰﾙﾄﾞﾌｧｲﾙ拡張子を設定
			if(!string.IsNullOrEmpty(strWldFile)) {
				IRasterProps	agRProps = (IRasterProps)agRas2;
				IEnvelope		agEnv = agRProps.Extent;
				IPnt			agCellSize = agRProps.MeanCellSize();

				try {
					using(StreamWriter sw = new StreamWriter(strWldFile, false)) {
						sw.WriteLine(Math.Round(agCellSize.X, 14).ToString());
						sw.WriteLine("0");
						sw.WriteLine("-0");
						sw.WriteLine(Math.Round(-agCellSize.Y, 14).ToString());
						sw.WriteLine(agEnv.XMin - (agCellSize.X / 2));
						sw.WriteLine(agEnv.YMax - (agCellSize.Y / 2));
						
						sw.Close();
						
						blnRet = true;
					}
				}
				catch(Exception ex) {
					// ﾛｸﾞ出力
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);
				}
			}
			
			// 返却
			return blnRet;
		}
		
		// ﾚｸﾃｨﾌｧｲ
		private bool RectifyRaster(IRaster TargetRaster) {
			bool blnRet = false;
			
			// ﾚｸﾃｨﾌｧｲ･ﾌｫｰﾑを開く
			FormGeoRefRectify	frmRectify = new FormGeoRefRectify(this._agRLayer.Raster, this.m_mapControl);
			
			if(frmRectify.ShowDialog(this) == DialogResult.OK) {
				blnRet = true;
			}
			
			// 返却
			return blnRet;
		}
		
		/// <summary>
		/// ライン・オブジェクトを生成します
		/// </summary>
		/// <param name="FromPoint">始点</param>
		/// <param name="ToPoint">終点</param>
		/// <returns>単純ライン</returns>
		private ILine CreateLine(IPoint FromPoint, IPoint ToPoint) {
			ILine	agLine = null;
			
			// 入力ﾁｪｯｸ
			if(!(FromPoint == null || ToPoint == null)) {
				agLine = new LineClass();
				agLine.FromPoint = FromPoint;
				agLine.ToPoint = ToPoint;
			}
			
			// 返却
			return agLine;
		}
		
		/// <summary>
		/// ポリライン・オブジェクトを生成します
		/// </summary>
		/// <param name="Nodes">構成ノード群</param>
		/// <returns>ポリライン</returns>
		private IPolyline CreatePolyLine(IPoint[] Nodes) {
			// ｼﾞｵﾒﾄﾘﾌﾞﾘｯｼﾞを介して作成する
			IGeometryBridge2	agGeoBrdg = new GeometryEnvironmentClass();
			IPointCollection4	agLineNodes = new PolylineClass();
			
			agGeoBrdg.SetPoints(agLineNodes, ref Nodes);
			
			// 返却
			return (IPolyline)agLineNodes;
		}

		/// <summary>
		/// ポリライン・オブジェクトを生成します
		/// </summary>
		/// <param name="Nodes">構成ノード群</param>
		/// <returns>ポリライン</returns>
		private IPolyline CreatePolyLine(WKSPoint[] Nodes) {
			// ｼﾞｵﾒﾄﾘﾌﾞﾘｯｼﾞを介して作成する
			IGeometryBridge2	agGeoBrdg = new GeometryEnvironmentClass();
			IPointCollection4	agLineNodes = new PolylineClass();
			
			agGeoBrdg.SetWKSPoints(agLineNodes, ref Nodes);
			
			// 返却
			return (IPolyline)agLineNodes;
		}

		/// <summary>
		/// ポリライン・オブジェクトを生成します
		/// </summary>
		/// <param name="Nodes">構成セグメント群</param>
		/// <returns>ポリライン</returns>
		private IPolyline CreatePolyLine(ISegment[] Nodes) {
			// ｼﾞｵﾒﾄﾘﾌﾞﾘｯｼﾞを介して作成する
			IGeometryBridge2	agGeoBrdg = new GeometryEnvironmentClass();
			ISegmentCollection	agLineNodes = new PolylineClass();
			
			agGeoBrdg.SetSegments(agLineNodes, ref Nodes);
			
			// 返却
			return (IPolyline)agLineNodes;
		}
		
		/// <summary>
		/// コントロールポイント群を取得します
		/// </summary>
		/// <returns></returns>
		private IPointCollection4 GetControlPoints(bool IsTarget) {
			IPointCollection4	agPnts = new PolygonClass();
			
			// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ数をﾁｪｯｸ
			if(this.listView1.Items.Count > 0) {
				// 値の取得列を設定 (補正 / 元)
				int[]	intCol = IsTarget ? new int[] {3, 4} : new int[] {1, 2};
			
				object	objM = Type.Missing;
				IPoint	agPnt;
				double	dblCoord;
				foreach(ListViewItem lviRow in this.listView1.Items) {
					agPnt = new PointClass();
					
					// X座標を取得
					if(double.TryParse(lviRow.SubItems[intCol[0]].Text, out dblCoord)) {
						agPnt.X = dblCoord;
					}
					// Y座標を取得
					if(double.TryParse(lviRow.SubItems[intCol[1]].Text, out dblCoord)) {
						agPnt.Y = dblCoord;
					}
					
					// ｺﾚｸｼｮﾝに追加
					agPnts.AddPoint(agPnt, ref objM, ref objM);
				}
			}
			
			// 返却
			return agPnts;
		}
		
		/// <summary>
		/// 残差を表示します
		/// </summary>
		/// <param name="RowIndex">インデックス</param>
		/// <param name="SubValue">残差</param>
		/// <param name="SubPoint">残差補正位置</param>
		private void ShowSubstructValue(int RowIndex, double SubValue, IPoint SubPoint) {
			// 入力ﾁｪｯｸ
			if(RowIndex >= 0 && RowIndex < this.listView1.Items.Count) {
				ListViewItem	lviCtlPoint = this.listView1.Items[RowIndex];
				
				// 残差を表示 (小数点以下5桁)
				lviCtlPoint.SubItems[5].Text = Math.Round(SubValue, 5).ToString(DISPLAY_FORMAT_SUBST);
				// 補正の補正ﾎﾟｲﾝﾄをﾀｸﾞに潜ませておきます
				lviCtlPoint.Tag = SubPoint;
			}
		}
		
		/// <summary>
		/// 残差表示をクリアします
		/// </summary>
		private void ClearSubstructValue() {
			foreach(ListViewItem lviTemp in this.listView1.Items) {
				if(!lviTemp.SubItems[5].Text.Equals("")) {
					lviTemp.SubItems[5].Text = "";
					lviTemp.Tag = null;				// 残差位置を破棄
				}
			}
		}
		
		/// <summary>
		/// ポイント群から一部のポイントを取得します
		/// </summary>
		/// <param name="Points">ポイント群</param>
		/// <param name="PointIndex">指定位置</param>
		/// <param name="Count">取得個数　※0時は、指定位置以降すべてを対象</param>
		/// <returns>一部のポイント群</returns>
		private IPointCollection4 QueryPoints(IPointCollection4 Points, int PointIndex, int Count) {
			// 取得個数を調整
			if(Count <= 0 || Count > (Points.PointCount - PointIndex)) {
				Count = Points.PointCount - PointIndex;
			}
			
			// ｼﾞｵﾒﾄﾘﾌﾞﾘｯｼﾞを介す
			IGeometryBridge2	agGeoBrdg = new GeometryEnvironmentClass();
			IPoint[]			agPoints = new PointClass[Count];
			
			// 出力用のﾎﾟｲﾝﾄ･ｵﾌﾞｼﾞｪｸﾄを生成しておく
			for(int intCnt=0; intCnt < agPoints.Length; intCnt++) {
				agPoints[intCnt] = new PointClass();
			}
			
			// 取得
			agGeoBrdg.QueryPoints(Points, PointIndex, ref agPoints);
			
			// 返却
			return this.CreatePointCollection(agPoints, new MultipointClass());
		}
		
		/// <summary>
		/// ポイント群にポイント群を追加します
		/// </summary>
		/// <param name="SourcePoints">追加先ポイント群</param>
		/// <param name="AddPoints">追加ポイント群</param>
		/// <returns>OK / NG</returns>
		private bool AddPointCollection(ref IPointCollection4 SourcePoints, IPointCollection4 AddPoints) {
			// 入力ﾁｪｯｸ
			if(!(SourcePoints == null || (AddPoints == null && AddPoints.PointCount > 0))) {
				// ｼﾞｵﾒﾄﾘﾌﾞﾘｯｼﾞを介す
				IGeometryBridge2	agGeoBrdg = new GeometryEnvironmentClass();
				IPoint[]			agAPoints = this.GetPointArray(AddPoints);
				
				// ﾎﾟｲﾝﾄ･ｾｯﾄを追加
				agGeoBrdg.AddPoints(SourcePoints, ref agAPoints);
			}
			
			// 返却
			return SourcePoints != null && SourcePoints.PointCount > 0;
		}

		/// <summary>
		/// ポイント群にポイント群を挿入します
		/// </summary>
		/// <param name="SourcePoints"></param>
		/// <param name="AddPoints"></param>
		/// <param name="Index">挿入先インデックス</param>
		/// <returns></returns>
		private bool InsertPointCollection(ref IPointCollection4 SourcePoints, IPointCollection4 AddPoints, int Index) {
			// 入力ﾁｪｯｸ
			if(!(SourcePoints == null || (AddPoints == null && AddPoints.PointCount > 0))) {
				// ｼﾞｵﾒﾄﾘﾌﾞﾘｯｼﾞを介す
				IGeometryBridge2	agGeoBrdg = new GeometryEnvironmentClass();
				IPoint[]			agAPoints = this.GetPointArray(AddPoints);
				
				// ｲﾝﾃﾞｯｸｽ調整
				if(Index < 0) {
					Index = 0;
				}
				else if(Index >= SourcePoints.PointCount) {
					Index = SourcePoints.PointCount - 1;
				}
				
				// ﾎﾟｲﾝﾄ･ｾｯﾄを指定ｲﾝﾃﾞｯｸｽに挿入
				agGeoBrdg.InsertPoints(SourcePoints, Index, ref agAPoints);
			}
			
			// 返却
			return SourcePoints != null && SourcePoints.PointCount > 0;
		}
				
		/// <summary>
		/// ポイントの配列を取得します
		/// </summary>
		/// <param name="Points">ポイントコレクション</param>
		/// <returns>ポイント配列</returns>
		private IPoint[] GetPointArray(IPointCollection4 Points) {
			List<IPoint>	agPoints = new List<IPoint>();
			
			// 入力ﾁｪｯｸ
			if(Points != null && Points.PointCount > 0) {
				for(int intCnt=0; intCnt < Points.PointCount; intCnt++) {
					agPoints.Add(Points.get_Point(intCnt));
				}
			}
			
			// 返却
			return agPoints.ToArray();
		}
		
		/// <summary>
		/// ポリゴン・グラフィックを表示します (確認用コード)
		/// </summary>
		/// <param name="PolygonGeo"></param>
		private void ShowEnvelope(IPolygon PolygonGeo) {
			if(PolygonGeo != null) {
				IElement	agElm = null;

				// ｼﾝﾎﾞﾙ設定
				ISimpleFillSymbol	agPGSym = new SimpleFillSymbolClass();
				IRgbColor			agRGBFillColor = new RgbColorClass();
				
				ISimpleLineSymbol	agPLSym = new SimpleLineSymbolClass();
				IRgbColor			agRGBOutColor = new RgbColorClass();
				
				// 塗り設定
				agPGSym.Style = esriSimpleFillStyle.esriSFSNull;
				agRGBFillColor.Red = 255;
				agRGBFillColor.Green = 0;
				agRGBFillColor.Blue = 0;

				agPGSym.Color = agRGBFillColor;

				// ｱｳﾄﾗｲﾝ設定
				agPLSym.Style = esriSimpleLineStyle.esriSLSSolid;
				agPLSym.Width = 2d;
				agRGBOutColor.Red = 0;
				agRGBOutColor.Green = 128;
				agRGBOutColor.Blue = 128;
				
				agPLSym.Color = agRGBOutColor;
				agPGSym.Outline = agPLSym;
				
				// ｴﾚﾒﾝﾄ
				IFillShapeElement		agPGElm = new PolygonElementClass();
				agPGElm.Symbol = agPGSym;
				
				agElm = (IElement)agPGElm;
				agElm.Geometry = PolygonGeo;
				agElm.Locked = false;
				
				// 表示
				IActiveView			agActView = (IActiveView)this.m_mapControl.Map;
				IGraphicsContainer	agGrpCont = (IGraphicsContainer)agActView;
				agGrpCont.AddElement(agElm, 0);
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, agElm, null);
			}
		}

		private void DrawPoint(IPoint PointGeo) {
			IActiveView		agActView = (IActiveView)this.m_mapControl.Map;
			IScreenDisplay	agScrDisplay = agActView.ScreenDisplay;

			// Constant.
			agScrDisplay.StartDrawing(agScrDisplay.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
			
			// ｼﾝﾎﾞﾙ設定
			ISimpleMarkerSymbol		agPntSym = new SimpleMarkerSymbolClass();
			IRgbColor				agRGBColor = new RgbColorClass();
			
			// 色設定
			agRGBColor.Red = 255;
			agRGBColor.Green = 64;
			agRGBColor.Blue = 64;
			
			agPntSym.Color = agRGBColor;
			agPntSym.Size = 18;
			agPntSym.Style = esriSimpleMarkerStyle.esriSMSCircle;

			agScrDisplay.SetSymbol((ISymbol)agPntSym);

			agScrDisplay.DrawPoint(PointGeo);
			agScrDisplay.FinishDrawing();
		}

		private void DrawPoint(int PixelX, int PixelY) {
			// 地図座標を生成
			IActiveView					agActView = (IActiveView)this.m_mapControl.Map;
			IScreenDisplay				agScrDisplay = agActView.ScreenDisplay;
			IDisplayTransformation		agDisplayTrans = agScrDisplay.DisplayTransformation;
			ESRI.ArcGIS.Geometry.IPoint agPoint = agDisplayTrans.ToMapPoint(PixelX, PixelY);

			// 描画
			this.DrawPoint(agPoint);
		}

		private void ShowPoint(IPoint PointGeo) {
			IElement	agElm = null;
			
			if(PointGeo != null) {
				// ｼﾝﾎﾞﾙ設定
				ISimpleMarkerSymbol		agPntSym = new SimpleMarkerSymbolClass();
				IRgbColor				agRGBColor = new RgbColorClass();
				
				// 色設定
				agRGBColor.Red = 255;
				agRGBColor.Green = 0;
				agRGBColor.Blue = 0;
				
				agPntSym.Color = agRGBColor;
				agPntSym.Size = 5;
				agPntSym.Style = esriSimpleMarkerStyle.esriSMSCircle;
				
				// ｴﾚﾒﾝﾄ
				IMarkerElement			agPntElm = new MarkerElementClass();
				agPntElm.Symbol = agPntSym;
				
				agElm = (IElement)agPntElm;
				agElm.Geometry = PointGeo;
				agElm.Locked = false;

				// 表示
				IActiveView			agActView = (IActiveView)this.m_mapControl.Map;
				IGraphicsContainer	agGrpCont = (IGraphicsContainer)agActView;
				agGrpCont.AddElement(agElm, 0);
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, agElm, null);
			}
		}		

		/// <summary>
		/// 機能実行可否状態を制御します
		/// </summary>
		/// <param name="IsChange">参照変更あり / なし</param>
		private void SetGeoRefFuncControl(bool IsUpdate, bool IsSaveAs) {
			// 更新
			this.ToolStripMenuItem_GG.Enabled = IsUpdate;
			// ﾚｸﾃｨﾌｧｲ
			this.ToolStripMenuItem_GY.Enabled = IsSaveAs;
			
			// ﾌｨｯﾄ
			this.ToolStripMenuItem_GF.Enabled = this.listView1.Items.Count <= 0;
			
			// ﾘｾｯﾄ
			this.ToolStripMenuItem_GE.Enabled = IsUpdate;
							
			// 保存
			this.button_SaveTextFile.Enabled = this.listView1.Items.Count > 0;
		}
		
		/// <summary>
		/// リンクファイルを読み込みます
		/// </summary>
		/// <param name="FilePath">リンクファイルのパス</param>
		/// <returns>OK / NG</returns>
		private bool OpenCtlPoints(string FilePath) {
			bool	blnRet = false;
			
			// ﾌｧｲﾙ有無を確認
			if(File.Exists(FilePath)) {
				// 全ﾘｾｯﾄ
				this.ResetRasterRef(this._agRLayer.Raster);
				
				// 指定ﾌｧｲﾙを展開
				using(StreamReader streamR = new StreamReader(FilePath, Encoding.GetEncoding("shift_jis"))) {
					try {
						// ﾌｧｲﾙ内容を読み込む
						List<LinkTable>	linkRows = new List<LinkTable>();
						LinkTable		linkRow;
						
						// 読み込みの仕様 (※Desktopに基づく)
						// ・1行目の先頭が空白でない文字列の場合は無効扱い
						// ・値がすべて0の場合は、無効扱い
						// ・行内に文字列があった場合に、読み込みを中断、それまでの数値をﾃｰﾌﾞﾙに反映
						// ・区切り文字は、ﾀﾌﾞ,ｶﾝﾏ,ｽﾍﾟｰｽ,全角ｽﾍﾟｰｽ
						while(!streamR.EndOfStream) {
							// 1行読み込み
							linkRow = new LinkTable(streamR.ReadLine());
							
							if(linkRow.IsValid) {
								linkRows.Add(linkRow);
							}
						}
						
						// 無効ﾒｯｾｰｼﾞを表示
						if(linkRows.Count <= 0) {
							Common.MessageBoxManager.ShowMessageBoxWarining(FilePath + Properties.Resources.FormGeoReference_WARNING_InvalidLinkFile);
						}
						else {
							// ﾘﾝｸﾃｰﾌﾞﾙに展開
							foreach(LinkTable linkTemp in linkRows) {
								// 
								this.AddControlPointList(this.CreatePoint(linkTemp.OrgX, linkTemp.OrgY),
														 this.CreatePoint(linkTemp.RevX, linkTemp.RevY),
														 false);
							}
							
							// ｱﾌｨﾝ実行
							this.ExecTrans(true);
							
							blnRet = true;
						}
					}
					catch(Exception ex) {
						// ﾛｸﾞに記録
		                Common.UtilityClass.DoOnError(ex);
						// ﾒｯｾｰｼﾞ表示
						Common.MessageBoxManager.ShowMessageBoxError(
							Properties.Resources.FormGeoReference_ERROR_ReadLinkFile + Environment.NewLine +ex.Message);
						
						// 全ﾘｾｯﾄ
						this.ResetRasterRef(this._agRLayer.Raster);
					}
					finally {
						// ｽﾄﾘｰﾑを閉じる
						streamR.Close();
					}
				}
			}
			
			// 返却
			return blnRet;
		}
		
		/// <summary>
		/// リンクファイルを作成します
		/// </summary>
		/// <param name="FilePath">リンクファイルのパス</param>
		/// <returns>OK / NG</returns>
		private bool SaveLinkFile(string FilePath) {
			bool	blnRet = false;

			// ﾘﾝｸﾌｧｲﾙの内容を生成
			LinkTable		linkRow;
			StringBuilder	sbCont = new StringBuilder();
			foreach(ListViewItem lviTemp in this.listView1.Items) {
				linkRow = new LinkTable(string.Format("{0}\t{1}\t{2}\t{3}", 
								lviTemp.SubItems[1].Text, lviTemp.SubItems[2].Text, lviTemp.SubItems[3].Text, lviTemp.SubItems[4].Text));
				sbCont.AppendLine(linkRow.ToString());
			}
			
			// ﾘﾝｸ･ﾌｧｲﾙへの出力
			using(StreamWriter streamW = new StreamWriter(FilePath, false, Encoding.GetEncoding("shift_jis"))) {
				try {
					// 書き込み
					streamW.Write(sbCont.ToString());
					
					blnRet = true;
				}
				catch(Exception ex) {
					// ﾛｸﾞに記録
	                Common.UtilityClass.DoOnError(ex);
					// ﾒｯｾｰｼﾞ表示
					Common.MessageBoxManager.ShowMessageBoxError(
						Properties.Resources.FormGeoReference_ERROR_SaveLinkFile + Environment.NewLine +ex.Message);
				}
				finally {
					// ｽﾄﾘｰﾑを閉じる
					streamW.Close();
				}
			}
			
			// 返却
			return  blnRet;
		}
		
	#region SHARED GEO MODULES
		
		/// <summary>
		/// エンベロープをポリゴンに変換します
		/// </summary>
		/// <param name="RegionEnvelope">エンベロープ</param>
		/// <returns>ポリゴン(ポイント群)</returns>
		private IPointCollection4 GetPointCollection(IEnvelope RegionEnvelope) {
			IPointCollection4	agPC;
			IPoint[]			agPnt = new PointClass[4];
			
			// ﾎﾟｲﾝﾄを生成
			for(int intCnt=0; intCnt < agPnt.Length; intCnt++) {
				agPnt[intCnt] = new PointClass();
				
				if(intCnt == 0) {
					agPnt[intCnt].PutCoords(RegionEnvelope.XMin, RegionEnvelope.YMin);
				}
				else if(intCnt == 1) {
					agPnt[intCnt].PutCoords(RegionEnvelope.XMin, RegionEnvelope.YMax);
				}
				else if(intCnt == 2) {
					agPnt[intCnt].PutCoords(RegionEnvelope.XMax, RegionEnvelope.YMax);
				}
				else if(intCnt == 3) {
					agPnt[intCnt].PutCoords(RegionEnvelope.XMax, RegionEnvelope.YMin);
				}
			}
			
			// ﾎﾟｲﾝﾄ群ｵﾌﾞｼﾞｪｸﾄを作成
			agPC = this.CreatePointCollection(agPnt, new PolygonClass());
			
			// 返却
			return agPC;
		}
		
		/// <summary>
		/// 重心（ポイント）を取得します
		/// </summary>
		/// <param name="RegionEnvelope">エンベロープ</param>
		/// <returns></returns>
		private IPoint GetCentroid(IEnvelope RegionEnvelope) {
			IPoint	agPoint = new PointClass();
			
			// 重心を取得
			IArea	agArea = (IArea)RegionEnvelope;
			agPoint = agArea.Centroid;

			// 重心を算出 (自力計算時)
			//agPoint.X = RegionEnvelope.XMin + (RegionEnvelope.XMax - RegionEnvelope.XMin) / 2;
			//agPoint.Y = RegionEnvelope.YMin + (RegionEnvelope.YMax - RegionEnvelope.YMin) / 2;
			
			// 返却
			return agPoint;
		}
		
		/// <summary>
		/// ポイント・オブジェクトを作成します
		/// </summary>
		/// <param name="X">X座標</param>
		/// <param name="Y">Y座標</param>
		/// <returns>ポイント・オブジェクト</returns>
		private IPoint CreatePoint(double X, double Y) {
			IPoint	agPoint = new ESRI.ArcGIS.Geometry.PointClass();
			
			// 座標設定
			agPoint.PutCoords(X, Y);
			
			// 返却
			return agPoint;
		}
		
		/// <summary>
		/// IPointCollectionのAddPointsを利用する場合は、本メソッドを使用する
		/// </summary>
		/// <param name="Points">ポイント群</param>
		/// <param name="GeoType">元ジオメトリ</param>
		/// <returns>ポイント群</returns>
		private IPointCollection4 CreatePointCollection(IPoint[] Points, IGeometry GeoType) {
			IPointCollection4	agPnts = (IPointCollection4)GeoType;

			IGeometryBridge	agGeoBrdg = new GeometryEnvironmentClass();
			agGeoBrdg.AddPoints(agPnts, ref Points);
			
			// 返却
			return agPnts;
		}
		
		/// <summary>
		/// ラジアンに変換します
		/// </summary>
		/// <param name="DegreeAngle">角度</param>
		/// <returns>ラジアン</returns>
		private double RadAngle(double DegreeAngle) {
			return DegreeAngle * (Math.PI / 180d);
		}
		
		/// <summary>
		/// 角度に変換します
		/// </summary>
		/// <param name="RadAngle">ラジアン</param>
		/// <returns>角度</returns>
		private double DegreeAngle(double RadAngle) {
			return RadAngle * (180d / Math.PI);
		}
		
		/// <summary>
		/// エンベロープを生成します
		/// </summary>
		/// <param name="UpperLeft">左上</param>
		/// <param name="LowerRight">右下</param>
		/// <returns>エンベロープ</returns>
		private IEnvelope CreateEnvelope(IPoint UpperLeft, IPoint LowerRight) {
			IEnvelope	agEnv = null;
			
			// 入力ﾁｪｯｸ
			if(UpperLeft != null && LowerRight != null) {
				// 入力ﾁｪｯｸ2
				if(!(UpperLeft.Y == LowerRight.Y || UpperLeft.X == LowerRight.X)) {
					agEnv = new EnvelopeClass();

					// Yの設定
					if(UpperLeft.Y < LowerRight.Y) {
						agEnv.YMin = UpperLeft.Y;
						agEnv.YMax = LowerRight.Y;
					}
					else {
						agEnv.YMin = LowerRight.Y;
						agEnv.YMax = UpperLeft.Y;
					}
					
					// Xの設定
					if(UpperLeft.X > LowerRight.X) {
						agEnv.XMin = LowerRight.X;
						agEnv.XMax = UpperLeft.X;
					}
					else {
						agEnv.XMin = UpperLeft.X;
						agEnv.XMax = LowerRight.X;
					}
				}
			}
			
			// 返却
			return agEnv;
		}
		
		/// <summary>
		/// エンベロープの範囲を拡張します
		/// </summary>
		/// <param name="SourceEnvelope">元のエンベロープ</param>
		/// <param name="NewPoint">新しい頂点</param>
		/// <returns>拡張エンベロープ</returns>
		private IEnvelope ReCreateEnvelope(IEnvelope SourceEnvelope, IPoint ExtendPoint) {
			IEnvelope	agEnv = null;
			
			// 入力ﾁｪｯｸ
			if(SourceEnvelope != null && ExtendPoint != null) {
				IPoint	agUL = new PointClass();
				IPoint	agLR = new PointClass();
				
				// 基準頂点の設定(左上)
				agUL.PutCoords(SourceEnvelope.XMin, SourceEnvelope.YMax);
				
				// 左上頂点から、右下の位置を指定時
				if(agUL.X <= ExtendPoint.X && agUL.Y >= ExtendPoint.Y) {
					agLR.PutCoords(ExtendPoint.X, ExtendPoint.Y);
				}
				// 反転
				else if(agUL.X > ExtendPoint.X && agUL.Y < ExtendPoint.Y) {
					agUL.PutCoords(ExtendPoint.X, ExtendPoint.Y);
					agLR.PutCoords(SourceEnvelope.XMax, SourceEnvelope.YMin);
				}
				// その他
				else {
					agLR.PutCoords(SourceEnvelope.XMax, SourceEnvelope.YMin);

					// X
					if(SourceEnvelope.XMin > ExtendPoint.X) {
						// 左頂点を調整
						agUL.X = ExtendPoint.X;
					}
					else if(SourceEnvelope.XMax < ExtendPoint.X) {
						// 右頂点を調整
						agLR.X = ExtendPoint.X;
					}
					
					// Y
					if(SourceEnvelope.YMin > ExtendPoint.Y) {
						// 下頂点を調整
						agLR.Y = ExtendPoint.Y;
					}
					else if(SourceEnvelope.YMax < ExtendPoint.Y) {
						// 上頂点を調整
						agUL.Y = ExtendPoint.Y;
					}
				}
				// ｴﾝﾍﾞﾛｰﾌﾟを生成
				agEnv = this.CreateEnvelope(agUL, agLR);
			}
			
			// 返却
			return agEnv;
		}
	#endregion

#endregion


    }
#region リンクデータ読み込み用クラス
    /// <summary>
    /// リンクデータ構造クラス
    /// </summary>
    internal class LinkTable {
		// ﾌﾟﾛﾊﾟﾃｨ
		public double OrgX { get; set; }
		public double OrgY { get; set; }
		public double RevX { get; set; }
		public double RevY { get; set; }
		
		/// <summary>
		/// 有効なリンクかどうかを取得(判定)します
		/// </summary>
		public bool	IsValid {
			get {
				return OrgX + OrgY + RevX + RevY != 0d;
			}
		}
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LinkTable() {
			//
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="TextLine">リンクファイル 行テキスト</param>
		public LinkTable(string TextLine) {
			// ﾘﾝｸﾌｧｲﾙ ﾗｲﾝ読み込み
			this.ReadLine(TextLine);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="OrgX">元のX座標</param>
		/// <param name="OrgY">元のY座標</param>
		/// <param name="RevX">補正X座標</param>
		/// <param name="RevY">補正Y座標</param>
		public LinkTable(double OrgX, double OrgY, double RevX, double RevY) {
			this.OrgX = OrgX;
			this.OrgY = OrgY;
			this.RevX = RevX;
			this.RevY = RevY;
		}
		
		#region 公開メソッド
		// 値の設定
		public void SetCoord(double Value, int Index) {
			switch(Index) {
			case 0:
				this.OrgX = Value;
				break;
			case 1:
				this.OrgY = Value;
				break;
			case 2:
				this.RevX = Value;
				break;
			case 3:
				this.RevY = Value;
				break;
			}
		}
		
		public void ReadLine(string FileLine) {
			// 行調整
			FileLine = FileLine.Trim();
			
			if(!string.IsNullOrEmpty(FileLine)) {
				// 改行文字列を追加
				if(!FileLine.EndsWith("\n")) {
					FileLine += "\n";
				}

				StringBuilder	sbValue = new StringBuilder();
				bool			blnOK = false;
				int				intID = 0;
				
				foreach(char charTemp in FileLine) {
					if(char.IsDigit(charTemp) || charTemp.Equals('.') || (charTemp.Equals('-') && sbValue.Length <= 0)) {	// 0 - 9
						sbValue.Append(charTemp);
					}
					else if(char.IsControl(charTemp)) {		// \t(既定の区切り文字), \n
						blnOK = true;
					}
					else if(char.IsSeparator(charTemp)) {	// ｽﾍﾟｰｽ(全角含む)
						blnOK = true;
					}
					else if(char.IsPunctuation(charTemp) && charTemp.Equals(',')) {	// ｶﾝﾏ
						blnOK = true;
					}
					else {
						// 読み込み中止
						if(sbValue.Length > 0) {
							this.SetCoord(Convert.ToDouble(sbValue.ToString()), intID++);
						}
						break;
					}
					
					if(blnOK) {
						// 値なしは無視
						if(sbValue.Length > 0) {
							this.SetCoord(Convert.ToDouble(sbValue.ToString()), intID++);
							sbValue.Length = 0;
						}
						if(intID < 4) {
							blnOK = false;
						}
						else {
							break;
						}
					}
				}
			}
		}
		
		public override string ToString() {
			return string.Format("{0:f6}\t{1:f6}\t{2:f6}\t{3:f6}", this.OrgX, this.OrgY, this.RevX, this.RevY);
 			//return base.ToString();
		}
		#endregion
    }
#endregion
}
