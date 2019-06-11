using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Flour.UI
{
	public interface IListItem
	{
		RectTransform RectTransform { get; }
		void UpdateData(int index);
	}

	[RequireComponent(typeof(RectTransform))]
	public class ListLayoutGroup : MonoBehaviour
	{
		private enum Scroll
		{
			Horizontal,
			Vertical,
		}

		[SerializeField]
		TextAnchor anchor = default;
		[SerializeField]
		RectOffset padding = default;
		[SerializeField]
		float spacing = default;

		Scroll scroll;

		int elementCount;
		Vector3 elementSize;

		Func<IListItem> createItem;

		int renderingMin, renderingMax;
		Tuple<float, float> LimitValue;

		Dictionary<int, IListItem> activeItems = new Dictionary<int, IListItem>();
		List<IListItem> poolItems = new List<IListItem>();


		RectTransform rectTransformCache;
		RectTransform RectTransform
		{
			get
			{
				if (rectTransformCache == null) rectTransformCache = GetComponent<RectTransform>();
				return rectTransformCache;
			}
		}

		public void Initialize(bool horizontal, bool vertical, RectTransform parent, int elementCount, Vector2 elementSize, Func<IListItem> createItem)
		{
			Assert.IsTrue(horizontal || vertical, "not selected scroll direction.");
			Assert.IsFalse(horizontal && vertical, "more than one scroll direction is selected.");

			scroll = horizontal ? Scroll.Horizontal : Scroll.Vertical;

			this.elementCount = elementCount;
			this.elementSize = elementSize;
			this.createItem = createItem;

			StretchContentSize(elementCount);

			renderingMin = renderingMax = 0;
			var min = parent.TransformPoint(parent.rect.min);
			var max = parent.TransformPoint(parent.rect.max);
			LimitValue = new Tuple<float, float>(max[(int)scroll], min[(int)scroll]);
		}

		private void StretchContentSize(int count)
		{
			var contentsSize = (elementSize[(int)scroll] * count) + (spacing * count) - spacing;
			var paddingSize = scroll == Scroll.Horizontal ? padding.horizontal : padding.vertical;

			var total = contentsSize + paddingSize;

			if (scroll == Scroll.Horizontal)
			{
				RectTransform.LeftStretch(total);
			}
			else
			{
				RectTransform.TopStretch(total);
			}
		}

		IListItem AddItem(int index)
		{
			IListItem item = null;

			if (poolItems.Count > 0)
			{
				item = poolItems[0];
				poolItems.Remove(item);
			}
			if (item == null)
			{
				item = createItem();
				item.RectTransform.SetParent(transform, false);
				item.RectTransform.SetAlignment(anchor);
			}

			item.RectTransform.gameObject.SetActive(true);
			activeItems[index] = item;
			return item;
		}

		void ToPool(int index, IListItem item)
		{
			item.RectTransform.gameObject.SetActive(false);
			poolItems.Add(item);
			activeItems.Remove(index);
		}

		bool WithinRange(int index)
		{
			var localPos = CalcLocalPosition(index);
			var worldPos = RectTransform.TransformPoint(localPos);
			return worldPos[(int)scroll] - elementSize[(int)scroll] < LimitValue.Item1 && worldPos[(int)scroll] > LimitValue.Item2;
		}

		Vector2 CalcLocalPosition(int index)
		{
			int direction = scroll == Scroll.Horizontal ? 1 : -1;

			var pos = new Vector2(padding.left, -padding.top);
			pos[(int)scroll] += ((direction * elementSize[(int)scroll]) + (direction * spacing)) * index;
			return pos;
		}

		bool TryCreate(int index)
		{
			if (index < 0 || index >= elementCount) return false;
			if (activeItems.ContainsKey(index)) return true;

			if (WithinRange(index))
			{
				var item = AddItem(index);
				item.RectTransform.anchoredPosition = CalcLocalPosition(index);
				item.UpdateData(index);
				return true;
			}

			return false;
		}

		bool TryDelete(int index)
		{
			if (index < 0 || index >= elementCount) return false;
			if (!activeItems.ContainsKey(index)) return false;

			if (!WithinRange(index))
			{
				ToPool(index, activeItems[index]);
				return true;
			}
			return false;
		}

		private void LateUpdate()
		{
			while (TryDelete(renderingMin + 1)) { renderingMin++; }
			while (TryDelete(renderingMax - 1)) { renderingMax--; }

			if (elementCount > 0 && activeItems.Count == 0)
			{
				int i = 0;
				for (i = 0; i < elementCount; i++)
				{
					if (WithinRange(i)) break;
				}
				renderingMin = i;
				renderingMax = i + 1;
			}

			while (TryCreate(renderingMin)) { renderingMin--; }
			while (TryCreate(renderingMax)) { renderingMax++; }
		}
	}
}
