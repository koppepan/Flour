using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Async;
using UnityEngine.SceneManagement;

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
				CurrentScene.OnBackInternal();
				return true;
			}
			return false;
		}

		public void ApplicationPause(bool pause)
		{
			CurrentScene?.ApplicationPauseInternal(pause);
			additiveScenes.ForEach(x => x.ApplicationPauseInternal(pause));
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
			if (additiveScenes.Count != 0)
			{
				await UniTask.WhenAll(additiveScenes.Select(x => x.UnloadInternal()));
				additiveScenes.Clear();
			}

			if (CurrentScene != null)
			{
				await CurrentScene.UnloadInternal();
			}

			await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

			var scene = GetAbstractScene(SceneManager.GetSceneByName(sceneName));
			CurrentScene = null;

			if (scene != null)
			{
				CurrentScene = scene;
				await LoadScene(CurrentScene, sceneName, param, args);
			}

			await awaitFunc();

			CurrentScene?.OpenInternal();
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

			scene?.OpenInternal();
		}

		public async UniTask AddAsyncInEditor(string scenePath, T param, params object[] args)
		{
#if UNITY_EDITOR
			await UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Additive));

			var sceneName = System.IO.Path.GetFileName(scenePath);

			var scene = GetAbstractScene(SceneManager.GetSceneByName(sceneName));
			if (scene != null)
			{
				additiveScenes.Add(scene);
				await LoadScene(scene, sceneName, param, args);
			}

			scene?.OpenInternal();
#endif
		}

		private async UniTask LoadScene(AbstractScene<T> scene, string sceneName, T param, params object[] args)
		{
			scene.SceneName = sceneName;
			scene.SetParameterInternal(param);
			await scene.LoadInternal(args);
		}

		public async UniTask UnloadAsync(string sceneName)
		{
			if (CurrentScene?.SceneName == sceneName)
			{
				UnityEngine.Debug.LogWarning("can not unload current scene.");
				return;
			}

			var scene = additiveScenes.FirstOrDefault(x => x.SceneName == sceneName);
			if (scene != null) await scene.UnloadInternal();

			await SceneManager.UnloadSceneAsync(sceneName);
		}
	}
}
