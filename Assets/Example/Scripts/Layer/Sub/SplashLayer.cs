using UniRx.Async;

namespace Example
{
	public class SplashLayer : AbstractSubLayer
	{
		public async UniTask Run()
		{
			CanvasGroup.alpha = 0;
			LeanTween.alphaCanvas(CanvasGroup, 1, 0.5f);
			await UniTask.Delay(2000);
			LeanTween.alphaCanvas(CanvasGroup, 0, 0.5f);
			await UniTask.Delay(500);
		}
	}
}
