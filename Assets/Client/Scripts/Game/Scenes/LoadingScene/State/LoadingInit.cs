using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//初期化をするステート
	public class LoadingInit : State<LoadingScene>
	{
		//このステートに入った時に呼ばれる
		public override void Enter(LoadingScene obj)
		{
			obj.tipsContainer.Initialize();
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(LoadingScene obj)
		{
			obj.screenFade.SetRange(1);
			obj.screenFade.FadeOut(1);

			SoundManager.Instance.StopBGM();
			SoundManager.Instance.PlayBGM("LoadingTheme");
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<LoadingScene> Update(LoadingScene obj)
		{
			return new LoadingLoad();
		}
	}
}
