using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Flour.Asset
{
	internal class ParallelAssetBundleCacheDownloader : Net.ParallelWebRequest<AssetBundle>
	{
		readonly string cachePath;

		public ParallelAssetBundleCacheDownloader(string baseUrl, string cachePath, int parallel, int timeout) : base(baseUrl, parallel, timeout)
		{
			this.cachePath = cachePath;
			if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);
		}
	}

	internal struct AssetBundleCacheDownloader : Net.IDownloader<AssetBundle>
	{
		public string Path { get; private set; }
		public UnityWebRequest Request { get; private set; }

		string cachePath;
		Hash128 hash;
		uint crc;

		public AssetBundleCacheDownloader(string path, string cachePath, Hash128 hash, uint crc = 0)
		{
			Path = path;
			Request = null;

			this.cachePath = cachePath;
			this.hash = hash;
			this.crc = crc;
		}

		public void Send(string baseUrl, int timeout)
		{
			//var cachedAb = new CachedAssetBundle(Path, hash);
			//Request = UnityWebRequestAssetBundle.GetAssetBundle(System.IO.Path.Combine(baseUrl, Path), cachedAb, crc);

			Request = new UnityWebRequest(System.IO.Path.Combine(baseUrl, Path), UnityWebRequest.kHttpVerbGET, new AssetBundleDownloadHandler(System.IO.Path.Combine(cachePath, Path)), null);

			Request.timeout = timeout;
			Request.SendWebRequest();
		}
		public AssetBundle GetContent()
		{
			return AssetBundle.LoadFromStream(new FileStream(System.IO.Path.Combine(cachePath, Path), FileMode.Open));
		}
		public void Dispose()
		{
			Request?.Dispose();
		}
	}

	internal class AssetBundleDownloadHandler : DownloadHandlerScript
	{
		FileStream fs;
		int offset = 0;
		ulong length = 0;

		public AssetBundleDownloadHandler(string cachePath) : base(new byte[256 * 1024])
		{
			if (!Directory.Exists(Path.GetDirectoryName(cachePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(cachePath));
			}
			fs = new FileStream(cachePath, FileMode.Create, FileAccess.Write);
		}

		protected override bool ReceiveData(byte[] data, int dataLength)
		{
			fs.Write(data, 0, dataLength);
			offset += dataLength;
			return true;
		}
		protected override void CompleteContent()
		{
			fs.Flush();
			fs.Close();
		}
		protected override void ReceiveContentLengthHeader(ulong contentLength)
		{
			length = contentLength;
		}
		protected override float GetProgress()
		{
			if (length == 0) return default;
			return (float)offset / length;
		}
	}
}
