
using System;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRIJapan.GISLight10.Ui;
using System.Windows.Forms;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.IO;
using System.Runtime.InteropServices;
using ESRIJapan.GISLight10.Common;
using System.ComponentModel;
using ESRI.ArcGIS.ADF;
using ESRIJapan.GISLight10.Properties;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// データソースの設定コマンド
    /// </summary>
    /// <history>
    ///  2012-07-05 新規作成 
    /// </history>
    public sealed class OpenDataSource : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// データソース情報画面を表示するか
        /// </summary>
        private bool showDatasourceInfo = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenDataSource()
        {
            base.captionName = "データソース設定";

        }


        /// <summary>
        /// クリック時処理
        /// データソース選択後、レイヤーに適用します
        /// </summary>
        public override void OnClick()
        {
            try
            {
                //レイヤー
                ILayer layer = (ILayer)m_mapControl.CustomProperty ?? mainFrm.SelectedLayer;

                if (layer == null || layer is IGroupLayer)
                {
                    throw new InvalidOperationException("対象レイヤなし");
                    return;
                }

                FormDatasource frm = new FormDatasource(m_mapControl, mainFrm,layer);
                frm.ShowDialog(mainFrm);
                frm.Dispose();

 
            }
            catch (Exception ex)
            {
                var msg = Resources.OpenDataSource_Error;

                Logger.Error(msg, ex);
                MessageBoxManager.ShowMessageBoxError(mainFrm, msg);

                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="showDatasourceInfo">データソース情報画面の有無</param>
        public void OnClick(bool showDatasourceInfo)
        {
            //var temp = this.showDatasourceInfo;
            //this.showDatasourceInfo = showDatasourceInfo;
            //try
            //{
            //    OnClick();
            //}
            //finally
            //{
            //    this.showDatasourceInfo = temp;
            //}
        }

        /// <summary>
        /// クリエイト時処理
        /// メインフォームへの参照取得
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;

            IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
            System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
            mainFrm = (Ui.MainForm)cntrl2.FindForm();
        }

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                ILayer layer = (ILayer)m_mapControl.CustomProperty;
                ILayer layer2 = this.mainFrm.SelectedLayer;

                if (layer is IMapServerLayer && !(layer is IGroupLayer))
                {
                    return true;
                }

                if (layer2 == null || layer2 is IGroupLayer)
                {
                    return false;
                }
                else
                {
                    if (mainFrm.HasFormAttributeTable())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

    }
}


