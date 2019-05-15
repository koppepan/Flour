using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flour
{
	static public class DontDestroyObjectList
	{
		static List<Tuple<Type, GameObject>> dontDestroyObjects = new List<Tuple<Type, GameObject>>();

		public static void Add<T>(GameObject obj)
		{
			if (Contains<T>())
			{
				UnityEngine.Object.Destroy(obj);
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(obj);
			dontDestroyObjects.Add(new Tuple<Type, GameObject>(typeof(T), obj));
		}

		public static void Remove<T>()
		{
			if (!Contains<T>())
			{
				return;
			}
			var item = dontDestroyObjects.First(x => x.Item1 == typeof(T));
			dontDestroyObjects.Remove(item);
			UnityEngine.Object.Destroy(item.Item2);
		}

		public static bool Contains<T>()
		{
			return dontDestroyObjects.Any(x => x.Item1 == typeof(T));
		}

		public static void Clear()
		{
			dontDestroyObjects.ForEach(x => UnityEngine.Object.Destroy(x.Item2));
			dontDestroyObjects.Clear();
		}
	}
}
