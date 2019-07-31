using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

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

		internal static async UniTask<AssetBundleManifest> LoadManifestAsync(string baseUrl, string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return null;

			using (var request = await UnityWebRequestAssetBundle.GetAssetBundle(Path.Combine(baseUrl, fileName)).SendWebRequest())
			{
				if (request.isHttpError || request.isNetworkError)
				{
					throw new ApplicationException($"download AssetBundleManifest in Error. => {request.error}");
				}
				var assetBundle = DownloadHandlerAssetBundle.GetContent(request);
				var manifest = await LoadManifestAsync(assetBundle);
				assetBundle.Unload(false);

				return manifest;
			}
		}

		internal static async UniTask<AssetBundleManifest> LoadManifestAsync(string baseUrl, string fileName, SecureString password)
		{
			if (string.IsNullOrEmpty(fileName)) return null;

			using (var request = await UnityWebRequest.Get(Path.Combine(baseUrl, fileName)).SendWebRequest())
			{
				if (request.isHttpError || request.isNetworkError)
				{
					throw new ApplicationException($"download AssetBundleManifest in Error. => {request.error}");
				}

				using (var ms = new MemoryStream(request.downloadHandler.data))
				{
					using (var aes = new SeekableAesStream(ms, password.ToPlainText(), Encoding.UTF8.GetBytes(fileName)))
					{
						var loadReq = AssetBundle.LoadFromStreamAsync(aes);
						await loadReq;
						var manifest = await LoadManifestAsync(loadReq.assetBundle);
						loadReq.assetBundle.Unload(false);

						return manifest;
					}
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

		private static async UniTask<string> DecryptAsync(byte[] bytes, SecureString password, string salt)
		{
			using (var ms = new MemoryStream(bytes))
			{
				using (var aes = new SeekableAesStream(ms, password.ToPlainText(), Encoding.UTF8.GetBytes(salt)))
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
		internal static async UniTask<AssetBundleSizeManifest> LoadSizeManifestAsync(string baseUrl, string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return null;

			using (var request = await UnityWebRequest.Get(Path.Combine(baseUrl, fileName)).SendWebRequest())
			{
				if (request.isHttpError || request.isNetworkError)
				{
					throw new ApplicationException($"download AssetBundle Size Manifest in Error. => {request.error}");
				}
				return CreateSizeManifest(request.downloadHandler.text);
			}
		}
		internal static async UniTask<AssetBundleSizeManifest> LoadSizeManifestAsync(string baseUrl, string fileName, SecureString password)
		{
			if (string.IsNullOrEmpty(fileName)) return null;

			using (var request = await UnityWebRequest.Get(Path.Combine(baseUrl, fileName)).SendWebRequest())
			{
				if (request.isHttpError || request.isNetworkError)
				{
					throw new ApplicationException($"download AssetBundle Size Manifest in Error. => {request.error}");
				}
				var body = await DecryptAsync(request.downloadHandler.data, password, fileName);
				return CreateSizeManifest(body);
			}
		}


		private static AssetBundleCrcManifest CreateCrcManifest(string body)
		{
			uint UintParse(string str) => uint.Parse(str);
			return new AssetBundleCrcManifest(ParseDictionary<uint>(body, UintParse));
		}
		internal static async UniTask<AssetBundleCrcManifest> LoadCrcManifestAsync(string baseUrl, string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return null;

			using (var request = await UnityWebRequest.Get(Path.Combine(baseUrl, fileName)).SendWebRequest())
			{
				if (request.isHttpError || request.isNetworkError)
				{
					throw new ApplicationException($"download AssetBundle crc Manifest in Error. => {request.error}");
				}
				return CreateCrcManifest(request.downloadHandler.text);
			}
		}
		internal static async UniTask<AssetBundleCrcManifest> LoadCrcManifestAsync(string baseUrl, string fileName, SecureString password)
		{
			if (string.IsNullOrEmpty(fileName)) return null;

			using (var request = await UnityWebRequest.Get(Path.Combine(baseUrl, fileName)).SendWebRequest())
			{
				if (request.isHttpError || request.isNetworkError)
				{
					throw new ApplicationException($"download AssetBundle crc Manifest in Error. => {request.error}");
				}

				var body = await DecryptAsync(request.downloadHandler.data, password, fileName);
				return CreateCrcManifest(body);
			}
		}
	}
}
