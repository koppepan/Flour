using System.IO;
using UnityEngine;
using UnityEditor;

public static class SceneListCreator
{
	public static void Create(string menuTitle, int priority, string namespaceName = "")
	{
		string[] sceneDirs = { "Assets/Example/Scenes" };
		string scriptDir = "Example/Editor";
		string scriptName = "SceneList.cs";

		string scriptFilePath = Path.Combine(Application.dataPath, scriptDir, scriptName);
		string scriptAssetPath = Path.Combine("Assets", scriptDir, scriptName);

		string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", sceneDirs);
		string tab = "";

		using (StreamWriter sw = File.CreateText(scriptFilePath))
		{
			sw.WriteLine("using UnityEditor;");
			sw.WriteLine("using UnityEditor.SceneManagement;");
			sw.WriteLine("");

			if (!string.IsNullOrEmpty(namespaceName))
			{
				sw.WriteLine($"namespace {namespaceName}");
				sw.WriteLine("{");
				AddTab(ref tab);
			}

			sw.WriteLine(tab + "public class SceneList");
			sw.WriteLine(tab + "{");
			AddTab(ref tab);

			foreach (var sceneGUID in sceneGUIDs)
			{
				string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
				string[] splittedScenePath = scenePath.Split(new char[] { '/' });
				string sceneName = splittedScenePath[splittedScenePath.Length - 1].Replace(".unity", "");

				sw.WriteLine(tab + $"[MenuItem(\"{menuTitle}/Scene/{sceneName}\", priority = {priority})]");
				sw.WriteLine(tab + $"public static void OpenScene{sceneName}()");
				sw.WriteLine(tab + "{");

				AddTab(ref tab);

				sw.WriteLine(tab + "if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())");
				sw.WriteLine(tab + "{");

				AddTab(ref tab);

				sw.WriteLine(tab + $"EditorSceneManager.OpenScene(\"{scenePath}\");");

				RemoveTab(ref tab);

				sw.WriteLine(tab + "}");
				RemoveTab(ref tab);
				sw.WriteLine(tab + "}");
			}
			RemoveTab(ref tab);
			sw.WriteLine("}");

			if (!string.IsNullOrEmpty(namespaceName))
			{
				RemoveTab(ref tab);
				sw.WriteLine(tab + "}");
			}
		}
		AssetDatabase.ImportAsset(scriptAssetPath);
		Debug.Log($"created {scriptName}");
	}

	static void AddTab(ref string tab)
	{
		tab += "\t";
	}
	static void RemoveTab(ref string tab)
	{
		tab = tab.Remove(0, "\t".Length);
	}
}
