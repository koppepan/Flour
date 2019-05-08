using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

namespace Components.UI
{
	[AddComponentMenu("UI/ButtonEx", 30)]
	public class ButtonEx : Button
	{
		private const float DoubleClickInterval = 0.3f;

		[System.Serializable]
		public class ButtonDoubleClickEvent : UnityEvent { }

		[FormerlySerializedAs("onDoubleClick")]
		[SerializeField]
		ButtonDoubleClickEvent doubleClick = new ButtonDoubleClickEvent();

		public ButtonDoubleClickEvent onDoubleClick
		{
			get { return doubleClick; }
		}

		[SerializeField]
		private bool activateDoubleClick = false;

		private float lastClickTime;
		private Coroutine clickWaitCoroutine;

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (!IsActive() || !IsInteractable())
			{
				return;
			}

			if (!activateDoubleClick)
			{
				Click();
				return;
			}

			if (Time.time - lastClickTime < DoubleClickInterval)
			{
				DoubleClick();
			}
			else
			{
				clickWaitCoroutine = StartCoroutine(Wait(DoubleClickInterval, Click));
			}
			lastClickTime = Time.time;
		}

		IEnumerator Wait(float time, System.Action complete)
		{
			yield return new WaitForSeconds(time);
			complete?.Invoke();
		}

		private void Click()
		{
			lastClickTime = 0;
			UISystemProfilerApi.AddMarker("Button.onClick", this);
			onClick.Invoke();
		}

		private void DoubleClick()
		{
			if (clickWaitCoroutine != null)
			{
				StopCoroutine(clickWaitCoroutine);
				clickWaitCoroutine = null;
			}

			lastClickTime = 0;
			UISystemProfilerApi.AddMarker("Button.onDoubleClick", this);
			onDoubleClick.Invoke();
		}
	}
}
