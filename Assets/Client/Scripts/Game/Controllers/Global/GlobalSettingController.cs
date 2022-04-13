using Client.FrameWork.Core;
using Client.Game.Containers;
using Client.Game.Containers.Managers;
using UnityEngine;

namespace Client.Game.Controllers
{
	//ゲームの基本となるいろいろな情報を扱う
	public class GlobalSettingController : GameSingleton<GlobalSettingController>
	{
		private const string GAME_SETTING_PATH = "MasterExcel/GameSetting";
		private const string STAGE_DATA_PATH = "MasterExcel/StageData";

		//メビウスマネージャーの参照
		[HideInInspector] public MobiusManager mobiusManager;
		//現在ねじれている輪があるか
		[HideInInspector] public bool nowScrew;

		//チュートリアルで実際に遊ばせるかどうか
		[HideInInspector] public bool tutorialPlayable = false;

		private bool _secondLoading;

		//ゲーム設定の参照
		private GameSetting _gameSetting;
		private StageData _stageData;
		//次にロードするステージの番号
		private int _nextStageID = 1;

		//直前に選択されたステージのID
		[HideInInspector] public int prevSelectStageID = 1;
		[HideInInspector] public int prevSelectEXStageID = 31;

		//ロードするべきステージのIDを設定
		public void SetStageID(int id, bool isEX)
		{
			if (isEX) {
				prevSelectEXStageID = id;
			} else {
				prevSelectStageID = id;
			}
			_nextStageID = id;
			_secondLoading = false;
		}

		//ロードするべきステージのパスを返す
		public string GetNextStagePath()
		{
			StageData data = GetStageData();
			return data.Sheet1[_nextStageID - 1].path;
		}

		public bool isSecondLoading()
		{
			return _secondLoading;
		}

		//ロードするべきステージのTipsIDを取得
		public int GetTipsID()
		{
			StageData data = GetStageData();
			_secondLoading = true;
			return data.Sheet1[_nextStageID - 1].tips;
		}

		//GameSettingを返す
		public GameSetting GetGameSetting()
		{
			//ロードされてなければロードする
			if (_gameSetting == null) {
				_gameSetting = Resources.Load(GAME_SETTING_PATH) as GameSetting;
			}
			return _gameSetting;
		}

		//StageData
		public StageData GetStageData()
		{
			//ロードされてなければロードする
			if (_stageData == null) {
				_stageData = Resources.Load(STAGE_DATA_PATH) as StageData;
			}
			return _stageData;
		}

	}
}
