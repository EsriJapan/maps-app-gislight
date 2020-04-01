using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF;

/*
 * クラス構成
 * 1. QueryTableOperator (クエリ テーブル操作用クラス)
 * 2. UserSelectQueryTableSet (クエリーテーブル選択パッケージ・クラス)
*/
namespace ESRIJapan.GISLight10.Common {
	/// <summary>
	/// クエリ テーブル操作用クラス
	/// </summary>
	class QueryTableOperator {
		private ISqlWorkspace		_agSqlWS = null;
		private string				_strTblName = null;
		private string				_strQueryString = null;
		private string				_strOIDFields = "";
		
		private List<string>		_strRelFields = new List<string>();
		private List<ListViewItem>	_listViewItems = new List<ListViewItem>();

		#region プロパティ
		/// <summary>
		/// 選択テーブル名を取得または設定します
		/// </summary>
		public string TableName {
			get {
				return this._strTblName;
			}
			set {
				if(this._strTblName != value) {
					// 更新
					this._strTblName = value;
					
					// ﾌﾟﾛﾊﾟﾃｨを更新
					this.CreateProperties(value);
				}
			}
		}
		/// <summary>
		/// OIDフィールド設定を
		/// </summary>
		public string OIDFields {
			get {
				return this._strOIDFields;
			}
			set {
				this._strOIDFields = value;
			}
		}
		
		/// <summary>
		/// クエリ文字列を取得します
		/// </summary>
		public string QueryString {
			get {
				return this._strQueryString;
			}
		}
		
		/// <summary>
		/// OIDフィールド選択用リストアイテム項目を取得します
		/// </summary>
		public ListViewItem[] OIDFieldsSelectListItems {
			get {
				return this._listViewItems.ToArray();
			}
		}
		
		/// <summary>
		/// テーブル結合に使用可能なフィールド(群)を取得します
		/// </summary>
		public string[]	RelateableFields {
			get {
				return this._strRelFields.ToArray();
			}
		}
		#endregion
		
		#region メソッド
		/// <summary>
		/// 選択テーブル情報に基づいてクエリテーブルを取得します
		/// </summary>
		/// <returns></returns>
		public ITable GetTable(string AliasName) {
			ITable	agTbl = null;
			
			try {
				// ｸｴﾘ仕様を取得
				IQueryDescription	agQDesc = this._agSqlWS.GetQueryDescription(this._strQueryString);
				
				// OIDﾌｨｰﾙﾄﾞを補完
				if(string.IsNullOrEmpty(agQDesc.OIDFields)) {
					agQDesc.OIDFields = this._strOIDFields;
				}
				
				// ﾃｰﾌﾞﾙを取得
				agTbl = this._agSqlWS.OpenQueryClass(this._strTblName, agQDesc);

				// 別名の設定
				if(!string.IsNullOrEmpty(AliasName)) {
					IDataset	agDS = agTbl as IDataset;
					agDS.BrowseName = AliasName;
				}
			}
			catch(Exception ex) {
#if DEBUG
				Debug.WriteLine(ex.Message);
#endif
			}
			
			return agTbl;
		}
		
		/// <summary>
		/// クエリに基づいてテーブル・カーソルを取得します
		/// </summary>
		/// <returns></returns>
		public ICursor GetTableCursor() {
			ICursor	agCur = null;
			
			try {
				agCur = this._agSqlWS.OpenQueryCursor(this._strQueryString);
			}
			catch(Exception ex) {
#if DEBUG
				Debug.WriteLine(ex.Message);
#endif
			}
			
			return agCur;
		}
		
		/// <summary>
		/// フィールド情報を取得します
		/// </summary>
		/// <param name="TableName"></param>
		/// <param name="ColumnNames"></param>
		/// <param name="ColumnTypes"></param>
		/// <param name="IsNulls"></param>
		/// <param name="Sizes"></param>
		/// <param name="Precisions"></param>
		/// <param name="Scales"></param>
		public void GetColumnInfos(string TableName, 
			out IStringArray ColumnNames, out IStringArray ColumnTypes, 
			out IVariantArray IsNulls, 
			out ILongArray Sizes, out ILongArray Precisions, out ILongArray Scales) {
			
			// 出力ﾊﾟﾗﾒｰﾀの初期化
			ColumnNames = new StrArrayClass();
			ColumnTypes = new StrArrayClass();
			IsNulls = new VarArrayClass();
			Sizes = new LongArrayClass();
			Precisions = new LongArrayClass();
			Scales = new LongArrayClass();
			
			try {
				// ﾌｨｰﾙﾄﾞ情報を取得
				this._agSqlWS.GetColumns(TableName, 
					out ColumnNames, out ColumnTypes, out IsNulls, out Sizes, out Precisions, out Scales);
			}
			catch(Exception ex) {
#if DEBUG
				Debug.WriteLine(ex.Message);
#endif
			}
		}
		
		/// <summary>
		/// DB接続ファイルの有効性を確認します
		/// </summary>
		/// <param name="ConnectionFile"></param>
		/// <returns></returns>
		static public bool IsValidConnectionFile(string ConnectionFile) {
			bool	blnRet = false;
			
			if(!string.IsNullOrEmpty(ConnectionFile)) {
				// ﾌｧｲﾙの有無のみﾁｪｯｸ
				if(System.IO.File.Exists(ConnectionFile)) {
					blnRet = true;
				}
			}
			
			return blnRet;
		}
		
		/// <summary>
		/// クエリ・テーブルの完全パスから接続ファイル部分のパスを抽出します ※接続ファイルが存在しない場合は、空文字。
		/// </summary>
		/// <param name="DataSourcePath"></param>
		/// <returns></returns>
		static public string GetConnectionFilePath(string DataSourcePath) {
			string	strRet = DataSourcePath;
			
			// ﾊﾟｽ(接続ﾌｧｲﾙ･ﾊﾟｽのみ)か確認
			if(!IsValidConnectionFile(DataSourcePath) && !string.IsNullOrEmpty(strRet)) {
				// ﾊﾟｽを分割
				string[]	strSegs = DataSourcePath.Split('\\');
				
				// 表示名の場合
				if(strSegs.Length <= 1) {
					strRet = "";
				}
				// 
				else {
					// 最後尾(ﾃｰﾌﾞﾙ名)をｶｯﾄ
					strRet = string.Join("\\", strSegs.Take(strSegs.Length - 1).ToArray());

					// ﾌｧｲﾙの有無を確認
					if(!IsValidConnectionFile(strRet)) {
						strRet = "";
					}
				}
			}
			
			return strRet;
		}
		
		/// <summary>
		/// 指定のテーブルが、クエリテーブルかどうか判定します
		/// </summary>
		/// <param name="TargetTable"></param>
		/// <returns></returns>
		static public bool IsQueryTable(ITable TargetTable) {
			// SqlWorkspaceを確認
			return LoadWorkspace(TargetTable) != null;
		}
		#endregion
		
		/// <summary>
		/// コンストラクタ（ワークスペースから、クエリテーブルの概要を構築するパターン）
		/// </summary>
		public QueryTableOperator(ISqlWorkspace SqlWorkspace) {
			// ﾜｰｸｽﾍﾟｰｽを保存
			this._agSqlWS = SqlWorkspace;
			
			// ※この後、TableNameプロパティにテーブル名をセットして構築すること。
		}
		/// <summary>
		/// コンストラクタ（既存のテーブル・インスタンスから、クエリテーブルの概要を構築するパターン）
		/// ※テーブル名に別名を指定していた場合は、NG。
		/// </summary>
		/// <param name="TargetTable"></param>
		public QueryTableOperator(ITable TargetTable) {
			// 基本情報を構築
			this.CreateProperties(TargetTable);
		}
		
		/// <summary>
		/// デストラクタ
		/// </summary>
		~QueryTableOperator() {
			// ﾜｰｸｽﾍﾟｰｽを解放 (呼び出し元で実行)
			//ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(_agSqlWS);
		}
		
		private void InitProperties() {
			// ｸｴﾘｰ文字列をｸﾘｱ
			this._strQueryString = "";
			
			// OID情報をｸﾘｱ
			this._strOIDFields = "";
			
			// ﾘｽﾄ･ｱｲﾃﾑをｸﾘｱ
			this._listViewItems.Clear();
			
			// 結合情報をｸﾘｱ
			this._strRelFields.Clear();
		}
		
		/// <summary>
		/// クエリテーブルの基本情報を構築します
		/// ・テーブル概要作成の素となるクエリ文の作成
		/// ・既定のOIDを勝手に定義
		/// ・OIDフィールド選択用リストアイテムの作成
		/// ・テーブル結合のキー選択候補となるフィールド名の抽出
		/// </summary>
		/// <param name="ColumnNames"></param>
		/// <param name="ColumnTypes"></param>
		/// <param name="IsNulls"></param>
		/// <param name="Sizes"></param>
		/// <param name="Precisions"></param>
		/// <param name="Scales"></param>
		private void CreateBaseQueryInfo(IStringArray ColumnNames, IStringArray ColumnTypes, IVariantArray IsNullables, 
			ILongArray Sizes, ILongArray Precisions, ILongArray Scales) {
			
			// ﾌｫｰﾑを構成
			bool				blnExistOID = false;
			bool				blnIsNullable = false;
			string				strColName;
			string				strFldType;
			StringBuilder		sbQuery = new StringBuilder("SELECT ");
			for(int intCnt=0; intCnt < ColumnNames.Count; intCnt++) {
				// 列名を取得
				strColName = ColumnNames.get_Element(intCnt);
				
				// ﾌｨｰﾙﾄﾞ型を取得
				strFldType = ColumnTypes.get_Element(intCnt);
				
				// OIDに有効なﾌｨｰﾙﾄﾞかどうか判定 → ﾘｽﾄ作成
				if(this.IsAbleToOIDField(strFldType)) {
					// Null可否取得
					blnIsNullable = IsNullables.get_Element(intCnt).Equals(true);
					
					// ﾌｨｰﾙﾄﾞ項目を作成
					ListViewItem	lviNew = new ListViewItem(new string[] { "", strColName, strFldType, (blnIsNullable ? "はい" : "いいえ") }, 0);
					if(!blnIsNullable && !blnExistOID) {
						lviNew.Checked = blnExistOID = true;
						
						// OIDｾｯﾄ
						this._strOIDFields = strColName;
					}

					// ﾘｽﾄに追加
					this._listViewItems.Add(lviNew);
				}
				
				// ﾃｰﾌﾞﾙ結合に有効なﾌｨｰﾙﾄﾞかどうか判定 → 抽出
				if(this.IsAbleToRelateField(strFldType)) {
					this._strRelFields.Add(strColName);
				}
				
				// ﾃｰﾌﾞﾙから除外するﾌｨｰﾙﾄﾞ(ｼﾞｵﾒﾄﾘ型, ﾊﾞｲﾅﾘ型)
				if(!(strFldType.ToLower().Equals("geometry") || strFldType.ToLower().Equals("blob"))) {
					// ｸｴﾘｰに追加
					sbQuery.Append(strColName + ",");
				}
				else {
					// ｼﾞｵﾒﾄﾘ型ﾌｨｰﾙﾄﾞを検出した場合、ArcMapではﾃﾞｰﾀを言及され、FCとして実装される
					// IQueryDescription - GeometryType, SpatialReference, Srid をｾｯﾄ
#if DEBUG
					Debug.WriteLine("Geometry Field : " + strColName);
#endif
				}
			}
			
			// ｸｴﾘｰを調整
			if(sbQuery.Length > 0) {
				sbQuery.Length -= 1;
				sbQuery.Append(" FROM " + TableName);
				
				// ﾌﾟﾛﾊﾟﾃｨにｾｯﾄ
				this._strQueryString = sbQuery.ToString();
			}
			
			// ﾃﾞﾌｫﾙﾄOIDがｾｯﾄできなかった場合 (ﾃｰﾌﾞﾙ結合等の機能向け･ITableをどうしても取得したい時用)
			if(string.IsNullOrEmpty(this._strOIDFields)) {
				// そもそもOID候補がない場合は諦める
				if(this._listViewItems.Count > 0) {
					this._strOIDFields = this._listViewItems[0].SubItems[1].Text;
				}
			}
		}
		
		/// <summary>
		/// プロパティを構築します
		/// </summary>
		/// <param name="TableName">テーブル名</param>
		private void CreateProperties(string TableName) {
			// ﾌﾟﾛﾊﾟﾃｨ情報をｸﾘｱ
			this.InitProperties();
			
			// ﾌｨｰﾙﾄﾞ情報を取得
			IStringArray	agSA_ColNames = new StrArrayClass();
			IStringArray	agSA_ColTypes = new StrArrayClass();
			IVariantArray	agVA_IsNulls = new VarArrayClass();
			ILongArray		agLA_Sizes = new LongArrayClass();
			ILongArray		agLA_Precs = new LongArrayClass();
			ILongArray		agLA_Scales = new LongArrayClass();
			
			this.GetColumnInfos(TableName, 
				out agSA_ColNames, out agSA_ColTypes, out agVA_IsNulls, out agLA_Sizes, out agLA_Precs, out agLA_Scales);
			
			// ﾌﾟﾛﾊﾟﾃｨを構築
			this.CreateBaseQueryInfo(agSA_ColNames, agSA_ColTypes, agVA_IsNulls, agLA_Sizes, agLA_Precs, agLA_Scales);
		}
		
		/// <summary>
		/// プロパティを構築します（既存テーブルから逆に構築する場合）
		/// </summary>
		/// <param name="TableInstance"></param>
		private void CreateProperties(ITable TableInstance) {
			// ﾌﾟﾛﾊﾟﾃｨ情報をｸﾘｱ
			this.InitProperties();

			// ﾜｰｸｽﾍﾟｰｽを取得
			this._agSqlWS = LoadWorkspace(TableInstance);
			
			// ﾃｰﾌﾞﾙの有無をﾁｪｯｸ
			if(this._agSqlWS != null && this.ContainsTableName((TableInstance as IDataset).Name)) {
				// ﾌｨｰﾙﾄﾞ情報を取得
				IStringArray	agSA_ColNames = new StrArrayClass();
				IStringArray	agSA_ColTypes = new StrArrayClass();
				IVariantArray	agVA_IsNulls = new VarArrayClass();
				ILongArray		agLA_Sizes = new LongArrayClass();
				ILongArray		agLA_Precs = new LongArrayClass();
				ILongArray		agLA_Scales = new LongArrayClass();

				IFields	agFlds = TableInstance.Fields;
				FieldManager.GetTableInfosArray(agFlds, out agSA_ColNames, out agSA_ColTypes, out agVA_IsNulls, 
					out agLA_Sizes, out agLA_Precs, out agLA_Scales);
				
				// ﾃｰﾌﾞﾙ名を設定
				this._strTblName = (TableInstance as IDataset).Name;
				
				// ﾌﾟﾛﾊﾟﾃｨを構築
				this.CreateBaseQueryInfo(agSA_ColNames, agSA_ColTypes, agVA_IsNulls, agLA_Sizes, agLA_Precs, agLA_Scales);
			}
		}
		
		/// <summary>
		/// 指定のテーブルがワークスペースに存在するかチェックします
		/// </summary>
		/// <param name="TableName"></param>
		/// <returns></returns>
		private bool ContainsTableName(string TableName) {
			bool	blnRet = false;
			
			// 入力ﾁｪｯｸ
			if(this._agSqlWS != null && !string.IsNullOrEmpty(TableName)) {
				// ﾜｰｸｽﾍﾟｰｽ内のﾃｰﾌﾞﾙを取得
				IStringArray	agStrs = this._agSqlWS.GetTables();
				string			strName;
				
				// ﾃｰﾌﾞﾙ名を探索
				for(int intCnt=0; intCnt < agStrs.Count; intCnt++) {
					strName = agStrs.get_Element(intCnt);
					if(strName == TableName) {
						blnRet = true;
						break;
					}
				}
			}
			
			return blnRet;
		}
		
		/// <summary>
		/// テーブル・インスタンスから、ワークスペースを取得します
		/// </summary>
		/// <param name="TableInstance"></param>
		/// <returns></returns>
		static private ISqlWorkspace LoadWorkspace(ITable TableInstance) {
			ISqlWorkspace	agSqlWS = null;
			
			// 入力ﾁｪｯｸ
			if(TableInstance != null) {
				IDataset	agDS = TableInstance as IDataset;
				if(agDS.Workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace
					 && agDS.Workspace is ISqlWorkspace) {
					agSqlWS = agDS.Workspace as ISqlWorkspace;
				}
			}
			
			return agSqlWS;
		}
		
		/// <summary>
		/// OIDフィールドに有効なフィールド型かどうかを判定します
		/// </summary>
		/// <param name="FieldType"></param>
		/// <returns></returns>
		private bool IsAbleToOIDField(string FieldType) {
			bool	blnRet = false;
			
			if(!string.IsNullOrEmpty(FieldType)) {
				// 有効なﾌｨｰﾙﾄﾞ型を判定 (ﾃｷｽﾄ または 整数)
				if(FieldType.ToLower().Equals("text") || FieldType.ToLower().Contains("integer")) {
					blnRet = true;
				}
			}
			
			return blnRet;
		}
		
		/// <summary>
		/// テーブル結合に有効なフィールド型かどうかを判定します
		/// </summary>
		/// <param name="FieldType"></param>
		/// <returns></returns>
		private bool IsAbleToRelateField(string FieldType) {
			bool	blnRet = false;
			
			if(!string.IsNullOrEmpty(FieldType)) {
				// 有効なﾌｨｰﾙﾄﾞ型を判定 (数値型)
				if(FieldType.ToLower().Contains("integer")
					|| FieldType.ToLower().Equals("double") || FieldType.ToLower().Equals("float")
					|| FieldType.ToLower().Equals("text")) {
					blnRet = true;
				}
			}
			
			return blnRet;
		}

	}

    /// <summary>
    /// クエリーテーブル選択パッケージ・クラス
    /// </summary>
    public class UserSelectQueryTableSet {
		/// <summary>
		/// テーブル名を取得または設定します
		/// </summary>
		public string		TableName		{ get; set; }
		/// <summary>
		/// DB接続情報を取得または設定します
		/// </summary>
		public IPropertySet	ConnectProperty	{ get; set; }
		public string		ConnectionFile	{ get; set; }
    }
}
