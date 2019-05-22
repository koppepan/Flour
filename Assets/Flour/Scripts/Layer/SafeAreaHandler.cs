using UnityEngine;

namespace Flour.Layer
{
	internal class SafeAreaHandler
	{
		Vector2 offsetMin;
		Vector2 offsetMax;

		public SafeAreaHandler(Vector2 screenSize, Rect safeArea)
		{
			safeArea = SafeAreaSimulateData.GetSafeArea(screenSize, safeArea);

			offsetMin = new Vector2(safeArea.position.x, safeArea.position.y);
			offsetMax = new Vector2(-(screenSize.x - (safeArea.position.x + safeArea.width)), -(screenSize.y - (safeArea.position.y + safeArea.height)));
		}

		public void Expansion(RectTransform rect)
		{
			rect.offsetMin = -(offsetMin + Vector2.one * 2);
			rect.offsetMax = -(offsetMax - Vector2.one * 2);
		}

		public void Reduction(RectTransform rect)
		{
			rect.offsetMin = offsetMin;
			rect.offsetMax = offsetMax;
		}
	}
}
