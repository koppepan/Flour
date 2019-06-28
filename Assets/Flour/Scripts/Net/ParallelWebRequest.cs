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
		readonly Subject<Tuple<string, long, string>> erroredObserver = new Subject<Tuple<string, long, string>>();

		public IObservable<Tuple<string, T>> DownloadedObservable { get { return downloadedObserver; } }
		public IObservable<Tuple<string, long, string>> ErroredObservable { get { return erroredObserver; } }

		int downloadedCount = 0;

		CompositeDisposable updateDisposable;

		private FloatReactiveProperty progress = new FloatReactiveProperty(0);
		public IReactiveProperty<float> Progress { get { return progress; } }

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
		}

		internal void ResetProgressCount()
		{
			downloadedCount = 0;
		}
		void StartUpdate()
		{
			if (updateDisposable != null)
			{
				return;
			}

			updateDisposable = new CompositeDisposable();
			Observable.FromCoroutine(EveryUpdate).Subscribe().AddTo(updateDisposable);
			Observable.EveryLateUpdate().Subscribe(UpdateProgress).AddTo(updateDisposable);
		}
		void StopUpdate()
		{
			if (updateDisposable != null)
			{
				updateDisposable.Dispose();
				updateDisposable = null;
			}
		}

		public void AddRequest(IDownloader<T> downloader)
		{
			if (waitingList.Any(x => x.Path.Equals(downloader.Path, StringComparison.Ordinal))) return;
			if (downloaders.Any(x => x.Path.Equals(downloader.Path, StringComparison.Ordinal))) return;

			waitingList.Add(downloader);

			StartUpdate();
		}

		IEnumerator EveryUpdate()
		{
			while (true)
			{
				if (downloaders.Count == 0 && waitingList.Count == 0)
				{
					StopUpdate();
				}

				for (int i = downloaders.Count - 1; i >= 0; i--)
				{
					var d = downloaders[i];
					if (d.Request.isDone || (d.Request.isHttpError || d.Request.isNetworkError))
					{
						downloaders.Remove(d);

						if (d.Request.isHttpError || d.Request.isNetworkError)
						{
							erroredObserver.OnNext(Tuple.Create(d.Path, d.Request.responseCode, d.Request.error));
						}
						else
						{
							downloadedCount++;
							UpdateProgress(0);

							downloadedObserver.OnNext(Tuple.Create(d.Path, d.GetContent()));
						}
					}
				}

				while (waitingList.Count > 0 && downloaders.Count < parallel)
				{
					var req = waitingList[0];
					req.Send(baseUrl, timeout);

					waitingList.Remove(req);
					downloaders.Add(req);
				}

				yield return waitForSeconds;
			}
		}

		void UpdateProgress(long _)
		{
			float currentProgress = 0;
			for (int i = 0; i < downloaders.Count; i++)
			{
				currentProgress += downloaders[i].Request.downloadProgress;
			}

			progress.Value = downloadedCount + currentProgress;
		}
	}
}
