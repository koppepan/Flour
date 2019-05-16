using UnityEngine;
using UniRx.Async;
using Flour.Scene;
using Flour.UI;

public class TitleScene : AbstractScene
{
	Footer footer;

	public override async UniTask Load(params object[] param)
	{
		footer = await LayerHandler.AddLayerAsync<Footer>(LayerType.Front, SubLayerType.Footer);
		footer.Setup(LayerHandler);
	}
	public override void Unload()
	{
		footer.Close();
	}

	public override void OnBack()
	{
		operationBundler.ApplicationQuit();
	}
}
