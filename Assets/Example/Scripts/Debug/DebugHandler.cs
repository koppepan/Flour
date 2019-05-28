using System;
using UnityEngine;
using UniRx;
using UniRx.Async;
using Flour.Scene;
using Flour.Layer;

public class DebugHandler
{
	SceneHandler<IOperationBundler> sceneHandler;
	LayerHandler layerHandler;

	public DebugHandler(MonoBehaviour root, SceneHandler<IOperationBundler> sceneHandler, LayerHandler layerHandler)
	{
		this.sceneHandler = sceneHandler;
		this.layerHandler = layerHandler;

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
			.Subscribe(_ => debugHandler.OpenDialog(Input.GetTouch(0).position))
#endif
			.AddTo(root);
	}

	private async UniTask<DebugDialog> Open(string title)
	{
		var dialog = await layerHandler.AddAsync<DebugDialog>(LayerType.Debug, SubLayerType.DebugDialog, true);
		dialog.Setup(title, Open);
		return dialog;
	}

	private async void OpenDialog(Vector2 position)
	{
		var dialog = await Open("debug");
		dialog.transform.position = position;

		((AbstractScene)sceneHandler.CurrentScene).OpenDebugDialog(dialog);
		foreach (var s in sceneHandler.AdditiveScenes)
		{
			((AbstractScene)s).OpenDebugDialog(dialog);
		}
	}
}
