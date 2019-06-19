using UnityEngine;
using UnityEngine.Networking;
using UniRx.Async;

namespace Flour.Asset
{
	internal static class ManifestHelper
	{
		internal static async UniTask<AssetBundleManifest> LoadManifestAsync(string url)
		{
			var request = UnityWebRequestAssetBundle.GetAssetBundle(url);
			await request.SendWebRequest();

			if (request.isHttpError || request.isNetworkError)
			{
				// manifest load error
				return null;
			}

			var loadReq = DownloadHandlerAssetBundle.GetContent(request).LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
			await loadReq;

			return (AssetBundleManifest)loadReq.asset;
		}
	}
}
