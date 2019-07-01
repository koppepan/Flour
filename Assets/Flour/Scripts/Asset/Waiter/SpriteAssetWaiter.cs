using UnityEngine;

namespace Flour.Asset
{
	public class SpriteAssetWaiter : AssetCacheWaiter<Sprite>
	{
		public SpriteAssetWaiter(string key, int compressCount) : base(key, compressCount) { }

		protected override Sprite CastAsset(Object asset)
		{
			var tex = (Texture2D)asset;
			var sp = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
			sp.name = asset.name;
			return sp;
		}
	}
}
