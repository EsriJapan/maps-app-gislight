using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// テーブル結合の解除関連のクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public class RemoveJoinFunctions
    {        
        private static ArrayList relQueryTableComboItemList = new ArrayList();

        /// <summary>
        /// コンストラクタ
        /// </summary>        
        public RemoveJoinFunctions()
        {
            
        }



        /// <summary>
        /// テーブル結合情報（IRelQueryTable）のリストの取得
        /// </summary>
        /// <param name="srcFcLayer">テーブル結合を持つフィーチャレイヤ</param>
        /// <returns>テーブル結合情報（IRelQueryTable）のリスト</returns>
        public static ArrayList GetRelQueryTableComboItemList(IFeatureLayer srcFcLayer)
        {
            //リストを初期化
            relQueryTableComboItemList.Clear();

            IDisplayTable displayTable = (IDisplayTable)srcFcLayer;
            ITable table = displayTable.DisplayTable;

            //テーブル結合を持っていない場合は、nullになる。
            IRelQueryTable relQueryTable = table as IRelQueryTable;            

            //結合元レイヤに結合されているテーブルの一覧をjoinedTableListに格納する。
            SetRelQueryTableComboItemList(relQueryTable);

            return relQueryTableComboItemList;
        }


        /// <summary>
        /// 結合先テーブル（リレーション情報）のリストを設定
        /// </summary>
        /// <param name="relQueryTable">IRelQueryTable</param>
        private static void SetRelQueryTableComboItemList(IRelQueryTable relQueryTable)
        {
            ITable rqSourceTable = relQueryTable.SourceTable;
            IDataset pSourceTableDataset = (IDataset)rqSourceTable;
            IRelQueryTable sourceRelQueryTable = rqSourceTable as IRelQueryTable;

            IDataset destDataset = (IDataset)relQueryTable.DestinationTable;
            string datasetName;

            if (destDataset.Category == "dBASE テーブル"
                || destDataset.Category == "シェープファイル フィーチャクラス")
            {
                datasetName = destDataset.Name + ".dbf";
            }
            else
            {
                datasetName = destDataset.Name;
            }

            //コンボボックスに追加した際、ラベルが適切に表示されるための処理
            ESRIJapan.GISLight10.Common.RelQueryTableComboItem relClassComboItem =
                   new ESRIJapan.GISLight10.Common.RelQueryTableComboItem(relQueryTable, datasetName);

            relQueryTableComboItemList.Add(relClassComboItem);

            if (sourceRelQueryTable != null)
            {
                // Call this method on the source table. 
                ////Console.WriteLine("S[R]:" + pSourceTableDataset.Name);

                SetRelQueryTableComboItemList(sourceRelQueryTable);
            }
        }


        /// <summary>
        /// 指定したテーブル結合の解除
        /// </summary>
        /// <param name="srcFcLayer">テーブル結合を持つフィーチャレイヤ</param>
        /// <param name="relQueryTable">解除するテーブル(IRelQueryTable)</param>
        public static void RemoveJoin(IFeatureLayer srcFcLayer, IRelQueryTable relQueryTable)
        {
            //解除は、結合情報の木構造（IRelQueryTableのHelp参照）を上書きすることで可能

            IDisplayRelationshipClass dispRelClass = (IDisplayRelationshipClass)srcFcLayer;

            IRelQueryTable sourceRelTable = relQueryTable.SourceTable as IRelQueryTable;
                       
            //nullの場合は、一番根っこのテーブル結合
            if (sourceRelTable != null)
            {
                dispRelClass.DisplayRelationshipClass
                    (sourceRelTable.RelationshipClass, esriJoinType.esriLeftOuterJoin);
            }
            else
            {
                dispRelClass.DisplayRelationshipClass
                    (null, esriJoinType.esriLeftOuterJoin);
            }         
        }



        /// <summary>
        /// すべてのテーブル結合の解除
        /// </summary>
        /// <param name="srcFcLayer">テーブル結合を持つフィーチャレイヤ</param>        
        public static void RemoveAllJoin(IFeatureLayer srcFcLayer)
        {   
            IDisplayRelationshipClass dispRelClass = (IDisplayRelationshipClass)srcFcLayer;            
   
            //nullを指定することで、すべての結合を解除できる
            dispRelClass.DisplayRelationshipClass(null, esriJoinType.esriLeftOuterJoin);            
        }


    }
}

