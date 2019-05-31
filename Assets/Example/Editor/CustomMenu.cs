using System.Linq;
using UnityEditor;

public static class CustomMenu
{
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
}
