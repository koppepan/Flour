﻿using UnityEngine;
using UnityEditor;

namespace Flour
{
	public class SubLayerTypeCreator
	{
		const string ExportPath = "Assets/Flour/UI/Layer";

		const string NamespaceName = "Flour.UI";
		const string TypeName = "SubLayerType";

		[MenuItem("Flour/Create SubLayerType Enum")]
		static void Create()
		{
			var asset = Resources.Load<TextAsset>("Config/SubLayerType");

			var ini = new IniFile(asset.text.Split('\n', '\r'));
			EnumCreator.Create(ExportPath, NamespaceName, "", TypeName, ini.GetKeys(TypeName));
		}
	}
}