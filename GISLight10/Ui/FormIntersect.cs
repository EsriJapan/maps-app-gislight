using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geoprocessor;

using ESRIJapan.GISLight10.Common;
using ESRI.ArcGIS.esriSystem;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class FormIntersect : Form
    {

        private Ui.MainForm mainFrm;
        private IMapControl3 m_pMapControl;
        private Common.OptionSettings settingFile = null;
        private bool m_execBackground = false;
        private const string FIELD_NAME_BACKGROUND = "バックグラウンド実行フラグ";
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="mainFrm"></param>
        public FormIntersect(ESRI.ArcGIS.Controls.IMapControl3 mapControl, Ui.MainForm mainFrm)
        {
            InitializeComponent();

            try
            {
                settingFile = new Common.OptionSettings();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }

            this.mainFrm = mainFrm;
            this.m_pMapControl = mapControl;

            //ダイアログ リセット
            this.comboBoxParameter11.Items.Clear();
            this.comboBoxParameter12.Items.Clear();
            this.textBoxParameter21.Clear();
            this.textBoxParameter22.Clear();

            //入力フィーチャ レイヤ一覧設定
            LayerManager layManager;
            List<IFeatureLayer> layers = null;
            LayerComboItem item;
            layManager = new LayerManager();
            layers = layManager.GetFeatureLayers(m_pMapControl.Map);

            foreach (IFeatureLayer pLayer in layers)
            {
                item = new LayerComboItem(pLayer);
                this.comboBoxParameter11.Items.Add(item);
                this.comboBoxParameter12.Items.Add(item);
            }

            //リストの選択
            if (this.comboBoxParameter11.Items.Count > 0)
            {
                this.comboBoxParameter11.SelectedIndex = 0;
            }

            if (this.comboBoxParameter12.Items.Count > 0)
            {
                this.comboBoxParameter12.SelectedIndex = 0;
            }

            //フォーカスの変更
            comboBoxParameter11.Focus();

        }

        /// <summary>
        /// [出力ワークスペース] ダイアログ選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showOpenWorkspace_Click(object sender, EventArgs e)
        {
            FormGISDataSelectDialog frm = new FormGISDataSelectDialog();
            //frm.StartFolder = @"C:\arcgis\ArcTutor\Editing"; // オプション1:開始フォルダ
            frm.SelectType = FormGISDataSelectDialog.ReturnType.WorkspaceName;//オプション2:選択したいデータ種類

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                textBoxParameter21.Text = frm.SelectedPath;
            }

            frm.Dispose();
        }

        /// <summary>
        /// [実行] ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            //パラメータ チェック
            if (!checkParameter())
            {
                return;
            }

            // オプション設定よりバックグラウンド実行設定を取得
            try
            {
                m_execBackground = (settingFile.GeoprocessingBackground == "1");
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_BACKGROUND + " ]"

                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_BACKGROUND + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                // 読み込めない場合でも、処理の継続をする（フォアグラウンド実行）
            }

            //フィーチャ レイヤ1からパスを取得
            string param11 = ComboBoxFeatureLayer.FileNameFullPath(((LayerComboItem)comboBoxParameter11.SelectedItem).Layer);
            string param12 = ComboBoxFeatureLayer.FileNameFullPath(((LayerComboItem)comboBoxParameter12.SelectedItem).Layer);
            string in_features = param11 + " #;" + param12 + " #";
            string out_feature_class = textBoxParameter21.Text + @"\" + textBoxParameter22.Text;


            //パラメータ セット
            IVariantArray pVariantArray = new VarArrayClass();
            pVariantArray.Add(in_features);
            pVariantArray.Add(out_feature_class);
            pVariantArray.Add("ALL");
            pVariantArray.Add("#");
            pVariantArray.Add("INPUT");

            //ジオプロセシング処理
            this.Visible = false;
            FormExecuteGP frm = new FormExecuteGP(this.Owner, m_pMapControl.Map);
            frm.Execute("Intersect_analysis", pVariantArray, checkBoxAddMap.Checked, m_execBackground, checkBoxOverwrite.Checked,"");

        }

        /// <summary>
        /// ジオプロセシング ツール実行前のパラメータ チェック
        /// </summary>
        /// <returns></returns>
        private bool checkParameter()
        {
            string errorStrings = "";

            if (this.comboBoxParameter11.SelectedIndex == -1)
            {
                errorStrings += "・入力フィーチャ レイヤが選択されていません。" + Environment.NewLine;
            }

            if (this.comboBoxParameter12.SelectedIndex == -1)
            {
                errorStrings += "・交差フィーチャ レイヤが選択されていません。" + Environment.NewLine;
            }

            if (comboBoxParameter11.SelectedIndex == comboBoxParameter12.SelectedIndex)
            {
                errorStrings += "・入力フィーチャ レイヤと交差フィーチャ レイヤが同一です。" + Environment.NewLine;
            }

            if (textBoxParameter21.Text == "")
            {
                errorStrings += "・出力ワークスペースが選択されていません。" + Environment.NewLine;
            }

            if (this.textBoxParameter22.Text == "")
            {
                errorStrings += "・出力フィーチャクラス名が入力されていません。" + Environment.NewLine;
            }

            if (errorStrings != "")
            {
                MessageBox.Show(errorStrings, "パラメータ チェック エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //フィーチャクラス名のバリデート
            errorStrings += Check.FeatureClassString(this.textBoxParameter21.Text, this.textBoxParameter22.Text, checkBoxOverwrite.Checked);

            if (errorStrings != "")
            {
                MessageBox.Show(errorStrings, "パラメータ チェック エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
