using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class TutorialInit : State<TutorialScene>
	{
		//このステートに入った時に呼ばれる
		public override void Enter(TutorialScene obj)
		{
			//グローバル設定に追加
			GlobalSettingController.Instance.mobiusManager = obj.mobiusManager;

			//FadeUIオブジェクトの子にあるMenuContainerを探して登録する
			foreach (Transform child in obj.dialog_TutorialEnd.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_TutorialEnd = menu;
					break;
				}
			}

			obj.dialog_TutorialEnd.gameObject.SetActive(false);
			obj.menu_TutorialEnd.Initialize();
			obj.menu_TutorialEnd.menuActive = false;

			//ゲームの初期化をする
			obj.mobiusManager.Initialize();
			obj.userController.Initialize();

			obj.gameCanvasContainer.Initialize();

			obj.ppsContainer.SetColor(false);

			if (obj.mobiusManager.GetAllMobiusCount() == 3) {
				Camera.main.transform.position = obj.cameraPosContainer.mobius_3_Pos.position;
				obj.userController.stageStart.transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
			} else if (obj.mobiusManager.GetAllMobiusCount() == 2) {
				Camera.main.transform.position = obj.cameraPosContainer.mobius_2_Pos.position;
				obj.userController.stageStart.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
			} else if (obj.mobiusManager.GetAllMobiusCount() == 1) {
				Camera.main.transform.position = obj.cameraPosContainer.mobius_1_Pos.position;
				obj.userController.stageStart.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
			}

			obj.gradationContainer = obj.messageText.GetComponent<GradationContainer>();

			obj.messageBG.gameObject.SetActive(false);
			obj.messageText.gameObject.SetActive(false);
			obj.message_AButton.gameObject.SetActive(false);
			obj.arrow.gameObject.SetActive(false);
			obj.arrow2.gameObject.SetActive(false);

			obj.userController.stageStart.gameObject.SetActive(false);
			obj.userController.stageStart.SetRange(1.0f);
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(TutorialScene obj)
		{
			//既に流れているBGMがあれば止める
			SoundManager.Instance.StopBGM();

			//ゲームBGMを再生
			SoundManager.Instance.PlayBGM("TutorialTheme");
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<TutorialScene> Update(TutorialScene obj)
		{
			//初期化が終わってUpdateがよばれたらGameIdleステートに切り替える
			return new TutorialStart();
		}
	}
}