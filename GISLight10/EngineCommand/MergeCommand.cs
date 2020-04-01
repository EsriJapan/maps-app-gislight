using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRIJapan.GISLight10.Common;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;

using ESRIJapan.GISLight10.EngineEditTask;
using System.Collections;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.ADF;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// �ҏW�@�\�̃}�[�W �R�}���h�̃N���X
    /// </summary>
    /// <history>
    ///  2010-11-01 �V�K�쐬 
    /// </history>
    public sealed class MergeCommand : Common.EjBaseCommand
    {
        //private IHookHelper m_HookHelper;
        //private IMapControl3 m_mapControl;
        //private Ui.MainForm m_mainForm;
        private IEngineEditor m_engineEditor;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public MergeCommand()
        {
            base.captionName = "�}�[�W";
        }

        #region Overriden Class Methods

        /// <summary>
        /// �N���G�C�g������
        /// </summary>
        /// <param name="hook">�c�[���o�[�R���g���[��</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_engineEditor = new EngineEditorClass();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// �N���b�N������
        /// �}�[�W���s
        /// </summary>
        public override void OnClick()
        {            
            // �ҏW�^�[�Q�b�g���C���̃Z���N�V�����Z�b�g���擾
            ISelectionSet selectionSet = GetTargetSelectionSet();            
            
            // �ҏW�^�[�Q�b�g���C���̃t�B�[�`���N���X���擾
            IFeatureClass targetFcClass = GetTargetFeatureClass();

            
            IEnumIDs enumIDs = null;
            IFeature srcFeature = null;
            IFeature destFeature = null;     
            ITopologicalOperator4 srcTopoOperator = null;
            ITopologicalOperator4 destTopoOperator = null;
            IRow destRow = null;
            ISet destRowSet = new SetClass();

            IGeometryBag geometryBag = new GeometryBagClass();
            IGeoDataset geoDataset = (IGeoDataset)targetFcClass;
            geometryBag.SpatialReference = geoDataset.SpatialReference;
            IGeometryCollection geometryCol = (IGeometryCollection)geometryBag;
            object missing = Type.Missing;

            enumIDs = selectionSet.IDs;
            enumIDs.Reset();
            int iD = enumIDs.Next();

            int count = 0;

            try
            {
                //�ҏW�I�y���[�V�����̊J�n�iundo/redo�̎n�_�j
                m_engineEditor.StartOperation();

                GISLight10.Common.Logger.Info("�}�[�W�̕ҏW�I�y���[�V�����F�J�n");

                while (iD != -1)
                {
                    if (count == 0)
                    {
                        //��ڂ̃t�B�[�`��
                        srcFeature = targetFcClass.GetFeature(iD);

                        srcTopoOperator = (ITopologicalOperator4)srcFeature.Shape;
                        srcTopoOperator.IsKnownSimple_2 = false;
                        srcTopoOperator.Simplify();                                                

                        //IGeometryCollection�ɃW�I���g����ǉ�
                        geometryCol.AddGeometry(srcFeature.ShapeCopy, ref missing, ref missing);                        
                    }
                    else
                    {
                        //��ڈȍ~�̃t�B�[�`��
                        //IGeometryCollection�ɃW�I���g���������Ă���
                        destFeature = targetFcClass.GetFeature(iD);

                        destTopoOperator = (ITopologicalOperator4)destFeature.Shape;
                        destTopoOperator.IsKnownSimple_2 = false;
                        destTopoOperator.Simplify();
                        
                        //IGeometryCollection�ɃW�I���g����ǉ�
                        geometryCol.AddGeometry(destFeature.Shape, ref missing, ref missing);

                        //�Ō�ɍ폜����p�Ɋi�[
                        destRow = (IRow)destFeature;
                        destRowSet.Add(destRow);                        
                    }

                    iD = enumIDs.Next();
                    count++;
                }
                

                //�����̕ҏW
                ESRIJapan.GISLight10.Common.Logger.Info("�����ҏW�̎��s");
                MergeAttribute(srcFeature, destRowSet, targetFcClass);

                //�}�[�W�̎��s
                srcTopoOperator.ConstructUnion((IEnumGeometry)geometryCol);

                //�t�B�[�`���̃W�I���g�� �� �}�[�W���ʂ̃W�I���g���ŏ㏑��                
                srcFeature.Shape = (IGeometry)srcTopoOperator;                
                srcFeature.Store();

                //�}�[�W���ꂽ�t�B�[�`���̍폜
                IFeatureEdit destFeatureEdit = (IFeatureEdit)destFeature;
                destFeatureEdit.DeleteSet(destRowSet);

                //�}�[�W��̃t�B�[�`���̃t���b�V��
                IActiveView activeView = m_engineEditor.Map as IActiveView;
                FlashGeometry(srcFeature.Shape, activeView.ScreenDisplay);

                //�ҏW�I�y���[�V�����̏I���iundo/redo�̏I�_�j
                m_engineEditor.StopOperation("merge");

				// �P��ýĥ�m�F����
				//IRow srcRow = (IRow)srcFeature;
				//for(int intCnt=0; intCnt < srcRow.Fields.FieldCount; intCnt++) {
				//    string	strTemp = srcRow.Fields.get_Field(intCnt).Name + " = " + srcRow.get_Value(intCnt).ToString();
				//}

                GISLight10.Common.Logger.Info("�}�[�W�̕ҏW�I�y���[�V�����F����I��");                
            }
            catch (COMException comExc)
            {
                m_engineEditor.AbortOperation();
                GISLight10.Common.Logger.Info("�}�[�W�̕ҏW�I�y���[�V�����F�A�{�[�g");

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(comExc.Message);
                GISLight10.Common.Logger.Error(comExc.StackTrace);
            }
            catch (Exception e)
            {
                m_engineEditor.AbortOperation();
                GISLight10.Common.Logger.Info("�}�[�W�̕ҏW�I�y���[�V�����F�A�{�[�g");

                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(e.Message);
                GISLight10.Common.Logger.Error(e.StackTrace);
            }
            finally
            {
                IActiveView activeView = m_engineEditor.Map as IActiveView;
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography | 
                    esriViewDrawPhase.esriViewGeoSelection, null, activeView.Extent);

                ComReleaser.ReleaseCOMObject(selectionSet);
                ComReleaser.ReleaseCOMObject(targetFcClass);
                ComReleaser.ReleaseCOMObject(srcFeature);
                ComReleaser.ReleaseCOMObject(destFeature);
                ComReleaser.ReleaseCOMObject(srcTopoOperator);
                ComReleaser.ReleaseCOMObject(destTopoOperator);
                ComReleaser.ReleaseCOMObject(destRow);
                ComReleaser.ReleaseCOMObject(destRowSet);
                ComReleaser.ReleaseCOMObject(geometryBag);
                ComReleaser.ReleaseCOMObject(geoDataset);
                ComReleaser.ReleaseCOMObject(geometryCol);               
            }            
        }
        #endregion

        /// <summary>
        /// ���s�\����
        /// </summary>
        public override bool Enabled {
            get {   
                //�ҏW���J�n����Ă���ꍇ
                if(m_engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) {
                    IFeatureLayer targetFcLayer = GetEditTargetLayer();

                    //�ҏW�^�[�Q�b�g���C�����|���S�� ���� ���X�^�J�^���O�łȂ��ꍇ
                    if ((targetFcLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) &&
                        !(targetFcLayer is GdbRasterCatalogLayer))
                    {
                        ISelectionSet selectionSet = GetTargetSelectionSet();

                        //�ҏW�^�[�Q�b�g���C���Ńt�B�[�`����2�ȏ�I������Ă���ꍇ
                        if (selectionSet.Count >= 2) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }                
            }
        }

        /// <summary>
        /// �ҏW�^�[�Q�b�g���C�����擾
        /// </summary>
        /// <returns>�ҏW�^�[�Q�b�g���C��</returns>
        private IFeatureLayer GetEditTargetLayer()
        {
            //�ҏW�^�[�Q�b�g���C���̎擾
            IEngineEditLayers editLayer = (IEngineEditLayers)m_engineEditor;
            IFeatureLayer targetFcLayer = editLayer.TargetLayer;

            return targetFcLayer;
        }

        /// <summary>
        /// �ҏW�^�[�Q�b�g���C���̃Z���N�V�����Z�b�g���擾
        /// </summary>
        /// <returns>�Z���N�V�����Z�b�g</returns>
        private ISelectionSet GetTargetSelectionSet()
        {
            //�ҏW�^�[�Q�b�g���C���̎擾
            IFeatureLayer targetFcLayer = GetEditTargetLayer();

            //�Z���N�V�����Z�b�g�̎擾
            IFeatureSelection fcSelection = (IFeatureSelection)targetFcLayer;
            ISelectionSet selectionSet = fcSelection.SelectionSet;
            
            return selectionSet;
        }

        /// <summary>
        /// �ҏW�^�[�Q�b�g���C���̃t�B�[�`���N���X���擾
        /// </summary>
        /// <returns>�ҏW�^�[�Q�b�g���C���̃t�B�[�`���N���X</returns>
        private IFeatureClass GetTargetFeatureClass()
        {            
            //�ҏW�^�[�Q�b�g���C���̎擾
            IFeatureLayer targetFcLayer = GetEditTargetLayer();
  
            //�t�B�[�`���N���X�̎擾
            IFeatureClass targetFcClass = targetFcLayer.FeatureClass;

            return targetFcClass;
        }

        /// <summary>
        /// �ҏW�|���V�[�ɏ]���đ����l���}�[�W����B
        /// </summary>
        /// <param name="srcFeature">�}�[�W���̃t�B�[�`��</param>
        /// <param name="destRowSet">�}�[�W��̃t�B�[�`�����i�[����ISet</param>
        /// <param name="targetFC">�ҏW�Ώۃ��C��</param>
        private void MergeAttribute(IFeature srcFeature, ISet destRowSet, IFeatureClass targetFC)
        {
            SplitAndMargeSettings splitAndMergeSetting = null;

            //�ݒ�t�@�C�������݂��邩�m�F����
            if(!ApplicationInitializer.IsUserSettingsExists()) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                throw new Exception();
            }

            try {
                //�ݒ�t�@�C���iGISLight10Settings.xml�j����ҏW�|���V�[�̐ݒ��ǂݍ���            
                splitAndMergeSetting = new SplitAndMargeSettings();
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }

            string editorMergeNumPolicy = "";
            string editorMergeDatePolicy = "";
            string editorMergeOtherPolicy = "";
            bool	blnDomainFollow = false;		// ��Ҳݥϰ�ނ�̫۰����

            //���l�^�t�B�[���h�̃|���V�[
            try {
                editorMergeNumPolicy = splitAndMergeSetting.EditorMargeNumField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�F���l�^�t�B�[���h�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�F���l�^�t�B�[���h�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }
            
            //���t�^�t�B�[���h�̃|���V�[
            try {
                editorMergeDatePolicy = splitAndMergeSetting.EditorMargeDateField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�F���t�^�t�B�[���h�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�F���t�^�t�B�[���h�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }
            
            //���̑��t�B�[���h�̃|���V�[
            try {
                editorMergeOtherPolicy = splitAndMergeSetting.EditorMargeField;
            }
            catch (Exception ex) {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�F���̑��t�B�[���h�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error
                    (Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead +
                     "[ �}�[�W�F���̑��t�B�[���h�̑����ҏW���@ ]" +
                     Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
                ESRIJapan.GISLight10.Common.Logger.Error(ex.StackTrace);

                throw ex;
            }            

            IRow srcRow = srcFeature.Table.GetRow(srcFeature.OID); 

            Dictionary<string,IFeature>	dicMaxAndMinAreaFeature = null;
            IFeature		maxAreaFeature = null;
            IFeature		minAreaFeature = null;
            IFeatureBuffer	sumAttrebuteFeature = null;
            IRow			maxRow = null;
            IRow			minRow = null;
            IRow			sumRow = null;
            IField			field = null;

            try {
                //�ʐς��ő�ƍŏ��̃t�B�[�`�������߂�
                if ((editorMergeNumPolicy.Equals("0") || editorMergeDatePolicy.Equals("0") || editorMergeOtherPolicy.Equals("0")) ||
                    (editorMergeNumPolicy.Equals("1") || editorMergeDatePolicy.Equals("1") || editorMergeOtherPolicy.Equals("1")))
                {
                    //maxAreaFeature = GetMaxAreaFeature(srcFeature, destRowSet);
                    dicMaxAndMinAreaFeature = GetMaxAreaFeature(srcFeature, destRowSet);

                    maxAreaFeature = dicMaxAndMinAreaFeature["MaxAreaFeature"];
                    maxRow = maxAreaFeature.Table.GetRow(maxAreaFeature.OID);

                    minAreaFeature = dicMaxAndMinAreaFeature["MinAreaFeature"];
                    minRow = minAreaFeature.Table.GetRow(minAreaFeature.OID);
                }

                //�t�B�[�`���̑����l�����v���� ��ϰ�ޥ��ؼ��̍��Z���@�\���Ȃ����̕�U������ǉ� 
                if (editorMergeNumPolicy.Equals("2") || DomainManager.UsedSplitDomain(targetFC)) {
                    sumAttrebuteFeature = GetSumAttributeFeature2(srcFeature, destRowSet, targetFC);
                    sumRow = (IRow)sumAttrebuteFeature;
                }

                //�t�B�[���h�����[�v����
                for(int i = 0; i < srcFeature.Fields.FieldCount; i++) {
                    field = srcFeature.Fields.get_Field(i);

                    //Shape_Length�AShape_Area�AOID�A�W�I���g���t�B�[���h�̓X�L�b�v����                    
                    if (field.Equals(targetFC.LengthField) ||
                        field.Equals(targetFC.AreaField) ||
                        field.Type == esriFieldType.esriFieldTypeOID ||
                        field.Type == esriFieldType.esriFieldTypeGeometry)
                    {                        
                        continue;
                    }

                    //�t�B�[���h�ɑ΂��ăh���C���ݒ�̃|���V�[���ݒ肳��Ă���ꍇ���X�L�b�v����
                    if(field.Domain != null) {
                        // ϰ�ޥ��ؼ��̍��Z���@�\���Ȃ����̕�U���� 
                        if(field.Domain.Name.StartsWith(DomainManager.DOMAIN_NAME_SPLIT) && 
							field.Domain.MergePolicy == esriMergePolicyType.esriMPTSumValues) {
	                        GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[����");
							blnDomainFollow = true;
						}
						else {
	                        GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[����");
							continue;
                        }
                    }
                    else {
                        GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[�Ȃ�");
                    }                    

                    //�� �ҏW�|���V�[�̐���
                    // �����l�^�t�B�[���h
                    //   �@EditorMargeNumField == 0�̏ꍇ
                    //      �ʐς��ő�̃t�B�[�`���̑����l���g�p����B
                    //   �AEditorMargeNumField == 1�̏ꍇ
                    //      �ʐς��ŏ��̃t�B�[�`���̑����l���g�p����B
                    //   �BEditorMargeNumField == 2�̏ꍇ
                    //      �����l�����v�����l���g�p����B
                    // �����t�^�t�B�[���h
                    //   �CEditorMargeDateField == 0�̏ꍇ
                    //      �ʐς��ő�̃t�B�[�`���̑����l���g�p����B
                    //   �DEditorMargeDateField == 1�̏ꍇ
                    //      �ʐς��ŏ��̃t�B�[�`���̑����l���g�p����B
                    //   �EEditorMargeDateField == 2�̏ꍇ
                    //      �}�[�W�����������g�p����B
                    // �����̑��̃t�B�[���h
                    //   �FEditorMargeField == 0�̏ꍇ
                    //      �ʐς��ő�̃t�B�[�`���̑����l���g�p����B
                    //   �GEditorMargeField == 1�̏ꍇ
                    //      �ʐς��ŏ��̃t�B�[�`���̑����l���g�p����B
                    //
                    if (field.Type.Equals(esriFieldType.esriFieldTypeDouble) ||
                        field.Type.Equals(esriFieldType.esriFieldTypeInteger) ||
                        field.Type.Equals(esriFieldType.esriFieldTypeSingle) ||
                        field.Type.Equals(esriFieldType.esriFieldTypeSmallInteger))
                    {
                        // ���Z�⏕����
                        if(blnDomainFollow) {
							srcRow.set_Value(i, sumRow.get_Value(i));
							blnDomainFollow = false;
                        }
                        else {
							switch (editorMergeNumPolicy)
							{
								case "0":
									srcRow.set_Value(i, maxRow.get_Value(i));
									break;
								case "1":
									srcRow.set_Value(i, minRow.get_Value(i));
									break;
								case "2":
									srcRow.set_Value(i, sumRow.get_Value(i));
									break;
								default:
									break;
							}
                        }
                    }
                    else if (field.Type.Equals(esriFieldType.esriFieldTypeDate))
                    {
                        switch (editorMergeDatePolicy)
                        {
                            case "0":
                                srcRow.set_Value(i, maxRow.get_Value(i));
                                break;
                            case "1":
                                srcRow.set_Value(i, minRow.get_Value(i));
                                break;
                            case "2":
                                srcRow.set_Value(i, System.DateTime.Now);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (editorMergeOtherPolicy)
                        {
                            case "0":
                                srcRow.set_Value(i, maxRow.get_Value(i));
                                break;
                            case "1":
                                srcRow.set_Value(i, minRow.get_Value(i));
                                break;
                            default:
                                break;
                        }
                    }
                }

                srcRow.Store();
            }
            catch(COMException comex) {
                throw comex;
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                //ComReleaser.ReleaseCOMObject(srcRow);
                //ComReleaser.ReleaseCOMObject(dicMaxAndMinAreaFeature);
                //ComReleaser.ReleaseCOMObject(maxAreaFeature);
                //ComReleaser.ReleaseCOMObject(minAreaFeature);
                //ComReleaser.ReleaseCOMObject(sumAttrebuteFeature);
                //ComReleaser.ReleaseCOMObject(maxRow);
                //ComReleaser.ReleaseCOMObject(minRow);
                //ComReleaser.ReleaseCOMObject(sumRow);
                ComReleaser.ReleaseCOMObject(field);                
            }
        }


        /// <summary>
        /// �ʐς��ő�̃t�B�[�`�����擾
        /// </summary>
        /// <param name="srcFeature">�}�[�W���̃t�B�[�`��</param>
        /// <param name="destRowSet">�}�[�W��̃t�B�[�`�����i�[����ISet</param>
        /// <returns>�ʐς��ő�̃t�B�[�`��</returns>
        private Dictionary<string, IFeature> GetMaxAreaFeature(IFeature srcFeature, ISet destRowSet)
        {
            Dictionary<string,IFeature> dicMaxAndMinAreaFeature = 
                new Dictionary<string,IFeature>();

            IFeature maxAreaFeature = srcFeature;
            IFeature minAreaFeature = srcFeature;
            destRowSet.Reset();

            IArea maxArea;
            IArea minArea;
            IArea challengerArea;
            IFeature challengerFeature;

            for (int i = 0; i < destRowSet.Count; i++)
            {
                maxArea = (IArea)maxAreaFeature.Shape;
                minArea = (IArea)minAreaFeature.Shape;

                challengerFeature = (IFeature)destRowSet.Next();
                challengerArea = (IArea)challengerFeature.Shape;

                //�ő�����߂�
                if (maxArea.Area < challengerArea.Area)
                {
                    maxAreaFeature = challengerFeature;
                }
                
                //�ŏ������߂�
                if (minArea.Area > challengerArea.Area)
                {
                    minAreaFeature = challengerFeature;
                }
            }

            dicMaxAndMinAreaFeature.Add("MaxAreaFeature", maxAreaFeature);
            dicMaxAndMinAreaFeature.Add("MinAreaFeature", minAreaFeature);

            return dicMaxAndMinAreaFeature;
        }        


        /// <summary>
        /// ���l�^�̃t�B�[���h�̑����l�����v�����t�B�[�`�����擾
        /// </summary>
        /// <param name="srcFeature">�}�[�W���̃t�B�[�`��</param>
        /// <param name="destRowSet">�}�[�W��̃t�B�[�`�����i�[����ISet</param>
        /// <param name="targetFC">�ҏW�Ώۃ��C��</param>
        /// <returns>���l�^�̃t�B�[���h�̑����l�����v�����t�B�[�`��</returns>
        private IFeature GetSumAttributeFeature(IFeature srcFeature, ISet destRowSet, IFeatureClass targetFC)
        {
            //�I�u�W�F�N�g�̃R�s�[�i�l�n���j
            //ESRI.ArcGIS.esriSystem.IObjectCopy objectCopy = 
            //    new ESRI.ArcGIS.esriSystem.ObjectCopyClass();
            //IFeature sumAttributeFeature = objectCopy.Copy(srcFeature) as IFeature;

            //�Q�Ɠn��
            IFeature sumAttributeFeature = srcFeature;

            destRowSet.Reset();

            IRow sumRow = sumAttributeFeature.Table.GetRow(srcFeature.OID);
            
            IFeature feature;
            IRow row;
            IField field;

			// �����l�̍��Z
            for(int i = 0; i < destRowSet.Count; i++) {
                feature = (IFeature)destRowSet.Next();
                row = feature.Table.GetRow(feature.OID);

                for(int j = 0; j < sumAttributeFeature.Fields.FieldCount; j++) {
                    field = sumAttributeFeature.Fields.get_Field(j);

                    //Shape_Length�AShape_Area�AOID�A�W�I���g���t�B�[���h�̓X�L�b�v����
                    if(field.Equals(targetFC.LengthField) ||
                        field.Equals(targetFC.AreaField) ||
                        field.Type == esriFieldType.esriFieldTypeOID ||
                        field.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }

                    //�t�B�[���h�ɑ΂��ăh���C���ݒ�̃|���V�[���ݒ肳��Ă���ꍇ���X�L�b�v����
                    if(field.Domain != null) {
						// ϰ�ޥ��ؼ��̍��Z���@�\���Ȃ����̕�U����
                        if(DomainManager.UsedSplitDomain(targetFC) && 
							(field.Domain.Name.StartsWith(DomainManager.DOMAIN_NAME_SPLIT) && 
								field.Domain.MergePolicy == esriMergePolicyType.esriMPTSumValues)) {
	                        GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[����");
	                    }
	                    else {
							GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[����");
							continue;
                        }
                    }
                    else {
                        GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[�Ȃ�");
                    }     

                    if(!sumRow.get_Value(j).Equals(System.DBNull.Value) &&
                                !row.get_Value(j).Equals(System.DBNull.Value))
                    {
                        switch (field.Type)
                        {
                            case esriFieldType.esriFieldTypeDouble:                                
                                double doubleValue = (double)sumRow.get_Value(j) + (double)row.get_Value(j);
                                sumRow.set_Value(j, doubleValue);                               
                                break;

                            case esriFieldType.esriFieldTypeInteger:                                
                                Int32 longValue = (Int32)sumRow.get_Value(j) + (Int32)row.get_Value(j);
                                sumRow.set_Value(j, longValue);                                
                                break;

                            case esriFieldType.esriFieldTypeSingle:                                
                                Single floatValue = (Single)sumRow.get_Value(j) + (Single)row.get_Value(j);
                                sumRow.set_Value(j, floatValue);                                
                                break;

                            case esriFieldType.esriFieldTypeSmallInteger:                                
                                int intValue = (Int16)sumRow.get_Value(j) + (Int16)row.get_Value(j);
                                sumRow.set_Value(j, intValue);                                
                                break;

                            default:
                                break;
                        }
                    }
                }
                sumRow.Store();
            }

            return sumAttributeFeature;
        }        

        /// <summary>
        /// ���l�^�̃t�B�[���h�̑����l�����v�����t�B�[�`�����擾
        /// </summary>
        /// <param name="srcFeature">�}�[�W���̃t�B�[�`��</param>
        /// <param name="destRowSet">�}�[�W��̃t�B�[�`�����i�[����ISet</param>
        /// <param name="targetFC">�ҏW�Ώۃ��C��</param>
        /// <returns>���l�^�̃t�B�[���h�̑����l�����v�����t�B�[�`��</returns>
        private IFeatureBuffer GetSumAttributeFeature2(IFeature srcFeature, ISet destRowSet, IFeatureClass targetFC) {
            // ���Q�Ɠn���ł͍ő�A�ŏ��̑��������Z����Ă��܂��ׁA�ޯ̧���쐬���đΉ�
            IFeatureBuffer	sumAttributeFeature = targetFC.CreateFeatureBuffer();
            IRow			sumRow = (IRow)sumAttributeFeature;
            
            IRow			row;
            IField			field;

			// �����l�̍��Z (�ꎞ�I�ɿ����ΏۂɊ܂߂�)
            destRowSet.Add(srcFeature);
            destRowSet.Reset();
            for(int i = 0; i < destRowSet.Count; i++) {
                row = (IRow)destRowSet.Next();

                for(int j = 0; j < sumAttributeFeature.Fields.FieldCount; j++) {
                    field = sumAttributeFeature.Fields.get_Field(j);

                    //Shape_Length�AShape_Area�AOID�A�W�I���g���t�B�[���h�̓X�L�b�v����
                    if(field.Equals(targetFC.LengthField) ||
                        field.Equals(targetFC.AreaField) ||
                        field.Type == esriFieldType.esriFieldTypeOID ||
                        field.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }

                    //�t�B�[���h�ɑ΂��ăh���C���ݒ�̃|���V�[���ݒ肳��Ă���ꍇ���X�L�b�v����
                    if(field.Domain != null) {
						// ϰ�ޥ��ؼ��̍��Z���@�\���Ȃ����̕�U���� 
                        if(DomainManager.UsedSplitDomain(targetFC) && 
							(field.Domain.Name.StartsWith(DomainManager.DOMAIN_NAME_SPLIT) && 
								field.Domain.MergePolicy == esriMergePolicyType.esriMPTSumValues)) {
	                        GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[����");
	                    }
	                    else {
							GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[����");
							continue;
                        }
                    }
                    else {
                        GISLight10.Common.Logger.Info(field.Name + ": �h���C���|���V�[�Ȃ�");
                    }     

                    if(!row.get_Value(j).Equals(System.DBNull.Value)) {
						// �W�v�l���擾
						object objVal = sumRow.get_Value(j);
						if(objVal.Equals(DBNull.Value)) {
							objVal = 0;
						}
						
                        // ���l̨���ނ�����Ώ�
                        switch(field.Type) {
                        case esriFieldType.esriFieldTypeDouble:
                            double doubleValue = Convert.ToDouble(objVal) + Convert.ToDouble(row.get_Value(j));
                            sumRow.set_Value(j, doubleValue);                               
                            break;

                        case esriFieldType.esriFieldTypeInteger:
                            Int32 longValue = Convert.ToInt32(objVal) + Convert.ToInt32(row.get_Value(j));
                            sumRow.set_Value(j, longValue);                                
                            break;

                        case esriFieldType.esriFieldTypeSingle:
                            Single floatValue = Convert.ToSingle(objVal) + Convert.ToSingle(row.get_Value(j));
                            sumRow.set_Value(j, floatValue);                                
                            break;

                        case esriFieldType.esriFieldTypeSmallInteger:
                            int intValue = Convert.ToInt16(objVal) + Convert.ToInt16(row.get_Value(j));
                            sumRow.set_Value(j, intValue);                                
                            break;

                        default:
                            break;
                        }
                    }
                }
                //sumRow.Store();
            }
            
            // �����Ώۂ���O�� �� �O���Ă����Ȃ��ƌ�ō폜����Ă��܂�����
            destRowSet.Remove(srcFeature);

            return sumAttributeFeature;
        }        

        /// <summary>
        /// �t�B�[�`���̃t���b�V��
        /// </summary>
        /// <param name="geo">�W�I���g��</param>
        /// <param name="display">IDisplay</param>
        private void FlashGeometry(IGeometry geo, IDisplay display)
        {
            //Flash the input polygon geometry.
            display.StartDrawing(display.hDC, (short)esriScreenCache.esriNoScreenCache);

            //Time in milliseconds to wait.
            int interval = 150;
            switch (geo.GeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    //Set the flash geometry's symbol.
                    IRgbColor color = new RgbColorClass();
                    color.Red = 255;

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = color;

                    ISymbol symbol = simpleFillSymbol as ISymbol;
                    symbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                    display.SetSymbol(symbol);
                    display.DrawPolygon(geo);
                    System.Threading.Thread.Sleep(interval);
                    display.DrawPolygon(geo);

                    break;
            }
            display.FinishDrawing();
        }
    }
}