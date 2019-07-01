using UniRx.Async;

namespace Example
{
	public class StartScene : AbstractScene
	{
		SplashLayer splash;

		public override async UniTask Load(params object[] args)
		{
			if (args.Length == 1 && !string.IsNullOrEmpty((string)args[0]))
			{
				AssetHandler.ChangeBaseUrl((string)args[1]);
				await AssetHandler.LoadManifestAsync();
			}

			splash = await LayerHandler.AddLayerAsync<SplashLayer>(LayerType.System, SubLayerType.Splash);
			await splash.Run();
		}

		public override void Open()
		{
			SceneHandler.LoadSceneAsync(SceneType.Title);
		}

		public override async UniTask Unload()
		{
			splash.Close();
			await base.Unload();
		}
	}
}
