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
    /// �f�[�^�t�@�C���̓ǂݍ��݃R�}���h
    /// </summary>
    /// <history>
    ///  2012-07-10 �V�K�쐬
    /// </history>
    public sealed class ReadDataSource : BaseCommand  
	{
		private IMapControl3	m_mapControl;
        private Ui.MainForm		mainFrm;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
		public ReadDataSource() {
			base.m_caption = "�f�[�^�\�[�X�̓ǂݍ���";
            base.m_toolTip = "CAD �f�[�^�̒ǉ�";
			
			if(base.m_name == "") {
			}
		}
	
        /// <summary>
        /// �N���b�N������
        /// ���C���폜���s
        /// </summary>
		public override void OnClick() {
			OpenFileDialog	openFileDialog = new OpenFileDialog();
			//openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			openFileDialog.Title = "CAD �f�[�^�t�@�C�����J��";
			openFileDialog.Filter = "CAD �f�[�^�t�@�C�� |*.dxf;*.dwg;*.dgn";
			openFileDialog.ShowReadOnly = true;
			openFileDialog.Multiselect = true;
			openFileDialog.CheckPathExists = true;

			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				// ������̧�ّI���ɑΉ�
				foreach(string strCad in openFileDialog.FileNames) {
					try {
						// �n�}�ɓW�J
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
        /// �N���G�C�g������
        /// ���C���t�H�[���ւ̎Q�Ǝ擾
        /// </summary>
        /// <param name="hook">�}�b�v�R���g���[��</param>
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
		}

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {
				return true;
            }
        }
        
        /// <summary>
        /// CAD�f�[�^�Z�b�g���I�u�W�F�N�g���擾���܂�
        /// </summary>
        /// <param name="CadFilePath">CAD�t�@�C���E�p�X</param>
        /// <returns>IDatasetName</returns>
        private IDatasetName GetCadDatasetName(string CadFilePath) {
			IWorkspaceFactory	pWSFact = ESRIJapan.GISLight10.Common.SingletonUtility.NewCadWorkspaceFactory();
			IFeatureWorkspace	pFWS = (IFeatureWorkspace)pWSFact.OpenFromFile(System.IO.Path.GetDirectoryName(CadFilePath), 0);
			IDataset			pDS = (IDataset)pFWS.OpenFeatureClass(System.IO.Path.GetFileName(CadFilePath));
			IWorkspaceName		pWSName = (IWorkspaceName)pDS.FullName;
			
			// �ԋp
			return (IDatasetName)pWSName;
        }
        
        /// <summary>
        /// CAD�h���[�C���O�E���C���[��n�}�ɒǉ����܂�
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

				// CADڲ԰���擾
				ICadLayer		pCadLayer = new CadLayerClass();
				pCadLayer.CadDrawingDataset = pCadDS;
				pCadLayer.Name = System.IO.Path.GetFileName(CadFilePath);
				
				// �n�}�ɓW�J
				m_mapControl.AddLayer(pCadLayer, 0);
			}
			catch(Exception ex) {
				blnRet = false;
				// �װ���e��\��
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(CadFilePath + "�t�@�C���̓ǂݍ��݂Ɏ��s���܂����F" + Environment.NewLine + ex.Message);
			}
			finally {
				// COM���
				if(pCadDS != null) {
					Marshal.ReleaseComObject(pCadDS);
				}
				if(pCadWS != null) {
					Marshal.ReleaseComObject(pCadWS);
				}
			}
			
			// �ԋp
			return blnRet;
        }
        
        /// <summary>
        /// CAD�O���[�v���C���[��n�}�ɒǉ����܂�
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
					
					// �ް���Ė����擾
					strFCName = ((IFeatureClass)pDS).AliasName;
					if(string.IsNullOrEmpty(strFCName)) {
						strFCName = strCadName;
					}
					pFLayer.Name = strFCName;
					
					// ��ٰ�߂ɒǉ�
					pGrpLayer.Add(pFLayer);
					
					// ����
					pDS = pEnumDs.Next();
				}
				
				// �n�}�ɓW�J
				m_mapControl.AddLayer(pGrpLayer, 0);
				
				Marshal.ReleaseComObject(pEnumDs);
			}
			catch(Exception ex) {
				blnRet = false;
				// �װ���e��\��
				ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(CadFilePath + "�t�@�C���̓ǂݍ��݂Ɏ��s���܂����F" + Environment.NewLine + ex.Message);
			}
			finally {
				// COM���
				if(pCadDS != null) {
					Marshal.ReleaseComObject(pCadDS);
				}
				if(pCadWS != null) {
					Marshal.ReleaseComObject(pCadWS);
				}
			}
			
			// �ԋp
			return blnRet;
        }
	}
}


