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
    /// �V���{���ݒ�N���R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class SymbolSettingsCommand : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public SymbolSettingsCommand()
        {
			//Set the tool properties
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.Magenta);

            //base.captionName = "�P��V���{���\���ݒ�";
            base.m_caption = "�V���{���ݒ�";
        }

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {
				// ڲ԰��Ȱ�ެ����擾
                ESRIJapan.GISLight10.Common.LayerManager pLayerManager = new ESRIJapan.GISLight10.Common.LayerManager();

                // �L����̨�����ڲ԰�Q���擾
                List<IFeatureLayer> featureLayerList = pLayerManager.GetFeatureLayers(m_mapControl.Map);

                // �I��ڲ԰���擾
                IFeatureLayer checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                
                // �I��ڲ԰��̨�����ڲ԰�ł���΁AOK
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
        /// �V���{���ݒ�t�H�[���̕\��
        /// </summary>
        public override void OnClick()
        {
            Ui.FormSymbolSettings frmRenderer = new Ui.FormSymbolSettings(m_mapControl);
            frmRenderer.ShowDialog(mainFrm);
        }
        #endregion
    }
}
