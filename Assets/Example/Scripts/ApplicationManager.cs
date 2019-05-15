using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Async;

using Flour;
using Flour.UI;
using Flour.Scene;

public class ApplicationManager : MonoBehaviour, IOperationBundler, ISceneHandler, ILayerHandler
{
	[Header("UI")]
	[SerializeField]
	Transform canvasRoot = default;
	[SerializeField]
	Vector2 referenceResolution = new Vector2(750, 1334);

	// 初期化時にPrefabをLoadしておくSubLayer一覧
	readonly SubLayerType[] FixedSubLayers = new SubLayerType[] { SubLayerType.Blackout, SubLayerType.Footer };

	SceneHandler sceneHandler;
	LayerHandler layerHandler;

	public ISceneHandler SceneHandler { get { return this; } }
	public ILayerHandler LayerHandler { get { return this; } }

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
		await LoadSceneAsync("Title");
	}

	private void OnApplicationQuit()
	{
		DontDestroyObjectList.Clear();
	}


	public async UniTask LoadSceneAsync(string sceneName, params object[] param)
	{
		var eventSystem = EventSystem.current;
		eventSystem.enabled = false;

		var fade = await layerHandler.AddAsync<FadeLayer>(LayerType.System, SubLayerType.Blackout);
		await fade.FadeIn();
		await sceneHandler.LoadSceneAsync(sceneName, this, param);
		await fade.FadeOut();
		fade.Close();

		eventSystem.enabled = true;
	}
	public async UniTask AddSceneAsync(string sceneName, params object[] param)
	{
		await sceneHandler.AddSceneAsync(sceneName, this, param);
	}
	public async UniTask UnloadSceneAsync(string sceneName)
	{
		await sceneHandler.UnloadSceneAsync(sceneName);
	}

	public async UniTask<AbstractSubLayer> AddLayerAsync(LayerType layer, SubLayerType subLayer)
	{
		return await layerHandler.AddAsync(layer, subLayer);
	}
	public async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer
	{
		return await layerHandler.AddAsync<T>(layer, subLayer);
	}
}
