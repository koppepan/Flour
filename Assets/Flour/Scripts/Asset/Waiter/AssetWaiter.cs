using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UniRx;

namespace Flour.Asset
{
	public class AssetWaiter<T> : IWaiter where T : UnityEngine.Object
	{
		public string Key { get; private set; }

		readonly List<Request<T>> requests = new List<Request<T>>();

		AssetBundleManifest manifest;
		AssetBundleSizeManifest sizeManifest;
		Action<string, string[]> addRequest;
		Action<string, string[]> cleanRequest;

		public AssetWaiter(string key) => Key = key;

		public void SetHandler(AssetBundleManifest manifest, AssetBundleSizeManifest sizeManifest, Action<string, string[]> addRequest, Action<string, string[]> cleanRequest)
		{
			this.manifest = manifest;
			this.sizeManifest = sizeManifest;
			this.addRequest = addRequest;
			this.cleanRequest = cleanRequest;
		}

		public bool ContainsRequest(string assetBundleName)
		{
			for (int i = 0; i < requests.Count; i++)
			{
				if (requests[i].AssetBundleName == assetBundleName || requests[i].Dependencies.Contains(assetBundleName))
				{
					return true;
				}
			}
			return false;
		}

		public long GetSize(string assetBundleName)
		{
			var ab = string.Intern(Path.Combine(Key, assetBundleName));
			return sizeManifest.GetSize(ab);
		}

		public IEnumerable<IAssetRequest> GetRequests(string assetBundleName)
		{
			for (int i = 0; i < requests.Count; i++)
			{
				if (requests[i].Containts(assetBundleName)) yield return requests[i];
			}
		}

		public IObservable<T> LoadAsync(string assetName, string valiant = "")
		{
			return LoadAsync(assetName, assetName, valiant);
		}

		public virtual IObservable<T> LoadAsync(string assetBundleName, string assetName, string valiant = "")
		{
			if (!string.IsNullOrEmpty(valiant))
			{
				assetBundleName += $".{valiant}";
			}

			var ab = string.Intern(Path.Combine(Key, assetBundleName));

#if UNITY_EDITOR && USE_LOCAL_ASSET
			var localAsset = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(ab, assetName);
			if (localAsset.Length > 0)
			{
				return Observable.Return<T>((T)UnityEditor.AssetDatabase.LoadAssetAtPath(localAsset[0], typeof(T)));
			}
#endif

			var req = new Request<T>(ab, manifest.GetAllDependencies(ab), assetName, new Subject<T>());
			requests.Add(req);

			addRequest.Invoke(req.AssetBundleName, req.Dependencies);
			return req.subject;
		}

		protected virtual T GetAsset(UnityEngine.Object asset)
		{
			return asset != null ? (T)asset : null;
		}

		public void OnLoaded(string assetBundleName, string assetName, UnityEngine.Object asset)
		{
			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var req = requests[i];
				if (req.AssetBundleName == assetBundleName && req.AssetName == assetName)
				{
					if (req.subject.HasObservers)
					{
						req.subject.OnNext(GetAsset(asset));
						req.subject.OnCompleted();
					}
					else
					{
						req.subject.Dispose();
					}

					requests.Remove(req);
					cleanRequest(req.AssetBundleName, req.Dependencies);
				}
			}
		}
		public void OnError(string assetBundleName, Exception e)
		{
			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var req = requests[i];
				if (req.AssetBundleName == assetBundleName)
				{
					req.subject.OnError(new Exception(assetBundleName, e));
					requests.Remove(req);
					cleanRequest(req.AssetBundleName, req.Dependencies);
				}
			}
		}
		public void OnError(string assetBundleName, string assetName, Exception e)
		{
			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var req = requests[i];
				if (req.AssetBundleName == assetBundleName && req.AssetName == assetName)
				{
					req.subject.OnError(new Exception($"{assetBundleName}.{assetName}", e));
					requests.Remove(req);
					cleanRequest(req.AssetBundleName, req.Dependencies);
				}
			}
		}
	}
}
