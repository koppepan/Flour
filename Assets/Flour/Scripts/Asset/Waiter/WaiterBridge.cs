using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flour.Asset
{
	internal class WaiterBridge
	{
		internal delegate void AddRequestDelegate(string[] assetBundleNames);
		internal delegate void CleanRequestDelegate(string assetBundleName);

		internal delegate bool ContainsRequestDelegate(string assetBundleName);
		internal delegate IEnumerable<IAssetRequest> GetRequestsDelegate(string assetBundleName);
		internal delegate void WaiterDispose();

		public AssetBundleManifest Manifest { get; private set; }
		public AssetBundleSizeManifest SizeManiefst { get; private set; }

		private AddRequestDelegate addRequestDelegate;
		private CleanRequestDelegate cleanRequestDelegate;

		private List<Tuple<string, ContainsRequestDelegate>> contains = new List<Tuple<string, ContainsRequestDelegate>>();
		private List<Tuple<string, GetRequestsDelegate>> requests = new List<Tuple<string, GetRequestsDelegate>>();
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
			contains.Add(Tuple.Create(key, containsRequest));
			requests.Add(Tuple.Create(key, getRequests));
			waiterDisposes.Add(dispose);
		}

		public void Dispose()
		{
			waiterDisposes.ForEach(x => x());
		}

		public void AddRequest(string[] assetBundleNames) => addRequestDelegate(assetBundleNames);
		public void CleanRequest(string[] assetBundleNames)
		{
			for (int i = 0; i < assetBundleNames.Length; i++)
			{
				if (!ContainsRequest(assetBundleNames[i])) cleanRequestDelegate(assetBundleNames[i]);
			}
		}

		bool ContainsRequest(string assetBundleName)
		{
			for (int i = 0; i < contains.Count; i++)
			{
				if (assetBundleName.StartsWith(contains[i].Item1) && contains[i].Item2(assetBundleName)) return true;
			}
			return false;
		}

		public IEnumerable<IAssetRequest> GetRequests(string assetBundleName)
		{
			for (int i = 0; i < requests.Count; i++)
			{
				if (!assetBundleName.StartsWith(requests[i].Item1)) continue;
				return requests[i].Item2(assetBundleName);
			}
			return Enumerable.Empty<IAssetRequest>();
		}

		public void OnLoaded(string assetBundleName, string assetName, UnityEngine.Object asset) => OnAssetLoaded.Invoke(assetBundleName, assetName, asset);
		public void OnError(string assetBundleName, Exception e) => OnDownloadedError.Invoke(assetBundleName, e);
		public void OnError(string assetBundleName, string assetName, Exception e) => OnLoadedError.Invoke(assetBundleName, assetName, e);
	}
}
