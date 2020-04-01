using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �V�F�[�v�t�@�C���G�N�X�|�[�g�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬
    /// </history>
    public sealed class ExportShapeFile : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public ExportShapeFile()
        {
            base.captionName = "�t�B�[�`�� �N���X�փG�N�X�|�[�g";

      
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new Bitmap(GetType(), bitmapResourceName);

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

                IFeatureLayer checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                if (featureLayerList.Contains(checkLayer))
                //if ((checkLayer != null) && !(checkLayer is GdbRasterCatalogLayer) &&
                //    !(checkLayer is CadFeatureLayer) &&
                //     (checkLayer.Valid) &&
                //     (ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(checkLayer).Type !=
                //      ESRI.ArcGIS.Geodatabase.esriWorkspaceType.esriRemoteDatabaseWorkspace) &&
                //      featureLayerList.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// �N���b�N������
        /// �V�F�[�v�t�@�C���G�N�X�|�[�g�t�H�[����\��
        /// </summary>
        public override void OnClick()
        {
            Ui.FormExportShapeFile frm = new Ui.FormExportShapeFile(mainFrm, m_mapControl);
            frm.ShowDialog(mainFrm);

            //Console.WriteLine("--->");
            //Console.WriteLine("map extent  Xmin=" + m_mapControl.Extent.XMin);
            //Console.WriteLine("map extent  Ymin=" + m_mapControl.Extent.YMin);
            //Console.WriteLine("map extent  Xmax=" + m_mapControl.Extent.XMax);
            //Console.WriteLine("map extent  Ymax=" + m_mapControl.Extent.YMax);
            //Console.WriteLine("<---");
        }
        #endregion
    }
}
