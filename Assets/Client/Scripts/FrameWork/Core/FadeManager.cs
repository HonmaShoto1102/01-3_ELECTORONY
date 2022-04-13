using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Client.FrameWork.Core
{
	//フェードを管理する
	public class FadeManager : GameSingleton<FadeManager>
	{
		//フェード中の透明度
		private float fadeAlpha = 0;
		//フェード中かどうか
		[HideInInspector] public bool isFading = false;
		//フェード色
		[HideInInspector] public Color fadeColor = Color.black;
		//実行中のコルーチン
		private IEnumerator nowCorutine = null;

		public void OnGUI()
		{
			if (isFading) {
				//色と透明度を更新してテクスチャを描画
				fadeColor.a = fadeAlpha;
				GUI.color = fadeColor;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
			}
		}

		public void FadeCancel()
		{
			if (nowCorutine == null) return; //フェードが無いなら無視

			StopCoroutine(nowCorutine);

			nowCorutine = null;
		}

		public void FadeIn(float interval)
		{
			if (nowCorutine != null) return; //他のフェードが起動中なら処理しない

			nowCorutine = FadeInScene(interval);
			StartCoroutine(nowCorutine);
		}


		public void FadeOut(string scene, float interval)
		{
			if (nowCorutine != null) return; //他のフェードが起動中なら処理しない

			nowCorutine = FadeOutScene(scene, interval);
			StartCoroutine(nowCorutine);
		}

		private IEnumerator FadeInScene(float interval)
		{
			isFading = true;
			float time = 0;

			//だんだん明るくなる
			time = 0;
			while (time <= interval) {
				fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
				time += Time.deltaTime;
				yield return 0;
			}

			isFading = false;
			nowCorutine = null;
		}

		private IEnumerator FadeOutScene(string scene, float interval)
		{
			//だんだん暗くなる
			isFading = true;
			float time = 0;
			while (time <= interval) {
				fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
				time += Time.deltaTime;
				yield return 0;
			}

			//シーン切替
			SceneManager.LoadScene(scene);

			time = 0;
			while (time <= interval) {
				fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
				time += Time.deltaTime;
				yield return 0;
			}
			isFading = false;
			nowCorutine = null;
		}
	}
}