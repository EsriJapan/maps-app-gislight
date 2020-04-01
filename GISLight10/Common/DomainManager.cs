using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Common {
    /// <summary>
    /// ドメインを操作するクラス
    /// </summary>
    /// <history>
    ///  2012-07-31 新規作成
    /// </history>
	public class DomainManager {

		/// <summary>
		/// 既定のドメイン名（接頭辞）
		/// </summary>
		public const string	DOMAIN_NAME_SPLIT = "面積按分比";
		
		/// <summary>
		/// 按分ドメインを作成します
		/// </summary>
		/// <param name="DomainName">ドメイン名</param>
		/// <param name="DomainDescription">ドメインの説明</param>
		/// <param name="FieldType">フィールド・データ型</param>
		/// <returns>ドメイン</returns>
		public static IDomain CreateSplitRatioDomain(string DomainName, string DomainDescription, esriFieldType FieldType) {
			IDomain	agDomain = null;
			
			// 入力ﾁｪｯｸ
			if(!string.IsNullOrEmpty(DomainName)) {
				agDomain = new RangeDomainClass();
				agDomain.Name = DomainName;
				agDomain.Description = DomainDescription;
				agDomain.FieldType = FieldType;
				
				agDomain.SplitPolicy = esriSplitPolicyType.esriSPTGeometryRatio;
				agDomain.MergePolicy = esriMergePolicyType.esriMPTSumValues;
			}
			
			// 返却
			return agDomain;
		}
		
		/// <summary>
		/// ドメインのワークスペースを取得します
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		/// <returns>ドメインワークスペース</returns>
		public static IWorkspaceDomains GetDomainWorkspace(IFeatureClass FClass) {
			IWorkspaceDomains	agWSDomains = null;
			
			try {
				agWSDomains = (IWorkspaceDomains)((IDataset)FClass).Workspace;
			}
			catch(COMException comex) {
                Common.Logger.Error(comex.Message.ToString());
                Common.Logger.Error(comex.StackTrace.ToString());
			}
			catch(Exception ex) {
                Common.Logger.Error(ex.Message.ToString());
                Common.Logger.Error(ex.StackTrace.ToString());
			}
			
			// 返却
			return agWSDomains;
		}

		/// <summary>
		/// 既存のドメインを取得します
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		/// <returns>ドメイン群</returns>
		public static IEnumDomain GetExistDomains(IFeatureClass FClass) {
			IEnumDomain domains = null;
			
			// ﾄﾞﾒｲﾝ･ﾜｰｸｽﾍﾟｰｽを取得
			IWorkspaceDomains	agWSDomains = GetDomainWorkspace(FClass);
			if(agWSDomains != null) {
				domains = agWSDomains.Domains;
			}
			
			// 返却
			return domains;
		}
		
		/// <summary>
		/// 名称に合致する既存ドメインを取得します
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		/// <param name="DomainName">ドメイン名</param>
		/// <returns>ドメイン</returns>
		public static IDomain FindDomain(IFeatureClass FClass, string DomainName) {
			IDomain				agDomain = null;
			
			// ﾄﾞﾒｲﾝ･ﾜｰｸｽﾍﾟｰｽを取得
			IWorkspaceDomains	agWSDomains = GetDomainWorkspace(FClass);
			if(agWSDomains != null) {
				// 指定ﾄﾞﾒｲﾝを探索
				agDomain = agWSDomains.get_DomainByName(DomainName);
			}

			// 返却
			return agDomain;
		}
		
		/// <summary>
		/// ドメイン名を取得します
		/// </summary>
		/// <param name="FieldType">フィールドデータ型</param>
		/// <returns>ドメイン名</returns>
		public static string GetDomainName(esriFieldType FieldType) {
			string	strRet = DOMAIN_NAME_SPLIT + "(" + FieldType.ToString().Replace("esriFieldType", "") + ")";
			
			// 返却
			return strRet;
		}
		
		/// <summary>
		/// フィーチャークラスにドメインを追加します
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		/// <param name="CustomDomain">ドメイン</param>
		/// <returns>OK / NG</returns>
		public static bool AddDomain(IFeatureClass FClass, IDomain CustomDomain) {
			int	intRet = -1;
			
			// ﾄﾞﾒｲﾝ･ﾜｰｸｽﾍﾟｰｽを取得
			IWorkspaceDomains	agWSDomains = GetDomainWorkspace(FClass);
			if(agWSDomains != null) {
				// ﾄﾞﾒｲﾝIDが生成される
				intRet = agWSDomains.AddDomain(CustomDomain);
			}
			
			// 返却
			return intRet >= 0;
		}

		/// <summary>
		/// 指定フィールドにドメインを設定します ※フィーチャークラスをロックする為、編集モード中は設定不可
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		/// <param name="FieldName">フィールド名</param>
		/// <param name="Domain">ドメイン</param>
		/// <returns>OK / NG</returns>
		public static bool SetDomain(IFeatureClass FClass, String FieldName, IDomain Domain) {
			bool	blnRet = false;
			
			// ﾌｨｰﾁｬｰ･ｸﾗｽをﾛｯｸ
			ISchemaLock	agSchemaLock = (ISchemaLock)FClass;
			try {
				// 排他ﾛｯｸ
				agSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

			    // ｽｷｰﾏ編集･ﾄﾞﾒｲﾝ設定
				IClassSchemaEdit	agSchemaEdit = (IClassSchemaEdit)FClass;
				agSchemaEdit.AlterDomain(FieldName, Domain);
			
				blnRet = true;
			}
			catch(COMException comex) {
				// 確認ｺｰﾄﾞ : ﾛｯｸ状態を取得
				//IEnumSchemaLockInfo	agSchemaLocks;
				//ISchemaLockInfo		agSchemaLockInfo;
				//string				strLockInfo = "";
				
				//// 既存のﾛｯｸを全て取得
				//agSchemaLock.GetCurrentSchemaLocks(out agSchemaLocks);
				
				//while((agSchemaLockInfo = agSchemaLocks.Next()) != null) {
				//    // ﾛｯｸ種別
				//    switch(agSchemaLockInfo.SchemaLockType) {
				//    case esriSchemaLock.esriExclusiveSchemaLock:
				//        strLockInfo += "排他ﾛｯｸ";
				//        break;
				//    case esriSchemaLock.esriSharedSchemaLock:
				//        strLockInfo += "共有ﾛｯｸ";
				//        break;
				//    }
					
				//    // 対象
				//    strLockInfo += (" : " + agSchemaLockInfo.UserName + " : " + agSchemaLockInfo.TableName + Environment.NewLine);
				//}
				Common.Logger.Error(comex.Message.ToString());
				Common.Logger.Error(comex.StackTrace.ToString());
				
				// ﾕｰｻﾞｰに通知 (MDBﾛｯｸ / GDBﾛｯｸ)
				if(comex.ErrorCode == -2147220970 || comex.ErrorCode == -2147220947) {
					throw new Exception("ご指定のジオデータベースが別の場所で開かれている為、設定を適用できません。" + Environment.NewLine + "他での接続を閉じて再度実行してください。");
				}
				else {
					throw comex;
				}
			}
			catch(Exception ex) {
                Common.Logger.Error(ex.Message.ToString());
                Common.Logger.Error(ex.StackTrace.ToString());

				// ﾕｰｻﾞｰに通知
				throw ex;
			}
			finally {
				// ﾛｯｸを解除 (共有ﾛｯｸへ降格)
				agSchemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
			}
			
			// 返却
			return blnRet;
		}

		/// <summary>
		/// 未使用の分筆ドメインを削除します
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		public static void RemoveSplitDomains(IFeatureClass FClass) {
			IDomain				agDomain = null;
			
			// ﾄﾞﾒｲﾝ･ﾜｰｸｽﾍﾟｰｽを取得
			IWorkspaceDomains	agWSDomains = GetDomainWorkspace(FClass);
			if(agWSDomains != null) {
				// 未使用の分筆ﾄﾞﾒｲﾝを削除
				IEnumDomain	agEnumDomains = agWSDomains.Domains;
				while((agDomain = agEnumDomains.Next()) != null) {
					// 分筆ﾄﾞﾒｲﾝの使用状況を取得
					if(agDomain.Name.StartsWith(DOMAIN_NAME_SPLIT) && !Common.DomainManager.UsedDomain((IWorkspace)agWSDomains, agDomain)) {
						// 未設定のﾄﾞﾒｲﾝは削除
						agWSDomains.DeleteDomain(agDomain.Name);
					}
				}
			}
			
			// COMﾘﾘｰｽ
			Marshal.ReleaseComObject(agWSDomains);
			agWSDomains = null;
		}
		
		/// <summary>
		/// 指定のドメインが他のフィーチャークラス等で使用されているかどうかをチェックします
		/// </summary>
		/// <param name="GDBWorkspace">ワークスペース</param>
		/// <param name="ExistDomain">ドメイン</param>
		/// <returns>使用中 / 未使用</returns>
		public static bool UsedDomain(IWorkspace GDBWorkspace, IDomain ExistDomain) {
			bool	blnRet = false;
			
			IFeatureClass	agFClass;

			// ｼﾞｵﾃﾞｰﾀﾍﾞｰｽ内の全ﾌｨｰﾁｬｰ･ｸﾗｽを検査
			IEnumDataset	agEnumDSs = GDBWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
			while((agFClass = (IFeatureClass)agEnumDSs.Next()) != null) {
				// ﾄﾞﾒｲﾝ･ﾁｪｯｸ
				if(blnRet = UsedDomain(agFClass, ExistDomain)) {
					break;
				}
			}
			
			// 返却
			return blnRet;
		}
		
		/// <summary>
		/// 指定のドメインが使用されているかどうかをチェックします
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		/// <param name="ExistDomain">ドメイン</param>
		/// <returns>使用中 / 未使用</returns>
		public static bool UsedDomain(IFeatureClass FClass, IDomain ExistDomain) {
			bool blnRet = false;

			IField			agFld;
			IDomain			agFldDomain;
			
			// 対象ﾌｨｰﾙﾄﾞの検査
			for(int intCnt=0; intCnt < FClass.Fields.FieldCount; intCnt++) {
				// ﾌｨｰﾙﾄﾞを取得
				agFld = FClass.Fields.get_Field(intCnt);
				
				// ﾄﾞﾒｲﾝが対象とするﾌｨｰﾙﾄﾞ型をﾁｪｯｸ
				if(ExistDomain.FieldType == agFld.Type) {
					// ﾄﾞﾒｲﾝを取得
					agFldDomain = agFld.Domain;
					
					if(agFldDomain != null && agFldDomain == ExistDomain) {
						blnRet = true;
						break;
					}
				}
			}
			
			// 返却
			return blnRet;
		}
		
		/// <summary>
		/// 分筆ドメインが設定されているかどうかをチェックします
		/// </summary>
		/// <param name="FClass">フィーチャークラス</param>
		/// <returns>使用中 / 未使用</returns>
		public static bool UsedSplitDomain(IFeatureClass FClass) {
			bool	blnRet = false;
			 
			IField		agFld;
			IDomain		agFldDomain;

			// 対象ﾌｨｰﾙﾄﾞの検査
			for(int intCnt=0; intCnt < FClass.Fields.FieldCount; intCnt++) {
				// ﾌｨｰﾙﾄﾞを取得
				agFld = FClass.Fields.get_Field(intCnt);
				
				// 分筆ﾄﾞﾒｲﾝが対象とするﾌｨｰﾙﾄﾞ型をﾁｪｯｸ
				if(FieldManager.GetFieldCategory(agFld.Type) == FieldManager.FieldCategory.Numeric) {
					// ﾄﾞﾒｲﾝを取得
					agFldDomain = FClass.Fields.get_Field(intCnt).Domain;
					
					// ﾄﾞﾒｲﾝ名で判定
					if(agFldDomain != null && agFldDomain.Name.StartsWith(DOMAIN_NAME_SPLIT)) {
						blnRet = true;
						break;
					}
				}
			}
			 
			 // 返却
			 return	blnRet;
		}
	}
}
