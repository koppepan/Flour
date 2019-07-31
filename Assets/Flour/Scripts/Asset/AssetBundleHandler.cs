using System;
using System.IO;
using System.Linq;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Flour.Asset
{
	public class AssetBundleHandler
	{
		string baseUrl;

		readonly Net.ParallelWebRequest<AssetBundle> downloadHandler;
		readonly AssetLoadHandler assetLoadHandler;

		AssetBundleManifest manifest = default;
		AssetBundleCrcManifest crcManifest = default;

		WaiterBridge _waiterBridge;
		WaiterBridge waiterBridge
		{
			get
			{
				if (_waiterBridge == null) _waiterBridge = new WaiterBridge(AddRequest, CleanRequest);
				return _waiterBridge;
			}
		}

		private readonly CompositeDisposable disposable = new CompositeDisposable();
		private readonly Subject<LoadError> errorSubject = new Subject<LoadError>();

		public IObservable<float> DownloadedCount { get { return downloadHandler.DownloadedCount; } }
		public IObservable<float> AssetLoadedCount { get { return assetLoadHandler.LoadedCount; } }

		public IObservable<LoadError> ErrorObservable { get { return errorSubject; } }

		public AssetBundleHandler(string baseUrl) : this(baseUrl, new ParallelAssetBundleDownloader(baseUrl, 5, 20)) { }

		internal AssetBundleHandler(string baseUrl, Net.ParallelWebRequest<AssetBundle> downloader)
		{
			this.baseUrl = baseUrl;
			downloadHandler = downloader;
			assetLoadHandler = new AssetLoadHandler();

			Initialize();
		}

		public void Dispose()
		{
			disposable.Dispose();

			downloadHandler.Dispose();
			assetLoadHandler.Dispose();

			errorSubject.Dispose();
			waiterBridge.Dispose();

			AssetBundle.UnloadAllAssetBundles(true);
		}

		private void Initialize()
		{
			downloadHandler.DownloadedObservable.Subscribe(OnCompleteDownload).AddTo(disposable);
			downloadHandler.ErroredObservable.Subscribe(OnDownloadError).AddTo(disposable);

			assetLoadHandler.LoadObservable.Subscribe(OnLoadedObject).AddTo(disposable);
			assetLoadHandler.ErrorObservable.Subscribe(OnAssetLoadError).AddTo(disposable);
		}

		protected virtual Net.IDownloader<AssetBundle> CreateRequest(string assetBundleName, Hash128 hash, uint crc) => new AssetBundleDownloader(assetBundleName, hash, crc);

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
			try
			{
				var result = await LoadManifestAsyncInternal(baseUrl, manifestName, sizeManifestName, crcManifestName);
				manifest = result.Item1;
				crcManifest = result.Item3;

				Debug.Log("loaded AssetBundleManifest.");
				waiterBridge.SetManifest(manifest, result.Item2);
			}
			catch (Exception e)
			{
				errorSubject.OnNext(new LoadError(ErrorType.MissingManifest, 0, "AssetBundleManifest", e));
			}
#endif
		}

		protected virtual async UniTask<Tuple<AssetBundleManifest, AssetBundleSizeManifest, AssetBundleCrcManifest>> LoadManifestAsyncInternal(string baseUrl, string manifestName, string sizeManifestName, string crcManifestName)
		{
			var (result1, result2, result3) = await UniTask.WhenAll(
				ManifestHelper.LoadManifestAsync(baseUrl, manifestName),
				ManifestHelper.LoadSizeManifestAsync(baseUrl, sizeManifestName),
				ManifestHelper.LoadCrcManifestAsync(baseUrl, crcManifestName)
				);

			return Tuple.Create(result1, result2, result3);
		}

		public Tuple<int, int> GetRequestCount()
		{
			var requests = waiterBridge.FindRequests();
			var abCount = requests.SelectMany(x => x.AssetBundleNames).Distinct().Count();
			var assetCount = requests.Select(x => x.AssetName).Distinct().Count();
			return Tuple.Create(abCount, assetCount);
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

		void AddRequest(IAssetRequest request)
		{
			if (assetLoadHandler.ContainsKey(request.AssetBundleNames))
			{
				assetLoadHandler.AddRequest(request.AssetBundleNames[0], request.AssetName, request.Type);
				return;
			}

			for (int i = 0; i < request.AssetBundleNames.Length; i++)
			{
				if (assetLoadHandler.ContainsKey(request.AssetBundleNames[i]))
				{
					continue;
				}
				var hash = manifest.GetAssetBundleHash(request.AssetBundleNames[i]);
				var crc = crcManifest == null ? 0 : crcManifest.GetCrc(request.AssetBundleNames[i]);
				downloadHandler.AddRequest(CreateRequest(request.AssetBundleNames[i], hash, crc));
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

			var requests = waiterBridge.FindRequests(asset.Item1);
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
						assetLoadHandler.AddRequest(req.AssetBundleNames[0], req.AssetName, req.Type);
					}
				}
			}
		}

		void OnLoadedObject(Tuple<string, string, UnityEngine.Object> asset)
		{
			//Debug.Log($"loaded Asset => {asset.Item1}.{asset.Item2}");

			waiterBridge.OnLoaded(asset.Item1, asset.Item2, asset.Item3);
		}

		void OnDownloadError(Tuple<string, long, string> error)
		{
			waiterBridge.OnError(error.Item1);

			var type = ErrorType.DownloadError;
			if (400 <= error.Item2 && error.Item2 < 500) type = ErrorType.ClientError;
			else if (500 <= error.Item2 && error.Item2 < 600) type = ErrorType.ServerError;

			errorSubject.OnNext(new LoadError(type, error.Item2, error.Item1, new Exception(error.Item3)));
		}

		void OnAssetLoadError(Tuple<ErrorType, string, string, string> error)
		{
			waiterBridge.OnError(error.Item2, error.Item3);

			errorSubject.OnNext(new LoadError(error.Item1, error.Item2, error.Item3, new Exception(error.Item4)));
		}
	}
}

