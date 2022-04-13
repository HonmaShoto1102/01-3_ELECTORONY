using Client.FrameWork.Core;
using Client.Game.Controllers;
using PathCreation;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Containers
{
	//メビウスの輪の軌道を作るために起点となるオブジェクトを円状に配置するコンテナー
	public class CircleContainer : SystemBehaviour
	{

		//輪の軌道の頂点データと曲線データ
		[HideInInspector] public VertexPath vertexPath;
		[HideInInspector] public BezierPath bezierPath;

		public void Initialize(Transform parnent)
		{
			//自分の位置を親の位置に合わせる
			transform.localPosition = parnent.localPosition;
			transform.localRotation = parnent.localRotation;
		}


		//等間隔の円状にオブジェクトを置きなおす
		public void ChangeCircleSize(float newSize, float globalAngle)
		{
			//子を取得
			List<GameObject> childList = _GetCircleObjectList();

			//数値、アルファベット順にソート
			childList.Sort((a, b) => { return string.Compare(a.name, b.name); });

			//ソートを逆にする
			childList.Reverse();

			//オブジェクト間の角度差
			float angleDiff = 360f / (float)childList.Count;

			//各オブジェクトを円状に配置
			for (int i = 0; i < childList.Count; i++) {
				Vector3 childPostion = transform.position;

				float angle = (90 - angleDiff * i) * Mathf.Deg2Rad;
				childPostion.x += newSize * Mathf.Cos(angle);
				childPostion.y += newSize * Mathf.Sin(angle);

				childList[i].transform.position = childPostion;
			}

			//配置が完了したら軌道を計算する
			_UpdateVertexPath(globalAngle);
		}

		public float GetCircleSize()
		{
			if (vertexPath == null) return 0.0f;

			return vertexPath.length;
		}

		//輪の軌道を計算する
		private void _UpdateVertexPath(float globalAngle)
		{
			List<Transform> childList = _GetCircleTransformList();

			bezierPath = new BezierPath(_GetCircleTransformList(), true, PathSpace.xyz);

			bezierPath.GlobalNormalsAngle = globalAngle;

			vertexPath = new VertexPath(bezierPath, MobiusController.Instance.transform);
		}

		//円を構成しているGameObjectのリストを返す
		private List<GameObject> _GetCircleObjectList()
		{
			List<GameObject> childList = new List<GameObject>();
			foreach (Transform child in transform) {
				childList.Add(child.gameObject);
			}

			return childList;
		}

		//円を構成している座標のリストを返す
		private List<Transform> _GetCircleTransformList()
		{
			List<Transform> childList = new List<Transform>();
			foreach (Transform child in transform) {
				childList.Add(child);
			}

			return childList;
		}

		//Transformに一番近いTravel座標を出す
		public float getNearTravel(Transform target)
		{
			float travel = 0.0f;

			float nearTravel = 0.0f;
			float nearDistance = 999.9f;

			Vector3 searchPos;
			while (travel < vertexPath.length) {
				searchPos = vertexPath.GetPointAtDistance(travel);

				float distance = Vector3.Distance(searchPos, target.position);
				if (distance < nearDistance) {
					nearDistance = distance;
					nearTravel = travel;
				}


				travel += 0.05f;
			}

			return nearTravel;
		}
	}
}
