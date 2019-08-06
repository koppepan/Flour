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
	public struct PlayerBuildConfig
	{
		public string productName;
		public string bundleVersion;
		public int buildNumber;
		public string outputDirectory;
		public BuildTarget buildTarget;
		public BuildOptions options;
		public List<ConnectInfomation> connectInfomations;

		public BuildTargetGroup TargetGroup
		{
			get
			{
				switch (buildTarget)
				{
					case BuildTarget.StandaloneWindows64: return BuildTargetGroup.Standalone;
					case BuildTarget.StandaloneOSX: return BuildTargetGroup.Standalone;
					case BuildTarget.Android: return BuildTargetGroup.Android;
					case BuildTarget.iOS: return BuildTargetGroup.iOS;

					default: return BuildTargetGroup.Unknown;
				}
			}
		}

		public string OutputPath
		{
			get
			{
				var path = Path.Combine(outputDirectory, productName);

				switch (buildTarget)
				{
					case BuildTarget.StandaloneWindows64: return path + ".exe";
					case BuildTarget.StandaloneOSX: return path + ".app";
					case BuildTarget.Android: return path + ".apk";

					// iosはXcodeProjectがBuildされるためフォルダを指定
					case BuildTarget.iOS: return outputDirectory;

					default: return path;
				}
			}
		}
	}

	public static class Client
	{
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
						config.outputDirectory = args[i + 1];
						i++;
						break;

					case "BuildTarget":
						config.buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), args[i + 1]);
						i++;
						break;
				}
			}
			var report = Build(config);

			if (report.summary.result == BuildResult.Succeeded)
			{
				EditorApplication.Exit(0);
			}
			else
			{
				EditorApplication.Exit(1);
			}
		}

		public static BuildReport Build(PlayerBuildConfig config)
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
				EditorUserBuildSettings.SwitchActiveBuildTarget(config.TargetGroup, config.buildTarget);
			}

			var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
			{
				scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
				locationPathName = config.OutputPath,
				target = config.buildTarget,
				targetGroup = config.TargetGroup,
				options = config.options,
			});

			Debug.Log(
				$"[Result:{report.summary.result}] " +
				$"[Output:{report.summary.outputPath}] " +
				$"[TotalSize:{report.summary.totalSize}] " +
				$"[BuildTime:{report.summary.totalTime}] " +
				$"[Error:{report.summary.totalErrors}] " +
				$"[Warning:{report.summary.totalWarnings}] "
			);

			return report;
		}

		public static List<ConnectInfomation> GetServerList(string path)
		{
			var ini = new IniFile(path);

			return ini.GetSections()
				.Where(x => !string.IsNullOrEmpty(x))
				.Select(x =>
				{
					return new ConnectInfomation { name = x, api = ini.GetValue(x, "API"), assetBundle = ini.GetValue(x, "AssetBundle") };
				}).ToList();
		}

		public static void SetDefineSymboles(BuildTargetGroup targetGroup, string addSymbol, string removeSymbol)
		{
			SetDefineSymboles(targetGroup, new string[] { addSymbol }, new string[] { removeSymbol });
		}
		public static void SetDefineSymboles(BuildTargetGroup targetGroup, IEnumerable<string> addSymbols, IEnumerable<string> removeSymbols)
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').Where(x => !string.IsNullOrEmpty(x));

			symbols = symbols.Where(x => !removeSymbols.Any(y => x == y));
			symbols = symbols.Concat(addSymbols).Where(x => !string.IsNullOrEmpty(x));

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
