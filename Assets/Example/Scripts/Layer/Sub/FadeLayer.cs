using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;

public class FadeLayer : AbstractSubLayer
{
	Image panel;

	protected override void OnOpen()
	{
		base.OnOpen();
		panel = GetComponent<Image>();
	}

	private async UniTask Fade(int milliseconds, Color befor, Color after)
	{
		panel.color = befor;
		LeanTween.color(panel.rectTransform, after, milliseconds * 0.001f);
		await UniTask.Delay(milliseconds);
	}

	public async UniTask FadeIn(int milliseconds = 200)
	{
		await Fade(milliseconds, Color.clear, Color.black);
	}
	public async UniTask FadeOut(int millisecondes = 200)
	{
		await Fade(millisecondes, Color.black, Color.clear);
		await CloseWait();
	}
}
