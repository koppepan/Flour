using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Example
{
	public sealed class SubLayerSourceRepository
	{
		readonly List<Repository<SubLayerType, AbstractSubLayer>> repositories = new List<Repository<SubLayerType, AbstractSubLayer>>();

		public void AddRepository(IEnumerable<SubLayerType> types, int cacheLimit)
		{
			types = types.Where(x => !x.ToResourcePath().StartsWith("UI/Debug", StringComparison.OrdinalIgnoreCase));
			repositories.Add(new Repository<SubLayerType, AbstractSubLayer>(types, cacheLimit));
		}
		public void AddDebugRepository()
		{
			var types = Flour.EnumExtension.ToEnumerable<SubLayerType>(x => x.ToResourcePath().StartsWith("UI/Debug", StringComparison.OrdinalIgnoreCase));
			repositories.Add(new Repository<SubLayerType, AbstractSubLayer>(types, types.Count()));
		}

		private Repository<SubLayerType, AbstractSubLayer> GetRepository(SubLayerType type)
		{
			for (int i = 0; i < repositories.Count; i++)
			{
				if (repositories[i].ContainsKey(type)) return repositories[i];
			}

			Debug.LogWarning(type.ToString() + " : unregistered key.");
			return null;
		}

		public async UniTask PreLoadAsync(params SubLayerType[] types) => await UniTask.WhenAll(types.Select(x => GetAsync<AbstractSubLayer>(x)));

		public async UniTask<T> GetAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			var repo = GetRepository(type);

			if (repo == null) return null;
			if (repo.TryGet(type, out var result)) return (T)result;

			result = await LoadAsync<T>(type);
			if (result == null) return null;

			repo.Add(type, result);
			return (T)result;
		}

		private async UniTask<T> LoadAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			var prefab = await Resources.LoadAsync<GameObject>(type.ToResourcePath());
			if (prefab == null)
			{
				Debug.LogWarning(type.ToString() + " : not found resource.");
				return null;
			}
			return ((GameObject)prefab).GetComponent<T>();
		}

	}
}
