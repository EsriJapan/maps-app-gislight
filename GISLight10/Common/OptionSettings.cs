using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// オプション設定操作クラス
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成 
    ///  2011/04/06 少数点表示桁数指定取得追加
    ///  2011/06/01 フィールド名エイリアス使用スイッチ追加
    ///  2012/08/18 ジオプロセシングのバックグランド実行を追加
    /// </history>
    class OptionSettings : XMLAccessClass
    {
		private const string CONNECT_FOLDER = "ConnectFolders";
        private const string GEOPROCESSING_BACKGROUND = "GeoprocessingBackground";
        private const string FIELD_NAME_USE_ALIAS = "UseAlias";
        private const string EXPORT_MAP_RESOLUTON_MAX = "ExportMapResolutionMax";
        private const string ATTRIBUTE_TABLE_DISPLAY_AFTER_DECIMAL_POINT_LENGTH = 
            "AttributeTableDisplayAftDecPntLength";
        private const string ATTRIBUTE_TABLE_DISPLAY_OID_MAX = "AttributeTableDisplayOIDMax";
        private const string INDIVISUAL_VALUE_DISPLAY_MAX = "IndividualValueDisplayMax";
        private const string UNIQUE_VALUE_MAX = "UniqueValueMax";
        private const string MAX = "max";
        private const string MIN = "min";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OptionSettings()
        {
            LoadSettings();
        }

        /// <summary>
        /// 属性テーブル設定
        /// 1ページあたりの表示最大レコード数
        /// </summary>
        public string AttributeTableDisplayOIDMax
        {
            get
            {
                return base.GetXMLValue(ATTRIBUTE_TABLE_DISPLAY_OID_MAX);
            }

            set
            {
                base.SetXMLValue(ATTRIBUTE_TABLE_DISPLAY_OID_MAX, value);
            }
        }

        /// <summary>
        /// 属性テーブル設定
        /// 1ページあたりの表示最大レコード数(最大)
        /// </summary>
        public string AttributeTableDisplayOIDMaxMax
        {
            get
            {
                return base.GetXMLAttributeValue(ATTRIBUTE_TABLE_DISPLAY_OID_MAX, MAX);
            }
        }


        /// <summary>
        /// 属性テーブル設定
        /// 少数点以下の表示桁数
        /// </summary>
        public string AttributeTableDisplayAftDecPntLength
        {
            get
            {
                return base.GetXMLValue(
                    ATTRIBUTE_TABLE_DISPLAY_AFTER_DECIMAL_POINT_LENGTH);
            }
        }

        /// <summary>
        /// 属性検索設定
        /// 個別値取得最大数
        /// </summary>
        public string IndividualValueDisplayMax
        {
            get
            {
                return base.GetXMLValue(INDIVISUAL_VALUE_DISPLAY_MAX);
            }

            set
            {
                base.SetXMLValue(INDIVISUAL_VALUE_DISPLAY_MAX, value);
            }
        }

        /// <summary>
        /// 属性検索設定
        /// 個別値取得最大数(最大)
        /// </summary>
        public string IndividualValueDisplayMaxMax
        {
            get
            {
                return base.GetXMLAttributeValue(INDIVISUAL_VALUE_DISPLAY_MAX, MAX);
            }
        }


        /// <summary>
        /// シンボル設定
        /// 個別値分類の最大分類数
        /// </summary>
        public string UniqueValueMax
        {
            get
            {
                return base.GetXMLValue(UNIQUE_VALUE_MAX);
            }

            set
            {
                base.SetXMLValue(UNIQUE_VALUE_MAX, value);
            }
        }

        /// <summary>
        /// シンボル設定
        /// 個別値分類の最大分類数(最大)
        /// </summary>
        public string UniqueValueMaxMax
        {
            get
            {
                return base.GetXMLAttributeValue(UNIQUE_VALUE_MAX, MAX);
            }
        }


        /// <summary>
        /// マップエクスポート解像度最大数
        /// </summary>
        public string ExportMapResolutionMax
        {
            get
            {
                return base.GetXMLValue(EXPORT_MAP_RESOLUTON_MAX);
            }
        }

        /// <summary>
        /// フィールド名エイリアス使用スイッチ
        /// </summary>
        public string FieldNmaeUseAlias
        {
            get
            {
                return base.GetXMLValue(FIELD_NAME_USE_ALIAS);
            }
            set
            {
                base.SetXMLValue(FIELD_NAME_USE_ALIAS, value);
            }
        }

        /// <summary>
        /// ジオプロセシングのバックグラウンド実行
        /// </summary>
        public string GeoprocessingBackground
        {
            get 
            {
                return base.GetXMLValue(GEOPROCESSING_BACKGROUND); 
            }
            set 
            {
                base.SetXMLValue(GEOPROCESSING_BACKGROUND, value);
            }
        }
        
        /// <summary>
        /// フォルダ接続リストを取得または設定します ※別途、管理クラスあり ConnectFoldersManager
        /// </summary>
        public List<ConnectFolder> ConnectFolders {
			get {
				List<ConnectFolder>	listCF = new List<ConnectFolder>();

				// 設定ﾌｧｲﾙ読み込み
				string[]	xmlFolders = base.GetXMLNodes(CONNECT_FOLDER);

				// ﾌｫﾙﾀﾞ･ｲﾝｽﾀﾝｽを生成
				XmlDocument	xmlDoc = new XmlDocument();
				xmlDoc.LoadXml("<ConnectFolders>" + string.Join("", xmlFolders) + "</ConnectFolders>");
				XmlNode		xmlParent = xmlDoc.FirstChild;
				if(xmlParent.HasChildNodes) {
					string	strName, strPath;
					for(int intCnt=0; intCnt < xmlParent.ChildNodes.Count; intCnt++) {
						// 属性を取得
						strName = xmlParent.ChildNodes[intCnt].Attributes["Name"].Value;
						strPath = xmlParent.ChildNodes[intCnt].Attributes["Path"].Value;
						
						if(!(string.IsNullOrEmpty(strName) || string.IsNullOrEmpty(strPath))) {
							listCF.Add(new ConnectFolder(strName, strPath));
						}
					}
				}
				
				return listCF;
			}
			set {
				// XMLを取得
				string[]		strXmls = new string[value.Count];
				ConnectFolder	cfNew;
				for(int intCnt=0; intCnt < strXmls.Length; intCnt++) {
					cfNew = value[intCnt];
					strXmls[intCnt] = cfNew.Xml;
				}
				
				// 設定ﾌｧｲﾙに書き込み
				base.SetXMLNodes(CONNECT_FOLDER, strXmls);
			}
        }

        /// <summary>
        /// 設定ファイル読み込み
        /// </summary>
        private void LoadSettings()
        {
            base.LoadXMLDocument();   
        }

        /// <summary>
        /// 設定ファイル保存
        /// </summary>
        public void SaveSettings()
        {
            base.SaveXMLDocument();
        }
    }
    
    /// <summary>
    /// フォルダ接続アイテム・クラス
    /// </summary>
    public class ConnectFolder {
		/// <summary>
		/// フォルダ表示名を取得または設定します
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// フォルダのパスを取得または設定します
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// XMLテキストを取得します
		/// </summary>
		public string Xml {
			get {
				return string.Format("<ConnectFolder Name=\"{0}\" Path=\"{1}\" />", Name, Path);
			}
		}
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="Name">表示名</param>
		/// <param name="Path">フォルダ・パス</param>
		public ConnectFolder(string Name, string FolderPath) {
			this.Name = Name;
			this.Path = FolderPath;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="Path">フォルダ・パス</param>
		public ConnectFolder(string FolderPath) {
			this.Name = FolderPath;
			this.Path = FolderPath;
		}
    }
}
