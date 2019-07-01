using System;

namespace Flour.Develop
{
	class DebugFloatKeypad : DebugKeypad, IContent<double>
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
