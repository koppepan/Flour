using UnityEngine;

namespace Flour.Layer
{
	internal static class SafeAreaSimulateData
	{
#if UNITY_EDITOR
		static readonly Vector2[] Resolutions = new Vector2[]
		{
			new Vector2(1125, 2436),
			new Vector2(828, 1792),
			new Vector2(1242, 2688),
			new Vector2(1668, 2388),
			new Vector2(2048, 2732),
		};

		static readonly Rect[,] SafeAreaResolutions = new Rect[,]
		{
			{ new Rect(0, 102, 1125, 2202), new Rect(132, 63, 2172, 1062) },
			{ new Rect(0, 68, 828, 1636), new Rect(88, 42, 1616, 786) },
			{ new Rect(0, 102, 1242, 2454), new Rect(132, 63, 2424, 1179) },
			{ new Rect(0, 40, 1668, 2348), new Rect(0, 40, 2388, 1628) },
			{ new Rect(0, 40, 2048, 2692), new Rect(0, 40, 2732, 2008) },
		};
#endif

		internal static Rect GetSafeArea(Vector2 screenSize, Rect safeArea)
		{
#if UNITY_EDITOR
			for (int i = 0; i < Resolutions.Length; i++)
			{
				var resolution = Resolutions[i];
				var portrait = screenSize.x == resolution.x && screenSize.y == resolution.y;
				var landscape = screenSize.x == resolution.y && screenSize.y == resolution.x;

				if (portrait || landscape)
				{
					return SafeAreaResolutions[i, portrait ? 0 : 1];
				}
			}
#endif
			return safeArea;
		}
	}
}
