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

			await SceneHandler.LoadSceneAsync(SceneType.Title);
		}

		public override async UniTask Unload()
		{
			await splash.CloseWait();
		}
	}
}
