using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;

namespace Flour.Scene
{
	public class SceneHandler
	{
		AbstractScene currentScene;

		public bool OnBack()
		{
			if (currentScene != null)
			{
				currentScene.OnBack();
				return true;
			}
			return false;
		}

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

		private async UniTask LoadAsync(string sceneName, IOperationBundler operationBundler, LoadSceneMode mode, params object[] param)
		{
			currentScene?.Unload();
			await SceneManager.LoadSceneAsync(sceneName, mode);

			var scene = SceneManager.GetSceneByName(sceneName);
			currentScene = GetAbstractScene(scene);
			if (currentScene != null)
			{
				currentScene.Setup(sceneName, operationBundler);
				await currentScene.Load(param);
			}
		}

		public async UniTask LoadAsync(string sceneName, IOperationBundler operationBundler, params object[] param)
		{
			await LoadAsync(sceneName, operationBundler, LoadSceneMode.Single, param);
		}

		public async UniTask AddAsync(string sceneName, IOperationBundler operationBundler, params object[] param)
		{
			await LoadAsync(sceneName, operationBundler, LoadSceneMode.Additive, param);
		}

		public async UniTask UnloadAsync(string sceneName)
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
	}
}
