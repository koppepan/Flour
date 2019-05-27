using System;
using System.Linq;
using UnityEngine;
using UniRx.Async;
using Flour;
using Flour.Layer;

class ConfigLoader : IDisposable
{
	static readonly string SubLayerConfigPath = "Config/SubLayerType";
	static readonly string SubLayerSection = "SubLayerType";

	TextAsset subLayerResourceAsset;

	public async UniTask<ConfigLoader> LoadAsync()
	{
		subLayerResourceAsset = (TextAsset)await Resources.LoadAsync<TextAsset>(SubLayerConfigPath);
		return this;
	}

	public void Dispose()
	{
		Resources.UnloadAsset(subLayerResourceAsset);
	}

	public async UniTask<SubLayerSourceRepository[]> LoadLayerSourceRepositories(SubLayerType[] fixedLayers)
	{
		var contents = new IniFile(subLayerResourceAsset.text.Split('\n', '\r')).GetContents(SubLayerSection);
		var subLayers = contents.ToDictionary(k => (SubLayerType)Enum.Parse(typeof(SubLayerType), k.Key), v => v.Value);

		var repositories = new SubLayerSourceRepository[2] { new SubLayerSourceRepository(fixedLayers.Length), new SubLayerSourceRepository(10) };

		foreach (var set in subLayers)
		{
			var index = fixedLayers.Contains(set.Key) ? 0 : 1;
			repositories[index].AddSourcePath(set.Key, set.Value);
		}
		for (int i = 0; i < fixedLayers.Length; i++)
		{
			await repositories[0].LoadAsync<AbstractSubLayer>(fixedLayers[i]);
		}

		return repositories;
	}
}
