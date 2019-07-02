using System.Collections.Generic;
using UnityEngine;

namespace Flour.Asset
{
	public class AssetBundleCrcManifest
	{
		Dictionary<string, uint> crc;

		public AssetBundleCrcManifest(Dictionary<string, uint> crc)
		{
			this.crc = crc;
		}

		public uint GetCrc(string assetBundleName)
		{
			if (!crc.ContainsKey(assetBundleName))
			{
				Debug.LogWarning($"[AssetBundleCrcManifest] not found AssetBundleName => {assetBundleName}");
				return 0;
			}
			return crc[assetBundleName];
		}
	}
}
