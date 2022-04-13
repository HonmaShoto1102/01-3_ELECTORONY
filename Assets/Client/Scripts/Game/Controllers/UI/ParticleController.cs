using Client.FrameWork.Core;
using Client.Game.Containers;
using System.Collections.Generic;

namespace Client.Game.Controllers
{
	//パーティクルの情報を管理するコントローラー
	public class ParticleController : SceneSingleton<ParticleController>
	{
		private List<ParticleContainer> _particleList = new List<ParticleContainer>();

		//リストにParticleContainerを登録する
		public void AddParticleController(ParticleContainer container)
		{
			_particleList.Add(container);
		}

		//指定されたIDのContainerを検索して返す
		public List<ParticleContainer> GetParticleAll(int id)
		{
			return _particleList.FindAll(data => data.areaID == id);
		}

		public List<ParticleContainer> GetList()
		{
			return _particleList;
		}
	}
}
