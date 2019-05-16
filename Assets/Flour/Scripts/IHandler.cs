using UniRx.Async;
using Flour.UI;

namespace Flour
{
	public interface IOperationBundler
	{
		ISceneHandler SceneHandler { get; }
		ILayerHandler LayerHandler { get; }
	}

	public interface ISceneHandler
	{
		UniTask LoadSceneAsync(string sceneName, params object[] param);
		UniTask AddSceneAsync(string sceneName, params object[] param);
		UniTask UnloadSceneAsync(string sceneName);
	}

	public interface ILayerHandler
	{
		UniTask<AbstractSubLayer> AddLayerAsync(LayerType layer, SubLayerType subLayer);
		UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer;
	}

}
