﻿using System;
using System.Linq;
using UnityEngine;
using UniRx;

using Flour;

namespace Example
{
	using SceneHandler = Flour.Scene.SceneHandler<IOperationBundler>;
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

#if DEBUG_BUILD
		private DebugHandler debugHandler;
#endif

		private void Awake()
		{
			DontDestroyObjectList.Add<ApplicationManager>(gameObject);
			DontDestroyObjectList.Add<LayerHandler>(canvasRoot.gameObject);
		}

		async void Start()
		{
			var sceneHandler = new SceneHandler();
			var layerHandler = new LayerHandler();

			InitializeDebug(sceneHandler, layerHandler);

			foreach (var t in EnumExtension.ToEnumerable<LayerType>(x => LayerType.Debug != x))
			{
				var safeArea = safeAreaLayers.Contains(t);
				layerHandler.AddLayer(t, t.ToOrder(), canvasRoot, referenceResolution, safeArea);
			}

			var fixedRepository = SubLayerSourceRepository.Create(FixedSubLayers, FixedSubLayers.Length);
			await fixedRepository.LoadAllAsync();

			appOperator = new ApplicationOperator(
				ApplicationQuit,
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

		private void InitializeDebug(SceneHandler sceneHandler, LayerHandler layerHandler)
		{
#if DEBUG_BUILD
			layerHandler.AddLayer(LayerType.Debug, LayerType.Debug.ToOrder(), canvasRoot, referenceResolution, false);

			var debugRepository = SubLayerSourceRepository.CreateDebug();
			debugHandler = new DebugHandler(this, sceneHandler, layerHandler, debugRepository);

			Observable.FromEvent(
				h => Application.logMessageReceived += debugHandler.LogMessageReceived,
				h => Application.logMessageReceived -= debugHandler.LogMessageReceived).Subscribe().AddTo(this);
#endif
		}
	}
}

