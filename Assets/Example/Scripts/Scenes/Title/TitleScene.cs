using UnityEngine;
using UniRx.Async;
using Flour.Scene;

public class TitleScene : AbstractScene
{
	Footer footer;

	public override async UniTask Load(params object[] param)
	{
		footer = new Footer(InputBinder, LayerHandler);
		await footer.Open();
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
