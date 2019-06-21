using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

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
	}
}
