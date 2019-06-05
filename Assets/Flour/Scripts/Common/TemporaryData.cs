using System;
using System.Collections.Generic;

namespace Flour
{
	public sealed class TemporaryData<TKey> : IDisposable where TKey : struct
	{
		private readonly DataSerializer serializer = new DataSerializer();
		private readonly Dictionary<TKey, string> cache = new Dictionary<TKey, string>();

		public TemporaryData()
		{
			if (!typeof(TKey).IsEnum)
			{
				throw new Exception("only enum can be used.");
			}
		}

		public void Dispose()
		{
			cache.Clear();
		}

		public void Add<T>(TKey key, T value)
		{
			cache[key] = serializer.Serialize<T>(value);
		}
		public T Get<T>(TKey key)
		{
			if (!cache.ContainsKey(key))
			{
				UnityEngine.Debug.LogWarning($"key not found. Key => {key}");
				return default(T);
			}
			return serializer.Deserialize<T>(cache[key]);
		}
		public void Remove(TKey key)
		{
			cache.Remove(key);
		}
	}
}
