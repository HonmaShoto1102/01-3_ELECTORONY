using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//初期化をするステート
	public class EXStageSelectPauseMenu : State<EXStageSelectScene>
	{

		private bool _oneAction = false;

		//このステートに入った時に呼ばれる
		public override void Enter(EXStageSelectScene obj)
		{
			//初期化をする
			obj.optionEnd = false;

			_oneAction = false;

			//項目を一番上に戻しとく
			obj.menu_OptionMenu.SetSelect(0);

			//メニューダイアログを表示
			obj.dialog_OptionMenu.gameObject.SetActive(true);

			obj.dialog_OptionMenu.FadeOut(0.7f, () =>
			{
				obj.menu_OptionMenu.menuActive = true;
			});
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(EXStageSelectScene obj)
		{
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<EXStageSelectScene> Update(EXStageSelectScene obj)
		{
			obj.menu_OptionMenu.OnUpdate();
			obj.menu_SettingMenu.OnUpdate();

			//再度オプションボタンが押された場合はメニューを閉じる
			if (!_oneAction) {
				if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Option)) {
					if (obj.menu_OptionMenu.menuActive) {
						obj.Click_OptionMenu_Continue();
						_oneAction = true;
						SoundManager.Instance.PlaySE("UI_Cancel");
					}
				}
			}

			if (obj.optionEnd) {
				return new EXStageSelectIdle();
			}
			return this;
		}
	}
}