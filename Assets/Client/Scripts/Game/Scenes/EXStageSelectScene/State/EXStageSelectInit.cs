using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//初期化をするステート
	public class EXStageSelectInit : State<EXStageSelectScene>
	{
		//このステートに入った時に呼ばれる
		public override void Enter(EXStageSelectScene obj)
		{
			//初期化をする

			//FadeUIオブジェクトの子にあるMenuContainerを探して登録する
			foreach (Transform child in obj.dialog_OptionMenu.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_OptionMenu = menu;
					break;
				}
			}

			foreach (Transform child in obj.dialog_SettingMenu.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_SettingMenu = menu;
					break;
				}
			}

			obj.dialog_OptionMenu.gameObject.SetActive(false);
			obj.dialog_SettingMenu.gameObject.SetActive(false);

			obj.menu_OptionMenu.Initialize();
			obj.menu_SettingMenu.Initialize();

			obj.menu_OptionMenu.menuActive = false;
			obj.menu_SettingMenu.menuActive = false;
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(EXStageSelectScene obj)
		{
			StageSelectContainer stageContainer = StageSelectController.Instance.GetStageSelect(GlobalSettingController.Instance.prevSelectEXStageID);
			if (stageContainer != null) {
				obj.nowSelectStage = stageContainer;
				if (UserSaveData.Instance.GetStar(5) >= 1) {
					stageContainer.SetStageSelect(true);
					obj.notEXPlay = false;
				} else {
					obj.notEXPlay = true;
				}
			}

			//パーティクルの描画
			foreach (var container in ParticleController.Instance.GetParticleAll(0)) {
				container.AllShow();
			}

			obj.fade.SetRange(1);
			obj.fade.FadeOut(1, () =>
			{
				obj.buttonEnd = false;
			});

			//ウィンドウの情報を更新する
			obj.UpdateWindow();

			SoundManager.Instance.StopBGM();
			SoundManager.Instance.PlayBGM("StageSelectTheme");
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<EXStageSelectScene> Update(EXStageSelectScene obj)
		{
			return new EXStageSelectIdle();
		}
	}
}
