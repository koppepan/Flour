using UniRx.Async;

namespace Example
{
	public class TitleScene : AbstractScene
	{
		TitleLayer titleLayer;
		public override async UniTask Load(params object[] param)
		{
			titleLayer = await LayerHandler.AddLayerAsync<TitleLayer>(LayerType.Back, SubLayerType.Title);
			titleLayer.Setup(GotoOutGame);
		}
		public override async UniTask Unload()
		{
			titleLayer.Close();
			await base.Unload();
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
}
