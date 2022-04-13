using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using Client.Game.Containers.Managers;
using Client.Game.Controllers;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

namespace Client.Game.Containers
{
	//敵にアタッチするコンテナー
	public class EnemyContainer : MonoBehaviour
	{
		[SerializeField, Label("Object Container")] public ObjectContainer objectContainer;
		[SerializeField, Label("Enemy Effect")] public EnemyEffect enemyEffect;
		[SerializeField, Label("Enemy Skin")] public EnemySkin enemySkin;
		[SerializeField, Label("Enemy Ghost")] public BakeMesh bakeMesh;

		[Header("スコア関係")]
		[SerializeField, Label("現在のスコア")] public int enemyScore;
		[SerializeField, Label("最大のスコア")] public int maxScore;
		[SerializeField, Label("Score Canvas")] private Canvas scoreCanvas;
		[SerializeField, Label("Score Text")] private Text scoreText;


		[Header("文字色の設定")]
		[SerializeField, Label("100%")] public Color color_0;
		[SerializeField, Label("80%以上")] public Color color_1;
		[SerializeField, Label("30%以上")] public Color color_2;
		[SerializeField, Label("11%以上")] public Color color_3;
		[SerializeField, Label("10%以下")] public Color color_4;

		private int _scoreFrameCouter = 0;

		private bool _scoreDownGimmick = false;
		private int _scoreDownTarget = 1;

		private float realPercent;
		private float showPercent;

		private GradationContainer gradationContainer;

		//フェードの速度
		private const float FADE_SPEED = 0.05f;

		//現在のアルファ値
		private float _fadeCount = 1.0f;
		private bool _fadeIn = true;

		//ダメージのエフェクト用
		private const int MAX_EFFECT = 10;
		private const int MAX_TIME = 7;
		private bool _effectActive = false;
		private int _effectCount = 0;
		private int _effectTime = 0;
		private bool _nowRed = false;


		[HideInInspector] public bool isDead = false;

		private Animator animator;

		private bool nowWarning;

		public void SetCanvasOrder(int order)
		{
			scoreCanvas.sortingOrder = order;
		}

		public void Initialize()
		{
			animator = GetComponent<Animator>();

			gradationContainer = scoreText.GetComponent<GradationContainer>();
		}

		public void OnUpdate()
		{
			SetSliderDirection();

			//足元の輪の位置が変わったかチェック
			if (objectContainer.isMobiusChange || objectContainer.nowMobiusData.isChange) {
				_ScrewCheck();
				//Debug.Log("Screw Check");

				objectContainer.isMobiusChange = false;
			}

			//スコアの増減処理 ジャンプ中はどの輪でも無いのでスコアの変動はしない
			if (objectContainer.jumpType == ObjectContainer.JumpType.None) {
				_ScoreCheck();
			}

			if (_effectActive) {
				_effectTime++;
				if (_effectTime > MAX_TIME) {
					_effectCount++;
					_nowRed = _nowRed ? false : true;
					enemySkin.gameObject.SetActive(_nowRed);
					_effectTime = 0;

					if (_effectCount > MAX_EFFECT) {
						_effectActive = false;
						enemySkin.gameObject.SetActive(true);
					}
				}
			}

			if (_scoreDownGimmick) {
				showPercent = Mathf.MoveTowards(showPercent, realPercent, 3.0f);
			} else {
				showPercent = Mathf.MoveTowards(showPercent, realPercent, 0.15f);
			}
			scoreText.text = (int)showPercent + "%";

			//30%以下になると点滅する
			if (showPercent <= 30) {

				//この時に一番内側にいるなら警告音を出す
				MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(objectContainer.nowMobiusID);
				if (mobius.mobiusID == 1 && objectContainer.isInside) {
					if (!nowWarning) {
						SoundManager.Instance.PlaySE("Warning");
						nowWarning = true;
					}
				}
				//選択中の項目をフェードさせる
				if (_fadeIn) {
					_fadeCount -= FADE_SPEED;
					if (_fadeCount < 0.0f) {
						_fadeCount = 0.0f;
						_fadeIn = false;
					}
				} else {
					_fadeCount += FADE_SPEED;
					if (_fadeCount > 1.0f) {
						_fadeCount = 1.0f;
						_fadeIn = true;
					}
				}
			} else {
				_fadeCount = 1.0f;
				nowWarning = false;
			}

			_SetScoreColor();
		}

		public void SetDeath()
		{
			enemyEffect.SetActive(false);
			_fadeCount = 1.0f;
			_SetScoreColor();
		}

		public void SetRealScore()
		{
			realPercent = (float)enemyScore / ObjectController.Instance.maxEnemyScore * 100.0f;
			showPercent = realPercent;
			scoreText.text = (int)showPercent + "%";

			SetSliderDirection();
			_SetScoreColor();
		}

		public void SetAnimationTrigger(string trigger)
		{
			if (animator == null) {
				animator = GetComponent<Animator>();
			}

			animator.SetTrigger(trigger);
		}

		public void SetSliderDirection()
		{
			if (objectContainer.isReverse && objectContainer.isInside) {
				Vector3 scale = scoreText.transform.localScale;
				scale.x = -0.004f;
				scoreText.transform.localScale = scale;
			} else if (!objectContainer.isReverse && !objectContainer.isInside) {
				Vector3 scale = scoreText.transform.localScale;
				scale.x = -0.004f;
				scoreText.transform.localScale = scale;
			} else {
				Vector3 scale = scoreText.transform.localScale;
				scale.x = 0.004f;
				scoreText.transform.localScale = scale;
			}
		}

		//スコアの増減を行う
		private void _ScoreCheck()
		{
			_scoreFrameCouter++;
			MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(objectContainer.nowMobiusID);

			//スコアを減らすギミックの効果が発動中
			if (_scoreDownGimmick) {
				if (_scoreFrameCouter > 4) {
					_scoreFrameCouter = 0;
					if (enemyScore > _scoreDownTarget) { //目標の数値になるまで減り続ける
						enemyScore--;
						realPercent = (float)enemyScore / ObjectController.Instance.maxEnemyScore * 100.0f;
					} else {
						_scoreDownGimmick = false;
					}
				}
				return;
			}

			if (GlobalSettingController.Instance.nowScrew) return;

			if (mobius.mobiusID == 1 && objectContainer.isInside) {
				enemyEffect.SetActive(true);
				//スコアが下がる
				if (_scoreFrameCouter > GlobalSettingController.Instance.GetGameSetting().Sheet1[mobius.mobiusID].score_remove_speed) {
					_scoreFrameCouter = 0;
					if (enemyScore > 0) {
						enemyScore--;
						realPercent = (float)enemyScore / ObjectController.Instance.maxEnemyScore * 100.0f;
					} else {
						isDead = true;
					}
				}
			} else {
				enemyEffect.SetActive(false);
				//スコアが増える
				if (_scoreFrameCouter > GlobalSettingController.Instance.GetGameSetting().Sheet1[mobius.mobiusID].score_add_speed) {
					_scoreFrameCouter = 0;

					//各輪の最大値よりは増えないようにする
					if (objectContainer.isInside && enemyScore >= mobius.maxChargeInSide) return;
					if (!objectContainer.isInside && enemyScore >= mobius.maxChargeOutSide) return;

					if (enemyScore < maxScore) {
						enemyScore++;
						realPercent = (float)enemyScore / ObjectController.Instance.maxEnemyScore * 100.0f;
					}
				}
			}
		}

		private void _SetScoreColor()
		{
			if (showPercent >= 100) {
				gradationContainer.colorBottom = color_0;
			} else if (showPercent >= 80) {
				gradationContainer.colorBottom = color_1;
			} else if (showPercent >= 30) {
				gradationContainer.colorBottom = color_2;
			} else if (showPercent >= 11) {
				gradationContainer.colorBottom = color_3;
			} else {
				gradationContainer.colorBottom = color_4;
			}

			Color c = scoreText.color;
			c.a = _fadeCount;
			scoreText.color = c;
			gradationContainer.SetAlpha(_fadeCount);
		}

		private void _ScrewCheck()
		{
			if (objectContainer.nowMobiusData == null) return;

			if (objectContainer.nowMobiusData.isScrew) {
				if (objectContainer.rotationType == ObjectContainer.RotationType.None) {
					//フラグ切り替え
					if (objectContainer.isInside) {
						//外側に向かう
						if (objectContainer.isReverse) {
							objectContainer.rotationType = ObjectContainer.RotationType.ClockWizeOut;
						} else {
							objectContainer.rotationType = ObjectContainer.RotationType.ReverseOut;
						}
						objectContainer.isInside = false;
					} else {
						//内側に向かう
						if (objectContainer.isReverse) {
							objectContainer.rotationType = ObjectContainer.RotationType.ClockWizeIn;
						} else {
							objectContainer.rotationType = ObjectContainer.RotationType.ReverseIn;
						}
						objectContainer.isInside = true;

						//内側に入ったとしばらくしてからカウントダウンを始めさせる
						_scoreFrameCouter = -80;
					}
				}
			} else {
				//objectContainer.rotationType = ObjectContainer.RotationType.None;

				////if (objectContainer.isInside) {
				////	objectContainer.angle = 180;
				////} else {
				////	objectContainer.angle = 0;
				////}
				//objectContainer.isCircleChange = true;
			}
		}

		//自分とオブジェクトが同じ面に居るか
		private bool _HitObjectCheck(ObjectContainer container)
		{
			return container.isInside == objectContainer.isInside;
		}

		//ギミックとの当たり判定
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag != "Gimmick") return;

			if (objectContainer.jumpType != ObjectContainer.JumpType.None) return;

			ObjectContainer con = other.gameObject.GetComponent<ObjectContainer>();
			if (!_HitObjectCheck(con)) return;

			GimmickContainer gimmick = other.gameObject.GetComponent<GimmickContainer>();
			switch (gimmick.gimmickType) {
				case GimmickContainer.GimmickType.Reverse:
					//ぶつかったら向きを反転させる
					if (objectContainer.isReverse) {
						objectContainer.isReverse = false;
					} else {
						objectContainer.isReverse = true;
					}
					break;
				case GimmickContainer.GimmickType.SpeedUp:
					objectContainer.aditionalSpeed = GlobalSettingController.Instance.GetGameSetting().Sheet1[0].gimmick_add_speed;
					objectContainer.aditionalTime = GlobalSettingController.Instance.GetGameSetting().Sheet1[0].gimmick_time;
					bakeMesh.SetGhostActive(true);
					SoundManager.Instance.PlaySE("SpeedUp");
					break;
				case GimmickContainer.GimmickType.SpeedDown:
					objectContainer.aditionalSpeed = -GlobalSettingController.Instance.GetGameSetting().Sheet1[0].gimmick_remove_speed;
					objectContainer.aditionalTime = GlobalSettingController.Instance.GetGameSetting().Sheet1[0].gimmick_time;
					break;
				case GimmickContainer.GimmickType.ScoreDown:
					_scoreDownGimmick = true;
					_scoreDownTarget = 1;

					_effectActive = true;
					_effectCount = 0;
					_effectTime = 0;
					_nowRed = false;
					SoundManager.Instance.PlaySE("Electric");
					break;
			}
		}
	}
}