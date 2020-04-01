using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// テーブル結合の解除時のコンボボックス処理
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-21 protectedメンバーのスコープ指定を削除
    ///  2011-01-27 GetHashCodeメソッドを追加
    /// </history>
    public class RelQueryTableComboItem
    {
        string m_label = null;        
        IRelQueryTable m_relQueryTable = null;


        /// <summary>
        /// クラスインスタンス
        /// </summary>
        /// <param name="relQueryTable">コンボボックスのアイテムに設定するIRelQueryTable </param>
        /// <param name="label">コンボボックスのラベルになる文字列</param>        
        public RelQueryTableComboItem(IRelQueryTable relQueryTable, string label)
        {
            m_relQueryTable = relQueryTable;
            m_label = label;
        }


        /// <summary>
        /// コンボボックスに設定されているIRelQueryTable
        /// </summary>        
        public IRelQueryTable RelQueryTable
        {
            get
            {
                return m_relQueryTable;
            }
        }

        /// <summary>
        /// コンボボックスのアイテムに設定する定数と
        /// 引数指定されたオブジェクトの比較結果を返す
        /// </summary>
        /// <param name="obj">比較対象オブジェクト</param>
        /// <returns>引数指定されたオブジェクトの比較結果</returns>
        public override bool Equals(object obj)
        {
            return m_relQueryTable.Equals(obj);
        }

        /// <summary>
        /// IRelQueryTableオブジェクトのハッシュコードを返す
        /// </summary>
        /// <returns>IRelQueryTableオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return m_relQueryTable.GetHashCode();
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

