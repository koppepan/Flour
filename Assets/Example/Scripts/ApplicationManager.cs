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

#if DEBUG_BUILD
	private DebugHandler debugHandler;
#endif

	private void Awake()
	{
		DontDestroyObjectList.Add<ApplicationManager>(gameObject);
		DontDestroyObjectList.Add<LayerHandler<SubLayerType>>(canvasRoot.gameObject);
	}

	async void Start()
	{
		var configLoader = new ConfigLoader();
		var repositories = await configLoader.LoadLayerSourceRepositories(FixedSubLayers);

		var sceneHandler = new SceneHandler<IOperationBundler>();
		var layerHandler = new LayerHandler<SubLayerType>();

		var types = new LayerType[] { LayerType.Back, LayerType.Middle, LayerType.Front, LayerType.System };
		for (int i = 0; i < types.Length; i++)
		{
			layerHandler.AddLayer(types[i], (int)types[i], canvasRoot, referenceResolution, safeAreaLayers.Contains(types[i]));
		}

#if DEBUG_BUILD
		layerHandler.AddLayer(LayerType.Debug, (int)LayerType.Debug, canvasRoot, referenceResolution, false);
		debugHandler = new DebugHandler(this, sceneHandler, layerHandler, configLoader.CreateDebugSorceRepository());
#endif

		appOperator = new ApplicationOperator(ApplicationQuit, sceneHandler, layerHandler, repositories);

		await appOperator.LoadSceneAsync(SceneType.Title);

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
		Application.Quit(0);
#endif
	}
	private void OnApplicationQuit()
	{
		appOperator.Dispose();
		DontDestroyObjectList.Clear();
	}
}
