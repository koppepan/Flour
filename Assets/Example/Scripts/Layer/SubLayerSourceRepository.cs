using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Example
{
	public sealed class SubLayerSourceRepository
	{
		readonly SubLayerType[] keys;
		Dictionary<SubLayerType, AbstractSubLayer> srcCaches = new Dictionary<SubLayerType, AbstractSubLayer>();

		int cacheLimit;

		public static SubLayerSourceRepository Create(IEnumerable<SubLayerType> subLayers, int cacheLimit)
		{
			subLayers = subLayers.Where(x => !x.ToResourcePath().StartsWith("UI/Debug"));
			return new SubLayerSourceRepository(subLayers, cacheLimit);
		}
		public static SubLayerSourceRepository CreateDebug()
		{
			var debugLayers = Flour.EnumExtension.ToEnumerable<SubLayerType>(x => x.ToResourcePath().StartsWith("UI/Debug"));
			return new SubLayerSourceRepository(debugLayers, debugLayers.Count());
		}

		public SubLayerSourceRepository(IEnumerable<SubLayerType> subLayers, int cacheLimit)
		{
			this.cacheLimit = cacheLimit == 0 ? 1 : cacheLimit;
			keys = subLayers.ToArray();
		}

		public bool ContainsKey(SubLayerType type) => keys.Contains(type);

		public async UniTask LoadAllAsync()
		{
			await UniTask.WhenAll(keys.Select(x => LoadAsync<AbstractSubLayer>(x)));
		}

		public async UniTask<T> LoadAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			if (!keys.Contains(type))
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

			var prefab = await Resources.LoadAsync<GameObject>(type.ToResourcePath());

			if (prefab == null)
			{
				Debug.LogWarning(type.ToString() + " : not found resource.");
				return null;
			}
			srcCaches.Add(type, ((GameObject)prefab).GetComponent<AbstractSubLayer>());

			if (srcCaches.Count > cacheLimit)
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
