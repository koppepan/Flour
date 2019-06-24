using System;
using UniRx;

namespace Example
{
	public class LoadProgress
	{
		int downloadCount = 0;
		int assetCount = 0;

		float downloadProgress = 0;
		float assetLoadProgress = 0;

		public Action<float> Progress = delegate { };
		public Action<bool> Running = delegate { };

		public float ProgressValue { get; private set; } = 0;

		private CompositeDisposable disposables = new CompositeDisposable();

		public LoadProgress(int downloadCount)
		{
			this.downloadCount = this.assetCount = downloadCount;
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
			download.Subscribe(UpdateDownloadProgress).AddTo(disposables);
			assetLoad.Subscribe(UpdateAssetLoadProgress).AddTo(disposables);
		}

		private void UpdateDownloadProgress(float val)
		{
			downloadProgress = val;
			UpdateProgress();
		}

		private void UpdateAssetLoadProgress(float val)
		{
			assetLoadProgress = val;
			UpdateProgress();
		}

		private void UpdateProgress()
		{
			var value = (downloadProgress + assetLoadProgress) * 0.5f;
			Progress.Invoke(value);

			if (value == 1)
			{
				Running.Invoke(false);
			}
		}
	}
}
