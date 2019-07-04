using System;
using UnityEngine;
using UniRx.Async;
using Flour.Asset;

namespace Example
{
	public class AssetHandler
	{
		readonly bool crypto;
		private AssetBundleHandler handler;

		public SceneWaiter SceneWaiter { get; private set; }
		public GameObjectWaiter PrefabWaiter { get; private set; }
		public SpriteAssetWaiter SpriteWaiter { get; private set; }

		public IObservable<LoadError> ErrorObservable { get { return handler.ErrorObservable; } }

		public AssetHandler(string baseUrl)
		{
			handler = new AssetBundleHandler(baseUrl);
			CreateWaiter();
		}
		public AssetHandler(string baseUrl, string cachePath)
		{
			crypto = true;
			handler = new AssetBundleHandler(baseUrl, cachePath);
			CreateWaiter();
		}

		public void ChangeBaseUrl(string baseUrl) => handler.ChangeBaseUrl(baseUrl);

		public void Dispose()
		{
			handler.Dispose();
		}

		private void CreateWaiter()
		{
			SceneWaiter = new SceneWaiter("scenes/");

			PrefabWaiter = new GameObjectWaiter("prefabs/", 50);
			SpriteWaiter = new SpriteAssetWaiter("icons/", 50);
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
			var manifest = AssetHelper.GetAssetBundleFolderName(Application.platform);
			if (crypto)
			{
				manifest = AssetHelper.GetEncryptAssetBundleFolderName(Application.platform);
			}
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
