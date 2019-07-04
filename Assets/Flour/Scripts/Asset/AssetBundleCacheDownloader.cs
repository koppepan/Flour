﻿using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UniRx.Async;

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

	internal class AssetBundleCacheDownloader : Net.IDownloader<AssetBundle>
	{
		private enum State
		{
			Wait,
			Download,
			Load,
			Completed,
		};

		public string FilePath { get; private set; }

		public bool IsDone { get { return currentState == State.Completed; } }

		public bool IsError { get; private set; }
		public long ResponseCode { get; private set; }
		public string Error { get; private set; }

		public float Progress
		{
			get
			{
				switch (currentState)
				{
					case State.Download: return asyncOperation.progress * 0.5f;
					case State.Load: return 0.5f + (asyncOperation.progress * 0.5f);
					case State.Completed: return 1;
					default: return 0;
				}
			}
		}

		string cachePath;
		Hash128 hash;
		uint crc;

		UnityWebRequest request = null;
		State currentState = State.Wait;
		AsyncOperation asyncOperation = null;

		public AssetBundleCacheDownloader(string path, string cachePath, Hash128 hash, uint crc = 0)
		{
			FilePath = path;

			this.cachePath = Path.Combine(cachePath, $"{FilePath}.{hash}");
			this.hash = hash;
			this.crc = crc;
		}

		public void Send(string baseUrl, int timeout)
		{
			if (!hash.isValid)
			{
				SetError(true, 404, "doesn't exist in the AssetBundleManifest");
				return;
			}

			if (File.Exists(cachePath))
			{
				InvokeLoadStream();
			}
			else
			{
				request = new UnityWebRequest(Path.Combine(baseUrl, FilePath), UnityWebRequest.kHttpVerbGET, new AssetBundleDownloadHandler(cachePath), null);
				request.timeout = timeout;

				asyncOperation = request.SendWebRequest();
				currentState = State.Download;
			}

			UniTask.Run(() =>
			{
				var files = Directory.GetFiles(Path.GetDirectoryName(cachePath), $"{Path.GetFileNameWithoutExtension(cachePath)}.*");
				for (int i = 0; i < files.Length; i++)
				{
					if (!Path.GetFullPath(files[i]).Equals(Path.GetFullPath(cachePath), System.StringComparison.Ordinal))
					{
						File.Delete(files[i]);
					}
				}
			}).Forget();
		}

		private void SetError(bool isError, long responseCode, string error)
		{
			IsError = isError;
			ResponseCode = responseCode;
			Error = error;
		}

		private void InvokeLoadStream()
		{
			asyncOperation = AssetBundle.LoadFromStreamAsync(new FileStream(cachePath, FileMode.Open));
			currentState = State.Load;
		}

		public void Update()
		{
			if (currentState == State.Completed || asyncOperation == null) return;
			if (asyncOperation.isDone)
			{
				if (currentState == State.Download)
				{
					SetError(request.isHttpError || request.isNetworkError, request.responseCode, request.error);

					if (IsError)
					{
						currentState = State.Completed;
						return;
					}

					InvokeLoadStream();
				}
				else if (currentState == State.Load)
				{
					currentState = State.Completed;
				}
			}
		}
		public AssetBundle GetContent()
		{
			return ((AssetBundleCreateRequest)asyncOperation).assetBundle;
		}
		public void Dispose()
		{
			request?.Dispose();
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