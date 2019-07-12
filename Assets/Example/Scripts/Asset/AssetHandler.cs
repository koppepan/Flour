using System;
using System.Security;
using UnityEngine;
using UniRx.Async;
using Flour.Asset;

namespace Example
{
	public class AssetHandler
	{
		readonly bool encrypt;
		private AssetBundleHandler handler;

		public AssetWaiter<UnityEngine.Object> SceneWaiter { get; private set; } = new AssetWaiter<UnityEngine.Object>("scenes/");
		public AssetCacheWaiter<GameObject> PrefabWaiter { get; private set; } = new AssetCacheWaiter<GameObject>("prefabs/", 50);
		public AssetCacheWaiter<Sprite> SpriteWaiter { get; private set; } = new AssetCacheWaiter<Sprite>("icons/", 50);

		public IObservable<LoadError> ErrorObservable { get { return handler.ErrorObservable; } }

		public AssetHandler(string baseUrl, string cachePath = "", SecureString password = null)
		{
			encrypt = !string.IsNullOrEmpty(cachePath) && password != null;
			handler = !encrypt ? new AssetBundleHandler(baseUrl) : new SecureAssetBundleHandler(baseUrl, cachePath, password);
		}

		public void ChangeBaseUrl(string baseUrl)
		{
			var platform = Application.platform;
			var folder = encrypt ? AssetHelper.GetEncryptAssetBundleFolderName(platform) : AssetHelper.GetAssetBundleFolderName(platform);
			handler.ChangeBaseUrl(System.IO.Path.Combine(baseUrl, folder));
		}

		public void Dispose()
		{
			handler.Dispose();
		}

		public LoadProgress GetProgress()
		{
			handler.ResetProgressCount();
			var count = handler.GetRequestCount();

			var progress = new LoadProgress(count.Item1, count.Item2);
			progress.SetObservable(handler.DownloadedCount, handler.AssetLoadedCount);
			return progress;
		}

		public async UniTask LoadManifestAsync()
		{
			var manifest = AssetHelper.GetAssetBundleFolderName(Application.platform);
			var sizeManifest = AssetHelper.AssetBundleSizeManifestName;

			await handler.LoadManifestAsync(manifest, sizeManifest, "");

			handler.AddWaiter(SceneWaiter);
			handler.AddWaiter(PrefabWaiter);
			handler.AddWaiter(SpriteWaiter);
		}

		public void Compress()
		{
			PrefabWaiter.Compress();
			SpriteWaiter.Compress();
		}
	}
}
