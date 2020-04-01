using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using stdole;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ラベル表示
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public partial class FormLabelSetting : Form
    {
        private Ui.MainForm m_mainFrm;
        private IMapControl3 m_mapControl;        
        private IGeoFeatureLayer m_geoSelectedFLayer;
        IAnnotateLayerPropertiesCollection2 m_annoLayerPropCol;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="frm">メインフォーム</param>
        /// <param name="mapControl">マップコントロール</param>
        public FormLabelSetting(Ui.MainForm frm, IMapControl3 mapControl)
        {
            this.m_mainFrm = frm;
            this.m_mapControl = mapControl;
            
            InitializeComponent();

            IFeatureLayer selectedFLayer = null;
            ILabelEngineLayerProperties labelEngineLayerProp = null;
            IBasicOverposterLayerProperties4 basicOverposterLayerProp = null;
            IOverposterLayerProperties2 overposterLayerProp = null;

            try
            {
                Common.Logger.Info("ラベル表示フォームの起動");

                //TOCで選択しているレイヤ（右クリックされたレイヤ）
                selectedFLayer = (IFeatureLayer)this.m_mainFrm.SelectedLayer;

                m_geoSelectedFLayer = (IGeoFeatureLayer)selectedFLayer;
                
                m_annoLayerPropCol =
                    (IAnnotateLayerPropertiesCollection2)m_geoSelectedFLayer.AnnotationProperties;

                labelEngineLayerProp =
                    (ILabelEngineLayerProperties)m_annoLayerPropCol.get_Properties(0);

                basicOverposterLayerProp =
                    (IBasicOverposterLayerProperties4)labelEngineLayerProp.BasicOverposterLayerProperties;
                
                overposterLayerProp =
                    (IOverposterLayerProperties2)basicOverposterLayerProp;                               


                /*** フォームを開いた時点のラベル設定を読み込む ***/

                //ポイントの場合
                if (m_geoSelectedFLayer.FeatureClass.ShapeType ==
                    esriGeometryType.esriGeometryPoint)
                {
                    //フォームのリサイズ
                    ResizePointForm();
                }


                //フォームの初期化
                //チェックボックス:[このレイヤのラベルを表示する]                
                InitCheckBoxShowLabel();


                //フォームの初期化
                //リストボックス:[フィールド一覧]
                InitListBoxAllFields();


                //フォームの初期化
                //リストボックス:[ラベル フィールド]
                InitListBoxLabelFields(labelEngineLayerProp);


                //フォームの初期化
                //グループ:[テキストシンボル]
                InitGroupBoxTextSymbol(labelEngineLayerProp);


                //配置コンボボックスにアイテムを追加
                AddComboItemLabelPosition(comboBoxLabelPosition);

                //フォームの初期化
                //コンボボックス:[配置]                
                if (m_geoSelectedFLayer.FeatureClass.ShapeType == 
                    esriGeometryType.esriGeometryPoint)
                {
                    InitPointComboBoxLabelPosition(basicOverposterLayerProp);
                }
                else if (m_geoSelectedFLayer.FeatureClass.ShapeType == 
                    esriGeometryType.esriGeometryPolyline)
                {
                    InitLineComboBoxLabelPosition(basicOverposterLayerProp);
                }
                else if (m_geoSelectedFLayer.FeatureClass.ShapeType ==
                    esriGeometryType.esriGeometryPolygon)
                {
                    InitPolygonComboBoxLabelPosition(basicOverposterLayerProp);
                }
 

                //フォームの初期化
                //チェックボックス:[重なりを許可]
                InitCheckBoxAllowOverlap(basicOverposterLayerProp, overposterLayerProp);                          


                //フォームの初期化
                //グループ:[同じラベルの配置方法]
                InitGroupBoxSameLabelMethod(basicOverposterLayerProp);                                
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormLabelSetting_ERROR_FormLoad);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_FormLoad);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);

                this.Close();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormLabelSetting_ERROR_FormLoad);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_FormLoad);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Close();
            }
            finally
            {
                //ComReleaser.ReleaseCOMObject(selectedFLayer);
                ComReleaser.ReleaseCOMObject(labelEngineLayerProp);
                ComReleaser.ReleaseCOMObject(basicOverposterLayerProp);
                ComReleaser.ReleaseCOMObject(overposterLayerProp);
            }         
        }           


        /// <summary>
        /// OKボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {            
            try
            {
                if (checkBoxShowLabel.Checked == true && listBoxLabelFields.Items.Count == 0)
                {
                    Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormLabelSetting_WARNING_NoLabelField);
                    Common.Logger.Warn(Properties.Resources.FormLabelSetting_WARNING_NoLabelField);

                    return;
                }

                //ラベル表示の実行
                Labeling();

                this.Close();
            }            
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }            
        }        
        

        /// <summary>
        /// 適用ボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAccept_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBoxShowLabel.Checked == true && listBoxLabelFields.Items.Count == 0)
                {
                    Common.MessageBoxManager.ShowMessageBoxWarining
                        (this, Properties.Resources.FormLabelSetting_WARNING_NoLabelField);
                    Common.Logger.Warn(Properties.Resources.FormLabelSetting_WARNING_NoLabelField);

                    return;
                }

                //ラベル表示の実行
                Labeling();
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_DoLabeling);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }           
        }  


        /// <summary>
        /// キャンセルボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// このレイヤのラベルを表示するチェックボックス 選択状態変更時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxShowLabel_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowLabel.Checked == true)
            {
                panel1.Enabled = true;
                
            }
            else
            {
                panel1.Enabled = false;
            }
        }

        
        /// <summary>
        /// 追加ボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Common.FieldComboItem addItem = (Common.FieldComboItem)listBoxAllFields.SelectedItem;

                if (listBoxLabelFields.Items.Count == 0)
                {
                    //listBoxLabelFields へ listBoxAllFieldsで選択中のフィールドを追加
                    listBoxLabelFields.Items.Add(listBoxAllFields.SelectedItem);
                }
                else
                {
                    //選択アイテムが追加済みでないか確認
                    for (int i = 0; i < listBoxLabelFields.Items.Count; i++)
                    {
                        Common.FieldComboItem alreadyAddItem = 
                            (Common.FieldComboItem)listBoxLabelFields.Items[i];

                        //選択アイテムが追加済みの場合
                        if (addItem.Field.Equals(alreadyAddItem.Field))
                        {
                            return;                            
                        }
                        else if (addItem.Field.Name.Equals(alreadyAddItem.Field.Name))
                        {
                            //ラベル フィールドに名前が同じフィールドを複数追加することはできません。
                            ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                                (this, Properties.Resources.FormLabelSetting_WARNING_AddSameName);
                            Common.Logger.Warn(Properties.Resources.FormLabelSetting_WARNING_AddSameName);

                            return;
                        }
                    }

                    //listBoxLabelFields へ listBoxAllFieldsで選択中のフィールドを追加
                    listBoxLabelFields.Items.Add(listBoxAllFields.SelectedItem);
                }                                                      
            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this, Properties.Resources.FormLabelSetting_ERROR_FiledAdd);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_FiledAdd);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormLabelSetting_ERROR_FiledAdd);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_FiledAdd);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }


        /// <summary>
        /// 削除ボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            try
            {
                //アイテムを選択している場合
                if (listBoxLabelFields.SelectedIndex != -1)
                {
                    //選択アイテムを除外する
                    listBoxLabelFields.Items.RemoveAt(listBoxLabelFields.SelectedIndex);    
                }
                else
                {
                    return;
                }
            }            
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormLabelSetting_ERROR_FieldRemove);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_FieldRemove);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }


        /// <summary>
        /// ↑ボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBoxLabelFields.SelectedIndex;

                //アイテムが選択されている かつ 選択アイテムが最上位ではない場合
                //選択アイテム と 1つ上のアイテムを入れ替える。
                if (selectedIndex > 0)
                {
                    int changeIndex = selectedIndex - 1;

                    //① 選択アイテムの1つ上のアイテムを取得
                    object item = listBoxLabelFields.Items[changeIndex];

                    //② 選択アイテムを1つ上のインデックスに格納
                    listBoxLabelFields.Items[changeIndex] = listBoxLabelFields.Items[selectedIndex];

                    //③ ①で取得したアイテムを選択アイテムのインデックスに格納
                    listBoxLabelFields.Items[selectedIndex] = item;

                    listBoxLabelFields.SelectedIndex = changeIndex;
                }
                else
                {
                    return;
                }
            }            
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormLabelSetting_ERROR_FormLoad);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_UpDownField);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }


        /// <summary>
        /// ↓ボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBoxLabelFields.SelectedIndex;
                int count = listBoxLabelFields.Items.Count;

                //アイテムが選択されている かつ 選択アイテムが最下位ではない場合
                //選択アイテム と 1つ下のアイテムを入れ替える。
                if (selectedIndex != -1 && selectedIndex < count-1)
                {
                    int changeIndex = selectedIndex + 1;

                    //① 選択アイテムの1つ下のアイテムを取得
                    object item = listBoxLabelFields.Items[changeIndex];

                    //② 選択アイテムを1つ下のインデックスに格納
                    listBoxLabelFields.Items[changeIndex] = listBoxLabelFields.Items[selectedIndex];

                    //③ ①で取得したアイテムを選択アイテムのインデックスに格納
                    listBoxLabelFields.Items[selectedIndex] = item;

                    listBoxLabelFields.SelectedIndex = changeIndex;
                }
                else
                {
                    return;
                }
            }            
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                                    (this, Properties.Resources.FormLabelSetting_ERROR_FormLoad);
                Common.Logger.Error(Properties.Resources.FormLabelSetting_ERROR_UpDownField);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }


        /// <summary>
        /// 色ボタン クリック時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTextColor_Click(object sender, EventArgs e)
        {
            //設定ファイルが存在するか確認する
            if (!ESRIJapan.GISLight10.Common.ApplicationInitializer.IsUserSettingsExists())
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                //this.Dispose();
                return;
            }

            try
            {
                //選択された色をボタンの背景色に設定する            
                buttonTextColor.BackColor =
                    Common.UtilityClass.GetColor(buttonTextColor.BackColor);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ カラーダイアログ ]" +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ カラーダイアログ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                //this.Dispose();
                return;
            }    

            

            //if (colorDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    //選択された色をボタンの背景色に設定する
            //    buttonTextColor.BackColor = colorDialog1.Color;
            //}
        }


        /// <summary>
        /// フォーム クローズ時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLabelSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            Common.Logger.Info("ラベル表示フォームの終了");
        }



        /// <summary>
        /// ラベル表示の実行
        /// </summary>
        private void Labeling()
        {
            ILabelEngineLayerProperties labelEngineLayerProp = null;
            IBasicOverposterLayerProperties4 basicOverposterLayerProp = null;
            IOverposterLayerProperties2 overposterLayerProp = null;

            try
            {                
                if (checkBoxShowLabel.Checked == true)
                {
                    //ラベル設定のクリア             
                    m_annoLayerPropCol.Clear();

                    labelEngineLayerProp = new LabelEngineLayerPropertiesClass();

                    basicOverposterLayerProp = (IBasicOverposterLayerProperties4)
                        labelEngineLayerProp.BasicOverposterLayerProperties;

                    overposterLayerProp = (IOverposterLayerProperties2)basicOverposterLayerProp;


                    //高度なラベル条件式を使用可能にする
                    labelEngineLayerProp.IsExpressionSimple = false;


                    //ラベル条件式の設定                          
                    string expression = CreateLabelExpression(listBoxLabelFields);
                    labelEngineLayerProp.Expression = expression;


                    /*** テキストシンボルの設定 ***/
                    //テキストシンボル（フォント）の設定
                    stdole.StdFont stdFont = new StdFont();
                    stdFont.Name = comboBoxTextFont.SelectedItem.ToString();
                    labelEngineLayerProp.Symbol.Font = (stdole.IFontDisp)stdFont;

                    //テキストシンボル（サイズ）の設定
                    labelEngineLayerProp.Symbol.Size = System.Convert.ToDouble(comboBoxTextSize.SelectedItem);

                    //テキストシンボル（色）の設定
                    IRgbColor rgbColor = new RgbColorClass();

                    //rgbColor.Red = colorDialog1.Color.R;
                    //rgbColor.Blue = colorDialog1.Color.B;
                    //rgbColor.Green = colorDialog1.Color.G;

                    rgbColor.Red = buttonTextColor.BackColor.R;
                    rgbColor.Blue = buttonTextColor.BackColor.B;
                    rgbColor.Green = buttonTextColor.BackColor.G;

                    labelEngineLayerProp.Symbol.Color = (IColor)rgbColor;
                    /*******/


                    /*** ラベル配置の設定 ***/
                    esriGeometryType geometryType = m_geoSelectedFLayer.FeatureClass.ShapeType;

                    string selectedLabelPosition = comboBoxLabelPosition.SelectedItem.ToString();

                    //ポイントの場合
                    if (geometryType == esriGeometryType.esriGeometryPoint)
                    {
                        basicOverposterLayerProp.FeatureType =
                            esriBasicOverposterFeatureType.esriOverposterPoint;

                        basicOverposterLayerProp.PointPlacementMethod =
                            esriOverposterPointPlacementMethod.esriAroundPoint;
                        
                        IPointPlacementPriorities pointPlacementPriorities =
                            ReturnPointLabelPosition(selectedLabelPosition);

                        basicOverposterLayerProp.PointPlacementPriorities = pointPlacementPriorities;
                    }
                    //ラインの場合
                    else if (geometryType == esriGeometryType.esriGeometryPolyline)
                    {
                        basicOverposterLayerProp.FeatureType =
                            esriBasicOverposterFeatureType.esriOverposterPolyline;
                        
                        ILineLabelPosition lineLabelPosition = 
                            ReturnLineLabelPosition(selectedLabelPosition);

                        basicOverposterLayerProp.LineLabelPosition = lineLabelPosition; 
                    }
                    //ポリゴンの場合
                    else if (geometryType == esriGeometryType.esriGeometryPolygon)
                    {
                        basicOverposterLayerProp.FeatureType =
                            esriBasicOverposterFeatureType.esriOverposterPolygon;
                        
                        esriOverposterPolygonPlacementMethod polygonPlacementMethod = 
                            ReturnPolygonLabelPosition(selectedLabelPosition);

                        basicOverposterLayerProp.PolygonPlacementMethod = polygonPlacementMethod;
                    }
                    /******************/


                    //重なり許可の設定
                    if (checkBoxAllowOverlap.Checked == true)
                    {
                        basicOverposterLayerProp.GenerateUnplacedLabels = true;

                        //許可
                        overposterLayerProp.TagUnplaced = false;
                    }
                    else
                    {
                        basicOverposterLayerProp.GenerateUnplacedLabels = true;

                        //不許可
                        overposterLayerProp.TagUnplaced = true;
                    }


                    //同じラベルの配置方法の設定                 
                    if (geometryType == esriGeometryType.esriGeometryPoint ||
                        radioButtonDeleteSameLabel.Checked == true)
                    {
                        //同じラベルを削除する
                        basicOverposterLayerProp.NumLabelsOption =
                            esriBasicNumLabelsOption.esriOneLabelPerName;
                    }
                    else if (radioButtonOneLabelByOneFeature.Checked == true)
                    {
                        //フィーチャ毎に1つのラベルを配置する
                        basicOverposterLayerProp.NumLabelsOption =
                            esriBasicNumLabelsOption.esriOneLabelPerShape;
                    }


                    m_annoLayerPropCol.Add((IAnnotateLayerProperties)labelEngineLayerProp);

                    //ラベルを表示する
                    m_geoSelectedFLayer.DisplayAnnotation = true;

                    m_mapControl.ActiveView.Refresh();
                }
                else
                {
                    //ラベルを非表示にする
                    m_geoSelectedFLayer.DisplayAnnotation = false;

                    m_mapControl.ActiveView.Refresh();
                }

                //this.Close();
            }
            catch (COMException comex)
            {
                throw comex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(labelEngineLayerProp);
                ComReleaser.ReleaseCOMObject(basicOverposterLayerProp);
                ComReleaser.ReleaseCOMObject(overposterLayerProp);
            }
        }


        #region コンボボックスにアイテム追加

        /// <summary>
        /// リストボックスにフィールドを追加
        /// </summary>
        /// <param name="listBox">リストボックス</param>
        /// <param name="FieldInfos">フィールド情報</param>
        private void AddListItemField(ListBox listBox, ITableFields FieldInfos) {
            listBox.Items.Clear();

            // ﾌｨｰﾙﾄﾞ･ｱｲﾃﾑを取得 (ﾊﾞｲﾅﾘ型を除く)
            Common.FieldComboItem[]	cmbFlds = this.m_mainFrm.GetFieldItems(FieldInfos, false, false, false, true, 
													esriFieldType.esriFieldTypeSmallInteger,
													esriFieldType.esriFieldTypeInteger,
													esriFieldType.esriFieldTypeSingle,
													esriFieldType.esriFieldTypeDouble,
													esriFieldType.esriFieldTypeString,
													esriFieldType.esriFieldTypeDate,
													esriFieldType.esriFieldTypeOID);
            // ｺﾝﾄﾛｰﾙに設定
            if(cmbFlds.Length > 0) {
				listBox.Items.AddRange(cmbFlds);
                listBox.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// コンボボックスにFontFamilyを追加
        /// </summary>
        /// <param name="combobox">コンボボックス</param>
        private void AddComboItemFont(ComboBox combobox)
        {
            int indexCount = 0;

            combobox.BeginUpdate();            

            foreach (FontFamily fontFamily in FontFamily.Families)
            {
                if (fontFamily.IsStyleAvailable(FontStyle.Regular))
                {                    
                    combobox.Items.Add(fontFamily.Name);                    

                    indexCount++;
                }
            }

            combobox.EndUpdate();
        }



        /// <summary>
        /// コンボボックスにラベル配置を追加
        /// </summary>
        /// <param name="combobox">コンボボックス</param>        
        private void AddComboItemLabelPosition(ComboBox combobox)
        {
            IFeatureClass fc = (IFeatureClass)m_geoSelectedFLayer.FeatureClass;
            string[] position = null;

            if (fc.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            {
                position = new string[] { 
                    Properties.Resources.FormLabelSetting_Point_AboveCenter,    //上優先
                    Properties.Resources.FormLabelSetting_Point_BelowCenter,    //下優先
                    Properties.Resources.FormLabelSetting_Point_CenterLeft,     //左優先
                    Properties.Resources.FormLabelSetting_Point_CenterRight,    //右優先             
                    Properties.Resources.FormLabelSetting_Point_AboveLeft,      //左上優先
                    Properties.Resources.FormLabelSetting_Point_AboveRight,     //右上優先
                    Properties.Resources.FormLabelSetting_Point_BelowLeft,      //左下優先
                    Properties.Resources.FormLabelSetting_Point_BelowRight      //右下優先
                };
            }
            else if (fc.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
            {
                position = new string[] { 
                    Properties.Resources.FormLabelSetting_Line_Horizontal,      //水平
                    Properties.Resources.FormLabelSetting_Line_CurveOnTop,      //ラインに沿う
                    Properties.Resources.FormLabelSetting_Line_CurveAbove,      //ラインに沿って上方
                    Properties.Resources.FormLabelSetting_Line_CurveBelow       //ラインに沿って下方
                };
            }
            else if (fc.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
            {
                position = new string[] { 
                    Properties.Resources.FormLabelSetting_Polygon_Horizontal,   //水平
                    Properties.Resources.FormLabelSetting_Polygon_Straight      //直線
                };
            }

            for (int i = 0; i < position.Length; i++)
            {
                combobox.Items.Add(position[i]);
            }

            combobox.SelectedIndex = 0;
        }

        #endregion

                
        #region フォームの初期化

        /// <summary>
        /// フォームのリサイズ等をおこなう
        /// </summary>        
        private void ResizePointForm()
        {
            //同じラベルの配置方法グループを非表示
            groupBoxSameLabelMethod.Visible = false;

            //フォームのサイズを変更                
            this.Size = new Size(480, 615);

            //OK,適用,キャンセルボタンの位置を変更                
            buttonOK.Location = new System.Drawing.Point(208, 560);
            buttonAccept.Location = new System.Drawing.Point(296, 560);
            buttonCancel.Location = new System.Drawing.Point(384, 560);
        }



        /// <summary>
        /// 初期化
        /// チェックボックス:[このレイヤのラベルを表示する]
        /// </summary>
        private void InitCheckBoxShowLabel()
        {
            //ラベルを表示する設定がされている場合
            if (m_geoSelectedFLayer.DisplayAnnotation == true)
            {
                checkBoxShowLabel.Checked = true;
            }
            else
            {
                checkBoxShowLabel.Checked = false;
            }
        }


        /// <summary>
        /// フォームの初期化
        /// リストボックス:[フィールド一覧]
        /// </summary>
        private void InitListBoxAllFields() {
            //リストボックスにフィールドを追加
            AddListItemField(listBoxAllFields, m_geoSelectedFLayer as ITableFields);
        }

        
        /// <summary>
        /// フォームの初期化
        /// リストボックス:[ラベル フィールド]
        /// </summary>
        /// <param name="labelEngineLayerProp">ILabelEngineLayerProperties</param>
        private void InitListBoxLabelFields(ILabelEngineLayerProperties labelEngineLayerProp)
        {                        
            MatchCollection matchFieldNameCol = null;
            Match matchFiledNames = null;
            char[] chTrims = { '[', ']' };

            //シンプルな条件式の場合
            if (labelEngineLayerProp.IsExpressionSimple == true)
            {
                matchFieldNameCol =
                    Regex.Matches(labelEngineLayerProp.Expression, @"\[[^\]]+\]");
            }
            //高度な条件式の場合
            else if (labelEngineLayerProp.IsExpressionSimple == false)
            {
                matchFiledNames =
                Regex.Match(labelEngineLayerProp.Expression, @"\([^\)]+\)");

                matchFieldNameCol =
                        Regex.Matches(matchFiledNames.ToString(), @"\[[^\]]+\]");
            }
            
            for (int i = 0; i < matchFieldNameCol.Count; i++)
            {
                string labelFieldName = matchFieldNameCol[i].ToString().Trim(chTrims);
                labelFieldName = labelFieldName.ToUpper();

                for (int j = 0; j < listBoxAllFields.Items.Count; j++)
                {
                    Common.FieldComboItem fieldItem =
                        (Common.FieldComboItem)listBoxAllFields.Items[j];

                    if (labelFieldName.Equals(fieldItem.Field.Name.ToUpper()))
                    {
                        listBoxLabelFields.Items.Add(listBoxAllFields.Items[j]);
                        break;
                    }
                }
            }                
        }



        /// <summary>
        /// フォームの初期化
        /// グループ:[テキストシンボル]
        /// </summary>
        /// <param name="labelEngineLayerProp">ILabelEngineLayerProperties</param>
        private void InitGroupBoxTextSymbol(ILabelEngineLayerProperties labelEngineLayerProp)
        {
            //サイズ
            for (int i = 0; i < comboBoxTextSize.Items.Count; i++)
            {
                if (comboBoxTextSize.Items[i].ToString() == labelEngineLayerProp.Symbol.Size.ToString())
                {
                    comboBoxTextSize.SelectedIndex = i;
                    break;
                }                
            }
            //サイズが見つからなかった場合
            if (comboBoxTextSize.SelectedIndex == -1)
            {
                comboBoxTextSize.SelectedIndex = 3;
            }


            //色
            buttonTextColor.BackColor =
                System.Drawing.ColorTranslator.FromWin32(labelEngineLayerProp.Symbol.Color.RGB);
                        
            //colorDialog1.Color = buttonTextColor.BackColor;


            //コンボボックスにフォント一覧を追加
            AddComboItemFont(comboBoxTextFont);            

            for (int i = 0; i < comboBoxTextFont.Items.Count; i++)
            {
                if (comboBoxTextFont.Items[i].ToString() == labelEngineLayerProp.Symbol.Font.Name)
                {
                    comboBoxTextFont.SelectedIndex = i;
                    break;
                }
            }
            //フォントが見つからなかった場合
            if (comboBoxTextFont.SelectedIndex == -1)
            {
                comboBoxTextFont.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// フォームの初期化
        /// コンボボックス:[配置]
        /// ポイントの場合
        /// 
        /// ※ UIが異なるArcMapでの設定を、GISLightで正確に再現はできない。
        ///    再現方針
        ///    ①GISLightのUIと類似する設定の場合
        ///    　類似するコンボアイテムを選択する。
        ///    ②GISLightのUIと類似する設定ではない場合
        ///    　"右上優先"コンボアイテムを選択する。
        /// </summary>
        /// <param name="basicOverposterLayerProp">ILabelEngineLayerProperties</param>
        private void InitPointComboBoxLabelPosition
            (IBasicOverposterLayerProperties4 basicOverposterLayerProp)
        {            
            if (basicOverposterLayerProp.PointPlacementMethod ==
                esriOverposterPointPlacementMethod.esriAroundPoint)
            {
                IPointPlacementPriorities pointPlacementPriorities =
                basicOverposterLayerProp.PointPlacementPriorities;

                //上優先
                if (pointPlacementPriorities.AboveCenter == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 0;
                }
                //下優先
                else if (pointPlacementPriorities.BelowCenter == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 1;
                }
                //左優先
                else if (pointPlacementPriorities.CenterLeft == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 2;
                }
                //右優先
                else if (pointPlacementPriorities.CenterRight == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 3;
                }
                //左上優先
                else if (pointPlacementPriorities.AboveLeft == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 4;
                }
                //右上優先
                else if (pointPlacementPriorities.AboveRight == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 5;
                }
                //左下優先
                else if (pointPlacementPriorities.BelowLeft == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 6;
                }
                //右下優先
                else if (pointPlacementPriorities.BelowRight == 1)
                {
                    comboBoxLabelPosition.SelectedIndex = 7;
                }
                else
                {
                    comboBoxLabelPosition.SelectedIndex = 5;
                }
            }
            else
            {
                comboBoxLabelPosition.SelectedIndex = 5;
            }
        }


        /// <summary>
        /// フォームの初期化
        /// コンボボックス:[配置]
        /// ラインの場合
        /// 
        /// ※ UIが異なるArcMapでの設定を、GISLightで正確に再現はできない。
        ///    再現方針
        ///    ①GISLightのUIと類似する設定の場合
        ///    　類似するコンボアイテムを選択する。
        ///    ②GISLightのUIと類似する設定ではない場合
        ///    　"ラインに沿って上方"コンボアイテムを選択する。
        /// </summary>
        /// <param name="basicOverposterLayerProp">ILabelEngineLayerProperties</param>
        private void InitLineComboBoxLabelPosition
            (IBasicOverposterLayerProperties4 basicOverposterLayerProp)
        {
            ILineLabelPosition lineLabelPosition = 
                basicOverposterLayerProp.LineLabelPosition;

            //水平
            if (lineLabelPosition.Horizontal == true)
            {
                comboBoxLabelPosition.SelectedIndex = 0;
            }
            //ラインに沿う
            else if (lineLabelPosition.Parallel == true &&
                     lineLabelPosition.OnTop == true)
            {
                comboBoxLabelPosition.SelectedIndex = 1;
            }
            //ラインに沿って上方
            else if (lineLabelPosition.Parallel == true &&
                     lineLabelPosition.Above == true)
            {
                comboBoxLabelPosition.SelectedIndex = 2;
            }
            //ラインに沿って下方
            else if (lineLabelPosition.Parallel == true &&
                     lineLabelPosition.Below == true)
            {
                comboBoxLabelPosition.SelectedIndex = 3;
            }
            else
            {
                comboBoxLabelPosition.SelectedIndex = 2;
            }
        }


        /// <summary>
        /// フォームの初期化
        /// コンボボックス:[配置]
        /// ラインの場合
        /// 
        /// ※ UIが異なるArcMapでの設定を、GISLightで正確に再現はできない。
        ///    再現方針
        ///    ①GISLightのUIと類似する設定の場合
        ///    　類似するコンボアイテムを選択する。
        ///    ②GISLightのUIと類似する設定ではない場合
        ///    　"水平"コンボアイテムを選択する。
        /// </summary>
        /// <param name="basicOverposterLayerProp">ILabelEngineLayerProperties</param>
        private void InitPolygonComboBoxLabelPosition
            (IBasicOverposterLayerProperties4 basicOverposterLayerProp)
        {
            if (basicOverposterLayerProp.PolygonPlacementMethod ==
                esriOverposterPolygonPlacementMethod.esriAlwaysHorizontal)
            {
                comboBoxLabelPosition.SelectedIndex = 0;
            }
            else if (basicOverposterLayerProp.PolygonPlacementMethod ==
                esriOverposterPolygonPlacementMethod.esriAlwaysStraight)
            {
                comboBoxLabelPosition.SelectedIndex = 1;
            }
            else
            {
                comboBoxLabelPosition.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// フォームの初期化
        /// チェックボックス:[重なりを許可]
        /// </summary>
        /// <param name="basicOverposterLayerProp">ILabelEngineLayerProperties</param>
        /// <param name="overposterLayerProp">IOverposterLayerProperties2</param>
        private void InitCheckBoxAllowOverlap(
            IBasicOverposterLayerProperties4 basicOverposterLayerProp,
            IOverposterLayerProperties2 overposterLayerProp)
        {
            //重なりを許可する
            if (basicOverposterLayerProp.GenerateUnplacedLabels == true &&
                overposterLayerProp.TagUnplaced == false)
            {
                checkBoxAllowOverlap.Checked = true;
            }
            //重なりを許可しない
            else if (basicOverposterLayerProp.GenerateUnplacedLabels == true &&
                     overposterLayerProp.TagUnplaced == true)
            {
                checkBoxAllowOverlap.Checked = false;
            }
            else
            {
                checkBoxAllowOverlap.Checked = false;
            }
        }


        /// <summary>
        /// フォームの初期化
        /// グループ:[同じラベルの配置方法]
        /// </summary>
        /// <param name="basicOverposterLayerProp">IBasicOverposterLayerProperties4</param>
        private void InitGroupBoxSameLabelMethod(IBasicOverposterLayerProperties4 basicOverposterLayerProp)
        {
            //同じラベルを削除する場合
            if (m_geoSelectedFLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint ||
                basicOverposterLayerProp.NumLabelsOption == esriBasicNumLabelsOption.esriOneLabelPerName)
            {
                radioButtonDeleteSameLabel.Checked = true;
            }
            //フィーチャ毎に1つのラベルを配置する場合
            else if (basicOverposterLayerProp.NumLabelsOption == esriBasicNumLabelsOption.esriOneLabelPerShape)
            {
                radioButtonOneLabelByOneFeature.Checked = true;
            }
            else
            {
                radioButtonDeleteSameLabel.Checked = true;
            }
        }

        #endregion


        #region ラベルの設定

        #region ラベル条件式の作成

        /// <summary>
        /// ラベル条件式を返す。
        /// </summary>
        /// <param name="listbox">リストボックス</param>
        /// <returns>ラベル条件式</returns>
        public static string CreateLabelExpression(ListBox listbox)
        {                        
            string expression = "";

            StringBuilder sbExpressionTop = new StringBuilder();
            StringBuilder sbExpressionMiddle = new StringBuilder();
            StringBuilder sbExpressionBottom = new StringBuilder();

            sbExpressionTop.Append("Function FindLabel ( ");
            sbExpressionMiddle.Append("FindLabel = ");
            sbExpressionBottom.Append("End Function");


            int count = listbox.Items.Count;

            for (int i = 0; i < count; i++)
            {
                Common.FieldComboItem fieldItem = (Common.FieldComboItem)listbox.Items[i];

                sbExpressionTop.Append("[" + fieldItem.Field.Name + "]");
                sbExpressionMiddle.Append("[" + fieldItem.Field.Name + "]");

                if (i < count - 1)
                {
                    sbExpressionTop.Append(",");
                    sbExpressionMiddle.Append("& vbCrLf &");
                }
                else if (i == count - 1)
                {
                    sbExpressionTop.Append(")");
                }
            }

            expression = sbExpressionTop.ToString() + " " +
                         sbExpressionMiddle.ToString() + " " +
                         sbExpressionBottom.ToString();

            return expression;
        }        

        #endregion        


        #region ポイントの配置設定

        /// <summary>
        /// ラベル配置の設定（ポイント）を返す。
        /// </summary>
        /// <param name="selectedLabelPosition">ラベル配置の文字列</param>
        /// <returns>ラベル配置の設定（ポイント）</returns>
        public static IPointPlacementPriorities ReturnPointLabelPosition(            
            string selectedLabelPosition)
        {            
            IPointPlacementPriorities pointPlacementPriorities = null;

            //上優先
            if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_AboveCenter))
            {                
                pointPlacementPriorities = SetPointLabelPositionAboveCenter();
            }
            //下優先
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_BelowCenter))
            {                
                pointPlacementPriorities = SetPointLabelPositionBelowCenter();
            }
            //左優先
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_CenterLeft))
            {                
                pointPlacementPriorities = SetPointLabelPositionCenterLeft();
            }
            //右優先
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_CenterRight))
            {                
                pointPlacementPriorities = SetPointLabelPositionCenterRight();
            }
            //左上優先
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_AboveLeft))
            {                
                pointPlacementPriorities = SetPointLabelPositionAboveLeft();
            }
            //右上優先
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_AboveRight))
            {                
                pointPlacementPriorities = SetPointLabelPositionAboveRight();
            }
            //左下優先
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_BelowLeft))
            {                
                pointPlacementPriorities = SetPointLabelPositionBelowLeft();
            }
            //右下優先
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Point_BelowRight))
            {                
                pointPlacementPriorities = SetPointLabelPositionBelowRight();
            }

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 上優先 の設定を返す。        
        /// </summary>        
        private static IPointPlacementPriorities SetPointLabelPositionAboveCenter()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 1;
            pointPlacementPriorities.AboveLeft = 2;
            pointPlacementPriorities.AboveRight = 2;
            pointPlacementPriorities.BelowCenter = 3;
            pointPlacementPriorities.BelowLeft = 2;
            pointPlacementPriorities.BelowRight = 2;
            pointPlacementPriorities.CenterLeft = 2;
            pointPlacementPriorities.CenterRight = 2;            

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 下優先 の設定を返す。
        /// </summary>
        private static IPointPlacementPriorities SetPointLabelPositionBelowCenter()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 3;
            pointPlacementPriorities.AboveLeft = 2;
            pointPlacementPriorities.AboveRight = 2;
            pointPlacementPriorities.BelowCenter = 1;
            pointPlacementPriorities.BelowLeft = 2;
            pointPlacementPriorities.BelowRight = 2;
            pointPlacementPriorities.CenterLeft = 2;
            pointPlacementPriorities.CenterRight = 2;            

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 左優先 の設定を返す。
        /// </summary>
        private static IPointPlacementPriorities SetPointLabelPositionCenterLeft()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 2;
            pointPlacementPriorities.AboveLeft = 2;
            pointPlacementPriorities.AboveRight = 2;
            pointPlacementPriorities.BelowCenter = 2;
            pointPlacementPriorities.BelowLeft = 2;
            pointPlacementPriorities.BelowRight = 2;
            pointPlacementPriorities.CenterLeft = 1;
            pointPlacementPriorities.CenterRight = 3;            

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 右優先 の設定を返す。
        /// </summary>
        private static IPointPlacementPriorities SetPointLabelPositionCenterRight()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 2;
            pointPlacementPriorities.AboveLeft = 2;
            pointPlacementPriorities.AboveRight = 2;
            pointPlacementPriorities.BelowCenter = 2;
            pointPlacementPriorities.BelowLeft = 2;
            pointPlacementPriorities.BelowRight = 2;
            pointPlacementPriorities.CenterLeft = 3;
            pointPlacementPriorities.CenterRight = 1;

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 左上優先 の設定を返す。
        /// </summary>
        private static IPointPlacementPriorities SetPointLabelPositionAboveLeft()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 2;
            pointPlacementPriorities.AboveLeft = 1;
            pointPlacementPriorities.AboveRight = 2;
            pointPlacementPriorities.BelowCenter = 3;
            pointPlacementPriorities.BelowLeft = 2;
            pointPlacementPriorities.BelowRight = 3;
            pointPlacementPriorities.CenterLeft = 2;
            pointPlacementPriorities.CenterRight = 3;

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 右上優先 の設定を返す。
        /// </summary>
        private static IPointPlacementPriorities SetPointLabelPositionAboveRight()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 2;
            pointPlacementPriorities.AboveLeft = 2;
            pointPlacementPriorities.AboveRight = 1;
            pointPlacementPriorities.BelowCenter = 3;
            pointPlacementPriorities.BelowLeft = 3;
            pointPlacementPriorities.BelowRight = 2;
            pointPlacementPriorities.CenterLeft = 3;
            pointPlacementPriorities.CenterRight = 2;

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 左下優先 の設定を返す。
        /// </summary>
        private static IPointPlacementPriorities SetPointLabelPositionBelowLeft()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 3;
            pointPlacementPriorities.AboveLeft = 2;
            pointPlacementPriorities.AboveRight = 3;
            pointPlacementPriorities.BelowCenter = 2;
            pointPlacementPriorities.BelowLeft = 1;
            pointPlacementPriorities.BelowRight = 2;
            pointPlacementPriorities.CenterLeft = 2;
            pointPlacementPriorities.CenterRight = 3;

            return pointPlacementPriorities;
        }


        /// <summary>
        /// 右下優先 の設定を返す。
        /// </summary>
        private static IPointPlacementPriorities SetPointLabelPositionBelowRight()
        {
            IPointPlacementPriorities pointPlacementPriorities =
                new PointPlacementPrioritiesClass();

            pointPlacementPriorities.AboveCenter = 3;
            pointPlacementPriorities.AboveLeft = 3;
            pointPlacementPriorities.AboveRight = 2;
            pointPlacementPriorities.BelowCenter = 2;
            pointPlacementPriorities.BelowLeft = 2;
            pointPlacementPriorities.BelowRight = 1;
            pointPlacementPriorities.CenterLeft = 3;
            pointPlacementPriorities.CenterRight = 2;

            return pointPlacementPriorities;
        }

        #endregion


        #region ラインの配置設定

        /// <summary>
        /// ラベル配置の設定（ライン）を返す。
        /// </summary>
        /// <param name="selectedLabelPosition">ラベル配置の文字列</param>
        /// <returns>ラベル配置の設定（ライン）</returns>
        public static ILineLabelPosition ReturnLineLabelPosition(
            string selectedLabelPosition)
        {            
            ILineLabelPosition lineLabelPosition = null;

            //水平
            if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Line_Horizontal))
            {                
                lineLabelPosition = SetLineLabelPositionHorizontal();
            }
            //ラインに沿う
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Line_CurveOnTop))
            {                
                lineLabelPosition = SetLineLabelPositionCurvedOnTop();
            }
            //ラインに沿って上方
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Line_CurveAbove))
            {                
                lineLabelPosition = SetLineLabelPositionCurvedAbove();
            }
            //ラインに沿って下方
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Line_CurveBelow))
            {                
                lineLabelPosition = SetLineLabelPositionCurvedBelow();
            }

            return lineLabelPosition;
        }


        /// <summary>
        /// 水平 の設定を返す。
        /// </summary>        
        private static ILineLabelPosition SetLineLabelPositionHorizontal()
        {
            ILineLabelPosition lineLabelPosition =
                new LineLabelPositionClass();

            lineLabelPosition.Above = false;
            lineLabelPosition.AtEnd = false;
            lineLabelPosition.AtStart = false;
            lineLabelPosition.Below = false;
            lineLabelPosition.Horizontal = true;
            lineLabelPosition.InLine = true;
            lineLabelPosition.Left = false;
            lineLabelPosition.Offset = 0;
            lineLabelPosition.OnTop = true;
            lineLabelPosition.Parallel = false;
            lineLabelPosition.Perpendicular = false;
            lineLabelPosition.ProduceCurvedLabels = false;
            lineLabelPosition.Right = false;
            
            return lineLabelPosition;
        }


        /// <summary>
        /// ラインに沿う の設定を返す。
        /// </summary>        
        private static ILineLabelPosition SetLineLabelPositionCurvedOnTop()
        {
            ILineLabelPosition lineLabelPosition =
                new LineLabelPositionClass();

            lineLabelPosition.Above = false;
            lineLabelPosition.AtEnd = false;
            lineLabelPosition.AtStart = false;
            lineLabelPosition.Below = false;
            lineLabelPosition.Horizontal = false;
            lineLabelPosition.InLine = true;
            lineLabelPosition.Left = false;
            lineLabelPosition.Offset = 0;
            lineLabelPosition.OnTop = true;
            lineLabelPosition.Parallel = true;
            lineLabelPosition.Perpendicular = false;
            lineLabelPosition.ProduceCurvedLabels = false;
            lineLabelPosition.Right = false;
            
            return lineLabelPosition;
        }


        /// <summary>
        /// ラインに沿って上方 の設定を返す。
        /// </summary>        
        private static ILineLabelPosition SetLineLabelPositionCurvedAbove()
        {
            ILineLabelPosition lineLabelPosition =
                new LineLabelPositionClass();

            lineLabelPosition.Above = true;
            lineLabelPosition.AtEnd = false;
            lineLabelPosition.AtStart = false;
            lineLabelPosition.Below = false;
            lineLabelPosition.Horizontal = false;
            lineLabelPosition.InLine = true;
            lineLabelPosition.Left = false;
            lineLabelPosition.Offset = 0;
            lineLabelPosition.OnTop = false;
            lineLabelPosition.Parallel = true;
            lineLabelPosition.Perpendicular = false;
            lineLabelPosition.ProduceCurvedLabels = false;
            lineLabelPosition.Right = false;
            
            return lineLabelPosition;
        }


        /// <summary>
        /// ラインに沿って下方 の設定を返す。
        /// </summary>        
        private static ILineLabelPosition SetLineLabelPositionCurvedBelow()
        {
            ILineLabelPosition lineLabelPosition =
                new LineLabelPositionClass();

            lineLabelPosition.Above = false;
            lineLabelPosition.AtEnd = false;
            lineLabelPosition.AtStart = false;
            lineLabelPosition.Below = true;
            lineLabelPosition.Horizontal = false;
            lineLabelPosition.InLine = true;
            lineLabelPosition.Left = false;
            lineLabelPosition.Offset = 0;
            lineLabelPosition.OnTop = false;
            lineLabelPosition.Parallel = true;
            lineLabelPosition.Perpendicular = false;
            lineLabelPosition.ProduceCurvedLabels = false;
            lineLabelPosition.Right = false;
            
            return lineLabelPosition;
        }

        #endregion


        #region ポリゴンの配置設定

        /// <summary>
        /// ラベル配置の設定（ポリゴン）を返す。
        /// </summary>        
        /// <param name="selectedLabelPosition">ラベル配置の文字列</param>
        /// <returns>ラベル配置の設定（ポリゴン）</returns>
        public static /*void*/ esriOverposterPolygonPlacementMethod ReturnPolygonLabelPosition(
            string selectedLabelPosition)
        {            
            esriOverposterPolygonPlacementMethod polygonPlacementMethod =
                esriOverposterPolygonPlacementMethod.esriAlwaysHorizontal;

            //水平
            if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Polygon_Horizontal))
            {         
                polygonPlacementMethod =
                    esriOverposterPolygonPlacementMethod.esriAlwaysHorizontal;
            }
            //直線
            else if (selectedLabelPosition.Equals(Properties.Resources.
                FormLabelSetting_Polygon_Straight))
            {             
                polygonPlacementMethod =
                    esriOverposterPolygonPlacementMethod.esriAlwaysStraight;
            }

            return polygonPlacementMethod;
        }

        #endregion

        #endregion
    }
}
