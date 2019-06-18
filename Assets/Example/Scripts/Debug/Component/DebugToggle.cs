using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	[RequireComponent(typeof(Toggle))]
	public class DebugToggle : MonoBehaviour
	{
		public void Setup(string title, System.Action<bool> onChanged)
		{
			transform.Find("Label").GetComponent<Text>().text = title;

			var t = GetComponent<Toggle>();
			t.onValueChanged.RemoveAllListeners();
			t.onValueChanged.AddListener(b => onChanged?.Invoke(b));
		}
	}
}
