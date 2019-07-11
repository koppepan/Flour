using System.Collections.Generic;
using UnityEngine;

namespace Flour.Asset
{
	public class AssetBundleSizeManifest
	{
		readonly Dictionary<string, long> size;

		public AssetBundleSizeManifest(Dictionary<string, long> size)
		{
			this.size = size;
		}

		public long GetSize(string assetBundleName)
		{
			if (!size.ContainsKey(assetBundleName))
			{
				Debug.LogWarning($"[AssetBundleSizeManifest] not found AssetBundleName => {assetBundleName}");
				return 0;
			}
			return size[assetBundleName];
		}
	}
}
