using System.Collections.Generic;
using UnityEngine;

namespace Flour.Layer
{
	internal class SafeAreaHandler<TLayerKey> where TLayerKey : struct
	{
		readonly Vector2 offsetMin;
		readonly Vector2 offsetMax;

		readonly List<TLayerKey> safeAreaLayers = new List<TLayerKey>();

		public SafeAreaHandler(Vector2 screenSize, Rect safeArea)
		{
			safeArea = SafeAreaSimulateData.GetSafeArea(screenSize, safeArea);

			offsetMin = new Vector2(safeArea.position.x, safeArea.position.y);
			offsetMax = new Vector2(-(screenSize.x - (safeArea.position.x + safeArea.width)), -(screenSize.y - (safeArea.position.y + safeArea.height)));
		}

		public void AddSafeLayer(TLayerKey layerType)
		{
			safeAreaLayers.Add(layerType);
		}

		public void Expansion(TLayerKey layerType, RectTransform rect)
		{
			if (!safeAreaLayers.Contains(layerType))
			{
				return;
			}
			rect.offsetMin = -offsetMin;
			rect.offsetMax = -offsetMax;
		}

		public void Reduction(TLayerKey layerType, RectTransform rect)
		{
			if (!safeAreaLayers.Contains(layerType))
			{
				return;
			}
			rect.offsetMin = offsetMin;
			rect.offsetMax = offsetMax;
		}
	}
}
