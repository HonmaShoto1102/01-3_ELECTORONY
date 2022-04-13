using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class GameInit : State<GameScene>
	{
		//ゲームBGMの数
		private const int GAMEBGM_COUNT = 5;

		//このステートに入った時に呼ばれる
		public override void Enter(GameScene obj)
		{
			//グローバル設定に追加
			GlobalSettingController.Instance.mobiusManager = obj.mobiusManager;

			//ゲームの初期化をする
			obj.mobiusManager.Initialize();
			obj.userController.Initialize();

			obj.gameCanvasContainer.Initialize();

			obj.clearParticleSystem.Stop();


			obj.ppsContainer.SetColor(false);

			//ボタンにアクションを登録する

			//ポーズメニュー
			obj.gameCanvasContainer.menu_PauseMenu.buttonList[0].clickedAction.AddListener(obj.Click_Continue);
			obj.gameCanvasContainer.menu_PauseMenu.buttonList[1].clickedAction.AddListener(obj.Click_Retry);
			obj.gameCanvasContainer.menu_PauseMenu.buttonList[2].clickedAction.AddListener(obj.Click_StageSelect);

			//リザルトメニュー
			obj.gameCanvasContainer.menu_ResultMenu.buttonList[2].clickedAction.AddListener(obj.Click_Next);
			obj.gameCanvasContainer.menu_ResultMenu.buttonList[1].clickedAction.AddListener(obj.Click_Retry);
			obj.gameCanvasContainer.menu_ResultMenu.buttonList[0].clickedAction.AddListener(obj.Click_StageSelect);

			//ゲームオーバーメニュー
			obj.gameCanvasContainer.menu_GameOverMenu.buttonList[0].clickedAction.AddListener(obj.Click_Retry);
			obj.gameCanvasContainer.menu_GameOverMenu.buttonList[1].clickedAction.AddListener(obj.Click_StageSelect);

			//エクストラ開放
			obj.gameCanvasContainer.menu_ExtraOpen.buttonList[0].clickedAction.AddListener(obj.Click_Extra_OK);

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

			obj.userController.stageStart.gameObject.SetActive(false);
			obj.userController.stageStart.SetRange(1.0f);
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(GameScene obj)
		{
			//既に流れているBGMがあれば止める
			SoundManager.Instance.StopBGM();

			//ゲームBGMをランダムで決める
			switch (Random.Range(0, GAMEBGM_COUNT)) {
				case 0:
					SoundManager.Instance.PlayBGM("GameTheme_1");
					break;
				case 1:
					SoundManager.Instance.PlayBGM("GameTheme_2");
					break;
				case 2:
					SoundManager.Instance.PlayBGM("GameTheme_3");
					break;
				case 3:
					SoundManager.Instance.PlayBGM("GameTheme_4");
					break;
				case 4:
					SoundManager.Instance.PlayBGM("GameTheme_5");
					break;
				default:
					SoundManager.Instance.PlayBGM("GameTheme_1");
					break;
			}
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<GameScene> Update(GameScene obj)
		{
			//初期化が終わってUpdateがよばれたらGameIdleステートに切り替える
			return new GameStart();
		}
	}
}