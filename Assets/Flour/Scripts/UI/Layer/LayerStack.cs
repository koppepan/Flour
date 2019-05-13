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
				layers[i].OnChangeSiblingIndex(layers.Count - (i + 1));
			}
		}

		public bool OnBack()
		{
			var sub = layers.LastOrDefault(x => !x.IgnoreBack);

			if (sub == null)
			{
				return false;
			}
			sub.OnBack();
			return true;
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
			Remove(layer);
			layer.transform.SetAsLastSibling();
			return layer;
		}
		public AbstractSubLayer Peek()
		{
			return layers.LastOrDefault();
		}

		public bool Remove(AbstractSubLayer subLayer)
		{
			var ret = layers.Remove(subLayer);
			if (ret)
			{
				ResetSiblingIndex();
			}
			return ret;
		}

		public AbstractSubLayer FirstOrDefault(SubLayerType type)
		{
			return layers.LastOrDefault(x => x.LayerType == type);
		}
		public AbstractSubLayer FirstOrDefault(AbstractSubLayer subLayer)
		{
			return layers.LastOrDefault(x => x == subLayer);
		}
	}
}
