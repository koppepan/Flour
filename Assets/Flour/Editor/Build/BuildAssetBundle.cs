using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Flour.Build
{
	public static class BuildAssetBundle
	{
		public static void Build(string outputPath)
		{
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
			BuildPipeline.BuildAssetBundles(outputPath, options, BuildTarget.StandaloneWindows64);

			Debug.Log("done build AssetBundles");
		}

		public static void CleanUnnecessaryAssetBundles(string outputPath)
		{
			var all = AssetDatabase.GetAllAssetBundleNames();
			var files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x) != "manifest");

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
