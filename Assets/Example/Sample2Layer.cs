using UnityEngine;
using Flour.UI;

public class Sample2Layer : AbstractSubLayer
{
	public override void Close()
	{
		Debug.Log("sample2 close");
		base.Close();
	}
	public override void OnOpen()
	{
		Debug.Log("sample2 on open");
	}
	public override void OnClose()
	{
		Debug.Log("sample2 on close");
		base.OnClose();
	}
	public override void OnBack()
	{
		Debug.Log("sample2 on back");
		Close();
	}

	public override void OnChangeSiblingIndex(int index)
	{
		Debug.Log("sample2 index " + index);
	}
}
