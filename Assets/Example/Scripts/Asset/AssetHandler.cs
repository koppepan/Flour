using UniRx.Async;
using Flour.Asset;

namespace Example
{
	public class AssetHandler
	{
		private AssetBundleHandler handler;

		public SceneWaiter SceneWaiter { get; private set; }
		public SpriteAssetWaiter SpriteWaiter { get; private set; }

		public AssetHandler(string baseUrl)
		{
			handler = new AssetBundleHandler(baseUrl);

			SceneWaiter = new SceneWaiter("scenes/");
			SpriteWaiter = new SpriteAssetWaiter("icons/", 50);
		}

		public void Dispose()
		{
			handler.Dispose();
		}

		public LoadProgress GetProgress(int downloadCount, int assetCount)
		{
			handler.ResetProgressCount();

			var progress = new LoadProgress(downloadCount, assetCount);
			progress.SetObservable(handler.DownloadProgress, handler.AssetLoadProgress);
			return progress;
		}

		public async UniTask LoadManifestAsync()
		{
			await handler.LoadManifestAsync();

			handler.AddWaiter(SceneWaiter);
			handler.AddWaiter(SpriteWaiter);
		}

		public void Compress()
		{
			SpriteWaiter.Compress();
		}
	}
}
