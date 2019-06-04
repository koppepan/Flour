using Flour;

public abstract class AbstractScene : Flour.Scene.AbstractScene<IOperationBundler>
{
	protected IOperationBundler AppOperator { get; private set; }

	protected IInputBinder InputBinder { get { return AppOperator.InputBinder; } }
	protected ISceneHandler SceneHandler { get { return AppOperator.SceneHandler; } }
	protected ILayerHandler LayerHandler { get { return AppOperator.LayerHandler; } }

	public override void SetParameter(IOperationBundler param) => AppOperator = param;


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
	static readonly AttributeCache<SceneType, string> cache;

	static SceneTypeExtention() => cache = new AttributeCache<SceneType, string>();
	public static string ToJpnName(this SceneType type) => cache[type];
}
