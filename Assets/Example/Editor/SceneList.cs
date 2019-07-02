using UnityEditor;
using UnityEditor.SceneManagement;

namespace Example
{
	public class SceneList
	{
		[MenuItem("CustomMenu/Scene/00_Start", priority = 0)]
		public static void OpenScene00_Start()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene("Assets/Example/Scenes/00_Start.unity");
			}
		}
		[MenuItem("CustomMenu/Scene/01_Title", priority = 0)]
		public static void OpenScene01_Title()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene("Assets/Example/Scenes/01_Title.unity");
			}
		}
		[MenuItem("CustomMenu/Scene/10_OutGame", priority = 0)]
		public static void OpenScene10_OutGame()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene("Assets/Example/Scenes/10_OutGame.unity");
			}
		}
		[MenuItem("CustomMenu/Scene/20_InGame", priority = 0)]
		public static void OpenScene20_InGame()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene("Assets/Example/Scenes/20_InGame.unity");
			}
		}
	}
}
