using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Flour
{
	public class SubLayerTypeCreator
	{
		const string ExportPath = "Assets/Flour/Scripts/UI/Layer";

		const string NamespaceName = "Flour.UI";
		const string TypeName = "SubLayerType";

		[MenuItem("Flour/Create SubLayerType Enum")]
		static void Create()
		{
			var asset = Resources.Load<TextAsset>("Config/SubLayerType");

			var ini = new IniFile(asset.text.Split('\n', '\r'));

			var contetns = new List<string>(ini.GetKeys(TypeName).OrderBy(x => x));
			contetns.Insert(0, "None");
			EnumCreator.Create(ExportPath, NamespaceName, "", TypeName, contetns);
		}
	}
}
