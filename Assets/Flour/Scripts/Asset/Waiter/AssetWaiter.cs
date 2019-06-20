using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UniRx;

namespace Flour.Asset
{
	public interface IAssetRequest
	{
		string AssetBundleName { get; }
		string[] Dependencies { get; }
		string AssetName { get; }
	}
	public interface IWaiter
	{
		string Key { get; }
		void SetManifest(AssetBundleManifest manifest, Action<string, string[]> addRequest);

		IEnumerable<IAssetRequest> GetRequests(string assetBundleName);
		void OnDownloaded(string assetBundleName, string assetName, UnityEngine.Object asset);
	}

	public class AssetWaiter<T> :IWaiter where T : UnityEngine.Object
	{
		internal class Request : IAssetRequest
		{
			public string AssetBundleName { get; private set; }
			public string[] Dependencies { get; private set; }
			public string AssetName { get; private set; }

			public Subject<T> subject;

			public Request(string assetbundleName, string[] dependencies, string assetName, Subject<T> subject)
			{
				AssetBundleName = assetbundleName;
				Dependencies = dependencies;
				AssetName = assetName;

				this.subject = subject;
			}
			public bool Containts(string assetBundleName) => AssetBundleName == assetBundleName || Dependencies.Contains(assetBundleName);
		}

		public string Key { get; private set; }

		readonly List<Request> requests = new List<Request>();

		IDisposable disposable;

		AssetBundleManifest manifest;
		Action<string, string[]> addRequest;

		public AssetWaiter(string key) => Key = key;

		public void SetManifest(AssetBundleManifest manifest, Action<string, string[]> addRequest)
		{
			this.manifest = manifest;
			this.addRequest = addRequest;
		}

		public IEnumerable<IAssetRequest> GetRequests(string assetBundleName)
		{
			for (int i = 0; i < requests.Count; i++)
			{
				if (requests[i].Containts(assetBundleName)) yield return requests[i];
			}
		}

		public IObservable<T> LoadAsync(string assetName)
		{
			return LoadAsync(assetName, assetName);
		}

		public virtual IObservable<T> LoadAsync(string assetbundleName, string assetName)
		{
			var ab = Path.Combine(Key, assetbundleName);
			var req = new Request(ab, manifest.GetAllDependencies(ab), assetName, new Subject<T>());
			requests.Add(req);

			addRequest.Invoke(req.AssetBundleName, req.Dependencies);
			return req.subject;
		}

		protected virtual T GetAsset(UnityEngine.Object asset)
		{
			return (T)asset;
		}

		public void OnDownloaded(string assetBundleName, string assetName, UnityEngine.Object asset)
		{
			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var req = requests[i];
				if (req.AssetBundleName == assetBundleName && req.AssetName == assetName)
				{
					req.subject.OnNext(GetAsset(asset));
					req.subject.OnCompleted();

					requests.Remove(req);
				}
			}
		}
	}
}
