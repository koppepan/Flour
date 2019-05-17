
public abstract class AbstractScene : Flour.Scene.AbstractScene<IOperationBundler>
{
	protected IOperationBundler AppOparator { get; private set; }

	protected IInputBinder InputBinder { get { return AppOparator.InputBinder; } }
	protected ISceneHandler SceneHandler { get { return AppOparator.SceneHandler; } }
	protected ILayerHandler LayerHandler { get { return AppOparator.LayerHandler; } }

	public override void SetParameter(IOperationBundler param) => AppOparator = param;
}
