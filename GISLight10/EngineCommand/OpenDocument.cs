#define usestndcmd__

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Output;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �T�|�[�g�ΏۃL�������g���J���R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public class OpenDocument : BaseCommand
    {

        private IHookHelper m_hookHelper = null;
        private IMapControl3 m_mapControl;
        private IPageLayoutControl3 m_pageLayoutControl = null;

        private Ui.MainForm mainForm;
        private ESRIJapan.GISLight10.Common.ControlsSynchronizer m_controlsSynchronizer = null;

        private ESRIJapan.GISLight10.Common.SubWindowNameClass subwinname = 
            ESRIJapan.GISLight10.Common.SubWindowNameClass.GetInstance();

        /// <summary>
        /// �}�b�v�R���g���[�����烁�C���t�H�[���ւ̎Q�Ƃ��擾
        /// </summary>
        /// <param name="mapControl">�}�b�v�R���g���[��</param>
        private void SetMainFormObj(IMapControl3 mapControl) {
            if (mapControl == null) return;

            m_mapControl = mapControl;
            IntPtr ptr2 = (System.IntPtr)mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();

            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;

        }
 
        /// <summary>
        /// ���݊J���Ă���MXD�̏�Ԃ��������܂�
        /// </summary>
        /// <returns>OK / NG</returns>
        private bool CompleteOpenedMXDStates() {
			bool	blnRet = true;

            IEngineEditor	engineEditor = new EngineEditorClass();
            DialogResult	result;

            // �ҏW�L����R��
            if((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) &&
                (engineEditor.HasEdits() == true))
            {
                result = MessageBox.Show(
                    "�ҏW���̏�Ԃ�ۑ����܂����H",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch(result) {
                case DialogResult.Cancel:
                    blnRet = false;
                    break;

                case DialogResult.No:
                    engineEditor.StopEditing(false);
                    break;

                case DialogResult.Yes:
                    engineEditor.StopEditing(true);
                    break;
                }
            }
            
            // �޷���ĕۑ�
            if(blnRet && mainForm.MainMapChanged) {
                DialogResult res = MessageBox.Show(
                    "���݂̃h�L�������g��ۑ����܂���?",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch(res) {
                case DialogResult.Cancel:
                    blnRet = false;
                    break;

                case DialogResult.No:
                    if(engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                        engineEditor.StopEditing(false);
                    }
                    break;

                case DialogResult.Yes:
                    if(engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                        engineEditor.StopEditing(true);
                    }

                    // MXD��ۑ�
                    ICommand command = new ControlsSaveAsDocCommandClass();
                    command.OnCreate(m_hookHelper.Hook);
                    command.OnClick();
                    break;
                }
            }
            
            return blnRet;
		}

        /// <summary>
        /// �}�b�v�E�h�L�������g��W�J���܂��iPageLayout.ReplaceMaps�����j
		/// ��10.0�ł́A����ϯ�߂�ڲ��Ă���Ă���ꍇ�Aϯ�߂̔z�u��������Ă��܂��s����������B
        /// </summary>
        /// <param name="sFilePath">MXḐ�٥�߽</param>
		/// <returns>OK / NG</returns>
		private bool OpenSupportDocument(string sFilePath) {
            IMapDocument mapDoc = new MapDocumentClass();

            // �}�b�v�h�L�������g���J���ꍇ�Ɏ��Ԃ��|���邱�Ƃ�����̂ŃJ�[�\����Wait�J�[�\���ɕύX
            System.Windows.Forms.Cursor preCursor = mainForm.Cursor;
            mainForm.Cursor = Cursors.WaitCursor;
            
            // ϯ�ߺ��۰َ擾�⏕(�ƭ�/°َ��s���ɂ���āAϯ�ߺ��۰ق���Ă���Ȃ���)
            if(m_mapControl == null) {
				m_mapControl = mainForm.axMapControl1.Object as IMapControl3;
            }

            try {
                if(mapDoc.get_IsPresent(sFilePath) && !mapDoc.get_IsPasswordProtected(sFilePath)) {
                    mapDoc.Open(sFilePath, string.Empty);
                    
                    // 2011/06/16 �Œ�k�ڂ̏ꍇ�ɉ���
                    IPageLayout pageLayout = mapDoc.PageLayout;

                    // �޷���Ă��������ݒ�
                    if(mapDoc.Printer != null) {
						// ������ݒ�̕���
						if(!mainForm.LoadPrintDocument(mapDoc.Printer)) {
							// �����������A4�ݒ� (�ݒ���������Ȃ��Ȃ��Ă���ꍇ)
							//mainForm.SetDefaultPrinter();
							mainForm.InitPrintSettings();
						}
					}
					else {
						//mainForm.SetDefaultPrinter();
					}
					mainForm.LoadPrintSetting(pageLayout);
                    
                    mainForm.originalMaps.Reset();
                    if(mapDoc.MapCount > 0) {
                        for(int i = 0; i < mapDoc.MapCount; i++) {
#if DEBUG
							Debug.WriteLine(mapDoc.get_Map(i).Name);
#endif
                            mainForm.originalMaps.Add(mapDoc.get_Map(i));
                        }
                    }

                    IMapFrame			mapFrame;
                    IMapSurroundFrame	agMSF;

                    IGraphicsContainer graphicsContainer = (IGraphicsContainer)pageLayout;
                    graphicsContainer.Reset();

                    IElement element = graphicsContainer.Next();
                    while(element != null) {
                        if(element is IMapFrame) {
                            mapFrame = (IMapFrame)element;

                            if(mapFrame.ExtentType != esriExtentTypeEnum.esriExtentDefault) {
                                mapFrame.ExtentType = esriExtentTypeEnum.esriExtentDefault;
                            }
                        }
                        else if(element is IMapSurroundFrame) {
							agMSF = element as IMapSurroundFrame;
							if(agMSF.MapSurround is ILegend) {
								agMSF.MapSurround.Refresh();

								// �}��̎���ڲԒǉ��ݒ�������I��OFF
								ILegend	agLeg = agMSF.MapSurround as ILegend;
								if(agLeg.AutoAdd) {
									agLeg.AutoAdd = false;
								}
							}
                        }

                        element = graphicsContainer.Next();
                    }
                    
					IActiveView	agActView = m_mapControl.Map as IActiveView;
					agActView.Clear();
					agActView.GraphicsContainer.DeleteAllElements();
					
					//m_controlsSynchronizer.PageLayoutControl.LoadMxFile(mapDoc.DocumentFilename, Type.Missing);

                    m_controlsSynchronizer.PageLayoutControl.DocumentFilename = mapDoc.DocumentFilename;
                    m_controlsSynchronizer.PageLayoutControl.PageLayout = pageLayout;
                    if(mapDoc.MapCount <= 1) {
						m_controlsSynchronizer.PageLayoutControl.PageLayout.ReplaceMaps(mainForm.originalMaps);
                    }

					m_mapControl.DocumentFilename = mapDoc.DocumentFilename;
					//m_mapControl.Map = mapDoc.ActiveView.FocusMap;
					m_controlsSynchronizer.ReplaceMap(mapDoc.ActiveView.FocusMap);

                    // �����ĥ�ިڸ�؂�ݒ肵�Ă�
                    Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(mapDoc.DocumentFilename);
#if DEBUG
                    // ʲ�߰�ݸ����� �������߽̫��ް�̏ꍇ�A�J����Ȃ����ۂ̊m�F (ArcMap�ł͖��Ȃ�)
                    // ���L�����è��ݒ蒼���̫��ް���J�����悤�ɂȂ�?���A���̌㌳�ɖ߂��Ă��܂��B
                    IDocumentInfo2	agDocInfo2 = mapDoc as IDocumentInfo2;
                    if(string.IsNullOrEmpty(agDocInfo2.HyperlinkBase)) {
						agDocInfo2.HyperlinkBase = System.IO.Path.GetDirectoryName(mapDoc.DocumentFilename);
                    }
#endif

                    // 2010-12-27 add (���߃��C���̃}�b�v�\������Ȃ����̑Ή�)
                    m_mapControl.Map = mapDoc.ActiveView.FocusMap;
                    m_mapControl.ActiveView.Refresh();
                    
                    if(mapDoc.ActiveView is IMap) {
						mainForm.tabControl1.SelectedIndex = 0;
                    }
                    else {
						mainForm.tabControl1.SelectedIndex = 1;
                    }

                    if(mainForm.tabControl1.SelectedIndex == 1) {
                        m_controlsSynchronizer.ActivatePageLayout();
                        mainForm.axPageLayoutControl1.ActiveView.Refresh();
                    }
                    // end add

                    mapDoc.Close();
                    
                    return true;
                }
                return false;
            }
            catch(COMException comex) {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
                return false;
            }
            catch(Exception ex) {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
                return false;
            }
            finally {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mapDoc);
                mainForm.Cursor = preCursor; // ���̃J�[�\���ɖ߂�
            }
        }
        

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public OpenDocument() {
            base.m_caption = "�J��";
            base.m_toolTip = "�J��";
            try {
                string bitmapResourceName = this.GetType().Name + ".bmp";
                base.m_bitmap = 
                    new Bitmap(this.GetType(), bitmapResourceName);

                //this.m_controlsSynchronizer = controlsSynchronizer;

            }
            catch(Exception ex) {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
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
			if (hook == null)
				return;

			if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// �N���b�N������
        /// �t�@�C���_�C�A���O��\��
        /// </summary>
        public override void OnClick() {
			if(this.m_hookHelper == null) {
				return;
			}
			if(this.m_mapControl == null) {
				if(m_hookHelper.Hook is IMapControl3) {
					this.m_mapControl = (IMapControl3)m_hookHelper.Hook;
				}
				else if(m_hookHelper.Hook is IToolbarControl2) {
					IToolbarControl2	agToolCtl = m_hookHelper.Hook as IToolbarControl2;
					this.m_mapControl = agToolCtl.Buddy as IMapControl3;
				}
				else {
					return;
				}
				SetMainFormObj(this.m_mapControl);
			}

            try {
                IMapControl3 mapControl = m_mapControl;

                //get the MapControl from the hook in case the container is a ToolbarControl
                if(m_hookHelper.Hook is IToolbarControl) {
                    try {
                        mapControl = (IMapControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;
                    }
                    catch { 
                        mapControl = m_mapControl;
                        if(mapControl == null) {
                            m_pageLayoutControl = (IPageLayoutControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;

                            IntPtr ptr2 = (System.IntPtr)m_pageLayoutControl.hWnd;
                            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                            mainForm = (Ui.MainForm)cntrl2.FindForm();
                            mapControl = mainForm.MapControl;

                            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;
                        }
                    }
                }
                //In case the container is MapControl
                else if(m_hookHelper.Hook is IMapControl3) {
                    mapControl = (IMapControl3)m_hookHelper.Hook;
                }
                else {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(
                        mainForm, "�A�N�e�B�u�R���g���[���̓}�b�v�R���g���[���ł���K�v������܂�");
                    return;
                }

                //allow the user to save the current document
                if (mainForm == null) SetMainFormObj(mapControl);

                // ���݂̕ҏW����݂��I��������, ϯ�߂̕ύX��ۑ�
                if(!this.CompleteOpenedMXDStates()) {
					return;
				}

                // 2011/06/09 
                // �V���N���i�C�U�g�p�̉e���Ńh�L�������g�J���ۂɂ̓}�b�v�^�u�J�����g���K�{
                // �������Ȃ��ƌŒ�k�ڂ�MXD�J���ۂɃ}�b�v���ŌŒ�k�ڂł͂Ȃ��Ȃ�
                // �{���_�C�A���O������ɂ��������A��������Ɠ���s���ɂȂ�̂ł����ōs�Ȃ�
                //if (mainForm != null && mainForm.tabControl1 != null)
                //{
                //    if (mainForm.tabControl1.SelectedIndex != 0)
                //    {
                //        mainForm.tabControl1.SelectedIndex = 0;
                //    }
                //}
                // 

#if usestndcmd

                //2011/06/08-->
                string currentDocPath = string.Empty;
                if (mainForm.tabControl1.SelectedIndex == 0)
                {
                    if (mainForm.axMapControl1 != null && 
                        mainForm.axMapControl1.DocumentFilename != null &&
                        mainForm.axMapControl1.DocumentFilename.Length > 0 && 
                        System.IO.File.Exists(mainForm.axMapControl1.DocumentFilename))
                    {
                        currentDocPath = mainForm.axMapControl1.DocumentFilename;
                    }
                }
                else
                {
                    if (mainForm.axPageLayoutControl1 != null &&
                        mainForm.axPageLayoutControl1.DocumentFilename != null &&
                        mainForm.axPageLayoutControl1.DocumentFilename.Length > 0 &&
                        System.IO.File.Exists(mainForm.axPageLayoutControl1.DocumentFilename))
                    {
                        currentDocPath = mainForm.axPageLayoutControl1.DocumentFilename;
                    }
                }

                ICommand cmd = new ControlsOpenDocCommandClass();
                cmd.OnCreate(m_hookHelper.Hook);
                cmd.OnClick();
                
                string docpath = string.Empty;

                if (mainForm.tabControl1.SelectedIndex == 0)
                {
                    if (mainForm.axMapControl1.DocumentFilename == null ||
                        mainForm.axMapControl1.DocumentFilename.Length == 0 ||
                        !System.IO.File.Exists(mainForm.axMapControl1.DocumentFilename))
                    {
                        return;
                    }
                    else
                    {
                        docpath = mainForm.axMapControl1.DocumentFilename;
                    }
                }
                else
                {
                    if (mainForm.axPageLayoutControl1.DocumentFilename == null ||
                        mainForm.axPageLayoutControl1.DocumentFilename.Length == 0 ||
                        !System.IO.File.Exists(mainForm.axPageLayoutControl1.DocumentFilename))
                    {
                        return;
                    }
                    else
                    {
                        docpath = mainForm.axPageLayoutControl1.DocumentFilename;
                    }
                }

                if (currentDocPath.Equals(docpath))
                {
                    // �ύX�Ȃ�
                    return;
                }

                string filepath = System.IO.Path.GetFileName(docpath);
                if (filepath != null)
                {
                    // �J���Ă��鑮���e�[�u���t�H�[������Ε���
                    ESRIJapan.GISLight10.Common.UtilityClass.DeleteAttributeTableObjects();

                    if (OpenSupportDocument(docpath))
                    {
                        this.mainForm.Text = filepath + " - " +
                            Properties.Resources.CommonMessage_ApplicationName;
                        this.mainForm.SetEvents();
                        this.mainForm.SetPageLayoutEvents();
                        this.mainForm.ClearMapChangeCount();
                    }
                }
                //return;
                //<--
#else
                // 2011/06/09 del -->
                OpenFileDialog ofd = new OpenFileDialog();
                try
                {

                    ofd.FileName = "";
                    ofd.InitialDirectory = "";

                    ofd.Filter =
                        "ArcMap�h�L�������g (*.mxd)|*.mxd|" +
                        "ArcMap�e���v���[�g (*.mxt)|*.mxt|" +
                        "�p�u���b�V���}�b�v�h�L�������g (*.pmf)|*.pmf|" +
                        "�T�|�[�g����S�Ẵ}�b�v�t�H�[�}�b�g (*.mxd, *.mxt, *.pmf)|*.mxd;*.mxt;*.pmf";

                    ofd.FilterIndex = 0;
                    ofd.Title = "�J��";
                    ofd.RestoreDirectory = true;
                    ofd.CheckFileExists = true;
                    ofd.CheckPathExists = true;
                    if (ofd.ShowDialog(mainForm) == DialogResult.OK)
                    {
                        string path = System.IO.Path.GetFileName(ofd.FileName);
                        if (path != null)
                        {
                            //this.mainForm.ClearPageLayoutEvents();


                            // �J���Ă���W�I���t�@�����X�t�H�[������Ε���
                            if (this.mainForm.OwnedForms.Length > 0)
                            {
                                for (int i = 0; i < this.mainForm.OwnedForms.Length; i++)
                                {
                                    if (this.mainForm.OwnedForms[i].Text.Contains("�W�I���t�@�����X"))
                                    {
                                        this.mainForm.OwnedForms[i].Dispose();
                                        break;
                                    }
                                }
                            }


                            // �J���Ă��鑮���e�[�u���t�H�[������Ε���
                            ESRIJapan.GISLight10.Common.UtilityClass.DeleteAttributeTableObjects();

                            // �J��
                            this.OpenDocFile(ofd.FileName);
                        }
                    }
                }
                finally
                {
                    ofd.Dispose();
                    ofd = null;
                }
                //<--
#endif

            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
            }
        }

        #endregion

        /// <summary>
        /// �w��̃h�L�������g�E�t�@�C����W�J���܂� (�����p)
        /// </summary>
        /// <param name="DocFilePath"></param>
        /// <returns></returns>
        private bool OpenDocFile(string DocFilePath) {
            bool	blnRet = false;
            
            // MapControl����
            try {
				// �w����޷���ḩ̂�ق��J��
				blnRet = OpenSupportDocument(DocFilePath);
			
				if(blnRet) {
					this.mainForm.Text = System.IO.Path.GetFileName(DocFilePath) + " - " + Properties.Resources.CommonMessage_ApplicationName;
					this.mainForm.SetEvents();
					this.mainForm.SetPageLayoutEvents();
					this.mainForm.ClearMapChangeCount();
				}
            }
            catch(Exception ex) {
				ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(ex);
            }
			
			return blnRet;
        }

        /// <summary>
        /// �w��̃h�L�������g�E�t�@�C����W�J���܂�
        /// </summary>
        /// <param name="DocFilePath">MXḐ�٥�߽</param>
        /// <param name="MapControl">MapControl</param>
        /// <returns>OK / NG ��MXD�����ۂɊJ���Ď��s����ꍇ�������AOK�Ƃ��܂��B</returns>
        public bool OpenDocFile(string DocFilePath, IMapControl3 MapControl) {
			bool	blnRet = true;
			
			// ϯ�ߥ���۰ق�ݒ�
			if(MapControl != null) {
				this.SetMainFormObj(MapControl);
			}

            // ���݂̕ҏW����݂��I��������, ϯ�߂̕ύX��ۑ�
			blnRet = OpenDocFile(DocFilePath);
			
			return blnRet;
        }
    }
}
