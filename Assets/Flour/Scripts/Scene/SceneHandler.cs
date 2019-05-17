using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;

namespace Flour.Scene
{
	public sealed class SceneHandler<T>
	{
		AbstractScene<T> currentScene;

		public bool OnBack()
		{
			if (currentScene != null)
			{
				currentScene.OnBack();
				return true;
			}
			return false;
		}

		private AbstractScene<T> GetAbstractScene(UnityEngine.SceneManagement.Scene scene)
		{
			if (scene == default)
			{
				return null;
			}

			var rootObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootObjects.Length; i++)
			{
				var abstractScene = rootObjects[i].GetComponent<AbstractScene<T>>();
				if (abstractScene != null)
				{
					return abstractScene;
				}
			}
			return null;
		}

		private async UniTask LoadAsync(string sceneName, T param, LoadSceneMode mode, params object[] values)
		{
			currentScene?.Unload();
			await SceneManager.LoadSceneAsync(sceneName, mode);

			var scene = SceneManager.GetSceneByName(sceneName);
			currentScene = GetAbstractScene(scene);
			if (currentScene != null)
			{
				currentScene.Setup(sceneName, param);
				await currentScene.Load(values);
			}
		}

		public async UniTask LoadAsync(string sceneName, T param, params object[] values)
		{
			await LoadAsync(sceneName, param, LoadSceneMode.Single, values);
		}

		public async UniTask AddAsync(string sceneName, T param, params object[] values)
		{
			await LoadAsync(sceneName, param, LoadSceneMode.Additive, values);
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
