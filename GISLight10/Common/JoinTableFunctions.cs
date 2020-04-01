using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// テーブル結合関連のクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-21 protectedメンバのスコープ指定記述を削除
    ///  2012-08-02 IMemoryRelationshipClassFactory, IRelQueryTableFactoryのインスタンス作成を変更
    /// </history>
    public class JoinTableFunctions
    {
        private ArrayList joinedTableList = new ArrayList();

        IFeatureLayer sourceLayer = null;        
        ITable destinationTable = null;       
        string sourceKeyFiled = "";
        string destinationKeyField = "";

        bool isAlreadyJoinValue = false;
        bool hasTableJoinValue = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcLayer">ソースレイヤ</param>
        /// <param name="destTable">結合先テーブル</param>
        /// <param name="srcKeyField">ソースレイヤーのキーフィールド名</param>
        /// <param name="destKeyField">結合先テーブルキーフィールド名</param>
        public JoinTableFunctions(IFeatureLayer srcLayer, ITable destTable,
            string srcKeyField, string destKeyField)
        {
            sourceLayer = srcLayer;
            destinationTable = destTable;
            sourceKeyFiled = srcKeyField;
            destinationKeyField = destKeyField;

            hasTableJoinValue = SetHasTableJoin();

            isAlreadyJoinValue = SetIsAlreadyJoin();
        }

        /// <summary>
        /// 結合先レイヤをソースレイヤで、既に結合しているかの判定結果を返す
        /// </summary>
        public bool IsAlreadyJoin
        {
            get
            {
                return isAlreadyJoinValue;
            }
        }

        /// <summary>
        /// 結合元レイヤが結合状態保持か判定結果を返す
        /// </summary>
        public bool HasTableJoin
        {
            get
            {
                return hasTableJoinValue;
            }
        }        

        /// <summary>
        /// 結合元レイヤがテーブル結合を持っているかの判定
        /// </summary>
        /// <returns>結合あり:true、結合なし:false</returns>
        private bool SetHasTableJoin()
        {
            bool hasJoin = true;

            //テーブル結合を持っていない場合は、nullになる。
            IRelQueryTable relQueryTable = GetRelQueryTable();

            if (relQueryTable == null)
            {
                hasJoin = false;
            }

            return hasJoin;
        }


        /// <summary>
        /// すでにテーブル結合済みの組み合わせかの判定
        /// </summary>
        /// <returns>結合済み:true、結合なし:false</returns>
        private bool SetIsAlreadyJoin()
        {
            bool alreadyJoin = false;
                                    
            //結合元レイヤがテーブル結合を持っているかの判定
            if (hasTableJoinValue == true)
            {
                IRelQueryTable relQueryTable = GetRelQueryTable();

                //結合元レイヤに結合されているテーブルの一覧をjoinedTableListに格納する。
                ListJoinedTables(relQueryTable);

                //結合しようとしてる結合先テーブル と 結合済みのテーブルを比較する。
                for (int i = 0; i < this.joinedTableList.Count; i++)
                {                    
                    if (this.destinationTable.Equals(this.joinedTableList[i]))
                    {
                        alreadyJoin = true;
                        break;
                    }
                }
            }
           
            return alreadyJoin;
        }        


        private IRelQueryTable GetRelQueryTable()
        {
            IDisplayTable displayTable = (IDisplayTable)this.sourceLayer;
            ITable table = displayTable.DisplayTable;

            //テーブル結合を持っていない場合は、nullになる。
            IRelQueryTable relQueryTable = table as IRelQueryTable;

            return relQueryTable;
        }



        /// <summary>
        /// 入力テーブルに結合されているテーブルのリストをArrayListに追加
        /// </summary>
        /// <param name="relQueryTable"></param>
        private void ListJoinedTables(IRelQueryTable relQueryTable)
        {
            //this.joinedTableList.Clear();

            ITable rqSourceTable = relQueryTable.SourceTable;
            ITable rqDestinationTable = relQueryTable.DestinationTable;

            // See if the source and destination tables are RelQueryTables.
            IRelQueryTable sourceRelQueryTable = rqSourceTable as IRelQueryTable;
            IRelQueryTable destinationRelQueryTable = rqDestinationTable as IRelQueryTable;

            if (sourceRelQueryTable != null)
            {
                // Call this method on the source table.                
                ListJoinedTables(sourceRelQueryTable);
            }
            else
            {                
                ITable joinedTable = (ITable)rqSourceTable;
                this.joinedTableList.Add(joinedTable);                
            }

            if (destinationRelQueryTable != null)
            {
                // Call this method on the destination table.                
                ListJoinedTables(destinationRelQueryTable);
            }
            else
            {
                ITable joinedTable = (ITable)rqDestinationTable;
                this.joinedTableList.Add(joinedTable);                
            }
        }        


        /// <summary>
        /// テーブル結合の実行
        /// </summary>
        public void Join()
        {
            ITable sourceTable = null;
            
            if (hasTableJoinValue == false)
            {                
                sourceTable = (ITable)this.sourceLayer.FeatureClass;
            }
            else
            {
                sourceTable = (ITable)GetRelQueryTable();
            }
            
            if (isAlreadyJoinValue == false)
            {                
                // Build a memory relationship class.
                IMemoryRelationshipClassFactory memRelClassFactory = SingletonUtility.NewMemoryRelationshipClassFactory(); //new MemoryRelationshipClassFactoryClass();

                // Open the RelQueryTable as a feature class.
                IRelQueryTableFactory rqtFactory =SingletonUtility.NewRelQueryTableFactory();// new RelQueryTableFactoryClass();

                IDataset destDataset = (IDataset)destinationTable;


                IRelationshipClass relationshipClass = memRelClassFactory.Open(
                    destDataset.Name,
                    (IObjectClass)this.destinationTable,
                    this.destinationKeyField,
                    (IObjectClass)sourceTable,
                    this.sourceKeyFiled,
                     "forward",
                     "backward",
                     esriRelCardinality.esriRelCardinalityOneToMany);

                //'IMemoryRelationshipClassFactory::Openの引数
                //'第1引数:テーブル結合の名称（GUIでは特に使用しません）
                //'第2引数:結合先のテーブル
                //'第3引数:結合先テーブルの結合キー（フィールド名）
                //'第4引数:結合元のテーブル（フィーチャクラス）
                //'第5引数:結合元テーブルの結合キー（フィールド名）
                //'第6引数:ForwardPathLabel（この文字列で設定）
                //'第7引数:BackwardPathLabel（この文字列で設定）
                //'第8引数:リレーションシップ方法（1対1，1体多，多対多）テーブル結合の場合は上記設定とする

                IRelQueryTable relQueryTable = rqtFactory.Open(relationshipClass, true, null, null, "", true, true);

                IDisplayRelationshipClass dispRelClass = (IDisplayRelationshipClass)this.sourceLayer;
                dispRelClass.DisplayRelationshipClass(relQueryTable.RelationshipClass, esriJoinType.esriLeftOuterJoin);
            }                    
        }
        
        /// <summary>
        /// テーブル結合されているかどうかを判定します
        /// </summary>
        /// <param name="TableFields">フィーチャーレイヤー / スタンドアロンテーブル</param>
        /// <returns></returns>
        static public bool HasTableJoined(ITableFields TableFields) {
			bool	blnRet = false;

			if(TableFields is IStandaloneTable) {
				blnRet = (TableFields as IStandaloneTable).Table is IRelQueryTable;
			}
			else if(TableFields is IFeatureLayer) {
				blnRet = (TableFields as IDisplayTable).DisplayTable is IRelQueryTable;
			}
			else {
				System.Diagnostics.Debug.WriteLine("●HasTableJoined QI ERROR");
			}

			return blnRet;
        }
        /// <summary>
        /// テーブル結合されているかどうかを判定します
        /// </summary>
        /// <param name="FLayer">フィーチャーレイヤー</param>
        /// <returns></returns>
        static public bool HasTableJoined(IFeatureLayer FLayer) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return (FLayer as IDisplayTable).DisplayTable is IRelQueryTable;
        }
        /// <summary>
        /// テーブル結合されているかどうかを判定します
        /// </summary>
        /// <param name="SATable">スタンドアロンテーブル</param>
        /// <returns></returns>
        static public bool HasTableJoined(IStandaloneTable SATable) {
			// ｵｰﾊﾞｰﾛｰﾄﾞ
			return SATable.Table is IRelQueryTable;
        }
    }
}
