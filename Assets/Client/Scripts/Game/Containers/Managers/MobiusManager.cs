using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using Client.Game.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Client.Game.Models.MobiusData;

namespace Client.Game.Containers.Managers
{
	//全てのメビウスの輪に共通する設定を管理する
	public class MobiusManager : MonoBehaviour
	{

		[Header("輪の見た目の設定")]
		[SerializeField, Label("NormalTypeのマテリアル")] public Material normalTypeMaterial;
		[SerializeField, Label("HardTypeのマテリアル")] public Material hardTypeMaterial;
		[SerializeField, Label("JumpTypeのマテリアル")] public Material jumpTypeMaterial;

		[Header("スコアの設定")]
		[SerializeField, Label("最大スコア数")] public int maxScore;

		private List<MobiusContainer> _mobiusList = new List<MobiusContainer>();

		public void Initialize()
		{
			MobiusController.Instance.manager = this;

			foreach (Transform child in transform) {
				if (!child.gameObject.activeSelf) continue;

				var mobiusContainer = child.GetComponent<MobiusContainer>();
				if (mobiusContainer == null) continue;

				mobiusContainer.mobiusManager = this;
				mobiusContainer.Initialize();

				_mobiusList.Add(mobiusContainer);

				//Controllerにデータを登録する
				MobiusController.Instance.AddDict(mobiusContainer.mobiusID, mobiusContainer);
			}

			//各輪で溜まる最大スコアを決める
			int i = maxScore;

			//登録した輪の数だけループ
			for (int count = _mobiusList.Count; count > 0; count--) {
				MobiusContainer container = MobiusController.Instance.GetMobiusContainer(count);
				if (container != null) {
					container.maxChargeOutSide = i;
					i -= GlobalSettingController.Instance.GetGameSetting().Sheet1[0].score_decrease;
					container.maxChargeInSide = i;
					i -= GlobalSettingController.Instance.GetGameSetting().Sheet1[0].score_decrease;
				}
			}
		}

		//輪のマテリアルを変更する
		public Material getMaterial(MobiusType type)
		{
			switch (type) {
				case MobiusType.NormalType:
					return normalTypeMaterial;
				case MobiusType.HardType:
					return hardTypeMaterial;
				case MobiusType.JumpType:
					return jumpTypeMaterial;
			}

			return null;
		}

		public void OnUpdate()
		{
			//輪の更新
			for (int i = 0; i < _mobiusList.Count; i++) {
				_mobiusList[i].OnUpdate();
			}
		}

		public int GetAllMobiusCount()
		{
			return _mobiusList.Count;
		}
	}
}
