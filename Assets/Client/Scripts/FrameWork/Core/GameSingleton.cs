using UnityEngine;
using System.Collections;

//==============================================================
// Class
//==============================================================
namespace Client.FrameWork.Core
{
	public class GameSingleton<T> : SystemBehaviour where T : SystemBehaviour
	{
		private static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					Debug.Log("create SingletonBehaviour<"+typeof(T)+">");
					instance = (T)FindObjectOfType(typeof(T));

					if (instance == null)
					{
						Debug.Log (typeof(T) + "is nothing");

						// ゲームオブジェクトを作成しコンポーネントを追加する
						GameObject obj = new GameObject(typeof(T).Name);
						instance = (T)obj.AddComponent<T>();
					}

					instance.Initialize();

					DontDestroyOnLoad(instance.gameObject);
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
