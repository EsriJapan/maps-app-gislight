using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// スナップ設定コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    ///  2011-01-27 継承元をEJBaseCommandに変更 
    /// </history>
    public sealed class EditingSnappingCommand : Common.EjBaseCommand //BaseCommand
    {

        private IHookHelper m_hookHelper;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditingSnappingCommand()
        {

            base.captionName = "スナップ設定";

        }

        #region Overriden Class Methods

        /// <summary>
        /// クリエイト時処理
        /// </summary>
        /// <param name="hook">ツールバーコントロール</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

        }

        /// <summary>
        /// クリック時処理
        /// </summary>
        public override void OnClick()
        {
            Type commandType =
                Type.GetTypeFromProgID("esriControls.ControlsEditingSnappingCommand");

            if (commandType != null)
            {
                object cmd = Activator.CreateInstance(commandType);
                ICommand command = cmd as ICommand;
                command.OnCreate(m_hookHelper.Hook);
                command.OnClick();
            }
        }

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IEngineEditor engineEditor = new EngineEditorClass();

                if ((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)) // &&
                //(engineEditor.HasEdits() == true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    }
}
