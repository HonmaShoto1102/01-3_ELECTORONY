using Client.FrameWork.Editors;
using UnityEngine;

namespace Client.Game.Containers
{
	//敵のエフェクトを管理
	public class EnemyEffect : MonoBehaviour
	{
		private Vector3 targetPos;
		[HideInInspector] public bool isActive;
		[SerializeField, Label("エフェクト")] ParticleSystem system;
		private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];

		private void Start()
		{
			targetPos = new Vector3(0.0f, 0.0f, 0.0f);
			SetActive(false);
		}

		private void Update()
		{
			if (isActive) {
				var count = system.GetParticles(particles);

				for (int i = 0; i < count; i++) {
					var particle = particles[i];

					float distance = Vector3.Distance(particle.position, targetPos);

					Vector3 v1 = system.transform.TransformPoint(particle.position);
					Vector3 v2 = targetPos;

					//パーティクル生成残り時間に応じて距離をつめる
					float lifeDelta = 1.0f - (particle.remainingLifetime / particle.startLifetime);

					Vector3 dist = system.transform.InverseTransformPoint(Vector3.Lerp(v1, v2, lifeDelta));
					particle.position = dist;
					particles[i] = particle;

				}

				system.SetParticles(particles, count);
			}
		}

		public void SetActive(bool active)
		{
			if (active) {
				if (!isActive) {
					system.Play();
				}
			} else {
				system.Stop();
			}
			isActive = active;
		}
	}
}
