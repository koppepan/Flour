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
	[SerializeField]
	LayerType[] safeAreaLayers = new LayerType[] { };

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
		var repositories = await LoadLayerSourceRepositories("Config/SubLayerType", "SubLayerType");

		var sceneHandler = new SceneHandler<IOperationBundler>();
		var layerHandler = new LayerHandler(canvasRoot, referenceResolution, repositories, safeAreaLayers);
		appOperator = new ApplicationOperator(ApplicationQuit, sceneHandler, layerHandler);

		await appOperator.LoadSceneAsync("Title");

		// AndroidのBackKey対応
		Observable.EveryUpdate()
			.Where(_ => Input.GetKeyDown(KeyCode.Escape))
			.ThrottleFirst(TimeSpan.FromMilliseconds(500))
			.Subscribe(_ => appOperator.OnBack()).AddTo(this);

		// EditorをPauseしたときにOnApplicationPauseと同じ挙動にする
#if UNITY_EDITOR
		Observable.FromEvent<UnityEditor.PauseState>(
			h => UnityEditor.EditorApplication.pauseStateChanged += h,
			h => UnityEditor.EditorApplication.pauseStateChanged -= h).Subscribe(PauseStateChanged).AddTo(this);
#endif
	}

#if UNITY_EDITOR
	private void PauseStateChanged(UnityEditor.PauseState state)
	{
		var pause = state == UnityEditor.PauseState.Paused;
#else
	private void OnApplicationPause(bool pause)
	{
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
