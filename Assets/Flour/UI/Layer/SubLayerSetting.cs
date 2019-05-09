using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flour.UI
{
	[CreateAssetMenu]
	public class SubLayerSetting : ScriptableObject
	{
		[Serializable]
		public struct SubLayer
		{
			public string typeName;
			public string srcPath;
		}

		public string exportPath;
		public List<SubLayer> settings = new List<SubLayer>();

		public Dictionary<SubLayerType, string> GetPaths()
		{
			return settings.ToDictionary(k => (SubLayerType)Enum.Parse(typeof(SubLayerType), k.typeName), v => v.srcPath);
		}
	}
}
