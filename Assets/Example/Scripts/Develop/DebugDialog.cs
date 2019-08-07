using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx.Async;

namespace Example
{
	[RequireComponent(typeof(Flour.Develop.DebugContentHandler))]
	public class DebugDialog : AbstractSubLayer, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public override bool IgnoreBack => true;

		[SerializeField]
		private Text titleText = default;
		[SerializeField]
		private Button closeButton = default;

		private Flour.Develop.DebugContentHandler contentHandler = default;

		bool dragging = false;
		Func<string, Vector2, UniTask<DebugDialog>> openDialogFunc;

		bool Frontmost => transform.GetSiblingIndex() == transform.parent.childCount - 1;

		public void Setup(string title, Func<string, Vector2, UniTask<DebugDialog>> openDialogFunc)
		{
			contentHandler = GetComponent<Flour.Develop.DebugContentHandler>();

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
				contentHandler.ContentEnable = !contentHandler.ContentEnable;
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
			contentHandler.RemoveKey(keys);
		}

		public T GetValeu<T>(string key)
		{
			return contentHandler.GetValeu<T>(key);
		}

		public DebugDialog AddText(string key)
		{
			contentHandler.AddText(key);
			return this;
		}
		public DebugDialog AddButton(string key, string title, Action onClick)
		{
			contentHandler.AddButton(key, title, onClick);
			return this;
		}
		public DebugDialog AddFloatKeypad(string key, Action<double> onSelect = null)
		{
			contentHandler.AddFloatKeypad(key, onSelect);
			return this;
		}
		public DebugDialog AddIntegerKeypad(string key, Action<long> onSelect = null)
		{
			contentHandler.AddIntegerKeypad(key, onSelect);
			return this;
		}
		public DebugDialog AddDropdown(string key, string[] contents, string defaultValue, Action<int, string> onChanged = null)
		{
			contentHandler.AddDropdown(key, contents, defaultValue, onChanged);
			return this;
		}
		public DebugDialog AddToggle(string key, string title, bool value, Action<bool> onChanged = null)
		{
			contentHandler.AddToggle(key, title, value, onChanged);
			return this;
		}
		public DebugDialog AddSlider(string key, float value, float min, float max, Action<float> onChanged = null)
		{
			contentHandler.AddSlider(key, value, min, max, onChanged);
			return this;
		}

		public DebugDialog AddInputField(string key, Action<string> onEndEdit = null)
		{
			contentHandler.AddInputField(key, onEndEdit);
			return this;
		}
		public DebugDialog AddInputDate(string key, Action<DateTime> onChanged = null)
		{
			contentHandler.AddInputDate(key, onChanged);
			return this;
		}
	}
}

