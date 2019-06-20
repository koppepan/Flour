﻿using UnityEngine;
using UnityEngine.Networking;

namespace Flour.Asset
{
	internal class ParallelAssetBundleDownloader : Net.ParallelWebRequest<AssetBundle>
	{
		public ParallelAssetBundleDownloader(string baseUrl, int parallel, int timeout) : base(baseUrl, parallel, timeout)
		{
		}
	}

	internal struct AssetBundleDownloader : Net.IDownloader<AssetBundle>
	{
		public string Path { get; private set; }
		public UnityWebRequest Request { get; private set; }

		Hash128 hash;

		public AssetBundleDownloader(string path, Hash128 hash)
		{
			Path = path;
			Request = null;
			this.hash = hash;
		}

		public void Send(string baseUrl, int timeout)
		{
			Request = UnityWebRequestAssetBundle.GetAssetBundle(System.IO.Path.Combine(baseUrl, Path), hash);
			Request.timeout = timeout;
			Request.SendWebRequest();
		}
		public AssetBundle GetContent()
		{
			return DownloadHandlerAssetBundle.GetContent(Request);
		}
		public void Dispose()
		{
			Request.Dispose();
		}
	}
}