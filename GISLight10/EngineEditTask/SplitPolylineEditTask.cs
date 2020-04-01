using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

using ESRIJapan.GISLight10.Common;


namespace ESRIJapan.GISLight10.EngineEditTask
{
    /// <summary>
    /// ラインを分割する編集タスク
    /// </summary>
    public class SplitPolylineEditTask : IEngineEditTask
    {
        private const string GROUP_NAME = "変更";
        private const string TASK_NAME = "線分割 (スプリット)";
        private const string UNIQUE_NAME = "CutPolylineWithoutSelection_CS_CutPolylineWithoutSelection";

        /// <summary>
        /// タスクを実行している編集エディタ
        /// </summary>
        protected IEngineEditor m_engineEditor;

        /// <summary>
        /// 編集タスクと関連しているスケッチツール
        /// </summary>
        protected IEngineEditSketch m_editSketch;

        /// <summary>
        /// 編集対象のレイヤ
        /// </summary>
        protected IEngineEditLayers m_editLayer;

        /// <summary>
        /// 設定ファイルから取得した数値型の分割ポリシー
        /// </summary>
        protected int editorSplitNumPolicy = 0;
        
        /// <summary>
        /// 設定ファイルから取得した日付型の分割ポリシー
        /// </summary>
        protected int editorSplitDatePolicy = 0;

        /// <summary>
        /// 設定ファイルから取得した分割ポリシー
        /// </summary>
        protected int editorSplitOtherPolicy = 0;

        
        #region IEngineEditTask メンバ

        /// <summary>
        /// タスクがアクティブになったときの処理
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="oldTask"></param>
        public void Activate(IEngineEditor editor, IEngineEditTask oldTask)
        {
            if (editor == null)
            {
                return;
            }

            //Initialize class member variables.
            m_engineEditor = editor;
            m_editSketch = editor as IEngineEditSketch;

            //変更（スケッチツール初期化）
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
        /// タスクがアクティブではなくなったときの処理
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
        /// タスクのグループ名
        /// </summary>
        public string GroupName
        {
            get {
                return GROUP_NAME;
            }
        }


        /// <summary>
        /// タスクの名前
        /// </summary>
        public string Name
        {
            get {
                return TASK_NAME;
            }
        }


        /// <summary>
        /// システムが使用するタスクの名前
        /// </summary>
        public string UniqueName
        {
            get {
                return UNIQUE_NAME;
            }
        }

        #endregion


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
        public void OnFinishSketch()
        {
            if (m_editSketch == null)
            {
                return;
            }

            int count = 0;
            double distance;
            IPoint pClickPoint;
            IFeatureLayer pFeatureLayer;
            IFeatureClass pFeatureClass;
            IFeatureSelection pFeatureSelection;
            ISelectionSet pSelectionSet;
            ISpatialFilter pSpatialFilter = null;
            ICursor pCursor = null;
            IFeatureCursor pFeatureCursor = null;
            IPolygon pBufferPolygon = null;
            IFeature pFeature;
            IFeatureEdit pFeatureEdit;
            ISet pFeatureSet;
            IActiveView pActiveView;
            IDisplay pDisplay;

            try
            {
                pActiveView = (IActiveView)m_engineEditor.Map;
                pDisplay = pActiveView.ScreenDisplay;

                // 設定の読み込み。設定を読み込むようにする場合、コメントアウトをやめる。
                //loadSettings();

                // クリックされたポイントを取得
                pClickPoint = (IPoint)m_editSketch.Geometry;
                // 編集対象レイヤ・フィーチャクラスの取得
                pFeatureLayer = m_editLayer.TargetLayer;
                pFeatureClass = pFeatureLayer.FeatureClass;

                // 検索範囲を取得
                distance = getSearchDistance();
                // 検索バッファを作成
                pBufferPolygon = createBufferPolygon(pClickPoint, distance);

                // 検索フィルタを作成
                pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
                pSpatialFilter.Geometry = pBufferPolygon;
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;

                m_engineEditor.StartOperation();

                // 切断するフィーチャを検索
                pFeatureSelection = (IFeatureSelection)pFeatureLayer;
                pSelectionSet = pFeatureSelection.SelectionSet;
                pSelectionSet.Search(pSpatialFilter, false, out pCursor);
                pFeatureCursor = (IFeatureCursor)pCursor;

                pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    try
                    {
                        pFeatureEdit = (IFeatureEdit)pFeature;

                        // フィーチャの分断
                        pFeatureSet = pFeatureEdit.Split(pClickPoint);

                        // 属性の更新
                        SplitAttribute(pFeature, pFeatureSet);

                        // 更新した図形をフラッシュ
                        //flashGeometry((IPolyline)pFeature.Shape, pDisplay);
                        pFeatureSet.Reset();
                        pFeature = (IFeature)pFeatureSet.Next();
                        while (pFeature != null)
                        {
                            flashGeometry((IPolyline)pFeature.Shape, pDisplay);
                            pSelectionSet.Add(pFeature.OID);
                            pFeature = (IFeature)pFeatureSet.Next();
                            count++;
                        }
                    }
                    catch (COMException ex)
                    {
                        Logger.Debug("フィーチャの分割失敗", ex);
                    }

                    pFeature = pFeatureCursor.NextFeature();
                }

                m_engineEditor.StopOperation("線分断");

                pSelectionSet.Refresh();
                pActiveView.PartialRefresh(
                            esriViewDrawPhase.esriViewGeography |
                            esriViewDrawPhase.esriViewGeoSelection, null,
                            pActiveView.Extent);

                if (count == 0)
                {
                    MessageBoxManager.ShowMessageBoxWarining(
                            Properties.Resources.SplitPolylineEditTask_WARNING_BadSplitPoint);
                }
            }
            catch (Exception ex)
            {
                m_engineEditor.AbortOperation();
                Logger.Error("ラインの分断に失敗", ex);
                MessageBoxManager.ShowMessageBoxError(
                            Properties.Resources.SplitPolylineEditTask_ERROR_DoSplit);
            }
            finally
            {
                if (pSpatialFilter != null)
                {
                    ComReleaser.ReleaseCOMObject(pSpatialFilter);
                }
                if (pCursor != null)
                {
                    ComReleaser.ReleaseCOMObject(pCursor);
                }
                if (pBufferPolygon != null)
                {
                    ComReleaser.ReleaseCOMObject(pBufferPolygon);
                }
            }
        }


        /// <summary>
        /// スケッチツールのステータスが更新された場合の処理
        /// </summary>
        private void UpdateSketchToolStatus()
        {
            if (m_editLayer == null)
            {
                return;
            }

            IFeatureSelection fcSelection = (IFeatureSelection)m_editLayer.TargetLayer;
            ISelectionSet selectionSet = fcSelection.SelectionSet;

            //Only enable the sketch tool if there is a polygon target layer.
            //編集ターゲットレイヤがポリライン または 
            //ラスタカタログでない　　　　　 または
            //選択フィーチャ数が0　　　　　　の場合
            if ((m_editLayer.TargetLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline) ||
                (m_editLayer.TargetLayer is GdbRasterCatalogLayer) ||
                (selectionSet.Count == 0))
            {
                m_editSketch.GeometryType = esriGeometryType.esriGeometryNull;
            }
            else
            {
                //Set the edit sketch geometry type to be esriGeometryPolyline.
                m_editSketch.GeometryType = esriGeometryType.esriGeometryPoint;
            }
        }


        /// <summary>
        /// 編集ポリシーに従ってフィーチャ属性を編集する。
        /// ■数値型フィールド
        ///   ①editorSplitNumPolicy == 0の場合
        ///      スプリット元のフィーチャの属性を使用する。
        ///   ②editorSplitNumPolicy == 1の場合
        ///      面積比で分割した属性値を使用する。
        ///
        /// ■日付型フィールド
        ///   ③editorSplitDatePolicy == 0の場合
        ///      スプリット元のフィーチャの属性を使用する。
        ///   ④editorSplitDatePolicy == 1の場合
        ///      スプリットした日時を使用する。
        ///
        /// ■その他のフィールド
        ///   ⑤editorSplitOtherPolicy == 0の場合
        ///      スプリット元のフィーチャの属性を使用する。
        ///
        ///　★①、③、⑤の場合は、
        ///    デフォルトの挙動なので、以下のコードでは明記しない。
        /// </summary>
        private void SplitAttribute(IFeature pOrgFeature, ISet pSplitFeatures)
        {
            short shtVal;
            int i, count, j, fieldnum = 0, intVal;
            float fltVal;
            double totalLength = 0f, dblVal;
            double[] ratio;
            IFeature pFeature;
            IPolyline pPolyline;
            IField pField;

            count = pSplitFeatures.Count;
            ratio = new double[count];

            i = 0;
            pSplitFeatures.Reset();
            pFeature = (IFeature)pSplitFeatures.Next();
            while (pFeature != null)
            {
                pPolyline = (IPolyline)pFeature.Shape;
                ratio[i++] = pPolyline.Length;

                pFeature = (IFeature)pSplitFeatures.Next();
            }

            // 割合を求める
            for (i = 0; i < count; i++)
            {
                totalLength += ratio[i];
            }
            for (i = 0; i < count; i++)
            {
                ratio[i] = ratio[i] / totalLength;
            }

            // 属性の更新
            i = 0;
            pSplitFeatures.Reset();
            pFeature = (IFeature)pSplitFeatures.Next();
            while (pFeature != null)
            {
                if (fieldnum == 0)
                {
                    // フィールド数の取得
                    fieldnum = pFeature.Fields.FieldCount;
                }
                for (j = 0; j < fieldnum; j++)
                {
                    pField = pFeature.Fields.get_Field(j);
                    if (pField.Editable == true && editorSplitDatePolicy == 1 && pField.Domain == null)
                    {
                        // 編集可能フィールドかつドメイン設定なしで、割合で分割する場合
                        switch (pField.Type)
                        {
                            case esriFieldType.esriFieldTypeSmallInteger:   // short型の場合
                                if ((pFeature.get_Value(j) is System.DBNull) == false)
                                {
                                    // 割合で分割する
                                    shtVal = (short)pFeature.get_Value(j);
                                    pFeature.set_Value(j, shtVal * ratio[i]);
                                }
                                break;

                            case esriFieldType.esriFieldTypeInteger:    // long型の場合
                                if ((pFeature.get_Value(j) is System.DBNull) == false)
                                {
                                    // 割合で分割する
                                    intVal = (int)pFeature.get_Value(j);
                                    pFeature.set_Value(j, intVal * ratio[i]);
                                }
                                break;

                            case esriFieldType.esriFieldTypeSingle:     // float型の場合
                                if ((pFeature.get_Value(j) is System.DBNull) == false)
                                {
                                    // 割合で分割する
                                    fltVal = (float)pFeature.get_Value(j);
                                    pFeature.set_Value(j, fltVal * ratio[i]);
                                }
                                break;

                            case esriFieldType.esriFieldTypeDouble:     // double型の場合
                                if ((pFeature.get_Value(j) is System.DBNull) == false)
                                {
                                    // 割合で分割する
                                    dblVal = (double)pFeature.get_Value(j);
                                    pFeature.set_Value(j, dblVal * ratio[i]);
                                }
                                break;

                            case esriFieldType.esriFieldTypeDate:
                                // 分割した日にちに更新する場合
                                pFeature.set_Value(j, System.DateTime.Now);
                                break;

                            default:
                                break;
                        }
                    }
                }
                pFeature.Store();

                pFeature = (IFeature)pSplitFeatures.Next();
                i++;
            }
        }


        /// <summary>
        /// 選択ツールの検索距離（ピクセル）から検索する距離（地図単位）を求める
        /// </summary>
        /// <returns>検索する距離（地図単位）</returns>
        protected double getSearchDistance()
        {
            double distance;
            ISelectionEnvironment pSelectionEnvironment;    // シングルトンクラスなので解放しないこと
            IEnvelope pEnvelope = null;
            tagRECT rect;
            IActiveView pActiveView;
            IDisplayTransformation pDisplayTrans;

            try
            {
                pActiveView = (IActiveView)m_engineEditor.Map;

                // 検索範囲のピクセル数と同じ大きさの矩形を作成する
                pSelectionEnvironment = new SelectionEnvironmentClass();
                pEnvelope = new EnvelopeClass();
                rect.left = rect.bottom = 0;
                rect.right = rect.top = pSelectionEnvironment.SearchTolerance;

                // 上記で作成した矩形をマップ単位の矩形に変換する
                pDisplayTrans = pActiveView.ScreenDisplay.DisplayTransformation;
                pDisplayTrans.TransformRect(pEnvelope, ref rect, 4);

                distance = pEnvelope.Height;

                return distance;
            }
            finally
            {
                if (pEnvelope != null)
                {
                    ComReleaser.ReleaseCOMObject(pEnvelope);
                }
            }
        }


        /// <summary>
        /// 中心点と距離でバッファポリゴンを作成する
        /// </summary>
        /// <param name="pPoint">バッファポリゴンの中心点</param>
        /// <param name="distance">バッファ距離</param>
        /// <returns>バッファポリゴン</returns>
        protected IPolygon createBufferPolygon(IPoint pPoint, double distance)
        {
            ITopologicalOperator pTopo;
            IPolygon pPolygon;

            pTopo = (ITopologicalOperator)pPoint;
            pPolygon = (IPolygon)pTopo.Buffer(distance);

            return pPolygon;
        }


        /// <summary>
        /// 設定ファイルから分割するルールを取得する。
        /// 現状はスプリット設定を取得するコードになっているので、
        /// 将来はポリラインを分割する設定から取得するコードに変更すること。
        /// このメソッドは呼び出されない。
        /// </summary>
        protected void loadSettings()
        {
            string work;
            SplitAndMargeSettings splitAndMergeSetting = null;

            //設定ファイルが存在するか確認する
            if (!ApplicationInitializer.IsUserSettingsExists())
            {
                MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                throw new Exception();
            }

            try
            {
                //設定ファイル（GISLight10Settings.xml）から編集ポリシーの設定を読み込む            
                splitAndMergeSetting = new SplitAndMargeSettings();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリットの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリットの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);

                throw ex;
            }

            //数値型フィールドのポリシー            
            try
            {
                work = splitAndMergeSetting.EditorSplitNumField;
                editorSplitNumPolicy = Int32.Parse(work);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット：数値型フィールドの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット：数値型フィールドの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);

                throw ex;
            }

            //日付型フィールドのポリシー            
            try
            {
                work = splitAndMergeSetting.EditorSplitDateField;
                editorSplitDatePolicy = Int32.Parse(work);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット：日付型フィールドの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット：日付型フィールドの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);

                throw ex;
            }


            //その他フィールドのポリシー      
            try
            {
                work = splitAndMergeSetting.EditorSplitField;
                editorSplitOtherPolicy = Int32.Parse(work);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット：その他フィールドの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                        "[ スプリット：その他フィールドの属性編集方法 ]" +
                        Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);

                throw ex;
            }
        }


        /// <summary>
        /// フィーチャのフラッシュ
        /// </summary>
        /// <param name="pPolyline">ジオメトリ</param>
        /// <param name="pDisplay">IDisplay</param>
        private void flashGeometry(IPolyline pPolyline, IDisplay pDisplay)
        {
            const int interval = 150;
            IRgbColor pColor = null;
            ISimpleLineSymbol pSimpleLineSymbol = null;
            ISymbol pSymbol = null;

            try
            {
                // 各種変数の初期化
                pColor = new RgbColorClass();
                pColor.Red = 255;

                pSimpleLineSymbol = new SimpleLineSymbolClass();
                pSimpleLineSymbol.Color = pColor;

                pSymbol = (ISymbol)pSimpleLineSymbol;
                pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                //Flash the input polygon geometry.
                pDisplay.StartDrawing(pDisplay.hDC, (short)esriScreenCache.esriNoScreenCache);
                pDisplay.SetSymbol(pSymbol);
                pDisplay.DrawPolyline(pPolyline);
                //Time in milliseconds to wait.
                System.Threading.Thread.Sleep(interval);
                pDisplay.DrawPolyline(pPolyline);
                pDisplay.FinishDrawing();
            }
            finally
            {
                if (pSymbol != null)
                {
                    ComReleaser.ReleaseCOMObject(pSymbol);
                }
                if (pSimpleLineSymbol != null)
                {
                    ComReleaser.ReleaseCOMObject(pSimpleLineSymbol);
                }
                if (pColor != null)
                {
                    ComReleaser.ReleaseCOMObject(pColor);
                }
            }
        }
    }

}
