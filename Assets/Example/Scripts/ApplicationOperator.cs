﻿using UnityEngine.EventSystems;
using UniRx.Async;

using Flour;
using Flour.Scene;
using Flour.UI;

public class ApplicationOperator : IOperationBundler, ISceneHandler, ILayerHandler
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

	System.Action onApplicationQuit;

	UIInputBinder inputBinder = new UIInputBinder();
	SceneHandler sceneHandler;
	LayerHandler layerHandler;

	public IInputBinder InputBinder { get { return inputBinder; } }
	public ISceneHandler SceneHandler { get { return this; } }
	public ILayerHandler LayerHandler { get { return this; } }


	public ApplicationOperator(System.Action onApplicationQuit, SceneHandler sceneHandler, LayerHandler layerHandler)
	{
		this.onApplicationQuit = onApplicationQuit;
		this.sceneHandler = sceneHandler;
		this.layerHandler = layerHandler;
	}

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

	public async UniTask LoadSceneAsync(string sceneName, params object[] param)
	{
		InputBinder.Bind();

		var fade = await layerHandler.AddAsync<FadeLayer>(LayerType.System, SubLayerType.Blackout);
		await fade.FadeIn();

		await sceneHandler.LoadAsync(sceneName, this, param);

		await UnityEngine.Resources.UnloadUnusedAssets();
		await UniTask.Run(() => System.GC.Collect(0, System.GCCollectionMode.Optimized));

		await fade.FadeOut();
		fade.Close();

		InputBinder.Unbind();
	}
	public async UniTask AddSceneAsync(string sceneName, params object[] param)
	{
		InputBinder.Bind();
		await sceneHandler.AddAsync(sceneName, this, param);
		InputBinder.Unbind();
	}
	public async UniTask UnloadSceneAsync(string sceneName)
	{
		InputBinder.Bind();
		await sceneHandler.UnloadAsync(sceneName);
		InputBinder.Unbind();
	}

	public async UniTask<AbstractSubLayer> AddLayerAsync(LayerType layer, SubLayerType subLayer)
	{
		InputBinder.Bind();
		var sub = await layerHandler.AddAsync(layer, subLayer);
		InputBinder.Unbind();
		return sub;
	}
	public async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer
	{
		InputBinder.Bind();
		var sub = await layerHandler.AddAsync<T>(layer, subLayer);
		InputBinder.Unbind();
		return sub;
	}
}
