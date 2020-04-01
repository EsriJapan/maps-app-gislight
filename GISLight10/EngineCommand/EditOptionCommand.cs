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
    /// 編集オプションのコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class EditOptionCommand : Common.EjBaseCommand
    {
        private IHookHelper m_HookHelper;
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditOptionCommand()
        {
            base.captionName = "オプション";
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
        /// オプション設定フォームを表示
        /// </summary>
        public override void OnClick()
        {
            
            Ui.FormEditOption frm = new Ui.FormEditOption(m_mapControl);
            frm.ShowDialog(mainForm);
        }
        #endregion

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                //エディタ ツールバーを取得
                ESRI.ArcGIS.Controls.IToolbarItem titm = mainForm.m_ToolbarControl2.GetItem(0);
                //編集開始コマンドを取得
                ESRI.ArcGIS.Controls.IToolbarItem titmMem = titm.Menu.GetItem(0);

                IEngineEditor engineEditor = new EngineEditorClass();
                
                //編集開始コマンドがアクティブな場合
                if (titmMem.Command.Enabled ||
                    engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)                
                {                    
                    return true;                    
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
