using System.Collections;
using System.Collections.Generic;
using Client.FrameWork.Editors;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Containers
{
	//ギミックの管理をするコンテナー 当たり判定はEnemy側で行う
	public class GimmickContainer : MonoBehaviour
	{
		//ギミックの種類
		public enum GimmickType
		{
			Reverse,   //向き反転
			SpeedUp,   //速度上昇
			SpeedDown, //速度低下
			ScoreDown, //スコアが減る
		}

		[SerializeField, Label("ギミックの種類")] public GimmickType gimmickType;
	}
}

