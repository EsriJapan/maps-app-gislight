using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// フィールドの追加と削除機能
    /// </summary>
    public partial class FormAddAndDeleteField : Form
    {
        /// <summary>
        /// フィールドの追加の削除を行うレイヤを取得するフォーム
        /// </summary>
        /// <history>
        ///  2010-11-01 新規作成 
        /// </history>
        private Ui.MainForm mainFrm;

        /// <summary>
        /// データソースのタイプ
        /// </summary>
        private const string TYPE_SHAPE = "SHAPE";
        private const string TYPE_FGDB = "FGDB";
        private const string TYPE_MDB = "MDB";
        private const string TYPE_SDE = "SDE";

        /// <summary>
        /// タブインデックス
        /// </summary>
        private const int TAB_INDEX_ADD_FIELD = 0;
        private const int TAB_INDEX_DELETE_FIELD = 1;

        /// <summary>
        /// テキストフィールド幅の最大最小数
        /// </summary>
        private const int SHAPE_MAX_STRING_LENGTH = 254;
        private const int SHAPE_MIN_STRING_LENGTH = 0;
        private const int FGDB_MDB_MAX_STRING_LENGTH = 2147483647;
        private const int FGDB_MDB_MIN_STRING_LENGTH = 0;

        /// <summary>
        /// テキストフィールド幅のデフォルト値
        /// </summary>
        private const int DEFAULT_STRING_LENGTH = 50;

        /// <summary>
        /// 全桁数のデフォルト値
        /// </summary>
        private const int DEFAULT_PRECISION_LENGTH = 0;

        /// <summary>
        /// 小数点以下桁数のデフォルト値
        /// </summary>
        private const int DEFAULT_SCALE_LENGTH = 0;

        /// <summary>
        /// フィールドの最大数
        /// </summary>
        private const int SHAPE_MAX_FIELD_NUM = 255;
        private const int FGDB_MDB_MAX_FIELD_NUM = 65534;

        /// <summary>
        /// エイリアス名の最大最小文字数
        /// </summary>
        private const int FGDB_MDB_MAX_ALIAS_NAME = 255;
        private const int FGDB_MDB_MIN_ALIAS_NAME = 0;

        /// <summary>
        /// コンストラクタ
        /// 各変数、UIの初期化を行う
        /// </summary>
        /// <param name="frm">メインフォーム</param>
        public FormAddAndDeleteField(Ui.MainForm frm)
        {
            try
            {
                InitializeComponent();
                this.mainFrm = frm;

                // 各タブの初期化
                SetupTabPageAddField();
                SetupTabPageDeleteField();
            }
            catch (COMException comex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormAddAndDeleteField_ERROR_Initialize);
                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormAddAndDeleteField_ERROR_Initialize);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Close();
            }
        }

        /// <summary>
        /// フィールドの追加タブの設定
        /// </summary>
        private void SetupTabPageAddField()
        {
            ILayer sourceLayer = this.mainFrm.SelectedLayer;

            // フィールド プロパティの表示設定
            // シェープファイル
            if (GetTypeOfDataSource(sourceLayer) == TYPE_SHAPE)
            {
                this.labelAddFieldAlias.Enabled = false;
                this.textBoxAddFieldAlias.Enabled = false;
            }
            // ファイルジオデータベース or パーソナルジオデータベース or SDE
            else if (GetTypeOfDataSource(sourceLayer) == TYPE_FGDB
                || GetTypeOfDataSource(sourceLayer) == TYPE_MDB
                || GetTypeOfDataSource(sourceLayer) == TYPE_SDE)
            {
                this.labelAddFieldAlias.Enabled = true;
                this.textBoxAddFieldAlias.Enabled = true;
            }
            else
            {
                this.Close();
            }

            // フィールドの追加タイプコンボボックスの設定
            SetupComboBoxAddFieldType();
        }

        /// <summary>
        /// フィールドの追加タイプコンボボックスの設定
        /// </summary>
        private void SetupComboBoxAddFieldType()
        {
            // コンボボックスのアイテム設定
            comboBoxAddFieldType.Items.Clear();

            // 各フィールドのタイプを設定
            // Short Integer
            comboBoxAddFieldType.Items.Add(new ConstComboItem(
                (int)esriFieldType.esriFieldTypeSmallInteger,
                Properties.Resources.FormAddAndDeleteField_FT_SmallInteger));

            // Long Integer
            comboBoxAddFieldType.Items.Add(new ConstComboItem(
                (int)esriFieldType.esriFieldTypeInteger,
                Properties.Resources.FormAddAndDeleteField_FT_Integer));

            // Float
            comboBoxAddFieldType.Items.Add(new ConstComboItem(
                (int)esriFieldType.esriFieldTypeSingle,
                Properties.Resources.FormAddAndDeleteField_FT_Single));

            // Double
            comboBoxAddFieldType.Items.Add(new ConstComboItem(
                (int)esriFieldType.esriFieldTypeDouble,
                Properties.Resources.FormAddAndDeleteField_FT_Double));

            // Text
            comboBoxAddFieldType.Items.Add(new ConstComboItem(
                (int)esriFieldType.esriFieldTypeString,
                Properties.Resources.FormAddAndDeleteField_FT_String));

            // Date
            comboBoxAddFieldType.Items.Add(new ConstComboItem(
                (int)esriFieldType.esriFieldTypeDate,
                Properties.Resources.FormAddAndDeleteField_FT_Date));

            comboBoxAddFieldType.SelectedIndex = 0;
        }

        /// <summary>
        /// データソースのタイプを判別
        /// </summary>
        /// <param name="sourceLayer">判別対象のレイヤ</param>
        /// <returns>データソースのタイプ</returns>
        private string GetTypeOfDataSource(ILayer sourceLayer)
        {
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
            IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;

            IWorkspace sourceWorkspace = LayerManager.getWorkspace(sourceFeatureClass);

            // シェープファイル
            if (sourceWorkspace.Type == esriWorkspaceType.esriFileSystemWorkspace)
            {
                return TYPE_SHAPE;
            }
            else if (sourceWorkspace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                // ファイルジオデータベース
                if (sourceWorkspace.WorkspaceFactory.GetClassID().Value.ToString() ==
                    "{71FE75F0-EA0C-4406-873E-B7D53748AE7E}")
                {
                    return TYPE_FGDB;
                }
                //パーソナルジオデータベース
                else
                {
                    return TYPE_MDB;
                }
            }
            // SDE
            else if (sourceWorkspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                return TYPE_SDE;
            }

            return string.Empty;
        }

        /// <summary>
        /// フィールドの削除タブの設定
        /// </summary>
        private void SetupTabPageDeleteField()
        {
            // コンボボックスのアイテム設定
            comboBoxDeleteField.Items.Clear();
            
            // 結合先のフィールドは取得しない
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)this.mainFrm.SelectedLayer;
            IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;
            IFields sourceFields = sourceFeatureClass.Fields;

            // コンボボックスにフィールドを追加
            for (int i = 0; i < sourceFields.FieldCount; i++)
            {
                IField sourceField = sourceFields.get_Field(i);

                // 編集不可フィールドは除外
                if (sourceField.Editable == true)
                {
                    // 該当タイプのフィールドのみコンボボックスへ追加
                    if (sourceField.Type == esriFieldType.esriFieldTypeSmallInteger
                        || sourceField.Type == esriFieldType.esriFieldTypeInteger
                        || sourceField.Type == esriFieldType.esriFieldTypeSingle
                        || sourceField.Type == esriFieldType.esriFieldTypeDouble
                        || sourceField.Type == esriFieldType.esriFieldTypeString
                        || sourceField.Type == esriFieldType.esriFieldTypeDate)
                    {
                        FieldComboItem fieldComboItem = new FieldComboItem(sourceField);

                        comboBoxDeleteField.Items.Add(fieldComboItem);
                    }
                }
            }

            if (comboBoxDeleteField.Items.Count > 0)
            {
                comboBoxDeleteField.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// フィールドの追加タイプコンボボックスの選択が変更されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAddFieldType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ILayer sourceLayer = this.mainFrm.SelectedLayer;
                ConstComboItem addFieldTypeComboItem = (ConstComboItem)comboBoxAddFieldType.SelectedItem;

                // シェープファイル
                if (GetTypeOfDataSource(sourceLayer) == TYPE_SHAPE)
                {
                    // Small Integer型, Long Integer型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeSmallInteger
                        || addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeInteger)
                    {
                        labelAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Text = "";

                        labelAddFieldPrecision.Enabled = true;
                        textBoxAddFieldPrecision.Enabled = true;
                        textBoxAddFieldPrecision.Text = DEFAULT_PRECISION_LENGTH.ToString();

                        labelAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Text = "";
                    }

                    // Float型, Double型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeSingle
                        || addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeDouble)
                    {
                        labelAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Text = "";

                        labelAddFieldPrecision.Enabled = true;
                        textBoxAddFieldPrecision.Enabled = true;
                        textBoxAddFieldPrecision.Text = DEFAULT_PRECISION_LENGTH.ToString();

                        labelAddFieldScale.Enabled = true;
                        textBoxAddFieldScale.Enabled = true;
                        textBoxAddFieldScale.Text = DEFAULT_SCALE_LENGTH.ToString();
                    }

                    // Text型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeString)
                    {
                        labelAddFieldLength.Enabled = true;
                        textBoxAddFieldLength.Enabled = true;
                        textBoxAddFieldLength.Text = DEFAULT_STRING_LENGTH.ToString();

                        labelAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Text = "";

                        labelAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Text = "";
                    }

                    // Date型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeDate)
                    {
                        labelAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Text = "";

                        labelAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Text = "";

                        labelAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Text = "";
                    }
                }

                // ジオデータベース
                if (GetTypeOfDataSource(sourceLayer) == TYPE_FGDB
                    || GetTypeOfDataSource(sourceLayer) == TYPE_MDB
                    || GetTypeOfDataSource(sourceLayer) == TYPE_SDE)
                {
                    // Small Integer型, Long Integer型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeSmallInteger
                        || addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeInteger)
                    {
                        labelAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Text = "";

                        labelAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Text = "";

                        labelAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Text = "";
                    }

                    // Float型, Double型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeSingle
                        || addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeDouble)
                    {
                        labelAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Text = "";

                        labelAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Text = "";

                        labelAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Text = "";
                    }

                    // Text型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeString)
                    {
                        labelAddFieldLength.Enabled = true;
                        textBoxAddFieldLength.Enabled = true;
                        textBoxAddFieldLength.Text = DEFAULT_STRING_LENGTH.ToString();

                        labelAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Text = "";

                        labelAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Text = "";
                    }

                    // Date型
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeDate)
                    {
                        labelAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Enabled = false;
                        textBoxAddFieldLength.Text = "";

                        labelAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Enabled = false;
                        textBoxAddFieldPrecision.Text = "";

                        labelAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Enabled = false;
                        textBoxAddFieldScale.Text = "";
                    }
                }
            }
            catch (COMException ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormAddAndDeleteField_ERROR_ChangeFieldType);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.FormAddAndDeleteField_ERROR_ChangeFieldType);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// テキストボックス入力キーチェック(数字のみ)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// フィールドの追加と削除(OKボタン)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            int tabIndex = -1;

            try
            {
                // フィールドの追加
                if (tabControlAddAndDeleteField.SelectedIndex == TAB_INDEX_ADD_FIELD)
                {
                    tabIndex = TAB_INDEX_ADD_FIELD;

                    // 入力パラメータのチェック
                    if (!AddFieldCheckParameter())
                    {
                        return;
                    }

                    // 追加処理
                    AddField();
                }
                // フィールドの削除
                else if (tabControlAddAndDeleteField.SelectedIndex == TAB_INDEX_DELETE_FIELD)
                {
                    tabIndex = TAB_INDEX_DELETE_FIELD;

                    // 入力パラメータのチェック
                    if (!DeleteFieldCheckParameter())
                    {
                        return;
                    }

                    // 削除処理
                    DeleteField();
                }
                else
                {
                    return;
                }

                mainFrm.axMapControl1.ActiveView.PartialRefresh(
                    esriViewDrawPhase.esriViewGeography, null, mainFrm.axMapControl1.ActiveView.Extent);
            }
            catch (COMException comex)
            {
                if (tabIndex == TAB_INDEX_ADD_FIELD)
                {
                    MessageBoxManager.ShowMessageBoxError(this,
                        Properties.Resources.FormAddAndDeleteField_ERROR_AddField);
                }
                else if (tabIndex == TAB_INDEX_DELETE_FIELD)
                {
                    MessageBoxManager.ShowMessageBoxError(this,
                        Properties.Resources.FormAddAndDeleteField_ERROR_DeleteField);
                }
                else
                {
                }

                Common.Logger.Error(comex.Message);
                Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                if (tabIndex == TAB_INDEX_ADD_FIELD)
                {
                    MessageBoxManager.ShowMessageBoxError(this,
                        Properties.Resources.FormAddAndDeleteField_ERROR_AddField);
                }
                else if (tabIndex == TAB_INDEX_DELETE_FIELD)
                {
                    MessageBoxManager.ShowMessageBoxError(this,
                        Properties.Resources.FormAddAndDeleteField_ERROR_DeleteField);
                }
                else
                {
                }

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }   
        }

        /// <summary>
        /// フィールドの追加
        /// </summary>
        private void AddField()
        {
            Logger.Info("フィールドの追加開始");

            bool closeForm = true;
            bool schemaLock = false;

            ILayer sourceLayer = this.mainFrm.SelectedLayer;
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
            IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;
            ISchemaLock sourceSchemaLock = (ISchemaLock)sourceFeatureClass;

            try
            {
                // フィールドのクローンを作成
                IFields sourceFields = sourceFeatureClass.Fields;
                IClone cloneSource = (IClone)sourceFields;
                IClone cloneTarget = cloneSource.Clone();
                IFields targetFields = (IFields)cloneTarget;

                IFieldsEdit targetFieldsEdit = (IFieldsEdit)targetFields;

                IField targetField = new FieldClass();
                IFieldEdit targetFieldEdit = (IFieldEdit)targetField;

                ConstComboItem addFieldTypeComboItem = (ConstComboItem)comboBoxAddFieldType.SelectedItem;

                // フィールド名の設定
                targetFieldEdit.Name_2 = textBoxAddFieldName.Text;

                // フィールドタイプを設定
                targetFieldEdit.Type_2 = (esriFieldType)addFieldTypeComboItem.Id;

                // FGDB,MDB,SDE
                if (GetTypeOfDataSource(sourceLayer) == TYPE_FGDB
                    || GetTypeOfDataSource(sourceLayer) == TYPE_MDB
                    || GetTypeOfDataSource(sourceLayer) == TYPE_SDE)
                {
                    // エイリアス名を設定
                    if (textBoxAddFieldAlias.Text.Length > 0)
                    {
                        targetFieldEdit.AliasName_2 = textBoxAddFieldAlias.Text;
                    }

                    // 長さを設定(Text型)
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeString)
                    {
                        if (textBoxAddFieldLength.Text.Length > 0)
                        {
                            targetFieldEdit.Length_2 = int.Parse(textBoxAddFieldLength.Text);
                        }
                    }
                }

                // SHAPE
                if (GetTypeOfDataSource(sourceLayer) == TYPE_SHAPE)
                {
                    // 全桁数を設定(Short Integer型, Long Integer型, Float型, Double型)
                    if (textBoxAddFieldPrecision.Text.Length > 0)
                    {
                        targetFieldEdit.Precision_2 = int.Parse(textBoxAddFieldPrecision.Text);
                    }

                    // 小数点以下桁数を設定(Float型, Double型)
                    if (textBoxAddFieldScale.Text.Length > 0)
                    {
                        targetFieldEdit.Scale_2 = int.Parse(textBoxAddFieldScale.Text);
                    }

                    // 長さを設定(Text型)
                    if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeString)
                    {
                        if (textBoxAddFieldLength.Text.Length > 0)
                        {
                            targetFieldEdit.Length_2 = int.Parse(textBoxAddFieldLength.Text);
                        }
                    }
                }

                targetFieldsEdit.AddField(targetField);

                IFields validateTargetFields;

                // 追加フィールドのチェック
                if (!CheckField(targetFields, out validateTargetFields))
                {
                    closeForm = false;
                    return;
                }

                // 編集セッション
                IWorkspace sourceWorkspace = Common.LayerManager.getWorkspace(sourceFeatureClass);
                IWorkspaceEdit sourceWorkspaceEdit = (IWorkspaceEdit)sourceWorkspace;

                // 編集セッションチェック
                try
                {
                    sourceWorkspaceEdit.StartEditing(false);
                    sourceWorkspaceEdit.StopEditing(false);
                }
                catch (COMException comex)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                            Properties.Resources.FormAddAndDeleteField_AF_WARNING_EditingLock);
                    Common.Logger.Error(comex.Message);
                    Common.Logger.Error(comex.StackTrace);

                    closeForm = false;
                    return;
                }

                // スキーマロックチェック
                IEnumSchemaLockInfo sourceEnumSchemaLockInfo;
                sourceSchemaLock.GetCurrentSchemaLocks(out sourceEnumSchemaLockInfo);
                ISchemaLockInfo sourceSchemaLockInfo;
                sourceSchemaLockInfo = sourceEnumSchemaLockInfo.Next();

                if (sourceSchemaLockInfo != null)
                {
                    try
                    {
                        // スキーマロック
                        sourceSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                    }
                    catch (COMException comex)
                    {
                        MessageBoxManager.ShowMessageBoxWarining(this,
                            Properties.Resources.FormAddAndDeleteField_AF_WARNING_SchemaLock);
                        Common.Logger.Error(comex.Message);
                        Common.Logger.Error(comex.StackTrace);

                        closeForm = false;
                        return;
                    }
                    
                    // フィールドの追加
                    sourceFeatureClass.AddField(
                        validateTargetFields.get_Field(validateTargetFields.FieldCount - 1));
                    
                    schemaLock = true;
                }
                else
                {
                    // フィールドの追加
                    sourceFeatureClass.AddField(
                        validateTargetFields.get_Field(validateTargetFields.FieldCount - 1));
                }
            }
            finally
            {
                // スキーマロック解除
                if (schemaLock == true)
                {
                    sourceSchemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                }

                if (closeForm == true)
                {
                    this.Close();
                    Logger.Info("フィールドの追加終了");
                }
            }
        }

        /// <summary>
        /// 追加フィールドのチェック
        /// </summary>
        private bool CheckField(IFields targetFields, out IFields validateTargetFields)
        {
            IFieldChecker fieldChecker = null;
            IEnumFieldError enumFieldError = null;

            try
            {
                ILayer sourceLayer = this.mainFrm.SelectedLayer;
                IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
                IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;

                IWorkspace sourceWorkspace = LayerManager.getWorkspace(sourceFeatureClass);

                fieldChecker = new FieldCheckerClass();
                fieldChecker.InputWorkspace = sourceWorkspace;
                fieldChecker.ValidateWorkspace = sourceWorkspace;

                // フィールド名のチェック
                fieldChecker.Validate(targetFields, out enumFieldError, out validateTargetFields);

                // エラーの場合メッセージを表示
                if (enumFieldError != null)
                {
                    enumFieldError.Reset();
                    IFieldError fieldError = enumFieldError.Next();

                    string errorMessage;

                    // フィールド名にSQLの予約語を含んでいる
                    if (fieldError.FieldError == esriFieldNameErrorType.esriSQLReservedWord)
                    {
                        errorMessage = Properties.Resources.FormAddAndDeleteField_FN_WARINIG_SQLReservedWord;
                    }
                    // フィールド名が重複している
                    else if (fieldError.FieldError == esriFieldNameErrorType.esriDuplicatedFieldName)
                    {
                        errorMessage = Properties.Resources.FormAddAndDeleteField_FN_WARINIG_DuplicatedFieldName;
                    }
                    // フィールド名に無効な文字がある
                    else if (fieldError.FieldError == esriFieldNameErrorType.esriInvalidCharacter)
                    {
                        errorMessage = Properties.Resources.FormAddAndDeleteField_FN_WARINIG_InvalidCharacter;
                    }
                    // フィールド名が長さが無効
                    else if (fieldError.FieldError == esriFieldNameErrorType.esriInvalidFieldNameLength)
                    {
                        errorMessage = Properties.Resources.FormAddAndDeleteField_FN_WARINIG_InvalidFieldNameLength;
                    }
                    else
                    {
                        return false;
                    }

                    // 変更を問うメッセージボックス
                    DialogResult resFixedData =
                        MessageBoxManager.ShowMessageBoxWarining2(this,
                        Properties.Resources.FormAddAndDeleteField_FN_FieldName
                        + targetFields.get_Field(fieldError.FieldIndex).Name
                        + errorMessage
                        + Properties.Resources.FormAddAndDeleteField_FN_NewFieldName
                        + validateTargetFields.get_Field(fieldError.FieldIndex).Name
                        + Properties.Resources.FormAddAndDeleteField_FN_ChangeFieldName);

                    // キャンセルの場合
                    if (resFixedData == DialogResult.Cancel)
                    {
                        return false;
                    }
                }

                return true;
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(fieldChecker);
            }
        }

        /// <summary>
        /// フィールドの削除
        /// </summary>
        private void DeleteField()
        {
            Logger.Info("フィールドの削除開始");

            bool closeForm = true;
            bool schemaLock = false;

            ILayer sourceLayer = this.mainFrm.SelectedLayer;
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
            IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;
            ISchemaLock sourceSchemaLock = (ISchemaLock)sourceFeatureClass;

            try
            {
                // 選択されたフィールドを取得
                FieldComboItem deleteFieldComboItem = (FieldComboItem)comboBoxDeleteField.SelectedItem;

                // 編集セッション
                IWorkspace sourceWorkspace = Common.LayerManager.getWorkspace(sourceFeatureClass);
                IWorkspaceEdit sourceWorkspaceEdit = (IWorkspaceEdit)sourceWorkspace;

                // 編集セッションチェック
                try
                {
                    sourceWorkspaceEdit.StartEditing(false);
                    sourceWorkspaceEdit.StopEditing(false);
                }
                catch (COMException comex)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                            Properties.Resources.FormAddAndDeleteField_DF_WARNING_EditingLock);
                    Common.Logger.Error(comex.Message);
                    Common.Logger.Error(comex.StackTrace);

                    closeForm = false;
                    return;
                }

                // スキーマロックチェック
                IEnumSchemaLockInfo sourceEnumSchemaLockInfo;
                sourceSchemaLock.GetCurrentSchemaLocks(out sourceEnumSchemaLockInfo);
                ISchemaLockInfo sourceSchemaLockInfo;
                sourceSchemaLockInfo = sourceEnumSchemaLockInfo.Next();

                if (sourceSchemaLockInfo != null)
                {
                    try
                    {
                        // スキーマロック
                        sourceSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                    }
                    catch (COMException comex)
                    {
                        MessageBoxManager.ShowMessageBoxWarining(this,
                            Properties.Resources.FormAddAndDeleteField_DF_WARNING_SchemaLock);
                        Common.Logger.Error(comex.Message);
                        Common.Logger.Error(comex.StackTrace);

                        closeForm = false;
                        return;
                    }

                    // フィールドの削除
                    sourceFeatureClass.DeleteField(deleteFieldComboItem.Field);

                    schemaLock = true;
                }
                else
                {
                    // フィールドの削除
                    sourceFeatureClass.DeleteField(deleteFieldComboItem.Field);
                }
            }
            finally
            {
                // スキーマロック解除
                if (schemaLock == true)
                {
                    sourceSchemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                }

                if (closeForm == true)
                {
                    this.Close();

                    Logger.Info("フィールドの削除終了");
                }
            }
        }

        /// <summary>
        /// 入力パラメータのチェック(フィールドの追加)
        /// </summary>
        private bool AddFieldCheckParameter()
        {
            ILayer sourceLayer = this.mainFrm.SelectedLayer;
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
            IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;
            IFields sourceFields = sourceFeatureClass.Fields;

            ConstComboItem addFieldTypeComboItem = (ConstComboItem)comboBoxAddFieldType.SelectedItem;

            IWorkspace sourceWorkspace = Common.LayerManager.getWorkspace(sourceFeatureClass);
            IWorkspaceEdit sourceWorkspaceEdit = (IWorkspaceEdit)sourceWorkspace;

            // シェープファイル
            if (GetTypeOfDataSource(sourceLayer) == TYPE_SHAPE)
            {
                // 編集セッションチェック
                if (sourceWorkspaceEdit.IsBeingEdited() == true)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                            Properties.Resources.FormAddAndDeleteField_AF_WARNING_InEditing);
                    return false;
                }

                // フィールドの最大数チェック
                if (sourceFields.FieldCount > SHAPE_MAX_FIELD_NUM)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormAddAndDeleteField_AF_WARNING_MaxFieldNum);
                    return false;
                }

                // フィールド名の入力チェック
                if (textBoxAddFieldName.Text.Length < 1)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormAddAndDeleteField_AF_WARNING_NoFieldName);
                    return false;
                }

                // フィールド名に " が含まれる場合エラー
                if (textBoxAddFieldName.Text.Contains("\""))
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormAddAndDeleteField_AF_WARNING_FieldNameContainsDoubleQuotes);
                    return false;
                }

                // Float型, Double型
                if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeSingle
                        || addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeDouble)
                {
                    // 全桁数, 小数点以下桁数チェック
                    if (textBoxAddFieldScale.Text.Length > 0)
                    {
                        if (textBoxAddFieldPrecision.Text.Length > 0)
                        {
                            // 全桁数 < 小数点以下桁数
                            if (int.Parse(textBoxAddFieldPrecision.Text) < int.Parse(textBoxAddFieldScale.Text))
                            {
                                MessageBoxManager.ShowMessageBoxWarining(this,
                                    Properties.Resources.FormAddAndDeleteField_AF_WARNING_ScaleInvalid);
                                return false;
                            }
                        }
                        // 全桁数が未入力, 小数点以下桁数が入力
                        else
                        {
                            MessageBoxManager.ShowMessageBoxWarining(this,
                                Properties.Resources.FormAddAndDeleteField_AF_WARNING_NoPrecision);
                            return false;
                        }
                    }
                }

                // Text型
                if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeString)
                {
                    // 長さのチェック
                    if (textBoxAddFieldLength.Text.Length > 0)
                    {
                        int fieldLength = -1;

                        try
                        {
                            fieldLength = int.Parse(textBoxAddFieldLength.Text);
                        }
                        catch
                        {
                            MessageBoxManager.ShowMessageBoxWarining(this,
                                Properties.Resources.FormAddAndDeleteField_AF_WARNING_MaxStringLength
                                + "[ "
                                + SHAPE_MIN_STRING_LENGTH
                                + " - "
                                + SHAPE_MAX_STRING_LENGTH
                                + " ]");
                            return false;
                        }

                        if (fieldLength > SHAPE_MAX_STRING_LENGTH)
                        {
                            MessageBoxManager.ShowMessageBoxWarining(this,
                                Properties.Resources.FormAddAndDeleteField_AF_WARNING_MaxStringLength
                                + "[ "
                                + SHAPE_MIN_STRING_LENGTH
                                + " - "
                                + SHAPE_MAX_STRING_LENGTH
                                + " ]");
                            return false;
                        }

                        // 長さに0が入力
                        if (int.Parse(textBoxAddFieldLength.Text) == 0)
                        {
                            DialogResult resZeroTextLength = MessageBoxManager.ShowMessageBoxQuestion2(
                                this, Properties.Resources.FormAddAndDeleteField_AF_QUESTION_ZeroTextLength);

                            if (resZeroTextLength == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                    // 長さが未入力
                    else
                    {
                        DialogResult resNoTextLength = MessageBoxManager.ShowMessageBoxQuestion2(
                            this, Properties.Resources.FormAddAndDeleteField_AF_QUESTION_NoTextLength);

                        if (resNoTextLength == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                }
            }
            // ファイルジオデータベース or パーソナルジオデータベース
            else if (GetTypeOfDataSource(sourceLayer) == TYPE_FGDB
                || GetTypeOfDataSource(sourceLayer) == TYPE_MDB
                || GetTypeOfDataSource(sourceLayer) == TYPE_SDE)
            {
                // 編集セッションチェック
                if (sourceWorkspaceEdit.IsBeingEdited() == true)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                            Properties.Resources.FormAddAndDeleteField_AF_WARNING_InEditing);
                    return false;
                }

                // フィールドの最大数チェック
                if (sourceFields.FieldCount > FGDB_MDB_MAX_FIELD_NUM)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormAddAndDeleteField_AF_WARNING_MaxFieldNum);
                    return false;
                }

                // フィールド名の入力チェック
                if (textBoxAddFieldName.Text.Length < 1)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormAddAndDeleteField_AF_WARNING_NoFieldName);
                    return false;
                }

                // フィールド名に " が含まれる場合エラー
                if (textBoxAddFieldName.Text.Contains("\""))
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormAddAndDeleteField_AF_WARNING_FieldNameContainsDoubleQuotes);
                    return false;
                }

                // エイリアス名のチェック
                if (textBoxAddFieldLength.Text.Length > 0)
                {
                    if (textBoxAddFieldAlias.Text.Length > FGDB_MDB_MAX_ALIAS_NAME)
                    {
                        MessageBoxManager.ShowMessageBoxWarining(this,
                            Properties.Resources.FormAddAndDeleteField_AF_WARNING_MaxAliasNameLength
                            + "[ "
                            + FGDB_MDB_MIN_ALIAS_NAME
                            + " - "
                            + FGDB_MDB_MAX_ALIAS_NAME
                            + Properties.Resources.FormAddAndDeleteField_AF_WARNING_StringCount
                            + " ]");
                        return false;
                    }
                }

                // Text型
                if (addFieldTypeComboItem.Id == (int)esriFieldType.esriFieldTypeString)
                {
                    // 長さのチェック
                    if (textBoxAddFieldLength.Text.Length > 0)
                    {
                        int fieldLength = -1;

                        try
                        {
                            fieldLength = int.Parse(textBoxAddFieldLength.Text);
                        }
                        catch
                        {
                            MessageBoxManager.ShowMessageBoxWarining(this,
                                Properties.Resources.FormAddAndDeleteField_AF_WARNING_MaxStringLength
                                + "[ "
                                + FGDB_MDB_MIN_STRING_LENGTH
                                + " - "
                                + FGDB_MDB_MAX_STRING_LENGTH
                                + " ]");
                            return false;
                        }

                        if (fieldLength > FGDB_MDB_MAX_STRING_LENGTH)
                        {
                            MessageBoxManager.ShowMessageBoxWarining(this,
                                Properties.Resources.FormAddAndDeleteField_AF_WARNING_MaxStringLength
                                + "[ "
                                + FGDB_MDB_MIN_STRING_LENGTH
                                + " - "
                                + FGDB_MDB_MAX_STRING_LENGTH
                                + " ]");
                            return false;
                        }

                        // 長さに0が入力
                        if (int.Parse(textBoxAddFieldLength.Text) == 0)
                        {
                            DialogResult resZeroTextLength = MessageBoxManager.ShowMessageBoxQuestion2(
                                this, Properties.Resources.FormAddAndDeleteField_AF_QUESTION_ZeroTextLength);

                            if (resZeroTextLength == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                    // 長さが未入力
                    else
                    {
                        DialogResult resNoTextLength = MessageBoxManager.ShowMessageBoxQuestion2(
                            this, Properties.Resources.FormAddAndDeleteField_AF_QUESTION_NoTextLength);

                        if (resNoTextLength == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                this.Close();
            }

            return true;
        }

        /// <summary>
        /// 入力パラメータのチェック(フィールドの削除)
        /// </summary>
        private bool DeleteFieldCheckParameter()
        {
            ILayer sourceLayer = this.mainFrm.SelectedLayer;
            IFeatureLayer sourceFeatureLayer = (IFeatureLayer)sourceLayer;
            IFeatureClass sourceFeatureClass = sourceFeatureLayer.FeatureClass;

            IWorkspace sourceWorkspace = Common.LayerManager.getWorkspace(sourceFeatureClass);
            IWorkspaceEdit sourceWorkspaceEdit = (IWorkspaceEdit)sourceWorkspace;

            // 編集セッションチェック
            if (sourceWorkspaceEdit.IsBeingEdited() == true)
            {
                MessageBoxManager.ShowMessageBoxWarining(this,
                        Properties.Resources.FormAddAndDeleteField_DF_WARNING_InEditing);
                return false;
            }

            // 削除対象フィールドの選択状況をチェック
            if (comboBoxDeleteField.SelectedIndex == -1)
            {
                MessageBoxManager.ShowMessageBoxWarining(this,
                    Properties.Resources.FormAddAndDeleteField_DF_WARNING_NoSelectedDeleteField);
                return false;
            }

            // シェープファイル
            // OID,Shapeフィールド以外に1フィールド必要
            if (GetTypeOfDataSource(sourceLayer) == TYPE_SHAPE)
            {
                if (comboBoxDeleteField.Items.Count == 1)
                {
                    MessageBoxManager.ShowMessageBoxWarining(this,
                    Properties.Resources.FormAddAndDeleteField_DF_WARNING_RequireField);
                    return false;
                }
            }

            // 削除確認
            FieldComboItem deleteFieldComboItem = (FieldComboItem)comboBoxDeleteField.SelectedItem;

            DialogResult resDeleteField =
                MessageBoxManager.ShowMessageBoxQuestion2(this,
                Properties.Resources.FormAddAndDeleteField_DF_QUESTION_NoUndo
                + Properties.Resources.FormAddAndDeleteField_DF_QUESTION_SelectedField
                + deleteFieldComboItem.Field.AliasName
                + Properties.Resources.FormAddAndDeleteField_DF_QUESTION_DeleteField);

            if (resDeleteField == DialogResult.Cancel)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
