using System.IO;
using UnityEngine;
using UnityEditor;
using Flour.Build;

namespace Example
{
	partial class CustomMenu
	{
		const string ClientBuildMenu = MenuTitle + "/Client Build";

		[MenuItem(ClientBuildMenu + "/Production/Windows", priority = BuildClientPriority)]
		public static void ClientBuildProductionForWindows() => Build("pro_win", BuildTarget.StandaloneWindows64, "Production", BuildOptions.None);

		[MenuItem(ClientBuildMenu + "/Production/OSX", priority = BuildClientPriority)]
		public static void ClientBuildProductionForOSX() => Build("pro_osx", BuildTarget.StandaloneOSX, "Production", BuildOptions.None);

		[MenuItem(ClientBuildMenu + "/Production/Android", priority = BuildClientPriority)]
		public static void ClientBuildProductionForAndroid() => Build("pro_and", BuildTarget.Android, "Production", BuildOptions.None);

		[MenuItem(ClientBuildMenu + "/Production/iOS", priority = BuildClientPriority)]
		public static void ClientBuildProductionForiOS() => Build("pro_ios", BuildTarget.iOS, "Production", BuildOptions.None);


		[MenuItem(ClientBuildMenu + "/Development/Windows", priority = BuildClientPriority)]
		public static void ClientBuildDevelopmentWindows() => Build("dev_win", BuildTarget.StandaloneWindows64, "Development", BuildOptions.Development);

		[MenuItem(ClientBuildMenu + "/Development/OSX", priority = BuildClientPriority)]
		public static void ClientBuildDevelopmentForOSX() => Build("dev_osx", BuildTarget.StandaloneOSX, "Development", BuildOptions.Development);

		[MenuItem(ClientBuildMenu + "/Development/Android", priority = BuildClientPriority)]
		public static void ClientBuildDevelopmentForAndroid() => Build("dev_and", BuildTarget.Android, "Development", BuildOptions.Development);

		[MenuItem(ClientBuildMenu + "/Development/iOS", priority = BuildClientPriority)]
		public static void ClientBuildDevelopmentForiOS() => Build("dev_ios", BuildTarget.iOS, "Development", BuildOptions.Development);


		static void Build(string productName, BuildTarget buildTarget, string infoKey, BuildOptions options)
		{
			var outputDirectory = $"Builds/{infoKey}/{buildTarget.ToString()}";
			var connectInfo = Client.GetServerList(Path.Combine(Application.dataPath, $"../BuildConfig/{infoKey}/ConnectInformation.ini"));

			Client.Build(new PlayerBuildConfig
			{
				productName = productName,
				bundleVersion = "1.0.0",
				buildNumber = 1,
				outputDirectory = outputDirectory,
				buildTarget = buildTarget,
				connectInfomations = connectInfo,
				options = options,
			});
		}
	}
}
