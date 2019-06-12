using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Example
{
	[RequireComponent(typeof(ScrollRect))]
	public class TestList : MonoBehaviour
	{
		[SerializeField]
		Test prefab = default;

		ScrollRect scrollRect;
		Flour.UI.LayoutGroup layoutGroup;

		List<string> source = new List<string>();

		private void Awake()
		{
			Assert.IsNotNull(GetComponent<ScrollRect>(), "not found ScrollRect component.");
			scrollRect = GetComponent<ScrollRect>();

			Assert.IsNotNull(scrollRect.content.GetComponent<Flour.UI.LayoutGroup>(), "LayoutGroup is not attached to content.");
			layoutGroup = scrollRect.content.GetComponent<Flour.UI.LayoutGroup>();

			for (int i = 0; i < 10000; i++)
			{
				source.Add(i.ToString());
			}

			prefab.gameObject.SetActive(false);
			var rect = scrollRect.GetComponent<RectTransform>();
			layoutGroup.Initialize(scrollRect.horizontal, scrollRect.vertical, rect, source.Count, prefab.RectTransform.sizeDelta, Create);
		}

		Flour.UI.IListItem Create()
		{
			var item = Instantiate(prefab);
			item.Initilize(index => source[index]);
			return item;
		}
	}
}
