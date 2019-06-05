using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;

namespace Flour.Scene
{
	public sealed class SceneHandler<T>
	{
		public AbstractScene<T> CurrentScene { get; private set; }

		List<AbstractScene<T>> additiveScenes = new List<AbstractScene<T>>();
		public IEnumerable<AbstractScene<T>> AdditiveScenes => additiveScenes;

		public bool OnBack()
		{
			if (CurrentScene != null)
			{
				CurrentScene.OnBack();
				return true;
			}
			return false;
		}

		public void ApplicationPause(bool pause)
		{
			CurrentScene?.ApplicationPause(pause);
			additiveScenes.ForEach(x => x.ApplicationPause(pause));
		}

		private AbstractScene<T> Find(string sceneName)
		{
			if (CurrentScene.SceneName == sceneName)
			{
				return CurrentScene;
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

		public async UniTask LoadAsync(string sceneName, T param, Func<UniTask> awaitFunc, params object[] args)
		{
			CurrentScene?.Unload();
			await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

			var scene = GetAbstractScene(SceneManager.GetSceneByName(sceneName));
			CurrentScene = null;

			if (scene != null)
			{
				CurrentScene = scene;
				await LoadScene(CurrentScene, sceneName, param, args);
			}

			await awaitFunc();

			CurrentScene?.Open();
		}

		public async UniTask AddAsync(string sceneName, T param, params object[] args)
		{
			await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			var scene = GetAbstractScene(SceneManager.GetSceneByName(sceneName));
			if (scene != null)
			{
				additiveScenes.Add(scene);
				await LoadScene(scene, sceneName, param, args);
			}

			scene?.Open();
		}

		private async UniTask LoadScene(AbstractScene<T> scene, string sceneName, T param, params object[] args)
		{
			scene.SetName(sceneName);
			scene.SetParameter(param);
			await scene.Load(args);
		}

		public async UniTask UnloadAsync(string sceneName)
		{
			if (CurrentScene?.SceneName == sceneName)
			{
				UnityEngine.Debug.LogWarning("can not unload current scene.");
				return;
			}
			Find(sceneName)?.Unload();
			await SceneManager.UnloadSceneAsync(sceneName);
		}
	}
}
