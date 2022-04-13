using UnityEngine;
using System.Collections;

//==============================================================
// Class
//==============================================================
namespace Client.FrameWork.Core
{
	public class Singleton<T> where T : new()
	{
		private static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new T();
				}
				return instance;
			}
		}
	}
}
