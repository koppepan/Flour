using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Flour
{
	/// <summary>
	/// Enumを生成するクラス
	/// </summary>
	public static class EnumCreator
	{
		public static void Create(string exportPath, string nameSpace, string summary, string enumName, IEnumerable<string> types)
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

			string tab = "";

			using (var sw = File.CreateText(Path.Combine(exportPath, enumName + ".cs")))
			{
				if (!string.IsNullOrEmpty(nameSpace))
				{
					sw.WriteLine($"namespace {nameSpace}");
					sw.WriteLine("{");

					AddTab(ref tab);
				}

				if (!string.IsNullOrEmpty(summary))
				{
					sw.WriteLine(tab + "/// <summary>");
					sw.WriteLine(tab + $"/// {summary}");
					sw.WriteLine(tab + "/// </summary>");
				}

				sw.WriteLine(tab + $"public enum {enumName}");
				sw.WriteLine(tab + "{");

				AddTab(ref tab);
				foreach (var type in types)
				{
					sw.WriteLine(tab + type + ",");
				}
				RemoveTab(ref tab);
				sw.WriteLine(tab + "}");

				if (!string.IsNullOrEmpty(nameSpace))
				{
					RemoveTab(ref tab);
					sw.WriteLine(tab + "}");
				}
			}

			var path = Path.Combine(exportPath, enumName + ".cs");
			AssetDatabase.ImportAsset(path);
			Debug.Log("created " + path);
		}

		static void AddTab(ref string tab)
		{
			tab += "\t";
		}
		static void RemoveTab(ref string tab)
		{
			tab = tab.Remove(0, "\t".Length);
		}
	}
}

