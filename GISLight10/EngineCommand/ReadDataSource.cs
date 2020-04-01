// Copyright 2006 ESRI
//
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
//
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
//
// See use restrictions at /arcgis/developerkit/userestrictions.
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using System.Drawing;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// データファイルの読み込みコマンド
    /// </summary>
    /// <history>
    ///  2012-07-10 新規作成
    /// </history>
    public sealed class ReadDataSource : BaseCommand  
	{
		private IMapControl3	m_mapControl;
        private Ui.MainForm		mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public ReadDataSource() {
			base.m_caption = "データソースの読み込み";
            base.m_toolTip = "CAD データの追加";
			
			if(base.m_name == "") {
			}
		}
	
        /// <summary>
        /// クリック時処理
        /// レイヤ削除実行
        /// </summary>
		public override void OnClick() {
			OpenFileDialog	openFileDialog = new OpenFileDialog();
			//openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			openFileDialog.Title = "CAD データファイルを開く";
			openFileDialog.Filter = "CAD データファイル |*.dxf;*.dwg;*.dgn";
			openFileDialog.ShowReadOnly = true;
			openFileDialog.Multiselect = true;
			openFileDialog.CheckPathExists = true;

			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				// 複数のﾌｧｲﾙ選択に対応
				foreach(string strCad in openFileDialog.FileNames) {
					try {
						// 地図に展開
						this.AddCadGroupLayer(strCad);
					}
					catch(Exception ex) {
						ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
						ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);
					}
				}
			}
		}
	
        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
		}

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
				return true;
            }
        }
        
        /// <summary>
        /// CADデータセット名オブジェクトを取得します
        /// </summary>
        /// <param name="CadFilePath">CADファイル・パス</param>
        /// <returns>IDatasetName</returns>
        private IDatasetName GetCadDatasetName(string CadFilePath) {
			IWorkspaceFactory	pWSFact = ESRIJapan.GISLight10.Common.SingletonUtility.NewCadWorkspaceFactory();
			IFeatureWorkspace	pFWS = (IFeatureWorkspace)pWSFact.OpenFromFile(System.IO.Path.GetDirectoryName(CadFilePath), 0);
			IDataset			pDS = (IDataset)pFWS.OpenFeatureClass(System.IO.Path.GetFileName(CadFilePath));
			IWorkspaceName		pWSName = (IWorkspaceName)pDS.FullName;
			
			// 返却
			return (IDatasetName)pWSName;
        }
        
        /// <summary>
        /// CADドローイング・レイヤーを地図に追加します
        /// </summary>
        /// <param name="CadFile"></param>
        /// <returns></returns>
        private bool AddCadDrawingLayer(string CadFilePath) {
			bool	blnRet = true;
			
			ICadDrawingWorkspace	pCadWS = null;
			ICadDrawingDataset		pCadDS = null;
			
			try {
				IWorkspaceFactory	pWSFact = ESRIJapan.GISLight10.Common.SingletonUtility.NewCadWorkspaceFactory();
				pCadWS = (ICadDrawingWorkspace)pWSFact.OpenFromFile(System.IO.Path.GetDirectoryName(CadFilePath), 0);
				pCadDS = pCadWS.OpenCadDrawingDataset(System.IO.Path.GetFileName(CadFilePath));

				// CADﾚｲﾔｰを取得
				ICadLayer		pCadLayer = new CadLayerClass();
				pCadLayer.CadDrawingDataset = pCadDS;
				pCadLayer.Name = System.IO.Path.GetFileName(CadFilePath);
				
				// 地図に展開
				m_mapControl.AddLayer(pCadLayer, 0);
			}
			catch(Exception ex) {
				blnRet = false;
				// ｴﾗｰ内容を表示
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(CadFilePath + "ファイルの読み込みに失敗しました：" + Environment.NewLine + ex.Message);
			}
			finally {
				// COM解放
				if(pCadDS != null) {
					Marshal.ReleaseComObject(pCadDS);
				}
				if(pCadWS != null) {
					Marshal.ReleaseComObject(pCadWS);
				}
			}
			
			// 返却
			return blnRet;
        }
        
        /// <summary>
        /// CADグループレイヤーを地図に追加します
        /// </summary>
        /// <param name="CadFile"></param>
        /// <returns></returns>
        private bool AddCadGroupLayer(string CadFilePath) {
			bool	blnRet = true;
			
			IFeatureWorkspace	pCadWS = null;
			IFeatureDataset		pCadDS = null;
			
			try {
				string			strCadName = System.IO.Path.GetFileName(CadFilePath);

				IWorkspaceFactory	pWSFact = ESRIJapan.GISLight10.Common.SingletonUtility.NewCadWorkspaceFactory();
				pCadWS = (IFeatureWorkspace)pWSFact.OpenFromFile(System.IO.Path.GetDirectoryName(CadFilePath), 0);
				pCadDS = pCadWS.OpenFeatureDataset(strCadName);

				IGroupLayer		pGrpLayer = new GroupLayerClass();
				pGrpLayer.Name = strCadName;
				
				//
				IEnumDataset		pEnumDs = pCadDS.Subsets;
				pEnumDs.Reset();
				string				strFCName;

				IDataset			pDS = pEnumDs.Next();
				while(pDS != null) {
					IFeatureLayer	pFLayer = new FeatureLayerClass();
					pFLayer.FeatureClass = (IFeatureClass)pDS;
					
					// ﾃﾞｰﾀｾｯﾄ名を取得
					strFCName = ((IFeatureClass)pDS).AliasName;
					if(string.IsNullOrEmpty(strFCName)) {
						strFCName = strCadName;
					}
					pFLayer.Name = strFCName;
					
					// ｸﾞﾙｰﾌﾟに追加
					pGrpLayer.Add(pFLayer);
					
					// 次へ
					pDS = pEnumDs.Next();
				}
				
				// 地図に展開
				m_mapControl.AddLayer(pGrpLayer, 0);
				
				Marshal.ReleaseComObject(pEnumDs);
			}
			catch(Exception ex) {
				blnRet = false;
				// ｴﾗｰ内容を表示
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(CadFilePath + "ファイルの読み込みに失敗しました：" + Environment.NewLine + ex.Message);
			}
			finally {
				// COM解放
				if(pCadDS != null) {
					Marshal.ReleaseComObject(pCadDS);
				}
				if(pCadWS != null) {
					Marshal.ReleaseComObject(pCadWS);
				}
			}
			
			// 返却
			return blnRet;
        }
	}
}


