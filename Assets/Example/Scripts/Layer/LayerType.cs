using Flour;

namespace Example
{
	public abstract class AbstractSubLayer : Flour.Layer.AbstractSubLayer<LayerType, SubLayerType> { }

	public enum LayerType
	{
		[Int(0)] Scene = 0,

		[Int(10)] Back = 10,
		[Int(11)] Middle = 11,
		[Int(12)] Front = 12,
		[Int(13)] System = 13,

		[Int(100)] Debug = 100,
	}

	public static class LayerTypeExtention
	{
		static readonly AttributeCache<LayerType, int> cache = new AttributeCache<LayerType, int>();
		public static int ToOrder(this LayerType type) => cache[type];
	}

	public enum SubLayerType
	{
		[Japanease("")] None,
		[Japanease("UI/Blackout")] Blackout,
		[Japanease("UI/Progress")] Progress,

		[Japanease("UI/Splash")] Splash,
		[Japanease("UI/Title")] Title,

		[Japanease("UI/Footer")] Footer,
		[Japanease("UI/Sample1")] Sample1,
		[Japanease("UI/Sample2")] Sample2,
		[Japanease("UI/Sample3")] Sample3,
		[Japanease("UI/Sample4")] Sample4,


		[Japanease("UI/Debug/DebugDialog")] DebugDialog,
	}

	public static class SubLayerTypeExtention
	{
		static readonly AttributeCache<SubLayerType, string> cache = new AttributeCache<SubLayerType, string>();
		public static string ToResourcePath(this SubLayerType type) => cache[type];
	}
}
