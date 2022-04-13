using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using Client.Game.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Containers
{
	//メニューを管理するコンテナー
	public class MenuContainer : SystemBehaviour
	{
		[SerializeField, Label("メニュー有効化")] public bool menuActive = true;

		[SerializeField, Label("メニュー項目")] public List<ButtonContainer> buttonList = new List<ButtonContainer>();

		[SerializeField, Label("選択中の項目番号")] private int nowSelect;

		[SerializeField, Label("項目が進む速度")] public int scrollSpeed;

		[SerializeField, Label("前に戻る")] private GlobalButtonInput.ButtonAction prevAction;
		[SerializeField, Label("次に進む")] private GlobalButtonInput.ButtonAction nextAction;
		[SerializeField, Label("決定ボタン")] private GlobalButtonInput.ButtonAction selectButton;
		//[SerializeField, Label("閉じるボタン")] private ButtonAction closeButton;

		//閉じるボタンが押されたときの処理
		//public UnityEvent closeAction;

		[SerializeField, Label("選択肢をループさせるか")] private bool menuLoop;

		private int _actionSkip = 0;

		[HideInInspector] public int invicibleCount = 0;


		public override void Initialize()
		{
			foreach (ButtonContainer button in buttonList) {
				button.Initialize();
			}

			buttonList[nowSelect].changeSprite(true);
		}

		public void SetSelect(int number)
		{
			//選択した番号に設定できない場合は終了
			if (number > buttonList.Count - 1) return;

			buttonList[nowSelect].changeSprite(false);

			nowSelect = number;
			buttonList[nowSelect].changeSprite(true);
		}

		public void OnUpdate()
		{
			//オブジェクトが有効化されてないなら処理しない
			if (!gameObject.activeSelf || !menuActive) return;

			//無効化中
			if (invicibleCount > 0) {
				invicibleCount--;
				return;
			}

			if (_actionSkip > 0) {
				_actionSkip--;
			}

			buttonList[nowSelect].OnUpdate();

			if (GlobalButtonInput.Instance.isButtonDown(prevAction)) { //前に戻る
				if (_actionSkip > 0) return;
				if (!menuLoop && nowSelect == 0) return; //これ以上戻れなければ無視

				buttonList[nowSelect].changeSprite(false);

				//一番先頭の項目なら一番後ろの項目に移動する
				if (nowSelect == 0) {
					nowSelect = buttonList.Count - 1;
				} else {
					nowSelect--;
				}

				buttonList[nowSelect].changeSprite(true);
				SoundManager.Instance.PlaySE("UI_Cursor");

				_actionSkip = scrollSpeed;
				return;
			} 

			if (GlobalButtonInput.Instance.isButtonDown(nextAction)) { //次に進む
				if (_actionSkip > 0) return;
				if (!menuLoop && nowSelect == buttonList.Count - 1) return; //これ以上進めなければ無視

				buttonList[nowSelect].changeSprite(false);

				//一番後ろの項目なら先頭に移動
				if (nowSelect == buttonList.Count - 1) {
					nowSelect = 0;
				} else {
					nowSelect++;
				}

				buttonList[nowSelect].changeSprite(true);
				SoundManager.Instance.PlaySE("UI_Cursor");

				_actionSkip = scrollSpeed;
				return;
			}

			if (GlobalButtonInput.Instance.isButtonDown(selectButton)) { //決定ボタンが押された
				buttonList[nowSelect].OnAction();
				invicibleCount = buttonList[nowSelect].invicibleTime;
			}

			if (buttonList[nowSelect].isSlider) {
				buttonList[nowSelect].OnSliderUpdate();
			}

			//if (_isButtonDown(closeButton)) { //閉じるボタンが押された
			//	SoundManager.Instance.PlaySE("UI_Cancel");
			//	closeAction.Invoke();
			//}
		}
	}
}