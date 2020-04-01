using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Linq;
using System.Diagnostics;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// 属性検索
    ///
    /// </summary>
    /// <history>
    ///  2010/11/01 新規作成 
    ///  2011/03/10 検索条件入力リストボックス上段の表示用検索SQL文表示修正
    ///  2011/04/05 フィールド名一覧でのフィールド名をダブルクォーテーションで囲み、
    ///             (フィールド名の先頭が数値や、全角スペースである場合の対応)
    ///             個別値取得時には取り除く
    /// </history>
    public partial class FormAttributeSearch : Form
    {
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        #region selection methods string
        private const string CREATE_NEW_SELECTION_SET =
            "新しい選択セットの作成";

        private const string ADD_TO_CURRENT_SELECTION_SET =
            "現在の選択セットに追加";

        private const string DELETE_FROM_CURRENT_SELECTION_SET =
            "現在の選択セットから削除";

        private const string SELECT_FROM_CURRENT_SELECTION_SET =
            "現在の選択セットから絞り込み選択";

        private const string DATA_SOURCE_MDB = ".mdb";
        private const string DATA_SOURCE_FGDB = ".gdb";
        private const string DATA_SOURCE_SHP = ".shp";
        private const string DATA_SOURCE_SDE = ".sde";	// ArcSDE
        private const string DATA_SOURCE_RDB = ".qcf";	// Remote DB
        private const string DATA_SOURCE_OLE = ".odc";	// OLE DB

        //private int IndValDisptMax =
        //    Properties.Settings.Default.IndividualValueDisplayMax;

        // 条件文入力テクストボックスへの追加時の改行文字削除用
        private char[] RETURN_NEWLINE_CHAR = { '\r', '\n' };

        private const string INDIVIDUAL_VALUE_DISPLAY_MAX = "個別値取得最大数";

        #endregion

        private Common.OptionSettings	settingsFile = null;
        private string					_strWSType = null;

        /// <summary>
        /// 個別値取得最大件数
        /// </summary>
        private int IndValDisptMax = -1;

        /// <summary>
        /// 属性検索用
        /// </summary>
        private string m_targetFieldName;

        // 既定のﾚｲﾔｰ(単独ﾃｰﾌﾞﾙ)
        private object _objTargetLayer = null;

        /// <summary>
        /// スレッド用
        /// </summary>
        private Thread threadTask = null;
        private delegate void RethrowExceptionDelegate(Exception ex);
        private delegate void RethrowCOMExceptionDelegate(COMException comex);

        /// <summary>
        /// プログレスバーフォーム
        /// </summary>
        private FormProgressManager frmProgress = null;

        /// <summary>
        /// アイテム変更イベント
        /// </summary>
        private bool changeLayerNames = false;
        private bool changeSelectionMethods = false;
        private bool changeCriteria = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl">マップコントロール</param>
        /// <param name="mainFrm">メインフォーム</param>
        public FormAttributeSearch(
            ESRI.ArcGIS.Controls.IMapControl3 mapControl, Ui.MainForm mainFrm, object TargetLayer)
        {
            InitializeComponent();

            this.m_mapControl = mapControl;
            this.mainFrm = mainFrm;
            this.labelMessage.Visible = false;

            // 既定ﾚｲﾔを保存
            if(TargetLayer == null) {
				// TOC選択中のﾚｲﾔｰをﾋﾟｯｸｱｯﾌﾟ
				this._objTargetLayer = this.mainFrm.SelectedLayer;
				// ない場合は、ﾄｯﾌﾟ･ﾌｨｰﾁｬｰﾚｲﾔｰ
				if(this._objTargetLayer == null) {
					LayerManager		clsLM = new LayerManager();
					List<IFeatureLayer>	agFLs = clsLM.GetFeatureLayers(this.m_mapControl.Map);
					if(agFLs.Count > 0) {
						this._objTargetLayer = agFLs[0];
					}
				}
            }
            else {
				this._objTargetLayer = TargetLayer;
            }

            if (!InitializeForm())
            {
                this.Close();
            }
        }

        #region private methods

        /// <summary>
        /// 初期化
        /// </summary>
        private bool InitializeForm()
        {
            if (ApplicationInitializer.IsUserSettingsExists() == false)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_FileNotExist
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                return false;
            }

            try
            {
                // 設定の読み込み
                settingsFile = new Common.OptionSettings();
                IndValDisptMax = Int32.Parse(settingsFile.IndividualValueDisplayMax);
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + INDIVIDUAL_VALUE_DISPLAY_MAX + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                return false;
            }

            return true;
        }

        /// <summary>
        /// ソースレイヤ名称の設定
        /// </summary>
        private void SetSourceLayerNames()
        {
            comboBoxLayerNames.Items.Clear();

            // (通常)ﾌｨｰﾁｬｰ･ﾚｲﾔｰ対象
            if(this._objTargetLayer == null || this._objTargetLayer is ILayer) {
				ESRIJapan.GISLight10.Common.LayerManager layerManager =
					new ESRIJapan.GISLight10.Common.LayerManager();

				List<IFeatureLayer> fcLayerList =
					layerManager.GetFeatureLayers(m_mapControl.Map);

				if (fcLayerList.Count > 0)
				{
					int	intSelLayer = 0;
					for (int i = 0; i < fcLayerList.Count; i++)
					{
						ESRIJapan.GISLight10.Common.LayerComboItem layerItem =
							new ESRIJapan.GISLight10.Common.LayerComboItem(fcLayerList[i]);

						comboBoxLayerNames.Items.Add(layerItem);

						// 既定ﾚｲﾔｰの位置を保存
						if(this._objTargetLayer == fcLayerList[i]) {
							intSelLayer = i;
						}
					}
					if (comboBoxLayerNames.Items.Count > 0)
					{
						comboBoxLayerNames.SelectedIndex = intSelLayer;
					}
				}
			}
			// ﾃｰﾌﾞﾙ対象
			else {
				comboBoxLayerNames.Items.Add((this._objTargetLayer as IStandaloneTable).Name);
				comboBoxLayerNames.SelectedIndex = 0;
			}
        }

        /// <summary>
        /// 選択方法文字列を設定
        /// </summary>
        private void SetSelectionMethodsString()
        {
            comboBoxSelectionMethods.Items.Clear();
            comboBoxSelectionMethods.Items.Add(CREATE_NEW_SELECTION_SET);
            comboBoxSelectionMethods.Items.Add(ADD_TO_CURRENT_SELECTION_SET);
            comboBoxSelectionMethods.Items.Add(DELETE_FROM_CURRENT_SELECTION_SET);
            comboBoxSelectionMethods.Items.Add(SELECT_FROM_CURRENT_SELECTION_SET);
            comboBoxSelectionMethods.SelectedIndex = 0;
        }

        /// <summary>
        /// 指定フィールドが文字列型か判定
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool IsStringField(IField field)
        {
            if (field == null) return false;
            return (field.Type.Equals(esriFieldType.esriFieldTypeString));
        }

        /// <summary>
        /// 指定フィールドが数値型か判定
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool IsNumericField(IField field)
        {
            if (field == null) return false;
            if (field.Type.Equals(esriFieldType.esriFieldTypeSmallInteger) ||
                field.Type.Equals(esriFieldType.esriFieldTypeInteger) ||
                field.Type.Equals(esriFieldType.esriFieldTypeSingle) ||
                field.Type.Equals(esriFieldType.esriFieldTypeDouble) ||
                field.Type.Equals(esriFieldType.esriFieldTypeOID))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// コンボボックスで選択されているレイヤを取得
        /// </summary>
        /// <returns></returns>
        private object GetSelectedLayer() {
			object objSelLayer = null;

            if(this.comboBoxLayerNames.SelectedIndex >= 0) {
				// IFeatureLayer
				if(this.comboBoxLayerNames.SelectedItem is LayerComboItem) {
					// コンボボックス選択アイテム取得
					ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
						(ESRIJapan.GISLight10.Common.LayerComboItem)comboBoxLayerNames.SelectedItem;
					objSelLayer = selectedLayerItem.Layer;
				}
				else {
					// 単独ﾃｰﾌﾞﾙ
					objSelLayer = this._objTargetLayer;
				}
            }

            return objSelLayer;
        }

        /// <summary>
        /// 選択レイヤーのフィールド名称を設定
        /// </summary>
        private void SetFieldNames()
        {
            listBoxFieldNames.Items.Clear();

            // フィールド一覧の取得
			object	objLayer = null;
            if(this.comboBoxLayerNames.SelectedItem is LayerComboItem) {
				objLayer = this.GetSelectedLayer();
            }
            else {
				objLayer = this._objTargetLayer;
            }

            // ﾌｨｰﾙﾄﾞ･ｱｲﾃﾑを取得 (ﾊﾞｲﾅﾘ型を除く)
            Common.FieldComboItem[]	cmbFlds = this.mainFrm.GetFieldItems(objLayer as ITableFields, true, false, false, true,
													esriFieldType.esriFieldTypeSmallInteger,
													esriFieldType.esriFieldTypeInteger,
													esriFieldType.esriFieldTypeSingle,
													esriFieldType.esriFieldTypeDouble,
													esriFieldType.esriFieldTypeString,
													esriFieldType.esriFieldTypeDate,
													esriFieldType.esriFieldTypeOID);
			listBoxFieldNames.Items.AddRange(cmbFlds);
            if (listBoxFieldNames.Items.Count > 0)
            {
                listBoxFieldNames.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 検索条件入力リストボックス上段の表示用検索SQL文の初期設定
        /// </summary>
        private void SetDefaultSqlString(int itemIndex)
        {
            if (comboBoxLayerNames.Items.Count > 0)
            {
                labelSqlString.Text = "SELECT * FROM " +
                    comboBoxLayerNames.Items[itemIndex].ToString() + " WHERE ";
            }
            else
            {
                labelSqlString.Text = "";
            }

            richTextBoxCriteria.Text = string.Empty;
        }

        /// <summary>
        /// 数値タイプ内容を格納
        /// </summary>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <param name="arr"></param>
        private void GetNumericValue(
            IField field, string val, System.Collections.ArrayList arr)
        {
            if (val != null && val.Length > 0)
            {
                if (field.Type.Equals(esriFieldType.esriFieldTypeSmallInteger) ||
                    field.Type.Equals(esriFieldType.esriFieldTypeInteger) ||
                    field.Type.Equals(esriFieldType.esriFieldTypeOID))
                {
                    arr.Add(Convert.ToInt64(val));
                }
                else
                {
                    if (field.Type.Equals(esriFieldType.esriFieldTypeSingle) ||
                        field.Type.Equals(esriFieldType.esriFieldTypeDouble))
                    {
                        arr.Add(Convert.ToDouble(val));
                    }
                }
            }
        }

        /// <summary>
        /// MDB DateTime Value
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strbldr"></param>
        private void GetMDBDateTimeValue(DateTime dt, StringBuilder strbldr)
        {
            strbldr.Append("#");
            strbldr.Append(dt.Month.ToString("d2"));
            strbldr.Append("-");
            strbldr.Append(dt.Day.ToString("d2"));
            strbldr.Append("-");
            strbldr.Append(dt.Year.ToString("d4"));
            strbldr.Append(" ");
            strbldr.Append(dt.Hour.ToString("d2"));
            strbldr.Append(":");
            strbldr.Append(dt.Minute.ToString("d2"));
            strbldr.Append(":");
            strbldr.Append(dt.Second.ToString("d2"));
            strbldr.Append("#");
        }

        /// <summary>
        /// FGDB DateTime Vaue
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strbldr"></param>
        private void GetFGDBDateTimeValue(DateTime dt, StringBuilder strbldr)
        {
            strbldr.Append("date '");
            strbldr.Append(dt.Year.ToString("d4"));
            strbldr.Append("-");
            strbldr.Append(dt.Month.ToString("d2"));
            strbldr.Append("-");
            strbldr.Append(dt.Day.ToString("d2"));
            strbldr.Append(" ");
            strbldr.Append(dt.Hour.ToString("d2"));
            strbldr.Append(":");
            strbldr.Append(dt.Minute.ToString("d2"));
            strbldr.Append(":");
            strbldr.Append(dt.Second.ToString("d2"));
            strbldr.Append("'");
        }

        /// <summary>
        /// Shape Date Value
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strbldr"></param>
        private void GetSHAPEDateValue(DateTime dt, StringBuilder strbldr)
        {
            strbldr.Append("date '");
            strbldr.Append(dt.Year.ToString("d4"));
            strbldr.Append("-");
            strbldr.Append(dt.Month.ToString("d2"));
            strbldr.Append("-");
            strbldr.Append(dt.Day.ToString("d2"));
            strbldr.Append("'");
        }

        /// <summary>
        /// データソースタイプ別日付内容を格納
        /// </summary>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <param name="arr"></param>
        /// <param name="comReleaser"></param>
        /// <param name="pLayer"></param>
        private void GetDateTimeValue(
            IField field, string val, System.Collections.ArrayList arr,
            ESRI.ArcGIS.ADF.ComReleaser comReleaser, ILayer pLayer)
        {
            DateTime dt = Convert.ToDateTime(val);
            if (dt != null)
            {
                StringBuilder strbldr = new StringBuilder();
                string dataType = this._strWSType;
                //string dataType = WhatTypeOfDataSource(pLayer as IFeatureLayer, field as IField2);

                // ﾃｰﾌﾞﾙ結合状態を取得
                bool	blnIsTblRel = ((pLayer as IDisplayTable).DisplayTable is IRelQueryTable);

                if (dataType.Equals(DATA_SOURCE_MDB)) {
					// ※ﾃｰﾌﾞﾙ結合時はMDBの修飾子は使用不可
					if(!blnIsTblRel) {
						// MDB仕様
						GetMDBDateTimeValue(dt, strbldr);
                    }
                    else {
						// ﾌｧｲﾙ･ﾍﾞｰｽDBと同様
						GetSHAPEDateValue(dt, strbldr);
                    }
                }
                else if (dataType.Equals(DATA_SOURCE_FGDB))
                {
                    GetFGDBDateTimeValue(dt, strbldr);
                }
                else if (dataType.Equals(DATA_SOURCE_SHP) || dataType.Equals(DATA_SOURCE_RDB) || dataType.Equals(DATA_SOURCE_SDE) || dataType.Equals(DATA_SOURCE_OLE))
                {
                    GetSHAPEDateValue(dt, strbldr);
                }
                arr.Add(strbldr.ToString());
            }
        }

        /// <summary>
        /// 数値タイプ以外（文字列、日付）内容を格納
        /// </summary>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <param name="arr"></param>
        /// <param name="comReleaser"></param>
        /// <param name="pLayer"></param>
        private void GetOtherValue(
            IField field, string val, System.Collections.ArrayList arr,
            ESRI.ArcGIS.ADF.ComReleaser comReleaser, ILayer pLayer)
        {
            if (field.Type != esriFieldType.esriFieldTypeGeometry &&
                field.Type != esriFieldType.esriFieldTypeGUID &&
                field.Type != esriFieldType.esriFieldTypeBlob &&
                field.Type != esriFieldType.esriFieldTypeRaster)
            {
                if (val != null && val.Length > 0)
                {
                    if (field.Type == esriFieldType.esriFieldTypeDate)
                    {
                        GetDateTimeValue(field, val, arr, comReleaser, pLayer);
                    }
                    else
                    {
                        arr.Add(val);
                    }
                }
                else
                {
                    arr.Add("NULL");
                }
            }
        }



        /// <summary>
        /// 指定レイヤの指定属性の属性値コレクションを取得,
        /// 必要なソートをして返す
        /// </summary>
        /// <param name="targetLayer"></param>
        /// <param name="targetFieldName"></param>
        /// <returns></returns> //System.Collections.ICollection
        private System.Collections.ArrayList GetIndividualValues(
            IGeoFeatureLayer targetLayer, string targetFieldName)
        {
            using (
                ESRI.ArcGIS.ADF.ComReleaser comReleaser =
                    new ESRI.ArcGIS.ADF.ComReleaser())
            {

                IFeatureClass fcObj =
                        targetLayer.DisplayFeatureClass;

                IQueryFilter queryFilter = new QueryFilterClass();
                //queryFilter.WhereClause =
                //    fcObj.OIDFieldName + " <= " +
                //    Properties.Settings.Default.AttributeTableDisplayOIDMax.ToString();

                comReleaser.ManageLifetime(queryFilter);

                // 2011/06/17 modify
                //IFeatureCursor fCursor = fcObj.Search(queryFilter, false);
                IFeatureCursor fCursor = targetLayer.SearchDisplayFeatures(queryFilter, false);

                comReleaser.ManageLifetime(fCursor);

                IFeature ifeat = fCursor.NextFeature();

                System.Collections.Hashtable fldNamesTbl =
                    new System.Collections.Hashtable(IndValDisptMax);

                if (ifeat.Fields.FindField(targetFieldName) < 0) return null;
                IField field =
                    ifeat.Fields.get_Field(ifeat.Fields.FindField(targetFieldName));

                if (ifeat == null) return null;

                int ii = 0;

                while (ifeat != null)
                {
                    if (ifeat.get_Value(ifeat.Fields.FindField(targetFieldName)) != null)
                    {
                        string tempVal = ifeat.get_Value(
                                ifeat.Fields.FindField(targetFieldName)).ToString();

                        if (tempVal != null)
                        {
                            // 空白も対象にする
                            if (fldNamesTbl.Count == 0 || !(fldNamesTbl.ContainsKey(tempVal)))
                            {
                                fldNamesTbl.Add(tempVal, fldNamesTbl);
                                if (fldNamesTbl.Count >= IndValDisptMax) break;
                            }
                        }
                    }

                    ii += 1;

                    ifeat = fCursor.NextFeature();
                }



                if (fldNamesTbl.Count > 0)
                {
                    System.Collections.ArrayList arr =
                        new System.Collections.ArrayList();

                    foreach (string val in fldNamesTbl.Keys)
                    {
                        if (IsStringField(field))
                        {
                            if (val != null && val.Length > 0)
                            {
                                arr.Add("\'" + val + "\'");
                            }
                            else
                            {
                                arr.Add("NULL");
                            }
                        }
                        else
                        {
                            if (IsNumericField(field))
                            {
                                GetNumericValue(field, val, arr);
                            }
                            else
                            {
                                GetOtherValue(field, val, arr, comReleaser, targetLayer as ILayer);
                            }
                        }
                    }
                    arr.Sort();
                    for (int i = 0; i < arr.Count; i++)
                    {
                        arr[i] = arr[i].ToString();
                    }

                    return arr;
                }
                return null;
            }
        }

        /// <summary>
        /// 条件文に追加
        /// </summary>
        /// <param name="addString"></param>
        private void SetCriteria(string addString)
        {
			// 接続詞判定
			string[]	strConOpes = {"=","<>",">","<",">=","<=","Like","And","Or","Not","Is"};
			// 他
			string[]	strWCs = {"_","%","?","*","( )"};
			// ｶｰｿﾙ位置調整値
			int			intPosSub = 0;

			// 選択文字列を消去
			StringBuilder sbWhere = new StringBuilder();
			if(this.richTextBoxCriteria.SelectionLength > 0) {
				if(this.richTextBoxCriteria.SelectionStart <= 0) {
					sbWhere.Append(this.richTextBoxCriteria.Text.Substring(this.richTextBoxCriteria.SelectionLength));
				}
				else {
					sbWhere.Append(this.richTextBoxCriteria.Text.Substring(0, this.richTextBoxCriteria.SelectionStart));
					if(this.richTextBoxCriteria.SelectionStart + this.richTextBoxCriteria.SelectionLength < this.richTextBoxCriteria.Text.Length) {
						sbWhere.Append(this.richTextBoxCriteria.Text.Substring(this.richTextBoxCriteria.SelectionStart + this.richTextBoxCriteria.SelectionLength));
					}
				}
			}
			else {
				sbWhere.Append(this.richTextBoxCriteria.Text);
			}

			// 挿入ﾃｷｽﾄの調整
			if(strWCs.Contains(addString)) {
				// 演算子
				addString = addString.Replace(" ", "");
				if(addString == "()") {
					--intPosSub;
				}
			}
			else {
				// 接続詞の調整
				if(strConOpes.Contains(addString)) {
					// 大文字化
					addString = addString.ToUpper();
				}

				// ﾌｨｰﾙﾄﾞ名, 値などの調整
				if(this.richTextBoxCriteria.SelectionLength <= 0) {
					if(this.richTextBoxCriteria.SelectionStart > 0) {
						// 左をﾁｪｯｸ
						if(sbWhere.ToString()[this.richTextBoxCriteria.SelectionStart - 1] != ' ') {
							addString = " " + addString;
						}
						// 右をﾁｪｯｸ
						if(this.richTextBoxCriteria.SelectionStart < this.richTextBoxCriteria.Text.Length
							&& sbWhere.ToString()[this.richTextBoxCriteria.SelectionStart] != ' ') {
							addString += " ";
						}
					}
				}
			}

			// 文字挿入
			sbWhere.Insert(this.richTextBoxCriteria.SelectionStart, addString);

			// ｶｰｿﾙ位置調整
			int	intSelStart = this.richTextBoxCriteria.SelectionStart + addString.Length + intPosSub;

			// ｺﾝﾄﾛｰﾙ調整
			this.richTextBoxCriteria.Text = sbWhere.ToString();
			this.richTextBoxCriteria.SelectionStart = intSelStart;

			return;

            if (richTextBoxCriteria.Text == null ||
                richTextBoxCriteria.Text.Length == 0)
            {
                richTextBoxCriteria.Text = addString.TrimEnd(RETURN_NEWLINE_CHAR);

                richTextBoxCriteria.SelectionStart = richTextBoxCriteria.SelectionStart + addString.Length;
            }
            else
            {
                // ワイルドカード等をカレントカーソル位置に挿入
                int charIndex = richTextBoxCriteria.SelectionStart;

                StringBuilder cstr = new StringBuilder(richTextBoxCriteria.Text);

                //if (addString.Equals("_") ||
                //    addString.Equals("?") ||
                //    addString.Equals("*") ||
                //    addString.Equals("%"))
                //{

                //    richTextBoxCriteria.Text =
                //        richTextBoxCriteria.Text.Insert(
                //            charIndex, addString);

                //    richTextBoxCriteria.SelectionStart = charIndex + 1;
                //}
                //else
                //{
                //    cstr.Append(" ");
                //    cstr.Append(addString.TrimEnd(RETURN_NEWLINE_CHAR));

                //    richTextBoxCriteria.Text = cstr.ToString();
                //}

                if (addString.Equals("_") ||
                    addString.Equals("?") ||
                    addString.Equals("*") ||
                    addString.Equals("%"))
                {
                    richTextBoxCriteria.Text =
                            richTextBoxCriteria.Text.Insert(charIndex, addString);

                    richTextBoxCriteria.SelectionStart = charIndex + addString.Length;
                }
                else
                {
                    richTextBoxCriteria.Text =
                            richTextBoxCriteria.Text.Insert(charIndex, " ");

                    richTextBoxCriteria.Text =
                            richTextBoxCriteria.Text.Insert(charIndex + 1, addString);

                    richTextBoxCriteria.SelectionStart = charIndex + addString.Length + 1;
                }
            }
        }

        /// <summary>
        /// 空白時点までが属性名
        /// </summary>
        /// <param name="remvDblQtn">ダブルクォーテーション取り除き指定</param>
        /// <returns>属性名</returns>
        private string GetTargetFieldName(bool remvDblQtn)
        {
            try
            {
                // ﾌｨｰﾙﾄﾞ修飾子の設定
                string	strFldFix1 = this._strWSType.Equals(DATA_SOURCE_MDB) ? "[" : "\"";
                string	strFldFix2 = this._strWSType.Equals(DATA_SOURCE_MDB) ? "]" : "\"";

                // ﾌｨｰﾙﾄﾞ名を取得
                string targetFieldName =
                    //listBoxFieldNames.Items[listBoxFieldNames.SelectedIndex].ToString();
					string.Format("{1}{0}{2}", (listBoxFieldNames.Items[listBoxFieldNames.SelectedIndex] as Common.FieldComboItem).FieldName, strFldFix1, strFldFix2);

                if (targetFieldName.IndexOf(' ') > 0)
                {
                    targetFieldName =
                        targetFieldName.Substring(0, targetFieldName.IndexOf(' '));
                }

                // 2011/04/05 -->
                if (remvDblQtn)
                {
                    char[] chArray = targetFieldName.ToCharArray();
                    StringBuilder strtmp = new StringBuilder();

                    for (int i = 0; i < chArray.Length; i++)
                    {
                        if (this._strWSType.Equals(DATA_SOURCE_MDB))
                        {
                            if (chArray[i] != '[' && chArray[i] != ']')
                            {
                                strtmp.Append(chArray[i]);
                            }
                        }
                        else
                        {
                            if (chArray[i] != '\\' && chArray[i] != '"')
                            {
                                strtmp.Append(chArray[i]);
                            }
                        }
                    }
                    targetFieldName = strtmp.ToString();
                }
                //<--

                return targetFieldName;
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// メイン画面のマップのフィーチャ選択状態の変更
        /// </summary>
        /// <param name="selFeat"></param>
        private void SetMainMapSelectionChanged(IFeatureSelection selFeat)
        {
            selFeat.SelectionChanged();
            this.m_mapControl.ActiveView.Refresh();
            this.mainFrm.SetMapStateChanged();
        }

        /// <summary>
        /// 現状選択セットのクリア
        /// </summary>
        private void ClearCurrentSelectionSet(IFeatureSelection selFeat)
        {
            selFeat.Clear();
            SetMainMapSelectionChanged(selFeat);
        }

        /// <summary>
        /// OKボタンまたは適用ボタン時の入力チェック
        /// </summary>
        /// <returns></returns>
        private bool IsValidCriteria()
        {
            if (richTextBoxCriteria.Text == null ||
                richTextBoxCriteria.Text.Length == 0)
            {
                return false;
            }
            return true;
        }

        #region ProgressDialogのクラスを使い、ロジックも書き直し
        /// <summary>
        /// 新規選択セットの作成
        /// </summary>
        private void CreateNewSelectionSet2(int SelectionResultID) {
            ProgressDialog pd = new ProgressDialog();
            pd.Minimum = 0;
            pd.Maximum = 2;
            pd.CancelEnable = false;

            string	strMsg = "";

            esriSelectionResultEnum	agEnumSelRes = esriSelectionResultEnum.esriSelectionResultNew;
            switch(SelectionResultID) {
            case 1:		// 追加
				agEnumSelRes = esriSelectionResultEnum.esriSelectionResultAdd;
				strMsg = ADD_TO_CURRENT_SELECTION_SET;
				break;
            case 2:		// 削除
				agEnumSelRes = esriSelectionResultEnum.esriSelectionResultSubtract;
				strMsg = DELETE_FROM_CURRENT_SELECTION_SET;
				break;
            case 3:		// 絞り込み
				agEnumSelRes = esriSelectionResultEnum.esriSelectionResultAnd;
				strMsg = SELECT_FROM_CURRENT_SELECTION_SET;
				break;
			default:
				strMsg = CREATE_NEW_SELECTION_SET;
				break;
            }

            try {
				pd.Title = string.Format("{0}:[{1}]", this.Text, strMsg);  //"属性検索"
                pd.Show(this);

                IQueryFilter queryFilter = new QueryFilterClass();
				queryFilter.WhereClause = richTextBoxCriteria.Text;

				object	objSelLayer = this.GetSelectedLayer();
				if(objSelLayer is IFeatureLayer) {
					IGeoFeatureLayer	featLayer = objSelLayer as IGeoFeatureLayer;
					IFeatureSelection	selFeat = (IFeatureSelection)featLayer;

					pd.Value = 1;
					selFeat.SelectFeatures(queryFilter, agEnumSelRes, false);
					pd.Value = 2;

					//selFeat.SelectionChanged();	// ※ｲﾍﾞﾝﾄは発行されない ( 2014/09/10)

					IActiveView			pActiveView = m_mapControl.ActiveView;
					pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);

					// ※SelectionChanged ｲﾍﾞﾝﾄを発行する場合、Editorの選択状態を得るには、IMap.SelectFeature を使用する
					// 編集ｾｸｼｮﾝにも選択状態を反映させる
					if(this.mainFrm.IsEditMode && (featLayer as IDataset).Workspace.Equals(mainFrm.EditWorkspace)) {
						IMap		agMap = this.m_mapControl.Map;
						//ICursor		agCur;
						//IFeature	agFeat;

						// 強制的に SelectionChanged ｲﾍﾞﾝﾄを発行させる
						agMap.SelectFeature(featLayer, null);

						// ※当初、下記ｺｰﾄﾞを検証していたが、選択が多数の場合、ひどいことになるので上記で解消。
						//// 選択を追加
						//if(selFeat.SelectionSet.Count > 0) {
						//    // 選択ﾌｨｰﾁｬｰを通知
						//    //selFeat.SelectionSet.Search(null, false, out agCur);
						//    //while((agFeat = agCur.NextRow() as IFeature) != null) {
						//    //    agMap.SelectFeature(featLayer, agFeat);
						//    //}
						//    //Marshal.ReleaseComObject(agCur);
						//    //agCur = null;
						//}
						//else {
						//    // 編集ﾚｲﾔｰに選択ﾌｨｰﾁｬｰがないことを通知
						//    agMap.SelectFeature(featLayer, null);
						//}
					}
                }
                else {
					pd.Value = 1;
					ITableSelection	agTblSel = this._objTargetLayer as ITableSelection;
					agTblSel.SelectRows(queryFilter, agEnumSelRes, false);
					pd.Value = 2;
                }
            }
            catch (Exception ex) {
                throw;
            }
            finally {
                pd.Close();
            }
        }

        /// <summary>
        /// 個別値をセット
        /// </summary>
        /// <param name="targetLayer"></param>
        /// <param name="targetFieldName"></param>
        private void SetIndividualValues(object targetLayer, string targetFieldName)
        {
            ProgressDialog pd = new ProgressDialog();
            try {
                using(ComReleaser comReleaser = new ComReleaser()) {

                    IQueryFilter queryFilter = new QueryFilterClass();
                    ICursor fCursor;

                    int max = 0;
                    ITableDefinition	agTblDef;

                    if(targetLayer is IGeoFeatureLayer) {
						IGeoFeatureLayer	agGeoFL = targetLayer as IGeoFeatureLayer;
						IFeatureClass		fcObj = agGeoFL.DisplayFeatureClass;
						fCursor = agGeoFL.SearchDisplayFeatures(queryFilter, true) as ICursor;

						agTblDef = agGeoFL as ITableDefinition;
						queryFilter.WhereClause = agTblDef.DefinitionExpression;
						max = fcObj.FeatureCount(queryFilter);
                    }
                    else {
						IDisplayTable	agDisTbl = (this._objTargetLayer as IStandaloneTable) as IDisplayTable;
						fCursor = agDisTbl.SearchDisplayTable(queryFilter, true);

						agTblDef = agDisTbl as ITableDefinition;
						queryFilter.WhereClause = agTblDef.DefinitionExpression;
						max = agDisTbl.DisplayTable.RowCount(queryFilter);
                    }
                    comReleaser.ManageLifetime(fCursor);

                    pd.Minimum = 0;
                    pd.Maximum = max;
                    pd.Title = string.Format("{0}:[{1}]", this.Text, "個別値の取得中");  //"属性検索"
                    pd.Show(this);

                    IRow ifeat = fCursor.NextRow();
                    comReleaser.ManageLifetime(ifeat);

                    System.Collections.Hashtable fldNamesTbl =
                        new System.Collections.Hashtable(IndValDisptMax);

                    IFields agFlds = fCursor.Fields;
                    int ifield = agFlds.FindField(targetFieldName);
                    int ii = 0;

                    while (ifeat != null)
                    {
                        if (ifeat.get_Value(ifield) != null)
                        {
                            string tempVal = ifeat.get_Value(ifield).ToString();

                            if (tempVal != null)
                            {
                                // 空白も対象にする
                                if (fldNamesTbl.Count == 0 || !(fldNamesTbl.ContainsKey(tempVal)))
                                {
                                    fldNamesTbl.Add(tempVal, fldNamesTbl);
                                }
                            }
                        }

                        if (pd.Canceled)
                        {
                            pd.CancelEnable = false;
                            break;
                        }

                        ii += 1;
                        pd.Value = ii;
                        pd.Message = string.Format("{0} / {1}を取得中・・・。 個別値: {2}件", ii, max, fldNamesTbl.Count);

                        ifeat = fCursor.NextRow();
                    }

                    if (fldNamesTbl.Count > 0) // && !pd.Canceled)
                    {
                        ii = 0;
                        pd.Minimum = 0;
                        pd.Maximum = fldNamesTbl.Count;

                        IField field = agFlds.get_Field(ifield);
                        System.Collections.ArrayList arr = new System.Collections.ArrayList();

                        foreach (string val in fldNamesTbl.Keys)
                        {
                            ii += 1;
                            pd.Value = ii;
                            pd.Message = string.Format("{0}/{1}のソート中・・・", ii, fldNamesTbl.Count);

                            //if (pd.Canceled)
                            //{
                            //    break;
                            //}

                            if (IsStringField(field))
                            {
                                if (val != null && val.Length > 0)
                                {
                                    arr.Add("\'" + val + "\'");
                                }
                                else
                                {
                                    arr.Add("NULL");
                                }
                            }
                            else
                            {
                                if (IsNumericField(field))
                                {
                                    GetNumericValue(field, val, arr);
                                }
                                else
                                {
                                    GetOtherValue(field, val, arr, null, targetLayer as ILayer);
                                }
                            }
                        }
                        arr.Sort();

                        listBoxIndividualValues.Items.AddRange(arr.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                pd.Close();
            }
        }

        #endregion

        /// <summary>
        /// 新しい選択セットの作成
        /// 現状の選択セットが存在すればクリアしてから
        /// 新規作成
        /// </summary>
        private void CreateNewSelectionSet()
        {
            IFeatureLayer featLayer = GetSelectedLayer() as IFeatureLayer;
            IFeatureSelection selFeat = (IFeatureSelection)featLayer;

            ClearCurrentSelectionSet(selFeat);

            string criteria = richTextBoxCriteria.Text;

            // シリアライズ
            IXMLSerializer serializer = new XMLSerializerClass();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("FeatureLayer", featLayer);
            propertySet.SetProperty("Criteria", criteria);
            string serializeData = serializer.SaveToString(propertySet, null, null);

            Common.TaskInfo taskInfo = new Common.TaskInfo();
            taskInfo.SerializeData = serializeData;

            taskInfo.CallBackParam = new ParameterizedThreadStart(SelectionSetCallBackResult);

            // スレッド開始
            threadTask = new Thread(new ParameterizedThreadStart(CreateNewSelectionSetThread));
            threadTask.SetApartmentState(ApartmentState.STA);
            threadTask.IsBackground = true;
            threadTask.Start(taskInfo);

            // プログレスバー表示
            frmProgress = new FormProgressManager();
            frmProgress.Owner = this;
            frmProgress.SetTitle(this);
            frmProgress.SetMessage(Properties.Resources.FormAttributeSearch_WhenSearch);
            frmProgress.ShowDialog();
        }

        /// <summary>
        /// 現在の選択セットに追加
        /// 現状の選択セットが存在すれば
        /// それに追加
        /// 存在しなければ、新規作成
        /// </summary>
        private void AddSelectionSet()
        {
            string criteria = richTextBoxCriteria.Text;
            IFeatureLayer featLayer = GetSelectedLayer() as IFeatureLayer;

            // シリアライズ
            IXMLSerializer serializer = new XMLSerializerClass();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("FeatureLayer", featLayer);
            propertySet.SetProperty("Criteria", criteria);
            string serializeData = serializer.SaveToString(propertySet, null, null);

            Common.TaskInfo taskInfo = new Common.TaskInfo();
            taskInfo.SerializeData = serializeData;

            taskInfo.CallBackParam = new ParameterizedThreadStart(SelectionSetCallBackResult);

            // スレッド開始
            threadTask = new Thread(new ParameterizedThreadStart(AddSelectionSetThread));
            threadTask.SetApartmentState(ApartmentState.STA);
            threadTask.IsBackground = true;
            threadTask.Start(taskInfo);

            // プログレスバー表示
            frmProgress = new FormProgressManager();
            frmProgress.Owner = this;
            frmProgress.SetTitle(this);
            frmProgress.SetMessage(Properties.Resources.FormAttributeSearch_WhenSearch);
            frmProgress.ShowDialog();
        }

        /// <summary>
        /// 現状の選択セットから削除
        /// 現状の選択セットが存在し、
        /// 該当するフィーチャ存在すれば削除
        /// </summary>
        private void DeleteFromSelectionSet()
        {
            string criteria = richTextBoxCriteria.Text;
            IFeatureLayer featLayer = GetSelectedLayer() as IFeatureLayer;

            // シリアライズ
            IXMLSerializer serializer = new XMLSerializerClass();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("FeatureLayer", featLayer);
            propertySet.SetProperty("Criteria", criteria);
            string serializeData = serializer.SaveToString(propertySet, null, null);

            Common.TaskInfo taskInfo = new Common.TaskInfo();
            taskInfo.SerializeData = serializeData;

            taskInfo.CallBackParam = new ParameterizedThreadStart(SelectionSetCallBackResult);

            // スレッド開始
            threadTask = new Thread(new ParameterizedThreadStart(DeleteFromSelectionSetThread));
            threadTask.SetApartmentState(ApartmentState.STA);
            threadTask.IsBackground = true;
            threadTask.Start(taskInfo);

            // プログレスバー表示
            frmProgress = new FormProgressManager();
            frmProgress.Owner = this;
            frmProgress.SetTitle(this);
            frmProgress.SetMessage(Properties.Resources.FormAttributeSearch_WhenSearch);
            frmProgress.ShowDialog();
        }

        /// <summary>
        /// 絞り込み指定
        /// 選択セットの中に指定IDのデータが存在するか確認
        /// </summary>
        /// <param name="featLayer"></param>
        /// <param name="tID"></param>
        /// <param name="selFeat"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private bool IsExistsInSelectionQueryResult(
            IGeoFeatureLayer featLayer,
            int tID,
            IFeatureSelection selFeat,
            string criteria)
        {
            IFeatureCursor featCur = null;

            try
            {
                IFeatureClass featClass = featLayer.DisplayFeatureClass;

                // 絞り込み指定条件
                IQueryFilter queryFilter = new QueryFilterClass();
                criteria = criteria +
                    " AND (" + featClass.OIDFieldName + " = " + tID.ToString() + ")";
                if (criteria.Length > 0) queryFilter.WhereClause = criteria;

                // 2011/06/17 modify
                //featCur = featClass.Search(queryFilter, false);
                featCur = featLayer.SearchDisplayFeatures(queryFilter, false);

                IFeature checkFeature = featCur.NextFeature();

                while (checkFeature != null)
                {
                    string tid = checkFeature.OID.ToString();
                    if (tid.Equals(tID.ToString())) return true;
                    checkFeature = featCur.NextFeature();
                }
                return false;
            }
            catch (COMException comex)
            {
                throw comex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(featCur);
            }
        }

        /// <summary>
        /// 現在の選択セットから絞り込み検索
        /// 現状の選択セットが存在し
        /// 該当するフィーチャ存在すれば
        /// そのフィーチャを選択
        /// </summary>
        private void SelectFromCurrentSelectionSet()
        {
            string criteria = richTextBoxCriteria.Text;
            IFeatureLayer featLayer = GetSelectedLayer() as IFeatureLayer;

            // シリアライズ
            IXMLSerializer serializer = new XMLSerializerClass();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("FeatureLayer", featLayer);
            propertySet.SetProperty("Criteria", criteria);
            string serializeData = serializer.SaveToString(propertySet, null, null);

            Common.TaskInfo taskInfo = new Common.TaskInfo();
            taskInfo.SerializeData = serializeData;

            taskInfo.CallBackParam = new ParameterizedThreadStart(SelectionSetCallBackResult);

            // スレッド開始
            threadTask = new Thread(new ParameterizedThreadStart(SelectFromCurrentSelectionSetThread));
            threadTask.SetApartmentState(ApartmentState.STA);
            threadTask.IsBackground = true;
            threadTask.Start(taskInfo);

            // プログレスバー表示
            frmProgress = new FormProgressManager();
            frmProgress.Owner = this;
            frmProgress.SetTitle(this);
            frmProgress.SetMessage(Properties.Resources.FormAttributeSearch_WhenSearch);
            frmProgress.ShowDialog();
        }

        /// <summary>
        /// データソースタイプの判定(MDB or FGDB or Shape)
        /// </summary>
		/// <param name="comReleaser"></param>
        /// <param name="selectedLayer"></param>
        /// <returns></returns>
        private string WhatTypeOfDataSource(ESRI.ArcGIS.ADF.ComReleaser comReleaser, object selectedLayer) {
			string	strRet = "";

            // 対象の切り分け
            IWorkspace agWS = null;
			if(selectedLayer is ILayer) {
				agWS = Common.LayerManager.getWorkspace((selectedLayer as IFeatureLayer).FeatureClass);
			}
			else if(selectedLayer is IStandaloneTable) {
				agWS = ((selectedLayer as IStandaloneTable).Table as IDataset).Workspace;
			}

            // レイヤのデータソースタイプ別にワイルドカードなどの使用文字を切り分ける
            //comReleaser.ManageLifetime(agWS);

            string	strExt;
            switch(agWS.Type) {
            case esriWorkspaceType.esriLocalDatabaseWorkspace:	/* Local GDB */
				strExt = agWS.PathName.ToLower();
                if(strExt.Contains(DATA_SOURCE_MDB)) {
                    strRet = DATA_SOURCE_MDB;
                }
                else if(strExt.Contains(DATA_SOURCE_FGDB)) {
                    strRet = DATA_SOURCE_FGDB;
                }
                break;

            case esriWorkspaceType.esriFileSystemWorkspace:		/* Folder */
                strRet = DATA_SOURCE_SHP;
                break;

            case esriWorkspaceType.esriRemoteDatabaseWorkspace:	/* Remote DB */
				strExt = Path.GetExtension(agWS.PathName).ToLower();
				if(strExt == DATA_SOURCE_RDB) {
					strRet = DATA_SOURCE_RDB;
				}
				else if(strExt == DATA_SOURCE_SDE) {
					strRet = DATA_SOURCE_SDE;
				}
				else if(strExt == DATA_SOURCE_OLE) {
					strRet = DATA_SOURCE_OLE;
				}
                break;
            }
            Marshal.ReleaseComObject(agWS);
            agWS = null;

            return strRet;
        }

        /// <summary>
        /// 指定フィールドのワークスペース・タイプを判定します
        /// </summary>
        /// <param name="TargetLayer"></param>
        /// <param name="TargetField"></param>
        /// <returns></returns>
        private string WhatTypeOfDataSource(IFeatureLayer TargetLayer, IField2 TargetField) {
			string	strRet = "";

			// 対象ﾌｨｰﾙﾄﾞのﾜｰｸｽﾍﾟｰｽを判別する
			ITable		agTbl = (TargetLayer as IDisplayTable).DisplayTable;
			IWorkspace	agWS = null;
			if(!(agTbl is IRelQueryTable)) {
				agWS = Common.LayerManager.getWorkspace(TargetLayer.FeatureClass);
			}
			else {
				IDataset		agDS;
				IRelQueryTable	agRelQTbl = agTbl as IRelQueryTable;
				int				intFld;
				string			strFldName = FieldManager.GetSimpleFieldName(TargetField);
				while(agTbl is IRelQueryTable) {
					agRelQTbl = agTbl as IRelQueryTable;

					foreach(ITable agTblTemp in new ITable[] {agRelQTbl.SourceTable, agRelQTbl.DestinationTable}) {
						if(!(agTblTemp is IRelQueryTable)) {
							intFld = agTblTemp.Fields.FindField(strFldName);
							if(intFld >= 0) {
								agDS = agTblTemp as IDataset;
								if(agDS != null && TargetField.Name.StartsWith(agDS.BrowseName)) {
									agWS = agDS.Workspace;
									break;
								}
							}
						}
					}

					// 終了判定
					if(agWS != null) {
						break;
					}
					else {
						agTbl = agRelQTbl.SourceTable;
					}
				}
			}

            // レイヤのデータソースタイプ別にワイルドカードなどの使用文字を切り分ける
            string	strExt;
            switch(agWS.Type) {
            /* Local GDB */
            case esriWorkspaceType.esriLocalDatabaseWorkspace:
				strExt = agWS.PathName.ToLower();
                if(strExt.Contains(DATA_SOURCE_MDB)) {
                    strRet = DATA_SOURCE_MDB;
                }
                else if(strExt.Contains(DATA_SOURCE_FGDB)) {
                    strRet = DATA_SOURCE_FGDB;
                }
                break;

			/* Folder */
            case esriWorkspaceType.esriFileSystemWorkspace:
                strRet = DATA_SOURCE_SHP;
                break;

			/* Remote DB */
            case esriWorkspaceType.esriRemoteDatabaseWorkspace:
				strExt = Path.GetExtension(agWS.PathName).ToLower();
				if(strExt == DATA_SOURCE_RDB) {
					strRet = DATA_SOURCE_RDB;
				}
				else if(strExt == DATA_SOURCE_SDE) {
					strRet = DATA_SOURCE_SDE;
				}
				else if(strExt == DATA_SOURCE_OLE) {
					strRet = DATA_SOURCE_OLE;
				}
                break;
            }
            Marshal.ReleaseComObject(agWS);
            agWS = null;

            return strRet;
        }

        #endregion private methods

        /// <summary>
        /// ロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAttributeSearch_Load(object sender, EventArgs e)
        {
            bool dispMessage = false;

            try
            {
                SetSourceLayerNames();
                SetSelectionMethodsString();
            }
            catch (Exception ex)
            {
                dispMessage = true;
                Common.UtilityClass.DoOnError(ex);
                this.Close();
                //this.Dispose();
            }
            finally
            {
                if (dispMessage)
                {
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this, Properties.Resources.FormAttributeSearch_WhenLoad);
                }
            }
        }

        /// <summary>
        /// キャンセル時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 選択レイヤの個別値を取得して個別値リストボックスに設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetIndividualValue_Click(object sender, EventArgs e)
        {
            using (ESRI.ArcGIS.ADF.ComReleaser comReleaser = new ESRI.ArcGIS.ADF.ComReleaser()) {
                bool dispMessage = false;
                try {
                    this.Enabled = false;

                    listBoxIndividualValues.Items.Clear();
                    if (listBoxFieldNames.SelectedIndex >= 0)
                    {
                        string targetFieldName = GetTargetFieldName(true);

                        //// レコード数0の時
                        //IFeatureLayer featureLayer = (IFeatureLayer)selectedLayerItem.Layer;
                        //IQueryFilter queryFilter = new QueryFilterClass();
                        //if (featureLayer.FeatureClass.FeatureCount(queryFilter) <= 0)
                        //{
                        //    return;
                        //}

                        //m_targetFieldName = targetFieldName;


                        //// シリアライズ
                        //IXMLSerializer serializer = new XMLSerializerClass();
                        //IPropertySet propertySet = new PropertySetClass();
                        //propertySet.SetProperty("ItemLayer", selectedLayerItem.Layer);
                        //string serializeData = serializer.SaveToString(propertySet, null, null);

                        //Common.TaskInfo taskInfo = new Common.TaskInfo();
                        //taskInfo.SerializeData = serializeData;

                        //taskInfo.CallBackParam = new ParameterizedThreadStart(GetIndividualValuesCallBackResult);

                        //// スレッド開始
                        //threadTask = new Thread(new ParameterizedThreadStart(GetIndividualValuesThread));
                        //threadTask.SetApartmentState(ApartmentState.STA);
                        //threadTask.IsBackground = true;
                        //threadTask.Start(taskInfo);

                        //// プログレスバー表示
                        //frmProgress = new FormProgressManager();
                        //frmProgress.Owner = this;
                        //frmProgress.SetTitle(this);
                        //frmProgress.SetMessage(Properties.Resources.FormAttributeSearch_WhenGetIndividualValues);
                        //frmProgress.ShowDialog();

                        // リストボックスにValueを追加
                        object	objSelLayer = this.GetSelectedLayer();

                        // 個別値取得
                        SetIndividualValues(objSelLayer, targetFieldName);
                    }
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
                    this.Enabled = true;

                    if (dispMessage)
                    {
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            this,
                            Properties.Resources.FormAttributeSearch_WhenButtonGetIndividualValue);
                    }

                    // スレッドメモリ解放
                    if (threadTask != null)
                    {
                        if (threadTask.IsAlive)
                        {
                            threadTask.Abort();
                        }

                        threadTask = null;
                    }
                }
            }
        }

        /// <summary>
        /// OK Or 適用ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOKorApply_Click(object sender, EventArgs e) {
            if(comboBoxSelectionMethods.SelectedIndex >= 0) {
				bool dispMessage = false;
				if(IsValidCriteria()) {
					this.Enabled = false;

					try {
						// 選択実行
						this.CreateNewSelectionSet2(comboBoxSelectionMethods.SelectedIndex);
						if(sender.ToString().Contains("OK"))
							this.Close();
					}
					catch(COMException comex) {
						dispMessage = true;
						Common.UtilityClass.DoOnError(comex);
					}
					catch(Exception ex) {
						dispMessage = true;
						Common.UtilityClass.DoOnError(ex);
					}
					finally {
						if(dispMessage) {
							ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
								this,
								Properties.Resources.FormAttributeSearch_WhenBeginSearch);
						}

						// スレッドメモリ解放
						if(threadTask != null) {
							if(threadTask.IsAlive) {
								threadTask.Abort();
							}
							threadTask = null;
						}

						SetChangedItemFlag(false);
						this.Enabled = true;
					}
				}
				else {
					ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxWarining(
						this, Properties.Resources.FomAttributeSearch_WhenButtonApplyParamError);
				}
            }
            else {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxInfo(
                    this, Properties.Resources.FomAttributeSearch_ErrorWhenButtonApply);
            }
        }

        /// <summary>
        /// フィールド名を条件文に設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxFieldNames_DoubleClick(object sender, EventArgs e)
        {
            using (
                ESRI.ArcGIS.ADF.ComReleaser comReleaser =
                    new ESRI.ArcGIS.ADF.ComReleaser())
            {
                bool dispMessage = false;
                try
                {
                    if (listBoxFieldNames.SelectedIndex >= 0)
                    {
						bool	blnDelQ = false;
                        if(comboBoxLayerNames.SelectedItem is LayerComboItem) {
							ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
								(ESRIJapan.GISLight10.Common.LayerComboItem)comboBoxLayerNames.SelectedItem;

							// ﾌｨｰﾙﾄﾞ名を取得　※ﾃｰﾌﾞﾙ結合時は、ﾌｨｰﾙﾄﾞ区切り文字が不要 ?
							blnDelQ = ((selectedLayerItem.Layer as IDisplayTable).DisplayTable is IRelQueryTable);
						}
                        string	targetFieldName = GetTargetFieldName(blnDelQ);

                        SetCriteria(targetFieldName);
                    }
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
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            this,
                            Properties.Resources.FormAttributeSearch_WhenAddCriteria);
                    }
                }
            }
        }

        /// <summary>
        /// 条件文クリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClearCriteria_Click(object sender, EventArgs e)
        {
            richTextBoxCriteria.Text = "";
        }


        /// <summary>
        /// オペランドを条件文に設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOperation(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                string ope = sender.ToString();
                int stope = ope.IndexOf("Text:");
                if (stope > 0)
                {
                    SetCriteria(ope.Substring(stope + "Text:".Length + 1));
                }
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
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this,
                        Properties.Resources.FormAttributeSearch_WhenAddCriteria);
                }
            }
        }

        /// <summary>
        /// 個別値を条件文に設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxIndividualValues_DoubleClick(object sender, EventArgs e)
        {
            bool dispMessage = false;
            try
            {
                if (listBoxIndividualValues.SelectedIndex >= 0)
                {
                    string indVal =
                        listBoxIndividualValues.Items[
                            listBoxIndividualValues.SelectedIndex].ToString();

                    SetCriteria(indVal);
                }
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
                    ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this,
                        Properties.Resources.FormAttributeSearch_WhenAddCriteria);
                }
            }
        }

        /// <summary>
        /// 処理対象レイヤ選択時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxLayerNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool dispMessage = false;
            using(ESRI.ArcGIS.ADF.ComReleaser comReleaser = new ESRI.ArcGIS.ADF.ComReleaser()) {
                try {
                    if(comboBoxLayerNames.SelectedIndex >= 0) {
						bool	blnFBase = true;	// SQL表示設定(MDB用/その他)

                        // ﾜｰｸｽﾍﾟｰｽのﾃﾞｰﾀ型を取得
                        if(comboBoxLayerNames.SelectedItem is LayerComboItem) {
							// ﾚｲﾔｰ
							ESRIJapan.GISLight10.Common.LayerComboItem selectedLayerItem =
								(ESRIJapan.GISLight10.Common.LayerComboItem)comboBoxLayerNames.SelectedItem;

							this._strWSType = WhatTypeOfDataSource(comReleaser, selectedLayerItem.Layer);
							if(this._strWSType.Equals(DATA_SOURCE_MDB)) {
								ITable table = (selectedLayerItem.Layer as IDisplayTable).DisplayTable;
								blnFBase = table is IRelQueryTable;
							}
						}
						else {
							// ﾃｰﾌﾞﾙ
							this._strWSType = WhatTypeOfDataSource(comReleaser, this._objTargetLayer);
							blnFBase = false;
						}

                        // ﾌｨｰﾙﾄﾞ･ﾘｽﾄ
                        SetFieldNames();

                        // SQL表示
                        SetDefaultSqlString(comboBoxLayerNames.SelectedIndex);

                        // ｺﾝﾄﾛｰﾙ制御 (演算子表示)
                        if(blnFBase) {
                            // fgdb or shape:共通で可
                            this.buttonPaddingChar.Text = "_";
                            this.buttonLikeChar.Text = "%";
                        }
                        else {
                            this.buttonPaddingChar.Text = "?";
                            this.buttonLikeChar.Text = "*";
                        }
                    }

                    // アイテム変更フラグ
                    changeLayerNames = true;
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
                        ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            this,
                            Properties.Resources.FormAttributeSearch_WhenLayerSelected);
                    }
                }
            }
        }

        /// <summary>
        /// 個別値取得処理スレッドから呼び出されるコールバック
        /// </summary>
        private void GetIndividualValuesCallBackResult(object obj)
        {
            try
            {
                frmProgress.CloseForm();
                this.Refresh();

                System.Collections.ICollection values = obj as System.Collections.ICollection;

                if (values != null)
                {
                    foreach (string val in values)
                    {
                        listBoxIndividualValues.Items.Add(val);
                    }
                }

            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(GetIndividualValuesRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(GetIndividualValuesRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
        }

        /// <summary>
        /// 個別値取得処理例外の再スロー(COMException)
        /// </summary>
        /// <param name="comex">例外(COMException)</param>
        private void GetIndividualValuesRethrowCOMException(COMException comex)
        {
            ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this,
                        Properties.Resources.FormAttributeSearch_WhenButtonGetIndividualValue);
            Common.UtilityClass.DoOnError(comex);

            throw new ApplicationException();
        }

        /// <summary>
        /// 個別値取得処理例外の再スロー(Exception)
        /// </summary>
        /// <param name="ex">例外(Exception)</param>
        private void GetIndividualValuesRethrowException(Exception ex)
        {
            ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                        this,
                        Properties.Resources.FormAttributeSearch_WhenButtonGetIndividualValue);
            Common.UtilityClass.DoOnError(ex);

            throw new ApplicationException();
        }

        /// <summary>
        /// 個別値取得処理
        /// </summary>
        /// <param name="obj">個別値取得処理を行うためのパラメータ</param>
        private void GetIndividualValuesThread(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                ILayer itemLayer = propertySet.GetProperty("ItemLayer") as ILayer;

                // 個別値取得
                System.Collections.ICollection values =
                    GetIndividualValues(itemLayer as IGeoFeatureLayer, m_targetFieldName);

                // コールバック
                this.Invoke(ti.CallBackParam, values);
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(GetIndividualValuesRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(GetIndividualValuesRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
        }

        /// <summary>
        /// 選択セットの作成処理スレッドから呼び出されるコールバック
        /// </summary>
        private void SelectionSetCallBackResult(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                IFeatureSelection selFeat = propertySet.GetProperty("ResultFeatureSelection") as IFeatureSelection;

                ISelectionSet2 selectionSet = selFeat.SelectionSet as ISelectionSet2;
                IEnumIDs ids = selectionSet.IDs;

                IFeatureLayer targetFeatureLayer = this.GetSelectedLayer() as IFeatureLayer;
                IFeatureSelection targetFeatureSelection = targetFeatureLayer as IFeatureSelection;

                // 現在の選択状態をクリア
                targetFeatureSelection.Clear();

                ISelectionSet2 targetSelectionSet = targetFeatureSelection.SelectionSet as ISelectionSet2;

                if (selectionSet.Count > 0)
                {
                    int[] aryOIDs = new int[selectionSet.Count];
                    int id = ids.Next();
                    int num = 0;

                    while (id != -1)
                    {
                        aryOIDs[num] = id;
                        id = ids.Next();

                        num++;
                    }

                    // 選択状態をセット
                    targetSelectionSet.AddList(aryOIDs.Length, ref aryOIDs[0]);
                    //targetSelectionSet.Refresh();
                }

                targetFeatureSelection.SelectionChanged();

                IActiveView pActiveView = m_mapControl.ActiveView;
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);

                //SetMainMapSelectionChanged(targetFeatureSelection);

                frmProgress.CloseForm();
                this.Refresh();
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(SelectionSetRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(SelectionSetRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
        }

        /// <summary>
        /// 選択セットの作成処理例外の再スロー(COMException)
        /// </summary>
        /// <param name="comex">例外(COMException)</param>
        private void SelectionSetRethrowCOMException(COMException comex)
        {
            ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            this,
                            Properties.Resources.FormAttributeSearch_WhenBeginSearch);
            Common.UtilityClass.DoOnError(comex);

            throw new ApplicationException();
        }

        /// <summary>
        /// 選択セットの作成処理例外の再スロー(Exception)
        /// </summary>
        /// <param name="ex">例外(Exception)</param>
        private void SelectionSetRethrowException(Exception ex)
        {
            ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError(
                            this,
                            Properties.Resources.FormAttributeSearch_WhenBeginSearch);
            Common.UtilityClass.DoOnError(ex);

            throw new ApplicationException();
        }

        /// <summary>
        /// 新しい選択セットの作成処理
        /// </summary>
        /// <param name="obj">新しい選択セットの作成処理を行うためのパラメータ</param>
        private void CreateNewSelectionSetThread(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                IFeatureLayer featureLayer = propertySet.GetProperty("FeatureLayer") as IFeatureLayer;
                string criteria = propertySet.GetProperty("Criteria") as string;

                IFeatureSelection selFeat = featureLayer as IFeatureSelection;

                // 選択セット作成
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = criteria;

                selFeat.SelectFeatures(
                    queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                // 結果をシリアライズ
                propertySet.SetProperty("ResultFeatureSelection", selFeat);
                string serializeData = serializer.SaveToString(propertySet, null, null);

                ti.SerializeData = serializeData;

                // コールバック
                this.Invoke(ti.CallBackParam, ti);
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(SelectionSetRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(SelectionSetRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
        }

        /// <summary>
        /// 現在の選択セットに追加処理
        /// </summary>
        /// <param name="obj">現在の選択セットに追加処理を行うためのパラメータ</param>
        private void AddSelectionSetThread(object obj)
        {


            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                IFeatureLayer featureLayer = propertySet.GetProperty("FeatureLayer") as IFeatureLayer;
                string criteria = propertySet.GetProperty("Criteria") as string;


                IFeatureSelection selFeat = featureLayer as IFeatureSelection;

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = criteria;

                selFeat.SelectFeatures(
                    queryFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);


                // 結果をシリアライズ
                propertySet.SetProperty("ResultFeatureSelection", selFeat);
                string serializeData = serializer.SaveToString(propertySet, null, null);

                ti.SerializeData = serializeData;

                // コールバック
                this.Invoke(ti.CallBackParam, ti);
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(SelectionSetRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(SelectionSetRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }

        }

        /// <summary>
        /// 現在の選択セットから削除処理
        /// </summary>
        /// <param name="obj">現在の選択セットから削除処理を行うためのパラメータ</param>
        private void DeleteFromSelectionSetThread(object obj)
        {

            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                IFeatureLayer featureLayer = propertySet.GetProperty("FeatureLayer") as IFeatureLayer;
                string criteria = propertySet.GetProperty("Criteria") as string;

                IFeatureSelection selFeat = featureLayer as IFeatureSelection;

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = criteria;

                selFeat.SelectFeatures(
                    queryFilter, esriSelectionResultEnum.esriSelectionResultSubtract, false);


                // 結果をシリアライズ
                propertySet.SetProperty("ResultFeatureSelection", selFeat);
                string serializeData = serializer.SaveToString(propertySet, null, null);

                ti.SerializeData = serializeData;

                // コールバック
                this.Invoke(ti.CallBackParam, ti);
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(SelectionSetRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(SelectionSetRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
            
        }

        /// <summary>
        /// 現在の選択セットから絞り込み選択処理
        /// </summary>
        /// <param name="obj">現在の選択セットから絞り込み選択処理を行うためのパラメータ</param>
        private void SelectFromCurrentSelectionSetThread(object obj)
        {
            try
            {
                Common.TaskInfo ti = obj as Common.TaskInfo;

                IXMLSerializer serializer = new XMLSerializerClass();
                IPropertySet propertySet =
                    serializer.LoadFromString(ti.SerializeData, null, null) as IPropertySet;
                IFeatureLayer featureLayer = propertySet.GetProperty("FeatureLayer") as IFeatureLayer;
                string criteria = propertySet.GetProperty("Criteria") as string;

                IFeatureSelection selFeat = featureLayer as IFeatureSelection;

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = criteria;


                IObjectCopy objectCopy = new ObjectCopyClass();
                IFeatureSelection newSelFeat = (IFeatureSelection)objectCopy.Copy(selFeat);

                IObjectCopy objectCopy2 = new ObjectCopyClass();
                IFeatureSelection newSelFeat2 = (IFeatureSelection)objectCopy.Copy(selFeat);

                newSelFeat.SelectFeatures(
                    queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                ISelectionSet2 newSelectionSet = (ISelectionSet2)newSelFeat.SelectionSet;
                IEnumIDs ids = newSelectionSet.IDs;

                if (newSelectionSet.Count > 0)
                {
                    int[] aryOIDs = new int[newSelectionSet.Count];
                    int id = ids.Next();
                    int num = 0;

                    while (id != -1)
                    {
                        aryOIDs[num] = id;
                        id = ids.Next();

                        num++;
                    }

                    newSelFeat2.SelectionSet.RemoveList(aryOIDs.Length, ref aryOIDs[0]);
                }

                ISelectionSet2 newSelectionSet2 = (ISelectionSet2)newSelFeat2.SelectionSet;
                IEnumIDs ids2 = newSelectionSet2.IDs;

                if (newSelectionSet2.Count > 0)
                {
                    int[] aryOIDs2 = new int[newSelectionSet2.Count];
                    int id2 = ids2.Next();
                    int num2 = 0;

                    while (id2 != -1)
                    {
                        aryOIDs2[num2] = id2;
                        id2 = ids2.Next();

                        num2++;
                    }

                    selFeat.SelectionSet.RemoveList(aryOIDs2.Length, ref aryOIDs2[0]);
                }

                // 結果をシリアライズ
                propertySet.SetProperty("ResultFeatureSelection", selFeat);
                string serializeData = serializer.SaveToString(propertySet, null, null);

                ti.SerializeData = serializeData;

                // コールバック
                this.Invoke(ti.CallBackParam, ti);
            }
            catch (COMException comex)
            {
                RethrowCOMExceptionDelegate dlgt = new RethrowCOMExceptionDelegate(SelectionSetRethrowCOMException);
                this.BeginInvoke(dlgt, new object[] { comex });
            }
            catch (Exception ex)
            {
                RethrowExceptionDelegate dlgt = new RethrowExceptionDelegate(SelectionSetRethrowException);
                this.BeginInvoke(dlgt, new object[] { ex });
            }
        }

        private void comboBoxSelectionMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            // アイテム変更フラグ
            changeSelectionMethods = true;
        }

        private void richTextBoxCriteria_TextChanged(object sender, EventArgs e)
        {
            // アイテム変更フラグ
            changeCriteria = true;
        }

        private void SetChangedItemFlag(bool flag)
        {
            changeLayerNames = flag;
            changeSelectionMethods = flag;
            changeCriteria = flag;
        }

        private bool IsChangedItem()
        {
            if (changeLayerNames == true
                || changeSelectionMethods == true
                || changeCriteria == true)
            {
                return true;
            }

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ProgressDialog pd = new ProgressDialog();

            pd.Title = "カウントアップ";
            //プログレスバーの最小値を設定
            pd.Minimum = 0;
            //プログレスバーの最大値を設定
            pd.Maximum = 10;
            //プログレスバーの初期値を設定
            pd.Value = 0;

            pd.CancelEnable = false;

            //進行状況ダイアログを表示する
            pd.Show(this);

            //処理を開始
            for (int i = 1; i <= 10; i++)
            {
                //プログレスバーの値を変更する
                pd.Value = i;
                //メッセージを変更する
                pd.Message = i.ToString() + "番目を処理中...";

                //キャンセルされた時はループを抜ける
                if (pd.Canceled)
                    break;

                //1秒間待機する（本来なら何らかの処理を行う）
                System.Threading.Thread.Sleep(1000);
            }

            //ダイアログを閉じる
            pd.Close();


        }
    }
}
