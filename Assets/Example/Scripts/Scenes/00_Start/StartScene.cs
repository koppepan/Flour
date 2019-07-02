using System.IO;
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

			var folder = AssetHelper.GetAssetBundleFolderName(Application.platform);
			var assetBundlePath = Path.Combine(config.list[0].assetBundle, folder);

			AssetHandler.ChangeBaseUrl(assetBundlePath);
			await AssetHandler.LoadManifestAsync(folder, AssetHelper.AssetBundleSizeManifestName);

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
