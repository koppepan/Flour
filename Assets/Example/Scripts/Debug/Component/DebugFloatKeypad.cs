using System;

namespace Example
{
	class DebugFloatKeypad : DebugKeypad, DebugDialog.IContent<double>
	{
		public void Setup(Action<double> onSelect)
		{
			SetupFloat(onSelect);
		}
		public double GetValue()
		{
			return double.Parse(input);
		}
	}
}
