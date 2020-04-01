using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Diagnostics;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �y�[�W���C�A�E�g������s�R�}���h
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class PrinitPageLayoutCommand : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public PrinitPageLayoutCommand()
        {

            base.m_caption = "���";
            base.m_category = "�y�[�W���C�A�E�g";
            base.m_toolTip = "���";

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
        /// ����w��_�C�A���O�\��
        /// </summary>
        public override void OnClick() {
			// ���
			Print(mainForm, true);
        }

        /// <summary>
        /// ����ݒ�^���
        /// </summary>
        /// <param name="mainForm"></param>
        /// <param name="IsPrint"></param>
        public static void Print(Ui.MainForm mainForm, bool IsPrint) {

            try {


                // ����޷���Ă̐ݒ�
                mainForm.document.DocumentName = mainForm.GetMapControlSyncronizer.MapControl.DocumentFilename;
                //set the Document property to the PrintDocument for which the PrintPage Event 
                //has been handled. To display the dialog, either this property or the 
                //PrinterSettings property must be set 

				// ����޲�۸ނ�\��
                mainForm.printDialog1 = new PrintDialog() {
					AllowSomePages = true,
					ShowHelp = false,
					/*PrinterSettings = mainForm.document.PrinterSettings,*/
					Document = mainForm.document,
				};

                if(IsPrint) {
	                // OK�{�^���ύX
					mainForm.printDialogButton = "���";
					mainForm.setPrintDialogButton = true;
                }
                else {
					// �^�C�g���ύX
					mainForm.printDialogTitle = "����ݒ�";
					mainForm.setPrintDialogTitle = true;
                }

                // ����ĥ�޲�۸ނ�\��
                DialogResult result = mainForm.printDialog1.ShowDialog();
                if(result == DialogResult.OK) {
                    Margins margins = (Margins)mainForm.document.DefaultPageSettings.Margins.Clone();

                    // �v�����^���قȂ�ꍇ�΍�
                    mainForm.document.DefaultPageSettings = mainForm.printDialog1.PrinterSettings.DefaultPageSettings;

#if DEBUG
					// �I�����ꂽ̫�ϯĂ����
					int			intCnt;
					int			intSelFormNumber = 0;	// ̫�ϯ�ID
					PaperSize	pSize;
					IEnumerator paperSizes = mainForm.printDialog1.PrinterSettings.PaperSizes.GetEnumerator();
					for(intCnt=0; intCnt < mainForm.printDialog1.PrinterSettings.PaperSizes.Count; intCnt++) {
						paperSizes.MoveNext();
						pSize = paperSizes.Current as PaperSize;

						if(pSize.Kind == mainForm.document.DefaultPageSettings.PaperSize.Kind) {
							if(pSize.PaperName.Equals(mainForm.document.DefaultPageSettings.PaperSize.PaperName)) {
								intSelFormNumber = intCnt;
								mainForm.document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
								Debug.Write("��");
								break;
							}
							else if(pSize.Width.Equals(mainForm.document.DefaultPageSettings.PaperSize.Width)
								&& pSize.Height.Equals(mainForm.document.DefaultPageSettings.PaperSize.Height)) {
								intSelFormNumber = intCnt;
								mainForm.document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
								Debug.Write("��");
							}
							else {
								Debug.Write("��");
							}
						}
						else {
							Debug.Write("��");
						}
						Debug.WriteLine(string.Format("PRINT SIZES ({0}) : KIND = {1}, NAME = {2}, W = {3} / H = {4}", intCnt, pSize.Kind, pSize.PaperName, pSize.Width, pSize.Height));
					}
#endif

                    mainForm.document.OriginAtMargins = true;
                    mainForm.document.DefaultPageSettings.Margins = margins;



                    // ������̐ݒ���y�[�W�ݒ�_�C�A���O�ɔ��f
                    mainForm.pageSetupDialog1.PrinterSettings = mainForm.document.PrinterSettings;
                    mainForm.pageSetupDialog1.PageSettings = mainForm.document.DefaultPageSettings;
                    mainForm.pageSetupDialog1.PageSettings.Margins = margins;
                    mainForm.document.OriginAtMargins = false;
                    
                    IPaper	agPaper = new PaperClass();

					agPaper.PrinterName = mainForm.document.DefaultPageSettings.PrinterSettings.PrinterName;
					agPaper.Orientation = Convert.ToInt16(mainForm.document.DefaultPageSettings.Landscape ? 2 : 1);


                    // ���޲��̊���ݒ�𔽉f
                    agPaper.Attach(mainForm.pageSetupDialog1.PrinterSettings.GetHdevmode(
                        mainForm.pageSetupDialog1.PageSettings).ToInt32(),
                        mainForm.pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

                    IPrinter	agPrinter = new EmfPrinterClass();
                    agPrinter.Paper = agPaper;

					// �߰�ސݒ���擾
					IPage	agPage = mainForm.axPageLayoutControl1.PageLayout.Page;
					
					// �߰�ނɍ��킹��ϯ�ߴ���Ă̻��ނ𽹰�ݸ�
					if(!agPage.StretchGraphicsWithPage) {
						agPage.StretchGraphicsWithPage = true;
					}

					// �p���̌�����ݒ�
					if(agPage.Orientation != agPaper.Orientation) {
						agPage.Orientation = agPaper.Orientation;
					}

					// ������̗p���ݒ���g�p
					if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
						agPage.FormID = esriPageFormID.esriPageFormSameAsPrinter;
					}
					
					// ���ޒ��� (������̗p���ݒ���g�p���Ȃ��ꍇ�A���͂ō��킹��)
					if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
						double	dblW, dblH;
						agPage.FormID = GetStretchPageSize(agPage, agPaper, out dblW, out dblH);
						if(agPage.FormID == esriPageFormID.esriPageFormCUSTOM) {
							agPage.PutCustomSize(dblW, dblH);
						}
					}

					// ���ޒP�ʂ�ݒ�
					if(agPage.Units != esriUnits.esriCentimeters) {
						agPage.Units = esriUnits.esriCentimeters;
					}

                    // ������̐ݒ���X�V
                    mainForm.axPageLayoutControl1.Printer = agPrinter;

					// ���
					if(IsPrint) {
						mainForm.isPrintPage = true;
						mainForm.document.Print();
						mainForm.isPrintPage = false;
					}

                    mainForm.ContinuPageLayout = true;

					// ����ݒ�̊T�v�\�����X�V
					mainForm.LoadPrintSetting(null);

                    // ڲ��ĕ`����X�V
                    mainForm.axPageLayoutControl1.Refresh();

                    mainForm.MainMapChanged = true;
                }
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
        
        /// <summary>
        /// ����ݒ�ɍ��킹���y�[�W�̃T�C�Y�ݒ���擾���܂�
        /// </summary>
        /// <param name="LayoutPage"></param>
        /// <param name="Paper"></param>
        /// <param name="PageWidth"></param>
        /// <param name="PageHeight"></param>
        /// <returns></returns>
        static internal esriPageFormID GetStretchPageSize(IPage LayoutPage, IPaper Paper, out double PageWidth, out double PageHeight) {
			double	dblPageW, dblPageH;
			double	dblPaperW, dblPaperH;
			
			// ���ނ̒P�ʂ𑵂��� (���޲��͒P�ʂ���� ?)
			if(Paper.Units != LayoutPage.Units) {
				LayoutPage.Units = Paper.Units;
			}
			
			// �߰�ނ̻��ނ��擾
			LayoutPage.QuerySize(out PageWidth, out PageHeight);
			// �p���̻��ނ��擾
			Paper.QueryPaperSize(out dblPaperW, out dblPaperH);
			
			bool	blnHit = false;
			IPage	agTempPage = null;
			
			// �񋓌^(�߰�ޥ̫��)��r
			esriPageFormID[]	agPFormIDs = (esriPageFormID[])Enum.GetValues(typeof(esriPageFormID));
			foreach(esriPageFormID agPFormID in agPFormIDs) {
				// �߰�މ��쐬
				agTempPage = new PageClass() {
					FormID = agPFormID,
					Orientation = Paper.Orientation,
					Units = Paper.Units,
				};
				
				// �߰�ޥ���ނ��擾
				agTempPage.QuerySize(out dblPageW, out dblPageH);
				
				// ���ޔ�r
				if(Math.Round(dblPageH, 2) == Math.Round(dblPaperH, 2) && Math.Round(dblPageW, 2) == Math.Round(dblPaperW, 2)) {
					blnHit = true;
					break;
				}
			}
			
			// �Y��������軲�ނ��Ȃ��ꍇ�Ͷ��ѐݒ�
			if(!blnHit) {
				agTempPage = new PageClass() {
					FormID = esriPageFormID.esriPageFormCUSTOM,
					Orientation = Paper.Orientation,
				};
			}
			agTempPage.QuerySize(out PageWidth, out PageHeight);

			return agTempPage.FormID;
        }

        /// <summary>
        /// �y�[�W�ݒ�ɍ��킹������T�C�Y���擾���܂��i���g�p���Ȃ��ׁA�J���r���̂܂܁j
        /// </summary>
        /// <param name="LayoutPage"></param>
        /// <param name="Paper"></param>
        /// <param name="PaperWidth"></param>
        /// <param name="PaperHeight"></param>
        /// <returns></returns>
        private static short GetStretchPaperSize(IPage LayoutPage, IPaper Paper, out double PaperWidth, out double PaperHeight) {
			double	dblPageW, dblPageH;
			double	dblPaperW, dblPaperH;
			
			// ���ނ̒P�ʂ𑵂���
			if(Paper.Units != LayoutPage.Units) {
				LayoutPage.Units = Paper.Units;
			}

			// �p���̻��ނ��擾
			Paper.QueryPaperSize(out PaperWidth, out PaperHeight);
			// �߰�ނ̻��ނ��擾
			LayoutPage.QuerySize(out dblPageW, out dblPageH);
			esriPageFormID	agPageFormID = LayoutPage.FormID;
			
			IEnumNamedID	agEnumForms = Paper.Forms;
			int				intCnt;
			string			strFormName;
			IPaper			agTempPaper;
			short			shoFormID = -1;
			
			while((intCnt = agEnumForms.Next(out strFormName)) >= 0) {
				// �p�����쐬
				agTempPaper = new PaperClass() {
					PrinterName = Paper.PrinterName,
					Orientation = LayoutPage.Orientation,
					FormID = (short)intCnt,
				};
				
				// �p�����ނ��擾
				agTempPaper.QueryPaperSize(out dblPaperW, out dblPaperH);
				
				// ���ޔ�r
				if(dblPageH == Math.Round(dblPaperH) && dblPageW == Math.Round(dblPaperW)) {
					shoFormID = agTempPaper.FormID;
					break;
				}
			}
			
			if(shoFormID < 0) {
				
			}
			
			return shoFormID;
        }
        #endregion
    }
}
