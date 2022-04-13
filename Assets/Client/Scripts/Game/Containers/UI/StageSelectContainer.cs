using Client.FrameWork.Editors;
using Client.Game.Controllers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Game.Containers
{
	//ステージセレクト用のコンテナー
	public class StageSelectContainer : MonoBehaviour
	{
		[SerializeField, Label("ステージID")] public int stageID;

		[SerializeField, Label("ステージではない")] public bool notStage;

		[SerializeField, Label("次に行くとスクロール")] public bool nextScroll;
		[SerializeField, Label("前に行くとスクロール")] public bool prevScroll;

		[SerializeField, Label("次のステージに行くボタン")] public List<GlobalButtonInput.ButtonAction> nextButtonList;
		[SerializeField, Label("前のステージに行くボタン")] public List<GlobalButtonInput.ButtonAction> prevButtonList;

		[Header("EXStage")]
		[SerializeField, Label("エクストラか")] public bool isEXStage;
		[SerializeField, Label("どのステージをクリアで開放か")] public int beforeClearStage;

		//画像のアウトライン
		private Outline _outline;

		private void Start()
		{
			if (notStage) {
				ParticleContainer particle = GetComponent<ParticleContainer>();
				particle.Initialize(true, false);
			} else {
				//Controllerに自分を登録する
				StageSelectController.Instance.AddStageSelectContainer(this);

				//アウトラインを取得する
				_outline = GetComponent<Outline>();

				ParticleContainer particle = GetComponent<ParticleContainer>();
				if (particle == null) return;

				if (isEXStage) {
					if (UserSaveData.Instance.GetStar(stageID) >= 1) {
						particle.Initialize(true, false);
					} else if (UserSaveData.Instance.GetStar(beforeClearStage) >= 1) {
						particle.Initialize(false, false);
					} else {
						particle.Initialize(false, true);
					}
				} else {
					if (UserSaveData.Instance.GetStar(stageID) == 0) {
						//一番最初のステージか、前のステージをクリアしてればICを明るくする
						if (stageID == 1 || UserSaveData.Instance.GetStar(stageID - 1) > 0) {
							particle.Initialize(false, false);
						} else {
							particle.Initialize(false, true);
						}
					} else {
						particle.Initialize(true, false);
					}
				}
			}
		}

		public void SetStageSelect(bool active)
		{
			if (active) {
				_outline.enabled = true;
			} else {
				_outline.enabled = false;
			}
		}
	}
}
