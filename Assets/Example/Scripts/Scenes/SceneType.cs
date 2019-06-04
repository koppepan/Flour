using Flour;

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
	static readonly AttributeCache<SceneType, string> cache;

	static SceneTypeExtention() => cache = new AttributeCache<SceneType, string>();
	public static string ToJpnName(this SceneType type) => cache[type];
}
