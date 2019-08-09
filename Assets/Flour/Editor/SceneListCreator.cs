using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Flour
{
	public static class SceneListCreator
	{
		const string SceneListClass = "SceneList";

		public static void Create(string sceneDirectory, string outputDirectory, string menuTitle, int priority, string namespaceName = "")
		{
			string scriptName = "SceneList.cs";

			string[] sceneDirs = { sceneDirectory };
			if (!Directory.Exists(sceneDirs[0]))
			{
				Debug.LogError($"not exists scene directory => {sceneDirs[0]}");
				return;
			}

			string scriptFilePath = Path.Combine(Application.dataPath.Replace("Assets", ""), outputDirectory, scriptName);
			if (!Directory.Exists(Path.GetDirectoryName(scriptFilePath)))
			{
				Debug.LogError($"not exists output directory => {Path.GetDirectoryName(scriptFilePath)}");
				return;
			}

			string scriptAssetPath = Path.Combine(outputDirectory, scriptName);

			string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", sceneDirs);

			using (var fw = new FileWriter(outputDirectory, SceneListClass + ".cs"))
			{
				fw.WriteUsing("UnityEditor");
				fw.WriteUsing("UnityEditor.SceneManagement");
				fw.WriteLine();

				using (fw.StartNamespaceScope(namespaceName))
				{
					using (fw.StartClassScope(SceneListClass))
					{
						foreach (var sceneGUID in sceneGUIDs)
						{
							string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
							string sceneName = scenePath.Remove(0, (sceneDirectory + "/").Length).Replace(".unity", "");
							int depth = sceneName.Count(x => x == '/');

							fw.WriteBody($"[MenuItem(\"{menuTitle}/Scene/{sceneName}\", priority = {priority + depth})]");
							fw.WriteBody($"public static void OpenScene{sceneName.Replace("/", "")}()");
							using (fw.StartScope())
							{
								fw.WriteBody("if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())");
								using (fw.StartScope())
								{
									fw.WriteBody($"EditorSceneManager.OpenScene(\"{scenePath}\");");
								}
							}
						}
					}
				}
			}
		}
	}
}
