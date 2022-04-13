using Client.FrameWork.Editors;
using Client.Game.Controllers;
using PathCreation;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Containers
{
	//他の輪に飛ぶための処理を行う
	public class JumpContainer : MonoBehaviour
	{
		[SerializeField, Label("左: パス")] private Transform leftPath;
		[SerializeField, Label("左: メビウスID")] private int mobiusID_1;
		[SerializeField, Label("左: 内側か")] private bool isInside_1 = true;
		[SerializeField, Label("左: 時計回りになるか")] private bool isReverse_1;

		[SerializeField, Label("右: パス")] private Transform rightPath;
		[SerializeField, Label("右: メビウスID")] private int mobiusID_2;
		[SerializeField, Label("右: 内側か")] private bool isInside_2 = false;
		[SerializeField, Label("右: 時計回りになるか")] private bool isReverse_2;


		private VertexPath _jumpPath;

		private void Start()
		{
			List<Vector3> transList = new List<Vector3>();
			//左から右で生成
			transList.Add(new Vector3(leftPath.localPosition.x, leftPath.localPosition.y, 0)); //自分の場所
			transList.Add(new Vector3(rightPath.localPosition.x, rightPath.localPosition.y, 0)); //行き先

			BezierPath bezierPath = new BezierPath(transList, false, PathSpace.xyz);
			bezierPath.ControlPointMode = BezierPath.ControlMode.Aligned;
			bezierPath.AutoControlLength = 0.3f;

			bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;

			_jumpPath = new VertexPath(bezierPath, transform);
		}

		public void JumpEnter(ObjectContainer hit)
		{
			if (hit.jumpSkip > 0) return;

			//Jump1に一致するかを調べる
			if (hit.nowMobiusID == mobiusID_1 && hit.isInside == isInside_1) {
				//左から右へ飛ぶ
				_mobiusJump(hit, mobiusID_2, isInside_2, isReverse_2, false);

			} else if (hit.nowMobiusID == mobiusID_2 && hit.isInside == isInside_2) {
				//右から左へ飛ぶ
				_mobiusJump(hit, mobiusID_1, isInside_1, isReverse_1, true);
			}

		}

		private void _mobiusJump(ObjectContainer container, int jumpID, bool jumpInside, bool jumpReverse, bool pathReverse)
		{
			EnemyContainer enemy = ObjectController.Instance.GetEnemyContainer(container);
			if (enemy != null) {
				enemy.SetAnimationTrigger("Anim_Jump");
			}
			container.jumpSkip = 150;

			//ジャンプ先の情報を設定
			container.nowMobiusID = jumpID;
			container.isInside = jumpInside;
			container.isReverse = jumpReverse;

			container.jumpPath = _jumpPath;

			//ルートの設定
			if (pathReverse) { //右から左なら反転
				container.jumpType = ObjectContainer.JumpType.Reverse;
				container.nowTravel = _jumpPath.length;
			} else { //左から右ならそのまま
				container.jumpType = ObjectContainer.JumpType.Normal;
				container.nowTravel = 0;
			}
		}
	}
}