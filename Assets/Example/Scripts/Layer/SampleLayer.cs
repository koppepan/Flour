using UnityEngine;
using UniRx.Async;

public class SampleLayer : FooterSubLayer
{
	public override void Close()
	{
		Debug.Log(SubLayer.ToString() + " close");
		base.Close();
	}
	public override void OnOpen()
	{
		Debug.Log(SubLayer.ToString() + " on open");
	}
	public override async UniTask OnClose()
	{
		Debug.Log(SubLayer.ToString() + " on close");
		await base.OnClose();
	}
	public override void OnBack()
	{
		Debug.Log(SubLayer.ToString() + " on back");
		base.OnBack();
	}

	public override void OnChangeSiblingIndex(int index)
	{
		Debug.Log(SubLayer.ToString() + " index " + index);
	}
}
