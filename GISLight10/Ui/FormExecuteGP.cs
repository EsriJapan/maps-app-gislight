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
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;


namespace ESRIJapan.GISLight10.Ui
{
    /// <summary>
    /// ジオプロセシング実行用
    /// </summary>
    public partial class FormExecuteGP : Form
    {
        ESRIJapan.GISLight10.Common.GPMessageEventHandler gpEventHandler;

        Geoprocessor m_Geoprocessor;
        IGPUtilities3 m_pGPUtilities;
        IVariantArray m_pVariantArray;
        string m_tool;
        bool m_isASync;
        bool m_isAddMap;
        IMap m_pMap;
        string m_CustomToolbox;
        bool m_Succeeded;   //ジオプロセシング処理終了フラグ

        //private bool blAddEvent = false;
        //private bool isReady = false;

        //public bool IsReady
        //{
        //    get { return isReady; }
        //}

        public FormExecuteGP(Form myOwner,IMap Map)
        {
            InitializeComponent();

            //初期化
            this.Owner = myOwner;
            //this.Height = 80;
            m_pMap = Map;

            //フォームの表示位置をオーナーの中央へ
            if (this.Owner != null)
            { 
                this.StartPosition = FormStartPosition.Manual;
                this.Left = this.Owner.Left + (this.Owner.Width - this.Width)/2;
                this.Top = this.Owner.Top + (this.Owner.Height - this.Height)/2;
            }

            // ジオプロ初期化
            InitializeGeopro();
        }

        ~FormExecuteGP()
        {

            if (m_pVariantArray != null)
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(m_pVariantArray);
            if (m_Geoprocessor != null)
                m_Geoprocessor = null;

        }

        /// <summary>
        /// ジオプロ関連の初期化
        /// </summary>
        private void InitializeGeopro() 
        {

            m_Geoprocessor = new Geoprocessor();
            gpEventHandler = new ESRIJapan.GISLight10.Common.GPMessageEventHandler();

            //m_pGPUtilities = new GPUtilitiesClass();
            m_pGPUtilities = ESRIJapan.GISLight10.Common.SingletonUtility.NewGPUtilities();
            m_Geoprocessor.AddOutputsToMap = false;
            m_pGPUtilities.SetInternalMap(m_pMap);

        }

        /// <summary>
        /// ジオプロイベントの登録
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="gpEventHandler"></param>
        private void RegisterGeoproEvent(Geoprocessor gp, ESRIJapan.GISLight10.Common.GPMessageEventHandler gpEventHandler)
        {
            gp.RegisterGeoProcessorEvents(gpEventHandler);
            gpEventHandler.GPPreToolExecute += new ESRIJapan.GISLight10.Common.PreToolExecuteEventHandler(gpPreToolExecute);
            gpEventHandler.GPPostToolExecute += new ESRIJapan.GISLight10.Common.PostToolExecuteEventHandler(gpPostToolExecute);

            gp.ToolExecuting += new EventHandler<ESRI.ArcGIS.Geoprocessor.ToolExecutingEventArgs>(gpToolExecuting);
            gp.ToolExecuted += new EventHandler<ESRI.ArcGIS.Geoprocessor.ToolExecutedEventArgs>(gpToolExecuted);       
        }

        /// <summary>
        /// ジオプロイベントの登録解除
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="gpEventHandler"></param>
        private void UnregisterGeoproEvent(Geoprocessor gp, ESRIJapan.GISLight10.Common.GPMessageEventHandler gpEventHandler)
        {
            gp.ToolExecuting -= new EventHandler<ESRI.ArcGIS.Geoprocessor.ToolExecutingEventArgs>(gpToolExecuting);
            gp.ToolExecuted -= new EventHandler<ESRI.ArcGIS.Geoprocessor.ToolExecutedEventArgs>(gpToolExecuted);

            gpEventHandler.GPPreToolExecute -= new ESRIJapan.GISLight10.Common.PreToolExecuteEventHandler(gpPreToolExecute);
            gpEventHandler.GPPostToolExecute -= new ESRIJapan.GISLight10.Common.PostToolExecuteEventHandler(gpPostToolExecute);

            gp.UnRegisterGeoProcessorEvents(gpEventHandler);

        }
        
        /// <summary>
        /// ジオプロセシングの実行
        /// </summary>
        /// <param name="tool">ツール名</param>
        /// <param name="pVariantArray">パラメータ</param>
        /// <param name="isAddMap">処理後のデータをマップに追加</param>
        /// <param name="isASync">非同期実行</param>
        /// <param name="isOverwrite">上書きを許可</param>
        /// <param name="CustomToolbox">カスタム ツー<ル/param>
        public void Execute(string tool, IVariantArray pVariantArray, bool isAddMap, bool isASync, bool isOverwrite, string CustomToolbox)
        {

            // ジオプロイベントの登録
            RegisterGeoproEvent(m_Geoprocessor, gpEventHandler);

            m_tool = tool;
            m_pVariantArray = pVariantArray;
            m_Geoprocessor.OverwriteOutput = isOverwrite;
            //m_Geoprocessor.AddOutputsToMap = isAddMap;        //バックグラウンド実行時に効かないため無効化
            m_isAddMap = isAddMap;
            m_isASync = isASync;
            m_CustomToolbox = CustomToolbox;

            //カスタム ツールボックスの追加
            if (m_CustomToolbox != "")
            {
                m_Geoprocessor.AddToolbox(m_CustomToolbox);
            }

            if (m_isASync == true)
            {
                this.Show();
            }
            else
            {
                this.ShowDialog();
            }

        }

        /// <summary>
        /// PostToolExecuteイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gpPostToolExecute(object sender, ESRIJapan.GISLight10.Common.GPPostToolExecuteEventArgs e)
        {
            this.progressBarStatus.Value = 75;
            //System.Threading.Thread.Sleep(100);
            labelStatus.Text = "実行後・・・";
        }

        /// <summary>
        /// PreToolExecuteイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gpPreToolExecute(object sender, ESRIJapan.GISLight10.Common.GPPreToolExecuteEventArgs e)
        {
            this.progressBarStatus.Value = 25;
            //System.Threading.Thread.Sleep(100);
            labelStatus.Text = "実行前・・・";
        }

        /// <summary>
        /// ToolExecutingイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gpToolExecuting(object sender, ESRI.ArcGIS.Geoprocessor.ToolExecutingEventArgs e)
        {
            //IGeoProcessorResult2 gpResult = (IGeoProcessorResult2)e.GPResult;

            this.progressBarStatus.Value = 50;
            labelStatus.Text = "実行中・・・";
        }

        /// <summary>
        /// ツール実行完了時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gpToolExecuted(object sender, ESRI.ArcGIS.Geoprocessor.ToolExecutedEventArgs e)
        {
            this.progressBarStatus.Value = 100;
            labelStatus.Text = "実行終了";
            m_Succeeded = false;

            IFeatureClass pFeatureClass = null;
            IGeoProcessorResult2 result = e.GPResult as IGeoProcessorResult2;

            try
            {
                //カーソルを戻す
                //this.Cursor = Cursors.Default;

                //処理結果のデータをマップに追加
                if (result.Status == esriJobStatus.esriJobSucceeded & m_isAddMap == true & (string)result.ReturnValue != "")
                {
                    pFeatureClass = m_pGPUtilities.OpenFeatureClassFromString((string)result.ReturnValue);

                    IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.Name = pFeatureClass.AliasName;
                    pFeatureLayer.FeatureClass = pFeatureClass;

                    m_pMap.AddLayer((ILayer)pFeatureLayer);
                }

                //メッセージの表示
                IGPMessages msgs = result.GetResultMessages();
                for (int i = 0; i < result.MessageCount; i++)
                {
                    IGPMessage msg = msgs.GetMessage(i) as IGPMessage;

                    textBoxMessages.Text += msg.Description + Environment.NewLine;
                    textBoxMessages.SelectionStart = textBoxMessages.Text.Length;
                    textBoxMessages.ScrollToCaret();
                }

                //ダイアログ表示変更
                //progressBarStatus.Value = 100;
                //progressBarStatus.Style = ProgressBarStyle.Continuous;
                buttonClose.Enabled = true;
                //this.Height = 190;

                if (result.Status.Equals(esriJobStatus.esriJobSucceeded))
                {
                    //処理成功
                    labelStatus.ForeColor = Color.Green;
                    labelStatus.Text = "成功";
                    m_Succeeded = true;
                }
                else
                {
                    labelStatus.ForeColor = Color.Red;
                    labelStatus.Text = "失敗";
                    m_Succeeded = false;
                }

                //カスタム ツールボックスの解除
                //if (m_CustomToolbox != "")
                //{
                //    m_Geoprocessor.RemoveToolbox(m_CustomToolbox);
                //}

                ////イベント ハンドラの解除
                //m_Geoprocessor.ToolExecuted -= new EventHandler<ToolExecutedEventArgs>(gpToolExecuted);

                this.Activate();

            }
            catch (Exception ex)
            {

                textBoxMessages.Text += Properties.Resources.Geoprocessing_Error +
                                        "[ ジオプロセシング異常終了 ]" +
                                        Properties.Resources.Geoprocessing_Error +
                                        Environment.NewLine;

                Common.Logger.Error
                    (Properties.Resources.Geoprocessing_Error +
                    "[ ジオプロセシング異常終了 ]" +
                     Properties.Resources.Geoprocessing_Error);
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);

                this.Dispose();
            } 
            finally
            {
                ComReleaser.ReleaseCOMObject(pFeatureClass);
                // ジオプロイベントの登録解除
                UnregisterGeoproEvent(m_Geoprocessor, gpEventHandler);
            }
        }

        /// <summary>
        /// フォームを閉じる際の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormExecuteGP_FormClosing(object sender, FormClosingEventArgs e)
        {
            //処理実行中はフォームを閉じないよう制御
            if (e.CloseReason == CloseReason.UserClosing)
                if (this.buttonClose.Enabled == false)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
        }

        /// <summary>
        /// フォーム表示時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormExecuteGP_Shown(object sender, EventArgs e)
        {

            //前処理
            //this.Height = 90;
            progressBarStatus.Value = 10;
            //progressBarStatus.Style = ProgressBarStyle.Marquee;
            //progressBarStatus.MarqueeAnimationSpeed = 50;
            buttonClose.Enabled = false;
            labelStatus.Text = "初期化中・・・";//"実行中";
            labelStatus.ForeColor = Color.Black;
            //this.Cursor = Cursors.WaitCursor;
            //this.Text = this.Text + " : " + m_tool;

            if (m_isASync == true)
            {
                this.Text = "ジオプロセシング[バックグラウンド]： " + m_tool;
            }
            else
            {
                this.Text = "ジオプロセシング[フォアグラウンド]： "+ m_tool;
            }

            textBoxMessages.Clear();


            IGeoProcessorResult pGeoProcessorResult = null;

            try
            {
                Application.DoEvents();

                if (m_isASync == true)
                {
                    //バックグラウンド実行
                    pGeoProcessorResult = (IGeoProcessorResult)m_Geoprocessor.ExecuteAsync(m_tool, m_pVariantArray);
                    this.Activate();
                }
                else
                {
                    //フォアグラウンド実行
                    pGeoProcessorResult = (IGeoProcessorResult)m_Geoprocessor.Execute(m_tool, m_pVariantArray, null);
                    //Application.DoEvents();
                    this.Activate();
                }
            }
            catch (System.Runtime.InteropServices.COMException comex)
            {

                textBoxMessages.Text += Properties.Resources.Geoprocessing_Error +
                                        "[ ジオプロセシング異常終了 ]" +
                                        Properties.Resources.Geoprocessing_Error +
                                        Environment.NewLine;

                    Common.Logger.Error
                        (Properties.Resources.Geoprocessing_Error +
                        "[ ジオプロセシング異常終了 ]" +
                         Properties.Resources.Geoprocessing_Error);
                    Common.Logger.Error(comex.Message);
                    Common.Logger.Error(comex.StackTrace);

                //this.Dispose();
            }
            catch (Exception ex)
            {
                if (m_Succeeded == true)
                {

                    textBoxMessages.Text += Properties.Resources.Geoprocessing_Error +
                                            "[ ジオプロセシング異常終了 ]" +
                                            Properties.Resources.Geoprocessing_Error +
                                            Environment.NewLine;

                    Common.Logger.Error
                        (Properties.Resources.Geoprocessing_Error +
                        "[ ジオプロセシング異常終了 ]" +
                         Properties.Resources.Geoprocessing_Error);
                    Common.Logger.Error(ex.Message);
                    Common.Logger.Error(ex.StackTrace);

                    //this.Dispose();
                }
            }

        }

        /// <summary>
        /// [閉じる] ボタンの操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormExecuteGP_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

    }


}
