using System;
using System.Collections.Generic;
using System.Linq;

namespace Flour
{
	internal interface IValue<T>
	{
		T Value { get; }
	}

	public class AttributeCache<TKey, TValue>
	{
		readonly Dictionary<TKey, TValue> cache;

		public TValue this[TKey t] => cache[t];

		public AttributeCache()
		{
			var type = typeof(TKey);

			cache = type.GetFields()
				.Where(fi => fi.FieldType == type)
				.SelectMany(fi => fi.GetCustomAttributes(false), (fi, Attribute) => new { Type = (TKey)fi.GetValue(null), Attribute })
				.ToDictionary(k => k.Type, v => ((IValue<TValue>)v.Attribute).Value);
		}

		public bool TryGetKey(TValue valeu, out TKey key)
		{
			foreach (var c in cache)
			{
				if (c.Value.Equals(valeu))
				{
					key = c.Key;
					return true;
				}
			}
			key = default;
			return false;
		}
	}


	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class IntAttribute : Attribute, IValue<int>
	{
		public int Value { get; private set; }
		public IntAttribute(int value) => Value = value;
	}

	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class JapaneaseAttribute : Attribute, IValue<string>
	{
		public string Value { get; private set; }
		public JapaneaseAttribute(string value) => Value = value;
	}
}
