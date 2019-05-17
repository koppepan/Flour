using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;
using Flour.Layer;

public class FadeLayer : AbstractSubLayer
{
	Image panel;

	public override void OnOpen()
	{
		base.OnOpen();
		panel = GetComponent<Image>();
	}

	public async UniTask FadeIn()
	{
		panel.color = Color.clear;
		LeanTween.color(panel.rectTransform, Color.black, 0.2f);
		await UniTask.Delay(200);
	}
	public async UniTask FadeOut()
	{
		panel.color = Color.clear;
		LeanTween.color(panel.rectTransform, Color.clear, 0.2f);
		await UniTask.Delay(200);
	}
}
