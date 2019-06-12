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
	public abstract class LayoutGroup : MonoBehaviour
	{
		protected enum Scroll
		{
			Horizontal,
			Vertical,
		}

		[SerializeField]
		protected RectOffset padding = default;
		[SerializeField]
		protected float spacing = default;

		protected Scroll scroll;

		int elementCount;
		Func<IListItem> createItem;

		protected Rect LimitRect;
		protected Dictionary<int, Rect> localPositionCache = new Dictionary<int, Rect>();

		int renderingMin, renderingMax;
		Dictionary<int, IListItem> activeItems = new Dictionary<int, IListItem>();
		List<IListItem> poolItems = new List<IListItem>();


		RectTransform rectTransformCache;
		protected RectTransform RectTransform
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
			this.createItem = createItem;

			renderingMin = renderingMax = 0;
			var min = parent.TransformPoint(parent.rect.min);
			var max = parent.TransformPoint(parent.rect.max);
			LimitRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

			SetLocalPosition(elementCount, elementSize);
			StretchContentSize(elementCount, elementSize);
		}

		protected abstract void SetLocalPosition(int elementCount, Vector2 elementSize);

		Rect GetLocalPosition(int index)
		{
			return localPositionCache[index];
		}

		private void StretchContentSize(int elementCount, Vector2 elementSize)
		{
			var lastRect = GetLocalPosition(elementCount - 1);
			var contentSize = Mathf.Abs(lastRect.position[(int)scroll]) + lastRect.size[(int)scroll] + (scroll == Scroll.Horizontal ? padding.right : padding.bottom);

			if (scroll == Scroll.Horizontal)
			{
				RectTransform.LeftStretch(contentSize);
			}
			else
			{
				RectTransform.TopStretch(contentSize);
			}
		}

		void AddItem(int index)
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
				item.RectTransform.SetAlignment(TextAnchor.UpperLeft);
			}

			activeItems[index] = item;

			item.UpdateData(index);
			item.RectTransform.anchoredPosition = GetLocalPosition(index).position;
			item.RectTransform.gameObject.SetActive(true);
		}

		void ToPool(int index, IListItem item)
		{
			item.RectTransform.gameObject.SetActive(false);
			poolItems.Add(item);
			activeItems.Remove(index);
		}

		bool WithinRange(int index)
		{
			var localRect = GetLocalPosition(index);
			var worldPos = RectTransform.TransformPoint(localRect.position);

			return worldPos[(int)scroll] - localRect.size[(int)scroll] < LimitRect.max[(int)scroll] && worldPos[(int)scroll] > LimitRect.min[(int)scroll];
		}

		bool TryCreate(int index)
		{
			if (index < 0 || index >= elementCount) return false;
			if (activeItems.ContainsKey(index)) return true;

			if (WithinRange(index))
			{
				AddItem(index);
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
