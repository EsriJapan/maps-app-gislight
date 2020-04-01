using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.Ui {
	public partial class FormTableView : Form {
		private IMap			_agMap = null;
		private IMapControl3	_agMapCtl = null;
		private IToolbarMenu2	_agToolMenu = null;
		private Ui.MainForm		_MainForm = null;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FormTableView(IMapControl3 MapCtl) {
			InitializeComponent();
			
			this._agMapCtl = MapCtl;
			this._agMap = MapCtl.Map;
			
            // ﾒｲﾝ･ﾌｫｰﾑを取得
            Control	ctlMapCtl = Control.FromHandle((IntPtr)MapCtl.hWnd);
            _MainForm = (Ui.MainForm)ctlMapCtl.FindForm();
		}

        /// <summary>
        /// 選択テーブルを設定します
        /// </summary>
        /// <param name="StdTblUnit"></param>
        private void SetSelectedTableUnit(Common.StandaloneTableUnitClass StdTblUnit) {
			// 選択ﾃｰﾌﾞﾙを通知 (Nullでも渡す)
			if(StdTblUnit != null && StdTblUnit.StdTable.Valid) {
				_MainForm.SelectedTable = StdTblUnit.StdTable;
			}
			else {
				_MainForm.SelectedTable = null;
			}
        }

		private void Form_Load(object sender, EventArgs e) {
			// ｲﾒｰｼﾞ･ｱｲｺﾝの関連付け
			Ui.LightCatalogView	uiLCView = new LightCatalogView();
			this.treeView1.ImageList = uiLCView.imgListIcon;
			
			// ﾃﾞｰﾀｿｰｽ･ﾋﾞｭｰを構成
			this.LoadTables(_agMap);
			
			// ﾎﾟｯﾌﾟｱｯﾌﾟ･ﾒﾆｭｰ構成
			_agToolMenu = new ToolbarMenuClass();
            /* ﾌｨｰﾙﾄﾞ別名設定 */
            _agToolMenu.AddItem(new EngineCommand.DefineFieldNameAliasCommandForStd(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            /* 属性検索 */
            _agToolMenu.AddItem(new EngineCommand.AttributeSearchCommandForStd(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            /* ﾌｨｰﾙﾄﾞ演算 */
            //_agToolMenu.AddItem(new EngineCommand.FieldCalculatorForStd(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
			/* 属性ﾃｰﾌﾞﾙ */
			_agToolMenu.AddItem(new EngineCommand.OpenStandaloneTable(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
			_agToolMenu.SetHook(this._agMapCtl);
			
			// 選択ﾃｰﾌﾞﾙをｸﾘｱ
			if(_MainForm.SelectedTable != null) {
				_MainForm.SelectedTable = null;
			}
		}
		
		private void Form_FormClosed(object sender, FormClosedEventArgs e) {
			// 選択ﾃｰﾌﾞﾙをｸﾘｱ
			if(_MainForm.SelectedTable != null) {
				_MainForm.SelectedTable = null;
			}
		}
		
		private void LoadTables(IMap TargetMap) {
			IStandaloneTableCollection	agSTblColl = TargetMap as IStandaloneTableCollection;
			IStandaloneTable			agStdTbl = null;
			
			// ﾄｯﾌﾟﾉｰﾄﾞを追加
			TreeNode	nodeRoot = new TreeNode("テーブル", 
						(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.StandaloneTable__Root,
						(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.StandaloneTable__Root) {
				Name = "",
			};

			this.treeView1.Nodes.Clear();
			this.treeView1.Nodes.Add(nodeRoot);
			
			if(agSTblColl != null && agSTblColl.StandaloneTableCount > 0) {
				// ﾃﾞｰﾀｿｰｽごとに分類
				Dictionary<string, List<Common.StandaloneTableUnitClass>>	dicDS = new Dictionary<string,List<Common.StandaloneTableUnitClass>>();
				IDataset							agDS = null;
				IWorkspace							agWS = null;
				string								strWSName = "";
				
				for(int intCnt=0; intCnt < agSTblColl.StandaloneTableCount; intCnt++) {
					agStdTbl = agSTblColl.get_StandaloneTable(intCnt);
					
					// ﾃｰﾌﾞﾙ単位を構成
					Common.StandaloneTableUnitClass clsST = new Common.StandaloneTableUnitClass() {
						TableName = agStdTbl.Name,
						StdTable = agStdTbl
					};

					// ﾃﾞｰﾀｾｯﾄを取得
					if(agStdTbl.Valid) {
						agDS = agStdTbl.Table as IDataset;
						agWS = agDS.Workspace;
					
						strWSName = Common.StandaloneTableUnitClass.GetWorkspaceBrowseName(agWS);
					}
					else {
						strWSName = "<不明>";
					}
					
					// ﾜｰｸｽﾍﾟｰｽの表記をｾｯﾄ
					clsST.WorkSpacePath = strWSName;
					
					if(!dicDS.ContainsKey(strWSName)) {
						dicDS.Add(strWSName, new List<Common.StandaloneTableUnitClass>());
					}
					dicDS[strWSName].Add(clsST);
				}
				
				// ﾂﾘｰを構成
				List<TreeNode>	nodeTbls = new List<TreeNode>();
				foreach(KeyValuePair<string,List<Common.StandaloneTableUnitClass>> kvp in dicDS) {
					// ﾃｰﾌﾞﾙ･ﾂﾘｰを追加
					foreach(Common.StandaloneTableUnitClass clsTbl in kvp.Value) {
						nodeTbls.Add(new TreeNode(clsTbl.TableName, 
							(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.TableStandalone,
							(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.TableStandalone) {
							Name = clsTbl.TableName,
							Tag = clsTbl	/* ﾀｸﾞにﾃｰﾌﾞﾙを潜ませる */
						});
					}
					
					// ﾃﾞｰﾀｿｰｽ･ﾂﾘｰを追加
					nodeRoot.Nodes.Add(new TreeNode(kvp.Key, 
								(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.Geodatabase,
								(int)ESRIJapan.GISLight10.Ui.LightCatalogView.ICON.Geodatabase,
								nodeTbls.ToArray()) {
						Name = kvp.Key,
						ForeColor = kvp.Key.Equals("<不明>") ? Color.Red : Color.Black,
					});
					
					nodeTbls.Clear();
				}
				
				// ﾉｰﾄﾞを展開
				nodeRoot.ExpandAll();
			}
		}

		/// <summary>
		/// テーブル・ツリー マウスダウン イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tree_MouseDown(object sender, MouseEventArgs e) {
			TreeView	ctlTree = sender as TreeView;
			
			// ｸﾘｯｸ位置のﾉｰﾄﾞを取得
			TreeViewHitTestInfo	hitTree = ctlTree.HitTest(e.X, e.Y);
			if(hitTree.Node != null) {
				// 選択ﾃｰﾌﾞﾙを設定
				this.SetSelectedTableUnit((Common.StandaloneTableUnitClass)hitTree.Node.Tag);

				// ｺﾝﾃｷｽﾄ表示
				if(hitTree.Node.Tag != null && e.Button == MouseButtons.Right) {

#region テストコード
	#if TEST
					// ﾌﾟﾛﾊﾟﾃｨ - 表示
					IDisplayString	agDisStr = (hitTree.Node.Tag as Common.StandaloneTableUnitClass).StdTable as IDisplayString;
					IDisplayExpressionProperties	agDisExProp = agDisStr.ExpressionProperties;
					string	strTemp = agDisExProp.Expression;
					
					ITableDefinition	agTblDef = (hitTree.Node.Tag as Common.StandaloneTableUnitClass).StdTable as ITableDefinition;
					strTemp = agTblDef.DefinitionExpression;
					
					IRelationshipClassCollection	agRelClsColl = (hitTree.Node.Tag as Common.StandaloneTableUnitClass).StdTable as IRelationshipClassCollection;
					IEnumRelationshipClass			agEnumRelCls = agRelClsColl.RelationshipClasses;
					IRelationshipClass3				agRelCls;
					while((agRelCls = agEnumRelCls.Next() as IRelationshipClass3) != null) {
						strTemp = agRelCls.DestinationPrimaryKey;
					}
					
					IDisplayRelationshipClass		agDisRel = (hitTree.Node.Tag as Common.StandaloneTableUnitClass).StdTable as IDisplayRelationshipClass;
					agRelCls = agDisRel.RelationshipClass as IRelationshipClass3;
					if(agRelCls != null) {
						strTemp = agRelCls.DestinationPrimaryKey;
					}
					
					IDataLayer2	agDataLayer = (hitTree.Node.Tag as Common.StandaloneTableUnitClass).StdTable as IDataLayer2;
					strTemp = agDataLayer.DataSourceName.NameString;
					if(agDataLayer.RelativeBase != "") {
						
					}
					
					IDataset	agDS = (hitTree.Node.Tag as Common.StandaloneTableUnitClass).StdTable.Table as IDataset;
					if(agDS.Workspace is ISqlWorkspace) {
						ISqlWorkspace	agSqlWS = agDS.Workspace as ISqlWorkspace;
					}
					if(agDS.Subsets != null) {
						IEnumDataset	agEnumDSs = agDS.Subsets;
						IDataset		agTempDS;
						while((agTempDS = agEnumDSs.Next()) != null) {
							strTemp = agTempDS.Name;
						}
					}
					if(agDS.FullName != null) {
						IName		agName = agDS.FullName;
						IQueryName2	agQName = agName as IQueryName2;
						if(agQName != null) {
							IQueryDef2	agQDef = agQName.QueryDef as IQueryDef2;
							strTemp = agQDef.Tables;
							strTemp = agQName.PrimaryKey;
						}
					}
					
					IDatabaseConnectionInfo2	agDBConInfo = agDS.Workspace as IDatabaseConnectionInfo2;
					strTemp = agDBConInfo.ConnectedDatabase;
					
					// 接続状態のﾁｪｯｸ
					IWorkspaceFactoryStatus	agWSF_S = agDS.Workspace.WorkspaceFactory as IWorkspaceFactoryStatus;
					try {
						IWorkspaceStatus		agWS_S = agWSF_S.PingWorkspaceStatus(agDS.Workspace);
						if(agWS_S.ConnectionStatus == esriWorkspaceConnectionStatus.esriWCSUp) {
							strTemp = "The workspace is up.";
						}
						else if(agWS_S.ConnectionStatus == esriWorkspaceConnectionStatus.esriWCSDown) {
							strTemp = "The workspace is down and not available.";
						}
						else if(agWS_S.ConnectionStatus == esriWorkspaceConnectionStatus.esriWCSAvailable) {
							strTemp = "The workspace is down, but is available for reconnection.";
						}
					}
					catch(Exception ex) {
						strTemp = ex.Message;
					}
	#endif
#endregion
					this._agToolMenu.PopupMenu(e.X + 5, e.Y, this.treeView1.Handle.ToInt32());
				}
				// ﾉｰﾄﾞを選択
				ctlTree.SelectedNode = hitTree.Node;
			}
		}

		/// <summary>
		/// テーブル・ツリー ノード選択イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tree_AfterSelect(object sender, TreeViewEventArgs e) {
			// 選択ﾃｰﾌﾞﾙを設定
			this.SetSelectedTableUnit((Common.StandaloneTableUnitClass)e.Node.Tag);
		}

	}
	

}
