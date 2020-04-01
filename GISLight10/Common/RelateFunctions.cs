using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// リレート関連のクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-21 protectedメンバーのスコープ指定を削除
    ///  2012-08-02 IMemoryRelationshipClassFactory, IRelQueryTableFactoryのインスタンス作成を変更
    /// </history>
    public class RelateFunctions
    {
        IFeatureLayer sourceFcLayer = null;
        ITable destinationTable = null;       
        string sourceKeyFiled = "";
        string destinationKeyField = "";
        string relateName = "";
        

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcFcLayer">ソースフィチャーレイヤ</param>
        /// <param name="destTable">リレート先テーブル</param>
        /// <param name="srcKeyField">ソースレイヤキーフィールド</param>
        /// <param name="destKeyField">リレート先テーブルキーフィールド名</param>
        /// <param name="name">リレート名</param>
        public RelateFunctions(IFeatureLayer srcFcLayer, ITable destTable,
            string srcKeyField, string destKeyField, string name)
        {
            this.sourceFcLayer = srcFcLayer;
            this.destinationTable = destTable;
            this.sourceKeyFiled = srcKeyField;
            this.destinationKeyField = destKeyField;
            this.relateName = name;
        }


        /// <summary>
        /// リレートの実行
        /// </summary>
        public void Relate()
        {
            //★DidplayTableからQIしないとテーブル結合しているレイヤに対してリレートできない。
            IDisplayTable dispTable = (IDisplayTable)sourceFcLayer;
            ITable sourceTable = (ITable)dispTable.DisplayTable;


            IDataset destDataset = (IDataset)this.destinationTable;

            // Build a memory relationship class.
            IMemoryRelationshipClassFactory memRelClassFactory = 
                SingletonUtility.NewMemoryRelationshipClassFactory();

            // Open the RelQueryTable as a feature class.
            IRelQueryTableFactory rqtFactory =
                SingletonUtility.NewRelQueryTableFactory();

            IRelationshipClass relationshipClass = memRelClassFactory.Open(
                this.relateName,
                (IObjectClass)this.destinationTable,
                this.destinationKeyField,
                (IObjectClass)sourceTable,
                this.sourceKeyFiled,
                "backward",
                destDataset.Name,
                esriRelCardinality.esriRelCardinalityManyToMany);

            
            IRelationshipClassCollectionEdit relClassColEdit = 
                (IRelationshipClassCollectionEdit)sourceFcLayer;
            
            relClassColEdit.AddRelationshipClass(relationshipClass);
        }        
    }
}
