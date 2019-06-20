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
	private static readonly string DebugSymboleValue = "DEBUG_BUILD";

	[MenuItem("CustomMenu/Build/Add Debug Symbole")]
	public static void AddDebugSymbole()
	{
		var group = EditorUserBuildSettings.selectedBuildTargetGroup;
		Flour.Build.BuildScript.SetDefineSynboles(group, new string[] { DebugSymboleValue }, Enumerable.Empty<string>());
	}
	[MenuItem("CustomMenu/Build/Remove Debug Symbole")]
	public static void RemoveDebugSymbole()
	{
		var group = EditorUserBuildSettings.selectedBuildTargetGroup;
		Flour.Build.BuildScript.SetDefineSynboles(group, Enumerable.Empty<string>(), new string[] { DebugSymboleValue });
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
