using System;
using System.Linq;
using UniRx;

namespace Flour.Asset
{
	public interface IAssetRequest
	{
		string[] AssetBundleNames { get; }
		string AssetName { get; }
	}

	internal class Request<T> : IAssetRequest where T : UnityEngine.Object
	{
		public string[] AssetBundleNames { get; private set; }
		public string AssetName { get; private set; }

		public Subject<T> subject;


		public Request(string assetBundleName, string[] dependencies, string assetName, Subject<T> subject)
		{
			AssetBundleNames = new string[dependencies.Length + 1];
			AssetBundleNames[0] = assetBundleName;
			for (int i = 0; i < dependencies.Length; i++)
			{
				AssetBundleNames[i + 1] = dependencies[i];
			}
			AssetName = assetName;

			this.subject = subject;
		}

		public bool Containts(string assetBundleName) => AssetBundleNames.Contains(assetBundleName);

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
