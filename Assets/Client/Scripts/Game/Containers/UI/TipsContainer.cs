using Client.FrameWork.Editors;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Game.Containers
{
	//Tips用のコンテナー
	public class TipsContainer : MonoBehaviour
	{
		[Header("Loading")]
		[SerializeField, Label("Loading Image")] GameObject loadingImage;
		[SerializeField, Label("Loading Text")] Text loadingText;
		[Header("Continue")]
		[SerializeField, Label("Continue Image")] Image continueImage;
		[SerializeField, Label("Continue Text")] Text continueText;

		//フェードの速度
		private const float FADE_SPEED = 0.02f;

		//現在のアルファ値
		private float _fadeCount = 1.0f;
		private bool _fadeIn = true;

		private GradationContainer gradationContainer;

		public void Initialize()
		{
			loadingImage.gameObject.SetActive(true);
			loadingText.gameObject.SetActive(true);

			continueImage.gameObject.SetActive(false);
			continueText.gameObject.SetActive(false);

			gradationContainer = continueText.GetComponent<GradationContainer>();
		}

		public void ChangeContinue()
		{
			loadingImage.gameObject.SetActive(false);
			loadingText.gameObject.SetActive(false);

			continueImage.gameObject.SetActive(true);
			continueText.gameObject.SetActive(true);
		}

		public void UpdateLoading()
		{
			//loadingImage.transform.Rotate(0, 0, -2.0f);
		}

		public void UpdateContinue()
		{
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

			Color color = continueText.color;
			color.a = _fadeCount;
			continueText.color = color;

			gradationContainer.SetAlpha(_fadeCount);

			color = continueImage.color;
			color.a = _fadeCount;
			continueImage.color = color;
		}
	}
}
