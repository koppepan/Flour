using UniRx.Async;
using Flour.Layer;

public interface IOperationBundler
{
	void ApplicationQuit();
	Flour.UserPrefs UserPrefs { get; }
	IInputBinder InputBinder { get; }
	ISceneHandler SceneHandler { get; }
	ILayerHandler LayerHandler { get; }
}

public interface IInputBinder
{
	bool Binded { get; }
	void Bind();
	void Unbind();
}

public interface ISceneHandler
{
	UniTask LoadSceneAsync(string sceneName, params object[] args);
	UniTask AddSceneAsync(string sceneName, params object[] args);
	UniTask UnloadSceneAsync(string sceneName);
}

public interface ILayerHandler
{
	UniTask<AbstractSubLayer> AddLayerAsync(LayerType layer, SubLayerType subLayer);
	UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer;
}

