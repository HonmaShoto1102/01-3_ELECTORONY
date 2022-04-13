using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class GameOver : State<GameScene>
	{
		private const float FADE_SPEED = 0.02f;
		private const int STAY_COUNT = 50;

		//ゲームオーバー演出の遷移
		private enum Step
		{
			CameraZoom,
			FadeInGameOver,  //ゲームオーバー画像のフェードイン
			StayGameOver,    //待機
			FadeOutGameOver, //ゲームオーバー画像のフェードアウト
			ShowDialog,        //ダイアログの表示
			StayDialog,        //ダイアログの表示待ち
			Idle,              //待機
		}

		private Step _step;
		private int frameCount;

		private const float MinMoveLength = 0.01f;
		private const float MoveSpeed = 2.0f;

		private Camera mainCamera;

		//このステートに入った時に呼ばれる
		public override void Enter(GameScene obj)
		{
			_step = Step.CameraZoom;

			//邪魔なUIを消す
			obj.gameCanvasContainer.slider.gameObject.SetActive(false);
			obj.gameCanvasContainer.sliderImage.gameObject.SetActive(false);
			obj.gameCanvasContainer.sliderValue.gameObject.SetActive(false);
			//obj.gameCanvasContainer.menuImage.gameObject.SetActive(false);
			obj.gameCanvasContainer.totalScore.gameObject.SetActive(false);
			obj.gameCanvasContainer.remainImage.gameObject.SetActive(false);
			obj.gameCanvasContainer.remainText.gameObject.SetActive(false);
			obj.gameCanvasContainer.howToPlay.SetActive(false);

			obj.gameCanvasContainer.gameOver.gameObject.SetActive(true);
			obj.gameCanvasContainer.gameOver2.gameObject.SetActive(true);

			mainCamera = Camera.main;

			SoundManager.Instance.StopBGM();
			SoundManager.Instance.PlaySE("Game_Faild");
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(GameScene obj)
		{

		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<GameScene> Update(GameScene obj)
		{
			obj.gameCanvasContainer.menu_GameOverMenu.OnUpdate();

			Color color;
			switch (_step) {
				case Step.CameraZoom:
					Vector3 dirVector = obj.gameOverZoomPos - mainCamera.transform.position;
					if (dirVector.sqrMagnitude >= MinMoveLength * MinMoveLength) {
						mainCamera.transform.position += dirVector * MoveSpeed * Time.deltaTime;
					} else {
						_step = Step.FadeInGameOver;
					}
					break;

				case Step.FadeInGameOver:
					//フェードインをする
					obj.CameraBlurActive();
					color = obj.gameCanvasContainer.gameOver.color;
					float alpha = color.a + FADE_SPEED;
					color.a = alpha;
					obj.gameCanvasContainer.gameOver.color = color;

					color = obj.gameCanvasContainer.gameOver2.color;
					color.a = alpha;
					obj.gameCanvasContainer.gameOver2.color = color;

					//表示しきったら待機へ
					if (alpha >= 1.0f) {
						_step = Step.ShowDialog;
						frameCount = 0;
					}
					break;
				case Step.StayGameOver:
					frameCount++;
					//指定フレーム待機してフェードアウトへ
					if (frameCount > STAY_COUNT) {
						_step = Step.ShowDialog;
					}
					break;
				case Step.FadeOutGameOver:
					color = obj.gameCanvasContainer.gameOver.color;
					//フェードアウトをする
					color.a -= FADE_SPEED;

					//フェードアウトしきったらダイアログを表示する
					if (color.a <= 0.0f) {
						_step = Step.ShowDialog;
						obj.gameCanvasContainer.gameOver.gameObject.SetActive(false);
					}
					obj.gameCanvasContainer.gameOver.color = color;
					break;
				case Step.ShowDialog:
					_step = Step.StayDialog;

					//項目をNEXTに合わせる
					obj.gameCanvasContainer.menu_GameOverMenu.SetSelect(0);

					//メニューダイアログを表示
					obj.gameCanvasContainer.dialog_GameOverMenu.gameObject.SetActive(true);

					obj.gameCanvasContainer.dialog_GameOverMenu.FadeOut(0.7f, () =>
					{
						//ダイアログが表示出来たら次の演出に移行
						_step = Step.Idle;
						obj.gameCanvasContainer.menu_GameOverMenu.menuActive = true;
					});
					break;
				case Step.StayDialog:
					break;
				case Step.Idle:
					break;
			}
			return this;
		}
	}
}