using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEngine;
using UniRx.Async;
using Flour;
using Flour.Config;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Example
{
	public static class AssetHelper
	{
		public static readonly string AssetBundleSizeManifestName = "AssetBundleSize";
		public static readonly string AssetBundleCrcManifestName = "AssetBundleCrc";

		public static string CacheAssetPath { get { return Path.Combine(Application.temporaryCachePath, "assets"); } }

		public static string GetAssetBundleFolderName(RuntimePlatform platform)
		{
			switch (platform)
			{
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer: return "Windows";
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer: return "OSX";
				case RuntimePlatform.Android: return "Android";
				case RuntimePlatform.IPhonePlayer: return "iOS";
				default: return "unknown";
			}
		}

		public static string GetEncryptAssetBundleFolderName(RuntimePlatform platform) => $"E{GetAssetBundleFolderName(platform)}";


#if UNITY_EDITOR
		public static string GetAssetBundleFolderName(BuildTarget buildTarget)
		{
			switch (buildTarget)
			{
				case BuildTarget.StandaloneWindows64: return "Windows";
				case BuildTarget.StandaloneOSX: return "OSX";
				case BuildTarget.Android: return "Android";
				case BuildTarget.iOS: return "iOS";
				default: return "unknown";
			}
		}

		public static string GetEncryptAssetBundleFolderName(BuildTarget buildTarget) => $"E{GetAssetBundleFolderName(buildTarget)}";
#endif

		public static async UniTask<SecureString> GetPasswordAsync()
		{
			var param = (SecureParameter)await Resources.LoadAsync<SecureParameter>("Config/SecureParameter");
			Resources.UnloadAsset(param);
			return new SecureString().Set(param.Password);
		}

#if UNITY_EDITOR
		public static async UniTask<string> GetPlainTextPasswordAsync()
		{
			var param = (SecureParameter)await Resources.LoadAsync<SecureParameter>("Config/SecureParameter");
			Resources.UnloadAsset(param);
			return param.Password;
		}
#endif

		public static async UniTask<List<ConnectInfomation>> LoadServerListAsync()
		{
			var config = (ServerList)await Resources.LoadAsync<ServerList>("Config/ServerList");
			Resources.UnloadAsset(config);
			return config.list;
		}
	}
}
