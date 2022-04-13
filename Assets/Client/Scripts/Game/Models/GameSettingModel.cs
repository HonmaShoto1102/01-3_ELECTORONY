namespace Client.Game.Models
{
	//全ステージ共通のゲーム設定を格納する
	[System.Serializable]
	public class GameSettingModel
	{
		public int mobius_id;
		public float inside_speed;
		public float outside_speed;
		public float score_add_speed;
		public float score_remove_speed;
		public int gimmick_time;
		public float gimmick_add_speed;
		public float gimmick_remove_speed;
		public int score_decrease;
	}
}