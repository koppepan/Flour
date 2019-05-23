using System.Linq;
using UnityEngine;

namespace Flour.Layer
{
	internal class SafeAreaHandler
	{
		readonly Vector2 offsetMin;
		readonly Vector2 offsetMax;

		readonly LayerType[] safeAreaLayers;

		public SafeAreaHandler(Vector2 screenSize, Rect safeArea, LayerType[] safeAreaLayers)
		{
			safeArea = SafeAreaSimulateData.GetSafeArea(screenSize, safeArea);

			offsetMin = new Vector2(safeArea.position.x, safeArea.position.y);
			offsetMax = new Vector2(-(screenSize.x - (safeArea.position.x + safeArea.width)), -(screenSize.y - (safeArea.position.y + safeArea.height)));

			this.safeAreaLayers = safeAreaLayers;
		}

		public void Expansion(LayerType layerType, RectTransform rect)
		{
			if (!safeAreaLayers.Contains(layerType))
			{
				return;
			}
			rect.offsetMin = -offsetMin;
			rect.offsetMax = -offsetMax;
		}

		public void Reduction(LayerType layerType, RectTransform rect)
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
