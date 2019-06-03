using System.Collections.Generic;
using System.Linq;

namespace Flour
{
	public class JapaneaseAttributeCache<T>
	{
		Dictionary<T, string> jpnCache;

		public JapaneaseAttributeCache()
		{
			var type = typeof(T);
			var lookup = type.GetFields()
				.Where(fi => fi.FieldType == type)
				.SelectMany(fi => fi.GetCustomAttributes(false), (fi, Attribute) => new { Type = (T)fi.GetValue(null), Attribute })
				.ToLookup(a => a.Attribute.GetType());

			jpnCache = lookup[typeof(JapaneaseAttribute)].ToDictionary(a => a.Type, a => ((JapaneaseAttribute)a.Attribute).Value);
		}

		public string GetJpnName(T type)
		{
			return jpnCache[type];
		}
	}
}
