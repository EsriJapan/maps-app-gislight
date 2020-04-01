using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// シェープファイルにエクスポートする対象フィールドを保持する
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public class TargetFieldNameClass
    {
        private static readonly 
            TargetFieldNameClass _instance = new TargetFieldNameClass();

        private IDictionary _holder = new Hashtable(); 

        private TargetFieldNameClass()
        { 
        }

        /// <summary>
        /// インスタンスを返す
        /// </summary>
        /// <returns>TargetFieldNameClassのインスタンス</returns>
        public static TargetFieldNameClass GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// シェープファイルエクスポート対象フィールド名称文字列の設定と取得
        /// </summary>
        /// <param name="key">保持対象シェープファイルエクスポート対象フィールド名称文字列に対応するキー</param>
        /// <returns>キーに対応したシェープファイルエクスポート対象フィールド名称文字列</returns>
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
        /// キーに対応したシェープファイルエクスポート対象フィールド名称文字列の削除
        /// </summary>
        /// <param name="key">保持対象シェープファイルエクスポート対象フィールド名称文字列に対応するキー</param>
        public void Remove(string key)
        {
            _holder.Remove(key);
        }

        /// <summary>
        /// 保持したシェープファイルエクスポート対象フィールド名称文字列の全削除
        /// </summary>
        public void RemoveAll()
        {
            _holder.Clear();
        }

        /// <summary>
        /// 保持したシェープファイルエクスポート対象フィールド名称文字列に対応したキーを返す
        /// </summary>
        public ICollection Keys
        {
            get
            {
                return _holder.Keys;
            }
        }

        /// <summary>
        /// 指定キーに対応したシェープファイルエクスポート対象フィールド名称文字列の存在結果を返す
        /// </summary>
        /// <param name="key">保持対象シェープファイルエクスポート対象フィールド名称文字列に対応するキー</param>
        /// <returns>存在チェック結果</returns>
        public bool IsContain(object key)
        {
            return _holder.Contains(key);
        }

        /// <summary>
        /// 保持しているシェープファイルエクスポート対象フィールド名称文字列の個数を返す
        /// </summary>
        /// <returns>保持しているシェープファイルエクスポート対象フィールド名称文字列の個数</returns>
        public int GetCount()
        {
            return _holder.Count;
        }
    }
}
