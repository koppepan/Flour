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

			var contents = new List<string>() { "[Japanease(\"\")] None" };
			foreach (var key in ini.GetKeys(TypeName).OrderBy(x => x))
			{
				var valeu = ini.GetValue(TypeName, key);
				contents.Add($"[Japanease(\"{valeu}\")] {key}");
			}
			EnumCreator.Create(ExportPath, NamespaceName, "", TypeName, contents);
		}
	}
}
