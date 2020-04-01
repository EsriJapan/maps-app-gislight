using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using System.Collections;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ハイパーリンクの設定機能
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormManageHyperlinks : Form
    {
        /// <summary>
        /// ハイパーリンクの設定を行うレイヤを取得するフォーム
        /// </summary>
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// 各変数、UIの初期化を行う
        /// </summary>
        /// <param name="frm">メインフォーム</param>
        public FormManageHyperlinks(Ui.MainForm frm)
        {
            try
            {
                InitializeComponent();
                this.mainFrm = frm;

                // UI設定
                InitializeForm();
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormManageHyperlinks_ERROR_FormOpenFailed);
                Common.Logger.Error(Properties.Resources.FormManageHyperlinks_ERROR_FormOpenFailed);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
                this.Close();
                //this.Dispose();
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormManageHyperlinks_ERROR_FormOpenFailed);
                Common.Logger.Error(Properties.Resources.FormManageHyperlinks_ERROR_FormOpenFailed);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
                this.Close();
                //this.Dispose();
            }
        }

        /// <summary>
        /// UI設定
        /// </summary>
        private void InitializeForm()
        {
            buttonOK.Enabled = false;

            ESRI.ArcGIS.Carto.IFeatureLayer sourceFeatureLayer =
                this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer;
            
            // ハイパーリンク対象フィールドをコンボボックスへ設定 (可視,文字型のみ)
            Common.FieldComboItem[]	cmbFlds = this.mainFrm.GetFieldItems(sourceFeatureLayer, true, false, false, true, esriFieldType.esriFieldTypeString);
			this.comboSelectField.Items.AddRange(cmbFlds);
            
            // ハイパーリンク対象フィールドのチェック
            if (comboSelectField.Items.Count > 0)
            {
                buttonOK.Enabled = true;
            }
            else
            {
                panelSelectField.Enabled = false;
                checkUseHyperlinks.Checked = false;
                checkUseHyperlinks.Enabled = false;

                Common.MessageBoxManager.ShowMessageBoxWarining(
                    this, Properties.Resources.FormManageHyperlinks_WARNING_NoField);

                this.Close();
                //this.Dispose();
                return;
            }

            IHotlinkContainer sourceHotlinkContainer = sourceFeatureLayer as IHotlinkContainer;

            // 既存ハイパーリンク設定のチェック
            //if (sourceHotlinkContainer.HotlinkField != "")
            if (Common.HyperlinkFunctions.HasHyperlinks(sourceFeatureLayer))
            {
                panelSelectField.Enabled = true;
                checkUseHyperlinks.Checked = true;

                for (int i = 0; i < comboSelectField.Items.Count; i++)
                {
                    Common.FieldComboItem sourceHotlinkField = comboSelectField.Items[i] as Common.FieldComboItem;

                    if (sourceHotlinkField.Field.Name == sourceHotlinkContainer.HotlinkField)
                    {
                        // 既存ハイパーリンクのフィールドを選択
                        comboSelectField.SelectedIndex = i;
                        break;
                    }
                }

                // 既存ハイパーリンクのタイプを選択
                if (sourceHotlinkContainer.HotlinkType == esriHyperlinkType.esriHyperlinkTypeDocument)
                {
                    radioUseDoc.Checked = true;
                }
                else if (sourceHotlinkContainer.HotlinkType == esriHyperlinkType.esriHyperlinkTypeURL)
                {
                    radioUseURL.Checked = true;
                }
                else
                {
                    radioUseDoc.Checked = true;
                }
            }
            else
            {
                panelSelectField.Enabled = false;
                checkUseHyperlinks.Checked = false;

                comboSelectField.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Formクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormManageHyperlinks_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Dispose();
        }

        /// <summary>
        /// ハイパーリンクを設定or解除(OKボタン)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Carto.IFeatureLayer sourceFeatureLayer =
                    this.mainFrm.SelectedLayer as ESRI.ArcGIS.Carto.IFeatureLayer;

            //IGeoFeatureLayer sourceGeoFeatureLayer = sourceFeatureLayer as IGeoFeatureLayer;
            //IFeatureClass sourceFeatureClass = sourceGeoFeatureLayer.DisplayFeatureClass;

            IHotlinkContainer sourceHotlinkContainer = sourceFeatureLayer as IHotlinkContainer;

            // 入力チェック
            // ハイパーリンクを設定するフィールドチェック
            if (checkUseHyperlinks.Checked && comboSelectField.SelectedIndex == -1)
            {
                Common.MessageBoxManager.ShowMessageBoxWarining(
                    this, Properties.Resources.FormManageHyperlinks_WARNING_SelectNoField);

                return;
            }

            // ハイパーリンク設定解除確認
            bool isDeleteHyaperlinks = false;

            if (!checkUseHyperlinks.Checked)
            {
                // 変更なし
                //if (sourceHotlinkContainer.HotlinkField == "")
                if (!Common.HyperlinkFunctions.HasHyperlinks(sourceFeatureLayer))
                {
                    this.Close();
                    //this.Dispose();

                    return;
                }
                // 解除確認
                else
                {
                    DialogResult resDeleteHyperlinks = Common.MessageBoxManager.ShowMessageBoxQuestion2(
                    this, Properties.Resources.FormManageHyperlinks_QUESTION_DeleteHyperlinks);

                    if (resDeleteHyperlinks == DialogResult.OK)
                    {
                        isDeleteHyaperlinks = true;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Common.Logger.Info("ハイパーリンクの設定開始");

            try
            {
                this.buttonOK.Enabled = false;

                // ハイパーリンクの解除
                if (!checkUseHyperlinks.Checked && isDeleteHyaperlinks)
                {
                    //sourceHotlinkContainer.HotlinkField = "";
                    //sourceHotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeDocument;

                    Common.HyperlinkFunctions.DeleteHyperlinks(sourceFeatureLayer);
                }
                // ハイパーリンクの設定
                else
                {
                    Common.FieldComboItem targetHotlinkField =
                        comboSelectField.SelectedItem as Common.FieldComboItem;

                    sourceHotlinkContainer.HotlinkField = targetHotlinkField.Field.Name;

                    int targetHotlinkType = 0;

                    if (radioUseDoc.Checked)
                    {
                        //sourceHotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeDocument;
                        //targetHotlinkType = 0;

                        targetHotlinkType = (int)esriHyperlinkType.esriHyperlinkTypeDocument;
                    }
                    else if (radioUseURL.Checked)
                    {
                        //sourceHotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeURL;
                        //targetHotlinkType = 1;

                        targetHotlinkType = (int)esriHyperlinkType.esriHyperlinkTypeURL;
                    }
                    else
                    {
                        //sourceHotlinkContainer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeDocument;
                    }

                    Common.HyperlinkFunctions.SetHyperlinks(
                        sourceFeatureLayer, targetHotlinkField.Field.Name, targetHotlinkType);
                }
            }
            catch (COMException comex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormManageHyperlinks_ERROR_SetHyperlinksFailed);
                Common.Logger.Error(Properties.Resources.FormManageHyperlinks_ERROR_SetHyperlinksFailed);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.FormManageHyperlinks_ERROR_SetHyperlinksFailed);
                Common.Logger.Error(Properties.Resources.FormManageHyperlinks_ERROR_SetHyperlinksFailed);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            finally
            {
                Common.Logger.Info("ハイパーリンクの設定終了");

                //mainFrm.axMapControl1.ActiveView.PartialRefresh
                //    (esriViewDrawPhase.esriViewGeoSelection, null, mainFrm.axMapControl1.ActiveView.Extent);
                mainFrm.axMapControl1.ActiveView.Refresh();

                this.buttonOK.Enabled = true;
                this.Close();
                //this.Dispose();
            }
        }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            //this.Dispose();
        }

        /// <summary>
        /// ハイパーリンク使用or不使用チェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkUseHyperlinks_CheckedChanged(object sender, EventArgs e)
        {
            if (checkUseHyperlinks.Checked)
            {
                panelSelectField.Enabled = true;
            }
            else
            {
                panelSelectField.Enabled = false;
            }
        }
    }
}
