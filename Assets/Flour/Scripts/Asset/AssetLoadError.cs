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
		public long HttpStatusCode { get; private set; }
		public string AssetBundle { get; private set; }
		public string Asset { get; private set; }
		public Exception Exception { get; private set; }

		internal LoadError(ErrorType error, long statusCode, string assetBundle, Exception exception)
		{
			Error = error;
			HttpStatusCode = statusCode;
			AssetBundle = assetBundle;
			Asset = "";
			Exception = exception;
		}

		internal LoadError(ErrorType error, string assetBundle, string asset, Exception exception)
		{
			Error = error;
			HttpStatusCode = -1;

			AssetBundle = assetBundle;
			Asset = asset;
			Exception = exception;
		}

		public override string ToString()
		{
			return $"AssetLoadError : {Error}\n status : {HttpStatusCode}\n name : {AssetBundle}.{Asset}\n {Exception}";
		}
	}
}
