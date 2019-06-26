using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flour.Asset
{
	internal class WaiterBridge
	{
		public string Key { get; private set; }
		public AssetBundleManifest Manifest { get; private set; }
		public AssetBundleSizeManifest SizeManiefst { get; private set; }

		public Action<string, string[]> AddRequest { get; private set; }
		public Action<string, string[]> CleanRequest { get; private set; }

		public Func<string, bool> ContainsRequest { get; private set; }
		public Func<string, IEnumerable<IAssetRequest>> GetRequests { get; private set; }
		public Action Dispose;


		public event Action<string, string, UnityEngine.Object> OnAssetLoaded = delegate { };
		public event Action<string, Exception> OnDownloadedError = delegate { };
		public event Action<string, string, Exception> OnLoadedError = delegate { };

		public WaiterBridge(string key, AssetBundleManifest manifest, AssetBundleSizeManifest sizeManifest, Action<string, string[]> addRequest, Action<string, string[]> cleanRequest)
		{
			Key = key;

			Manifest = manifest;
			SizeManiefst = sizeManifest;
			AddRequest = addRequest;
			CleanRequest = cleanRequest;
		}

		public void SetFunc(Func<string, bool> containtsRequest, Func<string, IEnumerable<IAssetRequest>> getRequests, Action dispose)
		{
			ContainsRequest = containtsRequest;
			GetRequests = getRequests;
			Dispose = dispose;
		}


		public void OnLoaded(string assetBundleName, string assetName, UnityEngine.Object asset) => OnAssetLoaded.Invoke(assetBundleName, assetName, asset);
		public void OnError(string assetBundleName, Exception e) => OnDownloadedError.Invoke(assetBundleName, e);
		public void OnError(string assetBundleName, string assetName, Exception e) => OnLoadedError.Invoke(assetBundleName, assetName, e);
	}
}
