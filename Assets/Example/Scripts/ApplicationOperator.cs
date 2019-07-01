using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx.Async;
using Flour;

namespace Example
{
	using SceneHandler = Flour.Scene.SceneHandler<Tuple<IOperationBundler, AssetHandler>>;
	using LayerHandler = Flour.Layer.LayerHandler<LayerType, SubLayerType>;

	public sealed class ApplicationOperator : IDisposable, IOperationBundler, ISceneHandler, ILayerHandler
	{
		class UIInputBinder : IInputBinder
		{
			int bindCount = 0;
			EventSystem eventSystem = EventSystem.current;

			public bool Binded => bindCount != 0;
			public void Bind() => eventSystem.enabled = (++bindCount) == 0;
			public void Unbind() => eventSystem.enabled = (--bindCount) == 0;
		}

		readonly Action onApplicationQuit;

		private readonly AssetHandler assetHandler;

		readonly SceneHandler sceneHandler;
		readonly LayerHandler layerHandler;

		readonly SubLayerSourceRepository[] subLayerRepositories;

		public UserPrefs<SaveKey> UserPrefs { get; private set; } = new UserPrefs<SaveKey>();
		public TemporaryData<TemporaryKey> TemporaryData { get; private set; } = new TemporaryData<TemporaryKey>();

		public IInputBinder InputBinder { get; private set; } = new UIInputBinder();
		public ISceneHandler SceneHandler { get { return this; } }
		public ILayerHandler LayerHandler { get { return this; } }

		private bool sceneLoading = false;

		public ApplicationOperator(
			Action onApplicationQuit,
			AssetHandler assetHandler,
			SceneHandler sceneHandler,
			LayerHandler layerHandler,
			params SubLayerSourceRepository[] subLayerRepositories
			)
		{
			this.onApplicationQuit = onApplicationQuit;

			this.assetHandler = assetHandler;

			this.sceneHandler = sceneHandler;
			this.layerHandler = layerHandler;

			this.subLayerRepositories = subLayerRepositories;
		}

		public void Dispose()
		{
			UserPrefs.Dispose();
			TemporaryData.Dispose();
		}

		public void ApplicationPause(bool pause) => sceneHandler.ApplicationPause(pause);
		public void ApplicationQuit() => onApplicationQuit?.Invoke();

		public void OnBack()
		{
			if (InputBinder.Binded)
			{
				return;
			}
			if (layerHandler.OnBack())
			{
				return;
			}
			if (sceneHandler.OnBack())
			{
				return;
			}
		}

		public async UniTask ResourceCompress()
		{
			assetHandler.Compress();
			await Resources.UnloadUnusedAssets();
			await UniTask.Run(() => GC.Collect(0, GCCollectionMode.Optimized));
		}

		public async UniTask LoadSceneAsync(SceneType sceneType, params object[] args)
		{
			if (sceneLoading)
			{
				throw new Exception("another scene load is running.");
			}

			sceneLoading = true;
			InputBinder.Bind();

			var fade = await AddLayerAsync<FadeLayer>(LayerType.System, SubLayerType.Blackout);
			await fade.FadeIn();

			async UniTask task()
			{
				await ResourceCompress();
				await fade.FadeOut();
				sceneLoading = false;
			}

			await layerHandler.RemoveLayer(LayerType.Scene);
			await sceneHandler.LoadAsync(sceneType.ToJpnName(), Tuple.Create<IOperationBundler, AssetHandler>(this, assetHandler), task, args);

			InputBinder.Unbind();
		}
		public async UniTask AddSceneAsync(string sceneName, params object[] args)
		{
			InputBinder.Bind();
			await sceneHandler.AddAsync(sceneName, Tuple.Create<IOperationBundler, AssetHandler>(this, assetHandler), args);
			InputBinder.Unbind();
		}
		public async UniTask UnloadSceneAsync(string sceneName)
		{
			InputBinder.Bind();
			await sceneHandler.UnloadAsync(sceneName);
			InputBinder.Unbind();
		}

		private async UniTask<T> SubLayerPrefabLoadAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			for (int i = 0; i < subLayerRepositories.Length; i++)
			{
				if (subLayerRepositories[i].ContainsKey(type))
				{
					return await subLayerRepositories[i].LoadAsync<T>(type);
				}
			}
			return null;
		}

		public void AddSceneLayer(Transform sceneObj, Camera camera)
		{
			layerHandler.AddLayer(LayerType.Scene, (int)LayerType.Scene, sceneObj, new Vector2(Screen.width, Screen.height), RenderMode.ScreenSpaceCamera, camera);
		}
		public async UniTask<T> AddSceneLayerAsync<T>(SubLayerType subLayer) where T : AbstractSubLayer
		{
			return await AddLayerAsync<T>(LayerType.Scene, subLayer, false);
		}

		private async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer, bool overlap) where T : AbstractSubLayer
		{
			InputBinder.Bind();

			T sub = !overlap ? layerHandler.GetFirst<T>(layer, subLayer) : null;

			if (sub != null)
			{
				sub.MoveFront();
			}
			else
			{
				var prefab = await SubLayerPrefabLoadAsync<T>(subLayer);
				sub = layerHandler.Add(layer, subLayer, prefab, overlap);
			}

			InputBinder.Unbind();

			return sub;
		}
		public async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer
		{
			return await AddLayerAsync<T>(layer, subLayer, false);
		}
		public async UniTask<T> AddLayerOverlappingAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer
		{
			return await AddLayerAsync<T>(layer, subLayer, true);
		}
	}
}
