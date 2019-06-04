using System.Collections.Generic;
using System.Linq;

namespace Flour.Layer
{
	internal sealed class SubLayerList
	{
		List<AbstractSubLayer> subLayers = new List<AbstractSubLayer>();
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

		public void Add(AbstractSubLayer layer)
		{
			subLayers.Add(layer);
			layer.transform.SetParent(subLayerParent, false);

			ResetSiblingIndex();
		}

		public bool Remove(AbstractSubLayer subLayer, bool reorder)
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

		public int FindIndex(AbstractSubLayer subLayer)
		{
			return subLayers.LastIndexOf(subLayer);
		}

		public AbstractSubLayer FirstOrDefault()
		{
			return subLayers.LastOrDefault();
		}
		public AbstractSubLayer FirstOrDefault(int id)
		{
			return FirstOrDefault(x => x.SubLayerId == id);
		}
		public AbstractSubLayer FirstOrDefault(AbstractSubLayer subLayer)
		{
			return FirstOrDefault(x => x == subLayer);
		}
		public AbstractSubLayer FirstOrDefault(System.Func<AbstractSubLayer, bool> func)
		{
			return subLayers.LastOrDefault(func);
		}
	}
}
