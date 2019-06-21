using System;
using System.Collections;
using System.Collections.Generic;
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

		readonly List<IWaiter> waiters = new List<IWaiter>();

		AssetBundleManifest manifest;

		CompositeDisposable disposable = new CompositeDisposable();

		public IObservable<float> Progress { get { return downloadHandler.Progress; } }
		public IReactiveProperty<bool> Running { get { return downloadHandler.Running; } }

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
			Debug.Log("loaded AssetBundleManifest.");
#endif
		}

		public void Dispose()
		{
			downloadHandler.Dispose();
			assetLoadHandler.Dispose();

			AssetBundle.UnloadAllAssetBundles(true);
		}

		public void AddWaiter(IWaiter waiter)
		{
			waiter.SetHandler(manifest, AddRequest, CleanRequest);
			waiters.Add(waiter);
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
			if (waiters.Any(x => !x.ContainsRequest(assetBundleName)))
			{
				assetLoadHandler.Unload(assetBundleName);
			}
			for (int i = 0; i < dependencies.Length; i++)
			{
				if (waiters.Any(x => !x.ContainsRequest(dependencies[i])))
				{
					assetLoadHandler.Unload(dependencies[i]);
				}
			}
		}


		void OnCompleteDownload(Tuple<string, AssetBundle> asset)
		{
			Debug.Log($"downloaded AssetBundle => {asset.Item1}");

			assetLoadHandler.AddAssetBundle(asset.Item1, asset.Item2);

			for (int i = 0; i < waiters.Count; i++)
			{
				var requests = waiters[i].GetRequests(asset.Item1);
				if (requests.Any())
				{
					foreach (var req in requests)
					{
						if (assetLoadHandler.AllExist(req.AssetBundleName, req.Dependencies))
						{
							if (asset.Item2.GetAllScenePaths().Length > 0)
							{
								var scene = Path.GetFileNameWithoutExtension(asset.Item2.GetAllScenePaths()[0]);
								waiters[i].OnLoaded(asset.Item1, scene, null);
							}
							else
							{
								assetLoadHandler.AddRequest(req.AssetBundleName, req.AssetName);
							}
						}
					}
				}
			}
		}

		void OnDownloadError(Tuple<string, long, string> error)
		{
			for (int i = 0; i < waiters.Count; i++)
			{
				if (error.Item1.StartsWith(waiters[i].Key))
				{
					waiters[i].OnError(error.Item1, new Exception(error.Item3));
				}
			}
		}

		void OnLoadedObject(Tuple<string, string, UnityEngine.Object> asset)
		{
			Debug.Log($"loaded Asset => {asset.Item1}.{asset.Item2}");

			for (int i = 0; i < waiters.Count; i++)
			{
				if (asset.Item1.StartsWith(waiters[i].Key))
				{
					waiters[i].OnLoaded(asset.Item1, asset.Item2, asset.Item3);
				}
			}
		}

		void OnAssetLoadError(Tuple<string, string, Exception> error)
		{
			for (int i = 0; i < waiters.Count; i++)
			{
				if (error.Item1.StartsWith(waiters[i].Key))
				{
					waiters[i].OnError(error.Item1, error.Item2, error.Item3);
				}
			}
		}
	}
}

