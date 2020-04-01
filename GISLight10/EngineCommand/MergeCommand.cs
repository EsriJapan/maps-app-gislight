using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRIJapan.GISLight10.Common;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;

using ESRIJapan.GISLight10.EngineEditTask;
using System.Collections;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.ADF;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// 編集機能のマージ コマンドのクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class MergeCommand : Common.EjBaseCommand
    {
        //private IHookHelper m_HookHelper;
        //private IMapControl3 m_mapControl;
        //private Ui.MainForm m_mainForm;
        private IEngineEditor m_engineEditor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MergeCommand()
        {
            base.captionName = "マージ";
        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// </summary>
        /// <param name="hook">ツールバーコントロール</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_engineEditor = new EngineEditorClass();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// クリック時処理
        /// マージ実行
        /// </summary>
        public override void OnClick()
        {            
            // 編集ターゲットレイヤのセレクションセットを取得
            ISelectionSet selectionSet = GetTargetSelectionSet();            
            
            // 編集ターゲットレイヤのフィーチャクラスを取得
            IFeatureClass targetFcClass = GetTargetFeatureClass();

            
            IEnumIDs enumIDs = null;
            IFeature srcFeature = null;
            IFeature destFeature = null;     
            ITopologicalOperator4 srcTopoOperator = null;
            ITopologicalOperator4 destTopoOperator = null;
            IRow destRow = null;
            ISet destRowSet = new SetClass();

            IGeometryBag geometryBag = new GeometryBagClass();
            IGeoDataset geoDataset = (IGeoDataset)targetFcClass;
            geometryBag.SpatialReference = geoDataset.SpatialReference;
            IGeometryCollection geometryCol = (IGeometryCollection)geometryBag;
            object missing = Type.Missing;

            enumIDs = selectionSet.IDs;
            enumIDs.Reset();
            int iD = enumIDs.Next();

            int count = 0;

            try
            {
                //編集オペレーションの開始（undo/redoの始点）
                m_engineEditor.StartOperation();

                GISLight10.Common.Logger.Info("マージの編集オペレーション：開始");

                while (iD != -1)
                {
                    if (count == 0)
                    {
                        //一つ目のフィーチャ
                        srcFeature = targetFcClass.GetFeature(iD);

                        srcTopoOperator = (ITopologicalOperator4)srcFeature.Shape;
                        srcTopoOperator.IsKnownSimple_2 = false;
                        srcTopoOperator.Simplify();                                                

                        //IGeometryCollectionにジオメトリを追加
                        geometryCol.AddGeometry(srcFeature.ShapeCopy, ref missing, ref missing);                        
                    }
                    else
                    {
                        //二つ目以降のフィーチャ
                        //IGeometryCollectionにジオメトリを加えていく
                        destFeature = targetFcClass.GetFeature(iD);

                        destTopoOperator = (ITopologicalOperator4)destFeature.Shape;
                        destTopoOperator.IsKnownSimple_2 = false;
                        destTopoOperator.Simplify();
                        
                        //IGeometryCollectionにジオメトリを追加
                        geometryCol.AddGeometry(destFeature.Shape, ref missing, ref missing);

                        //最後に削除する用に格納
                        destRow = (IRow)destFeature;
                        destRowSet.Add(destRow);                        
                    }

                    iD = enumIDs.Next();
                    count++;
                }
                

                //属性の編集
                ESRIJapan.GISLight10.Common.Logger.Info("属性編集の実行");
                MergeAttribute(srcFeature, destRowSet, targetFcClass);

                //マージの実行
                srcTopoOperator.ConstructUnion((IEnumGeometry)geometryCol);

                //フィーチャのジオメトリ を マージ結果のジオメトリで上書き                
                srcFeature.Shape = (IGeometry)srcTopoOperator;                
                srcFeature.Store();

                //マージされたフィーチャの削除
                IFeatureEdit destFeatureEdit = (IFeatureEdit)destFeature;
                destFeatureEdit.DeleteSet(destRowSet);

                //マージ後のフィーチャのフラッシュ
                IActiveView activeView = m_engineEditor.Map as IActiveView;
                FlashGeometry(srcFeature.Shape, activeView.ScreenDisplay);

                //編集オペレーションの終了（undo/redoの終点）
                m_engineEditor.StopOperation("merge");

				// 単体ﾃｽﾄ･確認ｺｰﾄﾞ
				//IRow srcRow = (IRow)srcFeature;
				//for(int intCnt=0; intCnt < srcRow.Fields.FieldCount; intCnt++) {
				//    string	strTemp = srcRow.Fields.get_Field(intCnt).Name + " = " + srcRow.get_Value(intCnt).ToString();
				//}

                GISLight10.Common.Logger.Info("マージの編集オペレーション：正常終了");                
            }
            catch (COMException comExc)
            {
                m_engineEditor.AbortOperation();
                GISLight10.Common.Logger.Info("マージの編集オペレーション：アボート");

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(comExc.Message);
                GISLight10.Common.Logger.Error(comExc.StackTrace);
            }
            catch (Exception e)
            {
                m_engineEditor.AbortOperation();
                GISLight10.Common.Logger.Info("マージの編集オペレーション：アボート");

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(e.Message);
                GISLight10.Common.Logger.Error(e.StackTrace);
            }
            finally
            {
                IActiveView activeView = m_engineEditor.Map as IActiveView;
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography | 
                    esriViewDrawPhase.esriViewGeoSelection, null, activeView.Extent);

                ComReleaser.ReleaseCOMObject(selectionSet);
                ComReleaser.ReleaseCOMObject(targetFcClass);
                ComReleaser.ReleaseCOMObject(srcFeature);
                ComReleaser.ReleaseCOMObject(destFeature);
                ComReleaser.ReleaseCOMObject(srcTopoOperator);
                ComReleaser.ReleaseCOMObject(destTopoOperator);
                ComReleaser.ReleaseCOMObject(destRow);
                ComReleaser.ReleaseCOMObject(destRowSet);
                ComReleaser.ReleaseCOMObject(geometryBag);
                ComReleaser.ReleaseCOMObject(geoDataset);
                ComReleaser.ReleaseCOMObject(geometryCol);               
            }            
        }
        #endregion

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {   
                //編集が開始されている場合
                if(m_engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                    IFeatureLayer targetFcLayer = GetEditTargetLayer();

                    //編集ターゲットレイヤがポリゴン かつ ラスタカタログでない場合
                    if ((targetFcLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) &&
                        !(targetFcLayer is GdbRasterCatalogLayer))
                    {
                        ISelectionSet selectionSet = GetTargetSelectionSet();

                        //編集ターゲットレイヤでフィーチャが2つ以上選択されている場合
                        if (selectionSet.Count >= 2) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }                
            }
        }

        /// <summary>
        /// 編集ターゲットレイヤを取得
        /// </summary>
        /// <returns>編集ターゲットレイヤ</returns>
        private IFeatureLayer GetEditTargetLayer()
        {
            //編集ターゲットレイヤの取得
            IEngineEditLayers editLayer = (IEngineEditLayers)m_engineEditor;
            IFeatureLayer targetFcLayer = editLayer.TargetLayer;

            return targetFcLayer;
        }

        /// <summary>
        /// 編集ターゲットレイヤのセレクションセットを取得
        /// </summary>
        /// <returns>セレクションセット</returns>
        private ISelectionSet GetTargetSelectionSet()
        {
            //編集ターゲットレイヤの取得
            IFeatureLayer targetFcLayer = GetEditTargetLayer();

            //セレクションセットの取得
            IFeatureSelection fcSelection = (IFeatureSelection)targetFcLayer;
            ISelectionSet selectionSet = fcSelection.SelectionSet;
            
            return selectionSet;
        }

        /// <summary>
        /// 編集ターゲットレイヤのフィーチャクラスを取得
        /// </summary>
        /// <returns>編集ターゲットレイヤのフィーチャクラス</returns>
        private IFeatureClass GetTargetFeatureClass()
        {            
            //編集ターゲットレイヤの取得
            IFeatureLayer targetFcLayer = GetEditTargetLayer();
  
            //フィーチャクラスの取得
            IFeatureClass targetFcClass = targetFcLayer.FeatureClass;

            return targetFcClass;
        }

        /// <summary>
        /// 編集ポリシーに従って属性値をマージする。
        /// </summary>
        /// <param name="srcFeature">マージ元のフィーチャ</param>
        /// <param name="destRowSet">マージ先のフィーチャを格納したISet</param>
        /// <param name="targetFC">編集対象レイヤ</param>
        private void MergeAttribute(IFeature srcFeature, ISet destRowSet, IFeatureClass targetFC)
        {
            SplitAndMargeSettings splitAndMergeSetting = null;

            //設定ファイルが存在するか確認する
            if(!ApplicationInitializer.IsUserSettingsExists()) {
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
                     "[ マージの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ マージの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }

            string editorMergeNumPolicy = "";
            string editorMergeDatePolicy = "";
            string editorMergeOtherPolicy = "";
            bool	blnDomainFollow = false;		// ﾄﾞﾒｲﾝ･ﾏｰｼﾞのﾌｫﾛｰ制御

            //数値型フィールドのポリシー
            try {
                editorMergeNumPolicy = splitAndMergeSetting.EditorMargeNumField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ マージ：数値型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ マージ：数値型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }
            
            //日付型フィールドのポリシー
            try {
                editorMergeDatePolicy = splitAndMergeSetting.EditorMargeDateField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ マージ：日付型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ マージ：日付型フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }
            
            //その他フィールドのポリシー
            try {
                editorMergeOtherPolicy = splitAndMergeSetting.EditorMargeField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ マージ：その他フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ マージ：その他フィールドの属性編集方法 ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }            

            IRow srcRow = srcFeature.Table.GetRow(srcFeature.OID); 

            Dictionary<string,IFeature>	dicMaxAndMinAreaFeature = null;
            IFeature		maxAreaFeature = null;
            IFeature		minAreaFeature = null;
            IFeatureBuffer	sumAttrebuteFeature = null;
            IRow			maxRow = null;
            IRow			minRow = null;
            IRow			sumRow = null;
            IField			field = null;

            try {
                //面積が最大と最小のフィーチャを求める
                if ((editorMergeNumPolicy.Equals("0") || editorMergeDatePolicy.Equals("0") || editorMergeOtherPolicy.Equals("0")) ||
                    (editorMergeNumPolicy.Equals("1") || editorMergeDatePolicy.Equals("1") || editorMergeOtherPolicy.Equals("1")))
                {
                    //maxAreaFeature = GetMaxAreaFeature(srcFeature, destRowSet);
                    dicMaxAndMinAreaFeature = GetMaxAreaFeature(srcFeature, destRowSet);

                    maxAreaFeature = dicMaxAndMinAreaFeature["MaxAreaFeature"];
                    maxRow = maxAreaFeature.Table.GetRow(maxAreaFeature.OID);

                    minAreaFeature = dicMaxAndMinAreaFeature["MinAreaFeature"];
                    minRow = minAreaFeature.Table.GetRow(minAreaFeature.OID);
                }

                //フィーチャの属性値を合計する ※ﾏｰｼﾞ･ﾎﾟﾘｼｰの合算が機能しない件の補填処理を追加 
                if (editorMergeNumPolicy.Equals("2") || DomainManager.UsedSplitDomain(targetFC)) {
                    sumAttrebuteFeature = GetSumAttributeFeature2(srcFeature, destRowSet, targetFC);
                    sumRow = (IRow)sumAttrebuteFeature;
                }

                //フィールドをループする
                for(int i = 0; i < srcFeature.Fields.FieldCount; i++) {
                    field = srcFeature.Fields.get_Field(i);

                    //Shape_Length、Shape_Area、OID、ジオメトリフィールドはスキップする                    
                    if (field.Equals(targetFC.LengthField) ||
                        field.Equals(targetFC.AreaField) ||
                        field.Type == esriFieldType.esriFieldTypeOID ||
                        field.Type == esriFieldType.esriFieldTypeGeometry)
                    {                        
                        continue;
                    }

                    //フィールドに対してドメイン設定のポリシーが設定されている場合もスキップする
                    if(field.Domain != null) {
                        // ﾏｰｼﾞ･ﾎﾟﾘｼｰの合算が機能しない件の補填処理 
                        if(field.Domain.Name.StartsWith(DomainManager.DOMAIN_NAME_SPLIT) && 
							field.Domain.MergePolicy == esriMergePolicyType.esriMPTSumValues) {
	                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーあり");
							blnDomainFollow = true;
						}
						else {
	                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーあり");
							continue;
                        }
                    }
                    else {
                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーなし");
                    }                    

                    //※ 編集ポリシーの説明
                    // ■数値型フィールド
                    //   ①EditorMargeNumField == 0の場合
                    //      面積が最大のフィーチャの属性値を使用する。
                    //   ②EditorMargeNumField == 1の場合
                    //      面積が最小のフィーチャの属性値を使用する。
                    //   ③EditorMargeNumField == 2の場合
                    //      属性値を合計した値を使用する。
                    // ■日付型フィールド
                    //   ④EditorMargeDateField == 0の場合
                    //      面積が最大のフィーチャの属性値を使用する。
                    //   ⑤EditorMargeDateField == 1の場合
                    //      面積が最小のフィーチャの属性値を使用する。
                    //   ⑥EditorMargeDateField == 2の場合
                    //      マージした日時を使用する。
                    // ■その他のフィールド
                    //   ⑦EditorMargeField == 0の場合
                    //      面積が最大のフィーチャの属性値を使用する。
                    //   ⑧EditorMargeField == 1の場合
                    //      面積が最小のフィーチャの属性値を使用する。
                    //
                    if (field.Type.Equals(esriFieldType.esriFieldTypeDouble) ||
                        field.Type.Equals(esriFieldType.esriFieldTypeInteger) ||
                        field.Type.Equals(esriFieldType.esriFieldTypeSingle) ||
                        field.Type.Equals(esriFieldType.esriFieldTypeSmallInteger))
                    {
                        // 合算補助処理
                        if(blnDomainFollow) {
							srcRow.set_Value(i, sumRow.get_Value(i));
							blnDomainFollow = false;
                        }
                        else {
							switch (editorMergeNumPolicy)
							{
								case "0":
									srcRow.set_Value(i, maxRow.get_Value(i));
									break;
								case "1":
									srcRow.set_Value(i, minRow.get_Value(i));
									break;
								case "2":
									srcRow.set_Value(i, sumRow.get_Value(i));
									break;
								default:
									break;
							}
                        }
                    }
                    else if (field.Type.Equals(esriFieldType.esriFieldTypeDate))
                    {
                        switch (editorMergeDatePolicy)
                        {
                            case "0":
                                srcRow.set_Value(i, maxRow.get_Value(i));
                                break;
                            case "1":
                                srcRow.set_Value(i, minRow.get_Value(i));
                                break;
                            case "2":
                                srcRow.set_Value(i, System.DateTime.Now);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (editorMergeOtherPolicy)
                        {
                            case "0":
                                srcRow.set_Value(i, maxRow.get_Value(i));
                                break;
                            case "1":
                                srcRow.set_Value(i, minRow.get_Value(i));
                                break;
                            default:
                                break;
                        }
                    }
                }

                srcRow.Store();
            }
            catch(COMException comex) {
                throw comex;
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                //ComReleaser.ReleaseCOMObject(srcRow);
                //ComReleaser.ReleaseCOMObject(dicMaxAndMinAreaFeature);
                //ComReleaser.ReleaseCOMObject(maxAreaFeature);
                //ComReleaser.ReleaseCOMObject(minAreaFeature);
                //ComReleaser.ReleaseCOMObject(sumAttrebuteFeature);
                //ComReleaser.ReleaseCOMObject(maxRow);
                //ComReleaser.ReleaseCOMObject(minRow);
                //ComReleaser.ReleaseCOMObject(sumRow);
                ComReleaser.ReleaseCOMObject(field);                
            }
        }


        /// <summary>
        /// 面積が最大のフィーチャを取得
        /// </summary>
        /// <param name="srcFeature">マージ元のフィーチャ</param>
        /// <param name="destRowSet">マージ先のフィーチャを格納したISet</param>
        /// <returns>面積が最大のフィーチャ</returns>
        private Dictionary<string, IFeature> GetMaxAreaFeature(IFeature srcFeature, ISet destRowSet)
        {
            Dictionary<string,IFeature> dicMaxAndMinAreaFeature = 
                new Dictionary<string,IFeature>();

            IFeature maxAreaFeature = srcFeature;
            IFeature minAreaFeature = srcFeature;
            destRowSet.Reset();

            IArea maxArea;
            IArea minArea;
            IArea challengerArea;
            IFeature challengerFeature;

            for (int i = 0; i < destRowSet.Count; i++)
            {
                maxArea = (IArea)maxAreaFeature.Shape;
                minArea = (IArea)minAreaFeature.Shape;

                challengerFeature = (IFeature)destRowSet.Next();
                challengerArea = (IArea)challengerFeature.Shape;

                //最大を求める
                if (maxArea.Area < challengerArea.Area)
                {
                    maxAreaFeature = challengerFeature;
                }
                
                //最小を求める
                if (minArea.Area > challengerArea.Area)
                {
                    minAreaFeature = challengerFeature;
                }
            }

            dicMaxAndMinAreaFeature.Add("MaxAreaFeature", maxAreaFeature);
            dicMaxAndMinAreaFeature.Add("MinAreaFeature", minAreaFeature);

            return dicMaxAndMinAreaFeature;
        }        


        /// <summary>
        /// 数値型のフィールドの属性値を合計したフィーチャを取得
        /// </summary>
        /// <param name="srcFeature">マージ元のフィーチャ</param>
        /// <param name="destRowSet">マージ先のフィーチャを格納したISet</param>
        /// <param name="targetFC">編集対象レイヤ</param>
        /// <returns>数値型のフィールドの属性値を合計したフィーチャ</returns>
        private IFeature GetSumAttributeFeature(IFeature srcFeature, ISet destRowSet, IFeatureClass targetFC)
        {
            //オブジェクトのコピー（値渡し）
            //ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = 
            //    new ESRI.ArcGIS.esriSystem.ObjectCopyClass();
            //IFeature sumAttributeFeature = objectCopy.Copy(srcFeature) as IFeature;

            //参照渡し
            IFeature sumAttributeFeature = srcFeature;

            destRowSet.Reset();

            IRow sumRow = sumAttributeFeature.Table.GetRow(srcFeature.OID);
            
            IFeature feature;
            IRow row;
            IField field;

			// 属性値の合算
            for(int i = 0; i < destRowSet.Count; i++) {
                feature = (IFeature)destRowSet.Next();
                row = feature.Table.GetRow(feature.OID);

                for(int j = 0; j < sumAttributeFeature.Fields.FieldCount; j++) {
                    field = sumAttributeFeature.Fields.get_Field(j);

                    //Shape_Length、Shape_Area、OID、ジオメトリフィールドはスキップする
                    if(field.Equals(targetFC.LengthField) ||
                        field.Equals(targetFC.AreaField) ||
                        field.Type == esriFieldType.esriFieldTypeOID ||
                        field.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }

                    //フィールドに対してドメイン設定のポリシーが設定されている場合もスキップする
                    if(field.Domain != null) {
						// ﾏｰｼﾞ･ﾎﾟﾘｼｰの合算が機能しない件の補填処理
                        if(DomainManager.UsedSplitDomain(targetFC) && 
							(field.Domain.Name.StartsWith(DomainManager.DOMAIN_NAME_SPLIT) && 
								field.Domain.MergePolicy == esriMergePolicyType.esriMPTSumValues)) {
	                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーあり");
	                    }
	                    else {
							GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーあり");
							continue;
                        }
                    }
                    else {
                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーなし");
                    }     

                    if(!sumRow.get_Value(j).Equals(System.DBNull.Value) &&
                                !row.get_Value(j).Equals(System.DBNull.Value))
                    {
                        switch (field.Type)
                        {
                            case esriFieldType.esriFieldTypeDouble:                                
                                double doubleValue = (double)sumRow.get_Value(j) + (double)row.get_Value(j);
                                sumRow.set_Value(j, doubleValue);                               
                                break;

                            case esriFieldType.esriFieldTypeInteger:                                
                                Int32 longValue = (Int32)sumRow.get_Value(j) + (Int32)row.get_Value(j);
                                sumRow.set_Value(j, longValue);                                
                                break;

                            case esriFieldType.esriFieldTypeSingle:                                
                                Single floatValue = (Single)sumRow.get_Value(j) + (Single)row.get_Value(j);
                                sumRow.set_Value(j, floatValue);                                
                                break;

                            case esriFieldType.esriFieldTypeSmallInteger:                                
                                int intValue = (Int16)sumRow.get_Value(j) + (Int16)row.get_Value(j);
                                sumRow.set_Value(j, intValue);                                
                                break;

                            default:
                                break;
                        }
                    }
                }
                sumRow.Store();
            }

            return sumAttributeFeature;
        }        

        /// <summary>
        /// 数値型のフィールドの属性値を合計したフィーチャを取得
        /// </summary>
        /// <param name="srcFeature">マージ元のフィーチャ</param>
        /// <param name="destRowSet">マージ先のフィーチャを格納したISet</param>
        /// <param name="targetFC">編集対象レイヤ</param>
        /// <returns>数値型のフィールドの属性値を合計したフィーチャ</returns>
        private IFeatureBuffer GetSumAttributeFeature2(IFeature srcFeature, ISet destRowSet, IFeatureClass targetFC) {
            // ※参照渡しでは最大、最小の属性も合算されてしまう為、ﾊﾞｯﾌｧを作成して対応
            IFeatureBuffer	sumAttributeFeature = targetFC.CreateFeatureBuffer();
            IRow			sumRow = (IRow)sumAttributeFeature;
            
            IRow			row;
            IField			field;

			// 属性値の合算 (一時的にｿｰｽを対象に含める)
            destRowSet.Add(srcFeature);
            destRowSet.Reset();
            for(int i = 0; i < destRowSet.Count; i++) {
                row = (IRow)destRowSet.Next();

                for(int j = 0; j < sumAttributeFeature.Fields.FieldCount; j++) {
                    field = sumAttributeFeature.Fields.get_Field(j);

                    //Shape_Length、Shape_Area、OID、ジオメトリフィールドはスキップする
                    if(field.Equals(targetFC.LengthField) ||
                        field.Equals(targetFC.AreaField) ||
                        field.Type == esriFieldType.esriFieldTypeOID ||
                        field.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }

                    //フィールドに対してドメイン設定のポリシーが設定されている場合もスキップする
                    if(field.Domain != null) {
						// ﾏｰｼﾞ･ﾎﾟﾘｼｰの合算が機能しない件の補填処理 
                        if(DomainManager.UsedSplitDomain(targetFC) && 
							(field.Domain.Name.StartsWith(DomainManager.DOMAIN_NAME_SPLIT) && 
								field.Domain.MergePolicy == esriMergePolicyType.esriMPTSumValues)) {
	                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーあり");
	                    }
	                    else {
							GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーあり");
							continue;
                        }
                    }
                    else {
                        GISLight10.Common.Logger.Info(field.Name + ": ドメインポリシーなし");
                    }     

                    if(!row.get_Value(j).Equals(System.DBNull.Value)) {
						// 集計値を取得
						object objVal = sumRow.get_Value(j);
						if(objVal.Equals(DBNull.Value)) {
							objVal = 0;
						}
						
                        // 数値ﾌｨｰﾙﾄﾞだけを対象
                        switch(field.Type) {
                        case esriFieldType.esriFieldTypeDouble:
                            double doubleValue = Convert.ToDouble(objVal) + Convert.ToDouble(row.get_Value(j));
                            sumRow.set_Value(j, doubleValue);                               
                            break;

                        case esriFieldType.esriFieldTypeInteger:
                            Int32 longValue = Convert.ToInt32(objVal) + Convert.ToInt32(row.get_Value(j));
                            sumRow.set_Value(j, longValue);                                
                            break;

                        case esriFieldType.esriFieldTypeSingle:
                            Single floatValue = Convert.ToSingle(objVal) + Convert.ToSingle(row.get_Value(j));
                            sumRow.set_Value(j, floatValue);                                
                            break;

                        case esriFieldType.esriFieldTypeSmallInteger:
                            int intValue = Convert.ToInt16(objVal) + Convert.ToInt16(row.get_Value(j));
                            sumRow.set_Value(j, intValue);                                
                            break;

                        default:
                            break;
                        }
                    }
                }
                //sumRow.Store();
            }
            
            // ｿｰｽを対象から外す → 外しておかないと後で削除されてしまうため
            destRowSet.Remove(srcFeature);

            return sumAttributeFeature;
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