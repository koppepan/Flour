using UnityEngine;

namespace Flour.UI
{
	public class LayoutGroupList : LayoutGroup
	{
		protected override void SetLocalPosition(int elementCount, Vector2 elementSize)
		{
			int direction = scroll == Scroll.Horizontal ? 1 : -1;

			var pos = new Vector2(padding.left, -padding.top);

			for (int i = 0; i < elementCount; i++)
			{
				localPositionCache[i] = new Rect(pos, elementSize);
				pos[(int)scroll] += direction * (elementSize[(int)scroll] + spacing);
			}
		}
	}
}
