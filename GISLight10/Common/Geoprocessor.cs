using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;  

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// コンボボックスにFeatureLayerオブジェクトをセット
    /// </summary>
    /// 
    public static class ComboBoxFeatureLayer
    {
        public static string FileNameFullPath(ILayer layer)
        {
            IFeatureLayer pFeatureLayer = (IFeatureLayer)layer;

            IDataset pDataset = (IDataset)pFeatureLayer.FeatureClass;
            IWorkspace pWorkspace = pDataset.Workspace;

            IUID pUID = pWorkspace.WorkspaceFactory.GetClassID();
            string ext = "";
            if (pUID.Value.Equals((string)"{A06ADB96-D95C-11D1-AA81-00C04FA33A15}"))
            {
                ext = ".shp";
            }

            string fullPath = pWorkspace.PathName + "\\" + pDataset.Name + ext;
            return fullPath;

        }
    }

    public static class Check
    {
        /// <summary>
        /// 出力フィーチャクラス名のバリデート
        /// </summary>
        /// <param name="WorkspaceName">ワークスペース フルパス名</param>
        /// <param name="FeatureClassName">フィーチャクラス名</param>
        /// <param name="IsOverwrite">上書き保存を許可</param>
        /// <returns></returns>
        public static string FeatureClassString(string WorkspaceName, string FeatureClassName, bool IsOverwrite)
        {
            string returnValue = "";
            IWorkspaceName pWorkspaceName;

            //WorkspaceNameの取得
            pWorkspaceName = getWorkspaceName(WorkspaceName, "esriDataSourcesGDB.FileGDBWorkspaceFactory");
            if (pWorkspaceName == null)
            {
                pWorkspaceName = getWorkspaceName(WorkspaceName, "esriDataSourcesFile.ShapefileWorkspaceFactory");
            }
            if (pWorkspaceName == null)
            {
                pWorkspaceName = getWorkspaceName(WorkspaceName, "esriDataSourcesGDB.AccessWorkspaceFactory");
            }
            if (pWorkspaceName == null)
            {
                returnValue = "・出力ワークスペースが不正です。";
                return returnValue;
            }

            //フィーチャクラス名から.shpを除外
            string strNewName = FeatureClassName.Replace(".shp", "");

            if (strNewName == "")
            {
                returnValue = "・フィーチャクラス名が空白です。";
                return returnValue;
            }

            try
            {
                if (IsOverwrite == false)   //フィーチャクラスの上書き禁止
                {
                    IDatasetName pFeatureClassName = new FeatureClassNameClass();
                    pFeatureClassName.Name = FeatureClassName;
                    pFeatureClassName.WorkspaceName = pWorkspaceName;
                    IName pName = (IName)pFeatureClassName;

                    pName.Open();
                    returnValue += "・フィーチャクラスがすでに存在します。";
                    return returnValue;
                }
            }
            catch
            {
            }
            finally
            {
                //フィーチャクラスの上書き許可
                if (System.Text.RegularExpressions.Regex.Match(strNewName, @"(\!|\#|\$|\%|\&|\'|\(|\)|\=|\~|\||\`|\{|\+|\*|\}|\<|\>|\?|\-|\^|\@|\[|\;|\:|\]|\,|\/|\\)|\ |\.").Success == true)
                {
                    returnValue += "・フィーチャクラス名にはアンダーバー（_）以外の半角記号を使用することはできません。";
                }
                else if (char.IsNumber(strNewName, 0) == true)
                {
                    returnValue += "・フィーチャクラス名を半角数字から始めることはできません。";
                }
                else
                {
                    //正常値
                    returnValue = "";
                }
            }

            return returnValue;
        }


        /// <summary>
        /// パス名からWorkspaceNameオブジェクトを取得
        /// </summary>
        /// <param name="strWorkspace">ワークスペース フルパス</param>
        /// <param name="strProgID">ProgID</param>
        /// <returns></returns>
        private static IWorkspaceName getWorkspaceName(string strWorkspace, string strProgID)
        {
            try
            {
                IWorkspaceName pWorkspaceName = new WorkspaceNameClass();
                pWorkspaceName.WorkspaceFactoryProgID = strProgID;
                pWorkspaceName.PathName = strWorkspace;
                IName pName = (IName)pWorkspaceName;
                pName.Open();

                return pWorkspaceName;
            }
            catch
            {
                return null;
            }
        }
    }
}
