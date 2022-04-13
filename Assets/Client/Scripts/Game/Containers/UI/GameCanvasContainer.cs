using Client.FrameWork.Editors;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Game.Containers
{
	//ゲームのメインキャンバスを管理する
	public class GameCanvasContainer : MonoBehaviour
	{
		[Header("ダイアログ系")]
		[SerializeField, Label("Dialog PauseMenu")] public Fade dialog_PauseMenu;
		[SerializeField, Label("Dialog ResultMenu")] public Fade dialog_ResultMenu;
		[SerializeField, Label("Dialog GameOverMenu")] public Fade dialog_GameOverMenu;
		[SerializeField, Label("Dialog ExtraOpen")] public Fade dialog_ExtraOpen;

		[Header("色の設定")]
		[SerializeField, Label("Not Clear Color")] public Color notClearColor;
		[SerializeField, Label("Clear Color")] public Color clearColor;
		[SerializeField, Label("Perfect Color")] public Color perfectColor;
		[SerializeField, Label("Not Clear Slider")] public Color notClearSliderColor;
		[SerializeField, Label("Clear Slider")] public Color clearSliderColor;
		[SerializeField, Label("Perfect Slider")] public Color perfectSliderColor;

		[Header("ゲームの設定")]
		[SerializeField, Label("Slider")] public Slider slider;
		[SerializeField, Label("Rect Mask 2D")] public RectMask2D rectMask2D;
		[SerializeField, Label("Slider Image")] public Image sliderImage;
		[SerializeField, Label("Slider Value")] public Image sliderValue;
		[SerializeField, Label("TotalScore Text")] public Text totalScore;
		[SerializeField, Label("Remain Image")] public Image remainImage;
		[SerializeField, Label("Remain Text")] public Text remainText;
		[SerializeField, Label("ExtraOpen Text")] public Text extraOpenText;

		[Header("非表示UI")]
		//[SerializeField, Label("Menu Image")] public Image menuImage;
		[SerializeField, Label("操作説明")] public GameObject howToPlay;

		[Header("ステージクリア")]
		[SerializeField, Label("Stage Clear Text")] public Text stageClear;
		[SerializeField, Label("Clear Slider")] public Slider clearSlider;
		[SerializeField, Label("Rect Mask 2D")] public RectMask2D clearrectMask2D;
		[SerializeField, Label("Slider Image")] public GameObject clearsliderImage;
		[SerializeField, Label("Slider Value")] public GameObject clearsliderValue;
		[SerializeField, Label("Score Text")] public Text scoreText;
		[SerializeField, Label("Star 1 Image")] public Image star_1_Image;
		[SerializeField, Label("Star 2 Image")] public Image star_2_Image;
		[SerializeField, Label("Star 3 Image")] public Image star_3_Image;
		[SerializeField, Label("Star 1 Effect")] public UIParticle particle_1;
		[SerializeField, Label("Star 2 Effect")] public UIParticle particle_2;
		[SerializeField, Label("Star 3 Effect")] public UIParticle particle_3;

		[SerializeField, Label("Next Text")] public Text nextText;
		[SerializeField, Label("Next None Text")] public Text nextNoneText;

		[Header("ゲームオーバー")]
		[SerializeField, Label("Game Over Text")] public Text gameOver;
		[SerializeField, Label("Game Over Text2")] public Text gameOver2;

		//メニューコンテナ
		[HideInInspector] public MenuContainer menu_PauseMenu;
		[HideInInspector] public MenuContainer menu_ResultMenu;
		[HideInInspector] public MenuContainer menu_GameOverMenu;
		[HideInInspector] public MenuContainer menu_ExtraOpen;

		private RectTransform _maskRect;
		private RectTransform _clarMaskRect;

		private GradationContainer _gradationContainer;

		private const string UICAMERA_PATH = "Prefabs/UICamera";

		public void Initialize()
		{
			GameObject cameraObj = Instantiate((GameObject)Resources.Load(UICAMERA_PATH), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

			Camera camera = cameraObj.GetComponent<Camera>();
			Canvas canvas = GetComponent<Canvas>();

			canvas.worldCamera = camera;

			//FadeUIオブジェクトの子にあるMenuContainerを探して登録する
			foreach (Transform child in dialog_PauseMenu.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					menu_PauseMenu = menu;
					break;
				}
			}

			foreach (Transform child in dialog_ResultMenu.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					menu_ResultMenu = menu;
					break;
				}
			}

			foreach (Transform child in dialog_GameOverMenu.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					menu_GameOverMenu = menu;
					break;
				}
			}

			foreach (Transform child in dialog_ExtraOpen.gameObject.transform) {
				var menu = child.GetComponent<MenuContainer>();
				if (menu != null) {
					menu_ExtraOpen = menu;
					break;
				}
			}

			//ダイアログの初期化
			dialog_PauseMenu.gameObject.SetActive(false);
			dialog_ResultMenu.gameObject.SetActive(false);
			dialog_GameOverMenu.gameObject.SetActive(false);
			dialog_ExtraOpen.gameObject.SetActive(false);

			menu_PauseMenu.Initialize();
			menu_ResultMenu.Initialize();
			menu_GameOverMenu.Initialize();
			menu_ExtraOpen.Initialize();

			menu_PauseMenu.menuActive = false;
			menu_ResultMenu.menuActive = false;
			menu_GameOverMenu.menuActive = false;
			menu_ExtraOpen.menuActive = false;

			//リザルトの初期化
			//テキストを透明にする
			Color color = stageClear.color;
			color.a = 0.0f;
			stageClear.color = color;

			color = gameOver.color;
			color.a = 0.0f;
			gameOver.color = color;

			color = gameOver2.color;
			color.a = 0.0f;
			gameOver2.color = color;

			clearSlider.value = 0;
			star_1_Image.gameObject.SetActive(false);
			star_2_Image.gameObject.SetActive(false);
			star_3_Image.gameObject.SetActive(false);

			particle_1.Stop();
			particle_2.Stop();
			particle_3.Stop();

			_maskRect = rectMask2D.transform as RectTransform;
			_clarMaskRect = clearrectMask2D.transform as RectTransform;

			_gradationContainer = totalScore.GetComponent<GradationContainer>();
		}

		public void SetMask2D()
		{
			float value = slider.value;
			float per = _maskRect.rect.height * (value / 100.0f);

			Vector4 padding = rectMask2D.padding;
			padding.w = _maskRect.rect.height - per;

			rectMask2D.padding = padding;
		}

		public void SetClearMask2D()
		{
			float value = clearSlider.value;
			float per = _clarMaskRect.rect.width * (value / 100.0f);

			Vector4 padding = clearrectMask2D.padding;
			padding.z = _clarMaskRect.rect.width - per;

			clearrectMask2D.padding = padding;
		}

		public void UpdateTotalScore(float percent)
		{
			if (percent >= 100.0f) {
				_gradationContainer.colorBottom = perfectColor;
				sliderImage.color = perfectSliderColor;
				sliderValue.color = perfectSliderColor;
			} else if (percent >= 45.0f) {
				_gradationContainer.colorBottom = clearColor;
				sliderImage.color = clearSliderColor;
				sliderValue.color = clearSliderColor;
			} else {
				_gradationContainer.colorBottom = notClearColor;
				sliderImage.color = notClearSliderColor;
				sliderValue.color = notClearSliderColor;
			}
		}
	}
}
