using Client.FrameWork.Editors;
using UnityEngine;
using System.Collections.Generic;
using PathCreation;
using PathCreation.Examples;
using System.Linq;
using Client.Game.Models;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Client.Game.Containers.Managers
{
	//メビウスの輪を生成するスクリプト
	public class MobiusCreator : MonoBehaviour
	{
#if UNITY_EDITOR
		[Header("メビウスの輪の生成")]
		[SerializeField, Label("輪の半径")] public float range;
		[SerializeField, Label("分割数")] public int pathCount;
		[SerializeField, Label("輪の補間値")] public float addRange;
		[SerializeField, Label("デフォルトタイプ")] public MobiusData.MobiusType mobiusType;
		[SerializeField, Label("Mobius Manager")] public MobiusManager mobiusManager;


		public void OnCreate()
		{
			//まず子要素を全部消す
			foreach (Transform child in transform) {
				EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
			}

			//分割数と同じだけオブジェクトを生成
			List<GameObject> circleList = _CreateCircle(range, pathCount);

			//分割数の4倍のオブジェクトを生成(ベジェ曲線の補間計算用)
			List<GameObject> circleList2 = _CreateCircle(range + addRange, pathCount * 4);


			//PathCreatorを配置する
			for (int i = 0; i < circleList.Count; i++) {
				GameObject path = new GameObject("Path_" + (i + 1));
				path.transform.SetParent(transform);
				var pathCreator = path.AddComponent<PathCreator>();
				var roadMeshCreator = path.AddComponent<RoadMeshCreator>();

				pathCreator.bezierPath.GlobalNormalsAngle = 90;
				pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;

				pathCreator.bezierPath.AddSegmentToEnd(circleList[i].transform.position);



				if (i == circleList.Count - 1) {
					pathCreator.bezierPath.AddSegmentToEnd(circleList[0].transform.position);
				} else {
					pathCreator.bezierPath.AddSegmentToEnd(circleList[i + 1].transform.position);
				}

				pathCreator.bezierPath.DeleteSegment(0);
				pathCreator.bezierPath.DeleteSegment(1);

				roadMeshCreator.pathCreator = pathCreator;
				roadMeshCreator.mobiusManager = mobiusManager;
				roadMeshCreator.mobiusType = mobiusType;




				pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Aligned;

				//補間を行い、きれいな円を作る
				pathCreator.bezierPath.SetPoint(1, circleList2[i * 4 + 1].transform.position);
				pathCreator.bezierPath.SetPoint(2, circleList2[i * 4 + 3].transform.position);
			}

			//Circleを消す
			foreach (GameObject n in circleList) {
				DestroyImmediate(n);
			}
			foreach (GameObject n in circleList2) {
				DestroyImmediate(n);
			}

			circleList.Clear();
		}

		private List<GameObject> _CreateCircle(float r, int c)
		{
			//指定された数のオブジェクトを生成
			List<GameObject> circleList = new List<GameObject>();
			for (int i = 0; i < c; i++) {
				GameObject circle = new GameObject("Circle" + (i + 1));
				circleList.Add(circle);
			}

			//円状に配置する
			float angleDiff = 360f / (float)circleList.Count;
			for (int i = 0; i < circleList.Count; i++) {
				Vector3 childPostion = transform.position;

				float angle = (90 - angleDiff * i) * Mathf.Deg2Rad;
				childPostion.x += r * Mathf.Cos(angle);
				childPostion.y += r * Mathf.Sin(angle);

				circleList[i].transform.position = childPostion;
			}

			return circleList;
		}
#endif
	}

#if UNITY_EDITOR
	//ボタンをインスペクターに置く処理
	[CustomEditor(typeof(MobiusCreator))]
	public class CreateButton : Editor
	{

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var t = target as MobiusCreator;

			GUILayout.Space(20);
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.Space();
				//EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
				if (GUILayout.Button("Mobius Create",
					GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(30))) {
					t.OnCreate();
				}
				//EditorGUI.EndDisabledGroup();
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(20);
		}
	}
#endif
}
