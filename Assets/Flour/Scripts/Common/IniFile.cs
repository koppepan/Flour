using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Flour
{
	public class IniFile
	{
		private readonly Regex RegexSection = new Regex(@"^\s*\[(?<section>[^\]]+)\].*$", RegexOptions.Singleline | RegexOptions.CultureInvariant);
		private readonly Regex RegexNameValue = new Regex(@"^\s*(?<name>[^=]+)=(?<value>.*?)(\s+;(?<comment>.*))?$", RegexOptions.Singleline | RegexOptions.CultureInvariant);

		Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();
		string currentSection = string.Empty;

		public IniFile(string filePath)
		{
			sections.Add(currentSection, new Dictionary<string, string>());

			using (var reader = new StreamReader(filePath))
			{
				while (!reader.EndOfStream)
				{
					ParseLine(reader.ReadLine());
				}
			}
		}
		public IniFile(string[] contents)
		{
			sections.Add(currentSection, new Dictionary<string, string>());
			for (int i = 0; i < contents.Length; i++)
			{
				ParseLine(contents[i]);
			}
		}


		void ParseLine(string line)
		{
			if (string.IsNullOrEmpty(line) || line.Length == 0)
			{
				return;
			}
			if (line.StartsWith(";", System.StringComparison.Ordinal))
			{
				return;
			}

			var matchName = RegexNameValue.Match(line);
			if (matchName.Success)
			{
				sections[currentSection][matchName.Groups["name"].Value.Trim()] = matchName.Groups["value"].Value.Trim();
				return;
			}

			var section = RegexSection.Match(line);
			if (section.Success)
			{
				currentSection = section.Groups["section"].Value.Trim();

				if (!sections.ContainsKey(currentSection))
				{
					sections.Add(currentSection, new Dictionary<string, string>());
				}
			}
		}

		public IEnumerable<string> GetSections()
		{
			return sections.Keys;
		}
		public IEnumerable<string> GetKeys(string section)
		{
			if (!sections.ContainsKey(section))
			{
				return Enumerable.Empty<string>();
			}
			return sections[section].Keys;
		}
		public Dictionary<string, string> GetContents(string section)
		{
			if (!sections.ContainsKey(section))
			{
				return new Dictionary<string, string>();
			}
			return sections[section];
		}

		public string GetValue(string section, string key)
		{
			if (!sections.ContainsKey(section) || !sections[section].ContainsKey(key))
			{
				return "";
			}
			return sections[section][key];
		}
		public bool GetBoolean(string section, string key)
		{
			var val = GetValue(section, key);
			return bool.TryParse(val, out bool ret) ? ret : default(bool);
		}
		public int GetInt(string section, string key)
		{
			var val = GetValue(section, key);
			return int.TryParse(val, out int ret) ? ret : default(int);
		}
		public float GetFloat(string section, string key)
		{
			var val = GetValue(section, key);
			return float.TryParse(val, out float ret) ? ret : default(float);
		}
		public double GetDouble(string section, string key)
		{
			var val = GetValue(section, key);
			return double.TryParse(val, out double ret) ? ret : default(double);
		}
	}
}
