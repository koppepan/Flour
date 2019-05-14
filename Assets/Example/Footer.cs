using System;
using UnityEngine;
using UnityEngine.UI;
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
	AbstractSubLayer currentLayer;

	public void Setup(ILayerHandler layerHandler)
	{
		this.layerHandler = layerHandler;
	}

	public override void OnOpen()
	{
		sample1Button.onClick.AddListener(() => OnOpen(SubLayerType.Sample1));
		sample2Button.onClick.AddListener(() => OnOpen(SubLayerType.Sample2));
		sample3Button.onClick.AddListener(() => OnOpen(SubLayerType.Sample3));
		sample4Button.onClick.AddListener(() => OnOpen(SubLayerType.Sample4));
	}

	async void OnOpen(SubLayerType type)
	{
		if (currentLayer?.SubLayer == type)
		{
			return;
		}
		currentLayer?.Close();
		currentLayer = await layerHandler.AddAsync<AbstractSubLayer>(Layer.Back, type);
	}

	public override void OnChangeSiblingIndex(int index)
	{
	}
}
