using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Example
{
	public class CustomMenu
	{
		private const string MenuTitle = "CustomMenu";

		#region Scenes
		[MenuItem(MenuTitle + "/Scene/Create Scene List", priority = 30)]
		static void CreateSceneList()
		{
			SceneListCreator.Create("Example/Scenes", "Example/Editor", "CustomMenu", 0, "Example");
		}
		#endregion


		#region DefineSymbols
		private const string DefineSymbolMenuTitle = MenuTitle + "/DefineSymbols";

		private static readonly string DebugSymbol = "DEBUG_BUILD";
		private const string DebugSymbolMenu = DefineSymbolMenuTitle + "/Debug";

		private static readonly string UseLocalAssetSymbol = "USE_LOCAL_ASSET";
		private const string UseLocalAssetSymbolMenu = DefineSymbolMenuTitle + "/Use Local Asset";

		private static readonly string UseSecureAssetSymbol = "USE_SECURE_ASSET";
		private const string UseSecureAssetSymbolMenu = DefineSymbolMenuTitle + "/Use Secure Asset";

		[MenuItem(DebugSymbolMenu, priority = 100)] static void SetDebugSymbol() => SetSymbolCheked(DebugSymbolMenu, DebugSymbol);
		[MenuItem(UseLocalAssetSymbolMenu, priority = 100)] static void SetUseLocalAssetSymbol() => SetSymbolCheked(UseLocalAssetSymbolMenu, UseLocalAssetSymbol);
		[MenuItem(UseSecureAssetSymbolMenu, priority = 100)] static void SetSecureAssetSymbol() => SetSymbolCheked(UseSecureAssetSymbolMenu, UseSecureAssetSymbol);

		static void SetSymbolCheked(string menu, string symbol)
		{
			var group = EditorUserBuildSettings.selectedBuildTargetGroup;
			var exist = Flour.Build.BuildClient.ExistsDefineSymbol(group, symbol);

			var add = !exist ? new string[] { symbol } : Enumerable.Empty<string>();
			var remove = exist ? new string[] { symbol } : Enumerable.Empty<string>();

			Flour.Build.BuildClient.SetDefineSymboles(group, add, remove);

			Menu.SetChecked(menu, !exist);
		}

		#endregion


		#region AssetBundle

		private const string AssetBundleMenuTitle = MenuTitle + "/AssetBundle Build";
		private const string SecureAssetBundleMenuTitle = MenuTitle + "/Secure AssetBundle Build";
			 

		[MenuItem(AssetBundleMenuTitle + "/Windows", priority = 200)] public static void BuildAssetBundleForWindows() => BuildAssetBundle(BuildTarget.StandaloneWindows64);
		[MenuItem(AssetBundleMenuTitle + "/OSX", priority = 200)] public static void BuildAssetBundleForOSX() => BuildAssetBundle(BuildTarget.StandaloneOSX);
		[MenuItem(AssetBundleMenuTitle + "/Android", priority = 200)] public static void BuildAssetBundleForAndroid() => BuildAssetBundle(BuildTarget.Android);
		[MenuItem(AssetBundleMenuTitle + "/iOS", priority = 200)] public static void BuildAssetBundleForiOS() => BuildAssetBundle(BuildTarget.iOS);

		[MenuItem(SecureAssetBundleMenuTitle + "/Windows", priority = 201)] public static void BuildSecureAssetBundleForWindows() => BuildEncryptAssetBundle(BuildTarget.StandaloneWindows64);
		[MenuItem(SecureAssetBundleMenuTitle + "/OSX", priority = 201)] public static void BuildSecureAssetBundleForOSX() => BuildEncryptAssetBundle(BuildTarget.StandaloneOSX);
		[MenuItem(SecureAssetBundleMenuTitle + "/Android", priority = 201)] public static void BuildSecureAssetBundleForAndroid() => BuildEncryptAssetBundle(BuildTarget.Android);
		[MenuItem(SecureAssetBundleMenuTitle + "/iOS", priority = 201)] public static void BuildSecureAssetBundleForiOS() => BuildEncryptAssetBundle(BuildTarget.iOS);

		static AssetBundleManifest BuildAssetBundle(BuildTarget buildTarget)
		{
			var assetBundleFolder = AssetHelper.GetAssetBundleFolderName(buildTarget);
			var outputPath = Path.Combine("AssetBundles", assetBundleFolder);

			var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
			var manifest = Flour.Build.BuildAssetBundle.Build(outputPath, buildTarget, options);

			Flour.Build.BuildAssetBundle.CleanUnnecessaryAssetBundles(outputPath, assetBundleFolder, manifest);
			Flour.Build.BuildAssetBundle.CreateAssetBundleSizeManifest(outputPath, AssetHelper.AssetBundleSizeManifestName, manifest);
			Flour.Build.BuildAssetBundle.CreateAssetBundleCrcManifest(outputPath, AssetHelper.AssetBundleCrcManifestName, manifest);

			return manifest;
		}

		static async void BuildEncryptAssetBundle(BuildTarget buildTarget)
		{
			var manifest = BuildAssetBundle(buildTarget);

			var manifestName = AssetHelper.GetAssetBundleFolderName(buildTarget);
			var password = await AssetHelper.GetPlainTextPasswordAsync();

			var srcPath = Path.Combine("AssetBundles", AssetHelper.GetAssetBundleFolderName(buildTarget));
			var cryptoPath = Path.Combine("AssetBundles", AssetHelper.GetEncryptAssetBundleFolderName(buildTarget));

			Flour.Build.BuildAssetBundle.BuildEncrypt(srcPath, cryptoPath,
				manifestName, AssetHelper.AssetBundleSizeManifestName, AssetHelper.AssetBundleCrcManifestName, password, manifest);
		}
		#endregion
	}
}
