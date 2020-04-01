using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.GISClient;

namespace ESRIJapan.GISLight10.Ui {
	public partial class FormArcGISServerConnect : Form {
		public FormArcGISServerConnect() {
			InitializeComponent();
		}

#region プロパティ
		/// <summary>
		/// インターネット・サーバーかどうかを取得または設定します
		/// </summary>
		public bool IsInternet {
			get {
				return this.radioButtonI.Checked;
			}
			set {
				this.radioButtonI.Checked = value;
				this.radioButtonL.Checked = !value;
			}
		}
		
		/// <summary>
		/// サーバーURLまたはサーバー名を取得または設定します
		/// </summary>
		public string ServerURL {
			get {
				string	strRet;
				if(this.IsInternet) {
					strRet = this.textBox_InternetURL.Text;
				}
				else {
					strRet = this.textBox_LocalHost.Text;
				}
				return strRet;
			}
			set {
				if(this.IsInternet) {
					this.textBox_InternetURL.Text = value ?? "";
				}
				else {
					this.textBox_LocalHost.Text = value ?? "";
				}
			}
		}
		
		/// <summary>
		/// ユーザ名とパスワードを保存するかどうかを取得または設定します
		/// </summary>
		public bool IsSavePassword {
			get {
				return this.checkBox_SaveUser.Checked;
			}
			set {
				this.checkBox_SaveUser.Checked = value;
			}
		}
		
		/// <summary>
		/// サーバー接続プロパティを取得します
		/// </summary>
		public IPropertySet ConnectProperties {
			get {
				// 接続ﾌﾟﾛﾊﾟﾃｨを作成
				return this.GetConnectProperties(this.checkBox_SaveUser.Checked);
			}
		}
		
		/// <summary>
		/// サーバー接続プロパティを作成します
		/// </summary>
		/// <param name="IsAuth"></param>
		/// <returns></returns>
		private IPropertySet GetConnectProperties(bool IsAuth) {
			IPropertySet	agProp = new PropertySetClass();

			// 接続ﾌﾟﾛﾊﾟﾃｨをｾｯﾄ
			if(this.IsInternet) {
				agProp.SetProperty("url", this.textBox_InternetURL.Text);
			}
			else {
				agProp.SetProperty("machine", this.textBox_LocalHost.Text);
			}
			
			// 認証情報追加
			string	strUser = this.textBox_User.Text.Trim();
			if(!strUser.Equals("") && IsAuth) {
				agProp.SetProperty("User", strUser);
				agProp.SetProperty("Password", this.textBox_Psw.Text);
			}
			
			return agProp;
		}

#endregion

		/// <summary>
		/// フォーム・ロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Load(object sender, EventArgs e) {
			// ｺﾝﾄﾛｰﾙの初期化
			this.radioButtonI.Checked = true;
			this.checkBox_SaveUser.Checked = true;
			textBox_User_TextChanged(this.textBox_User, EventArgs.Empty);
		}

		/// <summary>
		/// ラジオ選択変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Server_CheckedChanged(object sender, EventArgs e) {
			RadioButton	ctlRD = sender as RadioButton;
			
			// 入力項目の制御
			if(ctlRD.Name.EndsWith("I")) {
				if(ctlRD.Checked) {
					this.panelI.Enabled = true;
					this.panelL.Enabled = false;
				}
			}
			else if(ctlRD.Name.EndsWith("L")) {
				if(ctlRD.Checked) {
					this.panelI.Enabled = false;
					this.panelL.Enabled = true;
				}
			}
		}

		/// <summary>
		/// ユーザー名保存
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox_User_TextChanged(object sender, EventArgs e) {
			TextBox	ctlTB = sender as TextBox;
			
			// ﾕｰｻﾞｰ名保存ｵﾌﾟｼｮﾝを制御
			this.checkBox_SaveUser.Enabled = (ctlTB.Text.Length > 0);
		}

		/// <summary>
		/// 接続テスト
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Test_Click(object sender, EventArgs e) {
			// 入力ﾁｪｯｸ
			if(this.CheckInputParams()) {
				// 接続設定
				IAGSServerConnectionFactory2	agGISSvrConnFact = Common.SingletonUtility.NewAGSServerConnectionFactory() as IAGSServerConnectionFactory2;
				IPropertySet					agProp = this.GetConnectProperties(true);
				
				StringBuilder	sbMsg = new StringBuilder();

				// ｻｰﾊﾞｰに接続
				try {
					IAGSServerConnection		agGISSvrConn = agGISSvrConnFact.Open(agProp, this.Handle.ToInt32());

					sbMsg.AppendLine("接続に成功。" + Environment.NewLine);

					// ｻｰﾊﾞｰ･ｵﾌﾞｼﾞｪｸﾄを取得
					IAGSEnumServerObjectName	agEnumSOName = agGISSvrConn.ServerObjectNames;
					IAGSServerObjectName3		agSOName;

					while((agSOName = agEnumSOName.Next() as IAGSServerObjectName3) != null) {
						// 必須のﾏｯﾌﾟｻｰﾊﾞｰでﾂﾘｰ構成
						if(agSOName.Type == "MapServer") {
							sbMsg.AppendFormat("{0} : {1}\r\n", agSOName.Name, agSOName.URL);
						}
						else if(agSOName.Type == "FeatureServer") {
							
						}
						else if(agSOName.Type == "WMSServer") {
							
						}
						else if(agSOName.Type == "WFSServer") {
							
						}
						else if(agSOName.Type == "KmlServer") {
							
						}
						else if(agSOName.Type == "ImageServer") {
							
						}
					}
				}
				catch(Exception ex) {
					sbMsg.Append("接続に失敗 : " + ex.Message);
				}
				
				// 結果表示
				Common.MessageBoxManager.ShowMessageBoxInfo(this, sbMsg.ToString());
			}
			else {
				// 警告
				Common.MessageBoxManager.ShowMessageBoxWarining("入力内容を確認してください。");
			}
		}
		
		private bool CheckInputParams() {
			bool	blnRet = false;
			
			// ｻｰﾊﾞｰ指定
			if(!(this.textBox_InternetURL.Text.Trim().Equals("") && this.textBox_LocalHost.Text.Trim().Equals(""))) {
				// 入力補填
				if(this.radioButtonI.Checked) {
					string	strURL = this.textBox_InternetURL.Text.Trim();
					if(!strURL.StartsWith("http")) {
						this.textBox_InternetURL.Text = "http://" + strURL;
					}
				}
				
				// 認証
				if(!(!this.textBox_User.Text.Trim().Equals("") && this.textBox_User.Text.Trim().Equals(""))) {
					blnRet = true;
				}
			}
			
			return blnRet;
		}
		
	}
}
