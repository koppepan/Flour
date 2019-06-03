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
	static JapaneaseAttributeCache<SceneType> jpnCache;

	static SceneTypeExtention()
	{
		jpnCache = new JapaneaseAttributeCache<SceneType>();
	}
	public static string ToJpnName(this SceneType type)
	{
		return jpnCache.GetJpnName(type);
	}
}
