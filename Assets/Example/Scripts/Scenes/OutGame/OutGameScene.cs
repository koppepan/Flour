using UniRx.Async;
using UnityEngine;

public class OutGameScene : AbstractScene
{
	Footer footer;
	public override async UniTask Load(params object[] args)
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
		SceneHandler.LoadSceneAsync("Title");
	}
}
