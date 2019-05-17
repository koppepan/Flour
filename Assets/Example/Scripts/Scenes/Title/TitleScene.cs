using UnityEngine;
using UniRx.Async;

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
		AppOparator.ApplicationQuit();
	}
}
