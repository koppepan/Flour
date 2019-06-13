using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flour.Layer
{
	internal sealed class SubLayerList<TLayerKey, TSubKey> where TLayerKey : struct where TSubKey : struct
	{
		List<AbstractSubLayer<TLayerKey, TSubKey>> subLayers = new List<AbstractSubLayer<TLayerKey, TSubKey>>();
		Transform subLayerParent;

		public IList<AbstractSubLayer<TLayerKey, TSubKey>> SubLayers { get { return subLayers; } }

		public SubLayerList(Transform subLayerParent)
		{
			this.subLayerParent = subLayerParent;
		}

		private void ResetSiblingIndex()
		{
			for (int i = 0; i < subLayers.Count; i++)
			{
				subLayers[i].transform.SetSiblingIndex(i);
				subLayers[i].OnChangeSiblingIndexInternal(subLayers.Count - (i + 1));
			}
		}

		public void Add(AbstractSubLayer<TLayerKey, TSubKey> layer)
		{
			subLayers.Add(layer);
			layer.transform.SetParent(subLayerParent, false);

			var rect = (RectTransform)layer.transform;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = rect.offsetMax = Vector2.zero;

			ResetSiblingIndex();
		}

		public bool Remove(AbstractSubLayer<TLayerKey, TSubKey> subLayer, bool reorder)
		{
			if (subLayers.Remove(subLayer))
			{
				if (reorder)
				{
					ResetSiblingIndex();
				}
				return true;
			}
			return false;
		}

		public int FindIndex(AbstractSubLayer<TLayerKey, TSubKey> subLayer)
		{
			return subLayers.LastIndexOf(subLayer);
		}

		public IEnumerable<AbstractSubLayer<TLayerKey, TSubKey>> Find(TSubKey key)
		{
			for (int i = subLayers.Count - 1; i >= 0; i--)
			{
				if (subLayers[i].Key.Equals(key))
				{
					yield return subLayers[i];
				}
			}
		}

		public AbstractSubLayer<TLayerKey, TSubKey> FirstOrDefault()
		{
			return subLayers.LastOrDefault();
		}
		public AbstractSubLayer<TLayerKey, TSubKey> FirstOrDefault(TSubKey key)
		{
			return FirstOrDefault(x => x.Key.Equals(key));
		}
		public AbstractSubLayer<TLayerKey, TSubKey> FirstOrDefault(System.Func<AbstractSubLayer<TLayerKey, TSubKey>, bool> func)
		{
			return subLayers.LastOrDefault(func);
		}
	}
}
