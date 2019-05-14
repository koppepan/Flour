using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Flour.UI
{
	public class SubLayerSourceRepository
	{
		Dictionary<SubLayerType, string> srcPaths;
		Dictionary<SubLayerType, AbstractSubLayer> srcCaches = new Dictionary<SubLayerType, AbstractSubLayer>();

		int maxCache;

		public SubLayerSourceRepository(Dictionary<SubLayerType, string> srcPaths, int maxCache)
		{
			this.srcPaths = srcPaths;
			this.maxCache = maxCache == 0 ? 1 : maxCache;
		}

		public async UniTask<T> LoadAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			if (!srcPaths.ContainsKey(type))
			{
				Debug.LogWarning(type.ToString() + " : missing source path.");
				return null;
			}
			if (srcCaches.ContainsKey(type))
			{
				var cache = srcCaches[type];
				srcCaches.Remove(type);
				srcCaches.Add(type, cache);
				return (T)srcCaches[type];
			}

			var prefab = await Resources.LoadAsync<GameObject>(srcPaths[type]);

			if (prefab == null)
			{
				Debug.LogWarning(type.ToString() + " : not found resource.");
				return null;
			}
			srcCaches.Add(type, ((GameObject)prefab).GetComponent<AbstractSubLayer>());

			if (srcCaches.Count > maxCache)
			{
				var remove = srcCaches.First();
				srcCaches.Remove(remove.Key);

				Resources.UnloadAsset(remove.Value);
				remove = default;
			}

			return (T)srcCaches[type];
		}
	}
}
