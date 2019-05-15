using System;
using System.Linq;
using UnityEngine;
using UniRx.Async;
using Flour;
using Flour.UI;

public class Example : MonoBehaviour
{
	[SerializeField]
	Transform canvasRoot = default;
	[SerializeField]
	Vector2 referenceResolution = new Vector2(640, 1136);

	LayerHandler layerHandler;

	// 初期化時にPrefabをLoadしておくSubLayer一覧
	SubLayerType[] FixedSubLayers = new SubLayerType[] { SubLayerType.Blackout, SubLayerType.Footer };

    async void Start()
    {
		var config = await Resources.LoadAsync<TextAsset>("Config/SubLayerType");
		var contents = ((TextAsset)config).text.Split('\n', '\r');

		var subLayers = new IniFile(contents).GetContents("SubLayerType");
		Resources.UnloadAsset(config);

		var fixedRepo = new SubLayerSourceRepository(FixedSubLayers.Length);
		var repo = new SubLayerSourceRepository(10);

		foreach (var set in subLayers)
		{
			if (FixedSubLayers.Any(x => x.ToString() == set.Key))
			{
				fixedRepo.AddSourcePath((SubLayerType)Enum.Parse(typeof(SubLayerType), set.Key), set.Value);
			}
			else
			{
				repo.AddSourcePath((SubLayerType)Enum.Parse(typeof(SubLayerType), set.Key), set.Value);
			}
		}

		layerHandler = new LayerHandler(canvasRoot, referenceResolution, fixedRepo, repo);

		for (int i = 0; i < FixedSubLayers.Length; i++)
		{
			await fixedRepo.LoadAsync<AbstractSubLayer>(FixedSubLayers[i]);
		}

		var footer = await layerHandler.AddAsync<Footer>(LayerType.Front, SubLayerType.Footer);
		footer.Setup(layerHandler);
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			layerHandler.OnBack();
		}
	}
}
