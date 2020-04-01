#define enableSDE

using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// マップに追加されたレイヤ一覧を取得するクラス
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成
    ///  2011/01/21 protectedメンバのスコープ指定記述を削除 
    ///  2011/08/05 SDEレイヤ利用可
    /// </history>
    public class LayerManager
    {
        const string UID_IGEOFEATURELAYER = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
        const string UID_IRASTERLAYER = "{D02371C7-35F7-11D2-B1F2-00C04F8EDEFF}";

        /// <summary>
        /// フィーチャレイヤ一覧の取得       
        /// </summary>
        /// <param name="map">IMap</param>
        /// <returns>フィーチャレイヤ一覧</returns>
        public List<IFeatureLayer> GetFeatureLayers(IMap map)
        {
            List<IFeatureLayer> featureLayerList = new List<IFeatureLayer>();
            
            // 入力ﾁｪｯｸ
            if(map != null) {
				IUID layerUID = new UIDClass();
				layerUID.Value = UID_IGEOFEATURELAYER;

				IEnumLayer enumLayer = map.get_Layers((UID)layerUID, true);
				enumLayer.Reset();
				IFeatureLayer fcLayer = (IFeatureLayer)enumLayer.Next();
	            
				while(fcLayer != null) {
					//フィーチャレイヤ以外のレイヤも取得されるので除去する
					//if (!(fcLayer is GdbRasterCatalogLayer) && !(fcLayer is CadFeatureLayer))
					//ラスタカタログは取得されるように変更（2010/10/28）
					if(!(fcLayer is CadFeatureLayer)) {
						//パス切れレイヤは除去する
						if(fcLayer.Valid == true) {
							//SDEレイヤは除去する
							//SDEレイヤを除去しないように変更（2010/10/28）
							//SDEレイヤを除去するように再度変更（2010/11/12）
							//理由：SDEを編集できないライセンスでリリースするので。
							// ArcGIS10対応では利用する様に変更
#if !enableSDE
							if(getWorkspace(fcLayer).Type != esriWorkspaceType.esriRemoteDatabaseWorkspace)
							{
#endif
                            featureLayerList.Add(fcLayer);
#if !enableSDE
							}                         
#endif
						}                    
					}

					fcLayer = (IFeatureLayer)enumLayer.Next();
				}
			}
            return featureLayerList;
        }


        // 2012/08/06 ADD 
        /// <summary>
        /// レイヤ一覧の取得（ラスタも取得）       
        /// </summary>
        /// <param name="map">IMap</param>
        /// <returns>レイヤ一覧</returns>
        public List<ILayer> GetAllLayers(IMap map)
        {
            List<ILayer> LayerList = new List<ILayer>();

            if(map != null) {
				IEnumLayer enumLayer = map.get_Layers(null, true);
				enumLayer.Reset();
				ILayer fcLayer = (ILayer)enumLayer.Next();

				while(fcLayer != null) {
					//ラスタカタログは取得されるように変更（2010/10/28）
					if(!(fcLayer is CadFeatureLayer)) {
						//パス切れレイヤは除去する
						if(fcLayer.Valid == true) {
							//SDEレイヤは除去する
							//SDEレイヤを除去しないように変更（2010/10/28）
							//SDEレイヤを除去するように再度変更（2010/11/12）
							//理由：SDEを編集できないライセンスでリリースするので。
							// ArcGIS10対応では利用する様に変更
#if !enableSDE
                            if(getWorkspace(fcLayer).Type != esriWorkspaceType.esriRemoteDatabaseWorkspace)
                            {
#endif
							LayerList.Add(fcLayer);
#if !enableSDE
                            }                         
#endif
						}
					}

					fcLayer = (ILayer)enumLayer.Next();
				}
            }
            return LayerList;
        }

        /// <summary>
        /// フィーチャレイヤ判定
        /// </summary>
        /// <param name="GetLayer">レイヤオブジェクト</param>
        /// <returns>True  - フィーチャレイヤ 
        ///          False - フィーチャレイヤ以外</returns>
        public bool IsFeatureLayer(ILayer GetLayer) {
			bool	blnRet = false;
            try {
                IGeoFeatureLayer pGEoFeatureLayer = GetLayer as IGeoFeatureLayer;
                blnRet = (pGEoFeatureLayer != null);
            }
            catch {
                //
            }
            
            return blnRet;
        }

        // 2012/08/06 ADD 

        /// <summary>
        /// ラスタレイヤ一覧の取得
        /// </summary>
        /// <param name="map">IMap</param>
        /// <returns>ラスタレイヤ一覧</returns>
        public List<IRasterLayer> GetRasterLayers(IMap map) {
            List<IRasterLayer>	rasterLayerList = new List<IRasterLayer>();

            if(map != null) {
				IUID layerUID = new UIDClass();
				layerUID.Value = UID_IRASTERLAYER;

				IEnumLayer enumLayer = map.get_Layers((UID)layerUID, true);
				enumLayer.Reset();

				IRasterLayer rasterLayer = (IRasterLayer)enumLayer.Next();
				while (rasterLayer != null) {
					//パス切れレイヤは除去する
					if(rasterLayer.Valid) {
						//SDEレイヤは除去する
						//SDEレイヤを除去しないように変更（2010/10/28）
						//SDEレイヤを除去するように再度変更（2010/11/12）
						//理由：SDEを編集できないライセンスでリリースするので。
						// ArcGIS10対応では利用する様に変更
#if !enableSDE
                    if(getWorkspace(rasterLayer).Type != esriWorkspaceType.esriRemoteDatabaseWorkspace)
                    {
#endif
						rasterLayerList.Add(rasterLayer);
#if !enableSDE
                    }                         
#endif
					}                    

					rasterLayer = (IRasterLayer)enumLayer.Next();
				}
            }
            return rasterLayerList;
        }

        /// <summary>
        /// フィーチャレイヤーが選択状態のフィーチャを持っているか判断する
        /// </summary>
        /// <param name="pFeatureLayer">選択状態をチェックするフィーチャレイヤ</param>
        /// <returns>選択状態のフィーチャを持っている場合、true</returns>
        public static bool hasSelectedFeature(IFeatureLayer pFeatureLayer)
        {
            IFeatureSelection pFeatureSelection;
            ISelectionSet pSelectionSet;

            if (pFeatureLayer == null)
            {
                // 後でResouceファイルからメッセージ文字列を取ってくるように変更する
                throw new ArgumentException("NULLの引数でメソッドが呼ばれました");
            }

            if (!(pFeatureLayer is IFeatureSelection))
            {
                // 後でResouceファイルからメッセージ文字列を取ってくるように変更する
                throw new ArgumentException(
                    string.Format("ISelectionSetのインタフェースを実装しない{0}でメソッドが呼ばれました",
                    pFeatureLayer.Name));
            }

            pFeatureSelection = (IFeatureSelection)pFeatureLayer;
            pSelectionSet = pFeatureSelection.SelectionSet;

            return pSelectionSet.Count != 0;
        }        


        /// <summary>
        /// FeatureLayerが持つFeatureClassのWorkspaceを返す
        /// </summary>
        /// <param name="pFeatureLayer">Workspaceを取得するFeatureLayer</param>
        /// <returns>FeatureLayerのWorkspace</returns>
        public static IWorkspace getWorkspace(IFeatureLayer pFeatureLayer)
        {
            return getWorkspace(pFeatureLayer.FeatureClass);
        }


        /// <summary>
        /// FeatureClassが持つWorkspaceを返す
        /// </summary>
        /// <param name="pFeatureClass">Workspaceを取得するFeatureClass</param>
        /// <returns>FeatureClassのWorkspace</returns>
        public static IWorkspace getWorkspace(IFeatureClass pFeatureClass)
        {
            IDataset pDataset;

            pDataset = (IDataset)pFeatureClass;

            return pDataset.Workspace;
        }

		/// <summary>
		/// ワークスペースがジオデータベースかどうかチェックします。
		/// </summary>
		/// <param name="WorkSpace"></param>
		/// <returns>True:ｼﾞｵDB / False:その他</returns>
		public static bool IsGeoDB(IWorkspace WorkSpace) {
			bool	blnRet = false;
			
			// 3段ﾁｪｯｸ
			if(!WorkSpace.IsDirectory()) {		// ※GeoDBは、ここでﾁｪｯｸOK (MDB,GDBの場合、false)
				blnRet = true;
			}
			else if(WorkSpace.Type != esriWorkspaceType.esriFileSystemWorkspace) {
				blnRet = true;
			}
			else {
				// 拡張子を取得
				string	strExt = System.IO.Path.GetExtension(WorkSpace.PathName);
				if(!string.IsNullOrEmpty(strExt)) {
					strExt = strExt.ToLower();
					
					// ﾛｰｶﾙ･GeoDBﾁｪｯｸ
					if(strExt.Equals(".mdb") || strExt.Equals(".gdb")) {
						blnRet = true;
					}
				}
			}
			
			// 返却
			return blnRet;
		}

        /// <summary>
        /// テーブル結合を持つフィーチャレイヤ一覧の取得
        /// </summary>
        /// <param name="map">IMap</param>
        /// <returns>テーブル結合を持つフィーチャレイヤ一覧</returns>
        public List<IFeatureLayer> GetHasJoinLayers(IMap map)
        {
            List<IFeatureLayer> fcLayerList = GetFeatureLayers(map);
            List<IFeatureLayer> hasJoinLayerList = new List<IFeatureLayer>();

            for (int i = 0; i < fcLayerList.Count; i++)
            {
                IDisplayTable displayTable = (IDisplayTable)fcLayerList[i];
                ITable table = displayTable.DisplayTable;

                //テーブル結合を持っていない場合は、nullになる。
                IRelQueryTable relQueryTable = table as IRelQueryTable; 

                if (relQueryTable != null)
                {
                    hasJoinLayerList.Add(fcLayerList[i]);
                }    
            }

            return hasJoinLayerList;
        }


        /// <summary>
        /// リレートを持つフィーチャレイヤ一覧の取得
        /// </summary>
        /// <param name="map">IMap</param>
        /// <returns>リレートを持つフィーチャレイヤ一覧</returns>
        public List<IFeatureLayer> GetHasRelateLayers(IMap map)
        {
            List<IFeatureLayer> fcLayerList = GetFeatureLayers(map);
            List<IFeatureLayer> hasRelateLayerList = new List<IFeatureLayer>();

            for (int i = 0; i < fcLayerList.Count; i++)
            {
                //リレート元レイヤのRelationshipClass一覧の取得
                IRelationshipClassCollection relClassCol = (IRelationshipClassCollection)fcLayerList[i];
                IEnumRelationshipClass enumRelClass = relClassCol.RelationshipClasses;

                enumRelClass.Reset();
                IRelationshipClass relClass = enumRelClass.Next();

                //リレートを持っていない場合は、null
                if (relClass != null)
                {
                    hasRelateLayerList.Add(fcLayerList[i]);
                }
            }

            return hasRelateLayerList;
        }
    }
}
