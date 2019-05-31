using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneList
{
	[MenuItem("CustomMenu/Scene/InGame", priority = 100)]
	public static void OpenSceneInGame()
	{
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene("Assets/Example/Scenes/InGame.unity");
		}
	}
	[MenuItem("CustomMenu/Scene/OutGame", priority = 100)]
	public static void OpenSceneOutGame()
	{
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene("Assets/Example/Scenes/OutGame.unity");
		}
	}
	[MenuItem("CustomMenu/Scene/Start", priority = 100)]
	public static void OpenSceneStart()
	{
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene("Assets/Example/Scenes/Start.unity");
		}
	}
	[MenuItem("CustomMenu/Scene/Title", priority = 100)]
	public static void OpenSceneTitle()
	{
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene("Assets/Example/Scenes/Title.unity");
		}
	}
}
