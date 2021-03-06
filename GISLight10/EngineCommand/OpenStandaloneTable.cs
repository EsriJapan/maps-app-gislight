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
    /// 属性テーブルを開くコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class OpenStandaloneTable : BaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenStandaloneTable()
        {
            string bitmapResourceName = GetType().Name + ".bmp";
            base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_bitmap.MakeTransparent(Color.Magenta);

            base.m_caption = "属性テーブルを開く";
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
        /// 属性テーブル表示フォームの表示
        /// </summary>
        public override void OnClick() {
			// 単独ﾃｰﾌﾞﾙを開く
            new Ui.FormAttributeTable(this.mainForm.SelectedTable, this.mainForm).Show(mainForm);

            mainForm.m_ToolbarControl2.Enabled = false;
        }
        #endregion
    }
}
