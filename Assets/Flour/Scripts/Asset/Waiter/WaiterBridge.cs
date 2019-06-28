using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flour.Asset
{
	internal class WaiterBridge
	{
		internal delegate void AddRequestDelegate(string assetBundleName, string[] dependencies);
		internal delegate void CleanRequestDelegate(string assetBundleName);

		internal delegate bool ContainsRequestDelegate(string assetBundleName);
		internal delegate IEnumerable<IAssetRequest> GetRequestsDelegate(string assetBundleName);
		internal delegate void WaiterDispose();

		public AssetBundleManifest Manifest { get; private set; }
		public AssetBundleSizeManifest SizeManiefst { get; private set; }

		private AddRequestDelegate addRequestDelegate;
		private CleanRequestDelegate cleanRequestDelegate;

		private Dictionary<string, ContainsRequestDelegate> containsRequestDelegates = new Dictionary<string, ContainsRequestDelegate>();
		private Dictionary<string, GetRequestsDelegate> getRequestsDelegates = new Dictionary<string, GetRequestsDelegate>();
		private List<WaiterDispose> waiterDisposes = new List<WaiterDispose>();

		public event Action<string, string, UnityEngine.Object> OnAssetLoaded = delegate { };
		public event Action<string, Exception> OnDownloadedError = delegate { };
		public event Action<string, string, Exception> OnLoadedError = delegate { };

		public WaiterBridge(AddRequestDelegate addRequest, CleanRequestDelegate cleanRequest)
		{
			addRequestDelegate = addRequest;
			cleanRequestDelegate = cleanRequest;
		}

		public void SetManifest(AssetBundleManifest manifest, AssetBundleSizeManifest sizeManifest)
		{
			Manifest = manifest;
			SizeManiefst = sizeManifest;
		}

		public void AddWaiter(string key, ContainsRequestDelegate containsRequest, GetRequestsDelegate getRequests, WaiterDispose dispose)
		{
			containsRequestDelegates[key] = containsRequest;
			getRequestsDelegates[key] = getRequests;
			waiterDisposes.Add(dispose);
		}

		public void Dispose()
		{
			waiterDisposes.ForEach(x => x());
		}

		public void AddRequest(string assetBundleName, string[] dependencies) => addRequestDelegate(assetBundleName, dependencies);
		public void CleanRequest(string assetBundleName, string[] dependencies)
		{
			if (!ContainsRequest(assetBundleName)) cleanRequestDelegate(assetBundleName);
			for (int i = 0; i < dependencies.Length; i++)
			{
				if (!ContainsRequest(dependencies[i])) cleanRequestDelegate(dependencies[i]);
			}
		}

		bool ContainsRequest(string assetBundleName)
		{
			foreach (var pair in containsRequestDelegates)
			{
				if (assetBundleName.StartsWith(pair.Key) && pair.Value(assetBundleName)) return true;
			}
			return false;
		}

		public IEnumerable<IAssetRequest> GetRequests(string assetBundleName)
		{
			foreach (var pair in getRequestsDelegates)
			{
				if (!assetBundleName.StartsWith(pair.Key)) continue;
				return pair.Value(assetBundleName);
			}
			return Enumerable.Empty<IAssetRequest>();
		}

		public void OnLoaded(string assetBundleName, string assetName, UnityEngine.Object asset) => OnAssetLoaded.Invoke(assetBundleName, assetName, asset);
		public void OnError(string assetBundleName, Exception e) => OnDownloadedError.Invoke(assetBundleName, e);
		public void OnError(string assetBundleName, string assetName, Exception e) => OnLoadedError.Invoke(assetBundleName, assetName, e);
	}
}
