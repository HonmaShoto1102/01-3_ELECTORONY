using Client.FrameWork.Editors;
using Client.Game.Containers;
using Client.Game.Controllers;
using Client.Game.Models;
using PathCreation;
using UnityEditor;
using UnityEngine;

namespace Client.Game.Containers
{
	//自分、味方、敵、ギミック等全てのオブジェクトにアタッチされるコンテナー
	public class ObjectContainer : MonoBehaviour
	{
		//回転タイプ
		public enum RotationType
		{
			None,      //回転していない
			ClockWizeIn, //時計回りで内側に入る
			ClockWizeOut, //時計回りで外側に入る
			ReverseIn,   //反時計回りで内側に入る
			ReverseOut,   //反時計回りで外側に入る
		}

		//ジャンプタイプ
		public enum JumpType
		{
			None,    //ジャンプしてない
			Normal,  //左から右
			Reverse, //右から左
		}

		//リソースのパス
		private const string CIRCLE_PATH = "Prefabs/CirclePath";

		[SerializeField, Label("自動で移動するかしないか")] public bool autoMove;
		[SerializeField, Label("移動速度(正の値を入れること)")] public float moveSpeed;
		[SerializeField, Label("回転速度")] public float rotateSpeed;
		[SerializeField, Label("現在居るメビウスID")] public int nowMobiusID;
		[SerializeField, Label("輪の軌道からどれだけ離れるか")] public float travelOffset;
		[SerializeField, Label("輪の内側にいるかどうか")] public bool isInside;
		[SerializeField, Label("時計回りかどうか")] public bool isReverse;

		[HideInInspector] public float angle; //回転度
		[HideInInspector] public float nowTravel; //現在の輪の軌道上の座標
		[HideInInspector] public bool isMoveChange; //今回の更新で座標の移動が必要かどうか
		[HideInInspector] public bool isCircleChange; //今回の更新で軌道の再計算が必要かどうか

		[HideInInspector] public MobiusData nowMobiusData; //現在いる場所のデータ
		[HideInInspector] public bool isMobiusChange; //選択中の輪が変わったかどうか

		[HideInInspector] public JumpType jumpType; //他の輪にジャンプ中かどうか
		[HideInInspector] public VertexPath jumpPath; //ジャンプの軌道
		[HideInInspector] public int jumpSkip; //ジャンプの行わない時間

		[HideInInspector] public RotationType rotationType; //現在の回転タイプ
		[HideInInspector] public CircleContainer circleContainer; //メビウスの輪の軌道
		private bool _isInitialized; //初期化済みかどうか

		[HideInInspector] public float aditionalSpeed; //追加の移動速度
		[HideInInspector] public int aditionalTime; //追加の移動速度が有効な時間

		private int _jumpRotateCount; //ジャンプの回転度を格納する

		private void Start()
		{
			_isInitialized = false;
			//コントローラーに登録する
			ObjectController.Instance.AddObjectContainer(this);
		}


		public void Initialize()
		{
			//初回移動フラグを立てる
			isMoveChange = true;
			isCircleChange = true;

			if (isInside) {
				angle = 180;
			} else {
				angle = 0;
			}

			//プレハブからCirclePathを複製する
			GameObject obj = (GameObject)Resources.Load(CIRCLE_PATH);
			GameObject clone = Instantiate(obj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			clone.SetActive(false);

			//複製したオブジェクトのCircleContainerを取得する
			circleContainer = clone.GetComponent<CircleContainer>();

			//コンテナーを取得
			MobiusContainer mobiusContainer = MobiusController.Instance.GetMobiusContainer(nowMobiusID);
			//初期化
			circleContainer.Initialize(mobiusContainer.transform);

			//軌道生成
			_SetPlayerCircle(false, angle);

			//初期地点をTransformから計算してTravel座標に変換する
			nowTravel = circleContainer.getNearTravel(transform);

			_jumpRotateCount = 0;

			//初期化済みフラグ
			_isInitialized = true;
		}

		public void OnUpdate()
		{
			if (jumpSkip > 0) {
				jumpSkip--;
			}

			//初期化をまだしていないらする
			if (!_isInitialized) {
				Initialize();
			}

			//自動移動するオブジェクトなら座標を更新する
			if (autoMove) {
				AddMove();
			}

			if (rotationType != RotationType.None) {
				isCircleChange = true;
			}

			//軌道の再計算が必要なら行う
			if (isCircleChange) {
				_SetPlayerCircle(false, angle);
				//軌道の計算を行う場合は移動も更新する
				isMoveChange = true;
			}

			//移動が必要なら移動させる
			if (isMoveChange) {
				_SetPosition();
			}
		}

		private void _AddMoveAditional(bool plus)
		{
			if (aditionalTime > 0) {
				aditionalTime--;
				if (plus) {
					nowTravel += aditionalSpeed;
				} else {
					nowTravel -= aditionalSpeed;
				}

				if (aditionalTime == 0) {
					ObjectController.Instance.GetEnemyContainer(this).bakeMesh.SetGhostActive(false);
				}
			}
		}

		public void AddMove()
		{
			//移動が必要フラグを立てる
			isMoveChange = true;

			MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(nowMobiusID);

			//ジャンプ中(左から右)
			if (jumpType == JumpType.Normal) {
				if (isInside) {
					nowTravel += (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].inside_speed);
				} else {
					nowTravel += (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].outside_speed);
				}
				_AddMoveAditional(true);
				return;
			}

			//ジャンプ中(右から左)
			if (jumpType == JumpType.Reverse) {
				if (isInside) {
					nowTravel -= (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].inside_speed);
				} else {
					nowTravel -= (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].outside_speed);
				}
				_AddMoveAditional(false);
				return;
			}

			//時計回り
			if (isReverse) {
				if (isInside) {
					nowTravel -= (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].inside_speed);
				} else {
					nowTravel -= (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].outside_speed);
				}

				if (MobiusController.Instance.manager.GetAllMobiusCount() == 1) {
					nowTravel -= 0.01f;
				}

				_AddMoveAditional(false);

				//前後関係が分かるように座標を調整
				if (nowTravel <= 0) {
					nowTravel += circleContainer.GetCircleSize();
				}
			} else { //反時計回り
				if (isInside) {
					nowTravel += (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].inside_speed);
				} else {
					nowTravel += (moveSpeed * GlobalSettingController.Instance.GetGameSetting().Sheet1[nowMobiusID].outside_speed);
				}

				if (MobiusController.Instance.manager.GetAllMobiusCount() == 1) {
					nowTravel += 0.01f;
				}

				_AddMoveAditional(true);

				//前後関係が分かるように座標を調整
				if (nowTravel >= circleContainer.GetCircleSize()) {
					nowTravel -= circleContainer.GetCircleSize();
				}
			}

			//回転の計算
			switch (rotationType) {
				case RotationType.ClockWizeIn: //時計回りで内側に行く
					angle -= rotateSpeed;
					if (angle <= -180) {
						angle = 180;
						rotationType = RotationType.None;
					}
					break;
				case RotationType.ClockWizeOut: //時計回りで外側に行く
					angle -= rotateSpeed;
					if (angle <= 0) {
						angle = 0;
						rotationType = RotationType.None;
					}
					break;
				case RotationType.ReverseIn: //反時計回りで内側に行く
					angle -= rotateSpeed;
					if (angle <= -180) {
						angle = 180;
						rotationType = RotationType.None;
					}
					break;
				case RotationType.ReverseOut: //反時計回りで外側に行く
					angle -= rotateSpeed;
					if (angle <= 0) {
						angle = 0;
						rotationType = RotationType.None;
					}
					break;
			}
		}

		//オブジェクトが周回する輪の軌道を生成する
		private void _SetPlayerCircle(bool isChangeTravel, float angle)
		{
			isCircleChange = false;

			//コンテナーを取得
			MobiusContainer mobiusContainer = MobiusController.Instance.GetMobiusContainer(nowMobiusID);
			circleContainer.Initialize(mobiusContainer.transform);

			//輪のサイズを求めて軌道を生成する
			float size = mobiusContainer.radius + travelOffset;

			//位置の計算が必要かどうかを調べる
			bool travelSizeChange = circleContainer.GetCircleSize() != 0.0f;

			if (!isChangeTravel) travelSizeChange = false;

			float travelSize = 0.0f;
			if (travelSizeChange) {
				//輪のサイズが変わる前の輪に対する現在の位置の割合を計算
				travelSize = travelSize = nowTravel / circleContainer.GetCircleSize();
			}

			circleContainer.ChangeCircleSize(size, angle);


			if (travelSizeChange) {
				//新しいサイズの輪でサイズ変更前と同じ座標になるように計算する
				nowTravel = circleContainer.GetCircleSize() * travelSize;
			}
		}

		//オブジェクトの座標を更新する
		private void _SetPosition()
		{
			isMoveChange = false;

			//左から右
			if (jumpType == JumpType.Normal) {
				if (nowTravel > jumpPath.length) {
					jumpType = JumpType.None;

					if (isInside) {
						angle = 180;
					} else {
						angle = 0;
					}

					_SetPlayerCircle(false, angle);
					nowTravel = circleContainer.getNearTravel(transform);

					jumpSkip = 40;

					rotationType = RotationType.None;

					_jumpRotateCount = 0;

					//もしEnemyならJumpアニメーションを終了させる
					EnemyContainer enemy = ObjectController.Instance.GetEnemyContainer(this);
					if (enemy != null) {
						enemy.SetAnimationTrigger("Anim_End");
					}
					return;
				}

				//現在の座標を軌道上に配置
				transform.position = jumpPath.GetPointAtDistance(nowTravel);

				if (_jumpRotateCount < 180) {
					if (nowTravel > jumpPath.length / 3) {
						transform.Rotate(0, 0, 15);
						_jumpRotateCount += 15;
					}
				}
				return;
			}

			//右から左
			if (jumpType == JumpType.Reverse) {
				if (nowTravel < 0) {
					jumpType = JumpType.None;

					if (isInside) {
						angle = 180;
					} else {
						angle = 0;
					}

					_SetPlayerCircle(false, angle);
					nowTravel = circleContainer.getNearTravel(transform);

					jumpSkip = 40;

					rotationType = RotationType.None;

					_jumpRotateCount = 0;

					//もしEnemyならJumpアニメーションを終了させる
					EnemyContainer enemy = ObjectController.Instance.GetEnemyContainer(this);
					if (enemy != null) {
						enemy.SetAnimationTrigger("Anim_End");
					}
					return;
				}

				//現在の座標を軌道上に配置
				transform.position = jumpPath.GetPointAtDistance(nowTravel);

				if (_jumpRotateCount < 180) {
					if (nowTravel < jumpPath.length / 2) {
						transform.Rotate(0, 0, 15);
						_jumpRotateCount += 15;
					}
				}
				return;
			}

			//現在の座標を軌道上に配置
			transform.position = circleContainer.vertexPath.GetPointAtDistance(nowTravel);
			//現在の回転を軌道上に配置
			transform.rotation = circleContainer.vertexPath.GetRotationAtDistance(nowTravel);

			//時計回りならオブジェクトの向きも回転させる
			if (isReverse) {
				Quaternion qua = transform.rotation;
				qua.x = qua.x * -1;
				qua.y = qua.y * -1;
				transform.rotation = qua;
			}
		}

		public void UpdateObject()
		{
			isCircleChange = true;
			isMoveChange = true;

			if (isInside) {
				angle = 180;
			} else {
				angle = 0;
			}
		}
	}
}
#if UNITY_EDITOR
//手動更新ボタンをインスペクターに置く処理
[CustomEditor(typeof(ObjectContainer))]
public class UpdateButton : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var t = target as ObjectContainer;

		GUILayout.Space(20);
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.Space();
			EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
			if (GUILayout.Button("Manual Update",
				GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(30))) {
				t.UpdateObject();
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Space();
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(20);
	}
}
#endif