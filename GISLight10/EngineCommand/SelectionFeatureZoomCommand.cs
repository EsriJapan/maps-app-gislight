using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

using ESRIJapan.GISLight10.Common;


namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// レイヤの選択フィーチャにズームコマンド
    /// </summary>
    public sealed class SelectionFeatureZoomCommand : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// クラスインスタンス。各種変数の初期化をおこなう。
        /// </summary>
        public SelectionFeatureZoomCommand()
        {
            string bitmapname;
            try
            {
                base.captionName = "選択フィーチャにズーム";
                base.buttonTooltip = "選択フィーチャにズーム";
                // base.categoryName = "レイヤ";

                bitmapname = this.GetType().Name + ".bmp";
                base.buttonImage = new Bitmap(this.GetType(), bitmapname);
                if (buttonImage != null)
                {
                    buttonHandle = buttonImage.GetHbitmap();
                }
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }

        /// <summary>
        /// コマンド起動判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                int index;
                string shapefieldname;
                LayerManager pLayerManager;
                List<IFeatureLayer> featureLayerList;
                IFeatureLayer checkLayer;
                IFeatureClass pFeatureClass;
                IFields pFields;
                IField pField;
                IGeometryDef pGeometryDef;
                IFeatureSelection pFeatureSelection;
                ISpatialReference pMapSr, pLayerSr;

                pLayerManager = new LayerManager();
                featureLayerList = pLayerManager.GetFeatureLayers(m_mapControl.Map);
                checkLayer = this.mainFrm.SelectedLayer as IFeatureLayer;

                if (featureLayerList.Contains(checkLayer))
                {
                    pFeatureSelection = checkLayer as IFeatureSelection;
                    if (pFeatureSelection != null)
                    {
                        if (pFeatureSelection.SelectionSet != null
                                && pFeatureSelection.SelectionSet.Count > 0)
                        {
                            // フィーチャ選択されている場合
                            pMapSr = this.m_mapControl.Map.SpatialReference;
                            pFeatureClass = checkLayer.FeatureClass;
                            shapefieldname = pFeatureClass.ShapeFieldName;
                            pFields = pFeatureClass.Fields;
                            index = pFields.FindField(shapefieldname);
                            pField = pFields.get_Field(index);
                            pGeometryDef = pField.GeometryDef;
                            pLayerSr = pGeometryDef.SpatialReference;
                            if (pMapSr.Name.Equals(pLayerSr.Name) == true)
                            {
                                // 空間参照チェックに通った場合
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// コマンド初期化の時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// コマンドが起動されたときの処理
        /// </summary>
        public override void OnClick()
        {
            IMap pMap;
            IGeometry5 pGeometry;
            IEnvelope pEnvelope;
            IActiveView pActiveView;
            IPoint pCenter;
            //ISpatialReference pFcSrName, pMapSrName;
            //IGeoTransformation pGeoTrans;

            pGeometry = (IGeometry5)createGeometry();
            if (pGeometry == null)
            {
                return;
            }
            pMap = m_mapControl.Map;

            // 投影法を考慮しないのでコメントアウト
            //pFcSrName = pGeometry.SpatialReference;
            //pMapSrName = m_mapControl.Map.SpatialReference;

            //pGeoTrans = getGeoTransformation(pGeometry.SpatialReference, pMap.SpatialReference);

            //project((IGeometry5)pEnvelope, pMap.SpatialReference, pGeoTrans);

            pActiveView = (IActiveView)pMap;
            
            if (pGeometry.Envelope.Width == 0.0 || pGeometry.Envelope.Height == 0.0)
            {
                // 表示範囲の矩形がない場合
                pEnvelope = pActiveView.Extent;
                pCenter = new PointClass();
                pCenter.PutCoords(pGeometry.Envelope.XMax, pGeometry.Envelope.YMax);
                pEnvelope.Offset(pCenter.X - (pEnvelope.XMin + (pEnvelope.Width / 2f)),
                                    pCenter.Y - (pEnvelope.YMin + (pEnvelope.Height / 2f)));
                pEnvelope.Expand(0.5, 0.5, true);
            }
            else
            {
                pEnvelope = pGeometry.Envelope;
                pEnvelope.Expand(1.2, 1.2, true);
                pActiveView.Extent = pEnvelope;
            }

            pActiveView.Extent = pEnvelope;
            pActiveView.Refresh();
        }


        /// <summary>
        /// データフレームに設定されている投影変換の方法の中から、
        /// 適切な投影変換の方法を取得する。
        /// </summary>
        /// <remarks>
        /// 本バージョンでは使用しない。
        /// </remarks>
        /// <param name="featureSr">フィーチャに設定されている空間参照</param>
        /// <param name="mapSr">データフレームに設定されている空間参照</param>
        /// <returns>データフレームに設定されている投影変換の方法</returns>
        private IGeoTransformation getGeoTransformation(ISpatialReference featureSr, ISpatialReference mapSr)
        {
            string fromSrName, toSrName, tempFromSrName, tempToSrName;
            IMap pMap;
            IMapGeographicTransformations pMapGeoTransformations;
            IGeoTransformationOperationSet pGeoTransformationSet;
            esriTransformDirection direction;
            IGeoTransformation pGeoTransformation;
            ISpatialReference pSrFrom, pSrTo;

            if (featureSr.Name.Equals(mapSr.Name) == true)
            {
                // 投影変換が必要ない場合
                return null;
            }

            fromSrName = getGcsName(featureSr);
            toSrName = getGcsName(mapSr);
            
            pMap = m_mapControl.Map;
            pMapGeoTransformations = (IMapGeographicTransformations)pMap;
            pGeoTransformationSet = pMapGeoTransformations.GeographicTransformations;

            pGeoTransformationSet.Reset();
            pGeoTransformationSet.Next(out direction, out pGeoTransformation);
            while (pGeoTransformation != null)
            {
                pGeoTransformation.GetSpatialReferences(out pSrFrom, out pSrTo);
                tempFromSrName = pSrFrom.Name;
                tempToSrName = pSrTo.Name;
                if (fromSrName.Equals(tempFromSrName) == true && toSrName.Equals(tempToSrName) == true ||
                    fromSrName.Equals(tempToSrName) == true && toSrName.Equals(tempFromSrName) == true)
                {
                    return pGeoTransformation;
                }

                pGeoTransformationSet.Next(out direction, out pGeoTransformation);
            }

            // 使える投影変換の方法が見つからなかった場合
            return null;
        }


        /// <summary>
        /// 空間参照の地理座標系の名前を取得する
        /// </summary>
        /// <remarks>
        /// 本バージョンでは使用しない。
        /// </remarks>
        /// <param name="pSr">地理座標系の名前を取得する空間参照</param>
        /// <returns>地理座標系の名前</returns>
        private string getGcsName(ISpatialReference pSr)
        {
            string gcsName = string.Empty;
            IProjectedCoordinateSystem pPcs;
            IGeographicCoordinateSystem pGcs;

            if (pSr is IProjectedCoordinateSystem)
            {
                pPcs = (IProjectedCoordinateSystem)pSr;
            }
            else if (pSr is IGeographicCoordinateSystem)
            {
                pGcs = (IGeographicCoordinateSystem)pSr;
            }

            return gcsName;
        }

        /// <summary>
        /// 選択セットからジオメトリを作成する
        /// </summary>
        /// <returns>選択セットのジオメトリ</returns>
        private IGeometry createGeometry()
        {
            IFeatureLayer pFeatureLayer;
            IFeatureSelection pFeatureSelection;
            ISelectionSet pSelectionSet;
            IEnumGeometryBind pEnumGeometryBind = null;
            IEnumGeometry pEnumGeometry = null;
            IGeometryFactory3 pGeometryFactory = null;
            IGeometry pGeometry = null;

            pFeatureLayer = this.mainFrm.SelectedLayer as IFeatureLayer;
            if (pFeatureLayer == null)
            {
                // レイヤが選択されていない場合、選択レイヤがフィーチャレイヤではない場合
                return null;
            }

            pFeatureSelection = (IFeatureSelection)pFeatureLayer;
            pSelectionSet = pFeatureSelection.SelectionSet;

            pEnumGeometryBind = new EnumFeatureGeometryClass();
            pEnumGeometryBind.BindGeometrySource(null, pSelectionSet);
            pEnumGeometry = (IEnumGeometry)pEnumGeometryBind;
            pGeometryFactory = new GeometryEnvironmentClass();
            pGeometry = pGeometryFactory.CreateGeometryFromEnumerator(pEnumGeometry);

            return pGeometry;
        }


        /// <summary>
        /// 矩形を投影変換する。
        /// </summary>
        /// <remarks>
        /// 本バージョンでは使用しない。
        /// </remarks>
        /// <param name="pGeometry">変換する矩形</param>
        /// <param name="pMapSpatialRef">変換先の空間参照</param>
        /// <param name="pGeoTransformation">使用する投影変換の方法</param>
        private void project(IGeometry5 pGeometry, ISpatialReference pMapSpatialRef, IGeoTransformation pGeoTransformation)
        {
            string envelopeSr, mapSr, fromSrName, toSrName;
            ISpatialReference pFromSr, pToSr;
            esriTransformDirection direction = esriTransformDirection.esriTransformForward;

            mapSr = pMapSpatialRef.Name;
            envelopeSr = pGeometry.SpatialReference.Name;
            if (mapSr.Equals(envelopeSr) == true)
            {
                // 投影変換の必要なし
                return;
            }

            if (pGeoTransformation == null)
            {
                // 投影変換の方法が指定されていない
                pGeometry.Project(pMapSpatialRef);

                return;
            }

            pGeoTransformation.GetSpatialReferences(out pFromSr, out pToSr);
            fromSrName = pFromSr.Name;
            toSrName = pToSr.Name;

            if (envelopeSr.Equals(toSrName) == true && mapSr.Equals(fromSrName) == true)
            {
                direction = esriTransformDirection.esriTransformReverse;
            }

            pGeometry.ProjectEx(pMapSpatialRef, direction, pGeoTransformation, false, 0f, 0f);
        }
    }
}
