using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flour.Asset
{
	public interface IWaiter
	{
		string Key { get; }
		void SetHandler(AssetBundleManifest manifest, AssetBundleSizeManifest sizeManifest, Action<string, string[]> addRequest, Action<string, string[]> cleanRequest);

		bool ContainsRequest(string assetBundleName);
		IEnumerable<IAssetRequest> GetRequests(string assetBundleName);
		void OnLoaded(string assetBundleName, string assetName, UnityEngine.Object asset);
		void OnError(string assetBundleName, Exception e);
		void OnError(string assetBundleName, string assetName, Exception e);
	}
}
