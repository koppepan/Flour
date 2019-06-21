using System.Linq;
using System.IO;
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
		Flour.Build.BuildScript.SetDefineSynboles(group, new string[] { add }, Enumerable.Empty<string>());
	}
	static void RemoveSymbol(string remove)
	{
		var group = EditorUserBuildSettings.selectedBuildTargetGroup;
		Flour.Build.BuildScript.SetDefineSynboles(group, Enumerable.Empty<string>(), new string[] { remove });
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


	[MenuItem("CustomMenu/AssetBundles Build")]
	public static void BuildAssetBundle()
	{
		if (!Directory.Exists("AssetBundles"))
		{
			Directory.CreateDirectory("AssetBundles");
		}

		var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
		BuildPipeline.BuildAssetBundles("AssetBundles", options, BuildTarget.StandaloneWindows64);
	}
}
