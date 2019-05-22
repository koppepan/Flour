using System;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

using Flour;
using Flour.Scene;
using Flour.Layer;

public sealed class ApplicationManager : MonoBehaviour
{
	[Header("UI")]
	[SerializeField]
	Transform canvasRoot = default;
	[SerializeField]
	Vector2 referenceResolution = new Vector2(750, 1334);

	// 初期化時にPrefabをLoadしておくSubLayer一覧
	readonly SubLayerType[] FixedSubLayers = new SubLayerType[] { SubLayerType.Blackout, SubLayerType.Footer };

	private ApplicationOperator appOperator;

	private void Awake()
	{
		DontDestroyObjectList.Add<ApplicationManager>(gameObject);
		DontDestroyObjectList.Add<LayerHandler>(canvasRoot.gameObject);
	}

	async void Start()
	{
		var r = await LoadLayerSourceRepositories("Config/SubLayerType", "SubLayerType");
		appOperator = new ApplicationOperator(ApplicationQuit, new SceneHandler<IOperationBundler>(), new LayerHandler(canvasRoot, referenceResolution, r));
		await appOperator.LoadSceneAsync("Title");
	}

#if UNITY_EDITOR
	private void OnEnable()
	{
		UnityEditor.EditorApplication.pauseStateChanged += PauseStateChanged;
	}
	private void OnDisable()
	{
		UnityEditor.EditorApplication.pauseStateChanged -= PauseStateChanged;
	}
#endif

#if !UNITY_EDITOR
	private void OnApplicationPause(bool pause)
	{
#else
	private void PauseStateChanged(UnityEditor.PauseState state)
	{
		var pause = state == UnityEditor.PauseState.Paused;
#endif
		appOperator?.ApplicationPause(pause);
	}

	private void ApplicationQuit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode();
#else
		Application.Quit(1);
#endif
	}
	private void OnApplicationQuit()
	{
		DontDestroyObjectList.Clear();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			appOperator.OnBack();
		}
	}

	async UniTask<SubLayerSourceRepository[]> LoadLayerSourceRepositories(string path, string typeName)
	{
		var config = await Resources.LoadAsync<TextAsset>(path);
		var contents = new IniFile(((TextAsset)config).text.Split('\n', '\r')).GetContents(typeName);
		var subLayers = contents.ToDictionary(k => (SubLayerType)Enum.Parse(typeof(SubLayerType), k.Key), v => v.Value);

		Resources.UnloadAsset(config);

		var repositories = new SubLayerSourceRepository[2] { new SubLayerSourceRepository(FixedSubLayers.Length), new SubLayerSourceRepository(10) };

		foreach (var set in subLayers)
		{
			var index = FixedSubLayers.Contains(set.Key) ? 0 : 1;
			repositories[index].AddSourcePath(set.Key, set.Value);
		}
		for (int i = 0; i < FixedSubLayers.Length; i++)
		{
			await repositories[0].LoadAsync<AbstractSubLayer>(FixedSubLayers[i]);
		}

		return repositories;
	}
}
