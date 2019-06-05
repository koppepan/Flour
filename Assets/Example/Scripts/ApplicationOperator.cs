﻿using System;
using UnityEngine.EventSystems;
using UniRx.Async;
using Flour;

namespace Example
{
	using SceneHandler = Flour.Scene.SceneHandler<IOperationBundler>;
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

		readonly SubLayerSourceRepository[] subLayerRepositories;

		readonly SceneHandler sceneHandler;
		readonly LayerHandler layerHandler;

		public UserPrefs<SaveKey> UserPrefs { get; private set; } = new UserPrefs<SaveKey>();
		public TemporaryData<TemporaryKey> TemporaryData { get; private set; } = new TemporaryData<TemporaryKey>();

		public IInputBinder InputBinder { get; private set; } = new UIInputBinder();
		public ISceneHandler SceneHandler { get { return this; } }
		public ILayerHandler LayerHandler { get { return this; } }

		private bool sceneLoading = false;

		public ApplicationOperator(
			Action onApplicationQuit,
			SceneHandler sceneHandler,
			LayerHandler layerHandler,
			SubLayerSourceRepository[] subLayerRepositories
			)
		{
			this.onApplicationQuit = onApplicationQuit;

			this.subLayerRepositories = subLayerRepositories;

			this.sceneHandler = sceneHandler;
			this.layerHandler = layerHandler;
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
				await UnityEngine.Resources.UnloadUnusedAssets();
				await UniTask.Run(() => GC.Collect(0, GCCollectionMode.Optimized));
				await fade.FadeOut();
				sceneLoading = false;
			}

			await sceneHandler.LoadAsync(sceneType.ToJpnName(), this, task, args);

			InputBinder.Unbind();
		}
		public async UniTask AddSceneAsync(string sceneName, params object[] args)
		{
			InputBinder.Bind();
			await sceneHandler.AddAsync(sceneName, this, args);
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

		private async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer, bool overlap) where T : AbstractSubLayer
		{
			InputBinder.Bind();

			T sub = !overlap ? layerHandler.Get<T>(layer, subLayer) : null;

			if (sub == null)
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
