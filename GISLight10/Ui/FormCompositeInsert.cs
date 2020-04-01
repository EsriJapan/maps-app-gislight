using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Ui {
	public partial class FormCompositeInsert : Form {
		private MainForm	_frmMain;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="TargetCompositeLayer">目的のグループレイヤー</param>
		/// <param name="Main">メインフォーム</param>
		public FormCompositeInsert(ILayer TargetCompositeLayer, MainForm Main) {
			InitializeComponent();
			
			// ﾂﾘｰ･ﾋﾞｭｰを構築
			this.layerTreeView1.TargetMap = Main.MapControl.Map;
			this.layerTreeView1.TargetLayer = TargetCompositeLayer as ICompositeLayer;
			
			// ﾌｫｰﾑを保存
			this._frmMain = Main;
		}

		/// <summary>
		/// 「閉じる」ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Close_Click(object sender, EventArgs e) {
			this.Close();
		}

		/// <summary>
		/// 「OK」ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_OK_Click(object sender, EventArgs e) {
			// 現在のﾏｯﾌﾟ範囲を保存
			IEnvelope	agEnv = this._frmMain.MapExtent;
			
			// 実行ﾁｪｯｸ
			//if(this.layerTreeView1.CheckGroupinCondition()) {
				// 実行
				//this.layerTreeView1.CommitLayers();
				this.layerTreeView1.CommitAllLayers();
				
				// 描画更新
				this._frmMain.MapControl.ActiveView.ContentsChanged();
				this._frmMain.MapExtent = agEnv;
				this._frmMain.MapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
				
				// 閉じる
				this.Close();
			//}
			//else {
			//	ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining("レイヤ設定が変更されていません。");
			//}
		}

		/// <summary>
		/// フォーム・ロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Load(object sender, EventArgs e) {
			// 設定ﾊﾟﾈﾙを隠す
			this.panel_Option.Left = this.linkLabel_Option.Left + this.linkLabel_Option.Width + 14;
			this.panel_Option.Top = this.linkLabel_Option.Top + this.linkLabel_Option.Height - this.panel_Option.Height + 9;
			this.panel_Option.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			this.panel_Option.Visible = false;
			
			// 初期値設定
			this.radioButton1.Checked = false;
			
			// 説明を表示
			this.ShowDescription(this.radioButton1.Checked);

			// 指定ﾓｰﾄﾞ(ﾄﾞﾗｯｸﾞ&ﾄﾞﾛｯﾌﾟ/ﾁｪｯｸﾎﾞｯｸｽ)
			this.layerTreeView1.CanDragDrop = !this.radioButton1.Checked;
		}

#region 指定方法の設定
		// 設定ﾊﾟﾈﾙを表示
		private void linkLabel_Option_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			// 設定ｵﾌﾟｼｮﾝの表示切替
			this.panel_Option.Visible = !this.panel_Option.Visible;

			// 現在の設定を復元
			if(this.panel_Option.Visible) {
				this.radioButton2.Checked = this.layerTreeView1.CanDragDrop;
			}
		}

		// 設定の確定
		private void linkLabel_OK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			// 設定をｺﾐｯﾄ
			this.layerTreeView1.CanDragDrop = this.radioButton2.Checked;
			this.panel_Option.Visible = false;
		}
		
		// 操作説明を表示します
		private void ShowDescription(bool IsCheck) {
			if(IsCheck) {
				this.label_Desc1.Text = string.Format("［{0}］に含めるレイヤにチェックを入れてください。", (this.layerTreeView1.TargetLayer as ILayer).Name);
				this.label_Desc2.Text = "グループから除外する場合はチェックを外してください。";
			}
			else {
				this.label_Desc1.Text = string.Format("［{0}］のレイヤ構成を変更します。", (this.layerTreeView1.TargetLayer as ILayer).Name);
				this.label_Desc2.Text = "ドラッグ＆ドロップでレイヤ構成を調整できます。";
			}
		}
	}
#endregion
}
