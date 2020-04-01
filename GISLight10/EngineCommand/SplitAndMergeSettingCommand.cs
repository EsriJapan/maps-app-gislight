using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// スプリット・マージ設定のコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
	///  2012-07-26 起動条件変更(編集前後で起動OK → 編集前のみOK)
    /// </history>
    public sealed class SplitAndMergeSettingCommand : Common.EjBaseCommand
    {
        private IHookHelper m_HookHelper;
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SplitAndMergeSettingCommand()
        {
            base.captionName = "スプリット・マージ設定";
        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">ツールバーコントロール</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_HookHelper = new HookHelperClass();
                m_HookHelper.Hook = hook;

                IToolbarControl2 tlb2 = (IToolbarControl2)m_HookHelper.Hook;
                m_mapControl = (IMapControl3)tlb2.Buddy;

                IntPtr ptr = (System.IntPtr)m_mapControl.hWnd;

                System.Windows.Forms.Control cntrl =
                    System.Windows.Forms.Control.FromHandle(ptr);
                mainForm = (Ui.MainForm)cntrl.FindForm();
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// クリック時処理
        /// スプリット・マージ設定フォームの表示
        /// </summary>
        public override void OnClick()
        {
			// 設定画面を表示
            Ui.FormSplitAndMergeSetting frm = new Ui.FormSplitAndMergeSetting(m_mapControl);
            frm.Text = base.captionName;

            // 設定ﾌｫｰﾑ画面を表示
            frm.ShowDialog(mainForm);
        }

        /// <summary>
        /// 編集ターゲットレイヤを取得
        /// </summary>
        /// <returns>編集ターゲットレイヤ</returns>
        private IFeatureLayer GetEditTargetLayer() {
            // ｴﾃﾞｨﾀを取得
            IEngineEditor engineEditor = new EngineEditorClass();

            // 編集対象ﾚｲﾔｰを取得
            IEngineEditLayers editLayer = (IEngineEditLayers)engineEditor;
            IFeatureLayer targetFcLayer = editLayer.TargetLayer;

            return targetFcLayer;
        }

        #endregion

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
                //エディタ ツールバーを取得
                ESRI.ArcGIS.Controls.IToolbarItem titm = mainForm.m_ToolbarControl2.GetItem(0);
                //編集開始コマンドを取得
                ESRI.ArcGIS.Controls.IToolbarItem titmMem = titm.Menu.GetItem(0);

                IEngineEditor engineEditor = new EngineEditorClass();

                // 編集ﾓｰﾄﾞでないこと
                if(titmMem.Command.Enabled && engineEditor.EditState == esriEngineEditState.esriEngineStateNotEditing) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
    }
}
