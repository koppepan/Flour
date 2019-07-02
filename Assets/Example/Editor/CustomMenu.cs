using System.Linq;
using UnityEditor;

public class CustomMenu
{
	#region Scenes
	[MenuItem("CustomMenu/Scene/Create Scene List", priority = 30)]
	static void CreateSceneList()
	{
		SceneListCreator.Create(0);
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

	[MenuItem("CustomMenu/DefineSymbols/Add Debug Symbole", priority = 0)]
	public static void AddDebugSymbole()
	{
		AddSymbol(DebugSymbole);
	}
	[MenuItem("CustomMenu/DefineSymbols/Remove Debug Symbole", priority = 1)]
	public static void RemoveDebugSymbole()
	{
		RemoveSymbol(DebugSymbole);
	}

	[MenuItem("CustomMenu/DefineSymbols/Add UseLocalAsset Symbole", priority = 100)]
	public static void AddUseLocalAssetSymbole()
	{
		AddSymbol(UseLocalAssetSymbol);
	}
	[MenuItem("CustomMenu/DefineSymbols/Remove UseLocalAsset Symbole", priority = 101)]
	public static void RemoveUseLocalAssetSymbole()
	{
		RemoveSymbol(UseLocalAssetSymbol);
	}
	#endregion


	[MenuItem("CustomMenu/AssetBundle Build/Windows")] public static void BuildAssetBundleForWindows() => BuildAssetBundle(BuildTarget.StandaloneWindows64);
	[MenuItem("CustomMenu/AssetBundle Build/OSX")] public static void BuildAssetBundleForOSX() => BuildAssetBundle(BuildTarget.StandaloneOSX);
	[MenuItem("CustomMenu/AssetBundle Build/Android")] public static void BuildAssetBundleForAndroid() => BuildAssetBundle(BuildTarget.Android);
	[MenuItem("CustomMenu/AssetBundle Build/iOS")] public static void BuildAssetBundleForiOS() => BuildAssetBundle(BuildTarget.iOS);

	static void BuildAssetBundle(BuildTarget buildTarget)
	{
		var outputPath = System.IO.Path.Combine("AssetBundles", buildTarget.ToString());

		var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
		var manifest = Flour.Build.BuildAssetBundle.Build(outputPath, buildTarget, options);
		Flour.Build.BuildAssetBundle.CleanUnnecessaryAssetBundles(outputPath, manifest);
		Flour.Build.BuildAssetBundle.CreateAssetBundleSizeManifest(outputPath, manifest);
	}
}
