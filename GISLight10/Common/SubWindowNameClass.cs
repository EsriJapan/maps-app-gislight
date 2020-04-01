using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 開いているサブウィンドウの名称文字列を保持する
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public class SubWindowNameClass
    {
        private static readonly 
            SubWindowNameClass _instance = new SubWindowNameClass();

        private IDictionary _holder = new Hashtable();

        private SubWindowNameClass()
        { 
        }

        /// <summary>
        /// インスタンスを返す
        /// </summary>
        /// <returns>SubWindowNameClassのインスタンス</returns>
        public static SubWindowNameClass GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// サブウィンドウ名称文字列の設定と取得
        /// </summary>
        /// <param name="key">保持対象サブウィンドウ名称文字列に対応するキー</param>
        /// <returns>キーに対応したサブウィンドウ名称文字列</returns>
        public object this[object key]
        {
            get
            {
                return _holder[key];
            }
            set
            {
                if (_holder.Contains(key))
                {
                    _holder.Remove(key);
                }
                _holder[key] = value;
            }
        }

        /// <summary>
        /// キーに対応したサブウィンドウ名称文字列の削除
        /// </summary>
        /// <param name="key">保持対象サブウィンドウ名称文字列に対応するキー</param>
        public void Remove(string key)
        {
            _holder.Remove(key);
        }

        /// <summary>
        /// 保持したサブウィンドウ名称文字列の全て削除
        /// </summary>
        public void RemoveAll()
        {
            _holder.Clear();
        }

        /// <summary>
        /// 保持したサブウィンドウ名称文字列のキーを取得
        /// </summary>
        public ICollection Keys
        {
            get
            {
                return _holder.Keys;
            }
        }

        /// <summary>
        /// キーに対応したサブウィンドウ名称文字列の存在チェック
        /// </summary>
        /// <param name="key">保持対象サブウィンドウ名称文字列に対応するキー</param>
        /// <returns>存在チェック結果</returns>
        public bool IsContain(object key)
        {
            return _holder.Contains(key);
        }

        /// <summary>
        /// 保持しているサブウィンドウ名称文字列の個数を返す
        /// </summary>
        /// <returns>保持しているサブウィンドウ名称文字列の個数</returns>
        public int GetCount()
        {
            return _holder.Count;
        }
    }
}
