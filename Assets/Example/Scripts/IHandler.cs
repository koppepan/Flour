using UniRx.Async;
using Flour.Layer;

public interface IOperationBundler
{
	void ApplicationQuit();
	SaveData SaveData { get; }
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
	UniTask LoadSceneAsync(SceneType sceneType, params object[] args);
	UniTask AddSceneAsync(string sceneName, params object[] args);
	UniTask UnloadSceneAsync(string sceneName);
}

public interface ILayerHandler
{
	UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer;
	UniTask<T> AddLayerOverlappingAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer;
}

