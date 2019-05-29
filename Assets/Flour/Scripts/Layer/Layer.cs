using UnityEngine;
using UnityEngine.UI;

namespace Flour.Layer
{
	[RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
	sealed internal class Layer : MonoBehaviour
	{
		public SubLayerStack Stack { get; private set; }

		public void Initialize(LayerType layer, Vector2 referenceResolution, System.Action<LayerType, RectTransform> safeAreaReduction)
		{
			var canvas = GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = (int)layer;

			var scaler = GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = referenceResolution;
			scaler.referencePixelsPerUnit = 100;

			var parent = GetContentsArea();
			safeAreaReduction?.Invoke(layer, parent);

			Stack = new SubLayerStack(parent);
		}

		RectTransform GetContentsArea()
		{
			var rect = new GameObject("ContentsArea", typeof(RectTransform)).GetComponent<RectTransform>();
			rect.SetParent(this.transform, false);

			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = rect.offsetMax = Vector2.zero;

			return rect;
		}

		public bool OnBack()
		{
			var sub = Stack.FirstOrDefault(x => !x.IgnoreBack);

			if (sub == null)
			{
				return false;
			}
			sub.OnBackInternal();
			return true;
		}
	}
}
