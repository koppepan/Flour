using System;
using UniRx;

namespace Flour.Asset
{
	internal interface IAssetRequest
	{
		Type AssetType { get; }
		string[] AssetBundleNames { get; }
		string AssetName { get; }
	}

	internal class Request<T> : IAssetRequest where T : UnityEngine.Object
	{
		public Type AssetType { get { return typeof(T); } }
		public string[] AssetBundleNames { get; private set; }
		public string AssetName { get; private set; }

		public Subject<T> Subject { get; private set; } = new Subject<T>();


		public Request(string assetBundleName, string[] dependencies, string assetName)
		{
			AssetBundleNames = new string[dependencies.Length + 1];
			AssetBundleNames[0] = assetBundleName;
			for (int i = 0; i < dependencies.Length; i++)
			{
				AssetBundleNames[i + 1] = dependencies[i];
			}
			AssetName = assetName;
		}

		public bool Containts(string assetBundleName)
		{
			for (int i = 0; i < AssetBundleNames.Length; i++)
			{
				if (AssetBundleNames[i].Equals(assetBundleName, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		public bool Equals(string assetBundleName)
		{
			return AssetBundleNames[0].Equals(assetBundleName, StringComparison.Ordinal);
		}
		public bool Equals(string assetBundleName, string assetName)
		{
			var hit = Equals(assetBundleName);
			return string.IsNullOrEmpty(assetName) ? hit : hit && AssetName.Equals(assetName, StringComparison.Ordinal);
		}
	}
}
