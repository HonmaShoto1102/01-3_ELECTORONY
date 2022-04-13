using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Containers.Managers;
using Client.Game.Controllers;
using Client.Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class GameClear : State<GameScene>
	{
		private const float FADE_SPEED = 0.02f;
		private const float SLIDER_SPEED = 2.0f;
		private const int STAY_COUNT = 50;

		//ゲームクリア演出の遷移
		private enum Step
		{
			FadeInStageClear,  //ステージクリア画像のフェードイン
			StayStageClear,    //待機
			FadeOutStageClear, //ステージクリア画像のフェードアウト
			ShowDialog,        //ダイアログの表示
			StayDialog,        //ダイアログの表示待ち
			SliderAnimation,   //スライダーの演出
			StarAnimation,     //★の演出
			ExtraOpen,
			Idle,              //待機
		}

		private Step _step;

		private int frameCount;
		private int effectFrameCount;

		private int starCount;

		private List<List<int>> _stageEffectList = new List<List<int>>();

		private bool _openExtra = false;

		//このステートに入った時に呼ばれる
		public override void Enter(GameScene obj)
		{
			for (int i = 0; i < MobiusController.Instance.GetMobiusCount(); i++) {
				_stageEffectList.Add(new List<int>());
			}

			_step = Step.FadeInStageClear;

			if (obj.stageID == 5 && UserSaveData.Instance.GetStar(obj.stageID) == 0) {
				_openExtra = true;
				obj.gameCanvasContainer.extraOpenText.text = "「EX-1」";
			} else if (obj.stageID == 10 && UserSaveData.Instance.GetStar(obj.stageID) == 0) {
				_openExtra = true;
				obj.gameCanvasContainer.extraOpenText.text = "「EX-2」";
			} else if (obj.stageID == 15 && UserSaveData.Instance.GetStar(obj.stageID) == 0) {
				_openExtra = true;
				obj.gameCanvasContainer.extraOpenText.text = "「EX-3」";
			} else if (obj.stageID == 20 && UserSaveData.Instance.GetStar(obj.stageID) == 0) {
				_openExtra = true;
				obj.gameCanvasContainer.extraOpenText.text = "「EX-4」";
			} else if (obj.stageID == 25 && UserSaveData.Instance.GetStar(obj.stageID) == 0) {
				_openExtra = true;
				obj.gameCanvasContainer.extraOpenText.text = "「EX-5」";
			}

			//先にセーブデータに保存を行う
			if (obj.clearScore >= 100) {
				starCount = 3;
			} else if (obj.clearScore >= 75) {
				starCount = 2;
			} else {
				starCount = 1;
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

			obj.gameCanvasContainer.stageClear.gameObject.SetActive(true);

			SoundManager.Instance.StopBGM();
			SoundManager.Instance.PlaySE("StageClear");

			effectFrameCount = 0;

			obj.clearParticleSystem.Play();

			obj.gameCanvasContainer.SetClearMask2D();

			for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
				ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_Stop");
			}
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(GameScene obj)
		{

		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<GameScene> Update(GameScene obj)
		{
			obj.gameCanvasContainer.menu_ResultMenu.OnUpdate();
			obj.gameCanvasContainer.menu_ExtraOpen.OnUpdate();
			bool isChange = false;
			if (effectFrameCount % 8 == 0) {
				isChange = true;

				for (int i = 0; i < MobiusController.Instance.GetMobiusCount(); i++) {
					MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(i + 1);
					List<int> tempList = new List<int>(_stageEffectList[i]);
					foreach (int z in tempList) {
						if (z >= mobius.mobiusDataList.Count / 2) {
							_stageEffectList[i].Remove(z);
							if (z + 1 <= mobius.mobiusDataList.Count) {
								_stageEffectList[i].Add(z + 1);
							}
						} else {
							_stageEffectList[i].Remove(z);
							if (z - 1 >= 0) {
								_stageEffectList[i].Add(z - 1);
							}
						}
					}
				}
			}

			if (effectFrameCount == 0 || effectFrameCount % 26 == 0) {
				for (int i = 0; i < MobiusController.Instance.GetMobiusCount(); i++) {
					MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(i + 1);
					int id1 = (mobius.mobiusDataList.Count / 2) - 1;
					int id2 = id1 + 1;
					_stageEffectList[i].Add(id1);
					_stageEffectList[i].Add(id2);
				}
				isChange = true;
			}

			if (isChange) {
				for (int i = 0; i < MobiusController.Instance.GetMobiusCount(); i++) {
					MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(i + 1);
					foreach (MobiusData data in mobius.mobiusDataList) {
						if (_stageEffectList[i].Contains(data.pathID)) {
							data.mobiusType = MobiusData.MobiusType.NormalType;
						} else {
							data.mobiusType = MobiusData.MobiusType.HardType;
						}
						data.isChange = true;
					}

					mobius.OnUpdate();
				}
			}

			effectFrameCount++;

			Color color;
			switch (_step) {
				case Step.FadeInStageClear:
					//フェードインをする
					color = obj.gameCanvasContainer.stageClear.color;
					color.a += FADE_SPEED;

					//表示しきったら待機へ
					if (color.a >= 1.0f) {
						_step = Step.StayStageClear;
						frameCount = 0;

						//現在のスコアがセーブよりも高ければ保存する
						if (UserSaveData.Instance.GetStar(obj.stageID) < starCount) {
							UserSaveData.Instance.SetStar(obj.stageID, starCount);
						}
					}
					obj.gameCanvasContainer.stageClear.color = color;
					break;
				case Step.StayStageClear:
					frameCount++;
					//指定フレーム待機してフェードアウトへ
					if (frameCount > STAY_COUNT) {
						_step = Step.FadeOutStageClear;
					}
					break;
				case Step.FadeOutStageClear:
					color = obj.gameCanvasContainer.stageClear.color;
					//フェードアウトをする
					color.a -= FADE_SPEED;

					//フェードアウトしきったらダイアログを表示する
					if (color.a <= 0.0f) {
						_step = Step.ShowDialog;
						obj.gameCanvasContainer.stageClear.gameObject.SetActive(false);
						SoundManager.Instance.PlayBGM("GameClearTheme");
					}
					obj.gameCanvasContainer.stageClear.color = color;
					break;
				case Step.ShowDialog:
					obj.CameraBlurActive();
					_step = Step.StayDialog;

					//ステージが30以降ならNEXTメニューを消す
					if (obj.stageID >= 30) {
						obj.gameCanvasContainer.menu_ResultMenu.SetSelect(0);
						obj.gameCanvasContainer.menu_ResultMenu.buttonList.RemoveAt(2);

						obj.gameCanvasContainer.nextText.gameObject.SetActive(false);
						obj.gameCanvasContainer.nextNoneText.gameObject.SetActive(true);
					} else {
						//項目をNEXTに合わせる
						obj.gameCanvasContainer.menu_ResultMenu.SetSelect(2);
					}


					//メニューダイアログを表示
					obj.gameCanvasContainer.dialog_ResultMenu.gameObject.SetActive(true);

					obj.gameCanvasContainer.dialog_ResultMenu.FadeOut(0.7f, () =>
					{
						//ダイアログが表示出来たら次の演出に移行
						_step = Step.SliderAnimation;
						if (!_openExtra) {
							obj.gameCanvasContainer.menu_ResultMenu.menuActive = true;
						}
					});
					break;
				case Step.StayDialog:
					break;
				case Step.SliderAnimation:
					obj.gameCanvasContainer.clearSlider.value = Mathf.Lerp(obj.gameCanvasContainer.clearSlider.value, obj.clearScore + 1, Time.deltaTime * SLIDER_SPEED);
					obj.gameCanvasContainer.scoreText.text = (int)obj.gameCanvasContainer.clearSlider.value + "%";
					obj.gameCanvasContainer.SetClearMask2D();
					if ((int)obj.gameCanvasContainer.clearSlider.value == obj.clearScore) {
						_step = Step.StarAnimation;
						frameCount = 0;
					}
					break;
				case Step.StarAnimation:
					frameCount++;
					if (frameCount == 10) {
						obj.gameCanvasContainer.star_1_Image.gameObject.SetActive(true);
						SoundManager.Instance.PlaySE("Star_1");
						obj.gameCanvasContainer.particle_1.Play();
						if (starCount == 1) {
							if (_openExtra) {
								_step = Step.ExtraOpen;
							} else {
								_step = Step.Idle;
							}
						}
					}

					if (frameCount == 27) {
						obj.gameCanvasContainer.star_2_Image.gameObject.SetActive(true);
						SoundManager.Instance.PlaySE("Star_2");
						obj.gameCanvasContainer.particle_2.Play();
						if (starCount == 2) {
							if (_openExtra) {
								_step = Step.ExtraOpen;
							} else {
								_step = Step.Idle;
							}
						}
					}

					if (frameCount == 44) {
						obj.gameCanvasContainer.star_3_Image.gameObject.SetActive(true);
						SoundManager.Instance.PlaySE("Star_3");
						obj.gameCanvasContainer.particle_3.Play();
						if (_openExtra) {
							_step = Step.ExtraOpen;
						} else {
							_step = Step.Idle;
						}
					}
					break;
				case Step.ExtraOpen:
					frameCount++;
					if (frameCount >= 50) {
						obj.OpenExtraDialog();
						_step = Step.Idle;
					}
					break;
				case Step.Idle:
					break;
			}
			return this;
		}
	}
}