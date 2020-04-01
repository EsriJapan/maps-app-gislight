using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using ESRIJapan.GISLight10.Ui;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRIJapan.GISLight10.Common;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRIJapan.GISLight10.Properties;


namespace ESRIJapan.GISLight10.EngineCommand
{
    class SaveLayer : Common.EjBaseCommand
    {
        private IMapControl3 m_mapControl;
        private Ui.MainForm mainFrm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SaveLayer()
        {
            base.captionName = "レイヤ ファイルへ保存";


            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.buttonImage = new System.Drawing.Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                GISLight10.Common.Logger.Error(ex.Message + " " + ex.StackTrace);
            }

        }


        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                //レイヤー
                ILayer layer = (ILayer)m_mapControl.CustomProperty;


                //ファイル保存ダイアログ表示
                FileInfo fileInfo;//選択ファイル情報
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "レイヤファイル|*.lyr|すべての種類のファイル|*.*";
                    dialog.Title = "レイヤの保存";
                    dialog.RestoreDirectory = true;
                    dialog.FileName = Path.Combine(dialog.InitialDirectory, layer.Name + ".lyr");
                    //上書き確認
                    dialog.OverwritePrompt = true;


                    dialog.FileOk += new System.ComponentModel.CancelEventHandler((sender, e) =>
                    {
                        if (dialog.FileName == "")
                        {
                            e.Cancel = true;
                        }
                    });

                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    fileInfo = new FileInfo(dialog.FileName);
                }


                if (fileInfo.Exists)
                {
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch (FileNotFoundException ex)
                    {
                        if (fileInfo.Exists)
                        {
                            throw;
                        }
                    }
                }

                //レイヤファイル作成
                using (ComReleaser releaser = new ComReleaser())
                {
                    ILayerFile layerFile = new LayerFileClass();
                    releaser.ManageLifetime(layerFile);
                    layerFile.New(fileInfo.FullName);
                    layerFile.ReplaceContents(layer);
                    layerFile.Save();
                }

            }
            catch (Exception ex)
            {
                var msg = Resources.SaveLayer_Error;

                Logger.Error(msg, ex);
                MessageBoxManager.ShowMessageBoxError(mainFrm, msg);

                return;
            }
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

                if (layer is IMapServerLayer)
                {
                    //return true;
                    return layer.Valid;
                }

                if (layer2 == null)
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
                        //return true;
                        return layer2.Valid;
                    }
                }
            }
        }

    }
}
