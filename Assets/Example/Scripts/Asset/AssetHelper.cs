using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Example
{
	public static class AssetHelper
	{
		public static readonly string AssetBundleSizeManifestName = "AssetBundleSize";
		public static readonly string AssetBundleCrcManifestName = "AssetBundleCrc";

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
			return "E" + GetAssetBundleFolderName(buildTarget);
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
			return "E" + GetAssetBundleFolderName(platform);
		}
	}
}
