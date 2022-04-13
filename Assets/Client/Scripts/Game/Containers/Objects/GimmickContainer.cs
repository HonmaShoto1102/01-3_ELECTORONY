using System.Collections;
using System.Collections.Generic;
using Client.FrameWork.Editors;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Containers
{
	//�M�~�b�N�̊Ǘ�������R���e�i�[ �����蔻���Enemy���ōs��
	public class GimmickContainer : MonoBehaviour
	{
		//�M�~�b�N�̎��
		public enum GimmickType
		{
			Reverse,   //�������]
			SpeedUp,   //���x�㏸
			SpeedDown, //���x�ቺ
			ScoreDown, //�X�R�A������
		}

		[SerializeField, Label("�M�~�b�N�̎��")] public GimmickType gimmickType;
	}
}

