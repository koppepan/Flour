using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Flour
{
	public class SubLayerTypeCreator
	{
		const string ExportPath = "Assets/Flour/Scripts/Layer";

		const string NamespaceName = "Flour.Layer";
		const string TypeName = "SubLayerType";

		[MenuItem("Flour/Create SubLayerType Enum")]
		static void Create()
		{
			var asset = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Flour/Config/SubLayerType.txt", typeof(TextAsset));

			var ini = new IniFile(asset.text.Split('\n', '\r'));

			var contents = new Dictionary<string, string>() { { "None", "" } };
			foreach (var key in ini.GetKeys(TypeName).OrderBy(x => x))
			{
				contents.Add(key, ini.GetValue(TypeName, key));
			}
			EnumCreator.Create(ExportPath, NamespaceName, "", TypeName, contents);
		}
	}
}
