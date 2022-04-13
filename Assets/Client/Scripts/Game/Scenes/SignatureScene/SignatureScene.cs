using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using UnityEngine;
using UnityEngine.Video;

namespace Client.Game.Scenes
{
	class SignatureScene : MonoBehaviour
	{

		[SerializeField, Label("再生する動画")] private VideoPlayer video;
		private bool _isFading = false;

		//動画の再生が終わったら呼び出される
		private void _VideoEnd(VideoPlayer video)
		{
			_FadeStart();
		}

		private void _FadeStart()
		{
			if (_isFading) return; //既にフェード中なら無視

			_isFading = true;
			FadeManager.Instance.FadeCancel();
			FadeManager.Instance.FadeOut("TitleScene", 1.0f);
		}

		void Start()
		{
			//FPSを60に設定
			Application.targetFrameRate = 60;

			//フェードインさせる
			FadeManager.Instance.FadeIn(3.0f);

			//動画の再生が終わった時に呼び出す関数をセット
			video.loopPointReached += _VideoEnd;

			//ロゴサウンドのSE再生
			SoundManager.Instance.PlaySE("LogoSound");
		}

		void Update()
		{
			//何かキーが押された場合(マウスのクリックは除外する)
			if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) {
				_FadeStart();
				SoundManager.Instance.AllStopSE(); //SEも止める
			}

		}
	}
}