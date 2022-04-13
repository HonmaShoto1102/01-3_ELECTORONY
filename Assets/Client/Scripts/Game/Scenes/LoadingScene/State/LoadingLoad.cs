using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//初期化をするステート
	public class LoadingLoad : State<LoadingScene>
	{
		private int _loadingTime = 0;

		//このステートに入った時に呼ばれる
		public override void Enter(LoadingScene obj)
		{
			if (obj.secondLoading) {
				_loadingTime = Random.Range(50, 130);
			} else {
				_loadingTime = Random.Range(100, 210);
			}
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(LoadingScene obj)
		{
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<LoadingScene> Update(LoadingScene obj)
		{
			obj.tipsContainer.UpdateLoading();

			if (_loadingTime > 0) {
				_loadingTime--;
				return this;
			} else {
				return new LoadingIdle();
			}
		}
	}
}
