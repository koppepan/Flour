using System.Collections.Generic;
using System.Linq;

namespace Flour.Layer
{
	internal sealed class SubLayerList<TKey> where TKey : struct
	{
		List<AbstractSubLayer<TKey>> subLayers = new List<AbstractSubLayer<TKey>>();
		UnityEngine.Transform subLayerParent;

		public SubLayerList(UnityEngine.Transform subLayerParent)
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

		public void Add(AbstractSubLayer<TKey> layer)
		{
			subLayers.Add(layer);
			layer.transform.SetParent(subLayerParent, false);

			ResetSiblingIndex();
		}

		public bool Remove(AbstractSubLayer<TKey> subLayer, bool reorder)
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

		public int FindIndex(AbstractSubLayer<TKey> subLayer)
		{
			return subLayers.LastIndexOf(subLayer);
		}

		public AbstractSubLayer<TKey> FirstOrDefault()
		{
			return subLayers.LastOrDefault();
		}
		public AbstractSubLayer<TKey> FirstOrDefault(TKey key)
		{
			return FirstOrDefault(x => x.Key.Equals(key));
		}
		public AbstractSubLayer<TKey> FirstOrDefault(AbstractSubLayer<TKey> subLayer)
		{
			return FirstOrDefault(x => x == subLayer);
		}
		public AbstractSubLayer<TKey> FirstOrDefault(System.Func<AbstractSubLayer<TKey>, bool> func)
		{
			return subLayers.LastOrDefault(func);
		}
	}
}
