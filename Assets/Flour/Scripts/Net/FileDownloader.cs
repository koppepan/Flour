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
		public string Path { get; private set; }
		public UnityWebRequest Request { get; private set; }

		public FileDownloader(string path)
		{
			Path = path;
			Request = null;
		}

		public void Send(string baseUrl, int timeout)
		{
			Request = UnityWebRequest.Get(System.IO.Path.Combine(baseUrl, Path));
			Request.timeout = timeout;
			Request.SendWebRequest();
		}
		public string GetContent()
		{
			return Request.downloadHandler.text;
		}
		public void Dispose()
		{
			Request.Dispose();
		}
	}
}
