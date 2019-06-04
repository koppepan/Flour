using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

public sealed class SubLayerSourceRepository
{
	List<SubLayerType> keys = new List<SubLayerType>();
	Dictionary<SubLayerType, AbstractSubLayer> srcCaches = new Dictionary<SubLayerType, AbstractSubLayer>();

	int maxCache;

	public SubLayerSourceRepository(int maxCache)
	{
		this.maxCache = maxCache == 0 ? 1 : maxCache;
		keys.Clear();
	}

	public void AddType(SubLayerType key)
	{
		if (!keys.Contains(key))
		{
			keys.Add(key);
		}
	}

	public bool ContainsKey(SubLayerType type)
	{
		return keys.Contains(type);
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

		var prefab = await Resources.LoadAsync<GameObject>(type.ToJpnName());

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
