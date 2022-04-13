using UnityEngine;
using System.Collections;
using System;

//==============================================================
// Class
//==============================================================
namespace Client.FrameWork.Core
{
	// Start / Update等はオーバーヘッドになるので継承しない
	public class SystemBehaviour : MonoBehaviour
	{
		private Transform _transform;
		public Transform cachedTransform
		{
			get
			{
				if (_transform == null) _transform = GetComponent<Transform>();
				return _transform;
			}
		}

		private GameObject _gameObject;
		public GameObject cachedGameObject
		{
			get
			{
				if (_gameObject == null) _gameObject = this.gameObject;
				return _gameObject;
			}
		}

		public virtual void Initialize()
		{
		}

		public virtual void SetActive(bool sw)
		{
			if (cachedGameObject != null)
			{
				cachedGameObject.SetActive(sw);
			}
		}

		// Transform => RectTransfromへSetParentした時に呼ぶ
		public virtual void InitializeTransform()
		{
			cachedTransform.localPosition = Vector3.zero;
			cachedTransform.localScale = Vector3.one;
			cachedTransform.localEulerAngles = Vector3.zero;
		}
	}
}
