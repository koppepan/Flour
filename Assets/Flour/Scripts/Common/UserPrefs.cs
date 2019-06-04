using System;

using UnityEngine;

namespace Flour
{
	public class UserPrefs : IDisposable
	{
		static readonly string LastTimeUserKey = "LastTimeUserKey";
		public string UserKey { get; private set; } = "default";

		readonly DataSerializer serializer = new DataSerializer();

		public UserPrefs()
		{
			ChangeUser(PlayerPrefs.GetString(LastTimeUserKey, UserKey));
		}
		public UserPrefs(string userKey)
		{
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
		

		private string GetKey(string key) => $"{UserKey}:{key}";

		public bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(GetKey(key));
		}
		public void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(GetKey(key));
			PlayerPrefs.Save();
		}

		public void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(GetKey(key), value);
			PlayerPrefs.Save();
		}
		public void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(GetKey(key), value);
			PlayerPrefs.Save();
		}
		public void SetString(string key, string value)
		{
			PlayerPrefs.SetString(GetKey(key), value);
			PlayerPrefs.Save();
		}
		public void SetValue<T>(string key, T value)
		{
			var str = serializer.Serialize<T>(value);
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			PlayerPrefs.SetString(GetKey(key), str);
			PlayerPrefs.Save();
		}

		public int GetInt(string key, int defaultValue = 0)
		{
			if (HasKey(GetKey(key)))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetInt(GetKey(key), defaultValue);
		}
		public float GetFloat(string key, float defaultValue = 0)
		{
			if (HasKey(GetKey(key)))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetFloat(GetKey(key), defaultValue);
		}
		public string GetString(string key, string defaultValue = "")
		{
			if (HasKey(GetKey(key)))
			{
				Debug.LogWarning($"key not found. {key}");
				return defaultValue;
			}
			return PlayerPrefs.GetString(GetKey(key), defaultValue);
		}
		public T GetValue<T>(string key)
		{
			if (HasKey(GetKey(key)))
			{
				Debug.LogWarning($"key not found. {key}");
				return default(T);
			}
			var str = PlayerPrefs.GetString(GetKey(key), "");
			return serializer.Deserialize<T>(str);
		}
	}
}
