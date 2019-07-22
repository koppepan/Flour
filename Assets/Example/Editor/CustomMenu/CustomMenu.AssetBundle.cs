using System.IO;
using UnityEngine;
using UnityEditor;

using BuildAssetBundle = Flour.Build.AssetBundle;

namespace Example
{
	partial class CustomMenu
	{
		private const string AssetBundleMenuTitle = MenuTitle + "/AssetBundle Build";
		private const string SecureAssetBundleMenuTitle = MenuTitle + "/Secure AssetBundle Build";


		[MenuItem(AssetBundleMenuTitle + "/Windows", priority = AssetBundlePriority)] public static void BuildAssetBundleForWindows() => AssetBundleBuild(BuildTarget.StandaloneWindows64);
		[MenuItem(AssetBundleMenuTitle + "/OSX", priority = AssetBundlePriority)] public static void BuildAssetBundleForOSX() => AssetBundleBuild(BuildTarget.StandaloneOSX);
		[MenuItem(AssetBundleMenuTitle + "/Android", priority = AssetBundlePriority)] public static void BuildAssetBundleForAndroid() => AssetBundleBuild(BuildTarget.Android);
		[MenuItem(AssetBundleMenuTitle + "/iOS", priority = AssetBundlePriority)] public static void BuildAssetBundleForiOS() => AssetBundleBuild(BuildTarget.iOS);

		[MenuItem(SecureAssetBundleMenuTitle + "/Windows", priority = SecureAssetBundlePriority)] public static void BuildSecureAssetBundleForWindows() => BuildSecureAssetBundle(BuildTarget.StandaloneWindows64);
		[MenuItem(SecureAssetBundleMenuTitle + "/OSX", priority = SecureAssetBundlePriority)] public static void BuildSecureAssetBundleForOSX() => BuildSecureAssetBundle(BuildTarget.StandaloneOSX);
		[MenuItem(SecureAssetBundleMenuTitle + "/Android", priority = SecureAssetBundlePriority)] public static void BuildSecureAssetBundleForAndroid() => BuildSecureAssetBundle(BuildTarget.Android);
		[MenuItem(SecureAssetBundleMenuTitle + "/iOS", priority = SecureAssetBundlePriority)] public static void BuildSecureAssetBundleForiOS() => BuildSecureAssetBundle(BuildTarget.iOS);

		static AssetBundleManifest AssetBundleBuild(BuildTarget buildTarget)
		{
			var assetBundleFolder = AssetHelper.GetAssetBundleFolderName(buildTarget);
			var outputPath = Path.Combine("AssetBundles", assetBundleFolder);

			var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
			var manifest = BuildAssetBundle.Build(outputPath, buildTarget, options);

			BuildAssetBundle.CleanUnnecessaryAssetBundles(outputPath, assetBundleFolder, manifest);
			BuildAssetBundle.CreateAssetBundleSizeManifest(outputPath, AssetHelper.AssetBundleSizeManifestName, manifest);
			BuildAssetBundle.CreateAssetBundleCrcManifest(outputPath, AssetHelper.AssetBundleCrcManifestName, manifest);

			return manifest;
		}

		static async void BuildSecureAssetBundle(BuildTarget buildTarget)
		{
			var manifest = AssetBundleBuild(buildTarget);

			var manifestName = AssetHelper.GetAssetBundleFolderName(buildTarget);
			var password = await AssetHelper.GetPlainTextPasswordAsync();

			var srcPath = Path.Combine("AssetBundles", AssetHelper.GetAssetBundleFolderName(buildTarget));
			var cryptoPath = Path.Combine("AssetBundles", AssetHelper.GetEncryptAssetBundleFolderName(buildTarget));

			BuildAssetBundle.BuildEncrypt(srcPath, cryptoPath,
				manifestName, AssetHelper.AssetBundleSizeManifestName, AssetHelper.AssetBundleCrcManifestName, password, manifest);
		}
	}
}
