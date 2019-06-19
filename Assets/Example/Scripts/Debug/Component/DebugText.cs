using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	[RequireComponent(typeof(Text))]
	public class DebugText : MonoBehaviour, DebugDialog.IContent<string>
	{
		public void Setup(string title)
		{
			GetComponent<Text>().text = title;
		}
		public string GetValue()
		{
			return GetComponent<Text>().text;
		}
	}
}
