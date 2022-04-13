using Client.FrameWork.Editors;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Containers
{
	//敵の残像を管理する
	public class BakeMesh : MonoBehaviour
	{
		[SerializeField, Label("元のオブジェクト")] private SkinnedMeshRenderer baseMeshObj;

		[SerializeField, Label("複製するプレハブ")] private GameObject bakeMeshObj;

		[SerializeField, Label("残像の数")] private int cloneCount;
		[SerializeField, Label("残像の更新頻度")] private int frameCountMax;

		private List<SkinnedMeshRenderer> _bakeCloneMeshList;
		private int _frameCount = 0;

		private bool _isActive;

		private void Start()
		{
			_bakeCloneMeshList = new List<SkinnedMeshRenderer>();

			// 残像を複製
			for (int i = 0; i < cloneCount; i++) {
				var obj = Instantiate(bakeMeshObj);
				//obj.transform.SetParent(transform);
				obj.SetActive(false);
				_bakeCloneMeshList.Add(obj.GetComponent<SkinnedMeshRenderer>());
			}
		}

		private void FixedUpdate()
		{
			if (!_isActive) return;

			_frameCount++;

			if (_frameCount < frameCountMax) return;


			// BakeしたMeshを１つ前にずらしていく
			for (int i = _bakeCloneMeshList.Count - 1; i >= 1; i--) {
				if (_bakeCloneMeshList[i - 1] == null) continue;

				_bakeCloneMeshList[i].sharedMesh = _bakeCloneMeshList[i - 1].sharedMesh;

				// 位置と回転をコピー
				_bakeCloneMeshList[i].transform.position = _bakeCloneMeshList[i - 1].transform.position;
				_bakeCloneMeshList[i].transform.rotation = _bakeCloneMeshList[i - 1].transform.rotation;


				if (!_bakeCloneMeshList[i].gameObject.activeSelf) {
					_bakeCloneMeshList[i].gameObject.SetActive(true);
				}
			}

			// 今のスキンメッシュをBakeする
			// ボトルネックになりやすいから注意！
			Mesh mesh = new Mesh();
			baseMeshObj.BakeMesh(mesh);
			_bakeCloneMeshList[0].sharedMesh = mesh;

			// 位置と回転をコピー
			_bakeCloneMeshList[0].transform.position = transform.position;
			_bakeCloneMeshList[0].transform.rotation = transform.rotation;

			if (!_bakeCloneMeshList[0].gameObject.activeSelf) {
				_bakeCloneMeshList[0].gameObject.SetActive(true);
			}

			_frameCount = 0;
		}

		public void SetGhostActive(bool active)
		{
			_isActive = active;
			if (!_isActive) {
				//消す
				foreach (SkinnedMeshRenderer mesh in _bakeCloneMeshList) {
					mesh.gameObject.SetActive(false);
				}
			} else {
				foreach (SkinnedMeshRenderer mesh in _bakeCloneMeshList) {
					mesh.sharedMesh = null;
				}
			}
		}
	}
}
