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
    /// ページレイアウト印刷実行コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class PrinitPageLayoutCommand : BaseCommand
    {
        private IHookHelper m_hookHelper = null;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PrinitPageLayoutCommand()
        {

            base.m_caption = "印刷";
            base.m_category = "ページレイアウト";
            base.m_toolTip = "印刷";

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
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">ツールバーコントロール</param>
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
        /// クリック時処理
        /// 印刷指定ダイアログ表示
        /// </summary>
        public override void OnClick() {
			// 印刷
			Print(mainForm, true);
        }

        /// <summary>
        /// 印刷設定／印刷
        /// </summary>
        /// <param name="mainForm"></param>
        /// <param name="IsPrint"></param>
        public static void Print(Ui.MainForm mainForm, bool IsPrint) {

            try {


                // 印刷ﾄﾞｷｭﾒﾝﾄの設定
                mainForm.document.DocumentName = mainForm.GetMapControlSyncronizer.MapControl.DocumentFilename;
                //set the Document property to the PrintDocument for which the PrintPage Event 
                //has been handled. To display the dialog, either this property or the 
                //PrinterSettings property must be set 

				// 印刷ﾀﾞｲｱﾛｸﾞを表示
                mainForm.printDialog1 = new PrintDialog() {
					AllowSomePages = true,
					ShowHelp = false,
					/*PrinterSettings = mainForm.document.PrinterSettings,*/
					Document = mainForm.document,
				};

                if(IsPrint) {
	                // OKボタン変更
					mainForm.printDialogButton = "印刷";
					mainForm.setPrintDialogButton = true;
                }
                else {
					// タイトル変更
					mainForm.printDialogTitle = "印刷設定";
					mainForm.setPrintDialogTitle = true;
                }

                // ﾌﾟﾘﾝﾄ･ﾀﾞｲｱﾛｸﾞを表示
                DialogResult result = mainForm.printDialog1.ShowDialog();
                if(result == DialogResult.OK) {
                    Margins margins = (Margins)mainForm.document.DefaultPageSettings.Margins.Clone();

                    // プリンタが異なる場合対策
                    mainForm.document.DefaultPageSettings = mainForm.printDialog1.PrinterSettings.DefaultPageSettings;

#if DEBUG
					// 選択されたﾌｫｰﾏｯﾄを特定
					int			intCnt;
					int			intSelFormNumber = 0;	// ﾌｫｰﾏｯﾄID
					PaperSize	pSize;
					IEnumerator paperSizes = mainForm.printDialog1.PrinterSettings.PaperSizes.GetEnumerator();
					for(intCnt=0; intCnt < mainForm.printDialog1.PrinterSettings.PaperSizes.Count; intCnt++) {
						paperSizes.MoveNext();
						pSize = paperSizes.Current as PaperSize;

						if(pSize.Kind == mainForm.document.DefaultPageSettings.PaperSize.Kind) {
							if(pSize.PaperName.Equals(mainForm.document.DefaultPageSettings.PaperSize.PaperName)) {
								intSelFormNumber = intCnt;
								mainForm.document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
								Debug.Write("◎");
								break;
							}
							else if(pSize.Width.Equals(mainForm.document.DefaultPageSettings.PaperSize.Width)
								&& pSize.Height.Equals(mainForm.document.DefaultPageSettings.PaperSize.Height)) {
								intSelFormNumber = intCnt;
								mainForm.document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
								Debug.Write("○");
							}
							else {
								Debug.Write("●");
							}
						}
						else {
							Debug.Write("●");
						}
						Debug.WriteLine(string.Format("PRINT SIZES ({0}) : KIND = {1}, NAME = {2}, W = {3} / H = {4}", intCnt, pSize.Kind, pSize.PaperName, pSize.Width, pSize.Height));
					}
#endif

                    mainForm.document.OriginAtMargins = true;
                    mainForm.document.DefaultPageSettings.Margins = margins;



                    // 印刷時の設定をページ設定ダイアログに反映
                    mainForm.pageSetupDialog1.PrinterSettings = mainForm.document.PrinterSettings;
                    mainForm.pageSetupDialog1.PageSettings = mainForm.document.DefaultPageSettings;
                    mainForm.pageSetupDialog1.PageSettings.Margins = margins;
                    mainForm.document.OriginAtMargins = false;
                    
                    IPaper	agPaper = new PaperClass();

					agPaper.PrinterName = mainForm.document.DefaultPageSettings.PrinterSettings.PrinterName;
					agPaper.Orientation = Convert.ToInt16(mainForm.document.DefaultPageSettings.Landscape ? 2 : 1);


                    // ﾃﾞﾊﾞｲｽの既定設定を反映
                    agPaper.Attach(mainForm.pageSetupDialog1.PrinterSettings.GetHdevmode(
                        mainForm.pageSetupDialog1.PageSettings).ToInt32(),
                        mainForm.pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

                    IPrinter	agPrinter = new EmfPrinterClass();
                    agPrinter.Paper = agPaper;

					// ﾍﾟｰｼﾞ設定を取得
					IPage	agPage = mainForm.axPageLayoutControl1.PageLayout.Page;
					
					// ﾍﾟｰｼﾞに合わせてﾏｯﾌﾟｴﾚﾒﾝﾄのｻｲｽﾞをｽｹｰﾘﾝｸﾞ
					if(!agPage.StretchGraphicsWithPage) {
						agPage.StretchGraphicsWithPage = true;
					}

					// 用紙の向きを設定
					if(agPage.Orientation != agPaper.Orientation) {
						agPage.Orientation = agPaper.Orientation;
					}

					// ﾌﾟﾘﾝﾀの用紙設定を使用
					if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
						agPage.FormID = esriPageFormID.esriPageFormSameAsPrinter;
					}
					
					// ｻｲｽﾞ調整 (ﾌﾟﾘﾝﾀの用紙設定を使用しない場合、自力で合わせる)
					if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
						double	dblW, dblH;
						agPage.FormID = GetStretchPageSize(agPage, agPaper, out dblW, out dblH);
						if(agPage.FormID == esriPageFormID.esriPageFormCUSTOM) {
							agPage.PutCustomSize(dblW, dblH);
						}
					}

					// ｻｲｽﾞ単位を設定
					if(agPage.Units != esriUnits.esriCentimeters) {
						agPage.Units = esriUnits.esriCentimeters;
					}

                    // ﾌﾟﾘﾝﾀの設定を更新
                    mainForm.axPageLayoutControl1.Printer = agPrinter;

					// 印刷
					if(IsPrint) {
						mainForm.isPrintPage = true;
						mainForm.document.Print();
						mainForm.isPrintPage = false;
					}

                    mainForm.ContinuPageLayout = true;

					// 印刷設定の概要表示を更新
					mainForm.LoadPrintSetting(null);

                    // ﾚｲｱｳﾄ描画を更新
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
        /// 印刷設定に合わせたページのサイズ設定を取得します
        /// </summary>
        /// <param name="LayoutPage"></param>
        /// <param name="Paper"></param>
        /// <param name="PageWidth"></param>
        /// <param name="PageHeight"></param>
        /// <returns></returns>
        static internal esriPageFormID GetStretchPageSize(IPage LayoutPage, IPaper Paper, out double PageWidth, out double PageHeight) {
			double	dblPageW, dblPageH;
			double	dblPaperW, dblPaperH;
			
			// ｻｲｽﾞの単位を揃える (ﾃﾞﾊﾞｲｽは単位がｲﾝﾁ ?)
			if(Paper.Units != LayoutPage.Units) {
				LayoutPage.Units = Paper.Units;
			}
			
			// ﾍﾟｰｼﾞのｻｲｽﾞを取得
			LayoutPage.QuerySize(out PageWidth, out PageHeight);
			// 用紙のｻｲｽﾞを取得
			Paper.QueryPaperSize(out dblPaperW, out dblPaperH);
			
			bool	blnHit = false;
			IPage	agTempPage = null;
			
			// 列挙型(ﾍﾟｰｼﾞ･ﾌｫｰﾑ)比較
			esriPageFormID[]	agPFormIDs = (esriPageFormID[])Enum.GetValues(typeof(esriPageFormID));
			foreach(esriPageFormID agPFormID in agPFormIDs) {
				// ﾍﾟｰｼﾞ仮作成
				agTempPage = new PageClass() {
					FormID = agPFormID,
					Orientation = Paper.Orientation,
					Units = Paper.Units,
				};
				
				// ﾍﾟｰｼﾞ･ｻｲｽﾞを取得
				agTempPage.QuerySize(out dblPageW, out dblPageH);
				
				// ｻｲｽﾞ比較
				if(Math.Round(dblPageH, 2) == Math.Round(dblPaperH, 2) && Math.Round(dblPageW, 2) == Math.Round(dblPaperW, 2)) {
					blnHit = true;
					break;
				}
			}
			
			// 該当する既定ｻｲｽﾞがない場合はｶｽﾀﾑ設定
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
        /// ページ設定に合わせた印刷サイズを取得します（※使用しない為、開発途中のまま）
        /// </summary>
        /// <param name="LayoutPage"></param>
        /// <param name="Paper"></param>
        /// <param name="PaperWidth"></param>
        /// <param name="PaperHeight"></param>
        /// <returns></returns>
        private static short GetStretchPaperSize(IPage LayoutPage, IPaper Paper, out double PaperWidth, out double PaperHeight) {
			double	dblPageW, dblPageH;
			double	dblPaperW, dblPaperH;
			
			// ｻｲｽﾞの単位を揃える
			if(Paper.Units != LayoutPage.Units) {
				LayoutPage.Units = Paper.Units;
			}

			// 用紙のｻｲｽﾞを取得
			Paper.QueryPaperSize(out PaperWidth, out PaperHeight);
			// ﾍﾟｰｼﾞのｻｲｽﾞを取得
			LayoutPage.QuerySize(out dblPageW, out dblPageH);
			esriPageFormID	agPageFormID = LayoutPage.FormID;
			
			IEnumNamedID	agEnumForms = Paper.Forms;
			int				intCnt;
			string			strFormName;
			IPaper			agTempPaper;
			short			shoFormID = -1;
			
			while((intCnt = agEnumForms.Next(out strFormName)) >= 0) {
				// 用紙を作成
				agTempPaper = new PaperClass() {
					PrinterName = Paper.PrinterName,
					Orientation = LayoutPage.Orientation,
					FormID = (short)intCnt,
				};
				
				// 用紙ｻｲｽﾞを取得
				agTempPaper.QueryPaperSize(out dblPaperW, out dblPaperH);
				
				// ｻｲｽﾞ比較
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
