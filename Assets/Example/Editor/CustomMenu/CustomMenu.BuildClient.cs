using UnityEditor;
using Flour.Build;

namespace Example
{
	partial class CustomMenu
	{
		[MenuItem(MenuTitle + "/Client Build/Production", priority = BuildClientPriority)]
		public static void ClientBuildProduction()
		{
			Client.Build(new Client.PlayerBuildConfig
			{
				productName = "production",
				bundleVersion = "1.0.0",
				buildNumber = 1,
				outputPath = "Builds/Production/Production.exe",
				targetGroup = BuildTargetGroup.Standalone,
				buildTarget = BuildTarget.StandaloneWindows64,
				connectInfomations = Client.GetServerList("Production"),
				options = BuildOptions.None,
			});
		}

		[MenuItem(MenuTitle + "/Client Build/Development", priority = BuildClientPriority)]
		public static void ClientBuildDevelopment()
		{
			Client.Build(new Client.PlayerBuildConfig
			{
				productName = "development",
				bundleVersion = "1.0.0",
				buildNumber = 1,
				outputPath = "Builds/Development/Development.exe",
				targetGroup = BuildTargetGroup.Standalone,
				buildTarget = BuildTarget.StandaloneWindows64,
				connectInfomations = Client.GetServerList("Development"),
				options = BuildOptions.Development,
			});
		}
	}
}
