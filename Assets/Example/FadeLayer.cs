using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;
using Flour.UI;

public class FadeLayer : AbstractSubLayer
{
	Image panel;

	public override void OnOpen()
	{
		base.OnOpen();
		panel = GetComponent<Image>();
		panel.color = Color.black;
	}

	public async UniTask FadeIn()
	{
		await UniTask.Delay(200);
	}
	public async UniTask FadeOut()
	{
		await UniTask.Delay(200);
	}
}
