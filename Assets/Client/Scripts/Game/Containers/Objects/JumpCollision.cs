using Client.FrameWork.Editors;
using UnityEngine;

namespace Client.Game.Containers
{
	//Jumpの当たり判定を行う
	public class JumpCollision : MonoBehaviour
	{
		[SerializeField, Label("Jump Container")] private JumpContainer jumpContainer;

		//当たり判定
		void OnTriggerEnter(Collider other)
		{
			ObjectContainer hit = other.GetComponent<ObjectContainer>();
			if (hit == null) return;


			jumpContainer.JumpEnter(hit);
		}
	}
}