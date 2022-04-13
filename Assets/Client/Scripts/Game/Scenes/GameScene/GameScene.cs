using Client.FrameWork.Editors;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Containers.Managers;
using Client.Game.Controllers;
using SuperBlur;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client.Game.Scenes
{
	public class GameScene : MonoBehaviour
	{
		[Header("ゲームの設定")]
		[SerializeField, Label("ステージ番号")] public int stageID;
		[SerializeField, Label("Mobius Manager")] public MobiusManager mobiusManager;
		[SerializeField, Label("Super Blur")] public SuperBlurBase superBlur;

		//ステートマシン本体
		private StateMachine<GameScene> _stateMachine;

		[HideInInspector] public int clearScore;

		[HideInInspector] public bool pauseEnd = false;

		//キャンバスのプレハブのパス
		private const string GAME_CANVAS_PATH = "Prefabs/Canvas/Canvas";
		private const string FADE_CANVAS_PATH = "Prefabs/Canvas/FadeCanvas";
		private const string USER_CONTROLLER_PATH = "Prefabs/Canvas/UserController";
		private const string CAMERA_POS_PATH = "Prefabs/CameraPos";
		private const string CENTER_EFFECT_PATH = "Prefabs/Center_Effect";
		private const string CLEAR_EFFECT_PATH = "Prefabs/Effect/ScifiTris";
		private const string PPS_PATH = "Prefabs/PPS";

		//メインのキャンバス
		[HideInInspector] public GameCanvasContainer gameCanvasContainer;
		[HideInInspector] public Fade screenFade;
		[HideInInspector] public UserController userController;

		//カメラのポジション
		[HideInInspector] public CameraPosContainer cameraPosContainer;

		//クリアエフェクト
		[HideInInspector] public ParticleSystem clearParticleSystem;

		//ズームするカメラの場所
		[HideInInspector] public Vector3 gameOverZoomPos;

		//ポストプロセスエフェクトのコンテナー
		[HideInInspector] public PPSContainer ppsContainer;


		void Start()
		{
			//FPSを60に設定
			Application.targetFrameRate = 60;

			//ステートはMonoBehaviourを継承して無いのでここでプレハブ作成
			GameObject canvas = Instantiate((GameObject)Resources.Load(GAME_CANVAS_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			canvas.transform.SetParent(transform);

			GameObject fadeCanvas = Instantiate((GameObject)Resources.Load(FADE_CANVAS_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			fadeCanvas.transform.SetParent(transform);

			GameObject user = Instantiate((GameObject)Resources.Load(USER_CONTROLLER_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			user.transform.SetParent(transform);

			GameObject cameraPos = Instantiate((GameObject)Resources.Load(CAMERA_POS_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			cameraPos.transform.SetParent(transform);

			GameObject centerEffect = Instantiate((GameObject)Resources.Load(CENTER_EFFECT_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			centerEffect.transform.SetParent(transform);

			GameObject clearEffect = Instantiate((GameObject)Resources.Load(CLEAR_EFFECT_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			clearEffect.transform.SetParent(transform);

			GameObject pps = Instantiate((GameObject)Resources.Load(PPS_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			pps.transform.SetParent(transform);

			//コンテナーを取得
			gameCanvasContainer = canvas.GetComponent<GameCanvasContainer>();
			screenFade = fadeCanvas.GetComponent<Fade>();
			userController = user.GetComponent<UserController>();
			cameraPosContainer = cameraPos.GetComponent<CameraPosContainer>();
			clearParticleSystem = clearEffect.GetComponent<ParticleSystem>();
			ppsContainer = pps.GetComponent<PPSContainer>();

			//ステートマシンの初期化をする
			_stateMachine = new StateMachine<GameScene>();
			_stateMachine.Initalize(new GameInit(), this);
		}

		void Update()
		{
			//ステートマシンの更新をする
			_stateMachine.Update(this);
		}

		public void CameraBlurActive()
		{
			superBlur.interpolation = 1.0f;
			superBlur.iterations = 3;
		}

		public void Click_Continue()
		{
			gameCanvasContainer.dialog_PauseMenu.SetRange(0.5f);
			//ダイアログを消す
			gameCanvasContainer.dialog_PauseMenu.FadeIn(0.7f, () =>
			{
				gameCanvasContainer.dialog_PauseMenu.gameObject.SetActive(false);

				pauseEnd = true; //オプションを終了させる
			});
		}

		public void Click_Retry()
		{
			GlobalSettingController.Instance.SetStageID(stageID, (stageID >= 31));
			screenFade.FadeIn(1, () =>
			{
				SceneManager.LoadScene("LoadingScene");
			});
		}

		public void Click_StageSelect()
		{
			screenFade.FadeIn(1, () =>
			{
				//プレイしてたのがEXステージならEXステージのステージセレクトへ飛ばす
				if (stageID >= 31) {
					SceneManager.LoadScene("EXStageSelectScene");
				} else {
					SceneManager.LoadScene("StageSelectScene");
				}
			});
		}

		public void Click_Next()
		{
			GlobalSettingController.Instance.SetStageID(stageID + 1, false);
			screenFade.FadeIn(1, () =>
			{
				SceneManager.LoadScene("LoadingScene");
			});
		}

		public void Click_Extra_OK()
		{
			//ダイアログを停止させる
			gameCanvasContainer.menu_ExtraOpen.menuActive = false;

			gameCanvasContainer.dialog_ExtraOpen.SetRange(0.5f);
			//ダイアログを消す
			gameCanvasContainer.dialog_ExtraOpen.FadeIn(0.4f, () =>
			{
				gameCanvasContainer.dialog_ExtraOpen.gameObject.SetActive(false);

				//リザルトを出す

				gameCanvasContainer.dialog_ResultMenu.SetRange(1.0f);

				//メニューダイアログを表示
				gameCanvasContainer.dialog_ResultMenu.gameObject.SetActive(true);

				gameCanvasContainer.dialog_ResultMenu.FadeOut(0.4f, () =>
				{
					gameCanvasContainer.menu_ResultMenu.menuActive = true;
				});
			});
		}

		public void OpenExtraDialog() {
			//ダイアログを停止させる
			gameCanvasContainer.menu_ResultMenu.menuActive = false;

			gameCanvasContainer.dialog_ResultMenu.SetRange(0.5f);
			//ダイアログを消す
			gameCanvasContainer.dialog_ResultMenu.FadeIn(0.4f, () =>
			{
				gameCanvasContainer.dialog_ResultMenu.gameObject.SetActive(false);

				//リザルトを出す

				gameCanvasContainer.dialog_ExtraOpen.SetRange(1.0f);

				//メニューダイアログを表示
				gameCanvasContainer.dialog_ExtraOpen.gameObject.SetActive(true);

				gameCanvasContainer.dialog_ExtraOpen.FadeOut(0.4f, () =>
				{
					gameCanvasContainer.menu_ExtraOpen.menuActive = true;
				});
			});
		}
	}
}