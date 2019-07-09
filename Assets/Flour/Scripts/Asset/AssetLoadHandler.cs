using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UniRx;

namespace Flour.Asset
{
	internal class AssetLoadHandler
	{
		readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

		readonly Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();
		readonly List<Tuple<string, string, AssetBundleRequest>> requests = new List<Tuple<string, string, AssetBundleRequest>>();

		readonly Subject<Tuple<string, string, UnityEngine.Object>> loadedSubject = new Subject<Tuple<string, string, UnityEngine.Object>>();
		readonly Subject<Tuple<ErrorType, string, string, string>> erroredSubject = new Subject<Tuple<ErrorType, string, string, string>>();

		internal IObservable<Tuple<string, string, UnityEngine.Object>> LoadObservable { get { return loadedSubject; } }
		internal IObservable<Tuple<ErrorType, string, string, string>> ErrorObservable { get { return erroredSubject; } }

		CompositeDisposable updateDisposables;

		int loadedCount = 0;
		private FloatReactiveProperty loadedCountProperty = new FloatReactiveProperty(0);
		public IReactiveProperty<float> LoadedCount { get { return loadedCountProperty; } }

		public void Dispose()
		{
			StopUpdate();

			loadedSubject.OnCompleted();
			loadedSubject.Dispose();
			erroredSubject.OnCompleted();
			erroredSubject.Dispose();

			requests.Clear();
			foreach (var asset in assetBundles)
			{
				asset.Value.Unload(true);
			}
			assetBundles.Clear();
		}

		internal void ResetProgressCount()
		{
			loadedCount = 0;
		}
		void StartUpdate()
		{
			if (updateDisposables != null)
			{
				return;
			}
			updateDisposables = new CompositeDisposable();

			Observable.FromCoroutine(EveryUpdate).Subscribe().AddTo(updateDisposables);
			Observable.EveryLateUpdate().Subscribe(UpdateProgress).AddTo(updateDisposables);
		}
		void StopUpdate()
		{
			if (updateDisposables != null)
			{
				updateDisposables.Dispose();
				updateDisposables = null;
			}
		}

		IEnumerator EveryUpdate()
		{
			while (true)
			{
				if (requests.Count <= 0)
				{
					StopUpdate();
				}

				for (int i = requests.Count - 1; i >= 0; i--)
				{
					var req = requests[i];
					if (req.Item3.isDone)
					{
						requests.Remove(req);
						loadedSubject.OnNext(Tuple.Create(req.Item1, req.Item2, req.Item3.asset));

						loadedCount++;
						UpdateProgress(0);
					}
				}

				yield return waitForSeconds;
			}
		}

		void UpdateProgress(long _)
		{
			float currentProgress = 0;
			for (int i = 0; i < requests.Count; i++)
			{
				if (requests[i].Item3.isDone) continue;
				currentProgress += requests[i].Item3.progress;
			}
			loadedCountProperty.Value = loadedCount + currentProgress;
		}


		public bool ContainsKey(params string[] paths)
		{
			for (int i = 0; i < paths.Length; i++)
			{
				if (!assetBundles.ContainsKey(paths[i])) return false;
			}
			return true;
		}
		public bool AllExist(string[] assetBundleNames)
		{
			for (int i = 0; i < assetBundleNames.Length; i++)
			{
				if (!ContainsKey(assetBundleNames[i])) return false;
			}
			return true;
		}

		public void Unload(string assetBundleName)
		{
			if (!assetBundles.ContainsKey(assetBundleName))
			{
				return;
			}
			if (requests.Any(x => x.Item1.Equals(assetBundleName, StringComparison.Ordinal)))
			{
				return;
			}
			assetBundles[assetBundleName].Unload(false);
			assetBundles.Remove(assetBundleName);
			//Debug.Log($"unload AssetBundle => {assetBundleName}");
		}

		public void AddAssetBundle(string path, AssetBundle assetBundle)
		{
			assetBundles[path] = assetBundle;
		}

		public void AddRequest(string path, string assetName)
		{
			if (!ContainsKey(path))
			{
				erroredSubject.OnNext(Tuple.Create(ErrorType.MissingAssetBundle, path, assetName, $"Missing AssetBundle for requested Asset => {path}"));
				return;
			}
			if (!requests.Any(x => x.Item1.Equals(path, StringComparison.Ordinal) && x.Item2.Equals(assetName, StringComparison.Ordinal)))
			{
				if (!assetBundles[path].GetAllAssetNames().Any(x => Path.GetFileNameWithoutExtension(x).Equals(assetName, StringComparison.Ordinal)))
				{
					erroredSubject.OnNext(Tuple.Create(ErrorType.NotFoundAsset, path, assetName, "no asset in AssetBundle."));
				}
				else
				{
					requests.Add(Tuple.Create(path, assetName, assetBundles[path].LoadAssetAsync(assetName)));
				}
			}

			if (requests.Count > 0)
			{
				StartUpdate();
			}
		}
	}
}
