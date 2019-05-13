using UnityEngine;
using Flour.UI;

public class SampleLayer : AbstractSubLayer
{
	public override void Close()
	{
		Debug.Log("sample close");
		base.Close();
	}
	public override void OnOpen()
	{
		Debug.Log("sample on open");
	}
	public override void OnClose()
	{
		Debug.Log("sample on close");
		base.OnClose();
	}
	public override void OnBack()
	{
		Debug.Log("sample on back");
		Close();
	}

	public override void OnChangeSiblingIndex(int index)
	{
		Debug.Log("sample index " + index);
	}
}
