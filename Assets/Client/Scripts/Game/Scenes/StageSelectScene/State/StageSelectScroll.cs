using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;
using static Client.Game.Containers.MenuContainer;

namespace Client.Game.Scenes
{
	//待機中のステート
	public class StageSelectScroll : State<StageSelectScene>
	{
		private int _scrollCount;

		private const int SCROLL_SIZE = 1920;

		//このステートに入った時に呼ばれる
		public override void Enter(StageSelectScene obj)
		{
			//初期化をする
			_scrollCount = 0;
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(StageSelectScene obj)
		{
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<StageSelectScene> Update(StageSelectScene obj)
		{

			int add = obj.areaScrollSpeed;

			//スクロールの最大値を超えそうならそれ以下になるようにする
			if (_scrollCount + add > SCROLL_SIZE) {
				add = SCROLL_SIZE - _scrollCount;
			}

			_scrollCount += add;

			for (int i = 0; i < obj.areaList.Count; i++) {
				Vector3 pos = obj.areaList[i].transform.position;

				if (obj.addAreaScroll) {
					//右にスクロールする(Xを引く)
					pos.x -= add;
				} else {
					//左にスクロールする(Xを足す)
					pos.x += add;
				}

				obj.areaList[i].transform.position = pos;
			}

			if (_scrollCount >= SCROLL_SIZE) {
				foreach (var container in ParticleController.Instance.GetParticleAll(obj.nowAreaID)) {
					container.AllShow();
				}
				return new StageSelectIdle();
			}

			return this;
		}
	}
}