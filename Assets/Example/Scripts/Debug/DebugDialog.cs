using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx.Async;

namespace Example
{
	public class DebugDialog : AbstractSubLayer, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public interface IContent<T>
		{
			T GetValue();
		}

		public override bool IgnoreBack => true;

		[SerializeField]
		private Text titleText = default;
		[SerializeField]
		private Button closeButton = default;

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
		private DebugInputField inputFieldPrefab = default;
		[SerializeField]
		private DebugInputDate inputDatePrefab = default;

		Func<string, Vector2, UniTask<DebugDialog>> openDialogFunc;

		bool dragging = false;
		bool contentEnable = true;
		readonly Dictionary<string, GameObject> contents = new Dictionary<string, GameObject>();

		bool Frontmost => transform.GetSiblingIndex() == transform.parent.childCount - 1;

		public void Setup(string title, Func<string, Vector2, UniTask<DebugDialog>> openDialogFunc)
		{
			titleText.text = title;
			closeButton.onClick.AddListener(() => Close());

			this.openDialogFunc = openDialogFunc;
		}
		public async UniTask<DebugDialog> CreateNewDialog(string title)
		{
			return await openDialogFunc(title, transform.position + new Vector3(40, -40));
		}

		public void OnPointerDown(PointerEventData eventData) { }
		public void OnPointerUp(PointerEventData eventData)
		{
			if (!dragging && Frontmost)
			{
				contentEnable = !contentEnable;
				foreach (var b in contents.Values)
				{
					b.gameObject.SetActive(contentEnable);
				}
			}
			dragging = false;
			transform.SetAsLastSibling();
		}
		public void OnDrag(PointerEventData eventData)
		{
			dragging = true;
			transform.SetAsLastSibling();
			transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0f);
		}

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
			content.gameObject.SetActive(contentEnable);
			contents.Add(key, content.gameObject);

			return content;
		}

		public DebugDialog AddText(string key)
		{
			var text = AddContent<DebugText>(key, textPrefab);
			text.Setup(key);
			return this;
		}
		public DebugDialog AddButton(string key, string title, Action onClick)
		{
			var button = AddContent<DebugButton>(key, buttonPrefab);
			button?.Setup(title, onClick);
			return this;
		}
		public DebugDialog AddFloatKeypad(string key, Action<double> onSelect)
		{
			var keypad = AddContent<DebugFloatKeypad>(key, floatKeypadPrefab);
			keypad?.Setup(onSelect);
			return this;
		}
		public DebugDialog AddIntegerKeypad(string key, Action<long> onSelect)
		{
			var keypad = AddContent<DebugIntegerKeypad>(key, integerKeypadPrefab);
			keypad?.Setup(onSelect);
			return this;
		}
		public DebugDialog AddDropdown(string key, string[] contents, string defaultValue, Action<int, string> onChanged)
		{
			var dropdown = AddContent<DebugDropdown>(key, dropdownPrefab);
			dropdown?.Setup(contents, defaultValue, onChanged);
			return this;
		}
		public DebugDialog AddToggle(string key, string title, Action<bool> onChanged)
		{
			var toggle = AddContent<DebugToggle>(key, togglePrefab);
			toggle?.Setup(title, onChanged);
			return this;
		}

		public DebugDialog AddInputField(string key, Action<string> onEndEdit)
		{
			var input = AddContent<DebugInputField>(key, inputFieldPrefab);
			input?.Setup(onEndEdit);
			return this;
		}
		public DebugDialog AddInputDate(string key, Action<DateTime> onChanged)
		{
			var input = AddContent<DebugInputDate>(key, inputDatePrefab);
			input?.Setup(DateTime.Now, onChanged);
			return this;
		}
	}
}

