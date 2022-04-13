using Client.FrameWork.Core;
using Client.Game.Containers;
using Client.Game.Containers.Managers;
using Client.Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Controllers
{
	//輪の管理を一括で行うコントローラー
	public class MobiusController : SceneSingleton<MobiusController>
	{
		//現在選択されているメビウスの輪のパーツリスト
		[HideInInspector] public List<MobiusData> selectMobiusList = new List<MobiusData>();

		//溜めねじりのカウント
		[HideInInspector] public int chargeCount = 0;

		//MobiusIDとMobiusContainerを紐づける辞書
		private Dictionary<int, MobiusContainer> _mobiusDict = new Dictionary<int, MobiusContainer>();

		//マネージャー
		[HideInInspector] public MobiusManager manager;

		//辞書にデータを追加する
		public void AddDict(int id, MobiusContainer container)
		{
			_mobiusDict.Add(id, container);
		}

		public int GetMobiusCount() {
			return _mobiusDict.Count;
		}


		//辞書から一致するIDのContainerを返す 無いならnull
		public MobiusContainer GetMobiusContainer(int id)
		{
			if (_mobiusDict.ContainsKey(id)) {
				return _mobiusDict[id];
			}
			return null;
		}

		public void SelectMobiusAllClear()
		{
			if (selectMobiusList.Count == 0) return;

			for (int i = 0; i < selectMobiusList.Count; i++) {
				selectMobiusList[i].isChange = true;
			}
			selectMobiusList.Clear();
		}
	}
}
