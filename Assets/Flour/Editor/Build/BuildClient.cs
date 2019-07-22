using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Flour.Config;

namespace Flour.Build
{
	public static class Client
	{
		public struct PlayerBuildConfig
		{
			public string productName;
			public string bundleVersion;
			public int buildNumber;
			public string outputPath;
			public BuildTargetGroup targetGroup;
			public BuildTarget buildTarget;
			public BuildOptions options;
			public List<ConnectInfomation> connectInfomations;
		}

		public static void Build()
		{
			var command = Environment.CommandLine;
			var args = command.Split(' ');

			var config = new PlayerBuildConfig
			{
				options = BuildOptions.None,
				connectInfomations = GetServerList("Development"),
			};

			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "ProductName":
						config.productName = args[i + 1];
						i++;
						break;

					case "BundleVersion":
						config.bundleVersion = args[i + 1];
						i++;
						break;

					case "BuildNumber":
						config.buildNumber = int.Parse(args[i + 1]);
						i++;
						break;

					case "OutputPath":
						config.outputPath = args[i + 1];
						i++;
						break;

					case "TargetGroup":
						config.targetGroup = (BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), args[i + 1]);
						i++;
						break;

					case "BuildTarget":
						config.buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), args[i + 1]);
						i++;
						break;
				}
			}
			Build(config);
		}

		public static void Build(PlayerBuildConfig config)
		{
			PlayerSettings.productName = config.productName;
			PlayerSettings.bundleVersion = config.bundleVersion;

			if (config.buildTarget == BuildTarget.iOS)
			{
				PlayerSettings.iOS.buildNumber = config.buildNumber.ToString();
			}
			else if (config.buildTarget == BuildTarget.Android)
			{
				PlayerSettings.Android.bundleVersionCode = config.buildNumber;
			}

			var connectInfo = Resources.Load<ServerList>("Config/ServerList");
			connectInfo.list = config.connectInfomations;

			if (config.buildTarget != EditorUserBuildSettings.activeBuildTarget)
			{
				EditorUserBuildSettings.SwitchActiveBuildTarget(config.targetGroup, config.buildTarget);
			}

			var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
			{
				scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
				locationPathName = config.outputPath,
				target = config.buildTarget,
				targetGroup = config.targetGroup,
				options = config.options,
			});

			if (report.summary.result == BuildResult.Succeeded)
			{
				Debug.Log("Build succeeded: " + report.summary.totalSize + " bytes");
			}
			else if (report.summary.result == BuildResult.Failed)
			{
				Debug.Log("Build failed");
			}
		}

		public static List<ConnectInfomation> GetServerList(string key)
		{
			var ini = new IniFile(Path.Combine(Application.dataPath, $"../BuildConfig/{key}/ConnectInformation.ini"));

			return ini.GetSections()
				.Where(x => !string.IsNullOrEmpty(x))
				.Select(x =>
				{
					return new ConnectInfomation { name = x, api = ini.GetValue(x, "API"), assetBundle = ini.GetValue(x, "AssetBundle") };
				}).ToList();
		}

		public static void SetDefineSymboles(BuildTargetGroup targetGroup, IEnumerable<string> addSymbols, IEnumerable<string> removeSymbols)
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').Where(x => !string.IsNullOrEmpty(x));

			symbols = symbols.Where(x => !removeSymbols.Any(y => x == y));
			symbols = symbols.Concat(addSymbols);

			var joinSymbols = string.Join(";", symbols.OrderBy(x => x).Distinct());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, joinSymbols);

			Debug.Log($"apply define symbols => \"{joinSymbols}\"");
			AssetDatabase.SaveAssets();
		}

		public static bool ExistsDefineSymbol(BuildTargetGroup targetGroup, string value)
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').Where(x => !string.IsNullOrEmpty(x));
			return symbols.Any(x => x == value);
		}
	}
}
