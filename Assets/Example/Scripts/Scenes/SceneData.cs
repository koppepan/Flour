
public enum SceneType
{
	Start = 0,
	Title = 1,

	OutGame = 10,

	InGame = 20,
}

public static class SceneData
{
	static readonly string Format = "{0:00}_{1}";

	public static string GetName(SceneType type)
	{
		return string.Format(Format, (int)type, type.ToString());
	}
}
