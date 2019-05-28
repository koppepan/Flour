using System;
using UnityEngine.EventSystems;
using UniRx.Async;

using Flour.Scene;
using Flour.Layer;

public sealed class ApplicationOperator : IDisposable, IOperationBundler, ISceneHandler, ILayerHandler
{
	class UIInputBinder : IInputBinder
	{
		int bindCount;
		EventSystem eventSystem;
		public bool Binded => bindCount != 0;
		public UIInputBinder()
		{
			bindCount = 0;
			eventSystem = EventSystem.current;
		}
		public void Bind() => eventSystem.enabled = (++bindCount) == 0;
		public void Unbind() => eventSystem.enabled = (--bindCount) == 0;
	}

	readonly Action onApplicationQuit;

	readonly SceneHandler<IOperationBundler> sceneHandler;
	readonly LayerHandler layerHandler;

	public SaveData SaveData { get; private set; }
	public IInputBinder InputBinder { get; private set; } = new UIInputBinder();
	public ISceneHandler SceneHandler { get { return this; } }
	public ILayerHandler LayerHandler { get { return this; } }

	public ApplicationOperator(Action onApplicationQuit, SceneHandler<IOperationBundler> sceneHandler, LayerHandler layerHandler)
	{
		this.onApplicationQuit = onApplicationQuit;
		this.sceneHandler = sceneHandler;
		this.layerHandler = layerHandler;

		SaveData = new SaveData();
	}

	public void Dispose()
	{
		SaveData.Dispose();
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

	public async UniTask LoadSceneAsync(string sceneName, params object[] args)
	{
		InputBinder.Bind();

		var fade = await AddLayerAsync<FadeLayer>(LayerType.System, SubLayerType.Blackout);
		await fade.FadeIn();

		await sceneHandler.LoadAsync(sceneName, this, args);

		await UnityEngine.Resources.UnloadUnusedAssets();
		await UniTask.Run(() => System.GC.Collect(0, System.GCCollectionMode.Optimized));

		await fade.FadeOut(close: true);

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

	private async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer, bool overlap) where T : AbstractSubLayer
	{
		InputBinder.Bind();
		var sub = await layerHandler.AddAsync<T>(layer, subLayer, overlap);
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
