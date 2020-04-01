using System;
using System.Collections.Generic;
using System.Text;

namespace ESRIJapan.GISLight10.dispatcher
{
    /// <summary>
    /// Dispatcherでコマンド割り当て時に使用される
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-25 OpenFormPrintの記述削除
    ///  2011-05-16 ジオメトリ演算の記述追加
    /// </history>
    class Participant
    {
        // メニューアイテム選択時に起動するコマンドまたはフォーム
        // キー：メニューアイテム名、値：実行するクラスまたはESRIコマンド名
        private System.Collections.Hashtable participantTable = null;

        /// <summary>
        /// 起動対象のコマンドクラス名を格納したハッシュテーブルを返す
        /// </summary>
        public System.Collections.Hashtable Participants
        {
            get { 
                return this.participantTable; 
            }
        }

        /// <summary>
        /// Dispatcherクラスでコマンド割り当て行なう対象のコマンドクラス名称
        /// </summary>
        public Participant()
        {
            // ツールバーメニューに対応したコマンドまたはフォーム
            this.participantTable = new System.Collections.Hashtable();

            #region [ファイル]メニュー
            // file menu
            this.participantTable.Add("menuNewDoc", 
                "ESRIJapan.GISLight10.EngineCommand.CreateNewDocument");

            //this.participantTable.Add("menuOpenDoc",
            //    "esriControls.ControlsOpenDocCommand"); // esri control command  
            this.participantTable.Add("menuOpenDoc",
                "ESRIJapan.GISLight10.EngineCommand.OpenDocument"); 
            
            this.participantTable.Add("menuSaveDoc",
                "ESRIJapan.GISLight10.EngineCommand.SaveDocument");

            this.participantTable.Add("menuSaveAs",
                "esriControls.ControlsSaveAsDocCommand"); // esri control command
            
            #endregion

            #region [選択]メニュー
            // main form への参照の要るコマンドの場合は
            // ESRIJapan.GISLight10.EngineCommand.Openで始まる名称
            //this.participantTable.Add("menuPrint",
            //    "ESRIJapan.GISLight10.EngineCommand.OpenFormPrint"); 

            this.participantTable.Add("menuExportMap",
                "ESRIJapan.GISLight10.EngineCommand.OpenExportMapCommand");

            //this.participantTable.Add("menuExitApp", 
            //    "ESRIJapan.GISLight10.EngineCommand.");

            // sentakuk menu
            this.participantTable.Add("menuSentakuZokukeiKensaku",
                "ESRIJapan.GISLight10.EngineCommand.AttributeSearchCommand");
            
            this.participantTable.Add("menuSentakuKukanKensaku",
                "ESRIJapan.GISLight10.EngineCommand.SpatialSearchCommand");
            
            this.participantTable.Add("menuSentakuZokuseichiSyukei",
                "ESRIJapan.GISLight10.EngineCommand.AttributeSumCommand");

            this.participantTable.Add("menuSentakuSelectableLayerSettings",
                "ESRIJapan.GISLight10.EngineCommand.SelectableLayerSettingsCommand");

            // データソース設定
            this.participantTable.Add("menuDataSourceSetting",
                "ESRIJapan.GISLight10.EngineCommand.OpenDataSource");

            // CADﾃﾞｰﾀの読み込み
            this.participantTable.Add("menuReadCAD",
                "ESRIJapan.GISLight10.EngineCommand.ReadDataSource");
                
            // ArcGISﾃﾞｰﾀの読み込み
            this.participantTable.Add("menuAddData",
				"esriControls.ControlsAddDataCommand");

            #endregion

            #region [テーブル結合とリレート]メニュー

            // table join relate
            this.participantTable.Add("menuTableJoin", 
                "ESRIJapan.GISLight10.EngineCommand.JoinTableCommand");

            this.participantTable.Add("menuRemoveJoin",
                "ESRIJapan.GISLight10.EngineCommand.RemoveJoinCommand");            

            this.participantTable.Add("menuRelate", 
                "ESRIJapan.GISLight10.EngineCommand.RelateCommand");

            this.participantTable.Add("menuRemoveRelate",
                "ESRIJapan.GISLight10.EngineCommand.RemoveRelateCommand");
            #endregion

            #region [演算と解析]メニュー
            // ジオメトリ演算
            this.participantTable.Add("menuGeometryCalculate",
                "ESRIJapan.GISLight10.EngineCommand.GeometryCalculatorCommand");

            // フィールド演算
            this.participantTable.Add("menuFieldCalculate",
                "ESRIJapan.GISLight10.EngineCommand.FieldCalculatorCommand");

            // インターセクト
            this.participantTable.Add("menuIntersect",
                "ESRIJapan.GISLight10.EngineCommand.IntersectCommand");

            // XYデータの追加
            this.participantTable.Add("menuAddXYData",
                "ESRIJapan.GISLight10.EngineCommand.AddXYDataCommand");

            // ジオリファレンス
            this.participantTable.Add("menuGeoReference",
                "ESRIJapan.GISLight10.EngineCommand.GeoReferenceCommand");
            #endregion

            #region [プラグイン]メニュー

            // なし

            #endregion

            #region [オプション]メニュー

            this.participantTable.Add("toolMenuOptionSettings",
                "ESRIJapan.GISLight10.EngineCommand.OptionSettingsCommand");

            #endregion

            #region [ページレイアウト]メニュー
            // PageLayout
            // menuItemAddTextElement
            this.participantTable.Add("menuItemAddTextElement",
                "ESRIJapan.GISLight10.EngineCommand.CreateScaleText");

            // menuItemAddNorthArrowElement
            this.participantTable.Add("menuItemAddNorthArrowElement",
                "ESRIJapan.GISLight10.EngineCommand.CreateNorthArrow");

            // menuItemAddScaleElement
            this.participantTable.Add("menuItemAddScaleElement",
                "ESRIJapan.GISLight10.EngineCommand.CreateScaleBar");
            #endregion



        }
    }
}
