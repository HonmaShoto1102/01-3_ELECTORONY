using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using Client.Game.Controllers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Client.Game.Containers
{
	//UIのボタンを構成するコンテナー
	public class ButtonContainer : SystemBehaviour
	{
		//効果音リスト
		public enum SelectSE
		{
			SE_Enter,  //決定音
			SE_Cancel, //キャンセル音
			SE_None,   //音無し
		}

		public enum SliderType
		{
			Slider_BGM,
			Slider_SE,
		}

		public enum ButtonGraphic
		{
			Button_Text,
			Button_Image,
		}

		[Header("共通設定")]
		[SerializeField, Label("ボタンの見た目")] private ButtonGraphic buttonGraphic;
		[SerializeField, Label("選択されたときの音")] private SelectSE selectSE;
		[SerializeField, Label("ボタンクリック後の無効化時間")] public int invicibleTime;

		[Header("テキストボタン用")]
		[SerializeField, Label("通常時の輪郭色")] private Color normalColor;
		[SerializeField, Label("選択時の輪郭色")] private Color selectColor;
		[SerializeField, Label("追加ボタン")] private Image buttonImage;

		[Header("画像ボタン用")]
		[SerializeField, Label("Image")] private Image image;
		[SerializeField, Label("通常時の画像")] private Sprite normalSprite;
		[SerializeField, Label("選択時の画像")] private Sprite selectSprite;

		[Header("スライダー用")]
		[SerializeField, Label("スライダーか")] public bool isSlider;
		[SerializeField, Label("Slider")] private Slider slider;
		[SerializeField, Label("Slider Speed")] private int sliderSpeed;
		[SerializeField, Label("Slider Type")] private SliderType sliderType;
		[SerializeField, Label("変動量")] private float addValue;

		//ボタンが選択されたときに実行する関数
		public UnityEvent clickedAction;

		//スライダーの数値
		private int _sliderCount;
		//短形トランスフォーム
		private RectTransform _rectTransform;

		//このオブジェクトについているテキストコンポーネント
		private Outline outline;
		private GradationContainer gradationContainer;
		private Text text;

		//フェードの速度
		private const float FADE_SPEED = 0.03f;

		//現在のアルファ値
		private float _fadeCount = 1.0f;
		private bool _fadeIn = true;

		public override void Initialize()
		{
			//コンポーネントを取得する
			_rectTransform = transform as RectTransform;

			outline = GetComponent<Outline>();
			gradationContainer = GetComponent<GradationContainer>();
			text = GetComponent<Text>();

			//初期画面を設定する
			switch (buttonGraphic) {
				case ButtonGraphic.Button_Image:
					image.sprite = normalSprite;
					break;
				case ButtonGraphic.Button_Text:
					outline.effectColor = normalColor;
					break;
			}

			//スライダーの場合は音量データを取得してくる
			if (isSlider) {
				switch (sliderType) {
					case SliderType.Slider_BGM:
						slider.value = UserSaveData.Instance.GetBGMVolume();
						break;
					case SliderType.Slider_SE:
						slider.value = UserSaveData.Instance.GetSEVolume();
						break;
				}
			}

			_fadeCount = 1.0f;
			_fadeIn = true;

			base.Initialize();
		}

		public void OnUpdate()
		{
			//選択中の項目をフェードさせる
			if (_fadeIn) {
				_fadeCount -= FADE_SPEED;
				if (_fadeCount < 0.0f) {
					_fadeCount = 0.0f;
					_fadeIn = false;
				}
			} else {
				_fadeCount += FADE_SPEED;
				if (_fadeCount > 1.0f) {
					_fadeCount = 1.0f;
					_fadeIn = true;
				}
			}

			Color color;
			switch (buttonGraphic) {
				case ButtonGraphic.Button_Image:
					color = image.color;
					color.a = _fadeCount;
					image.color = color;
					break;
				case ButtonGraphic.Button_Text:

					color = text.color;
					color.a = _fadeCount;
					text.color = color;

					gradationContainer.SetAlpha(_fadeCount);
					break;
			}
		}

		public void changeSprite(bool select)
		{
			Color color;
			switch (buttonGraphic) {
				case ButtonGraphic.Button_Image:
					if (select) {
						image.sprite = selectSprite;
						if (buttonImage != null) {
							buttonImage.gameObject.SetActive(false);
						}
					} else {
						image.sprite = normalSprite;
						color = image.color;
						color.a = 1.0f;
						image.color = color;
					}
					break;
				case ButtonGraphic.Button_Text:
					if (select) {
						outline.effectColor = selectColor;

						//追加画像の位置を計算する
						Vector3 pos = _rectTransform.anchoredPosition3D;
						pos.x -= _rectTransform.rect.width / 2.0f;
						pos.x -= buttonImage.rectTransform.rect.width / 2.0f;
						buttonImage.rectTransform.anchoredPosition3D = pos;
						buttonImage.gameObject.SetActive(true);
					} else {
						outline.effectColor = normalColor;

						color = text.color;
						color.a = 1.0f;
						text.color = color;

						gradationContainer.SetAlpha(1.0f);
					}
					break;
			}
		}

		public void OnAction()
		{
			//設定されたSEを再生
			switch (selectSE) {
				case SelectSE.SE_Enter:
					SoundManager.Instance.PlaySE("UI_Enter");
					break;
				case SelectSE.SE_Cancel:
					SoundManager.Instance.PlaySE("UI_Cancel");
					break;
				case SelectSE.SE_None:
					break;
			}
			clickedAction.Invoke();
		}

		public void OnSliderUpdate()
		{
			if (_sliderCount > 0) {
				_sliderCount--;
				return;
			}

			switch (sliderType) {
				case SliderType.Slider_BGM:
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Left)) {
						slider.value -= addValue;
						SoundManager.Instance.SetBGMVolume(slider.value);
						_sliderCount = sliderSpeed;
					}
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Right)) {
						slider.value += addValue;
						SoundManager.Instance.SetBGMVolume(slider.value);
						_sliderCount = sliderSpeed;
					}
					break;
				case SliderType.Slider_SE:
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Left)) {
						slider.value -= addValue;
						SoundManager.Instance.SetSEVolume(slider.value);
						SoundManager.Instance.PlaySE("UI_Enter");
						_sliderCount = sliderSpeed;
					}
					if (GlobalButtonInput.Instance.isButtonDown(GlobalButtonInput.ButtonAction.Right)) {
						slider.value += addValue;
						SoundManager.Instance.SetSEVolume(slider.value);
						SoundManager.Instance.PlaySE("UI_Enter");
						_sliderCount = sliderSpeed;
					}
					break;
			}
		}

		public void SettingDataSave()
		{
			UserSaveData.Instance.SetBGMVolume(SoundManager.Instance.GetBGMVolume());
			UserSaveData.Instance.SetSEVolume(SoundManager.Instance.GetSEVolume());
		}
	}
}