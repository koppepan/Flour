﻿using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	[RequireComponent(typeof(Button))]
	public class DebugButton : MonoBehaviour, DebugDialog.IContent<bool>
	{
		[SerializeField]
		private Text titleText = default;

		public void Setup(string title, System.Action onClick)
		{
			titleText.text = title;

			var button = GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => onClick?.Invoke());
		}
		public bool GetValue()
		{
			return GetComponent<Button>().interactable;
		}
	}
}
