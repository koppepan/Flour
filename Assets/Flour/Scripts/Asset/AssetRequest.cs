using System.Linq;
using UniRx;

namespace Flour.Asset
{
	public interface IAssetRequest
	{
		string AssetBundleName { get; }
		string[] Dependencies { get; }
		string AssetName { get; }
	}

	internal class Request<T> : IAssetRequest where T : UnityEngine.Object
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
}
