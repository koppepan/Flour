﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;

namespace Flour.Asset
{
	public class AssetWaiter<T> where T : UnityEngine.Object
	{
		public string Key { get; private set; }

		readonly List<Request<T>> requests = new List<Request<T>>();

		WaiterBridge bridge;

		public AssetWaiter(string key) => Key = key;

		internal void SetBridge(WaiterBridge bridge)
		{
			this.bridge = bridge;

			this.bridge.OnAssetLoaded += OnLoaded;
			this.bridge.OnDownloadedError += OnError;
			this.bridge.OnLoadedError += OnError;

			this.bridge.SetFunc(ContainsRequest, GetRequests, Dispose);
		}

		internal void Dispose()
		{
			this.bridge.OnAssetLoaded -= OnLoaded;
			this.bridge.OnDownloadedError -= OnError;
			this.bridge.OnLoadedError -= OnError;
		}

		public long GetSize(string assetBundleName)
		{
			var ab = string.Intern(Path.Combine(Key, assetBundleName));
			return bridge.SizeManiefst.GetSize(ab);
		}

		protected virtual T GetAsset(UnityEngine.Object asset)
		{
			return asset != null ? (T)asset : null;
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

			var req = new Request<T>(ab, bridge.Manifest.GetAllDependencies(ab), assetName, new Subject<T>());
			requests.Add(req);

			bridge.AddRequest(req.AssetBundleName, req.Dependencies);
			return req.subject;
		}

		bool ContainsRequest(string assetBundleName)
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
		IEnumerable<IAssetRequest> GetRequests(string assetBundleName)
		{
			for (int i = 0; i < requests.Count; i++)
			{
				if (requests[i].Containts(assetBundleName)) yield return requests[i];
			}
		}

		void OnLoaded(string assetBundleName, string assetName, UnityEngine.Object asset)
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
					bridge.CleanRequest(req.AssetBundleName, req.Dependencies);
				}
			}
		}
		void OnError(string assetBundleName, Exception e)
		{
			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var req = requests[i];
				if (req.AssetBundleName == assetBundleName)
				{
					req.subject.OnError(new Exception(assetBundleName, e));
					requests.Remove(req);
					bridge.CleanRequest(req.AssetBundleName, req.Dependencies);
				}
			}
		}
		void OnError(string assetBundleName, string assetName, Exception e)
		{
			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var req = requests[i];
				if (req.AssetBundleName == assetBundleName && req.AssetName == assetName)
				{
					req.subject.OnError(new Exception($"{assetBundleName}.{assetName}", e));
					requests.Remove(req);
					bridge.CleanRequest(req.AssetBundleName, req.Dependencies);
				}
			}
		}
	}
}
