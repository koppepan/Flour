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
		string baseUrl;

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
		AssetBundleCrcManifest crcManifest;

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

		public void ChangeBaseUrl(string baseUrl)
		{
			this.baseUrl = baseUrl;
			downloadHandler.ChangeBaseUrl(baseUrl);
		}

		public async UniTask LoadManifestAsync(string manifestName, string sizeManifestName, string crcManifestName = "")
		{
#if UNITY_EDITOR && USE_LOCAL_ASSET
			await UniTask.DelayFrame(1);
			Debug.Log("use editor local asset");
#else
			manifest = await ManifestHelper.LoadManifestAsync(Path.Combine(baseUrl, manifestName));
			sizeManifest = await ManifestHelper.LoadSizeManifestAsync(Path.Combine(baseUrl, sizeManifestName));

			if (!string.IsNullOrEmpty(crcManifestName))
			{
				crcManifest = await ManifestHelper.LoadCrcManifestAsync(Path.Combine(baseUrl, crcManifestName));
			}
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

		void AddRequest(string[] assetBundleNames)
		{
			for (int i = 0; i < assetBundleNames.Length; i++)
			{
				if (assetLoadHandler.ContainsKey(assetBundleNames[i]))
				{
					continue;
				}
				var hash = manifest.GetAssetBundleHash(assetBundleNames[i]);
				var crc = crcManifest == null ? 0 : crcManifest.GetCrc(assetBundleNames[i]);
				downloadHandler.AddRequest(new AssetBundleDownloader(assetBundleNames[i], hash, crc));
			}
		}
		void CleanRequest(string assetBundleName)
		{
			assetLoadHandler.Unload(assetBundleName);
		}

		void OnCompleteDownload(Tuple<string, AssetBundle> asset)
		{
			//Debug.Log($"downloaded AssetBundle => {asset.Item1}");

			assetLoadHandler.AddAssetBundle(asset.Item1, asset.Item2);

			var requests = waiterBridge.GetRequests(asset.Item1);
			if (!requests.Any()) return;

			foreach (var req in requests)
			{
				if (assetLoadHandler.AllExist(req.AssetBundleNames))
				{
					if (asset.Item2.GetAllScenePaths().Length > 0)
					{
						waiterBridge.OnLoaded(asset.Item1, req.AssetName, null);
					}
					else
					{
						assetLoadHandler.AddRequest(asset.Item1, req.AssetName);
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

