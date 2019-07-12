using System;
using UnityEngine;
using UniRx.Async;
using Flour;

namespace Example
{
	using SceneHandler = Flour.Scene.SceneHandler<Tuple<IOperationBundler, AssetHandler>>;
	using LayerHandler = Flour.Layer.LayerHandler<LayerType, SubLayerType>;

	public class DebugDialogCreator
	{
		readonly SceneHandler sceneHandler;
		readonly LayerHandler layerHandler;
		readonly SubLayerSourceRepository repository;

		public DebugDialogCreator(SceneHandler sceneHandler, LayerHandler layerHandler, SubLayerSourceRepository repository)
		{
			this.sceneHandler = sceneHandler;
			this.layerHandler = layerHandler;
			this.repository = repository;
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

		public async void OpenDebugDialog(Vector2 position)
		{
			var dialog = await Open("debug", position);

			((AbstractScene)sceneHandler.CurrentScene)?.OpenDebugDialog(dialog);
			foreach (var s in sceneHandler.AdditiveScenes)
			{
				((AbstractScene)s).OpenDebugDialog(dialog);
			}
		}
	}
}
