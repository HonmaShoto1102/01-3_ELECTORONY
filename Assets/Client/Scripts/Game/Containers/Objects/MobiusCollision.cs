using Client.FrameWork.Editors;
using Client.Game.Controllers;
using Client.Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Containers
{
	//輪とオブジェクトの当たり判定を行う
	public class MobiusCollision : MonoBehaviour
	{
		[SerializeField, Label("Object Container")] private ObjectContainer objectContainer;

		private bool _collisionReset = false; //当たり判定を更新するべきかどうか
		private List<Collider> _collisionList = new List<Collider>(); //当たった判定リスト
		private GameObject oldTriggerObject; //前のフレームで当たっていたオブジェクト

		private void Update()
		{
			//当たり判定が無かった(輪から浮いている?)
			if (_collisionList.Count == 0) {
				objectContainer.nowMobiusData = null;
				objectContainer.isMobiusChange = true;
			} else if (_collisionList.Count == 1) { //当たったオブジェクトは一つだけ
				if (oldTriggerObject == null || _collisionList[0].gameObject != oldTriggerObject) {
					_UpdateTargetObject(_collisionList[0].gameObject);
				}
			} else { //複数の輪と当たり判定があった
				//一番近いオブジェクトを探す
				float distance = 9999.9f;
				GameObject near = null;

				foreach (Collider collider in _collisionList) {
					if (Vector3.Distance(transform.position, collider.bounds.center) < distance) {
						distance = Vector3.Distance(transform.position, collider.bounds.center);
						near = collider.gameObject;
					}
				}

				if (near != null) {
					if (oldTriggerObject == null || near != oldTriggerObject) {
						_UpdateTargetObject(near);
					}
				}
			}

			_collisionReset = true;
		}

		private void _UpdateTargetObject(GameObject hit)
		{
			oldTriggerObject = hit;

			//当たり判定のあったオブジェクトが輪のどのパーツかを調べる
			MobiusContainer mobius = MobiusController.Instance.GetMobiusContainer(objectContainer.nowMobiusID);

			foreach (MobiusData data in mobius.mobiusDataList) {
				//データの中にあるGameObjectが当たり判定で当たったものと一致するか調べる
				if (data.roadMeshCreator.meshHolder == hit) {
					//見つかった
					objectContainer.nowMobiusData = data;
					objectContainer.isMobiusChange = true;
					return;
				}
			}
		}

		//オブジェクトとトリガーの当たり判定
		void OnTriggerStay(Collider other)
		{
			if (_collisionReset) {
				_collisionList.Clear();
				_collisionReset = false;
			}

			//当たり判定のあったオブジェクトが輪のTagを持っているかどうか
			if (!other.gameObject.CompareTag("RoadMesh")) return;

			_collisionList.Add(other);
		}
	}
}