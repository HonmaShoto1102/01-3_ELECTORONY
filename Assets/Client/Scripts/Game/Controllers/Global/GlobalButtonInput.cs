using Client.FrameWork.Core;
using UnityEngine;

namespace Client.Game.Controllers
{
	//コントローラーの情報を管理する
	public class GlobalButtonInput : SceneSingleton<GlobalButtonInput>
	{
		//ボタンアクション
		public enum ButtonAction
		{
			//ボタン操作
			A_Button,
			B_Button,
			X_Button,
			Y_Button,

			//上下左右はスティックでも十字パッドでも動く
			Up,
			Down,
			Right,
			Left,

			Option,

			None,
		}

		public bool isButtonDown(ButtonAction action)
		{
			float axisValue;

			switch (action) {
				case ButtonAction.Option:
					return Input.GetButtonDown("Option_Button") || Input.GetKeyDown(KeyCode.Escape);
				case ButtonAction.A_Button:
					return Input.GetButtonDown("A_Button") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
				case ButtonAction.B_Button:
					return Input.GetButtonDown("B_Button");
				case ButtonAction.X_Button:
					return Input.GetButtonDown("X_Button");
				case ButtonAction.Y_Button:
					return Input.GetButtonDown("Y_Button");
				case ButtonAction.Up:
					axisValue = Input.GetAxis("DPad_V");
					if (axisValue > 0.0f) { //十字パッドで押されている
						return true;
					} else {
						//押されてないなら左スティックを調べる
						axisValue = Input.GetAxis("Left_Stick_V");
						if (axisValue > 0.5f) {
							return true;
						}
					}



					return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow); //キーボードチェック
				case ButtonAction.Down:
					axisValue = Input.GetAxis("DPad_V");
					if (axisValue < 0.0f) { //十字パッドで押されている
						return true;
					} else {
						//押されてないなら左スティックを調べる
						axisValue = Input.GetAxis("Left_Stick_V");
						if (axisValue < -0.5f) {
							return true;
						}
					}

					return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow); //キーボードチェック
				case ButtonAction.Right:
					axisValue = Input.GetAxis("DPad_H");
					if (axisValue > 0.0f) { //十字パッドで押されている
						return true;
					} else {
						//押されてないなら左スティックを調べる
						axisValue = Input.GetAxis("Left_Stick_H");
						if (axisValue > 0.5f) {
							return true;
						}
					}

					return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow); //キーボードチェック
				case ButtonAction.Left:
					axisValue = Input.GetAxis("DPad_H");
					if (axisValue < 0.0f) { //十字パッドで押されている
						return true;
					} else {
						//押されてないなら左スティックを調べる
						axisValue = Input.GetAxis("Left_Stick_H");
						if (axisValue < -0.5f) {
							return true;
						}
					}

					return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow); //キーボードチェック

				case ButtonAction.None:
					return false;
			}


			return false;
		}
	}
}
