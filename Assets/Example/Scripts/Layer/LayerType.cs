using Flour;

public enum LayerType
{
	[Int(10)] Back = 10,
	[Int(11)] Middle = 11,
	[Int(12)] Front = 12,
	[Int(13)] System = 13,

	[Int(100)] Debug = 100,
}

public static class LayerTypeExtention
{
	static readonly AttributeCache<LayerType, int> cache;

	static LayerTypeExtention() => cache = new AttributeCache<LayerType, int>();
	public static int ToOrder(this LayerType type) => cache[type];
}

public enum SubLayerType
{
	[Japanease("")] None,
	[Japanease("Blackout")] Blackout,

	[Japanease("Title")] Title,

	[Japanease("Footer")] Footer,
	[Japanease("Sample1")] Sample1,
	[Japanease("Sample2")] Sample2,
	[Japanease("Sample3")] Sample3,
	[Japanease("Sample4")] Sample4,


	[Japanease("Debug/DebugDialog")] DebugDialog,
}

public static class SubLayerTypeExtention
{
	static readonly AttributeCache<SubLayerType, string> cache;

	static SubLayerTypeExtention() => cache = new AttributeCache<SubLayerType, string>();
	public static string ToResourcePath(this SubLayerType type) => cache[type];
}
