using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using Client.Game.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class TutorialIdle : State<TutorialScene>
	{
		//敵全員のMaxScoreの合計
		private int _totalMax = 0;

		private enum DialogStep
		{
			Idle,
			Show,
			Open,
			Close,
		}

		//チュートリアルの演出遷移
		private enum Step
		{
			Message_1, //Electronyの世界へようこそ!
			Message_2, //Electronyはナノマシーンの進路を操作して、エネルギーを集めるパズルゲームです
			Message_3, //ナノマシーンは円状のステージを自動で走り続けます
			Wait_1,    //ナノマシーンをねじれる輪の前まで走らせる
			Message_4, //プレイヤーはステージ上の青色の部分をねじり、ナノマシーンの進路を変更することが出来ます
			Message_5, //左スティックとAボタンを使って矢印が表示されている部分をねじってみましょう
			Controll_1,//プレイヤーにねじらせる
			Wait_2,    //ナノマシーンが内側に入るまで待つ
			Message_6, //進路を変更したことにより、ナノマシーンを円の内側に誘導することが出来ました
			Message_7, //ねじった部分は一定時間が経過すると自動で元に戻ります
			Message_8, //一度にねじれるのは1ヵ所のみで、ねじった部分が戻るまで待つ必要があります
			Message_9, //また、Aボタンを長押ししてから離すと通常よりも長い間、ねじる事が出来ます
			Message_10, //円の一番内側に入ったナノマシーンは自身のエネルギーをステージに供給し始めます
			Message_11, //供給中はナノマシーンのエネルギーは減り続け、それ以外の時は徐々に回復していきます
			Wait_3,    //エネルギーの増減を見せる
			Message_12, //画面右側のゲージはエネルギー供給中のナノマシーンのエネルギー残量の合計値の割合です
			Wait_4,    //一旦UIを消してゲージに矢印をつけて見せる
			Message_13,//ステージ内の全てのナノマシーンを円の一番内側に誘導した時に、エネルギー残量の合計が45%を超えていたらステージクリアです
			Message_14,//エネルギー残量の合計が高いほど、良い評価が貰えます
			Message_15,//全てのナノマシーンを誘導する前に、ナノマシーンのエネルギーが0%になってしまうとゲームオーバーです
			Wait_5,    //ナノマシーンのエネルギーが0%になるのを見せる
			CameraZoom_1,//死んだナノマシーンにカメラズームする
			Message_16, //エネルギーが0%になる前にクリアするか、一旦ナノマシーンを外側に誘導して、エネルギー切れを回避しましょう
			Message_17,//それでは実際にナノマシーンを誘導して、ステージをクリアしてみましょう!
			Setup_1,   //ステージをリセットして実際に遊ばせる準備をする
			Game_1,    //実際に遊ばせる
			GameOver_1,//失敗演出
			GameOver_2,//ナノマシーンのエネルギーが0%になってしまいました もう一度チャレンジしてみましょう!
			StageClear_Effect,//クリア演出
			StageClear_1,//ステージクリアです! おめでとうございます
			StageClear_2,//以上でチュートリアルは終了になります
			StageClear_3,//このチュートリアルはステージセレクトのメニューからいつでもやり直すことが出来ます
			StageClear_4,//それではElectronyの世界をお楽しみください!
			Wait,
		}

		private Step _step;

		private DialogStep _dialogStep;

		private int frameCount;

		private const float FADE_SPEED = 0.06f;

		private float _fadeCount = 0.0f;
		private bool _fadeIn = true;

		private const float BUTTON_FADE_SPEED = 0.03f;
		private float _a_Button_fadeCount = 0.0f;
		private bool _a_Button_fadeIn = true;

		private const float MinMoveLength = 0.01f;
		private const float MoveSpeed = 2.0f;

		private Camera mainCamera;

		private List<List<int>> _stageEffectList = new List<List<int>>();
		private float _effectFrameCount;

		//このステートに入った時に呼ばれる
		public override void Enter(TutorialScene obj)
		{
			//敵全員のMaxScoreの合計を求める
			foreach (EnemyContainer enemy in ObjectController.Instance.enemyList) {
				_totalMax += enemy.maxScore;
			}

			_dialogStep = DialogStep.Idle;

			if (GlobalSettingController.Instance.tutorialPlayable) {
				_step = Step.Game_1;
				for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
					ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_End");
				}
			} else {
				_step = Step.Message_1;
				obj.messageBG.gameObject.SetActive(true);
				obj.messageText.gameObject.SetActive(true);
				obj.message_AButton.gameObject.SetActive(true);

				obj.userController.tutorialMode = true;
			}

			mainCamera = Camera.main;

			_effectFrameCount = 0;
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(TutorialScene obj)
		{
		}

		private void _StageClearEffect(TutorialScene obj)
		{
			obj.gameCanvasContainer.menu_ResultMenu.OnUpdate();
			bool isChange = false;
			if (_effectFrameCount % 8 == 0) {
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

			if (_effectFrameCount == 0 || _effectFrameCount % 26 == 0) {
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

			_effectFrameCount++;
		}

		private void _TextFade(TutorialScene obj, string newMessage)
		{
			if (!_fadeIn && _fadeCount == 1.0f) return;
			if (_fadeIn) {
				_fadeCount -= FADE_SPEED;
				if (_fadeCount < 0.0f) {
					_fadeCount = 0.0f;
					_fadeIn = false;
					obj.messageText.text = newMessage;
				}
			} else {
				_fadeCount += FADE_SPEED;
				if (_fadeCount > 1.0f) {
					_fadeCount = 1.0f;
				}
			}

			Color c = obj.messageText.color;
			c.a = _fadeCount;
			obj.messageText.color = c;
			obj.gradationContainer.SetAlpha(_fadeCount);
		}

		private void _A_Button_Fade(TutorialScene obj)
		{
			if (_a_Button_fadeIn) {
				_a_Button_fadeCount -= BUTTON_FADE_SPEED;
				if (_a_Button_fadeCount < 0.0f) {
					_a_Button_fadeCount = 0.0f;
					_a_Button_fadeIn = false;
				}
			} else {
				_a_Button_fadeCount += BUTTON_FADE_SPEED;
				if (_a_Button_fadeCount > 1.0f) {
					_a_Button_fadeCount = 1.0f;
					_a_Button_fadeIn = true;
				}
			}

			Color c = obj.message_AButton.color;
			c.a = _a_Button_fadeCount;
			obj.message_AButton.color = c;
		}

		private void _ArrowFade(TutorialScene obj)
		{
			if (_a_Button_fadeIn) {
				_a_Button_fadeCount -= BUTTON_FADE_SPEED;
				if (_a_Button_fadeCount < 0.0f) {
					_a_Button_fadeCount = 0.0f;
					_a_Button_fadeIn = false;
				}
			} else {
				_a_Button_fadeCount += BUTTON_FADE_SPEED;
				if (_a_Button_fadeCount > 1.0f) {
					_a_Button_fadeCount = 1.0f;
					_a_Button_fadeIn = true;
				}
			}

			Color c = obj.arrow.color;
			c.a = _a_Button_fadeCount;
			obj.arrow.color = c;
		}

		private void _ArrowFade2(TutorialScene obj)
		{
			if (_a_Button_fadeIn) {
				_a_Button_fadeCount -= FADE_SPEED;
				if (_a_Button_fadeCount < 0.0f) {
					_a_Button_fadeCount = 0.0f;
					_a_Button_fadeIn = false;
				}
			} else {
				_a_Button_fadeCount += FADE_SPEED;
				if (_a_Button_fadeCount > 1.0f) {
					_a_Button_fadeCount = 1.0f;
					_a_Button_fadeIn = true;
				}
			}

			Color c = obj.arrow2.color;
			c.a = _a_Button_fadeCount;
			obj.arrow2.color = c;
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<TutorialScene> Update(TutorialScene obj)
		{
			switch (_dialogStep) {
				case DialogStep.Idle:
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Option)) {
						_dialogStep = DialogStep.Open;
						obj.dialog_TutorialEnd.SetRange(1.0f);
						obj.optionEnd = false;

						//メニューダイアログを表示
						obj.dialog_TutorialEnd.gameObject.SetActive(true);

						obj.dialog_TutorialEnd.FadeOut(0.6f, () =>
						{
							obj.menu_TutorialEnd.menuActive = true;
							_dialogStep = DialogStep.Show;

							//コントローラーの振動を止める
							GamePad.SetVibration(0, 0, 0);
						});

						return this;
					}
					break;
				case DialogStep.Open:
					return this;
				case DialogStep.Show:
					obj.menu_TutorialEnd.OnUpdate();
					if (obj.optionEnd) {
						_dialogStep = DialogStep.Idle;
					}

					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Option)) {
						_dialogStep = DialogStep.Close;
						SoundManager.Instance.PlaySE("UI_Cancel");

						obj.dialog_TutorialEnd.SetRange(0.5f);
						//メニューダイアログを消す
						obj.menu_TutorialEnd.menuActive = false;
						obj.dialog_TutorialEnd.FadeIn(0.6f, () =>
						{
							obj.dialog_TutorialEnd.gameObject.SetActive(false);
							_dialogStep = DialogStep.Idle;
						});
					}
					return this;
				case DialogStep.Close:
					return this;
			}


			//テキストの更新
			int count = 0;
			int totalScore = 0;

			bool isDenger = false;

			//ゴール条件に居るエネミーを探す
			for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
				EnemyContainer enemy = ObjectController.Instance.enemyList[i];
				totalScore += enemy.enemyScore;

				if (enemy.objectContainer.nowMobiusData == null) continue;

				MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(enemy.objectContainer.nowMobiusID);
				//ゴール条件に居る敵ならスコアをカウントする
				if (mobius.mobiusID == 1 && enemy.objectContainer.isInside) {
					if (!enemy.isDead) {
						count++;
						if (enemy.enemyScore < 3) {
							isDenger = true;
						}
					}
				}
			}

			obj.ppsContainer.SetColor(isDenger);

			obj.gameCanvasContainer.remainText.text = count + " / " + ObjectController.Instance.enemyList.Count;

			float percent = 0.0f;
			if (totalScore > 0) {
				percent = (float)totalScore / _totalMax * 100.0f;
			}

			obj.gameCanvasContainer.slider.value = Mathf.MoveTowards(obj.gameCanvasContainer.slider.value, percent, 0.4f);
			obj.gameCanvasContainer.SetMask2D();
			obj.gameCanvasContainer.totalScore.text = (int)obj.gameCanvasContainer.slider.value + "%";

			obj.gameCanvasContainer.UpdateTotalScore(obj.gameCanvasContainer.slider.value);

			switch (_step) {
				case Step.Message_1:
					_TextFade(obj, "Electronyの世界へようこそ!");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_2;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.Message_2:
					_TextFade(obj, "Electronyはナノマシーンの進路を操作して、\nエネルギーを集めるパズルゲームです");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_3;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.Message_3:
					_TextFade(obj, "ナノマシーンは輪状のステージを自動で走り続けます");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Wait_1;
						SoundManager.Instance.PlaySE("UI_Enter");
						frameCount = 0;
						obj.messageBG.gameObject.SetActive(false);
						obj.messageText.gameObject.SetActive(false);
						obj.message_AButton.gameObject.SetActive(false);

						for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
							ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_End");
						}
					}
					break;
				case Step.Wait_1:
					//オブジェクトの更新
					for (int i = 0; i < ObjectController.Instance.GetObjectList().Count; i++) {
						ObjectController.Instance.GetObjectList()[i].OnUpdate();
					}

					//エネミーの更新
					for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
						ObjectController.Instance.enemyList[i].OnUpdate();
					}

					frameCount++;
					if (frameCount > 160) {
						_fadeIn = true;
						_fadeCount = 0.0f;
						obj.messageText.text = "";
						_step = Step.Message_4;
						obj.messageBG.gameObject.SetActive(true);
						obj.messageText.gameObject.SetActive(true);
						obj.message_AButton.gameObject.SetActive(true);

						for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
							ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_Stop");
						}
					}
					break;
				case Step.Message_4:
					_TextFade(obj, "プレイヤーはステージ上の青色の部分をねじり、\nナノマシーンの進路を変更することが出来ます");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_5;
						SoundManager.Instance.PlaySE("UI_Enter");
						obj.arrow.gameObject.SetActive(true);
					}
					break;
				case Step.Message_5:
					_TextFade(obj, "左スティックとAボタンを使って\n矢印が表示されている部分をねじってみましょう");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Controll_1;
						SoundManager.Instance.PlaySE("UI_Enter");
						obj.messageBG.gameObject.SetActive(false);
						obj.messageText.gameObject.SetActive(false);
						obj.message_AButton.gameObject.SetActive(false);
					}
					break;
				case Step.Controll_1:
					_ArrowFade(obj);
					obj.mobiusManager.OnUpdate();
					obj.userController.OnUpdate();
					if (!obj.userController.tutorialMode) {
						_step = Step.Wait_2;
						frameCount = 0;
						obj.arrow.gameObject.SetActive(false);

						for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
							ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_End");
						}
					}
					break;
				case Step.Wait_2:
					frameCount++;
					obj.mobiusManager.OnUpdate();

					//オブジェクトの更新
					for (int i = 0; i < ObjectController.Instance.GetObjectList().Count; i++) {
						ObjectController.Instance.GetObjectList()[i].OnUpdate();
					}

					//エネミーの更新
					for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
						ObjectController.Instance.enemyList[i].OnUpdate();
					}

					if (frameCount > 80) {
						_fadeIn = true;
						_fadeCount = 0.0f;
						_step = Step.Message_6;
						obj.messageText.text = "";
						obj.messageBG.gameObject.SetActive(true);
						obj.messageText.gameObject.SetActive(true);
						obj.message_AButton.gameObject.SetActive(true);

						for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
							ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_Stop");
						}
					}
					break;
				case Step.Message_6:
					_TextFade(obj, "進路を変更したことにより、\nナノマシーンを輪の内側に誘導することが出来ました");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_9;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				//case Step.Message_7:
				//	_TextFade(obj, "ねじった部分は一定時間が経過すると自動で元に戻ります");
				//	_A_Button_Fade(obj);
				//	if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
				//		_fadeIn = true;
				//		_step = Step.Message_8;
				//		SoundManager.Instance.PlaySE("UI_Enter");
				//	}
				//	break;
				//case Step.Message_8:
				//	_TextFade(obj, "一度にねじれるのは1ヵ所のみで、操作した後は\nねじれた部分が戻るまで待つ必要があります");
				//	_A_Button_Fade(obj);
				//	if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
				//		_fadeIn = true;
				//		_step = Step.Message_9;
				//		SoundManager.Instance.PlaySE("UI_Enter");
				//	}
				//	break;
				case Step.Message_9:
					_TextFade(obj, "Aボタンを長押ししてから離すと\n通常よりも長い時間ねじる事が出来ます");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_10;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.Message_10:
					_TextFade(obj, "輪の一番内側に入ったナノマシーンは\n自身のエネルギーを中心ある「コア」に供給し始めます");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_11;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.Message_11:
					_TextFade(obj, "供給中はナノマシーンのエネルギーは減り続け、\nそれ以外の時は徐々に回復していきます");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Wait_3;
						SoundManager.Instance.PlaySE("UI_Enter");
						frameCount = 0;
						obj.messageBG.gameObject.SetActive(false);
						obj.messageText.gameObject.SetActive(false);
						obj.message_AButton.gameObject.SetActive(false);

						for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
							ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_End");
						}
					}
					break;
				case Step.Wait_3:
					//オブジェクトの更新
					for (int i = 0; i < ObjectController.Instance.GetObjectList().Count; i++) {
						ObjectController.Instance.GetObjectList()[i].OnUpdate();
					}

					//エネミーの更新
					for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
						ObjectController.Instance.enemyList[i].OnUpdate();
					}

					frameCount++;
					if (frameCount > 300) {
						_fadeIn = true;
						_fadeCount = 0.0f;
						_step = Step.Message_12;
						obj.messageText.text = "";
						obj.messageBG.gameObject.SetActive(true);
						obj.messageText.gameObject.SetActive(true);
						obj.message_AButton.gameObject.SetActive(true);
						obj.arrow2.gameObject.SetActive(true);

						for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
							ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_Stop");
						}
					}
					break;
				case Step.Message_12:
					_TextFade(obj, "画面右側のゲージは全てのナノマシーンの\nエネルギー残量の合計値の割合です");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Wait_4;
						SoundManager.Instance.PlaySE("UI_Enter");
						frameCount = 0;
						obj.messageBG.gameObject.SetActive(false);
						obj.messageText.gameObject.SetActive(false);
						obj.message_AButton.gameObject.SetActive(false);
					}
					break;
				case Step.Wait_4:
					_ArrowFade2(obj);
					frameCount++;

					if (frameCount > 150) {
						_fadeIn = true;
						_fadeCount = 0.0f;
						_step = Step.Message_13;
						obj.messageText.text = "";
						obj.messageBG.gameObject.SetActive(true);
						obj.messageText.gameObject.SetActive(true);
						obj.message_AButton.gameObject.SetActive(true);
						obj.arrow2.gameObject.SetActive(false);
					}
					break;
				case Step.Message_13:
					_TextFade(obj, "ステージ内の全てのナノマシーンを一番内側に誘導した時に、\n合計値が45%以上をキープしていればステージクリアです");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_15;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				//case Step.Message_14:
				//	_TextFade(obj, "エネルギー残量の合計が高いほど、良い評価が貰えます");
				//	_A_Button_Fade(obj);
				//	if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
				//		_fadeIn = true;
				//		_step = Step.Message_15;
				//		SoundManager.Instance.PlaySE("UI_Enter");
				//	}
				//	break;
				case Step.Message_15:
					_TextFade(obj, "全てのナノマシーンを誘導する前に、\nエネルギーが0%になってしまうとゲームオーバーです");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Wait_5;
						SoundManager.Instance.PlaySE("UI_Enter");
						frameCount = 0;
						_fadeCount = 0.0f;
						obj.messageBG.gameObject.SetActive(false);
						obj.messageText.gameObject.SetActive(false);
						obj.message_AButton.gameObject.SetActive(false);

						for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
							ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_End");
						}
					}
					break;
				case Step.Wait_5:
					//オブジェクトの更新
					for (int i = 0; i < ObjectController.Instance.GetObjectList().Count; i++) {
						ObjectController.Instance.GetObjectList()[i].OnUpdate();
					}

					//エネミーの更新
					for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
						EnemyContainer enemy = ObjectController.Instance.enemyList[i];
						enemy.OnUpdate();

						if (enemy.isDead) {
							obj.gameOverZoomPos = enemy.transform.position;
							obj.gameOverZoomPos.z = -1.7f;
							_step = Step.CameraZoom_1;

							for (int z = 0; z < ObjectController.Instance.enemyList.Count; z++) {
								if (ObjectController.Instance.enemyList[z] == enemy) {
									enemy.SetAnimationTrigger("Anim_Die");
									enemy.SetDeath();
								} else {
									ObjectController.Instance.enemyList[z].SetAnimationTrigger("Anim_Stop");
								}
							}
						}
					}
					break;
				case Step.CameraZoom_1:
					Vector3 dirVector = obj.gameOverZoomPos - mainCamera.transform.position;
					if (dirVector.sqrMagnitude >= MinMoveLength * MinMoveLength) {
						mainCamera.transform.position += dirVector * MoveSpeed * Time.deltaTime;
					} else {
						_step = Step.Message_16;
						_fadeIn = true;
						_fadeCount = 0.0f;
						obj.messageText.text = "";
						obj.messageBG.gameObject.SetActive(true);
						obj.messageText.gameObject.SetActive(true);
						obj.message_AButton.gameObject.SetActive(true);
					}
					break;
				case Step.Message_16:
					_TextFade(obj, "エネルギーが0%になる前にクリアするか、一旦ナノマシーン\nを外側に誘導して、エネルギー切れを回避しましょう");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Message_17;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.Message_17:
					_TextFade(obj, "それでは実際にナノマシーンを誘導して、\nステージをクリアしてみましょう!");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Setup_1;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.Setup_1:
					_step = Step.Wait;
					obj.screenFade.FadeIn(1, () =>
					{
						GlobalSettingController.Instance.tutorialPlayable = true;
						SceneManager.LoadScene("TutorialScene");
					});
					break;
				case Step.Game_1:
					//プレイヤー操作の更新
					obj.userController.OnUpdate();

					//ステージの更新
					obj.mobiusManager.OnUpdate();

					//クリア判定チェック(45%以上でクリア)
					if (count == ObjectController.Instance.enemyList.Count) {
						if (percent >= 45) {
							if (!GlobalSettingController.Instance.nowScrew) {
								//クリア判定を出す
								_step = Step.StageClear_Effect;
								for (int i = 0; i < MobiusController.Instance.GetMobiusCount(); i++) {
									_stageEffectList.Add(new List<int>());
								}
								//邪魔なUIを消す
								obj.gameCanvasContainer.slider.gameObject.SetActive(false);
								obj.gameCanvasContainer.sliderImage.gameObject.SetActive(false);
								obj.gameCanvasContainer.sliderValue.gameObject.SetActive(false);
								//obj.gameCanvasContainer.menuImage.gameObject.SetActive(false);
								obj.gameCanvasContainer.totalScore.gameObject.SetActive(false);
								obj.gameCanvasContainer.howToPlay.SetActive(false);

								obj.gameCanvasContainer.stageClear.gameObject.SetActive(true);

								SoundManager.Instance.StopBGM();
								SoundManager.Instance.PlaySE("StageClear");

								_effectFrameCount = 0;
								frameCount = 0;

								obj.clearParticleSystem.Play();

								for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
									ObjectController.Instance.enemyList[i].SetAnimationTrigger("Anim_Stop");
								}

								return this;
							}
						}
					}

					//オブジェクトの更新
					for (int i = 0; i < ObjectController.Instance.GetObjectList().Count; i++) {
						ObjectController.Instance.GetObjectList()[i].OnUpdate();
					}

					//エネミーの更新
					for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
						EnemyContainer enemy = ObjectController.Instance.enemyList[i];
						enemy.OnUpdate();
						if (enemy.isDead) {
							obj.gameOverZoomPos = enemy.transform.position;
							obj.gameOverZoomPos.z = -1.7f;
							_step = Step.GameOver_1;

							for (int z = 0; z < ObjectController.Instance.enemyList.Count; z++) {
								if (ObjectController.Instance.enemyList[z] == enemy) {
									enemy.SetAnimationTrigger("Anim_Die");
									enemy.SetDeath();
								} else {
									ObjectController.Instance.enemyList[z].SetAnimationTrigger("Anim_Stop");
								}
							}
						}
					}
					break;
				case Step.GameOver_1:
					Vector3 dirVector2 = obj.gameOverZoomPos - mainCamera.transform.position;
					if (dirVector2.sqrMagnitude >= MinMoveLength * MinMoveLength) {
						mainCamera.transform.position += dirVector2 * MoveSpeed * Time.deltaTime;
					} else {
						_step = Step.GameOver_2;
						_fadeIn = true;
						_fadeCount = 0.0f;
						obj.messageText.text = "";
						obj.messageBG.gameObject.SetActive(true);
						obj.messageText.gameObject.SetActive(true);
						obj.message_AButton.gameObject.SetActive(true);
					}
					break;
				case Step.GameOver_2:
					_TextFade(obj, "ナノマシーンのエネルギーが0%になってしまいました\nもう一度チャレンジしてみましょう!");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.Setup_1;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.StageClear_Effect:
					_StageClearEffect(obj);

					//フェードインをする
					Color color = obj.gameCanvasContainer.stageClear.color;
					color.a += FADE_SPEED;

					if (color.a >= 1.0f) {
						color.a = 1.0f;
						frameCount++;
					}
					obj.gameCanvasContainer.stageClear.color = color;

					if (frameCount > 60) {
						_step = Step.StageClear_1;
						_fadeIn = true;
						_fadeCount = 0.0f;
						obj.messageText.text = "";
						obj.messageBG.gameObject.SetActive(true);
						obj.messageText.gameObject.SetActive(true);
						obj.message_AButton.gameObject.SetActive(true);

						//既に流れているBGMがあれば止める
						SoundManager.Instance.StopBGM();

						//ゲームBGMを再生
						SoundManager.Instance.PlayBGM("GameClearTheme");
					}
					break;
				case Step.StageClear_1:
					_StageClearEffect(obj);
					_TextFade(obj, "ステージクリアです! おめでとうございます!");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.StageClear_2;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.StageClear_2:
					_StageClearEffect(obj);
					_TextFade(obj, "以上でチュートリアルは終了になります");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.StageClear_3;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.StageClear_3:
					_StageClearEffect(obj);
					_TextFade(obj, "このチュートリアルはステージセレクトのメニューから\nいつでもやり直すことが出来ます");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						_fadeIn = true;
						_step = Step.StageClear_4;
						SoundManager.Instance.PlaySE("UI_Enter");
					}
					break;
				case Step.StageClear_4:
					_StageClearEffect(obj);
					_TextFade(obj, "それではElectronyの世界をお楽しみください!");
					_A_Button_Fade(obj);
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
						SoundManager.Instance.PlaySE("UI_Enter");
						obj.screenFade.FadeIn(1, () =>
						{
							_step = Step.Wait;
							GlobalSettingController.Instance.tutorialPlayable = false;
							SceneManager.LoadScene("StageSelectScene");
						});
					}
					break;
				case Step.Wait:
					break;
			}
			return this;
		}
	}
}