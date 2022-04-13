using Client.FrameWork.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Client.Game.Containers
{
	//ポストプロセスエフェクトを管理するコンテナー
	public class PPSContainer : MonoBehaviour
	{
		[SerializeField, Label("Post Process")] private PostProcessVolume postProcessVolume;
		[SerializeField, Label("Normal Color")] private Color normalEffectColor;
		[SerializeField, Label("Denger Color")] private Color dengerEffectColor;

		private PostProcessProfile _postProcessProfile;

		private bool _nowDenger = false;

		private void Start()
		{
			_postProcessProfile = postProcessVolume.profile;
		}

		public void SetColor(bool isDenger)
		{
			//同じ状態なら何もしなくてもいいので終了
			if (_nowDenger == isDenger) return;

			_nowDenger = isDenger;

			if (_nowDenger) {
				Vignette vignette = _postProcessProfile.GetSetting<Vignette>();
				vignette.color.Override(dengerEffectColor);
			} else {
				Vignette vignette = _postProcessProfile.GetSetting<Vignette>();
				vignette.color.Override(normalEffectColor);
			}
		}
	}
}