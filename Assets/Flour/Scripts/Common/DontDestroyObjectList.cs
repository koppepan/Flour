using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flour
{
	static public class DontDestroyObjectList
	{
		static Dictionary<Type, GameObject> dontDestroyObjects = new Dictionary<Type, GameObject>();

		public static void Add<T>(GameObject obj)
		{
			if(dontDestroyObjects.ContainsKey(typeof(T)))
			{
				UnityEngine.Object.Destroy(obj);
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(obj);
			dontDestroyObjects.Add(typeof(T), obj);
		}

		public static void Remove<T>()
		{
			if(dontDestroyObjects.ContainsKey(typeof(T)))
			{
				return;
			}
			UnityEngine.Object.Destroy(dontDestroyObjects[typeof(T)]);
			dontDestroyObjects.Remove(typeof(T));
		}

		public static void Clear()
		{
			foreach (var obj in dontDestroyObjects.Values)
			{
				UnityEngine.Object.Destroy(obj);
			}
			dontDestroyObjects.Clear();
		}
	}
}
