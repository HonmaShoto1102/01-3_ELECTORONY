using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Client.Game.Containers;

namespace Client.FrameWork.Core
{
	/**
     *  @brief  サウンド管理クラス
     */
	public class SoundManager : GameSingleton<SoundManager>
	{
		private AudioSource bgmSource;
		private List<AudioSource> seSourceList = new List<AudioSource>();

		private Dictionary<string, AudioClip> bgmClipDic = new Dictionary<string, AudioClip>();
		private Dictionary<string, AudioClip> seClipDic = new Dictionary<string, AudioClip>();

		private float seVolume = 0.5f;

		public override void Initialize()
		{
			BGMInitalize();

			SetBGMVolume(UserSaveData.Instance.GetBGMVolume());
			SetSEVolume(UserSaveData.Instance.GetSEVolume());

			base.Initialize();
		}

		/**
         *  @brief  初期化処理
         */
		public void BGMInitalize()
		{
			bgmSource = gameObject.AddComponent<AudioSource>();
			bgmSource.loop = true;
		}

		/**
         *  @brief  BGMの読み込み
         *  @param  読み込むBGMのリスト
         */
		public void LoadBGMList(List<string> bgmPathList)
		{
			foreach (string bgmPath in bgmPathList) {
				LoadBGM(bgmPath);
			}
		}

		/**
         *  @brief  BGMの読み込み
         *  @param  読み込むBGM
         */
		public void LoadBGM(string bgmPath)
		{
			// まだAudioClipを読み込んでいなければ読み込んでいなければseClipDicに追加
			if (!bgmClipDic.ContainsKey(bgmPath)) {
				AudioClip bgmClip = Resources.Load<AudioClip>(bgmPath);
				bgmClipDic.Add(bgmPath, bgmClip);
			}
		}

		/**
         *  @brief  SEの読み込み
         *  @param  読み込むSEのリスト
         */
		public void LoadSEList(List<string> sePathList)
		{
			foreach (string sePath in sePathList) {
				LoadSE(sePath);
			}
		}

		/**
         *  @brief  SEの読み込み
         *  @param  読み込むSE
         */
		public void LoadSE(string sePath)
		{
			if (!seClipDic.ContainsKey(sePath)) {
				AudioClip seClip = Resources.Load<AudioClip>(sePath);
				seClipDic.Add(sePath, seClip);
			}
		}

		/**
         *  @brief  BGMを再生
         *  @param  BGMのAudioClipのファイルパス
         */
		public void PlayBGM(string bgmPath)
		{
			bgmPath = "Sounds/BGM/" + bgmPath;
			// まだロードしてなければロードして再生
			LoadBGM(bgmPath);
			bgmSource.clip = bgmClipDic[bgmPath];
			bgmSource.Play();
		}

		/**
         *  @brief  BGMを一時停止
         */
		public void PauseBGM()
		{
			bgmSource.Pause();
		}

		/**
         *  @brief  BGMを一時停止を解除
         */
		public void UnPauseBGM()
		{
			bgmSource.UnPause();
		}

		/**
         *  @brief  BGMを停止
         */
		public void StopBGM()
		{
			if (bgmSource == null) return;

			bgmSource.Stop();
		}

		/**
         *  @brief  BGMのボリュームを設定
         *  @param  BGMのボリューム
         */
		public void SetBGMVolume(float volume)
		{
			bgmSource.volume = volume;
		}


		/**
         *  @brief  BGMのボリュームを取得
         *  @return BGMのボリューム
         */
		public float GetBGMVolume()
		{
			return bgmSource.volume;
		}

		/**
		 *  @brief  SEのボリュームを設定
		 *  @param  SEのボリューム
		 */
		public void SetSEVolume(float volume)
		{
			seVolume = volume;
		}


		/**
         *  @brief  SEのボリュームを取得
         *  @return SEのボリューム
         */
		public float GetSEVolume()
		{
			return seVolume;
		}

		/**
         *  @brief  SEを再生
         *  @param  sePath SEのAudioClipのファイルパス
         *  @param  SEのボリューム
         */
		public void PlaySE(string sePath)
		{
			sePath = "Sounds/SE/" + sePath;
			// まだロードしていなければロードする
			LoadSE(sePath);

			// 使っていないAudiooSourceを使ってSEを鳴らす
			foreach (AudioSource seSource in seSourceList) {
				if (!seSource.isPlaying) {
					seSource.volume = seVolume;
					seSource.PlayOneShot(seClipDic[sePath]);
					return;
				}
			}

			// 使っていないAudiooSourceがないので追加してSEを鳴らす
			AudioSource se = gameObject.AddComponent<AudioSource>();
			se.volume = seVolume;
			se.loop = false;
			seSourceList.Add(se);
			se.PlayOneShot(seClipDic[sePath]);
		}

		public void AllStopSE()
		{
			foreach (AudioSource seSource in seSourceList) {
				if (seSource.isPlaying) {
					seSource.Stop();
				}
			}
		}

		/**
         *  @brief  保持データのクリア
         */
		public void Clear()
		{
			StopBGM();
			seSourceList.Clear();
			bgmClipDic.Clear();
			seClipDic.Clear();
		}
	}
}