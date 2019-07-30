using System;
using System.Security;
using UnityEngine;
using UniRx.Async;
using Flour.Asset;

namespace Example
{
	public class AssetHandler
	{
		readonly string assetFolderName;
		readonly AssetBundleHandler handler;

		public AssetWaiter<UnityEngine.Object> SceneWaiter { get; private set; } = new AssetWaiter<UnityEngine.Object>("scenes/");
		public AssetCacheWaiter<GameObject> PrefabWaiter { get; private set; } = new AssetCacheWaiter<GameObject>("prefabs/", 50);
		public AssetCacheWaiter<Sprite> SpriteWaiter { get; private set; } = new AssetCacheWaiter<Sprite>("icons/", 50);

		public IObservable<LoadError> ErrorObservable { get { return handler.ErrorObservable; } }

		public AssetHandler(string baseUrl)
		{
			assetFolderName = AssetHelper.GetAssetBundleFolderName(Application.platform);
			handler = new AssetBundleHandler(baseUrl);

			RegisterWaiter();
		}
		public AssetHandler(string baseUrl, string cachePath, SecureString pass)
		{
			assetFolderName = AssetHelper.GetEncryptAssetBundleFolderName(Application.platform);
			handler = new SecureAssetBundleHandler(baseUrl, cachePath, pass);

			RegisterWaiter();
		}

		private void RegisterWaiter()
		{
			handler.AddWaiter(SceneWaiter);
			handler.AddWaiter(PrefabWaiter);
			handler.AddWaiter(SpriteWaiter);
		}

		public void Compress()
		{
			PrefabWaiter.Compress();
			SpriteWaiter.Compress();
		}

		public void Dispose() => handler.Dispose();

		public void ChangeBaseUrl(string baseUrl) => handler.ChangeBaseUrl(System.IO.Path.Combine(baseUrl, assetFolderName));

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
		}
	}
}
