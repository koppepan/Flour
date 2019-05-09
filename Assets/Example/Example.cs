using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flour.UI;

public class Example : MonoBehaviour
{
	[SerializeField]
	Transform canvasRoot = default;
	[SerializeField]
	Vector2 referenceResolution = new Vector2(640, 1136);
	[SerializeField]
	SubLayerSetting subLayerSetting = default;

	LayerHandler layerHandler;

    void Start()
    {
		var paths = subLayerSetting.settings.ToDictionary(k => (SubLayerType)Enum.Parse(typeof(SubLayerType), k.typeName), v => v.srcPath);
		layerHandler = new LayerHandler(canvasRoot, referenceResolution, new SubLayerSourceRepository(paths));
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			AddWindow();
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			RemoveLayer();
		}
	}

	int hoge = 0;
	async void AddWindow()
	{
		var task = layerHandler.AddAsync<AbstractSubLayer>(Layer.Back, hoge % 2 == 0 ? SubLayerType.Sample1 : SubLayerType.Sample2);
		hoge++;
		await task;
	}
	void RemoveLayer()
	{
		layerHandler.Remove(Layer.Back);
	}
}
