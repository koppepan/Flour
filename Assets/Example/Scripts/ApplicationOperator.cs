using Flour;
using System;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Example
{
	using LayerHandler = Flour.Layer.LayerHandler<LayerType, SubLayerType>;
	using SceneHandler = Flour.Scene.SceneHandler<Tuple<IOperationBundler, AssetHandler>>;

	sealed partial class ApplicationOperator : IDisposable, IOperationBundler, ISceneHandler, ILayerHandler
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

		readonly AssetHandler assetHandler;

		readonly SceneHandler sceneHandler;
		readonly LayerHandler layerHandler;

		public UserPrefs<SaveKey> UserPrefs { get; private set; } = new UserPrefs<SaveKey>();
		public TemporaryData<TemporaryKey> TemporaryData { get; private set; } = new TemporaryData<TemporaryKey>();

		public IInputBinder InputBinder { get; private set; } = new UIInputBinder();
		public ISceneHandler SceneHandler { get { return this; } }
		public ILayerHandler LayerHandler { get { return this; } }

		private IDisposable disposable;

		public ApplicationOperator(
			Action onApplicationQuit,
			AssetHandler assetHandler,
			SceneHandler sceneHandler, LayerHandler layerHandler,
			params SubLayerSourceRepository[] subLayerRepositories
			)
		{
			this.onApplicationQuit = onApplicationQuit;

			this.assetHandler = assetHandler;

			this.sceneHandler = sceneHandler;
			this.layerHandler = layerHandler;

			this.subLayerRepositories = subLayerRepositories;

			disposable = this.assetHandler.ErrorObservable.Subscribe(OnAssetLoadError);
		}

		public void Dispose()
		{
			UserPrefs.Dispose();
			TemporaryData.Dispose();

			assetHandler.Dispose();

			disposable?.Dispose();
			disposable = null;
		}

		public void ApplicationPause(bool pause) => sceneHandler.ApplicationPause(pause);
		public void ApplicationQuit() => onApplicationQuit?.Invoke();

		public void OnBack()
		{
			if (InputBinder.Binded) return;
			if (layerHandler.OnBack()) return;
			if (sceneHandler.OnBack()) return;
		}

		public async UniTask ResourceCompress()
		{
			assetHandler.Compress();
			await Resources.UnloadUnusedAssets();
			await UniTask.Run(() => GC.Collect(0, GCCollectionMode.Optimized));
		}

		private void OnAssetLoadError(Flour.Asset.LoadError error)
		{
			Debug.LogError(error.ToString());
		}
	}
}
