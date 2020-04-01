using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using ESRI.ArcGIS.SystemUI;

namespace ESRIJapan.GISLight10.dispatcher
{
    /// <summary>
    /// 実行するコマンドクラスの割り振り
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    class Dispatcher
    {
        private System.Collections.Hashtable Participants = null;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Dispatcher()
        {
            Participant participant = new Participant();
            this.Participants = participant.Participants;
        }

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="sender">ToolStripDropDownIemへのポインタ</param>
        /// <param name="hook">未使用</param>
        //public void DoCommand(object sender, object hook, Ui.MainForm mainForm)
        public void DoCommand(object sender, object hook)
        {
            try
            {
                if (sender == null) return;

                System.Windows.Forms.ToolStripMenuItem menuitem =
                    sender as System.Windows.Forms.ToolStripMenuItem;

                // esriControlコマンド判定
                if (this.Participants[menuitem.Name].ToString().Contains("esriControls."))
                {
                    Type commandType =
                        Type.GetTypeFromProgID(this.Participants[menuitem.Name].ToString());

                    if (commandType != null)
                    {
                        object cmd = Activator.CreateInstance(commandType);
                        ICommand command = cmd as ICommand;
                        command.OnCreate(hook);
                        command.OnClick();
                    }
                }
                else
                {
                    Type cmdType =
                        Type.GetType(this.Participants[menuitem.Name].ToString());

                    if (cmdType == null) return;

                    object cmd = null;

                    //if (this.Participants[menuitem.Name].ToString().Contains(
                    //    "ESRIJapan.GISLight10.EngineCommand.Open"))
                    //{
                    //    // MainFormへの参照指定の場合:フォーム開くコマンド
                    //    cmd = cmdType.InvokeMember(null,
                    //        System.Reflection.BindingFlags.CreateInstance,
                    //        null, null,
                    //        new object[] { mainForm });
                    //}
                    //else
                    //{
                        cmd = cmdType.InvokeMember(null,
                            System.Reflection.BindingFlags.CreateInstance,
                            null, null,
                            new object[] { });
                    //}

                    object result1 = cmdType.InvokeMember("OnCreate",
                        System.Reflection.BindingFlags.InvokeMethod,
                        null, cmd,
                        new object[] { hook });

                    object result2 = cmdType.InvokeMember("OnClick",
                        System.Reflection.BindingFlags.InvokeMethod,
                        null, cmd,
                        new object[] { });
                }
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex.Message);
                Common.Logger.Error(ex.StackTrace);
                return;
            }
        }
    }
}
