using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;
using XInputDotNetPure;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class GamePauseMenu : State<GameScene>
	{
		private bool _oneAction = false;

		//このステートに入った時に呼ばれる
		public override void Enter(GameScene obj)
		{
			obj.pauseEnd = false;

			_oneAction = false;

			for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
				ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_Stop");
			}

			//項目を一番上に戻しとく
			obj.gameCanvasContainer.menu_PauseMenu.SetSelect(0);

			//メニューダイアログを表示
			obj.gameCanvasContainer.dialog_PauseMenu.gameObject.SetActive(true);

			obj.gameCanvasContainer.dialog_PauseMenu.FadeOut(0.7f, () =>
			{
				obj.gameCanvasContainer.menu_PauseMenu.menuActive = true;

				//コントローラーの振動を止める
				GamePad.SetVibration(0, 0, 0);
			});
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(GameScene obj)
		{
			for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
				ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_End");
			}
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<GameScene> Update(GameScene obj)
		{
			if (obj.pauseEnd) {
				return new GameIdle();
			}

			if (!_oneAction) {
				if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Option)) {
					obj.Click_Continue();
					_oneAction = true;

					SoundManager.Instance.PlaySE("UI_Cancel");
					return this;
				}
			}

			obj.gameCanvasContainer.menu_PauseMenu.OnUpdate();


			return this;
		}
	}
}