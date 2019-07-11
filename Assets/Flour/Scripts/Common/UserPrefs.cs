using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Flour
{
	public sealed class UserPrefs<TKey> : IDisposable where TKey : struct
	{
		static readonly string DefaultUserKey = "default";
		static readonly string LastTimeUserKey = "LastTimeUserKey";
		public string UserKey { get; private set; } = DefaultUserKey;

		readonly DataSerializer serializer = new DataSerializer();

		public UserPrefs() : this(PlayerPrefs.GetString(LastTimeUserKey, DefaultUserKey)) { }
		public UserPrefs(string userKey)
		{
			Assert.IsTrue(typeof(TKey).IsEnum, "UserPrefs can use only enum.");
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


		private string GetKey(TKey key) => $"{UserKey}:{key.GetHashCode()}";
		private string GetKey(string userKey, TKey key) => $"{userKey}:{key.GetHashCode()}";

		public bool HasKey(TKey key) => PlayerPrefs.HasKey(GetKey(key));

		public void DeleteKey(TKey key)
		{
			PlayerPrefs.DeleteKey(GetKey(key));
			PlayerPrefs.Save();
		}
		private void DeleteKey(string userKey, TKey key)
		{
			if (PlayerPrefs.HasKey(GetKey(userKey, key)))
			{
				PlayerPrefs.DeleteKey(GetKey(userKey, key));
			}
			PlayerPrefs.Save();
		}
		public void DeleteUser(string userKey)
		{
			foreach (var key in EnumExtension.ToEnumerable<TKey>())
			{
				DeleteKey(userKey, key);
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
			if (!HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetInt(GetKey(key), defaultValue);
		}
		public float GetFloat(TKey key, float defaultValue = 0)
		{
			if (!HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetFloat(GetKey(key), defaultValue);
		}
		public string GetString(TKey key, string defaultValue = "")
		{
			if (!HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetString(GetKey(key), defaultValue);
		}
		public T GetValue<T>(TKey key)
		{
			if (!HasKey(key))
			{
				Debug.LogWarning($"key not found. {key}");
				return default(T);
			}
			var str = PlayerPrefs.GetString(GetKey(key), "");
			return serializer.Deserialize<T>(str);
		}
	}
}
