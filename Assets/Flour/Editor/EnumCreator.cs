using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Flour
{
	/// <summary>
	/// Enumを生成するクラス
	/// </summary>
	public static class EnumCreator
	{
		public static void Create(string exportPath, string nameSpace, string summary, string enumName, string[] types)
		{
			if (string.IsNullOrEmpty(exportPath))
			{
				Debug.LogError("export path empty.");
				return;
			}
			if (string.IsNullOrEmpty(enumName))
			{
				Debug.LogError("enum name empty.");
				return;
			}

			string code = "";
			string tab = "";

			if (!string.IsNullOrEmpty(nameSpace))
			{
				code += "namespace " + nameSpace + "\n{\n";
				tab += "\t";
			}

			if (!string.IsNullOrEmpty(summary))
			{
				code +=
					tab + "/// <summary>\n" +
					tab + "/// " + summary + "\n" +
					tab + "/// </summary>\n";
			}

			code += tab + "public enum " + enumName + "\n" + tab + "{\n";
			tab += "\t";

			foreach (var type in types.OrderBy(x => x))
			{
				code += tab + type + ",\n";
			}

			tab = tab.Remove(0, "\n".Length);
			code += tab + "}\n";

			if (!string.IsNullOrEmpty(nameSpace))
			{
				tab = tab.Remove(0, "\n".Length);
				code += tab + "}\n";
			}

			var path = Path.Combine(exportPath, enumName + ".cs");
			File.WriteAllText(path, code, Encoding.UTF8);
			AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
			Debug.Log("created " + path);
		}
	}
}

