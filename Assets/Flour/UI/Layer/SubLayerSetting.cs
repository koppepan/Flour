using System.Collections.Generic;
using UnityEngine;

namespace Flour.UI
{
	[CreateAssetMenu]
	public class SubLayerSetting : ScriptableObject
	{
		[System.Serializable]
		public struct SubLayer
		{
			public string typeName;
			public string srcPath;
		}

		public string exportPath;
		public List<SubLayer> settings = new List<SubLayer>();
	}
}
