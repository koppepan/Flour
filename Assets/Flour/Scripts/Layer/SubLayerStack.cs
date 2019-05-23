using System.Collections.Generic;
using System.Linq;

namespace Flour.Layer
{
	internal sealed class SubLayerStack
	{
		List<AbstractSubLayer> subLayers = new List<AbstractSubLayer>();
		UnityEngine.Transform subLayerParent;

		public SubLayerStack(UnityEngine.Transform subLayerParent)
		{
			this.subLayerParent = subLayerParent;
		}

		private void ResetSiblingIndex()
		{
			for (int i = 0; i < subLayers.Count; i++)
			{
				subLayers[i].transform.SetSiblingIndex(i);
				subLayers[i].OnChangeSiblingIndex(subLayers.Count - (i + 1));
			}
		}

		public void Push(AbstractSubLayer layer)
		{
			subLayers.Add(layer);
			layer.transform.SetParent(subLayerParent, false);

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
			return subLayers.LastOrDefault();
		}

		public bool Remove(AbstractSubLayer subLayer)
		{
			if (subLayers.Remove(subLayer))
			{
				ResetSiblingIndex();
				return true;
			}
			return false;
		}

		public int FindIndex(AbstractSubLayer subLayer)
		{
			return subLayers.LastIndexOf(subLayer);
		}

		public AbstractSubLayer FirstOrDefault(SubLayerType type)
		{
			return FirstOrDefault(x => x.SubLayer == type);
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
