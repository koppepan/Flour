using UnityEngine;
using UniRx.Async;
using Flour;

namespace Example
{
	public interface IOperationBundler
	{
		void ApplicationQuit();

		UserPrefs<SaveKey> UserPrefs { get; }
		TemporaryData<TemporaryKey> TemporaryData { get; }

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
		void AddSceneLayer(Transform sceneObj, Camera camera);
		UniTask<T> AddSceneLayerAsync<T>(SubLayerType subLayer) where T : AbstractSubLayer;

		UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer;
		UniTask<T> AddLayerOverlappingAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer;
	}
}
