using UnityEngine;
using UnityEngine.UI;

namespace Flour.Layer
{
	sealed internal class Layer<TLayerKey, TSubKey> where TLayerKey : struct where TSubKey : struct
	{
		public SubLayerList<TLayerKey, TSubKey> List { get; private set; }

		public Layer(Transform canvasRoot, TLayerKey layer, int sortingOrder, Vector2 referenceResolution, System.Action<TLayerKey, RectTransform> safeAreaReduction)
		{
			var obj = new GameObject(layer.ToString(), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			obj.transform.SetParent(canvasRoot, false);

			var canvas = obj.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = sortingOrder;

			var scaler = obj.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = referenceResolution;
			scaler.referencePixelsPerUnit = 100;

			var parent = GetContentsArea(obj.transform);
			safeAreaReduction?.Invoke(layer, parent);

			List = new SubLayerList<TLayerKey, TSubKey>(parent);
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
