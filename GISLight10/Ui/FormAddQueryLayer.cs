using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Ui {
	public partial class FormAddQueryLayer : Form {
		
		private ISqlWorkspace				_agSqlWS = null;
		private Common.QueryTableOperator	_objQTO = null;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="SqlWorkSpace"></param>
		public FormAddQueryLayer(ISqlWorkspace SqlWorkSpace) {
			InitializeComponent();
			
			// ﾜｰｸｽﾍﾟｰｽを保存
			this._agSqlWS = SqlWorkSpace;
			
			// 処理単位を生成
			_objQTO = new ESRIJapan.GISLight10.Common.QueryTableOperator(this._agSqlWS);
		}

		#region プロパティ
		/// <summary>
		/// クエリーテーブル情報を設定します
		/// </summary>
		public string TableName {
			set {
				this._objQTO.TableName = value;
				
				// 選択ﾌｨｰﾙﾄﾞ項目を取得
				if(this._objQTO.OIDFieldsSelectListItems.Count() > 0) {
					this.listView_Fields.Items.AddRange(this._objQTO.OIDFieldsSelectListItems);
					this.listView_Fields.Enabled = true;
				}
				else {
					this.listView_Fields.Enabled = false;
					Common.MessageBoxManager.ShowMessageBoxWarining("指定したデータ オブジェクトをマップに追加できませんでした。");
				}
			}
		}
		/// <summary>
		/// ユーザーに選択されたOIDフィールド名を取得または設定します
		/// </summary>
		public string OIDFields {
			get {
				return this._objQTO.OIDFields;
			}
			set {
				this._objQTO.OIDFields = value;
			}
		}
		/// <summary>
		/// テーブル照会用クエリーを取得します ※Geometryﾌｨｰﾙﾄﾞ抜き
		/// </summary>
		public string QueryString {
			get {
				return this._objQTO.QueryString;
			}
		}
		#endregion
		
		#region メソッド
		/// <summary>
		/// 設定に基づき、テーブルを取得します
		/// </summary>
		/// <param name="AliasTableName">別名（Null指定時は、本来のテーブル名で取得します）</param>
		/// <returns></returns>
		public ITable GetQueryTable(string AliasTableName) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return this._objQTO.GetTable(AliasTableName);
		}
		
		/// <summary>
		/// 設定に基づき、テーブル・カーソルを取得します
		/// </summary>
		/// <returns></returns>
		public ICursor GetQueryTableCursor() {
			return this._objQTO.GetTableCursor();
		}
		#endregion

		private void Form_Load(object sender, EventArgs e) {

		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView_Fields_EnabledChanged(object sender, EventArgs e) {
			ListView	ctlLV = sender as ListView;
			
			this.button_OK.Enabled = ctlLV.Enabled;
		}

		/// <summary>
		/// 「OK」クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_OK_Click(object sender, EventArgs e) {
			// 選択ﾌｨｰﾙﾄﾞを取得
			List<string>	strFlds = new List<string>();
			foreach(ListViewItem lviTemp in this.listView_Fields.Items) {
				if(lviTemp.Checked) {
					strFlds.Add(lviTemp.SubItems[1].Text);
				}
			}
			
			// ﾌﾟﾛﾊﾟﾃｨの設定
			this._objQTO.OIDFields = string.Join(",", strFlds.ToArray());
			
			if(strFlds.Count > 0) {
				this.DialogResult = DialogResult.OK;
			}
			else {
				// 警告ﾒｯｾｰｼﾞを表示してﾕｰｻﾞｰ選択を継続
				Common.MessageBoxManager.ShowMessageBoxWarining(
					"フィーチャ レイヤに必要な一意の識別子が指定されていません。" +
					"操作を続けるには、使用可能なフィールドのリストから一意の識別子を選択してください。"
				);
			}
		}

		/// <summary>
		/// 「キャンセル」ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Cancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}
	}
}
