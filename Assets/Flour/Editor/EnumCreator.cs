using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Flour
{
	/// <summary>
	/// Enumを生成するクラス
	/// </summary>
	public static class EnumCreator
	{
		public static void Create(string exportDirectory, string nameSpace, string summary, string enumName, IEnumerable<string> types)
		{
			if (string.IsNullOrEmpty(exportDirectory) || !Directory.Exists(exportDirectory))
			{
				Debug.LogError("export path empty.");
				return;
			}
			if (string.IsNullOrEmpty(enumName))
			{
				Debug.LogError("enum name empty.");
				return;
			}

			using (var fw = new FileWriter(exportDirectory, enumName + ".cs"))
			{
				fw.WriteSummary(summary);

				using (fw.StartNamespaceScope(nameSpace))
				{
					using (fw.StartScope($"public enum {enumName}"))
					{
						foreach (var t in types)
						{
							fw.WriteBody(t + ",");
						}
					}
				}
			}
		}
	}
}

