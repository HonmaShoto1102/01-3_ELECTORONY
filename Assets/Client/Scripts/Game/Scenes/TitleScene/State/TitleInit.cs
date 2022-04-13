using Client.FrameWork.Core;
using Client.FrameWork.State;
using Client.Game.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Client.Game.Scenes
{
	//タイトルシーンの初期化
	public class TitleInit : State<TitleScene>
	{
		//このステートに入った時に呼ばれる
		public override void Enter(TitleScene obj)
		{
			//初期化をする
			obj.titleMenu.Initialize();

			obj.pressAnyKey.gameObject.SetActive(true);
			obj.titleMenu.gameObject.SetActive(false);

			//FadeUIオブジェクトの子にあるMenuContainerを探して登録する
			foreach (Transform child in obj.dialog_PlayTutorial.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_PlayTutorial = menu;
					break;
				}
			}

			foreach (Transform child in obj.dialog_SaveDataFound.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_saveDataFound = menu;
					break;
				}
			}

			foreach (Transform child in obj.dialog_NotSaveData.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_NotSaveData = menu;
					break;
				}
			}

			foreach (Transform child in obj.dialog_Setting.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_Setting = menu;
					break;
				}
			}

			foreach (Transform child in obj.dialog_Exit.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					obj.menu_Exit = menu;
					break;
				}
			}

			obj.dialog_PlayTutorial.gameObject.SetActive(false);
			obj.dialog_SaveDataFound.gameObject.SetActive(false);
			obj.dialog_NotSaveData.gameObject.SetActive(false);
			obj.dialog_Setting.gameObject.SetActive(false);
			obj.dialog_Exit.gameObject.SetActive(false);

			obj.menu_PlayTutorial.Initialize();
			obj.menu_saveDataFound.Initialize();
			obj.menu_NotSaveData.Initialize();
			obj.menu_Setting.Initialize();
			obj.menu_Exit.Initialize();

			obj.menu_PlayTutorial.menuActive = false;
			obj.menu_saveDataFound.menuActive = false;
			obj.menu_NotSaveData.menuActive = false;
			obj.menu_Setting.menuActive = false;
			obj.menu_Exit.menuActive = false;
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(TitleScene obj)
		{
			obj.fade.SetRange(1);
			obj.fade.FadeOut(1);

			//タイトルテーマを再生
			SoundManager.Instance.StopBGM();
			SoundManager.Instance.PlayBGM("TitleTheme");
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<TitleScene> Update(TitleScene obj)
		{
			return new TitleWaitAnyKey();
		}
	}
}
