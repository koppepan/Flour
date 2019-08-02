using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Example
{
	class Repository<TKey, TValue>
	{
		private readonly TKey[] keys;
		private readonly Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();
		private readonly int cacheLimit;

		public Repository(IEnumerable<TKey> keys, int cacheLimit)
		{
			this.keys = keys.ToArray();
			this.cacheLimit = cacheLimit;
		}

		public bool ContainsKey(TKey key) => keys.Contains(key);

		public bool TryGet(TKey type, out TValue result)
		{
			result = default;

			if (!keys.Contains(type))
			{
				Debug.LogWarning(type.ToString() + " : unregistered key.");
				return false;
			}

			if (!cache.ContainsKey(type)) return false;

			result = cache[type];
			return true;
		}

		public void Add(TKey type, TValue src)
		{
			if (!keys.Contains(type))
			{
				Debug.LogWarning(type.ToString() + " : unregistered key.");
				return;
			}

			if (cache.ContainsKey(type))
			{
				var temp = cache[type];
				cache.Remove(type);
				cache.Add(type, temp);
				return;
			}

			cache.Add(type, src);

			if (cacheLimit > 0 && cache.Count > cacheLimit)
			{
				var remove = cache.FirstOrDefault(x => !x.Key.Equals(type));
				cache.Remove(remove.Key);
				remove = default;
			}
		}
	}
}
