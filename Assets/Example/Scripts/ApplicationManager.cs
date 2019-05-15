using System;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

using Flour;
using Flour.UI;
using Flour.Scene;

public class ApplicationManager : MonoBehaviour
{
	[Header("UI")]
	[SerializeField]
	Transform canvasRoot = default;
	[SerializeField]
	Vector2 referenceResolution = new Vector2(750, 1334);

	SceneHandler sceneHandler;
	LayerHandler layerHandler;

	// 初期化時にPrefabをLoadしておくSubLayer一覧
	SubLayerType[] FixedSubLayers = new SubLayerType[] { SubLayerType.Blackout, SubLayerType.Footer };

	private void Awake()
	{
		DontDestroyObjectList.Add<ApplicationManager>(gameObject);
		DontDestroyObjectList.Add<LayerHandler>(canvasRoot.gameObject);
	}

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

		sceneHandler = new SceneHandler();
		await sceneHandler.LoadScene("Title");
	}

	private void OnApplicationQuit()
	{
		DontDestroyObjectList.Clear();
	}
}
