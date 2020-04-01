using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;


namespace ESRIJapan.GISLight10.EngineCommand.Common
{
    /// <summary>
    /// ArcGIS Engineのツール共通となる処理を記述する。
    /// </summary>
    /// <history>
    /// 2009/2/3     新規
    /// 2010/6/17    シンプルなクラスに再構築
    /// 2011/01/27 OnContextMenuのxmlコメント追記
    /// </history>
    public abstract class EjBaseTool : EjBaseCommand, ITool
    {
        /// <summary>
        /// カーソルアイコン
        /// </summary>
        protected System.Windows.Forms.Cursor cursorIcon = null;

        /// <summary>
        /// ツールの基底クラスコンストラクタ
        /// 各種変数の初期化を行う
        /// </summary>
        public EjBaseTool()
        {
            const string FORMAT_CUSOR = "ESRIJapan.GISLight10.Resources.{0}_CURSOR.cur";
            string key;

            // アイコンカーソルの取得
            key = string.Format(FORMAT_CUSOR, this.GetType().Name);
            cursorIcon = new System.Windows.Forms.Cursor(GetType().Assembly.GetManifestResourceStream(key));
        }


        #region ITool メンバ

        //---------------------------------------------------------------------
        /// <summary>
        /// マウスカーソルのハンドル
        /// </summary>
        //---------------------------------------------------------------------
        public int Cursor
        {
            get
            {
                if (cursorIcon == null)
                {
                    return 0;
                }

                return cursorIcon.Handle.ToInt32();
            }
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// ツールの利用許可
        /// </summary>
        /// <returns>常にtrueを返す</returns>
        //---------------------------------------------------------------------
        public virtual bool Deactivate()
        {
            int i, num;
            List<int> removeIndex;
            IToolbarControlDefault pToolbarControl;

            pToolbarControl = (IToolbarControlDefault)pHookHelper.Hook;

            // OperationStackの整理
            removeIndex = new List<int>();
            for (i = 0, num = pToolbarControl.OperationStack.Count; i < num; i++)
            {
                if (pToolbarControl.OperationStack.get_Item(i) is IEngineSketchOperation)
                {
                    removeIndex.Add(i);
                }
            }
            for (i = removeIndex.Count - 1; i > 0; i--)
            {
                pToolbarControl.OperationStack.Remove(removeIndex[i]);
            }

            return true;
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// 右クリックされたXY座標にコンテクストメニューが表示状態になった場合に呼ばれる。
        /// </summary>
        /// <remarks>
        /// 本クラスでは何の処理も行わない仮想関数として定義する。
        /// </remarks>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>正常に処理出来た場合にはtrue</returns>
        //---------------------------------------------------------------------
        public virtual bool OnContextMenu(int x, int y)
        {
            return false;
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// カレントツールの時、マップ上でマウスをダブルクリックしたときに呼ばれる。
        /// </summary>
        /// <remarks>
        /// 本クラスでは、何の処理も行わない仮想関数として定義する。
        /// </remarks>
        //---------------------------------------------------------------------
        public virtual void OnDblClick()
        {
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// カレントツールの時、キーが押下されたときに呼ばれる。
        /// </summary>
        /// <remarks>
        /// 本クラスでは何の処理も行わない仮想関数として定義する。
        /// </remarks>
        /// <param name="keyCode">押されたキーのコード値</param>
        /// <param name="shift">シフトキーの状態</param>
        //---------------------------------------------------------------------
        public virtual void OnKeyDown(int keyCode, int shift)
        {
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// カレントツールの時、押されたキーが上げられたときげられたときに呼ばれる。
        /// </summary>
        /// <remarks>
        /// 本クラスでは何の処理も行わない仮想関数として定義する。
        /// </remarks>
        /// <param name="keyCode">押されたキーのコード値</param>
        /// <param name="shift">シフトキーの状態</param>
        //---------------------------------------------------------------------
        public virtual void OnKeyUp(int keyCode, int shift)
        {
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// カレントツールの時、マウスがクリックされたときに呼び出される。
        /// </summary>
        /// <remarks>
        /// 本クラスでは何の処理も行わない仮想関数として定義する。
        /// </remarks>
        /// <param name="button">マウスのボタンの状態</param>
        /// <param name="shift">シフトキーの状態</param>
        /// <param name="x">クリックされたX座標</param>
        /// <param name="y">クリックされたY座標</param>
        //---------------------------------------------------------------------
        public virtual void OnMouseDown(int button, int shift, int x, int y)
        {
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// カレントツールの時、マウスがマップ上で動いたときに呼び出される。
        /// </summary>
        /// <remarks>
        /// 本クラスでは何の処理も行わない仮想関数として定義する。
        /// </remarks>
        /// <param name="button">マウスのボタンの状態</param>
        /// <param name="shift">シフトキーの状態</param>
        /// <param name="x">クリックされたX座標</param>
        /// <param name="y">クリックされたY座標</param>
        //---------------------------------------------------------------------
        public virtual void OnMouseMove(int button, int shift, int x, int y)
        {
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// カレントツールの時、マウスボタンがマップ上で上げられたときに呼び出される。
        /// </summary>
        /// <remarks>
        /// 本クラスでは何の処理も行わない仮想関数として定義する。
        /// </remarks>
        /// <param name="button">マウスのボタンの状態</param>
        /// <param name="shift">シフトキーの状態</param>
        /// <param name="x">クリックされたX座標</param>
        /// <param name="y">クリックされたY座標</param>
        //---------------------------------------------------------------------
        public virtual void OnMouseUp(int button, int shift, int x, int y)
        {
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// マップが再描画されるときに呼び出される
        /// </summary>
        /// <remarks>
        /// 本クラスでは何の処理も行わない仮想関数として定義する。
        /// </remarks>
        /// <param name="hdc">マップのデバイスコンテキスト</param>
        //---------------------------------------------------------------------
        public virtual void Refresh(int hdc)
        {
        }

        #endregion
    }
}
