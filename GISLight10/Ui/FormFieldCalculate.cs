using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

using ESRIJapan.GISLight10.Common;

namespace ESRIJapan.GISLight10.Ui
{
    public partial class FormFieldCalculate : Form
    {
        OpenFileDialog m_openFileDialog1;   //条件式ファイル読み込み用ダイアログ
        bool m_isExpressionActive;            //テキストボックスの条件式がアクティブか（falseならコードブロックがアクティブ）
        private Ui.MainForm mainFrm;
        private IMapControl3 m_pMapControl;
        // 2012/08/17 ADD 
        private Common.OptionSettings settingFile = null;
        private bool m_execBackground = false;
        private const string FIELD_NAME_BACKGROUND = "バックグラウンド実行フラグ";
        // 2012/08/17 ADD 

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="mainFrm"></param>
        public FormFieldCalculate(ESRI.ArcGIS.Controls.IMapControl3 mapControl, Ui.MainForm mainFrm)
        {
            initForm(mapControl,mainFrm);

            LayerManager layManager;
            List<IFeatureLayer> layers = null;
            LayerComboItem item;
            layManager = new LayerManager();
            layers = layManager.GetFeatureLayers(mapControl.Map);

            try
            {
                settingFile = new Common.OptionSettings();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }

            this.comboBoxParameter1.Items.Clear();

            foreach (IFeatureLayer pLayer in layers)
            {
                item = new LayerComboItem(pLayer);
                this.comboBoxParameter1.Items.Add(item);
            }

            //リストの選択
            if (this.comboBoxParameter1.Items.Count > 0)
            {
                this.comboBoxParameter1.SelectedIndex = 0;
            }

            if (this.comboBoxParameter2.Items.Count > 0)
            {
                this.comboBoxParameter2.SelectedIndex = 0;
            }

            //フォーカスの変更
            comboBoxParameter1.Focus();
        }

        /// <summary>
        /// コンストラクタ（処理対象レイヤが決まっている場合）
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="mainFrm"></param>
        /// <param name="TargetLayer">処理対象レイヤ</param>
        public FormFieldCalculate(ESRI.ArcGIS.Controls.IMapControl3 mapControl, Ui.MainForm mainFrm, object TargetLayer)
        {
            initForm(mapControl, mainFrm);

            try
            {
                settingFile = new Common.OptionSettings();
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
            }

            //コンボボックスに該当レイヤのみを再設定
            this.comboBoxParameter1.Items.Clear();
            if(TargetLayer is ILayer) {
				LayerComboItem item = new LayerComboItem((ILayer)TargetLayer);
				this.comboBoxParameter1.Items.Add(item);
			}
			else if(TargetLayer is IStandaloneTable) {
				this.comboBoxParameter1.Items.Add((TargetLayer as IStandaloneTable).Name);
			}
			if(this.comboBoxParameter1.Items.Count > 0) {
				this.comboBoxParameter1.SelectedIndex = 0;
            }

            //非アクティブ化
            comboBoxParameter1.Enabled = false;

            //フォーカスの変更
            comboBoxParameter2.Focus();
        }

        /// <summary>
        /// フォームの各種コンポーネントを設定する
        /// </summary>
        protected void initForm(ESRI.ArcGIS.Controls.IMapControl3 mapControl, Ui.MainForm mainFrm)
        {
            InitializeComponent();

            this.mainFrm = mainFrm;
            this.m_pMapControl = mapControl;

            //条件式ファイル読み込みダイアログ設定
            m_openFileDialog1 = new OpenFileDialog();
            m_openFileDialog1.Filter = "条件式ファイル (*.cal)|*.cal";
            m_openFileDialog1.FileName = "";
            m_openFileDialog1.FileOk += new CancelEventHandler(openFileDialog1_FileOk);

            // scriptフォルダは実行ファイルしたのサブディレクトリ
            string apppath = Application.StartupPath;
            string scriptpath = apppath + "\\script"; //System.IO.Path.GetDirectoryName(apppath) + "\\script";
            if (System.IO.Directory.Exists(scriptpath))
            {
                m_openFileDialog1.InitialDirectory = scriptpath;
            }


            //テキストボックスのフォーカス イベント ハンドラ
            textBoxParameter3.GotFocus += new EventHandler(textBoxParameter3_GotFocus);
            textBoxParameter5.GotFocus += new EventHandler(textBoxParameter5_GotFocus);

            m_isExpressionActive = true;

            //リストの初期化
            this.comboBoxParameter1.Items.Clear();
            this.textBoxParameter3.Clear();
            this.textBoxParameter5.Clear();

			// 選択ﾌｨｰﾁｬｰ処理の明示
			this.checkBox_SelFeats.Enabled = false;
        }


        /// <summary>
        /// フィールド一覧からダブルクリックしてテキストボックス内にフィールド名を追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fieldList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string prefix;
            string suffix;

            prefix = "!";
            suffix = "! ";

			// 選択ﾌｨｰﾙﾄﾞのﾌｨｰﾙﾄﾞ名を取得
			FieldComboItem	cbiFld = fieldList.SelectedItem as FieldComboItem;
            string fieldName = prefix + cbiFld.FieldName + suffix;


            //アクティブだったテキストボックスを判定
            if (m_isExpressionActive == true)
            {
                //条件式
                textBoxParameter3.Focus();
                int caret = textBoxParameter3.SelectionStart;                               //キャレット位置取得
                int selLength = textBoxParameter3.SelectionLength;                          //選択テキスト文字数取得
                textBoxParameter3.Text = textBoxParameter3.Text.Remove(caret, selLength);   //選択部分の削除
                textBoxParameter3.Text = textBoxParameter3.Text.Insert(caret, fieldName);   //挿入
                textBoxParameter3.SelectionStart = caret + fieldName.Length;                //カーソルを戻す
            }
            else
            {
                //コード ブロック
                textBoxParameter5.Focus();
                int caret = textBoxParameter5.SelectionStart;                               //キャレット位置取得
                int selLength = textBoxParameter5.SelectionLength;                          //選択テキスト文字数取得
                textBoxParameter5.Text = textBoxParameter5.Text.Remove(caret, selLength);   //選択部分の削除
                textBoxParameter5.Text = textBoxParameter5.Text.Insert(caret, fieldName);   //挿入
                textBoxParameter5.SelectionStart = caret + fieldName.Length;                //カーソルを戻す
            }
        }


        /// <summary>
        /// フィーチャ レイヤ選択時の制御
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxParameter1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //リストの初期化
            comboBoxParameter2.Items.Clear();
            fieldList.Items.Clear();

            //フィールド一覧を表示 (結合ﾌｨｰﾙﾄﾞを含む)
            if(comboBoxParameter1.SelectedItem is LayerComboItem) {
				IGeoFeatureLayer	agGeoFL = (IGeoFeatureLayer)((LayerComboItem)comboBoxParameter1.SelectedItem).Layer;

				// ﾌｨｰﾙﾄﾞ･ﾘｽﾄを作成
				FieldComboItem[]	flds = this.mainFrm.GetFieldItems(agGeoFL, true, false, false, true, null);
				this.fieldList.Items.AddRange(flds);
				
				flds = this.mainFrm.GetFieldItems(agGeoFL, true, true, true, true, null);
				this.comboBoxParameter2.Items.AddRange(flds);

				// 選択状況を取得
				this.checkBox_SelFeats.Checked = ((agGeoFL as IFeatureSelection).SelectionSet.Count > 0);
			}
			else {
				// ﾌｨｰﾙﾄﾞ･ﾘｽﾄを作成
				FieldComboItem[]	flds = this.mainFrm.GetFieldItems(mainFrm.SelectedTable, true, false, false, true, null);
				this.fieldList.Items.AddRange(flds);
				
				flds = this.mainFrm.GetFieldItems(mainFrm.SelectedTable, true, true, true, true, null);
				this.comboBoxParameter2.Items.AddRange(flds);

				// 選択状況を取得
				this.checkBox_SelFeats.Checked = (mainFrm.SelectedTable as ITableSelection).SelectionSet.Count > 0;
			}
            
            //リストの選択
            if (this.comboBoxParameter2.Items.Count > 0)
            {
                this.comboBoxParameter2.SelectedIndex = 0;
                if(!this.buttonParameter3.Enabled) {
					this.buttonParameter3.Enabled = true;
                }
            }
            else {
				this.fieldList.Items.Clear();
				this.buttonParameter3.Enabled = false;
				MessageBoxManager.ShowMessageBoxWarining("演算可能なフィールドがありません。");
            }
        }

        //条件式ファイルの読み込み
        private void buttonParameter3_Click(object sender, EventArgs e)
        {
            m_openFileDialog1.ShowDialog();
        }


        //条件式ファイル選択後の操作
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            System.IO.StreamReader cReader = new System.IO.StreamReader(m_openFileDialog1.FileName, Encoding.Unicode);

            // 読み込んだ結果をすべて格納するための変数を宣言する
            string stResult = string.Empty;

            // 読み込みできる文字がなくなるまで繰り返す
            while (cReader.Peek() >= 0)
            {
                // ファイルを 1 行ずつ読み込む
                string stBuffer = cReader.ReadLine();
                // 読み込んだものを追加で格納する
                stResult += stBuffer + System.Environment.NewLine;
            }
            // cReader を閉じる (正しくは オブジェクトの破棄を保証する を参照)
            cReader.Close();

            string separator = "__esri_field_calculator_splitter__";
            string[] lists;

            lists = Microsoft.VisualBasic.Strings.Split(stResult, separator, -1, Microsoft.VisualBasic.CompareMethod.Text);

            if (lists.Length == 2)
            {
                textBoxParameter5.Text = lists[0].Trim() + System.Environment.NewLine;
                textBoxParameter3.Text = lists[1].Trim();
            }
            else
            {
                textBoxParameter3.Text = lists[0].Trim();
            }
        }


        private void okButton_Click(object sender, EventArgs e)
        {
            //パラメータ チェック
            if (!checkParameter())
            {
                return;
            }

            // オプション設定よりバックグラウンド実行設定を取得
            try
            {
                m_execBackground = (settingFile.GeoprocessingBackground == "1");
            }
            catch (Exception ex)
            {
                MessageBoxManager.ShowMessageBoxError(this,
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_BACKGROUND + " ]"

                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);
                Common.Logger.Error(
                    Properties.Resources.CommonMessage_OptionSetting_ERROR_ValueCantRead
                    + "[ " + FIELD_NAME_BACKGROUND + " ]"
                    + Properties.Resources.CommonMessage_OptionSetting_ERROR_ResetMessage);

                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                // 読み込めない場合でも、処理の継続をする（フォアグラウンド実行）
            }

            //テーブル名取得
            object	objPrm1 = null;
            if(comboBoxParameter1.SelectedItem is LayerComboItem) {
				LayerComboItem	tableItem = (LayerComboItem)comboBoxParameter1.SelectedItem;
				objPrm1 = tableItem.Layer as IFeatureLayer;
            }
            else {
				objPrm1 = this.mainFrm.SelectedTable;
            }

            //フィールド名取得
            FieldComboItem fieldItem = (FieldComboItem)comboBoxParameter2.SelectedItem;

            //パラメータ
            //string in_table = ComboBoxFeatureLayer.FileNameFullPath(tableItem.Layer);
            string field = fieldItem.FieldName;
            string expression = textBoxParameter3.Text;
            string expression_type = "PYTHON_9.3";
            string code_block = textBoxParameter5.Text; //'"' + textBoxParameter5.Text + '"'; // ''で括らなくても動作する
                        

            //パラメータ セット
            IVariantArray pVariantArray = new VarArrayClass();
			pVariantArray.Add(objPrm1);
			//pVariantArray.Add(in_table);
            pVariantArray.Add(field);
            pVariantArray.Add(expression);
            pVariantArray.Add(expression_type);
            pVariantArray.Add(code_block);
            

            //ジオプロセシング処理
            this.Visible = false;
            FormExecuteGP frm = new FormExecuteGP(this.Owner, m_pMapControl.Map);
            frm.Execute("CalculateField_management", pVariantArray, false, m_execBackground, false,"");
        }

        /// <summary>
        /// GotFocusイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxParameter3_GotFocus(object sender, EventArgs e)
        {
            m_isExpressionActive = true;
        }

        /// <summary>
        /// GotFocusイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxParameter5_GotFocus(object sender, EventArgs e)
        {
            m_isExpressionActive = false;
        }


        /// <summary>
        /// ジオプロセシング ツール実行前のパラメータ チェック
        /// </summary>
        /// <returns></returns>
        private bool checkParameter()
        {
            string errorStrings = "";

            if (this.comboBoxParameter1.SelectedIndex == -1)
            {
                errorStrings += "・入力フィーチャ レイヤが選択されていません。" + Environment.NewLine;
            }

            if (this.comboBoxParameter2.SelectedIndex == -1)
            {
                errorStrings += "・フィールド名が選択されていません。" + Environment.NewLine;
            }

            if (this.textBoxParameter3.Text == "")
            {
                errorStrings += "・条件式が未入力です。" + Environment.NewLine;
            }

            if (errorStrings != "")
            {
                MessageBox.Show(errorStrings, "パラメータ チェック エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                return true;
            }

        }

        private void FormFieldCalculate_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

		private void comboBoxParameter2_SelectedIndexChanged(object sender, EventArgs e) {
			ComboBox	ctlCB = sender as ComboBox;
			
			// ﾕｰｻﾞｰ補助
			string		strText = "条件式";
			if(ctlCB.SelectedIndex >= 0) {
				strText += " : " + ctlCB.Text + " = ";
			}
			
			this.label3.Text = strText;
		}

    }
}
