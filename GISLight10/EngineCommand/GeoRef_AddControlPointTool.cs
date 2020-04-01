using System;
using System.Drawing;
using System.Collections.Generic;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ADF.CATIDs;

using System.Runtime.InteropServices;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �W�I���t�@�����X�E�R���g���[���|�C���g�R�}���h�c�[�� 
	/// �E�@�\�Ƃ��ẮA���[�U�[���w�肵�����W���W�I���t�@�����X�E�c�[���t�H�[���ɒʒm����
    /// </summary>
    /// <history>
    ///  2012-08-01 �V�K�쐬 
    /// </history>
    public sealed class GeoRef_AddControlPointTool : ICommand, ITool
    {
        private IMap						_agMap;
        private Ui.FormGeoReference			_frmGR;				// �޵�̧�ݽ�̫��

		[DllImport("gdi32.dll")]
		static extern bool DeleteObject(IntPtr hObject);

		private System.Drawing.Bitmap		_toolBitmap;		// °٥�ޯ�ϯ��
		private IntPtr						_ptrToolBitmap;		// °٥�ޯ�ϯ�߂ւ��߲��
		private IHookHelper					_agHookHelper;		// �߲���Ǘ�
		
		private INewLineFeedback			_agLFeedBack;		// ײݕ`��
		private IElement					_agElm_P;			// �n�_
		
		private Boolean						_blnAddWorking;		// ���۰٥�߲�Ēǉ����׸�
		private System.Windows.Forms.Cursor	_TCursor;			// °َ��s����ϳ������
		
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public GeoRef_AddControlPointTool(Ui.FormGeoReference OwnerForm) {
			// ؿ�������擾
            string	strResourceName = this.GetType().Name + ".bmp";

			// °٥�����è�̐ݒ�
            this._toolBitmap = new Bitmap(this.GetType(), strResourceName);
            this._toolBitmap.MakeTransparent(Color.White);
            _ptrToolBitmap = _toolBitmap.GetHbitmap();
            
            // �޵�̧�ݽ�̫�т��擾
            this._frmGR = OwnerForm;
            
            this._agHookHelper = new HookHelperClass();
		}
        
        /// <summary>
        /// �f�X�g���N�^
        /// </summary>
        ~GeoRef_AddControlPointTool() {
			if(this._ptrToolBitmap.ToInt32() != 0) {
				DeleteObject(this._ptrToolBitmap);
			}
        }

#region ICommand �����o�[

	#region �v���p�e�B

		public string Message {
			get {
				return "�W�I���t�@�����X�E�R���g���[���|�C���g��ǉ����܂�";
			}
		}
		
		public int Bitmap {
			get {
				return _ptrToolBitmap.ToInt32();
			}
		}

		public string Caption {
			get {
				return "�R���g���[���|�C���g�̒ǉ�";
			}
		}

		public string Tooltip {
			get {
				return "�W�I���t�@�����X�E�R���g���[���|�C���g��ǉ����܂�";
			}
		}

		public int HelpContextID {
			get {
				// TODO:  Add GeoRef_AddControlPointTool.HelpContextID getter implementation
				return 0;
			}
		}

		public string Name {
			get {
				return "GeoReference/AddControlPoint";
			}
		}

		public bool Checked {
			get {
				return false;
			}
		}	

		public bool Enabled {
			get {
				if(_agHookHelper.FocusMap == null) return false;

				return true;
			}
		}

		public string HelpFile {
			get {
				// TODO:  Add GeoRef_AddControlPointTool.HelpFile getter implementation
				return null;
			}
		}

		public string Category {
			get {
				return "ESRIJapanGIS_V200";
			}
		}
	#endregion		
        
	#region �C�x���g
        /// <summary>
        /// �C���X�^���X���� �C�x���g
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
        public void OnCreate(object hook) {
			this._agHookHelper.Hook = hook;
			
			// �n�}
            _agMap = this._agHookHelper.FocusMap;

            // ����
            try {
				this._TCursor = new System.Windows.Forms.Cursor(GetType().Assembly.GetManifestResourceStream(GetType(), this.GetType().Name + ".cur"));
			}
			catch(Exception ex) {
				string	strErr = ex.Message;
			}
        }

        /// <summary>
        /// �N���b�N �C�x���g
        /// </summary>
        public void OnClick() {
			// �n�}(Ҳ�̫��)��è�ނɐݒ�
			this._frmGR.SetMapActive();
        }
	#endregion
#endregion

#region ITool �����o�[

#region �v���p�e�B
		/// <summary>
		/// �}�E�X�E�J�[�\�����擾���܂�
		/// </summary>
		public int Cursor {
			get {
				return this._TCursor.Handle.ToInt32();
			}
		}
		
		/// <summary>
		/// ���݁A�R���g���[���|�C���g��ǉ������ǂ����𔻒肵�܂�
		/// </summary>
		public bool IsToolActive {
			get {
				return this._agLFeedBack != null;
			}
		}
#endregion

#region ���J���\�b�h
		public void EndTool() {
			// ��Ʊ��т�ر
			if(this._blnAddWorking) {
				this.CancelAddCtlPoint();
			}
		}
#endregion

#region �C�x���g
		/// <summary>
		/// �}�E�X�E�_�E�� �C�x���g
		/// </summary>
		/// <param name="button"></param>
		/// <param name="shift"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void OnMouseDown(int button, int shift, int x, int y) {
			if(_agHookHelper.ActiveView == null || this._blnAddWorking) return;

			// �߰��ڲ��Ăł��邱�Ƃ͂��肦�Ȃ��̂ł����A�ꉞ���ނ��c���Ă����܂�
			if(_agHookHelper.ActiveView is IPageLayout) {
				// Create a point in map coordinates
				IPoint pPoint = (IPoint)_agHookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);

				//Get the map if the point is within a data frame
				IMap pMap = _agHookHelper.ActiveView.HitTestMap(pPoint);

				if(pMap == null) return;

				//Set the map to be the page layout's focus map
				if(pMap != this._agHookHelper.FocusMap) {
					_agHookHelper.ActiveView.FocusMap = pMap;
					_agHookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
				}
			}

			IActiveView			agActView = (IActiveView)_agHookHelper.FocusMap;

			// �ʒu���L�^ AND �ʒu��\��
			if(this._agElm_P == null) {
				IPoint				agPoint = agActView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
				IGraphicsContainer	agGrpCont = (IGraphicsContainer)agActView;
			
				// �␳�O�ʒu��`��
				this._agElm_P = this.CreateCtlPointElement(agPoint, false);
			
				agGrpCont.AddElement(this._agElm_P, 0);
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, this._agElm_P, agActView.Extent);
			}
		
			// ���۰٥�߲�Ēǉ����׸ނ�ON (0�ړ��łȂ��ꍇ��ϳ�����߂�OFF)
			this._blnAddWorking = true;
		}

		/// <summary>
		/// �}�E�X�E�ړ� �C�x���g
		/// </summary>
		/// <param name="button"></param>
		/// <param name="shift"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void OnMouseMove(int button, int shift, int x, int y) {
			if(!this._blnAddWorking) return;

			IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;

			if(this._agLFeedBack == null) {
				this._agLFeedBack = new NewLineFeedbackClass();
				this._agLFeedBack.Display = agActView.ScreenDisplay;
				this._agLFeedBack.Start((IPoint)this._agElm_P.Geometry);
			}
			this._agLFeedBack.MoveTo(agActView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y));
		}

		/// <summary>
		/// �}�E�X�E�A�b�v �C�x���g
		/// </summary>
		/// <param name="button"></param>
		/// <param name="shift"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void OnMouseUp(int button, int shift, int x, int y) {
			if(!this._blnAddWorking) return;

			// �ړ����Ȃ��ꍇ�͖���
			if(this._agLFeedBack != null) {
				this._agLFeedBack.Stop();
				
				// �␳�ʒu���擾 AND �ʒu��\�� AND �␳�������s
				IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;
				IPoint	agPoint = agActView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
				
				IGraphicsContainer		agGrpCont = (IGraphicsContainer)agActView;
				IElement				agElm_P = this.CreateCtlPointElement(agPoint, true);
			
				// �ʒu�\���̐���
				agGrpCont.DeleteElement(this._agElm_P);
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
				
				// �␳���������s
				this._frmGR.AddControlPointList(this._agElm_P, agElm_P);
				
				// �L�^��j��
				Marshal.ReleaseComObject(this._agElm_P);
				Marshal.ReleaseComObject(this._agLFeedBack);
				this._agElm_P = null;
				this._agLFeedBack = null;

				// ���۰٥�߲�Ă̒ǉ�����
				this._blnAddWorking = false;
			}
		}

		/// <summary>
		/// �L�[�E�_�E�� �C�x���g
		/// </summary>
		/// <param name="keyCode"></param>
		/// <param name="shift"></param>
		public void OnKeyDown(int keyCode, int shift) {
			if(_blnAddWorking) {
				// ESC��(����̷�ݾ�)�ɑΉ�
				if(keyCode == 27) {
					// ���݂̐����j��
					this.CancelAddCtlPoint();
				}
			}
		}

		/// <summary>
		/// �L�[�E�A�b�v �C�x���g
		/// </summary>
		/// <param name="keyCode"></param>
		/// <param name="shift"></param>
		public void OnKeyUp(int keyCode, int shift) {
			//
		}

		public bool OnContextMenu(int x, int y) {
			return false;
		}
		
		/// <summary>
		/// �_�u���E�N���b�N �C�x���g
		/// </summary>
		public void OnDblClick() {
			//
		}		

		/// <summary>
		/// �c�[���񊈐��� �C�x���g
		/// </summary>
		/// <returns></returns>
		public bool Deactivate() {
			// �ǉ����̏ꍇ�́A��ݾُ���
			if(this._blnAddWorking) {
				//this.CancelAddCtlPoint();
				this._agLFeedBack = null;
				
				// ��ʍX�V
				IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics | esriViewDrawPhase.esriViewForeground, null, null);
			}
			
			return true;
		}

		/// <summary>
		/// �n�}�`�� �X�V�C�x���g
		/// </summary>
		/// <param name="hdc"></param>
		public void Refresh(int hdc) {

		}
	#endregion

		/// <summary>
		/// �R���g���[���|�C���g�`��v�f���쐬���܂�
		/// </summary>
		/// <param name="Position">�|�C���g�ʒu</param>
		/// <param name="IsFixed">�␳�� / �␳�O</param>
		/// <returns>�`��v�f</returns>
		private IElement CreateCtlPointElement(IPoint Position, bool IsFixed) {
			IElement	agElm = null;
			
			if(Position != null) {
				// ����ِݒ�
				ISimpleMarkerSymbol		agPntSym = new SimpleMarkerSymbolClass();
				IRgbColor				agRGBColor = new RgbColorClass();
				
				// �F�ݒ�
				if(IsFixed) {
					agRGBColor.Red = 255;
					agRGBColor.Green = 0;
					agRGBColor.Blue = 0;
				}
				else {
					agRGBColor.Red = 0;
					agRGBColor.Green = 255;
					agRGBColor.Blue = 0;
				}
				
				agPntSym.Color = agRGBColor;
				agPntSym.Size = 14;
				agPntSym.Style = esriSimpleMarkerStyle.esriSMSCross;
				
				// �����
				IMarkerElement			agPntElm = new MarkerElementClass();
				agPntElm.Symbol = agPntSym;
				
				agElm = (IElement)agPntElm;
				agElm.Geometry = Position;
				agElm.Locked = true;
			}
			
			// �ԋp
			return agElm;
		}
		
		/// <summary>
		/// �ǉ����̏�Ԃ�j�����܂�
		/// </summary>
		private void CancelAddCtlPoint() {
			IActiveView agActView = (IActiveView)_agHookHelper.FocusMap;

			// ���݂̐����j��
			if(this._agElm_P != null) {
				IGraphicsContainer		agGrpCont = (IGraphicsContainer)agActView;
				agGrpCont.DeleteElement(this._agElm_P);
			}
			
			Marshal.ReleaseComObject(this._agElm_P);
			Marshal.ReleaseComObject(this._agLFeedBack);
			
			this._agElm_P = null;
			this._agLFeedBack = null;
			this._blnAddWorking = false;

			// �n�}��ʍX�V
			agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics | esriViewDrawPhase.esriViewForeground, this._agElm_P, null);
		}
		
		/// <summary>
		/// �w��|�C���g�̕`����������܂�
		/// </summary>
		/// <param name="ControlPointElement"></param>
		private void DeleteCtlPointElement(IElement ControlPointElement) {
			if(ControlPointElement != null) {
				IActiveView			agActView = (IActiveView)_agHookHelper.FocusMap;
				IGraphicsContainer	agGrpCont = (IGraphicsContainer)agActView;

				// �w���߲�Ă̕`����폜
				agGrpCont.DeleteElement(ControlPointElement);

				// �n�}��ʍX�V
				agActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, ControlPointElement, null);
			}
		}
		
		
#endregion
    }
}
