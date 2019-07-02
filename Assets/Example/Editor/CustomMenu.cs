using System.Linq;
using UnityEditor;

namespace Example
{
	public class CustomMenu
	{
		#region Scenes
		[MenuItem("CustomMenu/Scene/Create Scene List", priority = 30)]
		static void CreateSceneList()
		{
			SceneListCreator.Create("CustomMenu", 0, "Example");
		}
		#endregion


		#region DefineSymbols
		private static readonly string DebugSymbole = "DEBUG_BUILD";
		private static readonly string UseLocalAssetSymbol = "USE_LOCAL_ASSET";

		static void AddSymbol(string add)
		{
			var group = EditorUserBuildSettings.selectedBuildTargetGroup;
			Flour.Build.BuildClient.SetDefineSynboles(group, new string[] { add }, Enumerable.Empty<string>());
		}
		static void RemoveSymbol(string remove)
		{
			var group = EditorUserBuildSettings.selectedBuildTargetGroup;
			Flour.Build.BuildClient.SetDefineSynboles(group, Enumerable.Empty<string>(), new string[] { remove });
		}

		[MenuItem("CustomMenu/DefineSymbols/Add Debug Symbole")] static void AddDebugSymbole() => AddSymbol(DebugSymbole);
		[MenuItem("CustomMenu/DefineSymbols/Remove Debug Symbole")] static void RemoveDebugSymbole() => RemoveSymbol(DebugSymbole);

		[MenuItem("CustomMenu/DefineSymbols/Add UseLocalAsset Symbole")] static void AddUseLocalAssetSymbole() => AddSymbol(UseLocalAssetSymbol);
		[MenuItem("CustomMenu/DefineSymbols/Remove UseLocalAsset Symbole")] static void RemoveUseLocalAssetSymbole() => RemoveSymbol(UseLocalAssetSymbol);
		#endregion


		[MenuItem("CustomMenu/AssetBundle Build/Windows")] public static void BuildAssetBundleForWindows() => BuildAssetBundle(BuildTarget.StandaloneWindows64);
		[MenuItem("CustomMenu/AssetBundle Build/OSX")] public static void BuildAssetBundleForOSX() => BuildAssetBundle(BuildTarget.StandaloneOSX);
		[MenuItem("CustomMenu/AssetBundle Build/Android")] public static void BuildAssetBundleForAndroid() => BuildAssetBundle(BuildTarget.Android);
		[MenuItem("CustomMenu/AssetBundle Build/iOS")] public static void BuildAssetBundleForiOS() => BuildAssetBundle(BuildTarget.iOS);

		static void BuildAssetBundle(BuildTarget buildTarget)
		{
			var assetBundleFolder = AssetHelper.GetAssetBundleFolderName(buildTarget);
			var outputPath = System.IO.Path.Combine("AssetBundles", assetBundleFolder);

			var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
			var manifest = Flour.Build.BuildAssetBundle.Build(outputPath, buildTarget, options);
			Flour.Build.BuildAssetBundle.CleanUnnecessaryAssetBundles(outputPath, assetBundleFolder, manifest);
			Flour.Build.BuildAssetBundle.CreateAssetBundleSizeManifest(outputPath, AssetHelper.AssetBundleSizeManifestName, manifest);
			Flour.Build.BuildAssetBundle.CreateAssetBundleCrcManifest(outputPath, AssetHelper.AssetBundleCrcManifestName, manifest);
		}
	}
}
