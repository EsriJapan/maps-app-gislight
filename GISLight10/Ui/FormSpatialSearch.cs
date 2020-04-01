using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// 空間検索機能
    /// </summary>
    /// <history>
    ///  2010-11-01新規作成 
    /// </history>
    public partial class FormSpatialSearch : Form
    {
        /// <summary>
        /// 空間検索の処理をおこなうマップ
        /// </summary>
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl;
        // ﾒｲﾝ･ﾌｫｰﾑ
        private Ui.MainForm m_mainFrm;

        /// <summary>
        /// 空間検索用
        /// </summary>
        private int m_method;
        private int m_spatialRel;
        private bool m_isSelectedFeature;
        private int m_targetFeatureLayerCount;

        /// <summary>
        /// スレッド用
        /// </summary>
        private Thread threadTask = null;
        private delegate void RethrowExceptionDelegate(Exception ex);
        private delegate void RethrowCOMExceptionDelegate(COMException comex);

        /// <summary>
        /// プログレスバーフォーム
        /// </summary>
        private FormProgressManager frmProgress = null;

        /// <summary>
        /// アイテム変更イベント
        /// </summary>
        private bool changeSourceLayer = false;
        private bool changeSpatialRel = false;
        private bool changeListTargetLayer = false;
        private bool chandeSetOperation = false;

        /// <summary>
        /// クラスのコンストラクタ。各種変数、UIの初期化を行う
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        public FormSpatialSearch(ESRI.ArcGIS.Controls.IMapControl3 mapControl)
        {
            List<IFeatureLayer> pLayers = null;

            try
            {
                Logger.Info("空間検索ダイアログを表示します");
                this.m_mapControl = mapControl;

                InitializeComponent();

                LayerManager layManager;

                // 処理対象となるレイヤの取得
                layManager = new LayerManager();
                pLayers = layManager.GetFeatureLayers(m_mapControl.Map);

                // コンボボックスの設定
                setupComboSpatialRel();
                setupComboSetOperation();
                setupComboLayer(pLayers);
                setupCheckboxList(pLayers);

				// ﾒｲﾝ･ﾌｫｰﾑを取得
				Control	ctlMain = System.Windows.Forms.Control.FromHandle((System.IntPtr)mapControl.hWnd);
				m_mainFrm = (Ui.MainForm)ctlMain.FindForm();
            }
            catch (COMException ex)
            {
                Logger.Error("検索処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_Initialize);
            }
            catch (Exception ex)
            {
                Logger.Error("検索処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_Initialize);
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(pLayers);
            }
        }


        /// <summary>
        /// 位置関係を指定するコンボボックスの設定
        /// </summary>
        protected void setupComboSpatialRel()
        {
            // コンボボックスのアイテムの設定
            comboSpatialRel.Items.Clear();

            // Touches (と重なる)
            comboSpatialRel.Items.Add(new ConstComboItem(
                    (int)esriSpatialRelEnum.esriSpatialRelIntersects,
                    Properties.Resources.FormSpatialSearch_SR_TOUCHES));
            // Within (を含む)
            comboSpatialRel.Items.Add(new ConstComboItem(
                    (int)esriSpatialRelEnum.esriSpatialRelWithin,
                    Properties.Resources.FormSpatialSearch_SR_WITHIN));
            // Contains (に含まれる)
            comboSpatialRel.Items.Add(new ConstComboItem(
                    (int)esriSpatialRelEnum.esriSpatialRelContains,
                    Properties.Resources.FormSpatialSearch_SR_CONTAINS));

            comboSpatialRel.SelectedIndex = 0;
        }

        /// <summary>
        /// 選択方法を指定するコンボボックスの設定
        /// </summary>
        protected void setupComboSetOperation()
        {
            // コンボボックスのアイテムの設定
            comboSetOperation.Items.Clear();

            // 新規に選択
            comboSetOperation.Items.Add(new ConstComboItem(
                    (int)esriSelectionResultEnum.esriSelectionResultNew,
                    Properties.Resources.FormSpatialSearch_SO_NONE));
            // 選択に追加
            comboSetOperation.Items.Add(new ConstComboItem(
                    (int)esriSelectionResultEnum.esriSelectionResultAdd,
                    Properties.Resources.FormSpatialSearch_SO_UNION));
            // 選択から削除
            comboSetOperation.Items.Add(new ConstComboItem(
                    (int)esriSelectionResultEnum.esriSelectionResultSubtract,
                    Properties.Resources.FormSpatialSearch_SO_DIFFERENCE));
            // 選択から絞込み
            comboSetOperation.Items.Add(new ConstComboItem(
                    (int)esriSelectionResultEnum.esriSelectionResultAnd,
                    Properties.Resources.FormSpatialSearch_SO_INTERSECTION));

            comboSetOperation.SelectedIndex = 0;
        }


        private void setupComboLayer(IList<IFeatureLayer> pLayers)
        {
            comboSourceLayer.Items.Clear();

            foreach (IFeatureLayer pLayer in pLayers)
            {
                comboSourceLayer.Items.Add(new LayerComboItem(pLayer));
            }
            if (comboSourceLayer.Items.Count > 0)
            {
                comboSourceLayer.SelectedIndex = 0;
            }
        }


        private void setupCheckboxList(IList<IFeatureLayer> pLayers)
        {
            checkedListTargetLayer.Items.Clear();

            foreach(IFeatureLayer pLayer in pLayers)
            {
                checkedListTargetLayer.Items.Add(new LayerComboItem(pLayer));
            }
        }



        private ISpatialFilter createSpatialFilter(IFeatureLayer pSourceLayer, esriSpatialRelEnum spatialRel, bool isSelectedFeature)
        {
            ISpatialFilter pSpatialFilter = null;
            IFeatureSelection pFeatureSelection;
            ISelectionSet pSelectionSet;
            IEnumGeometryBind pEnumGeometryBind = null;
            IEnumGeometry pEnumGeometry = null;
            IGeometryFactory3 pGeometryFactory = null;
            IGeometry pGeometry = null;

            try
            {
                // 検索条件となる条件の取得
                pFeatureSelection = (IFeatureSelection)pSourceLayer;
                pSelectionSet = pFeatureSelection.SelectionSet;

                if (isSelectedFeature == true)
                {
                    // 選択フィーチャで検索条件を作る場合
                    pEnumGeometryBind = new EnumFeatureGeometryClass();
                    pEnumGeometryBind.BindGeometrySource(null, pSelectionSet);
                    pEnumGeometry = (IEnumGeometry)pEnumGeometryBind;

                    pGeometryFactory = new GeometryEnvironmentClass();
                    pGeometry = pGeometryFactory.CreateGeometryFromEnumerator(pEnumGeometry);
                }
                else
                {
                    // 全フィーチャで検索条件を作る場合
                    IWorkspace pWorkspace;
                    IQueryFilter pQueryFilter;
                    IFeatureLayerDefinition pDefinintionLayer;

                    pWorkspace = LayerManager.getWorkspace(pSourceLayer);
                    pDefinintionLayer = (IFeatureLayerDefinition)pSourceLayer;
                    pQueryFilter = new QueryFilterClass();
                    pQueryFilter.WhereClause = pDefinintionLayer.DefinitionExpression;
                    pSelectionSet = pSourceLayer.FeatureClass.Select(pQueryFilter, esriSelectionType.esriSelectionTypeHybrid, esriSelectionOption.esriSelectionOptionNormal, pWorkspace);

                    pEnumGeometryBind = new EnumFeatureGeometryClass();
                    pEnumGeometryBind.BindGeometrySource(null, pSelectionSet);
                    pEnumGeometry = (IEnumGeometry)pEnumGeometryBind;

                    pGeometryFactory = new GeometryEnvironmentClass();
                    pGeometry = pGeometryFactory.CreateGeometryFromEnumerator(pEnumGeometry);
                }

                pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.SpatialRel = spatialRel;
                pSpatialFilter.GeometryField = pSourceLayer.FeatureClass.ShapeFieldName;

                return pSpatialFilter;
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(pEnumGeometry);
                ComReleaser.ReleaseCOMObject(pGeometryFactory);
            }
        }

        #region ProgressDialogのクラスを使って書き直し
        private void selectFeature2()
        {

            // 検索条件になるパラメータの取得
            ConstComboItem methodItem = (ConstComboItem)comboSetOperation.SelectedItem;

            ConstComboItem relItem = (ConstComboItem)comboSpatialRel.SelectedItem;

            LayerComboItem sourcelayerItem = (LayerComboItem)comboSourceLayer.SelectedItem;
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourcelayerItem.Layer;

            IFeatureLayer[] featureLayerList = new IFeatureLayer[checkedListTargetLayer.CheckedItems.Count];

            for (int i = 0, j = 0, num = checkedListTargetLayer.Items.Count; i < num; i++)
            {
                if (checkedListTargetLayer.GetItemChecked(i) == true)
                {
                    LayerComboItem layerItem = (LayerComboItem)checkedListTargetLayer.Items[i];
                    featureLayerList[j] = (IFeatureLayer)layerItem.Layer;
                    j++;
                }
            }

            m_method = methodItem.Id;
            m_spatialRel = relItem.Id;
            m_isSelectedFeature = checkSelectedFeature.Checked;
            m_targetFeatureLayerCount = checkedListTargetLayer.CheckedItems.Count;

            ProgressDialog pd = new ProgressDialog();
            try
            {

                pd.Title = string.Format("{0}",this.Text);//string.Format("{0}[{1}]", this.Text, methodItem.ToString());
                pd.Minimum = 0;
                pd.Maximum = 1 + m_targetFeatureLayerCount;
                pd.CancelEnable = false;
                pd.Show(this);

                ISpatialFilter spatialFilter = createSpatialFilter(sourceFeatureLayer, (esriSpatialRelEnum)m_spatialRel, m_isSelectedFeature);
                pd.Value = 1;

                // 高速化
                spatialFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                IGeometry pGeometry = spatialFilter.Geometry;
                ISpatialIndex pSpatialIndex = (ISpatialIndex)pGeometry;
                pSpatialIndex.AllowIndexing = true;
                pSpatialIndex.Invalidate();

                IActiveView activeView = m_mapControl.ActiveView;

                for(int i = 0; i < m_targetFeatureLayerCount; i++) {
                    IFeatureSelection targetSel = (IFeatureSelection)featureLayerList[i];
                    targetSel.SelectFeatures(spatialFilter, (esriSelectionResultEnum)m_method, false);

                    //targetSel.SelectionChanged(); ※ｲﾍﾞﾝﾄは発行されない

                    //activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, featureLayerList[i], activeView.Extent);
                    pd.Value = i+2;
                }

                // ﾏｯﾌﾟを更新
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                // 編集ﾓｰﾄﾞの場合 (上記 SelectionChanged ｲﾍﾞﾝﾄが発行されない為)
                if(this.m_mainFrm.IsEditMode) {
					// SelectionChanged ｲﾍﾞﾝﾄを発行
					IMap			agMap = m_mapControl.Map;
					agMap.SelectFeature(featureLayerList[0], null);
				}
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                pd.Close();
            }
        }

        #endregion

        /// <summary>
        /// 空間検索のパラメータを設定してスレッドを設定
        /// </summary>
        private void selectFeature()
        {
            // 検索条件になるパラメータの取得
            ConstComboItem methodItem = (ConstComboItem)comboSetOperation.SelectedItem;

            ConstComboItem relItem = (ConstComboItem)comboSpatialRel.SelectedItem;

            LayerComboItem sourcelayerItem = (LayerComboItem)comboSourceLayer.SelectedItem;
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourcelayerItem.Layer;

            IFeatureLayer[] featureLayerList = new IFeatureLayer[checkedListTargetLayer.CheckedItems.Count];

            for (int i = 0, j = 0, num = checkedListTargetLayer.Items.Count; i < num; i++)
            {
                if (checkedListTargetLayer.GetItemChecked(i) == true)
                {
                    LayerComboItem layerItem = (LayerComboItem)checkedListTargetLayer.Items[i];
                    featureLayerList[j] = (IFeatureLayer)layerItem.Layer;
                    j++;
                }
            }

            m_method = methodItem.Id;
            m_spatialRel = relItem.Id;
            m_isSelectedFeature = checkSelectedFeature.Checked;
            m_targetFeatureLayerCount = checkedListTargetLayer.CheckedItems.Count;

            //pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);

            // シリアライズ
            IXMLSerializer serializer = new XMLSerializerClass();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("SourceFeatureLayer", sourceFeatureLayer);

            for (int i = 0; i < checkedListTargetLayer.CheckedItems.Count; i++)
            {
                propertySet.SetProperty("FeatureLayerList" + i, featureLayerList[i]);
            }

            string serializeData = serializer.SaveToString(propertySet, null, null);

            Common.TaskInfo taskInfo = new Common.TaskInfo();
            taskInfo.SerializeData = serializeData;

            taskInfo.CallBackParam = new ParameterizedThreadStart(SelectFeatureCallBackResult);

            // スレッド開始
            threadTask = new Thread(new ParameterizedThreadStart(SelectFeatureThread));
            threadTask.SetApartmentState(ApartmentState.STA);
            threadTask.IsBackground = true;
            threadTask.Start(taskInfo);

            // プログレスバー表示
            frmProgress = new FormProgressManager();
            frmProgress.Owner = this;
            frmProgress.SetTitle(this);
            frmProgress.SetMessage(Properties.Resources.FormSpatialSearch_WhenSearch);
            frmProgress.ShowDialog();
        }


        /// <summary>
        /// 検索条件で使うレイヤと検索するレイヤが同一であるかチェックする
        /// </summary>
        /// <returns>falseの場合、検索で使うレイヤと一致する</returns>
        private bool checkTargetLayerSourceLayer()
        {
            LayerComboItem sourceItem, targetItem;
            ILayer pSourceLayer;

            sourceItem = (LayerComboItem)comboSourceLayer.SelectedItem;
            pSourceLayer = sourceItem.Layer;

            for (int i = 0, num = checkedListTargetLayer.Items.Count; i < num; i++)
            {
                if (checkedListTargetLayer.GetItemChecked(i) == true)
                {
                    targetItem = (LayerComboItem)checkedListTargetLayer.Items[i];
                    if (targetItem.Equals(pSourceLayer) == true)
                    {
                        // 検索条件で使うレイヤと検索するレイヤが同一の場合
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 空間検索の条件チェック
        /// </summary>
        /// <param name="formclose"></param>
        private void search(bool formclose)
        {
            //MainForm parent;

            try
            {
                // 検索するレイヤが選択されているかチェック
                if (checkedListTargetLayer.CheckedItems.Count == 0)
                {
                    // 検索するレイヤが選択されていない場合
                    MessageBoxManager.ShowMessageBox(
                            this,
                            Properties.Resources.FormSpatialSearch_NOT_SELECTED_LAYER,
                            Properties.Resources.FormSpatialSearch_TITLE,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.None);

                    formclose = false;
                    return;
                }

                // 検索する条件のチェック
                if (checkTargetLayerSourceLayer() == false)
                {
                    // 検索するレイヤと検索されるレイヤが同じ
                    MessageBoxManager.ShowMessageBox(
                            this,
                            Properties.Resources.FormSpatialSearch_WARNING_SAMELAYER,
                            Properties.Resources.FormSpatialSearch_TITLE,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.None);

                    formclose = false;
                    return;
                }

                // ダイアログの表示非表示の設定
                //this.Visible = !formclose;
                //Application.DoEvents();

                //parent = this.Parent as MainForm;
                //if (parent != null)
                //{
                //    parent.StartProgressBar = true;
                //}

                this.Enabled = false;

                // 検索処理
                //selectFeature();
                selectFeature2();


                //if (parent != null)
                //{
                //    parent.StartProgressBar = false;
                //}
            }
            catch (ApplicationException)
            {
                formclose = true;
            }
            finally
            {
                //parent = this.Parent as MainForm;
                //if (parent != null)
                //{
                //    parent.StartProgressBar = false;
                //}

                // スレッドメモリ解放
                if (threadTask != null)
                {
                    if (threadTask.IsAlive)
                    {
                        threadTask.Abort();
                    }

                    threadTask = null;
                }

                this.Enabled = true;

                //if (this.Visible == false)
                if (formclose == true)
                {
                    this.Close();
                    //this.Dispose();
                    Logger.Info("空間検索ダイアログを閉じます");
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                //this.Dispose();
                Logger.Info("空間検索ダイアログを閉じます");
            }
            catch (Exception ex)
            {
                Logger.Error("検索処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_SpatialSearch);
            }
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsChangedItem())
                {
                    this.Close();
                    return;
                }

                search(true);
            }
            catch (COMException ex)
            {
                Logger.Error("検索処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_SpatialSearch);
            }
            catch (Exception ex)
            {
                Logger.Error("検索処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_SpatialSearch);
            }
        }


        /// <summary>
        /// 入力レイヤコンボボックスの選択が変更されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboSourceLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool result;
            LayerComboItem item;
            ILayer pLayer;
            LayerManager layManager;

            try
            {
                item = (LayerComboItem)comboSourceLayer.SelectedItem;
                pLayer = item.Layer;

                // 選ばれたレイヤが選択状態のフィーチャを持っているかチェック
                layManager = new LayerManager();
                result = LayerManager.hasSelectedFeature((IFeatureLayer)pLayer);
                // チェックボックスのプロパティ設定
                checkSelectedFeature.Enabled = result;
                checkSelectedFeature.Checked = result;

                // アイテム変更フラグ
                changeSourceLayer = true;
            }
            catch (COMException ex)
            {
                Logger.Error("検索処理の入力レイヤチェック処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_CheckLayer);
            }
            catch (Exception ex)
            {
                Logger.Error("検索処理の入力レイヤチェック処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_CheckLayer);
            }
        }


        private void buttonAccept_Click(object sender, EventArgs e)
        {
            try
            {
                search(false);

                SetChangedItemFlag(false);
            }
            catch (COMException ex)
            {
                Logger.Error("検索処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_SpatialSearch);
            }
            catch (Exception ex)
            {
                Logger.Error("検索処理でエラーが発生しました", ex);
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormSpatialSearch_ERROR_SpatialSearch);
            }
        }

        /// <summary>
        /// 空間検索処理スレッドから呼び出されるコールバック
        /// </summary>
        private void SelectFeatureCallBackResult(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    (IPropertySet)serializer.LoadFromString(ti.SerializeData, null, null);

                IFeatureLayer[] featureLayerList = new IFeatureLayer[m_targetFeatureLayerCount];
                IFeatureLayer[] targetFeatureLayerList = new IFeatureLayer[m_targetFeatureLayerCount];

                for (int i = 0, j = 0, num = checkedListTargetLayer.Items.Count; i < num; i++)
                {
                    if (checkedListTargetLayer.GetItemChecked(i) == true)
                    {
                        LayerComboItem layerItem = (LayerComboItem)checkedListTargetLayer.Items[i];
                        targetFeatureLayerList[j] = (IFeatureLayer)layerItem.Layer;
                        j++;
                    }
                }

                for (int i = 0; i < m_targetFeatureLayerCount; i++)
                {
                    featureLayerList[i] = (IFeatureLayer)propertySet.GetProperty("ResultFeatureLayerList" + i);
                    IFeatureSelection featureSelection = (IFeatureSelection)featureLayerList[i];
                    ISelectionSet2 selectionSet = (ISelectionSet2)featureSelection.SelectionSet;
                    IEnumIDs ids = selectionSet.IDs;

                    IFeatureSelection targetFeatureSelection = (IFeatureSelection)targetFeatureLayerList[i];
                    targetFeatureSelection.Clear();
                    ISelectionSet2 targetSelectionSet = (ISelectionSet2)targetFeatureSelection.SelectionSet;

                    if (selectionSet.Count > 0)
                    {
                        int[] aryOIDs = new int[selectionSet.Count];
                        int id = ids.Next();
                        int num = 0;

                        while (id != -1)
                        {
                            aryOIDs[num] = id;
                            id = ids.Next();

                            num++;
                        }

                        // 選択状態をセット
                        targetSelectionSet.AddList(aryOIDs.Length, ref aryOIDs[0]);
                        //targetSelectionSet.Refresh();

                        targetFeatureSelection.SelectionChanged();
                    }
                }

                // 再描画
                IActiveView pActiveView = m_mapControl.ActiveView;
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);

                //frmProgress.CloseForm();
                this.Refresh();
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(SelectFeatureRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(SelectFeatureRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
        }

        /// <summary>
        /// 空間検索処理例外の再スロー(COMException)
        /// </summary>
        /// <param name="comex">例外(COMException)</param>
        private void SelectFeatureRethrowCOMException(COMException comex)
        {
            Logger.Error("検索処理でエラーが発生しました", comex);
            MessageBoxManager.ShowMessageBoxError(this,
                Properties.Resources.FormSpatialSearch_ERROR_SpatialSearch);

            throw new ApplicationException();
        }

        /// <summary>
        /// 空間検索処理例外の再スロー(Exception)
        /// </summary>
        /// <param name="ex">例外(Exception)</param>
        private void SelectFeatureRethrowException(Exception ex)
        {
            Logger.Error("検索処理でエラーが発生しました", ex);
            MessageBoxManager.ShowMessageBoxError(this,
                Properties.Resources.FormSpatialSearch_ERROR_SpatialSearch);

            throw new ApplicationException();
        }

        /// <summary>
        /// 空間検索処理
        /// </summary>
        /// <param name="obj">空間検索処理を行うためのパラメータ</param>
        private void SelectFeatureThread(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    (IPropertySet)serializer.LoadFromString(ti.SerializeData, null, null);
                IFeatureLayer sourceFeatureLayer = (IFeatureLayer)propertySet.GetProperty("SourceFeatureLayer");

                IFeatureLayer[] featureLayerList = new IFeatureLayer[m_targetFeatureLayerCount];

                for (int i = 0; i < m_targetFeatureLayerCount; i++)
                {
                    featureLayerList[i] = (IFeatureLayer)propertySet.GetProperty("FeatureLayerList" + i);
                }

                esriSelectionResultEnum method = (esriSelectionResultEnum)m_method;
                esriSpatialRelEnum spatialRel = (esriSpatialRelEnum)m_spatialRel;
                bool isSelectedFeature = m_isSelectedFeature;

                // 検索条件を取得
                ISpatialFilter pSpatialFilter = createSpatialFilter(sourceFeatureLayer, spatialRel, isSelectedFeature);

                // 高速化
                pSpatialFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                IGeometry pGeometry = pSpatialFilter.Geometry;
                ISpatialIndex pSpatialIndex = (ISpatialIndex)pGeometry;
                pSpatialIndex.AllowIndexing = true;
                pSpatialIndex.Invalidate();

                //ProgressDialog pd = new ProgressDialog();
                //pd.Minimum = 0;
                //pd.Maximum = m_targetFeatureLayerCount;

                //pd.Show(this);

                for (int i = 0; i < m_targetFeatureLayerCount; i++)
                {
                    IFeatureSelection pFeatureSelection = (IFeatureSelection)featureLayerList[i];
                    pFeatureSelection.SelectFeatures(pSpatialFilter, method, false);
                    pFeatureSelection.SelectionChanged();
                    //pd.Value = i;
                }

                //pd.Close();


                // 結果をシリアライズ
                for (int i = 0; i < m_targetFeatureLayerCount; i++)
                {
                    propertySet.SetProperty("ResultFeatureLayerList" + i, featureLayerList[i]);
                }

                string serializeData = serializer.SaveToString(propertySet, null, null);

                ti.SerializeData = serializeData;

                this.Invoke(ti.CallBackParam,ti);
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(SelectFeatureRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(SelectFeatureRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
        }

        private void comboSpatialRel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // アイテム変更フラグ
            changeSpatialRel = true;
        }

        private void checkedListTargetLayer_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // アイテム変更フラグ
            changeListTargetLayer = true;
        }

        private void comboSetOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            // アイテム変更フラグ
            chandeSetOperation = true;
        }

        private void SetChangedItemFlag(bool flag)
        {
            changeSourceLayer = flag;
            changeSpatialRel = flag;
            changeListTargetLayer = flag;
            chandeSetOperation = flag;
        }

        private bool IsChangedItem()
        {
            if (changeSourceLayer == true
                || changeSpatialRel == true
                || changeListTargetLayer == true
                || chandeSetOperation == true)
            {
                return true;
            }

            return false;
        }
    }
}
