using Client.FrameWork.Core;
using Client.Game.Containers;
using System.Collections.Generic;

namespace Client.Game.Controllers
{
	//ステージセレクトの情報を管理するコントローラー
	public class StageSelectController : SceneSingleton<StageSelectController>
	{
		private List<StageSelectContainer> _stageSelectList = new List<StageSelectContainer>();

		//リストにStageSelectContainerを登録する
		public void AddStageSelectContainer(StageSelectContainer container)
		{
			_stageSelectList.Add(container);
		}

		//指定されたIDのContainerを検索して返す
		public StageSelectContainer GetStageSelect(int id) {
			return _stageSelectList.Find(data => data.stageID == id);
		}
	}
}