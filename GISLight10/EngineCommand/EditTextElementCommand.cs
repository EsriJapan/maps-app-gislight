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
    /// �e�L�X�g�G�������g�̕ҏW�i�C���j
    /// </summary>
    public sealed class EditTextElementCommand : Common.EjBaseCommand
    {
        //private IMapControl3 m_mapControl;
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private IHookHelper m_hookHelper = null;

        private Ui.MainForm mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public EditTextElementCommand()
        {
            base.captionName = "�e�L�X�g�̕ҏW";
        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�c�[���o�[�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
				if(this.mainFrm == null) {
					if(m_hookHelper.Hook is IMapControl3) {
						this.m_pageLayoutControl = (IPageLayoutControl3)m_hookHelper.Hook;
					}
					else if(m_hookHelper.Hook is IToolbarControl2) {
						IToolbarControl2	agToolCtl = m_hookHelper.Hook as IToolbarControl2;
						this.m_pageLayoutControl = agToolCtl.Buddy as IPageLayoutControl3;
					}

					// Ҳݥ̫�т��擾
					if(this.m_pageLayoutControl != null) {
						System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle((IntPtr)m_pageLayoutControl.hWnd);
						this.mainFrm = (Ui.MainForm)cntrl2.FindForm();
					}
				}

                if(this.mainFrm != null) {
                    IPageLayout pl = mainFrm.axPageLayoutControl1.PageLayout;
                    IGraphicsContainerSelect graphicsSelect = pl as IGraphicsContainerSelect;
                    if(graphicsSelect.ElementSelectionCount > 0) {
                        IEnumElement ienum = graphicsSelect.SelectedElements;
                        ienum.Reset();
                        IElement selectedElement = ienum.Next();

                        //ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                        //    new ESRIJapan.GISLight10.Common.LayerManager();

                        //List<IFeatureLayer> featureLayerList =
                        //    pLayerManager.GetFeatureLayers(m_pageLayoutControl.ActiveView.FocusMap);

                        //IFeatureLayer checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
                        //if ((checkLayer != null) && !(checkLayer is GdbRasterCatalogLayer) &&
                        //    !(checkLayer is CadFeatureLayer) &&
                        //     (checkLayer.Valid) &&
                        // (ESRIJapan.GISLight10.Common.LayerManager.getWorkspace(checkLayer).Type !=
                        //      ESRI.ArcGIS.Geodatabase.esriWorkspaceType.esriRemoteDatabaseWorkspace) &&
                        //if (featureLayerList.Count > 0 &&
                        if(selectedElement != null && selectedElement is ITextElement) //)
                        {
                            blnRet = true;
                        }
                    }
                }

				return blnRet;
            }
        }

        /// <summary>
        /// �N���b�N������
        /// �e�L�X�g�G�������g�t�H�[�����C�����[�h�ŕ\��
        /// </summary>
        public override void OnClick()
        {
            IPageLayout pl = mainFrm.axPageLayoutControl1.PageLayout;
            IGraphicsContainerSelect graphicsSelect = pl as IGraphicsContainerSelect;

            if (graphicsSelect.ElementSelectionCount > 0)
            {
                IEnumElement ienum = graphicsSelect.SelectedElements;
                ienum.Reset();
                IElement selectedElement = ienum.Next();
                if (selectedElement != null)
                {
                    if (selectedElement is ITextElement)
                    {
                        ITextElement tel = selectedElement as ITextElement;
                        IGraphicsContainer graphicsContainer =
                            this.mainFrm.axPageLayoutControl1.PageLayout as IGraphicsContainer;

                        Ui.FormText frm = new Ui.FormText(
                            this.mainFrm.axPageLayoutControl1.Object as IPageLayoutControl3,
                            tel, graphicsContainer);

                        frm.ShowDialog(mainFrm);

                        mainFrm.MainMapChanged = true;
                    }
                }
            }
        }
        #endregion
    }
}
