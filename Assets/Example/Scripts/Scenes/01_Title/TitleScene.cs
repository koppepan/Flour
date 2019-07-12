using UnityEngine;
using UniRx.Async;
using Flour.Config;

namespace Example
{
	public class TitleScene : AbstractScene
	{
		TitleLayer titleLayer;

		public override async UniTask Load(params object[] args)
		{
			titleLayer = await LayerHandler.AddLayerAsync<TitleLayer>(LayerType.Back, SubLayerType.Title);
			titleLayer.Setup(GotoOutGame);
		}
		public override async void Open()
		{
			var config = (ServerList)await Resources.LoadAsync<ServerList>("Config/ServerList");
			AssetHandler.ChangeBaseUrl(config.list[0].assetBundle);
			Resources.UnloadAsset(config);

			await AssetHandler.LoadManifestAsync();
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
