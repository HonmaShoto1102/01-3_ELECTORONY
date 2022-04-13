using Client.FrameWork.Editors;
using UnityEngine;

namespace Client.Game.Containers
{
	//カメラ位置の格納
	public class CameraPosContainer : MonoBehaviour
	{
		[SerializeField, Label("輪が1個の時の座標")] public Transform mobius_1_Pos;
		[SerializeField, Label("輪が2個の時の座標")] public Transform mobius_2_Pos;
		[SerializeField, Label("輪が3個の時の座標")] public Transform mobius_3_Pos;
	}
}
