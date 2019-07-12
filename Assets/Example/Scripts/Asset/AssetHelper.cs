using System.IO;
using System.Security;
using UnityEngine;
using UniRx.Async;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Example
{
	public static class AssetHelper
	{
		static readonly string EncryptAssetFolderNameFormat = "E{0}";

		public static readonly string AssetBundleSizeManifestName = "AssetBundleSize";
		public static readonly string AssetBundleCrcManifestName = "AssetBundleCrc";

		static readonly string SecureParameterPath = "Config/SecureParameter";

		public static string CacheAssetPath { get { return Path.Combine(Application.temporaryCachePath, "assets"); } }

#if UNITY_EDITOR
		public static string GetAssetBundleFolderName(BuildTarget buildTarget)
		{
			switch (buildTarget)
			{
				case BuildTarget.StandaloneWindows64:
					return "Windows";

				case BuildTarget.StandaloneOSX:
					return "OSX";

				case BuildTarget.Android:
				case BuildTarget.iOS:
					return buildTarget.ToString();

				default:
					return "";
			}
		}

		public static string GetEncryptAssetBundleFolderName(BuildTarget buildTarget)
		{
			return string.Format(EncryptAssetFolderNameFormat, GetAssetBundleFolderName(buildTarget));
		}
#endif

		public static string GetAssetBundleFolderName(RuntimePlatform platform)
		{
			switch (platform)
			{
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					return "Windows";

				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer:
					return "OSX";

				case RuntimePlatform.Android:
					return "Android";

				case RuntimePlatform.IPhonePlayer:
					return "iOS";

				default:
					return "";
			}
		}

		public static string GetEncryptAssetBundleFolderName(RuntimePlatform platform)
		{
			return string.Format(EncryptAssetFolderNameFormat, GetAssetBundleFolderName(platform));
		}

		public static async UniTask<SecureString> GetPasswordAsync()
		{
			var param = await Resources.LoadAsync<Flour.Config.SecureParameter>(SecureParameterPath) as Flour.Config.SecureParameter;

			SecureString pass = new SecureString();
			for (int i = 0; i < param.Password.Length; i++) pass.AppendChar(param.Password[i]);

			Resources.UnloadAsset(param);

			return pass;
		}

#if UNITY_EDITOR
		public static async UniTask<string> GetPlainTextPasswordAsync()
		{
			var param = await Resources.LoadAsync<Flour.Config.SecureParameter>(SecureParameterPath) as Flour.Config.SecureParameter;
			var pass = param.Password;
			Resources.UnloadAsset(param);

			return pass;
		}
#endif
	}
}
