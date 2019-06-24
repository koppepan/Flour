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
			progress.Progress += UpdateProgress;
		}

		void UpdateProgress(float value)
		{
			if (LeanTween.isTweening(progressImage.gameObject)) LeanTween.cancel(progressImage.gameObject);
			LeanTween.value(progressImage.gameObject, val => progressImage.fillAmount = val, progressImage.fillAmount, value, 0.1f);
		}

		protected override async UniTask OnClose(bool force)
		{
			progress.Progress -= UpdateProgress;
			progress.Dispose();

			UpdateProgress(1);
			await UniTask.Delay(100);
			await base.OnClose(force);
		}
	}
}
