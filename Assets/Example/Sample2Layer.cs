using UnityEngine;
using Flour.UI;

public class Sample2Layer : AbstractSubLayer
{
	public override void OnOpen()
	{
		Debug.Log("sample2 open");
	}
	public override void OnClose()
	{
		Debug.Log("sample2 close");
		Destroy(gameObject);
	}
	public override void OnActivate()
	{
		Debug.Log("sample2 activate");
		gameObject.SetActive(true);
	}
	public override void OnInactivate()
	{
		Debug.Log("sample2 inactivate");
		gameObject.SetActive(false);
	}
	public override bool OnBackKey()
	{
		return false;
	}
}
