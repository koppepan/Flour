using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Flour.Layer
{
	public class SubLayerSourceRepository
	{
		Dictionary<SubLayerType, string> srcPaths = new Dictionary<SubLayerType, string>();
		Dictionary<SubLayerType, AbstractSubLayer> srcCaches = new Dictionary<SubLayerType, AbstractSubLayer>();

		int maxCache;

		public SubLayerSourceRepository(Dictionary<SubLayerType, string> srcPaths, int maxCache)
		{
			this.srcPaths = srcPaths;
			this.maxCache = maxCache == 0 ? 1 : maxCache;
		}

		public SubLayerSourceRepository(int maxCache)
		{
			this.maxCache = maxCache;
			srcPaths.Clear();
		}

		public void AddSourcePath(SubLayerType key, string path)
		{
			srcPaths[key] = path;
		}

		public bool ContainsKey(SubLayerType type)
		{
			return srcPaths.ContainsKey(type);
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
				var remove = srcCaches.FirstOrDefault(x => x.Key != type);
				srcCaches.Remove(remove.Key);
				remove = default;
				await Resources.UnloadUnusedAssets();
			}

			return (T)srcCaches[type];
		}
	}
}
