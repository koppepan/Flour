using UnityEngine;
using UnityEngine.UI;

namespace Flour.Layer
{
	[RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
	sealed internal class Layer<TKey>
	{
		public SubLayerList<TKey> List { get; private set; }

		public Layer(Transform canvasRoot, LayerType layer, Vector2 referenceResolution, System.Action<LayerType, RectTransform> safeAreaReduction)
		{
			var obj = new GameObject(layer.ToString(), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			obj.transform.SetParent(canvasRoot, false);

			var canvas = obj.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = (int)layer;

			var scaler = obj.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = referenceResolution;
			scaler.referencePixelsPerUnit = 100;

			var parent = GetContentsArea(obj.transform);
			safeAreaReduction?.Invoke(layer, parent);

			List = new SubLayerList<TKey>(parent);
		}

		RectTransform GetContentsArea(Transform parent)
		{
			var rect = new GameObject("ContentsArea", typeof(RectTransform)).GetComponent<RectTransform>();
			rect.SetParent(parent, false);

			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = rect.offsetMax = Vector2.zero;

			return rect;
		}

		public bool OnBack()
		{
			var sub = List.FirstOrDefault(x => !x.IgnoreBack);

			if (sub == null)
			{
				return false;
			}
			sub.OnBackInternal();
			return true;
		}
	}
}
