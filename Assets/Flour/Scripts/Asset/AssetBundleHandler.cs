using System;
using System.Linq;
using System.IO;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Flour.Asset
{
	public class AssetBundleHandler
	{
		readonly string baseUrl;

		readonly ParallelAssetBundleDownloader downloadHandler;
		readonly AssetLoadHandler assetLoadHandler;

		WaiterBridge _waiterBridge;
		WaiterBridge waiterBridge
		{
			get
			{
				if (_waiterBridge == null) _waiterBridge = new WaiterBridge(AddRequest, CleanRequest);
				return _waiterBridge;
			}
		}

		AssetBundleManifest manifest;
		AssetBundleSizeManifest sizeManifest;

		CompositeDisposable disposable = new CompositeDisposable();

		public IReactiveProperty<float> DownloadProgress { get { return downloadHandler.Progress; } }
		public IReactiveProperty<float> AssetLoadProgress { get { return assetLoadHandler.Progress; } }

		public AssetBundleHandler(string baseUrl)
		{
			this.baseUrl = baseUrl;

			downloadHandler = new ParallelAssetBundleDownloader(baseUrl, 5, 20);
			downloadHandler.DownloadedObservable.Subscribe(OnCompleteDownload).AddTo(disposable);
			downloadHandler.ErroredObservable.Subscribe(OnDownloadError).AddTo(disposable);

			assetLoadHandler = new AssetLoadHandler();
			assetLoadHandler.LoadObservable.Subscribe(OnLoadedObject).AddTo(disposable);
			assetLoadHandler.ErrorObservable.Subscribe(OnAssetLoadError).AddTo(disposable);
		}

		public async UniTask LoadManifestAsync()
		{
#if UNITY_EDITOR && USE_LOCAL_ASSET
			await UniTask.DelayFrame(1);
			Debug.Log("use editor local asset");
#else
			manifest = await ManifestHelper.LoadManifestAsync(Path.Combine(baseUrl, "AssetBundles"));
			sizeManifest = await ManifestHelper.LoadSizeManifestAsync(Path.Combine(baseUrl, "AssetBundleSize"));
			Debug.Log("loaded AssetBundleManifest.");

			waiterBridge.SetManifest(manifest, sizeManifest);
#endif
		}

		public void Dispose()
		{
			downloadHandler.Dispose();
			assetLoadHandler.Dispose();

			waiterBridge.Dispose();

			AssetBundle.UnloadAllAssetBundles(true);
		}

		public void ResetProgressCount()
		{
			downloadHandler.ResetProgressCount();
			assetLoadHandler.ResetProgressCount();
		}

		public void AddWaiter<T>(AssetWaiter<T> waiter) where T : UnityEngine.Object
		{
			waiter.SetBridge(waiterBridge);
		}

		private void AddRequestInternal(string assetbundlePath)
		{
			if (assetLoadHandler.ContainsKey(assetbundlePath) || assetLoadHandler.ContainsKey(assetbundlePath))
			{
				return;
			}
			downloadHandler.AddRequest(new AssetBundleDownloader(assetbundlePath, manifest.GetAssetBundleHash(assetbundlePath)));
		}

		void AddRequest(string assetBundleName, string[] dependencies)
		{
			AddRequestInternal(assetBundleName);
			for (int i = 0; i < dependencies.Length; i++) AddRequestInternal(dependencies[i]);
		}
		void CleanRequest(string assetBundleName, string[] dependencies)
		{
			if (!waiterBridge.ContainsRequest(assetBundleName)) assetLoadHandler.Unload(assetBundleName);

			for (int i = 0; i < dependencies.Length; i++)
			{
				if (!waiterBridge.ContainsRequest(dependencies[i])) assetLoadHandler.Unload(dependencies[i]);
			}
		}


		void OnCompleteDownload(Tuple<string, AssetBundle> asset)
		{
			//Debug.Log($"downloaded AssetBundle => {asset.Item1}");

			assetLoadHandler.AddAssetBundle(asset.Item1, asset.Item2);

			var requests = waiterBridge.GetRequests(asset.Item1);

			if (requests.Any())
			{
				foreach (var req in requests)
				{
					if (assetLoadHandler.AllExist(req.AssetBundleName, req.Dependencies))
					{
						if (asset.Item2.GetAllScenePaths().Length > 0)
						{
							var scene = Path.GetFileNameWithoutExtension(asset.Item2.GetAllScenePaths()[0]);
							waiterBridge.OnLoaded(asset.Item1, scene, null);
						}
						else
						{
							assetLoadHandler.AddRequest(req.AssetBundleName, req.AssetName);
						}
					}
				}
			}
		}

		void OnDownloadError(Tuple<string, long, string> error)
		{
			waiterBridge.OnError(error.Item1, new Exception(error.Item3));
		}

		void OnLoadedObject(Tuple<string, string, UnityEngine.Object> asset)
		{
			//Debug.Log($"loaded Asset => {asset.Item1}.{asset.Item2}");

			waiterBridge.OnLoaded(asset.Item1, asset.Item2, asset.Item3);
		}

		void OnAssetLoadError(Tuple<string, string, Exception> error)
		{
			waiterBridge.OnError(error.Item1, error.Item2, error.Item3);
		}
	}
}

