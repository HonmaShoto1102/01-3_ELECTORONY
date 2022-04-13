using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Game.Scenes
{
	//初期化をするステート
	public class LoadingIdle : State<LoadingScene>
	{
		private bool oneAction = false;

		//このステートに入った時に呼ばれる
		public override void Enter(LoadingScene obj)
		{
			obj.tipsContainer.ChangeContinue();
			oneAction = false;
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(LoadingScene obj)
		{
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<LoadingScene> Update(LoadingScene obj)
		{
			obj.tipsContainer.UpdateContinue();

			if (oneAction) return this;

			if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.A_Button)) {
				SoundManager.Instance.PlaySE("UI_Enter");
				oneAction = true;
				obj.screenFade.FadeIn(1, () =>
				{
					SceneManager.LoadScene(GlobalSettingController.Instance.GetNextStagePath());
				});
			}

			return this;
		}
	}
}
