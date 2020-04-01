using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.Ui {
	public partial class FormOLEDBConnect : Form {
		// 接続DB ※10.1になったらOracle10/11の区別不要
		private string[]	_strDBSystems = {"SQL Server", "Oracle10g", "Oracle11g"};
		private string[]	_strDBClients = {"SQLServer", "Oracle10g", "Oracle11g", "PostgreSQL", "Informix", "DB2", "DB2ZOS"};
		// 認証方法
		private string[]	_strAuthTypes = {"データベース認証", "オペレーティング システム認証"};
		private string[]	_strAuthModes = {"DBMS", "OSA"};
		
		private IMapControl3	_agMapCtl = null;			// ※破棄する予定
		
		private ISqlWorkspace	_agDBWS = null;				// 設定ﾜｰｸｽﾍﾟｰｽ
		private bool			_blnIsSavePsw = false;		// ﾊﾟｽﾜｰﾄﾞ保存指定
		
		private bool			_blnSQLDBList = false;		// SQLDB時、ﾃﾞｰﾀﾍﾞｰｽ･ﾘｽﾄの取得制御ﾌﾗｸﾞ
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FormOLEDBConnect() {
			InitializeComponent();

			// 選択項目の設定
			this.comboBox_DBSystem.Items.AddRange(_strDBSystems);
			this.comboBox_DBSystem.SelectedIndex = 0;
			
			this.comboBox_AuthType.Items.AddRange(_strAuthTypes);
			this.comboBox_AuthType.SelectedIndex = 0;

			// ﾎﾞﾀﾝ制御
			this.button_TblList.Enabled = false;
		}

		#region プロパティ
		/// <summary>
		/// 設定対象のワークスペースを取得または設定します
		/// </summary>
		public ISqlWorkspace DBWorkspace {
			get {
				return _agDBWS;
			}
			set {
				_agDBWS = value;
				
				if(this.Visible) {
					// 接続ﾌﾟﾛﾊﾟﾃｨを表示
					IPropertySet	agPropSet = (_agDBWS as IWorkspace).ConnectionProperties;
					object objVal = agPropSet.GetProperty("dbclient");
					if(objVal != null && _strDBClients.Contains(objVal)) {
						this.comboBox_DBSystem.SelectedValue = objVal;
					}
					
					
					// ﾊﾟｽﾜｰﾄﾞ
					
				}
			}
		}
		
		/// <summary>
		/// パスワード保存設定を取得します
		/// </summary>
		public bool IsSavePassword {
			get {
				return this._blnIsSavePsw;
			}
		}
		
		// 地図ｺﾝﾄﾛｰﾙ (暫定)
		public IMapControl3 MapControl {
			set {
				_agMapCtl = value;
				
				// 単独ﾃｰﾌﾞﾙを取得
				IStandaloneTableCollection	agStdTbls = null;
				if(_agMapCtl != null && this._agMapCtl.Map != null) {
					agStdTbls = this._agMapCtl.Map as IStandaloneTableCollection;
				}
				
				// ﾎﾞﾀﾝ制御
				this.button_TblList.Enabled = agStdTbls != null && agStdTbls.StandaloneTableCount > 0;
			}
		}
		#endregion

		/// <summary>
		/// フォーム・ロード イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Load(object sender, EventArgs e) {
			// SQLDB DB一覧取得ﾌﾗｸﾞをｸﾘｱ
			_blnSQLDBList = false;
		}

		/// <summary>
		/// 「DBの種類」選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DBSystem_SelectedIndexChanged(object sender, EventArgs e) {
			ComboBox	ctlCMB = sender as ComboBox;

			// DB一覧をｸﾘｱ
			this.ClearDBNameList();
			
			// ﾃﾞｰﾀﾍﾞｰｽ名設定の表示制御
			this.panel_DBName.Visible = ctlCMB.SelectedIndex >= 0;
		}

		/// <summary>
		/// 「認証方法」選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DBAuthType_SelectedIndexChanged(object sender, EventArgs e) {
			ComboBox	ctlCMB = sender as ComboBox;
			
			// DB認証の場合、認証情報を入力
			this.panel_DBAuth.Enabled = ctlCMB.SelectedIndex == 0;
			
			// 保存	
			this.checkBox_SaveAuth.Visible = this.panel_DBAuth.Enabled;
			
			// DB一覧をｸﾘｱ
			this.ClearDBNameList();
		}
		
		private void ClearDBNameList() {
			// DB一覧をｸﾘｱ
			if(this.panel_DBName.Visible) {
				this.comboBox_DBName.Items.Clear();
				this.comboBox_DBName.Text = "";
				
				// DB名取得ﾌﾗｸﾞをｸﾘｱ
				_blnSQLDBList = false;
			}
		}

		/// <summary>
		/// 「キャンセル」ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Cancel_Click(object sender, EventArgs e) {
			// ﾀﾞｲｱﾛｸﾞ返却値の初期化
			this.DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// 「OK」ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_OK_Click(object sender, EventArgs e) {
			// 入力状態をﾁｪｯｸ
			if(this.CheckParams()) {
				if(this.comboBox_DBSystem.SelectedIndex < 0 ||
					this.textBox_Instance.Text.Trim().Length <= 0 ||
					!this.IsValidAuth()) {
					ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining("データベース サーバーに接続してデータベース リストを取得することができませんでした。" + Environment.NewLine + "入力情報をご確認ください。");
				}
				else {
					// DB接続を作成して、DBﾘｽﾄを取得する
					//Type				factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SqlWorkspaceFactory");
					//IWorkspaceFactory	agWSF = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
					IWorkspaceFactory	agWSF = Common.SingletonUtility.NewSqlWorkspaceFactory();

					// 接続ﾌﾟﾛﾊﾟﾃｨ
					IPropertySet		agProp = new PropertySetClass();
					agProp.SetProperty("dbclient", _strDBClients[this.comboBox_DBSystem.SelectedIndex]);
					agProp.SetProperty("serverinstance", this.textBox_Instance.Text);
					
					// DB名
					object objVal = agProp.GetProperty("dbclient");
					if(!objVal.ToString().ToLower().StartsWith("oracle")) {
						agProp.SetProperty("database", this.comboBox_DBName.Text);
					}
					
					// ﾊﾟｽﾜｰﾄﾞ･保存ﾌﾟﾛﾊﾟﾃｨを設定
					this._blnIsSavePsw = this.checkBox_SaveAuth.Checked && this.comboBox_AuthType.SelectedIndex == 0;
					
					// 接続認証
					agProp.SetProperty("authentication_mode", _strAuthModes[this.comboBox_AuthType.SelectedIndex]);
					if(this.comboBox_AuthType.SelectedIndex == 0) {
						agProp.SetProperty("user", this.textBox_DBUser.Text);
						agProp.SetProperty("password", this.textBox_DBPsw.Text);
					}

					// DB接続
					try {
						IWorkspace	agWS = agWSF.Open(agProp, 0);
						
						this._agDBWS = agWS as ISqlWorkspace;
						
						this.DialogResult = DialogResult.OK;
					}
					catch(Exception ex) {
						Common.UtilityClass.DoOnError(ex);
					}
				}
			}
		}
		
		/// <summary>
		/// 入力状態をチェックします
		/// </summary>
		/// <returns></returns>
		private bool CheckParams() {
			string	strMsg = "";
			
			if(this.comboBox_DBSystem.SelectedIndex < 0) {
				strMsg = "データベースの種類を選択してください。";
			}
			else if(this.textBox_Instance.Text.Trim().Length <= 0) {
				strMsg = "インスタンスを入力してください。";
			}
			else if(!this.IsValidAuth()) {
				strMsg = "認証情報を入力してください。";
			}
			else if(this.panel_DBName.Visible && this.comboBox_DBName.Text.Trim().Length <= 0) {
				strMsg = "接続するデータベースを指定してください。";
			}
			
			// ﾒｯｾｰｼﾞ表示
			if(!strMsg.Equals("")) {
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(strMsg);
			}
			
			return strMsg == "";
		}
		
		/// <summary>
		/// 認証情報の入力状態をチェックします
		/// </summary>
		/// <returns></returns>
		private bool IsValidAuth() {
			bool	blnRet = true;
			
			// 認証情報の入力ﾁｪｯｸ
			if(this.comboBox_AuthType.SelectedIndex == 0) {
				if(this.textBox_DBUser.Text.Trim().Length <= 0 || this.textBox_DBPsw.Text.Trim().Length <= 0) {
					blnRet = false;
				}
			}
			
			return blnRet;
		}

		/// <summary>
		/// 「データベース」リストを取得します（SQL Serverのみ）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DBNames_Enter(object sender, EventArgs e) {
			// SQLServer選択時は、DBﾘｽﾄを取得
			if(!_blnSQLDBList && this.comboBox_DBSystem.SelectedIndex == 0) {
				if(this.textBox_Instance.Text.Trim().Equals("") || !this.IsValidAuth()) {
					// ﾒｯｾｰｼﾞ表示
					this.ShowDBInfo();
				}
				else {
					// 待ち表示
					this.Cursor = Cursors.WaitCursor;
					
					// DB接続文字列を作成
					StringBuilder	sbConn = new StringBuilder();
					sbConn.Append("Persist Security Info=False;");
					if(this.comboBox_AuthType.SelectedIndex == 0) {
						sbConn.AppendFormat("User ID={0};Password={1};", this.textBox_DBUser.Text.Trim(), this.textBox_DBPsw.Text);
					}
					else {
						sbConn.Append("Integrated Security=SSPI;");
					}
					
					sbConn.AppendFormat("Data Source={0};", this.textBox_Instance.Text.Trim());
					
					// ﾃﾞｰﾀﾍﾞｰｽ名の一覧を取得
					using(SqlConnection sqlCon = new SqlConnection(sbConn.ToString())) {
						SqlCommand	sqlCmd = sqlCon.CreateCommand();
						sqlCmd.CommandText = "SELECT name FROM sys.databases WHERE user_access IN (0, 1) AND state = 0ORDER BY name;";
						
						SqlDataReader	sqlDR;
						try {
							sqlCon.Open();
							sqlDR = sqlCmd.ExecuteReader();
							while(sqlDR.Read()) {
								this.comboBox_DBName.Items.Add(sqlDR[0]);
							}
							if(this.comboBox_DBName.Items.Count > 0) {
								//this.comboBox_DBName.SelectedIndex = 0;
							}
							_blnSQLDBList = true;
						}
						catch(Exception ex) {
#if DEBUG
							Debug.WriteLine("DB接続エラー : " + ex.Message);
#endif
							this.ShowDBInfo();
						}
						finally {
							sqlCon.Close();
						}
					}
					
					// 待ち解除
					this.Cursor = Cursors.Default;
				}
			}
		}
		
		// SQLDB接続エラーメッセージを表示します
		private void ShowDBInfo() {
			ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxInfo(this, 
				"データベース サーバーに接続してデータベース リストを取得することができません。" +
				"インスタンス名、ユーザ名、およびパスワードの情報を確認して、もう一度やり直してください。"
			);
		}

		private void button_TblList_Click(object sender, EventArgs e) {
			FormTableView	frmTblView = new FormTableView(_agMapCtl);
			frmTblView.ShowDialog();
		}
	}
}
