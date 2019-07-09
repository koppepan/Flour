using System;
using UnityEngine;
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

		public LoadProgress(int downloadCount, int assetCount)
		{
			this.downloadCount = downloadCount;
			this.assetCount = assetCount;
		}

		public void Dispose()
		{
			disposables.Dispose();
		}

		public void SetObservable(IObservable<float> download, IObservable<float> assetLoad)
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
			var d = downloadCount == 0 ? 1 : downloadProgress / downloadCount;
			var a = assetCount == 0 ? 1 : assetLoadProgress / assetCount;
			var value = Mathf.Clamp01((d + a) * 0.5f);

			ProgressValue = value;
			Progress.Invoke(value);

			if (value == 1)
			{
				Running.Invoke(false);
			}
		}
	}
}
