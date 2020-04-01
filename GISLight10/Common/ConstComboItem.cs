using System;
using System.Collections.Generic;
using System.Text;


namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 定数と表示文字列を別に管理するコンボボックスアイテムとなるクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-27 GetHashCodeメソッドを追加
    /// </history>
    public class ConstComboItem
    {
        /// <summary>
        /// コンボボックスのラベルになる文字列
        /// </summary>
        protected string m_label = null;
        
        /// <summary>
        /// コンボボックスのアイテムに設定する定数
        /// </summary>
        protected int m_id = -1;


        /// <summary>
        /// クラスインスタンス
        /// </summary>
        /// <param name="id">コンボボックスのアイテムに設定する定数</param>
        /// <param name="label">コンボボックスのラベルになる文字列</param>
        public ConstComboItem(int id, string label)
        {
            m_id = id;
            m_label = label;
        }


        /// <summary>
        /// コンボボックスのアイテムに設定する定数
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
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
            return m_id.Equals(obj);
        }

        /// <summary>
        /// コンボボックスアイテム定数のハッシュコードを返す
        /// </summary>
        /// <returns>コンボボックスアイテム定数オブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }

        /// <summary>
        /// コンボボックスのラベルになる文字列を返す
        /// </summary>
        /// <returns>コンボボックスのアイテムとなったときの文字列</returns>
        public override string ToString()
        {
            return m_label;
        }
    }
}
