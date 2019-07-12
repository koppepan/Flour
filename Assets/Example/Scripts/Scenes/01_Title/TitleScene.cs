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
			var connectList = await AssetHelper.LoadServerListAsync();
			AssetHandler.ChangeBaseUrl(connectList[0].assetBundle);

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
