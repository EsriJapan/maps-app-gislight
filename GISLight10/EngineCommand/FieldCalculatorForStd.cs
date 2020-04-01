using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// スタンドアロンテーブルのフィールド演算を行ないます
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class FieldCalculatorForStd : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FieldCalculatorForStd()
        {
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.White);

            base.m_caption = "フィールド演算";
        }

        #region Overriden Class Methods

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
				
                // ｴﾃﾞｨｯﾄ または ｼﾞｵﾘﾌｧﾚﾝｽを実行していないこと
                IEngineEditor engineEditor = new EngineEditorClass();
                if(engineEditor.EditState == esriEngineEditState.esriEngineStateNotEditing && !this.mainForm.HasGeoReference()) {
					// 選択ﾃｰﾌﾞﾙの有無を確認
					if(this.mainForm.SelectedTable != null) {
						blnRet = true;
					}
                }

				return blnRet;
            }
        }
        
        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook) {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainForm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// クリック時処理
        /// フィールド演算フォームを表示
        /// </summary>
        public override void OnClick() {
			// ﾌｨｰﾙﾄﾞ演算を開く
            Ui.FormFieldCalculate frmCalc = new Ui.FormFieldCalculate(this.m_mapControl, this.mainForm, this.mainForm.SelectedTable);
            frmCalc.ShowDialog(this.mainForm);
            frmCalc.Dispose();
        }
        #endregion
    }
}
