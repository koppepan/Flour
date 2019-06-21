using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	public class ProgressLayer : AbstractSubLayer
	{
		[SerializeField] Image progressImage = default;

		LoadProgress progress;

		public void Setup(LoadProgress progress)
		{
			this.progress = progress;
			progress.DownloadProgress += UpdateProgress;
			progress.DownloadRunning += Downloaded;
		}

		void UpdateProgress(float value)
		{
			if (LeanTween.isTweening(progressImage.gameObject))
			{
				LeanTween.cancel(progressImage.gameObject);
			}
			LeanTween.value(progressImage.gameObject, val => progressImage.fillAmount = val, progressImage.fillAmount, value, 0.1f);
		}

		void Downloaded(bool complete)
		{
			if (!complete) return;
			progress.DownloadProgress -= UpdateProgress;
			progress.DownloadRunning -= Downloaded;

			progress.AssetLoadProgress += UpdateProgress;
		}

		protected override async UniTask OnClose(bool force)
		{
			progress.DownloadProgress -= UpdateProgress;
			progress.DownloadRunning -= Downloaded;

			progress.AssetLoadProgress -= UpdateProgress;

			progress.Dispose();

			UpdateProgress(1);
			await UniTask.Delay(100);
			await base.OnClose(force);
		}
	}
}
