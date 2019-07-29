using UniRx.Async;

namespace Example
{
	public class OutGameScene : AbstractScene
	{
		Footer footer;
		protected override async UniTask Load(params object[] args)
		{
			footer = new Footer(InputBinder, LayerHandler);
			await footer.Open();
		}
		protected override async UniTask Unload()
		{
			await footer.CloseWait();
		}

		protected override void OnBack()
		{
			SceneHandler.LoadSceneAsync(SceneType.Title);
		}
	}
}
