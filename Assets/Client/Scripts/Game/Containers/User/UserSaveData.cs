using Client.FrameWork.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Client.Game.Containers
{
	//プレイヤーの操作を管理
	public class UserSaveData : GameSingleton<UserSaveData>
	{
		private const int MAX_STAGE = 30;

		//セーブデータを初期化する
		public void SaveDataInitialize()
		{
			PlayerPrefs.SetInt("Initialized", 1); //初期化済みフラグを立てる

			PlayerPrefs.SetInt("Tutorial", 0); //チュートリアルクリア済みフラグを消す

			for (int i = 1; i <= MAX_STAGE; i++) {
				//各ステージのクリア情報を初期化する
				PlayerPrefs.SetInt("Stage_" + i + "_Star", 0); //獲得した星の数を0にする(未クリア判定)
			}

			PlayerPrefs.Save();
		}

		//設定用のデータを初期化する
		private void _SettingDataInitialize()
		{
			PlayerPrefs.SetInt("Setting_Initialized", 1); //初期化済みフラグを立てる

			//初期の音量はどっちも0.5f
			PlayerPrefs.SetFloat("Setting_BGM_Volume", 0.5f);
			PlayerPrefs.SetFloat("Setting_SE_Volume", 0.5f);

			PlayerPrefs.Save();
		}

		//BGMの音量を取得する
		public float GetBGMVolume()
		{
			//設定データが無ければ初期化する 
			if (!PlayerPrefs.HasKey("Setting_Initialized")) {
				_SettingDataInitialize();
			}

			return PlayerPrefs.GetFloat("Setting_BGM_Volume");
		}

		//効果音の音量を取得する
		public float GetSEVolume()
		{
			//設定データが無ければ初期化する 
			if (!PlayerPrefs.HasKey("Setting_Initialized")) {
				_SettingDataInitialize();
			}

			return PlayerPrefs.GetFloat("Setting_SE_Volume");
		}

		//BGMの音量を設定する
		public void SetBGMVolume(float volume) {
			PlayerPrefs.SetFloat("Setting_BGM_Volume", volume);

			PlayerPrefs.Save();
		}

		//効果音の音量を設定する
		public void SetSEVolume(float volume)
		{
			PlayerPrefs.SetFloat("Setting_SE_Volume", volume);

			PlayerPrefs.Save();
		}

		//★デバッグ用 全てのステージを星3でクリア状態にする
		public void Debug_SetAllClear()
		{
			for (int i = 1; i <= MAX_STAGE; i++) {
				//各ステージのクリア情報を初期化する
				PlayerPrefs.SetInt("Stage_" + i + "_Star", 3); //獲得した星の数を0にする(未クリア判定)
			}

			PlayerPrefs.Save();
		}

		public void SetStar(int stage, int star)
		{
			PlayerPrefs.SetInt("Stage_" + stage + "_Star", star); //獲得した星の数をセーブする

			PlayerPrefs.Save();
		}

		//セーブデータが初期化されているかを調べる
		public bool isInitialized()
		{
			if (PlayerPrefs.HasKey("Initialized")) { //キーを持ってるか調べる
				return PlayerPrefs.GetInt("Initialized") == 1; //キーを持ってて値が1ならtrueを返す
			} else {
				return false; //キーが無かったらfalseを返す
			}
		}

		//チュートリアルをクリアしているかを調べる
		public bool isTutorialComplete()
		{
			if (PlayerPrefs.HasKey("Tutorial")) { //キーを持ってるか調べる
				return PlayerPrefs.GetInt("Tutorial") == 1; //キーを持ってて値が1ならtrueを返す
			} else {
				return false; //キーが無かったらfalseを返す
			}
		}

		//指定されたステージの星の数を返す
		public int GetStar(int stage)
		{
			if (PlayerPrefs.HasKey("Stage_" + stage + "_Star")) { //キーを持ってるか調べる
				return PlayerPrefs.GetInt("Stage_" + stage + "_Star"); //セーブデータにある値を返す
			}

			return 0; //データが無ければ0が帰る
		}
	}
}