using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;

using ESRIJapan.GISLight10.Common;
using ESRIJapan.GISLight10.Common.Calculator;


namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ジオメトリ演算のUIとなるクラス。
    /// </summary>
    public partial class FormGeometryCalculate : Form
    {
        /// <summary>
        /// 演算の種類を定義
        /// </summary>
        protected enum CalculateProperty
        {
            /// <summary>
            /// 面積
            /// </summary>
            Area,

            /// <summary>
            /// 周長
            /// </summary>
            Perimeter,

            /// <summary>
            /// 長さ
            /// </summary>
            Length
        };

        // ﾒｲﾝ･ﾌｫｰﾑ
        private Ui.MainForm mainFrm;
        // 別名表示設定
        private bool	_blnUseAlias;

        /// <summary>
        /// 設定ファイルから読み込んだ小数点以下の桁数
        /// </summary>
        protected int loadDecimalPlaces = -1;


        /// <summary>
        /// クラスのインスタンス。各種変数、コンポーネットの初期化・設定をおこなう。
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormGeometryCalculate(IMapControl3 mapControl)
        {
            Logger.Info("面積・長さ計算ダイアログを初期化します");

            InitializeComponent();

            initForm(mapControl, null);
        }


        /// <summary>
        /// クラスのインスタンス。各種変数、コンポーネットの初期化・設定をおこなう。
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        /// <param name="pTargetLayer">処理対象のレイヤ</param>
        public FormGeometryCalculate(IMapControl3 mapControl, IFeatureLayer pTargetLayer)
        {
            Logger.Info("面積・長さ計算ダイアログを初期化します");

            InitializeComponent();

            initForm(mapControl, pTargetLayer);
        }


        /// <summary>
        /// フォームの各種コンポーネントを設定する
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        /// <param name="pTargetLayer">処理対象のレイヤ</param>
        protected void initForm(IMapControl3 mapControl, IFeatureLayer pTargetLayer)
        {
            // 別名表示設定を取得
            this._blnUseAlias = (Control.FromHandle(((IntPtr)mapControl.hWnd)).FindForm() as MainForm).IsUseFieldAlias;
            IntPtr ptr2 = (System.IntPtr)mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();

            StringBuilder buff = null;
            LayerManager layManager;
            List<IFeatureLayer> layers = null;
            LayerComboItem layeritem;
            IFeatureLayer pSelectedLayer;
            esriGeometryType currentLayerType;
            ConstComboItem propitem;
            CalculateProperty currentProperty;

            try
            {
                layManager = new LayerManager();
                layers = layManager.GetFeatureLayers(mapControl.Map);

                setupComboBoxLayer(layers, pTargetLayer);

                layeritem = (LayerComboItem)comboLayer.SelectedItem;
                pSelectedLayer = (IFeatureLayer)layeritem.Layer;
                currentLayerType = pSelectedLayer == null ?
                        esriGeometryType.esriGeometryNull : pSelectedLayer.FeatureClass.ShapeType;
                setupComboBoxProperty(currentLayerType);
                setupComboBoxField(pSelectedLayer);
                propitem = (ConstComboItem)comboProperty.SelectedItem;
                currentProperty = (CalculateProperty)propitem.Id;
                setupComboBoxUnit(currentProperty);

                GeometryCalculatorSettings settings;

                // 小数点以下桁数の設定
                try
                {
                    settings = new GeometryCalculatorSettings();
                    try
                    {
                        loadDecimalPlaces = settings.DecimalPlaces;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("面積・長さ計算ダイアログの初期化に失敗しました", ex);
                        buff = new StringBuilder();
                        buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead);
                        buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("面積・長さ計算ダイアログの初期化に失敗しました", ex);
                    buff = new StringBuilder();
                    buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist);
                    buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                }
                if (loadDecimalPlaces == -1)
                {
                    // 設定ファイルの読み込みに失敗した場合
                    throw new Exception(buff != null ? buff.ToString() : null);
                }

                nudDecimalPlaces.Value = loadDecimalPlaces;
            }
            catch (Exception ex)
            {
                setupEnabled(false);
                comboLayer.Enabled = false;

                Logger.Error("面積・長さ計算ダイアログの初期化に失敗しました", ex);

                if (buff == null)
                {
                    buff = new StringBuilder();
                    buff.Append(Properties.Resources.FormGeometryCalculate_ERROR_Initialize);
                    throw new Exception(buff.ToString());
                }

                throw;
            }
        }

        /// <summary>
        /// レイヤコンボボックスの設定
        /// </summary>
        /// <param name="pLayers">表示中のマップに追加されているフィーチャレイヤ一覧</param>
        /// <param name="pTargetLayer">処理対象のレイヤ。ない場合はnull</param>
        protected void setupComboBoxLayer(IList<IFeatureLayer> pLayers, IFeatureLayer pTargetLayer)
        {
            int count = 0, targetItemIndex = -1, hashCode = 0;
            LayerComboItem item;
            esriGeometryType currentLayerType;

            comboLayer.Items.Clear();

            if (pTargetLayer != null)
            {
                hashCode = pTargetLayer.GetHashCode();
            }

            foreach (IFeatureLayer pLayer in pLayers)
            {
                currentLayerType = pLayer.FeatureClass.ShapeType;
                if (currentLayerType == esriGeometryType.esriGeometryPolygon ||
                    currentLayerType == esriGeometryType.esriGeometryPolyline)
                {
                    item = new LayerComboItem(pLayer);
                    comboLayer.Items.Add(item);
                    if (pLayer.GetHashCode() == hashCode)
                    {
                        targetItemIndex = count;
                    }
                    count++;
                }
            }

            if (targetItemIndex != -1)
            {
                comboLayer.Enabled = false;
            }
            
            if (comboLayer.Items.Count > 0)
            {
                comboLayer.SelectedIndex = targetItemIndex == -1 ? 0 : targetItemIndex;
            }
        }


        /// <summary>
        /// 座標系テキストボックスの設定
        /// </summary>
        /// <param name="pSelectedFeatureLayer">レイヤコンボボックスで選択されているレイヤ</param>
        protected void setupTextBoxSpatialRef(IFeatureLayer pSelectedFeatureLayer)
        {
            int index;
            string fieldName;
            IFeatureClass pFeatureClass;
            IField pField;
            ISpatialReference pSR;

            pFeatureClass = pSelectedFeatureLayer.FeatureClass;
            fieldName = pFeatureClass.ShapeFieldName;
            index = pFeatureClass.FindField(fieldName);

            pField = pFeatureClass.Fields.get_Field(index);
            pSR = pField.GeometryDef.SpatialReference;
            txtSr.Tag = pSR;

            txtSr.Text = pSR.Name;
            if (pSR is IProjectedCoordinateSystem)
            {
                // 空間参照が投影座標系の場合
                lblErrorSr.Text = string.Empty;
                setupEnabled(true);
            }
            else
            {
                // 空間参照が投影座標系以外の場合
                lblErrorSr.Text = Properties.Resources.FormGeometryCalculate_WARNING_SpatialReference;
                setupEnabled(false);
            }
        }


        /// <summary>
        /// プロパティコンボボックスの設定
        /// </summary>
        /// <param name="geometryType">レイヤコンボボックスで選択されているレイヤの図形タイプ</param>
        protected void setupComboBoxProperty(esriGeometryType geometryType)
        {
            if (comboProperty.Tag != null &&
                    (esriGeometryType)comboProperty.Tag == geometryType)
            {
                return;
            }

            comboProperty.Items.Clear();
            comboProperty.Tag = geometryType;

            switch (geometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    comboProperty.Items.Add(new ConstComboItem((int)CalculateProperty.Area,
                                        Properties.Resources.FormGeometryCalculate_Property_Area));
                    comboProperty.Items.Add(new ConstComboItem((int)CalculateProperty.Perimeter,
                                        Properties.Resources.FormGeometryCalculate_Property_Perimeter));
                    break;

                case esriGeometryType.esriGeometryPolyline:
                    comboProperty.Items.Add(new ConstComboItem((int)CalculateProperty.Length,
                                        Properties.Resources.FormGeometryCalculate_Property_Length));
                    break;
            }

            if (comboProperty.Items.Count > 0)
            {
                comboProperty.SelectedIndex = 0;
                comboProperty.Enabled = txtSr.Tag is IProjectedCoordinateSystem;
            }
            else
            {
                comboProperty.Enabled = false;
            }
        }


        /// <summary>
        /// フィールドコンボボックスの設定
        /// </summary>
        /// <param name="pSelectedFeatureLayer">レイヤコンボボックスで選択されているレイヤ</param>
        protected void setupComboBoxField(IFeatureLayer pSelectedFeatureLayer)
        {
            comboField.Items.Clear();

            // 対象ﾌｨｰﾙﾄﾞ･ｱｲﾃﾑを取得
            Common.FieldComboItem[]	cmbFlds = this.mainFrm.GetFieldItems(pSelectedFeatureLayer, true, true, true, true, 
													esriFieldType.esriFieldTypeInteger,
													esriFieldType.esriFieldTypeSingle,
													esriFieldType.esriFieldTypeDouble,
													esriFieldType.esriFieldTypeString);
			this.comboField.Items.AddRange(cmbFlds);
            if (comboField.Items.Count > 0)
            {
                comboField.SelectedIndex = 0;
                comboField.Enabled = txtSr.Tag is IProjectedCoordinateSystem;
            }
            else
            {
                comboField.Enabled = false;
            }
        }


        /// <summary>
        /// 単位コンボボックスの設定をする
        /// </summary>
        /// <param name="selectedProperty">レイヤコンボボックスで選択されているレイヤの図形タイプ</param>
        protected void setupComboBoxUnit(CalculateProperty selectedProperty)
        {
            if (comboUnit.Tag != null &&
                    (CalculateProperty)comboUnit.Tag == selectedProperty)
            {
                return;
            }

            comboUnit.Items.Clear();
            comboUnit.Tag = selectedProperty;

            switch (selectedProperty)
            {
                case CalculateProperty.Area:
                    comboUnit.Items.Add(new ConstComboItem((int)Unit.A,
                                        Properties.Resources.FormGeometryCalculate_Unit_A));
                    comboUnit.Items.Add(new ConstComboItem((int)Unit.HA,
                                        Properties.Resources.FormGeometryCalculate_Unit_Ha));
                    comboUnit.Items.Add(new ConstComboItem((int)Unit.KMETER2,
                                        Properties.Resources.FormGeometryCalculate_Unit_SqKm));
                    comboUnit.Items.Add(new ConstComboItem((int)Unit.METER2,
                                        Properties.Resources.FormGeometryCalculate_Unit_SqM));
                    break;

                case CalculateProperty.Perimeter:
                case CalculateProperty.Length:
                    comboUnit.Items.Add(new ConstComboItem((int)Unit.KMETER,
                                        Properties.Resources.FormGeometryCalculate_Unit_Km));
                    comboUnit.Items.Add(new ConstComboItem((int)Unit.METER,
                                        Properties.Resources.FormGeometryCalculate_Unit_M));
                    comboUnit.Items.Add(new ConstComboItem((int)Unit.CMETER,
                                        Properties.Resources.FormGeometryCalculate_Unit_Cm));
                    break;
            }

            if (comboUnit.Items.Count > 0)
            {
                comboUnit.SelectedIndex = 1;
                comboUnit.Enabled = txtSr.Tag is IProjectedCoordinateSystem;
            }
            else
            {
                comboUnit.Enabled = false;
            }
        }


        /// <summary>
        /// 単位の略名をテキストフィールドに追加チェックボックスの設定
        /// </summary>
        /// <param name="pSelectedField"></param>
        protected void setupCheckBoxAppendUnit(IField pSelectedField)
        {
            chkAppendUnit.Enabled = txtSr.Tag is IProjectedCoordinateSystem &&
                                    pSelectedField.Type == esriFieldType.esriFieldTypeString;
        }


        /// <summary>
        /// 小数点以下の桁数を指定するチェックボックスの設定
        /// </summary>
        /// <param name="pSelectedField"></param>
        protected void setupCheckBoxPrecision(IField pSelectedField)
        {
            chkPrecision.Enabled = txtSr.Tag is IProjectedCoordinateSystem &&
                                    pSelectedField.Type == esriFieldType.esriFieldTypeString;
            nudDecimalPlaces.Enabled = chkPrecision.Enabled;
        }


        /// <summary>
        /// 選択レコードのみ演算するチェックボックスの設定
        /// </summary>
        /// <param name="pSelectedFeatureLayer"></param>
        protected void setupCheckBoxSelectionSet(IFeatureLayer pSelectedFeatureLayer)
        {
            IFeatureSelection pFeatureSelection;

            pFeatureSelection = (IFeatureSelection)pSelectedFeatureLayer;
            if (pFeatureSelection.SelectionSet != null && pFeatureSelection.SelectionSet.Count > 0)
            {
                chkUseSelectionSet.Enabled = txtSr.Tag is IProjectedCoordinateSystem;
                chkUseSelectionSet.Checked = true;
            }
            else
            {
                chkUseSelectionSet.Enabled = false;
                chkUseSelectionSet.Checked = false;
            }
        }


        /// <summary>
        /// 主要コンポーネントのEnable値を設定する
        /// </summary>
        /// <param name="blnEnabled">設定するEnable値の値</param>
        protected void setupEnabled(bool blnEnabled)
        {
            btnOk.Enabled = blnEnabled;
            comboField.Enabled = blnEnabled;
            comboProperty.Enabled = blnEnabled;
            comboUnit.Enabled = blnEnabled;
            chkAppendUnit.Enabled = blnEnabled;
            chkPrecision.Enabled = blnEnabled;
            chkUseSelectionSet.Enabled = blnEnabled;
            nudDecimalPlaces.Enabled = blnEnabled;
        }


        /// <summary>
        /// フォーム上の選択されている内容で、ジオメトリ演算をおこなう
        /// </summary>
        /// <returns>エラーのときはfalse</returns>
        protected bool Calculate()
        {
            bool blnResult = true;
            string result;
            IGeometryCalculator calculator = null;
            IGeometryCalculatorUnit calcUnit = null;
            LayerComboItem layeritem;
            IFeatureLayer pSelectedLayer;
            FieldComboItem fielditem;
            IField pSelectedField;
            ConstComboItem unititem;
            Unit selectedUnit;
            ConstComboItem propitem;
            CalculateProperty selectedProperty;
            //FormProgressManager progressForm = null;
            
            try
            {
                layeritem = (LayerComboItem)comboLayer.SelectedItem;
                pSelectedLayer = (IFeatureLayer)layeritem.Layer;
                fielditem = (FieldComboItem)comboField.SelectedItem;
                pSelectedField = fielditem.Field;
                unititem = (ConstComboItem)comboUnit.SelectedItem;
                selectedUnit = (Unit)unititem.Id;
                propitem = (ConstComboItem)comboProperty.SelectedItem;
                selectedProperty = (CalculateProperty)propitem.Id;

                switch (selectedProperty)
                {
                    case CalculateProperty.Area:
                        calcUnit = new AreaCalculatorUnit();
                        break;

                    case CalculateProperty.Length:
                    case CalculateProperty.Perimeter:
                        calcUnit = new LengthCalculatorUnit();
                        break;
                }


                if (chkUseSelectionSet.Enabled == true &&
                    chkUseSelectionSet.Checked == true)
                {
                    //calculator = new GeometryCalculator2();
                    calculator = new GeometryCalculator2(this);
                }
                else
                {
                    //calculator = new GeometryCalculator();
                    calculator = new GeometryCalculator(this);
                }

                if (calculator == null || calcUnit == null)
                {
                    // プログラムのミス
                    throw new Exception("予期しないエラー");
                }

                calculator.TargetLayer = pSelectedLayer;
                calculator.TargetFieldName = pSelectedField.Name;
                calculator.ToUnit = selectedUnit;
                calculator.AppendUnit = chkAppendUnit.Checked;
                if (chkPrecision.Checked == true)
                {
                    calculator.DecimalPrecision = (int)nudDecimalPlaces.Value;
                }

                //progressForm = new FormProgressManager();
                //progressForm.Owner = this;
                //progressForm.SetTitle(this);
                //progressForm.SetMessage(string.Empty);
                //progressForm.TopLevel = true;
                //progressForm.Opacity = 0.0;
                //progressForm.Show();
                //System.Drawing.Point loc = new System.Drawing.Point(
                //    this.Location.X + (this.Width / 2) - (progressForm.Width / 2),
                //    this.Location.Y + (this.Height / 2) - (progressForm.Height / 2));
                //progressForm.Location = loc;
                //progressForm.Opacity = 1.0;
                //Application.DoEvents();

                this.Enabled = false;

                result = calculator.Calculate(calcUnit);

                //progressForm.Close();
                //progressForm.Dispose();
                //progressForm = null;

                if (result != null)
                {
                    blnResult = false;
                    MessageBoxManager.ShowMessageBoxWarining(this, result);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("ジオメトリ演算に失敗しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormGeometryCalculate_ERROR);

                blnResult = false;
            }
            finally
            {
                this.Enabled = true;
                //if (progressForm != null)
                //{
                //    progressForm.Close();
                //    progressForm.Dispose();
                //}

            }

            return blnResult;
        }


        /// <summary>
        /// OKボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            StringBuilder buff;
            GeometryCalculatorSettings settings;

            if (loadDecimalPlaces != nudDecimalPlaces.Value)
            {
                // 小数点以下の桁数を設定ファイルに保存する
                try
                {
                    settings = new GeometryCalculatorSettings();

                    try
                    {
                        settings.DecimalPlaces = (int)nudDecimalPlaces.Value;
                        settings.SaveSettings();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("設定の保存に失敗しました", ex);
                        buff = new StringBuilder();
                        buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantWrite);
                        buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        MessageBoxManager.ShowMessageBoxError(this, buff.ToString());
                    }
                }
                catch (Exception ex)
                {
                        Logger.Error("設定の保存に失敗しました", ex);
                        buff = new StringBuilder();
                        buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist);
                        buff.Append(Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                        MessageBoxManager.ShowMessageBoxError(this, buff.ToString());
                }
            }

            DialogResult result;
            result = MessageBoxManager.ShowMessageBoxQuestion2(this,
                            Properties.Resources.FormGeometryCalculate_WARNING);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            // ジオメトリ演算をおこなう
            if (Calculate() == true)
            {
                // 演算成功時はダイアログを閉じる
                this.Close();
            }
        }

        
        /// <summary>
        /// キャンセルボタンがクリックされたときの処理。フォームを閉じる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// レイヤコンボボックスの選択が変更されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            LayerComboItem item;
            IFeatureLayer pSelectedLayer;
            esriGeometryType currentLayerType;

            setupEnabled(true);
            lblErrorLayer.Text = string.Empty;

            // 選択されているレイヤを取得する
            item = (LayerComboItem)comboLayer.SelectedItem;
            pSelectedLayer = (IFeatureLayer)item.Layer;
            // 選択されたレイヤのレイヤ種別を取得
            currentLayerType = pSelectedLayer.FeatureClass.ShapeType;

            // 空間参照テキストボックスの設定
            setupTextBoxSpatialRef(pSelectedLayer);
            // フィールドコンボボックスの設定
            setupComboBoxField(pSelectedLayer);
            // プロパティコンボボックスの設定
            setupComboBoxProperty(currentLayerType);
            // 選択レコードのみ演算チェックボックスの設定
            setupCheckBoxSelectionSet(pSelectedLayer);

            if (comboField.Items.Count == 0)
            {
                // 更新できるフィールドがひとつもないレイヤの場合
                setupEnabled(false);
                lblErrorLayer.Text = Properties.Resources.FormGeometryCalculate_WARNING_Field;
            }
        }


        /// <summary>
        /// フィールドコンボボックスの選択が変更されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboField_SelectedIndexChanged(object sender, EventArgs e)
        {
            FieldComboItem item;
            IField pSelectedField;

            item = (FieldComboItem)comboField.SelectedItem;
            pSelectedField = item.Field; ;

            // 小数点以下の桁数チェックボックスの設定
            setupCheckBoxAppendUnit(pSelectedField);
            // 単位の略名チェックボックスの設定
            setupCheckBoxPrecision(pSelectedField);
        }

        /// <summary>
        /// プロパティコンボボックスの選択が変更されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboProperty_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConstComboItem item;
            CalculateProperty prop;

            item = (ConstComboItem)comboProperty.SelectedItem;
            prop = (CalculateProperty)item.Id;

            setupComboBoxUnit(prop);
        }
    }
}
