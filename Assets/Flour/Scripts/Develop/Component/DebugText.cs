using UnityEngine;
using UnityEngine.UI;

namespace Flour.Develop
{
	[RequireComponent(typeof(Text))]
	class DebugText : MonoBehaviour, IContent<string>
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
