using System.IO;
using UnityEngine.Networking;

namespace Flour.Net
{
	public class ParalleFileDownloader : ParallelWebRequest<string>
	{
		public ParalleFileDownloader(string baseUrl, int parallel, int timeout) : base(baseUrl, parallel, timeout)
		{
		}
	}

	public struct FileDownloader : IDownloader<string>
	{
		public string FilePath { get; private set; }

		public bool IsDone { get { return request.isDone; } }
		public bool IsError { get { return request.isHttpError || request.isNetworkError; } }
		public long ResponseCode { get { return request.responseCode; } }
		public string Error { get { return request.error; } }

		public float Progress { get { return request.downloadProgress; } }

		UnityWebRequest request;

		public FileDownloader(string path)
		{
			FilePath = path;
			request = null;
		}

		public void Send(string baseUrl, int timeout)
		{
			request = UnityWebRequest.Get(Path.Combine(baseUrl, FilePath));
			request.timeout = timeout;
			request.SendWebRequest();
		}
		public void Update() { }
		public string GetContent()
		{
			return request.downloadHandler.text;
		}
		public void Dispose()
		{
			request.Dispose();
		}
	}
}
