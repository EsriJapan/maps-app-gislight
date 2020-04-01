using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ESRIJapan.GISLight10.Common {
	/// <summary>
	/// リストビュー上でアイテムを編集するためのテキストボックス
	/// </summary>
	public class ListViewInputBox : TextBox
	{
		/// <summary>
		/// イベント・パラメータ
		/// </summary>
		public class InputEventArgs : EventArgs {
			public int		ListItemIndex = -1;
			public int		SubItemIndex = -1;
			public string	OldValue = "";
			public string	NewValue = "";
		}

		public delegate void InputEventHandler(object sender, InputEventArgs e);

		// イベントデリゲートの宣言
		public event InputEventHandler FinishInput;

		// 内部変数
		private InputEventArgs	_EvArgs = new InputEventArgs();
		private bool			_blnFinished = false;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parent">対象となるListViewコントロール</param>
		/// <param name="item">編集対象のアイテム</param>
		/// <param name="subitem_index">編集する対象の列</param>
		public ListViewInputBox(ListView TargetListView, ListViewItem SelectedListItem, int SubItemIndex) : base() {
			// イベントパラメータ設定
			_EvArgs.ListItemIndex = SelectedListItem.Index;
			_EvArgs.SubItemIndex = SubItemIndex;
			_EvArgs.OldValue = SelectedListItem.SubItems[SubItemIndex].Text;
			_EvArgs.NewValue = SelectedListItem.SubItems[SubItemIndex].Text;

			int intLeft = 0;
			for(int intCol = 0; intCol < SubItemIndex; intCol++) {
				intLeft += TargetListView.Columns[intCol].Width;
			}
			int intW = SelectedListItem.SubItems[SubItemIndex].Bounds.Width;
			int intH = SelectedListItem.SubItems[SubItemIndex].Bounds.Height;

			// 配置設定
			this.Parent = TargetListView;
			this.AutoSize = false;	// 隠し属性
			//this.Padding = new Padding(0, 0, 0, 0);
			this.Size = new Size(intW, intH);
			this.Left = intLeft;
			this.Top = SelectedListItem.Position.Y - 2;

			// プロパティ設定
			this.Text = SelectedListItem.SubItems[SubItemIndex].Text;
			this.ImeMode = ImeMode.NoControl;
			this.Multiline = false;
			this.BorderStyle = BorderStyle.FixedSingle;
			
			// イベントキャプチャリング設定
			this.LostFocus += new EventHandler(InputBox_LostFocus);
			this.KeyDown += new KeyEventHandler(InputBox_KeyDown);

			this.Focus();
		}

		/// <summary>
		/// イベント対応 (値の入力完了)
		/// </summary>
		/// <param name="ValueText">入力値</param>
		void Finish(string ValueText) {
			// イベントによる複数回コールの制御
			if(!_blnFinished) {
				_blnFinished = true;
				this.Hide();
				
				// イベント コール
				_EvArgs.NewValue = ValueText;
				FinishInput(this, _EvArgs);
			}
		}

		/// <summary>
		/// テキストボックス KeyDown EVENT
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void InputBox_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {			// Enter Key
				this.Finish(this.Text);
			}
			else if(e.KeyCode == Keys.Escape) {		// ESC Key
				this.Finish(_EvArgs.OldValue);
			}
			else if(e.KeyCode == Keys.Tab) {		// Tab Key
				this.Finish(this.Text);
			}
/*			else if(e.KeyCode == Keys.Up) {			// ↑ Key
				this.Finish(this.Text);
			}
			else if(e.KeyCode == Keys.Down) {		// ↓ Key
				this.Finish(this.Text);
			}*/
		}

		/// <summary>
		/// テキストボックス LostFocus EVENT
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void InputBox_LostFocus(object sender, EventArgs e) {
			this.Finish(this.Text);
		}
	}
}