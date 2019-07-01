using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using Flour.Config;

namespace Flour.Build
{
	public static class BuildClient
	{
		public static void SetDefineSynboles(BuildTargetGroup targetGroup, IEnumerable<string> addSymbols, IEnumerable<string> removeSymbols)
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').Where(x => !string.IsNullOrEmpty(x));

			symbols = symbols.Where(x => !removeSymbols.Any(y => x == y));
			symbols = symbols.Concat(addSymbols);

			var joinSymbols = string.Join(";", symbols.OrderBy(x => x).Distinct());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, joinSymbols);

			Debug.Log($"apply define symbols => \"{joinSymbols}\"");
			AssetDatabase.SaveAssets();
		}

		public static void Build()
		{
		}

		[MenuItem("Flour/Build/Production")]
		public static void BuildProduction()
		{
			ApplyServerList("Production");
		}

		[MenuItem("Flour/Build/Development")]
		public static void BuildDevelopment()
		{
			ApplyServerList("Development");
		}

		private static void ApplyServerList(string key)
		{
			var ini = new IniFile(Path.Combine(Application.dataPath, $"../BuildConfig/{key}/ConnectInformation.ini"));

			var connectInfo = Resources.Load<ServerList>("Config/ServerList");
			connectInfo.list.Clear();

			foreach (var section in ini.GetSections())
			{
				if (string.IsNullOrEmpty(section)) continue;
				connectInfo.list.Add(new ConnectInfomation
				{
					name = section,
					api = ini.GetValue(section, "API"),
					assetBundle = ini.GetValue(section, "AssetBundle"),
				});
			}
		}
	}
}
