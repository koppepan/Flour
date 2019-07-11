using UniRx.Async;

namespace Example
{
	public class StartScene : AbstractScene
	{
		SplashLayer splash;

		public override async UniTask Load(params object[] args)
		{
			splash = await LayerHandler.AddLayerAsync<SplashLayer>(LayerType.System, SubLayerType.Splash);
			await splash.Run();
		}

		public override async void Open()
		{
			await SceneHandler.LoadSceneAsync(SceneType.Title);
		}

		public override async UniTask Unload()
		{
			splash.Close();
			await base.Unload();
		}
	}
}
