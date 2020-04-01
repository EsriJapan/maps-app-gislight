using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;

namespace ESRIJapan.GISLight10.EngineCommand
{
    /// <summary>
    /// テーブル結合の解除コマンド
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成 
    /// </history>
    public sealed class RemoveJoinCommand : Common.EjBaseCommand
    {
           private IMapControl3 m_mapControl;
           private Ui.MainForm mainFrm;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public RemoveJoinCommand()
            {
                base.captionName = "テーブル結合の解除";

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
            /// クリック時処理
            /// テーブル結合の解除フォームの表示
            /// </summary>
            public override void OnClick()
            {
                Ui.FormRemoveJoin frm = new Ui.FormRemoveJoin(m_mapControl);

                //IntPtr ptr2 = (System.IntPtr)m_mapControl.hWnd;
                //System.Windows.Forms.Control cntrl2 = System.Windows.Forms.Control.FromHandle(ptr2);
                //mainFrm = (Ui.MainForm)cntrl2.FindForm();

                if (!frm.IsDisposed)
                {
                    frm.ShowDialog(mainFrm);
                }                
            }

            /// <summary>
            /// 実行可能判定
            /// </summary>
            public override bool Enabled
            {
                get
                {
                    ESRIJapan.GISLight10.Common.LayerManager pLayerManager =
                        new ESRIJapan.GISLight10.Common.LayerManager();

                    List<IFeatureLayer> featureLayerList =
                        pLayerManager.GetFeatureLayers(m_mapControl.Map);

                    if (featureLayerList.Count > 0)
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
                    else
                    {
                        return false;
                    }
                }
            }
    }
}
