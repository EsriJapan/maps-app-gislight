using System.Windows.Forms;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 数値以外のペーストを無効にしたテキストボックス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    class NumbersAndPointTextBox : TextBox
    {
        const int WM_PASTE = 0x302;

        /// <summary>
        /// Windowsメッセージを処理
        /// </summary>
        /// <param name="m">処理対象のWindows Message</param>
        [System.Security.Permissions.SecurityPermission(
            System.Security.Permissions.SecurityAction.LinkDemand,
            Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE)
            {
                IDataObject iData = Clipboard.GetDataObject();
                //文字列がクリップボードにあるか
                if (iData != null && iData.GetDataPresent(DataFormats.Text))
                {
                    string clipStr = (string)iData.GetData(DataFormats.Text);
                    //クリップボードの文字列が数字か調べる
                    if (!System.Text.RegularExpressions.Regex.IsMatch(
                        clipStr,
                        @"^[0-9|.]+$"))
                    {
                        return;
                    }
                    //else if()
                    //{
                    //    return;
                    //}
                        
                }
            }

            base.WndProc(ref m);
        }
    }
}