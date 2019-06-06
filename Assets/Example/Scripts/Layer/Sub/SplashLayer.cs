using System.Collections;
using UniRx.Async;

namespace Example
{
	public class SplashLayer : AbstractSubLayer
	{
		public async UniTask Run()
		{
			await Fade(0, 1, 0.5f);
			await UniTask.Delay(1500);
			await Fade(1, 0, 0.5f);
		}

		IEnumerator Fade(float from, float to, float time)
		{
			bool complete = false;
			LeanTween.value(gameObject, val => Alpha = val, from, to, time).setOnComplete(() => complete = true);

			while (!complete) yield return null;
		}
	}
}
