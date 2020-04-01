using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.GISClient;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// .NETでNewキーワードを使った場合にときどきRCWでのCASTエラーが発生するときの回避策として使うためのクラス
    /// 基本的にはSingletonのオブジェクトは
    /// 
    /// [参照]
    /// Interacting with singleton objects 
    ///   VS2008:
    ///   ms-help://ESRI.EDNv10.0/ArcObjects_NET/d91e445e-47c5-41ea-94ca-45f945b73c0f.htm
    ///   VS2012:
    ///   http://127.0.0.1:47873/help/1-5044/ms.help?method=page&id=D91E445E-47C5-41EA-94CA-45F945B73C0F&product=VS&productversion=100&locale=en-US&topiclocale=EN-US&topicversion=0&SQM=2
    /// </summary>
    public static class SingletonUtility
    {

        #region 使いそうなものをピックアップ
        // 
        // Class Library ProgID
        // *がついたものはプロセス単位、他のスレッド単位 
        //
        // AccessWorkspaceFactory DataSourcesGDB esriDataSourcesGDB.AccessWorkspaceFactory
        // AppRef* Framework esriFramework.AppRef
        // ArcInfoWorkspaceFactory DataSourcesFile esriDataSourcesFile.ArcInfoWorkspaceFactory
        // CadWorkspaceFactory DataSourcesFile esriDataSourcesFile.CadWorkspaceFactory
        // CommandsEnvironment Controls esriControls.CommandsEnvironment
        // DEGdbUtilities Geodatabase esriGeodatabase.DEGdbUtilities
        // DEUtilities Geoprocessing esriGeoprocessing.DEUtilities
        // EngineEditor Controls esriControls.EngineEditor
        // EnvironmentManager System esriSystem.EnvironmentManager 
        // ExcelWorkspaceFactory DataSourcesOleDB esriDataSourcesOleDB.ExcelWorkspaceFactory
        // FileGDBWorkspaceFactory DataSourcesGDB esriDataSourcesGDB.FileGDBWorkspaceFactory
        // FormatList DataSourcesRaster esriDataSourcesRaster.FormatList
        // GeoDatabaseHelper Geodatabase esriGeodatabase.GeoDatabaseHelper
        // GPHolder Geoprocessing esriGeoprocessing.GPHolder
        // GPServerFunctionFactory Geoprocessing esriGeoprocessing.GPServerFunctionFactory
        // GPUtilities Geoprocessing esriGeoprocessing.GPUtilities
        // IMSWorkspaceFactory GISClient esriGISClient.IMSWorkspaceFactory
        // InMemoryWorkspaceFactory DataSourcesGDB esriDataSourcesGDB.InMemoryWorkspaceFactory
        // LabelEnvironment Carto esriCarto.LabelEnvironment
        // LocatorManager Location esriLocation.LocatorManager
        // MemoryRelationshipClassFactory Geodatabase esriGeodatabase.MemoryRelationshipClassFactory
        // MyPlaceCollection Controls esriControls.MyPlaceCollection 83307360-8be1-4df2-b301-1e619a965b5c ArcGIS Engine 
        // NetCDFWorkspaceFactory DataSourcesNetCDF esriDataSourcesNetCDF.NetCDFWorkspaceFactory
        // OLEDBWorkspaceFactory DataSourcesOleDB esriDataSourcesOleDB.OLEDBWorkspaceFactory 
        // PCCoverageWorkspaceFactory DataSourcesFile esriDataSourcesFile.PCCoverageWorkspaceFactory
        // RasterWorkspaceFactory DataSourcesRaster esriDataSourcesRaster.RasterWorkspaceFactory
        // RelQueryTableFactory Geodatabase esriGeodatabase.RelQueryTableFactory
        // ScratchWorkspaceFactory DataSourcesGDB esriDataSourcesGDB.ScratchWorkspaceFactory
        // SdeWorkspaceFactory DataSourcesGDB esriDataSourcesGDB.SdeWorkspaceFactory
        // SelectionEnvironment Carto esriCarto.SelectionEnvironment
        // ServerStyleGallery Display esriDisplay.ServerStyleGallery
        // ShapefileWorkspaceFactory DataSourcesFile esriDataSourcesFile.ShapefileWorkspaceFactory
        // SpatialReferenceEnvironment Geometry esriGeometry.SpatialReferenceEnvironment
        // SymbologyEnvironment Display esriDisplay.SymbologyEnvironment
        // SystemHelper System esriSystem.SystemHelper
        // TextFileWorkspaceFactory DataSourcesOleDB esriDataSourcesOleDB.TextFileWorkspaceFactory 
        // TexturePersistenceProperties Geometry esriGeometry.TexturePersistenceProperties 
        // TinWorkspaceFactory DataSourcesFile esriDataSourcesFile.TinWorkspaceFactory
        // ToolboxWorkspaceFactory Geoprocessing esriGeoprocessing.ToolboxWorkspaceFactory
        // VpfWorkspaceFactory DataSourcesFile esriDataSourcesFile.VpfWorkspaceFactory
        // WMSConnectionFactory GISClient esriGISClient.WMSConnectionFactory
        // WorkspaceFactory Geodatabase esriGeodatabase.WorkspaceFactory
        // XMLTypeMapper System esriSystem.XMLTypeMapper 
        #endregion

        // ProgID
        const string ProgID_AccessWorkspaceFactory = "esriDataSourcesGDB.AccessWorkspaceFactory";
        const string ProgID_CadWorkspaceFactory = "esriDataSourcesFile.CadWorkspaceFactory";
        const string ProgID_ExcelWorkspaceFactory = "esriDataSourcesOleDB.ExcelWorkspaceFactory";
        const string ProgID_FileGDBWorkspaceFactory = "esriDataSourcesGDB.FileGDBWorkspaceFactory";
        const string ProgID_GeoDatabaseHelper = "esriGeodatabase.GeoDatabaseHelper";
        const string ProgID_GPUtilities = "esriGeoprocessing.GPUtilities";
        const string ProgID_InMemoryWorkspaceFactory = "esriDataSourcesGDB.InMemoryWorkspaceFactory";
        const string ProgID_OLEDBWorkspaceFactory = "esriDataSourcesOleDB.OLEDBWorkspaceFactory";
        const string ProgID_MemoryRelationshipClassFactory = "esriGeodatabase.MemoryRelationshipClassFactory";
        const string ProgID_PCCoverageWorkspaceFactory = "esriDataSourcesFile.PCCoverageWorkspaceFactory";
        const string ProgID_RasterWorkspaceFactory = "esriDataSourcesRaster.RasterWorkspaceFactory";
        const string ProgID_RelQueryTableFactory = "esriGeodatabase.RelQueryTableFactory";
        const string ProgID_ScratchWorkspaceFactory = "esriDataSourcesGDB.ScratchWorkspaceFactory";
        const string ProgID_SdeWorkspaceFactory = "esriDataSourcesGDB.SdeWorkspaceFactory";
        const string ProgID_ShapefileWorkspaceFactory = "esriDataSourcesFile.ShapefileWorkspaceFactory";
        const string ProgID_SpatialReferenceEnvironment = "esriGeometry.SpatialReferenceEnvironment";
        const string ProgID_ServerStyleGallery = "esriDisplay.ServerStyleGallery";
        const string ProgID_SymbologyEnvironment = "esriDisplay.SymbologyEnvironment";
        //const string ProgID_TextFileWorkspaceFactory = "esriDataSourcesOleDB.TextFileWorkspaceFactory";	/* - 10.0 */
		const string ProgID_TextFileWorkspaceFactory = "esriDataSourcesFile.TextFileWorkspaceFactory";		/* 10.2.1- */
        const string ProgID_TinWorkspaceFactory = "esriDataSourcesFile.TinWorkspaceFactory";
        const string ProgID_ToolboxWorkspaceFactory = "esriGeoprocessing.ToolboxWorkspaceFactory";
        const string ProgID_SqlWorkspaceFactory = "esriDataSourcesGDB.SqlWorkspaceFactory";
        const string ProgID_AGSServerConnectionFactory = "esriGISClient.AGSServerConnectionFactory";

        private static IWorkspaceFactory NewWorkspaceFactory(string progID)
        {
            Type t = Type.GetTypeFromProgID(progID);
#if DEBUG
			// ProgID ｴﾗｰ検地用
			if(t == null) {
				System.Diagnostics.Debug.WriteLine("●ERROR (SingletonUtility) ProgID = " + progID);
			}
#endif

            System.Object o = Activator.CreateInstance(t);
            return o as IWorkspaceFactory;
        }

        public static IWorkspaceFactory NewAccessWorkspaceFactory()
        {
            return NewWorkspaceFactory(ProgID_AccessWorkspaceFactory);
        }
        
        public static IWorkspaceFactory NewCadWorkspaceFactory()
        { 
            return NewWorkspaceFactory(ProgID_CadWorkspaceFactory);
        }

        public static IWorkspaceFactory NewExcelWorkspaceFactory()
        { 
            return NewWorkspaceFactory(ProgID_ExcelWorkspaceFactory);
        }

        public static IWorkspaceFactory NewFileGDBWorkspaceFactory()
        {
            return NewWorkspaceFactory(ProgID_FileGDBWorkspaceFactory);
        }
        
        public static IWorkspaceFactory NewInMemoryWorkspaceFactory()
        { 
            return NewWorkspaceFactory(ProgID_InMemoryWorkspaceFactory);
        }

        public static IWorkspaceFactory NewOLEDBWorkspaceFactory()
        {
            return NewWorkspaceFactory(ProgID_OLEDBWorkspaceFactory);
        }

        public static IMemoryRelationshipClassFactory NewMemoryRelationshipClassFactory()
        {
            Type t = Type.GetTypeFromProgID(ProgID_MemoryRelationshipClassFactory);
            System.Object o = Activator.CreateInstance(t);
            return o as IMemoryRelationshipClassFactory;
        }

        public static IWorkspaceFactory NewRasterWorkspaceFactory()
        { 
            return NewWorkspaceFactory(ProgID_RasterWorkspaceFactory);
        }

        public static IWorkspaceFactory NewScratchWorkspaceFactory()
        { 
            return NewWorkspaceFactory(ProgID_ScratchWorkspaceFactory);
        }

        public static IWorkspaceFactory NewSdeWorkspaceFactory()
        { 
            return NewWorkspaceFactory(ProgID_SdeWorkspaceFactory);
        }

        public static IWorkspaceFactory NewShapeFileWorkspaceFactory()
        {
            return NewWorkspaceFactory(ProgID_ShapefileWorkspaceFactory);
        }

        public static IWorkspaceFactory NewTextFileWorkspaceFactory()
        { 
            return NewWorkspaceFactory(ProgID_TextFileWorkspaceFactory);
        }

        public static IWorkspaceFactory NewToolboxWorkspaceFactory()
        {
            return NewWorkspaceFactory(ProgID_ToolboxWorkspaceFactory);
        }
        public static IWorkspaceFactory NewSqlWorkspaceFactory() {
			return NewWorkspaceFactory(ProgID_SqlWorkspaceFactory);
        }

        public static IGeoDatabaseBridge2 NewGeoDatabaseHelper()
        {
            Type t = Type.GetTypeFromProgID(ProgID_GeoDatabaseHelper);
            System.Object o = Activator.CreateInstance(t);
            return o as IGeoDatabaseBridge2;
        }

        public static IGPUtilities3 NewGPUtilities()
        {
            Type t = Type.GetTypeFromProgID(ProgID_GPUtilities);
            System.Object o = Activator.CreateInstance(t);
            return o as IGPUtilities3;
        }

        public static IRelQueryTableFactory NewRelQueryTableFactory()
        {
            Type t = Type.GetTypeFromProgID(ProgID_RelQueryTableFactory);
            System.Object o = Activator.CreateInstance(t);
            return o as IRelQueryTableFactory;
        }

        public static IStyleGallery NewServerStyleGallery()
        {
            Type t = Type.GetTypeFromProgID(ProgID_ServerStyleGallery);
            System.Object o = Activator.CreateInstance(t);
            return o as IStyleGallery;
        }

        public static IAGSServerConnectionFactory NewAGSServerConnectionFactory()
        {
            Type t = Type.GetTypeFromProgID(ProgID_AGSServerConnectionFactory);
            System.Object o = Activator.CreateInstance(t);
            return o as IAGSServerConnectionFactory;
        }

    }


}
