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
		readonly Subject<Tuple<string, string, Exception>> erroredSubject = new Subject<Tuple<string, string, Exception>>();

		internal IObservable<Tuple<string, string, UnityEngine.Object>> LoadObservable { get { return loadedSubject; } }
		internal IObservable<Tuple<string, string, Exception>> ErrorObservable { get { return erroredSubject; } }

		IDisposable updateDisposable;

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

		void StartUpdate()
		{
			if (updateDisposable != null)
			{
				return;
			}
			updateDisposable = Observable.FromCoroutine(EveryUpdate).Subscribe();
		}
		void StopUpdate()
		{
			if (updateDisposable != null)
			{
				updateDisposable.Dispose();
				updateDisposable = null;
			}
		}

		IEnumerator EveryUpdate()
		{
			while (true)
			{
				for (int i = requests.Count - 1; i >= 0; i--)
				{
					var req = requests[i];
					if (req.Item3.isDone)
					{
						loadedSubject.OnNext(Tuple.Create(req.Item1, req.Item2, req.Item3.asset));
						requests.Remove(req);
					}
				}

				if (requests.Count <= 0)
				{
					StopUpdate();
				}

				yield return waitForSeconds;
			}
		}


		public bool ContainsKey(string path)
		{
			return assetBundles.ContainsKey(path);
		}
		public bool AllExist(string assetBundleName, string[] dependencies)
		{
			return ContainsKey(assetBundleName) && dependencies.All(x => ContainsKey(x));
		}

		public void Unload(string assetBundleName)
		{
			if (!assetBundles.ContainsKey(assetBundleName))
			{
				return;
			}
			assetBundles[assetBundleName].Unload(false);
			assetBundles.Remove(assetBundleName);
			Debug.Log($"unload AssetBundle => {assetBundleName}");
		}

		public void AddAssetBundle(string path, AssetBundle assetBundle)
		{
			assetBundles[path] = assetBundle;
		}

		public void AddRequest(string path, string assetName)
		{
			if (!ContainsKey(path))
			{
				Debug.LogWarning($"Missing AssetBundle for requested Asset => {path}");
				return;
			}
			if (!requests.Any(x => x.Item1 == path && x.Item2 == assetName))
			{
				if (!assetBundles[path].GetAllAssetNames().Any(x => Path.GetFileNameWithoutExtension(x) == assetName))
				{
					erroredSubject.OnNext(Tuple.Create(path, assetName, new Exception("no asset in AssetBundle.")));
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
