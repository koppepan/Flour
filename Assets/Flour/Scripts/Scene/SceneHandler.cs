using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;

namespace Flour.Scene
{
	public class SceneHandler
	{
		AbstractScene currentScene;
		const string EmptySceneName = "Empty";

		private AbstractScene GetAbstractScene(UnityEngine.SceneManagement.Scene scene)
		{
			if (scene == default)
			{
				return null;
			}

			var rootObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootObjects.Length; i++)
			{
				var abstractScene = rootObjects[i].GetComponent<AbstractScene>();
				if (abstractScene != null)
				{
					return abstractScene;
				}
			}
			return null;
		}

		public async UniTask LoadScene(string sceneName, params object[] param)
		{
			var count = SceneManager.sceneCount;

			if (count == 1)
			{
				await SceneManager.LoadSceneAsync(EmptySceneName);
			}
			await UnloadScene(currentScene?.SceneName);
			await AddScene(sceneName, param);

			if (count == 1)
			{
				await SceneManager.UnloadSceneAsync(EmptySceneName);
			}
		}

		async UniTask UnloadScene(string sceneName)
		{
			var abstractScene = GetAbstractScene(SceneManager.GetSceneByName(sceneName));
			if (abstractScene != null)
			{
				abstractScene.Unload();
				if (currentScene.SceneName == sceneName)
				{
					currentScene = null;
				}
				await SceneManager.UnloadSceneAsync(abstractScene.SceneName);
			}
		}
		public async UniTask AddScene(string sceneName, params object[] param)
		{
			await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			var scene = SceneManager.GetSceneByName(sceneName);
			if (scene == default)
			{
				return;
			}
			SceneManager.SetActiveScene(scene);

			currentScene = GetAbstractScene(scene);

			if (currentScene != null)
			{
				await currentScene.Load(sceneName, param);
			}
		}
	}
}
