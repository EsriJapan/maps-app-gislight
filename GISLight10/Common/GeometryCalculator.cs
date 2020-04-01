using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using ESRIJapan.GISLight10.Ui;
using ESRIJapan.GISLight10.Common.Calculator;


namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// ジオメトリ演算をおこなうクラス
    /// </summary>
    public class GeometryCalculator : IGeometryCalculator
    {
        /// <summary>
        /// ジオメトリ演算をおこなうフィーチャレイヤ
        /// </summary>
        protected IFeatureLayer pTargetLayer;

        /// <summary>
        /// ジオメトリ演算結果を格納するフィールド名
        /// </summary>
        protected string targetFieldName;

        /// <summary>
        /// ジオメトリ演算結果の単位
        /// </summary>
        protected Unit targetUnit = Unit.UNKNOWN;

        /// <summary>
        /// 小数点以下の桁数(文字列型のフィールドに出力するときに使用)
        /// </summary>
        protected int decimalPrecision = -1;

        /// <summary>
        /// 単位出力の有無(文字列型のフィールドに出力するときに使用)
        /// </summary>
        protected bool appendUnit = false;

        /// <summary>
        /// 単位変換の係数を保持する辞書
        /// </summary>
        protected IDictionary<Unit, double> dicUnitConvert;

        /// <summary>
        /// 単位の文字列を保持する辞書
        /// </summary>
        protected IDictionary<Unit, string> dicUnitLabel;


        private System.Windows.Forms.Form _calcForm = null;

        /// <summary>
        /// クラスコンストラクタ。
        /// クラスメンバ変数の初期化をおこなう。
        /// </summary>
        public GeometryCalculator()
        {
            int i, num;
            pTargetLayer = null;
            targetFieldName = null;

            Unit[] unitDef2 = new Unit[] { Unit.KMETER, Unit.METER, Unit.KMETER2, Unit.METER2, Unit.CMETER, Unit.A, Unit.HA };
            string[] unitLabel = {  Properties.Resources.FormGeometryCalculate_UnitLabel_Km,
                                    Properties.Resources.FormGeometryCalculate_UnitLabel_M,
                                    Properties.Resources.FormGeometryCalculate_UnitLabel_SqKm,
                                    Properties.Resources.FormGeometryCalculate_UnitLabel_SqM,
                                    Properties.Resources.FormGeometryCalculate_UnitLabel_Cm,
                                    Properties.Resources.FormGeometryCalculate_UnitLabel_A,
                                    Properties.Resources.FormGeometryCalculate_UnitLabel_Ha };

            Unit[] unitDef = new Unit[] { Unit.KMETER, Unit.METER, Unit.METER2, Unit.KMETER2, Unit.CMETER, Unit.A, Unit.HA };
            double[] unitConv = { 1F / 1000F, 1F, 1F, 1F / (1000F * 1000F), 100F, 1F / 100F, 1F / (100F * 100F) };

            // 単位の変換係数設定
            num = unitDef.Length;
            dicUnitConvert = new Dictionary<Unit, double>();
            for (i = 0; i < num; i++)
            {
                dicUnitConvert.Add(new KeyValuePair<Unit, double>(unitDef[i], unitConv[i]));
            }

            // 単位文字列の設定
            num = unitDef2.Length;
            dicUnitLabel = new Dictionary<Unit, string>();
            for (i = 0; i < num; i++)
            {
                dicUnitLabel.Add(new KeyValuePair<Unit, string>(unitDef2[i], unitLabel[i]));
            }
        }


        public GeometryCalculator(System.Windows.Forms.Form calcForm) : this()
        {
            _calcForm = calcForm;
        }

        /// <summary>
        /// ジオメトリ演算をおこなうレイヤ
        /// </summary>
        public IFeatureLayer TargetLayer
        {
            set
            {
                pTargetLayer = value;
            }
        }


        /// <summary>
        /// ジオメトリ演算結果を格納するフィールドの名前
        /// </summary>
        public string TargetFieldName
        {
            set
            {
                targetFieldName = value;
            }
        }


        /// <summary>
        /// ジオメトリ演算結果の単位
        /// </summary>
        public Unit ToUnit
        {
            set
            {
                targetUnit = value;
            }
        }


        /// <summary>
        /// 小数点以下の桁数
        /// </summary>
        public int DecimalPrecision
        {
            set
            {
                decimalPrecision = value;
            }
        }

        /// <summary>
        /// 単位文字列を結果に付けるかの有無
        /// </summary>
        public bool AppendUnit
        {
            set
            {
                appendUnit = value;
            }
        }


        /// <summary>
        /// ジオメトリ演算を実行する
        /// </summary>
        /// <param name="processUnit">ジオメトリ演算処理</param>
        /// <returns>エラー発生時のエラーメッセージ</returns>
        public string Calculate(IGeometryCalculatorUnit processUnit)
        {

            ProgressDialog pd = null;

            bool isStringField, isShapefile;
            int index, count = 0, fieldLen = 0;
            double val;
            string work, errorMessage = null;
            IFeatureCursor pCursor = null;
            IFeature pFeature = null;
            ILinearUnit pLinearUnit;
            IWorkspace pWorkspace;
            IQueryFilter pQueryFilter = null;
            IFeatureLayerDefinition pFeatureLayerDefinition;

            try
            {


                // 編集セッションを開始できるかチェック
                pWorkspace = Common.LayerManager.getWorkspace(pTargetLayer.FeatureClass);
                if (canStartEditing(pWorkspace) == false)
                {
                    // 編集セッション開始不可能
                    return Properties.Resources.FormGeometryCalculate_WARNING_WarkspaceLocked;
                }

                isShapefile = isShapefileWorkspace(pWorkspace);

                int max = 100;
                int calcnt = 0;

                if (isShapefile == true)
                {
                    // シェープファイルの場合（処理速度重視）
                    pFeatureLayerDefinition = (IFeatureLayerDefinition)pTargetLayer;
                    pQueryFilter = new QueryFilterClass();
                    pQueryFilter.WhereClause = pFeatureLayerDefinition.DefinitionExpression;
                    pQueryFilter.SubFields = string.Format("{0},{1}",
                            pTargetLayer.FeatureClass.ShapeFieldName, targetFieldName);
                    pCursor = pTargetLayer.FeatureClass.Update(pQueryFilter, false);
                    max = pTargetLayer.FeatureClass.FeatureCount(pQueryFilter);
                }
                else
                {
                    // シェープファイル以外（シンプルフィーチャ以外の場合があるため）
                    pCursor = pTargetLayer.Search(null, false);
                    max = pTargetLayer.FeatureClass.FeatureCount(null);
                }

                index = pCursor.FindField(targetFieldName);
                isStringField = pCursor.Fields.get_Field(index).Type == esriFieldType.esriFieldTypeString;
                if (isStringField == true)
                {
                    fieldLen = pCursor.Fields.get_Field(index).Length;
                }

                pLinearUnit = getLinerUnit(pTargetLayer.FeatureClass);

                // 進捗を追加
                pd = new ProgressDialog();
                pd.Minimum = 0;
                pd.Maximum = max;
                pd.CancelEnable = false;
                if (_calcForm != null)
                {
                    pd.Title = string.Format("{0}", _calcForm.Text);
                    pd.Show(_calcForm);
                }
                else
                {
                    pd.Title = "面積・長さ計算";
                    pd.Show();
                }


                pFeature = pCursor.NextFeature();
                while (pFeature != null)
                {
                    pd.Value = calcnt + 1;
                    pd.Message = string.Format("{0} / {1}件の計算処理中・・・", calcnt + 1, max);

                    val = processUnit.Calculate(pFeature);
                    if (isStringField == true)
                    {
                        work = toString(pLinearUnit, targetUnit, val, appendUnit);
                        if (fieldLen < work.Length)
                        {
                            // 文字数が多い場合
                            if (errorMessage == null)
                            {
                                errorMessage = Properties.Resources.FormGeometryCalculate_WARNING_Length;
                            }
                            work = work.Substring(0, fieldLen - 1) + "*";
                        }
                        pFeature.set_Value(index, work);
                    }
                    else
                    {
                        val = convert(pLinearUnit, targetUnit, val);
                        try
                        {
                            pFeature.set_Value(index, val);
                        }
                        catch (COMException)
                        {
                            // 値が入らなかったときの処理
                            pFeature.set_Value(index, -1);
                            if (errorMessage == null)
                            {
                                errorMessage = Properties.Resources.FormGeometryCalculate_WARNING_Length2;
                            }
                        }
                    }

                    try
                    {
                        if (isShapefile == true)
                        {
                            pCursor.UpdateFeature(pFeature);
                        }
                        else
                        {
                            pFeature.Store();
                        }
                    }
                    catch(Exception ex)
                    {
                        // 更新に失敗した場合
                        Logger.Info("フィーチャのストアに失敗", ex);
                        return Properties.Resources.FormGeometryCalculate_WARNING_WarkspaceLocked;
                    }


                    pFeature = pCursor.NextFeature();
                    calcnt++;

                    count++;
                    if (count % 10 == 0)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        if (count % 1000 == 0)
                        {
                            if (isShapefile == true)
                            {
                                pCursor.Flush();
                            }
                            count = 0;
                        }
                    }

                }

                return errorMessage;
            }
            finally
            {
                if (pCursor != null)
                {
                    ComReleaser.ReleaseCOMObject(pCursor);
                }
                if (pd != null) 
                {
                    pd.Close();
                }

            }
        }


        /// <summary>
        /// フィーチャクラスに定義されている空間参照から距離単位を取得する。
        /// 空間参照が地理座標系の場合、nullを返す。
        /// </summary>
        /// <param name="pFeatureClass">距離単位を取得するフィーチャクラス</param>
        /// <returns>フィーチャクラスに設定されている単位</returns>
        protected ILinearUnit getLinerUnit(IFeatureClass pFeatureClass)
        {
            int indexGeoField;
            string geoFieldName;
            IField pField;
            IGeometryDef pGeoDef;
            ISpatialReference pSR;
            IProjectedCoordinateSystem pPCS;

            geoFieldName = pFeatureClass.ShapeFieldName;
            indexGeoField = pFeatureClass.FindField(geoFieldName);
            pField = pFeatureClass.Fields.get_Field(indexGeoField);

            pGeoDef = pField.GeometryDef;
            pSR = pGeoDef.SpatialReference;
            pPCS = pSR as IProjectedCoordinateSystem;

            return pPCS == null ? null : pPCS.CoordinateUnit;
        }


        /// <summary>
        /// 単位変換をおこなう。
        /// </summary>
        /// <param name="sourceUnit">変換元の単位</param>
        /// <param name="key">変換先の単位</param>
        /// <param name="source">単位を変換する値</param>
        /// <returns>変換結果</returns>
        protected double convert(ILinearUnit sourceUnit, Unit key, double source)
        {
            double meterPerUnit;

            if (dicUnitConvert.ContainsKey(key) == false)
            {
                // システムエラー
                throw new NotImplementedException("サポートしていない単位の変換です");
            }

            meterPerUnit = sourceUnit.MetersPerUnit;

            return meterPerUnit * source * dicUnitConvert[key];
        }


        /// <summary>
        /// 単位変換して文字列にする
        /// </summary>
        /// <param name="sourceUnit">変換元の単位</param>
        /// <param name="key">変換先の単位</param>
        /// <param name="source">単位を変換する値</param>
        /// <param name="useUnit">変換後の文字列に単位を付けるかのフラグ</param>
        /// <returns>変換結果文字列</returns>
        protected string toString(ILinearUnit sourceUnit, Unit key, double source, bool useUnit)
        {
            const string FORMAT2 = "{0} {1}";
            double val;
            string unitLabel;

            val = convert(sourceUnit, key, source);
            unitLabel = dicUnitLabel[key];

            if (decimalPrecision != -1)
            {
                //// 切り上げの場合
                //long work;
                //work = (long)Math.Pow(10, decimalPrecision);
                //val = Math.Ceiling(val * work) / work;
                // 四捨五入の場合
                val = Math.Round(val, decimalPrecision);
            }

            return useUnit ? string.Format(FORMAT2, val, unitLabel) : val.ToString();
        }


        /// <summary>
        /// 編集セッションを開始できるかチェックする
        /// </summary>
        /// <param name="pWorkspace">チェックするワークスペース</param>
        /// <returns>編集セッションを開始できる場合はtrue</returns>
        protected bool canStartEditing(IWorkspace pWorkspace)
        {
            IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)pWorkspace;

            // 編集セッションチェック
            try
            {
                pWorkspaceEdit.StartEditing(false);
                pWorkspaceEdit.StopEditing(false);
            }
            catch (Exception ex)
            {
                Logger.Info("ワークスペースの編集セッション開始に失敗", ex);
                return false;
            }

            return true;
        }


        /// <summary>
        /// ワークスペースがシェープファイルであるか判断する
        /// </summary>
        /// <param name="pWorkspace">判断するワークスペース</param>
        /// <returns>シェープファイルの場合、true</returns>
        protected bool isShapefileWorkspace(IWorkspace pWorkspace)
        {
            const string PROGID_SHAPE = "esriDataSourcesFile.ShapefileWorkspaceFactory";
            IDataset pDataset;
            IWorkspaceName pWorkspaceName;

            pDataset = (IDataset)pWorkspace;
            pWorkspaceName = (IWorkspaceName)pDataset.FullName;
            
            return pWorkspaceName.WorkspaceFactoryProgID.Contains(PROGID_SHAPE);
        }
    }
}
