using Client.FrameWork.Editors;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client.Game.Scenes
{
	public class TitleScene : MonoBehaviour
	{
		[SerializeField, Label("Press Any Key")] public Text pressAnyKey;
		[SerializeField, Label("タイトルメニュー")] public MenuContainer titleMenu;
		[SerializeField, Label("Fade Image")] public Fade fade;

		[Header("ダイアログの設定")]
		[SerializeField, Label("Dialog PlayTutorial")] public Fade dialog_PlayTutorial;
		[SerializeField, Label("Dialog SaveDataFound")] public Fade dialog_SaveDataFound;
		[SerializeField, Label("Dialog NotSaveData")] public Fade dialog_NotSaveData;
		[SerializeField, Label("Dialog Setting")] public Fade dialog_Setting;
		[SerializeField, Label("Dialog Exit")] public Fade dialog_Exit;

		[HideInInspector] public MenuContainer menu_PlayTutorial;
		[HideInInspector] public MenuContainer menu_saveDataFound;
		[HideInInspector] public MenuContainer menu_NotSaveData;
		[HideInInspector] public MenuContainer menu_Setting;
		[HideInInspector] public MenuContainer menu_Exit;

		[HideInInspector] public GradationContainer _gradationContainer;

		//ステートマシン本体
		private StateMachine<TitleScene> _stateMachine;

		void Start()
		{
			//FPSを60に設定
			Application.targetFrameRate = 60;

			_gradationContainer = pressAnyKey.GetComponent<GradationContainer>();

			//ステートマシンの初期化をする
			_stateMachine = new StateMachine<TitleScene>();
			_stateMachine.Initalize(new TitleInit(), this);
		}

		void Update()
		{
			//ステートマシンの更新をする
			_stateMachine.Update(this);
		}

		public void Fade_StageSelect()
		{
			fade.FadeIn(1, () =>
			{
				SceneManager.LoadScene("StageSelectScene");
			});
		}

		public void Click_NewGame()
		{
			if (UserSaveData.Instance.isInitialized()) {
				//既にセーブデータが存在するので初期化していいかを尋ねる
				dialog_SaveDataFound.gameObject.SetActive(true);

				//ダイアログが出ている間はメインメニューを無効にする
				titleMenu.menuActive = false;

				//ダイアログを表示する
				dialog_SaveDataFound.FadeOut(0.7f, () =>
				{
					menu_saveDataFound.menuActive = true;
				});
			} else {
				//チュートリアルをプレイするかを尋ねる
				dialog_PlayTutorial.gameObject.SetActive(true);

				//ダイアログが出ている間はメインメニューを無効にする
				titleMenu.menuActive = false;

				//ダイアログを表示する
				dialog_PlayTutorial.FadeOut(0.7f, () =>
				{
					menu_PlayTutorial.menuActive = true;
				});
			}
		}

		public void Click_LoadGame()
		{
			if (UserSaveData.Instance.isInitialized()) {
				//セーブデータをロードしてゲーム開始
				Fade_StageSelect();
			} else {
				//セーブデータが無いのでエラーダイアログを返す
				dialog_NotSaveData.gameObject.SetActive(true);

				//ダイアログが出ている間はメインメニューを無効にする
				titleMenu.menuActive = false;

				//ダイアログを表示する
				dialog_NotSaveData.FadeOut(0.7f, () =>
				{
					menu_NotSaveData.menuActive = true;
					titleMenu.invicibleCount = 0;
				});
			}
		}

		public void Click_Setting()
		{
			menu_Setting.SetSelect(0);
			dialog_Setting.gameObject.SetActive(true);

			//ダイアログが出ている間はメインメニューを無効にする
			titleMenu.menuActive = false;

			//ダイアログを表示する
			dialog_Setting.FadeOut(0.7f, () =>
			{
				menu_Setting.menuActive = true;
			});
		}

		public void Click_Exit()
		{
			dialog_Exit.gameObject.SetActive(true);

			//ダイアログが出ている間はメインメニューを無効にする
			titleMenu.menuActive = false;

			//ダイアログを表示する
			dialog_Exit.FadeOut(0.7f, () =>
			{
				menu_Exit.menuActive = true;
			});
		}

		public void Click_PlayTutorial_YES()
		{
			//チュートリアルをプレイさせる
			UserSaveData.Instance.SaveDataInitialize();
			GlobalSettingController.Instance.tutorialPlayable = false;
			fade.FadeIn(1, () =>
			{
				SceneManager.LoadScene("TutorialScene");
			});
		}

		public void Click_PlayTutorial_NO()
		{
			//データを初期化してゲーム開始
			UserSaveData.Instance.SaveDataInitialize();
			Fade_StageSelect();
		}

		public void Click_SaveDataFound_OK()
		{
			//セーブデータを初期化してゲーム開始
			UserSaveData.Instance.SaveDataInitialize();

			menu_saveDataFound.menuActive = false;
			dialog_SaveDataFound.SetRange(0.5f);
			//ダイアログを消す
			dialog_SaveDataFound.FadeIn(0.4f, () =>
			{
				dialog_SaveDataFound.gameObject.SetActive(false);
				//チュートリアルをプレイするかを尋ねる
				dialog_PlayTutorial.gameObject.SetActive(true);

				//ダイアログを表示する
				dialog_PlayTutorial.FadeOut(0.4f, () =>
				{
					menu_PlayTutorial.menuActive = true;
				});
			});
		}

		public void Click_SaveDataFound_Cancel()
		{
			menu_saveDataFound.menuActive = false;
			dialog_SaveDataFound.SetRange(0.5f);
			//ダイアログを消す
			dialog_SaveDataFound.FadeIn(0.7f, () =>
			{
				dialog_SaveDataFound.gameObject.SetActive(false);

				//メインメニューを有効化する
				titleMenu.menuActive = true;
			});
		}

		public void Click_NotSaveData_OK()
		{
			menu_NotSaveData.menuActive = false;
			dialog_NotSaveData.SetRange(0.5f);
			//ダイアログを消す
			dialog_NotSaveData.FadeIn(0.7f, () =>
			{
				dialog_NotSaveData.gameObject.SetActive(false);

				//メインメニューを有効化する
				titleMenu.menuActive = true;
			});
		}

		public void Click_Setting_OK()
		{
			menu_Setting.menuActive = false;
			dialog_Setting.SetRange(0.5f);
			//ダイアログを消す
			dialog_Setting.FadeIn(0.7f, () =>
			{
				dialog_Setting.gameObject.SetActive(false);

				//メインメニューを有効化する
				titleMenu.menuActive = true;
			});
		}

		public void Click_Exit_OK()
		{
			//ゲームを終了させる(エディタ上とビルド版では処理が違う)
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
		}

		public void Click_Exit_Cancel()
		{
			menu_Exit.menuActive = false;
			dialog_Exit.SetRange(0.5f);
			//ダイアログを消す
			dialog_Exit.FadeIn(0.7f, () =>
			{
				dialog_Exit.gameObject.SetActive(false);

				//メインメニューを有効化する
				titleMenu.menuActive = true;
			});
		}
	}
}