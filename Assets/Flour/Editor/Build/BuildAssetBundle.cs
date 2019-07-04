using System.Linq;
using System.IO;
using System.Text;
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

			Debug.Log($"done build {buildTarget} AssetBundles.");

			return manifest;
		}

		public static void BuildEncrypt(string srcPath, string outputPath, string password, AssetBundleManifest manifest)
		{
			if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

			foreach (var name in manifest.GetAllAssetBundles())
			{
				var folder = Path.Combine(outputPath, Path.GetDirectoryName(name));
				if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

				var uniqueSalt = Encoding.UTF8.GetBytes(name);

				var data = File.ReadAllBytes($"{srcPath}/{name}");
				using (var baseStream = new FileStream($"{outputPath}/{name}", FileMode.OpenOrCreate))
				{
					var cryptor = new SeekableAesStream(baseStream, password, uniqueSalt);
					cryptor.Write(data, 0, data.Length);
				}
			}

			Debug.Log($"done build Encrypt AssetBundles.");
		}

		public static void CreateAssetBundleSizeManifest(string directoryPath, string sizeFileName, AssetBundleManifest manifest)
		{
			var all = manifest.GetAllAssetBundles();
			var assetBunldes = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x) != ".manifest");

			using (var sw = File.CreateText(Path.Combine(directoryPath, sizeFileName)))
			{
				foreach (var ab in assetBunldes)
				{
					var name = ab.Remove(0, directoryPath.Length + 1).Replace('\\', '/');

					if (name == "AssetBundles" || all.Contains(name))
					{
						sw.WriteLine($"{name} {new FileInfo(ab).Length.ToString()}");
					}
				}
			}

			Debug.Log("done create AssetBundle size manifest.");
		}

		public static void CreateAssetBundleCrcManifest(string directoryPath, string crcFileName, AssetBundleManifest manifest)
		{
			var all = manifest.GetAllAssetBundles();

			using (var sw = File.CreateText(Path.Combine(directoryPath, crcFileName)))
			{
				for (int i = 0; i < all.Length; i++)
				{
					if (BuildPipeline.GetCRCForAssetBundle(Path.Combine(directoryPath, all[i]), out uint crc))
					{
						sw.WriteLine($"{all[i]} {crc}");
					}
				}
			}

			Debug.Log("done create AssetBundle crc manifest.");
		}

		public static void CleanUnnecessaryAssetBundles(string directoryPath, string manifestFileName, AssetBundleManifest manifest)
		{
			var all = manifest.GetAllAssetBundles();
			var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x) != ".manifest");

			foreach (var f in files)
			{
				var name = f.Remove(0, directoryPath.Length + 1).Replace('\\', '/');

				if (name == manifestFileName || name == $"{manifestFileName}.manifest")
				{
					continue;
				}

				if (!all.Contains(name))
				{
					FileDelete(f);
					FileDelete(f + ".manifest");
				}
			}

			var directoreis = Directory.GetDirectories(directoryPath);
			for (int i = 0; i < directoreis.Length; i++)
			{
				if (Directory.GetFiles(directoreis[i]).Length == 0)
				{
					Directory.Delete(directoreis[i]);
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
