
using System;
using System.Data;
using System.Collections;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ADF;

namespace ESRIJapan.GISLight10.Common
{
    // Implementation of interface IMaps which is eventually a collection of Maps
    /// <summary>
    /// ControlsSynchronizerで使用するIMaps
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成:ESRIサンプルから取り込み
    ///  2011-01-25 xmlコメントの修正
    ///  2011-01-26 インデントの修正
    /// </history>
    public class Maps : IMaps, IDisposable
    {
        //class member - using internally an ArrayList to manage the Maps collection
        private ArrayList m_array = null;

        #region class constructor
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Maps()
        {
            m_array = new ArrayList();
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose the collection
        /// </summary>
        public void Dispose()
        {
            if (m_array != null)
            {
                m_array.Clear();
                m_array = null;
            }
        }

        #endregion

        #region IMaps Members

        /// <summary>
        /// 引数指定されたインデクスに該当するIMapをArrayListコレクションから削除
        /// </summary>
        /// <param name="Index">ArrayListコレクション内でのインデクス</param>
        public void RemoveAt(int Index)
        {
            if (Index > m_array.Count || Index < 0)
                throw new Exception("Maps::RemoveAt:\r\nIndex is out of range!");

            m_array.RemoveAt(Index);
        }

        /// <summary>
        /// ArrayListコレクションに登録されたIMapをクリア
        /// </summary>
        public void Reset()
        {
            m_array.Clear();
        }

        /// <summary>
        /// ArrayListコレクションに登録されたIMapの数
        /// </summary>
        public int Count
        {
            get
            {
                return m_array.Count;
            }
        }

        /// <summary>
        /// 引数指定されたインデクスに該当するIMapをArrayListコレクションから取得
        /// </summary>
        /// <param name="Index">ArrayListコレクション内でのインデクス</param>
        /// <returns>インデクスに対応するIMap</returns>
        public IMap get_Item(int Index)
        {
            if (Index > m_array.Count || Index < 0)
                throw new Exception("Maps::get_Item:\r\nIndex is out of range!");

            return m_array[Index] as IMap;
        }

        /// <summary>
        /// 指定されたIMapをArrayListコレクションから削除
        /// </summary>
        /// <param name="Map">削除対象のIMap</param>
        public void Remove(IMap Map)
        {
            m_array.Remove(Map);
        }

        /// <summary>
        /// IMapを新規作成し、ArrayListコレクションに追加
        /// </summary>
        /// <returns>新規作成されたIMap</returns>
        public IMap Create()
        {
            IMap newMap = new MapClass();
            m_array.Add(newMap);

            return newMap;
        }

        /// <summary>
        /// IMapをArrayListコレクションに追加
        /// </summary>
        /// <param name="Map">ArrayListに追加するIMap</param>
        public void Add(IMap Map)
        {
            if (Map == null)
                throw new Exception("Maps::Add:\r\nNew Map is mot initialized!");

            m_array.Add(Map);
        }

        #endregion
    }
}
