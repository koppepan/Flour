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
		var config = await Resources.LoadAsync<TextAsset>("Config/SubLayerType");
		var contents = new IniFile(((TextAsset)config).text.Split('\n', '\r')).GetContents("SubLayerType");
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

		appOperator = new ApplicationOperator(ApplicationQuit, new SceneHandler<IOperationBundler>(), new LayerHandler(canvasRoot, referenceResolution, repositories));
		await appOperator.LoadSceneAsync("Title");
	}

	private void OnApplicationPause(bool pause)
	{
		appOperator.ApplicationPause(pause);
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
}
