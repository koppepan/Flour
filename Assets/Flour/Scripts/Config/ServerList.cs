using System.Collections.Generic;
using UnityEngine;

namespace Flour.Config
{
	[System.Serializable]
	public struct ConnectInfomation
	{
		public string name;
		public string api;
		public string assetBundle;
	}

	[CreateAssetMenu()]
	public class ServerList : ScriptableObject
	{
		public List<ConnectInfomation> list = new List<ConnectInfomation>();
	}
}
