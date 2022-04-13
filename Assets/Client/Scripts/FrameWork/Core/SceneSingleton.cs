using UnityEngine;
using System.Collections;

#pragma warning disable CS0162

//==============================================================
// Class
//==============================================================
namespace Client.FrameWork.Core
{
	public class SceneSingleton<T> : SystemBehaviour where T : SystemBehaviour
	{
		private const bool logging = false;

		private static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					if(logging)	Debug.Log("create SingletonBehaviour<"+typeof(T)+">");
					instance = (T)FindObjectOfType(typeof(T));

					if (instance == null)
					{
						if(logging)Debug.Log (typeof(T) + " is nothing");

						// ゲームオブジェクトを作成しコンポーネントを追加する
						GameObject obj = new GameObject(typeof(T).Name);
						instance = (T)obj.AddComponent<T>();
					}

					instance.Initialize();
				}
				return instance;
			}
		}
		
		public override void Initialize()
		{
			Debug.Log(this.name + " is Initialized");
		}
	}
} // end of namespace
