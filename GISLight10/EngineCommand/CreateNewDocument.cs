using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �}�b�v�h�L�������g�̐V�K�쐬
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public class CreateNewDocument : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private IMapControl3 m_mapControl;

        // 2012/08/22 ADD
        private IPageLayoutControl3 m_pageLayoutControl = null;

        private Ui.MainForm mainForm;
        private ESRIJapan.GISLight10.Common.ControlsSynchronizer m_controlsSynchronizer = null;

        /// <summary>
        /// �}�b�v�R���g���[�����烁�C���t�H�[���ւ̎Q�Ƃ��擾
        /// </summary>
        /// <param name="mapControl">�}�b�v�R���g���[��</param>
        private void SetMainFormObj(IMapControl3 mapControl)
        {
            if (mapControl == null) return;

            m_mapControl = mapControl;
            IntPtr ptr2 = (System.IntPtr)mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();

            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;

        }
        // 2012/08/22 ADD

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public CreateNewDocument()
        {
            //update the base properties
            // 2012/08/01 UPD
            base.m_caption = "�V�K�쐬";
            base.m_toolTip = "�V�K�쐬";

            try
            {
                string bitmapResourceName = this.GetType().Name + ".bmp";
                base.m_bitmap =
                    new Bitmap(this.GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
            // 2012/08/01 UPD
        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// �N���b�N������
        /// �}�b�v�h�L�������g�̐V�K�쐬
        /// </summary>
        public override void OnClick()
        {
			if(this.m_hookHelper == null) {
				return;
			}
			if(this.m_mapControl == null) {
				if(m_hookHelper.Hook is IMapControl3) {
					m_mapControl = (IMapControl3)m_hookHelper.Hook;
				}
				else if(m_hookHelper.Hook is IToolbarControl2) {
					IToolbarControl2	agToolCtl = m_hookHelper.Hook as IToolbarControl2;
					this.m_mapControl = agToolCtl.Buddy as IMapControl3;
				}
				else {
					return;
				}
				SetMainFormObj(m_mapControl);
			}

            try {
                IMapControl3 mapControl = null;

                //get the MapControl from the hook in case the container is a ToolbarControl
                if (m_hookHelper.Hook is IToolbarControl)
                {
                    // 2012/08/01 UPD
                    try
                    {
                        mapControl = (IMapControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;
                        // 2012/08/01 ADD
                        IntPtr ptr2 = (System.IntPtr)mapControl.hWnd;
                        System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                        mainForm = (Ui.MainForm)cntrl2.FindForm();
                        // 2012/08/01 ADD
                    }
                    catch
                    {
                        mapControl = m_mapControl;
                        if (mapControl == null)
                        {
                            m_pageLayoutControl =
                                (IPageLayoutControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;

                            IntPtr ptr2 = (System.IntPtr)m_pageLayoutControl.hWnd;
                            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                            mainForm = (Ui.MainForm)cntrl2.FindForm();
                            mapControl = mainForm.MapControl;

                            m_controlsSynchronizer = mainForm.GetMapControlSyncronizer;
                        }
                    }
                                   }
                //In case the container is MapControl
                else if (m_hookHelper.Hook is IMapControl3)
                {
                    mapControl = (IMapControl3)m_hookHelper.Hook;
                }
                else
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(
                        mainForm, "�A�N�e�B�u�R���g���[���̓}�b�v�R���g���[���ł���K�v������܂�");
                    return;
                }

                DialogResult result;
                IEngineEditor engineEditor = new EngineEditorClass();

                if ((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) &&
                    (engineEditor.HasEdits() == true))
                {
                    result = MessageBox.Show(
                        "�ҏW���̏�Ԃ�ۑ����܂����H",
                        Properties.Resources.CommonMessage_ApplicationName,
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    switch (result)
                    {
                        case DialogResult.Cancel:
                            return;

                        case DialogResult.No:
                            engineEditor.StopEditing(false);
                            break;

                        case DialogResult.Yes:
                            engineEditor.StopEditing(true);
                            break;
                    }
                }

				// �P��ð��ْǉ���Ԃ��擾
				bool	blnAddTable = mapControl.Map != null ? (mapControl.Map as IStandaloneTableCollection).StandaloneTableCount > 0 : false;
                
                // �ύX�𔻒�
                if ((mapControl.LayerCount > 0 || blnAddTable) && mainForm.MainMapChanged)
                {
                    DialogResult res = MessageBox.Show(
                        "���݂̃h�L�������g��ۑ����܂���?",
                        Properties.Resources.CommonMessage_ApplicationName,
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    switch (res)
                    {
                        case DialogResult.Cancel:
                            return;

                        case DialogResult.No:
                            break;

                        case DialogResult.Yes:
                            //launch the save command
                            ICommand command = new ControlsSaveAsDocCommandClass();
                            command.OnCreate(m_hookHelper.Hook);
                            command.OnClick();
                            break;
                    }
                }

                // �J���Ă��鑮���e�[�u���t�H�[������Ε���
                ESRIJapan.GISLight10.Common.UtilityClass.DeleteAttributeTableObjects();

                // 2012/08/24 add >>>>>
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

                this.mainForm.ClearPageLayout();

                // ���O�ɊJ���Ă����޷���Ă�������ݒ��Ԃɂ���ẮA�����������ݒ�ɖ߂�Ȃ��s��ɑΉ�
                this.mainForm.InitPrintSettings();

                //create a new Map
                IMap map = new MapClass();
                map.Name = "Map";

                //assign the new map to the MapControl
                mapControl.DocumentFilename = string.Empty;
                mapControl.Map = map;

                this.mainForm.GetMapControlSyncronizer.MapControl.DocumentFilename = string.Empty;

                //replace the shared map with the new Map instance
                this.mainForm.originalMaps.Reset();
                this.mainForm.originalMaps.Add(map);

                this.mainForm.GetMapControlSyncronizer.PageLayoutControl.PageLayout.ReplaceMaps(this.mainForm.originalMaps);
                this.mainForm.GetMapControlSyncronizer.ReplaceMap(map);
                
                this.mainForm.SetEvents();
                this.mainForm.SetPageLayoutEvents();
                this.mainForm.ClearMapChangeCount();
                this.mainForm.Text = Properties.Resources.CommonMessage_ApplicationName;
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
    }
}
