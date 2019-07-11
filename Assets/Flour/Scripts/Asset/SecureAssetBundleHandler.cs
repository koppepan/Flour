using System;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;
using UnityEngine;
using UniRx.Async;

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
			var pass = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(password));

			var manifest = await ManifestHelper.LoadManifestAsync(Path.Combine(baseUrl, manifestName), manifestName, pass);
			var sizeManifest = await ManifestHelper.LoadSizeManifestAsync(Path.Combine(baseUrl, sizeManifestName), sizeManifestName, pass);

			if (!string.IsNullOrEmpty(crcManifestName))
			{
				var crcManifest = await ManifestHelper.LoadCrcManifestAsync(Path.Combine(baseUrl, crcManifestName), crcManifestName, pass);
				return Tuple.Create(manifest, sizeManifest, crcManifest);
			}

			return Tuple.Create<AssetBundleManifest, AssetBundleSizeManifest, AssetBundleCrcManifest>(manifest, sizeManifest, null);
		}
	}
}
