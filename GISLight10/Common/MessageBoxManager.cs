using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESRIJapan.GISLight10.Common
{
	/// <summary>
    /// メッセージボックスを表示するクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    ///  2011-01-26 xmlコメント、publicメソッドの引数、返却値の記述修正と追加
    /// </history>
    public class MessageBoxManager
	{

		/// <summary>
		/// 警告メッセージボックスを表示(OKボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="messageStr">表示メッセージ</param>
		public static void ShowMessageBoxError(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try 
			{
				Form parentForm = (Form)parent;

                messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch 
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			ShowMessageBox(
				parent, messageStr, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// 警告メッセージボックスを表示(OK,Cancelボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
		/// <param name="messageStr">表示メッセージ</param>
        /// <returns>ダイアログボックスの戻り値を示す識別子</returns>
		public static DialogResult ShowMessageBoxError2(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try
			{
				Form parentForm = (Form)parent;

				messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			return ShowMessageBox2(
				parent, messageStr, messageTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
		}

		/// <summary>
		/// 問い合わせメッセージボックスを表示(OKボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="messageStr">表示メッセージ</param>
		public static void ShowMessageBoxQuestion(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try
			{
				Form parentForm = (Form)parent;

				messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			ShowMessageBox(
				parent, messageStr, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
		}

		/// <summary>
		/// 問い合わせメッセージボックスを表示(OK,Cancelボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="messageStr">表示メッセージ</param>
        /// <returns>ダイアログボックスの戻り値を示す識別子</returns>
        public static DialogResult ShowMessageBoxQuestion2(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try
			{
				Form parentForm = (Form)parent;

				messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			return ShowMessageBox2(
				parent, messageStr, messageTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
		}

		/// <summary>
		/// 注意メッセージボックスを表示(OKボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="messageStr">表示メッセージ</param>
		public static void ShowMessageBoxWarining(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try
			{
				Form parentForm = (Form)parent;

				messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			ShowMessageBox(
				parent, messageStr, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// 注意メッセージボックスを表示(OK,Cancelボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="messageStr">表示メッセージ</param>
        /// <returns>ダイアログボックスの戻り値を示す識別子</returns>
        public static DialogResult ShowMessageBoxWarining2(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try
			{
				Form parentForm = (Form)parent;

				messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			return ShowMessageBox2(
				parent, messageStr, messageTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// 情報メッセージボックスを表示(OKボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="messageStr">表示メッセージ</param>
		public static void ShowMessageBoxInfo(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try
			{
				Form parentForm = (Form)parent;

				messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			ShowMessageBox(
				parent, messageStr, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		/// 情報メッセージボックスを表示(OK,Cancelボタン)
		/// </summary>
        /// <param name="parent">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="messageStr">表示メッセージ</param>
        /// <returns>ダイアログボックスの戻り値を示す識別子</returns>
        public static DialogResult ShowMessageBoxInfo2(IWin32Window parent, string messageStr)
		{
			string messageTitle = null;

			try
			{
				Form parentForm = (Form)parent;

				messageTitle = GetMessageBoxTitle(parentForm);
			}
			catch
			{
				messageTitle = Properties.Resources.CommonMessage_ApplicationName;
			}

			return ShowMessageBox2(
				parent, messageStr, messageTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
		}

		/// <summary>
		/// メッセージボックスを表示
		/// </summary>
        /// <param name="owner">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="message">表示メッセージ</param>
		/// <param name="title">表示タイトル</param>
		/// <param name="buttons">表示ボタン</param>
		/// <param name="icon">表示アイコン</param>
		public static void ShowMessageBox(
			IWin32Window owner, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			MessageBox.Show(owner, message, title, buttons, icon);
		}

		/// <summary>
		/// メッセージボックスを表示(ボタンの選択状態を返す。)
		/// </summary>
        /// <param name="owner">呼び出し元フォームのWin32 HWNDハンドル</param>
        /// <param name="message">表示メッセージ</param>
		/// <param name="title">表示タイトル</param>
		/// <param name="buttons">表示ボタン</param>
		/// <param name="icon">表示アイコン</param>
        /// <returns>ダイアログボックスの戻り値を示す識別子</returns>
        public static DialogResult ShowMessageBox2(
			IWin32Window owner, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return MessageBox.Show(owner, message, title, buttons, icon);
		}

		/// <summary>
		/// メッセージボックスのタイトルを取得
		/// </summary>
        /// <param name="parentForm">呼び出し元フォーム</param>
		/// <returns>
        /// <br>呼び出し元フォームのタイトルがCommonMessage_ApplicationNameと同じ場合は、
        /// CommonMessage_ApplicationNameの内容</br>
        /// <br>呼び出し元フォームのタイトルがCommonMessage_ApplicationNameと違う場合は、
        /// CommonMessage_ApplicationNameの内容 + " - " + 呼び出し元フォームのタイトル</br>
        /// </returns>
		public static string GetMessageBoxTitle(Form parentForm)
		{
            // 2011/06/09 メッセージタイトルは常にアプリケーション名にする様に変更
            //if (parentForm.Text != Properties.Resources.CommonMessage_ApplicationName)
            //{
            //    return Properties.Resources.CommonMessage_ApplicationName + " - " + parentForm.Text;
            //}
            //else
            //{
                return Properties.Resources.CommonMessage_ApplicationName;
            //}
		}

        /// <summary>
        /// 情報メッセージボックスを表示(OKボタン)
        /// Ownerなし
        /// </summary>
        /// <param name="messageStr">表示メッセージ</param>
        public static void ShowMessageBoxWarining(string messageStr)
        {
            string messageTitle = null;
            
            messageTitle = Properties.Resources.CommonMessage_ApplicationName;            
            
            ShowMessageBoxNoOwner(
                messageStr, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 警告メッセージボックスを表示(OKボタン)
        /// Ownerなし
        /// </summary>
        /// <param name="messageStr">表示メッセージ</param>
        public static void ShowMessageBoxError(string messageStr)
        {
            string messageTitle = null;

            messageTitle = Properties.Resources.CommonMessage_ApplicationName;

            ShowMessageBoxNoOwner(
                messageStr, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// メッセージボックスを表示
        /// Ownerなし
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        /// <param name="title">表示タイトル</param>
        /// <param name="buttons">表示ボタン</param>
        /// <param name="icon">表示アイコン</param>
        private static void ShowMessageBoxNoOwner(
            string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, buttons, icon);
        }
	}
}
