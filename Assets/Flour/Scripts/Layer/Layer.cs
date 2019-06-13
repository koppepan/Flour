using UnityEngine;
using UnityEngine.UI;

namespace Flour.Layer
{
	sealed internal class Layer<TLayerKey, TSubKey> : System.IDisposable where TLayerKey : struct where TSubKey : struct
	{
		public RectTransform Parent { get; private set; }
		public SubLayerList<TLayerKey, TSubKey> List { get; private set; }

		private GameObject canvasObject;

		public Layer(Transform canvasRoot, TLayerKey layer, int sortingOrder, Vector2 referenceResolution)
		{
			CreateCanvas(canvasRoot, layer.ToString(), sortingOrder, referenceResolution, RenderMode.ScreenSpaceOverlay, null);
		}

		public Layer(Transform canvasRoot, TLayerKey layer, int sortingOrder, Vector2 referenceResolution, RenderMode renderMode, Camera camera)
		{
			CreateCanvas(canvasRoot, layer.ToString(), sortingOrder, referenceResolution, renderMode, camera);
		}

		public void Dispose()
		{
			GameObject.Destroy(canvasObject);
		}

		void CreateCanvas(Transform root, string name, int sortingOrder, Vector2 referenceResolution, RenderMode renderMode, Camera camera)
		{
			canvasObject = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			canvasObject.transform.SetParent(root, false);

			var canvas = canvasObject.GetComponent<Canvas>();
			canvas.renderMode = renderMode;
			canvas.sortingOrder = sortingOrder;
			if (renderMode != RenderMode.ScreenSpaceOverlay)
			{
				canvas.worldCamera = camera;
			}

			var scaler = canvasObject.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = referenceResolution;
			scaler.referencePixelsPerUnit = 100;

			Parent = GetContentsArea(canvasObject.transform);
			List = new SubLayerList<TLayerKey, TSubKey>(Parent);
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
