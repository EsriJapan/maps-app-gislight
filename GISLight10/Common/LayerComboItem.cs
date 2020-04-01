using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// コンボボックスのアイテムにレイヤを設定するクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    ///  2011-01-21 protectedメンバのスコープ指定記述を削除
    ///  2011-01-25 xmlコメントreturnsタグ記述漏れの対応
    ///  2011-01-27 GetHashCodeメソッドを追加
    /// </history>
    public class LayerComboItem
    {
        const string FORMAT = "{0}";
        int index = -1;
        ILayer item = null;
        string format_tostring = FORMAT;


        /// <summary>
        /// レイヤのコンストラクタ。コンボボックスのアイテムとなるレイヤを設定する。
        /// </summary>
        /// <param name="pLayer">レイヤ</param>
        public LayerComboItem(ILayer pLayer)
        {
            item = pLayer;
        }


        /// <summary>
        /// レイヤのコンストラクタ。コンボボックスのアイテムとなるレイヤとレイヤインデックスを設定する。
        /// </summary>
        /// <param name="pLayer">レイヤ</param>
        /// <param name="layerindex">レイヤのインデックス</param>
        public LayerComboItem(ILayer pLayer, int layerindex)
        {
            item = pLayer;
            index = layerindex;
        }


        /// <summary>
        /// コンボボックスに設定されているレイヤのインデックス番号
        /// </summary>
        public int LayerIndex
        {
            get
            {
                return index;
            }
        }


        /// <summary>
        /// コンボボックスに設定されているレイヤ
        /// </summary>
        public ILayer Layer
        {
            get
            {
                return item;
            }
        }


        /// <summary>
        /// コンボボックスのアイテムとなったときに出力する文字列の書式
        /// </summary>
        public string FormatToStringName
        {
            set
            {
                format_tostring = value;
            }
        }


        /// <summary>
        /// コンボボックスのアイテムとなったときの文字列を返す
        /// </summary>
        /// <returns>コンボボックスのアイテムとなったときの文字列</returns>
        public override string ToString()
        {
            return string.Format(format_tostring, item.Name);

        }

        /// <summary>
        /// コンボボックスのアイテムにレイヤと引数指定されたコンボボックスのアイテムの比較
        /// </summary>
        /// <param name="obj">比較対象のレイヤ</param>
        /// <returns>比較結果</returns>
        public override bool Equals(object obj)
        {
            return item.Equals(obj);
        }

        /// <summary>
        /// ILayerオブジェクトのハッシュコードを返す
        /// </summary>
        /// <returns>ILayerオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return item.GetHashCode();
        }
    }
}
