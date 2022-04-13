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
	public class TutorialScene : MonoBehaviour
	{
		[Header("ゲームの設定")]
		[SerializeField, Label("Mobius Manager")] public MobiusManager mobiusManager;
		[SerializeField, Label("Dialog TutorialEnd")] public Fade dialog_TutorialEnd;
		[SerializeField, Label("Super Blur")] public SuperBlurBase superBlur;
		[SerializeField, Label("Message Text")] public Text messageText;
		[SerializeField, Label("Message BG")] public Image messageBG;
		[SerializeField, Label("Message A_Button")] public Image message_AButton;
		[SerializeField, Label("Arrow Image")] public Image arrow;
		[SerializeField, Label("Arrow Image2")] public Image arrow2;

		//ステートマシン本体
		private StateMachine<TutorialScene> _stateMachine;

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

		[HideInInspector] public MenuContainer menu_TutorialEnd;
		[HideInInspector] public bool optionEnd;

		//カメラのポジション
		[HideInInspector] public CameraPosContainer cameraPosContainer;

		//クリアエフェクト
		[HideInInspector] public ParticleSystem clearParticleSystem;

		//ズームするカメラの場所
		[HideInInspector] public Vector3 gameOverZoomPos;

		//テキストグラデーション
		[HideInInspector] public GradationContainer gradationContainer;

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
			_stateMachine = new StateMachine<TutorialScene>();
			_stateMachine.Initalize(new TutorialInit(), this);
		}

		void Update()
		{
			//ステートマシンの更新をする
			_stateMachine.Update(this);
		}

		public void CameraBlurActive()
		{
			if (superBlur.interpolation < 1.0f) {
				superBlur.interpolation += 0.05f;
			}
			if (superBlur.iterations < 4) {
				superBlur.iterations++;
			}
		}

		public void Click_TutorialEnd_OK()
		{
			screenFade.FadeIn(1, () =>
			{
				SceneManager.LoadScene("StageSelectScene");
			});
		}

		public void Click_TutorialEnd_Cancel()
		{
			dialog_TutorialEnd.SetRange(0.5f);
			//ダイアログを消す
			dialog_TutorialEnd.FadeIn(0.7f, () =>
			{
				dialog_TutorialEnd.gameObject.SetActive(false);

				optionEnd = true; //オプションを終了させる
			});
		}
	}
}