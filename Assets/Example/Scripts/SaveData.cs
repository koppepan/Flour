using System.Linq;

public sealed class SaveData : System.IDisposable
{
	public enum Key
	{
	}

	readonly Flour.UserPrefs userPrefs;

	public SaveData() => userPrefs = new Flour.UserPrefs();
	public SaveData(string userId) => userPrefs = new Flour.UserPrefs(userId);
	public void Dispose() => userPrefs.Dispose();

	public void DeleteUser(string userId)
	{
		var keys = System.Enum.GetValues(typeof(Key)).Cast<Key>();
		foreach (var key in keys)
		{
			DeleteKey(key);
		}
	}
	public void DeleteKey(Key key) => userPrefs.DeleteKey(key.ToString());

	public void SetInt(Key key, int value) => userPrefs.SetInt(key.ToString(), value);
	public void SetFloat(Key key, int value) => userPrefs.SetFloat(key.ToString(), value);
	public void SetString(Key key, string value) => userPrefs.SetString(key.ToString(), value);
	public void SetValue<T>(Key key, T value) => userPrefs.SetValue<T>(key.ToString(), value);

	public int GetInt(Key key, int defautValue = 0) => userPrefs.GetInt(key.ToString(), defautValue);
	public float GetFloat(Key key, float defautValue = 0) => userPrefs.GetFloat(key.ToString(), defautValue);
	public string GetString(Key key, string defautValue = "") => userPrefs.GetString(key.ToString(), defautValue);
	public T GetValue<T>(Key key) => userPrefs.GetValue<T>(key.ToString());
}
