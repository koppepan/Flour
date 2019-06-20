using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Flour.Asset
{
	public class SpriteAssetWaiter : AssetWaiter<Sprite>
	{
		Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

		public SpriteAssetWaiter(string key) : base(key)
		{
		}

		public override IObservable<Sprite> LoadAsync(string assetbundleName, string assetName, string valiant)
		{
			if (cache.ContainsKey(assetName))
			{
				return Observable.Return<Sprite>(cache[assetName]);
			}
			return base.LoadAsync(assetbundleName, assetName, valiant);
		}

		protected override Sprite GetAsset(UnityEngine.Object asset)
		{
			if (cache.ContainsKey(asset.name))
			{
				return cache[asset.name];
			}
			var tex = (Texture2D)asset;
			var sp = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
			sp.name = asset.name;
			cache[asset.name] = sp;
			return sp;
		}
	}
}
