using PathCreation;
using PathCreation.Examples;

namespace Client.Game.Models
{
	//メビウスの輪を構成する各パーツそれぞれのデータを定義するクラス C++でいう構造体的なやつ
	public class MobiusData
	{
		//輪のパーツの種類
		public enum MobiusType
		{
			NormalType, //普通のパーツ
			JumpType,   //ジャンプ用
			HardType,   //ねじれないパーツ
		}

		//輪のアニメーションの種類
		public enum ScrewAnimationType
		{
			Screw, //ねじれるアニメーション
			Flat,  //平らに戻るアニメーション
			Stay,  //現在の状態を保持
		}

		public int pathID;                              //パーツのID
		public bool isChange;                           //パーツは更新が必要かどうか
		public MobiusType mobiusType;                   //パーツのタイプ
		public bool isScrew;                            //パーツはねじれているかどうか
		public RoadMeshCreator roadMeshCreator;         //パーツの軌道や頂点情報が入ってるクラス
		public int screwSpeed = 18;                      //ねじれる速度
		public ScrewAnimationType screwAnimationType;   //パーツのアニメーション

		public MobiusData(int pathID, bool isChange, MobiusType mobiusType, bool isScrew, RoadMeshCreator roadMeshCreator)
		{
			this.pathID = pathID;
			this.isChange = isChange;
			this.mobiusType = mobiusType;
			this.isScrew = isScrew;
			this.roadMeshCreator = roadMeshCreator;
			this.screwAnimationType = ScrewAnimationType.Stay;
		}
	}
}
