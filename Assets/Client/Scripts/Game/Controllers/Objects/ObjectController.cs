using Client.FrameWork.Core;
using Client.Game.Containers;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Game.Controllers
{
	//オブジェクト(自分、敵、ギミック等)を登録してUpdateの管理をするクラス
	public class ObjectController : SceneSingleton<ObjectController>
	{
		//登録されるオブジェクトのリスト
		private List<ObjectContainer> _objectContainerList = new List<ObjectContainer>();

		//敵のコンテナーリスト
		[HideInInspector] public List<EnemyContainer> enemyList = new List<EnemyContainer>();

		[HideInInspector] public int maxEnemyScore = 0;

		private int _count = 0;

		//ObjectContainerに一致するEnemyContainerを返す
		public EnemyContainer GetEnemyContainer(ObjectContainer objectContainer)
		{
			return enemyList.Find(data => data.objectContainer == objectContainer);
		}


		//オブジェクトを登録する
		public void AddObjectContainer(ObjectContainer container)
		{
			_objectContainerList.Add(container);

			//敵の判別を行う
			EnemyContainer enemy = container.GetComponent<EnemyContainer>();
			if (enemy != null) {
				enemyList.Add(enemy);

				enemy.Initialize();
				enemy.SetAnimationTrigger("Anim_Stop");

				_count++;
				enemy.SetCanvasOrder(_count);

				if (enemy.maxScore > maxEnemyScore) {
					maxEnemyScore = enemy.maxScore;
				}
			}
		}

		//リストを返す
		public List<ObjectContainer> GetObjectList()
		{
			return _objectContainerList;
		}

		//全員の動きを止める
		public void AllStop()
		{
			foreach (ObjectContainer container in _objectContainerList) {
				container.autoMove = false;
			}
		}
	}
}
