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
    /// フィールドの別名定義を行うフォームを呼び出す
    /// </summary>
    /// <history>
    ///  2011/05/31 新規作成 
    /// </history>
    public sealed class DefineFieldNameAliasCommandForStd : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// constructor
        /// </summary>
        public DefineFieldNameAliasCommandForStd()
        {
            base.captionName = "フィールドの別名定義";

        }

        #region Overriden Class Methods

        /// <summary>
        /// コマンド生成時
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = 
                System.Windows.Forms.Control.FromHandle(ptr2);
            
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// コマンドクリック時
        /// </summary>
        public override void OnClick()
        {
            Ui.FormDefineFieldNameAlias frm = new Ui.FormDefineFieldNameAlias(mainFrm, false);
            frm.ShowDialog(mainFrm);
        }


        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled {
            get {
				bool	blnRet = false;
				// 選択ﾃｰﾌﾞﾙの有無を確認 / 属性ﾃｰﾌﾞﾙの表示状態を確認
				if(this.mainFrm.SelectedTable != null && !mainFrm.HasFormAttributeTable()) {
					blnRet = true;
				}
				return blnRet;
            }
        }
        #endregion
    }
}
