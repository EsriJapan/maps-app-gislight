using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ページレイアウトへの縮尺記号の設定
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormScaleBarSettings : Form
    {

        private IMapFrame mapFrame;
        private IPageLayoutControl3 m_pageLayoutControl = null;
        private Ui.MainForm mainFrm;
        private IElement target_element;

        static string ResizeHintTitle1 = "幅を固定して調整";
        static string ResizeHintTitle2 = "目盛幅を自動調整";
        static string ResizeHintTitle3 = "目盛数を自動調整";
        static string[] ResizeHintTitle = { ResizeHintTitle1, ResizeHintTitle2, ResizeHintTitle3 };
        //enum ResizeHint : byte { ResizeHint1=0, ResizeHint2, ResizeHint3 };
        static ESRI.ArcGIS.Carto.esriScaleBarResizeHint[] RsizeHints ={
            ESRI.ArcGIS.Carto.esriScaleBarResizeHint.esriScaleBarFixed,
            ESRI.ArcGIS.Carto.esriScaleBarResizeHint.esriScaleBarAutoDivision,
            ESRI.ArcGIS.Carto.esriScaleBarResizeHint.esriScaleBarAutoDivisions};

        static string UnitName1 = "センチメートル";
        static string UnitName2 = "メートル";
        static string UnitName3 = "キロメートル";
        static string UnitName4 = "インチ";
        static string UnitName5 = "度(10進)";
        static string[] UnitTitles = { UnitName1, UnitName2, UnitName3, UnitName4, UnitName5 };
        static ESRI.ArcGIS.esriSystem.esriUnits[] UnitObjects = 
        { ESRI.ArcGIS.esriSystem.esriUnits.esriCentimeters, 
          ESRI.ArcGIS.esriSystem.esriUnits.esriMeters, 
          ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers, 
          ESRI.ArcGIS.esriSystem.esriUnits.esriInches, 
          ESRI.ArcGIS.esriSystem.esriUnits.esriDecimalDegrees};


        static string UnitLabelPositionTitle2 = "数字の左";
        static string UnitLabelPositionTitle3 = "数字の右";
        static string UnitLabelPositionTitle1 = "バーの上";
        static string UnitLabelPositionTitle6 = "バーの下";
        static string UnitLabelPositionTitle4 = "バーの左";
        static string UnitLabelPositionTitle5 = "バーの右";
        static string[] UnitLabelPositionTitles = { 
            UnitLabelPositionTitle2, 
            UnitLabelPositionTitle3, 
            UnitLabelPositionTitle1,
            UnitLabelPositionTitle6,
            UnitLabelPositionTitle4, 
            UnitLabelPositionTitle5 
            };

        static ESRI.ArcGIS.Carto.esriScaleBarPos[] UnitLabelPositions = {
            ESRI.ArcGIS.Carto.esriScaleBarPos.esriScaleBarBeforeLabels,
            ESRI.ArcGIS.Carto.esriScaleBarPos.esriScaleBarAfterBar,
            ESRI.ArcGIS.Carto.esriScaleBarPos.esriScaleBarAbove,
            ESRI.ArcGIS.Carto.esriScaleBarPos.esriScaleBarBelow,
            ESRI.ArcGIS.Carto.esriScaleBarPos.esriScaleBarBeforeBar,
            ESRI.ArcGIS.Carto.esriScaleBarPos.esriScaleBarAfterLabels
            };

        //private bool CloseThis =false;

        private bool changeDivision = false;
        private bool changeDivisions = false;
        private bool changeSubdivisions = false;
        private bool changeDivisionsBeforeZero = false;
        private bool changeResizeHint = false;
        private bool changeUnits = false;
        private bool changeUnitLabelPosition = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapFrame">ページレイアウトコントロールのマップフレーム</param>
        /// <param name="pageLayoutControl">ページレイアウトコントロール</param>
        /// <param name="targetelm">ページレイアウトコントロール上で選択状態のエレメント</param>
        public FormScaleBarSettings(
            IMapFrame mapFrame,
            IPageLayoutControl3 pageLayoutControl,
            IElement targetelm)
        {
            InitializeComponent();

            this.mapFrame = mapFrame;
            this.m_pageLayoutControl = pageLayoutControl;
            this.target_element = targetelm;

            IntPtr ptr2 = (System.IntPtr)this.m_pageLayoutControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();

            SetInit();
            this.Cursor = Cursors.Default;
            this.Text = mainFrm.SCALE_BAR_ELEMENT_NAME + "プロパティ";
            CheckCurrent();
        }

        private void CheckCurrent()
        {
            IMapSurroundFrame mapSurroundFrame = this.target_element as IMapSurroundFrame;
            IMapSurround mapSurround = mapSurroundFrame.MapSurround;
            mapSurroundFrame.MapFrame = mapFrame;

            IScaleBar scaleBar = mapSurround as IScaleBar;
            Common.Logger.Debug(scaleBar.Units.ToString());
            Common.Logger.Debug(scaleBar.UnitLabel);
            Common.Logger.Debug(scaleBar.UnitLabelPosition.ToString());
            Common.Logger.Debug(scaleBar.Division.ToString());
            Common.Logger.Debug(scaleBar.Subdivisions.ToString());
            Common.Logger.Debug(scaleBar.DivisionsBeforeZero.ToString());
            Common.Logger.Debug(scaleBar.ResizeHint.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetInit()
        {
            IMapSurroundFrame mapSurroundFrame = this.target_element as IMapSurroundFrame;
            IMapSurround mapSurround = mapSurroundFrame.MapSurround;
            mapSurroundFrame.MapFrame = mapFrame;
            IScaleBar scaleBar = mapSurround as IScaleBar;

            TextBoxDivision.Text = scaleBar.Division.ToString();

            numericUpDownSubDiv.Value = scaleBar.Subdivisions;
            numericUpDownDiv.Value = Convert.ToDecimal(scaleBar.Divisions);

            if (scaleBar.DivisionsBeforeZero == 1)
            {
                checkBoxDivisionsBeforeZero.Checked = true;
            }
            else
            {
                checkBoxDivisionsBeforeZero.Checked = false;
            }

            for (int i = 0; i < ResizeHintTitle.Length; i++)
            {
                this.comboBoxResizeHint.Items.Add(ResizeHintTitle[i]);
            }
            //this.comboBoxResizeHint.SelectedIndex = 0;

            for (int i = 0; i < RsizeHints.Length; i++)
            {
                //if (RsizeHints[i] == scaleBar.ResizeHint)
                if (scaleBar.ResizeHint == RsizeHints[i])
                {
                    this.comboBoxResizeHint.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < UnitTitles.Length; i++)
            {
                this.comboBoxUnits.Items.Add(UnitTitles[i]);
            }
            this.comboBoxUnits.SelectedIndex = 0;

            for (int i = 0; i < UnitObjects.Length; i++)
            {
                if (UnitObjects[i] == scaleBar.Units)
                {
                    this.comboBoxUnits.SelectedIndex = i;
                    break;
                }
            }
            
            for (int i = 0; i < UnitLabelPositionTitles.Length; i++)
            {
                this.comboBoxUnitLabelPosition.Items.Add(UnitLabelPositionTitles[i]);
            }

            this.comboBoxUnitLabelPosition.SelectedIndex = 0;

            for (int i = 0; i < UnitLabelPositionTitles.Length; i++)
            {
                if (UnitLabelPositions[i] == scaleBar.UnitLabelPosition)
                {
                    this.comboBoxUnitLabelPosition.SelectedIndex = i;
                    break;
                }
            }

            // ResizeHintによるUI設定
            if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarFixed)
            {
                TextBoxDivision.Enabled = true;
                numericUpDownDiv.Enabled = true;
                numericUpDownSubDiv.Enabled = true;
            }
            else if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarAutoDivision)
            {
                TextBoxDivision.Enabled = false;
                numericUpDownDiv.Enabled = true;
                numericUpDownSubDiv.Enabled = true;
            }
            else if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarAutoDivisions)
            {
                TextBoxDivision.Enabled = true;
                numericUpDownDiv.Enabled = false;
                numericUpDownSubDiv.Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetScaleBar()
        {
            IMapSurroundFrame mapSurroundFrame =
                this.target_element as IMapSurroundFrame;

            IMapSurround mapSurround = mapSurroundFrame.MapSurround;
            mapSurroundFrame.MapFrame = mapFrame;

            IScaleBar scaleBar = mapSurround as IScaleBar;

            //scaleBar.UseMapSettings();

            scaleBar.ResizeHint = RsizeHints[comboBoxResizeHint.SelectedIndex];

            if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarFixed)
            {
                scaleBar.Division = double.Parse(TextBoxDivision.Text);
                scaleBar.Divisions = Convert.ToInt16(numericUpDownDiv.Value);
            }
            else if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarAutoDivision)
            {
                scaleBar.Divisions = Convert.ToInt16(numericUpDownDiv.Value);
            }
            else
            {
                scaleBar.Division = double.Parse(TextBoxDivision.Text);
            }

            if (checkBoxDivisionsBeforeZero.Checked)
            {
                scaleBar.DivisionsBeforeZero = 1;
            }
            else
            {
                scaleBar.DivisionsBeforeZero = 0;
            }

            scaleBar.Subdivisions = Convert.ToInt16(numericUpDownSubDiv.Value);

            // 20101202
            //scaleBar.UseMapSettings();

            scaleBar.Units = UnitObjects[comboBoxUnits.SelectedIndex];
            scaleBar.UnitLabel = UnitTitles[this.comboBoxUnits.SelectedIndex];
            scaleBar.UnitLabelPosition =
                UnitLabelPositions[comboBoxUnitLabelPosition.SelectedIndex];

            IClone scaleBarClone = (IClone)mapSurroundFrame;
            IMapSurroundFrame newMapSurroundFrame = (IMapSurroundFrame)scaleBarClone.Clone();
            
            //IObjectCopy scaleBarClone = new ObjectCopyClass();
            //IMapSurroundFrame newMapSurroundFrame = (IMapSurroundFrame)scaleBarClone.Copy(mapSurroundFrame);

            m_pageLayoutControl.GraphicsContainer.DeleteElement(
                (IElement)mapSurroundFrame);

            m_pageLayoutControl.GraphicsContainer.UpdateElement(
                (IElement)mapSurroundFrame);

            m_pageLayoutControl.GraphicsContainer.AddElement(
                (IElement)newMapSurroundFrame, 0);

            this.target_element = (IElement)newMapSurroundFrame;

            IMapSurround newMapSurround = newMapSurroundFrame.MapSurround;
            newMapSurroundFrame.MapFrame = mapFrame;

            IScaleBar newScaleBar = (IScaleBar)newMapSurround;

            m_pageLayoutControl.ActiveView.PartialRefresh(
                esriViewDrawPhase.esriViewGraphics, null, null);

            // UIに反映
            TextBoxDivision.Text = newScaleBar.Division.ToString();
            numericUpDownDiv.Value = newScaleBar.Divisions;
            numericUpDownSubDiv.Value = newScaleBar.Subdivisions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                if (!IsChangedItem())
                {
                    if (sender.ToString().Contains("OK"))
                    {
                        this.Close();
                    }

                    return;
                }

                SetScaleBar();
                if (sender.ToString().Contains("OK"))
                {
                    //this.CloseThis = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
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
                        Properties.Resources.FormScaleBarSettings_ErrorWhenSetScaleBar);
                }

                SetChangedItemFlag(false);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //this.CloseThis = true;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// テキストボックス入力キーチェック(数字のみ)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// ResizeHintコンボボックス変更によるUI設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxResizeHint_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ResizeHintによるUI設定
            if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarFixed)
            {
                TextBoxDivision.Enabled = true;
                numericUpDownDiv.Enabled = true;
                numericUpDownSubDiv.Enabled = true;
            }
            else if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarAutoDivision)
            {
                TextBoxDivision.Enabled = false;
                numericUpDownDiv.Enabled = true;
                numericUpDownSubDiv.Enabled = true;
            }
            else if (comboBoxResizeHint.SelectedIndex ==
                (int)esriScaleBarResizeHint.esriScaleBarAutoDivisions)
            {
                TextBoxDivision.Enabled = true;
                numericUpDownDiv.Enabled = false;
                numericUpDownSubDiv.Enabled = true;
            }

            changeResizeHint = true;
        }

        private void TextBoxDivision_TextChanged(object sender, EventArgs e)
        {
            changeDivision = true;
        }

        private void numericUpDownDiv_ValueChanged(object sender, EventArgs e)
        {
            changeDivisions = true;
        }

        private void numericUpDownSubDiv_ValueChanged(object sender, EventArgs e)
        {
            changeSubdivisions = true;
        }

        private void checkBoxDivisionsBeforeZero_CheckedChanged(object sender, EventArgs e)
        {
            changeDivisionsBeforeZero = true;
        }

        private void comboBoxUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeUnits = true;
        }

        private void comboBoxUnitLabelPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeUnitLabelPosition = true;
        }

        private void SetChangedItemFlag(bool flag)
        {
            changeDivision = flag;
            changeDivisions = flag;
            changeSubdivisions = flag;
            changeDivisionsBeforeZero = flag;
            changeResizeHint = flag;
            changeUnits = flag;
            changeUnitLabelPosition = flag;
        }

        private bool IsChangedItem()
        {
            if (changeDivision == true
                || changeDivisions == true
                || changeSubdivisions == true
                || changeDivisionsBeforeZero == true
                || changeResizeHint == true
                || changeUnits == true
                || changeUnitLabelPosition == true)
            {
                return true;
            }

            return false;
        }
    }
}
