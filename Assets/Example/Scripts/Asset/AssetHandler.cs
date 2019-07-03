using System;
using UniRx.Async;
using Flour.Asset;

namespace Example
{
	public class AssetHandler
	{
		private AssetBundleHandler handler;

		public SceneWaiter SceneWaiter { get; private set; }
		public SpriteAssetWaiter SpriteWaiter { get; private set; }

		public IObservable<LoadError> ErrorObservable { get { return handler.ErrorObservable; } }

		public AssetHandler(string baseUrl)
		{
			handler = new AssetBundleHandler(baseUrl);

			SceneWaiter = new SceneWaiter("scenes/");
			SpriteWaiter = new SpriteAssetWaiter("icons/", 50);
		}

		public void ChangeBaseUrl(string baseUrl) => handler.ChangeBaseUrl(baseUrl);

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

		public async UniTask LoadManifestAsync(string manifestName, string sizeManifestName, string crcManifestName = "")
		{
			await handler.LoadManifestAsync(manifestName, sizeManifestName, crcManifestName);

			handler.AddWaiter(SceneWaiter);
			handler.AddWaiter(SpriteWaiter);
		}

		public void Compress()
		{
			SpriteWaiter.Compress();
		}
	}
}
