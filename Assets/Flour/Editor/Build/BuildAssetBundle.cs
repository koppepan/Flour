using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Flour.Build
{
	public static class BuildAssetBundle
	{
		public static void Build(string outputPath, BuildAssetBundleOptions options = BuildAssetBundleOptions.None)
		{
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			AssetDatabase.RemoveUnusedAssetBundleNames();
			BuildPipeline.BuildAssetBundles(outputPath, options, BuildTarget.StandaloneWindows64);

			Debug.Log("done build AssetBundles");
		}

		public static void CreateAssetBundleSizeManifest(string assetBundleDirectoryPath)
		{
			AssetDatabase.RemoveUnusedAssetBundleNames();
			var all = AssetDatabase.GetAllAssetBundleNames();
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

			Debug.Log("done build AssetBundle size manifest");
		}

		public static void CleanUnnecessaryAssetBundles(string outputPath)
		{
			AssetDatabase.RemoveUnusedAssetBundleNames();
			var all = AssetDatabase.GetAllAssetBundleNames();
			var files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x) != ".manifest");

			foreach (var f in files)
			{
				var name = f.Remove(0, outputPath.Length + 1).Replace('\\', '/');

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
		}

		private static void FileDelete(string path)
		{
			if (!File.Exists(path)) return;
			File.Delete(path);
		}
	}
}
