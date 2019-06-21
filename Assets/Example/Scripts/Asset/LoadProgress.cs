using System;
using UniRx;

namespace Example
{
	public class LoadProgress
	{
		int downloadCount;
		int assetCount;

		public Action<float> DownloadProgress = delegate { };
		public Action<bool> DownloadRunning = delegate { };
		public Action<float> AssetLoadProgress = delegate { };
		public Action<bool> AssetLoadRunning = delegate { };

		private CompositeDisposable disposables = new CompositeDisposable();

		public LoadProgress(int downloadCount)
		{
			this.downloadCount = assetCount = downloadCount;
		}
		public LoadProgress(int downloadCount, int assetCount)
		{
			this.downloadCount = downloadCount;
			this.assetCount = assetCount;
		}
		public void Dispose()
		{
			disposables.Dispose();
		}

		public void SetObservable(IReactiveProperty<float> download, IReactiveProperty<float> assetLoad)
		{
			download.Subscribe(p =>
			{
				DownloadProgress.Invoke(downloadCount == 0 ? 0 : p / downloadCount);
				if (downloadCount == p)
				{
					DownloadRunning.Invoke(true);
				}
			}).AddTo(disposables);

			assetLoad.Subscribe(p =>
			{
				AssetLoadProgress.Invoke(assetCount == 0 ? 0 : p / assetCount);
				if (assetCount == p)
				{
					AssetLoadRunning.Invoke(true);
				}
			}).AddTo(disposables);
		}
	}
}
