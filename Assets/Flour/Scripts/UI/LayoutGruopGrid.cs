using UnityEngine;

namespace Flour.UI
{
	public class LayoutGruopGrid : LayoutGroup
	{
		protected override void SetLocalPosition(int elementCount, Vector2 elementSize)
		{
			Vector2 startPos, pos;
			startPos = pos = new Vector2(padding.left, -padding.top);

			for (int i = 0; i < elementCount; i++)
			{
				localPositionCache[i] = new Rect(pos, elementSize);

				switch (scroll)
				{
					case Scroll.Horizontal:

						pos.y -= elementSize.y + spacing;

						if (RectTransform.TransformPoint(pos + elementSize).y > LimitRect.max.y)
						{
							pos.y = startPos.y;
							pos.x += elementSize.x + spacing;
						}

						break;

					case Scroll.Vertical:
						pos.x += elementSize.x + spacing;

						if (RectTransform.TransformPoint(pos + elementSize).x > LimitRect.max.x)
						{
							pos.x = startPos.x;
							pos.y -= elementSize.y + spacing;
						}
						break;
				}
			}
		}
	}
}
