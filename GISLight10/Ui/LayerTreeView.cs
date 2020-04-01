using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.Ui {
	public partial class LayerTreeView : UserControl {
		// Image Index
		private enum LayerImage {
			GroupLayer,
			BaseMapLayer,
			PointLayer,
			LineLayer,
			PolygonLayer,
			AnnotationLayer,
			RasterLayer
		}
		
		private IMap			_agMap;
		private ILayer			_agLayer;
		private TreeNode		_nodeLayer;
		
		private bool			_blnIsChecked = false;
		
		/// <summary>
		/// ﾉｰﾄﾞ複数選択制御用クラス
		/// </summary>
		private class MultiSelectStatus {
			/// <summary>
			/// Control Key
			/// </summary>
 			public bool		ControlKey = false;
			/// <summary>
			/// ShiftKey
			/// </summary>
			public bool		ShiftKey = false;
			/// <summary>
			/// KeyBoard - Key Up
			/// </summary>
			public bool		KeyUp = false;
			/// <summary>
			/// KeyBoard - Key Down
			/// </summary>
			public bool		KeyDown = false;
			/// <summary>
			/// Last Select TreeNode
			/// </summary>
			public TreeNode	LastNode = null;
			
			/// <summary>
			/// Control / Shift 押下
			/// </summary>
			public bool IsShiftOrCtl {
				get {
					return this.ControlKey || this.ShiftKey;
				}
			}
			
			/// <summary>
			/// ｷｰﾎﾞｰﾄﾞ操作 ?
			/// </summary>
			public bool IsKeyboard {
				get {
					return this.KeyDown || this.KeyUp;
				}
			}
			
			/// <summary>
			/// Constructor
			/// </summary>
			public MultiSelectStatus() {
				this.ClearStatus();
			}
			
			/// <summary>
			/// Clear Status
			/// </summary>
			public void ClearStatus() {
				this.ControlKey = false;
				this.ShiftKey = false;
				this.KeyUp = false;
				this.KeyDown = false;
				this.LastNode = null;
			}
			
			/// <summary>
			/// ｷｰ操作を記録します
			/// </summary>
			/// <param name="Ctl"></param>
			/// <param name="Shift"></param>
			/// <param name="KDown"></param>
			/// <param name="KUp"></param>
			public void SetKeys(bool Ctl, bool Shift, bool KDown, bool KUp) {
				this.ControlKey = Ctl;
				this.ShiftKey = Shift;
				this.KeyDown = KDown;
				this.KeyUp = KUp;
			}
		}
		private MultiSelectStatus	_clsOperate = new MultiSelectStatus();
		
		/// <summary>
		/// ドラッグ＆ドロップ操作用クラス
		/// </summary>
		private class DragDropManager {
			/// <summary>
			/// 最後に DragOverされたﾉｰﾄﾞを取得します
			/// </summary>
			public TreeNode	PrevOverNode {
				get {
					return this._nodeLast;
				}
			}
			/// <summary>
			/// 下線開始位置 Xｸﾗｲｱﾝﾄ座標
			/// </summary>
			public int			LeftPosition = 0;
			/// <summary>
			/// 移動先相対ﾚﾍﾞﾙ
			/// </summary>
			public int			Level = 0;
			
			private int			_intIndent = 0;
			private int			_intImg = 0;
			private TreeNode	_nodeLast = null;
			
			/// <summary>
			/// ｺﾝｽﾄﾗｸﾀ
			/// </summary>
			/// <param name="TreeIndentWidth">ﾂﾘｰのｲﾝﾃﾞﾝﾄ幅</param>
			/// <param name="ImageWidth">ﾉｰﾄﾞ･ｲﾒｰｼﾞ幅</param>
			public DragDropManager(int TreeIndentWidth, int ImageWidth) {
				this._intIndent = TreeIndentWidth;
				this._intImg = ImageWidth;
			}
			
			/// <summary>
			/// Clear Status
			/// </summary>
			public void ClearStatus() {
				this._nodeLast = null;
				this.LeftPosition = 0;
				this.Level = 0;
			}
			
			/// <summary>
			/// 層の繰り上げ処理
			/// </summary>
			public void PerformDown() {
				++this.Level;
				this.LeftPosition -= this._intIndent;
			}
			
			/// <summary>
			/// 前回描画した下線を消去します
			/// </summary>
			public void ClearPrevLine() {
				if(this.PrevOverNode != null) {
					TreeView	ctlTV = this.PrevOverNode.TreeView;
					Graphics	g = ctlTV.CreateGraphics();
					Rectangle	rect = this.PrevOverNode.Bounds;
					Pen			pen = new Pen(ctlTV.BackColor, 2);
					
					int			intContWidth = ctlTV.ClientSize.Width - 10;
					
					// 下線を消す
					g.DrawLine(pen, this.LeftPosition, rect.Bottom, intContWidth, rect.Bottom);
					pen.Dispose();
					
					this.ClearStatus();
				}
			}
			
			/// <summary>
			/// 移動先を示す下線を描画します
			/// </summary>
			/// <param name="NextNode"></param>
			/// <param name="MousePosition">X座標(ｸﾗｲｱﾝﾄ座標)</param>
			public bool DrawUnderline(TreeNode NextNode, int X, bool IsSame) {
				bool		blnRet = true;
				TreeView	ctlTV = NextNode.TreeView;
				Graphics	g = ctlTV.CreateGraphics();
				int			intContWidth = ctlTV.ClientSize.Width - 10;
				
				// ﾃｷｽﾄ部の範囲を計算
				//SizeF		rectSize = g.MeasureString(NextNode.Text, ctlTV.Font);
				//rect = new Rectangle(NextNode.Bounds.X + (int)rectSize.Width + 1, NextNode.Bounds.Y, (int)rectSize.Width + 1, NextNode.Bounds.Height);
				
				Rectangle	rect = NextNode.Bounds;
				
				// ｸﾞﾙｰﾌﾟ･ﾚｲﾔ && 子ありの場合は、下層に留める
				if(NextNode.Tag is ICompositeLayer && (NextNode.Nodes.Count > 0 && NextNode.IsExpanded)) {
					this.LeftPosition = rect.Left;
				}
				else {
					// 開始ﾎﾟｼﾞｼｮﾝの設定
					if(NextNode.Tag is ICompositeLayer) {
						// 配下から
						this.LeftPosition = rect.Left;
						this.Level = -1;
					}
					else {
						// 下層から
						this.LeftPosition = rect.Left - this._intImg;
					}

					// ﾚﾍﾞﾙ判定
					if(X < this.LeftPosition) {
						TreeNode	nodeCur = NextNode;
						TreeNode	nodePar = NextNode.Parent;
						while(nodePar != null) {
							if(this.Level < 0) {
								// ｸﾞﾙｰﾌﾟ配下から下層へ
								this.PerformDown();
								if(X < this.LeftPosition) {
									continue;
								}
							}
							else if(nodeCur.Index >= nodePar.Nodes.Count - 1) {
								// ﾚﾍﾞﾙを上げる
								this.PerformDown();
								if(X < this.LeftPosition) {
									nodeCur = nodePar;
									nodePar = nodePar.Parent;
									continue;
								}
							}
							break;
						}
					}
				}

				// 同一ﾉｰﾄﾞ時の制御
				if(IsSame) {
					blnRet = this.Level >= 1;
				}
				
				if(blnRet) {
					// 新しい下線を引く
					Pen			pen = new Pen(Color.Black, 2);
					g.DrawLine(pen, this.LeftPosition, rect.Bottom, intContWidth, rect.Bottom);

					pen.Dispose();
				}
				
				// ﾉｰﾄﾞを記録
				this._nodeLast = NextNode;
				
				return blnRet;
			}
		}
		private DragDropManager	_clsD_D = null;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LayerTreeView() {
			InitializeComponent();
			
			// ﾂﾘｰ･ﾋﾞｭｰを構築
			this.treeView1.ImageList = this.imageList_LayerTypes;
			this.treeView1.ShowNodeToolTips = false;
			this.treeView1.ItemHeight = 21;
			this.treeView1.Indent = 22;
			this.treeView1.LineColor = ColorTranslator.FromHtml("#ccc");
			this.treeView1.ShowRootLines = false;
			this.treeView1.ShowLines = false;
			this.treeView1.ShowPlusMinus = true;
			this.treeView1.HideSelection = true;
			this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;	// ﾉｰﾄﾞのﾗﾍﾞﾙ部分は自分で描画
			
			// 指定方法の設定
			this.CanDragDrop = false;
			
			// ﾚｲﾔｰ･ﾂﾘｰを構築
			this.ShowLayerTree();
			
			// ﾄﾞﾗｯｸﾞ&ﾄﾞﾛｯﾌﾟ用ｸﾗｽを準備
			this._clsD_D = new DragDropManager(this.treeView1.Indent, this.treeView1.ImageList.ImageSize.Width);
		}

		/// <summary>
		/// ロード イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UC_Load(object sender, EventArgs e) {
			// FILL
			this.treeView1.Dock = DockStyle.Fill;
			
			// 最上位ﾚｲﾔを表示
			if(this.treeView1.Nodes.Count > 0) {
				this.treeView1.TopNode = this.treeView1.Nodes[0];
			}
			
			// いずれ、窓の高さを調節したい
			//this.treeView1.GetNodeCount(true);
		}
		
#region プロパティ
		/// <summary>
		/// 対象のマップを取得または設定します
		/// </summary>
		public IMap TargetMap {
			get {
				return this._agMap;
			}
			set {
				this._agMap = value;
				
				// ﾂﾘｰを構築
				this.ShowLayerTree();
			}
		}
		
		/// <summary>
		/// 対象のグループレイヤを取得または設定します
		/// </summary>
		public ICompositeLayer TargetLayer {
			get {
				return this._agLayer as ICompositeLayer;
			}
			set {
				this._agLayer = value as ILayer;
				
				// ﾂﾘｰを構築
				this.ShowLayerTree();
			}
		}
		
		/// <summary>
		/// 選択されたレイヤーを取得します
		/// </summary>
		public ILayer[] SelectedLayers {
			get {
				List<ILayer>	arrLayers = new List<ILayer>();
				
				// 指定ﾓｰﾄﾞにより取得方法が異なる
				if(this.treeView1.CheckBoxes) {
					// ﾁｪｯｸﾎﾞｯｸｽ指定方式 (ﾁｪｯｸされたﾚｲﾔｰ)
					foreach(TreeNode node in this.treeView1.Nodes) {
						arrLayers.AddRange(this.GetCheckedLayers(node));
					}
				}
				else {
					// ﾄﾞﾗｯｸﾞ&ﾄﾞﾛｯﾌﾟ指定方式 (ﾊｲﾗｲﾄ･ﾚｲﾔｰ)
					foreach(TreeNode node in this.GetAllHighlightNodes()) {
						arrLayers.Add(node.Tag as ILayer);
					}
				}
				
				return arrLayers.ToArray();
			}
		}
		
		/// <summary>
		/// グループに追加するレイヤーを取得します
		/// </summary>
		public ILayer[] GroupinLayers {
			get {
				List<ILayer>	arrLayers = new List<ILayer>();
				
				ICompositeLayer	agCompoLayer = this._agLayer as ICompositeLayer;

				// 指定ﾓｰﾄﾞにより取得方法が異なる
				if(this.treeView1.CheckBoxes) {
					// ﾁｪｯｸﾎﾞｯｸｽ指定方式 (ﾁｪｯｸされたﾚｲﾔｰ)
					foreach(ILayer agSelLayer in this.SelectedLayers) {
						// 対象のｸﾞﾙｰﾌﾟﾚｲﾔｰでなく、
						if(agSelLayer != this._agLayer) {
							// 子ﾚｲﾔｰでもない
							if(!this.IsGroupChildLayer(agSelLayer)) {
								// 追加対象
								arrLayers.Add(agSelLayer);
							}
						}
					}
				}
				else {
					// ﾄﾞﾗｯｸﾞ&ﾄﾞﾛｯﾌﾟ指定方式 (内部に移動されたﾚｲﾔｰ)
					foreach(TreeNode node in this._nodeLayer.Nodes) {
						// 未追加のﾚｲﾔｰを取得
						if(!this.IsGroupChildLayer(node.Tag as ILayer)) {
							// 追加対象
							arrLayers.Add(node.Tag as ILayer);
						}
					}
				}
				
				return arrLayers.ToArray();
			}
		}
		
		/// <summary>
		/// グループから除外するレイヤーを取得します
		/// </summary>
		public ILayer[] GroupoutLayers {
			get {
				List<ILayer>	arrLayers = new List<ILayer>();
				
				ICompositeLayer	agCompoLayer = this._agLayer as ICompositeLayer;
				TreeNode		nodeChild;
				ILayer			agChildLayer;

				// 指定ﾓｰﾄﾞにより取得方法が異なる
				if(this.treeView1.CheckBoxes) {
					// ﾁｪｯｸﾎﾞｯｸｽ指定方式 (ﾁｪｯｸされたﾚｲﾔｰ)
					foreach(TreeNode node in this._nodeLayer.Nodes) {
						if(!node.Checked) {
							arrLayers.Add(node.Tag as ILayer);
						}
					}
				}
				else {
					// ﾄﾞﾗｯｸﾞ&ﾄﾞﾛｯﾌﾟ指定方式 (外部に移動されたﾚｲﾔｰ)
					for(int intCnt=0; intCnt < agCompoLayer.Count; intCnt++) {
						// 既存の子ﾚﾄﾔｰを取得
						agChildLayer = agCompoLayer.get_Layer(intCnt);
						
						nodeChild = null;
						// 対象ﾉｰﾄﾞの配下をﾁｪｯｸ
						foreach(TreeNode node in this._nodeLayer.Nodes) {
							if(node.Tag == agChildLayer) {
								nodeChild = node;
								break;
							}
						}
						if(nodeChild == null) {
							arrLayers.Add(agChildLayer);
						}
					}
				}
						//if(nodeChild == null) {	// 配下にﾉｰﾄﾞがない場合 (ﾄﾞﾗｯｸﾞ操作による)
						//    // 外部ﾉｰﾄﾞをﾁｪｯｸ
						//    foreach(TreeNode node in this.treeView1.Nodes) {
						//        nodeChild = this.FindNodeByLayer(node, agChildLayer);
						//        if(nodeChild != null) {
						//            break;
						//        }
						//    }
						//}
				
				return arrLayers.ToArray();
			}
		}
		
		/// <summary>
		/// レイヤーのドラッグ＆ドロップを有効にするかどうか（チェックボックス指定方法／ドラッグ指定方法の切り替え）
		/// </summary>
		public bool CanDragDrop {
			get {
				return this.treeView1.AllowDrop;
			}
			set {
				this.treeView1.AllowDrop = value;
				this.treeView1.CheckBoxes = !value;
				
				// ﾄﾞﾗｯｸﾞ&ﾄﾞﾛｯﾌﾟ式に変更すると、ﾂﾘｰが閉じてしまう
				this.treeView1.ExpandAll();
				
				// 最上位ﾚｲﾔを表示
				if(this.treeView1.Nodes.Count > 0) {
					this.treeView1.TopNode = this.treeView1.Nodes[0];
				}
			}
		}
#endregion
		
		/// <summary>
		/// レイヤー・ツリーを構築します
		/// </summary>
		private void ShowLayerTree() {
			// 実行ﾁｪｯｸ
			if(!(this._agMap == null || this._agLayer == null)) {
				// ﾚｲﾔ･ﾂﾘｰを作成
				this.treeView1.Nodes.Clear();
				TreeNode[] nodeLayers = this.MakeLayerTreeNodes();
				if(nodeLayers.Length > 0) {
					this.treeView1.Nodes.AddRange(nodeLayers);

					// 全展開
					this.treeView1.ExpandAll();

					// 先頭ﾉｰﾄﾞを表示
					this.treeView1.TopNode = this.treeView1.Nodes[0];
				}
			}
		}

		/// <summary>
		/// レイヤ・ツリーを作成します
		/// </summary>
		/// <returns>ヒットしたツリー・ノード群</returns>
		private TreeNode[] MakeLayerTreeNodes() {
			List<TreeNode>	arrNodes = new List<TreeNode>();

			// 対象ﾚｲﾔを探索
			IMap		agMap = this._agMap;
			ILayer		agLayer;
			TreeNode	nodeNew;
			for(int intCnt=0; intCnt < agMap.LayerCount; intCnt++) {
				agLayer = agMap.get_Layer(intCnt);
				
				if(agLayer is ICompositeLayer) {
					nodeNew = this.GetGroupLayerTree(agLayer as ICompositeLayer, agLayer.Equals(this._agLayer));
				}
				else {
					nodeNew = this.GetLayerTree(agLayer, false);
				}

				// 追加
				if(nodeNew != null) {
					arrNodes.Add(nodeNew);
				}	
			}

			return arrNodes.ToArray();
		}
		
		/// <summary>
		/// グループレイヤ内のレイヤ・ツリーを作成します
		/// </summary>
		/// <param name="GroupLayer">グループ・レイヤ</param>
		/// <returns></returns>
		private TreeNode GetGroupLayerTree(ICompositeLayer GroupLayer, bool IsChecked) {
			TreeNode	node = null;
			
			if(!IsChecked) {
				IsChecked = GroupLayer.Equals(this._agLayer);
			}

			// 子ﾚｲﾔを取得
			List<TreeNode>	arrCNodes = new List<TreeNode>();
			ILayer		agLayer;
			for(int intCnt=0; intCnt < GroupLayer.Count; intCnt++) {
				agLayer = GroupLayer.get_Layer(intCnt);
				if(agLayer is ICompositeLayer) {
					arrCNodes.Add(this.GetGroupLayerTree(agLayer as ICompositeLayer, IsChecked));
				}
				else {
					arrCNodes.Add(this.GetLayerTree(agLayer, IsChecked));
				}
			}

			// ｸﾞﾙｰﾌﾟ･ﾚｲﾔ ﾉｰﾄﾞを作成
			if(arrCNodes.Count > 0 || GroupLayer.Count <= 0) {
				string	strLayerName = (GroupLayer as ILayer).Name;

				// ﾚｲﾔ種類を識別
				int		intImageID = GroupLayer is IGroupLayer ? LayerImage.GroupLayer.GetHashCode() : LayerImage.BaseMapLayer.GetHashCode();

				// ﾉｰﾄﾞ
				node = new TreeNode(strLayerName, intImageID, intImageID);
				node.Name = strLayerName;
				node.Checked = IsChecked;
				node.Tag = GroupLayer;

				// 子をｾｯﾄ
				node.Nodes.AddRange(arrCNodes.ToArray());
			}

			return node;
		}

		/// <summary>
		/// レイヤ・ツリーノードを作成します
		/// </summary>
		/// <param name="Layer">レイヤ</param>
		/// <param name="LabelClass">ラベルクラス名</param>
		/// <returns></returns>
		private TreeNode GetLayerTree(ILayer Layer, bool Checked) {
			TreeNode node = new TreeNode(Layer.Name);
			node.Name = Layer.Name;
			
			ILayer	agLayer;
			if(Layer is IBasemapSubLayer) {
				agLayer = (Layer as IBasemapSubLayer).Layer;
			}
			else {
				agLayer = Layer;
			}
			
			// Set Image
			if(agLayer is IFeatureLayer) {
				// 表示ｲﾒｰｼﾞをｾｯﾄ
				IFeatureClass	agFC = (agLayer as IFeatureLayer).FeatureClass;
				switch(agFC.ShapeType) {
				case esriGeometryType.esriGeometryPoint:
				case esriGeometryType.esriGeometryMultipoint:
					node.ImageIndex = node.SelectedImageIndex = LayerImage.PointLayer.GetHashCode();
					break;
				case esriGeometryType.esriGeometryPolyline:
				case esriGeometryType.esriGeometryLine:
				case esriGeometryType.esriGeometryPath:
					node.ImageIndex = node.SelectedImageIndex = LayerImage.LineLayer.GetHashCode();
					break;
				case esriGeometryType.esriGeometryPolygon:
				case esriGeometryType.esriGeometryEnvelope:
				case esriGeometryType.esriGeometryRing:
				case esriGeometryType.esriGeometrySphere:
					node.ImageIndex = node.SelectedImageIndex = LayerImage.PolygonLayer.GetHashCode();
					break;
				}
			}
			else if(agLayer is IRasterLayer) {
				node.ImageIndex = node.SelectedImageIndex = LayerImage.RasterLayer.GetHashCode();
			}

			// ﾚｲﾔを潜入
			node.Tag = Layer;
			node.Checked = Checked;

			return node;
		}
		
		// 描画 ｲﾍﾞﾝﾄ ※Drag&Drop中は発生しない
		private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e) {
			if(this.treeView1.DrawMode == TreeViewDrawMode.OwnerDrawText) {
				// ﾃｷｽﾄ出力用ﾌｫﾝﾄを取得
				var font = e.Node.NodeFont ?? e.Node.TreeView.Font;

				SolidBrush	fill;			// 背景色
				Color		colLetter;		// ﾃｷｽﾄ色

				// 選択ﾉｰﾄﾞを描画する時
				if(e.Node != null) {
					// 設定対象ﾚｲﾔ
					if(this.IsTargetLayer(e.Node)) {
						// 背景色
						fill = new SolidBrush(Color.Red);
						e.Graphics.FillRectangle(fill, e.Bounds);
						
						// 文字色
						colLetter = SystemColors.HighlightText;
					}
					// 疑似的に選択を示唆
					else if(this.IsHighlight(e.Node)) {
						fill = new SolidBrush(SystemColors.Highlight);
						//e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);

						// 文字色
						colLetter = SystemColors.HighlightText;
					}
					// SelectedNode
					else if((e.State & TreeNodeStates.Selected) != 0) {
						// 背景色
						fill = new SolidBrush(SystemColors.Window);
						e.Graphics.FillRectangle(fill, e.Bounds);
						
						// 文字色
						colLetter = SystemColors.ControlText;
					}
					// 一般
					else {
						fill = new SolidBrush(Color.Empty);
						colLetter = SystemColors.ControlText;
					}

					// 背景描画
					e.Graphics.FillRectangle(fill, e.Bounds);
					fill.Dispose();
					
					// ﾃｷｽﾄを描画
					TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, colLetter, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

					// ﾌｫｰｶｽ･ﾉｰﾄﾞ
					if((e.State & TreeNodeStates.Focused) != 0) {
						using(Pen focusPen = new Pen(Color.Black)) {
							focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
							Rectangle focusBounds = e.Bounds;
							focusBounds.Size = new Size(focusBounds.Width - 1, focusBounds.Height - 1);
							e.Graphics.DrawRectangle(focusPen, focusBounds);
						}
					}
				}
			}
		}

		// ﾁｪｯｸ(前) ｲﾍﾞﾝﾄ
		private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e) {
			// ﾁｪｯｸを制御
			if(this.IsTargetLayer(e.Node) || this.IsParent(this._nodeLayer, e.Node)) {
				e.Cancel = true;
			}
		}

		// ﾁｪｯｸ(後) ｲﾍﾞﾝﾄ
		private void treeView1_AfterCheck(object sender, TreeViewEventArgs e) {
			bool	blnChecked = e.Node.Checked;
			
			// 全選択 / 全解除
			foreach(TreeNode node in e.Node.Nodes)  {
				node.Checked = blnChecked;
			}
			
			if(!this._blnIsChecked) {
				this._blnIsChecked = true;
				
				// ﾊｲﾗｲﾄ連動
				if(this.IsHighlight(e.Node)) {
					foreach(TreeNode node in this.GetAllHighlightNodes()) {
						if(node != e.Node) {
							node.Checked = blnChecked;
						}
					}
				}
			}
		}

		// ｷｰﾎﾞｰﾄﾞ Down
		private void treeView1_KeyDown(object sender, KeyEventArgs e) {
			// ﾕｰｻﾞｰ操作を記録
			this._clsOperate.SetKeys(e.Control, e.Shift, e.KeyCode == Keys.Down, e.KeyCode == Keys.Up);
		}

		// ﾏｳｽ Down
		private void treeView1_MouseDown(object sender, MouseEventArgs e) {
			// ﾕｰｻﾞｰ操作を記録
			this._clsOperate.SetKeys(
				(Control.ModifierKeys & Keys.Control) == Keys.Control,
				(Control.ModifierKeys & Keys.Shift) == Keys.Shift,
				false,
				false
			);
		}

		// ﾏｳｽ Up
		private void treeView1_MouseUp(object sender, MouseEventArgs e) {
			TreeView	ctlTV = sender as TreeView;

			if(this._blnIsChecked) {
				this._blnIsChecked = false;
			}
			else if(this._clsOperate.LastNode != null) {
				// 選択ﾉｰﾄﾞが前回と一緒の時
				TreeViewHitTestInfo	hitInfo = ctlTV.HitTest(e.X, e.Y);
				if(this._clsOperate.LastNode.Equals(hitInfo.Node)) {
					// SINGLE
					if(!this._clsOperate.IsShiftOrCtl) {
						// 全選択をｸﾘｱ
						this.ClearAllHighlight();
						// 指定ﾉｰﾄﾞを選択
						this.HighlightNode(hitInfo.Node, true);
					}
					// CONTROL
					else if(this._clsOperate.ControlKey) {
						// 選択ｲﾍﾞﾝﾄが発生するように調整
						this.treeView1.SelectedNode = null;
					}
					// SHIFT
					else if(this._clsOperate.ShiftKey) {
						// ｽﾙｰ
					}
				}
			}
		}

		// ﾂﾘｰ ﾉｰﾄﾞ選択前 ｲﾍﾞﾝﾄ
		private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
			// SHIFT時は、選択ﾉｰﾄﾞを制御 (同列ﾉｰﾄﾞを選択する)
			if(this._clsOperate.LastNode != null && this._clsOperate.ShiftKey) {
				// 親が異なる
				if(e.Node.Parent != this._clsOperate.LastNode.Parent) {
					TreeNode	nodeLast = null;
					
					// ｷｰ操作の場合、同列のﾉｰﾄﾞを選択する
					if(this._clsOperate.IsKeyboard) {
						// 選択ﾉｰﾄﾞ群を取得
						TreeNode[] nodeSels = this.GetSameColumnNodes(this._clsOperate.LastNode, true);
						if(nodeSels.Length > 1) {
							// ↓
							if(nodeSels[0].Equals(this._clsOperate.LastNode)) {
								// 一番下のﾉｰﾄﾞを取得
								nodeLast = nodeSels.Last();
							}
							// ↑
							else {
								// 一番上のﾉｰﾄﾞを取得
								nodeLast = nodeSels.First();
							}
						}
						else {
							nodeLast = this._clsOperate.LastNode;
						}

						// ↓ ｷｰ操作
						if(this._clsOperate.KeyDown) {
							e.Cancel = true;
							if(nodeLast.NextNode != null) {
								this.treeView1.SelectedNode = nodeLast.NextNode;
							}
						}
						// ↑ 
						else if(this._clsOperate.KeyUp) {
							e.Cancel = true;
							if(nodeLast.PrevNode != null) {
								this.treeView1.SelectedNode = nodeLast.PrevNode;
							}
						}
					}
					// ﾏｳｽ操作の場合、既存の選択を全ｸﾘｱ
					else {
						this.ClearAllHighlight();
					}
				}
			}
		}

		// ﾂﾘｰ ﾉｰﾄﾞ選択後 ｲﾍﾞﾝﾄ
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
			if(this._clsOperate.LastNode != null && this._clsOperate.IsShiftOrCtl) {
				// SHIFT
				if(this._clsOperate.ShiftKey) {
					// 親が同じなら...
					if(this._clsOperate.LastNode.Parent == e.Node.Parent) {
						// 全選択解除
						if(!this._clsOperate.ControlKey) {
							this.ClearAllHighlight();
						}
						
						int	intFrom, intEnd;
						if(this._clsOperate.LastNode.Index > e.Node.Index) {
							intFrom = e.Node.Index;
							intEnd = this._clsOperate.LastNode.Index;
						}
						else {
							intFrom = this._clsOperate.LastNode.Index;
							intEnd = e.Node.Index;
						}
						
						// 順次選択
						TreeNode[]	nodes = this.GetSameColumnNodes(this._clsOperate.LastNode, false);
						for(int intCnt=intFrom; intCnt <= intEnd; intCnt++) {
							this.HighlightNode(nodes[intCnt], true);
						}
					}
					else {
						// 常に指定箇所を選択 (事前に既存の選択は全ｸﾘｱされている)
						this.HighlightNode(e.Node, true);
						this._clsOperate.ShiftKey = false;	// 選択ﾉｰﾄﾞを保存する為
					}
				}
				// CONTROL
				else {
					// 選択を切り替え
					this.HighlightToggle(e.Node);
				}
			}
			else {
				// 全選択をｸﾘｱ
				this.ClearAllHighlight();

				// 常に指定箇所を選択
				this.HighlightNode(e.Node, true);
			}
			
			// 最後に選択したﾉｰﾄﾞを記録
			if(!this._clsOperate.ShiftKey) {	// SHIFT時はｽﾙｰ
				this._clsOperate.LastNode = this.treeView1.SelectedNode;
			}
		}
		
		/// <summary>
		/// 同列のノード群を取得します
		/// </summary>
		/// <param name="TargetNode"></param>
		/// <returns></returns>
		private TreeNode[] GetSameColumnNodes(TreeNode TargetNode, bool IsSelected) {
			TreeNodeCollection	nodeColls;
			List<TreeNode>		nodes = new List<TreeNode>();
			
			if(TargetNode.Parent == null) {
				nodeColls = this.treeView1.Nodes;
			}
			else {
				nodeColls = TargetNode.Parent.Nodes;
			}
			
			if(IsSelected) {
				// 選択されたﾉｰﾄﾞ群を取得
				nodes.AddRange(nodeColls.Cast<TreeNode>().Where(n=>this.IsHighlight(n)));
			}
			else {
				// 全ﾉｰﾄﾞ群を取得
				nodes.AddRange(nodeColls.Cast<TreeNode>());
			}
			
			return nodes.ToArray();
		}
		
#region ハイライト制御
		/// <summary>
		/// 選択状態を全解除します
		/// </summary>
		private void ClearAllHighlight() {
			foreach(TreeNode node in this.treeView1.Nodes) {
				this.ClearHilight(node);
			}
		}
		
		/// <summary>
		/// 指定ノードの選択を解除します
		/// </summary>
		/// <param name="TargetNode"></param>
		private void ClearHilight(TreeNode TargetNode) {
			this.HighlightNode(TargetNode, false);
			foreach(TreeNode node in TargetNode.Nodes) {
				if(node.Nodes.Count > 0) {
					this.ClearHilight(node);
				}
				else if(this.IsHighlight(node)) {
					this.HighlightNode(node, false);
				}
			}
		}
		
		/// <summary>
		/// 全選択ノードを取得します
		/// </summary>
		/// <returns></returns>
		private TreeNode[] GetAllHighlightNodes() {
			List<TreeNode>	nodes = new List<TreeNode>();
			
			// 全ﾊｲﾗｲﾄ･ﾉｰﾄﾞを取得
			foreach(TreeNode node in this.treeView1.Nodes) {
				nodes.AddRange(this.GetHighlightNodes(node));
			}
			
			return nodes.ToArray();
		}

		/// <summary>
		/// 指定ノード下の選択ノードを取得します
		/// </summary>
		/// <param name="TargetNode"></param>
		/// <returns></returns>
		private TreeNode[] GetHighlightNodes(TreeNode TargetNode) {
			List<TreeNode>	nodes = new List<TreeNode>();
			
			// 自分
			if(this.IsHighlight(TargetNode)) {
				nodes.Add(TargetNode);
			}
			
			// 子
			foreach(TreeNode node in TargetNode.Nodes) {
				if(node.Nodes.Count > 0) {
					// 再帰
					nodes.AddRange(this.GetHighlightNodes(node));
				}
				else if(this.IsHighlight(node)) {
					nodes.Add(node);
				}
			}
			
			return nodes.ToArray();
		}
		
		// 選択ﾉｰﾄﾞかどうか判定
		private bool IsHighlight(TreeNode TargetNode) {
			return TargetNode.BackColor == SystemColors.Highlight;
		}
		
		/// <summary>
		/// 指定ノードの選択状態を反転させます
		/// </summary>
		/// <param name="TargetNode"></param>
		private void HighlightToggle(TreeNode TargetNode) {
			if(!this.IsHighlight(TargetNode)) {
				// 選択
				TargetNode.BackColor = SystemColors.Highlight;
				TargetNode.ForeColor = SystemColors.HighlightText;
			}
			else {
				// 解除
				TargetNode.BackColor = Color.Empty;
				TargetNode.ForeColor = this.ForeColor;
			}
		}
		
		/// <summary>
		/// 指定ノードの選択状態を設定します
		/// </summary>
		/// <param name="TargetNode"></param>
		/// <param name="bTreeViewActive"></param>
		private void HighlightNode(TreeNode TargetNode, bool bTreeViewActive) {
			// 選択
			if(bTreeViewActive) {
				if(!this.IsHighlight(TargetNode)) {
					this.HighlightToggle(TargetNode);
				}
			}
			// 解除
			else {
				if(this.IsHighlight(TargetNode)) {
					this.HighlightToggle(TargetNode);
				}
			}
		}
#endregion
		
		/// <summary>
		/// 処理対象ﾚｲﾔかどうか
		/// </summary>
		/// <param name="TargetNode"></param>
		/// <returns></returns>
		private bool IsTargetLayer(TreeNode TargetNode) {
			bool	blnRet = false;
			
			// 処理対象ﾉｰﾄﾞをｾｯﾄ
			if(this._nodeLayer == null) {
				blnRet = TargetNode.Tag.Equals(this._agLayer);
				if(blnRet) {
					this._nodeLayer = TargetNode;
				}
			}
			else {
				blnRet = TargetNode == this._nodeLayer;
			}
			return TargetNode.Tag.Equals(this._agLayer);
		}
		
		/// <summary>
		/// 指定ノードが親ノードかどうか
		/// </summary>
		/// <param name="MeNode">自分</param>
		/// <param name="OtherNode">他</param>
		/// <returns>Yes / No</returns>
		private bool IsParent(TreeNode MeNode, TreeNode OtherNode) {
			bool	blnRet = false;
			
			// ﾚﾍﾞﾙで判定
			if(MeNode.Level > 0 || OtherNode.Level < MeNode.Level) {
				TreeNode	node = MeNode.Parent;
				while(node != null) {
					if(node == OtherNode) {
						blnRet = true;
						break;
					}
					node = node.Parent;
				}
			}
			
			return blnRet;
		}

		/// <summary>
		/// 全てのチェック済みノードを取得します
		/// </summary>
		/// <returns></returns>
		private ILayer[] GetAllCheckedLayers() {
			List<ILayer>	nodes = new List<ILayer>();
			
			// 全ﾊｲﾗｲﾄ･ﾉｰﾄﾞを取得
			foreach(TreeNode node in this.treeView1.Nodes) {
				nodes.AddRange(this.GetCheckedLayers(node));
			}
			
			return nodes.ToArray();
		}

		/// <summary>
		/// 指定ノード下のチェック済みレイヤーを取得します
		/// </summary>
		/// <param name="TargetNode"></param>
		/// <returns></returns>
		private ILayer[] GetCheckedLayers(TreeNode TargetNode) {
			List<ILayer>	nodes = new List<ILayer>();
			
			// 自分
			if(TargetNode.Checked) {
				nodes.Add(TargetNode.Tag as ILayer);
			}
			
			// 子
			foreach(TreeNode node in TargetNode.Nodes) {
				if(node.Nodes.Count > 0) {
					// 再帰
					nodes.AddRange(this.GetCheckedLayers(node));
				}
				else if(node.Checked) {
					nodes.Add(node.Tag as ILayer);
				}
			}
			
			return nodes.ToArray();
		}
		
		/// <summary>
		/// 指定レイヤーがグループの構成レイヤーかどうか
		/// </summary>
		/// <param name="ChildLayer"></param>
		/// <returns></returns>
		public bool IsGroupChildLayer(ILayer ChildLayer) {
			bool	blnRet = false;
			
			// 事前ﾁｪｯｸ
			if(this.TargetLayer != null && ChildLayer != null) {
				blnRet = this.IsGroupChildLayer(this.TargetLayer, ChildLayer);
			}
			
			return blnRet;
		}
		
		/// <summary>
		/// 指定レイヤーが指定グループの構成レイヤーかどうか
		/// </summary>
		/// <param name="ParentLayer"></param>
		/// <param name="ChildLayer"></param>
		/// <returns></returns>
		private bool IsGroupChildLayer(ICompositeLayer ParentLayer, ILayer ChildLayer) {
			bool	blnRet = false;
			
			ILayer	agChildLayer;
			for(int intCnt=0; intCnt < ParentLayer.Count; intCnt++) {
				// 子ﾚｲﾔｰを取得
				agChildLayer = ParentLayer.get_Layer(intCnt);
				// 判定
				if(ChildLayer == agChildLayer) {
					blnRet = true;
					break;
				}
				else if(agChildLayer is ICompositeLayer) {
					// 子ｸﾞﾙｰﾌﾟをﾁｪｯｸ
					blnRet = this.IsGroupChildLayer(agChildLayer as ICompositeLayer, ChildLayer);
				}
			}
			
			return blnRet;
		}
		
		/// <summary>
		/// 指定レイヤーのツリーノードを取得します
		/// </summary>
		/// <param name="TargetNode"></param>
		/// <param name="agLayer"></param>
		/// <returns></returns>
		private TreeNode FindNodeByLayer(TreeNode TargetNode, ILayer agLayer) {
			TreeNode	nodeRet = null;
			
			if(TargetNode.Tag == agLayer) {
				nodeRet = TargetNode;
			}
			else {
				// 子ﾉｰﾄﾞを探索
				foreach(TreeNode node in TargetNode.Nodes) {
					if(node.Nodes.Count > 0) {
						nodeRet = this.FindNodeByLayer(node, agLayer);
					}
					else if(node.Tag == agLayer) {
						nodeRet = node;
					}
					if(nodeRet != null) {
						break;
					}
				}
			}
			
			return nodeRet;
		}

#region ドラッグ＆ドロップ制御
		private void treeView1_ItemDrag(object sender, ItemDragEventArgs e) {
			TreeView	ctlTV = sender as TreeView;
			
			// ﾄﾞﾗｯｸﾞ&ﾄﾞﾛｯﾌﾟ操作を初期化
			var dde = this.DoDragDrop(e.Item, DragDropEffects.Move);
#if DEBUG
			Debug.WriteLine("●●EVENT : ITEM DRAG");
#endif
			// 前に描いた線を消す
			this._clsD_D.ClearPrevLine();
		}

		private void treeView1_DragEnter(object sender, DragEventArgs e) {
#if DEBUG
			Debug.WriteLine("●●EVENT : DRAG ENTER");
#endif
			// 対象を制御
			if(e.Data.GetDataPresent(typeof(TreeNode))) {
				// 対象ﾉｰﾄﾞを取得
				TreeNode	nodeSrc = e.Data.GetData(typeof(TreeNode)) as TreeNode;
				
				if(!this.IsHighlight(nodeSrc)) {
					// 全選択をｸﾘｱ
					this.ClearAllHighlight();
					// 指定ﾉｰﾄﾞを選択
					this.HighlightNode(nodeSrc, true);
					nodeSrc.TreeView.SelectedNode = nodeSrc;
				}
				e.Effect = DragDropEffects.Move;
			}
			else {
				e.Effect = DragDropEffects.None;
			}
		}

		private void treeView1_DragOver(object sender, DragEventArgs e) {
			// 対応ﾃﾞｰﾀか判定
			if(e.Data.GetDataPresent(typeof(TreeNode))) {
				TreeView	ctlTV = sender as TreeView;
				
				// 前回の下線を消去
				this._clsD_D.ClearPrevLine();

				// 対象ﾉｰﾄﾞを取得
				System.Drawing.Point	pnt = ctlTV.PointToClient(new System.Drawing.Point(e.X, e.Y));
				TreeNode				nodeDes = ctlTV.GetNodeAt(pnt);
				TreeNode				nodeSrc = e.Data.GetData(typeof(TreeNode)) as TreeNode;
				
				// 対象が親族の場合は処理しない
				if(!this.IsParent(nodeDes, nodeSrc)) {
					// 下線を描画
					if(this._clsD_D.DrawUnderline(nodeDes, pnt.X, nodeSrc == nodeDes)) {
						e.Effect = DragDropEffects.Move;
					}
					else {
						e.Effect = DragDropEffects.None;
					}
#if DEBUG
					Debug.WriteLine("●EVENT : DRAG OVER - SELECTED NODE = " + nodeDes.Text);
#endif
				}
				else {
					e.Effect = DragDropEffects.None;
				}
			}
			else {
				e.Effect = DragDropEffects.None;
#if DEBUG
				Debug.WriteLine("・EVENT : DRAG OVER - NONE");
#endif
			}
		}

		private void treeView1_DragDrop(object sender, DragEventArgs e) {
			// 対象ﾃﾞｰﾀ ?
			if(e.Data.GetDataPresent(typeof(TreeNode))) {
				TreeView	ctlTV = sender as TreeView;
				
				// 移動対象ﾉｰﾄﾞを取得
				TreeNode[]	nodes = this.GetAllHighlightNodes();
				TreeNode	nodeNew;
				if(nodes.Length > 0) {
					// 移動先ﾉｰﾄﾞを取得
					TreeNode	nodeDest = this._clsD_D.PrevOverNode;
					
					// 対象が同じ場合は処理しない
					//if(!(nodes.Length == 1 && nodeDest == nodes[0])) {
						this.Cursor = Cursors.WaitCursor;
						ctlTV.SuspendLayout();
						
						// 対象ﾚｲﾔを調整
						if(this._clsD_D.Level >= 1) {
							for(int intCnt=1; intCnt <= this._clsD_D.Level; intCnt++) {
								nodeDest = nodeDest.Parent;
							}
						}
						
						int	intIdx;
						TreeNodeCollection	nodeParent;
						
						// ｸﾞﾙｰﾌﾟ･ﾚｲﾔの場合
						if(nodeDest.Tag is ICompositeLayer && this._clsD_D.Level <= 0) {
							intIdx = 0;
							nodeParent = nodeDest.Nodes;
							// 配下に移動
							foreach(TreeNode nodeSrc in nodes) {
								nodeNew = (TreeNode)nodeSrc.Clone();
								nodeParent.Insert(intIdx++, nodeNew);

								this.HighlightNode(nodeNew, true);
								if(nodeSrc.IsSelected) {
									ctlTV.SelectedNode = nodeNew;
								}
								if(nodeSrc.IsExpanded) {
									nodeNew.Expand();
								}
							}

							// 子ﾉｰﾄﾞを展開
							if(!nodeDest.IsExpanded) {
								nodeDest.Expand();
							}
						}
						else {
							// 下位に移動
							intIdx = nodeDest.Index;
							nodeParent = nodeDest.Parent == null ? ctlTV.Nodes : nodeDest.Parent.Nodes;
							
							foreach(TreeNode nodeSrc in nodes) {
								nodeNew = (TreeNode)nodeSrc.Clone();
								nodeParent.Insert(++intIdx, nodeNew);

								this.HighlightNode(nodeNew, true);
								if(nodeSrc.IsSelected) {
									ctlTV.SelectedNode = nodeNew;
								}
								if(nodeSrc.IsExpanded) {
									nodeNew.Expand();
								}
							}
						}
						
						// 元のﾉｰﾄﾞを削除
						for(int intCnt=nodes.Length - 1; intCnt >= 0; intCnt--) {
							nodes[intCnt].Remove();
						}
					//}

					ctlTV.ResumeLayout();
					this.Cursor = Cursors.Default;
					
					// ｸﾘｱ & 再描画
					this._clsD_D.ClearStatus();
					ctlTV.Invalidate();
				}
			}
		}
#endregion

		/// <summary>
		/// 処理続行チェック
		/// </summary>
		/// <returns>OK / NG</returns>
		public bool CheckGroupinCondition() {
			// ｸﾞﾙｰﾌﾟに追加するﾚｲﾔ数をｶｳﾝﾄ
			int	intCnt = this.GroupinLayers.Length;
			
			// ｸﾞﾙｰﾌﾟから外すﾚｲﾔ数をｶｳﾝﾄ
			intCnt += this.GroupoutLayers.Length;
			
			return intCnt > 0;
		}

		/// <summary>
		/// 処理を実行します
		/// </summary>
		public bool CommitLayers() {
			bool	blnRet = false;
			
			// 対象のｸﾞﾙｰﾌﾟ･ﾚｲﾔｰ
			IGroupLayer	agGrpLayer = this._agLayer as IGroupLayer;
			
			// ｸﾞﾙｰﾌﾟ･ﾚｲﾔから外す
			this.ExecuteWithout();
			
			// ｸﾞﾙｰﾌﾟ･ﾚｲﾔに追加
			this.ExecuteInclude();
			
			blnRet = true;
			
			return blnRet;
		}
		
		/// <summary>
		/// グループレイヤから外します
		/// </summary>
		/// <returns></returns>
		private bool ExecuteWithout() {
			ILayer[]	agLayers = this.GroupoutLayers;
			
			if(agLayers.Length > 0) {
				IGroupLayer	agTargetGroup = this._agLayer as IGroupLayer;
				IGroupLayer	agParentGroup = null;
				
				// 削除ﾚｲﾔｰの編入先
				if(this._nodeLayer.Parent != null) {
					agParentGroup = this._nodeLayer.Parent.Tag as IGroupLayer;
				}
				
				if(agParentGroup == null) {
					// 地図直下に移動
					ILayer	agLayer;
					for(int intCnt = agLayers.Length - 1; intCnt >= 0; intCnt--) {
						agLayer = agLayers[intCnt];
						agTargetGroup.Delete(agLayer);
						this._agMap.AddLayer(agLayer);
						//this._agMap.MoveLayer(agLayer, 0);
					}
				}
				else {
					// 親ｸﾞﾙｰﾌﾟに移動
					foreach(ILayer agLayer in agLayers) {
						agTargetGroup.Delete(agLayer);
						agParentGroup.Add(agLayer);
					}
				}
			}
			
			return agLayers.Length > 0;
		}
		
		/// <summary>
		/// グループレイヤに追加します
		/// </summary>
		/// <returns></returns>
		private bool ExecuteInclude() {
			ILayer[]	agLayers = this.GroupinLayers;

			if(agLayers.Length > 0) {
				IGroupLayer	agTargetGroup = this._agLayer as IGroupLayer;
				IGroupLayer	agParentGroup;

				foreach(ILayer agLayer in agLayers) {
					// 元親を取得
					agParentGroup = (IGroupLayer)this.GetParentLayer(agLayer);
				
					// 元の親から引き離す
					if(agParentGroup == null) {
						// 地図直下
						this._agMap.DeleteLayer(agLayer);
					}
					else {
						// ｸﾞﾙｰﾌﾟ･ﾚｲﾔｰ
						agParentGroup.Delete(agLayer);
					}
					
					// 新親に追加
					agTargetGroup.Add(agLayer);
				}
			}
			
			return agLayers.Length > 0;
		}
		
		/// <summary>
		/// 指定レイヤーの親レイヤーを取得します
		/// </summary>
		/// <param name="ChildLayer">対象レイヤー</param>
		/// <returns>親レイヤー(Null時はマップ直下)</returns>
		private ILayer GetParentLayer(ILayer ChildLayer) {
			ILayer	agParentLayer = null;
			
			bool	blnDetect = false;
			List<ICompositeLayer>	agCompLayers = new List<ICompositeLayer>();
			
			// ﾏｯﾌﾟ直下を検索
			ILayer	agLayer;
			for(int intCnt=0; intCnt < this._agMap.LayerCount; intCnt++) {
				agLayer = this._agMap.get_Layer(intCnt);
				
				if(agLayer == ChildLayer) {
					blnDetect = true;
					break;
				}
				else if(agLayer is ICompositeLayer) {
					agCompLayers.Add(agLayer as ICompositeLayer);
				}
			}
			
			// 未検出時はｸﾞﾙｰﾌﾟﾚｲﾔｰを調査
			if(!blnDetect) {
				foreach(ICompositeLayer agCompLayer in agCompLayers) {
					agParentLayer = (ILayer)this.GetParentLayer(agCompLayer, ChildLayer);
					if(agParentLayer != null) {
						break;
					}
				}
			}
			
			return agParentLayer;
		}
		
		/// <summary>
		/// 指定レイヤーの親レイヤーを取得します
		/// </summary>
		/// <param name="ParentLayer">親レイヤー</param>
		/// <param name="ChildLayer">対象レイヤー</param>
		/// <returns>親レイヤー</returns>
		private ICompositeLayer GetParentLayer(ICompositeLayer ParentLayer, ILayer ChildLayer) {
			ICompositeLayer	agCompLayer = null;
			
			ILayer	agTempLayer;
			for(int intCnt=0; intCnt < ParentLayer.Count; intCnt++) {
				agTempLayer = ParentLayer.get_Layer(intCnt);
				if(ChildLayer == agTempLayer) {
					agCompLayer = ParentLayer;
				}
				// 子ｸﾞﾙｰﾌﾟを検索
				else if(agTempLayer is ICompositeLayer) {
					agCompLayer = this.GetParentLayer(agTempLayer as ICompositeLayer, ChildLayer);
				}
				
				if(agCompLayer != null) {
					break;
				}
			}
			
			return agCompLayer;
		}
		
		/// <summary>
		/// 全レイヤの構成を適用します
		/// </summary>
		public void CommitAllLayers() {
			// 全ﾚｲﾔを削除
			for(int intCnt=this._agMap.LayerCount - 1; intCnt >= 0; intCnt--) {
				this._agMap.DeleteLayer(this._agMap.get_Layer(intCnt));
			}
			
			// 全ﾚｲﾔを追加
			ILayer		agLayer;
			TreeNode	node;
			for(int intCnt=this.treeView1.Nodes.Count - 1; intCnt >= 0; intCnt--) {
				// ﾉｰﾄﾞを取得
				node = this.treeView1.Nodes[intCnt];
				
				// ﾏｯﾌﾟにﾚｲﾔを追加
				agLayer = node.Tag as ILayer;
				this._agMap.AddLayer(agLayer);
				
				// ｸﾞﾙｰﾌﾟﾚｲﾔを構成
				if(agLayer is ICompositeLayer) {
					this.AddGroupinLayer(node);
				}
			}
		}
		
		private void AddGroupinLayer(TreeNode GroupNode) {
			IGroupLayer	agGLayer = GroupNode.Tag as IGroupLayer;
			
			agGLayer.Clear();
			
			ILayer		agLayer;
			foreach(TreeNode node in GroupNode.Nodes) {
				agLayer = node.Tag as ILayer;
				agGLayer.Add(agLayer);

				if(agLayer is ICompositeLayer) {
					// ｸﾞﾙｰﾌﾟﾚｲﾔに追加
					this.AddGroupinLayer(node);
				}
			}
		}
	}
}
