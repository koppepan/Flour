
public abstract class AbstractScene : Flour.Scene.AbstractScene<IOperationBundler>
{
	protected IOperationBundler AppOperator { get; private set; }

	protected IInputBinder InputBinder { get { return AppOperator.InputBinder; } }
	protected ISceneHandler SceneHandler { get { return AppOperator.SceneHandler; } }
	protected ILayerHandler LayerHandler { get { return AppOperator.LayerHandler; } }

	public override void SetParameter(IOperationBundler param) => AppOperator = param;
}
