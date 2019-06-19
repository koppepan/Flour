using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
		readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

		readonly string baseUrl;

		readonly int parallel;
		readonly int timeout;

		readonly List<IDownloader<T>> waitingList = new List<IDownloader<T>>();
		readonly List<IDownloader<T>> downloaders = new List<IDownloader<T>>();

		readonly Subject<Tuple<string, T>> downloadedObserver = new Subject<Tuple<string, T>>();
		readonly Subject<Tuple<string, long>> erroredObserver = new Subject<Tuple<string, long>>();

		public IObservable<Tuple<string, T>> DownloadedObservable { get { return downloadedObserver; } }
		public IObservable<Tuple<string, long>> ErroredObservable { get { return erroredObserver; } }


		int requestCount = 0;
		int _downloadedCount = 0;

		int DownloadedCount
		{
			get
			{
				return _downloadedCount;
			}
			set
			{
				_downloadedCount = value;
				progress.OnNext(requestCount == 0 ? 0 : (float)_downloadedCount / (float)requestCount);
			}
		}

		IDisposable updateDisposable;

		private Subject<float> progress = new Subject<float>();
		public ISubject<float> Progress { get { return progress; } }

		BoolReactiveProperty runningProperty = new BoolReactiveProperty(false);
		public IReactiveProperty<bool> Running { get { return runningProperty; } }

		public ParallelWebRequest(string baseUrl, int parallel, int timeout)
		{
			this.baseUrl = baseUrl;

			this.parallel = parallel;
			this.timeout = timeout;
		}

		public void Dispose()
		{
			StopUpdate();

			waitingList.ForEach(x => x.Dispose());
			waitingList.Clear();
			downloaders.ForEach(x => x.Dispose());
			downloaders.Clear();

			downloadedObserver.OnCompleted();
			downloadedObserver.Dispose();

			erroredObserver.OnCompleted();
			erroredObserver.Dispose();

			ResetProgress();
		}

		void StartUpdate()
		{
			if (updateDisposable != null)
			{
				return;
			}
			runningProperty.Value = true;
			updateDisposable = Observable.FromCoroutine(EveryUpdate).Subscribe();
		}
		void StopUpdate()
		{
			if (updateDisposable != null)
			{
				updateDisposable.Dispose();
				updateDisposable = null;
			}
			runningProperty.Value = false;
		}

		public void ResetProgress()
		{
			requestCount = 0;
			DownloadedCount = 0;
		}

		public void AddRequest(IDownloader<T> downloader)
		{
			if (waitingList.Any(x => x.Path == downloader.Path)) return;
			if (downloaders.Any(x => x.Path == downloader.Path)) return;

			requestCount++;
			waitingList.Add(downloader);

			StartUpdate();
		}

		IEnumerator EveryUpdate()
		{
			while (true)
			{
				for (int i = downloaders.Count - 1; i >= 0; i--)
				{
					var d = downloaders[i];
					if (d.Request.isDone || (d.Request.isHttpError || d.Request.isNetworkError))
					{
						if (d.Request.isHttpError || d.Request.isNetworkError)
						{
							erroredObserver.OnNext(Tuple.Create(d.Path, d.Request.responseCode));
						}
						else
						{
							DownloadedCount++;
							downloadedObserver.OnNext(Tuple.Create(d.Path, d.GetContent()));
						}
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

				if (downloaders.Count == 0 && waitingList.Count == 0)
				{
					StopUpdate();
				}

				yield return waitForSeconds;
			}
		}
	}
}
