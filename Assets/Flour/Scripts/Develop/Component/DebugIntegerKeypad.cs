using System;

namespace Flour.Develop
{
	class DebugIntegerKeypad : DebugKeypad, IContent<long>
	{
		public void Setup(Action<long> onSelect)
		{
			SetupInteger(onSelect);
		}
		public long GetValue()
		{
			return long.Parse(input);
		}
	}
}
