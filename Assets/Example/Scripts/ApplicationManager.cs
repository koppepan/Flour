using System;
using System.Linq;
using System.Security;
using UnityEngine;
using UniRx;
using UniRx.Async;
using Flour;

namespace Example
{
	using SceneHandler = Flour.Scene.SceneHandler<Tuple<IOperationBundler, AssetHandler>>;
	using LayerHandler = Flour.Layer.LayerHandler<LayerType, SubLayerType>;

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
		private AssetHandler assetHandler;

		private void Awake()
		{
			DontDestroyObjectList.Add<ApplicationManager>(gameObject);
			DontDestroyObjectList.Add<LayerHandler>(canvasRoot.gameObject);
		}

		async void Start()
		{
			Observable.FromEvent(
				_ => Application.lowMemory += OnLowMemory,
				_ => Application.lowMemory -= OnLowMemory).Subscribe().AddTo(this);

			var sceneHandler = new SceneHandler();
			var layerHandler = new LayerHandler();

			foreach (var t in EnumExtension.ToEnumerable<LayerType>(x => LayerType.Scene != x && LayerType.Debug != x))
			{
				var safeArea = safeAreaLayers.Contains(t);
				layerHandler.AddLayer(t, t.ToOrder(), canvasRoot, referenceResolution, safeArea);
			}

			InitializeDebug(sceneHandler, layerHandler);

			var fixedRepository = SubLayerSourceRepository.Create(FixedSubLayers, FixedSubLayers.Length);
			await fixedRepository.LoadAllAsync();

			//var pass = await GetPassword();
			//assetHandler = new AssetHandler("", AssetHelper.CacheAssetPath, pass);

			assetHandler = new AssetHandler("");

			appOperator = new ApplicationOperator(
				ApplicationQuit,
				assetHandler,
				sceneHandler,
				layerHandler,
				SubLayerSourceRepository.Create(EnumExtension.ToEnumerable<SubLayerType>(x => !FixedSubLayers.Contains(x)), 10),
				fixedRepository
				);
		
			await appOperator.LoadSceneAsync(SceneType.Start);

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

		private async UniTask<SecureString> GetPassword()
		{
			var param = await Resources.LoadAsync<Flour.Config.SecureParameter>("Config/SecureParameter") as Flour.Config.SecureParameter;

			SecureString pass = new SecureString();
			for (int i = 0; i < param.Password.Length; i++) pass.AppendChar(param.Password[i]);

			Resources.UnloadAsset(param);
			return pass;
		}

		private void OnLowMemory()
		{
			Debug.LogWarning("low memory");
			appOperator.ResourceCompress().Forget();
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
			assetHandler.Dispose();

			DontDestroyObjectList.Clear();
		}

		private void InitializeDebug(SceneHandler sceneHandler, LayerHandler layerHandler)
		{
#if DEBUG_BUILD
			layerHandler.AddLayer(LayerType.Debug, LayerType.Debug.ToOrder(), canvasRoot, referenceResolution, false);

			var debugHandler = new GameObject("DebugHandler", typeof(DebugHandler)).GetComponent<DebugHandler>();
			debugHandler.Initialize(sceneHandler, layerHandler, SubLayerSourceRepository.CreateDebug());

			DontDestroyObjectList.Add<DebugHandler>(debugHandler.gameObject);
#endif
		}
	}
}

