using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// EngineではGxDialogが使えずデータ追加ボタンしかないため
    /// 代替としてGISデータを選択するダイアログ
    /// </summary>
    #region 呼び出し例
    /*
     * 
            using (FormGISDataSelectDialog frm = new FormGISDataSelectDialog()) // usingを使って頂戴
            {
                //frm.StartFolder = @"C:\arcgis\ArcTutor\Editing"; // オプション1:開始フォルダ
                frm.SelectType = FormGISDataSelectDialog.ReturnType.WorkspaceName;//オプション2:選択したいデータ種類

                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    string msg = "選択したもの\n\n" + frm.SelectedPath;
                    if (frm.ReturnObject) // IDatasetName or IWorkspaceName
                    {
                        msg += "\n ReturnObject = true";
                        //MessageBox.Show(frm.SelectedPath + " : " + frm.SelectedObject.GetType().FullName);
                        if (frm.SelectedObject is System.Collections.Generic.List<IDatasetName>)
                        {
                            List<IDatasetName> dsNameColl = (List<IDatasetName>)frm.SelectedObject;
                            foreach (var dsName in dsNameColl)
                            {
                                if (dsName.WorkspaceName.WorkspaceFactoryProgID == "esriDataSourcesFile.CadWorkspaceFactory.1")
                                {
                                    msg += "\nCAD ドローイング" + "," + dsName.Name;
                                }
                                else
                                {
                                    msg += "\n" + dsName.Category + "," + dsName.Name;
                                }
                                //IDataset ds = (IDataset)name.Open();
                            }
                        }
                        else if (frm.SelectedObject is System.Collections.Generic.List<IWorkspaceName>)
                        {
                            List<IWorkspaceName> wsNameColl = (List<IWorkspaceName>)frm.SelectedObject;
                            foreach (var wsName in wsNameColl)
                            {
                                msg += "\n" + wsName.Category + "," + wsName.BrowseName;
                                //IName name = (IName)wsName;
                                //IWorkspace ws = (IWorkspace)name.Open();
                            }
                        }
                        else if (frm.SelectedObject is System.Collections.Generic.List<IFileName>)
                        {
                            List<IFileName> fileName = (List<IFileName>)frm.SelectedObject;
                            foreach (var fname in fileName)
                            {
                                msg += "LAYER ファイル ," + System.IO.Path.GetFileName(fname.Path);
                            }
                        }
                    }
                    else // それ以外
                    {
                        msg += "\n ReturnObject = false";
                        if (System.IO.Directory.Exists(frm.SelectedPath))
                        {
                            msg += "\nフォルダ";
                        }
                        else if (System.IO.File.Exists(frm.SelectedPath))
                        {
                            string ext = System.IO.Path.GetExtension(frm.SelectedPath);
                            if (ext.ToUpper().Equals(".MXD"))
                            {
                                msg += "\nマップ ドキュメント";
                            }
                            else if (ext.ToUpper().Equals(".PRJ"))
                            {
                                msg += "\nプロジェクト ファイル";
                            }
                        }
                    }
                    MessageBox.Show(msg);
                }
            }

     *  
     * 
     */
    #endregion
    public partial class FormGISDataSelectDialog : Form
    {
        internal enum ReturnType
        {
            All = 0,
            DatasetName = 1,
            WorkspaceName = 2,
            LayerFile = 3,
            MapDocumentFile = 4,
            FeaatureClass = 5,
            TableOrText = 6,
            GdbWorkspaceName = 7 // PGDB or FGDB
        }

        private ReturnType selectType = ReturnType.All;
        internal ReturnType SelectType
        {
            set { this.selectType = value; }
        }

        internal string StartFolder
        {
            set {
				this.lightTreeView1.StartFolder = value;
			}
        }

        internal bool ReturnObject
        {
            get { return this.lightTreeView1.ReturnObject; }
        }

        internal string SelectedPath
        {
            get { return this.lightTreeView1.SelectPath; }
        }

        internal object SelectedObject
        {

            get { return this.lightTreeView1.SelectObject; }

        }
        
        /// <summary>
        /// 「フォルダ接続」を表示するかどうか
        /// </summary>
        internal bool ShowConnectFolder {
			get {
				return this.lightTreeView1.ShowConnectFolder;
			}
			set {
				this.lightTreeView1.ShowConnectFolder = value;
			}
        }
        
        /// <summary>
        /// 「DB接続」を表示するかどうか
        /// </summary>
        internal bool ShowDBConnect {
			get {
				return this.lightTreeView1.ShowDBConnect;
			}
			set {
				this.lightTreeView1.ShowDBConnect = value;
			}
        }

        /// <summary>
        /// 「ArcGISServer接続」を表示するかどうか
        /// </summary>
        internal bool ShowGISServer {
			get {
				return this.lightTreeView1.ShowDBConnect;
			}
			set {
				this.lightTreeView1.ShowGISServer = value;
			}
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormGISDataSelectDialog() {
            InitializeComponent();
        }

        private esriControlsDragDropEffect m_Effect;
        private void axMapControl1_OnOleDrop(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnOleDropEvent e)
        {
            IDataObjectHelper dataObject = (IDataObjectHelper)e.dataObjectHelper;
            esriControlsDropAction action = e.dropAction;

            e.effect = (int)esriControlsDragDropEffect.esriDragDropNone;

            if (action == esriControlsDropAction.esriDropEnter)
            {
                if (dataObject.CanGetFiles() | dataObject.CanGetNames())
                {
                    m_Effect = esriControlsDragDropEffect.esriDragDropCopy;
                }
            }

            if (action == esriControlsDropAction.esriDropOver)
            {
                e.effect = (int)m_Effect;
            }

            if (action == esriControlsDropAction.esriDropped)
            {
                System.Diagnostics.Debug.WriteLine("esriDropped");
                if (dataObject.CanGetFiles())
                {
                    System.Array filePaths = System.Array.CreateInstance(typeof(string), 0, 0);
                    filePaths = (System.Array)dataObject.GetFiles();
                    for (int i = 0; i <= filePaths.Length - 1; i++)
                    {
                        System.Diagnostics.Debug.WriteLine(filePaths.GetValue(i));
                    }

                }
                else if (dataObject.CanGetNames())
                {

                }
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            errorProvider1.Clear();

            switch (selectType)
            {
                case ReturnType.FeaatureClass:
                    if (lightTreeView1.SelectObject is List<ESRI.ArcGIS.Geodatabase.IDatasetName>)
                    {
                        IDatasetName dsName = ((List<IDatasetName>)lightTreeView1.SelectObject)[0];
                        if (dsName.Type == esriDatasetType.esriDTFeatureClass)
                        {
                            this.DialogResult = DialogResult.OK;
                        }
                        else 
                        {
                            errorProvider1.SetError(this.label1, "フィーチャ クラスを選択してください");
                            errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                            this.DialogResult = DialogResult.None;
                        }
                    }
                    else 
                    {
                        errorProvider1.SetError(this.label1, "フィーチャ クラスを選択してください");
                        errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                        this.DialogResult = DialogResult.None;
                    }
                    break;

                case ReturnType.TableOrText:
                    if(lightTreeView1.SelectObject is List<ESRI.ArcGIS.Geodatabase.IDatasetName>)
                    {
                        IDatasetName dsName = ((List<IDatasetName>)lightTreeView1.SelectObject)[0];
                        if ((dsName.Type == esriDatasetType.esriDTTable) || (dsName.Type == esriDatasetType.esriDTText))
                        {
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            errorProvider1.SetError(this.label1, "テーブル または テキストファイルを選択してください");
                            errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                            this.DialogResult = DialogResult.None;
                        }
                    }
                    // DB ｸｴﾘ ﾃｰﾌﾞﾙ
                    else if(lightTreeView1.SelectObject is List<Common.UserSelectQueryTableSet>) {
						this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        errorProvider1.SetError(this.label1, "テーブル または テキストファイルを選択してください");
                        errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                        this.DialogResult = DialogResult.None;
                    }
                    break;
                case ReturnType.DatasetName:
                    if (lightTreeView1.SelectObject is List<ESRI.ArcGIS.Geodatabase.IDatasetName>)
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        errorProvider1.SetError(this.label1, "データ セットを選択してください");
                        errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                        this.DialogResult = DialogResult.None;
                    }
                    break;
                case ReturnType.GdbWorkspaceName:
                    if (lightTreeView1.SelectObject is List<ESRI.ArcGIS.Geodatabase.IWorkspaceName>)
                    {
                        IWorkspaceName wsName = ((List<IWorkspaceName>)lightTreeView1.SelectObject)[0];
                        if (wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.AccessWorkspaceFactory") ||
                            wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.FileGDBWorkspaceFactory"))
                        {
                            this.DialogResult = DialogResult.OK;
                        }
                        else 
                        {
                            errorProvider1.SetError(this.label1, "ジオデータベース ワーク スペースを選択してください");
                            errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                            this.DialogResult = DialogResult.None;
                        }
                    }
                    else 
                    {
                        errorProvider1.SetError(this.label1, "ジオデータベース ワーク スペースを選択してください");
                        errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                        this.DialogResult = DialogResult.None;
                    }
                    break;
                case ReturnType.WorkspaceName:
                    if (lightTreeView1.SelectObject is List<ESRI.ArcGIS.Geodatabase.IWorkspaceName>)
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else if (System.IO.Directory.Exists(lightTreeView1.SelectPath))
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        errorProvider1.SetError(this.label1, "ワーク スペースを選択してください");
                        errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                        this.DialogResult = DialogResult.None;
                    }
                    break;
                case ReturnType.LayerFile:
                    if (lightTreeView1.SelectObject is List<ESRI.ArcGIS.esriSystem.IFileName>)
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        errorProvider1.SetError(this.label1, "レイヤ ファイルを選択してください");
                        errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                        this.DialogResult = DialogResult.None;
                    }
                    break;
                case ReturnType.MapDocumentFile:
                    string ext = System.IO.Path.GetExtension(lightTreeView1.SelectPath);
                    if (ext.ToUpper().Equals(".MXD"))
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        errorProvider1.SetError(this.label1, "マップ ドキュメントファイルを選択してください");
                        errorProvider1.SetIconAlignment(this.label1, ErrorIconAlignment.TopRight);
                        this.DialogResult = DialogResult.None;
                    }
                    break;
                default:
                    this.DialogResult = DialogResult.OK;
                    break;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormGISDataSelectDialog_Load(object sender, EventArgs e)
        {

            string subtext = "";
            switch (selectType)
            {
                case ReturnType.GdbWorkspaceName:
                    subtext = "ジオデータベース ワーク スペース";
                    break;
                case ReturnType.DatasetName:
                    subtext = "データ セット";
                    break;
                case ReturnType.WorkspaceName:
                    subtext = "ワーク スペース";
                    break;
                case ReturnType.LayerFile:
                    subtext = "レイヤ ファイル";
                    break;
                case ReturnType.MapDocumentFile:
                    subtext = "マップ ドキュメントファイル";
                    break;
                case ReturnType.FeaatureClass:
                    subtext = "フィーチャ クラス";
                    break;
                case ReturnType.TableOrText:
                    subtext = "テーブル , テキストファイル";
                    break;
                default:
                    break;
            }
            this.Text = string.Format("{0} [{1}の選択]", this.Text, subtext);

            this.Width = 480;
            this.Height = 360;

            // 親フォームの左上を基準に
            if (this.Owner != null)
            {
                int offset = 25;

                //this.Width = this.Owner.Width;
                //this.Height = this.Owner.Height;
                this.StartPosition = FormStartPosition.Manual;
                this.Left = this.Owner.Left + offset; //(this.Owner.Width - this.Width) / 2;
                this.Top = this.Owner.Top + offset; // (this.Owner.Height - this.Height) / 2;
            }
            

        }


    }

}
