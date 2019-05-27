using UnityEngine;

namespace Flour
{
	public class UserPrefs : System.IDisposable
	{
		static readonly string LastTimeUserKey = "LastTimeUserKey";
		public string UserKey { get; private set; } = "default";

		public UserPrefs()
		{
			ChangeUser(PlayerPrefs.GetString(LastTimeUserKey, "default"));
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
			var str = JsonUtility.ToJson(value);
			PlayerPrefs.SetString(GetKey(key), str);
			PlayerPrefs.Save();
		}

		public int GetInt(string key, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(GetKey(key), defaultValue);
		}
		public float GetFloat(string key, float defaultValue = 0)
		{
			return PlayerPrefs.GetFloat(GetKey(key), defaultValue);
		}
		public string GetString(string key, string defaultValue = "")
		{
			return PlayerPrefs.GetString(GetKey(key), defaultValue);
		}
		public T GetValue<T>(string key)
		{
			var str = PlayerPrefs.GetString(GetKey(key), "");
			if (string.IsNullOrEmpty(str))
			{
				Debug.LogWarning("UserPrefs.GetValue : key not found.");
				return default(T);
			}

			try
			{
				return JsonUtility.FromJson<T>(str);
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				return default(T);
			}
		}
	}
}
