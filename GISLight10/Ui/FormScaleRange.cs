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

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class FormScaleRange : Form
    {
        private Ui.MainForm mainFrm;
        private IMapControl3 m_mapControl;
        private IGeoFeatureLayer m_geoSelectedFLayer;
        private IAnnotateLayerPropertiesCollection2 m_annoLayerPropCol;

        private ILayer m_SelectedLayer;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="mainFrm"></param>
        public FormScaleRange(ESRI.ArcGIS.Controls.IMapControl3 mapControl, Ui.MainForm mainFrm)
        {
            InitializeComponent();

            this.mainFrm = mainFrm;
            this.m_mapControl = mapControl;

        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormScaleRange_Load(object sender, EventArgs e)
        {
            double scaleMin = 0;
            double scaleMax = 0;
            IFeatureLayer selectedFLayer = null;
            IAnnotateLayerProperties annotateLayerProp = null;

            ILayer selectedLayer = null;

            // 判定時に使用するため初期化
            m_geoSelectedFLayer = null;
            m_SelectedLayer = null;

            
            //TOCで選択しているレイヤ（右クリックされたレイヤ）

            selectedLayer = this.mainFrm.SelectedLayer;

  
            ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                new ESRIJapan.GISLight10.Common.LayerManager();

            // レイヤ種別の判定
            if (pLayerManager.IsFeatureLayer(selectedLayer) == true)
            {
                selectedFLayer = (IGeoFeatureLayer)selectedLayer;
                m_geoSelectedFLayer = (IGeoFeatureLayer)selectedFLayer;

                m_annoLayerPropCol =
                    (IAnnotateLayerPropertiesCollection2)m_geoSelectedFLayer.AnnotationProperties;

                annotateLayerProp = (IAnnotateLayerProperties)m_annoLayerPropCol.get_Properties(0);

                // ラベル
                if (annotateLayerProp.AnnotationMinimumScale != 0)
                {
                    this.radioButtonScaleRange.Checked = true;
                    scaleMin = annotateLayerProp.AnnotationMinimumScale;
                    this.comboBoxMin.Text = "1:" + scaleMin.ToString("#,###,###");
                }

                if (annotateLayerProp.AnnotationMaximumScale != 0)
                {
                    this.radioButtonScaleRange.Checked = true;
                    scaleMax = annotateLayerProp.AnnotationMaximumScale;
                    this.comboBoxMax.Text = "1:" + scaleMax.ToString("#,###,###");
                }

                if (annotateLayerProp.AnnotationMinimumScale == 0 && annotateLayerProp.AnnotationMaximumScale == 0)
                {
                    this.radioButtonScaleRange.Checked = false;
                    // コンボボックス非活性
                    comboBoxMin.Enabled = false;
                    comboBoxMax.Enabled = false;
                }

                // レイヤ
                if (selectedFLayer.MinimumScale != 0)
                {
                    this.radioButtonLayerScaleRange.Checked = true;
                    scaleMin = selectedFLayer.MinimumScale;
                    this.comboBoxLayerMin.Text = "1:" + scaleMin.ToString("#,###,###");
                }

                if (selectedFLayer.MaximumScale != 0)
                {
                    this.radioButtonLayerScaleRange.Checked = true;
                    scaleMax = selectedFLayer.MaximumScale;
                    this.comboBoxLayerMax.Text = "1:" + scaleMax.ToString("#,###,###");
                }

                if (selectedFLayer.MinimumScale == 0 && selectedFLayer.MaximumScale == 0)
                {
                    this.radioButtonLayerScaleRange.Checked = false;
                    // コンボボックス非活性
                    comboBoxLayerMin.Enabled = false;
                    comboBoxLayerMax.Enabled = false;
                }
            }
            else
            {
                // ラベル
                this.tabControlRange.TabPages.Remove(tabPage1);

                // レイヤ
                m_SelectedLayer = selectedLayer;
                if (selectedLayer.MinimumScale != 0)
                {
                    this.radioButtonLayerScaleRange.Checked = true;
                    scaleMin = selectedLayer.MinimumScale;
                    this.comboBoxLayerMin.Text = "1:" + scaleMin.ToString("#,###,###");
                }

                if (selectedLayer.MaximumScale != 0)
                {
                    this.radioButtonLayerScaleRange.Checked = true;
                    scaleMax = selectedLayer.MaximumScale;
                    this.comboBoxLayerMax.Text = "1:" + scaleMax.ToString("#,###,###");
                }

                if (selectedLayer.MinimumScale == 0 && selectedLayer.MaximumScale == 0)
                {
                    this.radioButtonLayerScaleRange.Checked = false;
                    // コンボボックス非活性
                    comboBoxLayerMin.Enabled = false;
                    comboBoxLayerMax.Enabled = false;
                }
            }
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                switch (tabControlRange.SelectedIndex)
                {
                    // レイヤ
                    case 0:
                        // レイヤ縮尺設定
                        SetLayerScaleRange();
                        this.m_mapControl.ActiveView.Refresh();
                        this.Close();
                        break;
                    // ラベル
                    case 1:
                        // ラベル縮尺設定
                        SetLabelScaleRange();
                        this.m_mapControl.ActiveView.Refresh();
                        this.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 適用ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                switch (tabControlRange.SelectedIndex)
                {
                    // レイヤ
                    case 0:
                        // レイヤ縮尺設定
                        SetLayerScaleRange();
                        this.m_mapControl.ActiveView.Refresh();
                        break;
                    // ラベル
                    case 1:
                        // ラベル縮尺設定
                        SetLabelScaleRange();
                        this.m_mapControl.ActiveView.Refresh();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ラジオボタン（ラベル）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonNoScaleRange_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonNoScaleRange.Checked == true)
            {
                // コンボボックス非活性
                comboBoxMin.Enabled = false;
                comboBoxMax.Enabled = false;
            }
        }

        /// <summary>
        /// ラジオボタン（ラベル）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonScaleRange_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonScaleRange.Checked == true)
            {
                // コンボボックス活性
                comboBoxMin.Enabled = true;
                comboBoxMax.Enabled = true;
            }
        }

        /// <summary>
        /// ラジオボタン（レイヤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonNoLayerScaleRange_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonNoLayerScaleRange.Checked == true)
            {
                // コンボボックス非活性
                comboBoxLayerMin.Enabled = false;
                comboBoxLayerMax.Enabled = false;
            }
        }

        /// <summary>
        /// ラジオボタン（レイヤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonLayerScaleRange_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonLayerScaleRange.Checked == true)
            {
                // コンボボックス活性
                comboBoxLayerMin.Enabled = true;
                comboBoxLayerMax.Enabled = true;
            }
        }

        /// <summary>
        /// ラベル縮尺範囲設定
        /// </summary>
        private void SetLabelScaleRange()
        {
            double scaleMin = 0;
            double scaleMax = 0;
            IFeatureLayer selectedFLayer = null;
            IAnnotateLayerProperties annotateLayerProp = null;

            try
            {
                //TOCで選択しているレイヤ（右クリックされたレイヤ）
                selectedFLayer = (IFeatureLayer)this.mainFrm.SelectedLayer;

                m_geoSelectedFLayer = (IGeoFeatureLayer)selectedFLayer;

                m_annoLayerPropCol =
                    (IAnnotateLayerPropertiesCollection2)m_geoSelectedFLayer.AnnotationProperties;

                annotateLayerProp = (IAnnotateLayerProperties)m_annoLayerPropCol.get_Properties(0);
                
                if (this.radioButtonScaleRange.Checked == true)
                {
                    // 最小値
                    if (this.comboBoxMin.Text == "<なし>" || this.comboBoxMin.Text.Trim() == "")
                    {
                        // 縮尺範囲設定
                        annotateLayerProp.AnnotationMinimumScale = 0;
                    }
                    else
                    {
                        scaleMin = Convert.ToDouble(this.comboBoxMin.Text.Trim().Substring(2, this.comboBoxMin.Text.Length - 2));
                        // 縮尺範囲設定
                        annotateLayerProp.AnnotationMinimumScale = scaleMin;
                    }

                    // 最大値
                    if (this.comboBoxMax.Text == "<なし>" || this.comboBoxMax.Text.Trim() == "")
                    {
                        // 縮尺範囲設定
                        annotateLayerProp.AnnotationMaximumScale = 0;
                    }
                    else
                    {
                        scaleMax = Convert.ToDouble(this.comboBoxMax.Text.Trim().Substring(2, this.comboBoxMax.Text.Length - 2));
                        // 縮尺範囲設定
                        annotateLayerProp.AnnotationMaximumScale = scaleMax;
                    }
                }
                else
                {
                    // 縮尺範囲設定
                    annotateLayerProp.AnnotationMinimumScale = 0;
                    annotateLayerProp.AnnotationMaximumScale = 0;
                }
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxWarining(this, ex.Message);
            }
        }

        /// <summary>
        /// レイヤ縮尺範囲設定
        /// </summary>
        private void SetLayerScaleRange()
        {
            double scaleMin = 0;
            double scaleMax = 0;


            try
            {
 
                // レイヤオブジェクトへの変換
                if (m_geoSelectedFLayer != null)
                {
                    m_SelectedLayer = m_geoSelectedFLayer as ILayer;
                }

                if (this.radioButtonLayerScaleRange.Checked == true)
                {
                    // 最小値
                    if (this.comboBoxLayerMin.Text == "<なし>" || this.comboBoxLayerMin.Text.Trim() == "")
                    {
                        // 縮尺範囲設定
                        m_SelectedLayer.MinimumScale = 0;
                    }
                    else
                    {
                        scaleMin = Convert.ToDouble(this.comboBoxLayerMin.Text.Trim().Substring(2, this.comboBoxLayerMin.Text.Length - 2));
                        // 縮尺範囲設定
                        m_SelectedLayer.MinimumScale = scaleMin;
                    }

                    // 最大値
                    if (this.comboBoxLayerMax.Text == "<なし>" || this.comboBoxLayerMax.Text.Trim() == "")
                    {
                        // 縮尺範囲設定
                        m_SelectedLayer.MaximumScale = 0;
                    }
                    else
                    {
                        scaleMax = Convert.ToDouble(this.comboBoxLayerMax.Text.Trim().Substring(2, this.comboBoxLayerMax.Text.Length - 2));
                        // 縮尺範囲設定
                        m_SelectedLayer.MaximumScale = scaleMax;
                    }
                }
                else
                {
                    // 縮尺範囲設定
                    m_SelectedLayer.MinimumScale = 0;
                    m_SelectedLayer.MaximumScale = 0;
                }
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxWarining(this, ex.Message);
            }
        }

        /// <summary>
        /// テキストフォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_Leave(object sender, EventArgs e)
        {
            int idx = -1;
            double scale = 0;
            ComboBox combo;

            combo = (ComboBox)sender;

            if (combo.Text == "" || combo.Text == "<なし>")
            {
                return;
            }

            try
            {
                idx = combo.Text.IndexOf("1:");
                if (idx == -1)
                {
                    scale = Convert.ToDouble(combo.Text);
                    combo.Text = "1:" + scale.ToString("#,###,###");
                }
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxWarining(this, ex.Message);
            }
        }
    }
}
