using UniRx.Async;

namespace Example
{
	public class TitleScene : AbstractScene
	{
		TitleLayer titleLayer;

		protected override async UniTask Load(params object[] args)
		{
			titleLayer = await LayerHandler.AddLayerAsync<TitleLayer>(LayerType.Back, SubLayerType.Title);
			titleLayer.Setup(GotoOutGame);
		}
		protected override async void Open()
		{
			var connectList = await AssetHelper.LoadServerListAsync();
			AssetHandler.ChangeBaseUrl(connectList[0].assetBundle);

			await AssetHandler.LoadManifestAsync();
		}
		protected override async UniTask Unload()
		{
			await titleLayer.CloseWait();
		}

		protected override void OnBack()
		{
			AppOperator.ApplicationQuit();
		}

		protected void GotoOutGame()
		{
			SceneHandler.LoadSceneAsync(SceneType.OutGame);
		}
	}
}
