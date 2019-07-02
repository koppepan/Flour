using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Flour.Build
{
	public static class BuildAssetBundle
	{
		public static AssetBundleManifest Build(string outputPath, BuildTarget buildTarget, BuildAssetBundleOptions options = BuildAssetBundleOptions.None)
		{
			if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

			AssetDatabase.RemoveUnusedAssetBundleNames();
			var manifest = BuildPipeline.BuildAssetBundles(outputPath, options, buildTarget);

			Debug.Log($"done build {buildTarget} AssetBundles");

			return manifest;
		}

		public static void CreateAssetBundleSizeManifest(string assetBundleDirectoryPath, AssetBundleManifest manifest)
		{
			var all = manifest.GetAllAssetBundles();
			var assetBunldes = Directory.GetFiles(assetBundleDirectoryPath, "*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x) != ".manifest");

			using (var sw = File.CreateText(Path.Combine(assetBundleDirectoryPath, "AssetBundleSize")))
			{
				foreach (var ab in assetBunldes)
				{
					var name = ab.Remove(0, assetBundleDirectoryPath.Length + 1).Replace('\\', '/');

					if (name == "AssetBundles" || all.Contains(name))
					{
						sw.WriteLine($"{name} {new FileInfo(ab).Length.ToString()}");
					}
				}
			}

			Debug.Log($"done create AssetBundle size manifest");
		}

		public static void CleanUnnecessaryAssetBundles(string assetBundleDirectoryPath, AssetBundleManifest manifest)
		{
			var all = manifest.GetAllAssetBundles();
			var files = Directory.GetFiles(assetBundleDirectoryPath, "*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x) != ".manifest");

			foreach (var f in files)
			{
				var name = f.Remove(0, assetBundleDirectoryPath.Length + 1).Replace('\\', '/');

				if (name == "AssetBundles" || name == "AssetBundles.manifest")
				{
					continue;
				}

				if (!all.Contains(name))
				{
					FileDelete(f);
					FileDelete(f + ".manifest");
				}
			}

			Debug.Log("done clean unnecessary AssetBundles.");
		}

		private static void FileDelete(string path)
		{
			if (!File.Exists(path)) return;
			File.Delete(path);
		}
	}
}
