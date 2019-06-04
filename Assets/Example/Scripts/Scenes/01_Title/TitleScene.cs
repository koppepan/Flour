using UnityEngine;
using UniRx.Async;

public class TitleScene : AbstractScene
{
	TitleLayer titleLayer;
	public override async UniTask Load(params object[] param)
	{
		titleLayer = await LayerHandler.AddLayerAsync<TitleLayer>(LayerType.Back, SubLayerType.Title);
		titleLayer.Setup(GotoOutGame);
	}
	public override void Unload()
	{
		titleLayer.Close();
	}

	public override void OnBack()
	{
		AppOperator.ApplicationQuit();
	}

	private void GotoOutGame()
	{
		SceneHandler.LoadSceneAsync(SceneType.OutGame);
	}
}
