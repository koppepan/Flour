using System.Collections.Generic;
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
				Debug.LogError($"download AssetBundleManifest in Error. => {request.error}");
				return null;
			}

			var loadReq = DownloadHandlerAssetBundle.GetContent(request).LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
			await loadReq;

			return (AssetBundleManifest)loadReq.asset;
		}

		internal static async UniTask<AssetBundleSizeManifest> LoadSizeManifestAsync(string url)
		{
			var request = UnityWebRequest.Get(url);
			await request.SendWebRequest();

			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundle Size Manifest in Error. => {request.error}");
				return null;
			}

			var dic = new Dictionary<string, long>();
			var split = request.downloadHandler.text.Split('\n');
			for (int i = 0; i < split.Length; i++)
			{
				if (string.IsNullOrEmpty(split[i])) continue;

				var size = split[i].Split(' ');
				dic[size[0]] = long.Parse(size[1]);
			}

			return new AssetBundleSizeManifest(dic);
		}

		internal static async UniTask<AssetBundleCrcManifest> LoadCrcManifestAsync(string url)
		{
			var request = UnityWebRequest.Get(url);
			await request.SendWebRequest();

			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundle crc Manifest in Error. => {request.error}");
				return null;
			}

			var dic = new Dictionary<string, uint>();
			var split = request.downloadHandler.text.Split('\n');
			for (int i = 0; i < split.Length; i++)
			{
				if (string.IsNullOrEmpty(split[i])) continue;

				var crc = split[i].Split(' ');
				dic[crc[0]] = uint.Parse(crc[1]);
			}

			return new AssetBundleCrcManifest(dic);
		}
	}
}
