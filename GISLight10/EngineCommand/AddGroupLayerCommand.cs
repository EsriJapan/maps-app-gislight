using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using System.Runtime.InteropServices;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// グループレイヤを追加するコマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    class AddGroupLayerCommand : Common.EjBaseCommand
    {        
        private IMapControl3 m_mapControl;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AddGroupLayerCommand()
        {
            base.captionName = "新規グループ レイヤ";

            // 2012/08/15 ADD
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new System.Drawing.Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }
            // 2012/08/15 ADD
        }

        /// <summary>
        /// クリック時処理
        /// メインフォームへの参照取得
        /// </summary>
        public override void OnClick()
        {
            try
            {
                GISLight10.Common.Logger.Info("グループ レイヤの追加を実行");

                IGroupLayer groupLayer = new GroupLayerClass();                   
                m_mapControl.AddLayer(groupLayer, 0);
            }
            catch(COMException comex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.AddGroupLayerCommand_Error_AddGroupLayer);
                GISLight10.Common.Logger.Error(Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(comex.Message);
                GISLight10.Common.Logger.Error(comex.StackTrace);
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.MessageBoxManager.ShowMessageBoxError
                    (Properties.Resources.AddGroupLayerCommand_Error_AddGroupLayer);
                GISLight10.Common.Logger.Error(Properties.Resources.MergeCommand_ERROR_DoMerge);
                GISLight10.Common.Logger.Error(ex.Message);
                GISLight10.Common.Logger.Error(ex.StackTrace);                
            }            
        }

        /// <summary>
        /// クリエイト時処理
        /// </summary>
        /// <param name="hook">マップコントロール</param>
        public override void OnCreate(object hook)
        {            
            try
            {
                m_mapControl = (IMapControl3)hook;
            }
            catch (Exception ex)
            {
                ESRIJapan.GISLight10.Common.Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 実行可能判定
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return true;
            }
        }
    }
}
