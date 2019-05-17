using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;

namespace Flour.Scene
{
	public sealed class SceneHandler<T>
	{
		AbstractScene<T> currentScene;
		List<AbstractScene<T>> additiveScenes = new List<AbstractScene<T>>();

		public bool OnBack()
		{
			if (currentScene != null)
			{
				currentScene.OnBack();
				return true;
			}
			return false;
		}

		private AbstractScene<T> Find(string sceneName)
		{
			if (currentScene.SceneName == sceneName)
			{
				return currentScene;
			}
			return additiveScenes.FirstOrDefault(x => x.SceneName == sceneName);
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

		public async UniTask LoadAsync(string sceneName, T param, params object[] args)
		{
			currentScene?.Unload();
			await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

			var scene = GetAbstractScene(SceneManager.GetSceneByName(sceneName));
			currentScene = null;

			if (scene != null)
			{
				currentScene = scene;
				await LoadScene(currentScene, sceneName, param, args);
			}
		}

		public async UniTask AddAsync(string sceneName, T param, params object[] args)
		{
			await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			var scene = GetAbstractScene(SceneManager.GetSceneByName(sceneName));
			if (scene != null)
			{
				additiveScenes.Add(scene);
				await LoadScene(currentScene, sceneName, param, args);
			}
		}

		private async UniTask LoadScene(AbstractScene<T> scene, string sceneName, T param, params object[] args)
		{
			scene.SetName(sceneName);
			scene.SetParameter(param);
			await scene.Load(args);
		}

		public async UniTask UnloadAsync(string sceneName)
		{
			if (currentScene?.SceneName == sceneName)
			{
				UnityEngine.Debug.LogWarning("can not unload current scene.");
				return;
			}
			Find(sceneName)?.Unload();
			await SceneManager.UnloadSceneAsync(sceneName);
		}
	}
}
