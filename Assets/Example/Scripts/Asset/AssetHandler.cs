using UniRx.Async;
using Flour.Asset;

namespace Example
{
	class AssetHandler : IAssetHandler
	{
		private AssetBundleHandler handler;

		public SpriteAssetWaiter SpriteWaiter { get; private set; }

		public AssetHandler(string baseUrl)
		{
			handler = new AssetBundleHandler(baseUrl);
			SpriteWaiter = new SpriteAssetWaiter("icons/");
		}

		public void Dispose()
		{
			handler.Dispose();
		}

		public async UniTask LoadManifestAsync()
		{
			await handler.LoadManifestAsync();
			handler.AddWaiter(SpriteWaiter);
		}
	}
}
