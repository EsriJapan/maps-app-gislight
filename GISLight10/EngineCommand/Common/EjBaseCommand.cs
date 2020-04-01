using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

// ESRI ArcGIS Engine関連の名前空間をインポート
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.EngineCommand.Common
{
    /// <summary>
    /// ArcGIS Engineのコマンド共通となる処理を記述する。
    /// </summary>
    /// <remarks>
    /// コマンドのキャプション、カテゴリ名などはリソースファイルから取得する。
    /// リソース名は以下の書式に従う"ICommand_クラス名_項目名"
    /// </remarks>
    /// <history>
    /// 2009/2/2     新規
    /// 2010/6/17    シンプルなクラスに再構築
    /// 2011/01/27 ArcGISCategoryRegistration,ArcGISCategoryUnregistrationの引数xmlコメント追記
    /// </history>
    public abstract class EjBaseCommand : ICommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        /// <param name="registerType">Register Type(CLSID)</param>
        public static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        /// <param name="registerType">Unregister Type(CLSID)</param>
        public static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        /// <summary>
        /// コマンドのキャプション名
        /// </summary>
        protected string captionName;

        /// <summary>
        /// コマンドのカテゴリ名
        /// </summary>
        protected string categoryName;

        /// <summary>
        /// コマンドのボタンのイメージ
        /// </summary>
        protected Bitmap buttonImage;

        /// <summary>
        /// コマンドボタンのビットマップハンドル
        /// </summary>
        protected IntPtr buttonHandle = (IntPtr)0;

        /// <summary>
        /// コマンドの名前（ArcGIS Engineのシステム内で使われる文字列)
        /// </summary>
        protected string commandName;

        /// <summary>
        /// コマンドの説明
        /// </summary>
        protected string commandMessage;

        /// <summary>
        /// ボタンのツールチップ
        /// </summary>
        protected string buttonTooltip;

        /// <summary>
        /// OnCreateメソッドで渡されるデータを扱いやすくするためのクラス
        /// </summary>
        protected IHookHelper pHookHelper = null;

        /// <summary>
        /// Engine編集機能
        /// </summary>
        protected IEngineEditor pEngineEditor;

        /// <summary>
        /// コマンドの基底クラスコンストラクタ。
        /// 各種変数の初期化を行う。
        /// Caption,Category,Bitmap,Message,Tooltipはリソースファイルか取得する
        /// </summary>
        public EjBaseCommand()
        {
            const string FORMAT_CAPTION = "ICommand_{0}_CAPTION";
            const string FORMAT_CATEGORY = "ICommand_{0}_CATEGORY";
            const string FORMAT_BITMAP = "ICommand_{0}_BITMAP";
            const string FORMAT_MESSAGE = "ICommand_{0}_MESSAGE";
            const string FORMAT_TOOLTIP = "ICommand_{0}_TOOLTIP";
            string className, key;

            className = GetClassName();

            // キャプションの取得
            key = string.Format(FORMAT_CAPTION, className);
            captionName = GetResourceString(key);

            // カテゴリの取得
            key = string.Format(FORMAT_CATEGORY, className);
            categoryName = GetResourceString(key);

            // 画像の取得
            key = string.Format(FORMAT_BITMAP, className);
            //buttonImage = (Bitmap)ESRIJapan.GISLight10.EngineCommand.CommandResource.ResourceManager.GetObject(key);
            if (buttonImage != null)
            {
                buttonHandle = buttonImage.GetHbitmap();
            }

            // コマンド名の設定
            commandName = string.Format("{0}_{1}", categoryName, captionName);

            // メッセージの取得
            key = string.Format(FORMAT_MESSAGE, className);
            commandMessage = GetResourceString(key);

            // ツールチップの取得
            key = string.Format(FORMAT_TOOLTIP, className);
            buttonTooltip = GetResourceString(key);
        }


        /// <summary>
        /// クラス名の文字列を返す。
        /// </summary>
        /// <returns>クラス名</returns>
        protected string GetClassName()
        {
            string className;

            className = this.GetType().Name;

            return className;
        }


        /// <summary>
        /// リソースファイルから文字列を取得する。
        /// 取得できなかった場合、キーを返す
        /// </summary>
        /// <param name="key">取得する文字列のキー</param>
        /// <returns>リソースファイルから取得した文字列</returns>
        protected string GetResourceString(string key)
        {
            //System.Resources.ResourceManager manager = null;
            string caption = "";

            //manager = ESRIJapan.GISLight10.EngineCommand.CommandResource.ResourceManager;
            //caption = manager.GetString(key);
            //if (caption == null)
            //{
            //    caption = key;
            //}

            return caption;
        }

        #region ICommand メンバ

        /// <summary>
        /// ボタンの画像となるビットマップを返す
        /// </summary>
        public int Bitmap
        {
            get
            {
                if (buttonImage == null)
                {
                    return 0;
                }
                if (buttonHandle == (IntPtr)0)
                {
                    buttonHandle = buttonImage.GetHbitmap();
                }
                return buttonHandle.ToInt32();
            }
        }


        /// <summary>
        /// コマンドのテキストとなる文字列
        /// </summary>
        public string Caption
        {
            get
            {
                return captionName;
            }
        }


        /// <summary>
        /// コマンドのカテゴリ名
        /// </summary>
        public string Category
        {
            get
            {
                return categoryName;
            }
        }


        /// <summary>
        /// コマンドのチェック状態。
        /// </summary>
        public bool Checked
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// コマンドのEnable状態。
        /// 編集状態が開始され、編集対象レイヤがポイント、ポリライン、ポリゴンの場合trueを返す
        /// </summary>
        public virtual bool Enabled
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// ヘルプのコンテキストIDを返す
        /// </summary>
        public int HelpContextID
        {
            get
            {
                return 0;
            }
        }


        /// <summary>
        /// ヘルプファイル
        /// </summary>
        public string HelpFile
        {
            get
            {
                return null;
            }
        }


        /// <summary>
        /// コマンドの説明文字列
        /// </summary>
        public string Message
        {
            get
            {
                return commandMessage;
            }
        }


        /// <summary>
        /// システムで利用するコマンドの名前
        /// </summary>
        public string Name
        {
            get
            {
                return commandName;
            }
        }


        /// <summary>
        /// ArcGIS Engineのツールから呼び出される
        /// </summary>
        /// <param name="hook">コマンドが使われるツールバーコントロール</param>
        public virtual void OnCreate(object hook)
        {
            try
            {
                pHookHelper = new HookHelperClass();
                pHookHelper.Hook = hook;
                pEngineEditor = new EngineEditorClass(); //this class is a singleton
            }
            catch(Exception)
            {
                pHookHelper = null;
                throw;
            }
        }


        /// <summary>
        /// ボタンのツールチップ
        /// </summary>
        public string Tooltip
        {
            get
            {
                return buttonTooltip;
            }
        }


        /// <summary>
        /// ボタンクリック時の処理
        /// </summary>
        /// <remarks>
        /// 派生クラスが実装する。
        /// </remarks>
        public abstract void OnClick();

        #endregion
    }
}
