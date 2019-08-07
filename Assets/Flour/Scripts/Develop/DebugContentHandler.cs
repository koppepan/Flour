using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flour.Develop
{
	public interface IContent<T>
	{
		T GetValue();
	}

	public class DebugContentHandler : MonoBehaviour
	{
		[SerializeField]
		private DebugText textPrefab = default;
		[SerializeField]
		private DebugButton buttonPrefab = default;
		[SerializeField]
		private DebugFloatKeypad floatKeypadPrefab = default;
		[SerializeField]
		private DebugIntegerKeypad integerKeypadPrefab = default;
		[SerializeField]
		private DebugDropdown dropdownPrefab = default;
		[SerializeField]
		private DebugToggle togglePrefab = default;
		[SerializeField]
		private DebugSlider sliderPrefab = default;

		[SerializeField]
		private DebugInputField inputFieldPrefab = default;
		[SerializeField]
		private DebugInputDate inputDatePrefab = default;

		bool _contentEnable = true;
		public bool ContentEnable
		{
			get { return _contentEnable; }
			set
			{
				_contentEnable = value;
				foreach (var c in contents) c.Value.SetActive(value);
			}
		}
		readonly Dictionary<string, GameObject> contents = new Dictionary<string, GameObject>();

		public void RemoveKey(params string[] keys)
		{
			foreach (var key in keys)
			{
				if (contents.ContainsKey(key))
				{
					Destroy(contents[key].gameObject);
					contents.Remove(key);
				}
			}
		}

		public T GetValeu<T>(string key)
		{
			if (!contents.ContainsKey(key))
			{
				Debug.LogWarning($"not found debug content key => {key}");
				return default;
			}
			return contents[key].GetComponent<IContent<T>>().GetValue();
		}

		private T AddContent<T>(string key, T prefab) where T : MonoBehaviour
		{
			if (contents.ContainsKey(key))
			{
				Debug.LogWarning($"already debug content key => {key}");
				return null;
			}

			var content = Instantiate(prefab, transform, false);
			content.gameObject.SetActive(ContentEnable);
			contents.Add(key, content.gameObject);

			return content;
		}

		public void AddText(string key)
		{
			var text = AddContent<DebugText>(key, textPrefab);
			text.Setup(key);
		}
		public void AddButton(string key, string title, Action onClick)
		{
			var button = AddContent<DebugButton>(key, buttonPrefab);
			button?.Setup(title, onClick);
		}
		public void AddFloatKeypad(string key, Action<double> onSelect = null)
		{
			var keypad = AddContent<DebugFloatKeypad>(key, floatKeypadPrefab);
			keypad?.Setup(onSelect);
		}
		public void AddIntegerKeypad(string key, Action<long> onSelect = null)
		{
			var keypad = AddContent<DebugIntegerKeypad>(key, integerKeypadPrefab);
			keypad?.Setup(onSelect);
		}
		public void AddDropdown(string key, string[] contents, string defaultValue, Action<int, string> onChanged = null)
		{
			var dropdown = AddContent<DebugDropdown>(key, dropdownPrefab);
			dropdown?.Setup(contents, defaultValue, onChanged);
		}
		public void AddToggle(string key, string title, Action<bool> onChanged = null)
		{
			var toggle = AddContent<DebugToggle>(key, togglePrefab);
			toggle?.Setup(title, onChanged);
		}
		public void AddSlider(string key, float value, float min, float max, Action<float> onChanged = null)
		{
			var slider = AddContent<DebugSlider>(key, sliderPrefab);
			slider.Setup(value, min, max, onChanged);
		}

		public void AddInputField(string key, Action<string> onEndEdit = null)
		{
			var input = AddContent<DebugInputField>(key, inputFieldPrefab);
			input?.Setup(onEndEdit);
		}
		public void AddInputDate(string key, Action<DateTime> onChanged = null)
		{
			var input = AddContent<DebugInputDate>(key, inputDatePrefab);
			input?.Setup(DateTime.Now, onChanged);
		}
	}
}
