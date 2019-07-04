using UnityEngine;
using UniRx;
using UniRx.Async;
using Flour.Config;

namespace Example
{
	public class StartScene : AbstractScene
	{
		SplashLayer splash;

		public override async UniTask Load(params object[] args)
		{
			var config = (ServerList)await Resources.LoadAsync<ServerList>("Config/ServerList");
			AssetHandler.ChangeBaseUrl(config.list[0].assetBundle);

			await AssetHandler.LoadManifestAsync();

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
