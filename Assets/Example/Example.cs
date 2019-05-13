using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Flour;
using Flour.UI;

public class Example : MonoBehaviour
{
	[SerializeField]
	Transform canvasRoot = default;
	[SerializeField]
	Vector2 referenceResolution = new Vector2(640, 1136);

	LayerHandler layerHandler;

    IEnumerator Start()
    {
		var req = Resources.LoadAsync<TextAsset>("Config/SubLayerType");
		yield return req;
		var contents = ((TextAsset)req.asset).text.Split('\n', '\r');

		var ini = new IniFile(contents);
		var paths = ini.GetContents("SubLayerType").ToDictionary(k => (SubLayerType)Enum.Parse(typeof(SubLayerType), k.Key), v => v.Value);
		layerHandler = new LayerHandler(canvasRoot, referenceResolution, new SubLayerSourceRepository(paths, 10));

		AddFooter();
    }

	async void AddFooter()
	{
		var task = layerHandler.AddAsync<Footer>(Layer.Front, SubLayerType.Footer);
		await task;
		task.Result.Setup(layerHandler);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			layerHandler.OnBack();
		}
	}
}
