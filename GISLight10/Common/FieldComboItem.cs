using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;


namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// コンボボックスのアイテムにフィールドを設定するクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-11-21 protectedメンバーのスコープ指定削除
    ///  2011-01-25 xmlコメントreturnsタグ記述漏れの対応
    ///  2011-01-27 GetHashCodeメソッドを追加
	///  2012-07-11 フィールド別名の取扱いコンストラクタを追加
	///  2012-07-11 フィールド別名の取扱いプロパティを追加
    /// </history>
    public class FieldComboItem
    {
        private const string FORMAT = "{0} [{1}]";
        private const string TYPE_GEOMETRY = "SHAPE型";
        private const string TYPE_DATE = "日付型";
        private const string TYPE_OID = "数値型";
        private const string TYPE_SHORT = "数値型";
        private const string TYPE_INTEGER = "数値型";
        private const string TYPE_LONG = "数値型";
        private const string TYPE_FLOAT = "数値型";
        private const string TYPE_DOUBLE = "数値型";
        private const string TYPE_STRING = "文字列型";
        private const string TYPE_UNKNOWN = "不明";

        // ﾃﾞｰﾀ型の既定の表記を設定
        static private string		labelGeometry = TYPE_GEOMETRY;
        static private string		labelDate = TYPE_DATE;
        static private string		labelOid = TYPE_OID;
        static private string		labelInteger = TYPE_INTEGER;
        static private string		labelShortInteger = TYPE_SHORT;
        //string labelLong = TYPE_LONG;
        static private string		labelFloat = TYPE_FLOAT;
        static private string		labelDouble = TYPE_DOUBLE;
        static private string		labelString = TYPE_STRING;
        static private string		labelUnknown = TYPE_UNKNOWN;

        bool		_blnUseAlias = false;				// 別名使用ﾌﾗｸﾞ
        int			index = -1;
        IField		item = null;
        IFieldInfo	_agFldInfo = null;
        
        string		format_tostring = FORMAT;

	#region コンストラクタ
        /// <summary>
        /// クラスのコンストラクタ。コンボボックスのアイテムとなるフィールドを設定する。
        /// </summary>
        /// <param name="pField">フィールド</param>
        public FieldComboItem(IField pField) {
            item = pField;
        }

        /// <summary>
        /// クラスのコンストラクタ。コンボボックスのアイテムとなるフィールドとインデックスを設定する。
        /// </summary>
        /// <param name="pField">フィールド</param>
        /// <param name="fieldIndex">フィールドインデックス</param>
        public FieldComboItem(IField pField, int fieldIndex) {
            item = pField;
            index = fieldIndex;
        }
        
        /// <summary>
        /// クラスのコンストラクタ（フィールド別名に対応）
        /// </summary>
        /// <param name="pTblFields">フィーチャーテーブル／フィーチャーレイヤー</param>
        /// <param name="FieldIndex">フィールド・インデックス</param>
        /// <param name="UseAlias">別名使用（True:別名使用 / False:別名不使用）</param>
		/// <param name="DisplayFormat">表示文字列にデータ型を含めるかどうか（True:含める / False:フィールド名のみ）</param>
        public FieldComboItem(ITableFields pTblFields, int FieldIndex, bool UseAlias, bool ShowDataType) {
			// 初期化ﾒｿｯﾄﾞをｺｰﾙ
			this.InitClass(pTblFields, FieldIndex, UseAlias, (ShowDataType ? null : "{0}"));
        }

        /// <summary>
        /// クラスのコンストラクタ（フィールド別名に対応）
        /// </summary>
        /// <param name="pTblFields">フィーチャーテーブル／フィーチャーレイヤー</param>
        /// <param name="FieldIndex">フィールド・インデックス</param>
        /// <param name="UseAlias">別名使用（True:別名使用 / False:別名不使用）</param>
        public FieldComboItem(ITableFields pTblFields, int FieldIndex, bool UseAlias) {
			// 初期化ﾒｿｯﾄﾞをｺｰﾙ
			this.InitClass(pTblFields, FieldIndex, UseAlias, null);
        }

        /// <summary>
        /// クラスのコンストラクタ（フィールド別名に対応）
        /// </summary>
        /// <param name="pTblFields">フィーチャーテーブル／フィーチャーレイヤー</param>
        /// <param name="FieldIndex">フィールド・インデックス</param>
        public FieldComboItem(ITableFields pTblFields, int FieldIndex) {
			// 初期化ﾒｿｯﾄﾞをｺｰﾙ
			this.InitClass(pTblFields, FieldIndex, true, null);
        }

		/// <summary>
		/// クラスの内部変数を初期化します
		/// </summary>
        /// <param name="pTblFields">フィーチャーテーブル／フィーチャーレイヤー</param>
        /// <param name="FieldIndex">フィールド・インデックス</param>
        /// <param name="UseAlias">別名使用（True:別名使用 / False:別名不使用）</param>
		private void InitClass(ITableFields pTblFields, int FieldIndex, bool UseAlias, string DisplayFormat) {
			// 入力ﾁｪｯｸ
			if(pTblFields != null && FieldIndex >= 0) {
				try {
					// ﾌｨｰﾙﾄﾞ･ｵﾌﾞｼﾞｪｸﾄを取得
					this.item = pTblFields.get_Field(FieldIndex);
					// ﾌｨｰﾙﾄﾞ情報･ｵﾌﾞｼﾞｪｸﾄを取得
					if(UseAlias) {
						this._agFldInfo = pTblFields.get_FieldInfo(FieldIndex);
					}
				}
				catch {
				
				}
				
				// ﾌｫｰﾏｯﾄ形式を設定
				if(!string.IsNullOrEmpty(DisplayFormat)) {
					this.format_tostring = DisplayFormat;
				}
				
				// ｲﾝﾃﾞｯｸｽを設定
				this.index = FieldIndex;
				// 別名使用の設定
				this._blnUseAlias = UseAlias;
			}
		}
	#endregion

	#region プロパティ
        /// <summary>
        /// コンボボックスに設定されているフィールドのインデックス番号
        /// </summary>
        public int FieldIndex {
            get {
                return index;
            }
        }

        /// <summary>
        /// コンボボックスに設定されているフィールド
        /// </summary>
        public IField Field {
            get {
                return item;
            }
        }

        /// <summary>
        /// コンボボックスのアイテムとなったときに出力する文字列の書式
        /// </summary>
        public string FormatToStringName {
            set {
                format_tostring = value;
            }
        }
        
        /// <summary>
        /// 別名使用設定を取得または設定します（デフォルトは「別名不使用」）
        /// </summary>
        public bool UseAlias {
			get {
				return this._blnUseAlias;
			}
			set {
				this._blnUseAlias = value;
			}
        }
        
        /// <summary>
        /// フィールドの別名を取得します
        /// </summary>
        public string FieldAlias {
			get {
				return this._blnUseAlias && this._agFldInfo != null && !string.IsNullOrWhiteSpace(this._agFldInfo.Alias) ? this._agFldInfo.Alias : this.item.Name;
			}
        }
        
        /// <summary>
        /// フィールド名を取得します
        /// </summary>
        public string FieldName {
			get {
				return this.item.Name;
			}
        }
	#endregion

	#region 公開メソッド
        /// <summary>
        /// コンボボックスのアイテムとなったときの文字列を返す
        /// </summary>
        /// <returns>コンボボックスのアイテムとなったときの文字列</returns>
        public override string ToString()　{
            const string	PARAM1 = "{1}";
            string			strText = null;
            
            // ﾌｨｰﾙﾄﾞの別名を取得
            string			strFldName = this.FieldAlias;

            if(format_tostring.Contains(PARAM1) == false)　{
                strText = string.Format(format_tostring, strFldName);
            }
            else {
				// ﾃﾞｰﾀ型文字列を取得
				string type = getDataTypeString(item.Type);
				strText = string.Format(format_tostring, strFldName, type);
            }
            
            // 返却
            return strText;
        }


        /// <summary>
        /// コンボボックスアイテムが保持するフィールド型のクラスオブジェクトと一致するか確認する
        /// </summary>
        /// <param name="obj">比較するフィールドクラスのオブジェクト</param>
        /// <returns>比較結果</returns>
        public override bool Equals(object obj)
        {
            return item.Equals(obj);
        }

        /// <summary>
        /// フィールドクラスオブジェクトのハッシュコードを返す
        /// </summary>
        /// <returns>フィールドクラスオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return item.GetHashCode();
        }

        /// <summary>
        /// フィールド型に対応する文字列の取得
        /// </summary>
        /// <param name="ftype">取得する文字列のフィールド型</param>
        /// <returns>フィールド型に対応する文字列</returns>
        static public string getDataTypeString(esriFieldType ftype) {
            string type;

            switch(ftype) {
            case esriFieldType.esriFieldTypeBlob:
                type = labelUnknown;
                break;
            case esriFieldType.esriFieldTypeDate:
                type = labelDate;
                break;
            case esriFieldType.esriFieldTypeDouble:
                type = labelDouble;
                break;
            case esriFieldType.esriFieldTypeGeometry:
                type = labelGeometry;
                break;
            case esriFieldType.esriFieldTypeGlobalID:
                type = labelUnknown;
                break;
            case esriFieldType.esriFieldTypeGUID:
                type = labelUnknown;
                break;
            case esriFieldType.esriFieldTypeInteger:
                type = labelInteger;
                break;
            case esriFieldType.esriFieldTypeOID:
                type = labelOid;
                break;
            case esriFieldType.esriFieldTypeRaster:
                type = labelUnknown;
                break;
            case esriFieldType.esriFieldTypeSingle:
                type = labelFloat;
                break;
            case esriFieldType.esriFieldTypeSmallInteger:
                type = labelShortInteger;
                break;
            case esriFieldType.esriFieldTypeString:
                type = labelString;
                break;
            case esriFieldType.esriFieldTypeXML:
                type = labelUnknown;
                break;
            default:
                type = labelUnknown;
                break;
            }

            return type;
        }

        /// <summary>
        /// フィールド型に対応する文字列を設定する
        /// </summary>
        /// <param name="ftype">フィールド型</param>
        /// <param name="label">フィールド型に対応する文字列</param>
        static public void setDataTypeString(esriFieldType ftype, string label) {
            switch(ftype) {
            case esriFieldType.esriFieldTypeDate:
                labelDate = label;
                break;
            case esriFieldType.esriFieldTypeDouble:
                labelDouble = label;
                break;
            case esriFieldType.esriFieldTypeGeometry:
                labelGeometry = label;
                break;
            case esriFieldType.esriFieldTypeInteger:
                labelInteger = label;
                break;
            case esriFieldType.esriFieldTypeOID:
                labelOid = label;
                break;
            case esriFieldType.esriFieldTypeSingle:
                labelFloat = label;
                break;
            case esriFieldType.esriFieldTypeSmallInteger:
                labelShortInteger = label;
                break;
            case esriFieldType.esriFieldTypeString:
                labelString = label;
                break;
            case esriFieldType.esriFieldTypeBlob:
            case esriFieldType.esriFieldTypeGlobalID:
            case esriFieldType.esriFieldTypeGUID:
            case esriFieldType.esriFieldTypeRaster:
            case esriFieldType.esriFieldTypeXML:
            default:
                throw new NotSupportedException(string.Format("指定された型のラベルはサポートしていません"));
            }
        }
	#endregion
    }
}
