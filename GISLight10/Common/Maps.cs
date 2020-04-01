
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
    /// ControlsSynchronizer�Ŏg�p����IMaps
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬:ESRI�T���v�������荞��
    ///  2011-01-25 xml�R�����g�̏C��
    ///  2011-01-26 �C���f���g�̏C��
    /// </history>
    public class Maps : IMaps, IDisposable
    {
        //class member - using internally an ArrayList to manage the Maps collection
        private ArrayList m_array = null;

        #region class constructor
        
        /// <summary>
        /// �R���X�g���N�^
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
        /// �����w�肳�ꂽ�C���f�N�X�ɊY������IMap��ArrayList�R���N�V��������폜
        /// </summary>
        /// <param name="Index">ArrayList�R���N�V�������ł̃C���f�N�X</param>
        public void RemoveAt(int Index)
        {
            if (Index > m_array.Count || Index < 0)
                throw new Exception("Maps::RemoveAt:\r\nIndex is out of range!");

            m_array.RemoveAt(Index);
        }

        /// <summary>
        /// ArrayList�R���N�V�����ɓo�^���ꂽIMap���N���A
        /// </summary>
        public void Reset()
        {
            m_array.Clear();
        }

        /// <summary>
        /// ArrayList�R���N�V�����ɓo�^���ꂽIMap�̐�
        /// </summary>
        public int Count
        {
            get
            {
                return m_array.Count;
            }
        }

        /// <summary>
        /// �����w�肳�ꂽ�C���f�N�X�ɊY������IMap��ArrayList�R���N�V��������擾
        /// </summary>
        /// <param name="Index">ArrayList�R���N�V�������ł̃C���f�N�X</param>
        /// <returns>�C���f�N�X�ɑΉ�����IMap</returns>
        public IMap get_Item(int Index)
        {
            if (Index > m_array.Count || Index < 0)
                throw new Exception("Maps::get_Item:\r\nIndex is out of range!");

            return m_array[Index] as IMap;
        }

        /// <summary>
        /// �w�肳�ꂽIMap��ArrayList�R���N�V��������폜
        /// </summary>
        /// <param name="Map">�폜�Ώۂ�IMap</param>
        public void Remove(IMap Map)
        {
            m_array.Remove(Map);
        }

        /// <summary>
        /// IMap��V�K�쐬���AArrayList�R���N�V�����ɒǉ�
        /// </summary>
        /// <returns>�V�K�쐬���ꂽIMap</returns>
        public IMap Create()
        {
            IMap newMap = new MapClass();
            m_array.Add(newMap);

            return newMap;
        }

        /// <summary>
        /// IMap��ArrayList�R���N�V�����ɒǉ�
        /// </summary>
        /// <param name="Map">ArrayList�ɒǉ�����IMap</param>
        public void Add(IMap Map)
        {
            if (Map == null)
                throw new Exception("Maps::Add:\r\nNew Map is mot initialized!");

            m_array.Add(Map);
        }

        #endregion
    }
}
