using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRIJapan.FileDlgExtenders;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// マップエクスポート画面での解像度、幅、高さを指定する
    /// </summary>
    /// <history>
    ///  2010-11-01(xx) 新規作成 (ej7022)
    /// </history>
    public partial class UserControlExportMap : FileDlgExtenders.FileDialogControlBase
    {
        string filterSave = "init";

        private int MaxResolution = 0;
        private const int MinResolution = 1;

        private int dpi;
        /// <summary>
        /// DPI
        /// </summary>
        public int Dpi
        {
            get { return this.dpi; }
        }

        private string lastExportPath = "";
        /// <summary>
        /// マップエクスポート先パス
        /// </summary>
        public string LastExportPath
        {
            set { this.lastExportPath = value; }
            get { return this.lastExportPath; }
        }

        private ESRI.ArcGIS.Carto.IActiveView activeView;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="activeView">マップコントロールのアクティブビュー</param>
        public UserControlExportMap( 
            ESRI.ArcGIS.Carto.IActiveView activeView)
        {
            InitializeComponent();

            try
            {
                // 指定可能な最大解像度
                Common.OptionSettings settingFile = new Common.OptionSettings();
                MaxResolution = int.Parse(settingFile.ExportMapResolutionMax);
            }
            catch 
            {
                Common.MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                this.Dispose();
                return;
            }

            this.activeView = activeView;
            SetDpiWidthHeight();
        }

        /// <summary>
        /// 継承元のFileDlgExtenders.FileDialogControlBaseのOnPrepareMSDialogをコール
        /// </summary>
        protected override void OnPrepareMSDialog()
        {
            base.FileDlgInitialDirectory = this.lastExportPath;
            base.OnPrepareMSDialog();
        }

        /// <summary>
        ///  initialize
        /// </summary>
        private void Init()
        {
            this.numericUpDownDpi.Value = 96;
            this.panelWidthHeight.Visible = true;
        }

        /// <summary>
        /// 解像度に合わせて幅高さを変更
        /// </summary>
        private void SetDpiWidthHeight()
        {
            try
            {
                if (base.FilterIndex > 0)
                {
                    string[] filterTypes = base.FilterFileTypes.Split(',');

                    if (filterTypes[base.FilterIndex - 1].Contains("PDF"))
                    {
                        if (this.panelWidthHeight.Visible)
                        {
                            this.numericUpDownDpi.Value = 300;
                            this.panelWidthHeight.Visible = false;
                        }
                        return;
                    }

                    if (!filterSave.Equals(filterTypes[base.FilterIndex - 1]))
                    {
                        filterSave = filterTypes[base.FilterIndex - 1];

                        Init();
                    }
                }

                if (this.activeView != null)
                {
                    decimal screenResolution = 96;
                    decimal outputResolution = Convert.ToDecimal(dpi);
                    decimal calcpix = outputResolution / screenResolution;

                    this.labelWidth.Text =
                        (Convert.ToInt32(
                        this.activeView.ExportFrame.right * calcpix)).ToString();

                    this.labelHeight.Text =
                        (Convert.ToInt32(
                        this.activeView.ExportFrame.bottom * calcpix)).ToString();
                }
            }
            catch (Exception ex) 
            {
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// マップエクスポート時DPI指定内容チェック
        /// </summary>
        /// <returns>true:マップエクスポート時DPI指定内容OK,
        /// false:マップエクスポート時DPI指定内容NG</returns>
        public bool IsDpiValid()
        {
            bool isNum = 
                Common.UtilityClass.IsNumeric(this.numericUpDownDpi.Text);

            if (isNum)
            {
                int dpival = Convert.ToInt32(this.numericUpDownDpi.Text);

                if (dpival < MinResolution || dpival > MaxResolution)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// マップエクスポート時DPI指定エラーメッセージ表示
        /// </summary>
        public void ShowDpiError()
        {
            StringBuilder msgstr = new StringBuilder();
            msgstr.Append(Properties.Resources.ExportMap_ErrorResolutionSetting);
            msgstr.Append("\n");
            msgstr.Append("[ ");
            msgstr.Append(MinResolution);
            msgstr.Append(" - ");
            msgstr.Append(MaxResolution);
            msgstr.Append(" ]");
            Common.MessageBoxManager.ShowMessageBoxWarining(this, msgstr.ToString());
        }


        /// <summary>
        /// 解像度指定変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownDpi_ValueChanged(object sender, EventArgs e)
        {
            dpi = Convert.ToInt32(this.numericUpDownDpi.Value);
            SetDpiWidthHeight();
        }

        /// <summary>
        /// ロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControlExportMap_Load(object sender, EventArgs e)
        {
            Init();
        }

        /// <summary>
        /// エクスポートファイルタイプ変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="index"></param>
        private void UserControlExporMap_FilterChanged(IWin32Window sender, int index)
        {
            SetDpiWidthHeight();
        }

        private void numericUpDownDpi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.numericUpDownDpi.Text.Length == 0) return;

            // このタイミングでは数値チェックだけは行なう
            bool isNum = 
                Common.UtilityClass.IsNumeric(this.numericUpDownDpi.Text);

            if (!isNum)
            {
                ShowDpiError();
            }
        }
    }
}
