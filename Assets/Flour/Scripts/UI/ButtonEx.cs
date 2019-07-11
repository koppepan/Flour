using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Flour.UI
{
	[AddComponentMenu("UI/ButtonEx", 30)]
	public class ButtonEx : Button
	{
		private const float DoubleClickInterval = 0.3f;
		private const float HoldTime = 0.8f;

		[System.Serializable]
		public class ButtonDoubleClickEvent : UnityEvent { }

		[System.Serializable]
		public class ButtonHoldEvent : UnityEvent { }

		[FormerlySerializedAs("onDoubleClick")]
		[SerializeField]
		ButtonDoubleClickEvent doubleClick = new ButtonDoubleClickEvent();

		[FormerlySerializedAs("onHold")]
		[SerializeField]
		ButtonHoldEvent hold = new ButtonHoldEvent();

		public ButtonDoubleClickEvent onDoubleClick
		{
			get { return doubleClick; }
		}
		public ButtonHoldEvent onHold
		{
			get { return hold; }
		}

		[SerializeField]
		private bool activateDoubleClick = false;
		[SerializeField]
		private bool activateHold = false;

		private readonly float[] downTimeQueue = new float[2] { 0, 0 };

		private bool excutedHold = false;
		private Coroutine holdWaitCoroutine;

		private bool Interactable(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return false;
			}
			if (!IsActive() || !IsInteractable())
			{
				return false;
			}
			return true;
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			// NOTE : DownとUpで制御したいのでClickでは何もしない
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!Interactable(eventData))
			{
				return;
			}
			base.OnPointerDown(eventData);

			if (activateDoubleClick)
			{
				downTimeQueue[0] = downTimeQueue[1];
				downTimeQueue[1] = Time.time;

				if (downTimeQueue[1] - downTimeQueue[0] <= DoubleClickInterval)
				{
					DoubleClick();
					return;
				}
			}

			if (activateHold)
			{
				holdWaitCoroutine = StartCoroutine(Wait(HoldTime, Hold));
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (holdWaitCoroutine != null)
			{
				StopCoroutine(holdWaitCoroutine);
				holdWaitCoroutine = null;
			}

			if (!Interactable(eventData))
			{
				return;
			}
			base.OnPointerUp(eventData);

			if (!excutedHold)
			{
				if (activateDoubleClick)
				{
					if (downTimeQueue[1] - downTimeQueue[0] > DoubleClickInterval)
					{
						Click();
					}
				}
				else
				{
					Click();
				}
			}
			excutedHold = false;
		}


		private void Click()
		{
			UISystemProfilerApi.AddMarker("Button.onClick", this);
			onClick.Invoke();
		}

		private void DoubleClick()
		{
			if (!activateDoubleClick)
			{
				return;
			}
			UISystemProfilerApi.AddMarker("Button.onDoubleClick", this);
			onDoubleClick.Invoke();
		}

		private void Hold()
		{
			if (!activateHold)
			{
				excutedHold = false;
				return;
			}
			UISystemProfilerApi.AddMarker("Button.onHold", this);
			onHold.Invoke();
			excutedHold = true;
		}

		IEnumerator Wait(float time, System.Action complete)
		{
			yield return new WaitForSeconds(time);
			complete?.Invoke();
		}
	}
}
