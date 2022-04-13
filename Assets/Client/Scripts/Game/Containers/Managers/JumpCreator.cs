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
	//Jumpを生成するスクリプト
	public class JumpCreator : MonoBehaviour
	{
#if UNITY_EDITOR
		[Header("Jumpの生成")]
		[SerializeField, Label("左パス")] public Transform leftPath;
		[SerializeField, Label("右パス")] public Transform rightPath;
		[SerializeField, Label("傾き")] public float angle;

		[SerializeField, Label("Game Manager")] public MobiusManager mobiusManager;
		[SerializeField, Label("横幅")] public float size;


		public void OnCreate()
		{
			var pathCreator = GetComponent<PathCreator>();
			var roadMeshCreator = GetComponent<RoadMeshCreator>();

			if (pathCreator == null) {
				pathCreator = gameObject.AddComponent<PathCreator>();
			}

			if (roadMeshCreator == null) {
				roadMeshCreator = gameObject.AddComponent<RoadMeshCreator>();

				roadMeshCreator.mobiusManager = mobiusManager;
				roadMeshCreator.mobiusType = MobiusData.MobiusType.JumpType;
				roadMeshCreator.roadWidth = size;
				roadMeshCreator.pathCreator = pathCreator;
				roadMeshCreator.useCol = false;
			}
			roadMeshCreator.mobiusType = MobiusData.MobiusType.JumpType;

			//PathCreatorを配置する
			List<Vector3> transList = new List<Vector3>();
			//左から右で生成
			transList.Add(new Vector3(leftPath.localPosition.x, leftPath.localPosition.y, leftPath.localPosition.z)); //自分の場所
			transList.Add(new Vector3(rightPath.localPosition.x, rightPath.localPosition.y, rightPath.localPosition.z)); //行き先

			pathCreator.bezierPath = new BezierPath(transList, false, PathSpace.xyz);
			pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Aligned;
			pathCreator.bezierPath.AutoControlLength = 0.3f;

			pathCreator.bezierPath.GlobalNormalsAngle = angle;

			pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
			roadMeshCreator.pathCreator = pathCreator;

			pathCreator.EditorData.displayAnchorPoints = false;
		}
#endif
	}

#if UNITY_EDITOR
	//ボタンをインスペクターに置く処理
	[CustomEditor(typeof(JumpCreator))]
	public class CreateButton2 : Editor
	{

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var t = target as JumpCreator;

			GUILayout.Space(20);
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.Space();
				//EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
				if (GUILayout.Button("Jump Create",
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
