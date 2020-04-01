using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// WindowHelperクラス
    /// </summary>
    static class WindowHelper
    {
        /// <summary>
        /// 未使用
        /// </summary>
        /// <param name="hwnd">ウィンドウハンドル</param>
        /// <returns>ウィンドウ クラス</returns>
        public static NativeWindow GetWindowFromHost(int hwnd)
        {
            IntPtr handle = new IntPtr(hwnd);
            NativeWindow nativeWindow = new NativeWindow();
            nativeWindow.AssignHandle(handle);
            return nativeWindow;
        }
    }
}
