using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Flour
{
	public static class SceneListCreator
	{
		public static void Create(string sceneDirectory, string outputDirectory, string menuTitle, int priority, string namespaceName = "")
		{
			string scriptName = "SceneList.cs";

			string[] sceneDirs = { Path.Combine("Assets", sceneDirectory) };
			if (!Directory.Exists(sceneDirs[0]))
			{
				Debug.LogError($"not exists scene directory => {sceneDirs[0]}");
				return;
			}

			string scriptFilePath = Path.Combine(Application.dataPath, outputDirectory, scriptName);
			if (!Directory.Exists(Path.GetDirectoryName(scriptFilePath)))
			{
				Debug.LogError($"not exists output directory => {Path.GetDirectoryName(scriptFilePath)}");
				return;
			}

			string scriptAssetPath = Path.Combine("Assets", outputDirectory, scriptName);

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
					string sceneName = scenePath.Remove(0, ("Assets/" + sceneDirectory + "/").Length).Replace(".unity", "");// splittedScenePath[splittedScenePath.Length - 1].Replace(".unity", "");
					int depth = sceneName.Count(x => x == '/');

					sw.WriteLine(tab + $"[MenuItem(\"{menuTitle}/Scene/{sceneName}\", priority = {priority + depth})]");
					sw.WriteLine(tab + $"public static void OpenScene{sceneName.Replace("/", "")}()");
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
				sw.WriteLine(tab + "}");

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
}
