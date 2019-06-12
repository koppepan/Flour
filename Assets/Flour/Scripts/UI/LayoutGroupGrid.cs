using UnityEngine;

namespace Flour.UI
{
	public class LayoutGroupGrid : LayoutGroup
	{
		protected override void SetLocalPosition(int elementCount, Vector2 elementSize)
		{
			Vector2 startPos, pos;
			startPos = pos = new Vector2(padding.left, -padding.top);

			var p = new Vector2(padding.right, padding.bottom);
			localPositionCache[0] = new Rect(pos, elementSize);

			for (int i = 1; i < elementCount; i++)
			{
				switch (scroll)
				{
					case Scroll.Horizontal:

						pos.y -= elementSize.y + spacing;

						if (RectTransform.TransformPoint(pos + elementSize + p).y > LimitRect.max.y)
						{
							pos.y = startPos.y;
							pos.x += elementSize.x + spacing;
						}

						break;

					case Scroll.Vertical:
						pos.x += elementSize.x + spacing;

						if (RectTransform.TransformPoint(pos + elementSize + p).x > LimitRect.max.x)
						{
							pos.x = startPos.x;
							pos.y -= elementSize.y + spacing;
						}
						break;
				}
				localPositionCache[i] = new Rect(pos, elementSize);
			}
		}
	}
}
