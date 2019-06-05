using System;
using System.Linq;
using UnityEngine;

namespace Flour
{
	public sealed class UserPrefs<TKey> : IDisposable where TKey : struct
	{
		static readonly string LastTimeUserKey = "LastTimeUserKey";
		public string UserKey { get; private set; } = "default";

		readonly DataSerializer serializer = new DataSerializer();

		public UserPrefs() : this(PlayerPrefs.GetString(LastTimeUserKey, "default")) { }
		public UserPrefs(string userKey)
		{
			if (!typeof(TKey).IsEnum)
			{
				throw new Exception("only enum can be used.");
			}
			ChangeUser(userKey);
		}

		public void ChangeUser(string userKey)
		{
			UserKey = userKey;
			PlayerPrefs.SetString(LastTimeUserKey, UserKey);
			PlayerPrefs.Save();
		}

		public void Dispose()
		{
			PlayerPrefs.SetString(LastTimeUserKey, UserKey);
			PlayerPrefs.Save();
		}
		

		private string GetKey(TKey key) => $"{UserKey}:{key}";

		public bool HasKey(TKey key) => PlayerPrefs.HasKey(GetKey(key));

		public void DeleteKey(TKey key)
		{
			PlayerPrefs.DeleteKey(GetKey(key));
			PlayerPrefs.Save();
		}
		public void DeleteUser(string userKey)
		{
			foreach (var key in Enum.GetValues(typeof(TKey)).Cast<TKey>())
			{
				DeleteKey(key);
			}
		}

		public void SetInt(TKey key, int value)
		{
			PlayerPrefs.SetInt(GetKey(key), value);
			PlayerPrefs.Save();
		}
		public void SetFloat(TKey key, float value)
		{
			PlayerPrefs.SetFloat(GetKey(key), value);
			PlayerPrefs.Save();
		}
		public void SetString(TKey key, string value)
		{
			PlayerPrefs.SetString(GetKey(key), value);
			PlayerPrefs.Save();
		}
		public void SetValue<T>(TKey key, T value)
		{
			var str = serializer.Serialize<T>(value);
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			PlayerPrefs.SetString(GetKey(key), str);
			PlayerPrefs.Save();
		}

		public int GetInt(TKey key, int defaultValue = 0)
		{
			if (HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetInt(GetKey(key), defaultValue);
		}
		public float GetFloat(TKey key, float defaultValue = 0)
		{
			if (HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetFloat(GetKey(key), defaultValue);
		}
		public string GetString(TKey key, string defaultValue = "")
		{
			if (HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetString(GetKey(key), defaultValue);
		}
		public T GetValue<T>(TKey key)
		{
			if (HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return default(T);
			}
			var str = PlayerPrefs.GetString(GetKey(key), "");
			return serializer.Deserialize<T>(str);
		}
	}
}
