using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Async;
using Flour;

namespace Example
{
	using SceneHandler = Flour.Scene.SceneHandler<Tuple<IOperationBundler, AssetHandler>>;
	using LayerHandler = Flour.Layer.LayerHandler<LayerType, SubLayerType>;

	public class DebugHandler : MonoBehaviour
	{
		SceneHandler sceneHandler;
		LayerHandler layerHandler;

		SubLayerSourceRepository repository;

		readonly Dictionary<LogType, List<Tuple<string, string>>> logMap = new Dictionary<LogType, List<Tuple<string, string>>>();

		public void Initialize(SceneHandler sceneHandler, LayerHandler layerHandler, SubLayerSourceRepository repository)
		{
			this.sceneHandler = sceneHandler;
			this.layerHandler = layerHandler;

			this.repository = repository;

			Observable.FromEvent(
				h => Application.logMessageReceived += LogMessageReceived,
				h => Application.logMessageReceived -= LogMessageReceived).Subscribe().AddTo(this);

#if UNITY_ANDROID || UNITY_IOS
			var mouseDownStream = Observable.EveryUpdate().Select(_ => Input.touchCount).Pairwise().Where(pair => pair.Current >= 3 && pair.Previous < 3);
			var mouseUpStream = Observable.EveryUpdate().Select(_ => Input.touchCount).Pairwise().Where(pair => pair.Current < 3 && pair.Previous >= 3);
#elif UNITY_EDITOR || UNITY_STANDALONE
			var mouseDownStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(1));
			var mouseUpStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(1));
#else
			var mouseDownStream = Observable.Empty<long>();
			var mouseUpStream = Observable.Empty<long>();
#endif

			mouseDownStream
				.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(1)))
				.TakeUntil(mouseUpStream)
				.RepeatUntilDestroy(this)
#if UNITY_ANDROID || UNITY_IOS
				.Subscribe(_ => OpenDialog(Input.GetTouch(0).position))
#elif UNITY_EDITOR || UNITY_STANDALONE
				.Subscribe(_ => OpenDialog(Input.mousePosition))
#else
				.Subscribe()
#endif
				.AddTo(this);
		}

		private async UniTask<DebugDialog> Open(string title, Vector2 position)
		{
			var prefab = await repository.LoadAsync<DebugDialog>(SubLayerType.DebugDialog);

			var dialog = layerHandler.Add(LayerType.Debug, SubLayerType.DebugDialog, prefab, true);
			if (dialog != null)
			{
				dialog.Setup(title, Open);
				dialog.GetComponent<RectTransform>().SetAlignment(TextAnchor.UpperCenter);
				dialog.transform.position = position;
			}
			return dialog;
		}

		private async void OpenDialog(Vector2 position)
		{
			var dialog = await Open("debug", position);

			((AbstractScene)sceneHandler.CurrentScene).OpenDebugDialog(dialog);
			foreach (var s in sceneHandler.AdditiveScenes)
			{
				((AbstractScene)s).OpenDebugDialog(dialog);
			}
		}

		private void LogMessageReceived(string body, string stackTrace, LogType logType)
		{
			if (logType == LogType.Log)
			{
				return;
			}
			if (!logMap.ContainsKey(logType))
			{
				logMap[logType] = new List<Tuple<string, string>>();
			}
			logMap[logType].Add(Tuple.Create(body, stackTrace));

			if (logMap[logType].Count >= 50)
			{
				logMap[logType].RemoveAt(0);
			}
		}
	}
}

