using System.Collections.Generic;
using System.Linq;

namespace Flour
{
	public class JapaneaseAttributeCache<T>
	{
		readonly Dictionary<T, string> jpnCache;

		public string this[T t] => jpnCache[t];

		public JapaneaseAttributeCache()
		{
			var type = typeof(T);

			jpnCache = type.GetFields()
				.Where(fi => fi.FieldType == type)
				.SelectMany(fi => fi.GetCustomAttributes(false), (fi, Attribute) => new { Type = (T)fi.GetValue(null), Attribute })
				.ToDictionary(k => k.Type, v => ((JapaneaseAttribute)v.Attribute).Value);
		}
	}
}
