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
    /// �����e�[�u�����J���R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬
    /// </history>
    public sealed class OpenAttributeTable : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public OpenAttributeTable()
        {
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.Magenta);

            base.m_caption = "�����e�[�u�����J��";
        }

        #region Overriden Class Methods

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

                IFeatureLayer checkLayer = this.mainForm.SelectedLayer as IFeatureLayer;
                if (featureLayerList.Contains(checkLayer))
                // �P�P�E�P�O���_�FRasterCatalog�̑����e�[�u���ǂݍ��݂͑Ή��ł��Ă��Ȃ��B
                //if ((checkLayer != null) && !(checkLayer is GdbRasterCatalogLayer) &&
                //    !(checkLayer is CadFeatureLayer) &&
                //     (checkLayer.Valid) &&
                //     (ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(checkLayer).Type !=
                //      ESRI.ArcGIS.Geodatabase.esriWorkspaceType.esriRemoteDatabaseWorkspace) &&
                //      featureLayerList.Count > 0)
                {
                    IEngineEditor engineEditor = new EngineEditorClass();

                    //�ҏW���̏ꍇ
                    if (engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)
                    {
                        return false;
                    }

                    // 2012/08/07 ADD 
                    // �W�I���t�@�����X���s���̏ꍇ
                    if (this.mainForm.HasGeoReference())
                    {
                        return false;
                    }
                    // 2012/08/07 ADD 

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

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
            mainForm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// �N���b�N������
        /// �����e�[�u���\���t�H�[���̕\��
        /// </summary>
        public override void OnClick()
        {
            Ui.FormAttributeTable frmAttr =
                new Ui.FormAttributeTable(
                    this.mainForm.SelectedLayer, this.mainForm);

            frmAttr.Show(mainForm);

            mainForm.m_ToolbarControl2.Enabled = false;
        }
        #endregion
    }
}
