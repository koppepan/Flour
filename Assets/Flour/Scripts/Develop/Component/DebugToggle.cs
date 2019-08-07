using UnityEngine;
using UnityEngine.UI;

namespace Flour.Develop
{
	[RequireComponent(typeof(Toggle))]
	class DebugToggle : MonoBehaviour, IContent<bool>
	{
		public void Setup(string title, bool value, System.Action<bool> onChanged)
		{
			transform.Find("Label").GetComponent<Text>().text = title;

			var t = GetComponent<Toggle>();

			t.isOn = value;
			t.onValueChanged.RemoveAllListeners();
			t.onValueChanged.AddListener(b => onChanged?.Invoke(b));
		}

		public bool GetValue()
		{
			return GetComponent<Toggle>().isOn;
		}
	}
}
