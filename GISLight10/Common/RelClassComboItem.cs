using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// リレート時に使用するコンボボックスアイテム処理
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    ///  2011-01-21 protectedメンバーのスコープ指定を削除
    ///  2011-01-27 GetHashCodeメソッドを追加
    /// </history>
    public class RelClassComboItem
    {
        string m_label = null;
        IRelationshipClass m_relClass = null;

        /// <summary>
        /// クラスインスタンス
        /// </summary>
        /// <param name="relClass">コンボボックスのアイテムに設定するIRelationshipClass</param>
        /// <param name="label">コンボボックスのラベルになる文字列</param>        
        public RelClassComboItem(IRelationshipClass relClass, string label)
        {
            m_relClass = relClass;
            m_label = label;
        }


        /// <summary>
        /// コンボボックスに設定されているIRelationshipClass
        /// </summary>          
        public IRelationshipClass RelationshipClass
        {
            get
            {
                return m_relClass;
            }
        }

        /// <summary>
        /// コンボボックスのアイテムに設定するIRelationshipClassと
        /// 引数指定されたオブジェクトの比較結果を返す
        /// </summary>
        /// <param name="obj">比較対象オブジェクト</param>
        /// <returns>IRelationshipClassと引数指定されたオブジェクトの比較結果</returns>
        public override bool Equals(object obj)
        {
            return m_relClass.Equals(obj);
        }

        /// <summary>
        /// IRelationshipClassオブジェクトのハッシュコードを返す
        /// </summary>
        /// <returns>IRelationshipClassオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return m_relClass.GetHashCode();
        }

        /// <summary>
        /// コンボボックスのアイテムとなったときの文字列を返す
        /// </summary>
        /// <returns>コンボボックスのアイテムとなったときの文字列</returns>
        public override string ToString()
        {
            return m_label;
        }
    }
}
