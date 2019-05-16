using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;

using Flour;
using Flour.UI;

public class Footer : AbstractSubLayer
{
	public override bool IgnoreBack => true;

	[SerializeField]
	Button sample1Button = default;
	[SerializeField]
	Button sample2Button = default;
	[SerializeField]
	Button sample3Button = default;
	[SerializeField]
	Button sample4Button = default;

	ILayerHandler layerHandler;
	FooterSubLayer currentLayer;

	public void Setup(ILayerHandler layerHandler)
	{
		this.layerHandler = layerHandler;
	}

	public override void OnOpen()
	{
		sample1Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample1));
		sample2Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample2));
		sample3Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample3));
		sample4Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample4));
	}

	async void OpenSubLayer(SubLayerType type)
	{
		if (currentLayer?.SubLayer == type)
		{
			return;
		}
		await ChangeSubLayer(type);
	}

	async void CloseSubLayer()
	{
		await ChangeSubLayer(SubLayerType.None);
	}

	async UniTask ChangeSubLayer(SubLayerType subLayerType)
	{
		var fade = await layerHandler.AddLayerAsync<FadeLayer>(LayerType.Middle, SubLayerType.Blackout);
		await fade.FadeIn();

		currentLayer?.Close();
		if (subLayerType != SubLayerType.None)
		{
			currentLayer = await layerHandler.AddLayerAsync<FooterSubLayer>(LayerType.Back, subLayerType);
			currentLayer?.Setup(CloseSubLayer);
		}

		await fade.FadeOut();
		fade.Close();
	}

	public override void OnChangeSiblingIndex(int index)
	{
	}
}
