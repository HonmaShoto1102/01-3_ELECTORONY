using Client.FrameWork.Core;
using Client.FrameWork.State;
using UnityEngine;

namespace Client.Game.Scenes
{
	//最初のPress Any Key の時のステート
	public class TitleWaitAnyKey : State<TitleScene>
	{
		private const float FADE_SPEED = 0.02f;

		private float _fadeCount = 1.0f;
		private bool _fadeIn = true;

		//このステートに入った時に呼ばれる
		public override void Enter(TitleScene obj)
		{

			_fadeCount = 1.0f;
			_fadeIn = true;
		}

		//このステートから抜けるときに呼ばれる
		public override void Exit(TitleScene obj)
		{
			obj.pressAnyKey.gameObject.SetActive(false);
		}

		//このステートにいる間呼ばれる
		//(次のステートをreturnする このステートを続けるならreturn this)
		public override State<TitleScene> Update(TitleScene obj)
		{
			//何かキーが押された場合(マウスのクリックは除外する)
			if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) {
				SoundManager.Instance.PlaySE("TitleStart");
				return new TitleMenu();
			}

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

			obj.pressAnyKey.color = new Color(1.0f, 1.0f, 1.0f, _fadeCount);
			obj._gradationContainer.SetAlpha(_fadeCount);
			return this;
		}
	}
}