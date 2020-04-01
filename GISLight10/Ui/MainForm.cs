#region USING
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Xml;
using System.Drawing.Printing;
using System.Timers;
using System.Linq;
using System.Text;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;


using ESRIJapan.GISLight10.Properties;
using ESRIJapan.GISLight10.Common;
using ESRIJapan.GISLight10.EngineEditTask;
#endregion

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ESRIJapanGISLight�̃��C���t�H�[��
    /// </summary>
    ///
    /// <history>
    ///  2010/11/01 �V�K�쐬 
    ///  2011/01/07 �}�b�v��}�E�X�z�C�[������s����Ή�(�T�u�E�B���h�E�\�������Ή�) 
    ///  2011/01/21 xml�R�����g������:�L�q�ΏۃX�R�[�v��public,protected 
    ///  2011/01/25 xml�R�����g�L�q�ΏۃX�R�[�v��internal���ǉ� 
    ///  2011/01/25 xml�R�����greturns�^�O�L�q�R��̑Ή� 
    ///  2011/04/12 �y�[�W���C�A�E�g�ɉ摜��荞�݃R�}���h��ǉ�
    ///  2011/08/05 ArcGIS10���ŉ��L�����s����Ɖ�ʕ����̂�GetWindowLong�ȂǃR�����g�A�E�g
    /// </history>
    public sealed partial class MainForm : Form
    {
        #region WinAPI

        #region 2019/02/21 �e�X�g�p
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="childAfter"></param>
        /// <param name="className"></param>
        /// <param name="windowTitle"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);
        #endregion

        /// <summary>
        /// ����f�o�C�X���擾
        /// </summary>
        /// <param name="hdc">�f�o�C�X�R���e�L�X�g�̃n���h��</param>
        /// <param name="nIndex">�擾���̍��ڎ��</param>
        /// <returns>���ڎ�ނɑΉ������f�o�C�X���</returns>
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);

        /// <summary>
        /// �w��f�o�C�X�̃f�o�C�X�R���e�L�X�g�쐬
        /// </summary>
        /// <param name="strDriver">�h���C�o��</param>
        /// <param name="strDevice">�f�o�C�X��</param>
        /// <param name="strOutput">���g�p�ANULL</param>
        /// <param name="pData">�I�v�V�����̃v�����^�f�[�^</param>
        /// <returns>
        /// <br>�������F�w��f�o�C�X�Ɋ֘A����f�o�C�X�R���e�L�X�g�̃n���h��</br>
        /// <br>���s���FNULL</br>
        /// </returns>
        [DllImport("GDI32.dll")]
        public static extern int CreateDC(
            string strDriver, string strDevice, string strOutput, IntPtr pData);

        /// <summary>
        /// �f�o�C�X�R���e�L�X�g�̉��
        /// </summary>
        /// <param name="hWnd">�E�B���h�E�n���h��</param>
        /// <param name="hDC">�f�o�C�X�R���e�L�X�g�̃n���h��</param>
        /// <returns>
        /// <br>�f�o�C�X�R���e�L�X�g��������ꂽ�Ƃ��F1</br>
        /// <br>�f�o�C�X�R���e�L�X�g���������Ȃ��Ƃ��F0</br>
        /// </returns>
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);

        /// <summary>
        /// �E�C���h�E�J�X�^�}�C�Y
        /// </summary>
        /// <param name="h">�E�B���h�E�n���h��</param>
        /// <param name="s">�^�C�g���e�L�X�g�܂��̓R���g���[���e�L�X�g</param>
        [DllImport("User32.Dll")]
        public static extern void SetWindowText(int h, String s);

        /// <summary>
        /// �_�C�A���O�{�b�N�X���̃R���g���[���̃^�C�g���܂��̓e�L�X�g��ݒ�
        /// </summary>
        /// <param name="hWnd">�_�C�A���O�{�b�N�X�̃n���h��</param>
        /// <param name="nIDDlgItem">�R���g���[���̎��ʎq</param>
        /// <param name="lpString">�ݒ肵�����e�L�X�g</param>
        /// <returns>
        /// <br>�������F0 �ȊO�̒l</br>
        /// <br>���s���F0</br>
        /// </returns>
        [DllImport("user32.dll")]
        private static extern bool SetDlgItemText(IntPtr hWnd, int nIDDlgItem, string lpString);

        /// <summary>
        /// �E�B���h�E���A�N�e�B�u���܂��͔�A�N�e�B�u����
        /// �E�B���h�E�֑��M����郁�b�Z�[�W
        /// </summary>
        public const int WM_ACTIVATE = 0x0006;

        /// <summary>
        /// �c�[���o�[�̓��ߐݒ�p
        /// </summary>
        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hWnd, int Index);

        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int SetWindowLong(IntPtr hWnd, int Index, int Value);

        const int GWL_STYLE = -16;
        const int WS_CLIPCHILDREN = 0x02000000;

        // �}�E�X�z�C�[������s����Ή� -->
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const int WM_MOUSEWHEEL = 0x20A;
        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;

        #endregion

        /// <summary>
        /// �E�B���h�E�֑��M����郁�b�Z�[�W�ύX�`�F�b�N�t���O
        /// </summary>
        internal bool setPrintDialogTitle = false;

        /// <summary>
        /// ����ݒ�A����_�C�A���O�^�C�g��
        /// </summary>
        internal string printDialogTitle = "";

        /// <summary>
        /// SetDlgItemText�Ŏg�p����
        /// �R���g���[���̎��ʎq
        /// </summary>
        public const int ID_BUT_OK = 1;

        /// <summary>
        /// ����ݒ�A����_�C�A���O�^�C�g���ύX�t���O
        /// </summary>
        internal bool setPrintDialogButton = false;

        /// <summary>
        /// ����ݒ�A����_�C�A���OOK�{�^���L���v�V����
        /// </summary>
        internal string printDialogButton = "";


        /// <summary>
        /// Windows���b�Z�[�W������
        /// </summary>
        /// <param name="m">�����Ώۂ�Windows Message</param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // ���b�Z�[�W�`�F�b�N
            if (setPrintDialogTitle && (m.Msg == WM_ACTIVATE))
            {
                // �ύX�t���O
                setPrintDialogTitle = false;

                // �^�C�g���ύX
                SetWindowText((int)m.LParam, printDialogTitle);
            }

            // ���b�Z�[�W�`�F�b�N
            if (setPrintDialogButton && (m.Msg == WM_ACTIVATE))
            {
                // �ύX�t���O
                setPrintDialogButton = false;

                // �^�C�g���ύX
                SetDlgItemText(m.LParam, ID_BUT_OK, printDialogButton);
            }

            // �}�E�X�z�C�[������s����Ή��� -->
            if (m.Msg == WM_MOUSEWHEEL)
            {
                this.axMapControl1.Select();
                return;
            } //<--

            base.WndProc(ref m);
        }

        #region Declare Event Handlers

        // Notes:
        // Variables are prefixed with an 'm_' denoting that they are member variables.
        // This means they are global in scope for this class.
        //private ESRI.ArcGIS.Carto.IActiveViewEvents_AfterDrawEventHandler m_ActiveViewEventsAfterDraw;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_AfterItemDrawEventHandler m_ActiveViewEventsAfterItemDraw;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ContentsChangedEventHandler m_ActiveViewEventsContentsChanged;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ContentsClearedEventHandler m_ActiveViewEventsContentsCleared;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_FocusMapChangedEventHandler m_ActiveViewEventsFocusMapChanged;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ItemAddedEventHandler m_ActiveViewEventsItemAdded;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ItemDeletedEventHandler m_ActiveViewEventsItemDeleted;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ItemReorderedEventHandler m_ActiveViewEventsItemReordered;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_SelectionChangedEventHandler m_ActiveViewEventsSelectionChanged;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_SpatialReferenceChangedEventHandler m_ActiveViewEventsSpatialReferenceChanged;
        //private ESRI.ArcGIS.Carto.IActiveViewEvents_ViewRefreshedEventHandler m_ActiveViewEventsViewRefreshed;
        // �y�[�W���C�A�E�g�C�x���g
        private IPageLayoutControlEvents_OnViewRefreshedEventHandler m_PageLayoutControlEventsOnViewRefreshed;
        private IPageLayoutControlEvents_OnFocusMapChangedEventHandler m_PageLayoutControlEventsOnFocusMapChanged;
        private IPageLayoutControlEvents_OnExtentUpdatedEventHandler m_PageLayoutControlEventsOnExtentUpdated;

        #endregion Declare Event Handlers


        #region print concern
        //declare the dialogs for print preview, print dialog, page setup
        /// <summary>
        /// �v�����g�v���r���[�_�C�A���O
        /// </summary>
        internal PrintPreviewDialog printPreviewDialog1;

        /// <summary>
        /// �v�����g�_�C�A���O
        /// </summary>
        internal PrintDialog printDialog1;

        /// <summary>
        /// �v�����^�ݒ�_�C�A���O
        /// </summary>
        internal PageSetupDialog pageSetupDialog1;

        /// <summary>
        /// �p���T�C�Y�L���v�V����
        /// </summary>
        internal string[] supportPaperSizeName = { "A3", "A4" };
        /// <summary>
        /// �p���T�C�Y�ύX�\�t���O
        /// </summary>
        internal bool[] supportPaperSizeEnable = { false, false };
        /// <summary>
        /// A4�p���T�C�Y�L���v�V�����C���f�N�X
        /// </summary>
        internal int A3_Index = 0;
        /// <summary>
        /// A3�p���T�C�Y�L���v�V�����C���f�N�X
        /// </summary>
        internal int A4_Index = 1;
        /// <summary>
        /// �p���������L���v�V����
        /// </summary>
        internal string pageLandscape = "��";
        /// <summary>
        /// �p���c�����L���v�V����
        /// </summary>
        internal string pagePortrait = "�c";

        private bool canContinuePageLayout = false;

		/// <summary>
		/// �}�b�v�̃y�[�W�T�C�Y�\�L���擾���܂�
		/// </summary>
		private	Dictionary<int, string>	_dicPageSizes = new Dictionary<int,string>();
		// ϯ�߂��߰�ޥ���ޕ\�L��������
		private void InitPageSizes() {
			if(_dicPageSizes.Count <= 0) {
				_dicPageSizes.Add(esriPageFormID.esriPageFormLetter.GetHashCode(), "���^�[");
				_dicPageSizes.Add(esriPageFormID.esriPageFormLegal.GetHashCode(), "���[�K��");
				_dicPageSizes.Add(esriPageFormID.esriPageFormTabloid.GetHashCode(), "�^�u���C�h");
				_dicPageSizes.Add(esriPageFormID.esriPageFormC.GetHashCode(), "C");
				_dicPageSizes.Add(esriPageFormID.esriPageFormD.GetHashCode(), "D");
				_dicPageSizes.Add(esriPageFormID.esriPageFormE.GetHashCode(), "E");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA5.GetHashCode(), "A5");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA4.GetHashCode(), "A4");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA3.GetHashCode(), "A3");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA2.GetHashCode(), "A2");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA1.GetHashCode(), "A1");
				_dicPageSizes.Add(esriPageFormID.esriPageFormA0.GetHashCode(), "A0");
				_dicPageSizes.Add(esriPageFormID.esriPageFormCUSTOM.GetHashCode(), "�J�X�^��");
				_dicPageSizes.Add(esriPageFormID.esriPageFormSameAsPrinter.GetHashCode(), "�p��");
			}
		}

        /// <summary>
        /// �p���T�C�Y��������
        /// </summary>
        public bool ContinuPageLayout
        {
            get { return this.canContinuePageLayout; }
            set { this.canContinuePageLayout = value; }
        }

        //internal IPageLayout savePageLayout = null;

        /// <summary>
        /// ������Ԃ̃y�[�W���C�A�E�g
        /// </summary>
        internal IPageLayout initPageLayout = null;

        /// <summary>
        /// MXD�I�[�v������Maps
        /// </summary>
        internal IMaps originalMaps = null;

        //declare a PrintDocument object named document, to be displayed in the print preview

        /// <summary>
        /// �y�[�W���C�A�E�g�Ŏg�p�������\�ȃh�L�������g�I�u�W�F�N�g
        /// </summary>
        internal System.Drawing.Printing.PrintDocument document =
            new System.Drawing.Printing.PrintDocument();

        //cancel tracker which is passed to the output function when printing to the print preview
        private ITrackCancel m_TrackCancel = new CancelTrackerClass();

        //the page that is currently printed to the print preview
        private short m_CurrentPrintPage;

        /// <summary>
        /// �v���r���[�R�}���h���s�t���O
        /// </summary>
        internal bool isPreviewPage = false;

        /// <summary>
        /// �v�����g�R�}���h���s�t���O
        /// </summary>
        internal bool isPrintPage = false;

        #endregion

        #region class private members

        private ESRI.ArcGIS.Controls.IPageLayoutControl3 m_pageLayoutControl = null;
        private Common.ControlsSynchronizer m_controlsSynchronizer = null;

        private System.Windows.Forms.Cursor preCursor = Cursors.Default;

        /// <summary>
        /// �}�b�v�R���g���[���ƃy�[�W���C�A�E�g�R���g���[����
        /// �������s��ControlsSynchronizer�ւ̎Q��
        /// </summary>
        public Common.ControlsSynchronizer GetMapControlSyncronizer
        {
            get { return this.m_controlsSynchronizer; }
        }

        private string ScaleBarElementName = "ScaleBar";

        /// <summary>
        /// �y�[�W���C�A�E�g�Ŏg�p����
        /// �X�P�[���o�[�i�k�ڋL���j�I�u�W�F�N�g�̖���
        /// </summary>
        public string SCALE_BAR_ELEMENT_NAME
        {
            get { return this.ScaleBarElementName; }
        }

        // �}�b�v��ԕύX���씻�f���̑Ώۂ̃c�[���o�[�R�}���h�̃C���f�N�X�ێ�
        private int CurrentToolIndex = -1;
        private int PageLayoutCurrentToolIndex = -1;

        // �}�b�v��ԕύX���씻�f���̏��O�Ώۂ̃c�[���o�[�R�}���h�̃C���f�N�X
        private const int OpenDox_Index = 0;
        private const int SaveDoc_Index = 1;
        private const int Addlayer_Index = 2;
        private const int KobetsuZokusei_Index = 17;
        private const int Kensaku_Index = 18;
        private const int XY_Idou_Index = 19;
        private const int Keisoku_Index = 20;
        private const int HyperLink_Index = 21;
        private const int Swipe_Index = 22;

        // m_map is set to be global in scope because it will be used for both the AddHandler and RemoveHandler
        private ESRI.ArcGIS.Carto.IMap m_map;
        private ILayer _selectedLayer = null;

        private ITOCControl2 m_tocControl = null;

        private int map_sts_changed_cmt = 0;
        private bool map_sts_changed = false;

        /// <summary>
        /// ���C���}�b�v�ύX���
        /// </summary>
        public bool MainMapChanged
        {
            get { return this.map_sts_changed; }
            set { this.map_sts_changed = value;}
        }

        // ���C���[���ߐݒ�ɕK�v
        private CommandsEnvironmentClass m_CommandsEnvironment = null;

        /// <summary>
        /// ���C���t�H�[���̃^�C�g��
        /// </summary>
        public string Title_Name = null; // Form�^�C�g��

        // �v���O�C�����s���p (MapDocumentFile)
        private string m_mapDocumentName = string.Empty;

        // �v���O�C�����j���[�N���b�N��,���̂���v���O�C���C���X�^���X�̃C���f�N�X�擾
        private System.Collections.Hashtable _pluginNameList = null;

#region �v���O�C���E�C���^�[�t�F�[�X�p��
#if DEBUG
		// IActiveView ����ĥ�ڰ��p����
		static public bool Debug_Confirm = true;
#endif

	#region �v���p�e�B
		/// <summary>
		/// �J���Ă���}�b�v�h�L�������g���擾���܂�
		/// </summary>
		public string MapDocumentFile {
			get {
#if DEBUG
				Debug.WriteLineIf(Debug_Confirm, "��MapDocument Property Call : " + this.
                    m_mapDocumentName);
#endif
				return this.m_mapDocumentName;
			}
		}

		/// <summary>
		/// �}�b�v�̕\���k�ڂ��擾�܂��͐ݒ肵�܂�
		/// </summary>
		public double MapScale {
			get {
				double dblScale;

				try {
					// ���e�ɂ���Ă͎擾�s��
					dblScale = this.m_map.MapScale;
				}
				catch {
					dblScale = double.NaN;
				}

				return dblScale;
			}
			set {
				// �ݒ�\�ȏꍇ�̂ݑΉ�
				if(!this.MapScale.Equals(double.NaN)) {
					this.m_MapControl.MapScale = value;
				}
			}
		}

		/// <summary>
		/// �}�b�v�̕\���͈͂��擾�܂��͐ݒ肵�܂�
		/// </summary>
		public ESRI.ArcGIS.Geometry.IEnvelope MapExtent {
			get {
				return this.m_MapControl.Extent;
			}
			set {
#if DEBUG
				Debug.WriteLine("��MapExtent Set.");
#endif
				this.m_MapControl.Extent = value;
			}
		}

        /// <summary>
        /// �I�����C���[���擾�܂��͐ݒ肵�܂� (TOC)
        /// </summary>
        public ILayer SelectedLayer {
            get { return this._selectedLayer; }
            set {
				if(value != null) {
					this._selectedLayer = value;
					this.m_TocControl.SelectItem(value, null);
				}
			}
        }

        /// <summary>
        /// ���ׂẴt�B�[�`���[���C���[���擾���܂�
        /// </summary>
        public IFeatureLayer[] FeatureLayers {
			get {
				LayerManager	clsLM = new LayerManager();
				return clsLM.GetFeatureLayers(this.m_map).ToArray();
			}
        }

        delegate bool GetMapVisibleCallBack();
        delegate void SetMapVisibleCallBack(int TabID);
        private bool GetMapVisible() {
			return this.tabControl1.SelectedIndex.Equals(0);
        }
        private void SetMapVisible(int TabID) {
			if(TabID < this.tabControl1.TabCount) {
				this.tabControl1.SelectedIndex = TabID;
			}
        }

        /// <summary>
        /// �}�b�v��ʂ�\�����Ă��邩�ǂ������擾�܂��͐ݒ肵�܂�
        /// </summary>
        public bool IsMapVisible {
			get {
				if(this.InvokeRequired) {
					GetMapVisibleCallBack	dlgMV = new GetMapVisibleCallBack(this.GetMapVisible);
					return (bool)this.Invoke(dlgMV);
				}
				else {
					return this.GetMapVisible();
				}
			}
			set {
				int	intVal = value ? 0 : 1;
				if(this.InvokeRequired) {
					SetMapVisibleCallBack	dlgMV = new SetMapVisibleCallBack(this.SetMapVisible);
					this.Invoke(dlgMV, new object[] { intVal });
				}
				else {
					this.tabControl1.SelectedIndex = intVal;
				}
			}
        }

		/// <summary>
		/// �ҏW��Ԃ��ǂ������擾���܂�
		/// </summary>
		public bool IsEditMode {
			get {
				return m_EngineEditor.EditState != esriEngineEditState.esriEngineStateNotEditing;
			}
		}
		/// <summary>
		/// �ҏW�Ώۃ��C�����擾���܂�
		/// </summary>
		public IFeatureLayer EditTargetLayer {
			get {
				IFeatureLayer	agFL = null;
				if(this.IsEditMode) {
					agFL = (this.m_EngineEditor as IEngineEditLayers).TargetLayer;
				}

				return agFL;
			}
			set {
				if(this.IsEditMode) {
					// ��ĉ\���m�F
					IEngineEditLayers	agEdLayers = this.m_EngineEditor as IEngineEditLayers;
					if(agEdLayers.IsEditable(value)) {
						agEdLayers.SetTargetLayer(value, 0);
					}
				}
			}
		}

		/// <summary>
		/// �ҏW�Ώۃ��[�N�X�y�[�X���擾���܂�
		/// </summary>
		public IWorkspace EditWorkspace {
			get {
				IWorkspace	agWS = null;
				if(this.IsEditMode) {
					agWS = this.m_EngineEditor.EditWorkspace;
				}

				return agWS;
			}
		}

		/// <summary>
		/// �ҏW�I���t�B�[�`���[�����擾���܂�
		/// </summary>
		public int EditSelectionNum {
			get {
				int	intRet = 0;

				if(this.IsEditMode) {
					intRet = this.m_EngineEditor.SelectionCount;
				}

				return intRet;
			}
		}
	#endregion

	#region ���\�b�h
		/// <summary>
		/// �w��̃��C�������݂��邩�ǂ������擾���܂�
		/// </summary>
		/// <param name="LayerName">���C����</param>
		/// <returns>�L / ��</returns>
		public bool HasLayer(string LayerName) {
			return this.GetLayer(LayerName) != null;
		}

		/// <summary>
		/// �w��̃��C�����擾���܂�
		/// </summary>
		/// <param name="LayerName">���C����</param>
		/// <returns>�Ώۃ��C��</returns>
		public ILayer GetLayer(string LayerName) {
			// ڲԂ��擾
			LayerManager	clsLM = new LayerManager();
			ILayer agLayer = clsLM.GetAllLayers(this.m_map).Find(L=>L.Name.Equals(LayerName));

			return agLayer;
		}

		/// <summary>
		/// �}�b�v�͈͂�S��ɐݒ肵�܂�
		/// </summary>
		public void ShowMapFullExtent() {
			IActiveView	agActView = this.m_MapControl.ActiveView;
			agActView.Extent = agActView.FullExtent;

			// �`��X�V
			agActView.Refresh();
		}

		/// <summary>
		/// �w��̃I�u�W�F�N�g���w��͈̔͂ōĕ`�悵�܂�
		/// </summary>
		/// <param name="Extent"></param>
		/// <param name="DataObject"></param>
		public void RefreshViewExtent(ESRI.ArcGIS.Geometry.IEnvelope Extent, object DataObject) {
			IActiveView	agActView = (this.IsMapVisible ? this.m_MapControl.ActiveView : this.m_pageLayoutControl.ActiveView);
#if DEBUG
			Debug.WriteLine(string.Format("��RefreshViewExtent / IsActive : {0}, InvokeRequired : {1}", agActView.IsActive() ? "YES" : "NO", this.InvokeRequired ? "YES" : "NO"));
#endif
			if(agActView != null) {
				// �n�}�͈͂��擾
				if(Extent != null && !Extent.IsEmpty) {
					agActView.Extent = Extent;
				}

				agActView.PartialRefresh(esriViewDrawPhase.esriViewGeography | esriViewDrawPhase.esriViewGeoSelection | esriViewDrawPhase.esriViewBackground, DataObject, agActView.Extent);
				//agActView.PartialRefresh(esriViewDrawPhase.esriViewAll, DataObject, Extent);
				//this.m_MapControl.ActiveView.ScreenDisplay.UpdateWindow();
			}
		}
		/// <summary>
		/// ���݂̃r���[�͈͂��ĕ`�悵�܂�
		/// </summary>
		public void RefreshViewExtent() {
			// ���ް۰��
			this.RefreshViewExtent(null, null);
		}
		/// <summary>
		/// �w��͈̔͂��ĕ`�悵�܂�
		/// </summary>
		/// <param name="Extent"></param>
		public void RefreshViewExtent(ESRI.ArcGIS.Geometry.IEnvelope Extent) {
			// ���ް۰��
			this.RefreshViewExtent(Extent, null);
		}

		/// <summary>
		/// �r���[�̑S�̂��ĕ`�悵�܂�
		/// </summary>
		public void RefreshView() {
			//IActiveView	agActView = (this.IsMapVisible ? this.m_MapControl.ActiveView : this.m_pageLayoutControl.ActiveView);
			IActiveView	agActView = (this.IsMapVisible ? this.m_map as IActiveView : this.m_pageLayoutControl.PageLayout as IActiveView);
#if DEBUG
			Debug.WriteLine("��RefreshView");
#endif
			if(agActView != null) {
				agActView.Refresh();
			}
		}

		/// <summary>
		/// �t�H�[����\�����܂�
		/// </summary>
		/// <param name="FormWindow"></param>
		public void ShowForm(Form FormWindow) {
			FormWindow.Show();
		}
		public void ShowFormOnMainForm(Form FormWindow) {
			FormWindow.Show(this);
		}
		public DialogResult ShowFormOnModalWindow(Form FormWindow) {
			return FormWindow.ShowDialog(this);
		}

		// ��عް�
		delegate void			ShowMessageCallBack(string Message);
		delegate DialogResult	ShowConfirmCallBack(string Message);

		/// <summary>
		/// ���b�Z�[�W�{�b�N�X��\�����܂�
		/// </summary>
		/// <param name="Message"></param>
		public void ShowMessage_I(string Message) {
			if(this.InvokeRequired) {
				ShowMessageCallBack	dlgSM = new ShowMessageCallBack(ShowMessage_I);
				this.Invoke(dlgSM, new object[] { Message });
			}
			else {
				Common.MessageBoxManager.ShowMessageBoxInfo(this, Message);
			}
		}
		public void ShowMessage_W(string Message) {
			if(this.InvokeRequired) {
				ShowMessageCallBack	dlgSM = new ShowMessageCallBack(ShowMessage_W);
				this.Invoke(dlgSM, new object[] { Message });
			}
			else {
				Common.MessageBoxManager.ShowMessageBoxWarining(this, Message);
			}
		}
		public void ShowMessage_E(string Message) {
			if(this.InvokeRequired) {
				ShowMessageCallBack	dlgSM = new ShowMessageCallBack(ShowMessage_E);
				this.Invoke(dlgSM, new object[] { Message });
			}
			else {
				Common.MessageBoxManager.ShowMessageBoxError(this, Message);
			}
		}
		public DialogResult ShowMessage_C(string Message) {
			if(this.InvokeRequired) {
				ShowConfirmCallBack	dlgSC = new ShowConfirmCallBack(ShowMessage_C);
				return (DialogResult)this.Invoke(dlgSC, new object[] { Message });
			}
			else {
				return Common.MessageBoxManager.ShowMessageBoxQuestion2(this, Message);
			}
		}
	#endregion
#endregion

        private ITOCControl2 m_TocControl = null;
        private IToolbarMenu2 m_TocMenu = null;
        private IToolbarMenu2 m_TocLayerMenu = null;
        //private IToolbarMenu2 m_ToolBarMenuCustom = null;

        /// <summary>
        /// �I�����ꂽ�e�[�u���ւ̎Q�Ƃ��擾�܂��͐ݒ肵�܂�
        /// </summary>
        private IStandaloneTable _agSelTable = null;
        public IStandaloneTable SelectedTable {
			get {
				return _agSelTable;
			}
			set {
				this._agSelTable = value;
			}
        }

        private IMapControl4 m_MapControl = null;

        /// <summary>
        /// ���C���}�b�v�ւ̎Q��
        /// </summary>
        public IMapControl4 MapControl
        {
            get { return this.m_MapControl; }
        }

        private	bool	_blnUseFieldAlias = true;
        /// <summary>
        /// �t�B�[���h����\������ۂɃG�C���A�X���g�p���邩�ǂ������擾���܂�
        /// </summary>
        public bool IsUseFieldAlias {
			get {
				try {
					// ���݂̐ݒ���擾
					Common.OptionSettings	clsConf = new Common.OptionSettings();
					this._blnUseFieldAlias = clsConf.FieldNmaeUseAlias.Equals("1");
				}
				catch(Exception ex) {
					// �擾���װ (ү���ނ�\�����ď������p�� �߂�l�͑O��̓��e)
					MessageBoxManager.ShowMessageBoxError(this,
						Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead + "[USE FIELD NAME ALIAS SETTING]"
						+ Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
					Common.Logger.Error(Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
						+ "[USE FIELD NAME ALIAS SETTING]" + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

					Common.Logger.Error("MainForm.IsUseFieldAlias : " + ex.Message);
					Common.Logger.Error(ex.StackTrace);
				}

				return this._blnUseFieldAlias;
			}
        }

        /// <summary>
        /// �t�B�[���h�E���X�g�E�A�C�e�����擾���܂�
        /// </summary>
        /// <param name="TableFields">���C���[ / �X�^���h�A�����e�[�u��</param>
        /// <param name="VisibleOnly">���t�B�[���h?</param>
        /// <param name="EditableOnly">�ҏW�\�t�B�[���h?</param>
        /// <param name="OwnFieldOnly">�����t�B�[���h?</param>
        /// <param name="ShowFieldType">�A�C�e���Ƀt�B�[���h�^��\������?</param>
        /// <param name="FieldTypes">���o����t�B�[���h�^</param>
        /// <returns></returns>
        public FieldComboItem[] GetFieldItems(ITableFields TableFields, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// FieldManager���
			return FieldManager.GetFieldItems(TableFields, VisibleOnly, EditableOnly, OwnFieldOnly, this.IsUseFieldAlias, ShowFieldType, FieldTypes);
        }
        public FieldComboItem[] GetFieldItems(IStandaloneTable SATable, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// ���ް۰��
			return this.GetFieldItems(SATable as ITableFields, VisibleOnly, EditableOnly, OwnFieldOnly, ShowFieldType, FieldTypes);
        }
        public FieldComboItem[] GetFieldItems(IFeatureLayer FLayer, bool VisibleOnly, bool EditableOnly, bool OwnFieldOnly, bool ShowFieldType, params esriFieldType[] FieldTypes) {
			// ���ް۰��
			return this.GetFieldItems(FLayer as ITableFields, VisibleOnly, EditableOnly, OwnFieldOnly, ShowFieldType, FieldTypes);
        }

        // ToolBar ���۰�
        private IToolbarControl2 m_ToolbarControl = null;	// axToolbarControl1 (Map)
        internal IToolbarControl2 m_ToolbarControl2 = null; // axToolbarControl2 (Edit)
        //private IToolbarControl2 m_ToolbarControl3 = null;
        private IToolbarControl2 m_ToolbarControl4 = null;	// axToolbarControl4 (PageLayout)
		
		private IToolbarMenu m_ToolbarMenuEditStarted = null;
        private IToolbarMenu m_ToolbarMenu = null;
        //private IToolbarMenu m_ToolbarMenu2 = null;
        private IToolbarMenu m_ToolbarMenu3 = null;			// �߰�ޥڲ��� �E�د���ƭ�
        //private IToolbarMenu m_ToolbarMenu4 = null;			// 

        private IEngineEditor m_EngineEditor = null; //new EngineEditorClass();
        private IEngineEditSketch m_EngineEditSketch;

        private IEngineEditEvents_Event m_EngineEditEvents = null;

#if DEBUG
		// �c�[���o�[ �C�x���g�E�L���v�`��
        private IToolbarControlEvents_Event	_agToolbarCtlEvents = null;
        private void ToolOnItemClickEvent(int Index) {
			if(Index >= 0) {
				Debug.WriteLine("��TOOL CLICK EVENT : ToolID = " + Index.ToString());
                // 2019.2.21 �e�X�g�p
                if (m_ToolbarControl != null) {
                    
                    IToolbarItem clickItem =  m_ToolbarControl.GetItem(Index);
                    if (clickItem != null) {
                        //IntPtr child = FindWindowEx(m_ToolbarControl.hWnd, IntPtr.Zero, "", "");
                    }
                }
			}
        }
#endif

        //
        //internal void SavePageLayout(IPageLayout pagelayout)
        //{
        //    //Get IObjectCopy interface
        //    IObjectCopy objectCopy = new ObjectCopyClass();

        //    //Get IUnknown interface (map to copy)
        //    object toCopyLayout = pagelayout; //axPageLayoutControl1.PageLayout;
        //    savePageLayout = toCopyLayout as PageLayout;

        //}
        private ESRIJapan.GISLight10.dispatcher.Dispatcher dispatcher = null;

        #endregion


        #region Constructor
        /// <summary>
        /// ���C���t�H�[���̃R���X�g���N�^
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // ۶ް�̐ݒ� (�ް���ިڸ�؂̐ݒ�) ��Process���璼�ڋN�������ꍇ�A����ިڸ�ؕs���ׁ̈A�����߽�������ɂȂ��Ă��܂�
            Common.Logger.BaseDirectory = Application.StartupPath;
        }
        #endregion

        /// <summary>
        /// �X�e�[�^�X���x���ݒ�
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="status"></param>
        private void SetStatus(bool visible, string status)
        {
            this.label1.Visible = visible;
            this.progressBar1.Visible = visible;

            if (visible)
            {
                this.label1.Text = status;
                this.Enabled = false;
            }
            else
            {
                this.label1.Text = "";
                this.Enabled = true;
            }
        }

        /// <summary>
        /// TOC���C����ԂŎg�p�s�ݒ�
        /// </summary>
        /// <param name="enble"></param>
        private void SetToolStripMenuEnable(bool enble)
        {
            //menuHyperLinkKensaku.Enabled = enble;
            menuSentakuZokukeiKensaku.Enabled = enble;
            menuSentakuKukanKensaku.Enabled = enble;
            menuSentakuZokuseichiSyukei.Enabled = enble;
            menuSentakuSelectableLayerSettings.Enabled = enble; // 2010-11-10(wed) add

            menuExportMap.Enabled = enble;
            menuTableJoin.Enabled = enble;
            menuRemoveJoin.Enabled = enble;
            menuRelate.Enabled = enble;
            menuRemoveRelate.Enabled = enble;

            // �W�I���g�����Z�̎g�p�s��
            if(enble == true) {
                int count = 0;
                if(!this.IsEditMode) {
                    Common.LayerManager pLayerManager = new GISLight10.Common.LayerManager();
                    List<IFeatureLayer> featureLayerList = pLayerManager.GetFeatureLayers(m_MapControl.Map);

                    foreach(IFeatureLayer fLay in featureLayerList) {
                        if (fLay.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon ||
                            fLay.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                        {
                            count++;
                        }
                    }
                }

                // �ҏW�J�n�ł͂Ȃ��A�|���S���A�|�����C�����P�ȏ゠��ꍇtrue
                menuGeometryCalculate.Enabled = count > 0 ? true : false;
                menuFieldCalculate.Enabled = count > 0 ? true : false;
                menuIntersect.Enabled = count > 0 ? true : false;
            }
            else {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
            }

        }

        /// <summary>
        /// �}�b�v�R���g���[���C�x���g�n���h��
        /// </summary>
        internal void SetEvents()
        {
            // It is important to perfom the Explict Cast or else the wiring up of the Events with the
            // AddHandler will not function properly. Also the if the Explict Cast is not performed
            // when the RemoveHandler's are called a diconnected RCW error will result.
            m_map = (IMap)axMapControl1.ActiveView.FocusMap; // Explict Cast

            //Create an instance of the delegate, add it to AfterDraw event
            //m_ActiveViewEventsAfterDraw =
            //    new IActiveViewEvents_AfterDrawEventHandler(OnActiveViewEventsAfterDraw);
            //((IActiveViewEvents_Event)(m_map)).AfterDraw += m_ActiveViewEventsAfterDraw;

            //Create an instance of the delegate, add it to AfterItemDraw event
            m_ActiveViewEventsAfterItemDraw =
                new IActiveViewEvents_AfterItemDrawEventHandler(OnActiveViewEventsItemDraw);
            ((IActiveViewEvents_Event)(m_map)).AfterItemDraw += m_ActiveViewEventsAfterItemDraw;

            //Create an instance of the delegate, add it to ContentsChanged event
            m_ActiveViewEventsContentsChanged =
                new IActiveViewEvents_ContentsChangedEventHandler(OnActiveViewEventsContentsChanged);
            ((IActiveViewEvents_Event)(m_map)).ContentsChanged += m_ActiveViewEventsContentsChanged;

            //Create an instance of the delegate, add it to ContentsCleared event
            m_ActiveViewEventsContentsCleared =
                new IActiveViewEvents_ContentsClearedEventHandler(OnActiveViewEventsContentsCleared);
            ((IActiveViewEvents_Event)(m_map)).ContentsCleared += m_ActiveViewEventsContentsCleared;

            //Create an instance of the delegate, add it to FocusMapChanged event
            m_ActiveViewEventsFocusMapChanged =
                new IActiveViewEvents_FocusMapChangedEventHandler(OnActiveViewEventsFocusMapChanged);
            ((IActiveViewEvents_Event)(m_map)).FocusMapChanged += m_ActiveViewEventsFocusMapChanged;

            //Create an instance of the delegate, add it to ItemAdded event
            m_ActiveViewEventsItemAdded =
                new IActiveViewEvents_ItemAddedEventHandler(OnActiveViewEventsItemAdded);
            ((IActiveViewEvents_Event)(m_map)).ItemAdded += m_ActiveViewEventsItemAdded;

            //Create an instance of the delegate, add it to ItemDeleted event
            m_ActiveViewEventsItemDeleted =
                new IActiveViewEvents_ItemDeletedEventHandler(OnActiveViewEventsItemDeleted);
            ((IActiveViewEvents_Event)(m_map)).ItemDeleted += m_ActiveViewEventsItemDeleted;

            //Create an instance of the delegate, add it to ItemReordered event
            m_ActiveViewEventsItemReordered =
                new IActiveViewEvents_ItemReorderedEventHandler(OnActiveViewEventsItemReordered);
            ((IActiveViewEvents_Event)(m_map)).ItemReordered += m_ActiveViewEventsItemReordered;

            //Create an instance of the delegate, add it to SelectionChanged event
            m_ActiveViewEventsSelectionChanged =
                new IActiveViewEvents_SelectionChangedEventHandler(OnActiveViewEventsSelectionChanged);
            ((IActiveViewEvents_Event)(m_map)).SelectionChanged += m_ActiveViewEventsSelectionChanged;

            //Create an instance of the delegate, add it to SpatialReferenceChanged event
            m_ActiveViewEventsSpatialReferenceChanged =
                new IActiveViewEvents_SpatialReferenceChangedEventHandler(
                    OnActiveViewEventsSpatialReferenceChanged);
            ((IActiveViewEvents_Event)(m_map)).SpatialReferenceChanged +=
                m_ActiveViewEventsSpatialReferenceChanged;

            //Create an instance of the delegate, add it to ViewRefreshed event
            //m_ActiveViewEventsViewRefreshed =
            //    new IActiveViewEvents_ViewRefreshedEventHandler(OnActiveViewEventsViewRefreshed);
            //((IActiveViewEvents_Event)(m_map)).ViewRefreshed += m_ActiveViewEventsViewRefreshed;

        }

        /// <summary>
        /// �y�[�W���C�A�E�g�C�x���g�n���h���ǉ�
        /// </summary>
        internal void SetPageLayoutEvents()
        {
            m_PageLayoutControlEventsOnViewRefreshed =
                new IPageLayoutControlEvents_OnViewRefreshedEventHandler(
                    OnPageLayoutControlEventsOnViewRefreshed);
            ((IPageLayoutControlEvents_Event)(m_pageLayoutControl)).OnViewRefreshed +=
                m_PageLayoutControlEventsOnViewRefreshed;

            m_PageLayoutControlEventsOnFocusMapChanged =
                new IPageLayoutControlEvents_OnFocusMapChangedEventHandler(
                    OnPageLayoutControlEventsOnFocusMapChanged);
            ((IPageLayoutControlEvents_Event)(m_pageLayoutControl)).OnFocusMapChanged +=
                m_PageLayoutControlEventsOnFocusMapChanged;

            m_PageLayoutControlEventsOnExtentUpdated =
                new IPageLayoutControlEvents_OnExtentUpdatedEventHandler(
                    ONPageLayoutControlEventsOnExtentUpdated);
            ((IPageLayoutControlEvents_Event)(m_pageLayoutControl)).OnExtentUpdated +=
                m_PageLayoutControlEventsOnExtentUpdated;
        }

        /// <summary>
        /// �����ݒ�
        /// </summary>
        private void SetThisInit()
        {
            //this.Text = Properties.Resources.CommonMessage_ApplicationName;
            this.Title_Name = Properties.Resources.CommonMessage_ApplicationName;

			// ���ِݒ�
            this.Text = "���� - " + Properties.Resources.CommonMessage_ApplicationName;

            SetStatus(false, "");

            // Get the CommandsEnvironment singleton
            m_CommandsEnvironment = new CommandsEnvironmentClass();

            // get the MapControl
            //m_mapControl
            m_MapControl = (IMapControl4)axMapControl1.Object;
            //create a new Map
            IMap map = new MapClass();
            map.Name = "Map";
            m_MapControl.DocumentFilename = string.Empty;
            m_MapControl.Map = map;

            //Get IObjectCopy interface
            IObjectCopy objectCopy = new ObjectCopyClass();

            //Get IUnknown interface (map to copy)
            object toCopyLayout = axPageLayoutControl1.PageLayout;
            initPageLayout = toCopyLayout as PageLayout;
            //savePageLayout = toCopyLayout as PageLayout;
            //

            m_tocControl = (ITOCControl2)axTOCControl1.Object;

            originalMaps = new Maps();

            ////////
            // map pagelayout syncro init
            ////////
            m_pageLayoutControl = (IPageLayoutControl3)axPageLayoutControl1.Object;
            m_EngineEditor = new EngineEditorClass();

            // initialize the controls synchronization calss
            m_controlsSynchronizer = new Common.ControlsSynchronizer(m_MapControl, m_pageLayoutControl);

            // add the framework controls (TOC and Toolbars) in order to synchronize then when the
            // active control changes (call SetBuddyControl)
            m_controlsSynchronizer.AddFrameworkControl(axTOCControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl2.Object);
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl5.Object);
            //m_controlsSynchronizer.AddFrameworkControl(axMapControl1.Object);
            /////////

            // disable the Save menu (since there is no document yet)
            menuSaveDoc.Enabled = false;

            // �V�K�쐬
            axToolbarControl1.AddItem(new EngineCommand.CreateNewDocument(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            // �J��
            axToolbarControl1.AddItem(new EngineCommand.OpenDocument(), 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            // �㏑���ۑ�
            axToolbarControl1.AddItem(new EngineCommand.SaveDocument(), 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);

            axToolbarControl1.AddItem("esriControls.ControlsAddDataCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomToolControl", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomInTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomOutTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomInFixedCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomOutFixedCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapPanTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapFullExtentCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomToLastExtentBackCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapZoomToLastExtentForwardCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapRotateTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);            
            axToolbarControl1.AddItem("esriControls.ControlsMapClearMapRotationCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);            

            axToolbarControl1.AddItem("esriControls.ControlsSelectFeaturesTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsZoomToSelectedCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsClearSelectionCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsSelectTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapIdentifyTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);

            axToolbarControl1.AddItem("esriControls.ControlsMapFindCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapMeasureTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapHyperlinkTool", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapGoToCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsMapSwipeTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsLayerTransparencyCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl1.AddItem("esriControls.ControlsLayerListToolControl", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
			// ���m�F�p�ɒǉ�
			//axToolbarControl1.AddItem("esriControls.ControlsUndoCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            //axToolbarControl1.AddItem("esriControls.ControlsRedoCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
 
            //axToolbarControl1.AddItem("esriControls.ControlsOpenDocCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            //axToolbarControl1.AddItem("esriControls.ControlsSaveAsDocCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);

            #region �G�f�B�b�g�E�E�N���b�N���j���[
            // �ҏW�J�n��̒��_�}���A���_�폜�A���_�ړ�
            m_ToolbarMenuEditStarted = new ToolbarMenu();
            m_ToolbarMenuEditStarted.AddItem("esriControls.ControlsEditingSketchContextMenu", 0, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_ToolbarMenuEditStarted.AddItem("esriControls.ControlsEditingVertexContextMenu", 0, 0, true, esriCommandStyles.esriCommandStyleTextOnly);
            #endregion

            // map control context menu
            // �r���[�X�V 2010-12-27 add
            //m_ToolbarMenu.AddItem("esriControls.ControlsMapRefreshViewCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            #region ToolbarMenu
            m_ToolbarMenu = new ToolbarMenu();
            // �I������
            m_ToolbarMenu.AddItem("esriControls.ControlsClearSelectionCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �I���t�B�[�`���ɃY�[��
            m_ToolbarMenu.AddItem("esriControls.ControlsZoomToSelectedCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �ʑ����\��
            m_ToolbarMenu.AddItem("esriControls.ControlsMapIdentifyTool", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �t�B�[�`���I��
            m_ToolbarMenu.AddItem("esriControls.ControlsSelectFeaturesTool", 0, 0, true, esriCommandStyles.esriCommandStyleIconAndText);
            // �藦�k��
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomOutFixedCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �藦�g��
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomInFixedCommand", 0, 0, true, esriCommandStyles.esriCommandStyleIconAndText);
            // ���̕\���͈͂ɐi��
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomToLastExtentForwardCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �O�̕\���͈͂ɖ߂�
            m_ToolbarMenu.AddItem("esriControls.ControlsMapZoomToLastExtentBackCommand", 0, 0, true, esriCommandStyles.esriCommandStyleIconAndText);
            // �y�[�W�S�̂�\��
            //m_ToolbarMenu.AddItem("esriControls.ControlsPageZoomWholePageCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �S�̕\��
            m_ToolbarMenu.AddItem("esriControls.ControlsMapFullExtentCommand", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            #endregion

            #region Share the ToolbarControl command pool with the ToolbarMenu command pool
            m_ToolbarControl = (IToolbarControl2)axToolbarControl1.Object;
            m_ToolbarMenu.CommandPool = m_ToolbarControl.CommandPool;

            m_ToolbarMenuEditStarted.CommandPool = m_ToolbarControl.CommandPool;
#if DEBUG
			// EDIT CONTEXT MENU EVENT CAPTURE (�ł��Ȃ�)
            this._agToolbarCtlEvents = (IToolbarControlEvents_Event)m_ToolbarMenuEditStarted.Hook;
            this._agToolbarCtlEvents.OnItemClick += new IToolbarControlEvents_OnItemClickEventHandler(ToolOnItemClickEvent);
#endif

            axToolbarControl2.AddItem("esriControls.ControlsEditingEditorMenu", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingEditTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
			axToolbarControl2.AddItem("esriControls.ControlsEditingSketchTool", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingAttributeCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
			axToolbarControl2.AddItem("esriControls.ControlsEditingSketchPropertiesCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsUndoCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsRedoCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingCutCommand", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingCopyCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingPasteCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingClearCommand", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingTaskToolControl", 0, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            axToolbarControl2.AddItem("esriControls.ControlsEditingTargetToolControl", 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
			//axToolbarControl2.AddItem(new EngineCommand.GeoReferenceCommand(), 0, -1, true, 11, esriCommandStyles.esriCommandStyleIconOnly);
			//IOperationStack	agOpStack = new ControlsOperationStackClass();
			//this.axToolbarControl2.OperationStack = agOpStack;

            //
            m_ToolbarControl2 = (IToolbarControl2)axToolbarControl2.Object;
            //m_ToolbarMenu2 = new ToolbarMenu();
            //m_ToolbarMenu2.CommandPool = m_ToolbarControl2.CommandPool;

			// �ҏW°٥�ް�̕��𒲐�
			this.FitToolBarWidth(m_ToolbarControl2);

            // Undo / Redo�������Ȃ��Ȃ����ׁA�޲��ނ����ݸނ�ύX(10.2.1-)
			// bind the controls together (both point at the same map) and set the MapControl as the active control
            //m_controlsSynchronizer.BindControls(true);
			m_controlsSynchronizer.BindControls(m_MapControl, m_pageLayoutControl, true);

            m_ToolbarControl4 = (IToolbarControl2)axToolbarControl4.Object;
            //m_ToolbarMenu4 = new ToolbarMenu();
            //m_ToolbarMenu4.CommandPool = m_ToolbarControl4.CommandPool;

            //
            #region �y�[�W���C�A�E�g�E�E�N���b�N���j���[
            m_ToolbarMenu3 = new ToolbarMenu();
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapViewMenu", 0, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // ��ʈړ� page
            m_ToolbarMenu3.AddItem("esriControls.ControlsPagePanTool", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // ��ʈړ� map
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapPanTool", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �f�[�^�t���[���̉�]
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapRotateTool", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // ��]�̉���
            m_ToolbarMenu3.AddItem("esriControls.ControlsMapClearMapRotationCommand", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �t�B�[�`���I��
            m_ToolbarMenu3.AddItem("esriControls.ControlsSelectFeaturesTool", 0, m_ToolbarMenu3.Count, true, esriCommandStyles.esriCommandStyleIconAndText);
            // �I���t�B�[�`���ɃY�[��
            m_ToolbarMenu3.AddItem("esriControls.ControlsZoomToSelectedCommand", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �I������
            m_ToolbarMenu3.AddItem("esriControls.ControlsClearSelectionCommand", 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu3.AddItem(new EngineCommand.EditTextElementCommand(), 0, m_ToolbarMenu3.Count, true, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu3.AddItem(new EngineCommand.EditScaleBarElementPropCommand(), 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �}��̕ҏW
            m_ToolbarMenu3.AddItem(new EngineCommand.EditLegendCommand(), 0, m_ToolbarMenu3.Count, false, esriCommandStyles.esriCommandStyleIconAndText);

            //m_ToolbarControl3 = (IToolbarControl2)axToolbarControl1.Object;
            m_ToolbarMenu3.CommandPool = m_ToolbarControl.CommandPool;
            #endregion

			// �����°ق�I�� (����đI��°�)
			//this.SetElementSelectTool();

            #endregion

            #region TOC���j���[
            m_TocControl = (ITOCControl2)axTOCControl1.Object;

            m_TocMenu = new ToolbarMenuClass();

            //���ׂẴ��C����\��
            m_TocMenu.AddItem(
                new EngineCommand.LayerVisibility(),
                1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //���ׂẴ��C�����\��
            m_TocMenu.AddItem(
                new EngineCommand.LayerVisibility(),
                2, 1, false, esriCommandStyles.esriCommandStyleTextOnly);

            //�O���[�v���C���̒ǉ�
            m_TocMenu.AddItem(
                new EngineCommand.AddGroupLayerCommand(),
                3, 2, false, esriCommandStyles.esriCommandStyleIconAndText);

            //�x�[�X�}�b�v���C���̒ǉ�
            m_TocMenu.AddItem(
                new EngineCommand.AddBaseMapLayerCommand(),
                4, 3, false, esriCommandStyles.esriCommandStyleIconAndText);

            //��������
            m_TocMenu.AddItem(
                new EngineCommand.AttributeSearchCommand(),
                5, 4, true, esriCommandStyles.esriCommandStyleIconAndText);

            //��Ԍ���
            m_TocMenu.AddItem(
                new EngineCommand.SpatialSearchCommand(),
                6, 5, false, esriCommandStyles.esriCommandStyleIconAndText);

            //�����l�W�v
            m_TocMenu.AddItem(
                new EngineCommand.AttributeSumCommand(),
                7, 6, false, esriCommandStyles.esriCommandStyleTextOnly);

            //�e�[�u������
            m_TocMenu.AddItem(
                new EngineCommand.JoinTableCommand(),
                8, 7, true, esriCommandStyles.esriCommandStyleIconAndText);

            //�e�[�u�������̉���
            m_TocMenu.AddItem(
                new EngineCommand.RemoveJoinCommand(),
                9, 8, false, esriCommandStyles.esriCommandStyleTextOnly);

            //�����[�g
            m_TocMenu.AddItem(
                new EngineCommand.RelateCommand(),
                10, 9, false, esriCommandStyles.esriCommandStyleIconAndText);

            //�����[�g�̉���
            m_TocMenu.AddItem(
                new EngineCommand.RemoveRelateCommand(),
                11, 10, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// XY�f�[�^�̒ǉ�
            //m_TocMenu.AddItem(
            //    new EngineCommand.AddXYDataCommand(),
            //    13, 12, true, esriCommandStyles.esriCommandStyleIconAndText);

            // �C���^�[�Z�N�g
            m_TocMenu.AddItem(
                new EngineCommand.IntersectCommand(),
                12, 11, true, esriCommandStyles.esriCommandStyleIconAndText);

            //�I���\���C���̐ݒ�
            m_TocMenu.AddItem(
                new EngineCommand.SelectableLayerSettingsCommand(),
                13, 12, true, esriCommandStyles.esriCommandStyleTextOnly);

            // 2012/08/06 ADD
            // ���W�n�̐ݒ�
            m_TocMenu.AddItem(
                new EngineCommand.SetDataFrameProjectionCommand(),
                14, 13, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 ADD

            // 2012/08/06 DEL
            // �W�I���t�@�����X
            //m_TocMenu.AddItem(
            //    new EngineCommand.GeoReferenceCommand(),
            //    15, 14, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 DEL
            #endregion

            #region TOC���C�����j���[
            m_TocLayerMenu = new ToolbarMenuClass();

            // 2012/08/06 ADD 
            //// ���C���ۑ�
            m_TocLayerMenu.AddItem(new EngineCommand.SaveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // �t�B�[�`���N���X�փG�N�X�|�[�g
            m_TocLayerMenu.AddItem(new EngineCommand.ExportShapeFile(), -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            // 2012/08/14 chg >>>>>
            //// �t�B�[���h���Z
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.FieldCalculatorCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // �t�B�[���h�̒ǉ��ƍ폜
            m_TocLayerMenu.AddItem(new EngineCommand.AddAndDeleteFieldCommand(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // �t�B�[���h���Z
            m_TocLayerMenu.AddItem(new EngineCommand.LayerFieldCalculatorCommand(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 2012/08/14  chg <<<<<

            // �����E�ʐόv�Z
            m_TocLayerMenu.AddItem(new EngineCommand.LayerGeometryCalculatorCommand(), -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            // 2012/08/06 ADD 

            // 2012/08/06 DEL 
            //// �k�ڔ͈�
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.ScaleRangeCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// ���C���ۑ�
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SaveLayer(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 DEL 

            // �f�[�^�\�[�X�ݒ�
            m_TocLayerMenu.AddItem(
                new EngineCommand.OpenDataSource(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // 2012/08/06 ADD 
            // �n�C�p�[�����N�̐ݒ�
            m_TocLayerMenu.AddItem(
                new EngineCommand.HyperLinkSettingCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // ���x���V���{��
            m_TocLayerMenu.AddItem(
                new EngineCommand.LabelSettingCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // �V���{���ݒ�
            m_TocLayerMenu.AddItem(
                new EngineCommand.SymbolSettingsCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            // �k�ڔ͈�
            m_TocLayerMenu.AddItem(
                new EngineCommand.ScaleRangeCommand(),
                -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            // �����e�[�u���\��
            m_TocLayerMenu.AddItem(
                new EngineCommand.OpenAttributeTable(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            // �t�B�[���h�̕ʖ���`
            m_TocLayerMenu.AddItem(
                new EngineCommand.DefineFieldNameAliasCommand(),
                -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            // ���C���̍폜
            m_TocLayerMenu.AddItem(
                new EngineCommand.RemoveLayer(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            // �O���[�v���C������
            m_TocLayerMenu.AddItem(
                new EngineCommand.ShowTOCCommand(),
                -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // ���j���[��ԏ�
            // ���C���̑S�̕\��
            m_TocLayerMenu.AddItem(
                new EngineCommand.ZoomToLayer(),
                -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            // 2012/08/06 ADD 

            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SetLayerProperty(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // 2012/08/06 DEL 
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.ZoomToLayer(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            ////m_TocLayerMenu.AddItem(
            ////    "esriControls.ControlsZoomToSelectedCommand",
            ////    -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            ////
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SelectionFeatureZoomCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.RemoveLayer(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            //// export shape file
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.ExportShapeFile(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            //// �n�C�p�[�����N�̐ݒ�
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.HyperLinkSettingCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// ���x���V���{��
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.LabelSettingCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// �V���{���ݒ�
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.SymbolSettingsCommand(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleIconAndText);

            //// �t�B�[���h���Z
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.FieldCalculatorCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            //// �����E�ʐόv�Z
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.LayerGeometryCalculatorCommand(),
            //    -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);

            //// �����e�[�u���\��
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.OpenAttributeTable(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            //// �t�B�[���h�̕ʖ���`
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.DefineFieldNameAliasCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);

            // ���j���[��ԏ�
            //// �t�B�[���h�̒ǉ��ƍ폜
            //m_TocLayerMenu.AddItem(
            //    new EngineCommand.AddAndDeleteFieldCommand(),
            //    -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            // 2012/08/06 DEL 
            #endregion

            // ���j���[�o�[�ɒǉ�
            //m_ToolbarControl.AddItem(
            //    m_TocLayerMenu, 0, m_ToolbarControl.Count,
            //    false, 0, esriCommandStyles.esriCommandStyleIconAndText);

            //m_ToolBarMenuCustom = new ToolbarMenuClass();
            //m_ToolBarMenuCustom.Caption = "�v���O�C��";
            //m_ToolbarControl.AddItem(
            //    m_ToolBarMenuCustom, 0, m_ToolbarControl.Count,
            //    false, 0, esriCommandStyles.esriCommandStyleIconAndText);

            // 2012/08/07  
            // �W�I���t�@�����X
            this.axToolbarControl5.AddItem(new EngineCommand.GeoReferenceCommand(), 0, 0, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
			// 2012/08/07  

            // ̨�����ҏW���j���[�ɒǉ�
            IToolbarItem titm = m_ToolbarControl2.GetItem(0);
            titm.Menu.AddItem(new EngineCommand.MergeCommand(), 0, titm.Menu.Count, true, esriCommandStyles.esriCommandStyleTextOnly);
            titm.Menu.AddItem(new EngineCommand.SplitAndMergeSettingCommand(), 0, titm.Menu.Count, false, esriCommandStyles.esriCommandStyleTextOnly);
            // "esriControls.ControlsEditingSnappingCommand"
            titm.Menu.AddItem(new EngineCommand.EditingSnappingCommand(), 0, titm.Menu.Count, true, esriCommandStyles.esriCommandStyleTextOnly);
            titm.Menu.AddItem(new EngineCommand.EditOptionCommand(), 0, titm.Menu.Count, false, esriCommandStyles.esriCommandStyleTextOnly);

            //m_ToolbarControl3.AddItem(
            //    new EngineCommand.PrintActiveViewCommand(),
            //    -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);

            // PageLayoutTab �G�������g�}��
            IToolbarItem titm4 = axToolbarControl4.GetItem(0);
            titm4.Menu.AddItem(new EngineCommand.CreateLegend(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.InsertPictureCommand(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.CreateScaleBar(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.CreateNorthArrow(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            titm4.Menu.AddItem(new EngineCommand.CreateTextElement(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);

            #region PageLayout��°٥�ް�Ɉ���p����ނ�ǉ�
            m_ToolbarControl4.AddItem(
                new EngineCommand.PageSetUpCommand(),
                -1, m_ToolbarControl4.Count, true, -1, esriCommandStyles.esriCommandStyleIconAndText);

            m_ToolbarControl4.AddItem(
                new EngineCommand.PrinitPreviewCommand(),
                -1, m_ToolbarControl4.Count, false, -1, esriCommandStyles.esriCommandStyleIconAndText);

            m_ToolbarControl4.AddItem(
                new EngineCommand.PrinitPageLayoutCommand(),
                -1, m_ToolbarControl4.Count, false, -1, esriCommandStyles.esriCommandStyleIconAndText);

            m_ToolbarControl4.AddItem(
                new EngineCommand.ResetPageLayoutCommand(),
                -1, m_ToolbarControl4.Count, false, -1, esriCommandStyles.esriCommandStyleIconAndText);
            #endregion



            // hook
            //m_ToolbarMenu.SetHook(axToolbarControl1); //m_MapControl);
            //m_ToolbarMenuEditStarted.SetHook(axToolbarControl1);
            m_ToolbarMenu.SetHook(m_ToolbarControl);
            //m_ToolbarMenu2.SetHook(m_ToolbarControl2);
            //m_ToolbarMenu4.SetHook(m_ToolbarControl4);
            m_ToolbarMenu3.SetHook(m_ToolbarControl);

            m_TocMenu.SetHook(m_MapControl);
            m_TocLayerMenu.SetHook(m_MapControl);

            // ���s�R�}���h�N���X�̊���U�莞�Ɏg�p
            this.dispatcher = new GISLight10.dispatcher.Dispatcher();

            // Set the Engine Edit Sketch
            m_EngineEditor = new EngineEditorClass();
            m_EngineEditSketch = (IEngineEditSketch)m_EngineEditor;

            // TOC���C�����ݒ莞
            SetToolStripMenuEnable(false);

            // add edittask
            SplitPolylineEditTask splitPolylineEditTask = new SplitPolylineEditTask();
            m_EngineEditor.AddTask((IEngineEditTask)splitPolylineEditTask);
            SplitPolygonEditTask splitPolygonEditTask = new SplitPolygonEditTask();
            m_EngineEditor.AddTask((IEngineEditTask)splitPolygonEditTask);
        }

        /// <summary>
        /// �}�b�v�ύX��ԊĎ�
        /// </summary>
        public void SetMapStateChanged()
        {
            //if (map_sts_changed_cmt > 0)
            map_sts_changed = true;
            map_sts_changed_cmt++;
        }

        /// <summary>
        /// �}�b�v�ύX��ԃN���A
        /// </summary>
        public void ClearMapChangeCount()
        {
            this.map_sts_changed_cmt = 0;
            this.map_sts_changed = false;
        }

        /// <summary>
        /// �c�[�����j���[�g�p�\�s�؂�ւ�
        /// </summary>
        private void SetMenuItemEnabled()
        {
#if DEBUG
			Debug.WriteLine(string.Format("��SetMenuItemEnabled ���s {0:HH:mm:ss:ffff}", DateTime.Now));
#endif



            if(m_MapControl.LayerCount == 0) {
                // ���C�������݂��Ȃ����
                // ���C�����Q�Ƃ��郁�j���[�̃A�C�e�����g�p�s��
                SetToolStripMenuEnable(false);
                return;
            }

            Common.LayerManager pLayerManager = new GISLight10.Common.LayerManager();

            List<IFeatureLayer> featureLayerList = pLayerManager.GetFeatureLayers(m_MapControl.Map);

            if(featureLayerList.Count == 0) {
                // �L���ȃ��C�������݂��Ȃ����
                // ���C�����Q�Ƃ��郁�j���[�̃A�C�e�����g�p�s��
                SetToolStripMenuEnable(false);
            }
            else {
                // �L���ȃ��C�������݂����
                // ���C�����Q�Ƃ��郁�j���[�̃A�C�e�����g�p�\��
                SetToolStripMenuEnable(true);
            }

            // �����e�[�u���\�����Ă��鎞�͕ҏW���j���[�s��
            if(HasFormAttributeTable()) {
                m_ToolbarControl2.Enabled = false;
            }
			// �޵��̧�ݽ���͕s��
			else if(this.HasGeoReference()) {
                m_ToolbarControl2.Enabled = false;
			}
            else {
				m_ToolbarControl2.Enabled = this.IsMapVisible;
            }

        }

        /// <summary>
        /// �e�L�X�g�C��
        /// </summary>
        private void EditTextElement()
        {
            IPageLayout pl = axPageLayoutControl1.PageLayout;
            IGraphicsContainerSelect graphicsSelect = pl as IGraphicsContainerSelect;

            if (graphicsSelect.ElementSelectionCount > 0)
            {
                IEnumElement ienum = graphicsSelect.SelectedElements;
                ienum.Reset();
                IElement selectedElement = ienum.Next();
                if (selectedElement != null)
                {
                    if (selectedElement is ITextElement)
                    {
                        ITextElement tel = selectedElement as ITextElement;
                        IGraphicsContainer graphicsContainer =
                            this.axPageLayoutControl1.PageLayout as IGraphicsContainer;

                        Ui.FormText frm = new Ui.FormText(
                            this.axPageLayoutControl1.Object as IPageLayoutControl3,
                            tel, graphicsContainer);

                        frm.ShowDialog(this);
                    }
                }
            }
        }

        /// <summary>
        /// �X�P�[���o�[�C��
        /// </summary>
        private void SetEditEnableScaleBar()
        {
            IPageLayout pl = axPageLayoutControl1.PageLayout;
            IGraphicsContainerSelect graphicsSelect = pl as IGraphicsContainerSelect;
            if (graphicsSelect.ElementSelectionCount > 0)
            {
                IEnumElement ienum = graphicsSelect.SelectedElements;
                ienum.Reset();
                IElement selectedElement = ienum.Next();
                if (selectedElement != null)
                {
                    IElement targetelm =
                        m_pageLayoutControl.FindElementByName(SCALE_BAR_ELEMENT_NAME, 1);

                    if (selectedElement == targetelm)
                    {
                        IMapFrame mapFrame =
                            (IMapFrame)m_pageLayoutControl.ActiveView.GraphicsContainer.FindFrame(
                            m_pageLayoutControl.ActiveView.FocusMap);

                        FormScaleBarSettings frm =
                            new FormScaleBarSettings(mapFrame, m_pageLayoutControl, targetelm);

                        frm.ShowDialog(this);
                    }

                }
            }
        }

        /// <summary>
        /// �}�b�v��ԕύX�m�F�F
        /// �}�b�v�ł̃}�E�X����O�Ƀc�[���o�[�ŃN���b�N���ꂽ�R�}���h�m�F
        /// </summary>
        private void SetMapStatusChanged()
        {
            if (CurrentToolIndex != OpenDox_Index &&
                CurrentToolIndex != SaveDoc_Index &&
                CurrentToolIndex != Addlayer_Index &&
                CurrentToolIndex != KobetsuZokusei_Index &&
                CurrentToolIndex != Kensaku_Index &&
                CurrentToolIndex != XY_Idou_Index &&
                CurrentToolIndex != Keisoku_Index &&
                CurrentToolIndex != HyperLink_Index &&
                CurrentToolIndex != Swipe_Index)
            {
                map_sts_changed = true;
            }
            //else
            //{
            //    map_sts_changed = false;
            //}
        }

        /// <summary>
        /// �}�b�v�R���g���[�����Z�b�g
        /// </summary>
        private void ClearMap()
        {
            IMap map = new MapClass();
            map.Name = "Map";
            m_controlsSynchronizer.MapControl.DocumentFilename = string.Empty;

            //replace the shared map with the new Map instance
            this.originalMaps.Reset();
            originalMaps.Add(map);
            m_controlsSynchronizer.PageLayoutControl.PageLayout.ReplaceMaps(originalMaps);
            m_controlsSynchronizer.ReplaceMap(map);

            //this.Text = Properties.Resources.CommonMessage_ApplicationName;
            this.Text = "���� - " +
                Properties.Resources.CommonMessage_ApplicationName;

        }
        /// <summary>
        /// �y�[�W���C�A�E�g���Z�b�g
        /// </summary>
        internal void ClearPageLayout()
        {
            IMapFrame mapFrame =
                (IMapFrame)this.axPageLayoutControl1.ActiveView.GraphicsContainer.FindFrame(
                this.axPageLayoutControl1.ActiveView.FocusMap);

            if (mapFrame != null)
            {
                mapFrame.Container.DeleteAllElements();
            }

            this.axPageLayoutControl1.PageLayout = initPageLayout; // new PageLayoutClass();
            this.axPageLayoutControl1.ActiveView.Extent = this.axPageLayoutControl1.ActiveView.FullExtent;
        }

        #region FormLoad
        private void MainForm_Load(object sender, EventArgs e) {
            try {
                Common.Logger.Info(Properties.Resources.CommonMessage_ApplicationName + "�J�n");

                // ���O�C�����[�U�ʐݒ�t�@�C�����݃`�F�b�N
                if(!Common.ApplicationInitializer.IsUserSettingsExists()) {
                    // ���݂��Ȃ���΍쐬
                    // 2020.03.17 �R�����g�A�E�g
                    Common.ApplicationInitializer.CreateUserSettings();
                }

                // 2020/03/17 : Workaround for controls display issues in 96 versus 120 dpi
                // https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#Controls92DPIWorkaround.htm
                AdjustBounds(this.axToolbarControl1);
                AdjustBounds(this.axToolbarControl2);
                AdjustBounds(this.axToolbarControl4);
                AdjustBounds(this.axToolbarControl5);
                AdjustBounds(this.axLicenseControl1);
                AdjustBounds(this.axMapControl1);
                AdjustBounds(this.axTOCControl1);
                AdjustBounds(this.axPageLayoutControl1);

                // �Ƽ�ُ������J�n
                SetThisInit();

                //2011/08/05 ArcGIS10���ŉ��L�����s����Ɖ�ʕ����̂ŃR�����g�A�E�g -->
                //�c�[���o�[�̓��ߐݒ�
                //Set label to not clip controls so ToolbarControl background is re-painted
                //int style = GetWindowLong(this.Handle, GWL_STYLE);
                //SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_CLIPCHILDREN);

                //Set the ToolbarControl to be transparent
                //axToolbarControl1.Transparent = true;
                //axToolbarControl2.Transparent = true;
                // <--

                //Set the ToolbarControl to be a child control of the label
                //axToolbarControl1.Parent = this;
                //axToolbarControl2.Parent = this;

				// �w�i�װ�����킹��
				{
					Color	colCtl = Color.FromName("Control");
					this.axToolbarControl1.BackColor = colCtl;
					this.axToolbarControl2.BackColor = colCtl;
					this.axToolbarControl4.BackColor = colCtl;
					this.axToolbarControl5.BackColor = colCtl;
				}

                // ���t���b�V���{�^���ǉ�
                ToolStripStatusLabel dummyLabel = new ToolStripStatusLabel();
                dummyLabel.Text = "";
                dummyLabel.Spring = true;
                statusStrip1.Items.Add(dummyLabel);


                Bitmap refreshButtonBitmap = (Bitmap)imageList1.Images[0];
                refreshButtonBitmap.MakeTransparent();

                ToolStripButton refreshButton
                    = new ToolStripButton("�}�b�v�̃��t���b�V��", refreshButtonBitmap,
                        new EventHandler(this.refreshButton_Click));

                refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                statusStrip1.Items.Add(refreshButton);

                statusStrip1.ShowItemToolTips = true;

                //�ҏW�J�n�C�x���g�n���h���̒ǉ�
                m_EngineEditEvents = (IEngineEditEvents_Event)m_EngineEditor;
                m_EngineEditEvents.OnStartEditing +=
                    new IEngineEditEvents_OnStartEditingEventHandler(OnStartEditingMethod);

                //m_EngineEditEvents.OnStopEditing += new IEngineEditEvents_OnStopEditingEventHandler(OnStopEditingMethod);

#if DEBUG
                // ��ި������Ă������ݸ�
				m_EngineEditEvents.OnSelectionChanged += new IEngineEditEvents_OnSelectionChangedEventHandler(OnEditorSelectionChanged);
#endif

                InitPrintSettings();
                SetEvents();
                SetPageLayoutEvents();

                // �N�����Ұ�������
				if(Program.StartArguments != null && !string.IsNullOrEmpty(Program.StartArguments[0])) {
					// ��1���� - MXḐ�٥�߽
					string	strMXD = Program.StartArguments[0];
					// �g���q���擾
					string	strExt = Path.GetExtension(strMXD).ToLower();
					// �߽�̗L�������m�F
					if(File.Exists(strMXD) && (strExt.Equals(".mxd") | strExt.Equals(".mxt") | strExt.Equals(".pmf"))) {
						// ϯ���޷���Ă��J��
						this.OpenMXDFile(strMXD);
					}
				}
				Common.Logger.Info(Properties.Resources.CommonMessage_ApplicationName + "�N��");
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_WhenLoad);

                this.Close();
            }
        }
        #endregion

        #region MenuEventHandlers

        /// <summary>
        /// ���ʃC�x���g�n���h��
        /// �R�}���h�N���b�N��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void common_menuItemCommand_Click(object sender, EventArgs e)
        {
            try
            {
                this.dispatcher.DoCommand(sender, m_MapControl.Object);
            }
            catch (Exception ex)
            {
                Common.Logger.Error(sender.ToString() + " " + ex.Message);
            }
        }

        /// <summary>
        /// Save Document command
        /// �㏑���ۑ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveDoc_Click(object sender, EventArgs e)
        {
            IMapDocument mapDoc = new MapDocumentClass();
            try
            {
                if (m_MapControl.CheckMxFile(axMapControl1.DocumentFilename))
                {
                    mapDoc.Open(axMapControl1.DocumentFilename, string.Empty);
                    if (mapDoc.get_IsReadOnly(axMapControl1.DocumentFilename))
                    {
                        mapDoc.Close();
                        Common.Logger.Info("���̃h�L�������g�͓Ǎ��ݐ�p�ł�");
                        Common.MessageBoxManager.ShowMessageBoxInfo(
                            this, "���̃h�L�������g�͓Ǎ��ݐ�p�ł�");
                        return;
                    }
                    m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                    m_MapControl.ActiveView.ShowScrollBars = true;

                    mapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                    //mapDoc.ReplaceContents((IMxdContents)m_MapControl.Map);

                    // 2010-12-27
                    mapDoc.SetActiveView(m_pageLayoutControl.ActiveView);
                    mapDoc.SetActiveView(m_MapControl.ActiveView);
                    //

                    mapDoc.Save(true, false);

                    mapDoc.Close();
                    ClearMapChangeCount();
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMenuSave);

            }
            finally
            {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mapDoc);
            }
        }

        /// <summary>
        /// exit the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExitApp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// listen to MapReplaced evant in order to update the statusbar and the Save menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMapReplaced(
            object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            try
            {
                //get the current document name from the MapControl
                m_mapDocumentName = m_MapControl.DocumentFilename;

                //if there is no MapDocument, diable the Save menu and clear the statusbar
                if (m_mapDocumentName == string.Empty)
                {
                    menuSaveDoc.Enabled = false;

                    // Toolbar�̏㏑���ۑ�������
                    if (m_ToolbarControl != null)
                    {
                        for (int i = 0; i < m_ToolbarControl.Count; i++)
                        {
                            IToolbarItem ti = m_ToolbarControl.GetItem(i);

                            if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                            {
                                EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                saveDocumet.DisableCommand();
                            }
                        }
                    }

                    statusBarXY.Text = string.Empty;
                }
                else
                {
                    //enable the Save manu and write the doc name to the statusbar
                    menuSaveDoc.Enabled = true;

                    // Toolbar�̏㏑���ۑ��L����
                    if (m_ToolbarControl != null)
                    {
                        for (int i = 0; i < m_ToolbarControl.Count; i++)
                        {
                            IToolbarItem ti = m_ToolbarControl.GetItem(i);

                            if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                            {
                                EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                saveDocumet.EnableCommand();
                            }
                        }
                    }

                    statusBarXY.Text = Path.GetFileName(m_mapDocumentName);
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenReplaceMap);

            }
        }

        /// <summary>
        /// �X�e�[�^�X �o�[�̍��W�\��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMouseMove(
            object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            try
            {
                string mapUnitName = Common.UtilityClass.GetMapUnitText(axMapControl1.MapUnits);
                if (mapUnitName.Length == 0)
                {
                    mapUnitName = axMapControl1.MapUnits.ToString().Substring(4);
                }

                //���e���W�n
                if (axMapControl1.SpatialReference is
                        ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)
                {
                    //Point�I�u�W�F�N�g�̍쐬
                    ESRI.ArcGIS.Geometry.IPoint point = new ESRI.ArcGIS.Geometry.PointClass();
                    point.SpatialReference = axMapControl1.SpatialReference;
                    point.X = e.mapX;
                    point.Y = e.mapY;

                    //��ԎQ�Ɓi�n�����W�n:JGD 2000�j�̍쐬
                    ESRI.ArcGIS.Geometry.ISpatialReferenceFactory sprefFactry
                        = new ESRI.ArcGIS.Geometry.SpatialReferenceEnvironmentClass();
                    ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem geoCoordSystem =
                        sprefFactry.CreateGeographicCoordinateSystem
                            ((int)ESRI.ArcGIS.Geometry.esriSRGeoCS3Type.esriSRGeoCS_JapanGeodeticDatum2000);

                    //�n�����W�n:JGD 2000�œ��e
                    point.Project(geoCoordSystem);

                    if (point.IsEmpty == true)
                    {
                        statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}",
                            e.mapX.ToString("#######.###"),
                            e.mapY.ToString("#######.###"),
                            mapUnitName
                        );

                        return;
                    }

                    statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}    {3}, {4}  {5}",
                            e.mapX.ToString("#######.###"),         //0
                            e.mapY.ToString("#######.###"),         //1
                            mapUnitName,                            //2
                            point.X.ToString("####.#####"),         //3
                            point.Y.ToString("####.#####"),         //4
                            "�x(10�i)"                              //5
                        );
                }
                //�n�����W�n
                else if (axMapControl1.SpatialReference is
                            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem)
                {
                    statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}",
                            e.mapX.ToString("####.#####"),
                            e.mapY.ToString("####.#####"),
                            mapUnitName
                        );
                }
                //UnknownCoordinateSystem
                else
                {
                    statusBarXY.Text =
                        string.Format(
                            "{0}, {1}  {2}",
                            e.mapX.ToString("#######.#####"),
                            e.mapY.ToString("#######.#####"),
                            mapUnitName
                        );
                }
            }
            catch (COMException comex)
            {
                Common.UtilityClass.DoOnError(comex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseMove);
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseMove);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e) {
            try {
                if(e.button != 2) {
                    SetMapStatusChanged();

                    // �E�N���b�N�ł͂Ȃ�
                    return;
                }

                // ���_�}���A�폜���j���[��Enable Switching
                // Call the SetEditLocation method using the OnMouseUp location
                m_EngineEditSketch.SetEditLocation(e.x, e.y);

                if(this.IsEditMode) {
                    m_ToolbarMenuEditStarted.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
                else {
                  //m_MapMenu.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                    m_ToolbarMenu.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
            }
            catch(Exception ex) {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseDown);
            }
        }

        /// <summary>
        /// TOCControl�ŉE�N���b�N���Ƀ��j���[��\��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e) {
            try {
                if(e.button != 2) // �E�N���b�N�݂̂œ���
                    return;

                // splitcontainer�ɓ��ꂽ�ꍇpopup���\�������u�Ԃ�
                // ��ԏ�̃��j���[�A�C�e�������s�����̂����
                // 2012/07/25 UPD 
                //int offsetx = 1;
                int offsetx = 5;
                // 2012/07/25 UPD 

                // ���C�A�E�g�\���̏ꍇ�͕\�����Ȃ�
                if(!this.IsMapVisible) {
                    return;
                }

                esriTOCControlItem tocItem = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                System.Object obj = null;
                System.Object index = null;

                // �A�C�e���I�𔻒�
                m_TocControl.HitTest(e.x, e.y, ref tocItem, ref map, ref layer, ref obj, ref index);

                if(tocItem == esriTOCControlItem.esriTOCControlItemNone)
                    return;

                if(tocItem == esriTOCControlItem.esriTOCControlItemMap) {
                    m_TocControl.SelectItem(m_MapControl.ActiveView, null);
                }
                else if(tocItem == esriTOCControlItem.esriTOCControlItemLayer) {
					this.SelectedLayer = layer;
                }

                // ���C�����J�X�^���v���p�e�B�ݒ�
                m_MapControl.CustomProperty = layer;

                // �|�b�v�A�b�v�\��
                if (tocItem == esriTOCControlItem.esriTOCControlItemMap)
                    m_TocMenu.PopupMenu(e.x + offsetx, e.y, m_TocControl.hWnd);

                if (tocItem == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    // �����\�ȃ��C���ł���ꍇ��
                    // �����e�[�u���\����V�F�[�v�t�@�C���G�N�X�|�[�g���N���\�ɂ���
                    // �����ł͂Ȃ��ꍇ�ɂ͎g�p�s�ɂ���B
                    // �Y������R�}���h�N���X����Enable�v���p�e�B�Őݒ肵�Ă���
                    m_TocLayerMenu.PopupMenu(e.x + offsetx, e.y, m_TocControl.hWnd);
                }

            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenTocControlMouseDown);
            }
        }

        /// <summary>
        /// TOCControl�}�E�X�A�b�v��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            try
            {
                //���s�A���N���b�N�݂̂ł̑��삪�Ώۂł��邽�߃t�B���^��������
                if (e.button != 1)
                    return;


                esriTOCControlItem tocItem = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                System.Object obj = null;
                System.Object index = null;

                // �A�C�e���I�𔻒�
                m_TocControl.HitTest(
                    e.x, e.y, ref tocItem, ref map, ref layer, ref obj, ref index);

                // ���C�����J�X�^���v���p�e�B�ݒ�
                m_MapControl.CustomProperty = layer;

                //���C�����N���b�N��
                if (tocItem == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    _selectedLayer = layer;

                    if (e.button == 1)
                    {
                        //�O���[�v���C���ȊO�Ń����N�؂�̏ꍇ
                        if (layer != null
                            && !(layer is IGroupLayer)
                            && !layer.Valid)
                        {
                            var command = new EngineCommand.OpenDataSource();
                            command.OnCreate(m_MapControl.Object);
                            command.OnClick(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenTocControlMouseDown);

            }
        }

        /// <summary>
        /// �I��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            this.Dispose();
            Common.Logger.Info(Properties.Resources.CommonMessage_ApplicationName + "�I��");
        }


        /// <summary>
        /// �o�[�W�������\��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolMenuVersionInfo_Click(object sender, EventArgs e)
        {
            try
            {
                // �������g�̃o�[�W���������擾
                // Assembly
                System.Reflection.Assembly asm =
                    System.Reflection.Assembly.GetExecutingAssembly();

                // �o�[�W�����擾
                System.Version ver = asm.GetName().Version;


                //�o�[�W�������̕\��
                FormVersionInfo formVersionInfo =
                    new FormVersionInfo(ver.Major.ToString(),ver.Minor.ToString(),ver.Build.ToString());
                formVersionInfo.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_WhenToolMenuVersionInfo_Click);
            }
        }


        #region Custom Functions that you write to add additionaly functionality for the events
        // <summary>
        // MapControl Event handler
        // </summary>
        // <param name="sender"></param>
        // <param name="e"></param>
        //private void OnActiveViewEventsAfterDraw(
        //    ESRI.ArcGIS.Display.IDisplay Display, ESRI.ArcGIS.Carto.esriViewDrawPhase phase)
        //{
        //    SetMenuItemEnabled();
        //    //Common.Logger.Debug("AfterDraw");
        //}
        /// <summary>
        ///
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Display"></param>
        /// <param name="phase"></param>
        private void OnActiveViewEventsItemDraw(
            short Index, ESRI.ArcGIS.Display.IDisplay Display, ESRI.ArcGIS.esriSystem.esriDrawPhase phase)
        {
            if(!this.IsMapVisible) return;

#if DEBUG
            Common.Logger.Debug("ItemDraw");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : ItemDraw");
#endif
        }

        private void OnActiveViewEventsContentsChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("ContentsChanged");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : ContentsChanged");
#endif
        }

        private void OnActiveViewEventsContentsCleared()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("ContentsCleared");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : ContentsCleared");
#endif
        }

        private void OnActiveViewEventsFocusMapChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("FocusMapChanged");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : FocusMapChanged");
#endif
        }

        private void OnActiveViewEventsItemAdded(object Item)
        {
            SetMapStateChanged();
            SetMenuItemEnabled();
#if DEBUG
            Common.Logger.Debug("ItemAdded");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : ItemAdded");
#endif
        }

        private void OnActiveViewEventsItemDeleted(object Item)
        {
            SetMapStateChanged();
            SetMenuItemEnabled();
#if DEBUG
            Common.Logger.Debug("ItemDeleted");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : ItemDeleted");
#endif
        }

        private void OnActiveViewEventsItemReordered(object Item, int toIndex)
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("ItemReordered");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : ItemReordered");
#endif
        }

        private void OnActiveViewEventsSelectionChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("SelectionChanged");
            Debug.WriteLineIf(Debug_Confirm, "����ACTVIEW EVENT : SelectionChanged, " + DateTime.Now.ToString("HH:mm:ss"));

            StringBuilder	sbRepo = new StringBuilder();
            // �ҏW����߰�
            if(this.IsEditMode) {
				IFeatureLayer		agFLayer = this.EditTargetLayer;
				if(agFLayer == null) {
					sbRepo.AppendLine("�ҏW�Ώ�ڲ� : �Ȃ�");
				}
				else {
					IFeatureSelection	agFSel = agFLayer as IFeatureSelection;
					sbRepo.AppendFormat("�ҏW�Ώ�ڲ� : {0}, �I��̨������ : {1}\r\n", agFLayer.Name, agFSel.SelectionSet.Count);
				}

				// �ҏW�I��Ă��m�F
				int	intFeats = 0;
				IEnumFeature	agSelFeat = this.m_EngineEditor.EditSelection;
				IFeature		agFeat;
				while((agFeat = agSelFeat.Next()) != null) {
					++intFeats;
				}
            }
            // ϯ�ߑI��̨�������߰�
            if(this.m_map.SelectionCount > 0) {
				sbRepo.AppendLine("�I��̨���� :");

				IEnumFeature	agSelFEnum = this.m_map.FeatureSelection as IEnumFeature;
				IFeature		agFeat;
				while((agFeat = agSelFEnum.Next()) != null) {
					sbRepo.AppendFormat("FC = {0}, OID = {1}\r\n", (agFeat.Table as IFeatureClass).AliasName, agFeat.OID);
				}
            }
            else {
				sbRepo.AppendLine("�I��̨�����Ȃ�");
            }

            Debug.WriteLine(sbRepo.ToString());
#endif
        }

        private void OnActiveViewEventsSpatialReferenceChanged()
        {
            SetMapStateChanged();
#if DEBUG
            Common.Logger.Debug("SpatialReferenceChanged");
            Debug.WriteLineIf(Debug_Confirm, "��ACTVIEW EVENT : SpatialReferenceChanged");
#endif
        }

        //private void OnActiveViewEventsViewRefreshed(
        //    ESRI.ArcGIS.Carto.IActiveView view,
        //    ESRI.ArcGIS.Carto.esriViewDrawPhase phase,
        //    object data, ESRI.ArcGIS.Geometry.IEnvelope envelope)
        //{
        //    Common.Logger.Debug("ViewRefreshed");
        //}

        private void OnPageLayoutControlEventsOnViewRefreshed(
            object ActiveView, int viewDrawPhase, object layerOrElement, object envelope)
        {
            if (viewDrawPhase == (int)esriViewDrawPhase.esriViewGraphics)
            {
                SetMapStateChanged();
            }
        }

        private void OnPageLayoutControlEventsOnFocusMapChanged()
        {
            SetMapStateChanged();
        }

        private void ONPageLayoutControlEventsOnExtentUpdated(
            object displayTransformation, bool sizeChanged, object newEnvelope)
        {
            SetMapStateChanged();
        }


        /// <summary>
        /// �h�L�������g�ۑ��m�F�ƕۑ�
        /// </summary>
        private bool CheckDocumentSaveOrNot()
        {
            IEngineEditor engineEditor = new EngineEditorClass();
            if((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) &&
                (engineEditor.HasEdits() == true))
            {
                DialogResult result = MessageBox.Show(
                    "�ҏW���̏�Ԃ�ۑ����܂����H",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.No:
                        engineEditor.StopEditing(false);
                        break;

                    case DialogResult.Yes:
                        engineEditor.StopEditing(true);
                        break;
                }
            }

            bool doSave = false;
            //if (m_MapControl.LayerCount > 0 && this.MainMapChanged)
            if (this.MainMapChanged)
            {
                DialogResult res = MessageBox.Show(
                    "���݂̃h�L�������g��ۑ����܂���?",
                    Properties.Resources.CommonMessage_ApplicationName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (res)
                {
                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.No:
                        break;

                    case DialogResult.Yes:
                        doSave = true;
                        break;
                }
            }

            // �ҏW�J�n��Ԃ̊m�F
            if(engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                engineEditor.StopEditing(true);
            }

            if(doSave) {
                IMapDocument mapDoc = new MapDocumentClass();
                try {
                    if(m_MapControl.CheckMxFile(axMapControl1.DocumentFilename)) {
                        mapDoc.Open(axMapControl1.DocumentFilename, string.Empty);
                        if(mapDoc.get_IsReadOnly(axMapControl1.DocumentFilename)) {
                            mapDoc.Close();
                            Common.Logger.Info("���̃h�L�������g�͓Ǎ��ݐ�p�ł�");
                            Common.MessageBoxManager.ShowMessageBoxInfo(
                                this, "���̃h�L�������g�͓Ǎ��ݐ�p�ł�");
                            return false;
                        }

                        m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                        m_MapControl.ActiveView.ShowScrollBars = true;

                        mapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                        //mapDoc.ReplaceContents((IMxdContents)m_MapControl.Map);

                        mapDoc.SetActiveView(m_MapControl.ActiveView);

                        mapDoc.Save(true, false);
                        mapDoc.Close();
                        ClearMapChangeCount();
                    }
                    else {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "ArcMap�h�L�������g (*.mxd)|*.mxd|" +
                                                "ArcMap�e���v���[�g (*.mxt)|*.mxt";

                        DialogResult result = saveFileDialog.ShowDialog(this);

                        if(result == DialogResult.OK) {
                            mapDoc.New(saveFileDialog.FileName);

                            m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                            m_MapControl.ActiveView.ShowScrollBars = true;

                            mapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                            mapDoc.SetActiveView(m_pageLayoutControl.ActiveView);

                            if (saveFileDialog.FilterIndex == 1) // mxd
                            {
                                // mxd�ۑ���
                                mapDoc.SetActiveView(m_MapControl.ActiveView);
                            }

                            mapDoc.Save(true, false);
                            axMapControl1.DocumentFilename = mapDoc.DocumentFilename;
                            mapDoc.Close();

                            axMapControl1.DocumentFilename = saveFileDialog.FileName;
                            string path = System.IO.Path.GetFileName(saveFileDialog.FileName);
                            this.Text = path + " - " +
                                Properties.Resources.CommonMessage_ApplicationName;

                            // �㏑���ۑ��L����
                            menuSaveDoc.Enabled = true;

                            // Toolbar�̏㏑���ۑ��L����
                            if (m_ToolbarControl != null)
                            {
                                for (int i = 0; i < m_ToolbarControl.Count; i++)
                                {
                                    IToolbarItem ti = m_ToolbarControl.GetItem(i);

                                    if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                                    {
                                        EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                        saveDocumet.EnableCommand();
                                    }
                                }
                            }

                            this.SetEvents();
                            this.ClearMapChangeCount();
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Common.UtilityClass.DoOnError(ex);

                    DialogResult dResult = GISLight10.Common.MessageBoxManager.ShowMessageBoxError2(
                        this, Properties.Resources.MainForm_ErrorWhenMenuSave + "\n" +
                        ex.Message + "\n" +
                        "�I�����Ă��悢�ł����H");

                    if (dResult == DialogResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mapDoc);
                }
            }
            return true;
        }

        #endregion write to add additionaly functionality for the events


        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axToolbarControl1_OnItemClick(object sender, IToolbarControlEvents_OnItemClickEvent e)
        {
            // �}�b�v�ύX��ԂłȂ���΁A�J�����g�c�[���C���f�N�X�ێ�
            if (!map_sts_changed)
            {
                CurrentToolIndex = e.index;

            }
            //Common.Logger.Debug(e.index.ToString());

        }

        /// <summary>
        /// �I����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 2012/08/28 add >>>>>
            // �J���Ă���W�I���t�@�����X�t�H�[������Ε���
            if (this.OwnedForms.Length > 0)
            {
                for (int i = 0; i < this.OwnedForms.Length; i++)
                {
                    if (this.OwnedForms[i].Text.Contains("�W�I���t�@�����X"))
                    {
                        this.OwnedForms[i].Dispose();
                        break;
                    }
                }
            }
            // 2012/08/28 add <<<<<

            if (!CheckDocumentSaveOrNot())
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// �}�b�v�R���g���[���`��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            if(!this.IsMapVisible) return;
#if DEBUG
			// ̪��ނ𔻒�
			string	strPhase = "";
			switch(e.viewDrawPhase) {
			case 0:
				strPhase = "None";				break;
			case 1:
				strPhase = "Background";		break;
			case 2:
				strPhase = "Geography";			break;
			case 4:
				strPhase = "GeoSelection";		break;
			case 8:
				strPhase = "Graphics";			break;
			case 16:
				strPhase = "GraphicSelection";	break;
			case 32:
				strPhase = "Foreground";		break;
			case 64:
				strPhase = "Initialized";		break;
			}

			// �\���͈͂��擾
			ESRI.ArcGIS.Geometry.IEnvelope	agEnv = this.m_MapControl.ActiveView.Extent;
			Debug.WriteLineIf(Debug_Confirm, string.Format("��MAPCONTROL EVENT : AfterDraw / EXTENT : L={0}, R={1}, B={2}, T={3} / DrawPhase : {4}", agEnv.XMin, agEnv.XMax, agEnv.YMin, agEnv.YMax, strPhase));
#endif
            if(e.viewDrawPhase == Convert.ToInt32(esriViewDrawPhase.esriViewForeground)) {
                SetMenuItemEnabled();
            }
            //Common.Logger.Debug("axMapControl1_OnAfterDraw " +  e.viewDrawPhase.ToString());
        }

        /// <summary>
        /// MapControl��PageLayout�̐ؑ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 2011/06/06 �ҏW���̓��C�A�E�g�؂�ւ��}�~����
            if(this.IsEditMode) return;

            if(this.IsMapVisible) //map view
            {
                //activate the MapControl and deactivate the PageLayoutControl
                //m_controlsSynchronizer.PageLayoutControl.PageLayout = axPageLayoutControl1.PageLayout;

                //2010-12-27 del m_controlsSynchronizer.ActivatePageLayout();
                m_controlsSynchronizer.ActivateMap();

                // ���j���[�L��
                toolMenuSentaku.Enabled = true;
                toolMenuTableJoinRelate.Enabled = true;

                // �ҏW�L��
                m_ToolbarControl2.Enabled = true;

                // TOC�h���b�O�L��
                m_TocControl.EnableLayerDragDrop = true;

                // �c�[���o�[�̑I����Ԃ��N���A
                m_ToolbarControl.CurrentTool = null;
            }
            else //layout view
            {
                //// �ҏW�I���m�F
                //DialogResult stopEditingResult;
                //DialogResult saveEditingResult;
                //IEngineEditor engineEditor = new EngineEditorClass();

                //if (engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)
                //{
                //    stopEditingResult = MessageBox.Show(
                //            "�ҏW���I�����܂����H",
                //            Properties.Resources.CommonMessage_ApplicationName,
                //            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //    if (stopEditingResult == DialogResult.No)
                //    {
                //        this.IsMapVisible = true;
                //        return;
                //    }
                //    else
                //    {
                //        if (engineEditor.HasEdits() == true)
                //        {
                //            saveEditingResult = MessageBox.Show(
                //                "�ҏW���̏�Ԃ�ۑ����܂����H",
                //                Properties.Resources.CommonMessage_ApplicationName,
                //                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                //            if (saveEditingResult == DialogResult.Cancel)
                //            {
                //                this.IsMapVisible = true;
                //                return;
                //            }
                //            else if (saveEditingResult == DialogResult.Yes)
                //            {
                //                engineEditor.StopEditing(true);
                //            }
                //            else
                //            {
                //                engineEditor.StopEditing(false);
                //            }
                //        }
                //    }
                //}

                //activate the PageLayoutControl and deactivate the MapControl
                m_controlsSynchronizer.ActivatePageLayout();

                // ���j���[����
                //toolMenuSentaku.Enabled = false;
                //toolMenuTableJoinRelate.Enabled = false;

                // �ҏW����
                m_ToolbarControl2.Enabled = false;

                // TOC�h���b�O����
                m_TocControl.EnableLayerDragDrop = false;

                // �c�[���o�[�̑I����Ԃ��N���A
                m_ToolbarControl.CurrentTool = null;

	            // ��۰٥�ް��L����
	            m_pageLayoutControl.ActiveView.ShowScrollBars = true;
            }
        }

		// °��ް�̕\������
		private void FitToolBarWidth(IToolbarControl2 TargetToolbar) {
			// �Ō��°٥���ѕ`��͈͂��擾
			int	intL = 0, intR = 0, intT = 0, intB = 0;
			TargetToolbar.GetItemRect(TargetToolbar.Count - 1, ref intT, ref intL, ref intB, ref intR);

			// °��ް�̕��𒲐� ��Width��intR�҂�����ɂ���ƁA���̂��E�[��°ق��g�p�ł��Ȃ��Ȃ��Ă��܂�...
			this.axToolbarControl2.Width = intR + 10;
			// �޵��̧�ݽ°٥�ް�̈ʒu�𒲐�
			this.axToolbarControl5.Left = this.axToolbarControl2.Width;

			// °��ް�ɸ���ް��݂�...
			//TargetToolbar.FillDirection = esriToolbarFillDirection.esriToolbarFillVertical;
			//TargetToolbar.Transparent = true;
			//TargetToolbar.ThemedDrawing = true;
			//TargetToolbar.Appearance = esriControlsAppearance.esri3D;
			//TargetToolbar.BackColor = ColorTranslator.FromHtml("#CCCCCC").ToArgb();
			//TargetToolbar.FadeColor = ColorTranslator.FromHtml("#FCFCFC").ToArgb();
		}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axPageLayoutControl1_OnMouseDown(
            object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {
            try
            {
                if (e.button != 2)
                {
                    SetMapStatusChanged();

                    // �E�N���b�N�ł͂Ȃ�
                    return;
                }

                // ���_�}���A�폜���j���[��Enable Switching
                // Call the SetEditLocation method using the OnMouseUp location
                m_EngineEditSketch.SetEditLocation(e.x, e.y);

                if(this.IsEditMode) {
                    m_ToolbarMenuEditStarted.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
                else {
                    m_ToolbarMenu3.PopupMenu(e.x, e.y, m_MapControl.hWnd);
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseDown);
            }
        }

        private void axToolbarControl4_OnItemClick(object sender, IToolbarControlEvents_OnItemClickEvent e)
        {
            PageLayoutCurrentToolIndex = e.index;
            Common.Logger.Debug("PageLayout CurrentTool Inde x= " + PageLayoutCurrentToolIndex.ToString());
        }

        /// <summary>
        /// �y�[�W���C�A�E�g�ݒ�
        /// �R�}���h�N���X�Ŏ��������� 10/18(mon)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPageSetup_Click(object sender, EventArgs e)
        {
            //bool dispMessage = false;
            //try
            //{
            //    //Show the page setup dialog storing the result.
            //    DialogResult result = pageSetupDialog1.ShowDialog();

            //    //set the printer settings of the preview document to the selected printer settings
            //    document.PrinterSettings = pageSetupDialog1.PrinterSettings;

            //    //set the page settings of the preview document to the selected page settings
            //    document.DefaultPageSettings = pageSetupDialog1.PageSettings;

            //    //due to a bug in PageSetupDialog the PaperSize has to be set explicitly by iterating through the
            //    //available PaperSizes in the PageSetupDialog finding the selected PaperSize
            //    //int i;
            //    IEnumerator paperSizes = pageSetupDialog1.PrinterSettings.PaperSizes.GetEnumerator();
            //    paperSizes.Reset();

            //    for (int i = 0; i < pageSetupDialog1.PrinterSettings.PaperSizes.Count; ++i)
            //    {
            //        paperSizes.MoveNext();
            //        if (((PaperSize)paperSizes.Current).Kind == document.DefaultPageSettings.PaperSize.Kind)
            //        {
            //            document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
            //        }
            //    }

            //    /////////////////////////////////////////////////////////////
            //    ///initialize the current printer from the printer settings selected
            //    ///in the page setup dialog
            //    /////////////////////////////////////////////////////////////
            //    IPaper paper = new PaperClass(); //create a paper object

            //    IPrinter printer = new EmfPrinterClass(); //create a printer object
            //    //in this case an EMF printer, alternatively a PS printer could be used

            //    //initialize the paper with the DEVMODE and DEVNAMES structures from the windows GDI
            //    //these structures specify information about the initialization and environment of a printer as well as
            //    //driver, device, and output port names for a printer
            //    paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(
            //        pageSetupDialog1.PageSettings).ToInt32(),
            //        pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

            //    //pass the paper to the emf printer
            //    printer.Paper = paper;

            //    //set the page layout control's printer to the currently selected printer
            //    axPageLayoutControl1.Printer = printer;


            //    if (pageSetupDialog1.PageSettings.PaperSize.PaperName.Contains(
            //        supportPaperSizeName[A4_Index]))
            //    {
            //        this.labelPageSize.Text = supportPaperSizeName[A4_Index];
            //    }
            //    else if (pageSetupDialog1.PageSettings.PaperSize.PaperName.Contains(
            //        supportPaperSizeName[A3_Index]))
            //    {
            //        this.labelPageSize.Text = supportPaperSizeName[A3_Index];
            //    }
            //    else
            //    {
            //        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
            //            this, Properties.Resources.MainForm_PrintLayoutPageSizeInvalid);

            //        this.labelPageSize.Text = pageSetupDialog1.PageSettings.PaperSize.PaperName;
            //        this.buttonPrint.Enabled = false;
            //        this.buttonPrintPreview.Enabled = false;
            //        return;
            //    }

            //    this.buttonPrint.Enabled = true;
            //    this.buttonPrintPreview.Enabled = true;
            //    if (pageSetupDialog1.PageSettings.Landscape)
            //        this.labelPageOrientation.Text = pageLandscape;
            //    else
            //        this.labelPageOrientation.Text = pagePortrait;

            //}
            //catch (COMException comex)
            //{
            //    dispMessage = true;
            //    Common.UtilityClass.DoOnError(comex);
            //}
            //catch (Exception ex)
            //{
            //    dispMessage = true;
            //    Common.UtilityClass.DoOnError(ex);
            //}
            //finally
            //{
            //    if (dispMessage)
            //    {

            //    }
            //}
        }


        /// <summary>
        /// �y�[�W���C�A�E�g�����ݒ�
        /// </summary>
        internal void InitPrintSettings()
        {
            //associate the event-handling method with the document's PrintPage event
            this.document.PrintPage +=
                new System.Drawing.Printing.PrintPageEventHandler(document_PrintPage);

            //create a new PageSetupDialog using constructor
            pageSetupDialog1 = new PageSetupDialog();

            // �]�������o�O�Ή�
            pageSetupDialog1.EnableMetric = true;

            // �v�����^�ݒ��A�N�e�B��
            //pageSetupDialog1.AllowPrinter = false;

            //initialize the dialog's PrinterSettings property to hold user defined printer settings
            pageSetupDialog1.PageSettings = new System.Drawing.Printing.PageSettings();
            //initialize dialog's PrinterSettings property to hold user set printer settings
            pageSetupDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            //do not show the network in the printer dialog
            pageSetupDialog1.ShowNetwork = false;

            pageSetupDialog1.PageSettings.Margins.Top = 0;
            pageSetupDialog1.PageSettings.Margins.Left = 0;
            pageSetupDialog1.PageSettings.Margins.Bottom = 0;
            pageSetupDialog1.PageSettings.Margins.Right = 0;

            //pageSetupDialog1.AllowMargins = false;

            for(int i = 0; i < supportPaperSizeName.Length; i++) {
	            foreach(PaperSize ps in pageSetupDialog1.PrinterSettings.PaperSizes) {
                    if(ps.PaperName.Contains(supportPaperSizeName[i])) {
                        supportPaperSizeEnable[i] = true;
                        break;
                    }
                }
            }

			//// �f�t�H���g A4 �c
			//if (supportPaperSizeEnable[A4_Index])
			//{
			//    this.labelPageSize.Text = supportPaperSizeName[A4_Index];
			//}
			//else if (supportPaperSizeEnable[A3_Index])
			//{
			//    this.labelPageSize.Text = supportPaperSizeName[A3_Index];
			//}

            pageSetupDialog1.PageSettings.Landscape = false;
			//this.labelPageOrientation.Text = pagePortrait;

            this.axPageLayoutControl1.PageLayout.Page.FormID =
                ESRI.ArcGIS.Carto.esriPageFormID.esriPageFormSameAsPrinter;

            // �C���X�g�[������Ă���v�����^������ꍇ
            if(System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count > 0) {
                // �y�[�W�ݒ���J���Ȃ��Ă�����ł���悤�ɑΉ�
                // �v�����^�̐ݒ�
                this.document.PrinterSettings = this.pageSetupDialog1.PrinterSettings;
                this.document.DefaultPageSettings = this.pageSetupDialog1.PageSettings;

                //IPaper paper = new PaperClass();
                //IPrinter printer = new EmfPrinterClass();

                //paper.Attach(this.pageSetupDialog1.PrinterSettings.GetHdevmode(
                //    this.pageSetupDialog1.PageSettings).ToInt32(),
                //    this.pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

                //printer.Paper = paper;

                //this.axPageLayoutControl1.Printer = printer;

                this.document.OriginAtMargins = false;

                //mainForm.axPageLayoutControl1.Refresh();
			}

			// �����������ݒ�
			this.SetDefaultPrinter();

            // ����ݒ�̊T�v�\�����X�V
            this.LoadPrintSetting(null);

            // �߰�ނ�̨��
            this.axPageLayoutControl1.PageLayout.ZoomToWhole();
        }

        /// <summary>
        /// APP����̃v�����^�ݒ���̗p���܂�
        /// </summary>
        private void SetDefaultPrinter() {
			IPrinter	agPrinter = new EmfPrinterClass();	//create a printer object
			IPaper		agPaper = new PaperClass();			//create a paper object
			// in this case an EMF printer, alternatively a PS printer could be used

			// initialize the paper with the DEVMODE and DEVNAMES structures from the windows GDI
			// these structures specify information about the initialization
			// and environment of a printer as well as
			// driver, device, and output port names for a printer
			agPaper.Attach(this.pageSetupDialog1.PrinterSettings.GetHdevmode(
				this.pageSetupDialog1.PageSettings).ToInt32(),
				this.pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

			//pass the paper to the emf printer
			agPrinter.Paper = agPaper;


			// �߰�ސݒ���擾
			IPage	agPage = this.axPageLayoutControl1.PageLayout.Page;

			// �߰�ނɍ��킹��ϯ�ߴ���Ă̻��ނ𽹰�ݸ�
			if(!agPage.StretchGraphicsWithPage) {
				agPage.StretchGraphicsWithPage = true;
			}

			// �p���̌�����ݒ�
			if(agPage.Orientation != agPaper.Orientation) {
				agPage.Orientation = agPaper.Orientation;
			}

			// ������̗p���ݒ���g�p
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				agPage.FormID = esriPageFormID.esriPageFormSameAsPrinter;
			}

			// ���ޒ��� (������̗p���ݒ���g�p���Ȃ��ꍇ�A���͂ō��킹��)
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				double	dblW, dblH;
				agPage.FormID = EngineCommand.PrinitPageLayoutCommand.GetStretchPageSize(agPage, agPaper, out dblW, out dblH);
				if(agPage.FormID == esriPageFormID.esriPageFormCUSTOM) {
					agPage.PutCustomSize(dblW, dblH);
				}
			}

			// ���ޒP�ʂ�ݒ�
			if(agPage.Units != esriUnits.esriCentimeters) {
				agPage.Units = esriUnits.esriCentimeters;
			}


            //set the page layout control's printer to the currently selected printer
            this.axPageLayoutControl1.Printer = agPrinter;

            // ������̐ݒ��\��

            // �p������
            string	strPaperSizeName = this.pageSetupDialog1.PageSettings.PaperSize.PaperName;
            if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A4_Index])) {
                this.labelPageSize.Text = this.supportPaperSizeName[this.A4_Index];
            }
            else if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A3_Index])) {
                this.labelPageSize.Text = this.supportPaperSizeName[this.A3_Index];
            }
            else {
                // �p���T�C�Y�̐����͂��Ȃ��l�ɕύX
                //ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                //    this.mainForm, Properties.Resources.MainForm_PrintLayoutPageSizeInvalid);

                this.labelPageSize.Text =
                    this.pageSetupDialog1.PageSettings.PaperSize.PaperName;

                //this.ContinuPageLayout = true; // false;
                //return;
            }

            this.ContinuPageLayout = true;

            // �p���̌���
            if (this.pageSetupDialog1.PageSettings.Landscape)
                this.labelPageOrientation.Text = this.pageLandscape;
            else
                this.labelPageOrientation.Text = this.pagePortrait;

            this.axPageLayoutControl1.Refresh();
        }

        /// <summary>
        /// OS����̃v�����^�ݒ���̗p���܂� (���g�p)
        /// </summary>
        private void SetDefaultPrinterSetting() {
			// �����������ݒ���擾
			PrintDocument	prDoc = new System.Drawing.Printing.PrintDocument();

			// �p���ݒ���擾
			IPaper			agPaper = new PaperClass();
			agPaper.Attach(prDoc.PrinterSettings.GetHdevmode().ToInt32(), prDoc.PrinterSettings.GetHdevnames().ToInt32());

			// �p����ݒ�
			IPrinter	agPrinter = new EmfPrinterClass();
			agPrinter.Paper = agPaper;

			// �߰�ސݒ���擾
			IPage	agPage = this.axPageLayoutControl1.PageLayout.Page;

			// �߰�ނɍ��킹��ϯ�ߴ���Ă̻��ނ𽹰�ݸ�
			if(!agPage.StretchGraphicsWithPage) {
				agPage.StretchGraphicsWithPage = true;
			}

			// �p���̌�����ݒ�
			if(agPage.Orientation != agPaper.Orientation) {
				agPage.Orientation = agPaper.Orientation;
			}

			// ������̗p���ݒ���g�p
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				agPage.FormID = esriPageFormID.esriPageFormSameAsPrinter;
			}

			// ���ޒ��� (������̗p���ݒ���g�p���Ȃ��ꍇ�A���͂ō��킹��)
			if(agPage.FormID != esriPageFormID.esriPageFormSameAsPrinter) {
				double	dblW, dblH;
				agPage.FormID = EngineCommand.PrinitPageLayoutCommand.GetStretchPageSize(agPage, agPaper, out dblW, out dblH);
				if(agPage.FormID == esriPageFormID.esriPageFormCUSTOM) {
					agPage.PutCustomSize(dblW, dblH);
				}
			}

			// ���ޒP�ʂ�ݒ�
			if(agPage.Units != esriUnits.esriCentimeters) {
				agPage.Units = esriUnits.esriCentimeters;
			}

            // ��������ݒ�
            this.axPageLayoutControl1.Printer = agPrinter;

            // ������ݒ��\��

            // �p������
            string	strPaperSizeName = this.axPageLayoutControl1.Printer.Paper.FormName;
            if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A4_Index])) {
				this.labelPageSize.Text = this.supportPaperSizeName[this.A4_Index];
            }
            else if(strPaperSizeName.Contains(this.supportPaperSizeName[this.A3_Index])) {
				this.labelPageSize.Text = this.supportPaperSizeName[this.A3_Index];
            }
            else {
				this.labelPageSize.Text = strPaperSizeName;
            }

            this.ContinuPageLayout = true;

            // �p���̌���
            if(agPaper.Orientation == 2) {
				this.labelPageOrientation.Text = this.pageLandscape;
            }
            else {
				this.labelPageOrientation.Text = this.pagePortrait;
            }

            // �ݒ�𔽉f
            this.axPageLayoutControl1.Refresh();
        }

        /// <summary>
        /// ����v���r���[
        /// �R�}���h�N���X�Ŏ����@�P�O�^�P�W�iMON)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrintPreview_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                //// create a new PrintPreviewDialog using constructor
                //printPreviewDialog1 = new PrintPreviewDialog();
                //printPreviewDialog1.ShowIcon = false;


                ////initialize the currently printed page number
                //m_CurrentPrintPage = 0;

                ////check if a document is loaded into PageLayout	control
                ////if (axPageLayoutControl1.DocumentFilename == null) return;
                ////if (m_pageLayoutControl.DocumentFilename == null) return;
                //if (m_controlsSynchronizer.MapControl.DocumentFilename == null)
                //{
                //    dispMessage = true;
                //    return;
                //}

                ////set the name of the print preview document to the name of the mxd doc
                //document.DocumentName = m_controlsSynchronizer.MapControl.DocumentFilename;

                ////set the PrintPreviewDialog.Document property to the PrintDocument object selected by the user
                //printPreviewDialog1.Document = document;

                ////show the dialog - this triggers the document's PrintPage event
                //printPreviewDialog1.ShowDialog(this);
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (dispMessage)
                {

                }
            }

        }



        /// <summary>
        ///  this code will be called when the PrintPreviewDialog.Show method is called
        ///  set the PageToPrinterMapping property of the Page. This specifies how the page
        ///  is mapped onto the printer page. By default the page will be tiled
        ///  get the selected mapping option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            short printPageCount = (short)0.0;
            int hInfoDC = 0;
            IntPtr hdc = new IntPtr();

            try
            {
                //string sPageToPrinterMapping = (string)this.comboBox1.SelectedItem;
                //if (sPageToPrinterMapping == null)
                //if no selection has been made the default is tiling

                //axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;

                //else if (sPageToPrinterMapping.Equals("esriPageMappingTile"))1
                //    axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;
                //else if (sPageToPrinterMapping.Equals("esriPageMappingCrop"))
                //    axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingCrop;
                //else if (sPageToPrinterMapping.Equals("esriPageMappingScale"))

                axPageLayoutControl1.Page.PageToPrinterMapping =
                        esriPageToPrinterMapping.esriPageMappingScale;

                //else
                //    axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;

                //get the resolution of the graphics device used by the print preview (including the graphics device)
                short dpi = (short)e.Graphics.DpiX;

                //envelope for the device boundaries
                ESRI.ArcGIS.Geometry.IEnvelope devBounds = new ESRI.ArcGIS.Geometry.EnvelopeClass();

                //get page
                IPage page = axPageLayoutControl1.Page;

                //the number of printer pages the page will be printed on
                printPageCount = axPageLayoutControl1.get_PrinterPageCount(0);
                m_CurrentPrintPage++;

                //the currently selected printer
                IPrinter printer = axPageLayoutControl1.Printer;

                //get the device bounds of the currently selected printer
                page.GetDeviceBounds(printer, m_CurrentPrintPage, 0, dpi, devBounds);

                //Returns the coordinates of lower, left and upper, right corners
                double xmin, ymin, xmax, ymax;
                devBounds.QueryCoords(out xmin, out ymin, out xmax, out ymax);

                //structure for the device boundaries
                tagRECT deviceRect;

                deviceRect.bottom = 0;
                deviceRect.left = 0;
                deviceRect.top = 0;
                deviceRect.right = 0;

                //initialize the structure for the device boundaries
                if (isPreviewPage == true)
                {
                    deviceRect.bottom = (int)ymax;
                    deviceRect.left = (int)xmin;
                    deviceRect.top = (int)ymin;
                    deviceRect.right = (int)xmax;
                }

                // Get the printer's hDC, so we can use the Win32 GetDeviceCaps function to
                //  get Printer's Physical Printable Area x and y margins
                hInfoDC = CreateDC(printer.DriverName, printer.Paper.PrinterName, "", IntPtr.Zero);

                if (isPrintPage == true)
                {
                    deviceRect.bottom = (int)(ymax - GetDeviceCaps(hInfoDC, 113));
                    deviceRect.left = (int)(xmin - GetDeviceCaps(hInfoDC, 112));
                    deviceRect.top = (int)(ymin - GetDeviceCaps(hInfoDC, 113));
                    deviceRect.right = (int)(xmax - GetDeviceCaps(hInfoDC, 112));
                }

                devBounds.PutCoords(0, 0, deviceRect.right - deviceRect.left, deviceRect.bottom - deviceRect.top);

                //determine the visible bounds of the currently printed page
                ESRI.ArcGIS.Geometry.IEnvelope visBounds = new ESRI.ArcGIS.Geometry.EnvelopeClass();
                page.GetPageBounds(printer, m_CurrentPrintPage, 0, visBounds);

                //get a handle to the graphics device that the print preview will be drawn to
                hdc = e.Graphics.GetHdc();

                //print the page to the graphics device using the specified boundaries
                axPageLayoutControl1.ActiveView.Output(
                    hdc.ToInt32(), dpi, ref deviceRect, visBounds, m_TrackCancel);

            }
            catch (COMException comex)
            {
                ESRIJapan.GISLight10.Common.Logger.Debug("COM Exception On document_PrintPage UsingMemorySize GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                ESRIJapan.GISLight10.Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Debug("UsingMemorySize GC :" +
                   GC.GetTotalMemory(false).ToString() + " byte");

                MessageBox.Show(ex.Message, "������̗\�����ʃG���[", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Common.Logger.Fatal(ex.Source);
                Common.Logger.Fatal(ex.Message);
                Common.Logger.Fatal(ex.StackTrace);
            }
            finally
            {
                //release the graphics device handle
                e.Graphics.ReleaseHdc(hdc);

                //release the DC...
                ReleaseDC(0, hInfoDC);

                //check if further pages have to be printed
                if (m_CurrentPrintPage < printPageCount)
                    e.HasMorePages = true; //document_PrintPage event will be called again
                else
                    e.HasMorePages = false;
            }
        }

        /// <summary>
        /// ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                printDialog1 = new PrintDialog(); //create a print dialog object

                //allow the user to choose the page range to be printed
                printDialog1.AllowSomePages = true;
                //show the help button.
                printDialog1.ShowHelp = true;

                //set the Document property to the PrintDocument for which the PrintPage Event
                //has been handled. To display the dialog, either this property or the
                //PrinterSettings property must be set
                printDialog1.Document = document;

                //show the print dialog and wait for user input
                DialogResult result = printDialog1.ShowDialog();

                // If the result is OK then print the document.
                if (result == DialogResult.OK) document.Print();
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (dispMessage)
                {

                }
            }
        }

        /// <summary>
        /// �y�[�W���C�A�E�g�̃��Z�b�g
        /// �R�}���h�N���X�Ŏ����������P�O�D�P�W�iMON)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResetLayout_Click(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                //ClearPageLayout();

                //// restore when map document open saved pagelayout
                //this.axPageLayoutControl1.PageLayout = savePageLayout;

                //m_controlsSynchronizer.ReplaceMap(this.axMapControl1.Map);
                //m_controlsSynchronizer.ActivatePageLayout();

                //buttonPrint.Enabled = true;
                //buttonPrintPreview.Enabled = true;
                //InitPrintSettings();

            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (dispMessage)
                {

                }
            }

        }

        /// <summary>
        /// ���O��t���ĕۑ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            IMapDocument pMapDoc = new MapDocumentClass();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "ArcMap�h�L�������g (*.mxd)|*.mxd|" +
                                        "ArcMap�e���v���[�g (*.mxt)|*.mxt";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    pMapDoc.New(saveFileDialog.FileName);

                    m_pageLayoutControl.ActiveView.ShowScrollBars = true;
                    m_MapControl.ActiveView.ShowScrollBars = true;

                    pMapDoc.ReplaceContents((IMxdContents)axPageLayoutControl1.PageLayout);
                    pMapDoc.SetActiveView(m_pageLayoutControl.ActiveView);

                    if (saveFileDialog.FilterIndex == 1) // mxd
                    {
                        // mxd�ۑ���
                        pMapDoc.SetActiveView(m_MapControl.ActiveView);
                    }

                    pMapDoc.Save(true, false);
                    axMapControl1.DocumentFilename = pMapDoc.DocumentFilename;
                    pMapDoc.Close();

                    axMapControl1.DocumentFilename = saveFileDialog.FileName;
                    string path = System.IO.Path.GetFileName(saveFileDialog.FileName);
                    this.Text = path + " - " +
                        Properties.Resources.CommonMessage_ApplicationName;

                    // �㏑���ۑ��L����
                    menuSaveDoc.Enabled = true;

                    // Toolbar�̏㏑���ۑ��L����
                    if (m_ToolbarControl != null)
                    {
                        for (int i = 0; i < m_ToolbarControl.Count; i++)
                        {
                            IToolbarItem ti = m_ToolbarControl.GetItem(i);

                            if (ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.SaveDocument")
                            {
                                EngineCommand.SaveDocument saveDocumet = (EngineCommand.SaveDocument)ti.Command;
                                saveDocumet.EnableCommand();
                            }
                        }
                    }

                    this.SetEvents();
                    this.ClearMapChangeCount();
                }
            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMenuSave);

            }
            finally
            {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pMapDoc);
            }
        }

        /// <summary>
        /// �y�[�W���C�A�E�g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axPageLayoutControl1_OnDoubleClick(object sender, IPageLayoutControlEvents_OnDoubleClickEvent e)
        {
            bool dispMessage = false;
            try
            {
                // �e�L�X�g�C��
                //EditTextElement();

                //SetEditEnableScaleBar();
            }
            catch (COMException comex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(comex);
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
            }
            finally
            {
                if (dispMessage)
                {

                }
            }
        }

        /// <summary>
        /// �y�[�W���C�A�E�g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            try
            {
                string unitName = Common.UtilityClass.GetMapUnitText(axPageLayoutControl1.Page.Units);
                if (unitName.Length == 0)
                {
                    unitName = axPageLayoutControl1.Page.Units.ToString().Substring(4);
                }

                statusBarXY.Text =
                    string.Format(
                    "{0} {1} {2}",
                    e.pageX.ToString("###.##"),
                    e.pageY.ToString("###.##"),
                    unitName);

            }
            catch (Exception ex)
            {
                Common.UtilityClass.DoOnError(ex);

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                    this, Properties.Resources.MainForm_ErrorWhenMapControlMouseMove);
            }
        }

        /// <summary>
        /// �V�K
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuNewDoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckDocumentSaveOrNot())
                {
                    // 2012/08/24 add >>>>>
                    // �J���Ă���W�I���t�@�����X�t�H�[������Ε���
                    if (this.OwnedForms.Length > 0)
                    {
                        for (int i = 0; i < this.OwnedForms.Length; i++)
                        {
                            if (this.OwnedForms[i].Text.Contains("�W�I���t�@�����X"))
                            {
                                this.OwnedForms[i].Dispose();
                                break;
                            }
                        }
                    }
                    // 2012/08/24 add <<<<<

                    ClearPageLayout();

	                // ���O�ɊJ���Ă����޷���Ă�������ݒ��Ԃɂ���ẮA�����������ݒ�ɖ߂�Ȃ��s��ɑΉ�
                    this.InitPrintSettings();

                    ClearMap();

                    SetEvents();
                    SetPageLayoutEvents();

                    ClearMapChangeCount();
                }
            }
            catch (Exception ex)
            {
                Common.Logger.Error(sender.ToString() + " " + ex.Message);
            }
        }

        /// <summary>
        /// �����e�[�u���\����Ԃ��擾
        /// </summary>
        /// <returns>�����e�[�u���\����Ԃł���ꍇ�ɂ�true</returns>
        internal bool HasFormAttributeTable()
        {
            if (this.OwnedForms.Length > 0)
            {
                for (int i = 0; i < this.OwnedForms.Length; i++)
                {
                    if (this.OwnedForms[i].Text.Contains("����:"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 2012/08/07 ADD
        /// <summary>
        /// �W�I���t�@�����X�@�\���s��Ԃ��擾
        /// </summary>
        /// <returns>�W�I���t�@�����X�@�\���s��Ԃł���ꍇ�ɂ�true</returns>
        internal bool HasGeoReference()
        {
            bool retrunVal;
            retrunVal = false;
            if (this.OwnedForms.Length > 0)
            {
                for (int i = 0; i < this.OwnedForms.Length; i++)
                {
                    if (this.OwnedForms[i].Text.Contains("�W�I���t�@�����X"))
                    {
                        retrunVal = true;
                        break;
                    }
                }
            }
            return retrunVal;
        }
        // 2012/08/07 ADD

        /// <summary>
        /// �I�����j���[�؂�ւ�
        /// </summary>
        private void toolMenuSentaku_DropDownOpening(object sender, EventArgs e)
        {
            if (HasFormAttributeTable())
            {
                menuSentakuZokukeiKensaku.Enabled = false;
                menuSentakuKukanKensaku.Enabled = false;
                menuSentakuSelectableLayerSettings.Enabled = false;
            }
            else if(!this.IsMapVisible) {
                menuSentakuZokukeiKensaku.Enabled = false;
                menuSentakuKukanKensaku.Enabled = false;
                menuSentakuZokuseichiSyukei.Enabled = false;
                menuSentakuSelectableLayerSettings.Enabled = false;
            }
        }

        /// <summary>
        /// �e�[�u�������ƃ����[�g���j���[�؂�ւ�
        /// </summary>
        private void toolMenuTableJoinRelate_DropDownOpening(object sender, EventArgs e)
        {
            if (HasFormAttributeTable())
            {
                menuTableJoin.Enabled = false;
                menuRemoveJoin.Enabled = false;
                menuRelate.Enabled = false;
                menuRemoveRelate.Enabled = false;
            }
            else if(!this.IsMapVisible) {
                menuTableJoin.Enabled = false;
                menuRemoveJoin.Enabled = false;
                menuRelate.Enabled = false;
                menuRemoveRelate.Enabled = false;
            }
        }

        /// <summary>
        /// ���Z���j���[�̐؂�ւ�
        /// </summary>
        private void toolMenuCalculate_DropDownOpening(object sender, EventArgs e)
        {
            if (HasFormAttributeTable()) {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
                menuGeoReference.Enabled = false;
            }
            else if(!this.IsMapVisible) {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
                menuGeoReference.Enabled = false;
            }
            else if(this.IsEditMode) {
                menuGeometryCalculate.Enabled = false;
                menuFieldCalculate.Enabled = false;
                menuIntersect.Enabled = false;
                menuGeoReference.Enabled = false;
            }
			else if(this.HasGeoReference()) {
                menuGeoReference.Enabled = false;
			}
			else {
			    int					count_R = 0;	// ׽���ڲ԰��
			    Common.LayerManager	pLayerManager = new GISLight10.Common.LayerManager();

			    // ׽���̶���
			    count_R = pLayerManager.GetRasterLayers(m_MapControl.Map).Count;
			    menuGeoReference.Enabled = count_R > 0;
			}
        }

        /// <summary>
        /// �ҏW�I�v�V�����̐ݒ�
        /// </summary>
        private void OnStartEditingMethod() {

            EditOptionSettings m_editOptionSettings = null;
            IEngineEditProperties m_engineEditProp = (IEngineEditProperties)m_EngineEditor;
            IEngineEditProperties2 m_engineEditProp2 = (IEngineEditProperties2)m_EngineEditor;
            IEngineSnapEnvironment m_engineSnapEnv = (IEngineSnapEnvironment)m_EngineEditor;

            //�ݒ�t�@�C�������݂��邩�m�F����
            if (!ApplicationInitializer.IsUserSettingsExists()) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                return;
            }

            try {
                //�ݒ�t�@�C��
                m_editOptionSettings = new EditOptionSettings();
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �ҏW�̃I�v�V���� ]" +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �ҏW�̃I�v�V���� ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }

            //�X�i�b�v���e�l��ݒ�
            try {
                m_engineSnapEnv.SnapTolerance = double.Parse(m_editOptionSettings.SnapTolerance);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �X�i�b�v���e�l ]" +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �X�i�b�v���e�l ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }


            //�ړ��}�����e�l��ݒ�
            try {
                m_engineEditProp2.StickyMoveTolerance = int.Parse(m_editOptionSettings.StickyMoveTolerance);
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �ړ��}�����e�l ]" +
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �ړ��}�����e�l ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }


            //�X�g���[�����e�l��ݒ�
            try {
                m_engineEditProp.StreamTolerance = double.Parse(m_editOptionSettings.StreamTolerance);
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                   (this,
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                   "[ �X�g���[�� ���[�h�F�X�g���[�����e�l ]" +
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �X�g���[�� ���[�h�F�X�g���[�����e�l ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }


            //�O���[�v�����钸�_����ݒ�
            try {
                m_engineEditProp.StreamGroupingCount = int.Parse(m_editOptionSettings.StreamGroupingCount);
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                   (this,
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                   "[ �X�g���[�� ���[�h�F�O���[�v�����钸�_�� ]" +
                   Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                    "[ �X�g���[�� ���[�h�F�O���[�v�����钸�_�� ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return;
            }



        }

#if DEBUG
        private void OnEditorSelectionChanged() {
			Debug.WriteLine("����INFO - EditorEvent : SelectionChanged.");
        }
#endif

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // 2011/06/06 �ҏW���̓��C�A�E�g�؂�ւ��}�~����
            if(this.IsMapVisible) return;

            if(this.IsEditMode) {
                // �؂�ւ��s��
                this.IsMapVisible = true;
                MessageBoxManager.ShowMessageBoxWarining(this, Properties.Resources.MainFrom_WARNING_TabChange);
            }

            // 2012/08/24 �W�I���t�@�����X�N�����͐؂�ւ��}�~����
            if(HasGeoReference()) {
                // �؂�ւ��s��
                this.IsMapVisible = true;
                MessageBoxManager.ShowMessageBoxWarining(this, Properties.Resources.FormGeoReference_WARNING_ChangeLayoutView);
            }
        }

        /// <summary>
        /// ���t���b�V���{�^��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshButton_Click(object sender, EventArgs e)
        {
            try {
                ICommand mapRefresh = new ControlsMapRefreshViewCommandClass();

                mapRefresh.OnCreate(this.axMapControl1.Object);
                mapRefresh.OnClick();

                //this.axMapControl1.Refresh();
                //this.axPageLayoutControl1.Refresh();
            }
            catch (Exception ex)
            {
                Common.Logger.Error(sender.ToString() + " " + ex.Message);
            }
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)�@{
            // ϳ�����ق�ύX
            if(this.Cursor != Cursors.SizeAll) {
				preCursor = this.Cursor;
				this.Cursor = Cursors.SizeAll;
            }

            // ػ��ގ��̺���`��X�V���y������
            if(this.IsMapVisible) {
				axMapControl1.SuppressResizeDrawing(true, this.Handle.ToInt32());
            }
            else {
				this.axPageLayoutControl1.SuppressResizeDrawing(true, this.Handle.ToInt32());
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)�@{
            //Stop bitmap draw and draw data
            if(this.IsMapVisible) {
				axMapControl1.SuppressResizeDrawing(false, this.Handle.ToInt32());
            }
            else {
				this.axPageLayoutControl1.SuppressResizeDrawing(false, this.Handle.ToInt32());
            }

            this.Cursor = preCursor;
        }

        private void tlbtnTocClose_ButtonClick(object sender, EventArgs e)
        {
            this.MapContainer.Panel1Collapsed = true;
            this.tlbtnTocClose.Visible = false;
            this.tlbtnTocExpand.Visible = true;
        }

        private void tlbtnTocExpand_Click(object sender, EventArgs e)
        {
            this.MapContainer.Panel1Collapsed = false;
            this.tlbtnTocClose.Visible = true;
            this.tlbtnTocExpand.Visible = false;
        }

        private esriControlsDragDropEffect m_Effect;

        /// <summary>
        /// MapControl�ւ̃h���b�v
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnOleDrop(object sender, IMapControlEvents2_OnOleDropEvent e) {
            IDataObjectHelper dataObject = (IDataObjectHelper)e.dataObjectHelper;
            esriControlsDropAction action = e.dropAction;

            e.effect = (int)esriControlsDragDropEffect.esriDragDropNone;

            if (action == esriControlsDropAction.esriDropEnter)
            {
                //if (dataObject.CanGetFiles() | dataObject.CanGetNames())
                //{
                m_Effect = esriControlsDragDropEffect.esriDragDropCopy;
                //}
                //e.effect = (int)esriControlsDragDropEffect.esriDragDropCopy;
                return;
            }

            if (action == esriControlsDropAction.esriDropOver)
            {
                e.effect = (int)m_Effect;
                //e.effect = (int)esriControlsDragDropEffect.esriDragDropCopy;
                return;
            }

            if(action == esriControlsDropAction.esriDropped) {
#if DEBUG
				Debug.WriteLine("���h���b�v / CanGetFiles : " + (dataObject.CanGetFiles() ? "YES" : "NO"));
#endif
				// ̧�٥���т�����ۯ�߂��ꂽ�AMXD���J����悤�Ɋg��
				if(dataObject.CanGetFiles()) {
					bool	blnOpened = false;
					System.Array	arrFiles = System.Array.CreateInstance(typeof(string), 0);
					arrFiles = (System.Array)dataObject.GetFiles();
					foreach(string strFile in arrFiles) {
						if(m_MapControl.CheckMxFile(strFile)) {
							if(this.CheckDocumentSaveOrNot()) {
								// ϯ���޷���Ă��J��
								this.OpenMXDFile(strFile);
							}
							blnOpened = true;	// �L����MXD�ɑΉ������A�Ƃ����Ӗ��ŁB
							break;
						}
					}
					if(!blnOpened) {
						// ү���ނ�\��
						ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.MainForm_ErrorWhenDataAdd);
					}
				}
                // �ް���ϯ�߂ɒǉ�
				else {
					IDataObject dataObj = null;
					try {
						// ��۸ޥ�ذ����̂��ް����󂯕t����
						dataObj = (IDataObject)dataObject.InternalObject;
					}
					catch {
						ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
						(this, Properties.Resources.MainForm_ErrorWhenDataAdd);
						return;
					}

#if DEBUG
	                System.Diagnostics.Debug.WriteLine("���h���b�v / GetDataPresent : " + dataObj.GetType().ToString());
#endif
					if(dataObj.GetDataPresent("ESRI.ArcGIS.Geodatabase.IDatasetName")) {
						try {
							ESRI.ArcGIS.Geodatabase.IDatasetName dsName = (ESRI.ArcGIS.Geodatabase.IDatasetName)dataObj.GetData("ESRI.ArcGIS.Geodatabase.IDatasetName");
							//MessageBox.Show(dsName.Name);
							ESRI.ArcGIS.esriSystem.IName name = (IName)dsName;
							//ESRI.ArcGIS.Geodatabase.IWorkspaceName wsName;
							//ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wsFact;

							switch (dsName.Type) {
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureDataset: // CAD or FeatureDataset
								if(dsName.WorkspaceName.WorkspaceFactoryProgID == "esriDataSourcesFile.CadWorkspaceFactory.1") {
									//addCadLayer(dsName);
									addCadGroupLayer(dsName);
								}
								else {
									addFDatasetLayer(dsName);
								}
								break;
							//case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTCadDrawing:
							//    ESRI.ArcGIS.DataSourcesFile.ICadDrawingDataset cadDataset
							//        = (ESRI.ArcGIS.DataSourcesFile.ICadDrawingDataset)name.Open();
							//    if (cadDataset == null)
							//        return;
							//    ICadLayer cadLayer = new CadLayerClass();
							//    cadLayer.CadDrawingDataset = cadDataset;
							//    cadLayer.Name = dsName.Name;
							//    this.MapControl.AddLayer(cadLayer, 0);
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass:
								addFeatureLayer(dsName);
								break;
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterCatalog:
								addRasterCatalog(dsName);
								break;
							case ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset:
								addRasterLayer(dsName);
								break;
							case esriDatasetType.esriDTTable:
								// ð��ق�ǉ����āA����ǉ� ����� ��@��
								this.OnActiveViewEventsItemAdded( this.addTableLayer(dsName) );
								break;
							default:
								break;
							}
						}
						catch(Exception ex) {
							ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
								(this, Properties.Resources.MainForm_ErrorWhenLayerAdd);
							return;
						}
					}
					else if(dataObj.GetDataPresent("ESRI.ArcGIS.esriSystem.IFileName")) {
						try {
							ESRI.ArcGIS.esriSystem.IFileName fileName = (ESRI.ArcGIS.esriSystem.IFileName)dataObj.GetData("ESRI.ArcGIS.esriSystem.IFileName");
							string	strExt = Path.GetExtension(fileName.Path).ToLower();
							if(strExt.Equals(".lyr")) {
								addLayerFile(fileName);
							}
							else if(strExt.Equals(".mxd")) {
								if(m_MapControl.CheckMxFile(fileName.Path)) {
									if(this.CheckDocumentSaveOrNot()) {
										// ϯ���޷���Ă��J��
										this.OpenMXDFile(fileName.Path);
									}
								}
								else {
									// ү���ނ�\��
									ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(this, Properties.Resources.MainForm_ErrorWhenDataAdd);
								}
							}
						}
						catch(Exception ex) {
							ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
								(this, Properties.Resources.MainForm_ErrorWhenLayerAdd);
						}
					}
					// �ް��ް��ð���
					else if(dataObj.GetDataPresent("ESRIJapan.GISLight10.Common.UserSelectQueryTableSet")) {
						Common.UserSelectQueryTableSet	objDB = dataObj.GetData("ESRIJapan.GISLight10.Common.UserSelectQueryTableSet") as Common.UserSelectQueryTableSet;
						// ð��ق�ǉ����āA����ǉ� ����� ��@��
						this.OnActiveViewEventsItemAdded( this.addDBTableLayer(objDB) );
					}
				}
            }
        }


        /// <summary>
        /// FeatureLayer�̒ǉ�
        /// </summary>
        /// <param name="dsName"></param>
        private void addFeatureLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IFeatureWorkspace featWs = null;
            IFeatureClass featClass = null;

            try
            {

                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                featWs = (IFeatureWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                featClass = featWs.OpenFeatureClass(dsName.Name);

                string name = featClass.AliasName;
                if (name.Length == 0)
                {
                    name = dsName.Name;
                }

                IFeatureLayer featLayer = null;
                if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    IFeatureDataset featDataset = featClass.FeatureDataset;
                    IAnnotationLayerFactory annoLayerFact = new FDOGraphicsLayerFactoryClass();
                    IAnnotationLayer annoLayer = annoLayerFact.OpenAnnotationLayer(featWs, featDataset, dsName.Name);
                    featLayer = (IFeatureLayer)annoLayer;
                }
                else
                {
                    featLayer = new FeatureLayerClass();
                }

                featLayer.FeatureClass = featClass;
                featLayer.Name = name;

                this.m_MapControl.AddLayer(featLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, featLayer, m_MapControl.ActiveView.Extent.Envelope);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (featWs != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(featWs);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// CadLayer�̒ǉ�
        /// </summary>
        /// <param name="dsName"></param>
        private void addCadLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            ICadDrawingWorkspace cadWs = null;
            ICadDrawingDataset cadDs = null;
            ICadLayer cadLayer = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                cadWs = (ICadDrawingWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                cadDs = cadWs.OpenCadDrawingDataset(dsName.Name);
                cadLayer = new CadLayerClass();
                cadLayer.CadDrawingDataset = cadDs;
                cadLayer.Name = dsName.Name;
                this.m_MapControl.AddLayer(cadLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, cadLayer, m_MapControl.ActiveView.Extent.Envelope);

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (cadWs != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(cadWs);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }

        }

        /// <summary>
        /// CadLayer���O���[�v���C���Ƃ��Ēǉ�
        /// </summary>
        /// <param name="dsName"></param>
        private void addCadGroupLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IFeatureWorkspace fws = null;
            IFeatureDataset featDs = null;
            IGroupLayer gLayer = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                fws = (IFeatureWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                featDs = fws.OpenFeatureDataset(dsName.Name);
                gLayer = new GroupLayerClass();
                gLayer.Name = dsName.Name;

                IEnumDataset enumDs = featDs.Subsets;
                enumDs.Reset();
                IDataset ds = enumDs.Next();
                while (ds != null)
                {
                    if (ds is IFeatureClass)
                    {
                        IFeatureLayer cadFeatLayer = new FeatureLayerClass();
                        cadFeatLayer.FeatureClass = (IFeatureClass)ds;
                        string name = ((IFeatureClass)ds).AliasName;
                        if (name.Length == 0)
                        {
                            name = dsName.Name;
                        }
                        cadFeatLayer.Name = name;
                        gLayer.Add(cadFeatLayer);
                    }

                    ds = enumDs.Next();
                }

                this.m_MapControl.AddLayer(gLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, gLayer, m_MapControl.ActiveView.Extent.Envelope);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (fws != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(fws);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// FeatureDataset���̃t�B�[�`���N���X�����ꂼ�ꃌ�C���Ƃ��Ēǉ�
        /// </summary>
        /// <param name="dsName"></param>
        private void addFDatasetLayer(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IFeatureWorkspace fws = null;
            IFeatureDataset featDs = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                fws = (IFeatureWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                featDs = fws.OpenFeatureDataset(dsName.Name);

                IEnumDataset enumDs = featDs.Subsets;
                enumDs.Reset();
                IDataset ds = enumDs.Next();
                while (ds != null)
                {
                    if (ds is IFeatureClass)
                    {
                        IFeatureClass featClass = (IFeatureClass)ds;
                        IFeatureLayer featLayer = null;
                        if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            IAnnotationLayerFactory annoLayerFact = new FDOGraphicsLayerFactoryClass();
                            IAnnotationLayer annoLayer = annoLayerFact.OpenAnnotationLayer(fws, featDs, ds.Name);
                            featLayer = (IFeatureLayer)annoLayer;
                        }
                        else
                        {
                            featLayer = new FeatureLayerClass();
                        }

                        featLayer.FeatureClass = featClass;//(IFeatureClass)ds;

                        string name = ((IFeatureClass)ds).AliasName;
                        if (name.Length == 0)
                        {
                            name = dsName.Name;
                        }
                        featLayer.Name = name;
                        this.m_MapControl.AddLayer(featLayer, 0);
                        this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, featLayer, m_MapControl.ActiveView.Extent.Envelope);
                    }

                    ds = enumDs.Next();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (fws != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(fws);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }

        }

        /// <summary>
        /// �� RasterLayer�̒ǉ�
        /// </summary>
        /// <param name="dsName"></param>
        private void addRasterLayer(IDatasetName dsName) {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IRasterLayer rasLayer = null;
            IRasterWorkspace rasWs = null;
            IRasterWorkspaceEx rasWsEx = null;

            try {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;

                if(wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesRaster.RasterWorkspaceFactory")) {
                    rasWs = (IRasterWorkspace)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                    rasLayer = new RasterLayerClass();
                    IRasterDataset rasDs = rasWs.OpenRasterDataset(dsName.Name);
                    //IRaster raster = rasDs.CreateDefaultRaster();
                    //rasLayer.CreateFromDataset(rasWs.OpenRasterDataset(dsName.Name));
                    //rasLayer.CreateFromRaster(raster);
                    rasLayer.CreateFromDataset(rasDs);
                }
                else if (wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.AccessWorkspaceFactory") ||
                    wsName.WorkspaceFactoryProgID.Contains("esriDataSourcesGDB.FileGDBWorkspaceFactory"))
                {
                    rasWsEx = (IRasterWorkspaceEx)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                    rasLayer = new RasterLayerClass();
                    //rasLayer.CreateFromDataset(rasWsEx.OpenRasterDataset(dsName.Name));
                    IRasterDataset rasDsEx = rasWsEx.OpenRasterDataset(dsName.Name);
                    //IRaster rasterEx = rasDsEx.CreateDefaultRaster();
                    //rasLayer.CreateFromRaster(rasterEx);
                    rasLayer.CreateFromDataset(rasDsEx);
                }
                else {
                    throw new Exception();
                }

                if(rasLayer != null) {
                    this.m_MapControl.AddLayer(rasLayer, 0);
                    this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, rasLayer, m_MapControl.ActiveView.Extent.Envelope);
                }
            }
            catch (Exception ex) {
                throw;
            }
            finally {
                if (rasWsEx != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rasWsEx);
                if (rasWs != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rasWs);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// GDB����RasterCatalog�̒ǉ�
        /// </summary>
        /// <param name="dsName"></param>
        private void addRasterCatalog(IDatasetName dsName)
        {
            IWorkspaceName wsName = null;
            IWorkspaceFactory wsFact = null;
            IRasterWorkspaceEx rasWsEx = null;
            IRasterCatalog rasCatalog = null;
            IGdbRasterCatalogLayer rasCatalogLayer = null;

            try
            {
                wsName = dsName.WorkspaceName;
                wsFact = wsName.WorkspaceFactory;
                rasWsEx = (IRasterWorkspaceEx)wsFact.OpenFromFile(wsName.PathName, this.Handle.ToInt32());
                rasCatalog = rasWsEx.OpenRasterCatalog(dsName.Name);
                rasCatalogLayer = new GdbRasterCatalogLayerClass();
                rasCatalogLayer.Setup((ITable)rasCatalog);
                this.m_MapControl.AddLayer((ILayer)rasCatalogLayer, 0);
                this.m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, rasCatalogLayer, m_MapControl.ActiveView.Extent.Envelope);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (rasWsEx != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rasWsEx);
                //if (wsFact != null)
                //    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsFact);
                if (wsName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(wsName);
            }
        }

        /// <summary>
        /// LayerFile�̒ǉ�
        /// </summary>
        /// <param name="fileName"></param>
        private void addLayerFile(IFileName fileName) {
            //if(System.IO.File.Exists(fileName.Path))
                this.m_MapControl.AddLayerFromFile(fileName.Path, 0);
        }


        /// <summary>
        /// �e�[�u���E���C���[��ǉ����܂�
        /// </summary>
        /// <param name="agDSName"></param>
        /// <returns></returns>
        private IStandaloneTable addTableLayer(IDatasetName agDSName) {
			IStandaloneTable	agStdTbl = null;
			IWorkspaceName		agWSName = null;
			IWorkspace			agWS = null;
			ITable				agTbl;

            try {
				// ܰ���߰����擾
                agWSName = agDSName.WorkspaceName;
                IWorkspaceFactory	agWSF = agWSName.WorkspaceFactory;
                agWS = agWSF.OpenFromFile(agWSName.PathName, this.Handle.ToInt32());

                if(agWS is ISqlWorkspace) {
					ISqlWorkspace				agSqlWS = agWS as ISqlWorkspace;
					Common.QueryTableOperator	clsQTO = new QueryTableOperator(agSqlWS);
					agTbl = clsQTO.GetTable(null);
				}
				else {
					IFeatureWorkspace	agFWS = agWS as IFeatureWorkspace;
					agTbl = agFWS.OpenTable(agDSName.Name);
                }

				// ð��ق�ǉ�
				agStdTbl = this.AddStandaloneTable(agTbl);
                if(agStdTbl == null) {
					Common.MessageBoxManager.ShowMessageBoxError("�e�[�u����ǉ��ł��܂���ł����B");
                }
            }
            catch(Exception ex) {
                throw ex;
            }
            finally {
                if(agWS != null) {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agWS);
                }
                if(agWSName != null) {
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agWSName);
                }
            }

            return agStdTbl;
        }

        // �����̒P�ƃe�[�u���̗L�����m�F���܂�
        private bool IsExistStandaloneTable(string TableName) {
			bool	blnRet = false;

			IStandaloneTableCollection	agStdTblColl = m_MapControl.Map as IStandaloneTableCollection;
			if(agStdTblColl.StandaloneTableCount > 0) {
				IStandaloneTable agStdTbl = null;
				for(int intCnt=0; intCnt < agStdTblColl.StandaloneTableCount; intCnt++) {
					agStdTbl = agStdTblColl.get_StandaloneTable(intCnt);
					if(agStdTbl.Name.Equals(TableName)) {
						blnRet = true;
						break;
					}
				}
			}

			return blnRet;
        }

        // �P�ƃe�[�u����ǉ����܂�
        private IStandaloneTable AddStandaloneTable(ITable TargetTable) {
			// ���ް۰��
			return Common.StandAloneTableOpener.AddStandaloneTable(TargetTable, m_MapControl.Map);
        }

        /// <summary>
        /// �N�G�� �e�[�u����ǉ����܂�
        /// </summary>
        /// <param name="DBTableSet"></param>
        /// <returns></returns>
        private IStandaloneTable addDBTableLayer(Common.UserSelectQueryTableSet DBTableSet) {
			IStandaloneTable	agStdTbl = null;

			// DB�ڑ�
			//ISqlWorkspace	agSQLWS = ConnectionFilesManager.LoadWorkspace(DBTableSet.ConnectProperty) as ISqlWorkspace;
			ISqlWorkspace	agSQLWS = ConnectionFilesManager.LoadWorkspace(DBTableSet.ConnectionFile) as ISqlWorkspace;
			if(agSQLWS != null) {
				// հ�ް�ɷ��̨���ނ��w�肵�Ă��炤
				Ui.FormAddQueryLayer	formAQLayer = new FormAddQueryLayer(agSQLWS) {
					TableName = DBTableSet.TableName,
				};
				if(formAQLayer.ShowDialog() == DialogResult.OK) {
					//// ð��ق̊T�v���擾
					//IQueryDescription	agQDesc = agSQLWS.GetQueryDescription(formAQLayer.QueryString);
					//// ���̨���ނ�ݒ�
					//agQDesc.OIDFields = string.Join(",", formAQLayer.OIDFields);

					//// ð��ٖ��𒲐�
					//string	strTblName = DBTableSet.TableName;
					////agSQLWS.CheckDatasetName(strTblName, agQDesc, out strTblName);

					//// ð��ق�ǉ�
					//agStdTbl = this.AddStandaloneTable(agSQLWS.OpenQueryClass(strTblName, agQDesc));
					agStdTbl = this.AddStandaloneTable(formAQLayer.GetQueryTable(null));
					if(agStdTbl == null) {
						Common.MessageBoxManager.ShowMessageBoxError("�e�[�u����ǉ��ł��܂���ł����B");
					}
				}

				ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(agSQLWS);
			}

			return agStdTbl;
        }

		/// <summary>
		/// �}�b�v�̃y�[�W�T�C�Y�\�L���擾���܂�
		/// </summary>
		/// <param name="PageFormID"></param>
		/// <returns></returns>
		public string GetPageSizeDescript(esriPageFormID PageFormID) {
			string	strRet = "";

			// ������̗p���ݒ���g�p ��
			if(PageFormID == esriPageFormID.esriPageFormSameAsPrinter) {
				// �����������̗p�����ނ��擾
				strRet = this.axPageLayoutControl1.Printer.Paper.FormName;
			}
			else {
				// ���ޕ\�L��������
				this.InitPageSizes();

				// ���ޕ\�L���擾
				int	intFormID = PageFormID.GetHashCode();
				if(this._dicPageSizes.ContainsKey(intFormID)) {
					strRet = this._dicPageSizes[intFormID];
				}
				else {
					strRet = " ? ";
				}
			}

			return strRet;
		}

		/// <summary>
		/// �v�����g�ݒ�i�}�b�v�E�y�[�W�j�����ʂ��܂�
		/// </summary>
		public void LoadPrintSetting(IPageLayout agPageLayout) {
			string	strToolTipPre = "";

			// �߰�ސݒ���擾
			IPage	agPage;
			if(agPageLayout != null) {
				agPage = agPageLayout.Page;
			}
			else {
				agPage = this.axPageLayoutControl1.PageLayout.Page;
			}

			// ������̗p���ݒ���g�p
			if(agPage.FormID == esriPageFormID.esriPageFormSameAsPrinter) {
				strToolTipPre = "�p��";

				// �p�����ނ�����
				IPaper	agPaper = this.axPageLayoutControl1.Printer.Paper;
				//this.labelPageSize.Text = this.GetPageSizeDescript(agPage.FormID);
				this.labelPageSize.Text = agPaper.FormName;

				// �p������������
				if(agPaper.Orientation == 2) {
					this.labelPageOrientation.Text = this.pageLandscape;
				}
				else {
					this.labelPageOrientation.Text = this.pagePortrait;
				}
			}
			// հ�ް���èݸ�
			else {
				strToolTipPre = "�y�[�W";

				// ϯ�߂��߰�޻��ނ�\��
				this.labelPageSize.Text = this.GetPageSizeDescript(agPage.FormID);

				// �߰�ނ̌���
				if(agPage.Orientation == 2) {
					this.labelPageOrientation.Text = this.pageLandscape;
				}
				else {
					this.labelPageOrientation.Text = this.pagePortrait;
				}
			}

			// °����߂�\��
			this.toolTip1.SetToolTip(this.labelPageSize, strToolTipPre + "�T�C�Y : " + this.labelPageSize.Text);
			this.toolTip1.SetToolTip(this.labelPageOrientation, strToolTipPre + "�̌��� : " + this.labelPageOrientation.Text);
		}

		/// <summary>
		/// �v�����^�ݒ�𕜌����܂�
		/// </summary>
		/// <param name="DocPrinter"></param>
		/// <returns></returns>
		public bool LoadPrintDocument(IPrinter DocPrinter) {
			bool	blnRet = true;

			try {
				// ڲ��Ă��������ݒ�
				this.axPageLayoutControl1.Printer = DocPrinter;

				// ������ݒ��۰��
				IPaper	agPaper = DocPrinter.Paper;

				// ����ĥ�޷���Ă̐ݒ�
				this.document.PrinterSettings.PrinterName = agPaper.PrinterName;
				this.document.DefaultPageSettings.Landscape = agPaper.Orientation == 2;

				// �p�����ނ�T��
				PaperSize	psDef = null;
				foreach(PaperSize ps in this.document.PrinterSettings.PaperSizes) {
					if(ps.PaperName.Equals(agPaper.FormName)) {
						psDef = ps;
						break;
					}
				}
				if(psDef != null) {
					this.document.DefaultPageSettings.PaperSize = psDef;
				}
			}
			catch(Exception ex) {
				blnRet = false;
				//GISLight10.Common.MessageBoxManager.ShowMessageBoxError("�ۑ�����Ă���v�����^�̐ݒ肪���s���܂����B");
			}

			return blnRet;
		}

		/// <summary>
		/// �N�����Ɏw�肳�ꂽ�h�L�������g�E�t�@�C����W�J���܂�
		/// </summary>
		/// <param name="DocFilePath"></param>
		/// <returns></returns>
		private bool OpenMXDFile(string DocFilePath) {
			bool	blnRet = false;

            for(int intCnt=0; intCnt < m_ToolbarControl.Count; intCnt++) {
                IToolbarItem ti = m_ToolbarControl.GetItem(intCnt);

                // MXD����ݥ����ނ�T��
                if(ti.Command.GetType().ToString() == @"ESRIJapan.GISLight10.EngineCommand.OpenDocument") {
                    var cmdOpemDoc = ti.Command as EngineCommand.OpenDocument;
                    if(cmdOpemDoc.OpenDocFile(DocFilePath, this.MapControl as IMapControl3)) {
						blnRet = true;
					}
					else {
                        // �װ�\��
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                            (this, "�h�L�������g���J���܂���B");
                    }
                    break;
                }
            }

            return blnRet;
		}

		/// <summary>
		/// �w��̃c�[����L���ɂ��܂� (ToolBar1 ONLY)
		/// </summary>
		public void SetElementSelectTool(string ToolName) {
			// �Ώ�°ق��擾 (°��ް1)
			if(m_ToolbarControl != null) {
				// ���̨���I��°ق��N��
				string	strCurToolName = ToolName;
				for(int intCnt=0; intCnt < m_ToolbarControl.Count; intCnt++) {
					var agTool = m_ToolbarControl.GetItem(intCnt);
					if(agTool.Command != null && agTool.Command.Name.Equals(strCurToolName)) {
						// Map
						if(this.IsMapVisible) {
							this.m_MapControl.CurrentTool = agTool.Command as ITool;
						}
						// PageLayout
						else {
							this.m_pageLayoutControl.CurrentTool = agTool.Command as ITool;
						}
					}
				}
			}
		}
		/// <summary>
		/// �G�������g�I���c�[����ݒ肵�܂�
		/// </summary>
		public void SetElementSelectTool() {
			// ���ް۰��
			this.SetElementSelectTool("ControlToolsGraphicElement_SelectTool");
		}


        // 2020/03/17
        /// <summary>
        /// ActiveX Control �̃��T�C�Y
        /// Workaround for controls display issues in 96 versus 120 dpi
        /// https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#Controls92DPIWorkaround.htm
        /// </summary>
        /// <param name="controlToAdjust"></param>
        private void AdjustBounds(AxHost controlToAdjust)
        {
            if (this.CurrentAutoScaleDimensions.Width != 6F)
            {
                //Adjust location: ActiveX control doesn't do this by itself 
                controlToAdjust.Left = Convert.ToInt32(controlToAdjust.Left * this.CurrentAutoScaleDimensions.Width / 6F);
                //Undo the automatic resize... 
                controlToAdjust.Width = controlToAdjust.Width / DPIX() * 96;
                //...and apply the appropriate resize
                controlToAdjust.Width = Convert.ToInt32(controlToAdjust.Width * this.CurrentAutoScaleDimensions.Width / 6F);
            }

            if (this.CurrentAutoScaleDimensions.Height != 13F)
            {
                //Adjust location: ActiveX control doesn't do this by itself 
                controlToAdjust.Top = Convert.ToInt32(controlToAdjust.Top * this.CurrentAutoScaleDimensions.Height / 13F);
                //Undo the automatic resize... 
                controlToAdjust.Height = controlToAdjust.Height / DPIY() * 96;
                //...and apply the appropriate resize 
                controlToAdjust.Height = Convert.ToInt32(controlToAdjust.Height * this.CurrentAutoScaleDimensions.Height / 13F);
            }
        }

        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;

        int DPIX()
        {
            return DPI(LOGPIXELSX);
        }

        int DPIY()
        {
            return DPI(LOGPIXELSY);
        }

        int DPI(int logPixelOrientation)
        {
            int displayPointer = CreateDC("DISPLAY", null, null, IntPtr.Zero);
            return Convert.ToInt32(GetDeviceCaps(displayPointer, logPixelOrientation));
        }
    }
}