using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Flour.Asset
{
	public class AssetCacheWaiter<T> : AssetWaiter<T> where T : UnityEngine.Object
	{
		readonly int compressCount;
		readonly Dictionary<string, T> cache = new Dictionary<string, T>();

		public AssetCacheWaiter(string key, int compressCount) : base(key)
		{
			this.compressCount = compressCount;
		}

		public override IObservable<T> LoadAsync(string assetbundleName, string assetName, string valiant)
		{
			if (cache.ContainsKey(assetName))
			{
				return Observable.Return<T>(cache[assetName]);
			}
			return base.LoadAsync(assetbundleName, assetName, valiant);
		}

		public void Compress()
		{
			while (cache.Count > compressCount) cache.Remove(cache.First().Key);
		}

		protected virtual T CastAsset(UnityEngine.Object asset)
		{
			return (T)asset;
		}

		protected override T GetAsset(UnityEngine.Object asset)
		{
			if (cache.ContainsKey(asset.name))
			{
				var hit = cache[asset.name];
				cache.Remove(asset.name);
				cache.Add(asset.name, hit);

				return cache[asset.name];
			}

			cache.Add(asset.name, CastAsset(asset));
			return cache[asset.name];
		}
	}
}
