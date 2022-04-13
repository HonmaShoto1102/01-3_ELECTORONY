using Client.FrameWork.Core;
using Client.FrameWork.Editors;
using Client.Game.Controllers;
using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Game.Containers
{
	//ステージセレクトのパーティクルを管理するコンテナー
	public class ParticleContainer : MonoBehaviour
	{
		[SerializeField, Label("エリアID")] public int areaID;
		[SerializeField, Label("最大パーティクル数")] private int particleMax;
		[SerializeField, Label("パーティクル同士の間隔")] private float particleDistance;
		[SerializeField, Label("パーティクルの移動速度")] private float particleSpeed;
		[SerializeField, Label("Path Creator")] private PathCreator pathCreator;

		[SerializeField, Label("エフェクト無効(最小)")] private float min;
		[SerializeField, Label("エフェクト無効(最大)")] private float max;

		[SerializeField, Label("Offline Image")] private Sprite offlineImage;

		private bool _isHide = false;

		//リソースのパス
		private const string OFFLINE_PARTICLE_PATH = "Prefabs/OfflineParticle";
		private const string ONLINE_PARTICLE_PATH = "Prefabs/OnlineParticle";

		private List<GameObject> _particleList = new List<GameObject>();
		private List<TrailRenderer> _trailRendererList = new List<TrailRenderer>();

		private float _nowTravel;

		public void Initialize(bool isOnline, bool changeImage)
		{
			ParticleController.Instance.AddParticleController(this);

			//パーティクルの数だけプレハブを生成する
			GameObject obj;
			if (isOnline) {
				obj = (GameObject)Resources.Load(ONLINE_PARTICLE_PATH);
			} else {
				obj = (GameObject)Resources.Load(OFFLINE_PARTICLE_PATH);
			}
			for (int i = 0; i < particleMax; i++) {
				GameObject clone = Instantiate(obj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
				clone.SetActive(true);
				clone.transform.SetParent(transform);
				_particleList.Add(clone);
				_trailRendererList.Add(clone.GetComponent<TrailRenderer>());
			}

			if (changeImage) {
				Image image = GetComponent<Image>();
				image.sprite = offlineImage;
			}

			_nowTravel = 0;
		}

		private void Update()
		{
			if (_isHide) return;
			if (pathCreator == null) return;

			_nowTravel += particleSpeed;
			if (_nowTravel >= pathCreator.path.length) {
				_nowTravel -= pathCreator.path.length;
			}

			for (int i = 0; i < _particleList.Count; i++) {
				float localTravel = _nowTravel + (particleDistance * (i + 1));

				float distance = localTravel - pathCreator.path.length;
				if (distance > min && distance < max) {
					_trailRendererList[i].emitting = false;
				} else {
					_trailRendererList[i].emitting = true;
				}

				//現在の座標を軌道上に配置
				_particleList[i].transform.position = pathCreator.path.GetPointAtDistance(localTravel);
				//現在の回転を軌道上に配置
				_particleList[i].transform.rotation = pathCreator.path.GetRotationAtDistance(localTravel);
			}
		}

		public void AllHide()
		{
			for (int i = 0; i < _particleList.Count; i++) {
				_particleList[i].SetActive(false);
			}

			_isHide = true;
		}

		public void AllShow()
		{
			for (int i = 0; i < _particleList.Count; i++) {
				_particleList[i].SetActive(true);
			}

			_isHide = false;
		}


	}
}