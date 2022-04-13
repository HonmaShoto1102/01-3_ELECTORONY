using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	//タイトルメニューの時のステート
	public class TitleMenu : State<TitleScene>
	{

		//このステートに入った時に呼ばれる
		public override void Enter(TitleScene obj)
		{
			obj.titleMenu.SetActive(true);
			obj.titleMenu.menuActive = true;
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(TitleScene obj)
		{
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<TitleScene> Update(TitleScene obj)
		{
			obj.titleMenu.OnUpdate();
			obj.menu_PlayTutorial.OnUpdate();
			obj.menu_saveDataFound.OnUpdate();
			obj.menu_NotSaveData.OnUpdate();
			obj.menu_Setting.OnUpdate();
			obj.menu_Exit.OnUpdate();

			////★デバッグ用 セーブデータを消す
			//if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Y_Button)) {
			//	SoundManager.Instance.PlaySE("UI_Enter");
			//	PlayerPrefs.DeleteAll();
			//}
			return this;
		}
	}
}