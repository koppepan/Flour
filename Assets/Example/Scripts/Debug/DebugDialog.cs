using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx.Async;

public class DebugDialog : Flour.Layer.AbstractSubLayer, IPointerDownHandler, IPointerUpHandler,  IDragHandler
{
	public override bool IgnoreBack => true;

	[SerializeField]
	private Text titleText = default;
	[SerializeField]
	private Button closeButton = default;

	[SerializeField]
	private DebugButton buttonPrefab = default;

	Func<string, UniTask<DebugDialog>> openDialogFunc;

	bool dragging = false;
	bool contentEnable = true;
	readonly Dictionary<string, GameObject> contents = new Dictionary<string, GameObject>();

	bool Frontmost => transform.GetSiblingIndex() == transform.parent.childCount - 1;

	public void Setup(string title, Func<string, UniTask<DebugDialog>> openDialogFunc)
	{
		titleText.text = title;
		closeButton.onClick.AddListener(Close);

		this.openDialogFunc = openDialogFunc;
	}
	public async UniTask<DebugDialog> Duplicate(string title)
	{
		var dialog = await openDialogFunc(title);
		dialog.transform.position = transform.position + new Vector3(40, -40);
		return dialog;
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

	public DebugDialog AddButton(string key, Action onClick)
	{
		if (contents.ContainsKey(key))
		{
			Debug.LogWarning($"already debug button key => {key}");
			return this;
		}
		var button = Instantiate(buttonPrefab, transform, false);
		button.Setup(key, onClick);
		button.gameObject.SetActive(contentEnable);
		contents.Add(key, button.gameObject);

		return this;
	}
}
