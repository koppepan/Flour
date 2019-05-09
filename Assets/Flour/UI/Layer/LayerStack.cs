using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Flour.UI
{
	[RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
	public class LayerStack : MonoBehaviour
	{
		List<AbstractSubLayer> layers = new List<AbstractSubLayer>();

		public void Initialize(Layer layer, Vector2 referenceResolution)
		{
			var canvas = GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = (int)layer;

			var scaler = GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = referenceResolution;
			scaler.referencePixelsPerUnit = 100;
		}

		private void ResetSiblingIndex()
		{
			for (int i = 0; i < layers.Count; i++)
			{
				layers[i].transform.SetSiblingIndex(i);
			}
		}

		public void Push(AbstractSubLayer layer)
		{
			if (layers.Any(x => x == layer))
			{
				layers.Remove(layer);
			}
			layers.Add(layer);
			layer.transform.SetParent(this.transform, false);

			ResetSiblingIndex();
		}
		public AbstractSubLayer Pop()
		{
			var layer = Peek();
			layers.Remove(layer);
			return layer;
		}
		public AbstractSubLayer Peek()
		{
			return layers.LastOrDefault();
		}

		public AbstractSubLayer FirstOrDefault(Guid guid)
		{
			return layers.LastOrDefault(x => x.Identify == guid);
		}
		public AbstractSubLayer FirstOrDefault(SubLayerType type)
		{
			return layers.LastOrDefault(x => x.LayerType == type);
		}
		public AbstractSubLayer FirstOrDefault(Guid guid, SubLayerType type)
		{
			return layers.LastOrDefault(x => x.Identify == guid && x.LayerType == type);
		}
	}
}
