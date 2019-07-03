using System;

namespace Flour.Asset
{
	public enum ErrorType
	{
		DownloadError,

		ClientError,
		ServerError,

		MissingAssetBundle,
		NotFoundAsset,
	}

	public struct LoadError
	{
		public ErrorType Error { get; private set; }
		public string AssetBundle { get; private set; }
		public string Asset { get; private set; }
		public Exception Exception { get; private set; }

		internal LoadError(ErrorType error, string assetBundle, string asset, Exception exception)
		{
			Error = error;
			AssetBundle = assetBundle;
			Asset = asset;
			Exception = exception;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Asset))
			{
				return $"{Error} : {AssetBundle} => {Exception}";
			}
			else
			{
				return $"{Error} : {AssetBundle}.{Asset} => {Exception}";
			}
		}
	}
}
