using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	[RequireComponent(typeof(Dropdown))]
	public class DebugDropdown : MonoBehaviour, DebugDialog.IContent<System.Tuple<int, string>>
	{
		public void Setup(string[] contents, string defaultValue, System.Action<int, string> onChanged)
		{
			var dropdown = GetComponent<Dropdown>();
			dropdown.ClearOptions();

			var options = contents.Select(x => new Dropdown.OptionData(x)).ToList();
			dropdown.AddOptions(options);
			if (!string.IsNullOrEmpty(defaultValue))
			{
				dropdown.value = contents.ToList().IndexOf(defaultValue);
			}

			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.onValueChanged.AddListener(_ => onChanged?.Invoke(dropdown.value, dropdown.options[dropdown.value].text));
		}

		public System.Tuple<int, string> GetValue()
		{
			var dropdown = GetComponent<Dropdown>();
			return System.Tuple.Create(dropdown.value, dropdown.options[dropdown.value].text);
		}
	}
}
