using System.Collections.Generic;
using System.Linq;

namespace Flour.Layer
{
	internal sealed class SubLayerList<TLayerKey, TSubKey> where TLayerKey : struct where TSubKey : struct
	{
		List<AbstractSubLayer<TLayerKey, TSubKey>> subLayers = new List<AbstractSubLayer<TLayerKey, TSubKey>>();
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

		public void Add(AbstractSubLayer<TLayerKey, TSubKey> layer)
		{
			subLayers.Add(layer);
			layer.transform.SetParent(subLayerParent, false);

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

		public AbstractSubLayer<TLayerKey, TSubKey> FirstOrDefault()
		{
			return subLayers.LastOrDefault();
		}
		public AbstractSubLayer<TLayerKey, TSubKey> FirstOrDefault(TSubKey key)
		{
			return FirstOrDefault(x => x.Key.Equals(key));
		}
		public AbstractSubLayer<TLayerKey, TSubKey> FirstOrDefault(AbstractSubLayer<TLayerKey, TSubKey> subLayer)
		{
			return FirstOrDefault(x => x == subLayer);
		}
		public AbstractSubLayer<TLayerKey, TSubKey> FirstOrDefault(System.Func<AbstractSubLayer<TLayerKey, TSubKey>, bool> func)
		{
			return subLayers.LastOrDefault(func);
		}
	}
}
