using Client.FrameWork.Editors;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client.Game.Scenes
{
	public class EXStageSelectScene : MonoBehaviour
	{
		[Header("フェード系")]
		[SerializeField, Label("Fade Image")] public Fade fade;
		[SerializeField, Label("Dialog OptionMenu")] public Fade dialog_OptionMenu;
		[SerializeField, Label("Dialog SettingMenu")] public Fade dialog_SettingMenu;

		[Header("データ系")]
		[SerializeField, Label("Stage Window")] public StageWindowContainer stageWindow;

		[Header("設定")]
		[SerializeField, Label("選択中のステージ")] public StageSelectContainer nowSelectStage;
		[SerializeField, Label("スクロール速度")] public int scrollSpeed;

		[HideInInspector] public MenuContainer menu_OptionMenu;
		[HideInInspector] public MenuContainer menu_SettingMenu;

		[HideInInspector] public bool optionEnd = false;

		[HideInInspector] public bool buttonEnd = true;

		[HideInInspector] public bool notEXPlay = false;

		//ステートマシン本体
		private StateMachine<EXStageSelectScene> _stateMachine;

		public float baseWidth = 9.0f;
		public float baseHeight = 16.0f;

		void Awake()
		{
			// フィット
			var scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
			var scaleRatio = Mathf.Min(scaleWidth, 1.0f);
			Camera.main.fieldOfView = Mathf.Atan(Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
		}

		void Start()
		{
			//FPSを60に設定 
			Application.targetFrameRate = 60;

			//ステートマシンの初期化をする
			_stateMachine = new StateMachine<EXStageSelectScene>();
			_stateMachine.Initalize(new EXStageSelectInit(), this);
		}

		void Update()
		{
			//ステートマシンの更新をする
			_stateMachine.Update(this);
		}

		public void UpdateWindow()
		{
			stageWindow.SetText(99, GlobalSettingController.Instance.GetStageData().Sheet1[nowSelectStage.stageID - 1]);
			stageWindow.SetStar(nowSelectStage.stageID);
		}

		public void Click_OptionMenu_Continue()
		{
			dialog_OptionMenu.SetRange(0.5f);
			//ダイアログを消す
			dialog_OptionMenu.FadeIn(0.7f, () =>
			{
				dialog_OptionMenu.gameObject.SetActive(false);

				optionEnd = true; //オプションを終了させる
			});
		}

		public void Click_OptionMenu_Setting()
		{
			//オプションメニューを停止させる
			menu_OptionMenu.menuActive = false;

			dialog_OptionMenu.SetRange(0.5f);
			//ダイアログを消す
			dialog_OptionMenu.FadeIn(0.4f, () =>
			{
				dialog_OptionMenu.gameObject.SetActive(false);

				//設定ダイアログを出す
				//項目を一番上に戻しとく
				menu_SettingMenu.SetSelect(0);

				dialog_SettingMenu.SetRange(1.0f);

				//メニューダイアログを表示
				dialog_SettingMenu.gameObject.SetActive(true);

				dialog_SettingMenu.FadeOut(0.4f, () =>
				{
					menu_SettingMenu.menuActive = true;
				});
			});
		}

		public void Click_Setting_OK()
		{
			menu_SettingMenu.menuActive = false;

			dialog_SettingMenu.SetRange(0.5f);
			//ダイアログを消す
			dialog_SettingMenu.FadeIn(0.4f, () =>
			{
				dialog_SettingMenu.gameObject.SetActive(false);

				//オプションメニューを再開させる
				//メニューダイアログを表示
				dialog_OptionMenu.gameObject.SetActive(true);

				dialog_OptionMenu.FadeOut(0.4f, () =>
				{
					menu_OptionMenu.menuActive = true;
				});
			});
		}

		public void Click_OptionMenu_Tutorial()
		{
			fade.FadeIn(1, () =>
			{
				SceneManager.LoadScene("TutorialScene");
			});
		}

		public void Click_OptionMenu_TitleMenu()
		{
			fade.FadeIn(1, () =>
			{
				SceneManager.LoadScene("TitleScene");
			});
		}
	}
}