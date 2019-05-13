using UnityEngine;
using Flour.UI;

public class Sample1Layer : AbstractSubLayer
{
	public override void Close()
	{
		Debug.Log("sample1 close");
		base.Close();
	}
	public override void OnOpen()
	{
		Debug.Log("sample1 on open");
	}
	public override void OnClose()
	{
		Debug.Log("sample1 on close");
		base.OnClose();
	}
	public override void OnBack()
	{
		Debug.Log("sample1 on back");
		Close();
	}

	public override void OnChangeSiblingIndex(int index)
	{
		Debug.Log("sample1 index " + index);
	}
}
