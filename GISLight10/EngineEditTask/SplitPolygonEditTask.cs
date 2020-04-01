using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.Text;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRIJapan.GISLight10.Common;
using ESRI.ArcGIS.ADF;
using System.Collections.Generic;
using ESRI.ArcGIS.Display;

namespace ESRIJapan.GISLight10.EngineEditTask
{
    /// <summary>
    /// 編集機能のスプリット タスクのクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public class SplitPolygonEditTask : ESRI.ArcGIS.Controls.IEngineEditTask
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SplitPolygonEditTask() {
            
        }        

        #region Private Members
        IEngineEditor m_engineEditor;
        IEngineEditSketch m_editSketch;
        IEngineEditLayers m_editLayer;
        #endregion

        /*** ここはコメントアウト ******/
        #region COM Registration Function(s)
        ////[ComRegisterFunction()]
        ////[ComVisible(false)]
        ////static void RegisterFunction(Type registerType)
        ////{
        ////    // Required for ArcGIS Component Category Registrar support
        ////    ArcGISCategoryRegistration(registerType);
        ////}

        ////[ComUnregisterFunction()]
        ////[ComVisible(false)]
        ////static void UnregisterFunction(Type registerType)
        ////{
        ////    // Required for ArcGIS Component Category Registrar support
        ////    ArcGISCategoryUnregistration(registerType);

        ////}

        #region ArcGIS Component Category Registrar generated code
        /////// <summary>
        /////// Required method for ArcGIS Component Category registration -
        /////// Do not modify the contents of this method with the code editor.
        /////// </summary>
        ////private static void ArcGISCategoryRegistration(Type registerType)
        ////{
        ////    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        ////    EngineEditTasks.Register(regKey);

        ////}
        /////// <summary>
        /////// Required method for ArcGIS Component Category unregistration -
        /////// Do not modify the contents of this method with the code editor.
        /////// </summary>
        ////private static void ArcGISCategoryUnregistration(Type registerType)
        ////{
        ////    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        ////    EngineEditTasks.Unregister(regKey);

        ////}

        #endregion
        #endregion
        /*******************************/


        //public ArrayList listStructSplitPare = new ArrayList();

        struct splitPare {
            /// <summary>
            /// オリジナルフィーチャ
            /// </summary>
            public IFeature orignFeature;

            /// <summary>
            /// 新規フィーチャ
            /// </summary>
            public IFeature newFeature;            
        }


        #region IEngineEditTask Implementations
        /// <summary>
        /// 活性化処理
        /// </summary>
        /// <param name="editor">エディタコントロール</param>
        /// <param name="oldTask">エディタタスクコントロール</param>
        public void Activate(ESRI.ArcGIS.Controls.IEngineEditor editor, ESRI.ArcGIS.Controls.IEngineEditTask oldTask)
        {
            if (editor == null)
                return;

            //Initialize class member variables.
            m_engineEditor = editor;
            m_editSketch = editor as IEngineEditSketch;

            //変更（スケッチツール初期化）
            //m_editSketch.GeometryType = esriGeometryType.esriGeometryPolyline;
            UpdateSketchToolStatus();

            m_editLayer = m_editSketch as IEngineEditLayers;

            //Wire editor events.        
            ((IEngineEditEvents_Event)m_editSketch).OnTargetLayerChanged += 
                new IEngineEditEvents_OnTargetLayerChangedEventHandler(OnTargetLayerChanged);
            ((IEngineEditEvents_Event)m_editSketch).OnCurrentTaskChanged += 
                new IEngineEditEvents_OnCurrentTaskChangedEventHandler(OnCurrentTaskChanged);

            //追加（フィーチャ選択イベント）
            ((IEngineEditEvents_Event)m_editSketch).OnSelectionChanged += 
                new IEngineEditEvents_OnSelectionChangedEventHandler(OnSelectionChanged);
        }

        /// <summary>
        /// 非活性化処理
        /// </summary>
        public void Deactivate()
        {
            //Stop listening for editor events.
            ((IEngineEditEvents_Event)m_engineEditor).OnTargetLayerChanged -= OnTargetLayerChanged;
            ((IEngineEditEvents_Event)m_engineEditor).OnCurrentTaskChanged -= OnCurrentTaskChanged;

            //追加（フィーチャ選択イベント）
            ((IEngineEditEvents_Event)m_editSketch).OnSelectionChanged -= OnSelectionChanged;

            //Release object references.
            m_engineEditor = null;
            m_editSketch = null;
            m_editLayer = null;
        }

        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName {
            get {
                //return "Modify Tasks";
                return "変更";
            }
        }

        /// <summary>
        /// スプリット名
        /// </summary>
        public string Name {
            get {
                //return "Cut Polygons Without Selection (C#)";
                return "スプリット";
            }
        }

        //This method is not expected to be fired since we have unregistered from the event in Deactivate
        /// <summary>
        /// カレントタスク変更時処理
        /// </summary>
        public void OnCurrentTaskChanged()
        {
            UpdateSketchToolStatus();
        }

        /// <summary>
        /// ターゲットレイヤー変更時処理
        /// </summary>
        public void OnTargetLayerChanged()
        {
            UpdateSketchToolStatus();
        }

        //追加（フィーチャ選択イベント）
        /// <summary>
        /// フィーチャ選択時処理
        /// </summary>
        public void OnSelectionChanged()
        {
            UpdateSketchToolStatus();
        }

        /// <summary>
        /// スケッチ削除時処理
        /// </summary>
        public void OnDeleteSketch()
        {
        }

        /// <summary>
        /// スケッチ終了時処理
        /// </summary>
        public void OnFinishSketch() {
            if (m_editSketch == null)
                return;            

            bool hasCutPolygons = false;

            //Change the cursor to be hourglass shape.
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            //COMリリースのためにtry文の外で宣言
            ICursor cursor = null;
            IFeatureCursor featureCursor = null;

            IGeometry cutGeometry = null;
            ITopologicalOperator2 topoOperator = null;
            ISpatialFilter spatialFilter = null;
            IFeatureClass featureClass = null;
            IFeatureSelection fcSelection = null;
            ISelectionSet selectionSet = null;
            IFeature origFeature = null;
            IZAware zAware = null;
            ArrayList listStructSplitPare = null;
            IFeatureEdit featureEdit = null;            

            try {
                //Get the geometry that performs the cut from the edit sketch.
                ////IGeometry 
                cutGeometry = m_editSketch.Geometry;

                //The sketch geometry is simplified to deal with a multi-part sketch as well
                //as the case where the sketch loops back over itself.
                ////ITopologicalOperator2 
                topoOperator = cutGeometry as ITopologicalOperator2;
                topoOperator.IsKnownSimple_2 = false;
                topoOperator.Simplify();

                //Create the spatial filter to search for features in the target feature class.
                //The spatial relationship we care about is whether the interior of the line 
                //intesects the interior of the polygon.
                ////ISpatialFilter 
                spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = m_editSketch.Geometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //Find the polygon features that cross the sketch.
                ////IFeatureClass 
                featureClass = m_editLayer.TargetLayer.FeatureClass;

                
                /*** 選択されているフィーチャのみスプリットするように変更 ********************/
                ////IFeatureSelection 
                fcSelection = (IFeatureSelection)m_editLayer.TargetLayer;
                ////ISelectionSet 
                selectionSet = fcSelection.SelectionSet;

                if (selectionSet.Count > 0)
                {                                        
                    //IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);

                    selectionSet.Search(spatialFilter, false, out cursor);
                    featureCursor = (IFeatureCursor)cursor;                    
                }                
                else
                {                 
                    return;
                }                
                /******************************************************************************/

                
                //Only do work if there are features that intersect the edit sketch.
                ////IFeature 
                origFeature = featureCursor.NextFeature();
                if (origFeature != null)
                {
                    //Check the first feature to see if it is ZAware and if it needs to make the
                    //cut geometry ZAware.
                    ////IZAware 
                    zAware = origFeature.Shape as IZAware;
                    if (zAware.ZAware)
                    {
                        zAware = cutGeometry as IZAware;
                        zAware.ZAware = true;
                    }

                    ArrayList comErrors = new ArrayList();                    

                    //Start an edit operation so we can have undo/redo.
                    m_engineEditor.StartOperation();

                    Common.Logger.Info("スプリットの編集オペレーション：開始");

                    /*** 属性編集用：スプリット前後のフィーチャを格納する入れ物。***/                    
                    ////ArrayList 
                    listStructSplitPare = new ArrayList();
                    /*************************************************************/

                    //Cycle through the features, cutting with the sketch.
                    while (origFeature != null) {
                        try {
                            //Split the feature. Use the IFeatureEdit::Split method which ensures
                            //the attributes are correctly dealt with.
                            ////IFeatureEdit 
                            featureEdit = origFeature as IFeatureEdit;

                            Common.Logger.Info("スプリットの実行 OID:" + origFeature.OID.ToString());

                            //Set to hold the new features that are created by the Split.    
                            //スプリットの実行
                            ISet newFeaturesSet = featureEdit.Split(cutGeometry);                            

                            //New features have been created.                            
                            if(newFeaturesSet != null) {
                                /*** 属性編集用：スプリット前・後のフィーチャを格納する。 *****/ 
                                newFeaturesSet.Reset();
                                IFeature newFeature = (IFeature)newFeaturesSet.Next();

                                splitPare structSplitPare = new splitPare();

                                while (newFeature != null) {
                                    structSplitPare.orignFeature = origFeature;
                                    structSplitPare.newFeature = newFeature;
                                    listStructSplitPare.Add(structSplitPare);

                                    newFeature = (IFeature)newFeaturesSet.Next();
                                }
                                /**************************************************************/

                                newFeaturesSet.Reset();
                                hasCutPolygons = true;
                            }
                        }
                        catch (COMException comExc) {                            
                            comErrors.Add(String.Format("OID: {0}, Error: {1} , {2}", origFeature.OID.ToString(), comExc.ErrorCode, comExc.Message));                          
                        }
                        finally {
                            //Continue to work on the next feature if it fails to split the current one.
                            origFeature = featureCursor.NextFeature();
                        }
                    }

                    //If any polygons were cut, refresh the display and stop the edit operation.
                    if(hasCutPolygons) {
                        //Clear the map's selection.
                        //m_engineEditor.Map.ClearSelection();                        
                        fcSelection.Clear();

                        /*** 属性編集用：スプリット後のフィーチャを編集する。**********/
                        Common.Logger.Info("属性編集の実行");

                        SplitAttribute(listStructSplitPare, featureClass);
                        /**************************************************************/

                        /*** スプリットされたフィーチャの選択とフラッシュ *************/
                        SelectAndFlashFeature(listStructSplitPare);
                        /**************************************************************/

                        //Complete the edit operation.
                        m_engineEditor.StopOperation("Cut Polygons Without Selection");                                                
                        Common.Logger.Info("スプリットの編集オペレーション：正常終了");
                    }
                    else {
                        m_engineEditor.AbortOperation();
                        Common.Logger.Info("スプリットの編集オペレーション：アボート");
                    }
                    
                    //report any errors that have arisen while splitting features
                    //エラーメッセージ表示の条件変更
                    ////if (comErrors.Count > 0)
                    if(hasCutPolygons == false) {
                        //スプリットラインが1つのスプリット対象フィーチャも横断していない場合
                        //（重なりはある）

                        //StringBuilder stringBuilder = new StringBuilder("The following features could not be split: \n", 200);
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (string comError in comErrors) {
                            stringBuilder.AppendLine(comError);
                        }

                        //MessageBox.Show(stringBuilder.ToString(), "Cut Errors");
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                            (Properties.Resources.SplitPolygonEditTask_WARNING_BadSplitLine);
                        Common.Logger.Warn(Properties.Resources.SplitPolygonEditTask_WARNING_BadSplitLine);
                        Common.Logger.Warn(stringBuilder.ToString());                        
                    }
                }
                else {
                    //スプリットラインが1つのスプリット対象フィーチャとも重なっていない場合
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining
                            (Properties.Resources.SplitPolygonEditTask_WARNING_BadSplitLine);
                    Common.Logger.Warn(Properties.Resources.SplitPolygonEditTask_WARNING_BadSplitLine);
                }
            }
            catch (Exception e) {
                m_engineEditor.Map.ClearSelection();

                //予期せぬエラー
                //MessageBox.Show("Unable to perform the cut task.\n" + e.Message);
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.SplitPolygonEditTask_ERROR_DoSplit);
                Common.Logger.Error(Properties.Resources.SplitPolygonEditTask_ERROR_DoSplit);
                Common.Logger.Error(e.Message);
                Common.Logger.Error(e.StackTrace);

                m_engineEditor.AbortOperation();

                Common.Logger.Info("スプリットの編集オペレーション：アボート");                
            }
            finally {
                //Refresh the display including modified layer and any previously selected component. 
                IActiveView activeView = m_engineEditor.Map as IActiveView;
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography |
                    esriViewDrawPhase.esriViewGeoSelection, null, activeView.Extent);

                //Change the cursor shape to default.
                System.Windows.Forms.Cursor.Current = Cursors.Default;

                //COMリリース
                ComReleaser.ReleaseCOMObject(cursor);
                ComReleaser.ReleaseCOMObject(featureCursor);

                ComReleaser.ReleaseCOMObject(cutGeometry);
                ComReleaser.ReleaseCOMObject(topoOperator);
                ComReleaser.ReleaseCOMObject(spatialFilter);
                ComReleaser.ReleaseCOMObject(featureClass);
                ComReleaser.ReleaseCOMObject(fcSelection);
                ComReleaser.ReleaseCOMObject(selectionSet);
                ComReleaser.ReleaseCOMObject(origFeature);
                ComReleaser.ReleaseCOMObject(zAware);
                ComReleaser.ReleaseCOMObject(listStructSplitPare);
                ComReleaser.ReleaseCOMObject(featureEdit);                                              
            }
        }

        /// <summary>
        /// ユニーク名称
        /// </summary>
        public string UniqueName {
            get {
                return "CutPolygonsWithoutSelection_CS_CutPolygonsWithoutSelection_CSharp";
            }
        }
        #endregion

        #region IEngineEditTask private methods
        private void UpdateSketchToolStatus() {
            if (m_editLayer == null)
                return;
            
            IFeatureSelection fcSelection = (IFeatureSelection)m_editLayer.TargetLayer;            
            ISelectionSet selectionSet = fcSelection.SelectionSet;

            //Only enable the sketch tool if there is a polygon target layer.
            //編集ターゲットレイヤがポリゴン または 
            //ラスタカタログでない　　　　　 または
            //選択フィーチャ数が0　　　　　　の場合
            if ((m_editLayer.TargetLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolygon) ||
                (m_editLayer.TargetLayer is GdbRasterCatalogLayer) ||
                (selectionSet.Count == 0))
            {
                m_editSketch.GeometryType = esriGeometryType.esriGeometryNull;
            }
            else
            {
                //Set the edit sketch geometry type to be esriGeometryPolyline.
                m_editSketch.GeometryType = esriGeometryType.esriGeometryPolyline;
            }
        }
        #endregion


        /// <summary>
        /// 編集ポリシーに従ってフィーチャ属性を編集する
        /// </summary>
        /// <param name="listStructSplitPare">スプリット前と後のフィーチャが格納された構造体のリスト</param>
        /// <param name="targetFC">編集対象のフィーチャクラス</param>
        private void SplitAttribute(ArrayList listStructSplitPare, IFeatureClass targetFC)
        {
            SplitAndMargeSettings splitAndMergeSetting = null;

            //設定ファイルが存在するか確認する
            if (!ApplicationInitializer.IsUserSettingsExists())
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                throw new Exception();
            }
            
            try {
                //設定ファイル（GISLight10Settings.xml）から編集ポリシーの設定を読み込む            
                splitAndMergeSetting = new SplitAndMargeSettings();
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリットの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリットの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }

            string editorSplitNumPolicy = "";
            string editorSplitDatePolicy = "";
            string editorSplitOtherPolicy = "";

            //数値型フィールドのポリシー            
            try {
                editorSplitNumPolicy = splitAndMergeSetting.EditorSplitNumField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリット：数値型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリット：数値型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }

            //日付型フィールドのポリシー            
            try {
                editorSplitDatePolicy = splitAndMergeSetting.EditorSplitDateField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリット：日付型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリット：日付型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }
            
            //その他フィールドのポリシー      
            try {
                editorSplitOtherPolicy = splitAndMergeSetting.EditorSplitField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリット：その他フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ スプリット：その他フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }

            for (int i = 0; i < listStructSplitPare.Count; i++) {
                splitPare structSplitPare = (splitPare)listStructSplitPare[i];

                IFeature originFeature = structSplitPare.orignFeature;
                IFeature newFeature = structSplitPare.newFeature;

                //スプリット前と後での面積比を求める
                IArea originArea = (IArea)originFeature.Shape;
                IArea newArea = (IArea)newFeature.Shape;                
                double areaRatio = newArea.Area / originArea.Area;                
                
                IRow row = newFeature.Table.GetRow(newFeature.OID);

                Common.Logger.Info("OID: " + newFeature.OID);
                
                //フィールドをループする
                for (int j = 0; j < newFeature.Fields.FieldCount; j++) {
                    IField field = newFeature.Fields.get_Field(j);                    

                    //Shape_Length、Shape_Area、OID、ジオメトリフィールドはスキップする                    
                    if (field.Equals(targetFC.LengthField) ||
                        field.Equals(targetFC.AreaField)   ||
                        field.Type == esriFieldType.esriFieldTypeOID ||
                        field.Type == esriFieldType.esriFieldTypeGeometry)
                    {                        
                        continue;
                    }

                    //フィールドに対してドメイン設定のポリシーが設定されている場合もスキップする
                    if (field.Domain != null) {
                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーあり");
                        continue;
                    }
                    else {
                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーなし");
                    }  

                    //※ 編集ポリシーの説明
                    // ■数値型フィールド
                    //   ①editorSplitNumPolicy == 0の場合
                    //      スプリット元のフィーチャの属性を使用する。
                    //   ②editorSplitNumPolicy == 1の場合
                    //      面積比で分割した属性値を使用する。
                    // ■日付型フィールド
                    //   ③editorSplitDatePolicy == 0の場合
                    //      スプリット元のフィーチャの属性を使用する。
                    //   ④editorSplitDatePolicy == 1の場合
                    //      スプリットした日時を使用する。
                    // ■その他のフィールド
                    //   ⑤editorSplitOtherPolicy == 0の場合
                    //      スプリット元のフィーチャの属性を使用する。
                    //
                    //　★①、③、⑤の場合は、
                    //    デフォルトの挙動なので、以下のコードでは明記しない。                                
                    switch (field.Type) {
                        case esriFieldType.esriFieldTypeDate:
                            if (editorSplitDatePolicy.Equals("1")) {
                                //スプリットした日時を設定
                                row.set_Value(j, System.DateTime.Now);
                                //row.Store();
                            }
                            break;

                        case esriFieldType.esriFieldTypeDouble:
                            if (editorSplitNumPolicy.Equals("1") &&
                                !row.get_Value(j).Equals(0) &&
                                !row.get_Value(j).Equals(System.DBNull.Value))
                            {
                                //面積比で分割した値を設定 
                                double newValue = (double)row.get_Value(j) * areaRatio;
                                row.set_Value(j, newValue);
                                //row.Store();
                            }
                            break;

                        case esriFieldType.esriFieldTypeInteger:
                            if (editorSplitNumPolicy.Equals("1") &&
                                !row.get_Value(j).Equals(0) &&
                                !row.get_Value(j).Equals(System.DBNull.Value))
                            {
                                //面積比で分割した値を設定 
                                double newValue = (Int32)row.get_Value(j) * areaRatio;
                                row.set_Value(j, newValue);                                
                            }
                            break;

                        case esriFieldType.esriFieldTypeSingle:
                            if (editorSplitNumPolicy.Equals("1") &&
                                !row.get_Value(j).Equals(0) &&
                                !row.get_Value(j).Equals(System.DBNull.Value))
                            {
                                //面積比で分割した値を設定 
                                double newValue = (Single)row.get_Value(j) * areaRatio;
                                row.set_Value(j, newValue);                               
                            }
                            break;

                        case esriFieldType.esriFieldTypeSmallInteger:
                            if (editorSplitNumPolicy.Equals("1") &&
                                !row.get_Value(j).Equals(0) &&
                                !row.get_Value(j).Equals(System.DBNull.Value))
                            {
                                //面積比で分割した値を設定                                                 
                                double newValue = (Int16)row.get_Value(j) * areaRatio;
                                row.set_Value(j, newValue);                                
                            }
                            break;

                        default:
                            break;
                    }
                }

                row.Store();
			}         
        }

        /// <summary>
        /// フィーチャの選択とフラッシュ
        /// </summary>
        /// <param name="listStructSplitPare">スプリット前と後のフィーチャが格納された構造体のリスト</param>
        private void SelectAndFlashFeature(ArrayList listStructSplitPare)
        {
            IFeatureSelection fcSelection = (IFeatureSelection)m_editLayer.TargetLayer;            
            
            splitPare structSplitPare;
            IFeature newFeature;

            for (int i = 0; i < listStructSplitPare.Count; i++)
            {
                IActiveView activeView = m_engineEditor.Map as IActiveView;

                structSplitPare = (splitPare)listStructSplitPare[i];
                newFeature = structSplitPare.newFeature;
                
                //フィーチャの選択
                fcSelection.Add(newFeature);
                //m_engineEditor.Map.SelectFeature(m_editLayer.TargetLayer, newFeature);

                //フィーチャのフラッシュ
                FlashGeometry(newFeature.Shape,activeView.ScreenDisplay);                  
            }            
        }

        /// <summary>
        /// フィーチャのフラッシュ
        /// </summary>
        /// <param name="geo">ジオメトリ</param>
        /// <param name="display">IDisplay</param>
        private void FlashGeometry(IGeometry geo, IDisplay display)
        {
            //Flash the input polygon geometry.
            display.StartDrawing(display.hDC, (short)esriScreenCache.esriNoScreenCache);

            //Time in milliseconds to wait.
            int interval = 150;
            switch (geo.GeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    //Set the flash geometry's symbol.
                    IRgbColor color = new RgbColorClass();
                    color.Red = 255;

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = color;

                    ISymbol symbol = simpleFillSymbol as ISymbol;
                    symbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                    display.SetSymbol(symbol);
                    display.DrawPolygon(geo);
                    System.Threading.Thread.Sleep(interval);
                    display.DrawPolygon(geo);

                    break;
            }
            display.FinishDrawing();
        }
    }
}