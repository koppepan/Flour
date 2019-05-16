using UnityEngine.EventSystems;
using UniRx.Async;

using Flour;
using Flour.Scene;
using Flour.UI;

public class ApplicationOperator : IOperationBundler, ISceneHandler, ILayerHandler
{
	System.Action onApplicationQuit;

	SceneHandler sceneHandler;
	LayerHandler layerHandler;

	public ISceneHandler SceneHandler { get { return this; } }
	public ILayerHandler LayerHandler { get { return this; } }

	public ApplicationOperator(System.Action onApplicationQuit, SceneHandler sceneHandler, LayerHandler layerHandler)
	{
		this.onApplicationQuit = onApplicationQuit;
		this.sceneHandler = sceneHandler;
		this.layerHandler = layerHandler;
	}

	public void ApplicationQuit()
	{
		onApplicationQuit?.Invoke();
	}

	public void OnBack()
	{
		if (layerHandler.OnBack())
		{
			return;
		}
		if (sceneHandler.OnBack())
		{
			return;
		}
	}

	private async UniTask SafeInputTask(System.Func<UniTask> func)
	{
		var eventSystem = EventSystem.current;
		if (eventSystem != null) eventSystem.enabled = false;

		await func();

		if (eventSystem != null) eventSystem.enabled = true;
	}
	private async UniTask<T> SafeInputTask<T>(System.Func<UniTask<T>> func)
	{
		var eventSystem = EventSystem.current;
		if (eventSystem != null) eventSystem.enabled = false;

		var t = await func();

		if (eventSystem != null) eventSystem.enabled = true;

		return t;
	}

	public async UniTask LoadSceneAsync(string sceneName, params object[] param)
	{
		await SafeInputTask(async () =>
		{
			var fade = await layerHandler.AddAsync<FadeLayer>(LayerType.System, SubLayerType.Blackout);
			await fade.FadeIn();
			await sceneHandler.LoadAsync(sceneName, this, param);
			await fade.FadeOut();
			fade.Close();
		});
	}
	public async UniTask AddSceneAsync(string sceneName, params object[] param)
	{
		await SafeInputTask(async () =>
		{
			await sceneHandler.AddAsync(sceneName, this, param);
		});
	}
	public async UniTask UnloadSceneAsync(string sceneName)
	{
		await SafeInputTask(async () =>
		{
			await sceneHandler.UnloadAsync(sceneName);
		});
	}

	public async UniTask<AbstractSubLayer> AddLayerAsync(LayerType layer, SubLayerType subLayer)
	{
		return await SafeInputTask(async () =>
		{
			return await layerHandler.AddAsync(layer, subLayer);
		});
	}
	public async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer
	{
		return await SafeInputTask<T>(async () =>
		{
			return await layerHandler.AddAsync<T>(layer, subLayer);
		});
	}
}
