using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	public class DebugDropdown : MonoBehaviour
	{
		[SerializeField]
		Dropdown dropdown = default;
		[SerializeField]
		Button selectButton = default;

		public void Setup(string[] contents, string defaultValue, System.Action<int, string> onSelect)
		{
			dropdown.ClearOptions();
			selectButton.onClick.RemoveAllListeners();

			var options = contents.Select(x => new Dropdown.OptionData(x)).ToList();
			dropdown.AddOptions(options);
			if (!string.IsNullOrEmpty(defaultValue))
			{
				dropdown.value = contents.ToList().IndexOf(defaultValue);
			}

			selectButton.onClick.AddListener(() => onSelect?.Invoke(dropdown.value, dropdown.options[dropdown.value].text));
		}
	}
}
