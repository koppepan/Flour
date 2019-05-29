using UnityEngine;
using Flour.Layer;

class TitleLayer : AbstractSubLayer
{
	[SerializeField]
	UnityEngine.UI.Button button;

	public override bool IgnoreBack => true;

	System.Action onClick;
	public void Setup(System.Action onClick)
	{
		this.onClick = onClick;
	}
	protected override void OnOpen()
	{
		button.onClick.AddListener(() => onClick?.Invoke());
	}
}
