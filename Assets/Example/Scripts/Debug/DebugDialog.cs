using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugDialog : Flour.Layer.AbstractSubLayer, IPointerDownHandler, IPointerUpHandler,  IDragHandler
{
	[SerializeField]
	private Text titleText = default;
	[SerializeField]
	private Button closeButton = default;

	[SerializeField]
	private DebugButton buttonPrefab = default;

	bool dragging = false;
	bool contentEnable = true;
	readonly Dictionary<string, GameObject> contents = new Dictionary<string, GameObject>();

	public void Setup(string title)
	{
		titleText.text = title;
		closeButton.onClick.AddListener(() => Close());
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

	public void OnPointerDown(PointerEventData eventData) { }
	public void OnPointerUp(PointerEventData eventData)
	{
		transform.SetAsFirstSibling();
		if (!dragging)
		{
			contentEnable = !contentEnable;
			foreach (var b in contents.Values)
			{
				b.gameObject.SetActive(contentEnable);
			}
		}
		dragging = false;
	}
	public void OnDrag(PointerEventData eventData)
	{
		dragging = true;
		transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0f);
	}
}
