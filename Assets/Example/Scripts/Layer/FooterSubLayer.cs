using Flour.Layer;

public class FooterSubLayer : AbstractSubLayer
{
	System.Action onClose;

	public void Setup(System.Action onClose)
	{
		this.onClose = onClose;
	}

	public override void OnBack()
	{
		onClose?.Invoke();
	}
}
