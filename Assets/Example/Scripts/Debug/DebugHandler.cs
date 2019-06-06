using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Example
{
	using SceneHandler = Flour.Scene.SceneHandler<IOperationBundler>;
	using LayerHandler = Flour.Layer.LayerHandler<LayerType, SubLayerType>;

	public class DebugHandler
	{
		readonly SceneHandler sceneHandler;
		readonly LayerHandler layerHandler;

		readonly SubLayerSourceRepository repository;

		readonly Dictionary<LogType, List<Tuple<string, string>>> logMap = new Dictionary<LogType, List<Tuple<string, string>>>();

		public DebugHandler(MonoBehaviour root, SceneHandler sceneHandler, LayerHandler layerHandler, SubLayerSourceRepository repository)
		{
			this.sceneHandler = sceneHandler;
			this.layerHandler = layerHandler;

			this.repository = repository;

#if UNITY_EDITOR
			var mouseDownStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(1));
			var mouseUpStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(1));
#else
			var mouseDownStream = Observable.EveryUpdate().Select(_ => Input.touchCount).Pairwise().Where(pair => pair.Current >= 3 && pair.Previous < 3);
			var mouseUpStream = Observable.EveryUpdate().Select(_ => Input.touchCount).Pairwise().Where(pair => pair.Current < 3 && pair.Previous >= 3);
#endif

			mouseDownStream
				.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(1)))
				.TakeUntil(mouseUpStream)
				.RepeatUntilDestroy(root)
#if UNITY_EDITOR
				.Subscribe(_ => OpenDialog(Input.mousePosition))
#else
				.Subscribe(_ => OpenDialog(Input.GetTouch(0).position))
#endif
				.AddTo(root);
		}

		private async UniTask<DebugDialog> Open(string title, Vector2 position)
		{
			var prefab = await repository.LoadAsync<DebugDialog>(SubLayerType.DebugDialog);

			var dialog = layerHandler.Add(LayerType.Debug, SubLayerType.DebugDialog, prefab, true);
			if (dialog != null)
			{
				dialog.Setup(title, Open);
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

		public void LogMessageReceived(string body, string stackTrace, LogType logType)
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

