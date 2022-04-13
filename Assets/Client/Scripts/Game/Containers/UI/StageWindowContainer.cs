using Client.FrameWork.Editors;
using Client.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Game.Containers
{
	//ステージセレクト用のコンテナー
	public class StageWindowContainer : MonoBehaviour
	{
		[Header("テキスト")]
		[SerializeField, Label("ステージID Text")] public Text stageID;
		[SerializeField, Label("ステージ名 Text")] public Text stageName;

		[Header("画像")]
		[SerializeField, Label("1 Star Image")] public Image star_1_Image;
		[SerializeField, Label("2 Star Image")] public Image star_2_Image;
		[SerializeField, Label("3 Star Image")] public Image star_3_Image;

		[Header("非活性時の設定")]
		[SerializeField, Label("Disable Color")] public Color color;

		public void SetText(int areaID, StageDataModel model)
		{
			if (areaID == 99) {
				stageID.text = "Stage EX - " + (model.id - (30));
				stageName.text = model.name;
			} else {
				stageID.text = "Stage " + (areaID + 1) + " - " + (model.id - (areaID * 5));
				stageName.text = model.name;
			}
		}

		public void SetStar(int stageID)
		{
			//一回全部黒にする
			star_1_Image.color = color;
			star_2_Image.color = color;
			star_3_Image.color = color;

			if (UserSaveData.Instance.GetStar(stageID) >= 1) { //星1
				star_1_Image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f); //星1の色を戻す
			}

			if (UserSaveData.Instance.GetStar(stageID) >= 2) { //星2
				star_2_Image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f); //星3の色を戻す
			}

			if (UserSaveData.Instance.GetStar(stageID) >= 3) { //星3
				star_3_Image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f); //星3の色を戻す
			}
		}
	}
}
