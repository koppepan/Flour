using Flour;

public enum LayerType
{
	Back = 10,
	Middle = 11,
	Front = 12,
	System = 13,

	Debug = 100,
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
	static readonly JapaneaseAttributeCache<SubLayerType> jpnCache;

	static SubLayerTypeExtention() => jpnCache = new JapaneaseAttributeCache<SubLayerType>();
	public static string ToJpnName(this SubLayerType type) => jpnCache[type];
}
