using UnityEngine;

namespace Flour.Config
{
	[CreateAssetMenu()]
	public class SecureParameter : ScriptableObject
	{
		public string Password { get { return _password; } }

		[SerializeField]
		private string _password = default;
	}
}
