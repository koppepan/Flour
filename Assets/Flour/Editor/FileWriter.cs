using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UniRx;

namespace Flour
{
	internal class FileWriter : IDisposable
	{
		readonly string filePath;
		readonly StreamWriter sw;

		int tabCount = 0;

		private string Tab
		{
			get
			{
				var tab = "";
				for (int i = 0; i < tabCount; i++)
				{
					tab += "\t";
				}
				return tab;
			}
		}

		public FileWriter(string outputPath, string fileName)
		{
			filePath = Path.Combine(outputPath, fileName);
			sw = File.CreateText(filePath);
		}

		public void Dispose()
		{
			sw.Close();
			sw.Dispose();

			AssetDatabase.ImportAsset(filePath);
			Debug.Log("created " + filePath);
		}

		public void WriteUsing(string type) => sw.WriteLine($"{Tab}using {type};");
		public void WriteSummary(string summary)
		{
			if (!string.IsNullOrEmpty(summary))
			{
				sw.WriteLine($"{Tab}/// <summary>");
				sw.WriteLine($"{Tab}/// {summary}");
				sw.WriteLine($"{Tab}/// </summary>");
			}
		}

		public void WriteBody(string body) => sw.WriteLine($"{Tab}{body}");
		public void WriteLine() => sw.WriteLine("");


		public IDisposable StartNamespaceScope(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return Disposable.Empty;
			}
			sw.WriteLine($"{Tab}namespace {name}");
			return StartScope();
		}

		public IDisposable StartClassScope(string name)
		{
			sw.WriteLine($"{Tab}public class {name}");
			return StartScope();
		}
		public IDisposable StartEnumScope(string name)
		{
			sw.WriteLine($"{Tab}public enum {name}");
			return StartScope();
		}

		public IDisposable StartScope()
		{
			sw.WriteLine($"{Tab}{{");
			tabCount++;
			return Disposable.Create(() =>
			{
				tabCount--;
				sw.WriteLine($"{Tab}}}");
			});
		}
	}
}
