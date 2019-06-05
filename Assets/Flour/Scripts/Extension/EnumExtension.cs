using System;
using System.Collections.Generic;
using System.Linq;

namespace Flour
{
	public static class EnumExtension
	{
		public static IEnumerable<T> ToEnumerable<T>() where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new Exception("not an enum.");
			}
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static IEnumerable<T> ToEnumerable<T>(Func<T, bool> func) where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new Exception("not an enum.");
			}
			return Enum.GetValues(typeof(T)).Cast<T>().Where(func);
		}
	}
}
