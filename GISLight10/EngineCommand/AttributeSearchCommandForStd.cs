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
    /// 属性検索
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class AttributeSearchCommandForStd : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AttributeSearchCommandForStd()
        {

            base.captionName = "属性検索";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new Bitmap(GetType(), bitmapResourceName);
				base.buttonImage.MakeTransparent(Color.White);
            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }

        #region Overriden Class Methods

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
        /// 属性検索フォームの表示
        /// </summary>
        public override void OnClick() {
            //new Ui.FormAttributeSearch(m_mapControl, mainForm, mainForm.SelectedTable).ShowDialog(mainForm);
        }

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
				// 選択ﾃｰﾌﾞﾙの有無を確認 / 属性ﾃｰﾌﾞﾙ非表示
				if(this.mainForm.SelectedTable != null && !mainForm.HasFormAttributeTable()) {
					blnRet = true;
				}
				return blnRet;
            }
        }
        #endregion
    }
}
