using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ESRIJapan.PluginInterface
{
    /// <summary>
    /// プラグイン側で実装するインターフェース
    /// </summary>
    public interface IPlugin {
		/// <summary>
		/// プラグインの名称を取得します
		/// </summary>
        string Name				{ get; }
        /// <summary>
        /// バージョンを取得します
        /// </summary>
        string Version			{ get; }
        /// <summary>
        /// プラグインの説明を取得します
        /// </summary>
        string Description		{ get; }
        /// <summary>
        /// プラグインが有効かどうかを取得します
        /// </summary>
        bool Enabled			{ get; }
        /// <summary>
        /// プラグインのホストインターフェースを取得します
        /// </summary>
        IPluginHostApp IHostApp	{ get; set; }
        
        /// <summary>
        /// プラグインを実行します
        /// </summary>
        /// <param name="parameters"></param>
        void Run(object[] parameters);
    }
    
    /// <summary>
    /// プラグインを実行するアプリケーション側で実装するインターフェース
    /// </summary>
    public interface IPluginHostApp {
		/// <summary>
		/// 開いているマップドキュメントを取得します
		/// </summary>
		string MapDocumentFile			{ get; }
		/// <summary>
		/// マップの表示縮尺を取得または設定します
		/// </summary>
		double MapScale					{ get; set; }
		/// <summary>
		/// マップの表示範囲を取得または設定します
		/// </summary>
		IEnvelope MapExtent				{ get; set; }
		/// <summary>
		/// 選択レイヤを取得または設定します
		/// </summary>
		ILayer SelectedLayer			{ get; set; }
		/// <summary>
		/// すべてのフィーチャーレイヤーを取得します
		/// </summary>
		IFeatureLayer[] FeatureLayers	{ get; }
		/// <summary>
		/// マップ画面を表示しているかどうかを取得または設定します（False時はレイアウト画面）
		/// </summary>
		bool IsMapVisible				{ get; set; }
		/// <summary>
		/// 編集状態かどうかを取得します
		/// </summary>
		bool IsEditMode					{ get; }

#region レイヤ関係のメソッド
		/// <summary>
		/// 指定のレイヤが存在するかどうかを取得します
		/// </summary>
		/// <param name="LayerName">レイヤ名</param>
		/// <returns>有 / 無</returns>
		bool HasLayer(string LayerName);
		/// <summary>
		/// 指定のレイヤを取得します
		/// </summary>
		/// <param name="LayerName">レイヤ名</param>
		/// <returns>レイヤ</returns>
		ILayer GetLayer(string LayerName);
#endregion

#region マップ関係のメソッド
		/// <summary>
		/// マップ範囲を全域に設定します
		/// </summary>
		void ShowMapFullExtent();
		/// <summary>
		/// 現在のビュー範囲を再描画します
		/// </summary>
		void RefreshViewExtent();
		/// <summary>
		/// 指定の範囲を再描画します
		/// </summary>
		/// <param name="Extent">地図範囲</param>
		void RefreshViewExtent(ESRI.ArcGIS.Geometry.IEnvelope Extent);
		/// <summary>
		/// 指定のオブジェクトを指定の範囲で再描画します
		/// </summary>
		/// <param name="Extent">地図範囲</param>
		/// <param name="DataObject">対象データ</param>
		void RefreshViewExtent(ESRI.ArcGIS.Geometry.IEnvelope Extent, object DataObject);
		/// <summary>
		/// ビューの全体を再描画します
		/// </summary>
		void RefreshView();
#endregion

#region フォームを表示するメソッド
		/// <summary>
		/// 指定のフォームを表示します
		/// </summary>
		/// <param name="FormWindow">フォーム</param>
		void ShowForm(Form FormWindow);
		/// <summary>
		/// メインフォーム上に指定のフォームを表示します
		/// </summary>
		/// <param name="FormWindow">フォーム</param>
		void ShowFormOnMainForm(Form FormWindow);
		/// <summary>
		/// モーダルウインドウとして指定のフォームを表示します
		/// </summary>
		/// <param name="FormWindow">フォーム</param>
		/// <returns>DialogResult</returns>
		DialogResult ShowFormOnModalWindow(Form FormWindow);
#endregion

#region メッセージボックスを表示するメソッド
		/// <summary>
		/// メッセージボックスを表示します（情報）
		/// </summary>
		/// <param name="Message">メッセージ</param>
		void ShowMessage_I(string Message);
		/// <summary>
		/// メッセージボックスを表示します（警告）
		/// </summary>
		/// <param name="Message">メッセージ</param>
		void ShowMessage_W(string Message);
		/// <summary>
		/// メッセージボックスを表示します（エラー）
		/// </summary>
		/// <param name="Message">メッセージ</param>
		void ShowMessage_E(string Message);
		/// <summary>
		/// メッセージボックスを表示します（確認:3択）
		/// </summary>
		/// <param name="Message">メッセージ</param>
		/// <returns>DialogResult</returns>
		DialogResult ShowMessage_C(string Message);
#endregion
    }
}
