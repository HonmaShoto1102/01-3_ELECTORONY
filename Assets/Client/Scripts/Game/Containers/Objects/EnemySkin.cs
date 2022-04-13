using Client.FrameWork.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Client.Game.Containers
{
	//敵にアタッチするコンテナー
	public class EnemySkin : MonoBehaviour
	{
		[Header("通常色")]
		[SerializeField, Label("Body Material")] private Material body;
		[SerializeField, Label("Under Material")] private Material under;
		[SerializeField, Label("Front Material")] private Material front;
		[SerializeField, Label("Battery Material")] private Material battery;
		[SerializeField, Label("Arm Material")] private Material arm;
		[SerializeField, Label("Heat Material")] private Material heat;
		[SerializeField, Label("Muffler Material")] private Material muffler;

		[Header("赤色")]
		[SerializeField, Label("Body Material")] private Material body_R;
		[SerializeField, Label("Under Material")] private Material under_R;
		[SerializeField, Label("Front Material")] private Material front_R;
		[SerializeField, Label("Battery Material")] private Material battery_R;
		[SerializeField, Label("Arm Material")] private Material arm_R;
		[SerializeField, Label("Heat Material")] private Material heat_R;
		[SerializeField, Label("Muffler Material")] private Material muffler_R;

		private SkinnedMeshRenderer _skinnedMeshRenderer;
		private void Start()
		{
			_skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		}

		public void SetMaterial(bool red)
		{
			Debug.Log("TEst" + red);
			if (red) {
				_skinnedMeshRenderer.sharedMaterials = new Material[] { body_R, under_R, front_R, battery_R, arm_R, heat_R, muffler_R };
				//_skinnedMeshRenderer.materials[0] = body_R;
				//_skinnedMeshRenderer.materials[1] = under_R;
				//_skinnedMeshRenderer.materials[2] = front_R;
				//_skinnedMeshRenderer.materials[3] = battery_R;
				//_skinnedMeshRenderer.materials[4] = arm_R;
				//_skinnedMeshRenderer.materials[5] = heat_R;
				//_skinnedMeshRenderer.materials[6] = muffler_R;
			} else {
				_skinnedMeshRenderer.sharedMaterials = new Material[] { body, under, front, battery, arm, heat, muffler };
				//_skinnedMeshRenderer.materials[0] = body;
				//_skinnedMeshRenderer.materials[1] = under;
				//_skinnedMeshRenderer.materials[2] = front;
				//_skinnedMeshRenderer.materials[3] = battery;
				//_skinnedMeshRenderer.materials[4] = arm;
				//_skinnedMeshRenderer.materials[5] = heat;
				//_skinnedMeshRenderer.materials[6] = muffler;
			}
		}
	}
}
