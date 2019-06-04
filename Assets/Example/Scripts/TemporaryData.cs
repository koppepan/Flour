using System;
using System.Collections.Generic;
using Flour;

public sealed class TemporaryData : IDisposable
{
	public enum Key
	{
	}

	private readonly DataSerializer serializer = new DataSerializer();
	private readonly Dictionary<Key, string> cache = new Dictionary<Key, string>();

	public void Dispose()
	{
		cache.Clear();
	}

	public void Add<T>(Key key, T value)
	{
		cache[key] = serializer.Serialize<T>(value);
	}
	public T Get<T>(Key key)
	{
		if (!cache.ContainsKey(key))
		{
			UnityEngine.Debug.LogWarning($"key not found. Key => {key}");
			return default(T);
		}
		return serializer.Deserialize<T>(cache[key]);
	}
	public void Remove(Key key)
	{
		cache.Remove(key);
	}
}
