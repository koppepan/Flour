using UnityEditor;
using Flour.Build;

namespace Example
{
	partial class CustomMenu
	{
		const string ClientBuildMenu = MenuTitle + "/Client Build";

		[MenuItem(ClientBuildMenu + "/Production", priority = BuildClientPriority)]
		public static void ClientBuildProduction()
		{
			Build("production", "1.0.0", 1, BuildTarget.StandaloneWindows64, "Production", BuildOptions.None);
		}

		[MenuItem(ClientBuildMenu + "/Development", priority = BuildClientPriority)]
		public static void ClientBuildDevelopment()
		{
			Build("development", "1.0.0", 1, BuildTarget.StandaloneWindows64, "Development", BuildOptions.Development);
		}

		static void Build(string productName, string bundleVersion, int buildNumber, BuildTarget buildTarget, string infoKey, BuildOptions options)
		{
			var outputDirectory = $"Builds/{infoKey}/{buildTarget.ToString()}";

			Client.Build(new PlayerBuildConfig
			{
				productName = productName,
				bundleVersion = bundleVersion,
				buildNumber = buildNumber,
				outputDirectory = outputDirectory,
				buildTarget = buildTarget,
				connectInfomations = Client.GetServerList(infoKey),
				options = options,
			});
		}
	}
}
