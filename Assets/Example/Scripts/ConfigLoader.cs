using System;
using System.Linq;
using UniRx.Async;
using Flour.Layer;

class ConfigLoader
{
	public async UniTask<SubLayerSourceRepository[]> LoadLayerSourceRepositories(SubLayerType[] fixedLayers)
	{
		var subLayers = Enum.GetValues(typeof(SubLayerType)).Cast<SubLayerType>();
		var repositories = new SubLayerSourceRepository[2] { new SubLayerSourceRepository(fixedLayers.Length), new SubLayerSourceRepository(10) };

		foreach (var type in subLayers)
		{
			var index = fixedLayers.Contains(type) ? 0 : 1;
			repositories[index].AddSourcePath(type, type.ToJpnName());
		}
		for (int i = 0; i < fixedLayers.Length; i++)
		{
			await repositories[0].LoadAsync<AbstractSubLayer>(fixedLayers[i]);
		}

		return repositories;
	}
}
