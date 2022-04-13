using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Game.Scenes
{
	//待機中のステート
	public class StageSelectIdle : State<StageSelectScene>
	{
		private int _actionSkip = 0;

		//このステートに入った時に呼ばれる
		public override void Enter(StageSelectScene obj)
		{
			//初期化をする

		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(StageSelectScene obj)
		{
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<StageSelectScene> Update(StageSelectScene obj)
		{

			if (obj.buttonEnd) return this;

			//★デバッグ用 Xボタンを押すと全クリ状態にする
			//if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.X_Button)) { //Xボタンが押された //Xボタンが押された
			//	UserSaveData.Instance.Debug_SetAllClear();
			//}


			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Option)) { //オプションボタンが押された
				SoundManager.Instance.PlaySE("TitleStart");
				return new StageSelectPauseMenu();
			}

			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
				GlobalSettingController.Instance.SetStageID(obj.nowSelectStage.stageID, false);
				obj.buttonEnd = true;
				SoundManager.Instance.PlaySE("UI_Enter");
				obj.fade.FadeIn(1, () =>
				{
					SceneManager.LoadScene("LoadingScene");
				});
			}

			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Y_Button)) {
				obj.buttonEnd = true;
				GlobalSettingController.Instance.prevSelectStageID = obj.nowSelectStage.stageID;
				SoundManager.Instance.PlaySE("UI_Enter");
				obj.fade.FadeIn(1, () =>
				{
					SceneManager.LoadScene("EXStageSelectScene");
				});
			}

			if (Input.GetButtonDown("L_Button")) { //LBが押された
				if (obj.nowAreaID > 0) { //これ以上左にスクロールできるか調べる
					obj.addAreaScroll = false;
					foreach (var container in ParticleController.Instance.GetParticleAll(obj.nowAreaID)) {
						container.AllHide();
					}
					obj.nowAreaID--;
					obj.LBRBIconUpdate();

					//スクロールした画面の一番最後のステージを選ぶ
					StageSelectContainer stage = StageSelectController.Instance.GetStageSelect((obj.nowAreaID + 1) * 5);
					if (stage != null) {
						obj.nowSelectStage.SetStageSelect(false);
						stage.SetStageSelect(true);
						obj.nowSelectStage = stage;

						//ウィンドウの情報を更新する
						obj.UpdateWindow();
						SoundManager.Instance.PlaySE("UI_Cursor");
					}

					return new StageSelectScroll();
				}
			}

			if (Input.GetButtonDown("R_Button")) { //RBが押された
				//現在のシーンの5ステージめをクリアしてないならRBは無効化
				if (UserSaveData.Instance.GetStar((obj.nowAreaID + 1) * 5) > 0) {
					if (obj.nowAreaID < obj.areaList.Count - 1) { //これ以上右にスクロールできるか調べる
						obj.addAreaScroll = true;
						foreach (var container in ParticleController.Instance.GetParticleAll(obj.nowAreaID)) {
							container.AllHide();
						}
						obj.nowAreaID++;
						obj.LBRBIconUpdate();

						//スクロールした画面の一番最初のステージを選ぶ
						StageSelectContainer stage = StageSelectController.Instance.GetStageSelect((obj.nowAreaID * 5) + 1);
						if (stage != null) {
							obj.nowSelectStage.SetStageSelect(false);
							stage.SetStageSelect(true);
							obj.nowSelectStage = stage;

							//ウィンドウの情報を更新する
							obj.UpdateWindow();
							SoundManager.Instance.PlaySE("UI_Cursor");
						}

						return new StageSelectScroll();
					}
				}
			}

			if (_actionSkip > 0) {
				_actionSkip--;
			}


			if (_actionSkip > 0) return this;

			bool scroll = false;

			if (obj.nowSelectStage.nextButtonList.Count > 0) {
				for (int i = 0; i < obj.nowSelectStage.nextButtonList.Count; i++) {
					if (GlobalButtonInput.Instance.isButtonDown(obj.nowSelectStage.nextButtonList[i])) {

						//このステージをクリアしていないならそれ以上先には行けない
						if (UserSaveData.Instance.GetStar(obj.nowSelectStage.stageID) <= 0) return this;

						//画面をスクロールするか調べる
						if (obj.nowSelectStage.nextScroll) {
							if (obj.nowAreaID < obj.areaList.Count - 1) { //これ以上右にスクロールできるか調べる
								obj.addAreaScroll = true;
								foreach (var container in ParticleController.Instance.GetParticleAll(obj.nowAreaID)) {
									container.AllHide();
								}
								obj.nowAreaID++;
								obj.LBRBIconUpdate();
								scroll = true;
							}
						}


						//次のステージを選ぶ
						StageSelectContainer stage = StageSelectController.Instance.GetStageSelect(obj.nowSelectStage.stageID + 1);
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
						//画面をスクロールするか調べる
						if (obj.nowSelectStage.prevScroll) {
							if (obj.nowAreaID > 0) { //これ以上左にスクロールできるか調べる
								obj.addAreaScroll = false;
								foreach (var container in ParticleController.Instance.GetParticleAll(obj.nowAreaID)) {
									container.AllHide();
								}
								obj.nowAreaID--;
								obj.LBRBIconUpdate();
								scroll = true;
							}
						}

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

			if (scroll) {
				return new StageSelectScroll();
			}
			return this;
		}
	}
}