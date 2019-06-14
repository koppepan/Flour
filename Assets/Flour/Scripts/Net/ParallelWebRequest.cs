using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UniRx;

namespace Flour.Net
{
	public interface IDownloader<T>
	{
		string Path { get; }
		UnityWebRequest Request { get; }

		void Send(string baseUrl, int timeout);
		T GetContent();
		void Dispose();
	}

	public abstract class ParallelWebRequest<T>
	{
		readonly string baseUrl;

		readonly int parallel;
		readonly int timeout;

		readonly IObserver<Tuple<string, T>> downloadObserver;
		readonly IObserver<Tuple<string, long>> errorObserver;

		readonly List<IDownloader<T>> waitingList = new List<IDownloader<T>>();
		readonly List<IDownloader<T>> downloaders = new List<IDownloader<T>>();

		int requestCount = 0;
		int downloadedCount = 0;

		IDisposable updateDisposable;

		public float Progress
		{
			get
			{
				if (requestCount == 0) return 0;
				return (float)downloadedCount / (float)requestCount;
			}
		}

		public ParallelWebRequest(string baseUrl, int parallel, int timeout, IObserver<Tuple<string, T>> downloadObserver, IObserver<Tuple<string, long>> errorObserver)
		{
			this.baseUrl = baseUrl;

			this.parallel = parallel;
			this.timeout = timeout;

			this.downloadObserver = downloadObserver;
			this.errorObserver = errorObserver;

			updateDisposable = Observable.EveryUpdate().Subscribe(EveryUpdate);
		}

		public void Dispose()
		{
			updateDisposable.Dispose();
			updateDisposable = null;


			waitingList.ForEach(x => x.Dispose());
			waitingList.Clear();
			downloaders.ForEach(x => x.Dispose());
			downloaders.Clear();

			ResetProgress();
		}

		public void ResetProgress()
		{
			requestCount = 0;
			downloadedCount = 0;
		}

		public void AddRequest(IDownloader<T> downloader)
		{
			if (waitingList.Any(x => x.Path == downloader.Path)) return;
			if (downloaders.Any(x => x.Path == downloader.Path)) return;

			requestCount++;
			waitingList.Add(downloader);
		}

		void EveryUpdate(long _)
		{
			for (int i = downloaders.Count - 1; i >= 0; i--)
			{
				var d = downloaders[i];
				if (d.Request.isDone || (d.Request.isHttpError || d.Request.isNetworkError))
				{
					if (d.Request.isDone)
					{
						downloadObserver.OnNext(Tuple.Create(d.Path, d.GetContent()));
					}
					else if (d.Request.isHttpError || d.Request.isNetworkError)
					{
						errorObserver.OnNext(Tuple.Create(d.Path, d.Request.responseCode));
					}
					downloadedCount++;
					downloaders.Remove(d);
				}
			}

			while (waitingList.Count > 0 && downloaders.Count < parallel)
			{
				var req = waitingList[0];
				req.Send(baseUrl, timeout);

				waitingList.Remove(req);
				downloaders.Add(req);
			}
		}
	}
}
