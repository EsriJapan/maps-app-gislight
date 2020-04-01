using System;
using System.Drawing;
using System.Collections.Generic;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ADF.CATIDs;

using System.Runtime.InteropServices;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// ジオリファレンス・コントロールポイントコマンドツール 
	/// ・機能としては、ユーザーが指定した座標をジオリファレンス・ツールフォームに通知する
    /// </summary>
    /// <history>
    ///  2012-08-01 新規作成 
    /// </history>
    public sealed class GeoRef_AddControlPointTool : ICommand, ITool
    {
        private IMap						_agMap;
        private Ui.FormGeoReference			_frmGR;				// ｼﾞｵﾘﾌｧﾚﾝｽ･ﾌｫｰﾑ

		[DllImport("gdi32.dll")]
		static extern bool DeleteObject(IntPtr hObject);

		private System.Drawing.Bitmap		_toolBitmap;		// ﾂｰﾙ･ﾋﾞｯﾄﾏｯﾌﾟ
		private IntPtr						_ptrToolBitmap;		// ﾂｰﾙ･ﾋﾞｯﾄﾏｯﾌﾟへのﾎﾟｲﾝﾀ
		private IHookHelper					_agHookHelper;		// ﾎﾟｲﾝﾀ管理
		
		private INewLineFeedback			_agLFeedBack;		// ﾗｲﾝ描画
		private IElement					_agElm_P;			// 始点
		
		private Boolean						_blnAddWorking;		// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ追加中ﾌﾗｸﾞ
		private System.Windows.Forms.Cursor	_TCursor;			// ﾂｰﾙ実行時のﾏｳｽ･ｶｰｿﾙ
		
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GeoRef_AddControlPointTool(Ui.FormGeoReference OwnerForm) {
			// ﾘｿｰｽ名を取得
            string	strResourceName = this.GetType().Name + ".bmp";

			// ﾂｰﾙ･ﾌﾟﾛﾊﾟﾃｨの設定
            this._toolBitmap = new Bitmap(this.GetType(), strResourceName);
            this._toolBitmap.MakeTransparent(Color.White);
            _ptrToolBitmap = _toolBitmap.GetHbitmap();
            
            // ｼﾞｵﾘﾌｧﾚﾝｽ･ﾌｫｰﾑを取得
            this._frmGR = OwnerForm;
            
            this._agHookHelper = new HookHelperClass();
		}
        
        /// <summary>
        /// デストラクタ
        /// </summary>
        ~GeoRef_AddControlPointTool() {
			if(this._ptrToolBitmap.ToInt32() != 0) {
				DeleteObject(this._ptrToolBitmap);
			}
        }

#region ICommand メンバー

	#region プロパティ

		public string Message {
			get {
				return "ジオリファレンス・コントロールポイントを追加します";
			}
		}
		
		public int Bitmap {
			get {
				return _ptrToolBitmap.ToInt32();
			}
		}

		public string Caption {
			get {
				return "コントロールポイントの追加";
			}
		}

		public string Tooltip {
			get {
				return "ジオリファレンス・コントロールポイントを追加します";
			}
		}

		public int HelpContextID {
			get {
				// TODO:  Add GeoRef_AddControlPointTool.HelpContextID getter implementation
				return 0;
			}
		}

		public string Name {
			get {
				return "GeoReference/AddControlPoint";
			}
		}

		public bool Checked {
			get {
				return false;
			}
		}	

		public bool Enabled {
			get {
				if(_agHookHelper.FocusMap == null) return false;

				return true;
			}
		}

		public string HelpFile {
			get {
				// TODO:  Add GeoRef_AddControlPointTool.HelpFile getter implementation
				return null;
			}
		}

		public string Category {
			get {
				return "ESRIJapanGIS_V200";
			}
		}
	#endregion		
        
	#region イベント
        /// <summary>
        /// インスタンス生成 イベント
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public void OnCreate(object hook) {
			this._agHookHelper.Hook = hook;
			
			// 地図
            _agMap = this._agHookHelper.FocusMap;

            // ｶｰｿﾙ
            try {
				this._TCursor = new System.Windows.Forms.Cursor(GetType().Assembly.GetManifestResourceStream(GetType(), this.GetType().Name + ".cur"));
			}
			catch(Exception ex) {
				string	strErr = ex.Message;
			}
        }

        /// <summary>
        /// クリック イベント
        /// </summary>
        public void OnClick() {
			// 地図(ﾒｲﾝﾌｫｰﾑ)をｱｸﾃｨﾌﾞに設定
			this._frmGR.SetMapActive();
        }
	#endregion
#endregion

#region ITool メンバー

#region プロパティ
		/// <summary>
		/// マウス・カーソルを取得します
		/// </summary>
		public int Cursor {
			get {
				return this._TCursor.Handle.ToInt32();
			}
		}
		
		/// <summary>
		/// 現在、コントロールポイントを追加中かどうかを判定します
		/// </summary>
		public bool IsToolActive {
			get {
				return this._agLFeedBack != null;
			}
		}
#endregion

#region 公開メソッド
		public void EndTool() {
			// 作業ｱｲﾃﾑをｸﾘｱ
			if(this._blnAddWorking) {
				this.CancelAddCtlPoint();
			}
		}
#endregion

#region イベント
		/// <summary>
		/// マウス・ダウン イベント
		/// </summary>
		/// <param name="button"></param>
		/// <param name="shift"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void OnMouseDown(int button, int shift, int x, int y) {
			if(_agHookHelper.ActiveView == null || this._blnAddWorking) return;

			// ﾍﾟｰｼﾞﾚｲｱｳﾄであることはありえないのですが、一応ｺｰﾄﾞを残しておきます
			if(_agHookHelper.ActiveView is IPageLayout) {
				// Create a point in map coordinates
				IPoint pPoint = (IPoint)_agHookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);

				//Get the map if the point is within a data frame
				IMap pMap = _agHookHelper.ActiveView.HitTestMap(pPoint);

				if(pMap == null) return;

				//Set the map to be the page layout's focus map
				if(pMap != this._agHookHelper.FocusMap) {
					_agHookHelper.ActiveView.FocusMap = pMap;
					_agHookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
				}
			}

			IActiveView			agActView = (IActiveView)_agHookHelper.FocusMap;

			// 位置を記録 AND 位置を表示
			if(this._agElm_P == null) {
				IPoint				agPoint = agActView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
				IGraphicsContainer	agGrpCont = (IGraphicsContainer)agActView;
			
				// 補正前位置を描画
				this._agElm_P = this.CreateCtlPointElement(agPoint, false);
			
				agGrpCont.AddElement(this._agElm_P, 0);
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, this._agElm_P, agActView.Extent);
			}
		
			// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄ追加中ﾌﾗｸﾞをON (0移動でない場合のﾏｳｽ･ｱｯﾌﾟでOFF)
			this._blnAddWorking = true;
		}

		/// <summary>
		/// マウス・移動 イベント
		/// </summary>
		/// <param name="button"></param>
		/// <param name="shift"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void OnMouseMove(int button, int shift, int x, int y) {
			if(!this._blnAddWorking) return;

			IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;

			if(this._agLFeedBack == null) {
				this._agLFeedBack = new NewLineFeedbackClass();
				this._agLFeedBack.Display = agActView.ScreenDisplay;
				this._agLFeedBack.Start((IPoint)this._agElm_P.Geometry);
			}
			this._agLFeedBack.MoveTo(agActView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y));
		}

		/// <summary>
		/// マウス・アップ イベント
		/// </summary>
		/// <param name="button"></param>
		/// <param name="shift"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void OnMouseUp(int button, int shift, int x, int y) {
			if(!this._blnAddWorking) return;

			// 移動がない場合は無視
			if(this._agLFeedBack != null) {
				this._agLFeedBack.Stop();
				
				// 補正位置を取得 AND 位置を表示 AND 補正処理実行
				IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;
				IPoint	agPoint = agActView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
				
				IGraphicsContainer		agGrpCont = (IGraphicsContainer)agActView;
				IElement				agElm_P = this.CreateCtlPointElement(agPoint, true);
			
				// 位置表示の制御
				agGrpCont.DeleteElement(this._agElm_P);
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
				
				// 補正処理を実行
				this._frmGR.AddControlPointList(this._agElm_P, agElm_P);
				
				// 記録を破棄
				Marshal.ReleaseComObject(this._agElm_P);
				Marshal.ReleaseComObject(this._agLFeedBack);
				this._agElm_P = null;
				this._agLFeedBack = null;

				// ｺﾝﾄﾛｰﾙ･ﾎﾟｲﾝﾄの追加完了
				this._blnAddWorking = false;
			}
		}

		/// <summary>
		/// キー・ダウン イベント
		/// </summary>
		/// <param name="keyCode"></param>
		/// <param name="shift"></param>
		public void OnKeyDown(int keyCode, int shift) {
			if(_blnAddWorking) {
				// ESCｷｰ(操作のｷｬﾝｾﾙ)に対応
				if(keyCode == 27) {
					// 現在の制御を破棄
					this.CancelAddCtlPoint();
				}
			}
		}

		/// <summary>
		/// キー・アップ イベント
		/// </summary>
		/// <param name="keyCode"></param>
		/// <param name="shift"></param>
		public void OnKeyUp(int keyCode, int shift) {
			//
		}

		public bool OnContextMenu(int x, int y) {
			return false;
		}
		
		/// <summary>
		/// ダブル・クリック イベント
		/// </summary>
		public void OnDblClick() {
			//
		}		

		/// <summary>
		/// ツール非活性化 イベント
		/// </summary>
		/// <returns></returns>
		public bool Deactivate() {
			// 追加中の場合は、ｷｬﾝｾﾙ処理
			if(this._blnAddWorking) {
				//this.CancelAddCtlPoint();
				this._agLFeedBack = null;
				
				// 画面更新
				IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics | esriViewDrawPhase.esriViewForeground, null, null);
			}
			
			return true;
		}

		/// <summary>
		/// 地図描画 更新イベント
		/// </summary>
		/// <param name="hdc"></param>
		public void Refresh(int hdc) {

		}
	#endregion

		/// <summary>
		/// コントロールポイント描画要素を作成します
		/// </summary>
		/// <param name="Position">ポイント位置</param>
		/// <param name="IsFixed">補正先 / 補正前</param>
		/// <returns>描画要素</returns>
		private IElement CreateCtlPointElement(IPoint Position, bool IsFixed) {
			IElement	agElm = null;
			
			if(Position != null) {
				// ｼﾝﾎﾞﾙ設定
				ISimpleMarkerSymbol		agPntSym = new SimpleMarkerSymbolClass();
				IRgbColor				agRGBColor = new RgbColorClass();
				
				// 色設定
				if(IsFixed) {
					agRGBColor.Red = 255;
					agRGBColor.Green = 0;
					agRGBColor.Blue = 0;
				}
				else {
					agRGBColor.Red = 0;
					agRGBColor.Green = 255;
					agRGBColor.Blue = 0;
				}
				
				agPntSym.Color = agRGBColor;
				agPntSym.Size = 14;
				agPntSym.Style = esriSimpleMarkerStyle.esriSMSCross;
				
				// ｴﾚﾒﾝﾄ
				IMarkerElement			agPntElm = new MarkerElementClass();
				agPntElm.Symbol = agPntSym;
				
				agElm = (IElement)agPntElm;
				agElm.Geometry = Position;
				agElm.Locked = true;
			}
			
			// 返却
			return agElm;
		}
		
		/// <summary>
		/// 追加中の状態を破棄します
		/// </summary>
		private void CancelAddCtlPoint() {
			IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;

			// 現在の制御を破棄
			if(this._agElm_P != null) {
				IGraphicsContainer		agGrpCont = (IGraphicsContainer)agActView;
				agGrpCont.DeleteElement(this._agElm_P);
			}
			
			Marshal.ReleaseComObject(this._agElm_P);
			Marshal.ReleaseComObject(this._agLFeedBack);
			
			this._agElm_P = null;
			this._agLFeedBack = null;
			this._blnAddWorking = false;

			// 地図画面更新
			agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics | esriViewDrawPhase.esriViewForeground, this._agElm_P, null);
		}
		
		/// <summary>
		/// 指定ポイントの描画を消去します
		/// </summary>
		/// <param name="ControlPointElement"></param>
		private void DeleteCtlPointElement(IElement ControlPointElement) {
			if(ControlPointElement != null) {
				IActiveView			agActView = (IActiveView)_agHookHelper.FocusMap;
				IGraphicsContainer	agGrpCont = (IGraphicsContainer)agActView;

				// 指定ﾎﾟｲﾝﾄの描画を削除
				agGrpCont.DeleteElement(ControlPointElement);

				// 地図画面更新
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, ControlPointElement, null);
			}
		}
		
		
#endregion
    }
}
