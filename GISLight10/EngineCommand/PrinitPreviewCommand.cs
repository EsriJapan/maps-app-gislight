using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Printing;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Output;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �y�[�W���C�A�E�g����v���r���[���s�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class PrinitPreviewCommand : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public PrinitPreviewCommand()
        {

            base.m_caption = "�v���r���[";
            base.m_category = "�y�[�W���C�A�E�g";
            base.m_toolTip = "����v���r���[";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�c�[���o�[�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            IToolbarControl2 tlb = (IToolbarControl2)m_hookHelper.Hook;
            IntPtr ptr = (System.IntPtr)tlb.hWnd;
            System.Windows.Forms.Control cntrl = System.Windows.Forms.Control.FromHandle(ptr);
            mainForm = (Ui.MainForm)cntrl.FindForm();
        }

        /// <summary>
        /// �N���b�N������
        /// ����v���r���[�_�C�A���O�\��
        /// </summary>
        public override void OnClick()
        {
            try
            {
                if (System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count == 0)
                {
                    return;
                }

                //if (!mainForm.ContinuPageLayout) return;

                // create a new PrintPreviewDialog using constructor
                mainForm.printPreviewDialog1 = new PrintPreviewDialog();
                mainForm.printPreviewDialog1.ShowIcon = false;

                // ����{�^���������ւ���
                ToolStrip toolStrip = (ToolStrip)mainForm.printPreviewDialog1.Controls[1];
                ToolStripButton toolStripButton = new ToolStripButton();
                ToolStripSeparator toolStripSeparator = new ToolStripSeparator();

                toolStripButton.Image = (Image)toolStrip.Items[0].Image.Clone();
                toolStripButton.ToolTipText = toolStrip.Items[0].ToolTipText;
                toolStripButton.Click += new EventHandler(printerselectAndPrint);

                toolStrip.Items.Remove(toolStrip.Items[0]);
                for (int i = 0; i < 6; i++)
                {
                    toolStrip.Items.Remove(toolStrip.Items[2]);
                }

                toolStrip.Items.Insert(0, toolStripButton);
                toolStrip.Items.Insert(1, toolStripSeparator);

                for (int i = 0; i < toolStrip.Items.Count; i++)
                {
                    ESRIJapan.GISLight10.Common.Logger.Debug(toolStrip.Items[i].Name);
                }

                //initialize the currently printed page number
                //mainForm.m_CurrentPrintPage = 0;

                //check if a document is loaded into PageLayout	control
                //if (axPageLayoutControl1.DocumentFilename == null) return;
                //if (m_pageLayoutControl.DocumentFilename == null) return;
                if (mainForm.GetMapControlSyncronizer.MapControl.DocumentFilename == null)
                {
                    //dispMessage = true;
                    return;
                }

                //set the name of the print preview document to the name of the mxd doc
                mainForm.document.DocumentName = mainForm.GetMapControlSyncronizer.MapControl.DocumentFilename;

                //set the PrintPreviewDialog.Document property to the PrintDocument object selected by the user
                mainForm.printPreviewDialog1.Document = mainForm.document;

                mainForm.isPreviewPage = true;

                //show the dialog - this triggers the document's PrintPage event
                mainForm.printPreviewDialog1.ShowDialog(mainForm);

                mainForm.isPreviewPage = false;

                mainForm.axPageLayoutControl1.Refresh();

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
        /// ���
        /// </summary>
        private void printerselectAndPrint(object sender, EventArgs e)
        {
            mainForm.isPrintPage = true;

            // ���
            mainForm.document.Print();

            mainForm.isPrintPage = false;
        }
    }
}
