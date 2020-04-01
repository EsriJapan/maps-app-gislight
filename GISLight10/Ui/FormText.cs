using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ページレイアウトへのテキストエレメントの設定
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public partial class FormText : Form
    {
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private IEnvelope envelope;

        private ITextElement textelement = null;
        private IGraphicsContainer graphicsContainer = null;

        /// <summary>
        /// テキストエレメント追加時のコンストラクタ
        /// </summary>
        /// <param name="pageLayoutControl">ページレイアウトコントロール</param>
        /// <param name="envelope">エンベロープ</param>
        public FormText(
            IPageLayoutControl3 pageLayoutControl,
            IEnvelope envelope)

        {
            InitializeComponent();
            this.m_pageLayoutControl = pageLayoutControl;
            this.envelope = envelope;

            SetInit();
            SetFontComboBox();
        }

        /// <summary>
        /// テキストエレメント修正時のコンストラクタ
        /// </summary>
        /// <param name="pageLayoutControl">ページレイアウトコントロール</param>
        /// <param name="tel">テキストエレメント</param>
        /// <param name="graphicsContainer">グラフィックコンテナ</param>
        public FormText(
            IPageLayoutControl3 pageLayoutControl,
            ITextElement tel,
            IGraphicsContainer graphicsContainer)
        {
            InitializeComponent();

            this.Text = "テキストの編集";
            this.m_pageLayoutControl = pageLayoutControl;
            this.buttonOk.Text = "変更";
            this.textelement = tel;
            this.richTextBoxPrintTitle.Text = tel.Text;
            this.graphicsContainer = graphicsContainer;

            SetInit();

            //if (!IsExistTextCurrentSize(tel))
            //    comboBoxFontSiza.Text = tel.Symbol.Size.ToString();
            SetFontComboBox();
            GetCurrentSymbol(tel);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetInit()
        {
            double p8 = 8.0;
            double p10 = 10.0;
            double p12 = 12.0;
            double p14 = 14.0;
            double p16 = 16.0;
            double p18 = 18.0;
            double p20 = 20.0;
            double p22 = 22.0;
            double p24 = 24.0;
            double p26 = 26.0;
            double p32 = 32.0;

            comboBoxFontSiza.Items.Add(p8);
            comboBoxFontSiza.Items.Add(p10);
            comboBoxFontSiza.Items.Add(p12);
            comboBoxFontSiza.Items.Add(p14);
            comboBoxFontSiza.Items.Add(p16);
            comboBoxFontSiza.Items.Add(p18);
            comboBoxFontSiza.Items.Add(p20);
            comboBoxFontSiza.Items.Add(p22);
            comboBoxFontSiza.Items.Add(p24);
            comboBoxFontSiza.Items.Add(p26);
            comboBoxFontSiza.Items.Add(p32);

            comboBoxFontSiza.SelectedIndex = comboBoxFontSiza.Items.Count-1;
        }



        /// <summary>
        /// テキストエレメント作成
        /// </summary>
        /// <param name="pageLayout"></param>
        /// <param name="map"></param>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="envelope"></param>
        /// <param name="activeView"></param>
        /// <param name="labelString"></param>
        /// <returns></returns>
        private IMapSurround CreateMapSurroundTextElement(
            IPageLayout pageLayout,
            IMap map,
            UID uid,
            string name,
            IEnvelope envelope,
            IActiveView activeView,
            string labelString
            )
        {
            // title element name const 
            const string title_name = "TitleLabel";

            IGraphicsContainer graphicsContainer = pageLayout as IGraphicsContainer;
            graphicsContainer.Reset();

            IMapFrame mapFrame = graphicsContainer.FindFrame(map) as IMapFrame;
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uid, null);

            // 既に存在すれば削除
            //IElement targetelm = this.m_pageLayoutControl.FindElementByName(title_name, 1);
            //if (targetelm != null) graphicsContainer.DeleteElement(targetelm);

            //'Next, cocreate a new TextElement    
            IArea pArea = activeView.Extent as IArea;
            IPoint pPoint = pArea.Centroid;// LabelPoint;

            ITextElement pTextElement = new TextElementClass();
            IElementProperties3 pElemProp = pTextElement as IElementProperties3;
            pElemProp.Name = title_name;

            // 2010-12-21 add
            SetUpSymbol(pTextElement);

            IElement pElement = pTextElement as IElement;
            pElement.Geometry = pPoint;

            // 2010-12-21 del
            //ITextSymbol pTextSymbol = new TextSymbol();
            //SetUpSymbol(pTextSymbol);
            //pTextElement.Symbol = pTextSymbol;
            // 2010-12-21 del end

            if (labelString != null)
            {
                pTextElement.Text = labelString;
            }
            else
            {
                pTextElement.Text = "<<Default Tile >>";
            }
            
            IElement element = mapSurroundFrame as IElement;
            element = pElement;
            element.Activate(activeView.ScreenDisplay);

            ITrackCancel trackCancel = new CancelTrackerClass();
            element.Draw(activeView.ScreenDisplay, trackCancel);

            graphicsContainer.AddElement(element, 0);
            element.Geometry = this.envelope; //envelope;
            
            return mapSurroundFrame.MapSurround;
        }

        /// <summary>
        /// フォント名称取得
        /// </summary>
        /// <param name="fontname"></param>
        private void GetCurrentFontName(string fontname)
        {
            for (int i = 0; i < comboBoxFonts.Items.Count; i++)
            {
                if (comboBoxFonts.Items[i].ToString().Equals(fontname))
                {
                    comboBoxFonts.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// フォントサイズ取得
        /// </summary>
        /// <param name="fontsize"></param>
        private void GetCurrentFontSize(double fontsize)
        {
            for (int i = 0; i < comboBoxFontSiza.Items.Count; i++)
            {
                if (Convert.ToDouble(comboBoxFontSiza.Items[i]) == fontsize)
                {
                    comboBoxFontSiza.SelectedIndex = i;
                    return;
                }
            }
            comboBoxFontSiza.Items.Add(fontsize.ToString());
            comboBoxFontSiza.SelectedIndex = comboBoxFontSiza.Items.Count-1;
        }

        /// <summary>
        /// テキストシンボルプロパティ設定
        /// </summary>
        /// <param name="pTextElement"></param>
        private void SetUpSymbol(ITextElement pTextElement)
        {
            ISymbolCollectionElement symbolCol =
                pTextElement as ISymbolCollectionElement;

            if (symbolCol != null)
            {
                IRgbColor rgb =
                    Common.UtilityClass.ConvertToESRIColor(buttonSetColor.BackColor);

                symbolCol.Color = rgb;

                symbolCol.FontName =
                    comboBoxFonts.Items[comboBoxFonts.SelectedIndex].ToString();

                symbolCol.Size =
                    Convert.ToDouble(this.comboBoxFontSiza.Text);

                symbolCol.Underline = false;

                symbolCol.CharacterSpacing =
                    Convert.ToDouble(this.numericUpDownCharacterSpacing.Value);
            }
        }

        /// <summary>
        /// 既存のプロパティ取得
        /// </summary>
        /// <param name="pTextElement"></param>
        private void GetCurrentSymbol(ITextElement pTextElement)
        {
            ISymbolCollectionElement symbolCol =
                pTextElement as ISymbolCollectionElement;

            if (symbolCol != null)
            {
                Common.UtilityClass.SetButtonBackColor(
                    symbolCol.Color as IRgbColor, this.buttonSetColor);

                GetCurrentFontName(symbolCol.FontName);

                GetCurrentFontSize(symbolCol.Size);

                this.numericUpDownCharacterSpacing.Value =
                    Convert.ToDecimal(symbolCol.CharacterSpacing);
            }

        }

        // 選択可能対象フォント
        private string[] TargetFontNameHeaders = { "ＭＳ", "MS", "HG", "メイリオ" };

        /// <summary>
        /// 選択可能対象フォント判定
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        private bool IsTargetFont(string fontName)
        {
            for (int i = 0; i < TargetFontNameHeaders.Length; i++)
            {
                if (fontName.ToLower().IndexOf(
                        TargetFontNameHeaders[i].ToLower()) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// フォントコンボボックス設定
        /// </summary>
        private void SetFontComboBox()
        {
            Graphics g = CreateGraphics();
            //System.Drawing.FontFamily[] ffs = System.Drawing.FontFamily.GetFamilies(g);
            System.Drawing.FontFamily[] ffs = System.Drawing.FontFamily.Families;

            int defalutIndex = 0;
            foreach (System.Drawing.FontFamily ff in ffs)
            {
                if (IsTargetFont(ff.Name))
                {
                    comboBoxFonts.Items.Add(ff.Name);
                }
            }
            g.Dispose();
            comboBoxFonts.SelectedIndex = defalutIndex;
        }

        /// <summary>
        /// 追加(OK)または更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            bool dispMesg = false;
            try
            {
                if (!Common.UtilityClass.IsNumeric(this.comboBoxFontSiza.Text))
                {
                    return;
                }

                if (this.buttonOk.Text.Equals("OK"))
                {
                    UID uid = new UIDClass();
                    uid.Value = "esriCarto.TextElement";

                    IMapSurround mapSurround =
                        CreateMapSurroundTextElement(
                            this.m_pageLayoutControl.PageLayout,
                            this.m_pageLayoutControl.ActiveView.FocusMap, uid,
                            "TextElement", envelope, this.m_pageLayoutControl.ActiveView,
                            this.richTextBoxPrintTitle.Text);

                    this.m_pageLayoutControl.ActiveView.PartialRefresh(
                        esriViewDrawPhase.esriViewGraphics, null, null);
                }
                else
                {

                    this.textelement.Text = this.richTextBoxPrintTitle.Text;
                    
                    // 2010-12-21 modify
                    SetUpSymbol(this.textelement);

                    this.graphicsContainer.UpdateElement(this.textelement as IElement);
                    this.m_pageLayoutControl.ActiveView.PartialRefresh(
                        esriViewDrawPhase.esriViewGraphics, null, null);

                }
                this.Close();
            }
            catch (COMException comex)
            {
                dispMesg = true;
                Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMesg = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (dispMesg)
                {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.FormText_ErrorWhenSetTextElement);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormText_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Dispose(); // 呼び元でDisposeする
        }

        private void FormText_Activated(object sender, EventArgs e)
        {
            richTextBoxPrintTitle.Focus();
        }

        /// <summary>
        /// 数値以外はエラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (!Common.UtilityClass.IsNumeric(comboBoxFontSiza.Text))
            {
                comboBoxFontSiza.ForeColor = Color.Red;
                return;
            }
            comboBoxFontSiza.ForeColor = Color.Black;
        }

        /// <summary>
        /// 色設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetColor_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                buttonSetColor.BackColor = 
                    Common.UtilityClass.GetColor(buttonSetColor.BackColor);
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (dispMessage)
                {
                    Common.MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.FormSymbolSettings_ErrorWhen_buttonSetColor_Click);
                }
            }
        }
    }
}
