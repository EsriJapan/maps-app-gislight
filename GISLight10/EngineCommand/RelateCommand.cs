using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �����[�g�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class RelateCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public RelateCommand()
        {
            base.captionName = "�����[�g";


            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new System.Drawing.Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }

        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// �N���b�N������
        /// �����[�g�t�H�[���̕\��
        /// </summary>
        public override void OnClick()
        {
            Ui.FormRelate frm = new Ui.FormRelate(m_mapControl);

            //IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            //System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            //mainFrm = (Ui.MainForm)cntrl2.FindForm();
            frm.ShowDialog(mainFrm);
        }

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled
        {
            get
            {
                ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                    new ESRIJapan.GISLight10.Common.LayerManager();

                List<IFeatureLayer> featureLayerList =
                    pLayerManager.GetFeatureLayers(m_mapControl.Map);

                if (featureLayerList.Count > 0)
                {
                    if (mainFrm.HasFormAttributeTable())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
