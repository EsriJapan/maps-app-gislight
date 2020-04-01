using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using ESRIJapan.GISLight10.Common.Calculator;
using ESRIJapan.GISLight10.Ui;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 選択フィーチャに対してジオメトリ演算をおこなうクラス。
    /// </summary>
    public class GeometryCalculator2 : GeometryCalculator, IGeometryCalculator
    {
        private System.Windows.Forms.Form _calcForm = null;


        /// <summary>
        /// クラスのコンストラクタ。
        /// </summary>
        public GeometryCalculator2() : base()
        {
        }

        public GeometryCalculator2(System.Windows.Forms.Form calcForm) : this()
        {
            _calcForm = calcForm;
        }

        /// <summary>
        /// ジオメトリ演算を実行する
        /// </summary>
        /// <param name="processUnit">ジオメトリ演算処理</param>
        /// <returns>エラー発生時のエラーメッセージ</returns>
        public new string Calculate(IGeometryCalculatorUnit processUnit)
        {
            ProgressDialog pd = null;

            bool isStringField;
            int index, count = 0, fieldLen = 0;
            double val;
            string work, errorMessage = null;
            IFeatureSelection pFeatureSelection;
            ISelectionSet pSelectionSet;
            ICursor pCursor;
            IFeatureCursor pFeatureCursor = null;
            IFeature pFeature = null;
            ILinearUnit pLinearUnit;
            IWorkspace pWorkspace;

            try
            {
                // 編集セッションを開始できるかチェック
                pWorkspace = Common.LayerManager.getWorkspace(pTargetLayer.FeatureClass);
                if (canStartEditing(pWorkspace) == false)
                {
                    // 編集セッション開始不可能
                    return Properties.Resources.FormGeometryCalculate_WARNING_WarkspaceLocked;
                }

                pFeatureSelection = (IFeatureSelection)pTargetLayer;
                pLinearUnit = getLinerUnit(pTargetLayer.FeatureClass); 
                pSelectionSet = pFeatureSelection.SelectionSet;

                pSelectionSet.Search(null, false, out pCursor);

                int max = pSelectionSet.Count;
                int calcnt = 0;
                pd = new ProgressDialog();
                pd.Minimum = 0;
                pd.Maximum = max;
                pd.CancelEnable = false;
                if (_calcForm != null) 
                {
                    pd.Title = string.Format("{0}",_calcForm.Text);
                    pd.Show(_calcForm);
                }
                else 
                {
                    pd.Title = "面積・長さ計算";
                    pd.Show();
                }
                
                pFeatureCursor = (IFeatureCursor)pCursor;
                index = pCursor.FindField(targetFieldName);
                isStringField = pCursor.Fields.get_Field(index).Type == esriFieldType.esriFieldTypeString;
                if (isStringField == true)
                {
                    fieldLen = pCursor.Fields.get_Field(index).Length;
                }

                pFeature = pFeatureCursor.NextFeature();
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
                        pFeature.set_Value(index, val);
                    }

                    pFeature.Store();
                    pFeature = pFeatureCursor.NextFeature();
                    calcnt++;

                    count++;
                    if (count % 10 == 0)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        count = 0;
                    }
                }

                return errorMessage;
            }
            finally
            {
                if (pFeatureCursor != null)
                {
                    ComReleaser.ReleaseCOMObject(pFeatureCursor);
                }
                if (pd != null)
                    pd.Close();
            }
        }
    }
}
