using UnityEngine;
using Flour.UI;

public class Sample1Layer : AbstractSubLayer
{
	public override void OnOpen()
	{
		Debug.Log("sample1 open");
	}
	public override void OnClose()
	{
		Debug.Log("sample1 close");
		Destroy(gameObject);
	}
	public override void OnActivate()
	{
		Debug.Log("sample1 activate");
		gameObject.SetActive(true);
	}
	public override void OnInactivate()
	{
		Debug.Log("sample1 inactivate");
		gameObject.SetActive(false);
	}
	public override bool OnBackKey()
	{
		return false;
	}
}
