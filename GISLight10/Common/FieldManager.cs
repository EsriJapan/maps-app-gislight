using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// フィールド一覧を取得するクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
	///  2012-07-11 ユーザー入力可能なフィールド大分類種別を追加
    /// </history>
    public class FieldManager
    {
		/// <summary>
		/// フィールド大分類（不明／数値／文字／日付／ﾊﾞｲﾅﾘ／ｼｽﾃﾑ）
		/// </summary>
		public enum FieldCategory {
			Unknown,
			Numeric,
			String,
			DateTime,
			Binary,
			System
		}
		
	#region STATIC METHODs
		/// <summary>
		/// フィールドデータ型から大分類を取得します
		/// </summary>
		/// <param name="FieldType">フィールドデータ型</param>
		/// <returns>フィールド大分類</returns>
		static public FieldCategory GetFieldCategory(esriFieldType FieldType) {
			FieldCategory fcRet = FieldCategory.Unknown;
			
			// 判定・分類
			switch(FieldType) {
			case esriFieldType.esriFieldTypeInteger:
			case esriFieldType.esriFieldTypeSmallInteger:
			case esriFieldType.esriFieldTypeSingle:
			case esriFieldType.esriFieldTypeDouble:
				fcRet = FieldCategory.Numeric;
				break;

			case esriFieldType.esriFieldTypeString:
			case esriFieldType.esriFieldTypeXML:
				fcRet = FieldCategory.String;
				break;

			case esriFieldType.esriFieldTypeDate:
				fcRet = FieldCategory.DateTime;
				break;
				
			case esriFieldType.esriFieldTypeGeometry:
			case esriFieldType.esriFieldTypeGlobalID:
			case esriFieldType.esriFieldTypeGUID:
			case esriFieldType.esriFieldTypeOID:
				fcRet = FieldCategory.System;
				break;

			case esriFieldType.esriFieldTypeBlob:
			case esriFieldType.esriFieldTypeRaster:
				fcRet = FieldCategory.Binary;
				break;
				
			default:
				//
				break;
			}
			
			// 返却
			return fcRet;
		}
		
		/// <summary>
		/// 各フィールド・プロパティ値のコレクションを取得します
		/// </summary>
		/// <param name="FieldsInstance">フィールド群</param>
		/// <param name="ColumnNames">Field Nameプロパティ</param>
		/// <param name="ColumnTypes">Field Typeプロパティ</param>
		/// <param name="IsNullables">Field Nullableプロパティ</param>
		/// <param name="Sizes">Field Sizeプロパティ</param>
		/// <param name="Precisions">Field Precisionプロパティ</param>
		/// <param name="Scales">Field Scaleプロパティ</param>
		static public void GetTableInfosArray(IFields FieldsInstance, 
			out IStringArray ColumnNames, out IStringArray ColumnTypes, 
			out IVariantArray IsNullables, 
			out ILongArray Sizes, out ILongArray Precisions, out ILongArray Scales) {
		
			// 出力ﾊﾟﾗﾒｰﾀの初期化
			ColumnNames = new StrArrayClass();
			ColumnTypes = new StrArrayClass();
			IsNullables = new VarArrayClass();
			Sizes = new LongArrayClass();
			Precisions = new LongArrayClass();
			Scales = new LongArrayClass();
			
			// 各ﾌｨｰﾙﾄﾞ･ﾌﾟﾛﾊﾟﾃｨを配列化
			IField	agFld;
			for(int intCnt=0; intCnt < FieldsInstance.FieldCount; intCnt++) {
				// 対象ﾌｨｰﾙﾄﾞを取得
				agFld = FieldsInstance.get_Field(intCnt);
				
				// ﾌﾟﾛﾊﾟﾃｨ値を取得
				ColumnNames.Add(agFld.Name);
				ColumnTypes.Add(GetFieldTypeText(agFld.Type));
				IsNullables.Add(!agFld.Required);
				Sizes.Add(agFld.Length);
				Precisions.Add(agFld.Precision);
				Scales.Add(agFld.Scale);
			}
		}
		
		/// <summary>
		/// フィールド型名称（テキスト）を取得します
		/// </summary>
		/// <param name="EsriFieldType"></param>
		/// <returns></returns>
		static public string GetFieldTypeText(esriFieldType EsriFieldType) {
			string	strRet = EsriFieldType.ToString().Replace("esriFieldType", "");
			
			// 特別処置
			switch(EsriFieldType) {
			case esriFieldType.esriFieldTypeString:
				strRet = "Text";
				break;
			case esriFieldType.esriFieldTypeInteger:
			case esriFieldType.esriFieldTypeOID:
				strRet = "Long Integer";
				break;
			case esriFieldType.esriFieldTypeSmallInteger:
				strRet = "Short Integer";
				break;
			case esriFieldType.esriFieldTypeSingle:
				strRet = "Float";
				break;
			}
			
			return strRet;
		}
		
		/// <summary>
		/// 純粋フィールド名称を取得します
		/// </summary>
		/// <param name="TargetField">フィールド</param>
		/// <returns>フィールド名</returns>
		static public string GetSimpleFieldName(IField2 TargetField) {
			string	strRet = "";
			
			if(TargetField != null) {
				string[]	strFldNames = TargetField.Name.Split('.');
				strRet = strFldNames[strFldNames.Length - 1];
			}
			
			return strRet;
		}

        /// <summary>
        /// フィールド・リスト・アイテムを取得します
        /// </summary>
        /// <param name="TableFields">フィーチャーレイヤー / スタンドアロンテーブル</param>
        /// <param name="VisibleOnly">可視フィールド?</param>
        /// <param name="EditableOnly">編集可能フィールド?</param>
        /// <param name="OwnFieldOnly">直属フィールド?</param>
		/// <param name="UseAliasName">エイリアス名使用?</param>
        /// <param name="ShowFieldType">アイテムにフィールド型を表示する?</param>
        /// <param name="FieldTypes">抽出するフィールド型</param>
        /// <returns></returns>
		static public FieldComboItem[] GetFieldItems(ITableFields TableFields, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool UseAliasName, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			List<FieldComboItem>	itemFlds = new List<FieldComboItem>();
			
			Debug.WriteLine("-- GetFieldItems START ----------");

			// ﾌｨｰﾙﾄﾞの並べ替えに対応しようと思ったが、属性ﾃｰﾌﾞﾙ表示で対応不可の為、中止する
			IOrderedLayerFields	agOrderdLFlds = TableFields as IOrderedLayerFields;
			IFieldInfoSet		agFldInfos = agOrderdLFlds.FieldInfos;
			IField				agFld;
			IFieldInfo3			agFldInfo;
			int					intFldId;
			bool				blnCanAdd;

			// ﾃｰﾌﾞﾙ結合状況を取得
			bool				blnJoined = JoinTableFunctions.HasTableJoined(TableFields);
			string				strDSName = "";
			// ﾃｰﾌﾞﾙ結合状態時は、接頭辞を取得(ﾃﾞｰﾀｾｯﾄ名)
			if(blnJoined) {
				// ﾃﾞｰﾀｾｯﾄ名を取得
				if(TableFields is IFeatureLayer) {
					strDSName = (TableFields as IFeatureLayer).FeatureClass.AliasName;
				}
				else if(TableFields is IStandaloneTable) {
					strDSName = ((TableFields as IStandaloneTable).Table as IDataset).Name;
				}
				else {
					Debug.WriteLine("●GetFieldItems QI ERROR");
				}
			}
			
			for(int intCnt=0; intCnt < agFldInfos.Count; intCnt++) {
				// ﾌｨｰﾙﾄﾞ情報を取得
				agFldInfo = agFldInfos.get_FieldInfo(intCnt) as IFieldInfo3;
				
				// 可視ﾁｪｯｸ
				if((VisibleOnly && agFldInfo.Visible) || !VisibleOnly) {
					// 編集可能ﾁｪｯｸ
					if((EditableOnly && !agFldInfo.Readonly) || !EditableOnly) {
						blnCanAdd = true;
						
						// ﾌｨｰﾙﾄﾞを取得
						intFldId = TableFields.FindField(agFldInfos.get_FieldName(intCnt));
						agFld = TableFields.get_Field(intFldId);

						// ﾃﾞｰﾀ型による抽出
						if(FieldTypes != null && FieldTypes.Length > 0) {
							if(!FieldTypes.Contains(agFld.Type)) {
								blnCanAdd = false;
							}
						}
						
						// 所有ﾌｨｰﾙﾄﾞﾁｪｯｸ
						if(blnCanAdd && OwnFieldOnly) {
							if(blnJoined && !agFld.Name.StartsWith(strDSName + ".")) {
								blnCanAdd = false;
							}
						}
						
						// ﾌｨｰﾙﾄﾞ選択ｱｲﾃﾑを作成
						if(blnCanAdd) {
							itemFlds.Add(new FieldComboItem(TableFields, intFldId, UseAliasName, ShowFieldType));
#if DEBUG
							Debug.WriteLine("●FIELD : " + itemFlds[itemFlds.Count - 1].FieldAlias);
#endif
						}
					}
				}
			}
			
			return itemFlds.ToArray();
        }
        /// <summary>
        /// フィールド・リスト・アイテムを取得します
        /// </summary>
        /// <param name="SATable">スタンドアロンテーブル</param>
        /// <param name="VisibleOnly">可視フィールド?</param>
        /// <param name="EditableOnly">編集可能フィールド?</param>
        /// <param name="OwnFieldOnly">直属フィールド?</param>
		/// <param name="UseAliasName">エイリアス名使用?</param>
        /// <param name="ShowFieldType">アイテムにフィールド型を表示する?</param>
        /// <param name="FieldTypes">抽出するフィールド型</param>
        /// <returns></returns>
        static public FieldComboItem[] GetFieldItems(IStandaloneTable SATable, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool UseAliasName, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return GetFieldItems(SATable as ITableFields, VisibleOnly, EditableOnly, OwnFieldOnly, UseAliasName, ShowFieldType, FieldTypes);
        }
        /// <summary>
        /// フィールド・リスト・アイテムを取得します
        /// </summary>
        /// <param name="FLayer">フィーチャーレイヤー</param>
        /// <param name="VisibleOnly">可視フィールド?</param>
        /// <param name="EditableOnly">編集可能フィールド?</param>
        /// <param name="OwnFieldOnly">直属フィールド?</param>
		/// <param name="UseAliasName">エイリアス名使用?</param>
        /// <param name="ShowFieldType">アイテムにフィールド型を表示する?</param>
        /// <param name="FieldTypes">抽出するフィールド型</param>
        /// <returns></returns>
        static public FieldComboItem[] GetFieldItems(IFeatureLayer FLayer, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool UseAliasName, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return GetFieldItems(FLayer as ITableFields, VisibleOnly, EditableOnly, OwnFieldOnly, UseAliasName, ShowFieldType, FieldTypes);
        }
        
        /// <summary>
        /// フィールドの並び順リストを取得します
        /// </summary>
        /// <param name="TableFields">対象レイヤー</param>
        /// <returns></returns>
        static public int[] GetOrderFieldIndexes(ITableFields TableFields) {
			List<int>			intFIDs = new List<int>();
			
			IOrderedLayerFields	agOLF = TableFields as IOrderedLayerFields;
			IFieldInfoSet		agFldInfos = agOLF.FieldInfos;

			Debug.WriteLine("■並び順を取得");
			
			IFieldInfo3	agFldInfo1;
			IFieldInfo3	agFldInfo2;
			for(int	intFld=0; intFld < agFldInfos.Count; intFld++) {
				agFldInfo1 = agFldInfos.get_FieldInfo(intFld) as IFieldInfo3;
				for(int intCnt=0; intCnt < TableFields.FieldCount; intCnt++) {
					agFldInfo2 = TableFields.get_FieldInfo(intCnt) as IFieldInfo3;
					if(agFldInfo1.Equals(agFldInfo2)) {
						if(!intFIDs.Contains(intCnt)) {
							intFIDs.Add(intCnt);
#if DEBUG
							Debug.WriteLine(string.Format("{0:00}. Field : {1}, FieldIndex : {2}", intFld, agFldInfo1.Alias, intCnt));
#endif
							break;
						}
					}
				}
			}
			
			return intFIDs.ToArray();
        }
#endregion

#region フィールド情報 取得関連
		/// <summary>
		/// 指定分類に該当するフィールド情報を取得します
		/// </summary>
		/// <param name="FeatureLayer">フィーチャーレイヤー</param>
		/// <param name="Category">大分類</param>
		/// <returns>該当するフィールド情報群</returns>
		public IFieldInfo[] GetSpecificFieldInfos(IFeatureLayer FeatureLayer, FieldCategory Category) {
			List<IFieldInfo>	agFldInfos = new List<IFieldInfo>();
			
			// 全ﾌｨｰﾙﾄﾞを取得
			ITableFields	agTFlds = (ITableFields)FeatureLayer;
			bool			blnMatch;
			for(int intCnt=0; intCnt < agTFlds.FieldCount; intCnt++) {
				// 指定分類に適合するﾌｨｰﾙﾄﾞかﾁｪｯｸ ※Unknown時は、対象とする
				blnMatch = (Category == FieldCategory.Unknown || GetFieldCategory(agTFlds.get_Field(intCnt).Type) == Category);
				
				// 該当
				if(blnMatch) {
					agFldInfos.Add(agTFlds.get_FieldInfo(intCnt));
				}
			}
			
			// 返却
			return agFldInfos.ToArray();
		}
#endregion

#region フィールド取得関連
		/// <summary>
		/// 指定分類に該当するフィールドを取得します
		/// </summary>
		/// <param name="FeatureLayer">フィーチャーレイヤー</param>
		/// <param name="Category">大分類</param>
		/// <returns>該当するフィールド群</returns>
		public IField[] GetSpecificFieldArray(IFeatureLayer FeatureLayer, FieldCategory Category) {
			List<IField>	agFlds = new List<IField>();
			
			IFields	agAllFlds = this.GetFcLayerFields(FeatureLayer);
			bool	blnMatch;
			for(int intCnt=0; intCnt < agAllFlds.FieldCount; intCnt++) {
				blnMatch = (Category == FieldCategory.Unknown || GetFieldCategory(agAllFlds.get_Field(intCnt).Type) == Category);

				// 該当
				if(blnMatch) {
					agFlds.Add(agAllFlds.get_Field(intCnt));
				}
			}
			
			// 返却
			return agFlds.ToArray();
		}
		
		public IField[] GetSpecificFieldArray(IFeatureClass FeatureClass, FieldCategory Category) {
			List<IField>	agFlds = new List<IField>();
			
			IFields	agAllFlds = FeatureClass.Fields;
			bool	blnMatch;
			for(int intCnt=0; intCnt < agAllFlds.FieldCount; intCnt++) {
				blnMatch = (GetFieldCategory(agAllFlds.get_Field(intCnt).Type) == Category);

				// 該当
				if(blnMatch) {
					agFlds.Add(agAllFlds.get_Field(intCnt));
				}
			}
			
			// 返却
			return agFlds.ToArray();
		}

		/// <summary>
		/// 指定分類に該当するフィールドを取得します
		/// </summary>
		/// <param name="FeatureLayer">フィーチャーレイヤー</param>
		/// <param name="Category">大分類</param>
		/// <returns>該当するフィールド群</returns>
		public IFields GetSpecificFields(IFeatureLayer FeatureLayer, FieldCategory Category) {
			IFieldsEdit	agFlds = new FieldsClass();
			
			IFields	agAllFlds = this.GetFcLayerFields(FeatureLayer);
			bool	blnMatch;
			for(int intCnt=0; intCnt < agAllFlds.FieldCount; intCnt++) {
				blnMatch = (Category == FieldCategory.Unknown || GetFieldCategory(agAllFlds.get_Field(intCnt).Type) == Category);

				// 該当
				if(blnMatch) {
					agFlds.AddField(agAllFlds.get_Field(intCnt));
				}
			}
			
			// 返却
			return agFlds;
		}
		
        /// <summary>
        /// フィーチャレイヤのフィールド一覧の取得
        /// </summary>
        /// <param name="featureLayer">フィーチャレイヤ</param>
        /// <returns>フィールド一覧</returns>
        public IFields GetFcLayerFields(IFeatureLayer featureLayer)
        {            
            //テーブル結合状態のフィールドも取得できる            
            IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)featureLayer;
            IFeatureClass dispFeatureClass = geoFeatureLayer.DisplayFeatureClass;
            IFields fields = dispFeatureClass.Fields;

            return fields;
        }
        
        /// <summary>
        /// 単独テーブルのフィールド一覧を取得します
        /// </summary>
        /// <param name="StdTable">単独テーブル</param>
        /// <returns></returns>
        public IFields GetStandaloneTableFields(IStandaloneTable StdTable) {
			IFields			agFlds = new FieldsClass();

			// 有効性を確認
			if(StdTable.Valid) {
				ITableFields	agTblFlds = StdTable as ITableFields;
				
				// ﾌｨｰﾙﾄﾞ群を構築
				IFieldsEdit		agFldsE = agFlds as IFieldsEdit;
				IClone			agClone;
				IField			agFld;
				
				// 表示対象のﾌｨｰﾙﾄﾞのみｺﾚｸｼｮﾝに追加する
				for(int intCnt= 0; intCnt < agTblFlds.FieldCount; intCnt++) {
					// ﾌｨｰﾙﾄﾞを取得
					agFld = agTblFlds.get_Field(intCnt);
					
					if(agTblFlds.get_FieldInfo(intCnt).Visible) {
						agClone = agFld as IClone;
						agFldsE.AddField(agClone.Clone() as IField);
					}
				}
			}
			
			return agFlds;
        }

#endregion

    }
}
