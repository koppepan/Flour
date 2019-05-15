using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
		sample1Button.onClick.AddListener(() => OnSubLayer(SubLayerType.Sample1));
		sample2Button.onClick.AddListener(() => OnSubLayer(SubLayerType.Sample2));
		sample3Button.onClick.AddListener(() => OnSubLayer(SubLayerType.Sample3));
		sample4Button.onClick.AddListener(() => OnSubLayer(SubLayerType.Sample4));
	}

	async void OnSubLayer(SubLayerType type)
	{
		if (currentLayer?.SubLayer == type)
		{
			return;
		}

		var eventSystem = EventSystem.current;
		eventSystem.enabled = false;

		var fade = await layerHandler.AddLayerAsync<FadeLayer>(LayerType.Middle, SubLayerType.Blackout);
		await fade.FadeIn();

		currentLayer?.Close();
		currentLayer = await layerHandler.AddLayerAsync<FooterSubLayer>(LayerType.Back, type);
		currentLayer.Setup(CloseSubLayer);

		await fade.FadeOut();
		fade.Close();

		eventSystem.enabled = true;
	}

	async void CloseSubLayer()
	{
		var eventSystem = EventSystem.current;
		eventSystem.enabled = false;

		var fade = await layerHandler.AddLayerAsync<FadeLayer>(LayerType.Middle, SubLayerType.Blackout);
		await fade.FadeIn();

		currentLayer?.Close();

		await fade.FadeOut();
		fade.Close();

		eventSystem.enabled = true;
	}

	public override void OnChangeSiblingIndex(int index)
	{
	}
}
