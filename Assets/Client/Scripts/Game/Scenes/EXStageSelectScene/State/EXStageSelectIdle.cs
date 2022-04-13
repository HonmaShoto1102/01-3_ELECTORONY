using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Game.Scenes
{
	//待機中のステート
	public class EXStageSelectIdle : State<EXStageSelectScene>
	{
		private int _actionSkip = 0;

		//このステートに入った時に呼ばれる
		public override void Enter(EXStageSelectScene obj)
		{
			//初期化をする

		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(EXStageSelectScene obj)
		{
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<EXStageSelectScene> Update(EXStageSelectScene obj)
		{

			if (obj.buttonEnd) return this;

			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Option)) { //オプションボタンが押された
				SoundManager.Instance.PlaySE("TitleStart");
				return new EXStageSelectPauseMenu();
			}

			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Y_Button)) {
				obj.buttonEnd = true;
				GlobalSettingController.Instance.prevSelectEXStageID = obj.nowSelectStage.stageID;
				SoundManager.Instance.PlaySE("UI_Enter");
				obj.fade.FadeIn(1, () =>
				{
					SceneManager.LoadScene("StageSelectScene");
				});
			}

			if (obj.notEXPlay) return this;

			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
				GlobalSettingController.Instance.SetStageID(obj.nowSelectStage.stageID, true);
				obj.buttonEnd = true;
				SoundManager.Instance.PlaySE("UI_Enter");
				obj.fade.FadeIn(1, () =>
				{
					SceneManager.LoadScene("LoadingScene");
				});
			}

			if (_actionSkip > 0) {
				_actionSkip--;
			}


			if (_actionSkip > 0) return this;


			if (obj.nowSelectStage.nextButtonList.Count > 0) {
				for (int i = 0; i < obj.nowSelectStage.nextButtonList.Count; i++) {
					if (GlobalButtonInput.Instance.isButtonDown(obj.nowSelectStage.nextButtonList[i])) {

						//このステージをクリアしていないならそれ以上先には行けない
						if (UserSaveData.Instance.GetStar(obj.nowSelectStage.stageID) <= 0) return this;

						//次のステージを選ぶ
						StageSelectContainer stage = StageSelectController.Instance.GetStageSelect(obj.nowSelectStage.stageID + 1);

						//ステージを開放していないなら選べない
						if (UserSaveData.Instance.GetStar(stage.beforeClearStage) <= 0) return this;

						if (stage != null) {
							obj.nowSelectStage.SetStageSelect(false);
							stage.SetStageSelect(true);
							obj.nowSelectStage = stage;

							//ウィンドウの情報を更新する
							obj.UpdateWindow();
							SoundManager.Instance.PlaySE("UI_Cursor");
						}

						_actionSkip = obj.scrollSpeed;
						break;
					}
				}
			}

			if (obj.nowSelectStage.prevButtonList.Count > 0) {
				for (int i = 0; i < obj.nowSelectStage.prevButtonList.Count; i++) {
					if (GlobalButtonInput.Instance.isButtonDown(obj.nowSelectStage.prevButtonList[i])) {

						//前のステージを選ぶ
						StageSelectContainer stage = StageSelectController.Instance.GetStageSelect(obj.nowSelectStage.stageID - 1);
						if (stage != null) {
							obj.nowSelectStage.SetStageSelect(false);
							stage.SetStageSelect(true);
							obj.nowSelectStage = stage;

							//ウィンドウの情報を更新する
							obj.UpdateWindow();
							SoundManager.Instance.PlaySE("UI_Cursor");
						}

						_actionSkip = obj.scrollSpeed;
						break;
					}
				}
			}
			return this;
		}
	}
}