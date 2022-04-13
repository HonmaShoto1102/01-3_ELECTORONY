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
	public class StageSelectScene : MonoBehaviour
	{
		[Header("フェード系")]
		[SerializeField, Label("Fade Image")] public Fade fade;
		[SerializeField, Label("Dialog OptionMenu")] public Fade dialog_OptionMenu;
		[SerializeField, Label("Dialog SettingMenu")] public Fade dialog_SettingMenu;

		[Header("画像系")]
		[SerializeField, Label("LB Image")] public Image lbImage;
		[SerializeField, Label("RB Image")] public Image rbImage;
		[SerializeField, Label("LB Text")] public Text lbText;
		[SerializeField, Label("RB Text")] public Text rbText;

		[Header("データ系")]
		[SerializeField, Label("Stage Window")] public StageWindowContainer stageWindow;

		[Header("設定")]
		[SerializeField, Label("エリア一覧")] public List<RectTransform> areaList;
		[SerializeField, Label("選択中のステージ")] public StageSelectContainer nowSelectStage;
		[SerializeField, Label("スクロール速度")] public int scrollSpeed;
		[SerializeField, Label("エリアスクロール速度")] public int areaScrollSpeed;

		[HideInInspector] public int nowAreaID;
		[HideInInspector] public bool addAreaScroll;

		[HideInInspector] public MenuContainer menu_OptionMenu;
		[HideInInspector] public MenuContainer menu_SettingMenu;

		[HideInInspector] public bool optionEnd = false;

		[HideInInspector] public bool buttonEnd = true;

		private GradationContainer lbGradation;
		private GradationContainer rbGradation;

		//ステートマシン本体
		private StateMachine<StageSelectScene> _stateMachine;

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

			lbGradation = lbText.GetComponent<GradationContainer>();
			rbGradation = rbText.GetComponent<GradationContainer>();

			//ステートマシンの初期化をする
			_stateMachine = new StateMachine<StageSelectScene>();
			_stateMachine.Initalize(new StageSelectInit(), this);

			nowAreaID = 0;
		}

		void Update()
		{
			//ステートマシンの更新をする
			_stateMachine.Update(this);
		}

		public void UpdateWindow()
		{
			stageWindow.SetText(nowAreaID, GlobalSettingController.Instance.GetStageData().Sheet1[nowSelectStage.stageID - 1]);
			stageWindow.SetStar(nowSelectStage.stageID);
		}

		public void LBRBIconUpdate()
		{
			//これ以上左が無いならアイコンを半透明にする
			Color lbColor = lbImage.color;
			Color lbTextColor = lbText.color;
			if (nowAreaID == 0) {
				lbColor.a = 0.3f;
				lbTextColor.a = 0.3f;
				lbGradation.SetAlpha(0.3f);
			} else {
				lbColor.a = 1.0f;
				lbTextColor.a = 1.0f;
				lbGradation.SetAlpha(1.0f);
			}
			lbImage.color = lbColor;
			lbText.color = lbTextColor;

			//これ以上右が無い or 現在のエリアの一番右のステージをクリアしてないならアイコンを半透明にする
			Color rbColor = rbImage.color;
			Color rbTextColor = rbText.color;
			if (nowAreaID == areaList.Count - 1) {
				rbColor.a = 0.3f;
				rbTextColor.a = 0.3f;
				rbGradation.SetAlpha(0.3f);
			} else if (UserSaveData.Instance.GetStar((nowAreaID + 1) * 5) == 0) {
				rbColor.a = 0.3f;
				rbTextColor.a = 0.3f;
				rbGradation.SetAlpha(0.3f);
			} else {
				rbColor.a = 1.0f;
				rbTextColor.a = 1.0f;
				rbGradation.SetAlpha(1.0f);
			}
			rbImage.color = rbColor;
			rbText.color = rbTextColor;
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