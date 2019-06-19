using System;

namespace Example
{
	class DebugIntegerKeypad : DebugKeypad, DebugDialog.IContent<long>
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
