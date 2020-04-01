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

namespace ESRIJapan.GISLight10.Ui {
	public partial class FormGeoRefRectify : Form {
		
		private	IRaster			_agRaster = null;
        private IMapControl3	m_mapControl;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="TargetRaster">保存対象ラスター</param>
		/// <param name="mapControl">地図コントロール</param>
		public FormGeoRefRectify(IRaster TargetRaster, ESRI.ArcGIS.Controls.IMapControl3 mapControl) {
			InitializeComponent();
			
			// 地図ｺﾝﾄﾛｰﾙを保存
			this.m_mapControl = mapControl;
			
			// 保存対象ﾗｽﾀｰを保存
			this._agRaster = TargetRaster;
			
			// 対象ﾗｽﾀｰのﾌﾟﾛﾊﾟﾃｨを取得
			IRaster2		agRaster = (IRaster2)TargetRaster;
			IRasterProps	agRProps = (IRasterProps)agRaster;
			IPnt			agCellSize = agRProps.MeanCellSize();

			// ﾃﾞｰﾀｾｯﾄを取得
			IRasterDataset	agRDSet = agRaster.RasterDataset;
			
			// 保存ﾗｽﾀｰ名を表示
			string	strRName = System.IO.Path.GetFileNameWithoutExtension(agRDSet.CompleteName);
			string	strRExt = System.IO.Path.GetExtension(agRDSet.CompleteName);
			IDataset		agDS = (IDataset)agRDSet;
			IWorkspace		agWS = agDS.Workspace;
			
			string	strRType = this.IsWorkspaceType(agWS);
			if(strRType.Equals("R")) {
				strRName = System.IO.Path.GetFileNameWithoutExtension(agRDSet.CompleteName);
				strRExt = System.IO.Path.GetExtension(agRDSet.CompleteName);

				// ﾜｰｸｽﾍﾟｰｽ･ﾊﾟｽを表示 ()
				this.textBox_WS.Text = System.IO.Path.GetDirectoryName(agWS.PathName);
			}
			else {
				strRName = agDS.Name;
				strRExt = "";

				// ﾜｰｸｽﾍﾟｰｽ･ﾊﾟｽを表示
				this.textBox_WS.Text = agWS.PathName;
			}
			
			// 非重複名を取得
			int		intCnt = 0;
			while(this.IsExistRaster(agWS, strRName + (++intCnt).ToString() + strRExt)) {
				//
			}
			this.textBox_RasterName.Text = strRName + intCnt.ToString() + strRExt;

			
			// 現在のｾﾙ値を表示
			this.textBox_CellSize.Text = Math.Round((agCellSize.X + agCellSize.Y) / 2d, 6).ToString("0.000000");

			// 現在のNoData値を表示
			string		strNoData = "";
			ushort		shoND;
			if(agRProps.NoDataValue is ushort[]) {
				// 先頭値だけ表示
				ushort[]		shoNoData = (ushort[])agRProps.NoDataValue;
				strNoData = shoNoData[0].ToString();
			}
			else {
				/* v10.2～ 256時、Nullが返されるようになったらしい ?
				 * v10.0では、256時、値を表示しなかったが、10.2から値を表示するようになった */
				if(agRProps.NoDataValue == null) {
					strNoData = "";
				}
				else {
					strNoData = agRProps.NoDataValue.ToString();
					if(!ushort.TryParse(strNoData, out shoND)) {
						strNoData = "";
					}
				}
			}
			this.textBox_NoDataColor.Text = strNoData;

			// 選択ﾘｽﾄを生成
			this.SetResampleItems();
			this.SetImageFormatItems(agWS);
			this.SetCompressionTypeItems(((ObjectComboItem)this.comboBox_ImageFormat.SelectedItem).ItemValue.ToString());
		}

		/// <summary>
		/// フォーム・ロード イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Load(object sender, EventArgs e) {
			// ﾌｫｰｶｽ設定
			this.comboBox_ImageFormat.Select();
		}
		
		/// <summary>
		/// リサンプリング手法選択コンボボックスを準備します
		/// </summary>
		private void SetResampleItems() {
			List<ObjectComboItem>	ociItems = new List<ObjectComboItem>();

			ociItems.Add(new ObjectComboItem("最近隣内挿法（不連続データ用）", ESRI.ArcGIS.Geodatabase.rstResamplingTypes.RSP_NearestNeighbor));
			ociItems.Add(new ObjectComboItem("共一次内挿法（連続データ用）", ESRI.ArcGIS.Geodatabase.rstResamplingTypes.RSP_BilinearInterpolation));
			ociItems.Add(new ObjectComboItem("三次たたみ込み内挿法（連続データ用）", ESRI.ArcGIS.Geodatabase.rstResamplingTypes.RSP_CubicConvolution));
		
			// ｺﾝﾎﾞ設定
			this.comboBox_ReSamplingType.Items.AddRange(ociItems.ToArray());
			
			// 既定を設定
			this.comboBox_ReSamplingType.SelectedIndex = 0;
		}
		
		/// <summary>
		/// 画像形式の選択群を取得します
		/// </summary>
		private void SetImageFormatItems(IWorkspace WorkSpace) {
			List<ObjectComboItem>	ociItems = new List<ObjectComboItem>();
			
			bool	blnEnabled = true;
			
			// 既存ｱｲﾃﾑを削除
			if(this.comboBox_ImageFormat.Items.Count > 0) {
				this.comboBox_ImageFormat.Items.Clear();
			}
			
			// ﾜｰｸｽﾍﾟｰｽ種別を判定
			string	strWSType = this.IsWorkspaceType(WorkSpace);
			if(strWSType.Equals("R") || string.IsNullOrEmpty(strWSType)) {
				ociItems.Add(new ObjectComboItem("TIFF", "TIFF"));
				//ociItems.Add(new ObjectComboItem("BMP", "BMP"));
				ociItems.Add(new ObjectComboItem("JPEG", "JPG"));
				ociItems.Add(new ObjectComboItem("JP2000", "JP2"));
				//ociItems.Add(new ObjectComboItem("PNG", "PNG"));
				//ociItems.Add(new ObjectComboItem("ENVI", "ENVI"));
				//ociItems.Add(new ObjectComboItem("GRID", "GRID"));
				ociItems.Add(new ObjectComboItem("IMAGINE Image", "IMAGINE Image"));
				//ociItems.Add(new ObjectComboItem("GIF", "GIF"));
				//ociItems.Add(new ObjectComboItem("ESRI BIL", "BIL"));
				//ociItems.Add(new ObjectComboItem("ESRI BIP", "BIP"));
				//ociItems.Add(new ObjectComboItem("ESRI BSQ", "BSQ"));
				//ociItems.Add(new ObjectComboItem("PCI Raster", "PIX"));
				//ociItems.Add(new ObjectComboItem("USGS ASCII DEM", "DEM"));
				//ociItems.Add(new ObjectComboItem("X11 Pixmap", "XPM"));
				//ociItems.Add(new ObjectComboItem("PCRaster", "MAP"));
				//ociItems.Add(new ObjectComboItem("Memory Raster", "MEM"));
				//ociItems.Add(new ObjectComboItem("HDF4", "HDF4"));
				//ociItems.Add(new ObjectComboItem("Idrisi Raster Format", "RST"));

				blnEnabled = true;
			}
			else if(strWSType.Equals("F")) {
				ociItems.Add(new ObjectComboItem("ファイルジオデータベース", "GDB"));
				blnEnabled = false;
			}
			else if(strWSType.Equals("P")) {
				ociItems.Add(new ObjectComboItem("パーソナルジオデータベース", "GDB"));
				blnEnabled = false;
			}
			
			// ｺﾝﾎﾞ設定
			this.comboBox_ImageFormat.Items.AddRange(ociItems.ToArray());
			
			// 有効状態の制御
			this.comboBox_ImageFormat.Enabled = blnEnabled;
			
			// 既定を設定
			if(this.comboBox_ImageFormat.Items.Count > 0) {
				this.comboBox_ImageFormat.SelectedIndex = 0;
			}
		}
		
		/// <summary>
		/// 画像の圧縮方式群を取得します
		/// </summary>
		/// <returns></returns>
		private void SetCompressionTypeItems(string ImageFormat) {
			List<ObjectComboItem>	ociItems = new List<ObjectComboItem>();

			int		intSelectedIndex = 0;
			bool	blnEnabled = ImageFormat.Equals("TIFF") || ImageFormat.Equals("GRID") || ImageFormat.Equals("IMAGINE Image") || ImageFormat.Equals("GDB");

			// 既存ｱｲﾃﾑを削除
			if(this.comboBox_CompressionType.Items.Count > 0) {
				this.comboBox_CompressionType.Items.Clear();
			}
			
			// NONE
			if(ImageFormat.Equals("TIFF") || ImageFormat.Equals("BMP") || ImageFormat.Equals("ENVI")
				 || ImageFormat.Equals("BIL") || ImageFormat.Equals("BIP") || ImageFormat.Equals("BSQ")
				 || ImageFormat.Equals("GRID") || ImageFormat.Equals("IMAGINE Image") || ImageFormat.Equals("GDB")) {
				ociItems.Add(new ObjectComboItem("NONE", esriRasterCompressionType.esriRasterCompressionUncompressed));
			}
			// LZ77
			if(ImageFormat.Equals("PNG") || ImageFormat.Equals("GDB")) {
				ociItems.Add(new ObjectComboItem("LZ77", esriRasterCompressionType.esriRasterCompressionLZ77));
			}
			// JPEG
			if(/*ImageFormat.Equals("TIFF") ||*/ ImageFormat.Equals("JPG") || ImageFormat.Equals("GDB")) {
				ociItems.Add(new ObjectComboItem("JPEG", esriRasterCompressionType.esriRasterCompressionJPEG));
			}
			// JPEG2000
			if(ImageFormat.Equals("JP2") || ImageFormat.Equals("GDB")) {
				ociItems.Add(new ObjectComboItem("JPEG2000", esriRasterCompressionType.esriRasterCompressionJPEG2000));
			}
			// Packbits
			if(ImageFormat.Equals("TIFF")) {
				ociItems.Add(new ObjectComboItem("Packbits", esriRasterCompressionType.esriRasterCompressionPackBits));
			}
			// LZW
			if(ImageFormat.Equals("TIFF") || ImageFormat.Equals("GIF")) {
				ociItems.Add(new ObjectComboItem("LZW", esriRasterCompressionType.esriRasterCompressionLZW));
			}
			// RLE
			if(/*ImageFormat.Equals("TIFF") ||*/ ImageFormat.Equals("IMAGINE Image") || ImageFormat.Equals("GRID")) {
				ociItems.Add(new ObjectComboItem("RLE", esriRasterCompressionType.esriRasterCompressionRLE));
			}
			// CCITT
			/*if(ImageFormat.Equals("TIFF")) {
				ociItems.Add(new ObjectComboItem("CCITT Group 3", esriRasterCompressionType.esriRasterCompressionCCITTG3));
				ociItems.Add(new ObjectComboItem("CCITT Group 4", esriRasterCompressionType.esriRasterCompressionCCITTG4));
				ociItems.Add(new ObjectComboItem("CCITT (1D)", esriRasterCompressionType.esriRasterCompressionCCITTRLE));
			}*/

			// ｺﾝﾎﾞ設定
			this.comboBox_CompressionType.Items.AddRange(ociItems.ToArray());

			// 有効状態の制御
			this.comboBox_CompressionType.Enabled = blnEnabled;

			// 既定を設定
			this.comboBox_CompressionType.SelectedIndex = intSelectedIndex;
		}
		
		/// <summary>
		/// 画像形式のファイル名を取得します
		/// </summary>
		/// <param name="FileName"></param>
		/// <param name="ImageFormat"></param>
		/// <returns></returns>
		private string GetFileName(string FileName, string ImageFormat) {
			string	strRet = System.IO.Path.GetFileNameWithoutExtension(FileName);
			
			if(ImageFormat.Equals("TIFF")) {
				strRet += ".tif";
			}
			else if(ImageFormat.Equals("BMP")) {
				strRet += ".bmp";
			}
			else if(ImageFormat.Equals("ENVI")) {
				strRet += ".dat";
			}
			else if(ImageFormat.Equals("ESRI BIL")) {
				strRet += ".bil";
			}
			else if(ImageFormat.Equals("ESRI BIP")) {
				strRet += ".bip";
			}
			else if(ImageFormat.Equals("ESRI BSQ")) {
				strRet += ".bsq";
			}
			else if(ImageFormat.Equals("GIF")) {
				strRet += ".gif";
			}
			else if(ImageFormat.Equals("IMAGINE Image")) {
				strRet += ".img";
			}
			else if(ImageFormat.Equals("JP2")) {
				strRet += ".jp2";
			}
			else if(ImageFormat.Equals("JPG")) {
				strRet += ".jpg";
			}
			else if(ImageFormat.Equals("PNG")) {
				strRet += ".png";
			}
			else {	// GRID / GDB
				strRet += "";
			}
			
			// 返却
			return strRet;
		}

		/// <summary>
		/// 「キャンセル」ボタン クリック イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Cancel_Click(object sender, EventArgs e) {
			// ﾌｫｰﾑを閉じる
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		/// <summary>
		/// 「ワークスペースの選択」ボタン クリック イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_OpenFolder_Click(object sender, EventArgs e) {
            // ﾜｰｸｽﾍﾟｰｽ選択ﾀﾞｲｱﾛｸﾞを表示
            FormGISDataSelectDialog	frm = new FormGISDataSelectDialog();
            
            // 既定ﾌｫﾙﾀﾞ
            string	strFolder = this.textBox_WS.Text.Trim();
            if(!strFolder.Equals("")) {
				if(!Directory.Exists(strFolder) || strFolder.EndsWith(".gdb")) {
					// GeoDBの場合 (親ﾌｫﾙﾀﾞ)
					strFolder = System.IO.Path.GetDirectoryName(strFolder);
				}
				frm.StartFolder = strFolder;
			}
            
            // ﾜｰｸｽﾍﾟｰｽ名
            frm.SelectType = FormGISDataSelectDialog.ReturnType.WorkspaceName;

            // ﾕｰｻﾞｰ選択
            if(frm.ShowDialog(this) == DialogResult.OK) {
                // ﾊﾟﾗﾒｰﾀ更新
                if(frm.ReturnObject) {
					if(frm.SelectedObject is List<IWorkspaceName>) {
						List<IWorkspaceName>	agWSNames = (List<IWorkspaceName>)frm.SelectedObject;
						IWorkspaceName			agWSName = agWSNames[0];
						IWorkspaceFactory		agWSF = agWSName.WorkspaceFactory;
						IWorkspace				agWS = agWSF.OpenFromFile(agWSName.PathName, 0);
						
						// ﾊﾟｽを表示
						this.textBox_WS.Text = agWSName.PathName;
						// 名前を再取得
						
						
						// 形式を更新
						this.SetImageFormatItems(agWS);
						
						Marshal.ReleaseComObject(agWS);
						Marshal.ReleaseComObject(agWSName);
					}
				}
            }
            
            frm.Dispose();
        }

		/// <summary>
		/// 画像形式の選択変更 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ImageFormat_SelectedIndexChanged(object sender, EventArgs e) {
			ComboBox	ctlCmb = sender as ComboBox;
			
			// 選択ｱｲﾃﾑを取得
			string	strVal = ((ObjectComboItem)ctlCmb.SelectedItem).ItemValue.ToString();
			
			// 圧縮品質の制御
			this.numericUpDown_CompressionQuality.Enabled = (strVal.Equals("JP2") || strVal.Equals("JPG"));
			
			// 圧縮ﾀｲﾌﾟの制御
			this.SetCompressionTypeItems(strVal);
			
			// ﾌｧｲﾙ名の調整
			this.textBox_RasterName.Text = this.GetFileName(this.textBox_RasterName.Text, strVal);
		}

		/// <summary>
		/// 「圧縮タイプ」の選択変更 イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CompressionType_SelectedIndexChanged(object sender, EventArgs e) {
			ComboBox	ctlCmb = sender as ComboBox;
			
			// 選択値を取得
			esriRasterCompressionType	agCType = (esriRasterCompressionType)((ObjectComboItem)ctlCmb.SelectedItem).ItemValue;
			
			// 圧縮品質を制御
			if(agCType == esriRasterCompressionType.esriRasterCompressionJPEG ||
				agCType == esriRasterCompressionType.esriRasterCompressionJPEG2000) {
				this.numericUpDown_CompressionQuality.Enabled = true;
			}
			else {
				this.numericUpDown_CompressionQuality.Enabled = false;
			}
		}

		/// <summary>
		/// 「保存」ボタン クリック イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Save_Click(object sender, EventArgs e) {
			// ｴﾗｰﾒｯｾｰｼﾞ
			string			strMsg = "";

			// ﾋﾞｼﾞｰ表示
			System.Windows.Forms.Cursor preCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			this.Enabled = false;

			// 入力ﾃﾞｰﾀの調整
			this.textBox_WS.Text = this.textBox_WS.Text.Trim();
			this.textBox_RasterName.Text = this.textBox_RasterName.Text.Trim();
			this.textBox_CellSize.Text = this.textBox_CellSize.Text.Trim();
			this.textBox_NoDataColor.Text = this.textBox_NoDataColor.Text.Trim();

			// ﾜｰｸｽﾍﾟｰｽの取得
			IWorkspace	agWS = null;
			string		strRName = null;
			string		strFormat = null;
			if(!(this.textBox_WS.Text.Equals("") || this.textBox_RasterName.Text.Equals(""))) {
				// ﾗｽﾀｰ･ﾜｰｸｽﾍﾟｰｽを取得
				agWS = this.OpenRasterWorkspace(this.textBox_WS.Text);
				if(agWS != null) {
					// 名前の重複ﾁｪｯｸ
					if(!this.IsExistRaster(agWS, this.textBox_RasterName.Text)) {
						// 名称ﾁｪｯｸ
						if(!(!this.IsWorkspaceType(agWS).Equals("R") && char.IsDigit(this.textBox_RasterName.Text, 0))) {
							strRName = this.textBox_RasterName.Text;
						}
						else {
							strMsg += "名前の先頭文字に数値は使用できません。" + Environment.NewLine;
						}
					}
					else {
						strMsg += "ご指定のラスターは既に存在します。" + Environment.NewLine;
					}
				}
				else {
					strMsg += "保存先が正しくありません。" + Environment.NewLine;
				}
			}
			else {
				strMsg += "保存先を指定してください。" + Environment.NewLine;
			}
			
			// ｾﾙ･ｻｲｽﾞ指定時
			double		dblCellSize = double.MinValue;
			if(!this.textBox_CellSize.Text.Equals("")) {
				if(!double.TryParse(this.textBox_CellSize.Text, out dblCellSize)) {
					strMsg += "セルサイズの指定が正しくありません。" + Environment.NewLine;
				}
			}

			// NoData指定時 (全ﾊﾞﾝﾄﾞ統一指定) ※ArcMapと同じ挙動
			object	objND = null;
			if(!this.textBox_NoDataColor.Text.Equals("")) {
				string[]		strNoDatas = this.textBox_NoDataColor.Text.Split(',');
				ushort			shoND;

				if(ushort.TryParse(strNoDatas[0].Trim(), out shoND)) {
					objND = shoND;
				}
				else {
					objND = (ushort)0;
				}
			}
			
			// ﾚｸﾃｨﾌｧｲを実行
			if(strMsg.Equals("")) {
				// 画像形式を取得
				strFormat = ((ObjectComboItem)this.comboBox_ImageFormat.SelectedItem).ItemValue.ToString();
				
				// 画像設定を作成
				IRasterStorageDef	agRSDef = new RasterStorageDefClass();
				IRasterStorageDef3	agRSDef3 = (IRasterStorageDef3)agRSDef;
				agRSDef3.CompressionType = (esriRasterCompressionType)((ObjectComboItem)this.comboBox_CompressionType.SelectedItem).ItemValue;
				agRSDef3.CompressionQuality = Convert.ToInt32(this.numericUpDown_CompressionQuality.Value);
				
				// 保存
				IRasterDataset		agNewRDS = this.ExecRectify((IRaster2)this._agRaster, agWS, strRName, strFormat, dblCellSize, objND, agRSDef);
				
				if(agNewRDS != null) {
					// ﾋﾞｼﾞｰ解除
					this.Cursor = preCursor;
					this.Enabled = true;

					// ﾚｲﾔｰ追加確認
					DialogResult	dr = ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxQuestion2(this, Properties.Resources.FormGeoRefRectify_Q_LoadNewRaster);
					if(dr == DialogResult.OK) {
						// ﾚｲﾔｰを生成
						IRasterLayer	agRLayer = new RasterLayerClass();
						agRLayer.CreateFromDataset(agNewRDS);
						
						// 地図に追加
						this.m_mapControl.Map.AddLayer(agRLayer);
						IActiveView	agActView = this.m_mapControl.ActiveView;
						agActView.PartialRefresh(esriViewDrawPhase.esriViewGeography, agRLayer, agActView.Extent.Envelope);
					}
					
					// COM解放
					Marshal.ReleaseComObject(agNewRDS);
					
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
				else {
					strMsg += "保存できませんでした。" + Environment.NewLine;
				}
			}

			// ﾋﾞｼﾞｰ解除
			this.Cursor = preCursor;
			this.Enabled = true;

			// ｴﾗｰ･ﾒｯｾｰｼﾞ表示
			if(!strMsg.Equals("")) {
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(strMsg);
			}
		}
		
		/// <summary>
		/// レクティファイを実行します
		/// </summary>
		/// <param name="TargetRaster"></param>
		/// <param name="RasterWorkSpace"></param>
		/// <param name="RasterName"></param>
		/// <param name="ImageFormat"></param>
		/// <param name="CellSize"></param>
		/// <param name="NoData"></param>
		/// <returns></returns>
		private IRasterDataset ExecRectify(IRaster2 TargetRaster, IWorkspace RasterWorkSpace, string RasterName, string ImageFormat, double CellSize, object NoData, IRasterStorageDef RasterStorageDef) {
			IRasterDataset	agNewRDS = null;

			// 入力ﾁｪｯｸ
			if(!(TargetRaster == null || RasterWorkSpace == null || string.IsNullOrEmpty(RasterName) || string.IsNullOrEmpty(ImageFormat) || RasterStorageDef == null)) {
				IRasterProps	agRProps = (IRasterProps)TargetRaster;
				IRaster2		agRaster = (IRaster2)TargetRaster;
				
				IPnt			agPnt = agRProps.MeanCellSize();
				double			dblCellSize = Math.Round((agPnt.X + agPnt.Y) / 2d, 6);

				// ｾﾙ･ｻｲｽﾞ指定時
				if(!(CellSize == double.MinValue || CellSize.Equals(dblCellSize))) {
					IEnvelope	agEnv = agRProps.Extent;

					// ﾗｽﾀｰ範囲を再計算
					int			intW = Convert.ToInt32((agEnv.XMax - agEnv.XMin) / CellSize);
					int			intH = Convert.ToInt32((agEnv.YMax - agEnv.YMin) / CellSize);

					IPoint		agLowL = agEnv.LowerLeft;
					IPoint		agUppR = new PointClass();
					IEnvelope	agNewEnv = new EnvelopeClass();
					agUppR.X = agLowL.X + intW * CellSize;
					agUppR.Y = agLowL.Y + intH * CellSize;
					agNewEnv.LowerLeft = agLowL;
					agNewEnv.UpperRight = agUppR;

					// ﾌﾟﾛﾊﾟﾃｨ更新
					agRProps.Extent = agNewEnv;
					agRProps.Width = intW;
					agRProps.Height = intH;
				}

				// NoData指定時 (全ﾊﾞﾝﾄﾞ統一指定とします)
				if(NoData != null) {
					agRProps.NoDataValue = NoData;
				}
				
				// 統計情報ﾁｪｯｸ
				bool	blnReSta = false;
				bool	blnHasSta;
				
				// ﾊﾞﾝﾄﾞを取得
				IRasterBandCollection	agRBC = (IRasterBandCollection)agRaster.RasterDataset;
				IRasterBand				agRBand;
				for(int intCnt=0; intCnt < agRBC.Count; intCnt++) {
					// ﾊﾞﾝﾄﾞを取得
					agRBand = agRBC.Item(intCnt);
					
					// 統計情報の状態を確認
					agRBand.HasStatistics(out blnHasSta);
					
					// 再計算ﾌﾗｸﾞをｾｯﾄ
					blnReSta = blnHasSta;
					if(blnReSta) {
						break;
					}
				}
				
				// 保存
				ISaveAs2	agSaveAs = (ISaveAs2)agRaster;
				
				try {
					// 新規保存
					agNewRDS = agSaveAs.SaveAsRasterDataset(RasterName, RasterWorkSpace, ImageFormat, RasterStorageDef);

					// 統計情報再計算処理 (特にDEMなど)
					agRBC = (IRasterBandCollection)agNewRDS;
					for(int intCnt=0; intCnt < agRBC.Count; intCnt++) {
						// ﾊﾞﾝﾄﾞを取得
						agRBand = agRBC.Item(intCnt);
						
						// 統計情報を確認
						agRBand.HasStatistics(out blnHasSta);
						if(!blnHasSta && blnReSta) {
							// 統計情報を計算
							agRBand.ComputeStatsAndHist();
						}
					}
				}
				catch(Exception ex) {
					// ﾛｸﾞに記録
	                Common.UtilityClass.DoOnError(ex);
				}
			}
			
			// 返却
			return agNewRDS;
		}
		
		/// <summary>
		/// ラスターワークスペースを取得します
		/// </summary>
		/// <param name="WSPath">パス</param>
		/// <returns>ラスタワークスペース</returns>
		private IWorkspace OpenRasterWorkspace(string WSPath) {
			IWorkspace			agWS = null;
			
			// 入力ﾁｪｯｸ
			if(!string.IsNullOrEmpty(WSPath)
				&& (Directory.Exists(WSPath) || (System.IO.Path.GetExtension(WSPath).ToLower().Equals(".mdb") && File.Exists(WSPath)))) {
				IWorkspaceFactory	agWSF;
				IRasterWorkspaceEx	agRWS_Ex = null;
				
				// ｼﾞｵDB
				if(WSPath.EndsWith(".gdb")) {
					agWSF = SingletonUtility.NewFileGDBWorkspaceFactory();
					agWS = agWSF.OpenFromFile(WSPath, 0);
					
					agRWS_Ex = (IRasterWorkspaceEx)agWS;
				}
				else if(this.textBox_WS.Text.EndsWith(".mdb")) {
					agWSF = SingletonUtility.NewAccessWorkspaceFactory();
					agWS = agWSF.OpenFromFile(WSPath, 0);

					agRWS_Ex = (IRasterWorkspaceEx)agWS;
				}
				else {
					agWSF = SingletonUtility.NewRasterWorkspaceFactory();
					agWS = agWSF.OpenFromFile(WSPath, 0);
				}
				
				// GDBの場合の儀式
				if(agRWS_Ex != null) {
					agWS = (IWorkspace)agRWS_Ex;
				}
			}
			
			// 返却
			return agWS;
		}
		
		/// <summary>
		/// ワークスペースの種類を判定します
		/// </summary>
		/// <param name="WorkSpace"></param>
		/// <returns>R(ﾗｽﾀｰ) / F(ﾌｧｲﾙｼﾞｵ) / P(ﾊﾟｰｿﾅﾙｼﾞｵ)</returns>
		private string IsWorkspaceType(IWorkspace WorkSpace) {
			string	strRet = "";
			
			// 入力ﾁｪｯｸ
			if(WorkSpace != null) {
				if(WorkSpace.PathName.ToLower().EndsWith(".gdb")) {
					strRet = "F";
				}
				else if(WorkSpace.PathName.ToLower().EndsWith(".mdb")) {
					strRet = "P";
				}
				else if(WorkSpace.IsDirectory() && WorkSpace.Exists()) {
					strRet = "R";
				}
			}
			
			// 返却
			return strRet;
		}
		
		/// <summary>
		/// 指定ラスターの有無を確認します
		/// </summary>
		/// <param name="RasterWorkSpace"></param>
		/// <param name="RasterName"></param>
		/// <returns>有 / 無</returns>
		private bool IsExistRaster(IWorkspace RasterWorkSpace, string RasterName) {
			bool	blnRet = false;
		
			// 入力ﾁｪｯｸ
			if(!(RasterWorkSpace == null || string.IsNullOrEmpty(RasterName))) {
				IEnumDatasetName	agEnumDSNames = RasterWorkSpace.get_DatasetNames(esriDatasetType.esriDTRasterDataset);
				IDatasetName		agDSName;
				
				while((agDSName = agEnumDSNames.Next()) != null) {
					if(agDSName.Name.Equals(RasterName)) {
						blnRet = true;
						break;
					}
				}
				
				// COM解放
				Marshal.ReleaseComObject(agEnumDSNames);
			}
		
			// 返却
			return blnRet;
		}

	}
	
	
#region コンボボックス・リストアイテム クラス
	/// <summary>
	/// コンボボックス・リストアイテム クラス
	/// </summary>
	internal class ObjectComboItem {
		private string	_strText = "";
		private object	_objValue = null;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ObjectComboItem(string DisplayText, object ItemValue) {
			// 引数を保存
			this._strText = DisplayText;
			this._objValue = ItemValue;
		}
		
		/// <summary>
		/// 値を取得または設定します
		/// </summary>
		public object ItemValue {
			get {
				return this._objValue;
			}
			set {
				this._objValue = value;
			}
		}
		
		/// <summary>
		/// 表示テキストを取得または設定します
		/// </summary>
		public string DisplayText {
			get {
				return this._strText;
			}
			set {
				this._strText = value;
			}
		}
		
		/// <summary>
		/// コンボボックスでの表示テキストを取得します
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
 			 return this._strText;
		}
	}
#endregion
}
