using UnityEngine;
using UnityEngine.UI;

namespace Flour.UI
{
	[RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
	internal class Layer : MonoBehaviour
	{
		Transform subLayerParent;

		public SubLayerStack Stack { get; private set; }

		public void Initialize(LayerType layer, Vector2 referenceResolution, Rect safeArea, Vector2 screenSize)
		{
			var canvas = GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = (int)layer;

			var scaler = GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = referenceResolution;
			scaler.referencePixelsPerUnit = 100;

			Stack = new SubLayerStack(GetContentsArea(safeArea, screenSize));
		}

		Transform GetContentsArea(Rect safeArea, Vector2 screenSize)
		{
#if UNITY_EDITOR
			if (screenSize.x == 1125 && screenSize.y == 2436 || screenSize.x == 2436 && screenSize.y == 1125)
			{
				Vector2 positionOffset;
				Vector2 sizeOffset;

				//縦持ち
				if (screenSize.x < screenSize.y)
				{
					positionOffset = new Vector2(0f, safeArea.size.y * 34f / 812f);
					sizeOffset = positionOffset + new Vector2(0f, safeArea.size.y * 44f / 812f);
				}
				//横持ち
				else
				{
					positionOffset = new Vector2(safeArea.size.x * 44f / 812f, safeArea.size.y * 21f / 375f);
					sizeOffset = positionOffset + new Vector2(safeArea.size.x * 44f / 812f, 0f);
				}

				safeArea.position = safeArea.position + positionOffset;
				safeArea.size = safeArea.size - positionOffset - sizeOffset;
			}
#endif

			if (safeArea.size == screenSize)
			{
				return transform;
			}

			var rect = new GameObject("ContentsArea", typeof(RectTransform)).GetComponent<RectTransform>();
			rect.SetParent(this.transform, false);

			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;

			var min = safeArea.position;
			var max = safeArea.position + safeArea.size;

			min.x /= screenSize.x;
			min.y /= screenSize.y;
			max.x /= screenSize.x;
			max.y /= screenSize.y;

			rect.anchorMin = min;
			rect.anchorMax = max;

			return rect;
		}

		public bool OnBack()
		{
			var sub = Stack.FirstOrDefault(x => !x.IgnoreBack);

			if (sub == null)
			{
				return false;
			}
			sub.OnBack();
			return true;
		}
	}
}
