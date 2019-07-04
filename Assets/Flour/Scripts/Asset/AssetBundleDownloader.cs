using UnityEngine;
using UnityEngine.Networking;

namespace Flour.Asset
{
	internal class ParallelAssetBundleDownloader : Net.ParallelWebRequest<AssetBundle>
	{
		public ParallelAssetBundleDownloader(string baseUrl, int parallel, int timeout) : base(baseUrl, parallel, timeout)
		{
		}
	}

	internal class AssetBundleDownloader : Net.IDownloader<AssetBundle>
	{
		public string FilePath { get; private set; }

		public bool IsDone { get { return request.isDone; } }
		public bool IsError { get { return request.isHttpError || request.isNetworkError; } }
		public long ResponseCode { get { return request.responseCode; } }
		public string Error { get { return request.error; } }

		public float Progress { get { return request.downloadProgress; } }

		UnityWebRequest request = null;
		Hash128 hash;
		uint crc;

		public AssetBundleDownloader(string path, Hash128 hash, uint crc = 0)
		{
			FilePath = path;

			this.hash = hash;
			this.crc = crc;
		}

		public void Send(string baseUrl, int timeout)
		{
			var cachedAb = new CachedAssetBundle(FilePath, hash);

			Caching.ClearOtherCachedVersions(cachedAb.name, cachedAb.hash);

			request = UnityWebRequestAssetBundle.GetAssetBundle(System.IO.Path.Combine(baseUrl, FilePath), cachedAb, crc);
			request.timeout = timeout;
			request.SendWebRequest();
		}
		public void Update() { }
		public AssetBundle GetContent()
		{
			return DownloadHandlerAssetBundle.GetContent(request);
		}
		public void Dispose()
		{
			request?.Dispose();
		}
	}
}
