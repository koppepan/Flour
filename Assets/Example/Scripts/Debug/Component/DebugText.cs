using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	[RequireComponent(typeof(Text))]
	public class DebugText : MonoBehaviour
	{
		public void Setup(string title)
		{
			GetComponent<Text>().text = title;
		}
	}
}
