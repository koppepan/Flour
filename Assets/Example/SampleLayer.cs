﻿using UnityEngine;
using Flour.UI;

public class SampleLayer : AbstractSubLayer
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
	public override void OnClose()
	{
		Debug.Log(SubLayer.ToString() + " on close");
		base.OnClose();
	}
	public override void OnBack()
	{
		Debug.Log(SubLayer.ToString() + " on back");
		Close();
	}

	public override void OnChangeSiblingIndex(int index)
	{
		Debug.Log(SubLayer.ToString() + " index " + index);
	}
}
