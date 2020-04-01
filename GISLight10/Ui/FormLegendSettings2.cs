using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class FormLegendSettings2 : Form
    {
		/// <summary>
		/// 凡例選択アイテム クラス
		/// </summary>
		private class LegendListItem {
			public ILegendItem3	LegendItem	{ get; set; }

			public LegendListItem() {
				// 初期化
				this.LegendItem = null;
			}

			public override string ToString() {
				return LegendItem == null ? "" : LegendItem.Layer.Name;
			}

			public override bool Equals(object obj) {
				bool	blnRet = false;

				if(LegendItem != null) {
					if(obj != null && object.ReferenceEquals(LegendItem, obj)) {
						blnRet = true;
					}
					else if(obj != null && LegendItem.Equals(obj)) {
						blnRet = true;
					}
				}
				else if(obj == null) {
					blnRet = true;
				}

				return blnRet;
			}

			public override int  GetHashCode() {
 				return LegendItem == null ? 0 : LegendItem.GetHashCode();
			}
		}

        private IPageLayoutControl3	m_pageLayoutControl = null;
        private IActiveView			activeView;
        private bool				editMode = false;
        private	double				_dblFontSize;
        private IMapSurroundFrame	_agLeg_Edit;	// 編集対象の凡例
		private ILegend3			_agLegTemp;		// 適用予定の凡例

        /// <summary>
        /// コンストラクタ ※凡例新規作成時
        /// </summary>
        /// <param name="pageLayoutControl"></param>
        /// <param name="envelope"></param>
        /// <param name="activeView"></param>
        public FormLegendSettings2(IPageLayoutControl3 pageLayoutControl, IActiveView activeView) {
            InitializeComponent();

            this.m_pageLayoutControl = pageLayoutControl;
            this.activeView = activeView;
            this._agLeg_Edit = null;

            editMode = false;
        }

        /// <summary>
        /// コンストラクタ ※既存凡例編集時
        /// </summary>
        /// <param name="pageLayoutControl"></param>
        /// <param name="activeView"></param>
        public FormLegendSettings2(IPageLayoutControl3 pageLayoutControl, IActiveView activeView, IMapSurroundFrame TargetLegend)
        {
            InitializeComponent();

            this.m_pageLayoutControl = pageLayoutControl;
            this.activeView = activeView;
            this._agLeg_Edit = TargetLegend;

			// 凡例変更後、移動すると元に戻ってしまう不具合に対応 (変更時のみ)
			var graphicsContainer = this.m_pageLayoutControl.PageLayout as IGraphicsContainer;
			// 凡例ｸﾞﾗﾌｨｯｸｽ･ｵﾌﾞｼﾞｪｸﾄを更新しておく
			graphicsContainer.UpdateElement(this._agLeg_Edit as IElement);

            editMode = true;
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLegendSettings_Load(object sender, EventArgs e) {

			// ﾌｫﾝﾄ･ﾘｽﾄを準備
            comboBoxFontName.DataSource = FontFamily.Families;
            comboBoxFontName.DisplayMember = "Name";

            // 凡例ﾘｽﾄを作成
			List<LegendListItem>	arrLegs = new List<LegendListItem>();
			IMapSurroundFrame		agMapSurFrame = this.CreateNewLegend();
			ILegend3				agLegTemp = agMapSurFrame.MapSurround as ILegend3;
			for(int intCnt=0; intCnt < agLegTemp.ItemCount; intCnt++) {
				arrLegs.Add(new LegendListItem() { LegendItem = agLegTemp.get_Item(intCnt) as ILegendItem3 });
			}
			this.listBoxMapLayer.Items.AddRange(arrLegs.ToArray());

            // 新規追加
            if(!editMode) {
				this._agLeg_Edit = agMapSurFrame;
				this._agLegTemp = agLegTemp;
            }
            // 既存編集
            else {
				// 凡例のｺﾋﾟｰを作成
				this._agLegTemp = this._agLeg_Edit.MapSurround as ILegend3;

                // 凡例項目をリストに設定
				arrLegs.Clear();
                for(int intCnt=0; intCnt < this._agLegTemp.ItemCount; intCnt++) {
                    arrLegs.Add(new LegendListItem() { LegendItem = this._agLegTemp.get_Item(intCnt) as ILegendItem3 });
                }
				this.listBoxLegend.Items.AddRange(arrLegs.ToArray());

				agMapSurFrame = this._agLeg_Edit;
			}

            // 凡例タイトルの設定
            this.textBoxTitle.Text = this._agLegTemp.Title;
                
            // フォント設定
            this.comboBoxFontName.Text = this._agLegTemp.Format.TitleSymbol.Font.Name;
            // 表示ｻｲｽﾞを設定
            this.comboBoxFontSize.Text = Math.Round(this._agLegTemp.Format.TitleSymbol.Size, 2).ToString();

            // ﾌｫﾝﾄ･ｽﾀｲﾙを設定
            checkBoxBold.Checked = this._agLegTemp.Format.TitleSymbol.Font.Bold;
            checkBoxItalic.Checked = this._agLegTemp.Format.TitleSymbol.Font.Italic;
            checkBoxUnderline.Checked = this._agLegTemp.Format.TitleSymbol.Font.Underline;

			// ﾀｲﾄﾙの配色を設定
            btnFontColor.BackColor = System.Drawing.ColorTranslator.FromOle(this._agLegTemp.Format.TitleSymbol.Color.RGB);

            // ﾀｲﾄﾙの配置を設定
            if(this._agLegTemp.Format.TitleSymbol.HorizontalAlignment == esriTextHorizontalAlignment.esriTHALeft) {
                radioButtonLeft.Checked = true;
            }
            else if(this._agLegTemp.Format.TitleSymbol.HorizontalAlignment == esriTextHorizontalAlignment.esriTHACenter) {
                radioButtonCenter.Checked = true;
            }
            else if(this._agLegTemp.Format.TitleSymbol.HorizontalAlignment == esriTextHorizontalAlignment.esriTHARight) {
                radioButtonRight.Checked = true;
            }
                
            // 塗り情報を取得
            ISymbolBackground	agSymBG = (ISymbolBackground)agMapSurFrame.Background;
            this.radioButton_FrameBG_W.Checked = agSymBG == null ? false : agSymBG.FillSymbol.Color.Transparency > 0;
            this.radioButton_FrameBG_T.Checked = !this.radioButton_FrameBG_W.Checked;

			// 枠線情報を取得
			ISymbolBorder	agSymBorder = (ISymbolBorder)agMapSurFrame.Border;

			// 旧Ver ﾊﾞｸﾞ対応
			if(agSymBorder == null && agSymBG != null
				&& (agSymBG.FillSymbol.Outline.Width > 0d && agSymBG.FillSymbol.Outline.Color.Transparency > 0)) {
				// 境界線をﾘﾍﾟｱ
				agMapSurFrame.Border = this.CreateSimpleBorderLine(agSymBG.FillSymbol.Outline.Color.RGB, agSymBG.FillSymbol.Outline.Width);

				// 再取得
				agSymBorder = (ISymbolBorder)agMapSurFrame.Border;
			}

            // 設定を復元
			this.checkBox_FrameBorder.Checked = (agSymBorder != null && agSymBorder.LineSymbol.Color.Transparency > 0);
				
			decimal	decLWidth;
			if(agSymBorder != null) {
				decLWidth = (decimal)agSymBorder.LineSymbol.Width;

				// 設定値調整
				if(decLWidth < this.numericUpDown_FrameBorderWidth.Minimum) {
					decLWidth = this.numericUpDown_FrameBorderWidth.Minimum;
				}
				else if(decLWidth > this.numericUpDown_FrameBorderWidth.Maximum) {
					decLWidth = this.numericUpDown_FrameBorderWidth.Maximum;
				}
			}
			else {
				decLWidth = this.numericUpDown_FrameBorderWidth.Minimum;
			}
			this.numericUpDown_FrameBorderWidth.Value = decLWidth;
            
			// ﾌｫﾝﾄ･ｻｲｽﾞを記録
			this._dblFontSize = double.Parse(this.comboBoxFontSize.Text);
        }

		/// <summary>
		/// 凡例を作成します
		/// </summary>
		/// <returns></returns>
		private IMapSurroundFrame CreateNewLegend() {
			// 凡例ｵﾌﾞｼﾞｪｸﾄを作成
			IGraphicsContainer	agGC = this.m_pageLayoutControl.PageLayout as IGraphicsContainer;
			IFrameElement		agFE = agGC.FindFrame(this.activeView.FocusMap);
			IMapFrame			agMapFrame = agFE as IMapFrame;
			UID					agUID = new UIDClass() { Value = "esriCarto.Legend" };

			IMapSurroundFrame	agMapSurFrame = agMapFrame.CreateSurroundFrame(agUID, null);

			return agMapSurFrame;
		}
		
		/// <summary>
		/// 凡例グラフィックを作成します
		/// </summary>
		/// <param name="LegendElement">凡例</param>
		/// <returns></returns>
		private IElement CreateLegendElement(IMapSurroundFrame LegendElement) {
			IElement	agElm = null;

			// 凡例のｻｲｽﾞを取得
			IQuerySize			agQSize = LegendElement.MapSurround as IQuerySize;
			double				dblW = 0, dblH = 0, dblX = 2, dblY = 2;
			agQSize.QuerySize(ref dblW, ref dblH);
			double				dblAspectRatio = dblW / dblH;

			// ﾍﾟｰｼﾞ･ｻｲｽﾞを取得
			double				dblPW = 0, dblPH = 0;
			this.m_pageLayoutControl.Page.QuerySize(out dblPW, out dblPH);
			double				dblLW = dblPW / 10d;		// 凡例幅の最大

			// 凡例ｻｲｽﾞを計算
			IEnvelope			agEnv = new EnvelopeClass();
			//agEnv.PutCoords(dblX, dblY, (dblX * dblLW), (dblY * dblLW / dblAspectRatio));
			agEnv.PutCoords(dblX, dblY, dblX + dblLW, dblPH - dblY);

			// ｸﾞﾗﾌｨｯｸを作成
			agElm = LegendElement as IElement;
			agElm.Geometry = agEnv;
			
			return agElm;
		}

		/// <summary>
		/// 凡例の境界線を作成します
		/// </summary>
		/// <param name="RGB">RGB値</param>
		/// <param name="LineWidth">線の太さ</param>
		/// <returns>境界オブジェクト</returns>
		private ISymbolBorder CreateSimpleBorderLine(int RGB, double LineWidth) {
			// 凡例の枠線を作成
			ILineSymbol	agLSym = new SimpleLineSymbolClass() {
				Color = new ESRI.ArcGIS.Display.RgbColor() {
					RGB = RGB,
					Transparency = 255
				}, 
				Width = LineWidth
			};

			return new SymbolBorderClass() {
				LineSymbol = agLSym,
				Gap = 12d,
			};
		}

        /// <summary>
        /// OKボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e) {
            // 入力ﾁｪｯｸ
            if(this.textBoxTitle.Text.Trim().Equals("") && this.listBoxLegend.Items.Count <= 0) {
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(Properties.Resources.FormLegendSettings_WARNING_LegendItemNone);
				return;
            }

            try {
#if DEBUG
				Debug.WriteLine("○現在の凡例");
				for(int i = 0; i < this._agLegTemp.ItemCount; i++) {
					Debug.WriteLine("  " + this._agLegTemp.get_Item(i).Layer.Name);
				}

				Debug.WriteLine("○新しい凡例");
				for(int i = 0; i < listBoxLegend.Items.Count; i++) {
					Debug.WriteLine("  " + listBoxLegend.Items[i].ToString());
				}
#endif

				// 凡例項目を設定
				this._agLegTemp.ClearItems();
				for(int intCnt = this.listBoxLegend.Items.Count - 1; intCnt >= 0; intCnt--) {
					this._agLegTemp.InsertItem(0, (this.listBoxLegend.Items[intCnt] as LegendListItem).LegendItem as ILegendItem);
				}

				// 凡例タイトルの設定
				if(this._agLegTemp.Title != textBoxTitle.Text) {
					this._agLegTemp.Title = textBoxTitle.Text;
				}

				// ﾌｫﾝﾄの設定
				stdole.IFontDisp fontDisp = new stdole.StdFontClass() as stdole.IFontDisp;
				fontDisp.Name = this.comboBoxFontName.Text;
				//fontDisp.Size = Convert.ToDecimal(this._dblFontSize);

				fontDisp.Bold = checkBoxBold.Checked;
				fontDisp.Italic = checkBoxItalic.Checked;
				fontDisp.Underline = checkBoxUnderline.Checked;
                
				IFormattedTextSymbol fomrttedTextSymbol = new TextSymbol();
				fomrttedTextSymbol.Font = fontDisp;

				// ﾌｫﾝﾄ･ｻｲｽﾞはｼﾝﾎﾞﾙ･ｻｲｽﾞに設定する
				fomrttedTextSymbol.Size = this._dblFontSize;
				fomrttedTextSymbol.Color = Common.UtilityClass.ConvertToESRIColor(btnFontColor.BackColor);

				if(radioButtonLeft.Checked == true) {
					fomrttedTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHALeft;
				}
				else if(radioButtonCenter.Checked == true) {
					fomrttedTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHACenter;
				}
				else if(radioButtonRight.Checked == true) {
					fomrttedTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHARight;
				}
				this._agLegTemp.Format.TitleSymbol = fomrttedTextSymbol;

				// レイヤーの自動追加オフ
				if(this._agLegTemp.AutoAdd) {
					this._agLegTemp.AutoAdd = false;
				}

				// 凡例枠を更新
				if(this.checkBox_FrameBorder.Checked) {
					// 現在のﾎﾞｰﾀﾞｰ設定を取得
					ISymbolBorder	agBorder = this._agLeg_Edit.Border as ISymbolBorder;

					// 凡例の枠線を作成
					this._agLeg_Edit.Border = this.CreateSimpleBorderLine(agBorder == null ? 0 : agBorder.LineSymbol.Color.RGB, (double)this.numericUpDown_FrameBorderWidth.Value);
				}
				else if(this._agLeg_Edit.Border != null) {
					this._agLeg_Edit.Border = null;
				}

				// 凡例背景の設定
				if(this.radioButton_FrameBG_W.Checked) {
					IFillSymbol	agFSym = new SimpleFillSymbolClass() {
						Color = new ESRI.ArcGIS.Display.RgbColor() {
							RGB = int.Parse("ffffff", System.Globalization.NumberStyles.HexNumber),
							Transparency = Convert.ToByte(this.radioButton_FrameBG_T.Checked ? 0 : 255),
						},
						Outline = new SimpleLineSymbol() {
							Color = new RgbColor() { Transparency = 255 },
							Width = 0,
						},
					};

					// 塗り情報を取得
					ISymbolBackground	agSymBG = this._agLeg_Edit.Background as ISymbolBackground;
					if(agSymBG == null) {
						agSymBG = new SymbolBackgroundClass() { Gap = 12d };
					}
					agSymBG.FillSymbol = agFSym;
					this._agLeg_Edit.Background = agSymBG;
				}
				else if(this._agLeg_Edit.Background != null) {
					this._agLeg_Edit.Background = null;
				}
				
				// ｸﾞﾗﾌｨｯｸｽを更新
				IElement	agElm;
                if(editMode) {
					// ｸﾞﾗﾌｨｯｸｽ範囲
					agElm = this._agLeg_Edit as IElement;

					IEnvelope	agEnv_O = agElm.Geometry.Envelope;
					IEnvelope	agEnv_N = new EnvelopeClass();

					this._agLegTemp.Refresh();
					this._agLegTemp.QueryBounds(this.activeView.ScreenDisplay as IDisplay, agEnv_O, agEnv_N);

					// ｸﾞﾗﾌｨｯｸｽを更新
					agElm = this._agLeg_Edit as IElement;
					agElm.Geometry = agEnv_N;
                }
                else {
					// ｸﾞﾗﾌｨｯｸｽを作成
                    agElm = this.CreateLegendElement(this._agLeg_Edit);
                    agElm.Activate(activeView.ScreenDisplay);

		            // 凡例ｵﾌﾞｼﾞｪｸﾄを追加
		            var graphicsContainer = this.m_pageLayoutControl.PageLayout as IGraphicsContainer;
                    graphicsContainer.AddElement(agElm, 0);

					this._agLegTemp.Refresh();
                }
                
				// 描画を更新
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

                // ﾌｫｰﾑを閉じる
                this.Close();
            }
            catch (COMException comex) {
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// フォントの色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFontColor_Click(object sender, EventArgs e)
        {
            //選択された色をボタンの背景色に設定する
            Color backcolor = Common.UtilityClass.GetColor(btnFontColor.BackColor);
            if (backcolor == btnFontColor.BackColor) return; // 変更なし

            btnFontColor.BackColor = backcolor;
        }

        /// <summary>
        /// 凡例項目リスト追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(this.listBoxMapLayer.SelectedIndex >= 0) {
                // 凡例項目リストに追加
                this.listBoxLegend.Items.Add(listBoxMapLayer.SelectedItem);
				this.listBoxLegend.TopIndex = this.listBoxLegend.Items.Count - 1;

				// 追加したﾚｲﾔを選択
				this.listBoxLegend.SelectedIndex = this.listBoxLegend.Items.Count - 1;
            }
        }

        /// <summary>
        /// 凡例項目リスト全追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllAdd_Click(object sender, EventArgs e)
        {
            // マップレイヤ全項目
            foreach (object item in listBoxMapLayer.Items)
            {
                // 凡例項目リストに追加
                listBoxLegend.Items.Add(item);
            }
        }

        /// <summary>
        /// 凡例項目リスト削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
			int	intSelID = this.listBoxLegend.SelectedIndex;
            if(intSelID >= 0) {
                // 凡例項目リストから削除
                this.listBoxLegend.Items.RemoveAt(intSelID);
                if(intSelID < this.listBoxLegend.Items.Count) {
					// 選択位置を維持
					this.listBoxLegend.SelectedIndex = intSelID;
                }
            }
        }

        /// <summary>
        /// 凡例項目リスト全削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllDel_Click(object sender, EventArgs e)
        {
            // 凡例項目リスト全削除
            listBoxLegend.Items.Clear();
        }

        /// <summary>
        /// 項目を上に
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            int idx = listBoxLegend.SelectedIndex;
            if(idx > 0) {
				object item = listBoxLegend.SelectedItem;
                // インデックスを－１
                listBoxLegend.Items.Insert(idx - 1, item);
                listBoxLegend.SelectedIndex = idx - 1;    
                // 項目を削除
                listBoxLegend.Items.RemoveAt(idx + 1);
            }
        }

        /// <summary>
        /// 項目を下に
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            int idx = listBoxLegend.SelectedIndex;
            if(idx >= 0 && idx < listBoxLegend.Items.Count - 1) {
				object item = listBoxLegend.SelectedItem;
                // インデックスを＋２
                listBoxLegend.Items.Insert(idx + 2, item);
                listBoxLegend.SelectedIndex = idx + 2;
                // 項目を削除
                listBoxLegend.Items.RemoveAt(idx);
            }
        }

		/// <summary>
		/// キー入力イベント
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData) {
			// 凡例ﾀｲﾄﾙ入力時にEnterｷｰを無効にする
			if(keyData == Keys.Enter && this.textBoxTitle.Focused) {
				return true;
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// フォントサイズ・キーダウン イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FontSize_KeyDown(object sender, KeyEventArgs e) {
			Control	ctlSelf = sender as Control;

			double	dblFSize;
			if(!ctlSelf.Text.Equals("") && double.TryParse(ctlSelf.Text, out dblFSize)) {
				// 値を記録
				this._dblFontSize = dblFSize;
			}
		}

		/// <summary>
		/// フォントサイズ・フォーカスアウト イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FontSize_Leave(object sender, EventArgs e) {
			Control	ctlSelf = sender as Control;
			
			// 異常値である場合、正常値を強制ｾｯﾄ
			double	dblFSize;
			if(double.TryParse(ctlSelf.Text, out dblFSize)) {
				this._dblFontSize = Math.Round(dblFSize, 2);
			}
			// 小数以下2桁まで
			ctlSelf.Text = this._dblFontSize.ToString();
		}
    }
}
