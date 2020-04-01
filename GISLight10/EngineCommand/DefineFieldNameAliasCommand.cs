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
    /// �t�B�[���h�̕ʖ���`���s���t�H�[�����Ăяo��
    /// </summary>
    /// <history>
    ///  2011/05/31 �V�K�쐬 
    /// </history>
    public sealed class DefineFieldNameAliasCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// constructor
        /// </summary>
        public DefineFieldNameAliasCommand()
        {
            base.captionName = "�t�B�[���h�̕ʖ���`";

        }

        #region Overriden Class Methods

        /// <summary>
        /// �R�}���h������
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = 
                System.Windows.Forms.Control.FromHandle(ptr2);
            
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// �R�}���h�N���b�N��
        /// </summary>
        public override void OnClick()
        {
            Ui.FormDefineFieldNameAlias frm = new Ui.FormDefineFieldNameAlias(mainFrm, true);
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

                IFeatureLayer checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                if (featureLayerList.Contains(checkLayer))
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
