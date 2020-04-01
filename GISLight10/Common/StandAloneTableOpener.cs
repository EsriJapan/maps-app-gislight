using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRIJapan.GISLight10.Ui;

/*
 * クラス構成
 * 1. StandAloneTableOpener (単独テーブル操作用クラス)
 * 2. StandaloneTableUnitClass (単独テーブル選択パッケージ・クラス)
*/
namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// テーブルを開くクラス
    /// </summary>
    /// <history>
    ///  2010-11-01(xx) 新規作成 (ejxxxx)
    /// </history>
    public class StandAloneTableOpener
    {
        /// <summary>
        /// dbf,csvを開く
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>テーブル</returns>
        public ITable OpenCsvAndDbfTable(string filePath, ref string selectTable)
        {            
            string directoryPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            string fileExtension = Path.GetExtension(filePath);
            string tableName = string.Empty;

            IWorkspaceFactory wsFactory = null;

            //大文字、小文字を区別せずに比較
            if (String.Compare(fileExtension, ".csv", true) == 0)
            {
                wsFactory = new TextFileWorkspaceFactoryClass();
            }
            else if (String.Compare(fileExtension, ".dbf", true) == 0)
            {
                wsFactory = new ShapefileWorkspaceFactoryClass();
            }
            // 2012/07/18 mdbに対応
            else if (String.Compare(fileExtension, ".mdb", true) == 0)
            {
                wsFactory = new AccessWorkspaceFactoryClass();
            }
            // 2012/07/23 gdbに対応
            else if (String.Compare(fileExtension, ".gdb", true) == 0)
            {
                wsFactory = new FileGDBWorkspaceFactoryClass();
            }
            else
            {
                return null;
            }

            // 2012/07/19 mdb,gdbに対応
            IWorkspace ws = null;
            if (String.Compare(fileExtension, ".mdb", true) == 0 ||
                String.Compare(fileExtension, ".gdb", true) == 0)
            {
                ws = wsFactory.OpenFromFile(filePath, 0);
            }
            else
            {
                ws = wsFactory.OpenFromFile(directoryPath, 0);
            }
            IFeatureWorkspace fws = (IFeatureWorkspace)ws;

            // 2012/07/19 mdb,gdbに対応
            if ((String.Compare(fileExtension, ".mdb", true) == 0 || 
                 String.Compare(fileExtension, ".gdb", true) == 0) && selectTable == null)
            {
                IEnumDatasetName enumDatasetName;
                IDatasetName datasetName;
                enumDatasetName = ws.get_DatasetNames(esriDatasetType.esriDTAny);

                FormSelectTable frmSelectTable = new FormSelectTable();

                datasetName = enumDatasetName.Next();
                while (datasetName != null)
                {
                    frmSelectTable.PropcomboBoxTable.Items.Add(datasetName.Name);
                    datasetName = enumDatasetName.Next();
                }

                frmSelectTable.ShowDialog();

                selectTable = frmSelectTable.tableName;
            }

            ITable table = null;

            try
            {
                // 2012/07/19 mdb,gdbに対応
                if (String.Compare(fileExtension, ".mdb", true) == 0 || 
                    String.Compare(fileExtension, ".gdb", true) == 0)
                {
                    //テーブル名を指定する。
                    table = fws.OpenTable(selectTable);
                }
                else
                {
                    //CSVをExcelで開いている場合、エラーが発生する。
                    table = fws.OpenTable(fileName);
                }

                return table;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Common.Logger.Error(ex.Message);
                return null;
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(ws);
                ComReleaser.ReleaseCOMObject(fws);
            }            
        }
        
        /// <summary>
        /// 単独テーブルをマップに追加します
        /// </summary>
        /// <param name="TargetTable"></param>
        /// <param name="TargetMap"></param>
        /// <returns></returns>
        static public IStandaloneTable AddStandaloneTable(ITable TargetTable, IMap TargetMap) {
			IStandaloneTable	agStdTbl = null;
			
			try {
				agStdTbl = new StandaloneTableClass();
				agStdTbl.Table = TargetTable;
				
				// ﾃｰﾌﾞﾙを追加 ※同じﾃｰﾌﾞﾙが既に存在する場合は、無視される
				IStandaloneTableCollection	agSTblColl = TargetMap as IStandaloneTableCollection;
				agSTblColl.AddStandaloneTable(agStdTbl);
			}
			catch(Exception ex) {
#if DEBUG
				Debug.WriteLine(ex.Message);
#endif
			}
			
			return agStdTbl;
        }
        
        /// <summary>
        /// テーブルの完全パスを取得します
        /// </summary>
        /// <param name="TargetTable"></param>
        /// <returns></returns>
        static public string GetFullPath(IStandaloneTable TargetTable) {
			string	strRet = "";
			
			// 入力ﾁｪｯｸ
			if(TargetTable != null && TargetTable.Valid) {
				IDataset	agDS = TargetTable.Table as IDataset;
				if(agDS != null) {
					// ﾊﾟｽを取得
					IWorkspace	agWS = agDS.Workspace;
					
					// 名称を取得
					strRet = agWS.PathName + "\\" + agDS.Name;
				}
			}
			
			return strRet;
        }
        
        /// <summary>
        /// テーブルの完全パスからテーブル名を取得します
        /// </summary>
        /// <param name="FullPath"></param>
        /// <returns></returns>
        static public string GetTableName(string FullPath) {
			string	strRet = "";
			
			// 入力ﾁｪｯｸ
			if(!string.IsNullOrEmpty(FullPath)) {
				// ﾊﾟｽを分割
				string[]	strSegs = FullPath.Split('\\');
				// 最後尾(ﾃｰﾌﾞﾙ名)を取得
				strRet = strSegs.Last();
			}
			
			return strRet;
        }
        
        /// <summary>
        /// 指定のテーブルが指定マップの単独テーブルであるかどうか確認します
        /// </summary>
        /// <param name="TargetTable">既存テーブル</param>
        /// <param name="TargetMap">対象マップ</param>
        /// <returns></returns>
        static public bool IsStandaloneTable(ITable TargetTable, IMap TargetMap) {
			// 単独ﾃｰﾌﾞﾙかどうか確認
			return GetStandaloneTable(TargetTable, TargetMap) != null;
        }
        
        /// <summary>
        /// 単独テーブル・インスタンスを取得します
        /// </summary>
        /// <param name="TargetTable"></param>
        /// <param name="TargetMap"></param>
        /// <returns></returns>
        static public IStandaloneTable GetStandaloneTable(ITable TargetTable, IMap TargetMap) {
			IStandaloneTable	agStdTbl = null;
			
			// ﾌｨｰﾁｬｰ･ｸﾗｽか判定
			if(!(TargetTable is IFeatureClass)) {
				// 単独ﾃｰﾌﾞﾙの有無を確認
				IStandaloneTableCollection	agStdColl = TargetMap as IStandaloneTableCollection;
				if(agStdColl.StandaloneTableCount > 0) {
					IStandaloneTable	agStdTblTemp;
					#if DEBUG
					// 検証用
					//IDataset	agDS1;
					//IDataset	agDS2;
					#endif
					
					// 同じ単独ﾃｰﾌﾞﾙを探索
					for(int intCnt=0; intCnt < agStdColl.StandaloneTableCount; intCnt++) {
						// 単独ﾃｰﾌﾞﾙを取得 ※有効性は度外視する
						agStdTblTemp = agStdColl.get_StandaloneTable(intCnt);
						
						// 下記判定だけでも良いか ??
						if(TargetTable.Equals(agStdTblTemp.Table)) {
							agStdTbl = agStdTblTemp;
							break;
						}
						
						#if DEBUG
						// ﾃﾞｰﾀｾｯﾄを取得して確認 OK
						//agDS1 = TargetTable as IDataset;
						//agDS2 = agStdTblTemp.Table as IDataset;
						//if(agDS1.Name == agDS2.Name) {
						//    if(agDS1.Workspace.PathName == agDS2.Workspace.PathName) {
						//        agStdTbl = agStdTblTemp;
						//        break;
						//    }
						//}
						#endif
					}
				}
			}
			
			return agStdTbl;
        }
        
    }

	/// <summary>
	/// 単独テーブルの取り扱いクラス
	/// </summary>
	public class StandaloneTableUnitClass : System.IEquatable<StandaloneTableUnitClass> {
		#region プロパティ
		/// <summary>
		/// ワークスペースのパス／または表記を取得または設定します
		/// </summary>
		public string WorkSpacePath			{ get; set; }
		/// <summary>
		/// テーブル名を取得または設定します
		/// </summary>
		public string TableName				{ get; set; }
		/// <summary>
		/// 単独テーブルを取得または設定します
		/// </summary>
		public IStandaloneTable StdTable	{ get; set; }
		/// <summary>
		/// オブジェクトに関連付けられた、ユーザー定義のデータです
		/// </summary>
		public object Tag					{ get; set; }
		#endregion
		
		#region メソッド
		/// <summary>
		/// コンボボックス・アイテム表記を取得します
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			string	strRet = "";
			
			if(string.IsNullOrEmpty(this.WorkSpacePath)) {
				strRet = this.TableName;
			}
			else {
				//string	strQT = this.WorkSpacePath.Contains(" ") ? "'" : "";
				string	strQT = "";
				strRet = string.Format(@"{0}{1}\{2}{0}", strQT, this.WorkSpacePath, this.TableName);
			}
			
			return strRet;
			//return base.ToString();
		}
		
		/// <summary>
		/// ワークスペースの表記(ArcMapに合わせた)を取得します　※用途 : テーブル一覧表示など
		/// </summary>
		/// <returns></returns>
		static public string GetWorkspaceBrowseName(IWorkspace WorkSpace) {
			string	strRet = "";
			
			// 入力ﾁｪｯｸ
			if(WorkSpace != null) {
				// 接続ﾌﾟﾛﾊﾟﾃｨを取得
				IPropertySet	agProp = WorkSpace.ConnectionProperties;
				object			objNames = null;
				object			objVals = null;
				agProp.GetAllProperties(out objNames, out objVals);

				/* 表記を校正 */
				// ﾌｫﾙﾀﾞｰ
				if(WorkSpace.Type == esriWorkspaceType.esriFileSystemWorkspace) {
					strRet = (string)agProp.GetProperty("DATABASE");
				}
				// FGDB / PGDB
				else if(WorkSpace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace) {
					strRet = (string)agProp.GetProperty("DATABASE");
				}
				// ArcSDE(EnterpriseDB) / OLE DB
				else if(WorkSpace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace) {
					if(!string.IsNullOrEmpty(WorkSpace.PathName)) {
						IDataset	agDS_W = WorkSpace as IDataset;
						strRet = agDS_W.BrowseName;
						
						// SDE接続の時は表記をArcMapに合わせる
						if(Path.GetExtension(WorkSpace.PathName) == ".sde") {
							// SDE (ArcMapでの表記に合わせる)
							string	strV = (string)agProp.GetProperty("VERSION");
							string	strS = (string)agProp.GetProperty("SERVER");
							strRet = string.Format("{0} ({1})", strV, strS);
						}
					}
					else {
						string	strTemp = (string)agProp.GetProperty("CONNECTSTRING");
						if(strTemp.Contains("Data Source=")) {
							strRet = strTemp.Substring(strTemp.IndexOf("Data Source=") + 12);
							if(strRet.Contains(";")) {
								strRet = strRet.Substring(0, strRet.IndexOf(";"));
							}
						}
						else {
							strRet = "?";
						}
					}
				}
			}
			
			return strRet;
		}
		
		/// <summary>
		/// ワークスペースの簡潔表記を取得します
		/// </summary>
		/// <param name="WorkSpace"></param>
		/// <returns></returns>
		static public string GetEasyWorkspaceBrowseName(IWorkspace WorkSpace) {
			string	strRet = "";
			
			// 入力ﾁｪｯｸ
			if(WorkSpace != null) {	
				// DB接続は、正規名とする
				if(WorkSpace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace) {
					strRet = GetWorkspaceBrowseName(WorkSpace);
				}
				else {
					// 接続ﾌﾟﾛﾊﾟﾃｨを取得
					IPropertySet	agProp = WorkSpace.ConnectionProperties;
					object			objNames = null;
					object			objVals = null;
					agProp.GetAllProperties(out objNames, out objVals);

					// ﾌｫﾙﾀﾞｰ
					if(WorkSpace.Type == esriWorkspaceType.esriFileSystemWorkspace) {
						// 表示なし
						strRet = "";
					}
					// FGDB / PGDB
					else if(WorkSpace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace) {
						strRet = (string)agProp.GetProperty("DATABASE");
						
						// DB名だけ表示
						strRet = Path.GetFileName(strRet);
					}
				}
			}
			
			return strRet;
		}

		/// <summary>
		/// 指定のオブジェクトと等しいかどうかを判定します
		/// </summary>
		/// <param name="OtherSTUC"></param>
		/// <returns></returns>
		public bool Equals(StandaloneTableUnitClass OtherSTUC) {
			bool	blnRet = false;
			
			if(OtherSTUC != null) {
				// 自身の参照等価を確認
				if(!System.Object.ReferenceEquals(this, OtherSTUC)) {
					if(this.StdTable != null && this.StdTable.Table != null && OtherSTUC.StdTable.Table != null) {
						// ﾃｰﾌﾞﾙの参照等価を確認
						blnRet = this.Equals(OtherSTUC.StdTable.Table);
					}
					else if((this.StdTable == null || this.StdTable.Table == null) && OtherSTUC.StdTable.Table == null) {
						// ﾌﾟﾛﾊﾟﾃｨの等価を確認
						blnRet = (this.WorkSpacePath == OtherSTUC.WorkSpacePath && this.TableName == OtherSTUC.TableName);
					}
				}
				else {
					blnRet = true;
				}
			}
			
			return blnRet;
		}
		
		/// <summary>
		/// 指定のテーブルと等しいかどうかを判定します
		/// </summary>
		/// <param name="OtherTable"></param>
		/// <returns></returns>
		public bool Equals(ITable OtherTable) {
			bool	blnRet = false;
			
			if(this.StdTable != null && this.StdTable.Table != null && OtherTable != null) {
				// ﾃｰﾌﾞﾙの参照等価を確認
				blnRet = System.Object.ReferenceEquals(this.StdTable.Table, OtherTable);
			}
			else if(OtherTable == null && (this.StdTable == null || this.StdTable.Table == null)) {
				blnRet = true;
			}
			
			return blnRet;
		}
		
		/// <summary>
		/// 指定のテーブル・パスと等しいかどうかを判定します
		/// </summary>
		/// <param name="TablePath"></param>
		/// <returns></returns>
		public bool Equals(string TablePath) {
			bool	blnRet = false;
			
			// 入力ﾁｪｯｸ
			if(!string.IsNullOrEmpty(TablePath)) {
				
				string	strTblPath = "";
				// ﾃｰﾌﾞﾙで照合
				if(this.StdTable != null && this.StdTable.Table != null) {
					// ﾌﾙﾊﾟｽを取得
					strTblPath = StandAloneTableOpener.GetFullPath(this.StdTable);
				}
				// 文字情報で照合
				else {
					strTblPath = string.Format("{0}\\{1}", this.WorkSpacePath, this.TableName);
				}
				if(TablePath == strTblPath) {
					blnRet = true;
				}
			}
			
			return blnRet;
		}

		/// <summary>
		/// 指定のオブジェクトと等しいかどうかを判定します
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			bool	blnRet = false;
			
			// ﾃﾞｰﾀの有効性と型をﾁｪｯｸ
			if(obj != null && obj.GetType().Equals(this.GetType())) {
				// ｵｰﾊﾞｰﾛｰﾄﾞ
				blnRet = this.Equals(obj as StandaloneTableUnitClass);
			}
			
			return blnRet;
		}

		/// <summary>
		/// ハッシュ値を取得します
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			int intHash1 = 0, intHash2 = 0, intHash3 = 0, intRet = 0;
			
			// 各ﾌﾟﾛﾊﾟﾃｨの個別のﾊｯｼｭｺｰﾄﾞを取得
			if(this.StdTable != null) {
				intHash1 = this.StdTable.GetHashCode();
			}
			if(this.TableName != null) {
				intHash2 = this.TableName.GetHashCode();
			}
			if(this.WorkSpacePath != null) {
				intHash3 = this.WorkSpacePath.GetHashCode();
			}
			
			// XOR
			intRet = intHash1 ^ intHash2 ^ intHash3;
			return intRet;
		}
		
		/// <summary>
		/// 比較演算子に対応
		/// </summary>
		/// <param name="stuc1"></param>
		/// <param name="stuc2"></param>
		/// <returns></returns>
		static public bool operator == (StandaloneTableUnitClass stuc1, StandaloneTableUnitClass stuc2) {
			bool	blnRet = false;
			
			// nullの確認　※objectとして比較
			if((object)stuc1 == null && (object)stuc2 == null) {
				blnRet = true;
			}
			else if((object)stuc1 != null && (object)stuc2 != null) {
				blnRet = stuc1.Equals(stuc2);
			}
			
			return blnRet;
		}
		
		static public bool operator != (StandaloneTableUnitClass stuc1, StandaloneTableUnitClass stuc2) {
			// == の結果を反転
			return !(stuc1 == stuc2);
		}
		#endregion
		
	}
}
