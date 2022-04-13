using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//初期化をするステート
	public class StageSelectInit : State<StageSelectScene>
	{
		//このステートに入った時に呼ばれる
		public override void Enter(StageSelectScene obj)
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

			obj.LBRBIconUpdate();
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(StageSelectScene obj)
		{
			//直前に選択されたステージにカーソルを合わせる
			int area = 0;

			for (int i = 0; i < GlobalSettingController.Instance.prevSelectStageID; i++) {
				int work = (i % 5);
				if (work == 0) {
					area++;
				}

			}
			obj.nowAreaID = area - 1;

			//自分の画面以外のパーティクルを消す
			for (int i = 0; i < obj.areaList.Count; i++) {
				if (i == obj.nowAreaID) {
					foreach (var container in ParticleController.Instance.GetParticleAll(i)) {
						container.AllShow();
					}
				} else {
					foreach (var container in ParticleController.Instance.GetParticleAll(i)) {
						container.AllHide();
					}
				}
			}

			obj.LBRBIconUpdate();

			StageSelectContainer stageContainer = StageSelectController.Instance.GetStageSelect(GlobalSettingController.Instance.prevSelectStageID);
			if (stageContainer != null) {
				stageContainer.SetStageSelect(true);
				obj.nowSelectStage = stageContainer;

				if (area >= 2) {
					for (int i = 0; i < obj.areaList.Count; i++) {
						Vector3 pos = obj.areaList[i].transform.position;

						pos.x -= (area - 1) * 1920;

						obj.areaList[i].transform.position = pos;
					}
				}
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
		public override State<StageSelectScene> Update(StageSelectScene obj)
		{
			return new StageSelectIdle();
		}
	}
}
