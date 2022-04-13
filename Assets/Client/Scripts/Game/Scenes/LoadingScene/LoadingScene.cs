using Client.FrameWork.Editors;
using Client.FrameWork.State;
using Client.Game.Containers;
using Client.Game.Controllers;
using UnityEngine;

namespace Client.Game.Scenes
{
	public class LoadingScene : MonoBehaviour
	{
		[SerializeField, Label("Fade")] public Fade screenFade;

		//ステートマシン本体
		private StateMachine<LoadingScene> _stateMachine;

		[HideInInspector] public TipsContainer tipsContainer;

		//ランダムなTipsの数
		private const int RANDOM_TIPS_COUNT = 13;

		private const string RANDOM_TIPS_PATH = "Prefabs/Canvas/Tips/Random/Tips_";
		private const string SELECT_TIPS_PATH = "Prefabs/Canvas/Tips/Tips_";

		[HideInInspector] public bool secondLoading;

		private void Start()
		{
			//FPSを60に設定
			Application.targetFrameRate = 60;

			secondLoading = GlobalSettingController.Instance.isSecondLoading();

			int tipsID = GlobalSettingController.Instance.GetTipsID();

			if (tipsID == 0) {
				//ランダムにTipsを呼び出す
				int random = Random.Range(1, RANDOM_TIPS_COUNT + 1);
				GameObject canvas = Instantiate((GameObject)Resources.Load(RANDOM_TIPS_PATH + random + ""), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
				canvas.transform.SetParent(transform);
				tipsContainer = canvas.GetComponent<TipsContainer>();
			} else {
				//きめられたTipsを呼び出す
				GameObject canvas = Instantiate((GameObject)Resources.Load(SELECT_TIPS_PATH + tipsID + ""), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
				canvas.transform.SetParent(transform);
				tipsContainer = canvas.GetComponent<TipsContainer>();
			}

			//ステートマシンの初期化をする
			_stateMachine = new StateMachine<LoadingScene>();
			_stateMachine.Initalize(new LoadingInit(), this);
		}

		private void Update()
		{
			//ステートマシンの更新をする
			_stateMachine.Update(this);
		}
	}
}
