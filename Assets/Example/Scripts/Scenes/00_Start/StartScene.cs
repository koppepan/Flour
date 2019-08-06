using UniRx.Async;

namespace Example
{
	public sealed class StartScene : AbstractScene
	{
		SplashLayer splash;

		protected override async UniTask Load(params object[] args)
		{
			splash = await LayerHandler.AddLayerAsync<SplashLayer>(LayerType.System, SubLayerType.Splash);
			await splash.Run();

			await SceneHandler.LoadSceneAsync(SceneType.Title);
		}

		protected override async UniTask Unload()
		{
			await splash.CloseWait();
		}
	}
}
