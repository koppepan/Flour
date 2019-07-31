using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using UniRx.Async;
using UnityEngine;

namespace Flour.Asset
{
	public class SecureAssetBundleHandler : AssetBundleHandler
	{
		readonly string cachePath;
		readonly SecureString password;

		public SecureAssetBundleHandler(string baseUrl, string cachePath, SecureString password)
			: base(baseUrl, new ParallelAssetBundleCacheDownloader(baseUrl, cachePath, 5, 20))
		{
			this.cachePath = cachePath;
			this.password = password;
		}

		protected override Net.IDownloader<AssetBundle> CreateRequest(string assetBundleName, Hash128 hash, uint crc)
		{
			return new AssetBundleCacheDownloader(assetBundleName, cachePath, password, hash, crc);
		}

		protected override async UniTask<Tuple<AssetBundleManifest, AssetBundleSizeManifest, AssetBundleCrcManifest>> LoadManifestAsyncInternal(string baseUrl, string manifestName, string sizeManifestName, string crcManifestName)
		{
			var (result1, result2, result3) = await UniTask.WhenAll(
				ManifestHelper.LoadManifestAsync(baseUrl, manifestName, password),
				ManifestHelper.LoadSizeManifestAsync(baseUrl, sizeManifestName, password),
				ManifestHelper.LoadCrcManifestAsync(baseUrl, crcManifestName, password)
				);

			return Tuple.Create(result1, result2, result3);
		}
	}
}
