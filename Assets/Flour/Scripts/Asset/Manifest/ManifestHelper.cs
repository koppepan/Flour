using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UniRx.Async;

namespace Flour.Asset
{
	internal static class ManifestHelper
	{
		private static async UniTask<AssetBundleManifest> LoadManifestAsync(AssetBundle assetBundle)
		{
			var loadReq = assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
			await loadReq;

			return (AssetBundleManifest)loadReq.asset;
		}

		internal static async UniTask<AssetBundleManifest> LoadManifestAsync(string url)
		{
			var request = await UnityWebRequestAssetBundle.GetAssetBundle(url).SendWebRequest();
			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundleManifest in Error. => {request.error}");
				return null;
			}

			return await LoadManifestAsync(DownloadHandlerAssetBundle.GetContent(request));
		}

		internal static async UniTask<AssetBundleManifest> LoadManifestAsync(string url, string fileName, string password)
		{
			var request = await UnityWebRequest.Get(url).SendWebRequest();
			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundleManifest in Error. => {request.error}");
				return null;
			}

			using (var ms = new MemoryStream(request.downloadHandler.data))
			{
				using (var aes = new SeekableAesStream(ms, password, Encoding.UTF8.GetBytes(fileName)))
				{
					var loadReq = AssetBundle.LoadFromStreamAsync(aes);
					await loadReq;
					return await LoadManifestAsync(loadReq.assetBundle);
				}
			}
		}

		private static Dictionary<string, T> ParseDictionary<T>(string src, Func<string, T> parse)
		{
			var dic = new Dictionary<string, T>();
			var split = src.Split('\n');
			for (int i = 0; i < split.Length; i++)
			{
				if (string.IsNullOrEmpty(split[i])) continue;

				var size = split[i].Split(' ');
				var ab = string.Intern(size[0]);
				dic[ab] = parse(size[1]);
			}
			return dic;
		}

		private static async UniTask<string> DecryptAsync(byte[] bytes, string password, string salt)
		{
			using (var ms = new MemoryStream(bytes))
			{
				using (var aes = new SeekableAesStream(ms, password, Encoding.UTF8.GetBytes(salt)))
				{
					using (var sr = new StreamReader(aes))
					{
						return await sr.ReadToEndAsync();
					}
				}
			}
		}

		private static AssetBundleSizeManifest CreateSizeManifest(string body)
		{
			long LongParse(string str) => long.Parse(str);
			return new AssetBundleSizeManifest(ParseDictionary(body, LongParse));
		}
		internal static async UniTask<AssetBundleSizeManifest> LoadSizeManifestAsync(string url)
		{
			var request = await UnityWebRequest.Get(url).SendWebRequest();
			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundle Size Manifest in Error. => {request.error}");
				return null;
			}
			return CreateSizeManifest(request.downloadHandler.text);
		}
		internal static async UniTask<AssetBundleSizeManifest> LoadSizeManifestAsync(string url, string fileName, string password)
		{
			var request = await UnityWebRequest.Get(url).SendWebRequest();
			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundle Size Manifest in Error. => {request.error}");
				return null;
			}

			var body = await DecryptAsync(request.downloadHandler.data, password, fileName);
			return CreateSizeManifest(body);
		}


		private static AssetBundleCrcManifest CreateCrcManifest(string body)
		{
			uint UintParse(string str) => uint.Parse(str);
			return new AssetBundleCrcManifest(ParseDictionary<uint>(body, UintParse));
		}
		internal static async UniTask<AssetBundleCrcManifest> LoadCrcManifestAsync(string url)
		{
			var request = await UnityWebRequest.Get(url).SendWebRequest();
			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundle crc Manifest in Error. => {request.error}");
				return null;
			}
			return CreateCrcManifest(request.downloadHandler.text);
		}
		internal static async UniTask<AssetBundleCrcManifest> LoadCrcManifestAsync(string url, string fileName, string password)
		{
			var request = await UnityWebRequest.Get(url).SendWebRequest();
			if (request.isHttpError || request.isNetworkError)
			{
				Debug.LogError($"download AssetBundle crc Manifest in Error. => {request.error}");
				return null;
			}

			var body = await DecryptAsync(request.downloadHandler.data, password, fileName);
			return CreateCrcManifest(body);
		}
	}
}
