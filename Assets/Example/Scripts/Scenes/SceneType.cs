using System;
using Flour;

namespace Example
{
	public abstract class AbstractScene : Flour.Scene.AbstractScene<Tuple<IOperationBundler, IAssetHandler>>
	{
		protected IOperationBundler AppOperator { get; private set; }
		protected IAssetHandler AssetHandler { get; private set; }

		protected IInputBinder InputBinder { get { return AppOperator.InputBinder; } }
		protected ISceneHandler SceneHandler { get { return AppOperator.SceneHandler; } }
		protected ILayerHandler LayerHandler { get { return AppOperator.LayerHandler; } }

		public override void SetParameter(Tuple<IOperationBundler, IAssetHandler> param)
		{
			AppOperator = param.Item1;
			AssetHandler = param.Item2;
		}


		public virtual void OpenDebugDialog(DebugDialog dialog) { }
	}

	public enum SceneType
	{
		[Japanease("00_Start")]
		Start,
		[Japanease("01_Title")]
		Title,

		[Japanease("10_OutGame")]
		OutGame,

		[Japanease("20_InGame")]
		InGame,
	}

	public static class SceneTypeExtention
	{
		static readonly AttributeCache<SceneType, string> cache = new AttributeCache<SceneType, string>();
		public static string ToJpnName(this SceneType type) => cache[type];
	}
}
