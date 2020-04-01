using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// リレート解除関連のクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public class RemoveRelateFunctions
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RemoveRelateFunctions()
        {

        }


        /// <summary>
        /// リレート情報（IRelQueryTable）のリストの取得
        /// </summary>
        /// <param name="srcFcLayer">リレートを持つフィーチャレイヤ</param>
        /// <returns>リレート情報（IRelationshipClass）のリスト</returns>
        public static ArrayList GetRelClassItemList(IFeatureLayer srcFcLayer)
        {
            ArrayList relClassItemList = new ArrayList();

            //リレート元レイヤのRelationshipClass一覧の取得
            IRelationshipClassCollection relClassCol = (IRelationshipClassCollection)srcFcLayer;
            IEnumRelationshipClass enumRelClass = relClassCol.RelationshipClasses;

            enumRelClass.Reset();
            IRelationshipClass relClass = enumRelClass.Next();

            IDataset relClassDataSet = null;

            while (relClass != null)
            {
                relClassDataSet = (IDataset)relClass;

                //コンボボックスに追加した際、ラベルが適切に表示されるための処理
                ESRIJapan.GISLight10.Common.RelClassComboItem relClassComboItem =
                    new ESRIJapan.GISLight10.Common.RelClassComboItem(relClass, relClassDataSet.Name);

                relClassItemList.Add(relClassComboItem);

                relClass = enumRelClass.Next();
            }

            return relClassItemList;
        }


        /// <summary>
        /// 指定したリレートの削除
        /// </summary>
        /// <param name="srcFcLayer">リレートを持つフィーチャレイヤ</param>
        /// <param name="relClass">リレート情報（IRelationshipClass）</param>
        public static void RemoveRelate(IFeatureLayer srcFcLayer, IRelationshipClass relClass)
        {
            IRelationshipClassCollectionEdit relClassColEdit =
                (IRelationshipClassCollectionEdit)srcFcLayer;

            relClassColEdit.RemoveRelationshipClass(relClass);
        }

        /// <summary>
        /// すべてのリレートの削除
        /// </summary>
        /// <param name="srcFcLayer">リレートを持つフィーチャレイヤ</param>
        public static void RemoveAllRelate(IFeatureLayer srcFcLayer)
        {
            IRelationshipClassCollectionEdit relClassColEdit =
                (IRelationshipClassCollectionEdit)srcFcLayer;

            relClassColEdit.RemoveAllRelationshipClasses();
        }
    }
}
