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
    /// �k�ڋL���̃G�������g�̏C��
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class EditScaleBarElementPropCommand : Common.EjBaseCommand
    {
        //private IMapControl3 m_mapControl;
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private IHookHelper m_hookHelper = null;

        private Ui.MainForm mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public EditScaleBarElementPropCommand()
        {
            base.captionName = "�k�ڋL���̐ݒ�";
        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�c�[���o�[�R���g���[��</param>
        public override void OnCreate(object hook) {
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

                    // �I������Ă���G�������g��1�ł���ꍇ��ΏۂƂ��܂��B
                    // �ҏW�Ώۂ̃X�P�[���o�[�G�������g�������I������Ă��Ă��A
                    // ��x�ɂ�1�̂��̂��������Ώۂɏo���Ȃ��ׁB
                    if(graphicsSelect.ElementSelectionCount == 1) {
                        IEnumElement ienum = graphicsSelect.SelectedElements;
                        ienum.Reset();
                        IElement selectedElement = ienum.Next();

                        IMapSurroundFrame selectMapSurroundFrame = selectedElement as IMapSurroundFrame;
                        IMapSurround mapSurround = selectMapSurroundFrame.MapSurround;

                        if(mapSurround is IScaleBar) {
                            blnRet = true;
                        }
                    }
                }
                return blnRet;
            }
        }

        /// <summary>
        /// �N���b�N������
        /// �k�ڋL���ݒ�t�H�[���̕\��
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

                IMapSurroundFrame selectMapSurroundFrame =
                    selectedElement as IMapSurroundFrame;

                IMapSurround mapSurround = selectMapSurroundFrame.MapSurround;
                if (mapSurround is IScaleBar)
                {
                    //IMapFrame mapFrame =
                    //    (IMapFrame)m_pageLayoutControl.ActiveView.GraphicsContainer.FindFrame(
                    //    m_pageLayoutControl.ActiveView.FocusMap);

                    IMapFrame mapFrame =
                        (IMapFrame)m_pageLayoutControl.ActiveView.GraphicsContainer.FindFrame(
                        selectMapSurroundFrame.MapFrame.Map);

                    Ui.FormScaleBarSettings frm =
                        new Ui.FormScaleBarSettings(mapFrame, m_pageLayoutControl, selectedElement);

                    frm.Text = base.captionName;
                    System.Windows.Forms.DialogResult res = frm.ShowDialog(mainFrm);

                    mainFrm.MainMapChanged = true;
                }
            }
        }
        #endregion
    }
}
