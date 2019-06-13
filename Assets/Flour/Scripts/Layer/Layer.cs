using UnityEngine;
using UnityEngine.UI;

namespace Flour.Layer
{
	sealed internal class Layer<TLayerKey, TSubKey> where TLayerKey : struct where TSubKey : struct
	{
		public RectTransform Parent { get; private set; }
		public SubLayerList<TLayerKey, TSubKey> List { get; private set; }

		public Layer(Transform canvasRoot, TLayerKey layer, int sortingOrder, Vector2 referenceResolution)
		{
			var canvas = CreateCanvas(canvasRoot, layer.ToString(), referenceResolution);
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = sortingOrder;

			Parent = GetContentsArea(canvas.gameObject.transform);
			List = new SubLayerList<TLayerKey, TSubKey>(Parent);
		}

		public Layer(Transform canvasRoot, TLayerKey layer, int sortingOrder, Vector2 referenceResolution, RenderMode renderMode, Camera camera)
		{
			var canvas = CreateCanvas(canvasRoot, layer.ToString(), referenceResolution);
			canvas.renderMode = renderMode;
			canvas.worldCamera = camera;
			canvas.sortingOrder = sortingOrder;

			Parent = GetContentsArea(canvas.gameObject.transform);
			List = new SubLayerList<TLayerKey, TSubKey>(Parent);
		}

		Canvas CreateCanvas(Transform root, string name, Vector2 referenceResolution)
		{
			var obj = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			obj.transform.SetParent(root, false);

			var scaler = obj.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = referenceResolution;
			scaler.referencePixelsPerUnit = 100;

			return obj.GetComponent<Canvas>();
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
