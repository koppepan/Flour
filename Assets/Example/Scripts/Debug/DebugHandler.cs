using UniRx.Async;
using Flour.Scene;
using Flour.Layer;

public class DebugHandler
{
	SceneHandler<IOperationBundler> sceneHandler;
	LayerHandler layerHandler;

	public DebugHandler(SceneHandler<IOperationBundler> sceneHandler, LayerHandler layerHandler)
	{
		this.sceneHandler = sceneHandler;
		this.layerHandler = layerHandler;
	}

	private async UniTask<DebugDialog> Open()
	{
		return await layerHandler.AddAsync<DebugDialog>(LayerType.Debug, SubLayerType.DebugDialog, true);
	}

	public async void OpenDialog()
	{
		var dialog = await Open();
		dialog.Setup("debug", Open);

		((AbstractScene)sceneHandler.CurrentScene).OpenDebugDialog(dialog);
		foreach (var s in sceneHandler.AdditiveScenes)
		{
			((AbstractScene)s).OpenDebugDialog(dialog);
		}
	}
}
