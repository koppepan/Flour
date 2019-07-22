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

		[MenuItem(DebugSymbolMenu, priority = DefineSymbolPriority)] static void SetDebugSymbol() => SetSymbolCheked(DebugSymbolMenu, DebugSymbol);
		[MenuItem(UseLocalAssetSymbolMenu, priority = DefineSymbolPriority)] static void SetUseLocalAssetSymbol() => SetSymbolCheked(UseLocalAssetSymbolMenu, UseLocalAssetSymbol);
		[MenuItem(UseSecureAssetSymbolMenu, priority = DefineSymbolPriority)] static void SetSecureAssetSymbol() => SetSymbolCheked(UseSecureAssetSymbolMenu, UseSecureAssetSymbol);

		static void SetSymbolCheked(string menu, string symbol)
		{
			var group = EditorUserBuildSettings.selectedBuildTargetGroup;
			var exist = Client.ExistsDefineSymbol(group, symbol);

			var add = !exist ? new string[] { symbol } : Enumerable.Empty<string>();
			var remove = exist ? new string[] { symbol } : Enumerable.Empty<string>();

			Client.SetDefineSymboles(group, add, remove);

			Menu.SetChecked(menu, !exist);
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
