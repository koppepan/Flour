using System;

namespace Flour
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class JapaneaseAttribute : Attribute
	{
		public string Value { get; private set; }
		public JapaneaseAttribute(string value) => Value = value;
	}
}
