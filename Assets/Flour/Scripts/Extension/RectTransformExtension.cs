using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flour
{
	public static class RectTransformExtension
	{
		static readonly Dictionary<TextAnchor, Tuple<Vector2, Vector2, Vector2>> AnchorValue = new Dictionary<TextAnchor, Tuple<Vector2, Vector2, Vector2>>
		{
			{ TextAnchor.UpperLeft, new Tuple<Vector2, Vector2, Vector2>(Vector2.up, Vector2.up, Vector2.up) },
			{ TextAnchor.UpperCenter, new Tuple<Vector2, Vector2, Vector2>(new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1)) },
			{ TextAnchor.UpperRight, new Tuple<Vector2, Vector2, Vector2>(Vector2.one, Vector2.one, Vector2.one) },

			{ TextAnchor.MiddleLeft, new Tuple<Vector2, Vector2, Vector2>(Vector2.up * 0.5f, Vector2.up * 0.5f, Vector2.up * 0.5f) },
			{ TextAnchor.MiddleCenter, new Tuple<Vector2, Vector2, Vector2>(Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f) },
			{ TextAnchor.MiddleRight, new Tuple<Vector2, Vector2, Vector2>(new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f)) },

			{ TextAnchor.LowerLeft, new Tuple<Vector2, Vector2, Vector2>(Vector2.zero, Vector2.zero, Vector2.zero) },
			{ TextAnchor.LowerCenter, new Tuple<Vector2, Vector2, Vector2>(Vector2.right * 0.5f, Vector2.right * 0.5f, Vector2.right * 0.5f) },
			{ TextAnchor.LowerRight, new Tuple<Vector2, Vector2, Vector2>(Vector2.right, Vector2.right, Vector2.right) },
		};

		public static void SetAlignment(this RectTransform self, TextAnchor anchor)
		{
			self.anchorMin = AnchorValue[anchor].Item1;
			self.anchorMax = AnchorValue[anchor].Item2;
			self.pivot = AnchorValue[anchor].Item3;

			self.anchoredPosition = Vector2.zero;
		}

		internal static void SetStretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 size)
		{
			rect.pivot = pivot;
			rect.anchorMin = anchorMin;
			rect.anchorMax = anchorMax;
			rect.sizeDelta = size;
		}

		public static void Stretch(this RectTransform self)
		{
			SetStretch(self, Vector2.zero, Vector2.one, Vector2.one * 0.5f, Vector2.zero);
		}

		public static void TopStretch(this RectTransform self, float height)
		{
			SetStretch(self, Vector2.up, Vector2.one, new Vector2(0.5f, 1), new Vector2(0, height));
		}
		public static void MiddleStretch(this RectTransform self, float height)
		{
			SetStretch(self, Vector2.up * 0.5f, new Vector2(1, 0.5f), Vector2.one * 0.5f, new Vector2(0, height));
		}
		public static void BottomStretch(this RectTransform self, float height)
		{
			SetStretch(self, Vector2.zero, Vector2.right, Vector2.right * 0.5f, new Vector2(0, height));
		}

		public static void LeftStretch(this RectTransform self, float width)
		{
			SetStretch(self, Vector2.zero, Vector2.up, Vector2.up * 0.5f, new Vector2(width, 0));
		}
		public static void CenterStretch(this RectTransform self, float width)
		{
			SetStretch(self, Vector2.right * 0.5f, new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f), new Vector2(width, 0));
		}
		public static void RightStretch(this RectTransform self, float width)
		{
			SetStretch(self, Vector2.right, Vector2.one, new Vector2(1, 0.5f), new Vector2(width, 0));
		}
			 
	}
}
