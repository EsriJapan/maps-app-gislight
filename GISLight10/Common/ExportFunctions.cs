using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.ADF;
using System.Collections;
using System.IO;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// シェープファイルエクスポート
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-06-17 レイヤのフィルタ設定対応
    /// </history>
    public static class ExportFunctions
    {
        public enum OutputFormat
        {
            ShapeFile = 0,
            Pgdb = 1,
            Fgdb = 2
        }


        public static bool DeleteDataset(string targetFilePath, OutputFormat format)
        {
            bool blSuccess = false;

            string file = System.IO.Path.GetFileNameWithoutExtension(targetFilePath);
            string dirpath = System.IO.Path.GetDirectoryName(targetFilePath);

            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            IEnumDataset enumDataset = null;
            IDataset dataset = null;

            try 
            {
                switch (format)
                { 
                    case OutputFormat.Fgdb:
                        workspaceFactory = SingletonUtility.NewFileGDBWorkspaceFactory();
                        break;
                    case OutputFormat.Pgdb:
                        workspaceFactory = SingletonUtility.NewAccessWorkspaceFactory();
                        break;
                    case OutputFormat.ShapeFile:
                        workspaceFactory = SingletonUtility.NewShapeFileWorkspaceFactory();
                        break;
                }

                workspace = workspaceFactory.OpenFromFile(dirpath, 0);
                enumDataset = workspace.get_Datasets(esriDatasetType.esriDTAny);
                enumDataset.Reset();
                dataset = enumDataset.Next();
                while (dataset != null)
                {
                    if ( (dataset.Name.Equals(file)) && (dataset.CanDelete()))   
                    {
                        dataset.Delete();
                        blSuccess = true;
                        break;
                    }
                    dataset = enumDataset.Next();
                }


            }
            finally 
            {
                if (dataset != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(dataset);
                if (enumDataset != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumDataset);
                if (workspace != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(workspace);
            }

            return blSuccess;
        }


        public static IFeatureClass CreateFeatureClass(string targetFilePath, List<IField> sourceFieldList, OutputFormat outFormat)
        {
            string file = System.IO.Path.GetFileNameWithoutExtension(targetFilePath);
            string dirpath = System.IO.Path.GetDirectoryName(targetFilePath);

            IWorkspaceFactory workspaceFactory = null;
            switch (outFormat)
            { 
                case OutputFormat.Fgdb:
                    workspaceFactory = SingletonUtility.NewFileGDBWorkspaceFactory();
                    break;
                case OutputFormat.Pgdb:
                    workspaceFactory = SingletonUtility.NewAccessWorkspaceFactory();
                    break;
                case OutputFormat.ShapeFile:
                    workspaceFactory = SingletonUtility.NewShapeFileWorkspaceFactory();
                    break;
            }

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(dirpath, 0);


            
            IObjectClassDescription objectClassDescription = new ESRI.ArcGIS.Geodatabase.FeatureClassDescriptionClass();

            IFields fields = objectClassDescription.RequiredFields;
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            foreach (IField field in sourceFieldList)
            {
                //System.Diagnostics.Debug.WriteLine(field.Required + " : " + field.Name);
                if (field.Type == esriFieldType.esriFieldTypeGeometry)
                { 
                    IGeometryDef geometryDef = field.GeometryDef;
                    IClone geometryDefClone = (IClone)geometryDef;
                    IClone targetGeometryDefClone = geometryDefClone.Clone();
                    IGeometryDef targetGeometryDef = (IGeometryDef)targetGeometryDefClone;

                    for (int i = 0; i < fields.FieldCount; i++)
                    {
                        IField geomField = fields.get_Field(i);
                        if (geomField.Type == esriFieldType.esriFieldTypeGeometry)
                        {
                            ((IFieldEdit)geomField).GeometryDef_2 = targetGeometryDef;
                            break;
                        }
                    }
                }

                if (!field.Required) // 必須以外のフィールドをコピーして作成
                {
                    // ObjectCopyで行った場合でも
                    // 結合先DBFテーブルのOIDがEditable = falseの場合に設定してもcreatefeatureclassで反映されない
                    IFieldEdit newFieldEdit = null;
                    if (field.Editable == false)
                    {
                        newFieldEdit = new FieldClass();
                    }
                    else 
                    {
                        IObjectCopy objectCopy = new ObjectCopyClass();
                        newFieldEdit = objectCopy.Copy(field) as IFieldEdit;
                    }

                    String[] realFieldName = field.Name.Split('.');
                    newFieldEdit.Name_2 = realFieldName[realFieldName.Length - 1];
                    newFieldEdit.Type_2 = field.Type;
                    newFieldEdit.Editable_2 = true;
                    newFieldEdit.AliasName_2 = field.AliasName;

                    fieldsEdit.AddField(newFieldEdit);


                }
                else if (field.Required)
                {
                    String[] realFieldName = field.Name.Split('.');

                    if (realFieldName[realFieldName.Length -1].ToUpper().Equals("SHAPE_AREA") || 
                        (realFieldName[realFieldName.Length -1].ToUpper().Equals("SHAPE_LENGTH")))
                    {
                        IFieldEdit newFieldEdit = new FieldClass();

                        String[] realFieldName2 = realFieldName[realFieldName.Length - 1].Split('_');//field.Name.Split('_');
                        newFieldEdit.Name_2 = realFieldName2[realFieldName2.Length - 1];
                        newFieldEdit.Type_2 = field.Type;
                        newFieldEdit.Editable_2 = true;
                        newFieldEdit.AliasName_2 = field.AliasName;

                        fieldsEdit.AddField(newFieldEdit);
                    }
                }

            }

            for (int i = 0; i < fields.FieldCount; i++)
            {
                System.Diagnostics.Debug.WriteLine(fields.get_Field(i).Name);
                System.Diagnostics.Debug.WriteLine(fields.get_Field(i).Type);
                System.Diagnostics.Debug.WriteLine(fields.get_Field(i).Editable);
            }

            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (IWorkspace)featureWorkspace;
            fieldChecker.Validate(fieldsEdit as IFields, out enumFieldError, out validatedFields);

            for (int i = 0; i < validatedFields.FieldCount; i++)
            {
                System.Diagnostics.Debug.WriteLine(validatedFields.get_Field(i).Name);
                System.Diagnostics.Debug.WriteLine(validatedFields.get_Field(i).Type);
                System.Diagnostics.Debug.WriteLine(validatedFields.get_Field(i).Editable);
            }

            UID CLSID = new UIDClass();
            CLSID.Value = "esriGeoDatabase.Feature";

            return featureWorkspace.CreateFeatureClass(file, 
                validatedFields, CLSID, null, esriFeatureType.esriFTSimple, ((IFeatureClassDescription)objectClassDescription).ShapeFieldName, "");

        }



        /// <summary>
        /// 指定フィーチャレイヤに選択フィーチャが存在するか確認
        /// </summary>
        /// <param name="targetLayer">フィーチャレイヤ</param>
        /// <returns>フィーチャ存在確認結果</returns>
        public static bool IsExistSelectedFeatures(IFeatureLayer2 targetLayer)
        {
            IFeatureLayer2 pFeatureLayer = null;
            IEnumLayer pEnumLayer = targetLayer as IEnumLayer;
            if (pEnumLayer == null)
            {
                pFeatureLayer = targetLayer;
            }
            else
            {
                pEnumLayer.Reset();
                pFeatureLayer = pEnumLayer.Next() as IFeatureLayer2;
            }

            if (pFeatureLayer != null)
            {
                IFeatureSelection pFeatureSelection = pFeatureLayer as IFeatureSelection;
                ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;

                ICursor icur = null;
                pSelectionSet.Search(null, false, out icur);

                IFeatureCursor pFeatureCursor = icur as IFeatureCursor;
                IFeature pFeature = pFeatureCursor.NextFeature();
                if (pFeature != null)
                {
                    return true;
                }
            }
            return false;
        }


    }
}
