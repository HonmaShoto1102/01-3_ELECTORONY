using cakeslice;
using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using Client.Game.Containers.Managers;
using Client.Game.Controllers;
using Client.Game.Models;
using PathCreation.Examples;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.Game.Containers
{
	//メビウスの輪の親にアタッチされるコンテナー
	public class MobiusContainer : MonoBehaviour
	{
		[Header("輪の設定")]
		[SerializeField, Label("輪のID")] public int mobiusID;
		[SerializeField, Label("輪の半径")] public float radius;
		[SerializeField, Label("パスの数")] public int pathCount;

		[HideInInspector] public int maxChargeInSide;  //内側で溜まる最大スコア
		[HideInInspector] public int maxChargeOutSide; //外側で溜まる最大スコア

		//メビウスの輪のパーツ一つあたりの情報を管理するクラス
		[HideInInspector] public List<MobiusData> mobiusDataList = new List<MobiusData>();

		//GameManagerクラス
		[HideInInspector] public MobiusManager mobiusManager;

		// MobiusCollisionaクラス
		private MobiusCollision _mobiusCollision;

		//輪が自動で戻るやつを管理する辞書
		private Dictionary<MobiusData, int> _mobiusCoolDownDict = new Dictionary<MobiusData, int>();

		public void Initialize()
		{
			//コンポーネントを取得してリストに入れる(毎回GetComponentしないように)
			GameObject[] childGameObjects = GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToArray();

			int pathId = 0;
			//取得してきた子オブジェクトからMobiusDataを取り出す
			foreach (Transform child in transform) {
				RoadMeshCreator roadMeshCreator = child.GetComponent<RoadMeshCreator>();

				//リストに追加
				MobiusData data = new MobiusData(pathId, true, roadMeshCreator.mobiusType, roadMeshCreator.defaultScrew, roadMeshCreator);
				mobiusDataList.Add(data);

				//最初からねじれてる輪の設定
				if (roadMeshCreator.defaultScrew) {
					roadMeshCreator.pathCreator.bezierPath.SetAnchorNormalAngle(0, 179.0f);

					//頂点更新
					roadMeshCreator.pathCreator.EditorData.VertexPathSettingsChanged();
					roadMeshCreator.TriggerUpdate();
				}

				pathId++;

				//マテリアルを設定
				_SetMaterial(data);
			}

			SetOutLine(false);
		}

		public void OnUpdate()
		{
			//輪を自動で戻す
			if (_mobiusCoolDownDict.Count > 0) {
				List<MobiusData> list = _mobiusCoolDownDict.Keys.ToList();

				foreach (MobiusData data in list) {
					//カウントが0になったら自動で戻す
					if (_mobiusCoolDownDict[data] == 0) {
						data.isScrew = false;
						data.screwAnimationType = MobiusData.ScrewAnimationType.Flat;
						data.isChange = true;

						_mobiusCoolDownDict.Remove(data);
					} else {
						_mobiusCoolDownDict[data] = _mobiusCoolDownDict[data] - 1;
					}
				}
			}

			//更新のフラグが立っているデータを探す
			for (int i = 0; i < mobiusDataList.Count; i++) {
				if (!mobiusDataList[i].isChange) continue;

				mobiusDataList[i].isChange = false;

				float angle = 0.0f;

				//輪のアニメーションを管理
				switch (mobiusDataList[i].screwAnimationType) {
					case MobiusData.ScrewAnimationType.Screw:
						angle = mobiusDataList[i].roadMeshCreator.pathCreator.bezierPath.GetAnchorNormalAngle(0);
						angle += mobiusDataList[i].screwSpeed;
						if (angle > 179.0f) {
							angle = 179.0f;
							mobiusDataList[i].screwAnimationType = MobiusData.ScrewAnimationType.Stay;
						} else {
							//ねじりきって無い場合は次のフレームも更新を行わせる
							mobiusDataList[i].isChange = true;
						}
						mobiusDataList[i].roadMeshCreator.pathCreator.bezierPath.SetAnchorNormalAngle(0, angle);
						break;
					case MobiusData.ScrewAnimationType.Flat:
						angle = mobiusDataList[i].roadMeshCreator.pathCreator.bezierPath.GetAnchorNormalAngle(0);
						angle -= mobiusDataList[i].screwSpeed;
						if (angle < 0.0f) {
							angle = 0.0f;
							GlobalSettingController.Instance.nowScrew = false;
							mobiusDataList[i].screwAnimationType = MobiusData.ScrewAnimationType.Stay;
						} else {
							//ねじりきって無い場合は次のフレームも更新を行わせる
							mobiusDataList[i].isChange = true;
						}
						mobiusDataList[i].roadMeshCreator.pathCreator.bezierPath.SetAnchorNormalAngle(0, angle);
						break;
				}

				//頂点更新
				mobiusDataList[i].roadMeshCreator.pathCreator.EditorData.VertexPathSettingsChanged();
				mobiusDataList[i].roadMeshCreator.TriggerUpdate();

				//マテリアル更新
				_SetMaterial(mobiusDataList[i]);
			}
		}

		public void SetOutLine(bool select)
		{
			foreach (MobiusData data in mobiusDataList) {
				if (select) {
					Outline line = data.roadMeshCreator.meshHolder.GetComponent<Outline>();
					line.eraseRenderer = false;
					line.color = 0;
					data.roadMeshCreator.meshHolder.layer = 7;
				} else {
					Outline line = data.roadMeshCreator.meshHolder.GetComponent<Outline>();
					line.eraseRenderer = true;
					data.roadMeshCreator.meshHolder.layer = 2;
				}
			}
		}

		//輪のマテリアルを更新する
		private void _SetMaterial(MobiusData mobiusData)
		{
			mobiusData.roadMeshCreator.mobiusType = mobiusData.mobiusType;

			//選択中の輪の場合、アウトライン付きマテリアルを適応
			if (MobiusController.Instance.selectMobiusList.Contains(mobiusData)) {
				mobiusData.roadMeshCreator.isSelected = true;
				mobiusData.roadMeshCreator.TriggerUpdate();
				return;
			}
			mobiusData.roadMeshCreator.isSelected = false;
			mobiusData.roadMeshCreator.TriggerUpdate();
		}

		//PathIDに一致するMobiusDataを返す
		public MobiusData GetMobiusDataFromID(int pathID)
		{
			return mobiusDataList.First(data => data.pathID == pathID);
		}

		//現在ねじれている輪の数を返す
		public int GetScrewCount()
		{
			return _mobiusCoolDownDict.Count;
		}

		//選択範囲にある輪をひねる
		public void MobiusScrew(MobiusData target, int screwTime)
		{
			//ひねる
			target.isScrew = true;
			target.screwAnimationType = MobiusData.ScrewAnimationType.Screw;

			if (!_mobiusCoolDownDict.ContainsKey(target)) {
				_mobiusCoolDownDict.Add(target, screwTime);
			}

			target.isChange = true;
		}
	}

}