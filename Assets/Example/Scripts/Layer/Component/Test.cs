using System;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	public class Test : MonoBehaviour, Flour.UI.IListItem
	{
		[SerializeField]
		Text text = default;

		RectTransform cache;
		public RectTransform RectTransform
		{
			get
			{
				if (cache == null) cache = GetComponent<RectTransform>();
				return cache;
			}
		}

		Func<int, string> getElement;

		public void Initilize(Func<int, string> getElement)
		{
			this.getElement = getElement;
		}

		public void UpdateData(int index)
		{
			text.text = getElement(index);
		}
	}
}
