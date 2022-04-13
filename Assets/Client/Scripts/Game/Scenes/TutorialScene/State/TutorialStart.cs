using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class TutorialStart : State<TutorialScene>
	{
		private const int STAY_COUNT = 70;

		//ゲームオーバー演出の遷移
		private enum Step
		{
			Screen_FadeOut,
			FadeInDialog,  //画像のフェードイン
			Stay,          //待機
			Stay_Dialog,          //待機
			FadeOutDialog, //画像のフェードアウト
			End,
		}

		private Step _step;
		private int frameCount;

		//このステートに入った時に呼ばれる
		public override void Enter(TutorialScene obj)
		{
			_step = Step.Screen_FadeOut;

			obj.userController.stageStartText.text = "- Tutorial -";

			//オブジェクトの更新
			for (int i = 0; i < ObjectController.Instance.GetObjectList().Count; i++) {
				ObjectController.Instance.GetObjectList()[i].OnUpdate();
			}

			//邪魔なUIを消す
			obj.gameCanvasContainer.slider.gameObject.SetActive(false);
			obj.gameCanvasContainer.sliderImage.gameObject.SetActive(false);
			obj.gameCanvasContainer.sliderValue.gameObject.SetActive(false);
			//obj.gameCanvasContainer.menuImage.gameObject.SetActive(false);
			obj.gameCanvasContainer.totalScore.gameObject.SetActive(false);
			obj.gameCanvasContainer.remainImage.gameObject.SetActive(false);
			obj.gameCanvasContainer.remainText.gameObject.SetActive(false);
			obj.gameCanvasContainer.howToPlay.SetActive(false);

			if (!GlobalSettingController.Instance.tutorialPlayable) {
				obj.userController.stageStartText2.gameObject.SetActive(false);
			}

			if (obj.mobiusManager.GetAllMobiusCount() == 3) {
				Camera.main.transform.position = obj.cameraPosContainer.mobius_3_Pos.position;
			} else if (obj.mobiusManager.GetAllMobiusCount() == 2) {
				Camera.main.transform.position = obj.cameraPosContainer.mobius_2_Pos.position;
			} else if (obj.mobiusManager.GetAllMobiusCount() == 1) {
				Camera.main.transform.position = obj.cameraPosContainer.mobius_1_Pos.position;
			}

			obj.gameCanvasContainer.SetMask2D();

			//エネミーの更新
			for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
				ObjectController.Instance.enemyList[i].SetRealScore();
			}
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(TutorialScene obj)
		{

		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<TutorialScene> Update(TutorialScene obj)
		{
			switch (_step) {
				case Step.Screen_FadeOut:
					//画面全体のフェードアウト
					//obj.screenFade.SetRange(1);

					_step = Step.Stay_Dialog;
					obj.screenFade.FadeOut(0.7f, () =>
					{
						_step = Step.FadeInDialog;
					});
					break;
				case Step.FadeInDialog:
					//フェードインをする
					obj.userController.stageStart.gameObject.SetActive(true);
					_step = Step.Stay_Dialog;
					obj.userController.stageStart.SetRange(1);
					obj.userController.stageStart.FadeOut(0.5f, () =>
					{
						_step = Step.Stay;
						frameCount = 0;
					});
					break;
				case Step.Stay:
					frameCount++;
					//指定フレーム待機してフェードアウトへ
					if (frameCount > STAY_COUNT) {
						_step = Step.FadeOutDialog;
					}
					break;
				case Step.FadeOutDialog:
					//フェードインをする
					_step = Step.Stay_Dialog;
					obj.userController.stageStart.FadeIn(0.8f, () =>
					{
						obj.userController.stageStart.gameObject.SetActive(false);
						_step = Step.End;
					});
					break;
				case Step.Stay_Dialog:
					break;
				case Step.End:
					//UIを戻す
					//obj.gameCanvasContainer.menuImage.gameObject.SetActive(true);
					obj.gameCanvasContainer.sliderImage.gameObject.SetActive(true);
					obj.gameCanvasContainer.sliderValue.gameObject.SetActive(true);
					obj.gameCanvasContainer.totalScore.gameObject.SetActive(true);
					obj.gameCanvasContainer.remainImage.gameObject.SetActive(true);
					obj.gameCanvasContainer.remainText.gameObject.SetActive(true);
					obj.gameCanvasContainer.howToPlay.SetActive(true);
					return new TutorialIdle();
			}
			return this;
		}
	}
}