using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;
using XInputDotNetPure;

namespace Client.Game.Scenes
{
	//ゲームの初期化をするステート
	public class GameIdle : State<GameScene>
	{
		//敵全員のMaxScoreの合計
		private int _totalMax = 0;

		//このステートに入った時に呼ばれる
		public override void Enter(GameScene obj)
		{
			//敵全員のMaxScoreの合計を求める
			foreach (EnemyContainer enemy in ObjectController.Instance.enemyList) {
				_totalMax += enemy.maxScore;
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
			//プレイヤー操作の更新
			obj.userController.OnUpdate();

			//ステージの更新
			obj.mobiusManager.OnUpdate();

			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Option)) {
				SoundManager.Instance.PlaySE("TitleStart");
				return new GamePauseMenu();
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
					} else {
						//コントローラーの振動を止める
						GamePad.SetVibration(0, 0, 0);

						obj.gameOverZoomPos = enemy.transform.position;
						obj.gameOverZoomPos.z = -1.7f;

						for (int z = 0; z < ObjectController.Instance.enemyList.Count; z++) {
							if (ObjectController.Instance.enemyList[z] == enemy) {
								enemy.SetAnimationTrigger("Anim_Die");
								enemy.SetDeath();
							} else {
								ObjectController.Instance.enemyList[z].SetAnimationTrigger("Anim_Stop");
							}
						}

						//エネミーが死んでいるのでゲームオーバー
						return new GameOver();
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

			//クリア判定チェック(45%以上でクリア)
			if (count == ObjectController.Instance.enemyList.Count) {
				if (percent >= 45) {
					if (!GlobalSettingController.Instance.nowScrew) {
						//クリア判定を出す
						obj.clearScore = (int)percent;
						return new GameClear();
					}
				}
			}

			//オブジェクトの更新
			for (int i = 0; i < ObjectController.Instance.GetObjectList().Count; i++) {
				ObjectController.Instance.GetObjectList()[i].OnUpdate();
			}

			//エネミーの更新
			for (int i = 0; i < ObjectController.Instance.enemyList.Count; i++) {
				ObjectController.Instance.enemyList[i].OnUpdate();
			}

			return this;
		}
	}
}