using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flour.UI;

public class Example : MonoBehaviour
{
	[SerializeField]
	Transform canvasRoot = default;
	[SerializeField]
	Vector2 referenceResolution = new Vector2(640, 1136);

	LayerHandler layerHandler;

	Dictionary<SubLayerType, string> subLayerSourcePaths = new Dictionary<SubLayerType, string>
	{
		{ SubLayerType.Sample1, "Sample1" },
		{ SubLayerType.Sample2, "Sample2" },
	};

    void Start()
    {
		layerHandler = new LayerHandler(canvasRoot, referenceResolution, new SubLayerSourceRepository(subLayerSourcePaths));
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
