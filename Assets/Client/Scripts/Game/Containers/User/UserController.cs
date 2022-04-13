using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using Client.Game.Controllers;
using Client.Game.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

namespace Client.Game.Containers
{
	//プレイヤーの操作を管理
	public class UserController : MonoBehaviour
	{
		private const int CHARGE_TIME = 40; //溜めねじりの間隔

		//溜めねじりでのコントローラー振動の強さ
		private const float POWER_CHARGE_1 = 0.05f;
		private const float POWER_CHARGE_2 = 0.6f;

		//チュートリアル用
		[HideInInspector] public bool tutorialMode = false;
		//どのパスをねじれるようにするか
		private const int ACTIVE_PATH = 4;

		[SerializeField, Label("操作する対象の輪ID")] private int selectMobiusID;
		[SerializeField, Label("同時にねじれる回数")] private int screwCount;

		[Header("アイコン")]
		[SerializeField, Label("Icon Image")] private Image image;
		[SerializeField, Label("ねじれるアイコン")] private Sprite selectIcon;
		[SerializeField, Label("ねじれないアイコン")] private Sprite hardIcon;

		[SerializeField, Label("ステージスタート")] public Fade stageStart;
		[SerializeField, Label("ステージスタートテキスト")] public Text stageStartText;
		[SerializeField, Label("ステージスタートテキスト2")] public Text stageStartText2;

		private int _chargePower = 0; //溜めている時間のカウント
		private bool _isChargeing; //溜め中かどうか

		private Vector3 _iconPos; //Aボタンのアイコンを出す位置

		private float _nowDegree;
		private float _oldStick;

		public void Initialize()
		{
			MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(true);

			//溜めの情報を初期化
			_ChargeReset();

			MobiusController.Instance.SelectMobiusAllClear();

			GlobalSettingController.Instance.nowScrew = false;
		}

		public void OnUpdate()
		{
			//LR入力で操作対象の輪を変える
			if (!_isChargeing) {
				if (Input.GetButtonDown("R_Button")) {
					if (MobiusController.Instance.GetMobiusCount() > 1) {
						if (MobiusController.Instance.GetMobiusContainer(selectMobiusID - 1) != null) {
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(false);
							selectMobiusID -= 1;
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(true);
						} else {
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(false);
							selectMobiusID = MobiusController.Instance.GetMobiusCount();
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(true);
						}
					}
				}

				if (Input.GetButtonDown("L_Button")) {
					if (MobiusController.Instance.GetMobiusCount() > 1) {
						if (MobiusController.Instance.GetMobiusContainer(selectMobiusID + 1) != null) {
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(false);
							selectMobiusID += 1;
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(true);
						} else {
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(false);
							selectMobiusID = 1;
							MobiusController.Instance.GetMobiusContainer(selectMobiusID).SetOutLine(true);
						}
					}
				}
			}

			//スティック入力を反映させる
			if (!_isChargeing) {
				MobiusData select = _GetStickSelectMobius();
				if (select == null) { //選択なし
					MobiusController.Instance.SelectMobiusAllClear();
					image.enabled = false;
				} else {
					//選択中の輪が変わってたら更新
					if (!MobiusController.Instance.selectMobiusList.Contains(select)) {
						MobiusController.Instance.SelectMobiusAllClear();
						MobiusController.Instance.selectMobiusList.Add(select);

						image.enabled = true;
						image.transform.position = _iconPos;
						if (select.mobiusType == MobiusData.MobiusType.HardType) {
							image.sprite = hardIcon;
						} else {
							image.sprite = selectIcon;
						}

						if (tutorialMode && select.pathID != ACTIVE_PATH) {
							image.sprite = hardIcon;
						}
					}

					select.isChange = true;
				}
			}

			//ボタンが押された瞬間
			if (Input.GetButtonDown("A_Button")) {
				if (MobiusController.Instance.selectMobiusList.Count >= 1) {
					//スティック操作ねじりは常に一か所のみなのでリストの先頭に必ず対象のデータがある
					MobiusData select = MobiusController.Instance.selectMobiusList[0];

					//選択できない所(ハードタイプか既にねじり済みかの場合)
					if (select.mobiusType == MobiusData.MobiusType.HardType || select.isScrew) {
						//コントローラー振動させる
						StartCoroutine("CancelVibration");
						SoundManager.Instance.PlaySE("Game_Hard");
						return; //終了
					}

					//チュートリアルの場合は指定された部分しかねじれない
					if (tutorialMode && select.pathID != ACTIVE_PATH) {
						//コントローラー振動させる
						StartCoroutine("CancelVibration");
						SoundManager.Instance.PlaySE("Game_Hard");
						return; //終了
					}

					//溜めの情報を初期化
					_ChargeReset();

					_isChargeing = true;
				}
			}

			//ボタンを押してる間
			if (Input.GetButton("A_Button")) {
				if (!_isChargeing) return;

				//チュートリアルの場合は溜らない
				if (!tutorialMode) {
					_chargePower++;
				}

				if (_chargePower < CHARGE_TIME) {
					GamePad.SetVibration(0, POWER_CHARGE_1, POWER_CHARGE_1);
					if (MobiusController.Instance.chargeCount == 0) {
						MobiusController.Instance.chargeCount = 1;
						MobiusController.Instance.selectMobiusList[0].isChange = true;
					}
				} else if (_chargePower < CHARGE_TIME * 2) {
					GamePad.SetVibration(0, POWER_CHARGE_2, POWER_CHARGE_2);
					if (MobiusController.Instance.chargeCount == 1) {
						MobiusController.Instance.chargeCount = 2;
						MobiusController.Instance.selectMobiusList[0].isChange = true;
					}
				}
			}

			//ボタンを離したとき
			if (Input.GetButtonUp("A_Button") || Input.GetButtonDown("Option_Button")) {
				if (!_isChargeing) return;

				_isChargeing = false;

				MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(selectMobiusID);

				//溜め時間に応じて捻る秒数を変える
				if (MobiusController.Instance.chargeCount <= 1) {
					mobius.MobiusScrew(MobiusController.Instance.selectMobiusList[0], 60 * 1);
				} else {
					mobius.MobiusScrew(MobiusController.Instance.selectMobiusList[0], 60 * 3);
				}

				MobiusController.Instance.SelectMobiusAllClear();

				GlobalSettingController.Instance.nowScrew = true;

				SoundManager.Instance.PlaySE("Game_Screw");

				//コントローラーの振動を止める
				GamePad.SetVibration(0, 0, 0);

				//溜めの情報を初期化
				_ChargeReset();

				if (tutorialMode) {
					tutorialMode = false;
					image.enabled = false;
				}
			}
		}

		private MobiusData _GetStickSelectMobius()
		{
			//ねじれている輪があれば処理しない
			if (GlobalSettingController.Instance.nowScrew) return null;

			//スティックの入力を取得
			float lh = Input.GetAxis("Left_Stick_H");
			float lv = Input.GetAxis("Left_Stick_V");

			//スティック入力なしなら終了
			if (lh == 0 && lv == 0) {
				return null;
			}
			MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(selectMobiusID);

			if (mobius == null) return null;

			//_nowDegree = Mathf.Atan2(lv, lh) * Mathf.Rad2Deg;

			//if (_nowDegree < 0) {
			//	_nowDegree += 360.0f;
			//}

			if (MobiusController.Instance.selectMobiusList.Count >= 1) {
				float degree = Mathf.Atan2(lv, lh) * Mathf.Rad2Deg;
				if (degree < 0) {
					degree += 360.0f;
				}

				//左回りの方が近い
				bool leftFlag = false;
				float left1 = (_nowDegree - degree);
				if (left1 < 0) {
					left1 *= -1.0f;
				}
				float left2 = (_nowDegree - (degree + 360));
				if (left2 < 0) {
					left2 *= -1.0f;
				}

				float left;
				if (left1 < left2) {
					left = left1;
				} else {
					leftFlag = true;
					left = left2;
				}

				float right = 360 - left;
				if (right < 0) {
					right *= -1.0f;
				}

				if (left < right) {
					if (_nowDegree > degree && leftFlag) {
						degree += 360.0f;
					}
				} else {
					//右回りの方が近い
					if (_nowDegree < degree) {
						degree -= 360.0f;
					}
				}


				if (mobius.mobiusID == 1 && mobius.mobiusDataList.Count == 10) {
					_nowDegree = Mathf.MoveTowards(_nowDegree, degree, 10.0f);
				} else if (mobius.mobiusID == 1 && mobius.mobiusDataList.Count == 20) {
					_nowDegree = Mathf.MoveTowards(_nowDegree, degree, 7.0f);
				} else if (mobius.mobiusID == 2) {
					_nowDegree = Mathf.MoveTowards(_nowDegree, degree, 7.0f);
				} else if (mobius.mobiusID == 3) {
					_nowDegree = Mathf.MoveTowards(_nowDegree, degree, 6.8f);
				}

			} else {
				//最初に輪を選択した場合は角度をスティック入力の方向にする
				_nowDegree = Mathf.Atan2(lv, lh) * Mathf.Rad2Deg;
			}

			if (_nowDegree < 0) {
				_nowDegree += 360.0f;
			}

			if (_nowDegree > 360) {
				_nowDegree -= 360.0f;
			}

			_nowDegree += 2.0f;

			//Rayの飛ばせる距離
			int distance = 10;

			float add = 0.0f;

			//Rayが当たるか無視でreturnされるまでループ
			while (true) {
				Vector3 vec = new Vector3(Mathf.Cos(_nowDegree * Mathf.Deg2Rad), Mathf.Sin(_nowDegree * Mathf.Deg2Rad), 0);

				Ray ray = new Ray(mobius.transform.position, vec);

				//デバッグ用 Rayを描画して入力の方向を可視化する
				//Debug.DrawRay(ray.origin, ray.direction * distance, Color.green, 0.1f, false);

				RaycastHit hit;

				int layerMask = 1 << 7;

				if (Physics.Raycast(ray, out hit, distance, layerMask)) {
					if (hit.collider.tag == "RoadMesh") {
						foreach (MobiusData data in mobius.mobiusDataList) {
							//データの中にあるGameObjectが当たり判定で当たったものと一致するか調べる
							if (data.roadMeshCreator.meshHolder == hit.collider.gameObject) {
								//見つかった

								float near = 0.0f;
								float iconDistance = 0.0f;
								if (mobius.mobiusID == 1 && mobius.mobiusDataList.Count == 10) {
									near = _GetNearDegree(_nowDegree, mobius.mobiusDataList.Count, 0.0f);
									iconDistance = 0.4f;
								} else if (mobius.mobiusID == 1 && mobius.mobiusDataList.Count == 20) {
									near = _GetNearDegree(_nowDegree, mobius.mobiusDataList.Count, 9.0f);
									iconDistance = 0.5f;
								} else if (mobius.mobiusID == 2) {
									near = _GetNearDegree(_nowDegree, mobius.mobiusDataList.Count, 9.0f);
									iconDistance = 0.5f;
								} else if (mobius.mobiusID == 3) {
									near = _GetNearDegree(_nowDegree, mobius.mobiusDataList.Count, 7.5f);
									iconDistance = 0.6f;
								}

								Vector3 vec2 = new Vector3(Mathf.Cos(near * Mathf.Deg2Rad), Mathf.Sin(near * Mathf.Deg2Rad), 0);

								Ray ray2 = new Ray(mobius.transform.position, vec2);
								_iconPos = ray2.GetPoint(hit.distance + iconDistance);

								//ひねれる回数が上限に達していたら無視
								if (mobius.GetScrewCount() >= screwCount) return null;

								return data;
							}
						}
					}
				}

				//Rayが当たらなかった 検索角度を2.0f増やす
				_nowDegree += 2.0f;
				add += 2.0f;

				//チェックが一周したらもうやめる
				if (add >= 360) return null;
			}
		}

		private float _GetNearDegree(float degree, int mobiusCount, float search)
		{
			//一周を分割数で割る
			float per = 360.0f / mobiusCount;

			float distance = 999999.0f; //一番近い距離
			float distance_search = 999999.0f; //一番近い距離の時の角度

			//一周分ループする
			for (int i = 0; i < mobiusCount + 1; i++) {
				//入力角度と検索角度の差を求める
				float work = degree - search;
				if (work < 0) {
					work *= -1;
				}

				//差が小さければ保存
				if (distance > work) {
					distance = work;
					distance_search = search;
				}

				//検索角度を増やす
				search += per;

			}

			//一番差が小さかった時の検索角度を返す
			return distance_search;

		}

		//溜めの情報を初期化する
		private void _ChargeReset()
		{
			_chargePower = 0;
			MobiusController.Instance.chargeCount = 0;
		}

		//ねじれない場所を選択した時のバイブレーション(ブッブッって二回短く振動する)
		IEnumerator CancelVibration()
		{
			GamePad.SetVibration(0, 2.0f, 2.0f);
			yield return new WaitForSecondsRealtime(0.08f);
			GamePad.SetVibration(0, 0, 0);
			yield return new WaitForSecondsRealtime(0.08f);
			GamePad.SetVibration(0, 2.0f, 2.0f);
			yield return new WaitForSecondsRealtime(0.08f);
			GamePad.SetVibration(0, 0, 0);
		}
	}
}