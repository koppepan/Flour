using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Flour.Build;

namespace Example
{
	partial class CustomMenu
	{
		private const string DefineSymbolMenuTitle = MenuTitle + "/DefineSymbols";

		private static readonly string DebugSymbol = "DEBUG_BUILD";
		private const string DebugSymbolMenu = DefineSymbolMenuTitle + "/Debug";

		private static readonly string UseLocalAssetSymbol = "USE_LOCAL_ASSET";
		private const string UseLocalAssetSymbolMenu = DefineSymbolMenuTitle + "/Use Local Asset";

		private static readonly string UseSecureAssetSymbol = "USE_SECURE_ASSET";
		private const string UseSecureAssetSymbolMenu = DefineSymbolMenuTitle + "/Use Secure Asset";

		static BuildTargetGroup CurrentTarget => EditorUserBuildSettings.selectedBuildTargetGroup;

		[MenuItem(DebugSymbolMenu, priority = DefineSymbolPriority)] static void SetDebugSymbol() => SwitchSymbol(DebugSymbol);
		[MenuItem(UseLocalAssetSymbolMenu, priority = DefineSymbolPriority)] static void SetUseLocalAssetSymbol() => SwitchSymbol(UseLocalAssetSymbol, UseSecureAssetSymbol);
		[MenuItem(UseSecureAssetSymbolMenu, priority = DefineSymbolPriority)] static void SetSecureAssetSymbol() => SwitchSymbol(UseSecureAssetSymbol, UseLocalAssetSymbol);

		static void SwitchSymbol(string symbol, string remove = "")
		{
			if (Client.ExistsDefineSymbol(CurrentTarget, symbol))
			{
				Client.SetDefineSymboles(CurrentTarget, Enumerable.Empty<string>(), new string[] { symbol, remove });
			}
			else
			{
				Client.SetDefineSymboles(CurrentTarget, symbol, remove);
			}

			ApplySymbolMenuChecked();
		}

		static void ApplySymbolMenuChecked()
		{
			var group = EditorUserBuildSettings.selectedBuildTargetGroup;

			var list = new List<Tuple<string, string>>
			{
				Tuple.Create(DebugSymbolMenu, DebugSymbol),
				Tuple.Create(UseLocalAssetSymbolMenu, UseLocalAssetSymbol),
				Tuple.Create(UseSecureAssetSymbolMenu, UseSecureAssetSymbol),
			};

			for (int i = 0; i < list.Count; i++)
			{
				var symbol = Client.ExistsDefineSymbol(group, list[i].Item2);
				Menu.SetChecked(list[i].Item1, symbol);
			}
		}
	}
}
